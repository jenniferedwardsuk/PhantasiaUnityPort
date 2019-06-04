using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Net;

namespace phantasiaclasses
{
    public class character //: MonoBehaviour
    {
        phantasiaclasses.eventsrc eventclass;
        phantasiaclasses.io ioclass;
        phantasiaclasses.file fileclass;
        phantasiaclasses.misc miscclass;
        phantasiaclasses.hack hackclass;
        phantasiaclasses.tags tagsclass;
        phantasiaclasses.account accountclass;
        phantasiaclasses.stats statsclass;
        phantasiaclasses.commands commandsclass;
        phantasiaclasses.socket socketclass;
        phantasiaclasses.info infoclass;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        static character Instance;
        private character()
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
            tagsclass = tags.GetInstance();
            eventclass = eventsrc.GetInstance();
            accountclass = account.GetInstance();
            statsclass = stats.GetInstance();
            commandsclass = commands.GetInstance();
            socketclass = socket.GetInstance();
            infoclass = info.GetInstance();
        }
        public static character GetInstance()
        {
            character instance = null;
            if (Instance != null)
            {
                instance = Instance;
            }
            else
            {
                instance = new character();
            }
            return instance;
        }

        char Do_player_special_type(short special_type, int sex) // male = 0 female = 1
        {
            /* character descriptions */
            char[] results = new char[] { ' ', 'N', 'S', 'K', 'C', 'X', 'V' };

            if (sex == phantdefs.FEMALE)
            {
                results[phantdefs.SC_KING] = 'Q';
            }

            return results[special_type];
        }

        int Do_character_playing(client_t c, string the_name)
        {
            game_t game_ptr;

            miscclass.Do_lock_mutex(c.realm.realm_lock);

            /* run though all the games and check the names */
            game_ptr = c.realm.games;
            while (game_ptr != null)
            {
                /* check for a name match */
                if (game_ptr.description != null &&
                !CFUNCTIONS.strcmp(the_name, game_ptr.description.lcname))
                {
                    miscclass.Do_unlock_mutex(c.realm.realm_lock);
                    return 1;
                }
                game_ptr = game_ptr.next_game;
            }

            miscclass.Do_unlock_mutex(c.realm.realm_lock);
            return 0;
        }


        internal player_t Do_copy_record(player_t orig)
        {
            player_t player_ptr;

            /* create a new player record */
            //player_ptr = new player_t();// (player_t)Do_malloc(phantdefs.SZ_PLAYER); //unity: unneeded

            /* copy the information */
            player_ptr = new player_t(orig);

            /* return the pointer */
            return player_ptr;
        }
        
        player_desc_t Do_make_description(client_t c)
        {
            player_desc_t desc_ptr;

            /* allocate for the new description */
            desc_ptr = new player_desc_t(); //(struct player_desc_t *) Do_malloc(phantdefs.SZ_PLAYER_DESC);

            /* copy the information */
            desc_ptr.name = c.modifiedName;
            desc_ptr.lcname = c.player.lcname;
            desc_ptr.parent_account = c.player.parent_account;
            desc_ptr.type = c.player.type;
            desc_ptr.special_type = c.player.special_type;
            desc_ptr.gender = c.player.gender;

            desc_ptr.level = c.player.level;
            desc_ptr.channel = c.channel;
            desc_ptr.cloaked = c.player.cloaked;
            desc_ptr.palantir = c.player.palantir;
            desc_ptr.blind = c.player.blind;

            desc_ptr.wizard = c.wizard;
            /*
                if (c.wizard > 2) {
                    desc_ptr.wizard = true;
                }
                else {
                    desc_ptr.wizard = false;
                }
            */

            return desc_ptr;
        }
        
        internal void Do_player_description(client_t c)
        {
            /* WARNING: Realm should be locked before calling this function */

            /* free the old description, if necessary */
            if (c.game.description != null)
            {
                c.game.description = null;
            }

            /* create a new one */
            c.game.description = Do_make_description(c);

            return;
        }


        player_spec_t Do_make_specification(client_t c)
        {
            player_spec_t spec_ptr;

            /* allocate for the new description */
            spec_ptr = new player_spec_t();// (struct player_spec_t *) Do_malloc(phantdefs.SZ_PLAYER_SPEC);

            /* copy the information */
            spec_ptr.name = c.modifiedName;
            CFUNCTIONS.strcat(ref spec_ptr.name,"\n");

            spec_ptr.type[0] = c.realm.charstats[c.player.type].short_class_name;

            if (c.wizard > 2)
            {
                spec_ptr.type[1] = 'W';
                spec_ptr.type[2] = ' ';
                spec_ptr.type[3] = '-';
                spec_ptr.type[4] = ("" + (c.channel + 48)).ToCharArray()[0];
                spec_ptr.type[5] = '\n';
                spec_ptr.type[6] = '\0';
            }
            else if (c.player.special_type == phantdefs.SC_NONE)
            {
                if (c.wizard == 2)
                {
                    spec_ptr.type[1] = 'A';
                    spec_ptr.type[2] = ' ';
                    spec_ptr.type[3] = '-';
                    spec_ptr.type[4] = ("" + (c.channel + 48)).ToCharArray()[0];
                    spec_ptr.type[5] = '\n';
                    spec_ptr.type[6] = '\0';
                }
                else
                {
                    spec_ptr.type[1] = ' ';
                    spec_ptr.type[2] = '-';
                    spec_ptr.type[3] = ("" + (c.channel + 48)).ToCharArray()[0];
                    spec_ptr.type[4] = '\n';
                    spec_ptr.type[5] = '\0';
                }
            }
            else
            {
                spec_ptr.type[1] = Do_player_special_type(c.player.special_type, genderToInt(c.player.gender));
                spec_ptr.type[2] = ' ';
                spec_ptr.type[3] = '-';
                spec_ptr.type[4] = ("" + (c.channel + 48)).ToCharArray()[0];
                spec_ptr.type[5] = '\n';
                spec_ptr.type[6] = '\0';
            }

            /* return the pointer */
            return spec_ptr;
        }

        int genderToInt(bool gender)
        {
            int gendnum;
            if (gender)
                gendnum = 1;
            else
                gendnum = 0;
            return gendnum;
        }


        internal void Do_send_specification(client_t c, long type)
        {
            event_t event_ptr;

            /* create a new event */
            event_ptr = new event_t();// (struct event_t *) eventclass.Do_create_event();

            /* fill it out */
            event_ptr.type = (short)type;
            event_ptr.arg3 = (long) phantdefs.SZ_PLAYER_SPEC;
            event_ptr.arg4 = Do_make_specification(c);
            event_ptr.from = c.game;

            /* send the event to everyone */
            eventclass.Do_broadcast_event(c, event_ptr);

            return;
        }
        
        internal void Do_starting_spec(client_t c)
        {
            /* WARNING: the realm should be locked before calling this function */
            string string_buffer = ""; //new char = ""; //[phantdefs.SZ_LINE];
            game_t game_ptr = new game_t();
            player_spec_t spec_ptr = new player_spec_t();
            event_t event_ptr = new event_t();

            /* run through the list of games */
            game_ptr = c.realm.games;
            while (game_ptr != null)
            {

                /* see if the game is currently playing */
                if (game_ptr.description != null)
                {

                    /* create a spec for this player */
                    spec_ptr = new player_spec_t(); //spec_ptr = (struct player_spec_t *) Do_malloc(phantdefs.SZ_PLAYER_SPEC);

                    /* copy the information */
                    spec_ptr.name = game_ptr.description.name;
                    CFUNCTIONS.strcat(ref spec_ptr.name, "\n");

                    spec_ptr.type[0] = c.realm.charstats[game_ptr.description.type].short_class_name;

                    if (game_ptr.description.wizard > 2)
                    {
                        spec_ptr.type[1] = 'W';
                        spec_ptr.type[2] = ' ';
                        spec_ptr.type[3] = '-';
                        spec_ptr.type[4] = ("" + (game_ptr.description.channel + 48)).ToCharArray()[0];
                        spec_ptr.type[5] = '\n';
                        spec_ptr.type[6] = '\0';
                    }
                    else if (game_ptr.description.special_type == 1)
                    {
                        spec_ptr.type[1] = Do_player_special_type(
                        game_ptr.description.special_type,
                        genderToInt(game_ptr.description.gender));

                        spec_ptr.type[2] = ' ';
                        spec_ptr.type[3] = '-';
                        spec_ptr.type[4] = ("" + (game_ptr.description.channel + 48)).ToCharArray()[0];
                        spec_ptr.type[5] = '\n';
                        spec_ptr.type[6] = '\0';
                    }
                    else if (game_ptr.description.wizard == 2)
                    {
                        spec_ptr.type[1] = 'A';
                        spec_ptr.type[2] = ' ';
                        spec_ptr.type[3] = '-';
                        spec_ptr.type[4] = ("" + (game_ptr.description.channel + 48)).ToCharArray()[0];
                        spec_ptr.type[5] = '\n';
                        spec_ptr.type[6] = '\0';
                    }
                    else
                    {
                        spec_ptr.type[1] = ' ';
                        spec_ptr.type[2] = '-';
                        spec_ptr.type[3] = ("" + (game_ptr.description.channel + 48)).ToCharArray()[0];
                        spec_ptr.type[4] = '\n';
                        spec_ptr.type[5] = '\0';
                    }

                    /* create a new event */
                    event_ptr = eventclass.Do_create_event();

                    /* fill it out */
                    event_ptr.type = (short)phantdefs.ADD_PLAYER_EVENT;
                    event_ptr.arg3 = (long)phantdefs.SZ_PLAYER_SPEC;
                    event_ptr.arg4 = spec_ptr;
                    event_ptr.from = game_ptr;

                    /* send the event */
                    eventclass.Do_file_event(c, event_ptr);
                }

                /* move to the next game */
                game_ptr = game_ptr.next_game;
            }

            return;
        }
        
        internal void Do_get_character(client_t c)
        {
            button_t buttons = new button_t();
            string error_msg = "";//new char = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            int i, rc;
            long answer = -1;

            for (; ; )
            {
                if (UnityGameController.StopApplication) //time to quit
                {
                    Debug.Log("Thread " + System.Threading.Thread.CurrentThread.Name + " stopping in Do_get_character");
                    return;
                }
                else
                {
                    System.Threading.Thread.Sleep(33); //30fps
                }

                /* see if the player has a saved game */
                ioclass.Do_send_line(c, "What do you wish to do?\n");

                buttons.button[0] = "New Char\n";
                buttons.button[1] = "Load Char\n";
                ioclass.Do_clear_buttons(buttons, 2);
                buttons.button[3] = "Characters\n";
                buttons.button[4] = "Account\n";
                buttons.button[6] = "Scoreboard\n";
                buttons.button[7] = "Quit\n";

                rc = ioclass.Do_buttons(c, ref answer, buttons);
                ioclass.Do_send_clear(c);

                if (rc != phantdefs.S_NORM)
                {
                    answer = 7;
                }

                /* switch on the player's answer */
                switch (answer)
                {
                    /* if the player has a character to run */
                    case 1:

                        /* go recall the player */
                        Do_recall_player(c);
                        break;

                    /* if the player does not have a character to run */
                    case 0:

                        /* create a new character */
                        Do_roll_new_player(c);
                        break;

                    /* character options */
                    case 3:

                        Do_character_options(c);
                        break;

                    /* account options */
                    case 4:

                        accountclass.Do_account_options(c);
                        break;

                    /* show the character the scoreboard */
                    case 6:

                        miscclass.Do_scoreboard(c, 0);
                        break;

                    /* exit if requested */
                    case 7:

                        c.run_level = (short)phantdefs.EXIT_THREAD;
                        break;

                    /* since it's a push button interface,
                        any other answer is a hacker */
                    default:

                        error_msg = CFUNCTIONS.sprintfSinglestring("[%s] Returned non-option in Do_get_character.\n", c.connection_id);

                        fileclass.Do_log_error(error_msg);
                        hackclass.Do_caught_hack(c, phantdefs.H_SYSTEM);
                        break;
                }
                return;
            }
        }

