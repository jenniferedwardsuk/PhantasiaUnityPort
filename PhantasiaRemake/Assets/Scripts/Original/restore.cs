using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace phantasiaclasses
{
    public class restore //: MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        static restore Instance;
        private restore()
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
            statsclass = stats.GetInstance();
            commandsclass = commands.GetInstance();
            socketclass = socket.GetInstance();
            infoclass = info.GetInstance();
            treasureclass = treasure.GetInstance();
            itcombatclass = itcombat.GetInstance();
            fightclass = fight.GetInstance();
            initclass = init.GetInstance();
        }
        public static restore GetInstance()
        {
            restore instance = null;
            if (Instance != null)
            {
                instance = Instance;
            }
            else
            {
                instance = new restore();
            }
            return instance;
        }

        phantasiaclasses.misc miscclass;
        phantasiaclasses.file fileclass;
        phantasiaclasses.socket socketclass;
        phantasiaclasses.tags tagsclass;
        phantasiaclasses.info infoclass;
        phantasiaclasses.io ioclass;
        phantasiaclasses.commands commandsclass;
        phantasiaclasses.stats statsclass;
        phantasiaclasses.itcombat itcombatclass;
        phantasiaclasses.fight fightclass;
        phantasiaclasses.treasure treasureclass;
        phantasiaclasses.eventsrc eventclass;
        phantasiaclasses.hack hackclass;
        phantasiaclasses.character characterclass;
        phantasiaclasses.account accountclass;
        phantasiaclasses.init initclass;

        /*
         * restore.c  Phantasia Ultimate routines for restores
         */

        int server_hook;

        /************************************************************************
        /
        / FUNCTION NAME: restoreclass.Do_restoreoptions(struct client_t *c)
        /
        / FUNCTION: wizard administrative powers - restore options
        /
        / AUTHOR: Samuel T,    8/20/02
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
        / DESCRIPTION: A set of restore options
        /
        *************************************************************************/

        internal void Do_restoreoptions(client_t c)
        {
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            string string_buffer = ""; //[phantdefs.SZ_LINE], 
            string string_buffer2 = ""; //[phantdefs.SZ_LINE];
            event_t event_ptr = new event_t();
            button_t theButtons = new button_t();
            long answer = -1;              /* pointer to option description */

            /* determine what the player wants to do */

            ioclass.Do_clear_buttons(theButtons, 0);

            CFUNCTIONS.strcpy(ref theButtons.button[0], "Natural\n");
            CFUNCTIONS.strcpy(ref theButtons.button[1], "Equip\n");
            CFUNCTIONS.strcpy(ref theButtons.button[2], "Currency\n");
            CFUNCTIONS.strcpy(ref theButtons.button[3], "Age\n");
            CFUNCTIONS.strcpy(ref theButtons.button[4], "Items\n");
            CFUNCTIONS.strcpy(ref theButtons.button[5], "Experience\n");
            CFUNCTIONS.strcpy(ref theButtons.button[6], "Restore Char\n");
            CFUNCTIONS.strcpy(ref theButtons.button[7], "Cancel\n");

            ioclass.Do_send_line(c, "Which restore options?\n");

            if (ioclass.Do_buttons(c, ref answer, theButtons) != phantdefs.S_NORM || answer == 7)
            {
                ioclass.Do_send_clear(c);
                return;
            }

            ioclass.Do_send_clear(c);

            switch (answer)
            {

                /* Natural Stats restore */
                case 0:


                    Do_naturalrestore(c);

                    return;

                case 1:


                    Do_equiprestore(c);

                    return;

                case 2:

                    Do_currencyrestore(c);

                    return;

                case 3:

                    event_ptr = eventclass.Do_create_event();
                    event_ptr.type = (short)phantdefs.AGING_EVENT;
                    CFUNCTIONS.strcpy(ref string_buffer2, "age to");

                    if (ioclass.Do_long_dialog(c, ref event_ptr.arg3, "How much age would you like to increase?\n") != 0)
                    {


                        event_ptr = null; //free((void*) event_ptr);
                        return;
                    }


                    /* if we're here, we have an event to send to another player */
                    /* prompt for player to affect */

                    string_buffer = CFUNCTIONS.sprintfSinglestring("Who do you wish to send the %s?\n", string_buffer2);

                    if (ioclass.Do_player_dialog(c, string_buffer, string_buffer2) != phantdefs.S_NORM)
                    {

                        event_ptr = null; //free((void*) event_ptr);
                        return;
                    }

                    event_ptr.arg4 = "";// (void*) Do_malloc(cfunctions.strlen(c.modifiedName) + 1);
                    
                    string arg4 = (string)event_ptr.arg4;
                    CFUNCTIONS.strcpy(ref arg4, c.modifiedName);
                    event_ptr.arg4 = arg4;
                    event_ptr.from = c.game;

                    if (eventclass.Do_send_character_event(c, event_ptr, string_buffer2) == 0)
                    {

                        event_ptr.arg4 = null; //free((void*) event_ptr.arg4);

                        event_ptr = null; //free((void*) event_ptr);

                        ioclass.Do_send_line(c, "That character just left the game.\n");

                        ioclass.Do_more(c);
                        return;
                    }


                    ioclass.Do_send_line(c, "Age restore complete.\n");

                    ioclass.Do_more(c);

                    ioclass.Do_send_clear(c);

                    return;

                case 4:


                    Do_itemsrestore(c);

                    return;

                case 5:
                    event_ptr = eventclass.Do_create_event();
                    event_ptr.type = (short)phantdefs.EXPERIENCE_EVENT;
                    CFUNCTIONS.strcpy(ref string_buffer2, "experience to");

                    if (ioclass.Do_double_dialog(c, ref event_ptr.arg1, "How much experience would you like to increase?\n") != 0)
                    {


                        event_ptr = null; //free((void*) event_ptr);
                        return;
                    }


                    /* if we're here, we have an event to send to another player */
                    /* prompt for player to affect */

                    string_buffer = CFUNCTIONS.sprintfSinglestring("Who do you wish to send the %s?\n", string_buffer2);

                    if (ioclass.Do_player_dialog(c, string_buffer, string_buffer2) != phantdefs.S_NORM)
                    {

                        event_ptr = null; //free((void*) event_ptr);
                        return;
                    }

                    event_ptr.arg4 = "";// (void*) Do_malloc(cfunctions.strlen(c.modifiedName) + 1);
                    
                    string arg4again = (string)event_ptr.arg4;
                    CFUNCTIONS.strcpy(ref arg4again, c.modifiedName);
                    event_ptr.arg4 = arg4again;
                    event_ptr.from = c.game;

                    if (eventclass.Do_send_character_event(c, event_ptr, string_buffer2) == 0)
                    {

                        event_ptr.arg4 = null; //free((void*) event_ptr.arg4);

                        event_ptr = null; //free((void*) event_ptr);

                        ioclass.Do_send_line(c, "That character just left the game.\n");

                        ioclass.Do_more(c);
                        return;
                    }


                    ioclass.Do_send_line(c, "Experience restore complete.\n");

                    ioclass.Do_more(c);

                    ioclass.Do_send_clear(c);

                    return;


                case 6:

                    fileclass.Do_restore_character(c);
                    return;

                default:
                    error_msg = CFUNCTIONS.sprintfSinglestring("[%s] Returned non-option in restoreclass.Do_restoreoptions.\n", c.connection_id);


                    fileclass.Do_log_error(error_msg);

                    hackclass.Do_caught_hack(c, phantdefs.H_SYSTEM);
                    return;
            }
        }

        /************************************************************************
        /
        / FUNCTION NAME: Do_naturalrestore(struct client_t *c)
        /
        / FUNCTION: Natural Restore Options
        /
        / AUTHOR: Samuel T, 8/20/02
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
        / DESCRIPTION: Restore Natural Stats
        /
        *************************************************************************/

        void Do_naturalrestore(client_t c)
        {
            event_t event_ptr = new event_t();
            button_t theButtons = new button_t();
            string string_buffer = ""; //[phantdefs.SZ_LINE], 
            string string_buffer2 = ""; //[phantdefs.SZ_LINE];
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            long answer = -1;              /* pointer to option description */

            ioclass.Do_clear_buttons(theButtons, 0);

            /* determine what the supreme wizard wants to increase */
            CFUNCTIONS.strcpy(ref theButtons.button[0], "Energy\n");
            CFUNCTIONS.strcpy(ref theButtons.button[1], "Strength\n");
            CFUNCTIONS.strcpy(ref theButtons.button[2], "Speed\n");
            CFUNCTIONS.strcpy(ref theButtons.button[3], "Magic Lvl\n");
            CFUNCTIONS.strcpy(ref theButtons.button[4], "Brains\n");
            CFUNCTIONS.strcpy(ref theButtons.button[5], "Lives\n");
            CFUNCTIONS.strcpy(ref theButtons.button[6], "Poison\n");
            CFUNCTIONS.strcpy(ref theButtons.button[7], "Cancel\n");

            ioclass.Do_send_line(c, "What natural restore command do you wish to use?\n");

            if (ioclass.Do_buttons(c, ref answer, theButtons) != phantdefs.S_NORM || answer == 7)
            {

                ioclass.Do_send_clear(c);
                return;
            }
            ioclass.Do_send_clear(c);

            switch (answer)
            {

                case 0:
                    event_ptr = eventclass.Do_create_event();
                    event_ptr.type = (short)phantdefs.NATINC_EVENT;
                    event_ptr.arg1 = 0;
                    CFUNCTIONS.strcpy(ref string_buffer2, "base energy");

                    if (ioclass.Do_long_dialog(c, ref event_ptr.arg3, "How much would you like to increase for base energy?\n") != 0)
                    {
                        event_ptr = null; //free((void*) event_ptr);
                        return;
                    }

                    if (event_ptr.arg3 <= 0)
                    {

                        event_ptr = null; //free((void*) event_ptr);
                        return;
                    }

                    break;

                case 1:
                    event_ptr = eventclass.Do_create_event();
                    event_ptr.type = (short)phantdefs.NATINC_EVENT;
                    event_ptr.arg1 = 1;
                    CFUNCTIONS.strcpy(ref string_buffer2, "base strength");

                    if (ioclass.Do_long_dialog(c, ref event_ptr.arg3, "How much would you like to increase for base strength?\n") != 0)
                    {


                        event_ptr = null; //free((void*) event_ptr);
                        return;
                    }

                    if (event_ptr.arg3 <= 0)
                    {

                        event_ptr = null; //free((void*) event_ptr);
                        return;
                    }

                    break;

                case 2:
                    event_ptr = eventclass.Do_create_event();
                    event_ptr.type = (short)phantdefs.NATINC_EVENT;
                    event_ptr.arg1 = 2;
                    CFUNCTIONS.strcpy(ref string_buffer2, "base speed");

                    if (ioclass.Do_long_dialog(c, ref event_ptr.arg3, "How much would you like to increase for base speed?\n") != 0)
                    {


                        event_ptr = null; //free((void*) event_ptr);
                        return;
                    }

                    if (event_ptr.arg3 <= 0)
                    {

                        event_ptr = null; //free((void*) event_ptr);
                        return;
                    }

                    break;

                case 3:
                    event_ptr = eventclass.Do_create_event();
                    event_ptr.type = (short)phantdefs.NATINC_EVENT;
                    event_ptr.arg1 = 3;
                    CFUNCTIONS.strcpy(ref string_buffer2, "magic level");

                    if (ioclass.Do_long_dialog(c, ref event_ptr.arg3, "How much would you like to increase for magic level?\n") != 0)
                    {


                        event_ptr = null; //free((void*) event_ptr);
                        return;
                    }

                    if (event_ptr.arg3 <= 0)
                    {

                        event_ptr = null; //free((void*) event_ptr);
                        return;
                    }

                    break;
                case 4:
                    event_ptr = eventclass.Do_create_event();
                    event_ptr.type = (short)phantdefs.NATINC_EVENT;
                    event_ptr.arg1 = 4;
                    CFUNCTIONS.strcpy(ref string_buffer2, "brains");

                    if (ioclass.Do_long_dialog(c, ref event_ptr.arg3, "How much would you like to increase for brains?\n") != 0)
                    {


                        event_ptr = null; //free((void*) event_ptr);
                        return;
                    }

                    if (event_ptr.arg3 <= 0)
                    {

                        event_ptr = null; //free((void*) event_ptr);
                        return;
                    }

                    break;

                case 5:
                    event_ptr = eventclass.Do_create_event();
                    event_ptr.type = (short)phantdefs.NATINC_EVENT;
                    event_ptr.arg1 = 5;
                    CFUNCTIONS.strcpy(ref string_buffer2, "lives");

                    if (ioclass.Do_long_dialog(c, ref event_ptr.arg3, "How much would you like to increase for lives?\n") != 0)
                    {


                        event_ptr = null; //free((void*) event_ptr);
                        return;
                    }

                    if (event_ptr.arg3 <= 0)
                    {

                        event_ptr = null; //free((void*) event_ptr);
                        return;
                    }

                    break;

                case 6:

                    event_ptr = eventclass.Do_create_event();
                    event_ptr.type = (short)phantdefs.NATINC_EVENT;
                    event_ptr.arg1 = 6;
                    CFUNCTIONS.strcpy(ref string_buffer2, "poison");

                    if (ioclass.Do_long_dialog(c, ref event_ptr.arg3, "How much poison would you like to increase?\n") != 0)
                    {


                        event_ptr = null; //free((void*) event_ptr);
                        return;
                    }

                    if (event_ptr.arg3 <= 0)
                    {

                        event_ptr = null; //free((void*) event_ptr);
                        return;
                    }

                    break;

                default:
                    error_msg = CFUNCTIONS.sprintfSinglestring("[%s] Returned non-option in Do_naturalrestore.\n",
                            c.connection_id);


                    fileclass.Do_log_error(error_msg);

                    hackclass.Do_caught_hack(c, phantdefs.H_SYSTEM);
                    return;
            }

            /* if we're here, we have an event to send to another player */
            /* prompt for player to affect */
            string_buffer = CFUNCTIONS.sprintfSinglestring("Whose %s do you wish to increase?\n", string_buffer2);
            if (ioclass.Do_player_dialog(c, string_buffer, string_buffer2) != phantdefs.S_NORM)
            {
                event_ptr = null; //free((void*) event_ptr);
                return;
            }

            event_ptr.arg4 = "";// (void*)Do_malloc(cfunctions.strlen(c.modifiedName) + 1);
            string arg4 = (string)event_ptr.arg4;
            CFUNCTIONS.strcpy(ref arg4, c.modifiedName);
            event_ptr.arg4 = arg4;
            event_ptr.from = c.game;

            if (eventclass.Do_send_character_event(c, event_ptr, string_buffer2) == 0)
            {
                event_ptr.arg4 = null; //free((void*) event_ptr.arg4);
                event_ptr = null; //free((void*) event_ptr);
                ioclass.Do_send_line(c, "That character just left the game.\n");
                ioclass.Do_more(c);
                return;
            }

            ioclass.Do_send_line(c, "Natural restore complete.\n");
            ioclass.Do_more(c);
            ioclass.Do_send_clear(c);
            return;
        }

        /************************************************************************
        /
        / FUNCTION NAME: Do_natincreceive(struct client_t *c, struct event_t *the_event)
        /
        / FUNCTION: do the natural increase 
        /
        / AUTHOR: Samuel T, 8/22/02
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
        / DESCRIPTION: Increases the natural stats set up by the supreme wizard
        /
        *************************************************************************/

        internal void Do_natincreceive(client_t c, event_t the_event)
        {
            string string_buffer = ""; //[phantdefs.SZ_LINE];
            double dtemp;
            long answer = -1;
            long increase;

            answer = (long)the_event.arg1;
            increase = the_event.arg3;
            /*
                string_buffer = CFUNCTIONS.sprintfSinglestring("%s's casts increase of %.0lf!\n", increase);
                ioclass.Do_send_line(c, string_buffer);
              */
            switch (answer)
            {

                case 0:

                    statsclass.Do_energy(c, c.player.max_energy + increase + c.player.shield,
                          c.player.max_energy + increase, c.player.shield, 0, 0);

                    break;

                case 1:

                    statsclass.Do_strength(c, c.player.max_strength + increase,
                          c.player.sword, 0, 0);

                    break;

                case 2:

                    statsclass.Do_speed(c, c.player.max_quickness + increase, c.player.quicksilver, 0, 0);

                    break;

                case 3:

                    c.player.magiclvl = c.player.magiclvl + increase;

                    break;

                case 4:

                    c.player.brains = c.player.brains + increase;

                    break;

                case 5:

                    c.player.lives = (short)(c.player.lives + increase);

                    if (c.wizard < 4)
                    {
                        if (c.player.lives > 5)
                        {
                            c.player.lives = 5;
                        }
                    }

                    break;


                case 6:
                    c.player.poison = c.player.poison + increase;

                    break;

            }


            string_buffer = CFUNCTIONS.sprintfSinglestring("%s's casts a natural restore spell on you, you feel different!\n",
                   the_event.arg4);

            ioclass.Do_send_line(c, string_buffer);

            string_buffer = CFUNCTIONS.sprintfSinglestring("%s casted a restore spell on %s.\n", the_event.arg4, c.player.lcname);


            fileclass.Do_log(pathnames.NATRESTORE_LOG, string_buffer);

            the_event.arg4 = null; //free((void*) the_event.arg4);

            ioclass.Do_more(c);
            ioclass.Do_send_clear(c);

            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_equiprestore(struct client_t *c)
        /
        / FUNCTION: Equipment Restore Options
        /
        / AUTHOR: Samuel T, 8/21/02
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
        / DESCRIPTION: Restore Equipment
        /
        *************************************************************************/

        void Do_equiprestore(client_t c)
        {
            event_t event_ptr = new event_t();
            button_t theButtons = new button_t();
            string string_buffer = ""; //[phantdefs.SZ_LINE], 
            string string_buffer2 = ""; //[phantdefs.SZ_LINE];
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            long answer = -1;              /* pointer to option description */

            ioclass.Do_clear_buttons(theButtons, 0);

            /* determine what the supreme wizard wants to increase */
            CFUNCTIONS.strcpy(ref theButtons.button[0], "Shield\n");
            CFUNCTIONS.strcpy(ref theButtons.button[1], "Sword\n");
            CFUNCTIONS.strcpy(ref theButtons.button[2], "Quicksilver\n");
            CFUNCTIONS.strcpy(ref theButtons.button[3], "Charms\n");
            CFUNCTIONS.strcpy(ref theButtons.button[4], "Amulets\n");
            CFUNCTIONS.strcpy(ref theButtons.button[5], "Holy Water\n");
            CFUNCTIONS.strcpy(ref theButtons.button[6], "Crowns\n");
            CFUNCTIONS.strcpy(ref theButtons.button[7], "Cancel\n");

            ioclass.Do_send_line(c, "What equipment restore command do you wish to use?\n");

            if (ioclass.Do_buttons(c, ref answer, theButtons) != phantdefs.S_NORM || answer == 7)
            {

                ioclass.Do_send_clear(c);
                return;
            }
            ioclass.Do_send_clear(c);

            switch (answer)
            {

                case 0:
                    event_ptr = eventclass.Do_create_event();
                    event_ptr.type = (short)phantdefs.EQINC_EVENT;
                    event_ptr.arg1 = 0;
                    CFUNCTIONS.strcpy(ref string_buffer2, "shield");

                    if (ioclass.Do_long_dialog(c, ref event_ptr.arg3, "How much would you like to increase for shield?\n") != 0)
                    {


                        event_ptr = null; //free((void*) event_ptr);
                        return;
                    }

                    if (event_ptr.arg3 <= 0)
                    {

                        event_ptr = null; //free((void*) event_ptr);
                        return;
                    }

                    break;

                case 1:
                    event_ptr = eventclass.Do_create_event();
                    event_ptr.type = (short)phantdefs.EQINC_EVENT;
                    event_ptr.arg1 = 1;
                    CFUNCTIONS.strcpy(ref string_buffer2, "sword");

                    if (ioclass.Do_long_dialog(c, ref event_ptr.arg3, "How much would you like to increase for sword?\n") != 0)
                    {


                        event_ptr = null; //free((void*) event_ptr);
                        return;
                    }

                    if (event_ptr.arg3 <= 0)
                    {

                        event_ptr = null; //free((void*) event_ptr);
                        return;
                    }

                    break;

                case 2:
                    event_ptr = eventclass.Do_create_event();
                    event_ptr.type = (short)phantdefs.EQINC_EVENT;
                    event_ptr.arg1 = 2;
                    CFUNCTIONS.strcpy(ref string_buffer2, "quicksilver");

                    if (ioclass.Do_long_dialog(c, ref event_ptr.arg3, "How much would you like to increase for quicksilver?\n") != 0)
                    {


                        event_ptr = null; //free((void*) event_ptr);
                        return;
                    }

                    if (event_ptr.arg3 <= 0)
                    {

                        event_ptr = null; //free((void*) event_ptr);
                        return;
                    }

                    break;

                case 3:
                    event_ptr = eventclass.Do_create_event();
                    event_ptr.type = (short)phantdefs.EQINC_EVENT;
                    event_ptr.arg1 = 3;
                    CFUNCTIONS.strcpy(ref string_buffer2, "charms");

                    if (ioclass.Do_long_dialog(c, ref event_ptr.arg3, "How much would you like to increase for charms?\n") != 0)
                    {


                        event_ptr = null; //free((void*) event_ptr);
                        return;
                    }

                    if (event_ptr.arg3 <= 0)
                    {

                        event_ptr = null; //free((void*) event_ptr);
                        return;
                    }

                    break;

                case 4:
                    event_ptr = eventclass.Do_create_event();
                    event_ptr.type = (short)phantdefs.EQINC_EVENT;
                    event_ptr.arg1 = 4;
                    CFUNCTIONS.strcpy(ref string_buffer2, "amulets");

                    if (ioclass.Do_long_dialog(c, ref event_ptr.arg3, "How much would you like to increase for amulets?\n") != 0)
                    {


                        event_ptr = null; //free((void*) event_ptr);
                        return;
                    }

                    if (event_ptr.arg3 <= 0)
                    {

                        event_ptr = null; //free((void*) event_ptr);
                        return;
                    }

                    break;

                case 5:
                    event_ptr = eventclass.Do_create_event();
                    event_ptr.type = (short)phantdefs.EQINC_EVENT;
                    event_ptr.arg1 = 5;
                    CFUNCTIONS.strcpy(ref string_buffer2, "holy water");

                    if (ioclass.Do_long_dialog(c, ref event_ptr.arg3, "How much would you like to increase for holy water?\n") != 0)
                    {


                        event_ptr = null; //free((void*) event_ptr);
                        return;
                    }

                    if (event_ptr.arg3 <= 0)
                    {

                        event_ptr = null; //free((void*) event_ptr);
                        return;
                    }

                    break;

                case 6:

                    event_ptr = eventclass.Do_create_event();
                    event_ptr.type = (short)phantdefs.EQINC_EVENT;
                    event_ptr.arg1 = 6;
                    CFUNCTIONS.strcpy(ref string_buffer2, "crowns");

                    if (ioclass.Do_long_dialog(c, ref event_ptr.arg3, "How much would you like to increase for crowns?\n") != 0)
                    {


                        event_ptr = null; //free((void*) event_ptr);
                        return;
                    }

                    if (event_ptr.arg3 <= 0)
                    {

                        event_ptr = null; //free((void*) event_ptr);
                        return;
                    }

                    break;

                default:
                    error_msg = CFUNCTIONS.sprintfSinglestring("[%s] Returned non-option in Do_equiprestore.\n", c.connection_id);


                    fileclass.Do_log_error(error_msg);

                    hackclass.Do_caught_hack(c, phantdefs.H_SYSTEM);
                    return;
            }

            /* if we're here, we have an event to send to another player */
            /* prompt for player to affect */
            string_buffer = CFUNCTIONS.sprintfSinglestring("Whose %s do you wish to increase?\n", string_buffer2);
            if (ioclass.Do_player_dialog(c, string_buffer, string_buffer2) != phantdefs.S_NORM)
            {
                event_ptr = null; //free((void*) event_ptr);
                return;
            }

            event_ptr.arg4 = "";// (void*)Do_malloc(cfunctions.strlen(c.modifiedName) + 1);
            string arg4 = (string)event_ptr.arg4;
            CFUNCTIONS.strcpy(ref arg4, c.modifiedName);
            event_ptr.arg4 = arg4;
            event_ptr.from = c.game;

            if (eventclass.Do_send_character_event(c, event_ptr, string_buffer2) == 0)
            {
                event_ptr.arg4 = null; //free((void*) event_ptr.arg4);
                event_ptr = null; //free((void*) event_ptr);
                ioclass.Do_send_line(c, "That character just left the game.\n");
                ioclass.Do_more(c);
                return;
            }

            ioclass.Do_send_line(c, "Equipment restore complete.\n");
            ioclass.Do_more(c);
            ioclass.Do_send_clear(c);
            return;
        }

        /************************************************************************
        /
        / FUNCTION NAME: Do_eqincreceive(struct client_t *c, struct event_t *the_event)
        /
        / FUNCTION: do the equipment increase 
        /
        / AUTHOR: Samuel T, 8/22/02
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
        / DESCRIPTION: Increases the equipment set up by the supreme wizard
        /
        *************************************************************************/

        internal void Do_eqincreceive(client_t c, event_t the_event)
        {
            string string_buffer = ""; //[phantdefs.SZ_LINE];
            double dtemp;
            long answer = -1;
            long increase;

            answer = (long)the_event.arg1;
            increase = the_event.arg3;

            switch (answer)
            {

                case 0:

                    statsclass.Do_energy(c, c.player.max_energy + c.player.shield + increase,
                          c.player.max_energy, c.player.shield + increase, 0, 0);

                    break;

                case 1:

                    statsclass.Do_strength(c, c.player.max_strength, c.player.sword + increase, 0, 0);

                    break;

                case 2:

                    statsclass.Do_speed(c, c.player.max_quickness, c.player.quicksilver + increase, 0, 0);

                    break;

                case 3:

                    c.player.charms = (int)(c.player.charms + increase);

                    break;

                case 4:

                    c.player.amulets = (int)(c.player.amulets + increase);

                    break;

                case 5:

                    c.player.holywater = (int)(c.player.holywater + increase);

                    break;

                case 6:

                    statsclass.Do_crowns(c, (int)increase, 1);

                    break;

            }


            string_buffer = CFUNCTIONS.sprintfSinglestring("%s's casts an equipment spell on you, you feel different!\n",
                   the_event.arg4);

            ioclass.Do_send_line(c, string_buffer);

            string_buffer = CFUNCTIONS.sprintfSinglestring("%s casted an equipment spell on %s.\n", the_event.arg4, c.player.lcname);


            fileclass.Do_log(pathnames.EQRESTORE_LOG, string_buffer);

            the_event.arg4 = null; //free((void*) the_event.arg4);

            ioclass.Do_more(c);
            ioclass.Do_send_clear(c);

            return;
        }

        /************************************************************************
        /
        / FUNCTION NAME: Do_currencyrestore(struct client_t *c)
        /
        / FUNCTION: Currency Restore Options
        /
        / AUTHOR: Samuel T, 8/22/02
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
        / DESCRIPTION: Restore Currency
        /
        *************************************************************************/

        void Do_currencyrestore(client_t c)
        {
            event_t event_ptr = new event_t();
            button_t theButtons = new button_t();
            string string_buffer = ""; //[phantdefs.SZ_LINE], 
            string string_buffer2 = ""; //[phantdefs.SZ_LINE];
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            long answer = -1;              /* pointer to option description */

            ioclass.Do_clear_buttons(theButtons, 0);

            /* determine what the supreme wizard wants to increase */
            CFUNCTIONS.strcpy(ref theButtons.button[0], "Gems\n");
            CFUNCTIONS.strcpy(ref theButtons.button[1], "Gold\n");
            CFUNCTIONS.strcpy(ref theButtons.button[7], "Cancel\n");

            ioclass.Do_send_line(c, "What currency restore command do you wish to use?\n");

            if (ioclass.Do_buttons(c, ref answer, theButtons) != phantdefs.S_NORM || answer == 7)
            {

                ioclass.Do_send_clear(c);
                return;
            }
            ioclass.Do_send_clear(c);

            switch (answer)
            {

                case 0:
                    event_ptr = eventclass.Do_create_event();
                    event_ptr.type = (short)phantdefs.CINC_EVENT;
                    event_ptr.arg1 = 0;
                    CFUNCTIONS.strcpy(ref string_buffer2, "gems");

                    if (ioclass.Do_long_dialog(c, ref event_ptr.arg3, "How much would you like to increase for gems?\n") != 0)
                    {


                        event_ptr = null; //free((void*) event_ptr);
                        return;
                    }

                    if (event_ptr.arg3 <= 0)
                    {

                        event_ptr = null; //free((void*) event_ptr);
                        return;
                    }

                    break;

                case 1:
                    event_ptr = eventclass.Do_create_event();
                    event_ptr.type = (short)phantdefs.CINC_EVENT;
                    event_ptr.arg1 = 1;
                    CFUNCTIONS.strcpy(ref string_buffer2, "gold");

                    if (ioclass.Do_long_dialog(c, ref event_ptr.arg3, "How much would you like to increase for gold?\n") != 0)
                    {


                        event_ptr = null; //free((void*) event_ptr);
                        return;
                    }

                    if (event_ptr.arg3 <= 0)
                    {

                        event_ptr = null; //free((void*) event_ptr);
                        return;
                    }

                    break;


                default:
                    error_msg = CFUNCTIONS.sprintfSinglestring("[%s] Returned non-option in Do_currencyrestore.\n", c.connection_id);


                    fileclass.Do_log_error(error_msg);

                    hackclass.Do_caught_hack(c, phantdefs.H_SYSTEM);
                    return;
            }

            /* if we're here, we have an event to send to another player */
            /* prompt for player to affect */
            string_buffer = CFUNCTIONS.sprintfSinglestring("Whose %s do you want to increase?\n", string_buffer2);
            if (ioclass.Do_player_dialog(c, string_buffer, string_buffer2) != phantdefs.S_NORM)
            {
                event_ptr = null; //free((void*) event_ptr);
                return;
            }

            event_ptr.arg4 = "";// (void*)Do_malloc(cfunctions.strlen(c.modifiedName) + 1);
            string arg4 = (string)event_ptr.arg4;
            CFUNCTIONS.strcpy(ref arg4, c.modifiedName);
            event_ptr.arg4 = arg4;
            event_ptr.from = c.game;

            if (eventclass.Do_send_character_event(c, event_ptr, string_buffer2) == 0)
            {
                event_ptr.arg4 = null; //free((void*) event_ptr.arg4);
                event_ptr = null; //free((void*) event_ptr);
                ioclass.Do_send_line(c, "That character just left the game.\n");
                ioclass.Do_more(c);
                return;
            }

            ioclass.Do_send_line(c, "Currency restore complete.\n");
            ioclass.Do_more(c);
            ioclass.Do_send_clear(c);
            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_cincreceive(struct client_t *c, struct event_t *the_event)
        /
        / FUNCTION: do the currency increase 
        /
        / AUTHOR: Samuel T,	8/22/02
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
        / DESCRIPTION: Increases the currency set up by the supreme wizard
        /
        *************************************************************************/

        internal void Do_cincreceive(client_t c, event_t the_event)
        {
            string string_buffer = ""; //[phantdefs.SZ_LINE];
            double dtemp;
            long answer = -1;
            long increase;

            answer = (long)the_event.arg1;
            increase = the_event.arg3;

            switch (answer)
            {

                case 0:
                    c.player.gems = c.player.gems + increase;

                    miscclass.Do_check_weight(c);


                    socketclass.Do_send_int(c, phantdefs.GEMS_PACKET);
                    socketclass.Do_send_double(c, c.player.gems);
                    break;

                case 1:
                    c.player.gold = c.player.gold + increase;

                    miscclass.Do_check_weight(c);


                    socketclass.Do_send_int(c, phantdefs.GOLD_PACKET);

                    socketclass.Do_send_double(c, c.player.gold);
                    break;

            }


            string_buffer = CFUNCTIONS.sprintfSinglestring("%s's casts a currency spell on you, you feel heavier!\n",
                   the_event.arg4);

            ioclass.Do_send_line(c, string_buffer);

            string_buffer = CFUNCTIONS.sprintfSinglestring("%s casted an currency spell on %s.\n", the_event.arg4, c.player.lcname);


            fileclass.Do_log(pathnames.CRESTORE_LOG, string_buffer);

            the_event.arg4 = null; //free((void*) the_event.arg4);

            ioclass.Do_more(c);
            ioclass.Do_send_clear(c);

            return;
        }

        /************************************************************************
        /
        / FUNCTION NAME: Do_ageincreceive(struct client_t *c, struct event_t *the_event)
        /
        / FUNCTION: do the age increase 
        /
        / AUTHOR: Samuel T,  8/22/02
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
        / DESCRIPTION: Increases the age set up by the supreme wizard
        /
        *************************************************************************/

        internal void Do_ageincreceive(client_t c, event_t the_event)
        {
            string string_buffer = ""; //[phantdefs.SZ_LINE];
            double dtemp;
            long increase;

            increase = the_event.arg3;

            c.player.age = (int)(c.player.age + increase);


            string_buffer = CFUNCTIONS.sprintfSinglestring("%s's casts an aging spell on you, you feel older!\n",
                   the_event.arg4);

            ioclass.Do_send_line(c, string_buffer);

            string_buffer = CFUNCTIONS.sprintfSinglestring("%s casted an aging spell on %s.\n", the_event.arg4, c.player.lcname);


            fileclass.Do_log(pathnames.AGERESTORE_LOG, string_buffer);

            the_event.arg4 = null; //free((void*) the_event.arg4);

            ioclass.Do_more(c);
            ioclass.Do_send_clear(c);

            return;
        }

        /************************************************************************
        /
        / FUNCTION NAME: Do_itemsrestore(struct client_t *c)
        /
        / FUNCTION: Item Restore Options
        /
        / AUTHOR: Samuel T, 8/22/02
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
        / DESCRIPTION: Restore Items
        /
        *************************************************************************/

        void Do_itemsrestore(client_t c)
        {
            event_t event_ptr = new event_t();
            button_t theButtons = new button_t();
            string string_buffer = ""; //[phantdefs.SZ_LINE], 
            string string_buffer2 = ""; //[phantdefs.SZ_LINE];
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            long answer = -1;              /* pointer to option description */

            ioclass.Do_clear_buttons(theButtons, 0);

            /* determine what the supreme wizard wants to give */
            CFUNCTIONS.strcpy(ref theButtons.button[0], "Bless\n");
            CFUNCTIONS.strcpy(ref theButtons.button[1], "Palintir\n");
            CFUNCTIONS.strcpy(ref theButtons.button[2], "Virgin\n");
            CFUNCTIONS.strcpy(ref theButtons.button[3], "Normal Ring\n");
            CFUNCTIONS.strcpy(ref theButtons.button[4], "The One Ring\n");
            ioclass.Do_clear_buttons(theButtons, 5);
            ioclass.Do_clear_buttons(theButtons, 6);
            CFUNCTIONS.strcpy(ref theButtons.button[7], "Cancel\n");

            ioclass.Do_send_line(c, "What item restore command do you wish to use?\n");

            if (ioclass.Do_buttons(c, ref answer, theButtons) != phantdefs.S_NORM || answer == 7)
            {

                ioclass.Do_send_clear(c);
                return;
            }
            ioclass.Do_send_clear(c);

            switch (answer)
            {

                case 0:
                    event_ptr = eventclass.Do_create_event();
                    event_ptr.type = (short)phantdefs.ITEMINC_EVENT;
                    event_ptr.arg1 = 0;
                    CFUNCTIONS.strcpy(ref string_buffer2, "bless");

                    break;

                case 1:
                    event_ptr = eventclass.Do_create_event();
                    event_ptr.type = (short)phantdefs.ITEMINC_EVENT;
                    event_ptr.arg1 = 1;
                    CFUNCTIONS.strcpy(ref string_buffer2, "palintir");

                    break;

                case 2:
                    event_ptr = eventclass.Do_create_event();
                    event_ptr.type = (short)phantdefs.ITEMINC_EVENT;
                    event_ptr.arg1 = 2;
                    CFUNCTIONS.strcpy(ref string_buffer2, "virgin");

                    break;

                case 3:
                    event_ptr = eventclass.Do_create_event();
                    event_ptr.type = (short)phantdefs.ITEMINC_EVENT;
                    event_ptr.arg1 = 3;
                    CFUNCTIONS.strcpy(ref string_buffer2, "normal ring");

                    break;

                case 4:
                    event_ptr = eventclass.Do_create_event();
                    event_ptr.type = (short)phantdefs.ITEMINC_EVENT;
                    event_ptr.arg1 = 4;
                    CFUNCTIONS.strcpy(ref string_buffer2, "The One Ring");

                    break;



                default:
                    error_msg = CFUNCTIONS.sprintfSinglestring("[%s] Returned non-option in Do_itemsrestore.\n",
                            c.connection_id);


                    fileclass.Do_log_error(error_msg);

                    hackclass.Do_caught_hack(c, phantdefs.H_SYSTEM);
                    return;
            }

            /* if we're here, we have an event to send to another player */
            /* prompt for player to affect */
            string_buffer = CFUNCTIONS.sprintfSinglestring("Who do you wish to give a %s to?\n", string_buffer2);
            if (ioclass.Do_player_dialog(c, string_buffer, string_buffer2) != phantdefs.S_NORM)
            {
                event_ptr = null; //free((void*) event_ptr);
                return;
            }

            event_ptr.arg4 = "";// (void*)Do_malloc(cfunctions.strlen(c.modifiedName) + 1);
            string arg4 = (string)event_ptr.arg4;
            CFUNCTIONS.strcpy(ref arg4, c.modifiedName);
            event_ptr.arg4 = arg4;
            event_ptr.from = c.game;

            if (eventclass.Do_send_character_event(c, event_ptr, string_buffer2) == 0)
            {
                event_ptr.arg4 = null; //free((void*) event_ptr.arg4);
                event_ptr = null; //free((void*) event_ptr);
                ioclass.Do_send_line(c, "That character just left the game.\n");
                ioclass.Do_more(c);
                return;
            }

            ioclass.Do_send_line(c, "Item restore complete.\n");
            ioclass.Do_more(c);
            ioclass.Do_send_clear(c);
            return;
        }

        /************************************************************************
        /
        / FUNCTION NAME: Do_itemincreceive(struct client_t *c, struct event_t *the_event)
        /
        / FUNCTION: do the currency increase 
        /
        / AUTHOR: Samuel T,	8/22/02
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
        / DESCRIPTION: Increases the currency set up by the supreme wizard
        /
        *************************************************************************/

        internal void Do_itemincreceive(client_t c, event_t the_event)
        {
            string string_buffer = ""; //[phantdefs.SZ_LINE];
            double dtemp;
            long answer = -1;

            answer = (long)the_event.arg1;

            switch (answer)
            {

                case 0:

                    treasureclass.Do_award_blessing(c);
                    break;

                case 1:
                    if (!c.player.palantir)
                    {

                        statsclass.Do_palantir(c, 1, 0);
                    }
                    break;

                case 2:

                    statsclass.Do_virgin(c, 1, 0);
                    break;

                case 3:
                    if (c.player.ring_type == phantdefs.R_NONE)
                    {
                        statsclass.Do_ring(c, phantdefs.R_NAZREG, 0);
                    }
                    break;

                case 4:
                    if (c.player.ring_type != phantdefs.R_NONE)
                    {

                        statsclass.Do_ring(c, phantdefs.R_NONE, 0);
                    }
                    statsclass.Do_ring(c, phantdefs.R_DLREG, 0);
                    break;

            }



            string_buffer = CFUNCTIONS.sprintfSinglestring("%s's casts an item spell on you. You leap for joy!!!\n",
                   the_event.arg4);

            ioclass.Do_send_line(c, string_buffer);

            string_buffer = CFUNCTIONS.sprintfSinglestring("%s casted an item spell on %s.\n", the_event.arg4, c.player.lcname);


            fileclass.Do_log(pathnames.ITEMRESTORE_LOG, string_buffer);

            the_event.arg4 = null; //free((void*) the_event.arg4);

            ioclass.Do_more(c);
            ioclass.Do_send_clear(c);

            return;
        }

    }
}