using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace phantasiaclasses
{
    public class treasure //: MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        static treasure Instance;
        private treasure()
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
            
            miscclass = misc.GetInstance();
            fileclass = file.GetInstance();
            restoreclass = restore.GetInstance();
            statsclass = stats.GetInstance();
            infoclass = info.GetInstance();
            ioclass = io.GetInstance();
            commandsclass = commands.GetInstance();
            socketclass = socket.GetInstance();
            itcombatclass = itcombat.GetInstance();
            fightclass = fight.GetInstance();
            tagsclass = tags.GetInstance();
            eventclass = eventsrc.GetInstance();
            hackclass = hack.GetInstance();
            characterclass = character.GetInstance();
            accountclass = account.GetInstance();
            initclass = init.GetInstance();
        }
        public static treasure GetInstance()
        {
            treasure instance = null;
            if (Instance != null)
            {
                instance = Instance;
            }
            else
            {
                instance = new treasure();
            }
            return instance;
        }

        phantasiaclasses.misc miscclass;
        phantasiaclasses.file fileclass;
        phantasiaclasses.restore restoreclass;
        phantasiaclasses.stats statsclass;
        phantasiaclasses.info infoclass;
        phantasiaclasses.io ioclass;
        phantasiaclasses.commands commandsclass;
        phantasiaclasses.socket socketclass;
        phantasiaclasses.itcombat itcombatclass;
        phantasiaclasses.fight fightclass;
        phantasiaclasses.tags tagsclass;
        phantasiaclasses.eventsrc eventclass;
        phantasiaclasses.hack hackclass;
        phantasiaclasses.character characterclass;
        phantasiaclasses.account accountclass;
        phantasiaclasses.init initclass;


        /************************************************************************
        /
        / FUNCTION NAME: Do_treasure(struct client_c *c)
        /
        / FUNCTION: select a treasure
        /
        / AUTHOR: E. A. Estes, 12/4/85
        /	  Brian Kelly, 6/17/99
        /
        / ARGUMENTS: none
        /
        / RETURN VALUE: none
        /
        / MODULES CALLED: pickmonster(), collecttaxes(), more(), cursedtreasure(),
        /       cfunctions.floor(), wmove(), drandom(), sscanf(), printw(), altercoordinates(),
        /       longjmp(), infloat(), waddstr(), getanswer(), getstring(), wclrtobot()
        /
        / GLOBAL INPUTS: Somebetter[], Curmonster, Whichmonster, Circle, Player,
        /       *stdscr, Databuf[], *Statptr, Fightenv[]
        /
        / GLOBAL OUTPUTS: Whichmonster, Shield, Player
        /
        / DESCRIPTION:
        /       Roll up a treasure based upon monster type and size, and
        /       certain player statistics.
        /       Handle cursed treasure.
        /
        *************************************************************************/

        internal void Do_treasure(client_t c, event_t the_event)
        {
            event_t event_ptr = new event_t();
            int whichtreasure;          /* calculated treasure to grant */
            long theAnswer = 0;
            int itemp = 0;
            char ch;                                /* input */
            double gold = 0.0;                     /* gold awarded */
            double gems = 0.0;                     /* gems awarded */
            double dtemp = 0, x_loc = 0, y_loc = 0;            /* for temporary calculations */
            string string_buffer = ""; //[phantdefs.SZ_LINE], 
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            float ftemp = 0;
            realm_object_t object_ptr = new realm_object_t();
            button_t buttons = new button_t();
            string[] druidmesg =
                {
                    "\tA blessing works perfectly if the bearer is free of any sin.\n",
                    "\tA palantir can even pierce a cloak.\n",
                    "\tA smurf berry a day keeps the medic away.\n",
                    "\tA thaumaturgist can really put you in hot water!\n",
                    "\tAll or nothing is your best friend against higher-level players.\n",
                    "\tAmulets protect you from cursed treasure.\n",
                    "\tBe careful to keep your sin low, or you may find yourself having a smurfy time.\n",
                    "\tBe sure to rest if your speed drops from fatigue.\n",
                    "\tBeware the Cracks of Doom!\n",
                    "\tBeware the Jabberwock, my friend!  The jaws that bite, the claws that catch!\n",
                    "\tBlindness wears off eventually... eventually.\n",
                    "\tBuy amulets to protect your charms from being used on treasure.\n",
                    "\tCatching a unicorn requires the virtue and control of a saint.\n",
                    "\tDo not meddle in the affairs of the Game Wizards, for they are subtle and quick to anger.\n",
                    "\tDo not ask to speak to a Game Wizard without giving your reason up front, or he will ignore you.\n",
                    "\tDon't beg the Game Wizards for help with the game.\n",
                    "\tDon't swear on channel 1.  Acronyms count, too.\n",
                    "\tDwarves regenerate energy faster than other characters.\n",
                    "\tElves are immune to the plague!\n",
                    "\tExperimentos have been known to come back from the dead!\n",
                    "\tFighting the Dark Lord leads to certain death...unless it's a Mimic!\n", "\tHalflings are said to be extremely lucky.\n",
                    "\tIf the game isn't acting the way it should, report it to an Apprentice.\n",
                    "\tIf your speed drops a lot, get rid of some of the heavy gold you are carrying.\n",
                    "\tIt doesn't matter if you buy books one at a time or all at once.\n",
                    "\tIt is impossible to teleport into the Dead Marshes except via the eagle Gwaihir.\n",
                    "\tIt is very hard to walk through the Dead Marshes.\n",
                    "\tKeep moving farther out, or you'll die from old age.\n",
                    "\tListen to the Apprentices, they speak with the voice of the Game Wizards.\n",
                    "\tMedics don't like liars, and punish accordingly.\n",
                    "\tMedics don't work for charity.\n",
                    "\tMerchants don't like players sleeping on their posts.\n",
                    "\tOnly a moron would fight morons for experience.\n",
                    "\tOnly a fool would dally with a Succubus or Incubus.\n",
                    "\tOnly the Steward can give gold, but beware their smurfs if you ask too many times!\n",
                    "\tParalyze can be a slow character's best friend.\n",
                    "\tPicking up treasure while poisoned is bad for your health.\n",
                    "\tReading the rules is a very good idea, and is often rewarded.\n",
                    "\tRings of power contain much power, but much peril as well.\n",
                    "\tSaintly adventurers may not have as much fun, but they tend to live longer.\n",
                    "\tSmurfs may look silly, but they are actually quite deadly.\n",
                    "\tStockpile amulets to protect your charms.\n",
                    "\tTeleports are more efficient over short distances.\n",
                    "\tThe corpse of a councilmember or a valar has never been found.\n",
                    "\tThe One Ring is most likely to be found in the Cracks of Doom.\n",
                    "\tThere are only three certainties in Phantasia : death, taxes, and morons.\n",
                    "\tThe Game Wizards are always right.\n",
                    "\tThe gods will revoke their blessing if you sin too much.\n",
                    "\tThe nastier your poison, the more gold a medic will want to cure it.\n",
                    "\tThere is a post in the Plateau of Gorgoroth that sells blessings.\n",
                    "\tWant to live dangerously?  Nick a shrieker.\n",
                    "\tWhen all else fails ... use all or nothing.\n",
                    "\tWhen starting, do not spend too much money at once, or you may be branded a thief.\n",
                    "\tWizards have been known to be nice if you are polite without being obsequious.\n",
                    };

            /* Gold and gems only in circles that aren't fully beyond */
            if ((the_event.arg2 == 10) ||
                ((c.player.circle <= phantdefs.D_BEYOND / 88.388 && macros.RND() > 0.65) &&
                 !(the_event.arg2 > 0)))
            {

                /* gold and gems */
                if (the_event.arg3 > 7)
                {

                    /* gems */
                    gems = CFUNCTIONS.floor(macros.ROLL(1.0, CFUNCTIONS.pow(the_event.arg3 - 7.0, 1.8) *
                    (the_event.arg1 - 1.0) / 4.0));

                    if (gems == 1)
                    {
                        string_buffer = CFUNCTIONS.sprintfSinglestring("You have discovered a gem!\n");

                        ioclass.Do_send_line(c, string_buffer);
                        ioclass.Do_send_line(c, "  Do you want to pick it up?\n");
                    }
                    else
                    {
                        string_buffer = CFUNCTIONS.sprintfSinglestring("You have discovered %.0lf gems!\n", gems);

                        ioclass.Do_send_line(c, string_buffer);
                        ioclass.Do_send_line(c, "  Do you want to pick them up?\n");
                    }
                }
                else
                {

                    /* gold */
                    gold = CFUNCTIONS.floor(macros.ROLL(the_event.arg3 * 10.0, the_event.arg3 *
                    the_event.arg3 * 8.0 * (the_event.arg1 - 1.0)));

                    /* avoid single gold pieces */
                    if (gold == 1.0)
                    {
                        gold = 2.0;
                    }

                    string_buffer = CFUNCTIONS.sprintfSinglestring("You have found %.0lf gold pieces.\n", gold);

                    ioclass.Do_send_line(c, string_buffer);
                    ioclass.Do_send_line(c, "  Do you want to pick them up?\n");

                }

                ioclass.Do_clear_buttons(buttons, 0);
                CFUNCTIONS.strcpy(ref buttons.button[5], "Yes\n");
                CFUNCTIONS.strcpy(ref buttons.button[6], "No\n");
                CFUNCTIONS.strcpy(ref buttons.button[7], "No to All\n");

                itemp = ioclass.Do_buttons(c, ref theAnswer, buttons);

                if (itemp != phantdefs.S_NORM || theAnswer == 7)
                {


                    eventclass.Do_orphan_events(c);
                    ioclass.Do_send_clear(c);
                    return;
                }
                else if (theAnswer == 6)
                {
                    ioclass.Do_send_clear(c);
                    return;
                }
                else if (theAnswer != 5)
                {

                    error_msg = CFUNCTIONS.sprintfSinglestring(
                            "[%s] Returned non-option %ld in Do_treasure.\n",
                            c.connection_id, theAnswer);

                    fileclass.Do_log_error(error_msg);
                    /*
                                hackclass.Do_caught_hack(c, phantdefs.H_SYSTEM);
                    */
                    return;
                }

                if (macros.RND() < the_event.arg3 / 35.0 + 0.04)
                {

                    /* cursed */
                    ioclass.Do_send_line(c, "They were cursed!\n");
                    Do_cursed_treasure(c);
                }
                else
                {

                    statsclass.Do_gold(c, gold, 0);

                    statsclass.Do_gems(c, gems, 0);
                }
            }
            else
            {
                /* other treasures */
                ioclass.Do_send_line(c,
                "You have found some treasure.  Do you want to inspect it?\n");

                ioclass.Do_clear_buttons(buttons, 0);
                CFUNCTIONS.strcpy(ref buttons.button[5], "Yes\n");
                CFUNCTIONS.strcpy(ref buttons.button[6], "No\n");
                CFUNCTIONS.strcpy(ref buttons.button[7], "No to All\n");

                itemp = ioclass.Do_buttons(c, ref theAnswer, buttons);

                if (itemp != phantdefs.S_NORM || theAnswer == 7)
                {


                    eventclass.Do_orphan_events(c);
                    ioclass.Do_send_clear(c);
                    return;
                }
                else if (theAnswer == 6)
                {
                    ioclass.Do_send_clear(c);
                    return;
                }
                else if (theAnswer != 5)
                {

                    error_msg = CFUNCTIONS.sprintfSinglestring(
                            "[%s] Returned non-option %ld in Do_treasure.\n",
                            c.connection_id, theAnswer);

                    fileclass.Do_log_error(error_msg);
                    /*
                                hackclass.Do_caught_hack(c, phantdefs.H_SYSTEM);
                    */
                    return;
                }

                if (macros.RND() < 0.08 && the_event.arg3 != 4)
                {
                    ioclass.Do_send_line(c, "It was cursed!\n");
                    Do_cursed_treasure(c);
                }
                else
                {

                    if ((the_event.arg2 > 0) && (the_event.arg2 < 4))
                    {
                        whichtreasure = (int)the_event.arg2;
                    }
                    else
                    {
                        whichtreasure = (int)macros.ROLL(1.0, 3.0);     /* pick a treasure */
                    }

                    switch (the_event.arg3)
                    {

                        case 1:     /* treasure type 1 */

                            switch (whichtreasure)
                            {

                                case 1:
                                    ioclass.Do_send_line(c, "You've found a vial of holy water!\n");

                                    ioclass.Do_more(c);
                                    ++c.player.holywater;
                                    break;

                                case 2:
                                    dtemp = CFUNCTIONS.floor(CFUNCTIONS.sqrt(the_event.arg1));

                                    if (dtemp < 1)
                                    {
                                        dtemp = 1;
                                    }

                                    if (dtemp == 1)
                                    {
                                        CFUNCTIONS.sprintf(ref string_buffer,
                                                "You've found an amulet of protection.\n");
                                    }
                                    else
                                    {
                                        CFUNCTIONS.sprintf(ref string_buffer,
                                                "You've found %.0lf amulets of protection.\n",
                                                dtemp);
                                    }

                                    ioclass.Do_send_line(c, string_buffer);

                                    ioclass.Do_more(c);
                                    c.player.amulets += CFUNCTIONS.floor(dtemp);

                                    break;

                                case 3:
                                    ioclass.Do_send_line(c,
                             "You have found a holy orb. You feel less sinful.\n");


                                    ioclass.Do_more(c);

                                    statsclass.Do_sin(c, -0.1);
                                    break;
                            }

                            /* end treasure type 1 */
                            break;

                        case 2:     /* treasure type 2 */

                            switch (whichtreasure)
                            {

                                case 1:
                                    if (c.player.sin < 9.5 * macros.RND() + 0.5)
                                    {

                                        ioclass.Do_send_line(c,
                          "You have encountered a druid who teaches you the following words of wisdom:\n");


                                        ioclass.Do_send_line(c, druidmesg[(int)macros.ROLL(0.0, 1 / 1)]);// (double)sizeof(druidmesg) / sizeof(char*))]); //todo


                                        ioclass.Do_more(c);


                                        statsclass.Do_experience(c, macros.ROLL(0.0, 2000.0 + the_event.arg1 *
                                            750.0), 0);
                                    }
                                    else
                                    {

                                        ioclass.Do_send_line(c,
                         "You have encountered a druid.  He runs in fear for his life!\n");


                                        ioclass.Do_more(c);
                                    }

                                    break;

                                case 2:
                                    dtemp = CFUNCTIONS.floor((.5 + macros.RND()) * 15 * the_event.arg1);

                                    string_buffer = CFUNCTIONS.sprintfSinglestring("You've found a +%.0lf buckler.\n",
                                                                dtemp);


                                    ioclass.Do_send_line(c, string_buffer);

                                    if (dtemp >= c.player.shield)
                                    {


                                        ioclass.Do_more(c);


                                        statsclass.Do_energy(c, c.player.energy - c.player.shield +

                                            dtemp, c.player.max_energy, dtemp, 0, 0);
                                    }
                                    else
                                    {

                                        ioclass.Do_send_line(c,
                                   "But you already have something better.\n");


                                        ioclass.Do_more(c);
                                    }
                                    break;

                                case 3:

                                    if (c.player.poison > 0.0)
                                    {


                                        statsclass.Do_adjusted_poison(c, -0.25);
                                        if (c.player.poison < 0.0)
                                        {
                                            c.player.poison = 0.0f;
                                            ioclass.Do_send_line(c,
                             "You've found some smurf berries!  You feel cured!\n");

                                        }
                                        else
                                        {
                                            ioclass.Do_send_line(c,
                             "You've found some smurf berries!  You feel slightly better.\n");
                                        }
                                    }
                                    else
                                    {

                                        ioclass.Do_send_line(c,
                             "You've found some smurf berries!  You feel smurfy!\n");

                                        c.battle.rounds /= 2;

                                        statsclass.Do_speed(c, c.player.max_quickness, c.player.quicksilver, 0, 0);
                                        statsclass.Do_energy(c,
                                        c.player.energy +
                                         (c.player.max_energy + c.player.shield) / 15.0 +
                                         c.player.level / 3.0 + 2.0,
                                        c.player.max_energy,
                                    c.player.shield, c.battle.force_field, 0);
                                    }


                                    ioclass.Do_more(c);

                                    break;
                            }

                            /* end treasure type 2 */
                            break;

                        case 3:     /* treasure type 3 */

                            switch (whichtreasure)
                            {

                                case 1:

                                    ioclass.Do_send_line(c,
                            "You've met a hermit!  You heal, gain mana, and lose sin.\n");


                                    ioclass.Do_more(c);

                                    if (c.player.sin > 6.66)
                                    {
                                        statsclass.Do_sin(c, -1.0);
                                    }
                                    else
                                    {

                                        statsclass.Do_sin(c, -0.15 * c.player.sin);
                                    }

                                    statsclass.Do_mana(c,
                                            c.realm.charstats[c.player.type].mana.increase
                                            / 2 * the_event.arg1, 0);
                                    statsclass.Do_energy(c,
                                        c.player.energy +
                                                                         (c.player.max_energy + c.player.shield) / 7.0 +
                                                                         c.player.level / 3.0 + 2.0,
                                                                        c.player.max_energy,
                                                                    c.player.shield, c.battle.force_field, 0);
                                    break;

                                case 2:


                                    Do_award_virgin(c);
                                    break;

                                case 3:
                                    dtemp = CFUNCTIONS.floor((.5 + macros.RND()) * the_event.arg1);

                                    if (dtemp < 1)
                                    {
                                        dtemp = 1;
                                    }

                                    CFUNCTIONS.sprintf(ref string_buffer, "You've found a +%.0lf short sword!\n", dtemp);


                                    ioclass.Do_send_line(c, string_buffer);
                                    if (dtemp >= c.player.sword)
                                    {


                                        ioclass.Do_more(c);


                                        statsclass.Do_strength(c, c.player.max_strength, dtemp, 0, 0);
                                    }
                                    else
                                    {

                                        ioclass.Do_send_line(c, "But you already have something better.\n");


                                        ioclass.Do_more(c);
                                    }
                                    break;
                            }

                            /* end treasure type 3 */
                            break;

                        case 4:     /* treasure type 4 */

                            if (c.player.blind)
                            {
                                ioclass.Do_send_line(c, "You've found a scroll.  Too bad you are blind!\n");

                                ioclass.Do_more(c);
                                break;
                            }



                            ioclass.Do_send_line(c, "You've found a scroll.  Will you read it?\n");

                            if (ioclass.Do_yes_no(c, ref theAnswer) != phantdefs.S_NORM || theAnswer == 1)
                            {

                                ioclass.Do_send_clear(c);
                                return;
                            }

                            if ((the_event.arg2 > 0) && (the_event.arg2 < 7))
                            {
                                dtemp = the_event.arg2;
                            }
                            else
                            {
                                dtemp = (int)macros.ROLL(1, 6);     /* pick a treasure */
                            }

                            if (dtemp == 1)
                            {
                                if (c.player.level <= 100 - c.player.gems)
                                {
                                    Do_treasure_map(c);
                                }
                                else
                                {
                                    /* character is too high level, pick another scroll */
                                    dtemp = (int)macros.ROLL(2, 5);
                                }
                            }


                            switch ((int)dtemp)
                            {

                                case 2:

                                    ioclass.Do_send_line(c,
                                  "It throws up a shield for your next monster.\n");


                                    ioclass.Do_more(c);
                                    ++c.player.shield_nf;
                                    break;

                                case 3:

                                    ioclass.Do_send_line(c,
                                 "It makes you faster for your next monster.\n");


                                    ioclass.Do_more(c);
                                    ++c.player.haste_nf;
                                    break;

                                case 4:

                                    ioclass.Do_send_line(c,
                            "It increases your strength for your next monster.\n");


                                    ioclass.Do_more(c);
                                    ++c.player.strong_nf;
                                    break;


                                case 5:

                                    ioclass.Do_send_line(c,
                                 "It tells you how to pick your next monster.\n");


                                    ioclass.Do_more(c);
                                    event_ptr = eventclass.Do_create_event();
                                    event_ptr.type = (short)phantdefs.MONSTER_EVENT;
                                    event_ptr.arg1 = phantdefs.MONSTER_SPECIFY;
                                    if (ioclass.Do_long_dialog(c, ref event_ptr.arg3,
                                "Which monster do you want [0-99]?\n") != 0)
                                    {


                                        event_ptr = null; // free((void*) event_ptr);
                                        break;
                                    }

                                    if (event_ptr.arg3 >= 0 && event_ptr.arg3 <
                                        phantdefs.NUM_MONSTERS)
                                    {


                                        eventclass.Do_file_event(c, event_ptr);
                                    }

                                    break;

                                case 6:
                                    ioclass.Do_send_line(c, "It was cursed!\n");
                                    Do_cursed_treasure(c);
                                    break;

                            }
                            /* end treasure type 4 */
                            break;

                        case 5:     /* treasure type 5 */

                            switch (whichtreasure)
                            {

                                case 1:

                                    ioclass.Do_send_line(c,
                          "You've discovered a power booster!  Your mana increases.\n");


                                    ioclass.Do_more(c);

                                    statsclass.Do_mana(c,
                                            c.realm.charstats[c.player.type].mana.increase
                                      * 2 * the_event.arg1, 0);
                                    break;

                                case 2:

                                    dtemp = CFUNCTIONS.floor((.5 + macros.RND()) * 50.0 * the_event.arg1);

                                    string_buffer = CFUNCTIONS.sprintfSinglestring("You've found a +%.0lf shield!\n",
                                                                dtemp);


                                    ioclass.Do_send_line(c, string_buffer);
                                    if (dtemp >= c.player.shield)
                                    {


                                        ioclass.Do_more(c);


                                        statsclass.Do_energy(c, c.player.energy - c.player.shield +

                                            dtemp, c.player.max_energy, dtemp, 0, 0);
                                    }
                                    else
                                    {

                                        ioclass.Do_send_line(c,
                                    "But you already have something better.\n");


                                        ioclass.Do_more(c);
                                    }
                                    break;

                                case 3:

                                    if (c.player.poison > 0.0)
                                    {

                                        statsclass.Do_adjusted_poison(c, -1.0);
                                        ioclass.Do_send_line(c, "You've discovered some lembas!  You feel much better!\n");
                                        if (c.player.poison < 0.0)
                                        {
                                            c.player.poison = 0.0f;
                                        }

                                        c.battle.rounds = 0;

                                        statsclass.Do_speed(c, c.player.max_quickness, c.player.quicksilver, 0, 0);
                                    }
                                    else
                                    {
                                        ioclass.Do_send_line(c, "You've discovered some lembas!  You feel energetic!\n");
                                        c.battle.rounds = 0;

                                        statsclass.Do_speed(c, c.player.max_quickness, c.player.quicksilver, 0, 0);
                                    }

                                    statsclass.Do_energy(c,
                                        c.player.energy +
                                         (c.player.max_energy + c.player.shield) / 3.0 +
                                         c.player.level / 3.0 + 2.0,
                                        c.player.max_energy,
                                    c.player.shield, c.battle.force_field, 0);


                                    ioclass.Do_more(c);

                                    break;
                            }

                            /* end treasure type 5 */
                            break;

                        case 6:     /* treasure type 6 */

                            switch (whichtreasure)
                            {

                                case 1:

                                    if (c.player.blind)
                                    {
                                        ioclass.Do_send_line(c, "You've discovered a tablet!  But you can't read it while blind!\n");
                                        ioclass.Do_more(c);
                                    }
                                    else
                                    {


                                        ioclass.Do_send_line(c, "You've discovered a tablet!  You feel smarter.\n");

                                        ioclass.Do_more(c);

                                        c.player.brains +=
                                                       c.realm.charstats[c.player.type].brains.increase
                                                       * (.75 + macros.RND() / 2) * (1 + the_event.arg1 / 10);
                                    }

                                    break;

                                case 2:

                                    if (c.player.sin * macros.RND() < 2.0)
                                    {

                                        ioclass.Do_send_line(c,
                            "You have come upon Treebeard!  He gives you some miruvor and you feel tougher!\n");


                                        ioclass.Do_more(c);

                                        dtemp =
                                                        c.realm.charstats[c.player.type].energy.increase
                                                        * (.75 + macros.RND() / 2) * (1 + the_event.arg1 / 5);


                                        statsclass.Do_energy(c,
                                                        c.player.max_energy + dtemp + c.player.shield,
                                            c.player.max_energy + dtemp, c.player.shield,
                                        0, 0);
                                    }
                                    else
                                    {

                                        ioclass.Do_send_line(c,
                     "You have come upon Treebeard! 'Hoom, hom!' he growls, and leaves.\n");


                                        ioclass.Do_more(c);
                                    }

                                    break;

                                case 3:

                                    dtemp = CFUNCTIONS.floor((.5 + macros.RND()) * 2 * the_event.arg1);

                                    CFUNCTIONS.sprintf(ref string_buffer,
                                                                "You've found a +%.0lf long sword!\n", dtemp);


                                    ioclass.Do_send_line(c, string_buffer);
                                    if (dtemp >= c.player.sword)
                                    {


                                        ioclass.Do_more(c);


                                        statsclass.Do_strength(c, c.player.max_strength, dtemp, 0,
                                            0);
                                    }
                                    else
                                    {

                                        ioclass.Do_send_line(c,
                                "But you already have something better.\n");


                                        ioclass.Do_more(c);
                                    }
                                    break;
                            }

                            /* end treasure type 6 */
                            break;

                        case 7:     /* treasure type 7 */

                            switch (whichtreasure)
                            {

                                case 1:

                                    if (c.player.sin < 3 * macros.RND() / 5)
                                    {
                                        ioclass.Do_send_line(c,
                                    "You've found an Aes Sedai.  She bows and completely cleanses you of the taint.\n");


                                        ioclass.Do_more(c);


                                        statsclass.Do_sin(c, -(c.player.sin));

                                    }
                                    else
                                    {
                                        ioclass.Do_send_line(c,
                                    "You've found an Aes Sedai.  She sniffs loudly and cleanses some of your taint.\n");

                                        ioclass.Do_more(c);

                                        if (c.player.sin > 4.5)
                                        {
                                            statsclass.Do_sin(c, -1.5);
                                        }
                                        else
                                        {

                                            statsclass.Do_sin(c, -0.33 * c.player.sin);
                                        }
                                    }


                                    break;

                                case 2:

                                    if (c.player.sin * macros.RND() < 2.5)
                                    {

                                        ioclass.Do_send_line(c,
                   "You release Gwaihir and he offers you a ride.  Do you wish to go anywhere?\n");

                                        if (ioclass.Do_yes_no(c, ref theAnswer) != phantdefs.S_NORM || theAnswer ==
                                1)
                                        {

                                            break;
                                        }


                                        event_ptr = eventclass.Do_create_event();
                                        event_ptr.type = (short)phantdefs.TELEPORT_EVENT;
                                        event_ptr.arg2 = 1;
                                        event_ptr.arg3 = 0;

                                        eventclass.Do_handle_event(c, event_ptr);
                                        break;
                                    }
                                    else
                                    {

                                        ioclass.Do_send_line(c,
                       "You release Gwaihir!  He thanks you for his freedom and flies off.\n");


                                        ioclass.Do_more(c);
                                    }
                                    break;

                                case 3:

                                    if (c.player.sin * macros.RND() < 2.0)
                                    {

                                        ioclass.Do_send_line(c,
                                "You have come upon Hercules!  He improves your strength and experience!\n");

                                        ioclass.Do_more(c);


                                        statsclass.Do_strength(c, c.player.max_strength +

                                      c.realm.charstats[c.player.type].strength.increase
                                      * (.75 + macros.RND() / 2) * (1 + the_event.arg1 / 5),
                                      c.player.sword, 0, 0);


                                        statsclass.Do_experience(c, 10000 * c.player.circle, 0); // added force (forces levelup if 1)
                                    }
                                    else
                                    {


                                        ioclass.Do_send_line(c,
                              "You have come upon Hercules!  He kicks sand in your face and walks off.\n");


                                        ioclass.Do_more(c);
                                    }

                                    break;
                            }

                            /* end treasure type 7 */
                            break;

                        case 8:     /* treasure type 8 */

                            switch (whichtreasure)
                            {

                                case 1:
                                    ioclass.Do_send_line(c,
                 "You've discovered some athelas!  You inhale its fragrance and feel wonderful!\n");


                                    ioclass.Do_more(c);


                                    /* remove blindness */
                                    if (c.player.blind)
                                    {
                                        c.player.blind = false;
                                    }

                                    /* zero out poison */
                                    if (c.player.poison > 0.0)
                                    {

                                        statsclass.Do_poison(c, (double)-c.player.poison);
                                    }

                                    /* make player immune to next plague at this circle */
                                    statsclass.Do_adjusted_poison(c, -1.0);


                                    /* heal the player completely and remove fatigue */
                                    statsclass.Do_energy(c, c.player.max_energy + c.player.shield,
                                        c.player.max_energy, c.player.shield, 0, 0);

                                    c.battle.rounds = 0;

                                    statsclass.Do_speed(c, c.player.max_quickness, c.player.quicksilver, 0, 0);
                                    break;

                                case 2:

                                    if (c.player.sin * macros.RND() < 1.0)
                                    {

                                        ioclass.Do_send_line(c, "You have encountered Merlyn!  He teaches you magic.\n");

                                        ioclass.Do_more(c);

                                        c.player.magiclvl +=
                                                      c.realm.charstats[c.player.type].magiclvl.increase
                                                      * (.75 + macros.RND() / 2) * (1 + the_event.arg1 / 10);

                                        statsclass.Do_mana(c,
                                            c.realm.charstats[c.player.type].mana.increase
                                            * the_event.arg1, 0);
                                    }
                                    else
                                    {


                                        ioclass.Do_send_line(c,
                                        "You have encountered Merlyn! He frowns and teleports off.\n");


                                        ioclass.Do_more(c);
                                    }

                                    break;


                                case 3:

                                    dtemp = (.75 + macros.RND() / 2) * .8 * (the_event.arg1 - 9);

                                    if (dtemp < 1)
                                    {
                                        dtemp = 1;
                                    }

                                    CFUNCTIONS.sprintf(ref string_buffer,
                              "You have discovered some +%.0lf quicksilver!\n", dtemp);


                                    ioclass.Do_send_line(c, string_buffer);
                                    if (dtemp >= c.player.quicksilver)
                                    {

                                        ioclass.Do_more(c);

                                        statsclass.Do_speed(c, c.player.max_quickness, dtemp, 0, 0);
                                    }
                                    else
                                    {

                                        ioclass.Do_send_line(c,
                                       "But you already have something better.\n");


                                        ioclass.Do_more(c);
                                    }
                                    break;
                            }

                            /* end treasure type 8 */
                            break;

                        case 10:
                        case 11:
                        case 12:
                        case 13:
                        case 14:    /* treasure types 10 - 14 */
                            if (macros.RND() < 0.33 || the_event.arg2 == 4)
                            {

                                /* Ungoliant treasure */
                                if (the_event.arg3 == 12)
                                {

                                    ioclass.Do_send_line(c,
                              "You've found a Silmaril!  Its light quickens your step!\n");


                                    ioclass.Do_more(c);

                                    if (c.player.max_quickness <
                                        c.realm.charstats[c.player.type].quickness.statbase +
                                        c.realm.charstats[c.player.type].quickness.interval)
                                    {
                                        statsclass.Do_speed(c, c.player.max_quickness + 2.0,
                            c.player.quicksilver, 0, 0);
                                    }
                                    else
                                    {
                                        statsclass.Do_speed(c, c.player.max_quickness + 1.0,
                            c.player.quicksilver, 0, 0);
                                    }

                                    break;
                                }

                                /* Saruman treasure */
                                else if (the_event.arg3 == 11)
                                {
                                    if (!c.player.palantir)
                                    {

                                        ioclass.Do_send_line(c,
                                "You've acquired Saruman's palantir.\n");


                                        ioclass.Do_more(c);
                                        statsclass.Do_palantir(c, 1, 0);
                                        break;
                                    }
                                    else
                                    {

                                        ioclass.Do_send_line(c,
                                                "You've rescued Gandalf!  He heals you, and casts some spells on you for your next combat!\n");

                                        ioclass.Do_more(c);


                                        statsclass.Do_energy(c, c.player.max_energy + c.player.shield,
                                        c.player.max_energy, c.player.shield, 0, 0);

                                        c.player.blind = false;

                                        c.battle.rounds = 0;

                                        statsclass.Do_speed(c, c.player.max_quickness, c.player.quicksilver, 0, 0);

                                        if (c.player.sin < macros.RND())
                                        {
                                            ++c.player.strong_nf;
                                        }

                                        if (c.player.sin < macros.RND())
                                        {
                                            ++c.player.haste_nf;
                                        }

                                        if (c.player.sin < macros.RND())
                                        {
                                            ++c.player.shield_nf;
                                        }

                                        if (c.player.sin * 2 < macros.RND())
                                        {

                                            event_ptr = eventclass.Do_create_event();
                                            event_ptr.type = (short)phantdefs.MONSTER_EVENT;
                                            event_ptr.arg1 = phantdefs.MONSTER_SPECIFY;
                                            if (ioclass.Do_long_dialog(c, ref event_ptr.arg3,
                                    "Which monster do you want [0-99]?\n") != 0)
                                            {


                                                event_ptr = null; // free((void*) event_ptr);
                                                break;
                                            }

                                            if (event_ptr.arg3 > 0 && event_ptr.arg3 <
                                                phantdefs.NUM_MONSTERS)
                                            {


                                                eventclass.Do_file_event(c, event_ptr);
                                            }
                                        }

                                        break;
                                    }
                                }

                                /* Nazgul treasure */
                                else if (c.player.ring_type == phantdefs.R_NONE
                                        && c.player.special_type < phantdefs.SC_COUNCIL
                                        && the_event.arg3 == 10)
                                {

                                    ioclass.Do_send_line(c,
                          "You've discovered a ring of power.  Will you pick it up?\n");

                                    if (ioclass.Do_yes_no(c, ref theAnswer) != phantdefs.S_NORM || theAnswer ==
                            1)
                                    {

                                        break;
                                    }

                                    /* roll up a ring */
                                    if (macros.RND() < 0.8)
                                    {

                                        /* regular ring */
                                        statsclass.Do_ring(c, phantdefs.R_NAZREG, 0);
                                    }
                                    else
                                    {

                                        /* bad ring */
                                        statsclass.Do_ring(c, phantdefs.R_BAD, 0);
                                    }
                                    break;
                                }

                                /* Dark Lord treasure */
                                else if (the_event.arg3 == 14)
                                {

                                    /* drop a ring */
                                    if (c.player.special_type < phantdefs.SC_KING
                                        && the_event.arg1 > 200
                                        && c.player.ring_type != phantdefs.R_DLREG)
                                    {

                                        statsclass.Do_sin(c, macros.RND());
                                        ioclass.Do_send_line(c,
                          "You've discovered The One Ring, the Ring to Rule Them All.  Will you pick it up?\n");

                                        if (ioclass.Do_yes_no(c, ref theAnswer) != phantdefs.S_NORM || theAnswer
                            == 1)
                                        {

                                            break;
                                        }

                                        if (c.player.ring_type != phantdefs.R_NONE)
                                        {

                                            ioclass.Do_send_line(c, "Your old ring no longer feels very precious to you and you throw it away.\n");
                                            ioclass.Do_more(c);
                                        }

                                        statsclass.Do_ring(c, phantdefs.R_NONE, 0);


                                        /* roll up a ring */
                                        if (macros.RND() < 0.8)
                                        {

                                            /* regular ring */
                                            statsclass.Do_ring(c, phantdefs.R_DLREG, 0);
                                        }
                                        else
                                        {

                                            /* bad ring */
                                            statsclass.Do_ring(c, phantdefs.R_BAD, 0);
                                        }
                                        break;
                                    }
                                    else if (!c.player.palantir)
                                    {


                                        ioclass.Do_send_line(c,
                            "You've acquired The Dark Lord's palantir.\n");


                                        ioclass.Do_more(c);
                                        statsclass.Do_palantir(c, 1, 0);
                                        break;
                                    }
                                    else if (c.player.special_type < phantdefs.SC_KING)
                                    {
                                        ioclass.Do_send_line(c,
                                          "You've discovered a ring of power.  Will you pick it up?\n");

                                        if (ioclass.Do_yes_no(c, ref theAnswer) != phantdefs.S_NORM || theAnswer ==
                                                1)
                                        {

                                            break;
                                        }

                                        /* roll up a ring */
                                        if (macros.RND() < 0.95)
                                        {

                                            /* regular ring */
                                            statsclass.Do_ring(c, phantdefs.R_NAZREG, 0);
                                        }
                                        else
                                        {

                                            /* bad ring */
                                            statsclass.Do_ring(c, phantdefs.R_BAD, 0);
                                        }
                                        break;
                                    }

                                }
                            }

                            /* end treasure types 10 - 13 */
                            /* fall through to treasure type 9 if no treasure from above */
                            goto case 9;
                        case 9: /* treasure type 9 */

                            switch (whichtreasure)
                            {

                                /* staff, volcano, smith */
                                case 1:

                                    if ((c.player.level < phantdefs.MAX_STEWARD) &&
                                        (c.player.level > 10) &&
                                        (c.player.crowns < 1) &&
                                        (c.player.special_type != phantdefs.SC_STEWARD) &&
                                        (macros.CALCLEVEL(c.player.experience) == c.player.level)
                                       )
                                    {

                                        ioclass.Do_send_line(c, "You have found a staff!\n");


                                        ioclass.Do_more(c);

                                        statsclass.Do_crowns(c, 1, 0);
                                    }
                                    else if ((c.player.circle > 26) &&
                                             (c.player.circle < 29))
                                    {
                                        miscclass.Do_volcano(c);
                                    }
                                    else
                                    {

                                        Do_smith(c, the_event);
                                    }

                                    break;

                                case 2:
                                    ioclass.Do_send_line(c, "You've received the blessing of the Valar for your heroism!\n");
                                    ioclass.Do_more(c);
                                    Do_award_blessing(c);
                                    break;

                                /* fall through otherwise */

                                case 3:
                                    dtemp = CFUNCTIONS.floor(macros.ROLL(CFUNCTIONS.floor(2 * CFUNCTIONS.sqrt(the_event.arg1)),
                                                               the_event.arg3 * .5 *
                                                               (CFUNCTIONS.sqrt(the_event.arg1))));

                                    if (dtemp < 1)
                                    {
                                        dtemp = 1;
                                    }

                                    if (dtemp == 1)
                                    {
                                        CFUNCTIONS.sprintf(ref string_buffer,
                                                "You've discovered a charm!\n");
                                    }
                                    else
                                    {
                                        CFUNCTIONS.sprintf(ref string_buffer,
                                    "You've discovered %.0lf charms!\n",
                                    dtemp);
                                    }

                                    ioclass.Do_send_line(c, string_buffer);

                                    ioclass.Do_more(c);
                                    c.player.charms += (int)dtemp;

                                    break;
                            }
                            /* end treasure type 9 */
                            break;
                    }

                }
            }

            ioclass.Do_send_clear(c);
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_cursed_treasure(client_t c)
        /
        / FUNCTION: take care of cursed treasure
        /
        / AUTHOR: E. A. Estes, 12/4/85
        /	  Brian Kelly, 6/17/99
        /
        / ARGUMENTS: none
        /
        / RETURN VALUE: none
        /
        / MODULES CALLED: waddstr()
        /
        / GLOBAL INPUTS: Player, *stdscr
        /
        / GLOBAL OUTPUTS: Player
        /
        / DESCRIPTION:
        /       Handle cursed treasure.  Look for amulets and charms to save
        /       the player from the curse.
        /
        *************************************************************************/

        void Do_cursed_treasure(client_t c)
        {
            float ftemp;
            double dtemp;

            if (c.player.amulets >= CFUNCTIONS.floor(CFUNCTIONS.sqrt(c.player.circle)))
            {
                ioclass.Do_send_line(c, "But your amulets saved you!\n");

                ioclass.Do_more(c);
                c.player.amulets -= CFUNCTIONS.floor(CFUNCTIONS.sqrt(c.player.circle));
            }
            else if (c.player.charms > 0)
            {
                ioclass.Do_send_line(c, "But your charm saved you!\n");

                ioclass.Do_more(c);
                --c.player.charms;
            }
            else
            {

                ioclass.Do_more(c);
                /*
                    statsclass.Do_energy(c, cfunctions.MIN(c.player.energy, (c.player.max_energy +
                        c.player.shield) / 10.0), c.player.max_energy,
                        c.player.shield, 0, false);
                */
                dtemp = c.player.energy - macros.RND() * (c.player.max_energy +
                        c.player.shield) / 3.0;

                if (dtemp < c.player.max_energy / 10.0)
                {
                    dtemp = CFUNCTIONS.MIN(c.player.energy, c.player.max_energy / 10.0);
                }


                statsclass.Do_energy(c, dtemp, c.player.max_energy, c.player.shield, 0, 0);


                statsclass.Do_adjusted_poison(c, 0.25);
            }

            ioclass.Do_send_clear(c);
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_corpse(client_t c, struct event_t *the_event)
        /
        / FUNCTION: take care of cursed treasure
        /
        / AUTHOR: E. A. Estes, 12/4/85
        /	  Brian Kelly, 6/17/99
        /
        / ARGUMENTS: none
        /
        / RETURN VALUE: none
        /
        / MODULES CALLED: waddstr()
        /
        / GLOBAL INPUTS: Player, *stdscr
        /
        / GLOBAL OUTPUTS: Player
        /
        / DESCRIPTION:
        /       Handle cursed treasure.  Look for amulets and charms to save
        /       the player from the curse.
        /
        *************************************************************************/

        internal void Do_corpse(client_t c, event_t the_event)
        {
            player_t the_player = new player_t();
            event_t event_ptr = new event_t();
            string string_buffer = ""; //[phantdefs.SZ_LINE];
            int theAnswer = 0;
            double dtemp;

            the_player = (player_t)the_event.arg4;

            /* check the cursed flag */
            if (the_event.arg1 != 0)
            {
                string_buffer = CFUNCTIONS.sprintfSinglestring("You have discovered the corpse of %s.\n",
                   the_player.name);

                ioclass.Do_send_line(c, string_buffer);
                ioclass.Do_send_line(c, "Do you wish to frisk the body?\n");

                if (ioclass.Do_yes_no(c, ref theAnswer) != phantdefs.S_NORM || theAnswer == 1)
                {

                    ioclass.Do_send_clear(c);
                    ioclass.Do_send_line(c, "The body crumbles to ashes.\n");
                    ioclass.Do_more(c);
                    ioclass.Do_send_clear(c);
                    return;
                }

                ioclass.Do_send_clear(c);

                /* Fresh discovered corpses have an 75% chance of being cursed */
                /* This probability drops to nothing over phantdefs.CORPSE_LIFE days */
                if (macros.RND() < 0.75 - 0.75 * (CFUNCTIONS.GetUnixEpoch(DateTime.Now) - c.player.last_load) /
                phantdefs.CORPSE_LIFE)
                {

                    the_player = null; // free((void*) the_player);

                    ioclass.Do_send_line(c, "The body was cursed!\n");

                    Do_cursed_treasure(c);
                    return;
                }
            }
            else
            {
                string_buffer = CFUNCTIONS.sprintfSinglestring("You search the defeated carcass of %s.\n",
                   the_player.name);

                ioclass.Do_send_line(c, string_buffer);
            }

            if (the_player.gold > 0.0)
            {

                string_buffer = CFUNCTIONS.sprintfSinglestring("You find %.0lf gold.\n", the_player.gold);

                ioclass.Do_send_line(c, string_buffer);

                statsclass.Do_gold(c, the_player.gold, 0);
            }

            if (the_player.gems > 1.0)
            {

                string_buffer = CFUNCTIONS.sprintfSinglestring("You find %.0lf gems.\n", the_player.gems);

                ioclass.Do_send_line(c, string_buffer);

                statsclass.Do_gems(c, the_player.gems, 0);
            }
            else if (the_player.gems > 0.0)
            {

                string_buffer = CFUNCTIONS.sprintfSinglestring("You find %.0lf gem.\n", the_player.gems);

                ioclass.Do_send_line(c, string_buffer);

                statsclass.Do_gems(c, the_player.gems, 0);
            }

            the_player.sword = CFUNCTIONS.floor(the_player.sword / 2);
            if (the_player.sword > c.player.sword)
            {


                string_buffer = CFUNCTIONS.sprintfSinglestring("You find a %.0lf strength sword.\n",
                    the_player.sword);


                ioclass.Do_send_line(c, string_buffer);

                statsclass.Do_strength(c, c.player.max_strength, the_player.sword, 0, 0);
            }

            the_player.shield = CFUNCTIONS.floor(the_player.shield / 2);
            if (the_player.shield > c.player.shield)
            {


                string_buffer = CFUNCTIONS.sprintfSinglestring("You find a %.0lf strength shield.\n",
                    the_player.shield);


                ioclass.Do_send_line(c, string_buffer);


                statsclass.Do_energy(c, c.player.energy - c.player.shield +

                    the_player.shield, c.player.max_energy,
                    the_player.shield, 0, 0);
            }

            the_player.quicksilver = CFUNCTIONS.floor(the_player.quicksilver / 4);
            if (the_player.quicksilver > c.player.quicksilver)
            {


                string_buffer = CFUNCTIONS.sprintfSinglestring("You find %.0f quicksilver.\n",
                    the_player.quicksilver);


                ioclass.Do_send_line(c, string_buffer);

                statsclass.Do_speed(c, c.player.max_quickness, the_player.quicksilver, 0,
                    0);
            }

            if (the_player.holywater > 0)
            {


                string_buffer = CFUNCTIONS.sprintfSinglestring("You find %hd holy water.\n",
                    the_player.holywater);


                ioclass.Do_send_line(c, string_buffer);
                c.player.holywater += the_player.holywater;
            }

            if (the_player.amulets > 0)
            {


                string_buffer = CFUNCTIONS.sprintfSinglestring("You find %hd amulets.\n",
                    the_player.amulets);


                ioclass.Do_send_line(c, string_buffer);
                c.player.amulets += the_player.amulets;
            }

            if (the_player.charms > 0)
            {


                string_buffer = CFUNCTIONS.sprintfSinglestring("You find %hd charms.\n",
                    the_player.charms);


                ioclass.Do_send_line(c, string_buffer);
                c.player.charms += the_player.charms;
            }

            if (the_player.virgin)
            {

                Do_award_virgin(c);
            }

            the_player = null; // free((void*) the_player);

            ioclass.Do_more(c);
            ioclass.Do_send_clear(c);

            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: treasureclass.Do_award_blessing(client_t c)
        /
        / FUNCTION: take care of cursed treasure
        /
        / AUTHOR: E. A. Estes, 12/4/85
        /	  Brian Kelly, 9/6/99
        /
        / ARGUMENTS: none
        /
        / RETURN VALUE: none
        /
        / MODULES CALLED: waddstr()
        /
        / GLOBAL INPUTS: Player, *stdscr
        /
        / GLOBAL OUTPUTS: Player
        /
        / DESCRIPTION:
        /       Handle cursed treasure.  Look for amulets and charms to save
        /       the player from the curse.
        /
        *************************************************************************/

        internal void Do_award_blessing(client_t c)
        {
            float ftemp;

            statsclass.Do_blessing(c, 1, 0);

            statsclass.Do_sin(c, -0.25 * c.player.sin);

            statsclass.Do_energy(c, c.player.max_energy + c.player.shield,
                c.player.max_energy, c.player.shield, 0.0, 0);

            statsclass.Do_mana(c, 100.0 * c.player.circle, 0);
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_award_virgin(client_t c)
        /
        / FUNCTION: take care of cursed treasure
        /
        / AUTHOR: E. A. Estes, 12/4/85
        /	  Brian Kelly, 9/6/99
        /
        / ARGUMENTS: none
        /
        / RETURN VALUE: none
        /
        / MODULES CALLED: waddstr()
        /
        / GLOBAL INPUTS: Player, *stdscr
        /
        / GLOBAL OUTPUTS: Player
        /
        / DESCRIPTION:
        /       Handle cursed treasure.  Look for amulets and charms to save
        /       the player from the curse.
        /
        *************************************************************************/

        void Do_award_virgin(client_t c)
        {
            int theAnswer = 0;
            float ftemp;

            if (c.player.gender == (phantdefs.MALE != 0 ? true : false))
            {

                ioclass.Do_send_line(c,
             "You have rescued a virgin. Do you wish you succumb to your carnal desires?\n");

                if (ioclass.Do_yes_no(c, ref theAnswer) != phantdefs.S_NORM)
                {
                    return;
                }

                if (theAnswer == 1)
                {

                    statsclass.Do_virgin(c, 1, 0);
                }
                else
                {

                    statsclass.Do_experience(c, 2500.0 * c.player.circle, 0);

                    statsclass.Do_sin(c, 1.0);
                }
            }
            else
            {

                ioclass.Do_send_line(c,
                   "You have rescued a virgin.  Do you wish to sacrifice her now?\n");

                if (ioclass.Do_yes_no(c, ref theAnswer) != phantdefs.S_NORM)
                {
                    return;
                }

                if (theAnswer == 1)
                {

                    statsclass.Do_virgin(c, 1, 0);
                }
                else
                {

                    statsclass.Do_experience(c, 2500.0 * c.player.circle, 0);

                    statsclass.Do_sin(c, 1.0);
                }
            }
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_treasure_trove(struct client_c *c)
        /
        / FUNCTION: select a treasure
        /
        / AUTHOR: Brian Kelly, 5/9/01
        /
        / ARGUMENTS: none
        /
        / RETURN VALUE: none
        /
        / MODULES CALLED: pickmonster(), collecttaxes(), more(), cursedtreasure(),
        /       cfunctions.floor(), wmove(), drandom(), sscanf(), printw(), altercoordinates(),
        /       longjmp(), infloat(), waddstr(), getanswer(), getstring(), wclrtobot()
        /
        / GLOBAL INPUTS: Somebetter[], Curmonster, Whichmonster, Circle, Player,
        /       *stdscr, Databuf[], *Statptr, Fightenv[]
        /
        / GLOBAL OUTPUTS: Whichmonster, Shield, Player
        /
        / DESCRIPTION:
        /       Roll up a treasure based upon monster type and size, and
        /       certain player statistics.
        /       Handle cursed treasure.
        /
        *************************************************************************/

        internal void Do_treasure_trove(client_t c)
        {
            event_t event_ptr = new event_t();

            double gems = 0.0;                     /* gems awarded */
            double charms = 0.0;                   /* charms awarded */
            string string_buffer = ""; //[phantdefs.SZ_LINE];

            ioclass.Do_send_line(c, "You have found a treasure trove!\n");

            gems = 1 + CFUNCTIONS.floor(c.player.circle * (macros.RND() * .33));

            if (!c.player.blessing)
            {
                string_buffer = CFUNCTIONS.sprintfSinglestring("You find %.0lf gems and a blessing!\n", gems);
                Do_award_blessing(c);
            }
            else if (c.player.charms < 32)
            {
                charms = 1 + CFUNCTIONS.floor(10 * macros.RND() * CFUNCTIONS.sqrt(c.player.circle));

                string_buffer = CFUNCTIONS.sprintfSinglestring("You find %.0lf gems and %.0lf charms!\n",
                        gems, charms);

                c.player.charms += (int)charms;
            }
            else
            {
                gems += CFUNCTIONS.floor(2 + macros.RND() * 3);
                string_buffer = CFUNCTIONS.sprintfSinglestring("You find %.0lf gems!\n", gems);
            }

            ioclass.Do_send_line(c, string_buffer);
            ioclass.Do_more(c);
            ioclass.Do_send_clear(c);

            statsclass.Do_gems(c, gems, 0);

            CFUNCTIONS.sprintf(ref string_buffer,
            "%s finds and unearths the treasure trove.  The hunt begins anew...\n",
                c.modifiedName);

            ioclass.Do_broadcast(c, string_buffer);

            return;
        }

        /************************************************************************
        /
        / FUNCTION NAME: Do_treasure_map(struct client_c *c)
        /
        / FUNCTION: output a treasure map
        /
        / AUTHOR: Brian Kelly and Eugene Hung, 7/26/01
        /
        / ARGUMENTS: none
        /
        / RETURN VALUE: none
        /
        / MODULES CALLED: 
        /
        / GLOBAL INPUTS: 
        /
        / GLOBAL OUTPUTS: 
        /
        / DESCRIPTION:
        /       Generate a treasure map.
        /
        *************************************************************************/

        void Do_treasure_map(client_t c)
        {

            double dtemp = 0, x_loc = 0, y_loc = 0;           /* for temporary calculations */
            realm_object_t object_ptr = new realm_object_t();
            string string_buffer = ""; //[phantdefs.SZ_LINE], 
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];

            /* find the treasure trove */
            miscclass.Do_lock_mutex(c.realm.realm_lock);
            object_ptr = c.realm.objects;

            /* run through the realm objects */
            while (object_ptr != null)
            {

                if (object_ptr.type == phantdefs.TREASURE_TROVE)
                {
                    break;
                }

                object_ptr = object_ptr.next_object;
            }

            if (object_ptr == null)
            {


                miscclass.Do_unlock_mutex(c.realm.realm_lock);

                error_msg = CFUNCTIONS.sprintfSinglestring(
                     "[%s] No trove found in realm objects in Do_treasure.\n",
                     c.connection_id);

                fileclass.Do_log_error(error_msg);

                ioclass.Do_send_line(c, "It was cursed!\n");
                Do_cursed_treasure(c);
                return;
            }

            x_loc = object_ptr.x;
            y_loc = object_ptr.y;

            miscclass.Do_unlock_mutex(c.realm.realm_lock);

            /* determine the distance to the trove */
            miscclass.Do_distance(c.player.x, x_loc, c.player.y, y_loc, ref dtemp);



            if (dtemp > 1.0)
            {
                /* throw in a fudge factor of up to 12.5% if not near trove */
                dtemp = CFUNCTIONS.floor(dtemp * (.875 + macros.RND() * .25) + .01);

                CFUNCTIONS.sprintf(ref string_buffer,
             "It says, 'To find me treasure trove, ye must move %.0lf squares to the ",
             dtemp);

                miscclass.Do_direction(c, x_loc, y_loc, ref string_buffer);


                CFUNCTIONS.strcat(ref string_buffer, " and then look for me next map.\n");
            }
            else if (dtemp == 1.0)
            {

                CFUNCTIONS.strcpy(ref string_buffer,
                  "Arr, you're almost there.  The booty is 1 square ");


                miscclass.Do_direction(c, x_loc, y_loc, ref string_buffer);

                CFUNCTIONS.strcat(ref string_buffer, ".\n");

            }
            else
            {
                CFUNCTIONS.strcpy(ref string_buffer, "You've found the treasure!  Dig matey, dig!\n");
            }

            ioclass.Do_send_line(c, string_buffer);
            ioclass.Do_more(c);

            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_smith(struct client_c *c, struct event_t *the_event)
        /
        / FUNCTION: select a treasure
        /
        / AUTHOR: Brian Kelly, 5/9/01
        /
        / ARGUMENTS: none
        /
        / RETURN VALUE: none
        /
        / MODULES CALLED: pickmonster(), collecttaxes(), more(), cursedtreasure(),
        /       cfunctions.floor(), wmove(), drandom(), sscanf(), printw(), altercoordinates(),
        /       longjmp(), infloat(), waddstr(), getanswer(), getstring(), wclrtobot()
        /
        / GLOBAL INPUTS: Somebetter[], Curmonster, Whichmonster, Circle, Player,
        /       *stdscr, Databuf[], *Statptr, Fightenv[]
        /
        / GLOBAL OUTPUTS: Whichmonster, Shield, Player
        /
        / DESCRIPTION:
        /       Roll up a treasure based upon monster type and size, and
        /       certain player statistics.
        /       Handle cursed treasure.
        /
        *************************************************************************/

        void Do_smith(client_t c, event_t the_event)
        {
            double dtemp = 0;
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            button_t buttons = new button_t();
            long answer = -1;
            int rc = 0;

            if (c.player.sin < macros.RND() * 2.0)
            {

                ioclass.Do_send_line(c,
                   "Wayland Smith offers to improve a piece of your equipment!\n");

                ioclass.Do_clear_buttons(buttons, 0);
                answer = 0;

                if (c.player.shield > 0)
                {
                    CFUNCTIONS.strcpy(ref buttons.button[0], "Shield\n");
                    answer = 1;
                }

                if (c.player.sword > 0)
                {
                    CFUNCTIONS.strcpy(ref buttons.button[1], "Sword\n");
                    answer = 1;
                }

                if (c.player.quicksilver > 0)
                {
                    CFUNCTIONS.strcpy(ref buttons.button[2], "Quicksilver\n");
                    answer = 1;
                }

                CFUNCTIONS.strcpy(ref buttons.button[7], "Cancel\n");

                if (answer == 0)
                {


                    ioclass.Do_send_line(c,
                        "He then sees you have none, guffaws and leaves.\n");


                    ioclass.Do_more(c);
                    return;
                }

                rc = ioclass.Do_buttons(c, ref answer, buttons);

                if (rc != phantdefs.S_NORM)
                {
                    answer = 7;
                }

                /* switch on the player's answer */
                switch (answer)
                {

                    /* upgrade shield */
                    case 0:

                        /* determine the shield bonus */
                        dtemp = .018 * c.player.shield;

                        if (dtemp > 1 + 60 * the_event.arg1)
                        {
                            dtemp = 1 + 60 * the_event.arg1;
                        }

                        if (dtemp < 1)
                        {
                            dtemp = 1;
                        }

                        /* award the bonus */
                        statsclass.Do_energy(c, c.player.energy, c.player.max_energy,
                            c.player.shield + dtemp, 0, 0);

                        break;

                    case 1:

                        /* determine the sword bonus */
                        dtemp = .018 * c.player.sword;

                        if (dtemp > 1 + 2 * the_event.arg1)
                        {
                            dtemp = 1 + 2 * the_event.arg1;
                        }

                        if (dtemp < 1)
                        {
                            dtemp = 1;
                        }

                        /* award the bonus */
                        statsclass.Do_strength(c, c.player.max_strength, c.player.sword + dtemp,
                            0, 0);

                        break;

                    case 2:

                        /* determine the quicksilver bonus */
                        dtemp = .018 * c.player.quicksilver;

                        if (dtemp > 1 + .12 * the_event.arg1)
                        {
                            dtemp = 1 + .12 * the_event.arg1;
                        }

                        if (dtemp < 1)
                        {
                            dtemp = 1;
                        }

                        /* award the bonus */
                        statsclass.Do_speed(c, c.player.max_quickness, c.player.quicksilver + dtemp,
                            0, 0);

                        break;

                    case 7:
                        return;

                    default:

                        error_msg = CFUNCTIONS.sprintfSinglestring(
                                "[%s] Returned non-option in Do_smith.\n",
                                c.connection_id);

                        fileclass.Do_log_error(error_msg);
                        hackclass.Do_caught_hack(c, phantdefs.H_SYSTEM);
                        break;
                }
                return;
            }
            else
            {


                ioclass.Do_send_line(c, "Wayland Smith appears!  He glares at you for a moment, then walks away.\n");


                ioclass.Do_more(c);
                return;
            }
        }
    }
}