        void Do_roll_new_player(client_t c)
        {
            charstats_t stat_ptr = new charstats_t();
            game_t game_ptr = new game_t();
            int i, theMask, suffixFlag;
            double x, y;
            char answer;
            string string_buffer = ""; //new char = ""; //[phantdefs.SZ_LINE];
            string string_buffer2 = "";// new char = ""; //[phantdefs.SZ_LINE];
            button_t buttons = new button_t();
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            long theAnswer = -1;
            CLibFile.CFILE liar_file;
            IPAddress theNetwork;

            /* muted characters would make characters with message names */
            if (c.muteUntil > CFUNCTIONS.GetUnixEpoch(DateTime.Now))
            {
                ioclass.Do_send_line(c, "Muted players may not create characters.\n");
                ioclass.Do_more(c);
                ioclass.Do_send_clear(c);

                return;
            }

            /* get the character type */
            ioclass.Do_send_line(c, "Which type of character do you want?\n");

            for (i = 0; i < phantdefs.NUM_CHARS; i++)
            {
                /* copy over the class name */
                buttons.button[i] = CFUNCTIONS.sprintfSinglestring("%s\n", c.realm.charstats[i].class_name);
            }

            ioclass.Do_clear_buttons(buttons, phantdefs.NUM_CHARS);
            buttons.button[7] = "Cancel\n";

            /* send the information */
            if (ioclass.Do_buttons(c, ref theAnswer, buttons) != phantdefs.S_NORM || theAnswer == 7)
            {
                ioclass.Do_send_clear(c);
                return;
            }

            ioclass.Do_send_clear(c);
            c.player.type = (short)theAnswer;

            /* if the information is out of range, we have a hacker */
            if (c.player.type < 0 && c.player.type >= phantdefs.NUM_CHARS)
            {
                error_msg = CFUNCTIONS.sprintfSinglestring("[%s] Returned non-option in Do_roll_new_player.\n",
                c.connection_id);
                fileclass.Do_log_error(error_msg);
                hackclass.Do_caught_hack(c, phantdefs.H_SYSTEM);
                return;
            }

            stat_ptr = c.realm.charstats[c.player.type];

            for (; ; )
            {
                if (UnityGameController.StopApplication) //time to quit
                {
                    Debug.Log("Thread " + System.Threading.Thread.CurrentThread.Name + " stopping in Do_roll_new_player");
                    return;
                }
                else
                {
                    System.Threading.Thread.Sleep(33); //30fps
                }

                /* give a random x and y */
                x = 0;
                y = 0;
                miscclass.Do_move_close(ref x, ref y, phantdefs.D_CIRCLE - 1);

                if (c.player.type != phantdefs.C_EXPER)
                {
                    statsclass.Do_location(c, x, y, 0);
                }

                c.player.energy = macros.ROLL(stat_ptr.energy.statbase, stat_ptr.energy.interval);

                statsclass.Do_energy(c, c.player.energy, c.player.energy, 0.0, 0.0, 0);

                statsclass.Do_strength(c, macros.ROLL(stat_ptr.strength.statbase, stat_ptr.strength.interval), 0.0, 0.0, 0);

                statsclass.Do_speed(c, macros.ROLL(stat_ptr.quickness.statbase, stat_ptr.quickness.interval), 0.0, 0.0, 0);

                statsclass.Do_mana(c, macros.ROLL(stat_ptr.mana.statbase, stat_ptr.mana.interval) - c.player.mana, 0);

                /* set the gold manually so no taxes are taken */
                c.player.gold = macros.ROLL(50.0, 75.0);
                statsclass.Do_gold(c, 0.0, 1);

                c.player.brains = macros.ROLL(stat_ptr.brains.statbase, stat_ptr.brains.interval);

                c.player.magiclvl = macros.ROLL(stat_ptr.magiclvl.statbase, stat_ptr.magiclvl.interval);

                /* experimento's have fixed stats */
                if (c.player.type == phantdefs.C_EXPER)
                    break;

                string_buffer = CFUNCTIONS.sprintfSinglestring("Brains      : %2.0f\n", c.player.brains);
                ioclass.Do_send_line(c, string_buffer);

                string_buffer = CFUNCTIONS.sprintfSinglestring("Magic Level : %2.0f\n", c.player.magiclvl);
                ioclass.Do_send_line(c, string_buffer);

                if (c.player.type == phantdefs.C_HALFLING)
                {
                    /* give halfling some experience */
                    c.player.experience = 0;
                    statsclass.Do_experience(c, macros.ROLL(600.0, 200.0), 0);

                    string_buffer = CFUNCTIONS.sprintfSinglestring("Experience  : %2.0f\n",
                    c.player.experience);

                    ioclass.Do_send_line(c, string_buffer);
                }

                /* see if the player wants to keep the character */
                ioclass.Do_send_line(c, "Do you wish to keep these stats?\n");

                buttons.button[0] = "Reroll\n";
                buttons.button[1] = "Keep\n";
                ioclass.Do_clear_buttons(buttons, 2);
                buttons.button[7] = "Cancel\n";

                /* see if the game is shutting down */
                miscclass.Do_shutdown_check(c);

                if (ioclass.Do_buttons(c, ref theAnswer, buttons) != phantdefs.S_NORM || theAnswer == 7)
                {
                    ioclass.Do_send_clear(c);
                    return;
                }
                else if (theAnswer == 1)
                {
                    ioclass.Do_send_clear(c);
                    break;
                }
                else if (theAnswer != 0)
                {
                    ioclass.Do_send_clear(c);
                    error_msg = CFUNCTIONS.sprintfSinglestring("[%s] Returned non-option in Do_roll_new_player(2).\n", c.connection_id);
                    fileclass.Do_log_error(error_msg);
                    hackclass.Do_caught_hack(c, phantdefs.H_SYSTEM);
                    return;
                }
                ioclass.Do_send_clear(c);
            }

            /* get coordinates for experimento */
            if (c.player.type == phantdefs.C_EXPER)
            {

                for (; ; )
                {
                    if (UnityGameController.StopApplication) //time to quit
                    {
                        Debug.Log("Thread " + System.Threading.Thread.CurrentThread.Name + " stopping in Do_roll_new_player");
                        return;
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(33); //30fps
                    }

                    if (ioclass.Do_coords_dialog(c, ref x, ref y,
                        "Enter the approximate X Y coordinates of your experimento?\n") != 0)
                    {
                        return;
                    }

                    if (Mathf.Abs((float)x) > phantdefs.D_EXPER || Mathf.Abs((float)y) > phantdefs.D_EXPER)
                    {

                        ioclass.Do_send_line(c, "Experimento starting coordinates must be between -2000 and 2000.  Please try again.\n");

                        ioclass.Do_more(c);
                        ioclass.Do_send_clear(c);
                    }
                    else
                    {
                        break;
                    }
                }

                /* experimentos never quite start where they wish */
                miscclass.Do_move_close(ref x, ref y, 5);

                if (x == 0 && y == 0)
                {
                    miscclass.Do_move_close(ref x, ref y, 0);
                }

                statsclass.Do_location(c, x, y, 0);
            }

            for (; ; )
            {
                if (UnityGameController.StopApplication) //time to quit
                {
                    Debug.Log("Thread " + System.Threading.Thread.CurrentThread.Name + " stopping in Do_roll_new_player");
                    return;
                }
                else
                {
                    System.Threading.Thread.Sleep(33); //30fps
                }

                /* name the new character */
                string_buffer = CFUNCTIONS.sprintfSinglestring("Give your character a name. [up to %d characters]\n", phantdefs.MAX_NAME_LEN);

                if (ioclass.Do_string_dialog(c, ref c.player.name, phantdefs.SZ_NAME - 1, string_buffer))
                    return;

                /* see if the name is approved */
                miscclass.Do_lowercase(ref c.player.lcname, c.player.name);

                int inttheAnswer = (int)theAnswer;
                if (Do_approve_name(c, c.player.lcname, c.player.name, ref inttheAnswer) != phantdefs.S_NORM)
                {
                    return;
                }
                theAnswer = (long)inttheAnswer;

                if (theAnswer != 0)
                {
                    break;
                }
            }

            c.modifiedName = c.player.name;
            statsclass.Do_name(c);

            /* determine the sex of the charcter */
            ioclass.Do_send_line(c, "Is the character male or female?\n");

            buttons.button[0] = "Male\n";
            buttons.button[1] = "Female\n";
            ioclass.Do_clear_buttons(buttons, 2);
            buttons.button[7] = "Cancel\n";

            if (ioclass.Do_buttons(c, ref theAnswer, buttons) != phantdefs.S_NORM || theAnswer == 7)
            {
                Do_release_name(c, c.player.lcname);
                ioclass.Do_send_clear(c);
                return;
            }

            ioclass.Do_send_clear(c);

            if (theAnswer == 1)
                c.player.gender = true; // phantdefs.FEMALE;
            else
                c.player.gender = false; // phantdefs.MALE;

            /* get a password for character */
            if (!ioclass.Do_new_password(c, c.player.password, "character"))
            {
                Do_release_name(c, c.player.lcname);
                return;
            }

            /* creation complete */
            c.run_level = (short)phantdefs.PLAY_GAME;
            c.player.date_created = CFUNCTIONS.time(null); //GetUnixEpoch(DateTime.Now); // 

            /* put a description in place */
            miscclass.Do_lock_mutex(c.realm.realm_lock);
            Do_player_description(c);
            miscclass.Do_unlock_mutex(c.realm.realm_lock);

            /* remove the name in limbo now the description is in place */
            Do_release_name(c, c.player.lcname);

            /* log the creation */
            string_buffer = CFUNCTIONS.sprintfSinglestring("[%s] %s created by %s\n", c.connection_id, c.player.lcname, c.lcaccount);

            fileclass.Do_log(pathnames.GAME_LOG, string_buffer);

            string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, 0 age, 0 seconds, level 0\n", c.player.lcname, c.realm.charstats[c.player.type].class_name);

            fileclass.Do_log(pathnames.LEVEL_LOG, string_buffer);

            return;
        }
        
