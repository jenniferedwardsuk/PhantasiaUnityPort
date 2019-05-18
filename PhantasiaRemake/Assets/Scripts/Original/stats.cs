using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace phantasiaclasses
{
    public class stats //: MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        static stats Instance;
        private stats()
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
            accountclass = account.GetInstance();
            commandsclass = commands.GetInstance();
            socketclass = socket.GetInstance();
            infoclass = info.GetInstance();
            treasureclass = treasure.GetInstance();
            restoreclass = restore.GetInstance();
            itcombatclass = itcombat.GetInstance();
            fightclass = fight.GetInstance();
            initclass = init.GetInstance();
        }
        public static stats GetInstance()
        {
            stats instance = null;
            if (Instance != null)
            {
                instance = Instance;
            }
            else
            {
                instance = new stats();
            }
            return instance;
        }

        phantasiaclasses.misc miscclass;
        phantasiaclasses.file fileclass;
        phantasiaclasses.restore restoreclass;
        phantasiaclasses.tags tagsclass;
        phantasiaclasses.info infoclass;
        phantasiaclasses.io ioclass;
        phantasiaclasses.commands commandsclass;
        phantasiaclasses.socket socketclass;
        phantasiaclasses.itcombat itcombatclass;
        phantasiaclasses.fight fightclass;
        phantasiaclasses.treasure treasureclass;
        phantasiaclasses.eventsrc eventclass;
        phantasiaclasses.hack hackclass;
        phantasiaclasses.character characterclass;
        phantasiaclasses.account accountclass;
        phantasiaclasses.init initclass;

        /************************************************************************
        /
        / FUNCTION NAME: Do_init_player(client_t c)
        /
        / FUNCTION: initialize a character
        /
        / AUTHOR: E. A. Estes, 12/4/85
        /         Brian Kelly, 5/4/99
        /
        / ARGUMENTS:
        /       struct player *playerp - pointer to structure to init
        /
        / RETURN VALUE: none
        /
        / MODULES CALLED: cfunctions.floor(), drandom()
        /
        / DESCRIPTION:
        /       Put a bunch of default values in the given structure.
        /
        *************************************************************************/

        internal void Do_init_player(client_t c)
        {
            c.player.name =
            c.player.lcname =
            c.modifiedName =
            c.player.area = "\0";
            c.player.last_load = 0;
            c.player.password[0] = '\0';

            /* initialize doubles */
            c.player.x =
            c.player.y =
            c.player.circle =
            c.player.brains =
            c.player.magiclvl =

            /* initialize floats */
            c.player.poison =
            c.player.sin =

            /* initialize ints and shorts */
            c.player.location =
            (short)(c.player.age =
            c.player.lives =
            (short)(c.player.holywater =
            c.player.amulets =
            c.player.charms = 0));

            /* initialize booleans */
            c.player.beyond =
            c.player.cloaked =
            c.player.blind =
            c.player.purgatoryFlag = false;
            c.player.gender = phantdefs.MALE == 0 ? false : true;

            /* specifics */
            c.player.degenerated = 1;                 /* don't degenerate initially */

            c.player.type = (short)phantdefs.C_FIGHTER;                /* default */
            c.player.special_type = phantdefs.SC_NONE;

            c.player.shield_nf =
            c.player.haste_nf =
            c.player.strong_nf = 0;

            /* reset the client stat panel */
            Do_name(c);

            c.player.level = 0.0;
            c.player.experience = 0.0;
            Do_experience(c, 0.0, 1);

            Do_energy(c, 0.0, 0.0, 0.0, 0.0, 0);
            Do_strength(c, 0.0, 0.0, 0.0, 0);
            Do_speed(c, 0.0, 0.0, 0.0, 0);

            c.player.mana = 0.0;
            Do_mana(c, 0.0, 1);

            c.player.gold = 0.0;
            Do_gold(c, 0.0, 1);

            c.player.gems = 0.0;
            Do_gems(c, 0.0, 1);

            Do_cloak(c, 0, 1);

            c.player.crowns = 0;
            Do_crowns(c, 0, 1);

            Do_blessing(c, 0, 0);
            Do_palantir(c, 0, 0);
            Do_virgin(c, 0, 0);
            Do_ring(c, phantdefs.R_NONE, 0);

            /* set user specific variables */
            CFUNCTIONS.strcpy(ref c.player.parent_account, c.lcaccount);
            CFUNCTIONS.strcpy(ref c.player.parent_network, c.network);
            c.player.date_created = 0;
            c.player.faithful = true;

            /* historic information */
            c.player.bad_passwords =
            c.player.muteCount =
            c.player.load_count =
            c.player.last_reset =
            c.player.time_played = 0;

            /* set a few client variables to false */
            c.wizard = 0;
            c.stuck = false;

            /* specific client variables */
            c.timeout = 120;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_update_stats(client_t c)
        /
        / FUNCTION: do trading post stuff
        /
        / AUTHOR: Brian Kelly, 11/1/99
        /
        / ARGUMENTS: none
        /
        / RETURN VALUE: none
        /
        / MODULES CALLED: writerecord(), adjuststats(), Mathf.Abs(), more(), cfunctions.sqrt(),
        /       sleep(), cfunctions.floor(), wmove(), drandom(), wclear(), printw(),
        /       altercoordinates(), infloat(), waddstr(), wrefresh(), mvprintw(), getanswer(),
        /       wclrtoeol(), wclrtobot()
        /
        / DESCRIPTION:
        /
        *************************************************************************/

        internal void Do_update_stats(client_t c)
        {
            /* send the player's name */
            Do_name(c);
            Do_experience(c, 0.0, 1);

            Do_energy(c, c.player.energy, c.player.max_energy, c.player.shield,
                c.battle.force_field, 1);

            Do_strength(c, c.player.max_strength, c.player.sword,
                c.battle.strengthSpell, 1);

            Do_speed(c, c.player.max_quickness, c.player.quicksilver,
                c.battle.speedSpell, 1);

            Do_mana(c, 0.0, 1);
            Do_gold(c, 0.0, 1);
            Do_gems(c, 0.0, 1);

            Do_cloak(c, c.player.cloaked ? 1 : 0, 1);
            Do_blessing(c, c.player.blessing ? 1 : 0, 1);
            Do_crowns(c, 0, 1);
            Do_palantir(c, c.player.palantir ? 1 : 0, 1);
            Do_ring(c, c.player.ring_type, 1);
            Do_virgin(c, c.player.virgin ? 1 : 0, 1);
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_name(client_t c)
        /
        / FUNCTION: do trading post stuff
        /
        / AUTHOR: Brian Kelly, 10/2/99
        /
        / ARGUMENTS: none
        /
        / RETURN VALUE: none
        /
        / MODULES CALLED: writerecord(), adjuststats(), Mathf.Abs(), more(), cfunctions.sqrt(),
        /       sleep(), cfunctions.floor(), wmove(), drandom(), wclear(), printw(),
        /       altercoordinates(), infloat(), waddstr(), wrefresh(), mvprintw(), getanswer(),
        /       wclrtoeol(), wclrtobot()
        /
        / DESCRIPTION:
        /
        *************************************************************************/

        internal void Do_name(client_t c)
        {
            /* send the name */
            socketclass.Do_send_int(c, phantdefs.NAME_PACKET);
            string namestr = c.modifiedName.Replace('\0', '¬').Replace("¬", "");
            if (namestr.Length == 0)
                namestr = new string(new char[] { '\0' });
            socketclass.Do_send_string(c, namestr);
            socketclass.Do_send_string(c, "\n");

            miscclass.Do_lock_mutex(c.realm.realm_lock);
            Do_location(c, c.player.x, c.player.y, 1);
            miscclass.Do_unlock_mutex(c.realm.realm_lock);
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_location(client_t c, double x, double y)
        /
        / FUNCTION: do trading post stuff
        /
        / AUTHOR: Brian Kelly, 10/2/99
        /
        / ARGUMENTS: none
        /
        / RETURN VALUE: none
        /
        / MODULES CALLED: writerecord(), adjuststats(), Mathf.Abs(), more(), cfunctions.sqrt(),
        /       sleep(), cfunctions.floor(), wmove(), drandom(), wclear(), printw(),
        /       altercoordinates(), infloat(), waddstr(), wrefresh(), mvprintw(), getanswer(),
        /       wclrtoeol(), wclrtobot()
        /
        / DESCRIPTION:
        /
        *************************************************************************/

        internal void Do_location(client_t c, double x, double y, int force)
        {
            double distance = 0;
            /* Call this function with the realm locked if playing */

            x = CFUNCTIONS.floor(x);
            y = CFUNCTIONS.floor(y);

            /* check for changes */
            if (c.player.x != x || c.player.y != y || force != 0)
            {


                miscclass.Do_distance(x, 0.0, y, 0.0, ref distance);
                c.player.circle = CFUNCTIONS.floor(distance / phantdefs.D_CIRCLE + 1);
                c.player.x = x;
                c.player.y = y;

                miscclass.Do_name_location(c);


                socketclass.Do_send_int(c, phantdefs.LOCATION_PACKET);

                socketclass.Do_send_double(c, x);

                socketclass.Do_send_double(c, y);

                socketclass.Do_send_string(c, c.player.area);

                socketclass.Do_send_string(c, "\n");

                if (c.run_level == phantdefs.PLAY_GAME)
                {
                    c.game.x = x;
                    c.game.y = y;
                    c.game.circle = c.player.circle;

                    CFUNCTIONS.strncpy(ref c.game.area, c.player.area, phantdefs.SZ_AREA);

                    if (c.player.location == phantdefs.PL_REALM)
                    {
                        c.game.useLocationName = false;
                    }
                    else
                    {
                        c.game.useLocationName = true;
                    }

                    if (c.wizard > 2)
                    {
                        c.game.virtualvirtual = true;
                    }
                    else
                    {
                        c.game.virtualvirtual = false;
                    }


                    characterclass.Do_player_description(c);
                }
            }
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_experience(struct client_t t, double theExpierence)
        /
        / FUNCTION: move player to new level
        /
        / AUTHOR: E. A. Estes, 12/4/85
        /	  Brian Kelly, 5/16/99
        /
        / ARGUMENTS: none
        /
        / RETURN VALUE: none
        /
        / MODULES CALLED: death(), cfunctions.floor(), wmove(), drandom(), waddstr(), explevel()
        /
        / GLOBAL INPUTS: Player, *stdscr, *Statptr, Stattable[]
        /
        / GLOBAL OUTPUTS: Player, Changed
        /
        / DESCRIPTION:
        /       Use lookup table to increment important statistics when
        /       progressing to new experience level.
        /       Players are rested to maximum as a bonus for making a new
        /       level.
        /       Check for council of wise, and being too big to be king.
        /
        *************************************************************************/

        internal void Do_experience(client_t c, double theExperience, int force)
        {
            charstats_t statptr = new charstats_t();       /* for pointing into Stattable */
            event_t event_ptr = new event_t();
            double newnew;                    /* new level */
            double inc;                    /* increment between new and old levels */
            string string_buffer = ""; //[phantdefs.SZ_LINE];

            /* add the experience */
            c.player.experience += theExperience;

            /* determine the new level */
            newnew = macros.CALCLEVEL(c.player.experience);
            /*
                if (c.player.experience < 1.1e7)
                    new = cfunctions.floor(cfunctions.pow((c.player.experience / 1000.0), 0.4748));
                else
                    new = cfunctions.floor(cfunctions.pow((c.player.experience / 1250.0), 0.4865));
            */

            inc = newnew - c.player.level;

            /* if we've gone up any levels */
            if (inc > 0)
            {

                /* make sure we send the level information */
                force = 1;
                c.player.level = newnew;

                if (c.player.type == phantdefs.C_EXPER)
                    /* roll a type to use for increment */
                    statptr = c.realm.charstats[(int)macros.ROLL(phantdefs.C_MAGIC,
                    (phantdefs.C_HALFLING - phantdefs.C_MAGIC + 1))];
                else
                    statptr = c.realm.charstats[c.player.type];

                /* add increments to statistics */
                Do_energy(c, c.player.energy + statptr.energy.increase * inc,
                    c.player.max_energy + statptr.energy.increase * inc,
                    c.player.shield, c.battle.force_field, 0);


                Do_strength(c, c.player.max_strength + statptr.strength.increase
                    * inc, c.player.sword, c.battle.strengthSpell, 0);


                Do_mana(c, statptr.mana.increase * inc, 0);

                c.player.brains += statptr.brains.increase * inc;

                c.player.magiclvl = c.player.magiclvl +
                            statptr.magiclvl.increase * inc;

                /* knights may get more energy */
                if (c.player.special_type == phantdefs.SC_KNIGHT)
                {


                    Do_energy(c, c.player.energy - c.knightEnergy +

                        CFUNCTIONS.floor(c.player.max_energy / 4), c.player.max_energy,
                        c.player.shield, c.battle.force_field, 0);

                    c.knightEnergy = CFUNCTIONS.floor(c.player.max_energy / 4);
                }

                if (c.player.level == 1000.0)
                {
                    /* send congratulations message */
                    ioclass.Do_send_line(c, "Congratulations on reaching level 1000!  The throne awaits...\n");

                    ioclass.Do_more(c);

                    ioclass.Do_send_clear(c);
                }

                if (c.player.level >= phantdefs.MAX_STEWARD &&
                    c.player.special_type == phantdefs.SC_STEWARD)
                {

                    /* no longer able to be steward -- dethrone */
                    ioclass.Do_send_line(c,
                       "After level 200, you can no longer be steward.\n");

                    commandsclass.Do_dethrone(c);
                }

                if (c.player.level >= phantdefs.MAX_KING &&
                    c.player.special_type == phantdefs.SC_KING)
                {
                    /* no longer able to be king -- dethrone */
                    ioclass.Do_send_line(c,
                        "After level 2000, you can no longer be king or queen.\n");
                    commandsclass.Do_dethrone(c);
                    c.player.special_type = phantdefs.SC_NONE;
                }

                /* see if staves/crowns should be cashed in */
                if (c.player.crowns > 0)
                {

                    if (c.player.level >= phantdefs.MAX_KING)
                    {



                        ioclass.Do_send_line(c, "Your crowns were cashed in.\n");

                        Do_gold(c, (c.player.crowns) * 5000.0, 0);

                        Do_crowns(c, -(c.player.crowns), 0);

                        if (c.player.special_type == phantdefs.SC_KING)
                        {

                            commandsclass.Do_dethrone(c);
                        }


                        ioclass.Do_more(c);

                        ioclass.Do_send_clear(c);
                    }
                    else if (c.player.level >= phantdefs.MAX_STEWARD && c.player.level < 1000.0)
                    {


                        ioclass.Do_send_line(c, "Your staves were cashed in.\n");

                        Do_gold(c, (c.player.crowns) * 1000.0, 0);

                        Do_crowns(c, -(c.player.crowns), 0);

                        if (c.player.special_type == phantdefs.SC_STEWARD)
                        {

                            commandsclass.Do_dethrone(c);
                        }


                        ioclass.Do_more(c);

                        ioclass.Do_send_clear(c);
                    }
                }

                if (c.player.level >= 3000.0 && c.player.special_type < phantdefs.SC_COUNCIL)
                {

                    /* if by some chance this person is king or knight, dethrone him */
                    if (c.player.special_type == phantdefs.SC_KING || c.player.special_type == phantdefs.SC_KNIGHT)
                    {

                        commandsclass.Do_dethrone(c);
                    }

                    /* announce the new member */
                    CFUNCTIONS.sprintf(ref string_buffer,
                      "The Council of the Wise announces its newest member, %s.\n",
                      c.modifiedName);


                    ioclass.Do_broadcast(c, string_buffer);

                    /* make a member of the council */
                    ioclass.Do_send_clear(c);
                    ioclass.Do_send_line(c, "You have made it to the Council of the Wise.\n");
                    ioclass.Do_send_line(c, "Good Luck on your search for the Holy Grail.\n");

                    c.player.special_type = phantdefs.SC_COUNCIL;

                    /* no rings for council and above */
                    Do_ring(c, phantdefs.R_NONE, 0);

                    c.player.lives = 2;             /* two extra lives */


                    characterclass.Do_send_specification(c, phantdefs.CHANGE_PLAYER_EVENT);


                    ioclass.Do_more(c);
                }

                /* death from old age */
                if ((c.player.level > 9999.0 - (10 * c.player.degenerated)) &&
                    (c.player.special_type != phantdefs.SC_VALAR))
                {

                    event_ptr = eventclass.Do_create_event();
                    event_ptr.type = (short)phantdefs.DEATH_EVENT;
                    event_ptr.arg3 = phantdefs.K_OLD_AGE;
                    event_ptr.to = c.game;

                    eventclass.Do_file_event(c, event_ptr);
                }

                /* player might gain some quickness */
                miscclass.Do_check_weight(c);

                /* update player description */
                miscclass.Do_lock_mutex(c.realm.realm_lock);

                characterclass.Do_player_description(c);

                miscclass.Do_unlock_mutex(c.realm.realm_lock);

                /* recalculate the player's timeout */
                if (c.wizard > 2)
                {
                    c.timeout = 900;
                }
                else if (c.player.level < 80)
                {
                    c.timeout = CFUNCTIONS.floor(60 - c.player.level / 2);
                }
                else
                {
                    c.timeout = 20;
                }

                /* log the increase */
                string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, %d age, %d seconds, level %0.0lf\n",
                    c.player.lcname,
                    c.realm.charstats[c.player.type].class_name,
                    c.player.age, c.player.time_played + CFUNCTIONS.GetUnixEpoch(DateTime.Now) -

                    c.player.last_load, c.player.level);

                fileclass.Do_log(pathnames.LEVEL_LOG, string_buffer);

                /* backup up this character in case of crash */
                characterclass.Do_backup_save(c, 1);

            }

            if (force != 0)
            {

                /* send the level increase */
                socketclass.Do_send_int(c, phantdefs.LEVEL_PACKET);
                socketclass.Do_send_double(c, c.player.level);
            }
            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_energy(client_t c, double energy, double maxEnergy, double shield)
        /
        / FUNCTION: do trading post stuff
        /
        / AUTHOR: Brian Kelly, 10/2/99
        /
        / ARGUMENTS: none
        /
        / RETURN VALUE: none
        /
        / MODULES CALLED: writerecord(), adjuststats(), Mathf.Abs(), more(), cfunctions.sqrt(),
        /       sleep(), cfunctions.floor(), wmove(), drandom(), wclear(), printw(),
        /       altercoordinates(), infloat(), waddstr(), wrefresh(), mvprintw(), getanswer(),
        /       wclrtoeol(), wclrtobot()
        /
        / DESCRIPTION:
        /
        *************************************************************************/

        internal void Do_energy(client_t c, double energy, double maxEnergy, double shield, double forceShield, int force)
        {
            maxEnergy = CFUNCTIONS.floor(maxEnergy);
            shield = CFUNCTIONS.floor(shield);
            energy = CFUNCTIONS.floor(energy);
            forceShield = CFUNCTIONS.floor(forceShield);

            if (maxEnergy < 0)
            {
                maxEnergy = 0;
            }

            if (energy > maxEnergy + shield + c.knightEnergy)
            {
                energy = maxEnergy + shield + c.knightEnergy;
            }

            /* check for changes */
            if (c.player.energy != energy
                || c.player.max_energy != maxEnergy
                || c.player.shield != shield
                || c.battle.force_field != forceShield
                || force != 0)
            {


                socketclass.Do_send_int(c, phantdefs.ENERGY_PACKET);

                socketclass.Do_send_double(c, energy);

                socketclass.Do_send_double(c, maxEnergy + shield + c.knightEnergy);

                socketclass.Do_send_double(c, forceShield);

                c.player.energy = energy;
                c.player.max_energy = maxEnergy;
                c.battle.force_field = forceShield;

                if (c.player.shield != shield || force != 0)
                {


                    socketclass.Do_send_int(c, phantdefs.SHIELD_PACKET);

                    socketclass.Do_send_double(c, shield);

                    c.player.shield = shield;
                }
            }
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_strength(client_t c, double strength, double maxStrength, double sword, double strengthSpell)
        /
        / FUNCTION: do trading post stuff
        /
        / AUTHOR: Brian Kelly, 10/2/99
        /
        / ARGUMENTS: none
        /
        / RETURN VALUE: none
        /
        / MODULES CALLED: writerecord(), adjuststats(), Mathf.Abs(), more(), cfunctions.sqrt(),
        /       sleep(), cfunctions.floor(), wmove(), drandom(), wclear(), printw(),
        /       altercoordinates(), infloat(), waddstr(), wrefresh(), mvprintw(), getanswer(),
        /       wclrtoeol(), wclrtobot()
        /
        / DESCRIPTION:
        /
        *************************************************************************/

        internal void Do_strength(client_t c, double maxStrength, double sword, double strengthSpell, int force)
        {
            double strength;

            /* a player can have a minimum of 0 strength */
            if (maxStrength < 0)
            {
                maxStrength = 0;
            }

            /* calculate strength based on poison */
            strength = 1.0 - c.player.poison *

                c.realm.charstats[c.player.type].weakness / 800.0;

            if (strength > 1.0)
            {
                strength = 1.0;
            }

            if (strength < .1)
            {
                strength = .1;
            }

            strength = maxStrength * strength;

            /* check for changes */
            if (c.player.strength != strength || c.player.max_strength !=
                maxStrength || c.player.sword != sword || c.battle.strengthSpell
                != strengthSpell || force != 0)
            {


                socketclass.Do_send_int(c, phantdefs.STRENGTH_PACKET);

                socketclass.Do_send_double(c, strength * (1 + CFUNCTIONS.sqrt(sword) * phantdefs.N_SWORDPOWER) + strengthSpell);

                socketclass.Do_send_double(c, maxStrength * (1 + CFUNCTIONS.sqrt(sword) * phantdefs.N_SWORDPOWER) + strengthSpell);

                c.player.strength = strength;
                c.player.max_strength = maxStrength;
                c.battle.strengthSpell = strengthSpell;

                if (c.player.sword != sword || force != 0)
                {


                    socketclass.Do_send_int(c, phantdefs.SWORD_PACKET);

                    socketclass.Do_send_double(c, sword);

                    c.player.sword = sword;
                }
            }
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_speed(client_t c, double maxQuickness, double quicksilver, double speedSpell, int force)
        /
        / FUNCTION: do trading post stuff
        /
        / AUTHOR: Brian Kelly, 10/2/99
        /
        / ARGUMENTS: none
        /
        / RETURN VALUE: none
        /
        / MODULES CALLED: writerecord(), adjuststats(), Mathf.Abs(), more(), cfunctions.sqrt(),
        /       sleep(), cfunctions.floor(), wmove(), drandom(), wclear(), printw(),
        /       altercoordinates(), infloat(), waddstr(), wrefresh(), mvprintw(), getanswer(),
        /       wclrtoeol(), wclrtobot()
        /
        / DESCRIPTION:
        /
        *************************************************************************/

        internal void Do_speed(client_t c, double maxQuickness, double quicksilver, double speedSpell, int force)
        {
            double dtemp;
            double quickness;

            /* a player can have a minimum of 0 quickness */
            if (maxQuickness < 0.0)
            {
                maxQuickness = 0.0;
            }

            /* see if the player is carrying too much treasure */
            if (c.wizard < 3)
            {
                dtemp = ((c.player.gold + c.player.gems / 2.0) - 1000.0) /
                    c.realm.charstats[c.player.type].goldtote - c.player.level;
            }
            else
            {
                dtemp = 0.0;
            }

            /* gold can only slow a player down */
            if (dtemp < 0.0)
            {
                dtemp = 0.0;
            }

            /* subtract speed for excessive combat */
            dtemp += c.battle.rounds / phantdefs.N_FATIGUE;

            quickness = maxQuickness + CFUNCTIONS.sqrt(CFUNCTIONS.floor(quicksilver)) + speedSpell +
                c.knightQuickness - dtemp;

            if (quickness < 0.0)
            {
                quickness = 0.0;
            }

            /* check for changes */
            if (c.player.quickness != quickness || c.player.max_quickness !=
                maxQuickness || c.player.quicksilver != quicksilver ||
                c.battle.speedSpell != speedSpell || force != 0)
            {


                socketclass.Do_send_int(c, phantdefs.SPEED_PACKET);

                socketclass.Do_send_double(c, quickness);


                socketclass.Do_send_double(c, maxQuickness + CFUNCTIONS.sqrt(CFUNCTIONS.floor(quicksilver)) + speedSpell +
                    c.knightQuickness);

                c.player.quickness = (float)quickness;
                c.player.max_quickness = (float)maxQuickness;
                c.battle.speedSpell = speedSpell;

                if (c.player.quicksilver != quicksilver || force != 0)
                {


                    socketclass.Do_send_int(c, phantdefs.QUICKSILVER_PACKET);

                    socketclass.Do_send_float(c, quicksilver);

                    c.player.quicksilver = (float)quicksilver;
                }
            }
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_mana(client_t c, double mana)
        /
        / FUNCTION: do trading post stuff
        /
        / AUTHOR: Brian Kelly, 10/2/99
        /
        / ARGUMENTS: none
        /
        / RETURN VALUE: none
        /
        / MODULES CALLED: writerecord(), adjuststats(), Mathf.Abs(), more(), cfunctions.sqrt(),
        /       sleep(), cfunctions.floor(), wmove(), drandom(), wclear(), printw(),
        /       altercoordinates(), infloat(), waddstr(), wrefresh(), mvprintw(), getanswer(),
        /       wclrtoeol(), wclrtobot()
        /
        / DESCRIPTION:
        /
        *************************************************************************/

        internal void Do_mana(client_t c, double mana, int force = 0)
        {
            double newMana;
            double maxMana;
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];

            /* make sure we're still under the maximum mana */
            newMana = CFUNCTIONS.floor(c.player.mana + mana);

            maxMana = 1000.0 + c.player.level *

                    c.realm.charstats[c.player.type].max_mana;

            if (newMana > maxMana)
            {
                newMana = maxMana;
            }

            if (newMana != c.player.mana || force != 0)
            {

                c.player.mana = newMana;

                socketclass.Do_send_int(c, phantdefs.MANA_PACKET);
                socketclass.Do_send_double(c, c.player.mana);
            }
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_gold(client_t c, double gold)
        /
        / FUNCTION: do trading post stuff
        /
        / AUTHOR: Brian Kelly, 10/2/99
        /
        / ARGUMENTS: none
        /
        / RETURN VALUE: none
        /
        / MODULES CALLED: writerecord(), adjuststats(), Mathf.Abs(), more(), cfunctions.sqrt(),
        /       sleep(), cfunctions.floor(), wmove(), drandom(), wclear(), printw(),
        /       altercoordinates(), infloat(), waddstr(), wrefresh(), mvprintw(), getanswer(),
        /       wclrtoeol(), wclrtobot()
        /
        / DESCRIPTION:
        /
        *************************************************************************/

        internal void Do_gold(client_t c, double gold, int force)
        {
            double dtemp;

            if (gold != 0)
            {

                force = 1;

                /* all gold is taxed to drain the economy */
                /*
                    if (gold > 0) {
                            dtemp = cfunctions.floor(gold / (15 + c.player.level));
                            gold -= dtemp;

                        if (dtemp > 0) {
                            miscclass.Do_lock_mutex(c.realm.kings_gold_lock);
                                c.realm.kings_gold += dtemp;
                                miscclass.Do_unlock_mutex(c.realm.kings_gold_lock);
                        }
                    }
                */

                /* make sure we never end up with negative gold */
                if (c.player.gold + gold > 0)
                    c.player.gold = CFUNCTIONS.floor(c.player.gold + gold);
                else
                    c.player.gold = 0;

                miscclass.Do_check_weight(c);
            }

            if (force != 0)
            {
                socketclass.Do_send_int(c, phantdefs.GOLD_PACKET);
                socketclass.Do_send_double(c, c.player.gold);
            }
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_gems(client_t c, double gems)
        /
        / FUNCTION: do trading post stuff
        /
        / AUTHOR: Brian Kelly, 10/3/99
        /
        / ARGUMENTS: none
        /
        / RETURN VALUE: none
        /
        / MODULES CALLED: writerecord(), adjuststats(), Mathf.Abs(), more(), cfunctions.sqrt(),
        /       sleep(), cfunctions.floor(), wmove(), drandom(), wclear(), printw(),
        /       altercoordinates(), infloat(), waddstr(), wrefresh(), mvprintw(), getanswer(),
        /       wclrtoeol(), wclrtobot()
        /
        / DESCRIPTION:
        /
        *************************************************************************/

        internal void Do_gems(client_t c, double gems, int force)
        {
            double dtemp;

            if (gems != 0)
            {

                force = 1;

                /*
                    if (gems > 0) {
                            dtemp = cfunctions.floor(gems / (15 + c.player.level));
                            gems -= dtemp;

                        if (dtemp > 0) {
                            miscclass.Do_lock_mutex(c.realm.kings_gold_lock);
                                c.realm.kings_gold += dtemp * phantdefs.N_GEMVALUE;
                                miscclass.Do_unlock_mutex(c.realm.kings_gold_lock);
                        }
                    }
                */

                /* make sure we never end up with negative gems */
                if (c.player.gems + gems > 0)
                    c.player.gems = CFUNCTIONS.floor(c.player.gems + gems);
                else
                    c.player.gems = 0;


                miscclass.Do_check_weight(c);
            }

            if (force != 0)
            {
                socketclass.Do_send_int(c, phantdefs.GEMS_PACKET);
                socketclass.Do_send_double(c, c.player.gems);
            }
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_cloak(client_t c)
        /
        / FUNCTION: do trading post stuff
        /
        / AUTHOR: E. A. Estes, 12/4/85
        /         Brian Kelly, 5/16/99
        /
        / ARGUMENTS: none
        /
        / RETURN VALUE: none
        /
        / MODULES CALLED: writerecord(), adjuststats(), Mathf.Abs(), more(), cfunctions.sqrt(),
        /       sleep(), cfunctions.floor(), wmove(), drandom(), wclear(), printw(),
        /       altercoordinates(), infloat(), waddstr(), wrefresh(), mvprintw(), getans
        wer(),
        /       wclrtoeol(), wclrtobot()
        /
        / DESCRIPTION:
        /
        *************************************************************************/

        internal void Do_cloak(client_t c, int state, int force)
        {
            realm_object_t object_ptr = new realm_object_t(), object_ptr_ptr = new realm_object_t();
            event_t event_ptr = new event_t();
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];

            /* if this state is a change */
            if (state != (c.player.cloaked ? 1 : 0))
            {
                force = 1;

                /* if we're cloaking */
                if (state != 0)
                {

                    if (c.player.mana < phantdefs.MM_CLOAK)
                    {
                        ioclass.Do_send_line(c, "No mana left.\n");
                        ioclass.Do_more(c);
                        return;
                    }

                    else
                    {
                        c.player.cloaked = true;
                        c.player.mana -= phantdefs.MM_CLOAK;
                    }


                    miscclass.Do_lock_mutex(c.realm.realm_lock);
                    characterclass.Do_player_description(c);
                    miscclass.Do_unlock_mutex(c.realm.realm_lock);

                }
                else
                {

                    miscclass.Do_lock_mutex(c.realm.realm_lock);
                    /* check for realm objects in the new location */
                    object_ptr_ptr = c.realm.objects;

                    while (object_ptr_ptr != null)
                    {

                        if ((object_ptr_ptr).x == c.player.x && (object_ptr_ptr).y
                        == c.player.y)
                        {

                            object_ptr = object_ptr_ptr;

                            object_ptr_ptr = object_ptr.next_object;

                            event_ptr = eventclass.Do_create_event();

                            switch (object_ptr.type)
                            {

                                case phantdefs.CORPSE:
                                    event_ptr.type = (short)phantdefs.CORPSE_EVENT;

                                    /* the corpse can be cursed */
                                    event_ptr.arg1 = 1;
                                    event_ptr.arg4 = object_ptr.arg1;
                                    break;

                                case phantdefs.HOLY_GRAIL:

                                    event_ptr.type = (short)phantdefs.NULL_EVENT;

                                    /* don't give the grail to 
                                       a player who uncloaks on it */
                                    ioclass.Do_send_line(c, "You found the Grail!  But you were cloaked...\n");
                                    ioclass.Do_more(c);

                                    initclass.Do_hide_grail(c.realm, CFUNCTIONS.floor(c.player.level));
                                    break;

                                case phantdefs.ENERGY_VOID:
                                    event_ptr.type = (short)phantdefs.ENERGY_VOID_EVENT;
                                    break;

                                case phantdefs.TREASURE_TROVE:
                                    event_ptr.type = (short)phantdefs.TROVE_EVENT;

                                    initclass.Do_hide_trove(c.realm);
                                    break;

                                default:


                                    error_msg = CFUNCTIONS.sprintfSinglestring(
                              "[%s] encountered realm object of type %d in Do_cloak.\n",
                                  c.connection_id, object_ptr.type);


                                    fileclass.Do_log_error(error_msg);
                                    break;
                            }

                            if (event_ptr.type != phantdefs.NULL_EVENT)
                            {

                                eventclass.Do_file_event(c, event_ptr);
                            }
                            else
                            {

                                event_ptr = null; // free((void*) event_ptr);
                            }


                            object_ptr = null; // free((void*) object_ptr);
                        }
                        else
                        {
                            object_ptr_ptr = ((object_ptr_ptr).next_object);
                        }
                    }
                    miscclass.Do_unlock_mutex(c.realm.realm_lock);
                }
            }

            c.player.cloaked = state == 0 ? false : true;

            if (force != 0)
            {

                socketclass.Do_send_int(c, phantdefs.CLOAK_PACKET);

                socketclass.Do_send_bool(c, (short)(c.player.cloaked ? 1 : 0));
            }

            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_crowns(client_t c, int crowns)
        /
        / FUNCTION: do trading post stuff
        /
        / AUTHOR: Brian Kelly, 10/3/99
        /
        / ARGUMENTS: none
        /
        / RETURN VALUE: none
        /
        / MODULES CALLED: writerecord(), adjuststats(), Mathf.Abs(), more(), cfunctions.sqrt(),
        /       sleep(), cfunctions.floor(), wmove(), drandom(), wclear(), printw(),
        /       altercoordinates(), infloat(), waddstr(), wrefresh(), mvprintw(), getanswer(),
        /       wclrtoeol(), wclrtobot()
        /
        / DESCRIPTION:
        /
        *************************************************************************/

        internal void Do_crowns(client_t c, int crowns, int force)
        {
            bool crownFlag;

            crownFlag = macros.ANY(c.player.crowns) == 0 ? false : true;

            if (crowns != 0)
            {

                c.player.crowns = (short)(c.player.crowns + crowns);
                if (c.player.crowns < 0)
                {
                    c.player.crowns = 0;
                }

                if (crownFlag != (macros.ANY(c.player.crowns) == 0 ? false : true))
                {

                    crownFlag = !crownFlag;
                    force = 1;

                    miscclass.Do_lock_mutex(c.realm.realm_lock);
                    characterclass.Do_player_description(c);
                    miscclass.Do_unlock_mutex(c.realm.realm_lock);
                }
            }

            if (force != 0)
            {

                socketclass.Do_send_int(c, phantdefs.CROWN_PACKET);

                socketclass.Do_send_bool(c, (short)(crownFlag ? 1 : 0));
            }
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_blessing(client_t c, bool blessing)
        /
        / FUNCTION: do trading post stuff
        /
        / AUTHOR: Brian Kelly, 10/3/99
        /
        / ARGUMENTS: none
        /
        / RETURN VALUE: none
        /
        / MODULES CALLED: writerecord(), adjuststats(), Mathf.Abs(), more(), cfunctions.sqrt(),
        /       sleep(), cfunctions.floor(), wmove(), drandom(), wclear(), printw(),
        /       altercoordinates(), infloat(), waddstr(), wrefresh(), mvprintw(), getanswer(),
        /       wclrtoeol(), wclrtobot()
        /
        / DESCRIPTION:
        /
        *************************************************************************/

        internal void Do_blessing(client_t c, int blessing, int force)
        {
            if (blessing != (c.player.blessing ? 1 : 0) || force != 0)
            {


                socketclass.Do_send_int(c, phantdefs.BLESSING_PACKET);

                socketclass.Do_send_bool(c, (short)blessing);

                c.player.blessing = blessing == 0 ? false : true;
            }
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_palantir(client_t c, bool palantir, int force)
        /
        / FUNCTION: do trading post stuff
        /
        / AUTHOR: Brian Kelly, 10/3/99
        /
        / ARGUMENTS: none
        /
        / RETURN VALUE: none
        /
        / MODULES CALLED: writerecord(), adjuststats(), Mathf.Abs(), more(), cfunctions.sqrt(),
        /       sleep(), cfunctions.floor(), wmove(), drandom(), wclear(), printw(),
        /       altercoordinates(), infloat(), waddstr(), wrefresh(), mvprintw(), getanswer(),
        /       wclrtoeol(), wclrtobot()
        /
        / DESCRIPTION:
        /
        *************************************************************************/

        internal void Do_palantir(client_t c, int palantir, int force)
        {
            if (palantir != (c.player.palantir ? 1 : 0) || force != 0)
            {


                socketclass.Do_send_int(c, phantdefs.PALANTIR_PACKET);

                socketclass.Do_send_bool(c, (short)palantir);

                c.player.palantir = palantir == 0 ? false : true;
            }

            /* if no palantir, kick player out of the palantir channel */
            if ((c.player.palantir == false) &&
                (c.channel == 8))
            {

                c.channel = 1;

                c.game.hearAllChannels = phantdefs.HEAR_SELF;

                miscclass.Do_lock_mutex(c.realm.realm_lock);
                characterclass.Do_player_description(c);
                miscclass.Do_unlock_mutex(c.realm.realm_lock);

                characterclass.Do_send_specification(c, phantdefs.CHANGE_PLAYER_EVENT);
            }

        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_virgin(client_t c, bool virgin)
        /
        / FUNCTION: do trading post stuff
        /
        / AUTHOR: Brian Kelly, 10/3/99
        /
        / ARGUMENTS: none
        /
        / RETURN VALUE: none
        /
        / MODULES CALLED: writerecord(), adjuststats(), Mathf.Abs(), more(), cfunctions.sqrt(),
        /       sleep(), cfunctions.floor(), wmove(), drandom(), wclear(), printw(),
        /       altercoordinates(), infloat(), waddstr(), wrefresh(), mvprintw(), getanswer(),
        /       wclrtoeol(), wclrtobot()
        /
        / DESCRIPTION:
        /
        *************************************************************************/

        internal void Do_virgin(client_t c, int virgin, int force)
        {
            if (virgin != (c.player.virgin ? 1 : 0) || force != 0)
            {
                socketclass.Do_send_int(c, phantdefs.VIRGIN_PACKET);

                socketclass.Do_send_bool(c, (short)virgin);

                c.player.virgin = virgin == 0 ? false : true;
            }
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_ring(client_t c, int ring, int force)
        /
        / FUNCTION: do trading post stuff
        /
        / AUTHOR: Brian Kelly, 10/3/99
        /
        / ARGUMENTS: none
        /
        / RETURN VALUE: none
        /
        / MODULES CALLED: writerecord(), adjuststats(), Mathf.Abs(), more(), cfunctions.sqrt(),
        /       sleep(), cfunctions.floor(), wmove(), drandom(), wclear(), printw(),
        /       altercoordinates(), infloat(), waddstr(), wrefresh(), mvprintw(), getanswer(),
        /       wclrtoeol(), wclrtobot()
        /
        / DESCRIPTION:
        /
        *************************************************************************/

        internal void Do_ring(client_t c, int ring, int force)
        {
            bool ringFlag = false;

            switch (ring)
            {

                case phantdefs.R_NONE:

                    if (c.player.ring_type != phantdefs.R_NONE)
                    {
                        c.player.ring_type = phantdefs.R_NONE;
                        force = 1;
                    }
                    ringFlag = false;

                    break;

                case phantdefs.R_NAZREG:

                    if (c.player.ring_type == phantdefs.R_NONE)
                    {
                        c.player.ring_type = phantdefs.R_NAZREG;
                        c.player.ring_duration = CFUNCTIONS.floor(macros.RND() * macros.RND() * 150 + 1);
                        force = 1;
                    }
                    ringFlag = true;

                    break;

                case phantdefs.R_DLREG:

                    if (c.player.ring_type == phantdefs.R_NONE)
                    {
                        c.player.ring_type = phantdefs.R_DLREG;
                        c.player.ring_duration = 0;
                        force = 1;
                    }
                    ringFlag = true;

                    break;

                case phantdefs.R_BAD:

                    if (c.player.ring_type == phantdefs.R_NONE)
                    {
                        c.player.ring_type = phantdefs.R_BAD;

                        c.player.ring_duration = 15 +
                                    c.realm.charstats[c.player.type].ring_duration +
                                    (int)macros.ROLL(0, 5);

                        force = 1;
                    }
                    ringFlag = true;

                    break;

                case phantdefs.R_SPOILED:

                    if (c.player.ring_type == phantdefs.R_BAD)
                    {
                        c.player.ring_type = phantdefs.R_SPOILED;

                        c.player.ring_duration = CFUNCTIONS.floor(macros.ROLL(10.0, 25.0));
                    }
                    ringFlag = true;

                    break;
            }

            if (force != 0)
            {

                socketclass.Do_send_int(c, phantdefs.RING_PACKET);

                socketclass.Do_send_bool(c, (short)(ringFlag ? 1 : 0));
            }
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_charm(client_t c, int charms)
        /
        / FUNCTION: do trading post stuff
        /
        / AUTHOR: Brian Kelly, 10/3/99
        /
        / ARGUMENTS: none
        /
        / RETURN VALUE: none
        /
        / MODULES CALLED: writerecord(), adjuststats(), Mathf.Abs(), more(), cfunctions.sqrt(),
        /       sleep(), cfunctions.floor(), wmove(), drandom(), wclear(), printw(),
        /       altercoordinates(), infloat(), waddstr(), wrefresh(), mvprintw(), getanswer(),
        /       wclrtoeol(), wclrtobot()
        /
        / DESCRIPTION:
        /
        *************************************************************************/

        void Do_charm(client_t c, int charms)
        {
            /* make sure we're still under the maximum charms */
            c.player.charms = CFUNCTIONS.floor(CFUNCTIONS.MIN(c.player.charms + charms, c.player.level + 10.0));
        }


        /***************************************************************************
        / FUNCTION NAME: Do_age(client_t c)
        /
        / FUNCTION: Default activity when no events are pending
        /
        / AUTHOR:  Brian Kelly, 6/17/99
        /
        / ARGUMENTS:
        /       client_t c - structure containing all thread info
        /
        / RETURN VALUE: none
        /
        / DESCRIPTION:
        /       Process arguments, initialize program, and loop forever processing
        /       player input.
        /
        ****************************************************************************/

        internal void Do_age(client_t c)
        {
            event_t event_ptr = new event_t();
            double temp;

            /* take hit points if player has poison */
            if (c.player.poison > 0.0)
            {

                temp = c.player.poison * c.realm.charstats[c.player.type].weakness
                       * c.player.max_energy / 600.0;

                /* Removed bit that prevents energy loss for large poison values
                        if (c.player.energy > c.player.max_energy / 10.0 && temp + 5.0 <
                                c.player.energy) {
                */
                if (c.player.energy > (c.player.max_energy + c.player.shield)
                / 10.0)
                {

                    /* Thrown in so sick energy can't drop below 10% */
                    temp = CFUNCTIONS.MIN(temp, c.player.energy - (c.player.max_energy +

                        c.player.shield) / 10.0);


                    Do_energy(c, c.player.energy - temp, c.player.max_energy,
                        c.player.shield, c.battle.force_field, 0);
                }
            }

            /* if cloaked, remove mana */
            if (c.player.cloaked)
            {

                if ((c.player.circle == 27) || (c.player.circle == 28))
                {
                    temp = CFUNCTIONS.floor(.02 * (1000 + c.player.level *
                        c.realm.charstats[c.player.type].max_mana));
                }
                else if ((c.player.circle > 24) && (c.player.circle < 31))
                {
                    temp = CFUNCTIONS.floor(.01 * (1000 + c.player.level *
                        c.realm.charstats[c.player.type].max_mana));
                }
                else if ((c.player.circle > 19) && (c.player.circle < 36))
                {
                    temp = CFUNCTIONS.floor(.005 * (1000 + c.player.level *
                        c.realm.charstats[c.player.type].max_mana));
                }
                else
                {
                    temp = c.player.circle;
                }

                if (c.player.mana >= temp)

                    Do_mana(c, -temp, 0);
                else
                {

                    ioclass.Do_send_line(c,
                        "You do not have enough mana to sustain your cloak.\n");

                    /* ran out of mana, uncloak */
                    event_ptr = eventclass.Do_create_event();
                    event_ptr.type = (short)phantdefs.CLOAK_EVENT;
                    eventclass.Do_handle_event(c, event_ptr);
                }
            }


            /* Check if the Knight should stick around */
            if (c.player.special_type == phantdefs.SC_KNIGHT && c.realm.king == null)
            {

                commandsclass.Do_dethrone(c);
            }


            /* Count out aging every other turn if you are holding the One Ring */
            if (c.player.ring_type == phantdefs.R_DLREG)
            {

                c.ageCount++;

                if (c.ageCount == 2)
                {
                    c.player.age++;
                    c.ageCount = 0;

                }
                else
                {

                    c.player.age++;

                }
            }
            else
            {
                c.player.age++;
                c.morgothCount--;
                if (c.morgothCount < 0)
                {
                    c.morgothCount = 0;
                }
            }

            while (c.player.age / phantdefs.N_AGE > c.player.degenerated && (c.wizard < 3))
            {

                commandsclass.Do_degenerate(c, 1);
            }
        }


        /***************************************************************************
        / FUNCTION NAME: Do_poison_modifier(client_t c, float poison)
        /
        / FUNCTION: Default activity when no events are pending
        /
        / AUTHOR:  Brian Kelly, 5/6/01
        /
        / ARGUMENTS:
        /       client_t c - structure containing all thread info
        /
        / RETURN VALUE: none
        /
        / DESCRIPTION:
        /       Process arguments, initialize program, and loop forever processing
        /       player input.
        /
        ****************************************************************************/

        internal void Do_adjusted_poison(client_t c, double poison)
        {
            double dtemp;

            dtemp = c.player.circle - 17;

            poison *= (macros.SGN(dtemp) * CFUNCTIONS.pow(Mathf.Abs((float)dtemp), .33) + 5.52) / 3;

            Do_poison(c, poison);
        }


        /***************************************************************************
        / FUNCTION NAME: Do_poison(client_t c, double poison)
        /
        / FUNCTION: Default activity when no events are pending
        /
        / AUTHOR:  Brian Kelly, 10/10/99
        /
        / ARGUMENTS:
        /       client_t c - structure containing all thread info
        /
        / RETURN VALUE: none
        /
        / DESCRIPTION:
        /       Process arguments, initialize program, and loop forever processing
        /       player input.
        /
        ****************************************************************************/

        internal void Do_poison(client_t c, double poison)
        {
            if (poison != 0)
            {
                c.player.poison += (float)poison;

                if (c.player.poison < 1e-4)
                {
                    c.player.poison = 0;
                }


                Do_strength(c, c.player.max_strength, c.player.sword,
                    c.battle.strengthSpell, 0);

            }
        }


        /***************************************************************************
        / FUNCTION NAME: Do_sin(client_t c, float *sin)
        /
        / FUNCTION: Default activity when no events are pending
        /
        / AUTHOR:  Brian Kelly, 06/15/01
        /
        / ARGUMENTS:
        /       client_t c - structure containing all thread info
        /
        / RETURN VALUE: none
        /
        / DESCRIPTION:
        /       Process arguments, initialize program, and loop forever processing
        /       player input.
        /
        ****************************************************************************/

        internal void Do_sin(client_t c, double sin)
        {
            event_t event_ptr = new event_t();

            if (CFUNCTIONS.isnan(c.player.sin))
            {
                c.player.sin = 0;
            }

            /* Doubles any ammount of sin you take in while carrying the one ring */
            if (c.player.ring_type == phantdefs.R_DLREG)
            {
                c.player.sin += (float)sin * 2;

            }
            else
            {
                c.player.sin += (float)sin;
            }

            if (c.player.sin < 0.0)
            {
                c.player.sin = 0;
            }

            if (c.player.blessing && c.player.sin > (1500.0 + c.player.level) /
                (c.player.level + 30.0))
            {

                ioclass.Do_send_line(c,
                   "Your blessing is consumed by the evil of your actions!\n");

                ioclass.Do_more(c);
                ioclass.Do_send_clear(c);


                Do_blessing(c, 0, 0);
            }


            if (c.player.sin > 25.0 + macros.RND() * 25.0)
            {
                event_ptr = (event_t)eventclass.Do_create_event();
                event_ptr.type = (short)phantdefs.DEATH_EVENT;
                event_ptr.arg3 = phantdefs.K_SIN;
                eventclass.Do_file_event(c, event_ptr);
            }
            else if ((c.player.sin > 20.0) && (macros.RND() < .2))
            {
                ioclass.Do_send_line(c,
                   "You cackle gleefully at the chaos you are causing!\n");

                ioclass.Do_more(c);
                ioclass.Do_send_clear(c);
            }

        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_book(client_t c, int bookCount)
        /
        / FUNCTION: initialize a character
        /
        / AUTHOR: E. A. Estes, 12/4/85
        /         Brian Kelly, 5/4/99
        /
        / ARGUMENTS:
        /       struct player *playerp - pointer to structure to init
        /
        / RETURN VALUE: none
        /
        / MODULES CALLED: cfunctions.floor(), drandom()
        /
        / DESCRIPTION:
        /       Put a bunch of default values in the given structure.
        /
        *************************************************************************/

        internal void Do_book(client_t c, int bookCount)
        {
            int i, multiple;
            float ftemp;
            double dtemp1, dtemp2;

            for (i = 0; i < bookCount; i++)
            {

                ftemp = (float)(c.player.brains - c.player.level *
                        c.realm.charstats[c.player.type].max_brains);

                multiple = CFUNCTIONS.ceil(ftemp / (c.player.level *
                                c.realm.charstats[c.player.type].max_brains));

                dtemp1 = c.realm.charstats[c.player.type].brains.increase;

                /* make it really hard to get more than max brains */
                if (ftemp > 0)
                {

                    if (multiple > 0)
                    {
                        dtemp2 = dtemp1 * (dtemp1 - 1) / multiple;
                    }
                    else
                    {
                        dtemp2 = dtemp1 * (dtemp1 - 1);
                    }

                    c.player.brains += dtemp2 / CFUNCTIONS.sqrt(ftemp + dtemp2);
                }
                else
                {
                    c.player.brains += dtemp1 / 2;
                }
            }
        }
        /************************************************************************
        /
        / FUNCTION NAME: Do_forceage(struct client_t t, double Degens)
        /
        / FUNCTION: forcefully age a player
        /
        / AUTHOR: Arella Kirstar 7/4/02
        /
        / ARGUMENTS: none
        /
        / RETURN VALUE: none
        /
        / MODULES CALLED: death(), cfunctions.floor(), wmove(), drandom(), waddstr(), explevel()
        /
        / GLOBAL INPUTS: Player, *stdscr, *Statptr, Stattable[]
        /
        / GLOBAL OUTPUTS: Player, Changed
        /
        / DESCRIPTION:
        /       Used to forcefully add degenerations and age to a player
        /
        *************************************************************************/

        void Do_forceage(client_t c, double Degens)
        {
            event_t event_ptr = new event_t();
            //string string_buffer = ""; //[phantdefs.SZ_LINE];

            /* add the degens and age */
            c.player.degenerated = CFUNCTIONS.floor(Degens);
            c.player.age = CFUNCTIONS.floor(Degens) * 750;

        }


    }

}
