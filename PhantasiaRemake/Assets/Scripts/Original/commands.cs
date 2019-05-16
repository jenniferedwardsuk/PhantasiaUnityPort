using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace phantasiaclasses
{
    public class commands //: MonoBehaviour
    {
        phantasiaclasses.misc miscclass;
        phantasiaclasses.io ioclass;
        phantasiaclasses.tags tagsclass;
        phantasiaclasses.eventsrc eventclass;
        phantasiaclasses.socket socketclass;
        phantasiaclasses.stats statsclass;
        phantasiaclasses.file fileclass;
        phantasiaclasses.character characterclass;
        phantasiaclasses.main mainclass;
        phantasiaclasses.treasure treasureclass;
        phantasiaclasses.hack hackclass;
        phantasiaclasses.info infoclass;
        phantasiaclasses.restore restoreclass;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        static commands Instance;
        private commands()
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
            socketclass = socket.GetInstance();
            infoclass = info.GetInstance();
            mainclass = main.GetInstance();
            treasureclass = treasure.GetInstance();
            restoreclass = restore.GetInstance();
        }
        public static commands GetInstance()
        {
            commands instance = null;
            if (Instance != null)
            {
                instance = Instance;
            }
            else
            {
                instance = new commands();
            }
            return instance;
        }


        int server_hook;

        /************************************************************************
        /
        / FUNCTION NAME: Do_death(struct client_t *c, struct event_t the_event)
        /
        / FUNCTION: death routine
        /
        / AUTHOR: E. A. Estes, 12/4/85
        /	  Brian Kelly, 5/9/99
        /
        / ARGUMENTS:
        /       char *how - pointer to string describing cause of death
        /
        / RETURN VALUE: none
        /
        / MODULES CALLED: freerecord(), enterscore(), more(), exit(), CLibFile.fread(ref ),
        /       fseek(), execl(), CLibFile.fopen(), cfunctions.floor(), wmove(), drandom(), wclear(), cfunctions.strcmp(),
        /       CLibFile.fwrite(), fflush(), printw(), CFUNCTIONS.strcpy(ref ), CLibFile.fclose(), waddstr(), cleanup(),
        /       cfunctions.fprintf(), wrefresh(), getanswer(), descrtype()
        /
        / GLOBAL INPUTS: Curmonster, Wizard, Player, *stdscr, Fileloc, *Monstfp
        /
        / GLOBAL OUTPUTS: Player
        /
        / DESCRIPTION:
        /       Kill off current player.
        /       Handle rings, and multiple lives.
        /       Print an appropriate message.
        /       Update scoreboard, lastdead, and let other players know about
        /       the demise of their comrade.
        /
        *************************************************************************/

        static string[] deathmesg =
            /* add more messages here, if desired */
            {
                    "You have been wounded beyond repair.\n",
                    "You have been disemboweled.\n",
                    "You've been mashed, mauled, and spit upon.  (You're dead.)\n",
                    "You died!\n",
                    "You're a complete failure -- you've died!!\n",
                    "You have been dealt a fatal blow!\n"
            };

        internal void Do_death(client_t c, event_t the_event)
        {
            scoreboard_t sb = new scoreboard_t();
            event_t event_ptr = new event_t();
            realm_object_t object_ptr = new realm_object_t();
            linked_list_t list_ptr = new linked_list_t();
            tag_t newTag = new tag_t();
            bool save_flag, corpse_flag, score_flag;        /* do extra lives help? */
            string string_buffer = ""; //[phantdefs.SZ_LINE], error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];

            save_flag = true;
            corpse_flag = false;
            score_flag = true;

            /* reset fatigue on death */
            c.battle.rounds = 0;
            c.battle.timeouts = 0;

            c.events = null;

            switch (the_event.arg3)
            {

                case phantdefs.K_TRANSFORMED:


                    miscclass.Do_lock_mutex(c.realm.monster_lock);


                    string_buffer = CFUNCTIONS.sprintfSinglestring("You were turned into a %s.\n", c.realm.monster[(int)the_event.arg1].name);


                    miscclass.Do_unlock_mutex(c.realm.monster_lock);


                    ioclass.Do_send_line(c, string_buffer);


                    CFUNCTIONS.sprintf(ref sb.how_died, "was turned into a %s", c.realm.monster[(int)the_event.arg1].name);

                    break;

                case phantdefs.K_OLD_AGE:

                    ioclass.Do_send_line(c, "Your character dies of old age.\n");

                    CFUNCTIONS.strcpy(ref sb.how_died, "died of old age");
                    save_flag = false;
                    break;

                case phantdefs.K_MONSTER:

                    if (!CFUNCTIONS.strcmp((string)the_event.arg4, c.realm.monster[40].name))
                    {

                        string_buffer = CFUNCTIONS.sprintfSinglestring("%s turned you into a Smurf!\n",
                        the_event.arg4);

                        ioclass.Do_send_line(c, string_buffer);

                        ioclass.Do_send_line(c, "How smurfy!\n");

                        /* name smurf after player */
                        miscclass.Do_lock_mutex(c.realm.monster_lock);
                        string_buffer = CFUNCTIONS.sprintfSinglestring("%s Smurf", c.modifiedName);

                        CFUNCTIONS.strcpy(ref c.realm.monster[40].name, string_buffer);

                        miscclass.Do_unlock_mutex(c.realm.monster_lock);

                        CFUNCTIONS.sprintf(ref sb.how_died, "was smurfed by %s", (string)the_event.arg4);

                    }
                    else if (!CFUNCTIONS.strcmp((string)the_event.arg4, "Morgoth"))
                    {
                        string_buffer = CFUNCTIONS.sprintfSinglestring("Morgoth roars in triumph as he bashes %s's skull in with Grond, Hammer of the Underworld!\n", c.modifiedName);
                        ioclass.Do_broadcast(c, string_buffer);


                        CFUNCTIONS.sprintf(ref sb.how_died, "was killed by %s", (string)the_event.arg4);

                    }
                    else
                    {

                        string_buffer = CFUNCTIONS.sprintfSinglestring("You were killed by %s.\n", the_event.arg4);

                        ioclass.Do_send_line(c, string_buffer);

                        CFUNCTIONS.sprintf(ref sb.how_died, "was killed by %s", (string)the_event.arg4);

                    }

                    //the_event.arg4 = null; //free(the_event.arg4);
                    the_event.arg4 = null;
                    c.battle.ring_in_use = the_event.arg1 != 0 ? true : false;
                    corpse_flag = true;

                    break;

                case phantdefs.K_FATIGUE:

                    CFUNCTIONS.sprintf(ref string_buffer, "Because of your fatigue, %s found you easy prey.\n", (string)the_event.arg4);

                    ioclass.Do_send_line(c, string_buffer);

                    CFUNCTIONS.sprintf(ref sb.how_died, "was killed by fatigue");

                    //the_event.arg4 = null; //free(the_event.arg4);
                    the_event.arg4 = null;
                    c.battle.ring_in_use = the_event.arg1 != 0 ? true : false;
                    corpse_flag = true;

                    break;

                case phantdefs.K_GREED:

                    CFUNCTIONS.sprintf(ref string_buffer, "Because of your greed, %s found you easy prey.\n", (string)the_event.arg4);

                    ioclass.Do_send_line(c, string_buffer);

                    CFUNCTIONS.sprintf(ref sb.how_died, "was killed by greed");

                    //the_event.arg4 = null; //free(the_event.arg4);
                    the_event.arg4 = null;
                    c.battle.ring_in_use = the_event.arg1 != 0 ? true : false;
                    corpse_flag = true;

                    break;

                case phantdefs.K_IT_COMBAT:

                    miscclass.Do_lock_mutex(c.realm.realm_lock);

                    string_buffer = CFUNCTIONS.sprintfSinglestring("You died in glorious battle against %s.\n", the_event.arg4);

                    ioclass.Do_send_line(c, string_buffer);

                    CFUNCTIONS.sprintf(ref sb.how_died, "died in glorious battle against %s", (string)the_event.arg4);

                    miscclass.Do_unlock_mutex(c.realm.realm_lock);

                    //the_event.arg4 = null; //free((void*) the_event.arg4);
                    the_event.arg4 = null;

                    break;

                case phantdefs.K_GHOSTBUSTERS:

                    CFUNCTIONS.strcpy(ref sb.how_died, "vanished from the internet");
                    save_flag = false;
                    break;

                case phantdefs.K_SEGMENTATION:


                    CFUNCTIONS.strcpy(ref sb.how_died, "fell into a segmentation fault");
                    save_flag = false;
                    break;

                case phantdefs.K_VAPORIZED:


                    miscclass.Do_lock_mutex(c.realm.realm_lock);


                    string_buffer = CFUNCTIONS.sprintfSinglestring("You were vaporized by %s!\n", the_event.arg4);


                    ioclass.Do_send_line(c, string_buffer);

                    ioclass.Do_send_line(c, "Next time, try a disintegration proof vest.\n");


                    CFUNCTIONS.sprintf(ref sb.how_died, "was vaporized by %s", (string)the_event.arg4);


                    miscclass.Do_unlock_mutex(c.realm.realm_lock);

                    //the_event.arg4 = null; //free((void*) the_event.arg4);
                    the_event.arg4 = null;

                    score_flag = false;
                    save_flag = false;
                    break;

                case phantdefs.K_RING:

                    ioclass.Do_send_line(c, "Your ring has taken control of you and turned you into a monster!\n");

                    /* bad ring in possession; name idiot after player */
                    miscclass.Do_lock_mutex(c.realm.monster_lock);

                    string_buffer = CFUNCTIONS.sprintfSinglestring("%s the Idiot", c.modifiedName);

                    CFUNCTIONS.strcpy(ref c.realm.monster[14].name, string_buffer);

                    miscclass.Do_unlock_mutex(c.realm.monster_lock);

                    CFUNCTIONS.strcpy(ref sb.how_died, "was consumed by a cursed ring");
                    save_flag = false;
                    break;

                case phantdefs.K_NO_ENERGY:

                    ioclass.Do_send_line(c, "You died from your earlier wounds.\n");

                    CFUNCTIONS.strcpy(ref sb.how_died, "died from massive internal bleeding");

                    corpse_flag = true;
                    break;

                case phantdefs.K_FELL_OFF:

                    ioclass.Do_send_line(c, "You stepped over the edge of the realm and died in the void.\n");

                    CFUNCTIONS.strcpy(ref sb.how_died, "leisurely stepped over the edge of the realm");

                    save_flag = false;
                    break;

                case phantdefs.K_SUICIDE:

                    ioclass.Do_send_line(c, "At the behest of the game administrators, you thrust your own weapon through your heart.\n");


                    CFUNCTIONS.strcpy(ref sb.how_died, "committed suicide");

                    save_flag = false;
                    corpse_flag = false;
                    score_flag = false;

                    /* ban the character for 5-30 minutes to prevent him from coming back so quickly */
                    newTag.type = (short)phantdefs.T_BAN;
                    newTag.validUntil = CFUNCTIONS.GetUnixEpoch(DateTime.Now) + 300 + (int)(macros.RND() * 1500);
                    newTag.affectNetwork = false;
                    CFUNCTIONS.strcpy(ref newTag.description, "suicide tag");

                    /* send it */
                    tagsclass.Do_tag_self(c, newTag);
                    break;

                case phantdefs.K_SQUISH:

                    ioclass.Do_send_line(c, "You come across an old bridge keeper who asks you your name, quest, and favorite color...\n");


                    ioclass.Do_more(c);

                    ioclass.Do_send_clear(c);


                    ioclass.Do_send_line(c, "Unfortuantely, in a momentary lapse of concentration, you say Blue instead of Green and are hurled into the gorge of eternal peril!\n");

                    ioclass.Do_more(c);

                    ioclass.Do_send_clear(c);


                    CFUNCTIONS.sprintf(ref sb.how_died, "was hurled off of the bridge of death into the gorge of eternal peril", (string)the_event.arg4);

                    save_flag = false;
                    corpse_flag = true;
                    score_flag = false;
                    break;

                case phantdefs.K_SIN:

                    if (c.player.level >= phantdefs.MIN_KING && c.player.special_type != phantdefs.SC_EXVALAR)
                    {
                        ioclass.Do_send_line(c, "Because of your great evil, you have become the new Dark Lord!\n");

                        /* name dark lord after player */
                        miscclass.Do_lock_mutex(c.realm.monster_lock);
                        if (c.player.gender == (phantdefs.MALE != 0 ? true : false))
                        {
                            string_buffer = CFUNCTIONS.sprintfSinglestring("The Dark Lord %s", c.modifiedName);
                        }
                        else
                        {
                            string_buffer = CFUNCTIONS.sprintfSinglestring("The Dark Lady %s", c.modifiedName);

                        }

                        CFUNCTIONS.strcpy(ref c.realm.monster[99].name, string_buffer);

                        miscclass.Do_unlock_mutex(c.realm.monster_lock);


                        CFUNCTIONS.strcpy(ref sb.how_died, "succumbed to the lure of the Dark Side");


                        /* don't put dark lords on the scoreboard */
                        score_flag = false;

                        c.player.lives = 0;
                        if (c.player.special_type == phantdefs.SC_VALAR)
                        {
                            eventclass.Do_send_self_event(c, phantdefs.VALAR_EVENT);
                        }
                    }
                    else
                    {

                        ioclass.Do_send_line(c, "Your pathetic attempts to abuse others has turned you into a Smurf!  Lah lah la lah la la!\n");

                        /* name smurf after player */
                        miscclass.Do_lock_mutex(c.realm.monster_lock);

                        if (c.player.gender == (phantdefs.MALE != 0 ? true : false))
                        {
                            string_buffer = CFUNCTIONS.sprintfSinglestring("Papa Smurf %s", c.modifiedName);
                        }
                        else
                        {
                            string_buffer = CFUNCTIONS.sprintfSinglestring("Mama Smurf %s", c.modifiedName);
                        }

                        CFUNCTIONS.strcpy(ref sb.how_died, "was transformed into a Smurf");

                        CFUNCTIONS.strcpy(ref c.realm.monster[40].name, string_buffer);

                        miscclass.Do_unlock_mutex(c.realm.monster_lock);
                    }


                    save_flag = false;
                    corpse_flag = false;
                    break;

                default:

                    ioclass.Do_send_line(c, deathmesg[(int)macros.ROLL(0.0, -1)]); // (double)sizeof(deathmesg) / sizeof(char*))]); //todo

                    CFUNCTIONS.strcpy(ref sb.how_died, "died in a bizarre gardening accident");
                    break;
            }

            if (c.player.type == phantdefs.C_EXPER)
            {
                corpse_flag = false;
                ioclass.Do_send_line(c, "A mist rises from your body . . .\n");
                socketclass.Do_send_buffer(c);
                CLibPThread.sleep(10);
                if (macros.RND() < .05)
                {
                    ioclass.Do_send_line(c, "and it reforms into your old body!\n");
                    if (c.player.lives == 0)
                        c.player.lives++;
                }
                else
                {
                    ioclass.Do_send_line(c, "But it disperses!\n");
                }
            }

            /* check if the person can be saved from this death */
            if (save_flag)
            {
                /* if the player has a good ring in use */
                if (c.battle.ring_in_use && (c.player.ring_type == phantdefs.R_DLREG || c.player.ring_type == phantdefs.R_NAZREG))
                {
                    ioclass.Do_send_line(c, "But your ring saved you from death!\n");

                    ioclass.Do_more(c);

                    ioclass.Do_send_clear(c);

                    statsclass.Do_ring(c, phantdefs.R_NONE, 0);

                    statsclass.Do_energy(c, c.player.max_energy / 12.0 + 1.0, c.player.max_energy, c.player.shield, 0, 0);

                    return;
                }
                /* extra lives */
                else if (c.player.lives > 0)
                {
                    ioclass.Do_more(c);

                    ioclass.Do_send_clear(c);

                    ioclass.Do_send_line(c, "You should be more cautious.  You've been killed.\n");

                    string_buffer = CFUNCTIONS.sprintfSinglestring("You only have %d more chance(s).\n", --c.player.lives);

                    ioclass.Do_send_line(c, string_buffer);

                    ioclass.Do_more(c);

                    ioclass.Do_send_clear(c);

                    statsclass.Do_energy(c, c.player.max_energy + c.player.shield, c.player.max_energy, c.player.shield, 0, 0);

                    return;
                }
                /* a king who is killed only loses his post */
                else if (c.player.special_type == phantdefs.SC_KING)
                {
                    ioclass.Do_more(c);

                    ioclass.Do_send_clear(c);

                    ioclass.Do_send_line(c, "The head page rushes to your aid.  'Rest, your Majesty.', he says, 'You have served your people long enough'.\n");


                    ioclass.Do_more(c);

                    ioclass.Do_send_clear(c);


                    statsclass.Do_energy(c, c.player.max_energy / 5.0, c.player.max_energy, c.player.shield, 0, 0);


                    eventclass.Do_send_self_event(c, phantdefs.DETHRONE_EVENT);

                    miscclass.Do_lock_mutex(c.realm.realm_lock);
                    c.realm.king_name = "\0";
                    c.realm.king = null;
                    c.realm.king_flag = false;
                    miscclass.Do_unlock_mutex(c.realm.realm_lock);

                    c.player.special_type = (short)phantdefs.SC_NONE;

                    return;
                }
                /* if a valar lost all his lives */
                else if (c.player.special_type == phantdefs.SC_VALAR)
                {
                    ioclass.Do_more(c);

                    ioclass.Do_send_clear(c);

                    ioclass.Do_send_line(c, "You had your chances, but Valar aren't totally immortal.  You are now an ex-valar! . . .\n");


                    ioclass.Do_more(c);

                    ioclass.Do_send_clear(c);

                    /* heal the player completely and remove fatigue */
                    statsclass.Do_energy(c, c.player.max_energy + c.player.shield, c.player.max_energy, c.player.shield, 0, 0);

                    c.battle.rounds = 0;

                    statsclass.Do_speed(c, c.player.max_quickness, c.player.quicksilver, 0, 0);


                    /*
                        statsclass.Do_energy(c, c.player.max_energy / 5.0, c.player.max_energy, c.player.shield, 0, true);

                        statsclass.Do_strength(c, c.player.max_strength, 0, 0, false);
                        statsclass.Do_speed(c, c.player.max_quickness, 0, 0, false);
                        c.player.brains = c.player.level / 25.0;
                    */

                    eventclass.Do_send_self_event(c, phantdefs.VALAR_EVENT);
                    return;

                }
            }

            /* chat no more */
            socketclass.Do_send_int(c, phantdefs.DEACTIVATE_CHAT_PACKET);

            ioclass.Do_more(c);
            ioclass.Do_send_clear(c);

            /* create a corpse, if necessary */
            if (corpse_flag && c.player.level < 3000)
            {
                object_ptr = new realm_object_t();// (struct realm_object_t *) Do_malloc(phantdefs.SZ_REALM_OBJECT);
                object_ptr.type = (short)phantdefs.CORPSE;
                object_ptr.x = c.player.x;
                object_ptr.y = c.player.y;
                object_ptr.arg1 = characterclass.Do_copy_record(c.player);//, false);

                /* put the new event in the realm object list */
                miscclass.Do_lock_mutex(c.realm.realm_lock);
                object_ptr.next_object = c.realm.objects;
                c.realm.objects = object_ptr;

                miscclass.Do_unlock_mutex(c.realm.realm_lock);

            }
            /* if the character is special, de-throne him */
            if (c.player.special_type == phantdefs.SC_KING || c.player.special_type == phantdefs.SC_STEWARD || c.player.special_type == phantdefs.SC_KNIGHT)
            {
                Do_dethrone(c);
            }

            /* put the character on the scoreboard if: */
            /* the character is between 50 and not an ex-valar/wizard */

            if (score_flag && (c.wizard == 0 || c.wizard == 2) && c.player.level >= 100 && !c.player.beyond && c.player.special_type < phantdefs.SC_EXVALAR)
            {
                CFUNCTIONS.strcpy(ref sb.classclass, c.realm.charstats[c.player.type].class_name);
                CFUNCTIONS.strcpy(ref sb.name, c.modifiedName);
                CFUNCTIONS.strcpy(ref sb.from, c.network);

                sb.level = c.player.level;
                sb.time = CFUNCTIONS.GetUnixEpoch(DateTime.Now);


                fileclass.Do_scoreboard_add(c, sb, true);

            }

            /* save the character name */
            CFUNCTIONS.strcpy(ref c.previousName, c.player.lcname);
            list_ptr = new linked_list_t();// (struct linked_list_t *) Do_malloc(phantdefs.SZ_LINKED_LIST);
            CFUNCTIONS.strcpy(ref list_ptr.name, c.player.lcname);

            miscclass.Do_lock_mutex(c.realm.character_file_lock);
            list_ptr.next = c.realm.name_limbo;
            c.realm.name_limbo = list_ptr;
            miscclass.Do_unlock_mutex(c.realm.character_file_lock);

            /* broadcast the death */
            string_buffer = CFUNCTIONS.sprintfSinglestring("%s %s.\n", c.modifiedName, sb.how_died);
            ioclass.Do_broadcast(c, string_buffer);

            /* log the death */
            string_buffer = CFUNCTIONS.sprintfSinglestring("[%s] %s died\n", c.connection_id, c.player.lcname);

            fileclass.Do_log(pathnames.GAME_LOG, string_buffer);

            string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, %d age, %d seconds, dead\n", c.player.lcname, c.realm.charstats[c.player.type].class_name, c.player.age, c.player.time_played + CFUNCTIONS.GetUnixEpoch(DateTime.Now) - c.player.last_load);

            fileclass.Do_log(pathnames.LEVEL_LOG, string_buffer);

            c.run_level = (short)phantdefs.GO_AGAIN;

        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_valar(struct client_t *c)
        /
        / FUNCTION: toggle the player's valar status
        /
        / AUTHOR: E. A. Estes, 12/4/85
        / 	  Brian Kelly, 5/16/99
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

        internal void Do_valar(client_t c)
        {
            event_t event_ptr = new event_t();
            scoreboard_t sb = new scoreboard_t();
            string string_buffer = ""; //[phantdefs.SZ_LINE];
            float ftemp;

            /* if the character is already valar */
            if (c.player.special_type == phantdefs.SC_VALAR)
            {

                miscclass.Do_lock_mutex(c.realm.realm_lock);

                if (c.realm.valar == c.game)
                {
                    c.realm.valar = null;
                    c.realm.valar_name = "\0";
                }
                miscclass.Do_unlock_mutex(c.realm.realm_lock);

                ioclass.Do_send_line(c, "You are no longer a Valar!\n");
                ioclass.Do_more(c);

                ioclass.Do_send_clear(c);
                c.player.special_type = (short)phantdefs.SC_EXVALAR;
                c.broadcast = false;
            }
            else
            {
                ioclass.Do_send_line(c, "You have ascended to the position of Valar!\n");
                ioclass.Do_more(c);

                ioclass.Do_send_clear(c);

                miscclass.Do_lock_mutex(c.realm.realm_lock);

                /* remove the current valar if there is one */
                if ((c.realm.valar != null) && (c.realm.valar != c.game))
                {
                    event_ptr = eventclass.Do_create_event();
                    event_ptr.type = (short)phantdefs.DETHRONE_EVENT;
                    event_ptr.to = c.realm.valar;
                    event_ptr.from = c.game;

                    eventclass.Do_send_event(event_ptr);

                }

                c.realm.valar = c.game;

                CFUNCTIONS.strcpy(ref c.realm.valar_name, c.modifiedName);
                miscclass.Do_unlock_mutex(c.realm.realm_lock);

                if (c.player.special_type == phantdefs.SC_COUNCIL)
                {
                    CFUNCTIONS.strcpy(ref sb.classclass, c.realm.charstats[c.player.type].class_name);
                    CFUNCTIONS.strcpy(ref sb.name, c.modifiedName);
                    CFUNCTIONS.strcpy(ref sb.from, c.network);
                    sb.level = 9999;
                    sb.time = CFUNCTIONS.GetUnixEpoch(DateTime.Now);


                    fileclass.Do_scoreboard_add(c, sb, false);
                    c.player.lives = 3;

                }
                else if (c.player.lives < 3)
                {
                    c.player.lives = 3;
                }

                c.player.special_type = (short)phantdefs.SC_VALAR;

                /* send a chat message notifing everyone */
                string_buffer = CFUNCTIONS.sprintfSinglestring("Behold the new Valar %s!\n", c.modifiedName);

                ioclass.Do_broadcast(c, string_buffer);

            }

            /* modify player description */
            miscclass.Do_lock_mutex(c.realm.realm_lock);
            characterclass.Do_player_description(c);
            miscclass.Do_unlock_mutex(c.realm.realm_lock);

            /* send everyone new specs */
            characterclass.Do_send_specification(c, phantdefs.CHANGE_PLAYER_EVENT);

            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_knight(struct client_t *c)
        /
        / FUNCTION: make the player a knight
        /
        / AUTHOR: E. A. Estes, 12/4/85
        / 	  Brian Kelly, 5/10/01
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

        internal void Do_knight(client_t c, event_t the_event)
        {
            event_t event_ptr;
            string string_buffer = ""; //[phantdefs.SZ_LINE];

            /* don't do anything if we're already special */
            if (c.player.special_type != 0)
            {
                return;
            }

            /* if there is currently another knight, dethrone him */
            miscclass.Do_lock_mutex(c.realm.realm_lock);
            if (c.realm.knight != null)
            {
                event_ptr = eventclass.Do_create_event();
                event_ptr.type = (short)phantdefs.DETHRONE_EVENT;
                event_ptr.to = c.realm.knight;
                event_ptr.from = c.game;

                eventclass.Do_send_event(event_ptr);

            }

            c.realm.knight = c.game;
            miscclass.Do_unlock_mutex(c.realm.realm_lock);
            c.player.special_type = (short)phantdefs.SC_KNIGHT;

            string_buffer = CFUNCTIONS.sprintfSinglestring("You have been knighted by %s!\n", the_event.arg4);
            ioclass.Do_send_line(c, string_buffer);
            //the_event.arg4 = null; //free((void*) the_event.arg4);
            the_event.arg4 = null;
            ioclass.Do_more(c);
            ioclass.Do_send_clear(c);

            c.knightEnergy = Mathf.Floor((float)c.player.max_energy / 4);
            c.knightQuickness = Mathf.Floor(c.player.max_quickness / 10);

            statsclass.Do_energy(c, c.player.energy + c.knightEnergy, c.player.max_energy, c.player.shield, 0, 0);

            statsclass.Do_speed(c, c.player.max_quickness, c.player.quicksilver, 0, 0);

            /* send everyone new specs */
            characterclass.Do_send_specification(c, phantdefs.CHANGE_PLAYER_EVENT);

            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_steward(struct client_t *c)
        /
        / FUNCTION: make the player a steward
        /
        / AUTHOR: E. A. Estes, 12/4/85
        / 	  Brian Kelly, 5/9/01
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

        internal void Do_steward(client_t c)
        {
            event_t event_ptr;
            realm_object_t object_ptr, object_ptr_ptr;
            string string_buffer = ""; //[phantdefs.SZ_LINE];

            if (c.player.special_type == phantdefs.SC_KNIGHT)
            {
                Do_dethrone(c);
            }

            /* if there is currently a steward, dethrone him */
            miscclass.Do_lock_mutex(c.realm.realm_lock);
            if (c.realm.king != null)
            {
                event_ptr = eventclass.Do_create_event();
                event_ptr.type = (short)phantdefs.DETHRONE_EVENT;
                event_ptr.to = c.realm.king;
                event_ptr.from = c.game;

                eventclass.Do_send_event(event_ptr);

            }

            /* erase all energy voids */
            miscclass.Do_lock_mutex(c.realm.object_lock);
            object_ptr_ptr = c.realm.objects;
            object_ptr = object_ptr_ptr;
            while (object_ptr != null)
            {
                if (object_ptr.type == phantdefs.ENERGY_VOID)
                {

                    object_ptr_ptr = object_ptr.next_object;

                    //free((void*) object_ptr);
                    object_ptr = null;
                }
                else
                {
                    object_ptr_ptr = object_ptr.next_object;
                }
                object_ptr = object_ptr_ptr;

            }
            miscclass.Do_unlock_mutex(c.realm.object_lock);

            c.realm.king = c.game;
            c.realm.king_flag = false;
            miscclass.Do_unlock_mutex(c.realm.realm_lock);
            c.player.special_type = (short)phantdefs.SC_STEWARD;

            ioclass.Do_send_line(c, "You have become the steward!\n");
            ioclass.Do_more(c);

            statsclass.Do_crowns(c, (-1), 0);

            /* send a chat message notifing everyone */
            string_buffer = CFUNCTIONS.sprintfSinglestring("%s has become the new steward.\n", c.modifiedName);

            ioclass.Do_broadcast(c, string_buffer);

            /* modify player description */
            miscclass.Do_lock_mutex(c.realm.realm_lock);
            characterclass.Do_player_description(c);
            miscclass.Do_unlock_mutex(c.realm.realm_lock);
            /* send everyone new specs */
            characterclass.Do_send_specification(c, phantdefs.CHANGE_PLAYER_EVENT);

            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_king(struct client_t *c)
        /
        / FUNCTION: do king stuff
        /
        / AUTHOR: E. A. Estes, 12/4/85
        / 	  Brian Kelly, 5/16/99
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

        internal void Do_king(client_t c)
        {
            event_t event_ptr;
            string string_buffer = ""; //[phantdefs.SZ_LINE];

            if (c.player.special_type == phantdefs.SC_KNIGHT)
            {
                Do_dethrone(c);
            }

            /* if there is currently a king or steward, dethrone him */
            miscclass.Do_lock_mutex(c.realm.realm_lock);
            if (c.realm.king != null)
            {
                event_ptr = eventclass.Do_create_event();
                event_ptr.type = (short)phantdefs.DETHRONE_EVENT;
                event_ptr.to = c.realm.king;
                event_ptr.from = c.game;

                eventclass.Do_send_event(event_ptr);
            }

            c.realm.king = c.game;
            c.realm.king_flag = true;
            CFUNCTIONS.strcpy(ref c.realm.king_name, c.modifiedName);
            miscclass.Do_unlock_mutex(c.realm.realm_lock);
            c.player.special_type = (short)phantdefs.SC_KING;

            if (c.player.gender == (phantdefs.MALE != 0 ? true : false))
                ioclass.Do_send_line(c, "You have become king!\n");
            else
                ioclass.Do_send_line(c, "You have become queen!\n");

            ioclass.Do_more(c);
            statsclass.Do_crowns(c, (-1), 0);

            /* send a chat message notifing everyone */
            if (c.player.gender == (phantdefs.MALE != 0 ? true : false))
            {
                string_buffer = CFUNCTIONS.sprintfSinglestring("Long live the king.  Hail King %s!\n",
                c.modifiedName);
            }
            else
            {
                string_buffer = CFUNCTIONS.sprintfSinglestring("Long live the queen.  Hail Queen %s!\n",
                c.modifiedName);
            }

            ioclass.Do_broadcast(c, string_buffer);

            /* modify player description */
            miscclass.Do_lock_mutex(c.realm.realm_lock);
            characterclass.Do_player_description(c);
            miscclass.Do_unlock_mutex(c.realm.realm_lock);
            /* send everyone new specs */
            characterclass.Do_send_specification(c, phantdefs.CHANGE_PLAYER_EVENT);

            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_dethrone(struct client_t *c)
        /
        / FUNCTION: remove the player from the throne
        /
        / AUTHOR: Brian Kelly, 7/06/00
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

        internal void Do_dethrone(client_t c)
        {
            event_t event_ptr;
            string string_buffer = ""; //[phantdefs.SZ_LINE];

            /* king or steward handled differently from knight */
            if (c.player.special_type == phantdefs.SC_KING || c.player.special_type ==
                phantdefs.SC_STEWARD)
            {

                /* if the player is on the throne, kick him off */
                if (c.player.location == phantdefs.PL_THRONE)
                {

                    /* if the player can not leave the chamber now, return */
                    if (c.stuck)
                    {


                        ioclass.Do_send_line(c,
                          "You are interrupted by another player entering the chamber.\n");


                        ioclass.Do_more(c);

                        ioclass.Do_send_clear(c);
                        return;
                    }

                    event_ptr = eventclass.Do_create_event();
                    event_ptr.type = (short)phantdefs.MOVE_EVENT;
                    event_ptr.arg3 = phantdefs.A_NEAR;
                    /* we must handle it now in case we're saving the game */
                    eventclass.Do_handle_event(c, event_ptr);


                    ioclass.Do_send_line(c,
                                        "Your pages say, 'Your Majesty, the throne is no place to sleep!'\n");
                }

                if (c.player.special_type == phantdefs.SC_KING)
                {

                    ioclass.Do_send_line(c, "You vacate the throne.\n");
                    string_buffer = CFUNCTIONS.sprintfSinglestring("%s has vacated the throne!\n",
                c.modifiedName);

                    miscclass.Do_lock_mutex(c.realm.realm_lock);
                    c.realm.king_flag = false;

                    /* check if player should get it back later*/
                    if ((c.realm.king != c.game) || c.player.level >= phantdefs.MAX_KING)
                    {
                        c.player.special_type = (short)phantdefs.SC_NONE;
                    }
                    else
                    {
                        c.realm.king = null;
                    }
                    miscclass.Do_unlock_mutex(c.realm.realm_lock);

                }
                else if (c.player.special_type == phantdefs.SC_STEWARD)
                {

                    ioclass.Do_send_line(c, "You are no longer steward!\n");
                    string_buffer = CFUNCTIONS.sprintfSinglestring("%s is no longer steward!\n",
                c.modifiedName);

                    /* dump the steward coffers into the king's coffers */

                    miscclass.Do_lock_mutex(c.realm.kings_gold_lock);
                    c.realm.kings_gold += c.realm.steward_gold;
                    c.realm.steward_gold = 0;

                    miscclass.Do_unlock_mutex(c.realm.kings_gold_lock);

                    c.player.special_type = (short)phantdefs.SC_NONE;

                    miscclass.Do_lock_mutex(c.realm.realm_lock);
                    if (c.realm.king == c.game)
                    {
                        c.realm.king = null;
                    }
                    miscclass.Do_unlock_mutex(c.realm.realm_lock);
                }


                ioclass.Do_more(c);

                /* send a chat message notifing everyone of the dethrone */
                ioclass.Do_broadcast(c, string_buffer);

                if (c.player.special_type == phantdefs.SC_KING && c.realm.knight != null)
                {
                    event_ptr = eventclass.Do_create_event();
                    event_ptr.type = (short)phantdefs.DETHRONE_EVENT;
                    event_ptr.to = c.realm.knight;
                    eventclass.Do_send_event(event_ptr);
                }

                miscclass.Do_lock_mutex(c.realm.realm_lock);

                /* modify player description */
                characterclass.Do_player_description(c);

                miscclass.Do_unlock_mutex(c.realm.realm_lock);

                /* send everyone new specs */
                characterclass.Do_send_specification(c, phantdefs.CHANGE_PLAYER_EVENT);
            }
            else if (c.player.special_type == phantdefs.SC_KNIGHT)
            {

                miscclass.Do_lock_mutex(c.realm.realm_lock);
                if (c.realm.knight == c.game)
                {
                    c.realm.knight = null;
                }

                /* modify player description */
                miscclass.Do_unlock_mutex(c.realm.realm_lock);

                ioclass.Do_send_line(c, "You are no longer a knight!\n");


                ioclass.Do_more(c);
                c.player.special_type = (short)phantdefs.SC_NONE;
                c.knightEnergy = 0;
                c.knightQuickness = 0;

                /* modify player description */
                miscclass.Do_lock_mutex(c.realm.realm_lock);
                characterclass.Do_player_description(c);
                miscclass.Do_unlock_mutex(c.realm.realm_lock);


                statsclass.Do_energy(c, c.player.energy, c.player.max_energy, c.player.shield,
                    0, 0);


                statsclass.Do_speed(c, c.player.max_quickness, c.player.quicksilver, 0, 0);

                /* send everyone new specs */
                characterclass.Do_send_specification(c, phantdefs.CHANGE_PLAYER_EVENT);
            }

            ioclass.Do_send_clear(c);
            return;
        }



        /************************************************************************
        /
        / FUNCTION NAME: Do_degenerate(struct client_t *c)
        /
        / FUNCTION: degenerate the player
        /
        / AUTHOR: E. A. Estes, 12/4/85
        /	  Brian Kelly, 5/16/99
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

        internal void Do_degenerate(client_t c, int counts)
        {
            event_t event_ptr = new event_t();

            /* age player slightly */
            if (counts != 0)
            {
                ++c.player.degenerated;
            }

            /* degenerate speed and quicksilver */
            statsclass.Do_speed(c, c.player.max_quickness * 0.975,
                            c.player.quicksilver * 0.93, c.battle.speedSpell, 0);

            statsclass.Do_energy(c, c.player.energy, c.player.max_energy * 0.97,
                c.player.shield * 0.93, c.battle.force_field, 0);

            statsclass.Do_strength(c, c.player.max_strength * 0.97,
                c.player.sword * 0.93, c.battle.strengthSpell, 0);

            c.player.brains *= 0.95;
            c.player.magiclvl *= 0.97;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_caused_degenerate(struct client_t *c, event_t *the_event)
        /
        / FUNCTION: do degenerate power
        /
        / AUTHOR: E. A. Estes, 12/4/85
        /	  Brian Kelly, 11/10/99
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

        internal void Do_caused_degenerate(client_t c, event_t the_event)
        {
            string string_buffer = ""; //[phantdefs.SZ_LINE];

            string_buffer = CFUNCTIONS.sprintfSinglestring("%s causes you and your equipment to degenerate!\n",
                the_event.arg4);

            ioclass.Do_send_line(c, string_buffer);
            the_event.arg4 = null; //the_event.arg4 = null; //free((void*)the_event.arg4);
            ioclass.Do_more(c);
            ioclass.Do_send_clear(c);

            Do_degenerate(c, 0);
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_blind(struct client_t *c, struct event_t *the_event)
        /
        / FUNCTION: toggle blindness
        /
        / AUTHOR: E. A. Estes, 12/4/85
        /	  Brian Kelly, 5/16/99
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

        internal void Do_blind(client_t c, event_t the_event)
        {
            string string_buffer = ""; //[phantdefs.SZ_LINE];

            /* if the character is blind */
            if (!c.player.blind)
            {
                CFUNCTIONS.sprintf(ref string_buffer,
                "You hear %s laugh as your sight dims and the world goes black!\n",
                (string)the_event.arg4);


                the_event.arg4 = null; //free((void*)the_event.arg4);
                ioclass.Do_send_line(c, string_buffer);
                ioclass.Do_more(c);

                ioclass.Do_send_clear(c);
                c.player.blind = true;
            }

            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_relocate(struct client_t *c, struct event_t *the_event)
        /
        / FUNCTION: do relocation command
        /
        / AUTHOR: E. A. Estes, 12/4/85
        /	  Brian Kelly, 5/16/99
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

        internal void Do_relocate(client_t c, event_t the_event)
        {
            event_t event_ptr = new event_t();
            string string_buffer = ""; //[phantdefs.SZ_LINE];

            if (Mathf.Abs((float)c.player.x) >= phantdefs.D_BEYOND
            || Mathf.Abs((float)c.player.y) >= phantdefs.D_BEYOND)
            {
                the_event.arg4 = null; //the_event.arg4 = null; //free((void*) the_event.arg4);

                return;
            }

            miscclass.Do_lock_mutex(c.realm.realm_lock);
            string_buffer = CFUNCTIONS.sprintfSinglestring("You have been relocated by %s!\n",
                the_event.arg4);
            miscclass.Do_unlock_mutex(c.realm.realm_lock);
            ioclass.Do_send_line(c, string_buffer);
            the_event.arg4 = null; //the_event.arg4 = null; //free((void*) the_event.arg4);

            ioclass.Do_more(c);
            ioclass.Do_send_clear(c);

            event_ptr = eventclass.Do_create_event();
            event_ptr.type = (short)phantdefs.MOVE_EVENT;
            event_ptr.arg1 = the_event.arg1;
            event_ptr.arg2 = the_event.arg2;
            event_ptr.arg3 = phantdefs.A_TELEPORT;
            eventclass.Do_handle_event(c, event_ptr);

            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_transport(struct client_t *c, struct event_t *the_event)
        /
        / FUNCTION: do transport and oust command
        /
        / AUTHOR: E. A. Estes, 12/4/85
        /	  Brian Kelly, 5/16/99
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

        internal void Do_transport(client_t c, event_t the_event)
        {
            event_t event_ptr;
            string string_buffer = ""; //[phantdefs.SZ_LINE];

            if (the_event.arg1 != 0)
            {


                string_buffer = CFUNCTIONS.sprintfSinglestring("%s ousts you from this location!\n",
                        the_event.arg4);
            }
            else
            {


                string_buffer = CFUNCTIONS.sprintfSinglestring("You have been transported by %s!\n",
                        the_event.arg4);
            }

            ioclass.Do_send_line(c, string_buffer);
            the_event.arg4 = null; // the_event.arg4 = null; //free((void*) the_event.arg4);

            if (the_event.arg1 == 0 && c.player.charms > 0)
            {

                ioclass.Do_send_line(c, "But your charm saved you. . .\n");
                ioclass.Do_more(c);

                ioclass.Do_send_clear(c);
                return;
            }

            ioclass.Do_more(c);
            ioclass.Do_send_clear(c);

            event_ptr = eventclass.Do_create_event();
            event_ptr.type = (short)phantdefs.MOVE_EVENT;

            /* is this the more powerful version? */
            if (the_event.arg1 != 0)
            {
                event_ptr.arg3 = phantdefs.A_OUST;
            }
            else
            {
                event_ptr.arg3 = phantdefs.A_TRANSPORT;
            }

            eventclass.Do_handle_event(c, event_ptr);
            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_curse(struct client_t *c, struct event_t *the_event)
        /
        / FUNCTION: do curse, smite, and execrate
        /
        / AUTHOR: E. A. Estes, 12/4/85
        /	  Brian Kelly, 5/16/99
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

        internal void Do_curse(client_t c, event_t the_event)
        {
            string string_buffer = ""; //[phantdefs.SZ_LINE];
            double dtemp;

            if (the_event.arg1 == phantdefs.P_SMITE)
            {

                CFUNCTIONS.sprintf(ref string_buffer,
                "%s smites you down for your insolence!\n",
                    (string)the_event.arg4);

            }
            else if (the_event.arg1 == phantdefs.P_EXECRATE)
            {

                string_buffer = CFUNCTIONS.sprintfSinglestring("You've been execrated by %s!\n",
                    the_event.arg4);
            }
            else
            {

                string_buffer = CFUNCTIONS.sprintfSinglestring("%s lays a heavy curse on you!\n",
                    the_event.arg4);
            }

            ioclass.Do_send_line(c, string_buffer);

            if (the_event.arg1 == phantdefs.P_CURSE && c.player.blessing)
            {

                ioclass.Do_send_line(c, "But your blessing saved you. . .\n");
                ioclass.Do_more(c);

                ioclass.Do_send_clear(c);

                return;
            }

            ioclass.Do_more(c);
            ioclass.Do_send_clear(c);

            if (the_event.arg1 == phantdefs.P_SMITE)
            {

                statsclass.Do_energy(c, 1.0, c.player.max_energy * 0.95, c.player.shield,
                    0, 0);

                /* no longer cloaked, blessed or sighted */
                statsclass.Do_cloak(c, 0, 0);
                statsclass.Do_blessing(c, 0, 0);
                c.player.blind = true;
                dtemp = 5.0;

                if (c.player.gender == (phantdefs.MALE != 0 ? true : false))
                {
                    string_buffer = CFUNCTIONS.sprintfSinglestring("%s smites %s for his insolence!\n", the_event.arg4, c.player.name);
                }
                else
                {
                    string_buffer = CFUNCTIONS.sprintfSinglestring("%s smites %s for her insolence!\n", the_event.arg4, c.player.name);
                }

                ioclass.Do_broadcast(c, string_buffer);
            }
            else if (the_event.arg1 == phantdefs.P_EXECRATE)
            {

                dtemp = Mathf.Floor((float)(c.player.energy / 10.0));

                if (dtemp < 1.0)
                {
                    dtemp = 1.0;
                }

                statsclass.Do_energy(c, dtemp, c.player.max_energy * 0.98, c.player.shield,
                    0, 0);

                string_buffer = CFUNCTIONS.sprintfSinglestring("%s pronounces an execration on %s!\n", the_event.arg4, c.player.name);

                /* no longer cloaked */
                statsclass.Do_cloak(c, 0, 0);
                dtemp = 2.0;
                ioclass.Do_broadcast(c, string_buffer);
            }
            else
            {

                dtemp = Mathf.Floor((float)(4.0 * c.player.energy / 5.0));

                if (dtemp < 1.0)
                {
                    dtemp = 1.0;
                }


                statsclass.Do_energy(c, dtemp, c.player.max_energy, c.player.shield, 0, 0);
                dtemp = 0.5;

                string_buffer = CFUNCTIONS.sprintfSinglestring("%s has received the curse of %s!\n", c.player.name, the_event.arg4);
                ioclass.Do_broadcast(c, string_buffer);
            }

            statsclass.Do_poison(c, dtemp);
            the_event.arg4 = null; // free((void*)the_event.arg4);
            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_bestow(struct client_t *c, struct event_t *the_event)
        /
        / FUNCTION: do bestowing
        /
        / AUTHOR: E. A. Estes, 12/4/85
        /	  Brian Kelly, 5/16/99
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

        internal void Do_bestow(client_t c, event_t the_event)
        {
            string string_buffer = ""; //[phantdefs.SZ_LINE];
            double dtemp;

            /* can't bestow to a cloaked player */
            if (c.player.cloaked)
            {

                string_buffer = CFUNCTIONS.sprintfSinglestring("You see %s's head page walk by.  He seems to be looking for something.\n", the_event.arg4);

                the_event.arg4 = null; // free((void*)the_event.arg4);

                ioclass.Do_send_line(c, string_buffer);

                ioclass.Do_more(c);

                ioclass.Do_send_clear(c);
                return;
            }

            if (Mathf.Abs((float)c.player.x) == Mathf.Abs((float)c.player.y) && c.player.location !=
                        phantdefs.PL_THRONE)
            {

                dtemp = Mathf.Sqrt(Mathf.Abs((float)c.player.x) / 100.0f);

                if (Mathf.Floor((float)dtemp) == dtemp)
                {
                    string_buffer = CFUNCTIONS.sprintfSinglestring("You see %s's head page approach.  The post owner leaves the shop, says \"Oh great!  My tax refund is finally here!\", grabs the gold from the page and disappears back into the post.\n", the_event.arg4);
                    ioclass.Do_send_line(c, string_buffer);

                    the_event.arg4 = null; // free((void*)the_event.arg4);

                    ioclass.Do_more(c);

                    ioclass.Do_send_clear(c);
                    return;
                }
            }

            string_buffer = CFUNCTIONS.sprintfSinglestring("%s's head page brings you %.0f gold pieces!\n",
                the_event.arg4, the_event.arg1 * phantdefs.N_GEMVALUE);

            ioclass.Do_send_line(c, string_buffer);

            string_buffer = CFUNCTIONS.sprintfSinglestring("%s bestowed %lf gems to %s.\n",
                                   the_event.arg4,
                                   the_event.arg1,
                           c.player.lcname);
            fileclass.Do_log(pathnames.PURCHASE_LOG, string_buffer);

            the_event.arg4 = null; // free((void*)the_event.arg4);

            ioclass.Do_more(c);
            ioclass.Do_send_clear(c);

            if (the_event.arg1 > 0)
            {
                c.player.gold += the_event.arg1 * phantdefs.N_GEMVALUE;
                statsclass.Do_gold(c, 0, 1);
                miscclass.Do_check_weight(c);
            }


            /* bestowing may attract attention from monsters */
            mainclass.Do_random_events(c);
            mainclass.Do_random_events(c);


            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_summon(struct client_t *c, struct event_t *the_event)
        /
        / FUNCTION: do summoning of monsters
        /
        / AUTHOR: E. A. Estes, 12/4/85
        /	  Brian Kelly, 5/16/99
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

        internal void Do_summon(client_t c, event_t the_event)
        {
            event_t event_ptr;
            string string_buffer = ""; //[phantdefs.SZ_LINE];

            if (c.player.cloaked)
            {
                string_buffer = CFUNCTIONS.sprintfSinglestring("%s hurls %s at you, but it goes whizzing by your cloaked head!\n",
                    the_event.arg4,
                    c.realm.monster[(long)the_event.arg1].name);
            }
            else if (c.player.special_type > phantdefs.SC_KING)
            {
                string_buffer = CFUNCTIONS.sprintfSinglestring("%s hurls %s at you, but you contemptuously flick it aside!\n",
                    the_event.arg4,
                    c.realm.monster[(long)the_event.arg1].name);
            }
            else
            {
                string_buffer = CFUNCTIONS.sprintfSinglestring("%s hurls %s at you!\n",
                the_event.arg4,
                    c.realm.monster[(long)the_event.arg1].name);
            }

            ioclass.Do_send_line(c, string_buffer);
            the_event.arg4 = null; // the_event.arg4 = null; //free((void*) the_event.arg4);

            ioclass.Do_more(c);
            ioclass.Do_send_clear(c);

            if (c.player.cloaked || c.player.special_type > phantdefs.SC_KING)
            {
                return;
            }

            event_ptr = eventclass.Do_create_event();
            event_ptr.type = (short)phantdefs.MONSTER_EVENT;
            event_ptr.arg1 = phantdefs.MONSTER_SUMMONED;
            event_ptr.arg3 = (long)the_event.arg1;

            eventclass.Do_handle_event(c, event_ptr);

            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_slap(struct client_t *c, struct event_t *the_event)
        /
        / FUNCTION: slapping
        /
        / AUTHOR: E. A. Estes, 12/4/85
        /	  Brian Kelly, 5/22/01
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

        internal void Do_slap(client_t c, event_t the_event)
        {
            string string_buffer = ""; //[phantdefs.SZ_LINE];
            event_t event_ptr = new event_t();
            short newChannel;
            double dtemp;

            if (c.wizard >= 2 || c.player.special_type >= phantdefs.SC_COUNCIL ||
                c.channel == 8)
            {
                string_buffer = CFUNCTIONS.sprintfSinglestring("%s tried to slap you, but failed!\n", the_event.arg4);

                ioclass.Do_send_line(c, string_buffer);

                the_event.arg4 = null; //free((void*) the_event.arg4);

                ioclass.Do_more(c);
                ioclass.Do_send_clear(c);

                return;
            }

            newChannel = (short)c.channel;

            /* find a new channel to send the player */
            while (newChannel == c.channel)
            {
                newChannel = (short)Mathf.Floor((float)(macros.RND() * 6 + 1));
            }

            /* knock the player into another channel */
            c.channel = newChannel;
            miscclass.Do_lock_mutex(c.realm.realm_lock);
            characterclass.Do_player_description(c);
            miscclass.Do_unlock_mutex(c.realm.realm_lock);
            characterclass.Do_send_specification(c, phantdefs.CHANGE_PLAYER_EVENT);

            /* move the player */
            event_ptr = eventclass.Do_create_event();
            event_ptr.type = (short)phantdefs.MOVE_EVENT;
            event_ptr.arg3 = phantdefs.A_NEAR;
            eventclass.Do_handle_event(c, event_ptr);

            CFUNCTIONS.sprintf(ref string_buffer,
                "You are firmly slapped by %s and are left disoriented!\n",
                (string)the_event.arg4);

            ioclass.Do_send_line(c, string_buffer);

            string_buffer = CFUNCTIONS.sprintfSinglestring("%s reels from a firm, harsh slap from %s!\n",
                c.player.name, the_event.arg4);

            ioclass.Do_broadcast(c, string_buffer);

            the_event.arg4 = null; //free((void*) the_event.arg4);
            ioclass.Do_more(c);
            ioclass.Do_send_clear(c);

            /* injure the player slightly */
            dtemp = 9.0 * c.player.energy / 10.0;
            if (dtemp < 1.0)
            {
                dtemp = 1.0;
            }

            statsclass.Do_energy(c, dtemp, c.player.max_energy, c.player.shield, 0, 0);

            /* 10% of the time the player is knocked out of the game */
            if (macros.RND() < .1)
            {
                event_ptr = eventclass.Do_create_event();
                event_ptr.type = (short)phantdefs.SAVE_EVENT;
                eventclass.Do_handle_event(c, event_ptr);
            }

            return;
        }

        /************************************************************************
        /
        / FUNCTION NAME: commandsclass.Do_reprimand(struct client_t *c, struct event_t *the_event)
        /
        / FUNCTION: apprentice reprimand
        /
        / AUTHOR: Arella Kirstar, 6/2/02
              Renee Gehlbach, 7/31/02
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
        / DESCRIPTION: Throws a player to another channel, setting off a server announcement.
        /
        *************************************************************************/

        internal void Do_reprimand(client_t c, event_t the_event)
        {
            string string_buffer = ""; //[phantdefs.SZ_LINE], string_buffer2 = ""; //[phantdefs.SZ_LINE];
            short newChannel = (short)c.channel;

            /* find another random channel */
            while (newChannel == c.channel)
            {
                newChannel = (short)CFUNCTIONS.floor(macros.RND() * 6 + 1);
            }

            /* knock the player into the above channel */
            c.channel = newChannel;

            miscclass.Do_lock_mutex(c.realm.realm_lock);

            characterclass.Do_player_description(c);

            miscclass.Do_unlock_mutex(c.realm.realm_lock);

            characterclass.Do_send_specification(c, phantdefs.CHANGE_PLAYER_EVENT);

            /* announce the reprimand to the player and the chat */
            string_buffer = CFUNCTIONS.sprintfSinglestring("You are suddenly knocked back by the force of a reprimand from %s!  After stumbling about for a few moments you realize it would probably be wise not to continue in the way that you have been behaving....\n", the_event.arg4);

            ioclass.Do_send_line(c, string_buffer);


            string_buffer = CFUNCTIONS.sprintfSinglestring("%s staggers under the brutal force of a reprimand from %s!\n", c.player.name, the_event.arg4);

            ioclass.Do_broadcast(c, string_buffer);


            ioclass.Do_more(c);

            ioclass.Do_send_clear(c);

            /* free up the memory from the event...I think this needs to be done here...I don't see it being done anywhere else */
            the_event.arg4 = null; //free((void*) the_event.arg4);
                                   /* free((void *)the_event); */

            return;
        }

        /************************************************************************
        /
        / FUNCTION NAME: Do_strong_nf(struct client_t *c, struct event_t *the_event)
        /
        / FUNCTION: strength spell
        /
        / AUTHOR: E. A. Estes, 12/4/85
        /	  Brian Kelly, 5/22/01
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

        internal void Do_strong_nf(client_t c, event_t the_event)
        {
            string string_buffer = ""; //[phantdefs.SZ_LINE];

            if (c.player.strong_nf == 0)
            {

                string_buffer = CFUNCTIONS.sprintfSinglestring("%s has transferred some of his might to you!\n",
                    the_event.arg4);

                ioclass.Do_send_line(c, string_buffer);

                ioclass.Do_more(c);
                ioclass.Do_send_clear(c);

                c.player.strong_nf = 1;
            }


            the_event.arg4 = null; //free((void*) the_event.arg4);
            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_bless(struct client_t *c, struct event_t *the_event)
        /
        / FUNCTION: blessing
        /
        / AUTHOR: E. A. Estes, 12/4/85
        /	  Brian Kelly, 9/6/99
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

        internal void Do_bless(client_t c, event_t the_event)
        {
            string string_buffer = ""; //[phantdefs.SZ_LINE];

            string_buffer = CFUNCTIONS.sprintfSinglestring("You are blessed by %s!\n",
                the_event.arg4);

            ioclass.Do_send_line(c, string_buffer);

            ioclass.Do_more(c);
            ioclass.Do_send_clear(c);

            treasureclass.Do_award_blessing(c);

            string_buffer = CFUNCTIONS.sprintfSinglestring("%s has received the blessing of %s!\n",
                c.player.name, the_event.arg4);
            ioclass.Do_broadcast(c, string_buffer);

            the_event.arg4 = null; //free((void*) the_event.arg4);
            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_heal(struct client_t *c, struct event_t *the_event)
        /
        / FUNCTION: healing and curing
        /
        / AUTHOR: E. A. Estes, 12/4/85
        /	  Brian Kelly, 9/6/99
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

        internal void Do_heal(client_t c, event_t the_event)
        {
            event_t event_ptr = new event_t();
            string string_buffer = ""; //[phantdefs.SZ_LINE];
            float ftemp;

            if (the_event.arg1 == phantdefs.P_RESTORE)
            {

                statsclass.Do_energy(c, c.player.max_energy + c.player.shield + c.knightEnergy,
                c.player.max_energy, c.player.shield, 0, 0);

                if (c.player.poison > 0)
                {
                    statsclass.Do_poison(c, (double)-c.player.poison);
                }

                /* remove all combat fatigue */
                c.battle.rounds = 0;


                statsclass.Do_speed(c, c.player.max_quickness, c.player.quicksilver,
                    c.battle.speedSpell, 0);
            }
            else if (the_event.arg1 == phantdefs.P_CURE)
            {

                event_ptr = eventclass.Do_create_event();
                event_ptr.type = (short)phantdefs.PLAGUE_EVENT;

                /* curing adds one hundred poison, make sure to fix it so people will
                   stick around to wait for it */
                event_ptr.arg3 = (long)(CFUNCTIONS.floor((c.player.poison * 1000) / 2) - 100000.0);

                /* don't cure if the king ran away */
                if (eventclass.Do_send_character_event(c, event_ptr, (string)the_event.arg4) == 0)
                {
                    the_event.arg4 = null; //free((void*) the_event.arg4);
                    event_ptr = null;// event_ptr = null; // free((void*) event_ptr);
                    return;
                }


                statsclass.Do_energy(c, c.player.energy + (c.player.max_energy +

                    c.player.shield - c.player.energy) / 3,
                    c.player.max_energy, c.player.shield, 0, 0);

                if (c.player.poison > 0)
                {

                    statsclass.Do_poison(c, (double)-(c.player.poison / 2));
                }

                /* remove some combat fatigue */
                c.battle.rounds -= 90;
                if (c.battle.rounds < 0)
                {
                    c.battle.rounds = 0;
                }


                statsclass.Do_speed(c, c.player.max_quickness, c.player.quicksilver,
                    c.battle.speedSpell, 0);

            }
            else
            {


                statsclass.Do_energy(c, c.player.energy + (c.player.max_energy +

                    c.player.shield - c.player.energy) / 10,
                    c.player.max_energy, c.player.shield, 0, 0);

                /* remove some combat fatigue */
                c.battle.rounds -= 30;
                if (c.battle.rounds < 0)
                {
                    c.battle.rounds = 0;
                }


                statsclass.Do_speed(c, c.player.max_quickness, c.player.quicksilver,
                    c.battle.speedSpell, 0);

            }

            if (the_event.arg1 == phantdefs.P_RESTORE)
            {

                string_buffer = CFUNCTIONS.sprintfSinglestring("%s restores you to full health!\n",
                    the_event.arg4);

                ioclass.Do_send_line(c, string_buffer);

                if (c.player.blind)
                {
                    ioclass.Do_send_line(c, "The veil of darkness lifts!\n");
                    c.player.blind = false;
                }
            }
            else if (the_event.arg1 == phantdefs.P_CURE)
            {

                string_buffer = CFUNCTIONS.sprintfSinglestring("You have been cured by %s!\n",
                    the_event.arg4);

                ioclass.Do_send_line(c, string_buffer);
            }
            else
            {

                string_buffer = CFUNCTIONS.sprintfSinglestring("Your wounds have been healed by %s!\n",
                    the_event.arg4);

                ioclass.Do_send_line(c, string_buffer);
            }

            ioclass.Do_more(c);
            ioclass.Do_send_clear(c);

            the_event.arg4 = null; //free((void*) the_event.arg4);

            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_examine(struct client_t *c, struct event_t *the_event)
        /
        / FUNCTION: examining a player
        /
        / AUTHOR: E. A. Estes, 12/4/85
        /	  Brian Kelly, 5/18/99
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

        internal void Do_examine(client_t c, event_t the_event)
        {
            game_t game_ptr = new game_t();
            event_t event_ptr = new event_t();
            string playerName = ""; //[phantdefs.SZ_LINE];

            /* prepare an event */
            event_ptr = eventclass.Do_create_event();
            event_ptr.type = (short)phantdefs.REQUEST_RECORD_EVENT;
            event_ptr.from = c.game;
            event_ptr.arg3 = c.wizard;

            /* get the name of the player to look at */
            if (the_event.arg3 != 0)
            {

                if (ioclass.Do_player_dialog(c, "Which character do you want to look at?\n",
                playerName) != phantdefs.S_NORM)
                {

                    return;
                }

                if (eventclass.Do_send_character_event(c, event_ptr, playerName) == 0)
                {

                    event_ptr = null; // free((void*)event_ptr);
                    ioclass.Do_send_line(c, "That character just left the game.\n");

                    ioclass.Do_more(c);
                    return;
                }

                return;
            }

            event_ptr.to = c.game;
            eventclass.Do_file_event(c, event_ptr);

            return;
        }



        /************************************************************************
        /
        / FUNCTION NAME: Do_enact(struct client_t *c)
        /
        / FUNCTION: steward commands
        /
        / AUTHOR: E. A. Estes, 2/28/86
        /	  Brian Kelly, 5/11/01
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

        internal void Do_enact(client_t c)
        {
            event_t event_ptr = new event_t();
            realm_object_t object_ptr = new realm_object_t();
            button_t buttons = new button_t();
            string string_buffer = ""; //[phantdefs.SZ_LINE], 
            string string_buffer2 = ""; //[phantdefs.SZ_LINE];
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            long answer = -1;
            double dtemp;
            float ftemp;

            /* determine what the player wants to do */
            CFUNCTIONS.strcpy(ref buttons.button[0], "Transport\n");
            CFUNCTIONS.strcpy(ref buttons.button[1], "Curse\n");
            CFUNCTIONS.strcpy(ref buttons.button[2], "Energy Void\n");
            CFUNCTIONS.strcpy(ref buttons.button[3], "Bestow\n");
            CFUNCTIONS.strcpy(ref buttons.button[4], "Collect Taxes\n");
            CFUNCTIONS.strcpy(ref buttons.button[5], "Throw Smurf\n");
            ioclass.Do_clear_buttons(buttons, 6);
            CFUNCTIONS.strcpy(ref buttons.button[7], "Cancel\n");

            if (c.player.gender == (phantdefs.MALE != 0 ? true : false))
                ioclass.Do_send_line(c, "What would you like done, sir?\n");
            else
                ioclass.Do_send_line(c, "What would you like done, ma'am?\n");

            if (ioclass.Do_buttons(c, ref answer, buttons) != phantdefs.S_NORM || answer == 7)
            {

                ioclass.Do_send_clear(c);
                return;
            }

            ioclass.Do_send_clear(c);
            switch (answer)
            {

                case 0:   /* transport someone */
                    event_ptr = eventclass.Do_create_event();
                    event_ptr.type = (short)phantdefs.TRANSPORT_EVENT;
                    event_ptr.arg3 = 0;
                    CFUNCTIONS.strcpy(ref string_buffer2, "transport");
                    break;

                case 1:   /* curse another */
                    event_ptr = eventclass.Do_create_event();
                    event_ptr.type = (short)phantdefs.CURSE_EVENT;
                    event_ptr.arg1 = phantdefs.P_CURSE;
                    CFUNCTIONS.strcpy(ref string_buffer2, "curse");
                    break;

                case 2:   /* create energy void */

                    /* create a realm object */
                    object_ptr = new realm_object_t();// (struct realm_object_t *) Do_malloc(phantdefs.SZ_REALM_OBJECT);
                    object_ptr.type = (short)phantdefs.ENERGY_VOID;

                    if (ioclass.Do_coords_dialog(c, ref object_ptr.x, ref object_ptr.y,
                        "What should the coordinates of the void be?\n") != 0)
                    {


                        object_ptr = null;//free((void*) object_ptr);
                        return;
                    }

                    /* put the object in place */
                    miscclass.Do_lock_mutex(c.realm.object_lock);
                    object_ptr.next_object = c.realm.objects;
                    c.realm.objects = object_ptr;

                    miscclass.Do_unlock_mutex(c.realm.object_lock);

                    if (c.player.gender == (phantdefs.MALE != 0 ? true : false))
                        ioclass.Do_send_line(c, "It shall be done as you have ordered, sir.\n");
                    else
                        ioclass.Do_send_line(c, "It shall be done as you have ordered, ma'am.\n");


                    ioclass.Do_more(c);

                    ioclass.Do_send_clear(c);

                    if (c.wizard < 3)

                        statsclass.Do_sin(c, .5);

                    return;

                case 3:   /* bestow gems to subject */
                    event_ptr = eventclass.Do_create_event();
                    event_ptr.type = (short)phantdefs.BESTOW_EVENT;
                    CFUNCTIONS.strcpy(ref string_buffer2, "give gems to");

                    if (ioclass.Do_double_dialog(c, ref event_ptr.arg1, "How many gems to bestow(0-5) ?\n") != 0)
                    {


                        event_ptr = null; // event_ptr = null; // free((void*) event_ptr);
                        return;
                    }

                    event_ptr.arg1 = CFUNCTIONS.floor(event_ptr.arg1);

                    if (c.wizard < 3)
                    {
                        if (event_ptr.arg1 > c.player.gems)
                        {
                            ioclass.Do_send_line(c, "You don't have that many *gems*!\n");

                            event_ptr = null; // free((void*) event_ptr);

                            ioclass.Do_more(c);

                            ioclass.Do_send_clear(c);
                            return;
                        }

                        if (event_ptr.arg1 <= 0)
                        {

                            event_ptr = null; // free((void*) event_ptr);

                            ioclass.Do_send_clear(c);
                            return;
                        }

                        /* cap the maximum bestow */
                        if (event_ptr.arg1 > 5)
                        {
                            ioclass.Do_send_line(c,
                        "Your head page ran off with the gems!\n");

                            statsclass.Do_gems(c, -event_ptr.arg1, 0);


                            event_ptr = null; // free((void*) event_ptr);

                            ioclass.Do_more(c);

                            ioclass.Do_send_clear(c);
                            return;
                        }


                        /* adjust gold after we are sure it will be given to someone */
                    }
                    break;

                case 4:   /* collect accumulated taxes */

                    /* lock up the stewards gold */
                    miscclass.Do_lock_mutex(c.realm.kings_gold_lock);

                    dtemp = c.realm.steward_gold;
                    c.realm.steward_gold = 0;

                    /* unlock the gold */
                    miscclass.Do_unlock_mutex(c.realm.kings_gold_lock);

                    string_buffer = CFUNCTIONS.sprintfSinglestring("You have collected %.0lf in gold.\n", dtemp);


                    ioclass.Do_send_line(c, string_buffer);

                    ioclass.Do_more(c);

                    ioclass.Do_send_clear(c);

                    if (dtemp > 0)
                    {
                        c.player.gold += dtemp;
                        statsclass.Do_gold(c, 0, 1);
                        miscclass.Do_check_weight(c);
                    }

                    if (c.wizard < 3)

                        statsclass.Do_sin(c, .25);

                    return;

                case 5:
                    event_ptr = eventclass.Do_create_event();
                    event_ptr.type = (short)phantdefs.SUMMON_EVENT;
                    event_ptr.arg1 = 40;
                    CFUNCTIONS.strcpy(ref string_buffer2, "smurf");

                    break;


                default:
                    error_msg = CFUNCTIONS.sprintfSinglestring("[%s] Returned non-option in Do_enact.\n",
                            c.connection_id);

                    fileclass.Do_log_error(error_msg);

                    event_ptr = null; // free((void*) event_ptr);

                    hackclass.Do_caught_hack(c, phantdefs.H_SYSTEM);
                    return;
            }

            /* if we're here, we have an event to send to another player */
            /* prompt for player to affect */
            string_buffer = CFUNCTIONS.sprintfSinglestring("Who do you want to %s?\n", string_buffer2);
            if (ioclass.Do_player_dialog(c, string_buffer, string_buffer2) != phantdefs.S_NORM)
            {
                event_ptr = null; // free((void*) event_ptr);
                return;
            }

            /* if the player named himself */
            if (!CFUNCTIONS.strcmp(string_buffer2, c.modifiedName) && c.wizard < 3)
            {

                event_ptr = null; // free((void*) event_ptr);
                ioclass.Do_send_line(c, "You may not do it to yourself!\n");

                ioclass.Do_more(c);

                ioclass.Do_send_clear(c);
                return;
            }

            event_ptr.arg3 = CFUNCTIONS.strlen(c.modifiedName) + 1;
            event_ptr.arg4 = "";// (void*) Do_malloc(event_ptr.arg3);
            string arg4 = (string)event_ptr.arg4;
            CFUNCTIONS.strcpy(ref arg4, c.modifiedName);
            event_ptr.arg4 = arg4;
            event_ptr.from = c.game;

            if (eventclass.Do_send_character_event(c, event_ptr, string_buffer2) == 0)
            {
                event_ptr.arg4 = null;// event_ptr.arg4 = null; //free((void*) event_ptr.arg4);
                event_ptr = null; // free((void*) event_ptr);
                ioclass.Do_send_line(c, "That character just left the game.\n");
                ioclass.Do_more(c);

                ioclass.Do_send_clear(c);
                return;
            }

            if (c.wizard < 3)
            {

                /* Get rid of gold, if we're giving it away */
                if (event_ptr.type == phantdefs.BESTOW_EVENT)
                {

                    statsclass.Do_gems(c, -event_ptr.arg1, 0);

                    /* check for impeachment */
                    if (macros.RND() * 40 < (event_ptr.arg1 + 1) * event_ptr.arg1)
                    {
                        ioclass.Do_send_line(c, "The Tax Collectors discovered your action and impeached you for your excessive generosity!\n");
                        Do_dethrone(c);
                        ioclass.Do_more(c);

                        ioclass.Do_send_clear(c);
                        return;
                    }

                }
                /* add some sin for the more evil powers */
                if (event_ptr.type == phantdefs.TRANSPORT_EVENT)
                {

                    statsclass.Do_sin(c, 1.0);
                }
                else if (event_ptr.type == phantdefs.CURSE_EVENT)
                {

                    statsclass.Do_sin(c, .5);
                }
                else if (event_ptr.type == phantdefs.MONSTER_EVENT)
                {

                    statsclass.Do_sin(c, .1);
                }
            }

            if (c.player.gender == (phantdefs.MALE != 0 ? true : false))
                ioclass.Do_send_line(c, "It has been done as you have ordered, sir.\n");
            else
                ioclass.Do_send_line(c, "It has been done as you have ordered, ma'am.\n");

            ioclass.Do_more(c);
            ioclass.Do_send_clear(c);

            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_decree(struct client_t *c)
        /
        / FUNCTION: king commands
        /
        / AUTHOR: E. A. Estes, 2/28/86
        /	  Brian Kelly, 5/16/99
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

        internal void Do_decree(client_t c)
        {
            event_t event_ptr = new event_t();
            button_t buttons = new button_t();
            string string_buffer = ""; //[phantdefs.SZ_LINE], 
            string string_buffer2 = ""; //[phantdefs.SZ_LINE];
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            long answer = -1;
            double dtemp;
            float ftemp;

            /* determine what the player wants to do */
            CFUNCTIONS.strcpy(ref buttons.button[0], "Oust\n");
            CFUNCTIONS.strcpy(ref buttons.button[1], "Execrate\n");
            CFUNCTIONS.strcpy(ref buttons.button[2], "Cure\n");


            CFUNCTIONS.strcpy(ref buttons.button[3], "Collect Taxes\n");
            CFUNCTIONS.strcpy(ref buttons.button[4], "Knight\n");
            CFUNCTIONS.strcpy(ref buttons.button[5], "Tax\n");
            ioclass.Do_clear_buttons(buttons, 6);
            CFUNCTIONS.strcpy(ref buttons.button[7], "Cancel\n");

            ioclass.Do_send_line(c, "What would you like done, your Majesty?\n");
            if (ioclass.Do_buttons(c, ref answer, buttons) != phantdefs.S_NORM || answer == 7)
            {

                ioclass.Do_send_clear(c);
                return;
            }

            ioclass.Do_send_clear(c);
            switch (answer)
            {

                case 0:   /* oust someone */
                    event_ptr = eventclass.Do_create_event();
                    event_ptr.type = (short)phantdefs.TRANSPORT_EVENT;
                    event_ptr.arg1 = 1;
                    CFUNCTIONS.strcpy(ref string_buffer2, "oust");
                    break;

                case 1:   /* execrate another */
                    event_ptr = eventclass.Do_create_event();
                    event_ptr.type = (short)phantdefs.CURSE_EVENT;
                    event_ptr.arg1 = phantdefs.P_EXECRATE;
                    CFUNCTIONS.strcpy(ref string_buffer2, "execrate");
                    break;

                case 2:   /* cure the character */

                    /* create a cure event */
                    event_ptr = eventclass.Do_create_event();
                    event_ptr.type = (short)phantdefs.HEAL_EVENT;
                    event_ptr.arg1 = phantdefs.P_CURE;
                    CFUNCTIONS.strcpy(ref string_buffer2, "cure");

                    /* add a lot of poison that will be cured when the poison from
                       curing is received */
                    if (c.wizard < 3)
                    {
                        statsclass.Do_poison(c, 100.0);
                    }
                    break;


                case 3:   /* collect accumulated taxes */

                    /* lock up the kings gold */
                    miscclass.Do_lock_mutex(c.realm.kings_gold_lock);

                    dtemp = c.realm.kings_gold;
                    c.realm.kings_gold = 0;
                    dtemp += c.realm.steward_gold;
                    c.realm.steward_gold = 0;

                    /* lock up the kings gold */
                    miscclass.Do_unlock_mutex(c.realm.kings_gold_lock);

                    string_buffer = CFUNCTIONS.sprintfSinglestring("You have collected %.0lf in gold.\n", dtemp);


                    ioclass.Do_send_line(c, string_buffer);

                    ioclass.Do_more(c);

                    ioclass.Do_send_clear(c);

                    if (dtemp > 0)
                    {
                        c.player.gold += dtemp;
                        statsclass.Do_gold(c, 0, 1);
                        miscclass.Do_check_weight(c);
                    }

                    if (c.wizard < 3)

                        statsclass.Do_sin(c, .25);

                    return;

                case 4:   /* knight the character */

                    /* create a knight event */
                    event_ptr = eventclass.Do_create_event();
                    event_ptr.type = (short)phantdefs.KNIGHT_EVENT;
                    CFUNCTIONS.strcpy(ref string_buffer2, "knight");
                    break;

                case 5:   /* send tax collector */

                    /* create a tax collector event */
                    event_ptr = eventclass.Do_create_event();
                    event_ptr.type = (short)phantdefs.TAX_EVENT;
                    CFUNCTIONS.strcpy(ref string_buffer2, "tax");
                    break;

                default:
                    error_msg = CFUNCTIONS.sprintfSinglestring("[%s] Returned non-option in Do_decree.\n",
                            c.connection_id);

                    fileclass.Do_log_error(error_msg);

                    event_ptr = null; // free((void*) event_ptr);

                    hackclass.Do_caught_hack(c, phantdefs.H_SYSTEM);
                    return;
            }

            /* if we're here, we have an event to send to another player */
            /* prompt for player to affect */
            string_buffer = CFUNCTIONS.sprintfSinglestring("Who do you want to %s?\n", string_buffer2);
            if (ioclass.Do_player_dialog(c, string_buffer, string_buffer2) != phantdefs.S_NORM)
            {
                event_ptr = null; // free((void*) event_ptr);
                return;
            }

            /* if the non-wizard player named himself */
            if (!CFUNCTIONS.strcmp(string_buffer2, c.modifiedName) && c.wizard < 3)
            {

                event_ptr = null; // free((void*) event_ptr);
                ioclass.Do_send_line(c, "You may not do it to yourself!\n");

                ioclass.Do_more(c);

                ioclass.Do_send_clear(c);
                return;
            }

            event_ptr.arg3 = CFUNCTIONS.strlen(c.modifiedName) + 1;
            event_ptr.arg4 = "";// (void*) Do_malloc(event_ptr.arg3);
            string arg4 = (string)event_ptr.arg4;
            CFUNCTIONS.strcpy(ref arg4, c.modifiedName);
            event_ptr.arg4 = arg4;
            event_ptr.from = c.game;

            if (eventclass.Do_send_character_event(c, event_ptr, string_buffer2) == 0)
            {
                event_ptr.arg4 = null;// event_ptr.arg4 = null; //free((void*) event_ptr.arg4);
                event_ptr = null; // free((void*) event_ptr);
                ioclass.Do_send_line(c, "That character just left the game.\n");
                ioclass.Do_more(c);

                ioclass.Do_send_clear(c);
                return;
            }

            if (c.wizard < 3)
            {

                /* Get rid of gold, if we're giving it away */
                if (event_ptr.type == phantdefs.BESTOW_EVENT)

                    statsclass.Do_gold(c, -event_ptr.arg1, 0);

                /* Add sin for some of the nastier powers */
                if (event_ptr.type == phantdefs.TRANSPORT_EVENT)
                {

                    statsclass.Do_sin(c, 1.5);
                }
                else if (event_ptr.type == phantdefs.CURSE_EVENT)
                {

                    statsclass.Do_sin(c, .75);
                }
                else if (event_ptr.type == phantdefs.TAX_EVENT)
                {

                    statsclass.Do_sin(c, .5);
                }
            }

            ioclass.Do_send_line(c, "It shall be done as you have ordered, your Majesty.\n");
            ioclass.Do_more(c);
            ioclass.Do_send_clear(c);
            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_intervene(struct client_t *c)
        /
        / FUNCTION: council commands
        /
        / AUTHOR: E. A. Estes, 2/28/86
        /	  Brian Kelly, 5/16/99
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

        internal void Do_intervene(client_t c)
        {
            event_t event_ptr = new event_t();
            realm_object_t object_ptr = new realm_object_t();
            realm_object_t theObject = new realm_object_t();
            button_t theButtons = new button_t();
            string string_buffer = ""; //[phantdefs.SZ_LINE], 
            string string_buffer2 = ""; //[phantdefs.SZ_LINE];
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            long answer = -1;              /* pointer to option description */
            double distance = -1;
            double theDistance = -1;
            float ftemp;
            double manaCost;

            /* determine what the player wants to do */

            /* one can only seek the grail with a palantir */
            CFUNCTIONS.strcpy(ref theButtons.button[0], "Slap\n");
            CFUNCTIONS.strcpy(ref theButtons.button[1], "Blind\n");
            CFUNCTIONS.strcpy(ref theButtons.button[2], "Heal\n");
            ioclass.Do_clear_buttons(theButtons, 3);

            if (c.player.palantir || (c.wizard > 2))
            {
                CFUNCTIONS.strcpy(ref theButtons.button[3], "Seek Grail\n");
                CFUNCTIONS.strcpy(ref theButtons.button[4], "Seek Corpse\n");
            }

            CFUNCTIONS.strcpy(ref theButtons.button[7], "Cancel\n");

            if (c.player.gender == (phantdefs.MALE != 0 ? true : false))
                ioclass.Do_send_line(c, "What would you like done, m'lord?\n");
            else
                ioclass.Do_send_line(c, "What would you like done, m'lady?\n");

            if (ioclass.Do_buttons(c, ref answer, theButtons) != phantdefs.S_NORM || answer == 7)
            {

                ioclass.Do_send_clear(c);
                return;
            }

            ioclass.Do_send_clear(c);

            manaCost = CFUNCTIONS.floor(c.player.level * .000005 * (1000 + c.player.level *
                        c.realm.charstats[c.player.type].max_mana));

            if (c.player.mana < manaCost && (c.wizard < 3))
            {

                ioclass.Do_send_line(c, "Not enough mana left.\n");

                ioclass.Do_more(c);
                ioclass.Do_send_clear(c);
                return;
            }

            switch (answer)
            {

                case 0:   /* slap */

                    event_ptr = eventclass.Do_create_event();
                    event_ptr.type = (short)phantdefs.SLAP_EVENT;
                    CFUNCTIONS.strcpy(ref string_buffer2, "slap");
                    break;

                case 1:   /* blind */

                    /* create a monster event */
                    event_ptr = eventclass.Do_create_event();
                    event_ptr.type = (short)phantdefs.BLIND_EVENT;
                    event_ptr.arg1 = 1;
                    CFUNCTIONS.strcpy(ref string_buffer2, "blind");
                    break;

                case 2:   /* heal */

                    /* create a heal event */
                    event_ptr = eventclass.Do_create_event();
                    event_ptr.type = (short)phantdefs.HEAL_EVENT;
                    event_ptr.arg1 = phantdefs.P_HEAL;
                    CFUNCTIONS.strcpy(ref string_buffer2, "heal");
                    break;

                case 3:   /* seek grail */

                    /* find the grail */
                    miscclass.Do_lock_mutex(c.realm.realm_lock);
                    object_ptr = c.realm.objects;

                    /* run through the realm objects */
                    while (object_ptr != null)
                    {

                        if (object_ptr.type == phantdefs.HOLY_GRAIL)
                        {
                            break;
                        }

                        object_ptr = object_ptr.next_object;
                    }

                    if (object_ptr == null)
                    {


                        miscclass.Do_unlock_mutex(c.realm.realm_lock);


                        error_msg = CFUNCTIONS.sprintfSinglestring(
                            "[%s] No grail found in realm objects in Do_intervene.\n",
                            c.connection_id);


                        fileclass.Do_log_error(error_msg);


                        ioclass.Do_send_line(c,
                       "The palantir fills with black clouds.  It is of no use to you now.\n");


                        ioclass.Do_more(c);

                        ioclass.Do_send_clear(c);
                        return;
                    }

                    /* determine the current distance to the grail */
                    miscclass.Do_distance(c.player.x, object_ptr.x, c.player.y, object_ptr.y, ref distance);


                    miscclass.Do_unlock_mutex(c.realm.realm_lock);

                    /* throw in a fudge factor of up to 25% */
                    if (c.player.special_type != phantdefs.SC_EXVALAR)
                    {
                        distance = CFUNCTIONS.floor(distance * (.75 + macros.RND() / 2) + .25);
                        /* throw in a fudge factor of up to 100% for exvalars */
                    }
                    else
                    {

                        /* also summon Morgoth in randomly when player gets close */
                        if ((distance < c.player.level / 1000) &&
                            (macros.RND() < (c.player.level * (10 - distance) / 100000))
                           )
                        {

                            event_ptr = eventclass.Do_create_event();
                            event_ptr.type = (short)phantdefs.MONSTER_EVENT;
                            event_ptr.arg1 = phantdefs.MONSTER_SUMMONED;
                            event_ptr.arg3 = 15;
                            eventclass.Do_file_event(c, event_ptr);
                        }

                        distance = CFUNCTIONS.floor(distance * (.5 + macros.RND()) + .25);
                    }

                    /*
                            if ((cfunctions.strcmp(c.player.parent_account, c.lcaccount) &&
                                macros.RND() < .5)) {
                                distance = distance * 3 * macros.RND();
                            }
                    */

                    if (distance == 0)
                    {
                        distance = 1;
                    }



                    CFUNCTIONS.sprintf(ref string_buffer,
                        "The palantir says the grail is %0.lf distance from here.\n",
                        distance);


                    ioclass.Do_send_line(c, string_buffer);

                    ioclass.Do_more(c);


                    if (c.wizard < 3)
                        statsclass.Do_mana(c, -manaCost, 0);

                    return;

                case 4:   /* find the nearest corpse */

                    /* find the grail */
                    miscclass.Do_lock_mutex(c.realm.realm_lock);
                    object_ptr = c.realm.objects;
                    theObject = null;
                    theDistance = 1000;

                    /* run through the realm objects */
                    while (object_ptr != null)
                    {

                        if (object_ptr.type == phantdefs.CORPSE)
                        {


                            miscclass.Do_distance(c.player.x, object_ptr.x, c.player.y,
                                object_ptr.y, ref distance);

                            if (distance < theDistance)
                            {
                                theDistance = distance;
                                theObject = object_ptr;
                            }
                        }

                        object_ptr = object_ptr.next_object;
                    }

                    if (theObject == null)
                    {


                        miscclass.Do_unlock_mutex(c.realm.realm_lock);


                        ioclass.Do_send_line(c,
                       "The palantir fills with black clouds.  It is of no use to you now.\n");


                        ioclass.Do_more(c);

                        ioclass.Do_send_clear(c);
                        return;
                    }

                    /* determine the direction to the corpse */
                    error_msg = "\0";

                    miscclass.Do_direction(c, theObject.x, theObject.y, ref error_msg);

                    miscclass.Do_unlock_mutex(c.realm.realm_lock);


                    CFUNCTIONS.sprintf(ref string_buffer,
                        "The palantir says there's a corpse to the %s.\n", error_msg);


                    ioclass.Do_send_line(c, string_buffer);


                    ioclass.Do_more(c);

                    ioclass.Do_send_clear(c);

                    if (c.wizard < 3)
                        statsclass.Do_mana(c, -manaCost, 0);

                    return;

                default:
                    error_msg = CFUNCTIONS.sprintfSinglestring("[%s] Returned non-option in Do_intervene.\n",
                            c.connection_id);


                    fileclass.Do_log_error(error_msg);

                    hackclass.Do_caught_hack(c, phantdefs.H_SYSTEM);
                    return;
            }

            /* if we're here, we have an event to send to another player */
            /* prompt for player to affect */
            string_buffer = CFUNCTIONS.sprintfSinglestring("Who do you want to %s?\n", string_buffer2);
            if (ioclass.Do_player_dialog(c, string_buffer, string_buffer2) != phantdefs.S_NORM)
            {
                event_ptr = null; // free((void*) event_ptr);
                return;
            }

            /* if the player named himself and isn't the wizard */
            if (!CFUNCTIONS.strcmp(string_buffer2, c.modifiedName) && (c.wizard < 3))
            {
                event_ptr = null; // free((void*) event_ptr);
                ioclass.Do_send_line(c, "You may not do it to yourself!\n");
                ioclass.Do_more(c);
                ioclass.Do_send_clear(c);
                return;
            }

            event_ptr.arg3 = CFUNCTIONS.strlen(c.modifiedName) + 1;
            string arg4 = (string)event_ptr.arg4;
            CFUNCTIONS.strcpy(ref arg4, c.modifiedName);
            event_ptr.arg4 = arg4;
            event_ptr.from = c.game;

            if (eventclass.Do_send_character_event(c, event_ptr, string_buffer2) == 0)
            {
                event_ptr.arg4 = null; // event_ptr.arg4 = null; //free((void*) event_ptr.arg4);
                event_ptr = null; // free((void*) event_ptr);
                ioclass.Do_send_line(c, "That character just left the game.\n");
                ioclass.Do_more(c);
                ioclass.Do_send_clear(c);
                return;
            }

            if (c.wizard < 3)
            {

                statsclass.Do_mana(c, -manaCost);

                if (event_ptr.type == phantdefs.SLAP_EVENT)
                {

                    statsclass.Do_sin(c, .25);
                }
                else if (event_ptr.type == phantdefs.BLIND_EVENT)
                {

                    statsclass.Do_sin(c, .5);
                }
            }

            if (c.player.gender == (phantdefs.MALE != 0 ? true : false))
                ioclass.Do_send_line(c, "It shall be done, m'lord.\n");
            else
                ioclass.Do_send_line(c, "It shall be done, m'lady.\n");

            ioclass.Do_more(c);
            ioclass.Do_send_clear(c);
            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_command(struct client_t *c)
        /
        / FUNCTION: valar commands
        /
        / AUTHOR: E. A. Estes, 2/28/86
        /	  Brian Kelly, 11/10/99
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

        internal void Do_command(client_t c)
        {
            event_t event_ptr = new event_t();
            realm_object_t object_ptr = new realm_object_t();
            button_t theButtons = new button_t();
            string string_buffer = ""; //[phantdefs.SZ_LINE], 
            string string_buffer2 = ""; //[phantdefs.SZ_LINE];
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            long answer = -1;              /* pointer to option description */
            float ftemp, manaCost;
            double dtemp;

            /* determine what the player wants to do */
            CFUNCTIONS.strcpy(ref theButtons.button[0], "Smite\n");
            CFUNCTIONS.strcpy(ref theButtons.button[1], "Degenerate\n");
            CFUNCTIONS.strcpy(ref theButtons.button[2], "Relocate\n");
            CFUNCTIONS.strcpy(ref theButtons.button[3], "Restore\n");
            CFUNCTIONS.strcpy(ref theButtons.button[4], "Bless\n");
            CFUNCTIONS.strcpy(ref theButtons.button[5], "Proclaim\n");
            CFUNCTIONS.strcpy(ref theButtons.button[6], "Slap\n");
            CFUNCTIONS.strcpy(ref theButtons.button[7], "Cancel\n");


            ioclass.Do_send_line(c, "What kind of mayhem did you have in mind, eminence?\n");
            if (ioclass.Do_buttons(c, ref answer, theButtons) != phantdefs.S_NORM || answer == 7)
            {

                ioclass.Do_send_clear(c);
                return;
            }

            ioclass.Do_send_clear(c);

            manaCost = CFUNCTIONS.floor(.33 * (1000 + c.player.level *
                        c.realm.charstats[c.player.type].max_mana));


            if (c.player.mana < manaCost && (c.wizard < 3))
            {

                ioclass.Do_send_line(c, "Not enough mana left.\n");

                ioclass.Do_more(c);

                ioclass.Do_send_clear(c);
                return;
            }

            event_ptr = eventclass.Do_create_event();

            switch (answer)
            {

                case 0:	/* smite */
                    event_ptr.type = (short)phantdefs.CURSE_EVENT;
                    event_ptr.arg1 = phantdefs.P_SMITE;
                    CFUNCTIONS.strcpy(ref string_buffer2, "smite");
                    break;

                case 1:	/* degenerate */
                    event_ptr.type = (short)phantdefs.DEGENERATE_EVENT;
                    CFUNCTIONS.strcpy(ref string_buffer2, "degenerate");
                    break;

                case 2:   /* relocate someone */
                    event_ptr.type = (short)phantdefs.RELOCATE_EVENT;
                    CFUNCTIONS.strcpy(ref string_buffer2, "relocate");

                    if (ioclass.Do_coords_dialog(c, ref event_ptr.arg1, ref event_ptr.arg2,
                        "Relocate to where?\n") != 0)
                    {


                        event_ptr = null; // free((void*) event_ptr);
                        return;
                    }

                    if (Mathf.Abs((float)event_ptr.arg1) >= phantdefs.D_BEYOND || Mathf.Abs((float)event_ptr.arg2) >=
                        phantdefs.D_BEYOND)
                    {


                        ioclass.Do_send_line(c,
                           "Relocate will only work inside the point of no return.\n");


                        event_ptr = null; // free((void*) event_ptr);

                        ioclass.Do_more(c);

                        ioclass.Do_send_clear(c);
                        return;
                    }

                    break;

                case 3:   /* restore someone */
                    event_ptr.type = (short)phantdefs.HEAL_EVENT;
                    event_ptr.arg1 = phantdefs.P_RESTORE;
                    CFUNCTIONS.strcpy(ref string_buffer2, "restore");
                    break;

                case 4:   /* bless another */
                    if (c.wizard < 3)
                    {

                        /* with blessings, pull lives */
                        if (c.player.lives != 0)
                        {


                            ioclass.Do_send_line(c,
                           "This bless will cost you a life.  Do you wish to continue?\n");
                        }
                        else
                        {


                            ioclass.Do_send_line(c,
                                "You will lose the position of Valar.  Do you wish to continue?\n");
                        }

                        if (ioclass.Do_yes_no(c, ref answer) != phantdefs.S_NORM || answer == 1)
                        {
                            event_ptr = null; // free((void*) event_ptr);

                            ioclass.Do_send_clear(c);
                            return;
                        }

                        if (c.player.lives != 0)
                        {
                            --c.player.lives;
                        }
                        else
                        {

                            Do_valar(c);
                        }
                    }

                    event_ptr.type = (short)phantdefs.BLESS_EVENT;
                    CFUNCTIONS.strcpy(ref string_buffer2, "bless");
                    break;

                case 5:   /* proclaim */


                    event_ptr = null; // free((void*) event_ptr);

                    if (c.wizard < 3)
                    {

                        statsclass.Do_mana(c, -manaCost, 0);
                    }

                    /* set the broadcast flag */
                    c.broadcast = true;


                    ioclass.Do_send_line(c,
                        "Your next chat message will be proclaimed on all channels.\n");


                    ioclass.Do_more(c);

                    ioclass.Do_send_clear(c);
                    return;

                case 6:
                    event_ptr.type = (short)phantdefs.SLAP_EVENT;
                    CFUNCTIONS.strcpy(ref string_buffer2, "slap");
                    break;


                default:

                    event_ptr = null; // free((void*) event_ptr);
                    error_msg = CFUNCTIONS.sprintfSinglestring("[%s] Returned non-option in Do_command.\n",
                            c.connection_id);


                    fileclass.Do_log_error(error_msg);

                    hackclass.Do_caught_hack(c, phantdefs.H_SYSTEM);
                    return;
            }

            /* if we're here, we have an event to send to another player */
            /* prompt for player to affect */
            string_buffer = CFUNCTIONS.sprintfSinglestring("Who do you want to %s?\n", string_buffer2);
            if (ioclass.Do_player_dialog(c, string_buffer, string_buffer2) != phantdefs.S_NORM)
            {
                event_ptr = null; // free((void*) event_ptr);
                return;
            }

            /* if the player named himself */
            if (!CFUNCTIONS.strcmp(string_buffer2, c.modifiedName) && c.wizard < 3)
            {
                event_ptr = null; // free((void*) event_ptr);
                ioclass.Do_send_line(c, "You may not do it to yourself!\n");
                ioclass.Do_send_line(c, "(You should know the routine by now!)\n");
                ioclass.Do_more(c);

                ioclass.Do_send_clear(c);
                return;
            }

            event_ptr.arg3 = CFUNCTIONS.strlen(c.modifiedName) + 1;
            event_ptr.arg4 = "";// (void*) Do_malloc(event_ptr.arg3);
            string arg4 = (string)event_ptr.arg4;
            CFUNCTIONS.strcpy(ref arg4, c.modifiedName);
            event_ptr.arg4 = arg4;
            event_ptr.from = c.game;

            if (eventclass.Do_send_character_event(c, event_ptr, string_buffer2) == 0)
            {
                event_ptr.arg4 = null;// event_ptr.arg4 = null; //free((void*) event_ptr.arg4);
                event_ptr = null; // free((void*) event_ptr);
                ioclass.Do_send_line(c, "That character just left the game.\n");
                ioclass.Do_more(c);

                ioclass.Do_send_clear(c);
                return;
            }

            if (c.wizard < 3)
            {

                /* remove the mana */
                statsclass.Do_mana(c, -manaCost, 0);

                /* add sin for the nastier abilities */
                if (event_ptr.type == phantdefs.CURSE_EVENT)
                {

                    statsclass.Do_sin(c, 0.5);
                }
                else if (event_ptr.type == phantdefs.DEGENERATE_EVENT)
                {

                    statsclass.Do_sin(c, 0.5);
                }
                else if (event_ptr.type == phantdefs.SLAP_EVENT)
                {

                    statsclass.Do_sin(c, 0.1);
                }
                else if (event_ptr.type == phantdefs.RELOCATE_EVENT)
                {
                    dtemp = CFUNCTIONS.floor((CFUNCTIONS.sqrt(CFUNCTIONS.pow(event_ptr.arg1, 2)
                                      + CFUNCTIONS.pow(event_ptr.arg2, 2)) / phantdefs.D_CIRCLE) + 1);

                    statsclass.Do_sin(c, CFUNCTIONS.MIN(5.0, (dtemp / 100.0)));
                }
            }

            ioclass.Do_send_line(c, "What you ask shall be done, eminence.\n");
            ioclass.Do_more(c);
            ioclass.Do_send_clear(c);
            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_administrate(struct client_t *c)
        /
        / FUNCTION: wizard commands
        /
        / AUTHOR: E. A. Estes, 2/28/86
        /	  Brian Kelly, 5/16/99
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

        internal void Do_administrate(client_t c)
        {
            event_t event_ptr = new event_t();
            realm_object_t object_ptr = new realm_object_t();
            button_t theButtons = new button_t();
            string string_buffer = ""; //[phantdefs.SZ_LINE], 
            string string_buffer2 = ""; //[phantdefs.SZ_LINE];
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            long answer = -1;              /* pointer to option description */
            int itemp;

            /* determine what the player wants to do */
            CFUNCTIONS.strcpy(ref theButtons.button[0], "Enact\n");
            CFUNCTIONS.strcpy(ref theButtons.button[1], "Decree\n");
            CFUNCTIONS.strcpy(ref theButtons.button[2], "Intervene\n");
            CFUNCTIONS.strcpy(ref theButtons.button[3], "Command\n");
            CFUNCTIONS.strcpy(ref theButtons.button[4], "Moderate\n");
            if (c.wizard >= 5)
            {
                CFUNCTIONS.strcpy(ref theButtons.button[5], "Send Event\n");
                CFUNCTIONS.strcpy(ref theButtons.button[6], "Shutdown\n");
            }
            else
            {
                ioclass.Do_clear_buttons(theButtons, 5);
                ioclass.Do_clear_buttons(theButtons, 6);
            }
            CFUNCTIONS.strcpy(ref theButtons.button[7], "Cancel\n");
            ioclass.Do_clear_buttons(theButtons, 8);

            ioclass.Do_send_line(c, "What would you like to do?\n");
            if (ioclass.Do_buttons(c, ref answer, theButtons) != phantdefs.S_NORM || answer == 7)
            {

                ioclass.Do_send_clear(c);
                return;
            }

            ioclass.Do_send_clear(c);
            switch (answer)
            {

                /* order a shutdown */
                case 6:

                    CFUNCTIONS.strcpy(ref theButtons.button[0], "Leisure\n");
                    CFUNCTIONS.strcpy(ref theButtons.button[1], "Normal\n");
                    CFUNCTIONS.strcpy(ref theButtons.button[2], "Fast\n");
                    CFUNCTIONS.strcpy(ref theButtons.button[3], "Hard\n");
                    ioclass.Do_clear_buttons(theButtons, 4);
                    CFUNCTIONS.strcpy(ref theButtons.button[7], "Cancel\n");


                    ioclass.Do_send_line(c, "How do you wish to shutdown the server?\n");
                    if (ioclass.Do_buttons(c, ref answer, theButtons) != phantdefs.S_NORM || answer == 7)
                    {

                        ioclass.Do_send_clear(c);
                        return;
                    }


                    ioclass.Do_send_clear(c);
                    switch (answer)
                    {

                        /* shutdown game when no players are logged on */
                        case 0:
                            itemp = phantdefs.LEISURE_SHUTDOWN;
                            break;

                        /* wait for all threads to exit */
                        case 1:
                            itemp = phantdefs.SHUTDOWN;
                            break;

                        /* forget the threads, start saving the realm */
                        case 2:
                            itemp = phantdefs.FAST_SHUTDOWN;
                            break;

                        /* order the server down now */
                        case 3:
                            itemp = phantdefs.HARD_SHUTDOWN;
                            break;

                        default:
                            error_msg = CFUNCTIONS.sprintfSinglestring("[%s] Returned non-option in Do_administrate.\n",
                                    c.connection_id);


                            fileclass.Do_log_error(error_msg);

                            hackclass.Do_caught_hack(c, phantdefs.H_SYSTEM);
                            return;
                    }

                    /* double check */
                    ioclass.Do_send_line(c, "Are you sure you wish to shutdown the server?\n");
                    if (ioclass.Do_yes_no(c, ref answer) != phantdefs.S_NORM || answer == 1)
                    {

                        ioclass.Do_send_clear(c);
                        return;
                    }


                    ioclass.Do_send_clear(c);
                    server_hook = itemp;
                    return;

                /* build your own event */
                case 5:

                    event_ptr = eventclass.Do_create_event();

                    if (ioclass.Do_long_dialog(c, ref event_ptr.arg3, "Event number?\n") != 0)
                    {

                        event_ptr = null; // free((void*) event_ptr);
                        return;
                    }

                    event_ptr.type = (short)event_ptr.arg3;

                    if (ioclass.Do_double_dialog(c, ref event_ptr.arg1, "Argument 1?\n") != 0)
                    {

                        event_ptr = null; // free((void*) event_ptr);
                        return;
                    }

                    if (ioclass.Do_double_dialog(c, ref event_ptr.arg2, "Argument 2?\n") != 0)
                    {

                        event_ptr = null; // free((void*) event_ptr);
                        return;
                    }


                    if (ioclass.Do_long_dialog(c, ref event_ptr.arg3, "Argument 3?\n") != 0)
                    {

                        event_ptr = null; // free((void*) event_ptr);
                        return;
                    }

                    CFUNCTIONS.strcpy(ref string_buffer2, "send the event to");
                    break;

                case 0:


                    Do_enact(c);
                    return;

                case 1:


                    Do_decree(c);
                    return;

                case 2:


                    Do_intervene(c);
                    return;

                case 3:


                    Do_command(c);
                    return;

                case 4:


                    Do_moderate(c);
                    return;

                default:
                    error_msg = CFUNCTIONS.sprintfSinglestring("[%s] Returned non-option in Do_administrate(2).\n",
                            c.connection_id);


                    fileclass.Do_log_error(error_msg);

                    hackclass.Do_caught_hack(c, phantdefs.H_SYSTEM);
                    return;
            }

            /* if we're here, we have an event to send to another player */
            /* prompt for player to affect */
            string_buffer = CFUNCTIONS.sprintfSinglestring("Who do you want to %s?\n", string_buffer2);
            if (ioclass.Do_player_dialog(c, string_buffer, string_buffer2) != phantdefs.S_NORM)
            {
                event_ptr = null; // free((void*) event_ptr);
                return;
            }

            event_ptr.from = c.game;

            if (eventclass.Do_send_character_event(c, event_ptr, string_buffer2) == 0)
            {
                event_ptr = null; // free((void*) event_ptr);
                ioclass.Do_send_line(c, "That character just left the game.\n");
                ioclass.Do_more(c);
                return;
            }

            ioclass.Do_send_line(c, "Game modification complete.\n");
            ioclass.Do_more(c);
            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_moderate(struct client_t *c)
        /
        / FUNCTION: wizard administrative powers
        /
        / AUTHOR: Brian Kelly, 11/8/00
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

        internal void Do_moderate(client_t c)
        {
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            button_t theButtons = new button_t();
            long answer = -1;              /* pointer to option description */

            /* determine what the player wants to do */
            CFUNCTIONS.strcpy(ref theButtons.button[0], "History\n");
            CFUNCTIONS.strcpy(ref theButtons.button[1], "Info\n");
            CFUNCTIONS.strcpy(ref theButtons.button[2], "Cantrip\n");
            CFUNCTIONS.strcpy(ref theButtons.button[3], "Flog\n");
            CFUNCTIONS.strcpy(ref theButtons.button[4], "Modify\n");
            CFUNCTIONS.strcpy(ref theButtons.button[5], "Restore\n");
            ioclass.Do_clear_buttons(theButtons, 6);
            CFUNCTIONS.strcpy(ref theButtons.button[7], "Cancel\n");

            ioclass.Do_send_line(c, "How do you wish to moderate the game?\n");
            if (ioclass.Do_buttons(c, ref answer, theButtons) != phantdefs.S_NORM || answer == 7)
            {
                ioclass.Do_send_clear(c);
                return;
            }

            ioclass.Do_send_clear(c);
            switch (answer)
            {

                /* Get game information */
                case 0:

                    infoclass.Do_history(c);
                    return;

                case 1:

                    infoclass.Do_wizard_information(c);
                    return;

                case 2:
                    Do_cantrip(c);
                    return;

                case 3:

                    Do_flog(c);
                    return;

                case 4:

                    Do_modify(c);
                    return;

                case 5:

                    restoreclass.Do_restoreoptions(c);
                    return;

                default:
                    error_msg = CFUNCTIONS.sprintfSinglestring("[%s] Returned non-option in Do_moderate.\n",
                            c.connection_id);


                    fileclass.Do_log_error(error_msg);

                    hackclass.Do_caught_hack(c, phantdefs.H_SYSTEM);
                    return;
            }
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_flog(struct client_t *c)
        /
        / FUNCTION: discipline players
        /
        / AUTHOR: Brian Kelly, 01/17/01
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

        void Do_flog(client_t c)
        {
            event_t event_ptr = new event_t();
            button_t theButtons = new button_t();
            string string_buffer = ""; //[phantdefs.SZ_LINE], 
            string string_buffer2 = ""; //[phantdefs.SZ_LINE];
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            long answer = -1;              /* pointer to option description */

            /* determine what the player wants to do */
            CFUNCTIONS.strcpy(ref theButtons.button[0], "Suspend\n");
            CFUNCTIONS.strcpy(ref theButtons.button[1], "Unsuspend\n");
            CFUNCTIONS.strcpy(ref theButtons.button[2], "Mute\n");
            CFUNCTIONS.strcpy(ref theButtons.button[3], "Kick\n");
            CFUNCTIONS.strcpy(ref theButtons.button[4], "Ban\n");
            CFUNCTIONS.strcpy(ref theButtons.button[5], "BoD\n");
            CFUNCTIONS.strcpy(ref theButtons.button[6], "Vaporize\n");
            CFUNCTIONS.strcpy(ref theButtons.button[7], "Cancel\n");

            ioclass.Do_send_line(c, "How do you wish to flog the players?\n");
            if (ioclass.Do_buttons(c, ref answer, theButtons) != phantdefs.S_NORM || answer == 7)
            {

                ioclass.Do_send_clear(c);
                return;
            }
            ioclass.Do_send_clear(c);
            switch (answer)
            {

                /* Suspend player */
                case 0:
                    event_ptr = eventclass.Do_create_event();
                    event_ptr.type = (short)phantdefs.SUSPEND_EVENT;
                    CFUNCTIONS.strcpy(ref string_buffer2, "lock");
                    break;

                /* Unsuspend player */
                case 1:
                    event_ptr = eventclass.Do_create_event();
                    event_ptr.type = (short)phantdefs.UNSUSPEND_EVENT;
                    CFUNCTIONS.strcpy(ref string_buffer2, "unlock");
                    break;

                /* mute player */
                case 2:

                    tagsclass.Do_create_tag(c, phantdefs.T_MUTE);
                    return;

                /* Kick player */
                case 3:
                    event_ptr = eventclass.Do_create_event();
                    event_ptr.type = (short)phantdefs.KICK_EVENT;
                    CFUNCTIONS.strcpy(ref string_buffer2, "kick from the game");
                    break;

                /* ban player */
                case 4:

                    tagsclass.Do_create_tag(c, phantdefs.T_BAN);
                    return;

                /* Squish player */
                case 5:
                    event_ptr = eventclass.Do_create_event();
                    event_ptr.type = (short)phantdefs.DEATH_EVENT;
                    event_ptr.arg3 = phantdefs.K_SQUISH;
                    CFUNCTIONS.strcpy(ref string_buffer2, "squish");
                    break;

                case 6:	/* vaporize */
                    event_ptr = eventclass.Do_create_event();
                    event_ptr.type = (short)phantdefs.DEATH_EVENT;
                    event_ptr.arg3 = phantdefs.K_VAPORIZED;
                    CFUNCTIONS.strcpy(ref string_buffer2, "vaporize");
                    break;

                default:
                    error_msg = CFUNCTIONS.sprintfSinglestring("[%s] Returned non-option in Do_flog.\n",
                            c.connection_id);


                    fileclass.Do_log_error(error_msg);

                    hackclass.Do_caught_hack(c, phantdefs.H_SYSTEM);
                    return;
            }

            /* if we're here, we have an event to send to another player */
            /* prompt for player to affect */
            string_buffer = CFUNCTIONS.sprintfSinglestring("Who do you want to %s?\n", string_buffer2);
            if (ioclass.Do_player_dialog(c, string_buffer, string_buffer2) != phantdefs.S_NORM)
            {
                event_ptr = null; // free((void*) event_ptr);
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
                event_ptr = null; // free((void*) event_ptr);
                ioclass.Do_send_line(c, "That character just left the game.\n");
                ioclass.Do_more(c);
                return;
            }

            ioclass.Do_send_line(c, "Flog complete.\n");
            ioclass.Do_more(c);
            ioclass.Do_send_clear(c);
            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_cantrip(struct client_t *c)
        /
        / FUNCTION: apprentice functions
        /
        / AUTHOR: Eugene Hung, 08/03/01
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

        internal void Do_cantrip(client_t c)
        {
            event_t event_ptr = new event_t();
            button_t theButtons = new button_t();
            string string_buffer = ""; //[phantdefs.SZ_LINE], 
            string string_buffer2 = ""; //[phantdefs.SZ_LINE];
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            string victim = ""; //[phantdefs.SZ_NAME];
            string warning = ""; //[phantdefs.SZ_LINE];
            double dtemp;
            long answer = -1;              /* pointer to option description */

            /* determine what the player wants to do */
            ioclass.Do_clear_buttons(theButtons, 0);
            CFUNCTIONS.strcpy(ref theButtons.button[0], "Mute\n");
            CFUNCTIONS.strcpy(ref theButtons.button[1], "Ban\n");
            CFUNCTIONS.strcpy(ref theButtons.button[2], "Tag\n");
            CFUNCTIONS.strcpy(ref theButtons.button[3], "Reprimand\n");
            CFUNCTIONS.strcpy(ref theButtons.button[4], "Clear Smurf\n");
            if (c.game.hearAllChannels == phantdefs.HEAR_SELF)
            {
                CFUNCTIONS.strcpy(ref theButtons.button[5], "Hear Ch 1\n");
            }
            else
            {
                CFUNCTIONS.strcpy(ref theButtons.button[5], "Tune Out\n");

            }


            if (c.player.location == phantdefs.PL_THRONE && c.player.special_type
                == phantdefs.SC_STEWARD && !c.player.cloaked)
            {

                CFUNCTIONS.strcpy(ref theButtons.button[6], "Enact\n");
            }
            else if (c.player.location == phantdefs.PL_THRONE && c.player.special_type
                == phantdefs.SC_KING && !c.player.cloaked)
            {

                CFUNCTIONS.strcpy(ref theButtons.button[6], "Decree\n");
            }
            else if ((c.player.special_type == phantdefs.SC_COUNCIL || c.player.special_type
            == phantdefs.SC_EXVALAR) && !c.player.cloaked)
            {

                CFUNCTIONS.strcpy(ref theButtons.button[6], "Intervene\n");
            }
            else if (c.player.special_type == phantdefs.SC_VALAR && !c.player.cloaked)
            {
                CFUNCTIONS.strcpy(ref theButtons.button[6], "Command\n");
            }
            else if (c.player.level < 10)
            {
                CFUNCTIONS.strcpy(ref theButtons.button[6], "Help\n");
            }
            CFUNCTIONS.strcpy(ref theButtons.button[7], "Cancel\n");

            /* create an event to handle the player action */
            event_ptr = eventclass.Do_create_event();
            event_ptr.from = c.game;
            event_ptr.type = (short)phantdefs.NULL_EVENT;

            ioclass.Do_send_line(c, "What cantrip do you wish to cast?\n");
            string_buffer = CFUNCTIONS.sprintfSinglestring("Current Smurf : %s\n", c.realm.monster[40].name);
            ioclass.Do_send_line(c, string_buffer);
            if (ioclass.Do_buttons(c, ref answer, theButtons) != phantdefs.S_NORM || answer == 7)
            {

                ioclass.Do_send_clear(c);
                return;
            }
            ioclass.Do_send_clear(c);
            switch (answer)
            {

                /* mute player */
                case 0:

                    tagsclass.Do_create_minitag(c, phantdefs.T_MUTE);
                    break;


                /* ban player */
                case 1:

                    tagsclass.Do_create_minitag(c, phantdefs.T_BAN);
                    break;


                /* tag player */
                case 2:
                    CFUNCTIONS.strcpy(ref theButtons.button[0], "Prefix\n");
                    CFUNCTIONS.strcpy(ref theButtons.button[1], "Suffix\n");

                    CFUNCTIONS.strcpy(ref theButtons.button[2], "Untag\n");
                    ioclass.Do_clear_buttons(theButtons, 3);
                    CFUNCTIONS.strcpy(ref theButtons.button[7], "Cancel\n");
                    ioclass.Do_send_line(c, "What type of tag?\n");
                    if (ioclass.Do_buttons(c, ref answer, theButtons) != phantdefs.S_NORM || answer == 7)
                    {

                        ioclass.Do_send_clear(c);
                        return;
                    }

                    ioclass.Do_send_clear(c);
                    if (answer == 0)
                    {
                        tagsclass.Do_create_minitag(c, phantdefs.T_PREFIX);
                    }
                    else if (answer == 1)
                    {
                        tagsclass.Do_create_minitag(c, phantdefs.T_SUFFIX);
                    }
                    else
                    {

                        event_t eventPtr = new event_t();
                        string untagee = ""; //[phantdefs.SZ_NAME];


                        fileclass.Do_log(pathnames.DEBUG_LOG, "Untag code reached\n");

                        /* whose tag will we remove? */
                        string_buffer = CFUNCTIONS.sprintfSinglestring("Whose tag do you wish to remove?\n");
                        if (ioclass.Do_player_dialog(c, string_buffer, untagee) != phantdefs.S_NORM)
                        {
                            return;
                        }


                        fileclass.Do_log(pathnames.DEBUG_LOG, "Untagee selected\n");

                        /* create the event */
                        eventPtr = eventclass.Do_create_event();

                        /* only the event type should be needed to throw this event */
                        eventPtr.type = (short)phantdefs.UNTAG_EVENT;


                        fileclass.Do_log(pathnames.DEBUG_LOG, "Untag event created\n");

                        eventclass.Do_send_character_event(c, eventPtr, untagee);
                    }
                    break;

                /* Reprimand player */
                case 3:
                    /* tell the event it's a reprimand */
                    event_ptr.type = (short)phantdefs.REPRIMAND_EVENT;

                    /* target the reprimand */
                    string_buffer = CFUNCTIONS.sprintfSinglestring("Who do you want to reprimand?\n");
                    if (ioclass.Do_player_dialog(c, string_buffer, string_buffer2) != phantdefs.S_NORM)
                    {

                        event_ptr = null; // free((void*) event_ptr);
                        return;
                    }
                    event_ptr.arg3 = CFUNCTIONS.strlen(c.modifiedName) + 1;
                    event_ptr.arg4 = ""; // (void*) Do_malloc(event_ptr.arg3);
                    string arg4 = (string)event_ptr.arg4;
                    CFUNCTIONS.strcpy(ref arg4, c.modifiedName);
                    event_ptr.arg4 = arg4;

                    /* send off the event */
                    if (eventclass.Do_send_character_event(c, event_ptr, string_buffer2) == 0)
                    {

                        event_ptr.arg4 = null; //free((void*) event_ptr.arg4);

                        event_ptr = null; // free((void*) event_ptr);

                        ioclass.Do_send_line(c, "That character just left the game.\n");

                        ioclass.Do_more(c);

                        ioclass.Do_send_clear(c);
                    }
                    break;


                /* clear smurf name */
                case 4:

                    miscclass.Do_lock_mutex(c.realm.monster_lock);

                    CFUNCTIONS.strcpy(ref c.realm.monster[40].name, "A Smurf");

                    miscclass.Do_unlock_mutex(c.realm.monster_lock);
                    break;

                /* let player always hear channel 1 */
                case 5:
                    miscclass.Do_lock_mutex(c.realm.realm_lock);
                    if (c.game.hearAllChannels == phantdefs.HEAR_SELF)
                    {
                        c.game.hearAllChannels = phantdefs.HEAR_ONE;
                    }
                    else
                    {
                        c.game.hearAllChannels = phantdefs.HEAR_SELF;
                    }
                    miscclass.Do_unlock_mutex(c.realm.realm_lock);
                    break;

                /* use special position powers */
                case 6:

                    event_ptr.to = c.game;

                    switch (c.player.special_type)
                    {

                        case phantdefs.SC_STEWARD:
                            Do_enact(c);
                            break;

                        case phantdefs.SC_KING:
                            Do_decree(c);
                            break;

                        case phantdefs.SC_COUNCIL:
                        case phantdefs.SC_EXVALAR:
                            Do_intervene(c);
                            break;

                        case phantdefs.SC_VALAR:
                            Do_command(c);
                            break;

                        default:
                            Do_help(c);
                            break;
                    }

                    break;


                default:
                    error_msg = CFUNCTIONS.sprintfSinglestring("[%s] Returned non-option in Do_cantrip.\n",
                            c.connection_id);


                    fileclass.Do_log_error(error_msg);

                    hackclass.Do_caught_hack(c, phantdefs.H_SYSTEM);
                    return;
            }

            if (event_ptr.type == phantdefs.NULL_EVENT)
            {

                event_ptr = null; // free((void*) event_ptr);
            }


            return;
        }

        /************************************************************************
        /
        / FUNCTION NAME: Do_modify(struct client_t *c)
        /
        / FUNCTION: more wizard commands
        /
        / AUTHOR: Brian Kelly, 01/17/01
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

        void Do_modify(client_t c)
        {
            button_t theButtons = new button_t();
            string string_buffer = ""; //[phantdefs.SZ_LINE], 
            string string_buffer2 = ""; //[phantdefs.SZ_LINE];
            string characterName = ""; //[phantdefs.SZ_NAME];
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            long answer = -1;              /* pointer to option description */

            /* determine what the player wants to do */
            CFUNCTIONS.strcpy(ref theButtons.button[0], "Prefix\n");
            CFUNCTIONS.strcpy(ref theButtons.button[1], "Suffix\n");
            CFUNCTIONS.strcpy(ref theButtons.button[2], "Suicide\n");
            CFUNCTIONS.strcpy(ref theButtons.button[3], "Special Clear\n");
            CFUNCTIONS.strcpy(ref theButtons.button[4], "Untag\n");
            CFUNCTIONS.strcpy(ref theButtons.button[5], "Restore Char\n");
            ioclass.Do_clear_buttons(theButtons, 6);
            CFUNCTIONS.strcpy(ref theButtons.button[7], "Cancel\n");

            ioclass.Do_send_line(c, "Please choose the modification you wish.\n");
            if (ioclass.Do_buttons(c, ref answer, theButtons) != phantdefs.S_NORM || answer == 7)
            {

                ioclass.Do_send_clear(c);
                return;
            }
            ioclass.Do_send_clear(c);
            switch (answer)
            {

                /* Prefix player */
                case 0:

                    tagsclass.Do_create_tag(c, phantdefs.T_PREFIX);
                    return;

                /* Suffix player */
                case 1:

                    tagsclass.Do_create_tag(c, phantdefs.T_SUFFIX);
                    return;

                /* kill player */
                case 2:

                    tagsclass.Do_create_tag(c, phantdefs.T_SUICIDE);
                    return;

                /*Clear or Add King or Valar information*/
                case 3:
                    /* determine what the wizard wants to do */
                    CFUNCTIONS.strcpy(ref theButtons.button[0], "Add King\n");
                    CFUNCTIONS.strcpy(ref theButtons.button[1], "Add Valar\n");

                    CFUNCTIONS.strcpy(ref theButtons.button[2], "Remove King\n");
                    CFUNCTIONS.strcpy(ref theButtons.button[3], "Remove Valar\n");
                    ioclass.Do_clear_buttons(theButtons, 4);
                    ioclass.Do_clear_buttons(theButtons, 5);
                    ioclass.Do_clear_buttons(theButtons, 6);
                    CFUNCTIONS.strcpy(ref theButtons.button[7], "Cancel\n");

                    ioclass.Do_send_line(c, "Which would you like to clear?\n");
                    if (ioclass.Do_buttons(c, ref answer, theButtons) != phantdefs.S_NORM || answer == 7)
                    {

                        ioclass.Do_send_clear(c);
                        return;
                    }
                    ioclass.Do_send_clear(c);
                    switch (answer)
                    {


                        /*shall we add a king*/
                        case 0:


                            ioclass.Do_send_line(c, "You never did this one Arella, silly girl\n");

                            break;


                        /*or shall we add a Valar*/
                        case 1:

                            if ((ioclass.Do_string_dialog(c, ref characterName, phantdefs.SZ_NAME - 1,
                                "What is the name of the character to be named Valar?\n") ? 1 : 0) != phantdefs.S_NORM)
                            {

                                return;
                            }



                            miscclass.Do_lock_mutex(c.realm.realm_lock);

                            c.realm.valar = c.game;

                            CFUNCTIONS.strcpy(ref c.realm.valar_name, characterName);

                            miscclass.Do_unlock_mutex(c.realm.realm_lock);

                            ioclass.Do_send_line(c, "In the words of Jean Luc Picard, it is so.\n");
                            return;

                            break;

                        /* Shall we remove the king?*/
                        case 2:

                            miscclass.Do_lock_mutex(c.realm.realm_lock);


                            c.realm.king = null;
                            c.realm.king_flag = false;
                            c.realm.king_name = "\0";

                            miscclass.Do_unlock_mutex(c.realm.realm_lock);

                            ioclass.Do_send_line(c, "The king has now been removed from office...No there was no intern involved.\n");
                            return;

                            break;

                        /*Or shall we remove the Valar?*/
                        case 3:

                            miscclass.Do_lock_mutex(c.realm.realm_lock);


                            c.realm.valar = null;
                            c.realm.valar_name = "\0";

                            miscclass.Do_unlock_mutex(c.realm.realm_lock);

                            ioclass.Do_send_line(c, "Bye Bye Valar... We're going to miss you so.\n");
                            return;

                            break;



                    }
                    return;

                /* remove a tag */
                case 4:

                    tagsclass.Do_untag(c);
                    return;

                /* restore a character */
                case 5:


                    fileclass.Do_restore_character(c);
                    return;

                default:
                    error_msg = CFUNCTIONS.sprintfSinglestring("[%s] Returned non-option in Do_modify.\n",
                            c.connection_id);


                    fileclass.Do_log_error(error_msg);

                    hackclass.Do_caught_hack(c, phantdefs.H_SYSTEM);
                    return;
            }
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_rest(struct client_t *c)
        /
        / FUNCTION: rest
        /
        / AUTHOR: E. A. Estes, 12/4/85
        /	  Brian Kelly, 5/19/99
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

        internal void Do_rest(client_t c)
        {
            float addMana;

            if (c.player.type == phantdefs.C_DWARF)
            {
                statsclass.Do_energy(c,
                        c.player.energy + (c.player.max_energy + c.player.shield)
                 / 12.0 + c.player.level / 3.0 + 2.0, c.player.max_energy,
                 c.player.shield, c.battle.force_field, 0);
            }
            else
            {
                statsclass.Do_energy(c,
                        c.player.energy + (c.player.max_energy + c.player.shield)
                 / 15.0 + c.player.level / 3.0 + 2.0, c.player.max_energy,
                 c.player.shield, c.battle.force_field, 0);
            }

            /* cannot find mana if cloaked */
            if (!c.player.cloaked)
            {

                addMana = CFUNCTIONS.floor((c.player.circle + c.player.level) / 5.0);

                if (addMana < 1.0)
                {
                    addMana = 1.0f;
                }


                statsclass.Do_mana(c, addMana, 0);
            }

            /* remove combat fatigue */
            if (c.battle.rounds > 0)
            {

                c.battle.rounds -= 30;
                if (c.battle.rounds < 0)
                {
                    c.battle.rounds = 0;
                }


                statsclass.Do_speed(c, c.player.max_quickness, c.player.quicksilver,
                    c.battle.speedSpell, 0);
            }
        }


        /************************************************************************
        /
        / FUNCTION NAME: statsclass.Do_energy_void(struct client_t *c)
        /
        / FUNCTION: energy void
        /
        / AUTHOR: E. A. Estes, 12/4/85
        /	  Brian Kelly, 5/19/99
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

        internal void Do_energy_void(client_t c)
        {
            event_t event_ptr = new event_t();

            ioclass.Do_send_line(c, "You've hit an energy void!\n");

            ioclass.Do_more(c);

            statsclass.Do_mana(c, -2.0 * c.player.mana / 3.0, 0);
            statsclass.Do_gold(c, -c.player.gold / 5, 0);
            statsclass.Do_energy(c, c.player.energy / 2.0 + 1.0, c.player.max_energy * .99,
                c.player.shield, 0, 0);

            miscclass.Do_check_weight(c);

            event_ptr = eventclass.Do_create_event();
            event_ptr.type = (short)phantdefs.MOVE_EVENT;
            event_ptr.arg3 = phantdefs.A_NEAR;
            eventclass.Do_handle_event(c, event_ptr);

            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_trading_post(struct client_t *c)
        /
        / FUNCTION: do trading post stuff
        /
        / AUTHOR: E. A. Estes, 12/4/85
        /	  Brian Kelly, 6/17/99
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
        /       Different trading posts have different items.
        /       Merchants cannot be cheated, but they can be dishonest
        /       themselves.
        /
        /       Shields, swords, and quicksilver are not cumulative.  This is
        /       one major area of complaint, but there are two reasons for this:
        /               1) It becomes MUCH too easy to make very large versions
        /                  of these items.
        /               2) In the real world, one cannot simply weld two swords
        /                  together to make a bigger one.
        /
        /       At one time, it was possible to sell old weapons at half the purchase
        /       price.  This resulted in huge amounts of gold floating around,
        /       and the game lost much of its challenge.
        /
        /       Also, purchasing gems defeats the whole purpose of gold.  Gold
        /       is small change for lower level players.  They really shouldn't
        /       be able to accumulate more than enough gold for a small sword or
        /       a few books.  Higher level players shouldn't even bother to pick
        /       up gold, except maybe to buy mana once in a while.
        /
        *************************************************************************/

        internal void Do_trading_post(client_t c)
        {
            button_t theButtons = new button_t();
            button_t theItems = new button_t();
            double numitems = -1;       /* number of items to purchase */
            double cost = -1;           /* cost of purchase */
            double blessingcost = -1;   /* cost of blessing */
            double amuletcost = -1;     /* cost of amulet */
            char ch;             /* input */
            int size = -1;   /* size of the trading post */
            int loop = -1;   /* loop counter */
            long itemp = -1;
            float ftemp = -1;
            int cheat = 0;      /* number of times player has tried to cheat */
            bool dishonest = false;   /* set when merchant is dishonest */
            bool thief = false;   /* set when player is too rich for his level */
            int spent = 0;       /* total amount of gold spent on this visit */
            string string_buffer = ""; //[phantdefs.SZ_LINE], 
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            event_t event_ptr = new event_t();
            long answer = -1;              /* pointer to option description */

            size = (int)CFUNCTIONS.sqrt(Mathf.Abs((float)c.player.x) / 100) + 1;

            /* only give blessings in the Cracks */
            if (size != 6)
            {
                size = (int)CFUNCTIONS.MIN(6, size);
            }
            else
            {
                size = 7;
            }


            /* set up cost of variable items */
            blessingcost = 500.0 * (c.player.level + 5.0) + 10000;
            amuletcost = 250.0 + CFUNCTIONS.floor(c.player.level / 5);

            ioclass.Do_clear_buttons(theItems, 0);

            for (loop = 0; loop < size; ++loop)
            {

                string_buffer = CFUNCTIONS.sprintfSinglestring("%s\n", c.realm.shop_item[loop].item);

                CFUNCTIONS.strcpy(ref theItems.button[loop], string_buffer);
            }


            CFUNCTIONS.strcpy(ref theItems.button[7], "Cancel\n");

            for (; ; )
            {
                if (UnityGameController.StopApplication) //time to quit
                {
                    Debug.Log("Thread " + System.Threading.Thread.CurrentThread.Name + " stopping in Do_trading_post");
                    return;
                }
                else
                {
                    System.Threading.Thread.Sleep(33); //30fps
                }

                /* print Menu */
                for (loop = 0; loop < size; ++loop)
                {

                    /* print Menu */
                    if (loop == 6)
                        cost = blessingcost;
                    else if (loop == 3)
                        cost = amuletcost;
                    else
                        cost = c.realm.shop_item[loop].cost;


                    string_buffer = CFUNCTIONS.sprintfSinglestring("%-12s: %6.0f\n",
                            c.realm.shop_item[loop].item, cost);


                    ioclass.Do_send_line(c, string_buffer);
                }


                ioclass.Do_send_line(c,
             "You are at a trading post. All purchases must be made with gold.\n");
                /*
                    ioclass.Do_send_line(c, "\n");
                */
                /* print some important statistics */
                /*
                        CFUNCTIONS.sprintf(ref string_buffer,
                        "Gold:   %9.0f  Gems:  %9.0f  Level:   %6.0f  Charms: %6d\n",
                                c.player.gold, c.player.gems, c.player.level,
                        c.player.charms);

                    ioclass.Do_send_line(c, string_buffer);

                        CFUNCTIONS.sprintf(ref string_buffer,
                           "Shield: %9.0f  Sword: %9.0f  Quicksilver:%3.0f  Blessed: %s\n",
                               c.player.shield, c.player.sword, c.player.quicksilver,
                               (c.player.blessing ? " True" : "False"));

                    ioclass.Do_send_line(c, string_buffer);

                        string_buffer = CFUNCTIONS.sprintfSinglestring("Brains: %9.0f  Mana:  %9.0f", c.player.brains,
                        c.player.mana);

                    ioclass.Do_send_line(c, string_buffer);
                */
                CFUNCTIONS.strcpy(ref theButtons.button[0], "Purchase\n");

                CFUNCTIONS.strcpy(ref theButtons.button[1], "Sell Gems\n");

                CFUNCTIONS.strcpy(ref theButtons.button[2], "Leave\n");

                ioclass.Do_clear_buttons(theButtons, 3);

                if (ioclass.Do_buttons(c, ref itemp, theButtons) != phantdefs.S_NORM)
                {

                    ioclass.Do_send_clear(c);
                    return;
                }



                ioclass.Do_send_clear(c);
                switch (itemp)
                {

                    case 2:           /* leave */
                        return;

                    case 0:           /* make purchase */


                        ioclass.Do_send_line(c, "What would you like to buy?\n");
                        if (ioclass.Do_buttons(c, ref itemp, theItems) != phantdefs.S_NORM)
                        {

                            ioclass.Do_send_clear(c);
                            return;
                        }


                        ioclass.Do_send_clear(c);
                        if ((itemp > size) && (itemp != 7) || itemp < 0)
                        {


                            error_msg = CFUNCTIONS.sprintfSinglestring(
                                "[%s] Returned non-option in Do_trading_post.\n",
                                c.connection_id);


                            fileclass.Do_log_error(error_msg);

                            hackclass.Do_caught_hack(c, phantdefs.H_SYSTEM);
                            return;
                        }
                        else
                        {
                            switch (itemp)
                            {

                                case 0:
                                    CFUNCTIONS.sprintf(ref string_buffer,
                    "Mana is one per %.0f gold piece.  How many do you want (%.0f max)?\n",
                                            c.realm.shop_item[0].cost,
                                CFUNCTIONS.floor(c.player.gold /

                                c.realm.shop_item[0].cost));

                                    if (ioclass.Do_double_dialog(c, ref numitems, string_buffer) != 0)
                                    {

                                        ioclass.Do_send_clear(c);
                                        break;
                                    }

                                    cost = numitems * c.realm.shop_item[0].cost;

                                    if (cost > c.player.gold || numitems < 0)
                                        ++cheat;

                                    else
                                    {
                                        cheat = 0;

                                        statsclass.Do_gold(c, -cost, 0);
                                        spent += (int)cost;


                                        string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, purchased %lf mana\n",
                                                        c.player.lcname,
                                            c.realm.charstats[c.player.type].class_name,
                                            numitems);


                                        fileclass.Do_log(pathnames.PURCHASE_LOG, string_buffer);

                                        if (macros.RND() < 0.02)
                                            dishonest = true;
                                        else

                                            statsclass.Do_mana(c, numitems, 0);
                                    }
                                    break;

                                case 1:

                                    string_buffer = CFUNCTIONS.sprintfSinglestring("Shields are %.0f gold per +1.\n",
                                c.realm.shop_item[1].cost);


                                    ioclass.Do_send_line(c, "Each plus will add to your energy.\n");

                                    CFUNCTIONS.sprintf(ref string_buffer,
                                "How strong a shield do you want (%.0f max)?\n",
                                CFUNCTIONS.floor(c.player.gold /

                                c.realm.shop_item[1].cost));

                                    if (ioclass.Do_double_dialog(c, ref numitems, string_buffer) != 0)
                                    {

                                        ioclass.Do_send_clear(c);
                                        break;
                                    }


                                    ioclass.Do_send_clear(c);
                                    cost = numitems * c.realm.shop_item[1].cost;

                                    if (numitems == 0.0)
                                        break;
                                    else if (cost > c.player.gold || numitems < 0)
                                        ++cheat;
                                    else if (numitems < c.player.shield)
                                    {

                                        ioclass.Do_send_line(c, "That's no better than what you already have.\n");


                                        ioclass.Do_more(c);

                                        ioclass.Do_send_clear(c);
                                    }
                                    else
                                    {
                                        cheat = 0;

                                        statsclass.Do_gold(c, -cost, 0);
                                        spent += (int)cost;

                                        string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, purchased %lf shield\n",
                                                c.player.lcname,
                                                c.realm.charstats[c.player.type].class_name,
                                                numitems);

                                        fileclass.Do_log(pathnames.PURCHASE_LOG, string_buffer);

                                        if ((c.player.level < 20) &&
                                             (macros.RND() < 1 -
                                                      (c.player.level * 350 + 150) / spent))
                                            thief = true;
                                        else if (macros.RND() < 0.02)
                                            dishonest = true;
                                        else

                                            statsclass.Do_energy(c, c.player.energy - c.player.shield
                                                + numitems, c.player.max_energy, numitems, 0, 0);
                                    }
                                    break;

                                case 2:

                                    if (c.player.blind)
                                    {
                                        ioclass.Do_send_line(c, "You can't read books while blind!\n");
                                        ioclass.Do_more(c);
                                        ioclass.Do_send_clear(c);
                                        break;
                                    }

                                    CFUNCTIONS.sprintf(ref string_buffer,
                             "A book costs %.0f gp.  How many do you want (%.0f max)?\n",
                                   c.realm.shop_item[2].cost, CFUNCTIONS.floor(c.player.gold /

                           c.realm.shop_item[2].cost));

                                    if (ioclass.Do_double_dialog(c, ref numitems, string_buffer) != 0)
                                    {

                                        ioclass.Do_send_clear(c);
                                        break;
                                    }


                                    ioclass.Do_send_clear(c);
                                    cost = numitems * c.realm.shop_item[2].cost;

                                    if (cost > c.player.gold || numitems < 0)
                                        ++cheat;

                                    else
                                    {
                                        cheat = 0;

                                        statsclass.Do_gold(c, -cost, 0);
                                        spent += (int)cost;

                                        string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, purchased %lf books\n",
                                                c.player.lcname,
                                                c.realm.charstats[c.player.type].class_name,
                                                numitems);

                                        fileclass.Do_log(pathnames.PURCHASE_LOG, string_buffer);

                                        if ((c.player.level < 20) &&
                                             (macros.RND() < 1 -
                                                      (c.player.level * 350 + 150) / spent))
                                            thief = true;
                                        else if (macros.RND() < 0.02)
                                            dishonest = true;
                                        else
                                        {

                                            statsclass.Do_book(c, (int)numitems);
                                        }
                                    }
                                    break;

                                case 3:

                                    CFUNCTIONS.sprintf(ref string_buffer,
                        "An amulet costs %.0f gp.  How many do you want (%.0f max)?\n",
                                amuletcost, CFUNCTIONS.floor(c.player.gold /

                        amuletcost));

                                    if (ioclass.Do_double_dialog(c, ref numitems, string_buffer) != 0)
                                    {

                                        ioclass.Do_send_clear(c);
                                        break;
                                    }


                                    ioclass.Do_send_clear(c);
                                    cost = numitems * amuletcost;

                                    if (cost > c.player.gold || numitems < 0)
                                        ++cheat;
                                    else
                                    {
                                        cheat = 0;
                                        statsclass.Do_gold(c, -cost, 0);
                                        spent += (int)cost;

                                        CFUNCTIONS.sprintf(ref string_buffer,
                                "%s, %s, purchased %lf amulets\n",
                                                c.player.lcname,
                                                c.realm.charstats[c.player.type].class_name,
                                                numitems);

                                        fileclass.Do_log(pathnames.PURCHASE_LOG, string_buffer);

                                        if (macros.RND() < 0.02)
                                            dishonest = true;
                                        else
                                        {
                                            c.player.amulets += (int)numitems;
                                        }
                                    }
                                    break;

                                case 4:


                                    string_buffer = CFUNCTIONS.sprintfSinglestring("Swords are %.0f gold per +1.\n",
                                        c.realm.shop_item[3].cost);


                                    ioclass.Do_send_line(c, "Each plus will improve your fighting ability.\n");


                                    CFUNCTIONS.sprintf(ref string_buffer,
                                "How strong a sword do you want (%.0f max)?\n",
                                        CFUNCTIONS.floor(c.player.gold /
                                            c.realm.shop_item[4].cost));

                                    if (ioclass.Do_double_dialog(c, ref numitems, string_buffer) != 0)
                                    {

                                        ioclass.Do_send_clear(c);
                                        break;
                                    }


                                    ioclass.Do_send_clear(c);
                                    cost = numitems * c.realm.shop_item[4].cost;

                                    if (numitems == 0.0)
                                        break;
                                    else if (cost > c.player.gold || numitems < 0)
                                        ++cheat;
                                    else if (numitems < c.player.sword)
                                    {

                                        ioclass.Do_send_line(c,
                                 "That's no better than what you already have.\n");


                                        ioclass.Do_more(c);

                                        ioclass.Do_send_clear(c);
                                    }
                                    else
                                    {
                                        cheat = 0;
                                        statsclass.Do_gold(c, -cost, 0);
                                        spent += (int)cost;

                                        string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, purchased %lf sword\n",
                                                c.player.lcname,
                                                c.realm.charstats[c.player.type].class_name,
                                                numitems);

                                        fileclass.Do_log(pathnames.PURCHASE_LOG, string_buffer);

                                        if ((c.player.level < 20) &&
                                             (macros.RND() < 1 -
                                                      (c.player.level * 350 + 150) / spent))
                                            thief = true;
                                        else if (macros.RND() < 0.02)
                                            dishonest = true;
                                        else

                                            statsclass.Do_strength(c, c.player.max_strength, numitems, 0, 0);
                                    }
                                    break;

                                case 5:
                                    CFUNCTIONS.sprintf(ref string_buffer,
                    "Quicksilver is %.0f gp per +1.  How many + do you want (%.0f max)?\n",
                    c.realm.shop_item[5].cost, CFUNCTIONS.floor(c.player.gold / c.realm.shop_item[5].cost));

                                    if (ioclass.Do_double_dialog(c, ref numitems, string_buffer) != 0)
                                    {

                                        ioclass.Do_send_clear(c);
                                        break;
                                    }


                                    ioclass.Do_send_clear(c);
                                    cost = numitems * c.realm.shop_item[5].cost;

                                    if (numitems == 0.0)
                                        break;
                                    else if (cost > c.player.gold || numitems < 0)
                                        ++cheat;
                                    else if (numitems < c.player.quicksilver)
                                    {

                                        ioclass.Do_send_line(c, "That's no better than what you already have.\n");
                                        
                                        ioclass.Do_more(c);

                                        ioclass.Do_send_clear(c);
                                    }
                                    else
                                    {
                                        cheat = 0;
                                        statsclass.Do_gold(c, -cost, 0);
                                        spent += (int)cost;

                                        CFUNCTIONS.sprintf(ref string_buffer, "%s, %s, purchased %lf quicksilver\n",
                                                c.player.lcname,
                                                c.realm.charstats[c.player.type].class_name,
                                                numitems);

                                        fileclass.Do_log(pathnames.PURCHASE_LOG, string_buffer);

                                        if ((c.player.level < 20) &&
                                             (macros.RND() < 1 -
                                                      (c.player.level * 350 + 150) / spent))
                                            thief = true;
                                        else if (macros.RND() < 0.02)
                                            dishonest = true;
                                        else

                                            statsclass.Do_speed(c, c.player.max_quickness, numitems, 0, 0);
                                    }
                                    break;

                                case 6:
                                    if (c.player.blessing)
                                    {
                                        ioclass.Do_send_line(c, "You already have a blessing.\n");

                                        ioclass.Do_more(c);

                                        ioclass.Do_send_clear(c);
                                        break;
                                    }

                                    CFUNCTIONS.sprintf(ref string_buffer, "A blessing requires a %.0f gp donation.  Still want one?\n", blessingcost);


                                    ioclass.Do_send_line(c, string_buffer);

                                    if (ioclass.Do_yes_no(c, ref answer) == phantdefs.S_NORM && answer == 0)
                                    {

                                        if (c.player.gold < blessingcost)
                                            ++cheat;
                                        else
                                        {
                                            cheat = 0;
                                            statsclass.Do_gold(c, -blessingcost, 0);
                                            spent += (int)cost;

                                            CFUNCTIONS.sprintf(ref string_buffer, "%s, %s, purchased 1 blessing, %lf\n",
                                                c.player.lcname,
                                                c.realm.charstats[c.player.type].class_name,
                                                blessingcost);

                                            fileclass.Do_log(pathnames.PURCHASE_LOG, string_buffer);

                                            if (macros.RND() < 0.02)
                                                dishonest = true;
                                            else

                                                statsclass.Do_blessing(c, 1, 0);
                                        }
                                    }

                                    ioclass.Do_send_clear(c);
                                    break;

                                case 7:
                                    break;

                            }
                            break;
                        }

                    case 1:           /* sell gems */


                        CFUNCTIONS.sprintf(ref string_buffer, "A gem is worth %.0f gp.  How many do you want to sell (%.0f max)?\n",
                            (double)phantdefs.N_GEMVALUE, c.player.gems);

                        if (ioclass.Do_double_dialog(c, ref numitems, string_buffer) != 0)
                        {

                            ioclass.Do_send_clear(c);
                            break;
                        }


                        ioclass.Do_send_clear(c);
                        if (numitems > c.player.gems || numitems < 0)
                        {
                            ++cheat;
                        }
                        else if (numitems > c.player.level * c.player.level + 1)
                        {
                            dishonest = true;
                        }
                        else
                        {
                            cheat = 0;

                            /* add gold manually to prevent taxes */
                            if (numitems > 0)
                            {

                                statsclass.Do_gems(c, -numitems, 0);
                                c.player.gold += numitems * phantdefs.N_GEMVALUE;

                                statsclass.Do_gold(c, 0, 1);

                                miscclass.Do_check_weight(c);
                            }
                        }
                        break;
                }

                if (cheat == 1)
                {

                    /* give the player some sin */
                    statsclass.Do_sin(c, .25);

                    ioclass.Do_send_line(c,
                    "Come on, merchants aren't stupid.  Stop cheating.\n");


                    ioclass.Do_more(c);

                    ioclass.Do_send_clear(c);
                }
                else if (cheat == 2)
                {

                    /* give the player more sin */
                    statsclass.Do_sin(c, .5);

                    ioclass.Do_send_line(c,
                    "You had your chance.  This merchant happens to be\n");

                    CFUNCTIONS.sprintf(ref string_buffer,
                    "a %.0f level magic user, and you made %s mad!\n",
                            macros.ROLL(c.player.circle * 20.0, 40.0), (macros.RND() <
                    0.5) ? "him" : "her");


                    ioclass.Do_send_line(c, string_buffer);

                    ioclass.Do_more(c);

                    ioclass.Do_send_clear(c);

                    /* prevent experimentos from using a post 
                       to bounce really far out */
                    event_ptr = eventclass.Do_create_event();

                    if (c.player.type != phantdefs.C_EXPER)
                    {
                        event_ptr.arg3 = phantdefs.A_FAR;
                    }
                    else
                    {
                        event_ptr.arg3 = phantdefs.A_NEAR;
                    }
                    event_ptr.type = (short)phantdefs.MOVE_EVENT;

                    eventclass.Do_handle_event(c, event_ptr);


                    statsclass.Do_energy(c, c.player.energy / 2.0 + 1.0, c.player.max_energy,
                        c.player.shield, 0, 0);

                    return;
                }
                else if (thief)
                {
                    event_ptr = eventclass.Do_create_event();
                    event_ptr.type = (short)phantdefs.MOVE_EVENT;
                    event_ptr.arg1 = CFUNCTIONS.floor(2350 + macros.RND() * 100);
                    event_ptr.arg2 = CFUNCTIONS.floor(2350 + macros.RND() * 100);
                    event_ptr.arg3 = phantdefs.A_SPECIFIC;

                    ioclass.Do_send_line(c, "The merchant wonders, 'How come a pipsqueak like you has so much gold?\n");
                    ioclass.Do_send_line(c, "The merchant cries, 'A thief!  Guards!  Throw this miscreant in the Cracks of Doom!\n");
                    string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, was called a thief.\n",
                                        c.player.lcname,
                                        c.realm.charstats[c.player.type].class_name);

                    fileclass.Do_log(pathnames.PURCHASE_LOG, string_buffer);


                    eventclass.Do_handle_event(c, event_ptr);

                    return;
                }
                else if (dishonest)
                {
                    ioclass.Do_send_line(c, "The merchant stole your money and teleported you!\n");
                    string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, was cheated.\n",
                                        c.player.lcname,
                                        c.realm.charstats[c.player.type].class_name);

                    fileclass.Do_log(pathnames.PURCHASE_LOG, string_buffer);


                    ioclass.Do_more(c);

                    ioclass.Do_send_clear(c);
                    event_ptr = eventclass.Do_create_event();
                    event_ptr.type = (short)phantdefs.MOVE_EVENT;
                    event_ptr.arg1 = CFUNCTIONS.floor(1.25 * c.player.x);
                    event_ptr.arg2 = CFUNCTIONS.floor(1.25 * c.player.y);
                    event_ptr.arg3 = phantdefs.A_SPECIFIC;

                    eventclass.Do_handle_event(c, event_ptr);
                    return;
                }
            }
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_teleport(struct client_t *c, struct event_t *the_event)
        /
        / FUNCTION: do trading post stuff
        /
        / AUTHOR: E. A. Estes, 12/4/85
        /	  Brian Kelly, 5/16/99
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

        internal void Do_teleport(client_t c, event_t the_event)
        {
            event_t event_ptr = new event_t();
            event_t event2_ptr = new event_t();
            double distance = 0;
            double mana = 0;
            double dtemp;
            int tempcircle;
            string string_buffer = ""; //[phantdefs.SZ_LINE];
            long answer = -1;

            event_ptr = eventclass.Do_create_event();
            event_ptr.type = (short)phantdefs.MOVE_EVENT;
            event_ptr.arg3 = phantdefs.A_FORCED;

            if (ioclass.Do_coords_dialog(c, ref event_ptr.arg1, ref event_ptr.arg2,
                "Where do you wish to teleport to?\n") != 0)
            {


                event_ptr = null; // free((void*) event_ptr);
                return;
            }

            miscclass.Do_distance(event_ptr.arg1, 0.0, event_ptr.arg2, 0.0, ref distance);
            tempcircle = CFUNCTIONS.floor(distance / phantdefs.D_CIRCLE + 1);

            /* block non-Gwaihir teleports into the Marshes/Cracks */
            if ((the_event.arg2 != 0 ? true : false) == false &&
                (tempcircle > 19 && tempcircle < 36))
            {
                ioclass.Do_send_line(c, "You try to teleport into the Marshes, but a mysterious force prevents you!\n");

                ioclass.Do_more(c);

                ioclass.Do_send_clear(c);
                return;
            }

            /* did the player teleport himself? */
            if ((the_event.arg3 != 0 ? true : false) == true)
            {


                miscclass.Do_distance(c.player.x, event_ptr.arg1, c.player.y,
                    event_ptr.arg2, ref distance);

                mana = CFUNCTIONS.ceil(distance * distance / (30 * c.player.magiclvl));

                /* make sure player has enough mana */
                if (mana > c.player.mana)
                {


                    event_ptr = null; // free((void*) event_ptr);


                    CFUNCTIONS.sprintf(ref string_buffer,
                       "You do not have the %.0lf mana that teleport would require.\n",
                       mana);


                    ioclass.Do_send_line(c, string_buffer);

                    ioclass.Do_more(c);

                    ioclass.Do_send_clear(c);
                    return;
                }
                else
                {


                    CFUNCTIONS.sprintf(ref string_buffer,
                      "That teleport will cost %.0lf mana.  Do you wish to cast it?\n",
                      mana);


                    ioclass.Do_send_line(c, string_buffer);

                    if (ioclass.Do_yes_no(c, ref answer) != phantdefs.S_NORM || answer == 1)
                    {

                        event_ptr = null; // free((void*) event_ptr);

                        ioclass.Do_send_clear(c);
                        return;
                    }
                }


                statsclass.Do_mana(c, -mana, 0);
                event_ptr.arg3 = phantdefs.A_TELEPORT;
            }




            eventclass.Do_handle_event(c, event_ptr);

            /* did the player use Gwaihir? */
            if ((the_event.arg2 != 0 ? true : false) == true)
            {

                /* is the destination a post? */
                if (event_ptr.arg1 == event_ptr.arg2)
                {

                    dtemp = CFUNCTIONS.sqrt(Mathf.Abs((float)event_ptr.arg1) / 100.0);

                    if (CFUNCTIONS.floor(dtemp) == dtemp)
                    {

                        /* bypass the monster check */
                        event2_ptr = eventclass.Do_create_event();
                        event2_ptr.type = (short)phantdefs.TRADING_EVENT;
                        eventclass.Do_handle_event(c, event2_ptr);
                    }
                }
            }

            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_holy_grail(struct client_t *c, struct event_t *the_event)
        /
        / FUNCTION: check if current player has been tampered with
        /
        / AUTHOR: E. A. Estes, 12/4/85
        /
        / ARGUMENTS: none
        /
        / RETURN VALUE: none
        /
        / MODULES CALLED: readrecord(), CLibFile.fread(ref ), fseek(), tampered(), writevoid()
        /
        / GLOBAL INPUTS: *Energyvoidfp, Other, Player, Fileloc, Enrgyvoid
        /
        / GLOBAL OUTPUTS: Enrgyvoid
        /
        / DESCRIPTION:
        /       Check for energy voids, holy grail, and tampering by other
        /       players.
        /
        *************************************************************************/

        internal void Do_holy_grail(client_t c, event_t the_event)
        {
            event_t event_ptr = new event_t();
            float ftemp = -1;

            ioclass.Do_send_line(c, "You have found The Holy Grail!!\n");

            /* must be council of wise to behold grail */
            if (c.player.special_type < phantdefs.SC_COUNCIL)
            {

                ioclass.Do_send_line(c,
                "However, you are not experienced enough to behold it.\n");


                ioclass.Do_more(c);

                statsclass.Do_sin(c, (double)c.player.sin);

                statsclass.Do_mana(c, 1000, 0);
            }
            else if (c.player.special_type == phantdefs.SC_VALAR)
            {

                ioclass.Do_send_line(c,
                "You have already made it to the position of Valar.\n");


                ioclass.Do_send_line(c, "The Grail is of no use to you now.\n");

                ioclass.Do_more(c);
            }
            else
            {

                ioclass.Do_send_line(c,
                   "It is now time to see if you are worthy to behold it. . .\n");


                socketclass.Do_send_buffer(c);
                CLibPThread.sleep(4);

                if ((macros.RND() / 4.0 < c.player.sin) ||
                    (CFUNCTIONS.strcmp(c.player.parent_account, c.lcaccount)))
                {

                    ioclass.Do_send_line(c, "You are unworthy!\n");

                    ioclass.Do_more(c);


                    statsclass.Do_energy(c, 1.0, 1.0 + c.player.shield, c.player.shield, 0.0, 0);


                    statsclass.Do_strength(c, 1.0, c.player.sword, 0.0, 0);

                    statsclass.Do_speed(c, 1.0, c.player.quicksilver, 0.0, 0);

                    statsclass.Do_mana(c, -(c.player.mana - 1), 0);

                    statsclass.Do_experience(c, -(c.player.experience), 0);

                    c.player.magiclvl =
                    c.player.brains = 0;

                    event_ptr = eventclass.Do_create_event();
                    event_ptr.type = (short)phantdefs.MOVE_EVENT;
                    event_ptr.arg1 = 1.0;
                    event_ptr.arg2 = 1.0;
                    event_ptr.arg3 = phantdefs.A_FORCED;
                    eventclass.Do_file_event(c, event_ptr);
                }
                else
                {

                    /* send yourself a valar event */
                    event_ptr = eventclass.Do_create_event();
                    event_ptr.type = (short)phantdefs.VALAR_EVENT;
                    eventclass.Do_file_event(c, event_ptr);


                    miscclass.Do_lock_mutex(c.realm.realm_lock);
                    if (c.realm.valar != null)
                    {
                        event_ptr = eventclass.Do_create_event();
                        event_ptr.type = (short)phantdefs.VALAR_EVENT;
                        event_ptr.to = c.realm.valar;
                        eventclass.Do_send_event(event_ptr);
                    }

                    miscclass.Do_unlock_mutex(c.realm.realm_lock);
                }
            }
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_help(struct client_t *c)
        /
        / FUNCTION: check if current player has been tampered with
        /
        / AUTHOR: E. A. Estes, 12/4/85
        /
        / ARGUMENTS: none
        /
        / RETURN VALUE: none
        /
        / MODULES CALLED: readrecord(), CLibFile.fread(ref ), fseek(), tampered(), writevoid()
        /
        / GLOBAL INPUTS: *Energyvoidfp, Other, Player, Fileloc, Enrgyvoid
        /
        / GLOBAL OUTPUTS: Enrgyvoid
        /
        / DESCRIPTION:
        /       Check for energy voids, holy grail, and tampering by other
        /       players.
        /
        *************************************************************************/

        internal void Do_help(client_t c)
        {

            ioclass.Do_send_line(c, "You move about in the realm, fighting monsters, and gaining treasure in order to gain levels.  Your first priority at this time is to stay alive.  Later goals may be to become king, and eventually to become Council of the Wise, and seek the holy grail.  When you leave, your game is automatically saved, but you cannot restore, once you die, you are dead and must re-roll a new char.\n");

            ioclass.Do_more(c);
            ioclass.Do_send_clear(c);

            ioclass.Do_send_line(c, "Fight by using melee or skirmish, or use your magic by choosing spells (although spells use mana).  Mana, your magical energy, can be restored by resting.  Plague and curses lower your energy and strength but can be cured by gurus and medics.  The medic will want about half of your gold, to cure you.  Amulets and charms protect from curses.  Players will encounter each other whenever they are at the same coordinates.\n");

            ioclass.Do_more(c);
            ioclass.Do_send_clear(c);

            ioclass.Do_send_line(c, "The first set of posts can be found at (100,100), (-100,100), (100,-100) and (-100,-100).  The second set are out at (+/- 400, +/- 400).  Posts sell things, and you get close by using the 'compass' keys on the bottom right side of the screen and get in to them by using the 'move to' key on the left side.  There, you can buy shields, swords, and additional sundries which make life more pleasant in the realm.  Higher the post, more it sells.\n");

            ioclass.Do_more(c);
            ioclass.Do_send_clear(c);

            ioclass.Do_send_line(c, "Some players are Game Wizards, with a W after their class.  Treat them with respect, for they administer the game and make sure it runs properly.  Do not pester Wizards to help you in your quest unless the game has performed incorrectly, as they tend to get extremely angry and tend to do nasty things to players that annoy them.  Above all, enjoy the game!\n");
            ioclass.Do_more(c);
            ioclass.Do_send_clear(c);

            /* don't penalize the player for asking for help */
            c.player.age--;
        }


    }
}