        void Do_recall_player(client_t c)
        {
            string characterName = ""; //[phantdefs.SZ_NAME], 
            string string_buffer = ""; //[phantdefs.SZ_LINE];
            string lcCharacterName = ""; //[phantdefs.SZ_NAME];
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            player_mod_t theMod = new player_mod_t();
            long answer = -1;
            int exceptionFlag = 0, wizType = 0;
            string wizNetwork = ""; //[phantdefs.SZ_FROM],
            string wizAccount = ""; //[phantdefs.SZ_NAME],
            string wizCharacter = ""; //[phantdefs.SZ_NAME];
            CLibFile.CFILE wizard_file;

            /* open the wizard file to see if this person is one */
            //wizard_file = CLibFile.FileOpenRead(pathnames.WIZARD_FILE);
            int errno = 0;
            if ((wizard_file = CLibFile.fopen(pathnames.WIZARD_FILE, "r", ref errno)) == null)
            {
                string_buffer = CFUNCTIONS.sprintfSinglestring("[%s] CLibFile.fopen of %s failed in Do_recall_player: %s\n", c.connection_id, pathnames.WIZARD_FILE, CFUNCTIONS.strerror(errno));

                fileclass.Do_log_error(string_buffer);
            }
            else
            {
                /* loop through the the names */
                int wizTypeInt = (int)answer;
                string[] extractedValues;
                while ((extractedValues = CLibFile.fscanf(wizard_file, "%ld %s %s %s %d\n", wizTypeInt, wizNetwork, wizAccount, wizCharacter, exceptionFlag)).Length == 5)
                {
                    //unity: populating extracted values manually due to param/ref argument restrictions
                    //exceptionFlag is unused anyway
                    wizTypeInt = Convert.ToInt32(extractedValues[0]);
                    wizNetwork = (extractedValues[1]);
                    wizAccount = (extractedValues[2]);
                    wizCharacter = (extractedValues[3]);
                    exceptionFlag = Convert.ToInt32(extractedValues[4]);

                    answer = (long)wizTypeInt;
                    /* put down the highest wizType */
                    if (!CFUNCTIONS.strcmp(wizAccount, c.lcaccount) && !CFUNCTIONS.strcmp(wizNetwork, c.network))
                    {
                        if (answer > wizType)
                        {
                            wizType = (int)answer;
                        }
                    }
                }
            }
            CLibFile.fclose(wizard_file);

            for (; ; )
            {
                if (UnityGameController.StopApplication) //time to quit
                {
                    Debug.Log("Thread " + System.Threading.Thread.CurrentThread.Name + " stopping in Do_recall_player");
                    return;
                }
                else
                {
                    System.Threading.Thread.Sleep(33); //30fps
                }

                /* prompt for the character */
                if (ioclass.Do_string_dialog(c, ref characterName, phantdefs.SZ_NAME - 1, "What is the name of the character?\n"))
                {

                    return;
                }

                /* load the character information */
                miscclass.Do_lowercase(ref lcCharacterName, characterName);
                ioclass.Do_send_line(c, "Looking up character information...\n");
                socketclass.Do_send_buffer(c);

                if (!CFUNCTIONS.strcmp("", lcCharacterName))
                {
                }
                else if (Do_look_character(c, lcCharacterName, ref c.player) != 0)
                {
                    ioclass.Do_send_clear(c);
                    break;
                }

                ioclass.Do_send_clear(c);

                /* see if the character is playing */
                if (Do_character_playing(c, lcCharacterName) != 0)
                {
                    ioclass.Do_send_line(c, "That character is currently in the game.  If you were just playing this character and was disconnected, wait a minute and the character will either be saved or killed.\n");
                }
                else
                {

                    string_buffer = CFUNCTIONS.sprintfSinglestring("I can not find a character named \"%s\".  Please check the spelling and try again.\n", characterName);

                    ioclass.Do_send_line(c, string_buffer);
                }

                ioclass.Do_more(c);
                ioclass.Do_send_clear(c);
            }

            /* if character is faithful, make sure this is parent account */
            /* allow wizards to load protected characters */
            if (wizType < 3 && c.player.faithful && CFUNCTIONS.strcmp(c.player.parent_account, c.lcaccount))
            {

                /* this is a possible hack attempt, so log it */
                Do_clear_character_mod(theMod);
                theMod.badPassword = true;
                Do_modify_character(c, lcCharacterName, theMod);

                string_buffer = CFUNCTIONS.sprintfSinglestring("The character named \"%s\" can only be loaded from the account that created it.\n", characterName);

                ioclass.Do_send_line(c, string_buffer);
                ioclass.Do_more(c);
                ioclass.Do_send_clear(c);
                return;
            }

            /* found character - now get the password */
            if (ioclass.Do_request_character_password(c, c.player.password, c.player.name, c.player.lcname, wizType) == 0)
            {

                return;
            }

            /* put a description in place - now so nobody can use the name */
            miscclass.Do_lock_mutex(c.realm.realm_lock);
            Do_player_description(c);
            miscclass.Do_unlock_mutex(c.realm.realm_lock);

            /* load the character */
            ioclass.Do_send_line(c, "Loading the character...\n");
            socketclass.Do_send_buffer(c);
            if (Do_load_character(c, lcCharacterName) == 0)
            {
                /* if false returns, the character was not loaded */
                ioclass.Do_send_clear(c);

                /* erase the description just put in */
                miscclass.Do_lock_mutex(c.realm.realm_lock);
                CFUNCTIONS.free(ref c.game.description);
                miscclass.Do_unlock_mutex(c.realm.realm_lock);

                /* inform the user */
                string_buffer = CFUNCTIONS.sprintfSinglestring("The character %s is no longer in the character file.  This is normally because the character was just loaded by someone else.\n", c.player.name);

                ioclass.Do_send_line(c, string_buffer);
                ioclass.Do_more(c);
                ioclass.Do_send_clear(c);
                return;
            }

            ioclass.Do_send_clear(c);

            /* backup the character */
            Do_backup_save(c, 1);

            /* import the character to this account if none */
            if (c.player.parent_account != null && c.player.parent_account[0] == '\0')
            {

                c.player.parent_account = c.lcaccount;
                c.player.parent_network = c.network;
                c.player.faithful = true;

                string_buffer = CFUNCTIONS.sprintfSinglestring("The character %s has been imported into account %s.  Future modifications to this character must be done through this account.\n", c.player.name, c.account);

                ioclass.Do_send_line(c, string_buffer);
                ioclass.Do_more(c);
                ioclass.Do_send_clear(c);
            }

            c.run_level = (short)phantdefs.PLAY_GAME;

            /* log the load */
            string_buffer = CFUNCTIONS.sprintfSinglestring("[%s] %s loaded by %s\n", c.connection_id, c.player.lcname, c.lcaccount);

            fileclass.Do_log(pathnames.GAME_LOG, string_buffer);

            return;
        }


        internal int Do_look_character(client_t c, string the_name, ref player_t thePlayer)
        {
            CLibFile.CFILE character_file;
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];

            miscclass.Do_lock_mutex(c.realm.character_file_lock);

