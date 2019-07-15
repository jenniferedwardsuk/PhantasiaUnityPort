using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace phantasiaclasses
{

    public class fight //: MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        static fight Instance;
        private fight()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.Log("Warning: attempt to create duplicate instance of singleton " + this);
                //Destroy(this);
            }

            ioclass = io.GetInstance();
            fileclass = file.GetInstance();
            miscclass = misc.GetInstance();
            hackclass = hack.GetInstance();
            characterclass = character.GetInstance();
            tagsclass = tags.GetInstance();
            eventclass = eventsrc.GetInstance();
            statsclass = stats.GetInstance();
            commandsclass = commands.GetInstance();
            socketclass = socket.GetInstance();
            infoclass = info.GetInstance();
            treasureclass = treasure.GetInstance();
            restoreclass = restore.GetInstance();
            itcombatclass = itcombat.GetInstance();
            fightclass = fight.GetInstance();
            
        }
        public static fight GetInstance()
        {
            fight instance = null;
            if (Instance != null)
            {
                instance = Instance;
            }
            else
            {
                instance = new fight();
            }
            return instance;
        }

        phantasiaclasses.misc miscclass;
        phantasiaclasses.file fileclass;
        phantasiaclasses.io ioclass;
        phantasiaclasses.socket socketclass;
        phantasiaclasses.tags tagsclass;
        phantasiaclasses.info infoclass;
        phantasiaclasses.commands commandsclass;
        phantasiaclasses.stats statsclass;
        phantasiaclasses.itcombat itcombatclass;
        phantasiaclasses.fight fightclass;
        phantasiaclasses.treasure treasureclass;
        phantasiaclasses.restore restoreclass;
        phantasiaclasses.eventsrc eventclass;
        phantasiaclasses.hack hackclass;
        phantasiaclasses.character characterclass;


        /************************************************************************
        /
        / FUNCTION NAME: Do_monster(struct client_t *c, struct event_t the_event)
        /
        / FUNCTION: monster battle routine
        /
        / AUTHOR: E. A. Estes, 2/20/86
        /	  Brian Kelly, 5/20/99
        /
        / ARGUMENTS:
        /       int particular - particular monster to fight if >= 0
        /
        / RETURN VALUE: none
        /
        / MODULES CALLED: monsthits(), playerhits(), readmessage(), callmonster(),
        /       writerecord(), pickmonster(), displaystats(), cfunctions.pow(), cancelmonster(),
        /       awardtreasure(), more(), death(), wmove(), setjmp(), drandom(), printw(),
        /       longjmp(), wrefresh(), mvprintw(), wclrtobot()
        / / DESCRIPTION: /       Choose a monster and check against some special types.
        /       Arbitrate between monster and player.  Watch for either
        /       dying.
        /
        *************************************************************************/

        internal void Do_monster(client_t c, event_t the_event)
        {
            event_t event_ptr = new event_t();
            bool firsthit = c.player.blessing;  /* set if player gets the first hit */
            bool monsthit = true;
            int count = 0;
            string string_buffer = ""; //[phantdefs.SZ_LINE];
            string string_buffer2 = ""; //[phantdefs.SZ_LINE];
            short sTemp = -1;
            double dTemp = -1;

            /* set battle stats */
            c.battle.ring_in_use = false;
            c.battle.tried_luckout = false;
            c.battle.melee_damage = 0.0;
            c.battle.skirmish_damage = 0.0;


            /* create a player opponent */
            c.battle.opponent = new opponent_t(); // (struct opponent_t *) Do_malloc(phantdefs.SZ_OPPONENT);
            c.battle.opponent.sin = 5;

            if (the_event.arg3 >= 0 && the_event.arg3 < phantdefs.NUM_MONSTERS)
            {
                /* monster is specified */
                c.battle.opponent.type = (short)the_event.arg3;
            }

            else
            {

                if (c.player.special_type == phantdefs.SC_VALAR)
                {

                    /* even chance of any monster */
                    c.battle.opponent.type = (short)macros.ROLL(0.0, 100.0);
                }

                /* 10% of getting cerbed with big shield */
                else if ((c.player.shield / c.player.degenerated > 50000.0) &&
                         (c.player.circle >= 36) && macros.RND() < .1)
                {

                    c.battle.opponent.type = 96;
                }
                /* 10% of getting jabbed with big quick */
                else if ((c.player.quicksilver / c.player.degenerated > 500.0) &&
                         (c.player.circle >= 36) && macros.RND() < .1)
                {

                    c.battle.opponent.type = 94;

                }
                else if (c.player.circle >= 36)
                {

                    /* even chance of all non-water monsters */
                    c.battle.opponent.type = (short)macros.ROLL(14.0, 86.0);

                }
                else if ((c.player.circle == 27) || (c.player.circle == 28))
                {

                    /* the cracks of doom - no weak monsters except modnar */

                    c.battle.opponent.type = (short)(macros.ROLL(50.0, 50.0));

                    if (c.battle.opponent.type < 52)
                    {
                        c.battle.opponent.type = 15;
                    }

                }
                else if ((c.player.circle < 31) && (c.player.circle > 24))
                {

                    /* gorgoroth - no weak monsters except modnar, 
                           weighed towards middle  */
                    c.battle.opponent.type = (short)(macros.ROLL(50.0, 25.0) +

                    macros.ROLL(0.0, 26.0));

                    if (c.battle.opponent.type < 52)
                    {
                        c.battle.opponent.type = 15;
                    }

                }
                else if (c.player.circle > 19)
                {

                    /* the marshes - water monsters, idiots, and modnar */
                    c.battle.opponent.type = (short)macros.ROLL(0.0, 17.0);

                }
                else if (c.player.circle > 15)
                {

                    /* chance of all non-water monsters, weighted toward middle */
                    c.battle.opponent.type =
                            (short)(macros.ROLL(0.0, 50.0) + macros.ROLL(14.0, 37.0));

                }
                else if (c.player.circle > 9)
                {
                    /* TT 1-8 monsters, weighted toward middle */
                    c.battle.opponent.type =
                            (short)(macros.ROLL(0.0, (-8.0 + c.player.circle * 4)) +
                                   macros.ROLL(14.0, 26.0));
                    /*  (int) (macros.ROLL(0.0, (35.0 + c.player.circle)) + 
                               macros.ROLL(14.0, 26.0)); */

                }
                else if (c.player.circle > 7)
                {
                    /* Hard type 3-5 monsters */
                    c.battle.opponent.type =
                            (short)macros.ROLL(14.0, (18.0 + c.player.circle * 4));

                }
                else if (c.player.circle > 4)
                {
                    /* even chance of type 1-3 + easy type 4-5 monsters */
                    c.battle.opponent.type = (short)macros.ROLL(14.0, 46.0);

                }
                else if (c.player.circle == 4)
                {
                    /* even chance of all type 1-3 */
                    c.battle.opponent.type = (short)macros.ROLL(14.0, 38.0);

                }
                else
                {

                    /* even chance of some of the tamest non-water monsters */
                    c.battle.opponent.type =
                        (short)macros.ROLL(14.0, (17.0 + c.player.circle * 4));
                }
            }

            if (c.battle.opponent.type == 100)
            {
                c.battle.opponent.type = 15;
            }

            /* set the monster's size */
            if (the_event.arg2 > 0)
            {
                /* monster size is specified */
                c.battle.opponent.size = the_event.arg2;
            }
            else if ((c.player.circle == 27) || (c.player.circle == 28))
            {
                /* cracks and gorgoroth scale with player level */

                if (c.player.ring_type == phantdefs.R_DLREG)
                {
                    c.battle.opponent.size = CFUNCTIONS.MAX(c.player.circle,
                                               (CFUNCTIONS.floor((.2 + .3 * macros.RND()) * c.player.level)));
                }
                else if (c.player.ring_type != phantdefs.R_NONE)
                {
                    c.battle.opponent.size = CFUNCTIONS.MAX(c.player.circle,
                                               (CFUNCTIONS.floor((.15 + .35 * macros.RND()) * c.player.level)));
                }
                else
                {
                    c.battle.opponent.size = CFUNCTIONS.MAX(c.player.circle,
                                               (CFUNCTIONS.floor((.1 + .25 * macros.RND()) * c.player.level)));
                }
            }
            else if ((c.player.circle < 31) && (c.player.circle > 24))
            {

                if (c.player.ring_type == phantdefs.R_DLREG)
                {
                    c.battle.opponent.size = CFUNCTIONS.MAX(c.player.circle,
                                               ((.15 + .1 * macros.RND()) * c.player.level));
                }
                else if (c.player.ring_type != phantdefs.R_NONE)
                {
                    c.battle.opponent.size = CFUNCTIONS.MAX(c.player.circle,
                                               ((.1 + .15 * macros.RND()) * c.player.level));
                }
                else
                {
                    c.battle.opponent.size = CFUNCTIONS.MAX(c.player.circle,
                                               ((.05 + .1 * macros.RND()) * c.player.level));
                }
            }
            else
            {
                c.battle.opponent.size = c.player.circle;
            }

            miscclass.Do_lock_mutex(c.realm.monster_lock);

            /* fill structure */
            CFUNCTIONS.strcpy(ref c.battle.opponent.name,
                c.realm.monster[c.battle.opponent.type].name);

            CFUNCTIONS.strcpy(ref c.battle.opponent.realName,
                c.realm.monster[c.battle.opponent.type].name);

            c.battle.opponent.experience = c.battle.opponent.size *

                    c.realm.monster[c.battle.opponent.type].experience;

            c.battle.opponent.brains = c.battle.opponent.size *

                c.realm.monster[c.battle.opponent.type].brains;

            c.battle.opponent.strength =
                            c.battle.opponent.max_strength = (1.0 + c.battle.opponent.size / 2.0)

                                * c.realm.monster[c.battle.opponent.type].strength;

            /* Randomize energy slightly */
            c.battle.opponent.energy =
                            c.battle.opponent.max_energy = CFUNCTIONS.floor(c.battle.opponent.size *
                                c.realm.monster[c.battle.opponent.type].energy *
                                    (.9 + macros.RND() * .2));

            c.battle.opponent.speed =
            c.battle.opponent.max_speed =
                (float)c.realm.monster[c.battle.opponent.type].speed;

            c.battle.opponent.special_type =
                c.realm.monster[c.battle.opponent.type].special_type;

            c.battle.opponent.treasure_type =
                c.realm.monster[c.battle.opponent.type].treasure_type;

            c.battle.opponent.flock_percent =
                c.realm.monster[c.battle.opponent.type].flock_percent;

            miscclass.Do_unlock_mutex(c.realm.monster_lock);

            c.battle.opponent.shield = 0.0;

            /* handle some special monsters */
            if (c.battle.opponent.special_type == phantdefs.SM_MODNAR)
            {

                if (c.player.special_type < phantdefs.SC_COUNCIL)
                {

                    /* randomize some stats */
                    c.battle.opponent.strength *= macros.RND() + 0.5;
                    c.battle.opponent.brains *= macros.RND() + 0.5;
                    c.battle.opponent.speed *= (float)(macros.RND() + 0.5);
                    c.battle.opponent.energy *= macros.RND() + 0.5;
                    c.battle.opponent.experience *= macros.RND() + 0.5;

                    c.battle.opponent.treasure_type =
                    (short)macros.ROLL(0.0, (double)c.battle.opponent.treasure_type);
                }
                else
                {

                    /* make Modnar into Morgoth */
                    CFUNCTIONS.strcpy(ref c.battle.opponent.name, "Morgoth");
                    CFUNCTIONS.strcpy(ref c.battle.opponent.realName, "Morgoth");
                    c.battle.opponent.special_type = (short)phantdefs.SM_MORGOTH;

                    c.battle.opponent.energy = c.battle.opponent.max_energy =

                    CFUNCTIONS.floor((8 + (c.player.level / 250)) *
                                  (c.player.strength *
                                   (1 + CFUNCTIONS.sqrt(c.player.sword) * phantdefs.N_SWORDPOWER)) *
                                  (.75 + macros.RND() * .5));

                    c.battle.opponent.strength = c.battle.opponent.max_strength =

                    CFUNCTIONS.floor((.025 + macros.RND() *
                                  ((.02 + .05 * c.player.level / 10000))) *
                                 (c.player.max_energy + c.player.shield));

                    if (c.player.special_type == phantdefs.SC_EXVALAR)
                    {
                        c.battle.opponent.speed = 1;
                    }
                    else if (macros.RND() < .5)
                    {
                        c.battle.opponent.speed = 1 +
                                        (float)(macros.RND() * ((c.player.level - 2500) / 7500));
                    }
                    else
                    {
                        c.battle.opponent.speed = 1 -
                                        (float)(macros.RND() * ((c.player.level - 2500) / 7500));
                    }

                    c.battle.opponent.speed *= (float)(c.player.max_quickness +
                    CFUNCTIONS.sqrt(c.player.quicksilver) - c.battle.opponent.size * 0.0005f);


                    /* Morgie gets faster as you go on to counter stat balancing */
                    if ((c.player.level >= 3000) && (c.player.level < 4000))
                    {

                        c.battle.opponent.max_speed = c.battle.opponent.speed;

                    }
                    else if ((c.player.level >= 4000) && (c.player.level < 5000))
                    {

                        c.battle.opponent.max_speed = c.battle.opponent.speed * 1.05f;

                    }
                    else if ((c.player.level >= 5000) && (c.player.level < 6000))
                    {

                        c.battle.opponent.max_speed = c.battle.opponent.speed * 1.1f;

                    }
                    else if ((c.player.level >= 6000) && (c.player.level < 7000))
                    {

                        c.battle.opponent.max_speed = c.battle.opponent.speed * 1.15f;

                    }
                    else if ((c.player.level >= 7000) && (c.player.level < 8000))
                    {

                        c.battle.opponent.max_speed = c.battle.opponent.speed * 1.2f;

                    }
                    else if ((c.player.level >= 8000) && (c.player.level < 9000))
                    {

                        c.battle.opponent.max_speed = c.battle.opponent.speed * 1.25f;

                    }
                    else if ((c.player.level >= 9000) && (c.player.level < 10000))
                    {

                        c.battle.opponent.max_speed = c.battle.opponent.speed * 1.3f;

                    }
                    else if (c.player.level >= 10000)
                    {

                        c.battle.opponent.max_speed = c.battle.opponent.speed * 1.35f;
                    }

                    c.battle.opponent.brains = c.player.brains * 20;
                    c.battle.opponent.flock_percent = (short)0.0;

                    /* Morgoth drops gold to annoy players */
                    c.battle.opponent.treasure_type = 7;
                    c.battle.opponent.experience = CFUNCTIONS.floor(c.battle.opponent.energy
                    / 4);

                }
            }

            else if (c.battle.opponent.special_type == phantdefs.SM_MIMIC)
            {

                /* pick another name */
                while (!CFUNCTIONS.strcmp(c.battle.opponent.name, "A Mimic"))
                {

                    CFUNCTIONS.strcpy(ref c.battle.opponent.name,
                    c.realm.monster[(int)macros.ROLL(0.0, 100.0)].name);

                    if (!CFUNCTIONS.strcmp(c.battle.opponent.name, "A Succubus")
                        && (c.player.gender == (phantdefs.FEMALE == 1 ? true : false)))
                    {
                        CFUNCTIONS.strcpy(ref c.battle.opponent.name, "An Incubus");
                    }

                    firsthit = true;
                }
            }

            else if ((c.battle.opponent.special_type == phantdefs.SM_SUCCUBUS) &&
                     (c.player.gender == (phantdefs.FEMALE == 1 ? true : false)))
            {

                /* females should be tempted by incubi, not succubi */
                CFUNCTIONS.strcpy(ref c.battle.opponent.name, "An Incubus");
                CFUNCTIONS.strcpy(ref c.battle.opponent.realName, "An Incubus");

            }

            ioclass.Do_send_clear(c);

            /* cannot see monster if blind */
            if (c.player.blind)
            {
                CFUNCTIONS.strcpy(ref c.battle.opponent.name, "A monster");
                the_event.arg1 = (double)phantdefs.MONSTER_RANDOM;
            }

            if (c.battle.opponent.special_type == phantdefs.SM_UNICORN)
            {

                if (c.player.virgin)
                {
                    if (macros.RND() < c.player.sin - .1)
                    {

                        CFUNCTIONS.sprintf(ref string_buffer,
                                "%s glares at you and gallops away with your virgin!\n",
                                c.battle.opponent.name);


                        ioclass.Do_send_line(c, string_buffer);

                        statsclass.Do_virgin(c, 0, 0);

                        Do_cancel_monster(c.battle.opponent);
                    }
                    else
                    {


                        CFUNCTIONS.sprintf(ref string_buffer,
                        "You just subdued %s, thanks to the virgin.\n",
                        c.battle.opponent.name);


                        ioclass.Do_send_line(c, string_buffer);

                        statsclass.Do_virgin(c, 0, 0);
                        c.battle.opponent.energy = 0.0;
                    }
                }
                else if (!c.player.blind)
                {

                    string_buffer = CFUNCTIONS.sprintfSinglestring("You just saw %s running away!\n",
                    c.battle.opponent.name);


                    ioclass.Do_send_line(c, string_buffer);


                    Do_cancel_monster(c.battle.opponent);
                }
                else
                {
                    string_buffer = CFUNCTIONS.sprintfSinglestring("You just heard %s running away!\n",
                    c.battle.opponent.name);


                    ioclass.Do_send_line(c, string_buffer);


                    Do_cancel_monster(c.battle.opponent);
                }
            }
            else
            {

                if (c.battle.opponent.special_type == phantdefs.SM_MORGOTH)
                {

                    CFUNCTIONS.sprintf(ref string_buffer,
                    "You've encountered %s, Bane of the Council and Enemy of the Vala.\n",
                            c.battle.opponent.name);


                    ioclass.Do_send_line(c, string_buffer);

                }
            }

            /* overpower Dark Lord with blessing and charms or if superchar */
            if (c.battle.opponent.special_type == phantdefs.SM_DARKLORD)
            {
                if (c.player.blessing)
                {
                    if (c.player.charms >= CFUNCTIONS.floor(c.battle.opponent.size
                                                  * (.8 - .1 * c.player.type)) + 1)
                    {

                        string_buffer = CFUNCTIONS.sprintfSinglestring("You just overpowered %s!\n",
                      c.battle.opponent.name);

                        ioclass.Do_send_line(c, string_buffer);


                        /* have dark lord take a percentage of charms 
                           if player is carrying way too many charms */
                        c.player.charms -= (int)CFUNCTIONS.MAX(CFUNCTIONS.floor(c.battle.opponent.size
                                                * (.8 - .1 * c.player.type)),
                                                          CFUNCTIONS.floor((.35 - (.05 * c.player.type)) * c.player.charms));

                        if (c.player.charms < 0)
                        {
                            c.player.charms = 0;
                        }

                        c.battle.opponent.energy = 0.0;


                        string_buffer = CFUNCTIONS.sprintfSinglestring("%s has just defeated %s!\n",
                            c.modifiedName, c.battle.opponent.name);


                        ioclass.Do_broadcast(c, string_buffer);
                    }
                }
            }

            /* give this new monster the proper introduction */
            if (c.battle.opponent.energy > 0)
            {

                switch ((int)the_event.arg1)
                {

                    case phantdefs.MONSTER_RANDOM:
                    default:


                        string_buffer = CFUNCTIONS.sprintfSinglestring("You are attacked by %s.",
                            c.battle.opponent.name);
                        break;

                    case phantdefs.MONSTER_CALL:


                        string_buffer = CFUNCTIONS.sprintfSinglestring("You find and attack %s.",
                            c.battle.opponent.name);
                        break;

                    case phantdefs.MONSTER_FLOCKED:


                        string_buffer = CFUNCTIONS.sprintfSinglestring("%s's friend appears and attacks.",
                            c.battle.opponent.name);
                        break;

                    case phantdefs.MONSTER_SHRIEKER:


                        string_buffer = CFUNCTIONS.sprintfSinglestring("%s responds to the shrieker's clamor.",
                            c.battle.opponent.name);
                        break;

                    case phantdefs.MONSTER_JABBERWOCK:


                        string_buffer = CFUNCTIONS.sprintfSinglestring("The Jabberwock summons %s.",
                            c.battle.opponent.name);
                        break;

                    case phantdefs.MONSTER_TRANSFORM:


                        string_buffer = CFUNCTIONS.sprintfSinglestring("%s now attacks.",
                            c.battle.opponent.name);
                        break;

                    case phantdefs.MONSTER_SUMMONED:
                    case phantdefs.MONSTER_SPECIFY:


                        string_buffer = CFUNCTIONS.sprintfSinglestring("%s appears and attacks.",
                            c.battle.opponent.name);
                        break;

                    case phantdefs.MONSTER_PURGATORY:


                        string_buffer = CFUNCTIONS.sprintfSinglestring("%s appears and attacks.",
                            c.battle.opponent.name);

                        /* Purgatory characters always get first attack since they
                        had the turn when they disconnected */
                        firsthit = true;

                        /* Assume they already lucked out */
                        c.battle.tried_luckout = true;

                        /* prevent players from rerolling monster size */
                        if ((c.player.circle < 31) && (c.player.circle > 24))
                        {
                            ioclass.Do_send_line(c, "The monster appears to have grown stronger in your absence!\n");
                            c.battle.opponent.size = CFUNCTIONS.MAX(c.player.circle, .5 * c.player.level);
                        }
                        break;
                }

                /* print header, and arbitrate between player and monster */
                CFUNCTIONS.sprintf(ref string_buffer2, " -  (Size: %.0f)\n",
                c.battle.opponent.size);

                CFUNCTIONS.strcat(ref string_buffer, string_buffer2);
                ioclass.Do_send_line(c, string_buffer);
            }

            /* throw up spells for this monster if not Morgoth */
            if (c.battle.opponent.special_type != phantdefs.SM_MORGOTH)
            {
                Do_starting_spells(c);
            }

            /* allow experimentos to test the speed equation */
            if (c.battle.opponent.special_type == phantdefs.SM_MORON)
            {
                if (c.player.type == phantdefs.C_EXPER && c.player.level == 0)
                {
                    c.battle.opponent.speed =
                    c.battle.opponent.max_speed =
                    c.player.quickness;

                    ioclass.Do_send_line(c, "Recognizing a kindred spirit, A Moron now moves as quickly as you!\n");

                    ioclass.Do_send_line(c, "You feel fully refreshed and healed for battle!\n");

                    statsclass.Do_energy(c, c.player.max_energy + c.player.shield,
                              c.player.max_energy, c.player.shield,
                              c.battle.force_field, 0);
                    c.battle.rounds = 0;
                }
            }

            /* adjust equipment-stealing monsters to knock off unbalanced chars */
            if ((c.battle.opponent.special_type == phantdefs.SM_CERBERUS) &&
                (c.player.shield / c.player.degenerated > 100000.0) &&
                (c.player.degenerated < 50))
            {
                c.battle.opponent.speed = c.player.max_quickness * 4;
                c.battle.opponent.max_speed = c.battle.opponent.speed;
                ioclass.Do_send_line(c, "Cerberus's eyes flare brightly as he sees the immense amounts of metal you are carrying!\n");
            }

            /* adjust equipment-stealing monsters to knock off unbalanced chars */
            if ((c.battle.opponent.special_type == phantdefs.SM_JABBERWOCK) &&
                (c.player.quicksilver / c.player.degenerated > 500.0) &&
                (c.player.degenerated < 50))
            {
                c.battle.opponent.speed = c.player.max_quickness * 4;
                c.battle.opponent.max_speed = c.battle.opponent.speed;
                ioclass.Do_send_line(c, "A Jabberwock whiffles in delight as it sees your immense stash of quicksilver!\n");
            }

            if ((c.battle.opponent.special_type == phantdefs.SM_DARKLORD) && (c.player.blessing) && (c.battle.opponent.energy > 0) && (c.morgothCount == 0))
            {
                ioclass.Do_send_line(c, "Your blessing is consumed by the great evil of your opponent!\n");
                statsclass.Do_blessing(c, 0, 0);
            }

            if ((c.battle.opponent.special_type == phantdefs.SM_DARKLORD) &&
                (c.morgothCount > 0) && (c.battle.opponent.energy > 0))
            {

                ioclass.Do_send_line(c, "The Dark Lord keeps a safe distance away from you, smelling the scent of its masters blood upon your sword!\n");
            }


            if ((c.player.ring_type != phantdefs.R_NONE) &&
                (c.battle.opponent.special_type != phantdefs.SM_IT_COMBAT) &&
                ((macros.RND() * 1000 / c.battle.opponent.treasure_type) < c.player.sin))
            {

                ioclass.Do_send_line(c, "You feel compelled to put your ring on your finger!\n");
                c.battle.ring_in_use = true;


                string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, forced_ring.\n",
                    c.player.lcname,
                    c.realm.charstats[c.player.type].class_name);

                statsclass.Do_sin(c, 0.1);


                fileclass.Do_log(pathnames.COMBAT_LOG, string_buffer);
            }

            int monstTotalHitsThisRound = 0;
            while (c.battle.opponent.energy > 0.0)
            {

                /* allow paralyzed monster to wake up */
                c.battle.opponent.speed = (short)CFUNCTIONS.MIN(c.battle.opponent.speed + 1.0,
                c.battle.opponent.max_speed);

                /* check if blessing gives first hit -- monster has to win
                   initiative extra times depending on sin level -- 
                   a sinless player always wins first strike */

                if ((firsthit) && (c.player.sin > 0))
                {

                    monsthit = true;

                    for (count = 0; count < (6 - CFUNCTIONS.floor((c.player.sin + 1.0) / 1.25)); count++)
                    {
                        monsthit &= (macros.RND() * c.battle.opponent.speed >
                                     macros.RND() * c.player.quickness);
                    }

                    if ((monsthit == true) && (c.player.quickness > 0))
                    {
                        firsthit = false;
                    }
                }

                /* monster is faster */
                if ((macros.RND() * c.battle.opponent.speed) > (macros.RND() * c.player.quickness)
                    /* not darklord */
                    && c.battle.opponent.special_type != phantdefs.SM_DARKLORD
                        /* not shrieker */
                        && c.battle.opponent.special_type != phantdefs.SM_SHRIEKER
                        /* not mimic */
                        && c.battle.opponent.special_type != phantdefs.SM_MIMIC
                        /* not first attack with a blessing */
                        && !firsthit)
                {
                    //added for unity: additional contests to reduce hitspamming from closely-matched monsters
                    bool monsthit2 = true;
                    for (count = 0; count < monstTotalHitsThisRound; count++)
                    {
                        monsthit2 &= (macros.RND() * c.battle.opponent.speed) > (macros.RND() * c.player.quickness);
                    }
                    if (monsthit2 == true)
                    {
                        monstTotalHitsThisRound++;

                        /* monster gets a hit */
                        Do_monsthits(c);
                    }

                }
                /* player gets a hit */
                else
                {
                    monstTotalHitsThisRound = 0;
                    firsthit = false;

                    /* if the player has a good ring in use */
                    if (c.battle.ring_in_use)
                    {

                        /* age ring */
                        if (c.player.ring_type != phantdefs.R_DLREG)
                        {
                            --c.player.ring_duration;
                        }

                        /* regardless of ring type, heal the character */
                        statsclass.Do_energy(c, c.player.max_energy + c.player.shield,
                                c.player.max_energy, c.player.shield,
                                c.battle.force_field, 0);
                    }

                    Do_playerhits(c, sTemp, dTemp);
                }

                /* player died */
                if ((c.player.energy <= 0.0) || (c.player.strength <= 0.0))
                {

                    event_ptr = eventclass.Do_create_event();
                    event_ptr.type = (short)phantdefs.DEATH_EVENT;
                    event_ptr.arg1 = (double)(c.battle.ring_in_use ? 1 : 0);

                    /* check if player died from greed or fatigue */
                    if (c.player.quickness == 0)
                    {
                        if (c.battle.rounds >= c.player.max_quickness * phantdefs.N_FATIGUE)
                        {
                            event_ptr.arg3 = phantdefs.K_FATIGUE;
                        }
                        else
                        {
                            event_ptr.arg3 = phantdefs.K_GREED;
                        }
                    }
                    else
                    {
                        event_ptr.arg3 = phantdefs.K_MONSTER;
                    }

                    event_ptr.arg4 = "";// (void*) Do_malloc(cfunctions.strlen(c.battle.opponent.realName) + 1);
                    
                    string arg4 = (string)event_ptr.arg4;
                    CFUNCTIONS.strcpy(ref arg4, c.battle.opponent.realName);
                    event_ptr.arg4 = arg4;

                    /* fight ends even if the player is saved from death */
                    Do_cancel_monster(c.battle.opponent);

                    /* log the loss */
                    string_buffer = CFUNCTIONS.sprintfSinglestring("%s killed_by size %.0lf %s\n",
                                            c.player.lcname, c.battle.opponent.size,
                                            c.battle.opponent.realName);


                    fileclass.Do_log(pathnames.BATTLE_LOG, string_buffer);


                    eventclass.Do_handle_event(c, event_ptr);

                }

                /* give the character fatigue */
                ++c.battle.rounds;

                statsclass.Do_speed(c, c.player.max_quickness, c.player.quicksilver,
                    c.battle.speedSpell, 0);
            }

            /* give player credit for killing monster */
            if (c.battle.opponent.experience != 0)
            {

                statsclass.Do_experience(c, c.battle.opponent.experience, 0);

                /* log the victory */
                string_buffer = CFUNCTIONS.sprintfSinglestring("%s killed size %.0lf %s\n",
                c.player.lcname, c.battle.opponent.size,
                c.battle.opponent.realName);


                fileclass.Do_log(pathnames.BATTLE_LOG, string_buffer);
            }
            else if (c.player.energy > 0.0)
            {

                /* log no_victor */
                string_buffer = CFUNCTIONS.sprintfSinglestring("%s survived_with size %.0lf %s\n",
                c.player.lcname, c.battle.opponent.size,
                c.battle.opponent.realName);


                fileclass.Do_log(pathnames.BATTLE_LOG, string_buffer);
            }

            ioclass.Do_more(c);
            ioclass.Do_send_clear(c);

            /* monster flocks */
            if (macros.RND() < c.battle.opponent.flock_percent / 100.0)
            {
                event_ptr = eventclass.Do_create_event();
                event_ptr.type = (short)phantdefs.MONSTER_EVENT;
                event_ptr.arg1 = phantdefs.MONSTER_FLOCKED;
                event_ptr.arg2 = the_event.arg2;
                event_ptr.arg3 = c.battle.opponent.type;
                eventclass.Do_file_event_first(c, event_ptr);
                /* last fight in sequence, remove timeout penalty */
            }
            else
            {
                c.battle.timeouts = 0;
            }

            /* monster has treasure */
            if (c.battle.opponent.treasure_type > 0.0 &&
                (macros.RND() > 0.2 + CFUNCTIONS.pow(0.4, (double)(c.battle.opponent.size / 3.0)) ||
                 c.battle.opponent.special_type == phantdefs.SM_UNICORN))
            {

                event_ptr = eventclass.Do_create_event();
                event_ptr.type = (short)phantdefs.TREASURE_EVENT;
                event_ptr.arg1 = c.battle.opponent.size;

                /* unicorns will always drop trove or pick scrolls */
                if (c.battle.opponent.special_type == phantdefs.SM_UNICORN)
                {
                    if (c.player.level <= 100 - c.player.gems)
                    {
                        event_ptr.arg2 = 1;
                    }
                    else
                    {
                        event_ptr.arg2 = 5;
                    }
                }

                event_ptr.arg3 = c.battle.opponent.treasure_type;
                eventclass.Do_file_event(c, event_ptr);
            }

            if (c.player.ring_duration <= 0)
            {
                if (c.player.ring_type == phantdefs.R_NAZREG)
                {


                    statsclass.Do_ring(c, phantdefs.R_NONE, 0);


                    ioclass.Do_send_clear(c);

                    ioclass.Do_send_line(c, "Your ring vanishes!\n");

                    ioclass.Do_more(c);
                }
                else if (c.player.ring_type == phantdefs.R_BAD)
                {


                    statsclass.Do_ring(c, phantdefs.R_SPOILED, 0);
                }
                else if (c.player.ring_type == phantdefs.R_SPOILED)
                {

                    event_ptr = eventclass.Do_create_event();
                    event_ptr.type = (short)phantdefs.DEATH_EVENT;
                    event_ptr.arg3 = phantdefs.K_RING;

                    eventclass.Do_file_event(c, event_ptr);
                }
            }

            /* remove player bonuses */
            statsclass.Do_energy(c, c.player.energy, c.player.max_energy, c.player.shield,
            0, 0);

            statsclass.Do_strength(c, c.player.max_strength, c.player.sword, 0, 0);
            statsclass.Do_speed(c, c.player.max_quickness, c.player.quicksilver, 0, 0);

            c.battle.force_field = 0;
            c.battle.strengthSpell = 0;
            c.battle.speedSpell = 0;

            /* destroy the opponent structure */
            c.battle.opponent = null; //free((void*) c.battle.opponent);

        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_playerhits(struct client_t *c)
        /
        / FUNCTION: prompt player for action in monster battle, and process
            c.battle.ring_in_use = false;
        /
        / AUTHOR: E. A. Estes, 12/4/85
        /	  Brian Kelly, 5/23/99
        /
        / ARGUMENTS: none
        /
        / RETURN VALUE: none
        /
        / MODULES CALLED: hitmonster(), throwspell(), inputoption(), cancelmonster(),
        /       cfunctions.floor(), wmove(), drandom(), altercoordinates(), waddstr(), mvprintw(),
        /       wclrtoeol(), wclrtobot()
        /
        / DESCRIPTION:
        /       Process all monster battle options.
        /
        *************************************************************************/

        internal void Do_playerhits(client_t c, short theAttack, double theArg)
        {
            button_t theButtons = new button_t();
            event_t event_ptr = new event_t();
            double might = -1;
            double inflict = -1;
            double dtemp = -1;        /* damage inflicted */
            string string_buffer = ""; //[phantdefs.SZ_LINE];
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            long answer = -1;
            int rc = -1;
            int temp = -1;
            int wizEvade = 1;

            /* see if the game is shutting down */
            miscclass.Do_shutdown_check(c);

            CFUNCTIONS.strcpy(ref theButtons.button[0], "Melee\n");
            CFUNCTIONS.strcpy(ref theButtons.button[1], "Skirmish\n");
            ioclass.Do_clear_buttons(theButtons, 2);
            if (c.player.sword != 0)
            {
                CFUNCTIONS.strcpy(ref theButtons.button[2], "Nick\n");
            }
            CFUNCTIONS.strcpy(ref theButtons.button[3], "Spell\n");
            CFUNCTIONS.strcpy(ref theButtons.button[4], "Rest\n");
            ioclass.Do_clear_buttons(theButtons, 5);
            CFUNCTIONS.strcpy(ref theButtons.button[6], "Evade\n");

            /* haven't tried to luckout yet */
            if (!c.battle.tried_luckout)
                /* cannot luckout against Morgoth */
                if (c.battle.opponent.special_type == phantdefs.SM_MORGOTH)

                    CFUNCTIONS.strcpy(ref theButtons.button[5], "Ally\n");
                else

                    CFUNCTIONS.strcpy(ref theButtons.button[5], "Luckout\n");

            /* player has a ring */
            if (c.player.ring_type != phantdefs.R_NONE)

                CFUNCTIONS.strcpy(ref theButtons.button[7], "Use Ring\n");

            rc = ioclass.Do_buttons(c, ref answer, theButtons);

            /* if player has dropped */
            if (rc == phantdefs.S_ERROR)
            {

                /* evade if in inter-player combat */
                if (c.battle.opponent.special_type == phantdefs.SM_IT_COMBAT)
                {
                    answer = 5;
                }
                else
                {

                    /* throw the character into purgatory */
                    c.player.purgatoryFlag = true;
                    c.player.monsterNumber = c.battle.opponent.type;

                    /* cancel the monster and save the game */
                    Do_cancel_monster(c.battle.opponent);
                    c.run_level = (short)phantdefs.SAVE_AND_EXIT;
                    return;
                }
            }

            else if (rc == phantdefs.S_TIMEOUT || rc == phantdefs.S_CANCEL)
            {

                statsclass.Do_age(c);

                /* these monsters would never attack on timeouts */
                if (macros.RND() < .25 &&
                    c.battle.opponent.special_type != phantdefs.SM_DARKLORD &&
                        c.battle.opponent.special_type != phantdefs.SM_SHRIEKER)
                {


                    Do_monsthits(c);
                }

                theAttack = (short)phantdefs.IT_PASS;


                string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, timeout.\n",
                    c.player.lcname,
                    c.realm.charstats[c.player.type].class_name);

                /* give the character fatigue from timeouts */
                ++c.battle.timeouts;
                if (c.battle.timeouts < phantdefs.N_FATIGUE * 4)
                {
                    c.battle.timeouts *= 2;
                }
                else if ((c.battle.opponent.special_type == phantdefs.SM_DARKLORD) ||
                         (c.battle.opponent.special_type == phantdefs.SM_SHRIEKER))
                {
                    ioclass.Do_send_line(c, "It wandered off!\n");

                    Do_cancel_monster(c.battle.opponent);

                    theAttack = (short)phantdefs.IT_EVADED;

                    string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, evade.\n", c.player.lcname,
                            c.realm.charstats[c.player.type].class_name);


                    fileclass.Do_log(pathnames.COMBAT_LOG, string_buffer);
                }

                c.battle.rounds += c.battle.timeouts;


                fileclass.Do_log(pathnames.COMBAT_LOG, string_buffer);
                return;
            }

            switch (answer)
            {

                case 0:               /* melee */
                                      /* melee affects monster's energy and strength */
                    might = c.player.strength * (1 + CFUNCTIONS.sqrt(c.player.sword) * phantdefs.N_SWORDPOWER)
                                + c.battle.strengthSpell;

                    /* use of ring doubles a player's might */
                    if (c.battle.ring_in_use)
                    {
                        might *= 2;
                    }

                    inflict = CFUNCTIONS.floor((.5 + 1.3 * macros.RND()) * might);

                    c.battle.melee_damage += inflict;

                    c.battle.opponent.strength = c.battle.opponent.max_strength
                            - (c.battle.melee_damage / c.battle.opponent.max_energy)
                            * (c.battle.opponent.max_strength / 3.0);

                    Do_hitmonster(c, inflict);

                    theAttack = (short)phantdefs.IT_MELEE;

                    theArg = inflict;


                    string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, melee %.0lf.\n", c.player.lcname,
                                c.realm.charstats[c.player.type].class_name, inflict);


                    fileclass.Do_log(pathnames.COMBAT_LOG, string_buffer);

                    break;

                case 1:               /* skirmish */
                    might = c.player.strength * (1 + CFUNCTIONS.sqrt(c.player.sword) * phantdefs.N_SWORDPOWER)
                                + c.battle.strengthSpell;

                    /* use of ring doubles a player's might */
                    if (c.battle.ring_in_use)
                    {
                        might *= 2;
                    }

                    /* skirmish affects monster's energy and speed */
                    inflict = CFUNCTIONS.floor((.33 + 1.1 * macros.RND()) * might);

                    c.battle.skirmish_damage += inflict;

                    c.battle.opponent.speed = (float)(c.battle.opponent.max_speed
                            - (c.battle.skirmish_damage / c.battle.opponent.max_energy)
                            * (c.battle.opponent.max_speed / 3.0));

                    Do_hitmonster(c, inflict);

                    theAttack = (short)phantdefs.IT_SKIRMISH;

                    theArg = inflict;

                    string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, skirmish %.0lf.\n", c.player.lcname,
                            c.realm.charstats[c.player.type].class_name, inflict);


                    fileclass.Do_log(pathnames.COMBAT_LOG, string_buffer);

                    break;

                case 6:               /* evade */

                    /* in itcombat, give the player a bonus to evade */
                    if (c.battle.opponent.special_type == phantdefs.SM_IT_COMBAT)
                    {

                        dtemp = 6;

                        /* huge bonus to wizards and apprentices */
                        if (c.wizard > 0)
                        {
                            wizEvade = 0;
                        }

                    }
                    else
                    {
                        dtemp = 1;
                    }

                    /* use brains and speed to try to evade */
                    if ((c.battle.opponent.special_type != phantdefs.SM_MIMIC || macros.RND() < .05) &&
                    /* cannot run from mimic very effectively */
                    ((c.battle.opponent.special_type == phantdefs.SM_DARKLORD ||
                      c.battle.opponent.special_type == phantdefs.SM_SHRIEKER) ||
                            /* can always run from D. L. and shrieker */
                            macros.RND() * (c.player.quickness * 1.1 + 1) * (c.player.brains + 1) *

                    dtemp > macros.RND() * (c.battle.opponent.speed * 1.1 + 1) *
                    (c.battle.opponent.brains + 1) * wizEvade))
                    {

                        if (c.battle.opponent.special_type == phantdefs.SM_MORGOTH)
                        {
                            ioclass.Do_send_line(c, "Morgoth cackles evilly as you run away.  You feel as if you have done something terribly wrong!\n");
                            statsclass.Do_sin(c, 3.0);
                        }
                        else if ((c.battle.opponent.type == 71) &&
                                 (!c.player.blind))
                        {
                            ioclass.Do_send_line(c, "You shun the frumious Bandersnatch!\n");
                        }
                        else if (wizEvade == 0)
                        {
                            ioclass.Do_send_line(c, "You magically whisk yourself away!\n");
                        }
                        else
                        {
                            ioclass.Do_send_line(c, "You got away!\n");
                        }

                        Do_cancel_monster(c.battle.opponent);

                        event_ptr = eventclass.Do_create_event();
                        event_ptr.type = (short)phantdefs.MOVE_EVENT;
                        event_ptr.arg3 = phantdefs.A_NEAR;


                        eventclass.Do_file_event(c, event_ptr);

                        if (wizEvade == 0)
                        {

                            theAttack = (short)phantdefs.IT_WIZEVADE;
                            string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, wiz_evade.\n", c.player.lcname,
                                c.realm.charstats[c.player.type].class_name);
                        }
                        else
                        {

                            theAttack = (short)phantdefs.IT_EVADED;
                            string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, evade.\n", c.player.lcname,
                                c.realm.charstats[c.player.type].class_name);
                        }


                        fileclass.Do_log(pathnames.COMBAT_LOG, string_buffer);
                    }
                    else
                    {

                        string_buffer = CFUNCTIONS.sprintfSinglestring("%s is still after you!\n",
                        c.battle.opponent.name);


                        ioclass.Do_send_line(c, string_buffer);

                        theAttack = (short)phantdefs.IT_NO_EVADE;

                        string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, no_evade.\n", c.player.lcname,
                                c.realm.charstats[c.player.type].class_name);


                        fileclass.Do_log(pathnames.COMBAT_LOG, string_buffer);
                    }
                    break;

                case 3:               /* magic spell */
                    Do_throwspell(c, theAttack, theArg);
                    break;

                case 2:               /* nick */

                    /* Morgoth can't be nicked */
                    if (c.battle.opponent.special_type == phantdefs.SM_MORGOTH)
                    {
                        string_buffer = CFUNCTIONS.sprintfSinglestring("You attempt to nick %s, but your sword bounces off him!\n",
                        c.battle.opponent.name, inflict);


                        ioclass.Do_send_line(c, string_buffer);
                        inflict = 0;
                    }
                    else
                    {
                        /* hit 1 plus sword; give some experience */
                        theArg = CFUNCTIONS.floor(c.battle.opponent.experience / 10.0);
                        c.battle.opponent.experience -= theArg;

                        statsclass.Do_experience(c, theArg, 0);

                        inflict = 1.0 + c.player.sword;

                        /* monster gets meaner */
                        c.battle.opponent.max_speed += 2.0f;
                        c.battle.opponent.speed = (c.battle.opponent.speed < 0.0f) ?
                                0.0f : c.battle.opponent.speed + 2.0f;

                        /* Dark Lord; doesn't like to be nicked */
                        if (c.battle.opponent.special_type == phantdefs.SM_DARKLORD)
                        {

                            string_buffer = CFUNCTIONS.sprintfSinglestring("You nicked %s for %.0f damage!  That wasn't a very good idea!\n",
                                    c.battle.opponent.name, inflict);

                            ioclass.Do_send_line(c, string_buffer);

                            statsclass.Do_speed(c, c.player.max_quickness / 2.0, c.player.quicksilver,
                                    c.battle.speedSpell, 0);

                            Do_cancel_monster(c.battle.opponent);

                            event_ptr = eventclass.Do_create_event();
                            event_ptr.type = (short)phantdefs.MOVE_EVENT;
                            event_ptr.arg3 = phantdefs.A_FAR;
                            eventclass.Do_file_event(c, event_ptr);
                        }
                        else
                        {
                            Do_hitmonster(c, inflict);
                        }
                    }


                    theAttack = (short)phantdefs.IT_NICKED;

                    string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, nick %.0lf.\n", c.player.lcname,
                            c.realm.charstats[c.player.type].class_name, inflict);


                    fileclass.Do_log(pathnames.COMBAT_LOG, string_buffer);

                    break;

                case 5:       /* luckout */

                    if (!c.battle.tried_luckout)
                    {

                        c.battle.tried_luckout = true;

                        /* Morgoth; ally */
                        if (c.battle.opponent.special_type == phantdefs.SM_MORGOTH)
                        {

                            if (macros.RND() < c.player.sin / 25.0)
                            {

                                string_buffer = CFUNCTIONS.sprintfSinglestring("%s accepted!\n",
                            c.battle.opponent.name);


                                ioclass.Do_send_line(c, string_buffer);
                                Do_cancel_monster(c.battle.opponent);

                                statsclass.Do_sin(c, 7.5);


                                string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, ally.\n",
                            c.player.lcname,
                            c.realm.charstats[c.player.type].class_name);


                                fileclass.Do_log(pathnames.COMBAT_LOG, string_buffer);
                            }
                            else
                                ioclass.Do_send_line(c, "Nope, he's not interested.\n");

                            ioclass.Do_more(c);


                            string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, no_ally.\n",
                        c.player.lcname,
                        c.realm.charstats[c.player.type].class_name);


                            fileclass.Do_log(pathnames.COMBAT_LOG, string_buffer);
                        }

                        /* routines for inter-player combat */
                        else if (c.battle.opponent.special_type == phantdefs.SM_IT_COMBAT)
                        {

                            /* use brains for success with a major handicap */
                            if (macros.RND() * c.player.brains < macros.RND() *
                                    c.battle.opponent.brains * 10)
                            {

                                string_buffer = CFUNCTIONS.sprintfSinglestring("You blew it, %s.\n",
                            c.modifiedName);


                                ioclass.Do_send_line(c, string_buffer);

                                theAttack = (short)phantdefs.IT_NO_LUCKOUT;


                                string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, no_luckout.\n",
                                c.player.lcname,
                                        c.realm.charstats[c.player.type].class_name);


                                fileclass.Do_log(pathnames.COMBAT_LOG, string_buffer);
                            }
                            else
                            {
                                ioclass.Do_send_line(c, "You made it!\n");
                                c.battle.opponent.energy = 0;

                                theAttack = (short)phantdefs.IT_LUCKOUT;


                                string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, luckout.\n",
                                    c.player.lcname,
                                    c.realm.charstats[c.player.type].class_name);


                                fileclass.Do_log(pathnames.COMBAT_LOG, string_buffer);
                            }
                        }
                        else
                        {
                            /* normal monster; use brains for success */
                            if (c.battle.opponent.special_type == phantdefs.SM_DARKLORD ||

                        macros.RND() * c.player.brains < macros.RND() *
                                    c.battle.opponent.brains)
                            {

                                string_buffer = CFUNCTIONS.sprintfSinglestring("You blew it, %s.\n",
                            c.modifiedName);


                                ioclass.Do_send_line(c, string_buffer);

                                theAttack = (short)phantdefs.IT_NO_LUCKOUT;


                                string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, no_luckout.\n",
                                c.player.lcname,
                                        c.realm.charstats[c.player.type].class_name);


                                fileclass.Do_log(pathnames.COMBAT_LOG, string_buffer);
                            }
                            else
                            {
                                ioclass.Do_send_line(c, "You made it!\n");
                                c.battle.opponent.energy = 0;

                                theAttack = (short)phantdefs.IT_LUCKOUT;


                                string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, luckout.\n",
                                    c.player.lcname,
                                    c.realm.charstats[c.player.type].class_name);


                                fileclass.Do_log(pathnames.COMBAT_LOG, string_buffer);
                            }
                        }
                    }
                    break;

                case 7:               /* use ring */

                    if (c.player.ring_type != phantdefs.R_NONE)
                    {

                        ioclass.Do_send_line(c, "Now using ring.\n");
                        c.battle.ring_in_use = true;

                        theAttack = (short)phantdefs.IT_RING;

                        statsclass.Do_sin(c, 0.1);


                        string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, ring.\n",
                            c.player.lcname,
                            c.realm.charstats[c.player.type].class_name);


                        fileclass.Do_log(pathnames.COMBAT_LOG, string_buffer);
                    }
                    else
                    {

                        theAttack = (short)phantdefs.IT_NO_RING;


                        string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, no_ring.\n",
                            c.player.lcname,
                            c.realm.charstats[c.player.type].class_name);


                        fileclass.Do_log(pathnames.COMBAT_LOG, string_buffer);
                    }
                    break;

                case 4:        /* rest */


                    theAttack = (short)phantdefs.IT_PASS;


                    string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, pass.\n",
                        c.player.lcname,
                        c.realm.charstats[c.player.type].class_name);


                    fileclass.Do_log(pathnames.COMBAT_LOG, string_buffer);

                    if (c.battle.opponent.special_type == phantdefs.SM_IT_COMBAT)
                    {
                        string_buffer = CFUNCTIONS.sprintfSinglestring("You attempt to rest, but your opponent prevents you!\n");

                        ioclass.Do_send_line(c, string_buffer);


                        statsclass.Do_age(c);

                    }
                    else
                    {

                        commandsclass.Do_rest(c);

                        statsclass.Do_age(c);

                        /* and just to keep life interesting */
                        Do_monsthits(c);

                        /* extra hit for morgoth */
                        if (c.battle.opponent.special_type == phantdefs.SM_MORGOTH)
                        {
                            c.battle.opponent.strength *= 1.1;

                            Do_monsthits(c);
                            string_buffer = CFUNCTIONS.sprintfSinglestring("You rest to catch your breath, but Morgoth seems to grow stronger!\n");

                            ioclass.Do_send_line(c, string_buffer);
                        }
                    }

                    break;

                default:

                    error_msg = CFUNCTIONS.sprintfSinglestring("[%s] Returned non-option in Do_playerhits.\n",
                            c.connection_id);

                    fileclass.Do_log_error(error_msg);

                    hackclass.Do_caught_hack(c, phantdefs.H_SYSTEM);
                    break;

            }
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_monsthits(struct client_t *c)
        /
        / FUNCTION: process a monster hitting the player
        /
        / AUTHOR: E. A. Estes, 12/4/85
        /	  Brian Kelly, 5/23/99
        /
        / ARGUMENTS: none
        /
        / RETURN VALUE: none
        /
        / MODULES CALLED: cancelmonster(), scramblestats(), more(), cfunctions.floor(), wmove(),
        /       drandom(), altercoordinates(), longjmp(), waddstr(), mvprintw(),
        /       getanswer()
        /
        / DESCRIPTION:
        /       Handle all special monsters here.  If the monster is not a special
        /       one, simply roll a hit against the player.
        /
        *************************************************************************/

        static string[] moronmesg =
            /* add more messages at front, if desired */
            /* last two messages for females only */
            {
                    "A Moron queries, 'what do u need to do to b a apprentice? I play for two years'\n",
                    "A Moron remembers, 'in the good old days I ruled the realm, I had  a MANs sword'\n",
                    "A Moron threatens, 'dont mess with me man, I a l33t haxxor you n00b!'\n",
                    "A Moron complains, 'aargh the lag! can u reset the server?'\n",
                    "A Moron complains, 'this game needs cheats!'\n",
                    "A Moron begs, 'steward can i please have 5k?  ill pay you back.'\n",
                    "A Moron complains, 'this game is too hard!'\n",
                    "A Moron complains, 'this game is too easy!'\n",
                    "A Moron complains, 'this game sucks!'\n",
                    "A Moron snarls, 'i hate the changes.  why cant they bring back the old version?'\n",
                    "A Moron grumbles, 'wizards never do anything.  why dont they add some pics?'\n",
                    "A Moron queries, 'where do i buy stuff?'\n",
                    "A Moron whimpers, 'how do I get rid of plague?'\n",
                    "A Moron boasts, 'i have a level 8k char, just you wait!'\n",
                    "A Moron wonders, 'what do i do with a virgin?\n",
                    "A Moron squeals, 'ooh a smurf how cute!\n",
                    "A Moron howls, 'but i don't want to read the rules!'\n",
                    "A Moron asks, 'how come morons never run out?\n",
                    "A Moron snivels, 'why is everything cursed?  this curse rate is too high!'\n",
                    "A Moron whines, 'how come a Troll hit me 5 times in a row?  it must be a bug!'\n",
                    "A Moron yells, 'HEY ALL COME CHEK OUT MY KNEW GAME!'\n",
                    "A Moron slobbers, 'please make me an apprentice please please please'\n",
                    "A Moron grouches, 'all the apprentices are power-hungry maniacs'\n",
                    "A Moron asserts, 'I'm not a liar, honest!'\n",
                    "A Moron sings, 'i love you!  you love me!'\n",
                    "A Moron exclaims, 'But I didn't MEAN to kill you!'\n",
                    "A Moron curses, 'smurfing smurf smurf!  why can't i swear?'\n",
                    "A Moron demands, 'i want a bank to store gold so i can get a bigger shield!'\n",
                    "A Moron bawls, 'Waa!  My mommy died!  Hey, will u make me apprentice?'\n",
                    "A Moron warns, 'my dad is a wizard, don't mess with me!'\n",
                    "A Moron leers, 'hey baby what's your sign?'\n",
                    "A Moron drools, 'If I had a pick scroll, I'd pick you!  What's your number?'\n"
                    };

        void Do_monsthits(client_t c)
        {
            event_t event_ptr = new event_t();
            double inflict = -1;                /* damage inflicted */
            string ch = "";
            string string_buffer = ""; //[phantdefs.SZ_LINE];
            long theAnswer = -1;
            float ftemp = -1;
            double dtemp, dtemp2 = -1;




            if (c.player.quickness <= 0)
            {
                /* kill a player at speed 0 with no network - saves time */
                /* also kill a player at speed 0 once monster inflicts 3 rounds 
                       of fatigue so that people won't have to sit forever */
                if (!c.socket_up || c.battle.rounds > phantdefs.N_FATIGUE * 3)
                {
                    inflict = c.player.energy + c.battle.force_field + 1.0;
                    c.battle.opponent.special_type = phantdefs.SM_NONE;
                }
            }

            /* may be a special monster */
            switch (c.battle.opponent.special_type)
            {

                case phantdefs.SM_DARKLORD:

                    /* hits just enough to kill player */
                    inflict = c.player.energy + c.battle.force_field + 1.0;

                    break;

                case phantdefs.SM_SHRIEKER:
                    event_ptr = eventclass.Do_create_event();
                    event_ptr.type = (short)phantdefs.MONSTER_EVENT;
                    event_ptr.arg1 = phantdefs.MONSTER_SHRIEKER;

                    if (c.player.shield / c.player.degenerated > 100000.0)
                    {
                        event_ptr.arg3 = 96;
                    }
                    else if (c.player.quicksilver / c.player.degenerated > 500.0)
                    {
                        event_ptr.arg3 = 94;
                    }
                    else
                    {
                        event_ptr.arg3 = (long)macros.ROLL(70.0, 30.0);
                    }
                    eventclass.Do_file_event_first(c, event_ptr);

                    /* call a big monster */
                    ioclass.Do_send_line(c,
                        "Shrieeeek!!  You scared it, and it called one of its friends.\n");


                    Do_cancel_monster(c.battle.opponent);
                    return;

                case phantdefs.SM_BALROG:

                    /* if there is no experience to take, do damage */
                    if (macros.RND() > .33 && c.player.experience > 0 &&
                        c.player.special_type < phantdefs.SC_KING)
                    {

                        /* take experience away */
                        inflict = (.001 + macros.RND() * .003) * c.player.experience;
                        inflict = CFUNCTIONS.MIN(c.player.experience, inflict);

                        /* add to strength */
                        c.battle.opponent.strength += CFUNCTIONS.MIN(.05 * c.player.shield, CFUNCTIONS.floor(CFUNCTIONS.sqrt(c.player.experience / 1800.0)));

                        string_buffer = CFUNCTIONS.sprintfSinglestring("%s lashes his whip and absorbs %.0f experience points from you!\n",
                            c.battle.opponent.name, inflict);


                        ioclass.Do_send_line(c, string_buffer);

                        statsclass.Do_experience(c, -inflict, 0);
                        return;
                    }
                    break;

                case phantdefs.SM_FAERIES:

                    /* holy water kills when monster tries to hit */
                    if (c.player.holywater >= CFUNCTIONS.floor(CFUNCTIONS.sqrt(c.battle.opponent.size)))
                    {
                        ioclass.Do_send_line(c, "Your holy water killed it!\n");
                        c.player.holywater -= CFUNCTIONS.floor(CFUNCTIONS.sqrt(c.battle.opponent.size));
                        c.battle.opponent.energy = 0.0;
                        return;
                    }
                    break;

                case phantdefs.SM_TITAN:

                    c.player.shield = CFUNCTIONS.ceil(c.player.shield * .99);
                    inflict = CFUNCTIONS.floor(1.0 + macros.RND() * c.battle.opponent.strength);
                    inflict = CFUNCTIONS.MIN(inflict, c.player.energy);

                    if (c.battle.force_field > 0.0)
                    {

                        /* inflict damage through force field */

                        CFUNCTIONS.sprintf(ref string_buffer,
                            "%s punches through your pitiful force field and hit you for %.0f damage!\n",
                            c.battle.opponent.name, inflict);


                        ioclass.Do_send_line(c, string_buffer);

                        c.battle.force_field = 0.0;


                        statsclass.Do_energy(c, c.player.energy - inflict,
                        c.player.max_energy, c.player.shield,
                        c.battle.force_field, 0);

                        return;
                    }
                    else
                    {

                        CFUNCTIONS.sprintf(ref string_buffer,
                            "%s hit you for %.0f damage and damages your shield!\n",
                            c.battle.opponent.name, inflict);


                        ioclass.Do_send_line(c, string_buffer);
                    }


                    break;

                case phantdefs.SM_NONE:
                    /* normal hit */
                    break;

                default:

                    /* special things */
                    if ((macros.RND() < 0.2) ||
                        ((c.battle.opponent.special_type == phantdefs.SM_CERBERUS) &&
                         (c.player.shield / c.player.degenerated > 50000.0)) ||
                        ((c.battle.opponent.special_type == phantdefs.SM_JABBERWOCK) &&
                         (c.player.quicksilver / c.player.degenerated > 500.0))
                       )
                    {

                        /* check for magic resistance */
                        if ((c.battle.opponent.special_type != phantdefs.SM_MODNAR) &&
                            (c.battle.opponent.special_type != phantdefs.SM_MORGOTH) &&
                            (c.battle.opponent.special_type != phantdefs.SM_MORON) &&
                            (c.battle.opponent.special_type != phantdefs.SM_SMURF) &&
                            (c.battle.opponent.special_type != phantdefs.SM_IDIOT) &&
                            (c.battle.opponent.special_type != phantdefs.SM_MIMIC) &&
                            (c.battle.opponent.special_type != phantdefs.SM_SMEAGOL) &&
                            (c.battle.opponent.special_type != phantdefs.SM_TROLL) &&
                            ((c.player.type == phantdefs.C_HALFLING) && (macros.RND() < 0.25)))
                        {

                            string_buffer = CFUNCTIONS.sprintfSinglestring("%s tries to do something special, but you resist the attack!\n", c.battle.opponent.name);


                            ioclass.Do_send_line(c, string_buffer);

                            return;
                        }

                        if ((c.battle.opponent.special_type != phantdefs.SM_MODNAR) &&
                            (c.battle.opponent.special_type != phantdefs.SM_MORON) &&
                            (c.battle.opponent.special_type != phantdefs.SM_SMURF) &&
                            (c.battle.opponent.special_type != phantdefs.SM_TROLL) &&
                            (c.battle.opponent.special_type != phantdefs.SM_SMEAGOL) &&
                            ((c.battle.opponent.special_type != phantdefs.SM_SARUMAN) ||
                             (c.player.amulets >= c.player.charms)) &&
                            (c.battle.opponent.special_type != phantdefs.SM_MIMIC))
                        {

                            /* dwarves/halflings/expers use fewer charms */
                            dtemp = c.battle.opponent.treasure_type
                                   - (CFUNCTIONS.floor((c.player.type + 1) * 1.5));

                            if (dtemp < 1) dtemp = 1;

                            dtemp2 = CFUNCTIONS.ceil(macros.RND() * CFUNCTIONS.floor(CFUNCTIONS.sqrt(c.battle.opponent.size) *
                                                  dtemp));
                            if (dtemp2 < dtemp)
                            {
                                dtemp2 = dtemp;
                            }

                            if (c.player.charms >= dtemp2)
                            {
                                CFUNCTIONS.sprintf(ref string_buffer,
                                        "%s, %s, %.0lf charms blocked size %.0lf attack (%.0lf TT %d)\n",
                                        c.player.lcname,
                                        c.realm.charstats[c.player.type].class_name,
                                        dtemp2, c.battle.opponent.size, dtemp,
                                        c.battle.opponent.treasure_type);

                                fileclass.Do_log(pathnames.COMBAT_LOG, string_buffer);

                                string_buffer = CFUNCTIONS.sprintfSinglestring("%s tries to do something special, but you used %.0lf of your charms to block it!\n", c.battle.opponent.name, dtemp2);


                                ioclass.Do_send_line(c, string_buffer);

                                c.player.charms -= (int)dtemp2;

                                return;
                            }
                        }

                        switch (c.battle.opponent.special_type)
                        {

                            case phantdefs.SM_LEANAN:

                                /* takes some of the player's strength */
                                inflict = macros.ROLL(1.0, c.battle.opponent.size * 4);
                                inflict = CFUNCTIONS.MIN(c.player.level, inflict);
                                inflict = CFUNCTIONS.MAX(.02 * c.player.strength, inflict);

                                if (inflict > c.player.strength)
                                {

                                    string_buffer = CFUNCTIONS.sprintfSinglestring("%s sucks all of your strength away, destroying your soul!\n",
                                        c.battle.opponent.name);

                                    inflict = c.player.strength;

                                }
                                else
                                {


                                    string_buffer = CFUNCTIONS.sprintfSinglestring("%s sapped %0.f of your strength!\n",
                                            c.battle.opponent.name, inflict);
                                }


                                ioclass.Do_send_line(c, string_buffer);


                                statsclass.Do_strength(c, c.player.max_strength - inflict,
                                    c.player.sword, c.battle.strengthSpell, 0);

                                return;

                            case phantdefs.SM_THAUMATURG:

                                /* transport player */
                                string_buffer = CFUNCTIONS.sprintfSinglestring("%s transported you!\n",
                                c.battle.opponent.name);


                                ioclass.Do_send_line(c, string_buffer);

                                event_ptr = eventclass.Do_create_event();
                                event_ptr.type = (short)phantdefs.MOVE_EVENT;
                                event_ptr.arg3 = phantdefs.A_FAR;

                                eventclass.Do_file_event(c, event_ptr);

                                Do_cancel_monster(c.battle.opponent);

                                return;

                            case phantdefs.SM_SARUMAN:

                                if (c.player.charms > c.player.amulets)
                                {

                                    CFUNCTIONS.sprintf(ref string_buffer,
                                        "%s turns your charms into amulets and vice versa!\n",
                                        c.battle.opponent.name);

                                    ioclass.Do_send_line(c, string_buffer);

                                    dtemp = c.player.charms;
                                    c.player.charms = c.player.amulets;
                                    c.player.amulets = (int)dtemp;
                                }
                                else if (c.player.palantir)
                                {
                                    /* take away palantir */
                                    ioclass.Do_send_line(c, "Wormtongue stole your palantir!\n");

                                    statsclass.Do_palantir(c, 0, 0);
                                }
                                else if (macros.RND() > 0.5)
                                {
                                    /* gems turn to gold */
                                    CFUNCTIONS.sprintf(ref string_buffer,
                                        "%s transformed your gems into gold!\n",
                                        c.battle.opponent.name);


                                    ioclass.Do_send_line(c, string_buffer);

                                    statsclass.Do_gold(c, c.player.gems, 0);

                                    statsclass.Do_gems(c, -c.player.gems, 0);
                                }
                                else
                                {
                                    /* scramble some stats */
                                    CFUNCTIONS.sprintf(ref string_buffer,
                                        "%s casts a spell and you feel different!\n",
                                        c.battle.opponent.name);


                                    ioclass.Do_send_line(c, string_buffer);

                                    Do_scramble_stats(c);
                                }
                                return;

                            case phantdefs.SM_VORTEX:

                                /* suck up some mana */
                                inflict = macros.ROLL(0, 50 * c.battle.opponent.size);
                                inflict = CFUNCTIONS.MIN(c.player.mana, CFUNCTIONS.floor(inflict));


                                string_buffer = CFUNCTIONS.sprintfSinglestring("%s sucked up %.0f of your mana!\n",
                                    c.battle.opponent.name, inflict);


                                ioclass.Do_send_line(c, string_buffer);


                                statsclass.Do_mana(c, -inflict, 0);
                                return;

                            case phantdefs.SM_SUCCUBUS:

                                /* take some brains */
                                string_buffer = CFUNCTIONS.sprintfSinglestring("%s caresses you and whispers sweet nothings in your ear.  You feel foolish!\n",
                            c.battle.opponent.name);


                                ioclass.Do_send_line(c, string_buffer);
                                c.player.brains *= 0.8;
                                return;

                            case phantdefs.SM_TIAMAT:

                                /* take some gold and gems */
                                CFUNCTIONS.sprintf(ref string_buffer,
                            "%s took half your gold and gems and flew off.\n",
                            c.battle.opponent.name);


                                ioclass.Do_send_line(c, string_buffer);

                                statsclass.Do_gold(c, -CFUNCTIONS.floor(c.player.gold / 2.0), 0);
                                statsclass.Do_gems(c, -CFUNCTIONS.floor(c.player.gems / 2.0), 0);
                                Do_cancel_monster(c.battle.opponent);
                                return;

                            case phantdefs.SM_KOBOLD:

                                /* steal a gold piece and run */
                                if (c.player.gold > 0)
                                {
                                    CFUNCTIONS.sprintf(ref string_buffer,
                            "%s stole one gold piece and ran away.\n",
                            c.battle.opponent.name);


                                    ioclass.Do_send_line(c, string_buffer);

                                    statsclass.Do_gold(c, -1.0, 0);

                                    miscclass.Do_check_weight(c);
                                    Do_cancel_monster(c.battle.opponent);
                                    return;
                                }
                                else break;

                            case phantdefs.SM_SHELOB:

                                /* bite and (medium) poison */
                                string_buffer = CFUNCTIONS.sprintfSinglestring("%s has bitten and poisoned you!\n",
                            c.battle.opponent.name);


                                ioclass.Do_send_line(c, string_buffer);

                                statsclass.Do_adjusted_poison(c, 1.0);
                                return;

                            case phantdefs.SM_LAMPREY:

                                if (macros.RND() * 10 < (c.battle.opponent.size / 2) - 1)
                                {
                                    /*  bite and (small) poison */
                                    string_buffer = CFUNCTIONS.sprintfSinglestring("%s bit and poisoned you!\n",
                                c.battle.opponent.name);


                                    ioclass.Do_send_line(c, string_buffer);

                                    statsclass.Do_adjusted_poison(c, 0.25);
                                }
                                return;

                            case phantdefs.SM_BONNACON:

                                /* fart and run */
                                string_buffer = CFUNCTIONS.sprintfSinglestring("%s farted and scampered off.\n",
                            c.battle.opponent.name);


                                ioclass.Do_send_line(c, string_buffer);

                                /* damage from fumes */
                                statsclass.Do_energy(c, c.player.energy / 2.0, c.player.max_energy,
                                    c.player.shield, c.battle.force_field, 0);

                                Do_cancel_monster(c.battle.opponent);
                                return;

                            case phantdefs.SM_SMEAGOL:

                                if (c.player.ring_type != phantdefs.R_NONE)
                                {

                                    /* try to steal ring */
                                    if (macros.RND() > 0.1)

                                        CFUNCTIONS.sprintf(ref string_buffer,
                                "%s tried to steal your ring, but was unsuccessful.\n",
                            c.battle.opponent.name);

                                    else
                                    {

                                        CFUNCTIONS.sprintf(ref string_buffer,
                                 "%s tried to steal your ring and ran away with it!\n",
                             c.battle.opponent.name);


                                        statsclass.Do_ring(c, phantdefs.R_NONE, 0);
                                        Do_cancel_monster(c.battle.opponent);
                                    }

                                    ioclass.Do_send_line(c, string_buffer);
                                    return;
                                }
                                else if (c.player.type == phantdefs.C_HALFLING)
                                {
                                    if (c.player.sin > 2.0)
                                    {
                                        CFUNCTIONS.sprintf(ref string_buffer,
                                     "%s cries, 'Thief!  Baggins!  We hates it, we hates it, for ever and ever!'\n",
                                             c.battle.opponent.name);

                                        ioclass.Do_send_line(c, string_buffer);

                                    }
                                    else
                                    {
                                        CFUNCTIONS.sprintf(ref string_buffer,
                                             "%s wonders, 'What has it got in itsss pocketsss?'\n",
                                             c.battle.opponent.name);

                                        ioclass.Do_send_line(c, string_buffer);
                                    }
                                }
                                break;

                            case phantdefs.SM_CERBERUS:

                                /* take all metal treasures */
                                string_buffer = CFUNCTIONS.sprintfSinglestring("%s took all your metal treasures and ran off!\n",
                            c.battle.opponent.name);


                                ioclass.Do_send_line(c, string_buffer);


                                string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, cerbed.\n",
                                       c.player.lcname,
                                    c.realm.charstats[c.player.type].class_name);


                                fileclass.Do_log(pathnames.COMBAT_LOG, string_buffer);


                                statsclass.Do_energy(c, c.player.energy, c.player.max_energy, 0.0,
                                    c.battle.force_field, 0);


                                statsclass.Do_strength(c, c.player.max_strength, 0.0,
                                    c.battle.strengthSpell, 0);

                                if (c.player.level > phantdefs.MIN_KING)
                                {

                                    statsclass.Do_crowns(c, -c.player.crowns, 0);
                                }


                                statsclass.Do_gold(c, -c.player.gold, 0);


                                Do_cancel_monster(c.battle.opponent);
                                return;

                            case phantdefs.SM_UNGOLIANT:

                                /* (large) poison and take a quickness */
                                string_buffer = CFUNCTIONS.sprintfSinglestring("%s stung you with a virulent poison.  You begin to slow down!\n",
                            c.battle.opponent.name);


                                ioclass.Do_send_line(c, string_buffer);

                                statsclass.Do_adjusted_poison(c, 5.0);


                                statsclass.Do_speed(c, c.player.max_quickness - 1.0,
                                    c.player.quicksilver, c.battle.speedSpell, 0);

                                return;

                            case phantdefs.SM_JABBERWOCK:

                                if ((macros.RND() > .75) || (c.player.quicksilver == 0))
                                {
                                    /* fly away, and leave either a Jubjub bird or Bandersnatch */
                                    CFUNCTIONS.sprintf(ref string_buffer,
                                "%s flew away, and left you to contend with one of its friends.\n",
                                c.battle.opponent.name);


                                    ioclass.Do_send_line(c, string_buffer);
                                    event_ptr = eventclass.Do_create_event();
                                    event_ptr.type = (short)phantdefs.MONSTER_EVENT;
                                    event_ptr.arg1 = phantdefs.MONSTER_JABBERWOCK;
                                    event_ptr.arg3 = 55 + ((macros.RND() > 0.5) ? 16 : 0);

                                    eventclass.Do_file_event_first(c, event_ptr);

                                    Do_cancel_monster(c.battle.opponent);
                                    return;
                                }
                                else
                                {
                                    /* burble, causing the player to lose quicksilver */
                                    CFUNCTIONS.sprintf(ref string_buffer,
                                            "%s burbles as it drinks half of your quicksilver!\n",
                                c.battle.opponent.name);


                                    ioclass.Do_send_line(c, string_buffer);

                                    statsclass.Do_speed(c, c.player.max_quickness,
                                        CFUNCTIONS.floor(c.player.quicksilver * .5), c.battle.speedSpell, 0);
                                    return;
                                }


                            case phantdefs.SM_TROLL:

                                /* partially regenerate monster */
                                CFUNCTIONS.sprintf(ref string_buffer,
                            "%s partially regenerated his energy!\n",
                            c.battle.opponent.name);


                                ioclass.Do_send_line(c, string_buffer);

                                c.battle.opponent.energy += CFUNCTIONS.floor(
                                (c.battle.opponent.max_energy -

                                c.battle.opponent.energy) / 2.0);

                                c.battle.opponent.strength = c.battle.opponent.max_strength;
                                c.battle.opponent.speed = c.battle.opponent.max_speed;
                                c.battle.melee_damage = c.battle.skirmish_damage = 0.0;
                                return;

                            case phantdefs.SM_WRAITH:

                                if (!c.player.blind)
                                {

                                    /* make blind */
                                    string_buffer = CFUNCTIONS.sprintfSinglestring("%s blinded you!\n",
                                c.battle.opponent.name);


                                    ioclass.Do_send_line(c, string_buffer);
                                    c.player.blind = true;
                                    CFUNCTIONS.strcpy(ref c.battle.opponent.name, "A monster");

                                    /* update the character description */
                                    miscclass.Do_lock_mutex(c.realm.realm_lock);

                                    characterclass.Do_player_description(c);

                                    miscclass.Do_unlock_mutex(c.realm.realm_lock);


                                    return;
                                }
                                break;

                            case phantdefs.SM_IDIOT:

                                string_buffer = CFUNCTIONS.sprintfSinglestring("%s drools.\n",
                                    c.battle.opponent.name);


                                ioclass.Do_send_line(c, string_buffer);
                                break;

                            case phantdefs.SM_MORON:

                                /* don't subject males to pickup line */
                                if (c.player.gender == (phantdefs.FEMALE == 1 ? true : false))
                                {

                                    ioclass.Do_send_line(c, moronmesg[(int)macros.ROLL(0.0, moronmesg.Length)]);
                                    // (double)sizeof(moronmesg) / sizeof(char*))]);
                                }
                                else
                                {

                                    ioclass.Do_send_line(c, moronmesg[(int)macros.ROLL(0.0, moronmesg.Length)]);
                                    //((double)(sizeof(moronmesg) / sizeof(char*)) - 2))]);
                                }

                                break;

                            case phantdefs.SM_SMURF:

                                if (macros.RND() < .5)
                                {
                                    if (macros.RND() < .5)
                                    {
                                        if (c.player.gender == (phantdefs.FEMALE == 1 ? true : false))
                                        {
                                            string_buffer = CFUNCTIONS.sprintfSinglestring("%s sneers, 'Smurfette is prettier than you!'\n", c.battle.opponent.name);
                                        }
                                        else if (c.player.type == phantdefs.C_MAGIC)
                                        {
                                            string_buffer = CFUNCTIONS.sprintfSinglestring("%s yells out, 'Aah!  I'm being attacked by Gargamel!'\n", c.battle.opponent.name);
                                        }
                                        else if (c.player.type == phantdefs.C_HALFLING)
                                        {
                                            string_buffer = CFUNCTIONS.sprintfSinglestring("%s wonders, 'Are you Angry Smurf?'\n", c.battle.opponent.name);
                                        }
                                        else if (c.player.type == phantdefs.C_DWARF)
                                        {
                                            string_buffer = CFUNCTIONS.sprintfSinglestring("%s howls, 'A giant!  Run!'\n", c.battle.opponent.name);
                                        }
                                        else
                                        {
                                            string_buffer = CFUNCTIONS.sprintfSinglestring("%s snarls, 'Smurf you, you smurfing smurf!'\n", c.battle.opponent.name);
                                        }
                                    }
                                    else
                                    {
                                        string_buffer = CFUNCTIONS.sprintfSinglestring("%s shrieks, 'Help, Papa Smurf, Help!'\n", c.battle.opponent.name);
                                    }

                                }
                                else
                                {
                                    string_buffer = CFUNCTIONS.sprintfSinglestring("%s sings, 'Lah lah la lah la la!'\n", c.battle.opponent.name);
                                }


                                ioclass.Do_send_line(c, string_buffer);
                                break;


                            case phantdefs.SM_NAZGUL:

                                /* try to take ring if player has one */
                                if (c.player.ring_type != phantdefs.R_NONE)
                                {

                                    /* player has a ring */
                                    string_buffer = CFUNCTIONS.sprintfSinglestring("%s demands your ring.  Do you hand it over?\n", c.battle.opponent.name);

                                    ioclass.Do_send_line(c, string_buffer);

                                    if (ioclass.Do_yes_no(c, ref theAnswer) == phantdefs.S_NORM && theAnswer == 0)
                                    {

                                        /* take ring away */
                                        statsclass.Do_ring(c, phantdefs.R_NONE, 0);
                                        c.battle.ring_in_use = false;
                                        Do_cancel_monster(c.battle.opponent);
                                        return;
                                    }
                                    else
                                    {
                                        c.battle.opponent.strength *= 1.1 + .4 * macros.RND();

                                        c.battle.opponent.max_speed++;
                                        c.battle.opponent.speed =
                                             c.battle.opponent.speed +
                                             CFUNCTIONS.ceil((c.battle.opponent.max_speed -
                                                  c.battle.opponent.speed) / 2);

                                        string_buffer = CFUNCTIONS.sprintfSinglestring("Angered by your refusal, %s attacks harder and faster!.\n", c.battle.opponent.name);


                                        ioclass.Do_send_line(c, string_buffer);
                                    }
                                    /* also fall through to the curse */

                                }
                                /* curse the player */

                                if (c.player.blessing == true)
                                {
                                    string_buffer = CFUNCTIONS.sprintfSinglestring("%s hurls an eldritch curse at you!  But you were saved by your blessing!\n", c.battle.opponent.name);

                                    ioclass.Do_send_line(c, string_buffer);

                                    statsclass.Do_blessing(c, 0, 0);
                                }
                                else
                                {
                                    string_buffer = CFUNCTIONS.sprintfSinglestring("%s hurls an eldritch curse at you!  You feel weak and ill!\n", c.battle.opponent.name);

                                    ioclass.Do_send_line(c, string_buffer);


                                    statsclass.Do_energy(c,
                                                (c.player.energy + c.battle.force_field) / 2,
                                    c.player.max_energy * .99, c.player.shield,
                                    c.battle.force_field, 0);


                                    statsclass.Do_adjusted_poison(c, 0.5);
                                    return;
                                }
                                break;
                        }
                    }
                    break;
            }

            /* fall through to here if monster inflicts a normal hit */
            if (inflict == -1)
            {

                inflict = CFUNCTIONS.floor(1.0 + macros.RND() * c.battle.opponent.strength);
            }

            string_buffer = CFUNCTIONS.sprintfSinglestring("%s hit you for %.0f damage!\n",
                c.battle.opponent.name, inflict);

            ioclass.Do_send_line(c, string_buffer);

            if (c.battle.force_field < inflict)
            {


                statsclass.Do_energy(c, c.player.energy + c.battle.force_field - inflict,
                    c.player.max_energy, c.player.shield, 0, 0);
            }
            else
            {


                statsclass.Do_energy(c, c.player.energy, c.player.max_energy, c.player.shield,
                    c.battle.force_field - inflict, 0);
            }

            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_cancel_monster(struct opponent_t the_opponent)
        /
        / FUNCTION: mark current monster as no longer active
        /
        / AUTHOR: E. A. Estes, 12/4/85
        /	  Brian Kelly, 5/23/99
        /
        / ARGUMENTS: none
        /
        / RETURN VALUE: none
        /
        / MODULES CALLED: none
        /
        / DESCRIPTION:
        /       Clear current monster's energy, experience, treasure type, and
        /       flock.  This is the same as having the monster run away.
        /
        *************************************************************************/

        internal void Do_cancel_monster(opponent_t the_opponent)
        {
            the_opponent.energy = 0.0;
            the_opponent.experience = 0.0;
            the_opponent.treasure_type = 0;
            the_opponent.flock_percent = 0;
        }


        /**/
        /************************************************************************
        /
        / FUNCTION NAME: Do_hitmonster()
        /
        / FUNCTION: inflict damage upon current monster
        /
        / AUTHOR: E. A. Estes, 12/4/85
        /	  Brian Kelly, 6/9/99
        /
        / ARGUMENTS:
        /       double inflict - damage to inflict upon monster
        /
        / RETURN VALUE: none
        /
        / MODULES CALLED: monsthits(), wmove(), cfunctions.strcmp(), waddstr(), mvprintw()
        /
        / DESCRIPTION:
        /       Hit monster specified number of times.  Handle when monster dies,
        /       and a few special monsters.
        /
        *************************************************************************/

        void Do_hitmonster(client_t c, double inflict)
        {
            string string_buffer = ""; //[phantdefs.SZ_LINE];

            string_buffer = CFUNCTIONS.sprintfSinglestring("You hit %s for %.0f damage!\n", c.battle.opponent.name,
                inflict);

            ioclass.Do_send_line(c, string_buffer);

            c.battle.opponent.shield -= inflict;

            if (c.battle.opponent.shield < 0.0)
            {
                c.battle.opponent.energy += c.battle.opponent.shield;
                c.battle.opponent.shield = 0.0;
            }

            if (c.battle.opponent.special_type != phantdefs.SM_IT_COMBAT)
            {

                if (c.battle.opponent.energy > 0.0)
                {

                    if (c.battle.opponent.special_type == phantdefs.SM_DARKLORD ||
                    c.battle.opponent.special_type == phantdefs.SM_SHRIEKER)
                    {

                        /* special monster didn't die */
                        Do_monsthits(c);
                    }
                }
                else
                {

                    /* monster died.  print message. */
                    if (c.battle.opponent.special_type == phantdefs.SM_DARKLORD)
                    {


                        string_buffer = CFUNCTIONS.sprintfSinglestring("%s has just defeated the Dark Lord!\n",
                            c.modifiedName);


                        ioclass.Do_broadcast(c, string_buffer);
                    }
                    else if (c.battle.opponent.special_type == phantdefs.SM_MORGOTH)
                    {


                        string_buffer = CFUNCTIONS.sprintfSinglestring("You have overthrown Morgoth!  But beware, he will return...\n",
                            c.modifiedName);
                        c.morgothCount = 500;


                        ioclass.Do_send_line(c, string_buffer);


                        string_buffer = CFUNCTIONS.sprintfSinglestring("All hail %s for overthrowing Morgoth, Enemy of the Vala!\n",
                            c.modifiedName);


                        ioclass.Do_broadcast(c, string_buffer);

                    }
                    else if (c.battle.opponent.special_type == phantdefs.SM_SMURF &&
                             !c.player.blind)
                    {


                        string_buffer = CFUNCTIONS.sprintfSinglestring("You finally manage to squish the Smurf.  Good work, %s.\n",
                            c.modifiedName);


                        ioclass.Do_send_line(c, string_buffer);

                    }
                    else
                    {

                        /* all other types of monsters */
                        string_buffer = CFUNCTIONS.sprintfSinglestring("You killed it.  Good work, %s.\n",
                            c.modifiedName);


                        ioclass.Do_send_line(c, string_buffer);

                        if (c.battle.opponent.special_type == phantdefs.SM_MIMIC &&

                        CFUNCTIONS.strcmp(c.battle.opponent.name, "A Mimic") &&
                        !c.player.blind)
                        {

                            CFUNCTIONS.sprintf(ref string_buffer,
                       "%s's body slowly changes into the form of a mimic.\n",
                       c.battle.opponent.name);


                            ioclass.Do_send_line(c, string_buffer);
                        }
                    }
                }
            }
        }


        /**/
        /************************************************************************
        /
        / FUNCTION NAME: Do_throwspell(struct client_t *c)
        /
        / FUNCTION: throw a magic spell
        /
        / AUTHOR: E. A. Estes, 12/4/85
        /	  Brian Kelly, 6/9/99
        /
        / ARGUMENTS: none
        /
        / RETURN VALUE: none
        /
        / MODULES CALLED: hitmonster(), cancelmonster(), cfunctions.sqrt(), cfunctions.floor(), wmove(),
        /       drandom(), altercoordinates(), longjmp(), infloat(), waddstr(), mvprintw(),
        /       getanswer()
        /
        / DESCRIPTION:
        /       Prompt player and process magic spells.
        /
        *************************************************************************/

        void Do_throwspell(client_t c, short theAttack, double theArg)
        {
            button_t theButtons = new button_t();
            double inflict = 0.0;        /* damage inflicted */
            double dtemp = -1;          /* for dtemporary calculations */
            int ch;             /* input */
            event_t event_ptr = new event_t();
            string string_buffer = ""; //[phantdefs.SZ_LINE];
            long answer = -1;
            short stemp;
            float ftemp;

            ioclass.Do_clear_buttons(theButtons, 0);

            if (c.player.magiclvl >= phantdefs.ML_ALLORNOTHING &&
                    c.battle.opponent.special_type != phantdefs.SM_MORGOTH)


                CFUNCTIONS.strcpy(ref theButtons.button[0], "All or Nothing\n");

            if (c.player.magiclvl >= phantdefs.ML_MAGICBOLT &&
                    c.battle.opponent.special_type != phantdefs.SM_MORGOTH)


                CFUNCTIONS.strcpy(ref theButtons.button[1], "Magic Bolt\n");

            if ((c.player.magiclvl >= phantdefs.ML_FORCEFIELD) &&
            (c.battle.opponent.special_type != phantdefs.SM_MORGOTH))
            {

                CFUNCTIONS.strcpy(ref theButtons.button[4], "Force Field\n");
            }

            if (c.player.magiclvl >= phantdefs.ML_XFORM &&
            c.battle.opponent.special_type != phantdefs.SM_MORGOTH)
            {


                CFUNCTIONS.strcpy(ref theButtons.button[7], "Transform\n");
            }
            else
            {

                CFUNCTIONS.strcpy(ref theButtons.button[7], "Cancel\n");
            }

            if (c.player.magiclvl >= phantdefs.ML_INCRMIGHT &&
            c.battle.opponent.special_type != phantdefs.SM_MORGOTH)


                CFUNCTIONS.strcpy(ref theButtons.button[2], "Increase Might\n");

            if (c.player.magiclvl >= phantdefs.ML_HASTE &&
            c.battle.opponent.special_type != phantdefs.SM_MORGOTH)


                CFUNCTIONS.strcpy(ref theButtons.button[3], "Haste\n");

            if (c.player.magiclvl >= phantdefs.ML_XPORT &&
                    c.battle.opponent.special_type != phantdefs.SM_MORGOTH)


                CFUNCTIONS.strcpy(ref theButtons.button[5], "Transport\n");

            if (c.player.magiclvl >= phantdefs.ML_PARALYZE &&
                    c.battle.opponent.special_type != phantdefs.SM_MORGOTH)


                CFUNCTIONS.strcpy(ref theButtons.button[6], "Paralyze\n");

            ioclass.Do_send_line(c, "Which spell do you wish to cast?\n");

            if (ioclass.Do_buttons(c, ref answer, theButtons) != phantdefs.S_NORM)
            {

                theAttack = (short)phantdefs.IT_PASS;
                return;
            }

            switch (answer)
            {

                case 0:   /* all or nothing */

                    ftemp = (float)(c.battle.opponent.sin - (c.player.level / 100));

                    /* for debugging purposes */
                    if (c.battle.opponent.special_type == phantdefs.SM_IT_COMBAT)
                    {
                        CFUNCTIONS.sprintf(ref string_buffer,
                                "%s, %s, IT AON, Level %.0lf, Op Sin: %.0f, %.0f/%\n",
                                c.player.lcname,
                                c.realm.charstats[c.player.type].class_name,
                                c.player.level,
                                c.battle.opponent.sin,
                                ftemp);

                        fileclass.Do_log(pathnames.COMBAT_LOG, string_buffer);
                    }


                    /* 25% or difference in levels div 10000 in it_combat */
                    if ((c.battle.opponent.special_type == phantdefs.SM_IT_COMBAT
                         && macros.RND() < CFUNCTIONS.MAX(.2, ftemp / 100))

                    /* success is based on ML and monster exp in combat */
                    || (c.battle.opponent.special_type != phantdefs.SM_IT_COMBAT &&
                    3000 * c.player.magiclvl / (c.battle.opponent.experience +
                    10000 * c.player.magiclvl) > macros.RND()))
                    {

                        /* success */
                        inflict = c.battle.opponent.energy +
                        c.battle.opponent.shield + 1.0;


                        theAttack = (short)phantdefs.IT_ALL_OR_NOT;

                        theArg = inflict;

                        if (c.battle.opponent.special_type == phantdefs.SM_DARKLORD)
                        {

                            /* all or nothing doesn't quite work against D. L. */
                            inflict *= 0.9;
                        }
                    }
                    else
                    {

                        /* failure -- monster gets stronger and quicker */
                        c.battle.opponent.strength = c.battle.opponent.max_strength
                        *= 2.0;

                        c.battle.opponent.max_speed *= 2.0f;

                        /* paralyzed monsters wake up a bit */
                        if (c.battle.opponent.speed * 2.0 > 1.0)
                        {
                            c.battle.opponent.speed = c.battle.opponent.speed * 2.0f;
                        }
                        else
                        {
                            ++c.battle.opponent.speed;
                        }


                        theAttack = (short)phantdefs.IT_NO_ALL_OR_NOT;


                        string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, no_allornothing.\n",
                            c.player.lcname,
                            c.realm.charstats[c.player.type].class_name);


                        fileclass.Do_log(pathnames.COMBAT_LOG, string_buffer);
                    }

                    if (c.player.mana >= phantdefs.MM_ALLORNOTHING)
                    {

                        /* take a mana if player has one */
                        statsclass.Do_mana(c, -phantdefs.MM_ALLORNOTHING, 0);
                    }

                    Do_hitmonster(c, inflict);


                    string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, allornothing %.0lf.\n",
                        c.player.lcname,
                        c.realm.charstats[c.player.type].class_name, inflict);


                    fileclass.Do_log(pathnames.COMBAT_LOG, string_buffer);

                    break;

                case 1:   /* magic bolt */

                    if (c.player.magiclvl < phantdefs.ML_MAGICBOLT)
                    {

                        ioclass.Do_send_line(c, "You can not cast that spell.\n");

                        theAttack = (short)phantdefs.IT_NO_BOLT;
                    }
                    else
                    {
                        /* prompt for amount to expend */
                        if (ioclass.Do_double_dialog(c, ref dtemp, "How much mana for bolt?\n") != 0)
                        {


                            string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, no_magicbolt.\n",
                                c.player.lcname,
                                c.realm.charstats[c.player.type].class_name);


                            fileclass.Do_log(pathnames.COMBAT_LOG, string_buffer);

                            theAttack = (short)phantdefs.IT_NO_BOLT;
                            break;
                        }

                        if (dtemp > 0.0 && dtemp <= c.player.mana)
                        {


                            statsclass.Do_mana(c, -dtemp, 0);

                            if (c.battle.opponent.special_type == phantdefs.SM_DARKLORD)
                            {

                                /* magic bolts don't work against D. L. */
                                inflict = 0.0;

                                ioclass.Do_send_line(c, "Magic Bolt fired!\n");
                                Do_hitmonster(c, inflict);

                                theAttack = (short)phantdefs.IT_BOLT;

                                theArg = inflict;


                                string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, magicbolt %.0lf.\n",
                                   c.player.lcname,
                                   c.realm.charstats[c.player.type].class_name,
                                   inflict);


                                fileclass.Do_log(pathnames.COMBAT_LOG, string_buffer);
                            }
                            else
                            {

                                if (c.battle.opponent.special_type == phantdefs.SM_IT_COMBAT)
                                {
                                    /* magic bolts in IT_COMBAT don't always work */
                                    if (macros.RND() > .05 + (CFUNCTIONS.sqrt(c.player.magiclvl) / 50))
                                    {

                                        /* the magic bolt fails */
                                        ioclass.Do_send_line(c, "You mess up the incantation!\n");

                                        theAttack = (short)phantdefs.IT_NO_BOLT;

                                        theArg = 0;


                                        string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, no_magicbolt.\n",
                                           c.player.lcname,
                                           c.realm.charstats[c.player.type].class_name);


                                        fileclass.Do_log(pathnames.COMBAT_LOG, string_buffer);
                                    }
                                    else
                                    {
                                        inflict = CFUNCTIONS.floor(dtemp * (.7 + .6 * macros.RND()) *
                            (CFUNCTIONS.pow(c.player.magiclvl, 0.20) + 1));

                                        ioclass.Do_send_line(c, "A weak Magic Bolt is fired!\n");
                                        Do_hitmonster(c, inflict);

                                        theAttack = (short)phantdefs.IT_BOLT;

                                        theArg = inflict;


                                        string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, magicbolt %.0lf.\n",
                                           c.player.lcname,
                                           c.realm.charstats[c.player.type].class_name,
                                           inflict);


                                        fileclass.Do_log(pathnames.COMBAT_LOG, string_buffer);
                                    }

                                }
                                else
                                {


                                    /* Bolt Damage */

                                    inflict = CFUNCTIONS.floor(dtemp * (.7 + .6 * macros.RND()) *
                            (CFUNCTIONS.pow(c.player.magiclvl, 0.40) + 1));


                                    /* Low levels dont get special bolts */

                                    if (c.player.level > 10)
                                    {

                                        /* Dud bolt, normal bolt, or super bolt? */

                                        if ((macros.RND() * (c.player.level + c.player.magiclvl)) <
                                            CFUNCTIONS.sqrt(c.player.level))
                                        {

                                            if (macros.RND() > .5)
                                            {

                                                /* Dud Bolt 50%-100% damage*/
                                                ioclass.Do_send_line(c, "You fumble over the incantation a bit...\n");
                                                inflict = inflict * ((macros.RND() / 2) + .5);
                                            }
                                            else
                                            {

                                                /* Super Bolt 100%-200% Damage + 25%-75% of expended Mana*/
                                                ioclass.Do_send_line(c, "Magical energies surge through you!\n");
                                                inflict = inflict + (inflict * macros.RND());

                                                statsclass.Do_mana(c, dtemp * ((macros.RND() / 2) + .25), 0);
                                            }
                                        }
                                    }

                                    ioclass.Do_send_line(c, "Magic Bolt fired!\n");
                                    Do_hitmonster(c, inflict);

                                    theAttack = (short)phantdefs.IT_BOLT;

                                    theArg = inflict;


                                    string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, magicbolt %.0lf.\n",
                                           c.player.lcname,
                                           c.realm.charstats[c.player.type].class_name,
                                           inflict);


                                    fileclass.Do_log(pathnames.COMBAT_LOG, string_buffer);
                                }
                            }
                        }
                        else if (dtemp > c.player.mana)
                        {

                            ioclass.Do_send_line(c, "You do not have enough mana.\n");


                            string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, no_magicbolt.\n",
                                   c.player.lcname,
                                   c.realm.charstats[c.player.type].class_name);


                            fileclass.Do_log(pathnames.COMBAT_LOG, string_buffer);

                            theAttack = (short)phantdefs.IT_NO_BOLT;
                        }
                        else
                        {

                            string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, no_magicbolt.\n",
                                   c.player.lcname,
                                   c.realm.charstats[c.player.type].class_name);


                            fileclass.Do_log(pathnames.COMBAT_LOG, string_buffer);

                            theAttack = (short)phantdefs.IT_NO_BOLT;
                        }
                    }

                    break;

                case 4:   /* force field */

                    if (c.player.magiclvl < phantdefs.ML_FORCEFIELD)
                    {


                        ioclass.Do_send_line(c, "You can not cast that spell.\n");


                        string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, no_forcefield.\n",
                           c.player.lcname,
                           c.realm.charstats[c.player.type].class_name);


                        fileclass.Do_log(pathnames.COMBAT_LOG, string_buffer);

                        theAttack = (short)phantdefs.IT_NO_SHIELD;
                    }
                    else if (c.player.mana <= phantdefs.MM_FORCEFIELD)
                    {


                        ioclass.Do_send_line(c, "You do not have enough mana.\n");


                        string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, no_forcefield.\n",
                           c.player.lcname,
                           c.realm.charstats[c.player.type].class_name);


                        fileclass.Do_log(pathnames.COMBAT_LOG, string_buffer);

                        theAttack = (short)phantdefs.IT_NO_SHIELD;
                    }
                    else if (c.player.mana < phantdefs.MM_FORCEFIELD + (c.player.magiclvl / 2))
                    {


                        ioclass.Do_send_line(c, "You run out of mana before you can put up a full-strength field.\n");


                        statsclass.Do_energy(c, c.player.energy, c.player.max_energy,
                            c.player.shield, c.battle.force_field + 40 *
                                    (c.player.mana - phantdefs.MM_FORCEFIELD), 0);

                        statsclass.Do_mana(c, -(c.player.mana), 0);


                        string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, weak-forcefield.\n",
                           c.player.lcname,
                           c.realm.charstats[c.player.type].class_name);


                        fileclass.Do_log(pathnames.COMBAT_LOG, string_buffer);

                        theAttack = (short)phantdefs.IT_SHIELD;
                    }
                    else
                    {


                        statsclass.Do_mana(c, -(phantdefs.MM_FORCEFIELD + (c.player.magiclvl / 2)), 0);


                        statsclass.Do_energy(c, c.player.energy, c.player.max_energy,
                            c.player.shield, c.battle.force_field + 20 *
                                    c.player.magiclvl, 0);

                        ioclass.Do_send_line(c, "Force Field up.\n");


                        string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, forcefield.\n",
                           c.player.lcname,
                           c.realm.charstats[c.player.type].class_name);


                        fileclass.Do_log(pathnames.COMBAT_LOG, string_buffer);

                        theAttack = (short)phantdefs.IT_SHIELD;
                    }

                    break;

                case 7:   /* transform */

                    if (c.player.magiclvl < phantdefs.ML_XFORM || c.battle.opponent.special_type
                    == phantdefs.SM_MORGOTH)
                    {

                        /* Drop out, this is a cancel */
                        string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, cancel.\n",
                           c.player.lcname,
                           c.realm.charstats[c.player.type].class_name);


                        fileclass.Do_log(pathnames.COMBAT_LOG, string_buffer);
                        theAttack = (short)phantdefs.IT_PASS;
                        Do_playerhits(c, theAttack, theArg);
                    }
                    else if (c.player.mana < phantdefs.MM_XFORM)
                    {

                        ioclass.Do_send_line(c, "You do not have enough mana.\n");


                        string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, no_transform.\n",
                           c.player.lcname,
                           c.realm.charstats[c.player.type].class_name);


                        fileclass.Do_log(pathnames.COMBAT_LOG, string_buffer);

                        theAttack = (short)phantdefs.IT_NO_TRANSFORM;
                    }
                    else if (c.battle.opponent.special_type == phantdefs.SM_IT_COMBAT)
                    {

                        statsclass.Do_mana(c, -phantdefs.MM_XFORM, 0);

                        /* spell succeeds against another player */
                        if (macros.RND() < 0.15)
                        {
                            event_ptr = eventclass.Do_create_event();
                            event_ptr.type = (short)phantdefs.MONSTER_EVENT;
                            event_ptr.arg1 = phantdefs.MONSTER_TRANSFORM;
                            event_ptr.arg3 = (int)macros.ROLL(0.0, 100.0);


                            string_buffer = CFUNCTIONS.sprintfSinglestring("%s screams in agony and becomes %s.\n",
                                c.battle.opponent.name,
                                    c.realm.monster[event_ptr.arg3].name);


                            ioclass.Do_send_line(c, string_buffer);


                            eventclass.Do_file_event_first(c, event_ptr);

                            Do_cancel_monster(c.battle.opponent);

                            theAttack = (short)phantdefs.IT_TRANSFORM;

                            theArg = event_ptr.arg3;


                            string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, transform.\n",
                               c.player.lcname,
                               c.realm.charstats[c.player.type].class_name);


                            fileclass.Do_log(pathnames.COMBAT_LOG, string_buffer);
                        }
                        /* spell backfires */
                        else if (macros.RND() < 0.02)
                        {
                            event_ptr = eventclass.Do_create_event();
                            event_ptr.type = (short)phantdefs.DEATH_EVENT;
                            event_ptr.arg1 = (double)macros.ROLL(0.0, 90.0);
                            event_ptr.arg3 = phantdefs.K_TRANSFORMED;
                            Do_cancel_monster(c.battle.opponent);
                            theAttack = (short)phantdefs.IT_TRANSFORM_BACK;
                            theArg = event_ptr.arg1;


                            CFUNCTIONS.sprintf(ref string_buffer,
                                "The spell backfires and you become %s.\n",
                                    c.realm.monster[event_ptr.arg3].name);


                            ioclass.Do_send_line(c, string_buffer);


                            string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, transform_back.\n",
                               c.player.lcname,
                               c.realm.charstats[c.player.type].class_name);


                            fileclass.Do_log(pathnames.COMBAT_LOG, string_buffer);
                            eventclass.Do_handle_event(c, event_ptr);
                        }
                        /* spell fails */
                        else
                        {

                            string_buffer = CFUNCTIONS.sprintfSinglestring("%s resists the Transform spell.\n",
                                c.battle.opponent.name);


                            ioclass.Do_send_line(c, string_buffer);


                            string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, no_transform.\n",
                               c.player.lcname,
                               c.realm.charstats[c.player.type].class_name);


                            fileclass.Do_log(pathnames.COMBAT_LOG, string_buffer);
                            theAttack = (short)phantdefs.IT_NO_TRANSFORM;
                        }
                    }
                    else
                    {


                        statsclass.Do_mana(c, -phantdefs.MM_XFORM, 0);

                        if (8000 * c.player.magiclvl / (c.battle.opponent.experience +
                            20000 * c.player.magiclvl) > macros.RND() - .1)
                        {

                            event_ptr = eventclass.Do_create_event();
                            event_ptr.type = (short)phantdefs.MONSTER_EVENT;
                            event_ptr.arg1 = phantdefs.MONSTER_TRANSFORM;
                            event_ptr.arg3 = (int)macros.ROLL(0.0, 100.0);


                            eventclass.Do_file_event_first(c, event_ptr);


                            string_buffer = CFUNCTIONS.sprintfSinglestring("The monster shudders and transforms!\n");

                            ioclass.Do_send_line(c, string_buffer);


                            Do_cancel_monster(c.battle.opponent);
                            theAttack = (short)phantdefs.IT_TRANSFORM;


                            string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, transform.\n",
                               c.player.lcname,
                               c.realm.charstats[c.player.type].class_name);


                            fileclass.Do_log(pathnames.COMBAT_LOG, string_buffer);
                        }
                        /* the spell backfires */

                        /*  Removed 12/29/00 - Insta-death is mean
                                else if (macros.RND() < 0.01) {
                                event_ptr = eventclass.Do_create_event();
                                        event_ptr.type = (short)phantdefs.DEATH_EVENT;
                                        event_ptr.arg1 = (double) macros.ROLL(0.0, 90.0);
                                        event_ptr.arg3 = phantdefs.K_TRANSFORMED;
                                        eventclass.Do_file_event(c, event_ptr);
                                        Do_cancel_monster(c.battle.opponent);
                                        theAttack = (short)phantdefs.IT_TRANSFORM_BACK;
                                        theArg = event_ptr.arg1;

                                    string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, transform_back.\n",
                                       c.player.lcname,
                                       c.realm.charstats[c.player.type].class_name);

                                    fileclass.Do_log(pathnames.COMBAT_LOG, string_buffer);

                                CFUNCTIONS.sprintf(ref string_buffer,
                                    "The spell backfires and you become %s.\n",
                                        c.realm.monster[event_ptr.arg3].name);

                                ioclass.Do_send_line(c, string_buffer);
                                }
                        */
                        /* spell fails */
                        else
                        {

                            string_buffer = CFUNCTIONS.sprintfSinglestring("%s resists the Transform spell.\n",
                                c.battle.opponent.name);


                            ioclass.Do_send_line(c, string_buffer);


                            string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, no_transform.\n",
                               c.player.lcname,
                               c.realm.charstats[c.player.type].class_name);


                            fileclass.Do_log(pathnames.COMBAT_LOG, string_buffer);
                            theAttack = (short)phantdefs.IT_NO_TRANSFORM;
                        }
                    }

                    break;

                case 2:   /* increase might */

                    if (c.player.magiclvl < phantdefs.ML_INCRMIGHT)
                    {

                        ioclass.Do_send_line(c, "You can not cast that spell.\n");
                        theAttack = (short)phantdefs.IT_NO_MIGHT;


                        string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, no_might.\n",
                           c.player.lcname,
                           c.realm.charstats[c.player.type].class_name);


                        fileclass.Do_log(pathnames.COMBAT_LOG, string_buffer);
                    }
                    else if (c.player.mana < phantdefs.MM_INCRMIGHT + (c.player.magiclvl / 2))
                    {

                        ioclass.Do_send_line(c, "You do not have enough mana.\n");
                        theAttack = (short)phantdefs.IT_NO_MIGHT;


                        string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, no_might.\n",
                           c.player.lcname,
                           c.realm.charstats[c.player.type].class_name);


                        fileclass.Do_log(pathnames.COMBAT_LOG, string_buffer);
                    }
                    else
                    {


                        statsclass.Do_mana(c, -(phantdefs.MM_INCRMIGHT + (c.player.magiclvl / 2)), 0);


                        statsclass.Do_strength(c, c.player.max_strength, c.player.sword,
                            c.battle.strengthSpell + c.player.max_strength *

                            (1 - 10 / (CFUNCTIONS.sqrt(c.player.magiclvl) + 10)), 0);

                        theAttack = (short)phantdefs.IT_MIGHT;


                        string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, incrmight.\n",
                           c.player.lcname,
                           c.realm.charstats[c.player.type].class_name);


                        fileclass.Do_log(pathnames.COMBAT_LOG, string_buffer);
                    }

                    break;

                case 3:   /* haste */

                    if (c.player.magiclvl < phantdefs.ML_HASTE)
                    {

                        ioclass.Do_send_line(c, "You can not cast that spell.\n");
                        theAttack = (short)phantdefs.IT_NO_HASTE;


                        string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, no_haste.\n",
                           c.player.lcname,
                           c.realm.charstats[c.player.type].class_name);


                        fileclass.Do_log(pathnames.COMBAT_LOG, string_buffer);
                    }
                    else if (c.player.mana < phantdefs.MM_HASTE + (c.player.magiclvl / 2))
                    {

                        ioclass.Do_send_line(c, "You do not have enough mana.\n");
                        theAttack = (short)phantdefs.IT_NO_HASTE;


                        string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, no_haste.\n",
                           c.player.lcname,
                           c.realm.charstats[c.player.type].class_name);


                        fileclass.Do_log(pathnames.COMBAT_LOG, string_buffer);
                    }
                    else
                    {


                        statsclass.Do_mana(c, -(phantdefs.MM_HASTE + (c.player.magiclvl / 2)), 0);


                        statsclass.Do_speed(c, c.player.max_quickness, c.player.quicksilver,
                            c.battle.speedSpell + c.player.max_quickness / 2 *

                            (1 - 10 / (CFUNCTIONS.sqrt(c.player.magiclvl) + 10)), 0);

                        theAttack = (short)phantdefs.IT_HASTE;


                        string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, haste.\n",
                           c.player.lcname,
                           c.realm.charstats[c.player.type].class_name);


                        fileclass.Do_log(pathnames.COMBAT_LOG, string_buffer);
                    }

                    break;

                case 5:   /* transport */

                    if (c.player.magiclvl < phantdefs.ML_XPORT)
                    {

                        ioclass.Do_send_line(c, "You can not cast that spell.\n");
                        theAttack = (short)phantdefs.IT_NO_TRANSPORT;


                        string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, no_transport.\n",
                           c.player.lcname,
                           c.realm.charstats[c.player.type].class_name);


                        fileclass.Do_log(pathnames.COMBAT_LOG, string_buffer);
                    }
                    else if (c.player.mana < phantdefs.MM_XPORT)
                    {

                        ioclass.Do_send_line(c, "You do not have enough mana.\n");
                        theAttack = (short)phantdefs.IT_NO_TRANSPORT;


                        string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, no_transport.\n",
                           c.player.lcname,
                           c.realm.charstats[c.player.type].class_name);


                        fileclass.Do_log(pathnames.COMBAT_LOG, string_buffer);
                    }
                    else
                    {


                        statsclass.Do_mana(c, -phantdefs.MM_XPORT, 0);

                        if (c.battle.opponent.special_type == phantdefs.SM_IT_COMBAT)
                        {

                            /* spell succeeds aginst another player */
                            if (macros.RND() < 0.25)
                            {

                                Do_cancel_monster(c.battle.opponent);

                                theAttack = (short)phantdefs.IT_TRANSPORT;


                                string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, transport.\n",
                                   c.player.lcname,
                                   c.realm.charstats[c.player.type].class_name);


                                fileclass.Do_log(pathnames.COMBAT_LOG, string_buffer);

                            }
                            else
                            {

                                string_buffer = CFUNCTIONS.sprintfSinglestring("%s resists the transport.\n",
                                c.battle.opponent.name);


                                ioclass.Do_send_line(c, string_buffer);

                                theAttack = (short)phantdefs.IT_NO_TRANSPORT;


                                string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, no_transport.\n",
                                   c.player.lcname,
                                   c.realm.charstats[c.player.type].class_name);


                                fileclass.Do_log(pathnames.COMBAT_LOG, string_buffer);
                            }
                        }
                        else if (1000 * c.player.magiclvl /
                        (c.battle.opponent.experience + 1000 *

                        c.player.magiclvl) > macros.RND())
                        {

                            string_buffer = CFUNCTIONS.sprintfSinglestring("%s is transported.\n",
                        c.battle.opponent.name);


                            ioclass.Do_send_line(c, string_buffer);

                            stemp = c.battle.opponent.treasure_type;

                            Do_cancel_monster(c.battle.opponent);

                            if (macros.RND() > 0.5 && c.battle.opponent.special_type != phantdefs.SM_DARKLORD)
                            {

                                /* monster dropped its treasure */
                                c.battle.opponent.treasure_type = stemp;
                            }


                            theAttack = (short)phantdefs.IT_TRANSPORT;


                            string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, transport.\n",
                               c.player.lcname,
                               c.realm.charstats[c.player.type].class_name);


                            fileclass.Do_log(pathnames.COMBAT_LOG, string_buffer);
                        }
                        else if (macros.RND() < 0.2 || c.battle.opponent.special_type == phantdefs.SM_DARKLORD)
                        {

                            ioclass.Do_send_line(c, "Transport backfired!\n");
                            event_ptr = eventclass.Do_create_event();
                            event_ptr.type = (short)phantdefs.MOVE_EVENT;
                            event_ptr.arg3 = phantdefs.A_FAR;

                            eventclass.Do_file_event(c, event_ptr);

                            Do_cancel_monster(c.battle.opponent);
                            theAttack = (short)phantdefs.IT_TRANSPORT_BACK;


                            string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, transport_back.\n",
                               c.player.lcname,
                               c.realm.charstats[c.player.type].class_name);


                            fileclass.Do_log(pathnames.COMBAT_LOG, string_buffer);
                        }
                        else
                        {

                            string_buffer = CFUNCTIONS.sprintfSinglestring("%s resists the transport.\n",
                                c.battle.opponent.name);


                            ioclass.Do_send_line(c, string_buffer);

                            theAttack = (short)phantdefs.IT_NO_TRANSPORT;


                            string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, no_transport.\n",
                               c.player.lcname,
                               c.realm.charstats[c.player.type].class_name);


                            fileclass.Do_log(pathnames.COMBAT_LOG, string_buffer);
                        }
                    }

                    break;

                case 6:   /* paralyze */

                    if (c.player.magiclvl < phantdefs.ML_PARALYZE)
                    {

                        ioclass.Do_send_line(c, "You can not cast that spell.\n");

                        theAttack = (short)phantdefs.IT_NO_PARALYZE;


                        string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, no_paralyze.\n",
                           c.player.lcname,
                           c.realm.charstats[c.player.type].class_name);


                        fileclass.Do_log(pathnames.COMBAT_LOG, string_buffer);
                    }
                    else if (c.player.mana < phantdefs.MM_PARALYZE)
                    {

                        ioclass.Do_send_line(c, "You do not have enough mana.\n");

                        theAttack = (short)phantdefs.IT_NO_PARALYZE;


                        string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, no_paralyze.\n",
                           c.player.lcname,
                           c.realm.charstats[c.player.type].class_name);


                        fileclass.Do_log(pathnames.COMBAT_LOG, string_buffer);
                    }
                    else
                    {


                        statsclass.Do_mana(c, -phantdefs.MM_PARALYZE, 0);

                        if (c.battle.opponent.special_type == phantdefs.SM_IT_COMBAT)
                        {

                            if (macros.RND() < 0.2)
                            {

                                string_buffer = CFUNCTIONS.sprintfSinglestring("%s is held.\n",
                                c.battle.opponent.name);


                                ioclass.Do_send_line(c, string_buffer);
                                c.battle.opponent.speed = -2.0f;

                                theAttack = (short)phantdefs.IT_PARALYZE;


                                string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, paralyze.\n",
                                   c.player.lcname,
                                   c.realm.charstats[c.player.type].class_name);


                                fileclass.Do_log(pathnames.COMBAT_LOG, string_buffer);
                            }
                            else
                            {

                                string_buffer = CFUNCTIONS.sprintfSinglestring("%s is unaffected.\n",
                                c.battle.opponent.name);


                                ioclass.Do_send_line(c, string_buffer);

                                theAttack = (short)phantdefs.IT_NO_PARALYZE;


                                string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, no_paralyze.\n",
                                   c.player.lcname,
                                   c.realm.charstats[c.player.type].class_name);


                                fileclass.Do_log(pathnames.COMBAT_LOG, string_buffer);
                            }
                        }

                        else if (4000 * c.player.magiclvl /
                        (c.battle.opponent.experience + 6000 *

                        c.player.magiclvl) > macros.RND() - .1)
                        {

                            string_buffer = CFUNCTIONS.sprintfSinglestring("%s is held.\n",
                            c.battle.opponent.name);


                            ioclass.Do_send_line(c, string_buffer);
                            c.battle.opponent.speed = -2.0f;

                            theAttack = (short)phantdefs.IT_PARALYZE;


                            string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, paralyze.\n",
                                c.player.lcname,
                                c.realm.charstats[c.player.type].class_name);


                            fileclass.Do_log(pathnames.COMBAT_LOG, string_buffer);
                        }

                        else
                        {
                            ioclass.Do_send_line(c, "Monster unaffected.\n");

                            theAttack = (short)phantdefs.IT_NO_PARALYZE;


                            string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, no_paralyze.\n",
                                c.player.lcname,
                                c.realm.charstats[c.player.type].class_name);


                            fileclass.Do_log(pathnames.COMBAT_LOG, string_buffer);
                        }
                    }

                    break;

                case 8:   /* specify */

                    if (c.player.special_type < phantdefs.SC_COUNCIL ||

                    c.battle.opponent.special_type == phantdefs.SM_IT_COMBAT)


                        ioclass.Do_send_line(c, "You can not cast that spell.\n");
                    else if (c.player.mana < phantdefs.MM_SPECIFY)

                        ioclass.Do_send_line(c, "You do not have enough mana.\n");
                    else
                    {


                        statsclass.Do_mana(c, -phantdefs.MM_SPECIFY, 0);
                        event_ptr = eventclass.Do_create_event();
                        event_ptr.type = (short)phantdefs.MONSTER_EVENT;
                        event_ptr.arg1 = phantdefs.MONSTER_SPECIFY;

                        if (ioclass.Do_long_dialog(c, ref event_ptr.arg3, "Which monster [0-99]?\n") != 0)
                        {
                            event_ptr = null;//free((void*)event_ptr);
                            return;
                        }
                        if (event_ptr.arg3 >= phantdefs.NUM_MONSTERS || event_ptr.arg3 < 0)
                        {
                            event_ptr.arg3 = 14;
                        }


                        eventclass.Do_handle_event(c, event_ptr);

                        Do_cancel_monster(c.battle.opponent);
                    }

                    break;
            }
        }


        /**/
        /************************************************************************
        /
        / FUNCTION NAME: Do_scramble_stats(struct client_t *c)
        /
        / FUNCTION: scramble some selected statistics
        /
        / AUTHOR: E. A. Estes, 12/4/85
        /	  Brian Kelly, 6/9/99
        /
        / ARGUMENTS: none
        /
        / RETURN VALUE: none
        /
        / MODULES CALLED: cfunctions.floor(), drandom()
        /
        / DESCRIPTION:
        /       Swap two player statistics randomly (truncating gains).
        /
        *************************************************************************/

        void Do_scramble_stats(client_t c)
        {
            double[] dbuf = new double[8];                 /* to put statistic in */
            double dtemp1 = -1;			 /* for swapping values */
            int count = 0, swapee = 0;
            float ftemp = -1;

            /* fill buffer */
            dbuf[0] = c.player.max_strength;
            dbuf[1] = c.player.max_energy;
            dbuf[2] = c.player.mana;
            dbuf[3] = c.player.brains;
            dbuf[4] = c.player.magiclvl;
            dbuf[5] = c.player.sin;

            while (count == swapee)
            {
                count = (int)macros.ROLL(0, 6);
                swapee = (int)macros.ROLL(0, 6);
            }

            /* swap the items */
            dtemp1 = dbuf[count];
            dbuf[count] = dbuf[swapee];
            dbuf[swapee] = dtemp1;

            /* assign a new strength */
            if (dbuf[0] > c.player.max_strength && (.95 > macros.RND()))
            {
                dbuf[0] = c.player.max_strength;
            }

            statsclass.Do_strength(c, dbuf[0], c.player.sword, c.battle.strengthSpell, 0);

            /* assign new energy */
            if (dbuf[1] > c.player.energy && (.95 > macros.RND()))
            {
                dbuf[1] = c.player.energy;
            }

            statsclass.Do_energy(c, c.player.energy, dbuf[1], c.player.shield, c.battle.force_field, 0);

            /* assign new mana */
            if (dbuf[2] > c.player.mana && (.95 > macros.RND()))
            {
                dbuf[2] = c.player.mana;
            }

            statsclass.Do_mana(c, dbuf[2] - c.player.mana, 0);

            /* assign new brains */
            if (dbuf[3] > c.player.brains && (.95 > macros.RND()))
            {
                dbuf[3] = c.player.brains;
            }
            else if (dbuf[3] < 0)
            {
                dbuf[3] = 0.0;
            }

            c.player.brains = dbuf[3];

            /* assign new magic level */
            if (dbuf[4] > c.player.magiclvl && (.95 > macros.RND()))
            {
                dbuf[4] = c.player.magiclvl;
            }
            else if (dbuf[4] < 0)
            {
                dbuf[4] = 0.0;
            }

            c.player.magiclvl = dbuf[4];

            /* assign new sin */
            if (dbuf[5] > c.player.sin && (.95 > macros.RND()))
            {
                dbuf[5] = c.player.sin;
            }
            else if (dbuf[4] < 0)
            {
                dbuf[5] = 0.0;
            }

            c.player.sin = (float)dbuf[5];

            /* force a smurf check to prevent people from resting on saruman */
            statsclass.Do_sin(c, 0.0);
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_starting_spells(struct client_t *c)
        /
        / FUNCTION: scramble some selected statistics
        /
        / AUTHOR: Brian Kelly, 11/9/99
        /
        / ARGUMENTS: none
        /
        / RETURN VALUE: none
        /
        / MODULES CALLED: cfunctions.floor(), drandom()
        /
        / DESCRIPTION:
        /       Swap a few player statistics randomly.
        /
        *************************************************************************/

        internal void Do_starting_spells(client_t c)
        {

            /* throw up spells */
            while (c.player.shield_nf != 0)
            {


                statsclass.Do_energy(c, c.player.energy, c.player.max_energy,
                    c.player.shield, c.battle.force_field + 9 *
                            c.player.max_energy, 0);


                ioclass.Do_send_line(c, "A force field spell is cast upon you.\n");
                --c.player.shield_nf;
            }

            while (c.player.strong_nf != 0)
            {

                if (c.player.sin < .5 * macros.RND())
                {

                    statsclass.Do_strength(c, c.player.max_strength, c.player.sword,
                    c.battle.strengthSpell + 9 * c.player.max_strength, 0);


                    ioclass.Do_send_line(c, "Your strength is as the strength of ten, because your heart is pure.\n");
                }
                else
                {

                    statsclass.Do_strength(c, c.player.max_strength, c.player.sword,
                    c.battle.strengthSpell + 3 * c.player.max_strength, 0);


                    ioclass.Do_send_line(c, "A strength spell is cast upon you.\n");
                }

                --c.player.strong_nf;
            }

            while (c.player.haste_nf != 0)
            {


                statsclass.Do_speed(c, c.player.max_quickness, c.player.quicksilver,
                    c.battle.speedSpell + 10 + c.player.max_quickness, 0);


                ioclass.Do_send_line(c, "A haste spell is cast upon you.\n");
                --c.player.haste_nf;
            }

            return;
        }

    }
}