            int errno = 0;
            if ((character_file = CLibFile.fopen(pathnames.CHARACTER_FILE, "r", ref errno)) == null)
            {

                miscclass.Do_unlock_mutex(c.realm.character_file_lock);
                error_msg = CFUNCTIONS.sprintfSinglestring("[%s] CLibFile.fopen of %s failed in Do_look_character: %s\n", c.connection_id, pathnames.CHARACTER_FILE, CFUNCTIONS.strerror(errno));

                fileclass.Do_log_error(error_msg);
                return 0;
            }
            else
            {
                /* loop through the the files */
                while (CLibFile.fread(ref thePlayer, phantdefs.SZ_PLAYER, 1, character_file) == 1)
                {
                    string filter1 = thePlayer.lcname.Replace('\0', '$');
                    string filter2 = the_name.Replace('\0', '$');
                    //Debug.LogError("Look character debug: || " + filter1 + " || " + filter2 + " ||");

                    if (!CFUNCTIONS.strcmp(thePlayer.lcname, the_name))
                    {
                        CLibFile.fclose(character_file);
                        miscclass.Do_unlock_mutex(c.realm.character_file_lock);
                        return 1;
                    }
                }
            }
            CLibFile.fclose(character_file);
            miscclass.Do_unlock_mutex(c.realm.character_file_lock);
            return 0;
        }


        int Do_load_character(client_t c, string the_name)
        {
            player_t readPlayer = new player_t();
            CLibFile.CFILE character_file, temp_file;
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            int timeNow;
            bool char_flag;

            miscclass.Do_lock_mutex(c.realm.character_file_lock);

            int errno = 0;
            if ((character_file=CLibFile.fopen(pathnames.CHARACTER_FILE, "r", ref errno)) == null)
            {
                miscclass.Do_unlock_mutex(c.realm.character_file_lock);
                error_msg = CFUNCTIONS.sprintfSinglestring("[%s] CLibFile.fopen of %s failed in Do_load_character: %s\n",
                c.connection_id, pathnames.CHARACTER_FILE, CFUNCTIONS.strerror(errno));

                fileclass.Do_log_error(error_msg);
                return 0;
            }

            /* create a temporary file */
          errno = 0;
            if ((temp_file = CLibFile.fopen(pathnames.TEMP_CHARACTER_FILE, "w", ref errno)) == null)
            {

                error_msg = CFUNCTIONS.sprintfSinglestring("[%s] CLibFile.fopen of %s failed in Do_load_character(2): %s\n", c.connection_id, pathnames.TEMP_CHARACTER_FILE, CFUNCTIONS.strerror(errno));

                fileclass.Do_log_error(error_msg);
                CLibFile.fclose(character_file);
                miscclass.Do_unlock_mutex(c.realm.character_file_lock);
                return 0;
            }

            char_flag = false;
            timeNow = CFUNCTIONS.time(null);

            /* read each line of the character file */
            while (CLibFile.fread(ref readPlayer, phantdefs.SZ_PLAYER, 1, character_file) == 1)
            {

                /* if we find our character, copy it over */
                if (!CFUNCTIONS.strcmp(readPlayer.lcname,the_name))
                {
                    c.player = readPlayer;// phantdefs.SZ_PLAYER);
                    char_flag = true;
                    continue;
                }

                /* see if the character has expired */

                else if ((readPlayer.level < 2) && 
                    (timeNow - readPlayer.last_load > phantdefs.NEWBIE_KEEP_TIME))
                {

                    /* log the deletion of the account */
                    error_msg = CFUNCTIONS.sprintfSinglestring("[%s] %s deleted\n", c.connection_id, readPlayer.lcname);

                    fileclass.Do_log(pathnames.GAME_LOG, error_msg);
                    continue;
                }
                else if ((readPlayer.level >= 2) && (timeNow - readPlayer.last_load > phantdefs.KEEP_TIME + (readPlayer.degenerated * 172800)))
                {

                    /* log the deletion of the account */
                    error_msg = CFUNCTIONS.sprintfSinglestring("[%s] %s deleted\n", c.connection_id, readPlayer.lcname);

                    fileclass.Do_log(pathnames.GAME_LOG, error_msg);
                    continue;
                }

                /* delete characters that are from the future */
                if (timeNow < readPlayer.last_load)
                {

                    /* log the deletion of the account */
                    error_msg = CFUNCTIONS.sprintfSinglestring("[%s] %s from the future and deleted\n", c.connection_id, readPlayer.lcname);

                    fileclass.Do_log(pathnames.GAME_LOG, error_msg);
                    continue;
                }

              errno = 0;
                if (CLibFile.fwrite(readPlayer, ref phantdefs.SZ_PLAYER, 1, temp_file) != 1)
                {

                    error_msg = CFUNCTIONS.sprintfSinglestring("[%s] CLibFile.fwrite of %s failed in Do_load_character: %s\n", c.connection_id, pathnames.TEMP_CHARACTER_FILE, CFUNCTIONS.strerror(errno));

                    fileclass.Do_log_error(error_msg);
                    CLibFile.fclose(character_file);
                    CLibFile.fclose(temp_file);
                    CLibFile.remove(pathnames.TEMP_CHARACTER_FILE);
                    miscclass.Do_unlock_mutex(c.realm.character_file_lock);
                    return 0;
                }
            }

            /* close the file handles */
            CLibFile.fclose(character_file);
            CLibFile.fclose(temp_file);

            /* delete the old character record */
            CLibFile.remove(pathnames.CHARACTER_FILE);  //unity: moved below close call, to avoid sharing violation

            /* replace it with the temporary file */
            CLibFile.rename(pathnames.TEMP_CHARACTER_FILE, pathnames.CHARACTER_FILE);


            miscclass.Do_unlock_mutex(c.realm.character_file_lock);

            /* if the character was found */
            if (char_flag)
            {

                /* There's an overflow with the mute count somewhere -- EH & BK - 1/6/02 */
                if (c.player.muteCount > 6 || c.player.muteCount < 0)
                {
                    c.player.muteCount = 0;
                }

                return 1;
            }

            return 0;
        }


        internal int Do_save_character(client_t c, player_t thePlayer)
        {
            CLibFile.CFILE character_file;
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            string string_buffer = ""; //[phantdefs.SZ_LINE];

            miscclass.Do_lock_mutex(c.realm.character_file_lock);

            int errno = 0;
            if ((character_file = CLibFile.fopen(pathnames.CHARACTER_FILE, "a", ref errno)) == null)
            {

                miscclass.Do_unlock_mutex(c.realm.character_file_lock);

                error_msg = CFUNCTIONS.sprintfSinglestring("[%s] CLibFile.fopen of %s failed in Do_save_character: %s\n", c.connection_id, pathnames.CHARACTER_FILE, "");//  CFUNCTIONS.strerror(errno));

                fileclass.Do_log_error(error_msg);

                ioclass.Do_send_line(c, "There was an error opening the character file to save your character.  Please contact the game administrator with this problem.\n");

                ioclass.Do_more(c);
                ioclass.Do_send_clear(c);
                return 0;
            }

            /* write the player record to the end of the character file */
          errno = 0;
            if (CLibFile.fwrite(thePlayer, ref phantdefs.SZ_PLAYER, 1, character_file) != 1)
            {

                CLibFile.fclose(character_file);
                miscclass.Do_unlock_mutex(c.realm.character_file_lock);

                error_msg = CFUNCTIONS.sprintfSinglestring("[%s] CLibFile.fwrite of %s failed in Do_save_character: %s\n", c.connection_id, pathnames.CHARACTER_FILE, "");//  CFUNCTIONS.strerror(errno));

                fileclass.Do_log_error(error_msg);

                ioclass.Do_send_line(c, "There was an error opening the character file to save your character.  Please contact the game administrator with this problem.\n");

                ioclass.Do_more(c);
                ioclass.Do_send_clear(c);
                return 0;
            }

            /* close the character file */
            CLibFile.fclose(character_file);
            miscclass.Do_unlock_mutex(c.realm.character_file_lock);

            return 1;
        }


        internal void Do_backup_save(client_t c, int backup)
        {
            CLibFile.CFILE backup_file;
            CLibFile.CFILE temp_backup_file;
            player_t the_player = new player_t();
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];

            miscclass.Do_lock_mutex(c.realm.backup_lock);

            int errno = 0;
            if ((temp_backup_file=CLibFile.fopen(pathnames.TEMP_BACKUP_FILE, "w", ref errno)) == null)
            {

                miscclass.Do_unlock_mutex(c.realm.backup_lock);
                error_msg = CFUNCTIONS.sprintfSinglestring("[%s] CLibFile.fopen of %s failed in Do_backup_save: %s\n", c.connection_id, pathnames.TEMP_BACKUP_FILE, "");//  CFUNCTIONS.strerror(errno));

            fileclass.Do_log_error(error_msg);
            return;
            }

            /* if we're supposed to back up our charcter */
            if (backup != 0)
            {

                /* write the player record to the end of the character file */
              errno = 0;
                if (CLibFile.fwrite(c.player, ref phantdefs.SZ_PLAYER, 1, temp_backup_file) != 1)
                {

                    error_msg = CFUNCTIONS.sprintfSinglestring("[%s] CLibFile.fwrite of %s failed in Do_backup_save: %s\n", c.connection_id, pathnames.TEMP_BACKUP_FILE, "");//  CFUNCTIONS.strerror(errno));

                    fileclass.Do_log_error(error_msg);
                    CLibFile.fclose(temp_backup_file);
                    miscclass.Do_unlock_mutex(c.realm.backup_lock);
                    return;
                }
            }

            /* open the real backup file */
          errno = 0;
            
            if ((backup_file = CLibFile.fopen(pathnames.BACKUP_FILE, "w+", ref errno)) == null) //switched from r to w+ (otherwise doesn't create file). bug in original?
            {
                error_msg = CFUNCTIONS.sprintfSinglestring("[%s] CLibFile.fopen of %s failed in Do_backup_save: %s\n", c.connection_id, pathnames.BACKUP_FILE, CFUNCTIONS.strerror(errno));

                fileclass.Do_log_error(error_msg);
                return;
            }
            else
            {
                /* read each line of the old backup file */
                errno = 0;
                while (CLibFile.fread(ref the_player, phantdefs.SZ_PLAYER, 1, backup_file) == 1)
                {

                    /* if its anyone elses character, copy it over */
                    if (CFUNCTIONS.strcmp(the_player.lcname, c.player.lcname))
                    {

                        if (CLibFile.fwrite(the_player, ref phantdefs.SZ_PLAYER, 1, temp_backup_file) != 1)
                        {

                            error_msg = CFUNCTIONS.sprintfSinglestring("[%s] CLibFile.fwrite of %s failed in Do_backup_save(2): %s\n", c.connection_id, pathnames.TEMP_BACKUP_FILE,  CFUNCTIONS.strerror(errno));

                            fileclass.Do_log_error(error_msg);
                            CLibFile.fclose(backup_file);
                            CLibFile.fclose(temp_backup_file);
                            miscclass.Do_unlock_mutex(c.realm.backup_lock);
                            return;
                        }
                    }
                }
                CLibFile.fclose(backup_file);
                CLibFile.remove(pathnames.BACKUP_FILE);   //unity: moved below close call to avoid sharing violation

                CLibFile.fclose(temp_backup_file);
                /* remove and replace the old backup file */
                CLibFile.rename(pathnames.TEMP_BACKUP_FILE, pathnames.BACKUP_FILE);
                miscclass.Do_unlock_mutex(c.realm.backup_lock);
                return;
            }
        }


        internal int Do_approve_name(client_t c, string lcname, string name, ref int answer)
        {
            int len, i, underscore_count;
            bool space_flag, double_underscore;
            string string_buffer = ""; //[phantdefs.SZ_LINE];
            account_t readAccount = new account_t();
            player_t readPlayer = new player_t();
            game_t game_ptr = new game_t();
            linked_list_t list_ptr = new linked_list_t();
            CLibFile.CFILE the_file;

            /* just stop right now if the name is null */
            len = CFUNCTIONS.strlen(lcname); //lcname.Length;
            if (len == 0)
            {
                string_buffer = CFUNCTIONS.sprintfSinglestring("Please enter the name you would like into the dialog box.\n", lcname);

                ioclass.Do_send_line(c, string_buffer);
                ioclass.Do_more(c);
                ioclass.Do_send_clear(c);
                answer = 0;
                return phantdefs.S_NORM;
            }

            /* see if the name is too long */
            if (len > phantdefs.MAX_NAME_LEN)
            {
                string_buffer = CFUNCTIONS.sprintfSinglestring("\"%s\" is too long.  Please use %d characters or less.\n", name, phantdefs.MAX_NAME_LEN);

                string filteredstring = name.Replace('\0', '$');
                Debug.LogError("Debug name: " + filteredstring + ", chars " + phantdefs.MAX_NAME_LEN);

                ioclass.Do_send_line(c, string_buffer);
                ioclass.Do_more(c);
                ioclass.Do_send_clear(c);
                answer = 0;
                return phantdefs.S_NORM;
            }


            /* see if we have this name reserved */
            if (c.previousName != null && c.previousName != "\0")
            {

                /* see if this name is reserved for us */
                if (!CFUNCTIONS.strcmp(c.previousName, lcname))
                {
                    c.previousName = "\0";
                    answer = 1;
                    return phantdefs.S_NORM;
                }

                /* remove the reserved name from limbo */
                Do_release_name(c, c.previousName);
                c.previousName = "\0";
            }

            /* see if the name looks okay */
            space_flag = false;
            double_underscore = false;
            underscore_count = 0;
            for (i = 0; i < len; i++)
            {

                if (CFUNCTIONS.isalnum(lcname[i]))
                {
                    double_underscore = false;
                }
                else
                {
                    if (lcname[i] == '_')
                    {

                        ++underscore_count;

                        if (double_underscore)
                        {

                            string_buffer = CFUNCTIONS.sprintfSinglestring("\"%s\" uses underscores together.  Please use only one underscore to represent a space.\n", name);

                            ioclass.Do_send_line(c, string_buffer);
                            ioclass.Do_more(c);
                            ioclass.Do_send_clear(c);
                            answer = 0;
                            return phantdefs.S_NORM;
                        }
                        else
                        {
                            double_underscore = true;
                        }
                    }
                    else
                    {

                        /* An error, but hold off in case of other bad chars */
                        if (lcname[i] == ' ')
                        {
                            space_flag = true;
                        }
                        else
                        {

                            string_buffer = CFUNCTIONS.sprintfSinglestring("\"%s\" contains invalid characters.  You may only use letters, numbers and underscores.  Please use a different name.\n", name);

                            ioclass.Do_send_line(c, string_buffer);
                            ioclass.Do_more(c);
                            ioclass.Do_send_clear(c);
                            answer = 0;
                            return phantdefs.S_NORM;
                        }
                    }
                }
            }

            if (space_flag)
            {

                string_buffer = CFUNCTIONS.sprintfSinglestring("\"%s\" uses spaces which is not permitted.  Please use underscores instead of spaces.\n", name);

                ioclass.Do_send_line(c, string_buffer);
                ioclass.Do_more(c);
                ioclass.Do_send_clear(c);
                answer = 0;
                return phantdefs.S_NORM;
            }

            if (lcname[0] == '_' || lcname[len] == '_')
            {

                string_buffer = CFUNCTIONS.sprintfSinglestring("\"%s\" is not using underscores to separate characters.  Please remove them if they are not going to be used as a space.\n", name);

                ioclass.Do_send_line(c, string_buffer);
                ioclass.Do_more(c);
                ioclass.Do_send_clear(c);
                answer = 0;
                return phantdefs.S_NORM;
            }

            if ((double)underscore_count / (double)len >= .25)
            {

                string_buffer = CFUNCTIONS.sprintfSinglestring("\"%s\" contains too many underscores.  Please use underscores only to separate words.\n", name);

                ioclass.Do_send_line(c, string_buffer);
                ioclass.Do_more(c);
                ioclass.Do_send_clear(c);
                answer = 0;
                return phantdefs.S_NORM;
            }

            /* the name is of an acceptable format - see if it is profane */
            if (miscclass.Do_profanity_check(lcname) != 0)
            {

                string_buffer = CFUNCTIONS.sprintfSinglestring("\"%s\" does not get past the profanity checker.  Please choose a different name.\n", lcname);

                ioclass.Do_send_line(c, string_buffer);
                ioclass.Do_more(c);
                ioclass.Do_send_clear(c);
                answer = 0;
                return phantdefs.S_NORM;
            }

            /* start checking accounts to lock the realm as little as possible */
            int errno = 0;
            miscclass.Do_lock_mutex(c.realm.account_lock);
            if ((the_file = CLibFile.fopen(pathnames.ACCOUNT_FILE, "r", ref errno)) == null)
            {

                string_buffer = CFUNCTIONS.sprintfSinglestring("[%s] CLibFile.fopen of %s failed Do_approve_name: %s\n",
                        c.connection_id, pathnames.ACCOUNT_FILE, CFUNCTIONS.strerror(errno));

                fileclass.Do_log_error(string_buffer);
            }
            else
            {                
                /* run through each entry and compare */
                while (CLibFile.fread(ref readAccount, phantdefs.SZ_ACCOUNT, 1, the_file) == 1)
                {

                    if (!CFUNCTIONS.strcmp(readAccount.lcname, lcname))
                    {
                        CLibFile.fclose(the_file);
                        miscclass.Do_unlock_mutex(c.realm.account_lock);

                        string_buffer = CFUNCTIONS.sprintfSinglestring("The name \"%s\" has already been taken.  Please choose another.\n", name);

                        ioclass.Do_send_line(c, string_buffer);
                        ioclass.Do_more(c);
                        ioclass.Do_send_clear(c);
                        answer = 0;
                        return phantdefs.S_NORM;
                    }
                }
                CLibFile.fclose(the_file);
            }

            /* We have to lock the realm now */
            miscclass.Do_lock_mutex(c.realm.realm_lock);

            /* start looking through the games in play */
            game_ptr = c.realm.games;

            /* run through all addresses in limbo */
            while (game_ptr != null)
            {

                if (game_ptr.description != null && !CFUNCTIONS.strcmp(lcname, game_ptr.description.lcname))
                {

                    miscclass.Do_unlock_mutex(c.realm.realm_lock);
                    miscclass.Do_unlock_mutex(c.realm.account_lock);

                    string_buffer = CFUNCTIONS.sprintfSinglestring("The name \"%s\" has already been taken.  Please choose another.\n", name);

                    ioclass.Do_send_line(c, string_buffer);
                    ioclass.Do_more(c);
                    ioclass.Do_send_clear(c);
                    answer = 0;
                    return phantdefs.S_NORM;
                }

                game_ptr = game_ptr.next_game;
            }

            /* now look through the character file */
            errno = 0;
            miscclass.Do_lock_mutex(c.realm.character_file_lock);
            if ((the_file = CLibFile.fopen(pathnames.CHARACTER_FILE, "r", ref errno)) == null)
            {

                string_buffer = CFUNCTIONS.sprintfSinglestring("[%s] CLibFile.fopen of %s failed in Do_approve_name: %s\n", c.connection_id, pathnames.CHARACTER_FILE, CFUNCTIONS.strerror(errno));

                fileclass.Do_log_error(string_buffer);
            }
            else
            {
                /* run through each entry and compare */
                while (CLibFile.fread(ref readPlayer, phantdefs.SZ_PLAYER, 1, the_file) == 1)
                {

                    if (!CFUNCTIONS.strcmp(readPlayer.lcname,lcname)) //false => a match
                    {
                        
                        CLibFile.fclose(the_file);
                        miscclass.Do_unlock_mutex(c.realm.character_file_lock);
                        miscclass.Do_unlock_mutex(c.realm.realm_lock);
                        miscclass.Do_unlock_mutex(c.realm.account_lock);

                        string_buffer = CFUNCTIONS.sprintfSinglestring("The name \"%s\" has already been taken.  Please choose another.\n", name);

                        ioclass.Do_send_line(c, string_buffer);
                        ioclass.Do_more(c);
                        ioclass.Do_send_clear(c);
                        answer = 0;
                        return phantdefs.S_NORM;
                    }
                }

                CLibFile.fclose(the_file);
            }

            /* start looking through names in limbo */
            list_ptr = c.realm.name_limbo;

            /* run through all addresses in limbo */
            while (list_ptr != null)
            {

                if (!CFUNCTIONS.strcmp(list_ptr.name , lcname))
                {

                    miscclass.Do_unlock_mutex(c.realm.character_file_lock);
                    miscclass.Do_unlock_mutex(c.realm.realm_lock);
                    miscclass.Do_unlock_mutex(c.realm.account_lock);

                    string_buffer = CFUNCTIONS.sprintfSinglestring("The name \"%s\" is currently being registered by another player.  Please choose another.\n", name);

                    ioclass.Do_send_line(c, string_buffer);
                    ioclass.Do_more(c);
                    ioclass.Do_send_clear(c);
                    answer = 0;
                    return phantdefs.S_NORM;
                }

                list_ptr = list_ptr.next;
            }

            /* name address checks out.  Put ours in limbo */
            list_ptr = new linked_list_t();// (struct linked_list_t *) Do_malloc(phantdefs.SZ_LINKED_LIST);

            list_ptr.name = lcname;
            list_ptr.next = c.realm.name_limbo;
            c.realm.name_limbo = list_ptr;

            miscclass.Do_unlock_mutex(c.realm.character_file_lock);
            miscclass.Do_unlock_mutex(c.realm.realm_lock);
            miscclass.Do_unlock_mutex(c.realm.account_lock);

            answer = 1;
            return phantdefs.S_NORM;
        }


        internal void Do_release_name(client_t c, string name)
        {
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            linked_list_t list_ptr = new linked_list_t();
            linked_list_t list_ptr_ptr = new linked_list_t();

            miscclass.Do_lock_mutex(c.realm.character_file_lock);

            /* start at the first pointer */
            list_ptr_ptr = c.realm.name_limbo;

            /* run through all addresses in limbo */
            while (list_ptr_ptr != null)
            {
                if (!CFUNCTIONS.strcmp(list_ptr_ptr.name, name))
                {

                    /* remove this section of linked list */
                    list_ptr = list_ptr_ptr;
                    list_ptr_ptr = list_ptr.next;
                    list_ptr = null;// free((void*) list_ptr);

                    miscclass.Do_unlock_mutex(c.realm.character_file_lock);
                    return;
                }

                list_ptr_ptr = list_ptr_ptr.next;
            }

            miscclass.Do_unlock_mutex(c.realm.character_file_lock);

            error_msg = CFUNCTIONS.sprintfSinglestring("[%s] The name %s was not found in limbo by Do_release_name.\n", c.connection_id, name);

            fileclass.Do_log_error(error_msg);
            return;
        }


        internal void Do_clear_character_mod(player_mod_t theMod)
        {
            theMod.newName = false;
            theMod.newPassword = false;
            theMod.passwordReset = false;
            theMod.newPermissions = false;
            theMod.badPassword = false;
            return;
        }


        internal int Do_modify_character(client_t c, string the_name, player_mod_t theMod)
        {
            player_t readPlayer = new player_t();
            CLibFile.CFILE character_file;
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            long loc = 0;

            miscclass.Do_lock_mutex(c.realm.character_file_lock);

            int errno = 0;
            if ((character_file = CLibFile.fopen(pathnames.CHARACTER_FILE, "r+", ref errno)) == null)
            {
                miscclass.Do_unlock_mutex(c.realm.character_file_lock);
                error_msg = CFUNCTIONS.sprintfSinglestring("[%s] CLibFile.fopen of %s failed in Do_modify_character: %s\n",
                    c.connection_id, pathnames.CHARACTER_FILE,  CFUNCTIONS.strerror(errno));

                fileclass.Do_log_error(error_msg);
                return 0;
            }

            /* read each line of the character file */
            while (CLibFile.fread(ref readPlayer, phantdefs.SZ_PLAYER, 1, character_file) == 1)
            {

                /* if we find our character, make the mods and save */
                if (!CFUNCTIONS.strcmp(readPlayer.lcname, the_name))
                {

                    /* are we changing the character name? */
                    if (theMod.newName)
                    {
                        readPlayer.name = theMod.name;
                        readPlayer.lcname = theMod.lcName;
                    }

                    /* are we putting in a new password? */
                    if (theMod.newPassword)
                    {
                        readPlayer.password = theMod.password;//, phantdefs.SZ_PASSWORD);
                    }

                    /* is this a password reset? */
                    if (theMod.passwordReset)
                    {
                        readPlayer.last_reset = CFUNCTIONS.time(null);
                    }

                    /* change permissions? */
                    if (theMod.newPermissions)
                    {
                        readPlayer.faithful = theMod.faithful;
                    }

                    /* if someone has typrd in a bad password */
                    if (theMod.badPassword)
                    {
                        ++readPlayer.bad_passwords;
                    }

                    /* now, write over the previous entry */
                    CLibFile.fseek(character_file, loc, 0);
                  errno = 0;
                    if (CLibFile.fwrite(readPlayer, ref phantdefs.SZ_PLAYER, 1, character_file) != 1)
                    {

                        error_msg = CFUNCTIONS.sprintfSinglestring("[%s] CLibFile.fwrite over %s failed in Do_modify_character: %s\n", c.connection_id, pathnames.CHARACTER_FILE,  CFUNCTIONS.strerror(errno));

                        fileclass.Do_log_error(error_msg);
                    }
                    
                    CLibFile.fclose(character_file);
                    miscclass.Do_unlock_mutex(c.realm.character_file_lock);
                    return 1;
                }

                loc += phantdefs.SZ_PLAYER;
            }

            miscclass.Do_unlock_mutex(c.realm.character_file_lock);

            error_msg = CFUNCTIONS.sprintfSinglestring("[%s] The character %s was not found by Do_modify_character.\n", c.connection_id, the_name);

            fileclass.Do_log_error(error_msg);
            return 0;
        }


        void Do_character_options(client_t c)
        {
            player_t readPlayer = new player_t();
            button_t buttons = new button_t();
            game_t game_ptr = new game_t();
            CLibFile.CFILE character_file;
            string string_buffer = ""; //[phantdefs.SZ_LINE];
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            string theTitle = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            bool found_flag;
            int rc;
            long answer = -1;

            found_flag = false;

            /* start by looking at games in play */
            ioclass.Do_send_line(c, "Searching for characters assigned to this account...\n");
            socketclass.Do_send_buffer(c);
            miscclass.Do_lock_mutex(c.realm.realm_lock);
            game_ptr = c.realm.games;

            while (game_ptr != null)
            {

                if (game_ptr.description != null)
                {

                    if (!CFUNCTIONS.strcmp(c.lcaccount,game_ptr.description.parent_account)) 
                    {

                        /* do we need to put up a header? */
                        if (!found_flag)
                        {

                            ioclass.Do_send_clear(c);

                            ioclass.Do_send_line(c, "Character Name - Level - Date Last Loaded\n");

                            found_flag = true;
                        }

                        infoclass.Do_make_character_title(c, game_ptr, ref theTitle);

                        /* put everything together */
                        string_buffer = CFUNCTIONS.sprintfSinglestring("%s - %.0lf - Currently Playing\n",
                        theTitle, game_ptr.description.level);

                        ioclass.Do_send_line(c, string_buffer);
                    }

                }
                game_ptr = game_ptr.next_game;
            }

            miscclass.Do_lock_mutex(c.realm.character_file_lock);

            int errno = 0;
            if ((character_file = CLibFile.fopen(pathnames.CHARACTER_FILE, "r", ref errno)) == null)
            {
                error_msg = CFUNCTIONS.sprintfSinglestring("[%s] CLibFile.fopen of %s failed in Do_character_options: %s\n", c.connection_id, pathnames.CHARACTER_FILE, CFUNCTIONS.strerror(errno));

                fileclass.Do_log_error(error_msg);
            }
            else
            {
                /* loop through the the characters */
                while (CLibFile.fread(ref readPlayer, phantdefs.SZ_PLAYER, 1, character_file) == 1)
                {

                    string filter1 = c.lcaccount.Replace('\0', '$');
                    string filter2 = readPlayer.parent_account.Replace('\0', '$');
                    //Debug.LogError("Character options debug: || " + filter1 + " || " + filter2 + " ||");
                    if (!CFUNCTIONS.strcmp(c.lcaccount, readPlayer.parent_account))
                    {

                        /* do we need to put up a header? */
                        if (!found_flag)
                        {

                            ioclass.Do_send_clear(c);

                            ioclass.Do_send_line(c, "Character Name - Level - Date Last Loaded\n");

                            found_flag = true;
                        }

                        /* no carriage return, provided by ctime */
                        CFUNCTIONS.ctime_r(readPlayer.last_load, ref error_msg);

                        /* put everything together */
                        string_buffer = CFUNCTIONS.sprintfSinglestring("%s the %s - %.0lf - %s", readPlayer.name, c.realm.charstats[readPlayer.type].class_name, readPlayer.level, error_msg);

                        ioclass.Do_send_line(c, string_buffer);
                    }
                }

                CLibFile.fclose(character_file);
            }

            miscclass.Do_unlock_mutex(c.realm.realm_lock);
            miscclass.Do_unlock_mutex(c.realm.character_file_lock);

            if (!found_flag)
            {
                ioclass.Do_send_clear(c);
                ioclass.Do_send_line(c, "No characters from this account were found.\n");
            }

            buttons.button[0] = "Change Pass\n";
            buttons.button[1] = "Reset Pass\n";
            ioclass.Do_clear_buttons(buttons, 2);

            if (c.wizard > 2)
            {
                buttons.button[3] = "Change Name\n";
            }

            buttons.button[4] = "Sharing\n";

            buttons.button[7] = "Go Back\n";

            rc = ioclass.Do_buttons(c, ref answer, buttons);
            ioclass.Do_send_clear(c);

            if (rc != phantdefs.S_NORM)
            {
                answer = 7;
            }

            /* switch on the player's answer */
            switch (answer)
            {

                /* The player wishes to change a character password */
                case 0:
                    Do_change_character_password(c);
                    break;

                /* The user wants a password reset */
                case 1:
                    Do_reset_character_password(c);
                    break;

                /* Rename the character */
                /*
                    case 3:
                    Do_rename_character(c);
                    break;
                */

                /* Change character sharing permissions */
                case 4:
                    Do_character_sharing(c);
                    break;

                /* Return to previous state */
                case 7:
                    return;

                default:

                    error_msg = CFUNCTIONS.sprintfSinglestring("[%s] Returned non-option in Do_character_options.\n", c.connection_id);

                    fileclass.Do_log_error(error_msg);
                    hackclass.Do_caught_hack(c, phantdefs.H_SYSTEM);
                    return;
            }

            return;
        }


        void Do_change_character_password(client_t c)
        {
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE], 
            string characterName = ""; //[phantdefs.SZ_NAME];
            string string_buffer = ""; //[phantdefs.SZ_LINE], 
            string lcCharacterName = ""; //[phantdefs.SZ_NAME];
            button_t theButtons = new button_t();
            player_mod_t theMod = new player_mod_t();
            int rc;
            long answer = -1;

            ioclass.Do_send_line(c, "This option allows you to change the password of one of your characters.  Do you wish to continue?\n");

            theButtons.button[0] = "Continue\n";
            ioclass.Do_clear_buttons(theButtons, 1);
            theButtons.button[7] = "Go Back\n";

            rc = ioclass.Do_buttons(c, ref answer, theButtons);
            ioclass.Do_send_clear(c);

            if (rc != phantdefs.S_NORM)
            {
                answer = 7;
            }

            /* switch on the player's answer */
            switch (answer)
            {

                /* Continiue */
                case 0:
                    break;

                /* Go Back */
                case 7:
                    return;

                default:

                    error_msg = CFUNCTIONS.sprintfSinglestring("[%s] Returned non-option in Do_change_character_password.\n", c.connection_id);

                    fileclass.Do_log_error(error_msg);
                    hackclass.Do_caught_hack(c, phantdefs.H_SYSTEM);
                    return;
            }

            for (; ; )
            {
                if (UnityGameController.StopApplication) //time to quit
                {
                    Debug.Log("Thread " + System.Threading.Thread.CurrentThread.Name + " stopping in Do_change_character_password");
                    return;
                }
                else
                {
                    System.Threading.Thread.Sleep(33); //30fps
                }

                /* prompt for the character */
                if (ioclass.Do_string_dialog(c, ref characterName, phantdefs.SZ_NAME - 1,
                    "Which character's password do you wish to change?\n"))
                {

                    return;
                }

                /* load the character information */
                miscclass.Do_lowercase(ref lcCharacterName, characterName);
                if (Do_look_character(c, lcCharacterName, ref c.player) != 0)
                {
                    break;
                }

                /* see if the character is playing */
                if (Do_character_playing(c, lcCharacterName) != 0)
                {

                    ioclass.Do_send_line(c, "That character is currently in the game.  You can not modify the passwords of characters in play.\n");

                    ioclass.Do_more(c);
                    ioclass.Do_send_clear(c);
                    return;
                }
                
                string_buffer = CFUNCTIONS.sprintfSinglestring("I can not find a character named \"%s\".  Please check the spelling and try again.\n", characterName);

                ioclass.Do_send_line(c, string_buffer);
                ioclass.Do_more(c);
                ioclass.Do_send_clear(c);
            }

            /* make sure we're on this character's parent account */
            if (CFUNCTIONS.strcmp(c.player.parent_account, c.lcaccount))
            {

                ioclass.Do_send_line(c, "This character was not created from this account.  You can only modify the passwords of characters you created.\n");

                ioclass.Do_more(c);
                ioclass.Do_send_clear(c);
                return;
            }

            /* found character - confirm player by asking for password */
            if (ioclass.Do_request_character_password(c, c.player.password, c.player.name, c.player.lcname, 0) == 0)
            {
                return;
            }

            Do_clear_character_mod(theMod);

            /* Get the new password from the player */
            if (!ioclass.Do_new_password(c, theMod.password, "character"))
            {
                return;
            }

            theMod.newPassword = true;

            if (Do_modify_character(c, lcCharacterName, theMod) == 0)
            {

                /* if false returns, the character was not modified */
                string_buffer = CFUNCTIONS.sprintfSinglestring("The character named \"%s\" is not in the character file.  This could be because the character was just loaded by someone else.  The password has NOT been changed.\n", c.player.name);

                ioclass.Do_send_line(c, string_buffer);
                ioclass.Do_more(c);
                ioclass.Do_send_clear(c);
                return;
            }

            string_buffer  = CFUNCTIONS.sprintfSinglestring("[%s] Changed the password to character %s.\n", c.connection_id, c.player.lcname);

            fileclass.Do_log(pathnames.CONNECTION_LOG, string_buffer);

            string_buffer = CFUNCTIONS.sprintfSinglestring("The password to the character \"%s\" has been successfully changed.\n", c.player.name);

            ioclass.Do_send_line(c, string_buffer);
            ioclass.Do_more(c);
            ioclass.Do_send_clear(c);
            return;
        }


        void Do_rename_character(client_t c)
        {

            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE], 
            string characterName = ""; //[phantdefs.SZ_NAME];
            string string_buffer = ""; //[phantdefs.SZ_LINE], 
            string lcCharacterName = ""; //[phantdefs.SZ_NAME];
            button_t theButtons = new button_t();
            player_mod_t theMod = new player_mod_t();
            int rc;
            int theAnswer = -1;
            long answer = -1;

            ioclass.Do_send_line(c, "You are asking to change the name of one of your characters.  Do you wish to continue?\n");

            theButtons.button[0] = "Continue\n";
            ioclass.Do_clear_buttons(theButtons, 1);
            theButtons.button[7] = "Go Back\n";

            rc = ioclass.Do_buttons(c, ref answer, theButtons);
            ioclass.Do_send_clear(c);

            if (rc != phantdefs.S_NORM) {
                answer = 7;
            }

            /* switch on the player's answer */
            switch (answer)
            {

                /* Continue */
                case 0:
                    break;

                /* Go Back */
                case 7:
                    return;

                default:

                    error_msg = CFUNCTIONS.sprintfSinglestring("[%s] Returned non-option in Do_rename_character.\n", c.connection_id);

                    fileclass.Do_log_error(error_msg);
                    hackclass.Do_caught_hack(c, phantdefs.H_SYSTEM);
                    return;
            }

            for (; ; )
            {
                if (UnityGameController.StopApplication) //time to quit
                {
                    Debug.Log("Thread " + System.Threading.Thread.CurrentThread.Name + " stopping in Do_rename_character");
                    return;
                }
                else
                {
                    System.Threading.Thread.Sleep(33); //30fps
                }

                /* prompt for the character */
                if (ioclass.Do_string_dialog(c, ref characterName, phantdefs.SZ_NAME - 1, "What is the name of the character you wish to change?\n"))
                {

                    return;
                }

                /* load the character information */
                miscclass.Do_lowercase(ref lcCharacterName, characterName);
                if (Do_look_character(c, lcCharacterName, ref c.player) != 0)
                {
                    break;
                }

                /* see if the character is playing */
                if (Do_character_playing(c, lcCharacterName) != 0)
                {

                    ioclass.Do_send_line(c, "That character is currently in the game.  You can not modifiy the passwords of characters in play.\n");

                    ioclass.Do_more(c);
                    ioclass.Do_send_clear(c);
                    return;
                }


                string_buffer = CFUNCTIONS.sprintfSinglestring("I can not find a character named \"%s\".  Please check the spelling and try again.\n", characterName);

                ioclass.Do_send_line(c, string_buffer);
                ioclass.Do_more(c);
                ioclass.Do_send_clear(c);
            }

            /* make sure we're on this character's parent account */
            /*
                if (cfunctions.strcmp(c.player.parent_account, c.lcaccount)) {

                ioclass.Do_send_line(c, "This character was not created from this account.  You can only modify the passwords of characters you created.\n");

                ioclass.Do_more(c);
                ioclass.Do_send_clear(c);
                return;
                }
            */

            /* found character - confirm player by asking for password */
            /*
                if (!Do_request_character_password(c, c.player.password, c.player.name,
                    c.player.lcname, 0)) {

                    return;
                }
            */

            Do_clear_character_mod(theMod);

            for (; ; )
            {
                if (UnityGameController.StopApplication) //time to quit
                {
                    Debug.Log("Thread " + System.Threading.Thread.CurrentThread.Name + " stopping in Do_rename_character");
                    return;
                }
                else
                {
                    System.Threading.Thread.Sleep(33); //30fps
                }

                /* get the new character name */
                if (ioclass.Do_string_dialog(c, ref theMod.name, phantdefs.SZ_NAME - 1, "What name would you like your character to have?\n"))
                {

                    return;
                }

                /* see if the name is approved */
                miscclass.Do_lowercase(ref theMod.lcName, theMod.name);

                if (Do_approve_name(c, theMod.lcName, theMod.name, ref theAnswer) != phantdefs.S_NORM)
                {

                    return;
                }

                if (theAnswer != 0)
                {
                    break;
                }
            }

            theMod.newName = true;

            if (Do_modify_character(c, lcCharacterName, theMod) == 0)
            {

                /* if false returns, the character was not modified */
                string_buffer = CFUNCTIONS.sprintfSinglestring("The character named \"%s\" is not in the character file so the name was not changed.  This could be because the character was just loaded by someone else.\n", c.player.name);

                Do_release_name(c, theMod.lcName);
                ioclass.Do_send_line(c, string_buffer);
                ioclass.Do_more(c);
                ioclass.Do_send_clear(c);
                return;
            }

            Do_release_name(c, theMod.lcName);

            string_buffer = CFUNCTIONS.sprintfSinglestring("[%s] Changed character name %s to %s.\n", c.connection_id, c.player.lcname, theMod.lcName);

            fileclass.Do_log(pathnames.CONNECTION_LOG, string_buffer);

            string_buffer = CFUNCTIONS.sprintfSinglestring("[%s] %s renamed %s.\n", c.connection_id, c.player.lcname, theMod.lcName);

            fileclass.Do_log(pathnames.GAME_LOG, string_buffer);

            string_buffer = CFUNCTIONS.sprintfSinglestring("The character \"%s\" is now called \"%s\".\n", c.player.name, theMod.name);

            ioclass.Do_send_line(c, string_buffer);
            ioclass.Do_more(c);
            ioclass.Do_send_clear(c);
            return;
        }



        void Do_reset_character_password(client_t c)
        {

            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE], 
            string characterName = ""; //[phantdefs.SZ_NAME];
            string string_buffer = ""; //[phantdefs.SZ_LINE], 
            string lcCharacterName = ""; //[phantdefs.SZ_NAME];
            char[] newPassword = new char[16]; // "";// = ""; //[phantdefs.SZ_PASSWORD];
            player_mod_t theMod = new player_mod_t();
            button_t theButtons = new button_t();
            int rc;
            long answer = -1;
            md5c.MD5_CTX context = new md5c.MD5_CTX();
            uint len;

            ioclass.Do_send_line(c, "With this option a random password will be created for one of your characters and e-mailed to your account address.  This is the only way to gain access to a character whose password you've forgotten.\n");

            theButtons.button[0] = "Continue\n";
            ioclass.Do_clear_buttons(theButtons, 1);
            theButtons.button[7] = "Go Back\n";

            rc = ioclass.Do_buttons(c, ref answer, theButtons);
            ioclass.Do_send_clear(c);

            if (rc != phantdefs.S_NORM) {
                answer = 7;
            }

            /* switch on the player's answer */
            switch (answer)
            {

                /* Continiue */
                case 0:
                    break;

                /* Go Back */
                case 7:
                    return;

                default:

                    error_msg = CFUNCTIONS.sprintfSinglestring("[%s] Returned non-option in Do_reset_character_password.\n", c.connection_id);

                    fileclass.Do_log_error(error_msg);
                    hackclass.Do_caught_hack(c, phantdefs.H_SYSTEM);
                    return;
            }

            for (; ; )
            {
                if (UnityGameController.StopApplication) //time to quit
                {
                    Debug.Log("Thread " + System.Threading.Thread.CurrentThread.Name + " stopping in Do_reset_character_password");
                    return;
                }
                else
                {
                    System.Threading.Thread.Sleep(33); //30fps
                }

                /* prompt for the character */
                if (ioclass.Do_string_dialog(c, ref characterName, phantdefs.SZ_NAME - 1, "Which character's password do you wish to reset?\n"))
                {

                    return;
                }

                /* load the character information */
                miscclass.Do_lowercase(ref lcCharacterName, characterName);
                if (Do_look_character(c, lcCharacterName, ref c.player) != 0)
                {
                    break;
                }

                /* see if the character is playing */
                if (Do_character_playing(c, lcCharacterName) != 0)
                {

                    ioclass.Do_send_line(c, "That character is currently in the game.  You can not modifiy the passwords of characters in play.\n");

                    ioclass.Do_more(c);
                    ioclass.Do_send_clear(c);
                    return;
                }


                string_buffer = CFUNCTIONS.sprintfSinglestring("I can not find a character named \"%s\".  Please check the spelling and try again.\n", characterName);

                ioclass.Do_send_line(c, string_buffer);
                ioclass.Do_more(c);
                ioclass.Do_send_clear(c);
            }

            /* make sure we're on this character's parent account */
            if (CFUNCTIONS.strcmp(c.player.parent_account, c.lcaccount))
            {

                ioclass.Do_send_line(c, "This character was not created from this account.  You can only modify the passwords of characters you created.\n");

                ioclass.Do_more(c);
                ioclass.Do_send_clear(c);
                return;
            }

            /* see if it's been longer than 24 hours since last reset */
            if (CFUNCTIONS.GetUnixEpoch(DateTime.Now) - c.player.last_reset < 86400)
            {

                string_buffer = CFUNCTIONS.sprintfSinglestring("The password to the character named \"%s\" has been reset within the last 24 hours.  You must wait before resetting it again.\n", c.player.name);

                ioclass.Do_send_line(c, string_buffer);
                ioclass.Do_more(c);
                ioclass.Do_send_clear(c);
                return;
            }

            string_buffer = CFUNCTIONS.sprintfSinglestring("Are you certain you wish to reset the password for the character named \"%s\"?\n", c.player.name);

            ioclass.Do_send_line(c, string_buffer);

            if (ioclass.Do_yes_no(c, ref answer) != phantdefs.S_NORM || answer == 1)
            {

                ioclass.Do_send_clear(c);
                string_buffer = CFUNCTIONS.sprintfSinglestring("Password reset aborted.  The password to the character named \"%s\" has NOT been changed.\n", c.player.name);

                ioclass.Do_send_line(c, string_buffer);
                ioclass.Do_more(c);
                ioclass.Do_send_clear(c);
                return;
            }

            ioclass.Do_send_clear(c);

            /* create a new password */
            miscclass.Do_create_password(ref newPassword);

            /* call the script to e-mail this new password */
            string_buffer = CFUNCTIONS.sprintfSinglestring("%s %s %s %s\n", pathnames.CHARACTER_PASSWORD_RESET_SCRIPT, c.player.name, newPassword, c.email);

            /* if the mail send fails */
            rc = -1; //added
            if (rc == CFUNCTIONS.system(string_buffer, c.player.name, new string(newPassword), c.email))
            {

                string_buffer = CFUNCTIONS.sprintfSinglestring("[%s] Character password reset e-mail failed with a code of %d.", c.connection_id, rc);

                fileclass.Do_log_error(string_buffer);

                ioclass.Do_send_line(c, "An error occured while trying to send e-mail containing the new password.  The character password has NOT been changed.  Please contact the game administrator about this problem.\n");

                ioclass.Do_more(c);
                ioclass.Do_send_clear(c);
                return;
            }

            Do_clear_character_mod(theMod);
            theMod.newPassword = true;
            theMod.passwordReset = true;
            
            /* run the password through a MD5 hash */
            len = (uint)newPassword.Length;
            md5c.MD5Init(ref context);
            md5c.MD5Update(ref context, newPassword, len);

            /* put the password hash into the change struct */
            md5c.MD5Final(ref theMod.password, ref context);     //md5c.MD5Final(theMod.password, context);

            /* make the modification */
            if (Do_modify_character(c, lcCharacterName, theMod) == 0)
            {

                /* if false returns, the character was not modified */
                string_buffer = CFUNCTIONS.sprintfSinglestring("The character named \"%s\" is not in the character file.  This could be because the character was just loaded by someone else.  The password has NOT been changed.\n", c.player.name);

                ioclass.Do_send_line(c, string_buffer);
                ioclass.Do_more(c);
                ioclass.Do_send_clear(c);
                return;
            }

            string_buffer = CFUNCTIONS.sprintfSinglestring("[%s] Reset the password to character %s.\n", c.connection_id, c.player.lcname);

            fileclass.Do_log(pathnames.CONNECTION_LOG, string_buffer);

            string_buffer = CFUNCTIONS.sprintfSinglestring("The password to the character named \"%s\" has been successfully changed.  Your new password has been e-mailed to your account address.\n", c.player.name);

            ioclass.Do_send_line(c, string_buffer);
            ioclass.Do_more(c);
            ioclass.Do_send_clear(c);
        }


        void Do_character_sharing(client_t c)
        {

            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE], 
            string characterName = ""; //[phantdefs.SZ_NAME];
            string string_buffer = ""; //[phantdefs.SZ_LINE], 
            string lcCharacterName = ""; //[phantdefs.SZ_NAME];
            player_mod_t theMod = new player_mod_t();
            button_t theButtons = new button_t();
            int rc;
            long answer = -1;

            ioclass.Do_send_line(c, "By default, characters can only be loaded by the account that created them.  Here you can remove or replace that restriction.\n");

            theButtons.button[0] = "Continue\n";
            ioclass.Do_clear_buttons(theButtons, 1);
            theButtons.button[7] = "Go Back\n";

            rc = ioclass.Do_buttons(c, ref answer, theButtons);
            ioclass.Do_send_clear(c);

            if (rc != phantdefs.S_NORM) {
                answer = 7;
            }

            /* switch on the player's answer */
            switch (answer)
            {

                /* Continiue */
                case 0:
                    break;

                /* Go Back */
                case 7:
                    return;

                default:

                    error_msg = CFUNCTIONS.sprintfSinglestring("[%s] Returned non-option in Do_character_sharing.\n", c.connection_id);

                    fileclass.Do_log_error(error_msg);
                    hackclass.Do_caught_hack(c, phantdefs.H_SYSTEM);
                    return;
            }

            for (; ; )
            {
                if (UnityGameController.StopApplication) //time to quit
                {
                    Debug.Log("Thread " + System.Threading.Thread.CurrentThread.Name + " stopping in Do_character_sharing");
                    return;
                }
                else
                {
                    System.Threading.Thread.Sleep(33); //30fps
                }

                /* prompt for the character */
                if (ioclass.Do_string_dialog(c, ref characterName, phantdefs.SZ_NAME - 1, "Which character's sharing options do you wish to modify ?\n"))
                {

                    return;
                }

                /* load the character information */
                miscclass.Do_lowercase(ref lcCharacterName, characterName);
                if (Do_look_character(c, lcCharacterName, ref c.player) != 0)
                {
                    break;
                }

                /* see if the character is playing */
                if (Do_character_playing(c, lcCharacterName) != 0)
                {

                    ioclass.Do_send_line(c, "That character is currently in the game.  You can not modifiy characters in play.\n");

                    ioclass.Do_more(c);
                    ioclass.Do_send_clear(c);
                    return;
                }


                string_buffer = CFUNCTIONS.sprintfSinglestring("I can not find a character named \"%s\".  Please check the spelling and try again.\n", characterName);

                ioclass.Do_send_line(c, string_buffer);
                ioclass.Do_more(c);
                ioclass.Do_send_clear(c);
            }

            /* make sure we're on this character's parent account */
            if (CFUNCTIONS.strcmp(c.player.parent_account,c.lcaccount))
            {

                ioclass.Do_send_line(c, "This character was not created from this account.  You can only modify characters you created.\n");

                ioclass.Do_more(c);
                ioclass.Do_send_clear(c);
                return;
            }

            /* found character - now get the password */
            if (ioclass.Do_request_character_password(c, c.player.password, c.player.name, c.player.lcname, 0) == 0)
            {

                return;
            }

            if (c.player.faithful)
            {
                string_buffer = CFUNCTIONS.sprintfSinglestring("Currently, other accounts can not load the character named \"%s\".\n", c.player.name);
            }
            else
            {
                string_buffer = CFUNCTIONS.sprintfSinglestring("Currently, other accounts have permission to load the character named \"%s\".\n", c.player.name);
            }

            ioclass.Do_send_line(c, string_buffer);
            ioclass.Do_send_line(c, "\n");
            ioclass.Do_send_line(c, "If another account attempts to load this character, do you wish to allow or deny their request?\n");

            Do_clear_character_mod(theMod);

            theButtons.button[0] = "Allow\n";
            theButtons.button[1] = "Deny\n";
            ioclass.Do_clear_buttons(theButtons, 2);
            theButtons.button[7] = "Cancel\n";

            rc = ioclass.Do_buttons(c, ref answer, theButtons);
            ioclass.Do_send_clear(c);

            if (rc != phantdefs.S_NORM)
            {
                answer = 7;
            }

            /* switch on the player's answer */
            switch (answer)
            {

                /* Allow */
                case 0:
                    theMod.faithful = false;
                    break;

                /* Deny */
                case 1:
                    theMod.faithful = true;
                    break;

                /* Cancel */
                case 7:
                    return;

                default:

                    error_msg = CFUNCTIONS.sprintfSinglestring("[%s] Returned non-option in Do_character_sharing(2).\n", c.connection_id);

                    fileclass.Do_log_error(error_msg);
                    hackclass.Do_caught_hack(c, phantdefs.H_SYSTEM);
                    return;
            }

            theMod.newPermissions = true;
            if (Do_modify_character(c, lcCharacterName, theMod) == 0)
            {

                /* if false returns, the character was not modified */
                string_buffer = CFUNCTIONS.sprintfSinglestring("The character named \"%s\" is not in the character file.  This could be because the character was just loaded by someone else.  The permissions have NOT been changed.\n", c.player.name);

                ioclass.Do_send_line(c, string_buffer);
                ioclass.Do_more(c);
                ioclass.Do_send_clear(c);
                return;
            }

            if (answer != 0)
            {

                error_msg = CFUNCTIONS.sprintfSinglestring("[%s] Faithful permissions set on character %s.\n", c.connection_id, c.player.lcname);

                string_buffer = CFUNCTIONS.sprintfSinglestring("Permissions have been set so other accounts will be denied access to the character named \"%s\".\n", c.player.name);

            }
            else
            {

                error_msg = CFUNCTIONS.sprintfSinglestring("[%s] Faithful permissions removed on character %s.\n", c.connection_id, c.player.lcname);

                string_buffer = CFUNCTIONS.sprintfSinglestring("Permissions have been set so other accounts may load the character named \"%s\".\n", c.player.name);

            }

            fileclass.Do_log(pathnames.CONNECTION_LOG, error_msg);
            ioclass.Do_send_line(c, string_buffer);
            ioclass.Do_more(c);
            ioclass.Do_send_clear(c);
        }


        internal void Do_approve_entrance(client_t c)
        {
            event_t event_ptr = new event_t();

            /* Mark this thread as with character */
            c.characterLoaded = true;

            /* assume the name is not modified */
            c.modifiedName = c.player.name;

                /* if the character was knight after crash */
            if (c.player.special_type == phantdefs.SC_KNIGHT)
            {
                commandsclass.Do_dethrone(c);
            }

            if (c.player.special_type == phantdefs.SC_STEWARD)
            {
                commandsclass.Do_dethrone(c);
            }

            /* if the character was king after crash */
            if (c.player.special_type == phantdefs.SC_KING)
            {

                miscclass.Do_lock_mutex(c.realm.realm_lock);

                /* check to see if we're the old king */
                if (!CFUNCTIONS.strcmp(c.realm.king_name, c.modifiedName))
                {
                    miscclass.Do_unlock_mutex(c.realm.realm_lock);

                    if (c.player.level >= phantdefs.MIN_KING && c.player.level < phantdefs.MAX_KING)
                    {
                        commandsclass.Do_king(c);
                    }
                    else
                    {
                        commandsclass.Do_dethrone(c);
                        c.player.special_type = (short)phantdefs.SC_NONE;
                    }
                }
                else
                {

                    /* there has been a new king */
                    miscclass.Do_unlock_mutex(c.realm.realm_lock);
                    ioclass.Do_send_line(c, "You are no longer the ruler!\n");
                    c.player.special_type = (short)phantdefs.SC_NONE;
                }
            }

            /* handle if the character was valar after crash */
            else if (c.player.special_type == phantdefs.SC_VALAR)
            {

                miscclass.Do_lock_mutex(c.realm.realm_lock);

                /* check for a current valar */
                if (!CFUNCTIONS.strcmp(c.realm.valar_name, c.modifiedName))
                {

                    /* no valar, put ourselves */
                    miscclass.Do_unlock_mutex(c.realm.realm_lock);
                    c.realm.valar = c.game;
                    c.realm.valar_name = c.modifiedName;
                }
                else
                {

                    /* there is a valar, bow out */
                    miscclass.Do_unlock_mutex(c.realm.realm_lock);
                    commandsclass.Do_valar(c);
                }
            }

            /* check for tags to this address, connection or character */
            tagsclass.Do_check_tags(c);

            /* done here */
            return;
        }


        internal void Do_entering_character(client_t c)
        {
            string string_buffer = ""; //[phantdefs.SZ_LINE],
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            CLibFile.CFILE wizard_file;//, 
            CLibFile.CFILE motd_file;
            string theNetwork = ""; //[phantdefs.SZ_FROM],
            string theAccount = ""; //[phantdefs.SZ_NAME],
            string theCharacter = ""; //[phantdefs.SZ_NAME];
            short wizType;
            char char_ptr;
            int minutes = -1;
            event_t event_ptr = new event_t();
            int exceptionFlag = -1;
            
            /* pull death name out of limbo if necessary */
            if (c.previousName != null && c.previousName != "\0")
            {
                Do_release_name(c, c.previousName);
                c.previousName = "\0";
            }

            int errno = 0;
            
            /* open the wizard file to see if this person is one */
            if ((wizard_file = CLibFile.fopen(pathnames.WIZARD_FILE, "r", ref errno)) == null)
            {
                error_msg = CFUNCTIONS.sprintfSinglestring("[%s] CLibFile.fopen of %s failed in Do_entering_character: %s\n", c.connection_id, pathnames.WIZARD_FILE, CFUNCTIONS.strerror(errno));

                fileclass.Do_log_error(error_msg);
            }
            else
            {
                /* loop through the the names */
                wizType = 0;
                int wizTypeInt = (int)wizType;
                string[] extractedValues;
                while ((extractedValues = CLibFile.fscanf(wizard_file, "%ld %s %s %s %d\n", wizTypeInt, theNetwork, theAccount, theCharacter, exceptionFlag)).Length == 5) //read each wizard from file
                {
                    //debug
                    if (UnityGameController.StopApplication) //time to quit
                    {
                        Debug.Log("Thread " + System.Threading.Thread.CurrentThread.Name + " stopping in Do_entering_character while");
                        break;
                    }
                    else
                    {
                        Debug.Log("extractedValues length is " + extractedValues.Length);
                        System.Threading.Thread.Sleep(33); //30fps
                    }

                    //unity: populating extracted values manually due to param/ref argument restrictions
                    //exceptionFlag is unused anyway
                    wizTypeInt = Convert.ToInt32(extractedValues[0]);
                    theNetwork = (extractedValues[1]);
                    theAccount = (extractedValues[2]);
                    theCharacter = (extractedValues[3]);
                    exceptionFlag = Convert.ToInt32(extractedValues[4]);

                    wizType = (short)wizTypeInt;
                    if (!CFUNCTIONS.strcmp(theCharacter, c.player.lcname)
                        && !CFUNCTIONS.strcmp(theAccount, c.lcaccount)
                        && ((!CFUNCTIONS.strcmp(theNetwork,c.network)) || exceptionFlag != 0))
                    {

                        c.wizard = wizType;
                        break;
                    }
                }
                
                CLibFile.fclose(wizard_file);
            }
            
            /* if appearing in ch 8 with a palantir, make them hear ch 1 */
            if (c.channel == 8)
            {

                if (!c.player.palantir)
                {
                    c.channel = 1;
                }
                else
                {
                    c.game.hearAllChannels = phantdefs.HEAR_ONE;
                }
            }
            
            /* send everyone your spec */
            Do_send_specification(c, phantdefs.ADD_PLAYER_EVENT);

            /* put characters in their proper area */
            if (c.wizard > 2)
            {
                c.player.location = (short)phantdefs.PL_VALHALLA;
            }
            else if (c.player.purgatoryFlag)
            {
                c.player.location = (short)phantdefs.PL_PURGATORY;
            }
            else
            {
                c.player.location = (short)phantdefs.PL_REALM;
            }
            
            /* force the loaded values to the players status window */
            statsclass.Do_update_stats(c);

            /* log the entry */
            string_buffer = CFUNCTIONS.sprintfSinglestring("[%s] Entering realm with character %s.\n", c.connection_id, c.player.lcname);

            fileclass.Do_log(pathnames.CONNECTION_LOG, string_buffer);

            /* set the player's timeout */
            if (c.wizard > 2)
            {
                c.timeout = 900;
            }
            else if (c.player.level < 80)
            {
                c.timeout = (int)Mathf.Floor(60 - (float)c.player.level / 2);
            }
            else
            {
                c.timeout = 20;
            }
            
            /* show the player last load stats and update */
            infoclass.Do_last_load_info(c);

            ioclass.Do_send_line(c, "\n");

            /* show the player the message of the day */
            /* try to open the MOTD file */
            errno = 0;
            //motd_file = CLibFile.FileOpenRead(pathnames.MOTD_FILE);
            if ((motd_file = CLibFile.fopen(pathnames.MOTD_FILE, "r", ref errno)) == null
                || (motd_file != null && CLibFile.fgets(ref string_buffer, phantdefs.SZ_LINE, motd_file) == null))
            {

                /* log an error message */
                error_msg = CFUNCTIONS.sprintfSinglestring("[%s] CLibFile.fopen of %s failed in Do_entering_character: %s.\n", c.connection_id, pathnames.MOTD_FILE,  CFUNCTIONS.strerror(errno));

                fileclass.Do_log_error(error_msg);
            }
            else
            {
                int indexOfSearchterm = CFUNCTIONS.strstr(string_buffer, "TESTING ");
                if (indexOfSearchterm < 0)
                    char_ptr = '\0';
                else
                    char_ptr = 'a'; //dummy value

                if ((char_ptr != '\0') && c.wizard < 1)
                {
                    //char_ptr += sizeof("TESTING ");
                    string incrementAmount = "TESTING ";
                    string char_ptrstring = string_buffer.Substring(indexOfSearchterm + incrementAmount.Length);

                    /* send a message to the user */
                    if (CFUNCTIONS.sscanf(char_ptrstring, "%d", ref minutes) != 0)
                    {
                        string_buffer = CFUNCTIONS.sprintfSinglestring("The game is currently down for testing.  Try back in 15 minutes.\n");
                    }
                    else
                    {
                        string_buffer = CFUNCTIONS.sprintfSinglestring("The game is currently down for testing.  Try back in %d minutes.\n", minutes);
                    }

                    socketclass.Do_send_error(c, string_buffer);

                    if (c.characterLoaded == true)
                    {
                        c.run_level = (short)phantdefs.SAVE_AND_EXIT;
                    }
                    else
                    {
                        c.run_level = (short)phantdefs.EXIT_THREAD;
                    }
                    
                    CLibFile.fclose(motd_file);

                    return;

                }
                else
                {
                    /* print message of the day */
                    ioclass.Do_send_line(c, string_buffer);
                }
                
                CLibFile.fclose(motd_file);
            }

            socketclass.Do_send_buffer(c);
            if (c.wizard < 1)
            {
                CLibPThread.sleep(3);
            }
            ioclass.Do_more(c);
            ioclass.Do_send_clear(c);

            /* announce the player entrance */
            if (c.wizard > 2)
            {

                string_buffer = CFUNCTIONS.sprintfSinglestring("In a brilliant flash, Wizard %s appears in the realm!\n", c.modifiedName);

            }
            else if (c.wizard == 2)
            {

                string_buffer = CFUNCTIONS.sprintfSinglestring("In a puff of smoke, Apprentice %s the %s appears in the realm!\n", c.modifiedName, c.realm.charstats[c.player.type].class_name);

            }
            else if (c.player.special_type == phantdefs.SC_VALAR)
            {

                string_buffer = CFUNCTIONS.sprintfSinglestring("Tremble, for the Valar %s has arrived!\n", c.modifiedName);

            }
            else
            {

                string_buffer = CFUNCTIONS.sprintfSinglestring("A new player appears in the realm, %s the %s.\n", c.modifiedName, c.realm.charstats[c.player.type].class_name);

            }

            c.characterAnnounced = true;

            ioclass.Do_broadcast(c, string_buffer);

            /* It's okay to chat now */
            socketclass.Do_send_int(c, phantdefs.ACTIVATE_CHAT_PACKET);


            if (c.player.energy <= 0.0)
            {
                event_ptr = eventclass.Do_create_event();
                event_ptr.type = (short)phantdefs.DEATH_EVENT;
                event_ptr.arg1 = (double) (c.battle.ring_in_use ? 1 : 0);
                event_ptr.arg3 = phantdefs.K_NO_ENERGY;
                eventclass.Do_handle_event(c, event_ptr);
            }



            /* handle purgatory */
            if (c.player.purgatoryFlag)
            {

                ioclass.Do_send_line(c, "This character was in combat previously when the connection was interrupted.  You will now re-encounter that monster.\n");

                ioclass.Do_more(c);
                ioclass.Do_send_clear(c);
                c.player.purgatoryFlag = false;

                /* Throw the monster at the player */
                event_ptr = eventclass.Do_create_event();
                event_ptr.type = (short)phantdefs.MONSTER_EVENT;
                event_ptr.arg1 = phantdefs.MONSTER_PURGATORY;
                event_ptr.arg3 = (long) c.player.monsterNumber;
                eventclass.Do_handle_event(c, event_ptr);

                /* return the player to the realm */
                c.player.location = (short)phantdefs.PL_REALM;
                statsclass.Do_location(c, c.player.x, c.player.y, 1);
            }
        }


        internal void Do_handle_save(client_t c)
        {
            string string_buffer = ""; //[phantdefs.SZ_LINE];

            Do_save_character(c, c.player);

            /* remove the character's backup */
            Do_backup_save(c, 0);
            c.characterLoaded = false;

            /* log the saving */
            string_buffer = CFUNCTIONS.sprintfSinglestring("[%s] %s saved\n", c.connection_id, c.player.lcname);

            fileclass.Do_log(pathnames.GAME_LOG, string_buffer);

            /* if server is shutting down or socket error */
            if (c.run_level == phantdefs.SAVE_AND_CONTINUE)
            {
                c.run_level = (short)phantdefs.GO_AGAIN;
            }
            else
            {
                c.run_level = (short)phantdefs.EXIT_THREAD;
            }

            return;
        }
        
        internal void Do_leaving_character(client_t c)
        {
            string string_buffer = ""; //[phantdefs.SZ_LINE];
            event_t event_ptr = new event_t();
            float ftemp;

            /* handle remaining events */
            eventclass.Do_orphan_events(c);
            while (c.events != null)
            {
                /* remove the next event */
                event_ptr = c.events;
                c.events = event_ptr.next_event;

                /* take care of it */
                if (event_ptr.type > phantdefs.GAME_MARKER)
                {
                    event_ptr = null;//free((void*) event_ptr);
                }
                else
                {
                    eventclass.Do_handle_event(c, event_ptr);
                }
            }
            
            /* if the character is steward, knight, or king, de-throne him first */
            if (c.player.special_type == phantdefs.SC_STEWARD || c.player.special_type == phantdefs.SC_KNIGHT || c.player.special_type == phantdefs.SC_KING)
            {

                commandsclass.Do_dethrone(c);
            }

            ftemp = Mathf.Pow(Mathf.Abs((float)c.player.x) / 100, 0.5f);

            /* if the character is on a post, boot him off */
            if ((Mathf.Abs((float)c.player.x) == Mathf.Abs((float)c.player.y)) && (Mathf.Floor(ftemp) == ftemp))
            {

                /* if the player can not leave the chamber now, return */
                if (c.stuck)
                {
                    ioclass.Do_send_line(c, "Another player is arriving...'\n");


                    ioclass.Do_more(c);

                    ioclass.Do_send_clear(c);
                    return;
                }

                /* Kick live players off of posts, dead characters off of door steps */

                if ((c.player.energy <= 0.0) || (c.player.strength <= 0.0))
                {
                    
                    ioclass.Do_send_line(c, "The merchant scowls, and kicks your corpse off the steps of his shop!'\n");

                }
                else
                {

                    event_ptr = eventclass.Do_create_event();
                    event_ptr.type = (short)phantdefs.MOVE_EVENT;
                    event_ptr.arg3 = phantdefs.A_NEAR;
                    eventclass.Do_handle_event(c, event_ptr);
                    
                    ioclass.Do_send_line(c, "The merchant scowls, says 'No loitering!, and throws you out!'\n");
                }
            }

            /* if the character is valar, remove from the realm */
            if (c.player.special_type == phantdefs.SC_VALAR)
            {
                miscclass.Do_lock_mutex(c.realm.realm_lock);
                c.realm.valar = null;
                miscclass.Do_unlock_mutex(c.realm.realm_lock);
            }

            /* turn off palantir */
            if (c.channel == 8)
            {
                c.game.hearAllChannels = phantdefs.HEAR_SELF;
            }
            
            /* announce the player's departure */
            if (c.run_level == phantdefs.SAVE_AND_CONTINUE || c.run_level == phantdefs.SAVE_AND_EXIT)
            {

                if (c.wizard > 2)
                {

                    string_buffer = CFUNCTIONS.sprintfSinglestring("%s slowly fades from the realm!\n", c.modifiedName);

                }
                else
                {

                    string_buffer = CFUNCTIONS.sprintfSinglestring("%s retires from the realm.\n", c.modifiedName);
                }

                ioclass.Do_broadcast(c, string_buffer);
            }

            /* kill the character if leaving quickly */
            else if (c.run_level == phantdefs.EXIT_THREAD)
            {

                /* kill the player */
                event_ptr = eventclass.Do_create_event();
                event_ptr.type = (short)phantdefs.DEATH_EVENT;
                event_ptr.arg1 = 0;
                event_ptr.arg3 = phantdefs.K_SUICIDE;
                eventclass.Do_handle_event(c, event_ptr);
            }

            /* erase the player description */
            miscclass.Do_lock_mutex(c.realm.realm_lock);
            //free(c.game.description);
            c.game.description = null;
            miscclass.Do_unlock_mutex(c.realm.realm_lock);
            
            /* remove the player specification */
            Do_send_specification(c, phantdefs.REMOVE_PLAYER_EVENT);

            c.characterAnnounced = false;

            c.timeout = 120;

            /* chat no more */
            socketclass.Do_send_int(c, phantdefs.DEACTIVATE_CHAT_PACKET);

            /* log the entry */
            string_buffer = CFUNCTIONS.sprintfSinglestring("[%s] Leaving realm.\n", c.connection_id);
            fileclass.Do_log(pathnames.CONNECTION_LOG, string_buffer);

            /* record time character has been playing */
            c.player.time_played += CFUNCTIONS.GetUnixEpoch(DateTime.Now) - c.player.last_load;

            if (c.wizaccount != null && c.wizaccount[0] != '\0')
            {
                c.account = c.wizaccount;
                c.wizaccount = new string(new char[] {'\0'});
            }

            if (c.wizIP != null && c.wizIP[0] != '\0')
            {
                c.IP = c.wizIP;
                c.wizIP = new string(new char[] { '\0' });
            }
        }
    }
}
