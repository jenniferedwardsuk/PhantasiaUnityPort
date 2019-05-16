using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace phantasiaclasses
{
    public class info //: MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        static info Instance;
        private info()
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
            restoreclass = restore.GetInstance();
            itcombatclass = itcombat.GetInstance();
            fightclass = fight.GetInstance();
        }
        public static info GetInstance()
        {
            info instance = null;
            if (Instance != null)
            {
                instance = Instance;
            }
            else
            {
                instance = new info();
            }
            return instance;
        }

        /*
     * info.c       Routines to retriving information
     */

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
        phantasiaclasses.account accountclass;

        /************************************************************************
        /
        / FUNCTION NAME: struct examine_t *Do_create_examine(c)
        /
        / FUNCTION: return a char specifying player type
        /
        / AUTHOR: Brian Kelly, 01/04/01
        /
        / ARGUMENTS:
        /       struct client_t c - pointer to the main client strcture
        /
        / RETURN VALUE: 
        /	char - character the describes the character
        /
        / MODULES CALLED: CFUNCTIONS.strcpy(ref )
        /
        / DESCRIPTION:
        /       Return a string describing the player type.
        /       King, council, valar, supercedes other types.
        /       The first character of the string is '*' if the player
        /       has a crown.
        /       If 'shortflag' is true, return a 3 character string.
        /
        *************************************************************************/

        internal examine_t Do_create_examine(client_t c, game_t requestor)
        {
            examine_t examine_ptr = new examine_t();

            /* create the strcture */
            examine_ptr = new examine_t();// (examine_t)Do_malloc(phantdefs.SZ_EXAMINE);

            /* lock the realm for the next two calls */
            miscclass.Do_lock_mutex(c.realm.realm_lock);

            /* to titles and specific information */
            Do_make_character_title(c, c.game, ref examine_ptr.title);
            CFUNCTIONS.strcat(ref examine_ptr.title, "\n");

            /* copy over the coords or description */
            if (Do_show_character_coords(requestor, c.game))
            {


                CFUNCTIONS.sprintf(ref examine_ptr.location, "(%.0lf, %.0lf)\n", c.player.x,
                    c.player.y);
            }
            else
            {

                CFUNCTIONS.strcpy(ref examine_ptr.location, c.player.area);

                CFUNCTIONS.strcat(ref examine_ptr.location, "\n");
            }
            miscclass.Do_unlock_mutex(c.realm.realm_lock);

            /* determine if the network is printable */
            if (!c.addressResolved || c.wizard > 2)
            {
                CFUNCTIONS.strcpy(ref examine_ptr.network, "<unavailable>\n");
            }
            else
            {
                CFUNCTIONS.strcpy(ref examine_ptr.network, c.network);

                CFUNCTIONS.strcat(ref examine_ptr.network, "\n");
            }

            /* determine when the next level will occur */
            examine_ptr.nextLevel = 1800 * (c.player.level + 1) *
                (c.player.level + 1);

            /* format player gender */
            if (c.player.gender == (phantdefs.MALE != 0 ? true : false))
            {

                CFUNCTIONS.strcpy(ref examine_ptr.gender, "Male\n");
            }
            else
            {

                CFUNCTIONS.strcpy(ref examine_ptr.gender, "Female\n");
            }

            Do_format_time(ref examine_ptr.time_played, c.player.time_played +
                CFUNCTIONS.GetUnixEpoch(DateTime.Now) - c.player.last_load); //todo: replace with time()?

            if (c.wizard > 2)
            {
                CFUNCTIONS.strcpy(ref examine_ptr.account, "<unavailable>\n");
            }
            else if (!CFUNCTIONS.strcmp(c.account, "eyhung"))
            {
                CFUNCTIONS.strcpy(ref examine_ptr.account, c.player.name);
                CFUNCTIONS.strcat(ref examine_ptr.account, "\n");
            }
            else
            {
                CFUNCTIONS.strcpy(ref examine_ptr.account, c.account);
                CFUNCTIONS.strcat(ref examine_ptr.account, "\n");
            }

            Do_true_false(ref examine_ptr.cloaked, c.player.cloaked);
            Do_true_false(ref examine_ptr.blind, c.player.blind);
            Do_true_false(ref examine_ptr.virgin, c.player.virgin);
            Do_true_false(ref examine_ptr.palantir, c.player.palantir);
            Do_true_false(ref examine_ptr.blessing, c.player.blessing);
            Do_true_false(ref examine_ptr.ring, (c.player.ring_type != 0 ? true : false));

            CFUNCTIONS.ctime_r(c.player.last_load, ref examine_ptr.date_loaded);
            CFUNCTIONS.ctime_r(c.player.date_created, ref examine_ptr.date_created);

            examine_ptr.channel = (short)c.channel;
            examine_ptr.level = c.player.level;
            examine_ptr.experience = c.player.experience;
            examine_ptr.energy = c.player.energy;
            examine_ptr.max_energy = c.player.max_energy;
            examine_ptr.strength = c.player.strength *
                                    (1 + CFUNCTIONS.sqrt(c.player.sword) * phantdefs.N_SWORDPOWER);
            examine_ptr.max_strength = c.player.max_strength;
            examine_ptr.quickness = c.player.quickness;
            examine_ptr.max_quickness = c.player.max_quickness;
            examine_ptr.mana = c.player.mana;
            examine_ptr.brains = c.player.brains;
            examine_ptr.magiclvl = c.player.magiclvl;
            examine_ptr.poison = c.player.poison;
            examine_ptr.sin = c.player.sin;
            examine_ptr.lives = c.player.lives;
            examine_ptr.gold = c.player.gold;
            examine_ptr.gems = c.player.gems;
            examine_ptr.sword = c.player.sword;
            examine_ptr.shield = c.player.shield;
            examine_ptr.quicksilver = c.player.quicksilver;
            examine_ptr.holywater = c.player.holywater;
            examine_ptr.amulets = c.player.amulets;
            examine_ptr.charms = c.player.charms;
            examine_ptr.crowns = c.player.crowns;
            examine_ptr.age = c.player.age;
            examine_ptr.degenerated = c.player.degenerated;

            return examine_ptr;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_make_character_title(struct client_t *c, struct game_t *game_ptr, char *theTitle)
        /
        / FUNCTION: find location in player file of given name
        /
        / AUTHOR: Brian Kelly, 01/04/01
        /
        / ARGUMENTS:
        /       char *name - name of character to look for
        /       struct player *playerp - pointer of structure to fill
        /
        / RETURN VALUE: location of player if found, -1 otherwise
        /
        / MODULES CALLED: CLibFile.fread(ref ), fseek(), cfunctions.strcmp()
        /
        / GLOBAL INPUTS: Wizard, *Playersfp
        /
        / GLOBAL OUTPUTS: none
        /
        / DESCRIPTION:
        /       Search the player file for the player of the given name.
        /       If player is found, fill structure with player data.
        /
        *************************************************************************/

        internal void Do_make_character_title(client_t c, game_t game_ptr, ref string theTitle)
        {
            string title = "";// new char[20];

            /* make sure the realm is locked before calling this function */

            title = "\0";

            if (game_ptr.description.wizard > 2)
            {
                CFUNCTIONS.strcpy(ref title, "Wizard ");
            }
            else if (game_ptr.description.wizard == 2)
            {
                CFUNCTIONS.strcpy(ref title, "Apprentice ");
            }


            switch (game_ptr.description.special_type)
            {

                case phantdefs.SC_KNIGHT:

                    if (game_ptr.description.gender == (phantdefs.MALE != 0 ? true : false))
                    {

                        CFUNCTIONS.strcpy(ref title, "Sir ");
                    }
                    else
                    {

                        CFUNCTIONS.strcpy(ref title, "Dame ");
                    }
                    break;

                case phantdefs.SC_STEWARD:


                    CFUNCTIONS.strcpy(ref title, "Steward ");
                    break;

                case phantdefs.SC_KING:

                    if (game_ptr.description.gender == (phantdefs.MALE != 0 ? true : false))
                    {

                        CFUNCTIONS.strcpy(ref title, "King ");
                    }
                    else
                    {

                        CFUNCTIONS.strcpy(ref title, "Queen ");
                    }
                    break;

                case phantdefs.SC_COUNCIL:
                case phantdefs.SC_EXVALAR:

                    if (game_ptr.description.gender == (phantdefs.MALE != 0 ? true : false))
                    {

                        CFUNCTIONS.strcpy(ref title, "Councilman ");
                    }
                    else
                    {

                        CFUNCTIONS.strcpy(ref title, "Councilwoman ");
                    }
                    break;

                case phantdefs.SC_VALAR:


                    CFUNCTIONS.strcpy(ref title, "Valar ");
                    break;
            }

            CFUNCTIONS.sprintf(ref theTitle, "%s%s the %s", title, game_ptr.description.name,
                c.realm.charstats[game_ptr.description.type].class_name);

            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_show_character_coords(struct game_t *requestor, struct game *requestee)
        /
        / FUNCTION: find location in player file of given name
        /
        / AUTHOR: Brian Kelly, 01/04/01
        /
        / ARGUMENTS:
        /       char *name - name of character to look for
        /       struct player *playerp - pointer of structure to fill
        /
        / RETURN VALUE: location of player if found, -1 otherwise
        /
        / MODULES CALLED: CLibFile.fread(ref ), fseek(), cfunctions.strcmp()
        /
        / GLOBAL INPUTS: Wizard, *Playersfp
        /
        / GLOBAL OUTPUTS: none
        /
        / DESCRIPTION:
        /       Search the player file for the player of the given name.
        /       If player is found, fill structure with player data.
        /
        *************************************************************************/

        bool Do_show_character_coords(game_t requestor, game_t requestee)
        {

            /* the realm should be locked before calling this procedure */

            /* Show name if in place like Valhalla */
            if (requestee.useLocationName)
            {
                return false;
            }

            /* Show coords if requestee is requestor */
            if (requestee == requestor)
            {
                return true;
            }

            /* Show name if requestor descriptionless */
            if (requestor.description == null)
            {
                return false;
            }

            /* Show name if requestee is virtual */
            if (requestee.virtualvirtual)
            {
                return false;
            }

            /* Show coords if the requestor is a game wizard */
            if (requestor.description.wizard > 2)
            {
                return true;
            }

            /* Show name if requestor is blind */
            if (requestor.description.blind)
            {
                return false;
            }

            /* Show coords if requestor has a palantir */
            if (requestor.description.palantir)
            {
                return true;
            }

            /* Show name if requestee is not special and outside requestor */
            if (requestee.description.special_type == phantdefs.SC_NONE &&
                requestee.circle > requestor.circle)
            {

                return false;
            }

            /* Show name if requestee is special and far out */
            if (requestee.description.special_type != phantdefs.SC_NONE &&
                requestee.circle > 400)
            {
                return false;
            }

            /* Show name if requestee is cloaked */
            if (requestee.description.cloaked)
            {
                return false;
            }

            /* Show name if requestor is experimento and low-level */
            if ((requestor.description.type == phantdefs.C_EXPER) &&
                (requestor.description.level < 50))
            {
                return false;
            }

            /* Otherwise show coords (Got all that?) */
            return true;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_format_time(struct client_t *c, char *theString, int theTime)
        /
        / FUNCTION: find location in player file of given name
        /
        / AUTHOR: Brian Kelly, 01/04/01
        /
        / ARGUMENTS:
        /       char *name - name of character to look for
        /       struct player *playerp - pointer of structure to fill
        /
        / RETURN VALUE: location of player if found, -1 otherwise
        /
        / MODULES CALLED: CLibFile.fread(ref ), fseek(), cfunctions.strcmp()
        /
        / GLOBAL INPUTS: Wizard, *Playersfp
        /
        / GLOBAL OUTPUTS: none
        /
        / DESCRIPTION:
        /       Search the player file for the player of the given name.
        /       If player is found, fill structure with player data.
        /
        *************************************************************************/

        void Do_format_time(ref string theString, int theTime)
        {
            int minutes, hours;

            hours = theTime / 3600;
            theTime -= hours * 3600;

            minutes = theTime / 60;
            theTime -= minutes * 60;

            CFUNCTIONS.sprintf(ref theString, "%2.2d:%2.2d:%2.2d\n", hours, minutes, theTime);

            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_true_false(char *theString, int theBool)
        /
        / FUNCTION: find location in player file of given name
        /
        / AUTHOR: Brian Kelly, 01/04/01
        /
        / ARGUMENTS:
        /       char *name - name of character to look for
        /       struct player *playerp - pointer of structure to fill
        /
        / RETURN VALUE: location of player if found, -1 otherwise
        /
        / MODULES CALLED: CLibFile.fread(ref ), fseek(), cfunctions.strcmp()
        /
        / GLOBAL INPUTS: Wizard, *Playersfp
        /
        / GLOBAL OUTPUTS: none
        /
        / DESCRIPTION:
        /       Search the player file for the player of the given name.
        /       If player is found, fill structure with player data.
        /
        *************************************************************************/

        void Do_true_false(ref string theString, bool theBool)
        {
            if (theBool)
            {
                CFUNCTIONS.strcpy(ref theString, "Yes\n");
            }
            else
            {
                CFUNCTIONS.strcpy(ref theString, "No\n");
            }
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_examine_character(c, struct examine_t *theInfo)
        /
        / FUNCTION: return a char specifying player type
        /
        / AUTHOR: Brian Kelly, 01/04/01
        /
        / ARGUMENTS:
        /       struct client_t c - pointer to the main client strcture
        /
        / RETURN VALUE: 
        /	char - character the describes the character
        /
        / MODULES CALLED: CFUNCTIONS.strcpy(ref )
        /
        / DESCRIPTION:
        /       Return a string describing the player type.
        /       King, council, valar, supercedes other types.
        /       The first character of the string is '*' if the player
        /       has a crown.
        /       If 'shortflag' is true, return a 3 character string.
        /
        *************************************************************************/

        internal void Do_examine_character(client_t c, examine_t theInfo)
        {

            socketclass.Do_send_int(c, phantdefs.PLAYER_INFO_PACKET);

            socketclass.Do_send_string(c, theInfo.title); //todo: this was null in testing
            socketclass.Do_send_string(c, theInfo.location);

            socketclass.Do_send_string(c, theInfo.account);
            socketclass.Do_send_string(c, theInfo.network);
            socketclass.Do_send_int(c, theInfo.channel);

            socketclass.Do_send_double(c, theInfo.level);
            socketclass.Do_send_double(c, theInfo.experience);
            socketclass.Do_send_double(c, theInfo.nextLevel);

            socketclass.Do_send_double(c, theInfo.energy);
            socketclass.Do_send_double(c, theInfo.max_energy);
            socketclass.Do_send_double(c, theInfo.shield);
            socketclass.Do_send_double(c, theInfo.strength);
            socketclass.Do_send_double(c, theInfo.max_strength);
            socketclass.Do_send_double(c, theInfo.sword);
            socketclass.Do_send_float(c, theInfo.quickness);
            socketclass.Do_send_float(c, theInfo.max_quickness);
            socketclass.Do_send_float(c, theInfo.quicksilver);
            socketclass.Do_send_double(c, theInfo.brains);
            socketclass.Do_send_double(c, theInfo.magiclvl);
            socketclass.Do_send_double(c, theInfo.mana);
            socketclass.Do_send_string(c, theInfo.gender);
            socketclass.Do_send_fpfloat(c, theInfo.poison);
            socketclass.Do_send_fpfloat(c, theInfo.sin);
            socketclass.Do_send_int(c, theInfo.lives);

            socketclass.Do_send_double(c, theInfo.gold);
            socketclass.Do_send_double(c, theInfo.gems);
            socketclass.Do_send_int(c, theInfo.holywater);
            socketclass.Do_send_int(c, theInfo.amulets);
            socketclass.Do_send_int(c, theInfo.charms);
            socketclass.Do_send_int(c, theInfo.crowns);
            socketclass.Do_send_string(c, theInfo.virgin);
            socketclass.Do_send_string(c, theInfo.blessing);
            socketclass.Do_send_string(c, theInfo.palantir);
            socketclass.Do_send_string(c, theInfo.ring);

            socketclass.Do_send_string(c, theInfo.cloaked);
            socketclass.Do_send_string(c, theInfo.blind);
            socketclass.Do_send_int(c, theInfo.age);
            socketclass.Do_send_int(c, theInfo.degenerated);
            socketclass.Do_send_string(c, theInfo.time_played);
            socketclass.Do_send_string(c, theInfo.date_loaded);
            socketclass.Do_send_string(c, theInfo.date_created);

            return;
        }



        /************************************************************************
        /
        / FUNCTION NAME: struct details_t *infoclass.Do_create_detail(c)
        /
        / FUNCTION: return a char specifying player type
        /
        / AUTHOR: Brian Kelly, 01/16/01
        /
        / ARGUMENTS:
        /       struct client_t c - pointer to the main client strcture
        /
        / RETURN VALUE: 
        /	char - character the describes the character
        /
        / MODULES CALLED: CFUNCTIONS.strcpy(ref )
        /
        / DESCRIPTION:
        /       Return a string describing the player type.
        /       King, council, valar, supercedes other types.
        /       The first character of the string is '*' if the player
        /       has a crown.
        /       If 'shortflag' is true, return a 3 character string.
        /
        *************************************************************************/

        internal detail_t Do_create_detail(client_t c)
        {
            detail_t detail_ptr = new detail_t();
            account_t theAccount = new account_t();

            /* create the strcture */
            detail_ptr = new detail_t();// (detail_t)Do_malloc(phantdefs.SZ_DETAIL);

            /* character information */
            CFUNCTIONS.strcpy(ref detail_ptr.modifiedName, c.modifiedName);
            CFUNCTIONS.strcat(ref detail_ptr.modifiedName, "\n");
            CFUNCTIONS.strcpy(ref detail_ptr.name, c.player.name);
            CFUNCTIONS.strcat(ref detail_ptr.name, "\n");
            Do_true_false(ref detail_ptr.faithful, c.player.faithful);
            CFUNCTIONS.strcpy(ref detail_ptr.parentAccount, c.player.parent_account);
            CFUNCTIONS.strcat(ref detail_ptr.parentAccount, "\n");
            CFUNCTIONS.strcpy(ref detail_ptr.charParentNetwork, c.player.parent_network);
            CFUNCTIONS.strcat(ref detail_ptr.charParentNetwork, "\n");
            accountclass.Do_look_account(c, (c.account), ref theAccount);
            detail_ptr.playerMutes = theAccount.muteCount;

            /* account information */
            CFUNCTIONS.strcpy(ref detail_ptr.account, c.account);
            CFUNCTIONS.strcat(ref detail_ptr.account, "\n");
            CFUNCTIONS.strcpy(ref detail_ptr.email, c.email);
            CFUNCTIONS.strcat(ref detail_ptr.email, "\n");
            CFUNCTIONS.strcpy(ref detail_ptr.accParentNetwork, c.parentNetwork);
            CFUNCTIONS.strcat(ref detail_ptr.accParentNetwork, "\n");

            /* connection information */
            CFUNCTIONS.strcpy(ref detail_ptr.IP, c.IP);
            CFUNCTIONS.strcat(ref detail_ptr.IP, "\n");
            CFUNCTIONS.strcpy(ref detail_ptr.network, c.network);
            CFUNCTIONS.strcat(ref detail_ptr.network, "\n");
            detail_ptr.machineID = (int)c.machineID;
            CFUNCTIONS.ctime_r(c.date_connected, ref detail_ptr.dateConnected);

            return detail_ptr;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_detail_connection(c, struct detail_t *theInfo)
        /
        / FUNCTION: return a char specifying player type
        /
        / AUTHOR: Brian Kelly, 01/16/01
        /
        / ARGUMENTS:
        /       struct client_t c - pointer to the main client strcture
        /
        / RETURN VALUE: 
        /	char - character the describes the character
        /
        / MODULES CALLED: CFUNCTIONS.strcpy(ref )
        /
        / DESCRIPTION:
        /       Return a string describing the player type.
        /       King, council, valar, supercedes other types.
        /       The first character of the string is '*' if the player
        /       has a crown.
        /       If 'shortflag' is true, return a 3 character string.
        /
        *************************************************************************/

        internal void Do_detail_connection(client_t c, detail_t theInfo)
        {

            socketclass.Do_send_int(c, phantdefs.CONNECTION_DETAIL_PACKET);

            socketclass.Do_send_string(c, theInfo.modifiedName);
            socketclass.Do_send_string(c, theInfo.name);
            socketclass.Do_send_string(c, theInfo.faithful);
            socketclass.Do_send_string(c, theInfo.parentAccount);
            socketclass.Do_send_string(c, theInfo.charParentNetwork);
            socketclass.Do_send_int(c, theInfo.playerMutes);

            socketclass.Do_send_string(c, theInfo.account);
            socketclass.Do_send_string(c, theInfo.email);
            socketclass.Do_send_string(c, theInfo.accParentNetwork);

            socketclass.Do_send_string(c, theInfo.IP);
            socketclass.Do_send_string(c, theInfo.network);
            socketclass.Do_send_int(c, theInfo.machineID);
            socketclass.Do_send_string(c, theInfo.dateConnected);

            return;
        }


        /***************************************************************************
        / FUNCTION NAME: Do_last_load_info(struct client_t *c)
        /
        / FUNCTION: Default activity when no events are pending
        /
        / AUTHOR:  Brian Kelly, 01/04/01
        /
        / ARGUMENTS: 
        /	struct client_t *c - structure containing all thread info
        /
        / RETURN VALUE: none
        /
        / DESCRIPTION:
        /       Process arguments, initialize program, and loop forever processing
        /       player input.
        /
        ****************************************************************************/

        internal void Do_last_load_info(client_t c)
        {
            string string_buffer = ""; //[phantdefs.SZ_LINE];
            string string_buffer2 = ""; //[phantdefs.SZ_LINE];
            long answer = -1;
            int exceptionFlag = 0, wizType = 0;
            string wizNetwork = ""; //[phantdefs.SZ_FROM], 
            string wizAccount = ""; //[phantdefs.SZ_NAME], 
            string wizCharacter = ""; //[phantdefs.SZ_NAME];
            CLibFile.CFILE wizard_file;

            /* quick character report */
            if (c.player.bad_passwords != 0)
            {

                string_buffer = CFUNCTIONS.sprintfSinglestring("There have been %d failed login attempts with this character since last successful login.\n", c.player.bad_passwords);

                ioclass.Do_send_line(c, string_buffer);
            }

            if (c.player.load_count == 0)
            {

                string_buffer = CFUNCTIONS.sprintfSinglestring("Welcome %s.  If this is your first journey into the realm, there is a good quick overview under the \"Help\" button that will appear after you hit \"More\".\n", c.player.name);

            }
            else
            {

                /* convert the last time and remove the "\n". */
                CFUNCTIONS.ctime_r(c.player.last_load, ref string_buffer2);
                string_buffer2 = string_buffer2.Substring(0, string_buffer2.Length - 2);//string_buffer2[cfunctions.strlen(string_buffer2) - 1] = '\0';

                CFUNCTIONS.sprintf(ref string_buffer,
                "Last login from \"%s\" with account \"%s\" at %s.\n",
                c.player.last_IP, c.player.last_account, string_buffer2);
            }

            ioclass.Do_send_line(c, string_buffer);

            string_buffer = CFUNCTIONS.sprintfSinglestring("You are currently playing from IP \"%s\".\n", c.IP); //added IP for clarity since current default is just 0
            ioclass.Do_send_line(c, string_buffer);

            /* old info is out, so the new comes in */

            /* open the wizard file to see if this person is one */
            int errno = 0;
            if ((wizard_file = CLibFile.fopen(pathnames.WIZARD_FILE, "r", ref errno)) == null)
            {

                CFUNCTIONS.sprintf(ref string_buffer,
                    "[%s] CLibFile.fopen of %s failed in Do_last_load_info : %s\n",
                    c.connection_id, pathnames.WIZARD_FILE, CFUNCTIONS.strerror(errno));

                fileclass.Do_log_error(string_buffer);
            }
            else
            {

                /* loop through the wizard names */
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
                    if (!CFUNCTIONS.strcmp(wizAccount, c.lcaccount) &&
                            !CFUNCTIONS.strcmp(wizNetwork, c.network))
                    {

                        if (answer > wizType)
                        {
                            wizType = (int)answer;
                        }
                    }
                }
                CLibFile.fclose(wizard_file);
            }

            if ((wizType > 2) && CFUNCTIONS.strcmp(c.player.last_account, c.account)
                && (c.player.load_count != 0))
            {

                ioclass.Do_send_line(c, "Using backdoor, not copying info.\n");
                CFUNCTIONS.strcpy(ref c.wizaccount, c.account);
                CFUNCTIONS.strcpy(ref c.account, c.player.last_account);
                CFUNCTIONS.strcpy(ref c.wizIP, c.IP);
                CFUNCTIONS.strcpy(ref c.IP, c.player.last_IP);

            }
            else
            {
                CFUNCTIONS.strcpy(ref c.player.last_IP, c.IP);
                CFUNCTIONS.strcpy(ref c.player.last_account, c.account);
                c.player.last_load = CFUNCTIONS.GetUnixEpoch(DateTime.Now);
                c.player.bad_passwords = 0;
                ++c.player.load_count;

            }

            return;
        }

        /************************************************************************
        /
        / FUNCTION NAME: Do_list_characters(struct client_t *c)
        /
        / FUNCTION: do trading post stuff
        /
        / AUTHOR: E. A. Estes, 2/2/86
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

        internal void Do_list_characters(client_t c)
        {
            game_t game_ptr;
            string string_buffer = ""; //[phantdefs.SZ_LINE], 
            string title = ""; //[phantdefs.SZ_LINE];
            string location = ""; //[phantdefs.SZ_LINE], 
            string network = ""; //[phantdefs.SZ_LINE];
            int numusers = 0;   /* number of users on file */

            /* print a header */
            ioclass.Do_send_line(c, "Character Title, Location, Level, Network\n");

            /* lock the realm */
            miscclass.Do_lock_mutex(c.realm.realm_lock);

            /* run through all the games */
            game_ptr = c.realm.games;
            while (game_ptr != null)
            {

                /* if the player is playing */
                if (game_ptr.description != null)
                {

                    /* get the character's title */
                    Do_make_character_title(c, game_ptr, ref title);

                    /* make the location */
                    if (Do_show_character_coords(c.game, game_ptr))
                    {

                        CFUNCTIONS.sprintf(ref location, "(%.0lf, %.0lf)", game_ptr.x, game_ptr.y);
                    }
                    else
                    {

                        CFUNCTIONS.strcpy(ref location, game_ptr.area);
                    }

                    /* choose the network display */
                    if (game_ptr.network == null || game_ptr.network[0] == '\0' ||
                        game_ptr.description.wizard > 2)
                    {


                        CFUNCTIONS.strcpy(ref network, "<unavailable>");
                    }
                    else
                    {

                        CFUNCTIONS.strcpy(ref network, game_ptr.network);
                    }

                    /* put everything together */
                    string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, %.0lf, %s\n", title, location,
                        game_ptr.description.level, network);

                    numusers++;

                    ioclass.Do_send_line(c, string_buffer);
                }
                game_ptr = game_ptr.next_game;
            }

            miscclass.Do_unlock_mutex(c.realm.realm_lock);

            ioclass.Do_send_line(c, "\n");
            string_buffer = CFUNCTIONS.sprintfSinglestring("There are currently %d players.\n", numusers);
            ioclass.Do_send_line(c, string_buffer);
            ioclass.Do_more(c);
            ioclass.Do_send_clear(c);
            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_list_connections(struct client_t *c)
        /
        / FUNCTION: do trading post stuff
        /
        / AUTHOR: Brian Kelly, 01/16/01
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

        void Do_list_connections(client_t c)
        {
            game_t game_ptr;
            string string_buffer = ""; //[phantdefs.SZ_LINE];
            string character = ""; //[phantdefs.SZ_NAME], 
            string account = ""; //[phantdefs.SZ_NAME];
            int numusers = 0;   /* number of users on file */

            /* print a header */
            ioclass.Do_send_line(c, "Character, Account, Address, Machine\n");

            /* lock the realm */
            miscclass.Do_lock_mutex(c.realm.realm_lock);

            /* run through all the games */
            game_ptr = c.realm.games;
            while (game_ptr != null)
            {

                /* if the player is playing */
                if (game_ptr.description == null)
                {

                    CFUNCTIONS.strcpy(ref character, "<no character>");
                }
                else
                {

                    CFUNCTIONS.strcpy(ref character, game_ptr.description.name);
                }

                /* if the player has an account */
                if (game_ptr.account == null || game_ptr.account[0] == '\0')
                {

                    CFUNCTIONS.strcpy(ref account, "<no account>");
                }
                else
                {

                    CFUNCTIONS.strcpy(ref account, game_ptr.account);
                }

                /* put everything together */
                string_buffer = CFUNCTIONS.sprintfSinglestring("%s, %s, %s, %ld\n", character,
                    account, game_ptr.IP, game_ptr.machineID);

                numusers++;

                ioclass.Do_send_line(c, string_buffer);

                game_ptr = game_ptr.next_game;
            }

            miscclass.Do_unlock_mutex(c.realm.realm_lock);

            ioclass.Do_send_line(c, "\n");
            string_buffer = CFUNCTIONS.sprintfSinglestring("There are currently %d connections.\n", numusers);
            ioclass.Do_send_line(c, string_buffer);
            ioclass.Do_more(c);
            ioclass.Do_send_clear(c);
            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_save_history(struct client_t *c, struct history_t *theHistory)
        /
        / FUNCTION: do trading post stuff
        /
        / AUTHOR: E. A. Estes, 2/2/86
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

        internal void Do_save_history(client_t c, history_t theHistory)
        {
            string string_buffer = ""; //[phantdefs.SZ_LINE];
            CLibFile.CFILE history_file;

            miscclass.Do_lock_mutex(c.realm.history_file_lock);

            /* open the history file for writing */
            int errno = 0;
            if ((history_file = CLibFile.fopen(pathnames.HISTORY_FILE, "a", ref errno)) == null)
            {

                miscclass.Do_unlock_mutex(c.realm.history_file_lock);
                CFUNCTIONS.sprintf(ref string_buffer,
                        "[%s] CLibFile.fopen of %s failed in Do_save_history: %s.\n",
                        c.connection_id, pathnames.HISTORY_FILE, CFUNCTIONS.strerror(errno));

                fileclass.Do_log_error(string_buffer);
                return;
            }

            /* write the new account at the end */
            if (CLibFile.fwrite(theHistory, ref phantdefs.SZ_HISTORY, 1, history_file) != 1)
            {

                CLibFile.fclose(history_file);
                miscclass.Do_unlock_mutex(c.realm.history_file_lock);
                CFUNCTIONS.sprintf(ref string_buffer,
                        "[%s] CLibFile.fwrite of %s failed in Do_save_history: %s.\n",
                        c.connection_id, pathnames.HISTORY_FILE, CFUNCTIONS.strerror(errno));

                fileclass.Do_log_error(string_buffer);
                return;
            }

            /* close the file */
            CLibFile.fclose(history_file);
            miscclass.Do_unlock_mutex(c.realm.history_file_lock);
            return;
        }



        /************************************************************************
        /
        / FUNCTION NAME: struct history_list_t *Do_look_history(struct client_t *c, int theType, char *theName)
        /
        / FUNCTION: do trading post stuff
        /
        / AUTHOR: Brian Kelly, 01/15/01
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

        history_list_t Do_look_history(client_t c, int historyType, string historyName)
        {
            history_list_t returnList = new history_list_t();
            history_list_t list_ptr = new history_list_t();
            history_t theHistory = new history_t();
            string string_buffer = ""; //[phantdefs.SZ_LINE];
            CLibFile.CFILE history_file;

            returnList = null;

            miscclass.Do_lock_mutex(c.realm.history_file_lock);

            /* open the history file for reading */
            int errno = 0;
            if ((history_file = CLibFile.fopen(pathnames.HISTORY_FILE, "r", ref errno)) == null)
            {

                miscclass.Do_unlock_mutex(c.realm.history_file_lock);
                CFUNCTIONS.sprintf(ref string_buffer,
                        "[%s] CLibFile.fopen of %s failed in Do_look_history: %s.\n",
                        c.connection_id, pathnames.HISTORY_FILE, CFUNCTIONS.strerror(errno));

                fileclass.Do_log_error(string_buffer);
                return returnList;
            }

            /* run through the history entries */
            while (CLibFile.fread(ref theHistory, phantdefs.SZ_HISTORY, 1, history_file) == 1)
            {

                /* if this this is one of what we want */
                if (theHistory.type == historyType &&
                !CFUNCTIONS.strcmp(theHistory.name, historyName))
                {

                    /* copy the information over */
                    list_ptr = new history_list_t();// (history_list_t) Do_malloc(phantdefs.SZ_HISTORY_LIST);

                    list_ptr.theHistory = theHistory;//, phantdefs.SZ_HISTORY);

                    /* put the element into the list */
                    list_ptr.next = returnList;
                    returnList = list_ptr;
                }
            }

            /* close the file */
            CLibFile.fclose(history_file);
            miscclass.Do_unlock_mutex(c.realm.history_file_lock);
            return returnList;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_show_history(struct client_t *c, struct history_list_t *theList)
        /
        / FUNCTION: do trading post stuff
        /
        / AUTHOR: Brian Kelly, 01/15/01
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

        void Do_show_history(client_t c, history_list_t theList)
        {
            history_list_t list_ptr;
            string string_buffer = ""; //[phantdefs.SZ_LINE], 
            char[] error_msg = new char[phantdefs.SZ_ERROR_MESSAGE]; //[phantdefs.SZ_ERROR_MESSAGE];

            string[] taggedTypes = { "address", "account" };

            if (theList == null)
            {

                ioclass.Do_send_line(c, "No history found.\n");
            }

            while (theList != null)
            {

                /* get the time of the event */
                string err = new string(error_msg);
                CFUNCTIONS.ctime_r(theList.theHistory.date, ref err);
                error_msg[CFUNCTIONS.strlen(new string(error_msg)) - 1] = '\0';

                /* format the message */
                string_buffer = CFUNCTIONS.sprintfSinglestring("%s - %s %s: %s", error_msg,
                    taggedTypes[theList.theHistory.type],
                    theList.theHistory.name, theList.theHistory.description);

                /* print the message */
                ioclass.Do_send_line(c, string_buffer);

                /* increment to the next in list */
                list_ptr = theList.next;

                theList = null; // free((void*) theList);
                theList = list_ptr;
            }
        }


        /************************************************************************
        /
        / FUNCTION NAME: infoclass.Do_wizard_information(struct client_t *c)
        /
        / FUNCTION: do random stuff
        /
        / AUTHOR: Brian Kelly, 01/16/01
        /
        / ARGUMENTS: none
        /
        / RETURN VALUE: none
        /
        / MODULES CALLED: collecttaxes(), cfunctions.floor(), wmove(), drandom(), infloat(),
        /       waddstr(), mvprintw(), getanswer()
        /
        / GLOBAL INPUTS: Player, *stdscr, *Statptr
        /
        / GLOBAL OUTPUTS: Player
        /
        / DESCRIPTION:
        /       Handle gurus, medics, etc.
        /
        *************************************************************************/

        internal void Do_wizard_information(client_t c)
        {
            event_t event_ptr = new event_t();
            button_t buttons = new button_t();
            realm_object_t object_ptr = new realm_object_t();
            realm_object_t grail_ptr = new realm_object_t();
            realm_object_t trove_ptr = new realm_object_t();
            long answer = -1;
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            string string_buffer = ""; //[phantdefs.SZ_LINE], 
            string playerName = ""; //[phantdefs.SZ_NAME];

            CFUNCTIONS.strcpy(ref buttons.button[0], "Details\n");
            CFUNCTIONS.strcpy(ref buttons.button[1], "Connections\n");
            ioclass.Do_clear_buttons(buttons, 2);
            CFUNCTIONS.strcpy(ref buttons.button[3], "Channel 9\n");
            CFUNCTIONS.strcpy(ref buttons.button[5], "Secrets\n");
            CFUNCTIONS.strcpy(ref buttons.button[7], "Cancel\n");

            miscclass.Do_lock_mutex(c.realm.realm_lock);
            if (c.game.hearAllChannels == phantdefs.HEAR_ALL)
            {
                CFUNCTIONS.strcpy(ref buttons.button[4], "Tune Out\n");
            }
            else
            {
                CFUNCTIONS.strcpy(ref buttons.button[4], "Hear All\n");
            }
            miscclass.Do_unlock_mutex(c.realm.realm_lock);

            if (ioclass.Do_buttons(c, ref answer, buttons) != phantdefs.S_NORM || answer == 7)
            {
                return;
            }

            switch (answer)
            {

                /* see this player's backstage information */
                case 0:
                    event_ptr = eventclass.Do_create_event();
                    event_ptr.to = c.game;
                    event_ptr.from = c.game;
                    event_ptr.type = (short)phantdefs.REQUEST_DETAIL_EVENT;

                    /* get the name of the player to look at */
                    if (ioclass.Do_player_dialog(c, "Which character do you want details on?\n",
                            playerName) != phantdefs.S_NORM)
                    {

                        event_ptr = null; // free((void*) event_ptr);
                        return;
                    }

                    if (eventclass.Do_send_character_event(c, event_ptr, playerName) == 0)
                    {
                        event_ptr = null; // free((void*) event_ptr);
                        ioclass.Do_send_line(c, "That character just left the game.\n");
                        ioclass.Do_more(c);

                        ioclass.Do_send_clear(c);
                        return;
                    }

                    return;

                case 1:

                    Do_list_connections(c);
                    return;

                case 3:

                    c.channel = 9;
                    miscclass.Do_lock_mutex(c.realm.realm_lock);
                    characterclass.Do_player_description(c);
                    miscclass.Do_unlock_mutex(c.realm.realm_lock);
                    characterclass.Do_send_specification(c, phantdefs.CHANGE_PLAYER_EVENT);
                    return;

                case 4:
                    miscclass.Do_lock_mutex(c.realm.realm_lock);
                    if (c.game.hearAllChannels == phantdefs.HEAR_ALL)
                    {
                        c.game.hearAllChannels = phantdefs.HEAR_SELF;
                    }
                    else
                    {
                        c.game.hearAllChannels = phantdefs.HEAR_ALL;
                    }
                    miscclass.Do_unlock_mutex(c.realm.realm_lock);
                    return;

                case 5:
                    grail_ptr = null;
                    trove_ptr = null;
                    miscclass.Do_lock_mutex(c.realm.realm_lock);
                    object_ptr = c.realm.objects;

                    while (object_ptr != null)
                    {

                        if (object_ptr.type == phantdefs.HOLY_GRAIL)
                        {

                            if (grail_ptr != null)
                            {


                                error_msg = CFUNCTIONS.sprintfSinglestring(
                    "[%s] Duplicate grails found in realm objects in infoclass.Do_wizard_information.\n",
                    c.connection_id);


                                fileclass.Do_log_error(error_msg);

                                ioclass.Do_send_line(c, "Extra grail found in the realm!\n");
                            }
                            else
                            {
                                grail_ptr = object_ptr;
                            }
                        }
                        else if (object_ptr.type == phantdefs.TREASURE_TROVE)
                        {

                            if (trove_ptr != null)
                            {


                                error_msg = CFUNCTIONS.sprintfSinglestring(
                    "[%s] Duplicate troves found in realm objects in infoclass.Do_wizard_information.\n",
                    c.connection_id);


                                fileclass.Do_log_error(error_msg);

                                ioclass.Do_send_line(c, "Extra trove found in the realm!\n");
                            }
                            else
                            {
                                trove_ptr = object_ptr;
                            }
                        }

                        object_ptr = object_ptr.next_object;
                    }

                    /* indicate the grail location */
                    if (grail_ptr == null)
                    {


                        error_msg = CFUNCTIONS.sprintfSinglestring(
                        "[%s] No grail found in realm objects in infoclass.Do_wizard_information.\n",
                        c.connection_id);


                        fileclass.Do_log_error(error_msg);

                        ioclass.Do_send_line(c, "Holy Grail: ** NOT FOUND **\n");
                    }
                    else
                    {


                        string_buffer = CFUNCTIONS.sprintfSinglestring("Holy Grail: %.0lf, %.0lf\n", grail_ptr.x,
                            grail_ptr.y);


                        ioclass.Do_send_line(c, string_buffer);
                    }

                    /* indicate the trove location */
                    if (trove_ptr == null)
                    {


                        error_msg = CFUNCTIONS.sprintfSinglestring(
                        "[%s] No trove found in realm objects in infoclass.Do_wizard_information.\n",
                        c.connection_id);


                        fileclass.Do_log_error(error_msg);

                        ioclass.Do_send_line(c, "Treasure Trove: ** NOT FOUND **\n");
                    }
                    else
                    {


                        string_buffer = CFUNCTIONS.sprintfSinglestring("Treasure Trove: %.0lf, %.0lf\n",
                            trove_ptr.x, trove_ptr.y);


                        ioclass.Do_send_line(c, string_buffer);
                    }

                    miscclass.Do_unlock_mutex(c.realm.realm_lock);

                    /* show the king and steward coffers */
                    miscclass.Do_lock_mutex(c.realm.kings_gold_lock);


                    string_buffer = CFUNCTIONS.sprintfSinglestring("King Treasury: %.0lf\n", c.realm.kings_gold);

                    ioclass.Do_send_line(c, string_buffer);


                    string_buffer = CFUNCTIONS.sprintfSinglestring("Steward Treasury: %.0lf\n",
                        c.realm.steward_gold);

                    ioclass.Do_send_line(c, string_buffer);


                    miscclass.Do_unlock_mutex(c.realm.kings_gold_lock);

                    miscclass.Do_lock_mutex(c.realm.realm_lock);
                    if (c.realm.king != null)
                    {
                        if (c.realm.king.description != null)
                        {
                            string_buffer = CFUNCTIONS.sprintfSinglestring("Ruler : %s\n",
                                c.realm.king.description.name);

                            ioclass.Do_send_line(c, string_buffer);
                        }
                        else
                        {
                            ioclass.Do_send_line(c, "Ruler, but no description.\n");
                        }
                    }



                    string_buffer = CFUNCTIONS.sprintfSinglestring("King : %s    Valar : %s \n",
                        c.realm.king_name, c.realm.valar_name);

                    ioclass.Do_send_line(c, string_buffer);

                    miscclass.Do_unlock_mutex(c.realm.realm_lock);

                    ioclass.Do_more(c);

                    ioclass.Do_send_clear(c);

                    break;

                default:


                    error_msg = CFUNCTIONS.sprintfSinglestring(
                        "[%s] Returned non-option %ld in infoclass.Do_wizard_information.\n",
                        c.connection_id, answer);


                    fileclass.Do_log_error(error_msg);

                    hackclass.Do_caught_hack(c, phantdefs.H_SYSTEM);
                    return;
            }
        }


        /************************************************************************
        /
        / FUNCTION NAME: infoclass.Do_history(struct client_t *c)
        /
        / FUNCTION: do random stuff
        /
        / AUTHOR: Brian Kelly, 01/17/01
        /
        / ARGUMENTS: none
        /
        / RETURN VALUE: none
        /
        / MODULES CALLED: collecttaxes(), cfunctions.floor(), wmove(), drandom(), infloat(),
        /       waddstr(), mvprintw(), getanswer()
        /
        / GLOBAL INPUTS: Player, *stdscr, *Statptr
        /
        / GLOBAL OUTPUTS: Player
        /
        / DESCRIPTION:
        /       Handle gurus, medics, etc.
        /
        *************************************************************************/

        internal void Do_history(client_t c)
        {
            button_t buttons = new button_t();
            player_t thePlayer = new player_t();
            long answer = -1;
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            string string_buffer = ""; //[phantdefs.SZ_LINE], playerName = ""; //[phantdefs.SZ_NAME];

            ioclass.Do_send_line(c, "What do you wish history on?\n");

            CFUNCTIONS.strcpy(ref buttons.button[0], "Char Online\n");
            CFUNCTIONS.strcpy(ref buttons.button[1], "Saved Char\n");
            CFUNCTIONS.strcpy(ref buttons.button[2], "Account\n");
            CFUNCTIONS.strcpy(ref buttons.button[3], "Address\n");
            ioclass.Do_clear_buttons(buttons, 4);
            CFUNCTIONS.strcpy(ref buttons.button[5], "Enter New\n");
            CFUNCTIONS.strcpy(ref buttons.button[7], "Cancel\n");

            if (ioclass.Do_buttons(c, ref answer, buttons) != phantdefs.S_NORM || answer == 7)
            {
                return;

                ioclass.Do_send_clear(c);
            }

            ioclass.Do_send_clear(c);
            switch (answer)
            {

                case 0:

                    Do_player_history(c);
                    return;

                case 1:

                    Do_saved_player_history(c);
                    return;

                case 2:

                    Do_account_history(c);
                    return;

                case 3:

                    Do_address_history(c);
                    return;

                case 5:

                    Do_make_history(c);
                    return;

                default:


                    error_msg = CFUNCTIONS.sprintfSinglestring(
                        "[%s] Returned non-option %ld in infoclass.Do_wizard_information.\n",
                        c.connection_id, answer);


                    fileclass.Do_log_error(error_msg);

                    hackclass.Do_caught_hack(c, phantdefs.H_SYSTEM);
                    return;
            }
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_account_history(struct client_t *c)
        /
        / FUNCTION: do trading post stuff
        /
        / AUTHOR: Brian Kelly, 01/15/01
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

        void Do_account_history(client_t c)
        {
            history_list_t list_ptr = new history_list_t();
            string string_buffer = ""; //[phantdefs.SZ_LINE], 
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            string theAccount = ""; //[phantdefs.SZ_NAME];

            if (ioclass.Do_string_dialog(c, ref theAccount, phantdefs.SZ_NAME - 1,
                "Retrive the history on which account?\n"))
            {

                return;
            }

            list_ptr = Do_look_history(c, phantdefs.T_ACCOUNT, theAccount);
            Do_show_history(c, list_ptr);

            ioclass.Do_more(c);
            ioclass.Do_send_clear(c);
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_address_history(struct client_t *c)
        /
        / FUNCTION: do trading post stuff
        /
        / AUTHOR: Brian Kelly, 01/15/01
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

        void Do_address_history(client_t c)
        {
            history_list_t list_ptr = new history_list_t();
            string string_buffer = ""; //[phantdefs.SZ_LINE], 
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            string theAddress = ""; //[phantdefs.SZ_NAME];

            if (ioclass.Do_string_dialog(c, ref theAddress, phantdefs.SZ_NAME - 1,
                "Retrive the history on which address?\n"))
            {

                return;
            }

            list_ptr = Do_look_history(c, phantdefs.T_ADDRESS, theAddress);
            Do_show_history(c, list_ptr);

            ioclass.Do_more(c);
            ioclass.Do_send_clear(c);
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_player_history(struct client_t *c)
        /
        / FUNCTION: do trading post stuff
        /
        / AUTHOR: Brian Kelly, 01/15/01
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

        void Do_player_history(client_t c)
        {
            history_list_t list_ptr = new history_list_t();
            history_list_t list_ptr_two = new history_list_t();
            game_t game_ptr;
            string string_buffer = ""; //[phantdefs.SZ_LINE], error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            string characterName = ""; //[phantdefs.SZ_NAME], 
            string account = ""; //[phantdefs.SZ_NAME], 
            string machineID = ""; //[phantdefs.SZ_NAME];
            bool characterFound;

            if (ioclass.Do_player_dialog(c, "Retrive the history on which player?\n", characterName) != phantdefs.S_NORM)
            {

                return;
            }

            characterFound = false;
            miscclass.Do_lock_mutex(c.realm.realm_lock);

            /* run though all the games and check the names */
            game_ptr = c.realm.games;
            while (game_ptr != null)
            {

                /* check for a name match */
                if (game_ptr.description != null &&
                        !CFUNCTIONS.strcmp(characterName, game_ptr.description.name))
                {


                    CFUNCTIONS.strcpy(ref account, game_ptr.account);
                    characterFound = true;
                    break;
                }

                game_ptr = game_ptr.next_game;
            }

            /* the character was not found */
            miscclass.Do_unlock_mutex(c.realm.realm_lock);

            if (!characterFound)
            {


                ioclass.Do_send_line(c, "That character just left the game.\n");

                ioclass.Do_more(c);

                ioclass.Do_send_clear(c);
                return;
            }

            /* find the machine ID */
            if (c.machineID != 0)
            {
                CFUNCTIONS.sprintf(ref machineID, "%ld", c.machineID);
                list_ptr = Do_look_history(c, phantdefs.T_MACHINE, machineID);
            }
            else
            {
                list_ptr = null;
            }

            /* get the player information */
            list_ptr_two = Do_look_history(c, phantdefs.T_ACCOUNT, account);

            Do_show_two_history_lists(c, list_ptr, list_ptr_two);

            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_saved_player_history(struct client_t *c)
        /
        / FUNCTION: do trading post stuff
        /
        / AUTHOR: Brian Kelly, 01/16/01
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

        void Do_saved_player_history(client_t c)
        {
            player_t thePlayer = new player_t();
            history_list_t list_ptr = new history_list_t();
            history_list_t list_ptr_two = new history_list_t();
            game_t game_ptr = new game_t();
            string string_buffer = ""; //[phantdefs.SZ_LINE], 
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            string characterName = ""; //[phantdefs.SZ_NAME], 
            string account = ""; //[phantdefs.SZ_NAME], 
            string network = ""; //[phantdefs.SZ_FROM];
            string lcCharacterName = ""; //[phantdefs.SZ_NAME];
            bool characterFound;
            CLibFile.CFILE character_file;

            if ((ioclass.Do_string_dialog(c, ref characterName, phantdefs.SZ_NAME - 1,
                "What is the name of the character to query?\n") ? 1 : 0) != phantdefs.S_NORM)
            {

                return;
            }

            characterFound = false;

            /* load the character information */
            miscclass.Do_lowercase(ref lcCharacterName, characterName);

            if (characterclass.Do_look_character(c, lcCharacterName, ref thePlayer) != 0)
            {


                CFUNCTIONS.strcpy(ref account, thePlayer.parent_account);

                CFUNCTIONS.strcpy(ref network, thePlayer.parent_network);
                characterFound = true;
            }

            else
            {

                miscclass.Do_lock_mutex(c.realm.realm_lock);

                /* run though all the games and check the names */
                game_ptr = c.realm.games;
                while (game_ptr != null)
                {

                    /* check for a name match */
                    if (game_ptr.description != null &&
                            !CFUNCTIONS.strcmp(lcCharacterName, game_ptr.description.lcname))
                    {


                        CFUNCTIONS.strcpy(ref account, game_ptr.account);

                        CFUNCTIONS.strcpy(ref network, game_ptr.network);
                        characterFound = true;
                        break;
                    }

                    game_ptr = game_ptr.next_game;
                }
            }

            miscclass.Do_unlock_mutex(c.realm.realm_lock);

            if (!characterFound)
            {


                CFUNCTIONS.sprintf(ref string_buffer,
                    "A character named \"%s\" could not be found.\n",
                    characterName);


                ioclass.Do_send_line(c, string_buffer);

                ioclass.Do_more(c);

                ioclass.Do_send_clear(c);
                return;
            }

            list_ptr = Do_look_history(c, phantdefs.T_ACCOUNT, account);

            /* if the network address is protected, only use the account */
            if (fileclass.Do_check_protected(c, network) != 0)
            {

                Do_show_history(c, list_ptr);
                ioclass.Do_more(c);
                ioclass.Do_send_clear(c);
            }
            else
            {
                /* get the player information */
                list_ptr_two = Do_look_history(c, phantdefs.T_ADDRESS, network);
                Do_show_two_history_lists(c, list_ptr_two, list_ptr);
            }

            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_show_two_history_lists(struct client_t *c, struct history_list_t *listOne, struct history_list_t *listTwo)
        /
        / FUNCTION: do trading post stuff
        /
        / AUTHOR: Brian Kelly, 01/16/01
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

        void Do_show_two_history_lists(client_t c, history_list_t listOne, history_list_t listTwo)
        {
            history_list_t temp_ptr = new history_list_t();
            history_list_t list_ptr_ptr = new history_list_t();

            /* put list_ptr_two into list_ptr in proper order */
            list_ptr_ptr = listOne;

            /* run theough each element of the second list */
            while (listTwo != null)
            {

                /* increment temp_ptr until at right space or null */
                while (list_ptr_ptr != null && (list_ptr_ptr).theHistory.date >
                    listTwo.theHistory.date)
                {

                    /* move to the next instance */
                    list_ptr_ptr = list_ptr_ptr.next;
                }

                /* put the account element here */
                temp_ptr = listTwo;
                listTwo = listTwo.next;
                temp_ptr.next = list_ptr_ptr;

                list_ptr_ptr = temp_ptr;
            }


            Do_show_history(c, listOne);
            ioclass.Do_more(c);
            ioclass.Do_send_clear(c);
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_make_history(struct client_t *c)
        /
        / FUNCTION: return a char specifying player type
        /
        / AUTHOR: Brian Kelly, 01/15/01
        /
        / ARGUMENTS:
        /       struct client_t c - pointer to the main client strcture
        /
        / RETURN VALUE: 
        /	char - character the describes the character
        /
        / MODULES CALLED: CFUNCTIONS.strcpy(ref )
        /
        / DESCRIPTION:
        /       Return a string describing the player type.
        /       King, council, valar, supercedes other types.
        /       The first character of the string is '*' if the player
        /       has a crown.
        /       If 'shortflag' is true, return a 3 character string.
        /
        *************************************************************************/

        void Do_make_history(client_t c)
        {
            button_t buttons = new button_t();
            history_t theHistory = new history_t();
            game_t game_ptr = new game_t();
            long answer = -1;
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            string string_buffer = ""; //[phantdefs.SZ_LINE];
            string name = ""; //[phantdefs.SZ_FROM], 
            string tagAddress = ""; //[phantdefs.SZ_FROM];
            bool characterFound;

            characterFound = false;

            /* what does the wizard wish to note */
            ioclass.Do_send_line(c,
              "Do you wish to make note of a player, an account or an address?\n");

            CFUNCTIONS.strcpy(ref buttons.button[0], "Player\n");
            CFUNCTIONS.strcpy(ref buttons.button[1], "Account\n");
            CFUNCTIONS.strcpy(ref buttons.button[2], "Address\n");
            ioclass.Do_clear_buttons(buttons, 3);
            CFUNCTIONS.strcpy(ref buttons.button[7], "Cancel\n");

            if (ioclass.Do_buttons(c, ref answer, buttons) != phantdefs.S_NORM || answer == 7)
            {

                ioclass.Do_send_clear(c);
                return;
            }

            ioclass.Do_send_clear(c);
            if (answer > 2 || answer < 0)
            {

                error_msg = CFUNCTIONS.sprintfSinglestring(
                        "[%s] Returned non-option in Do_make_history(3).\n",
                        c.connection_id);

                fileclass.Do_log_error(error_msg);
                hackclass.Do_caught_hack(c, phantdefs.H_SYSTEM);
                return;
            }

            switch (answer)
            {

                case 0:

                    if (ioclass.Do_player_dialog(c, "Which player do you wish to note?\n",
                    name) != phantdefs.S_NORM)
                    {

                        return;
                    }

                    characterFound = false;
                    miscclass.Do_lock_mutex(c.realm.realm_lock);

                    /* run though all the games and check the names */
                    game_ptr = c.realm.games;
                    while (game_ptr != null)
                    {

                        /* check for a name match */
                        if (game_ptr.description != null &&
                                !CFUNCTIONS.strcmp(name, game_ptr.description.name))
                        {

                            CFUNCTIONS.strcpy(ref name, game_ptr.account);
                            characterFound = true;
                            break;
                        }
                        game_ptr = game_ptr.next_game;
                    }

                    miscclass.Do_unlock_mutex(c.realm.realm_lock);

                    /* the character was not found */
                    if (!characterFound)
                    {

                        ioclass.Do_send_line(c, "That character just left the game.\n");
                        ioclass.Do_more(c);
                        ioclass.Do_send_clear(c);
                        return;
                    }

                    theHistory.type = (short)phantdefs.T_ACCOUNT;
                    break;

                case 1:

                    if ((ioclass.Do_string_dialog(c, ref name, phantdefs.SZ_NAME - 1,
                    "What is the name of the account?\n") ? 1 : 0) != phantdefs.S_NORM)
                    {

                        return;
                    }

                    /* prepare the history strcture */
                    theHistory.type = (short)phantdefs.T_ACCOUNT;
                    break;

                case 2:

                    if ((ioclass.Do_string_dialog(c, ref name, phantdefs.SZ_NAME - 1,
                    "What is the the network?\n") ? 1 : 0) != phantdefs.S_NORM)
                    {

                        return;
                    }

                    /* prepare the history strcture */
                    theHistory.type = (short)phantdefs.T_ADDRESS;
                    break;
            }

            if ((ioclass.Do_string_dialog(c, ref string_buffer, phantdefs.SZ_LINE - 1,
                "Please enter the note.\n") ? 1 : 0) != phantdefs.S_NORM)
            {

                return;
            }

            CFUNCTIONS.sprintf(ref theHistory.description, "%s wrote, \"%s\"\n", c.modifiedName,
                string_buffer);

            theHistory.date = CFUNCTIONS.GetUnixEpoch(DateTime.Now);
            miscclass.Do_lowercase(ref theHistory.name, name);
            Do_save_history(c, theHistory);

            if (characterFound)
            {
                miscclass.Do_lowercase(ref theHistory.name, tagAddress);
                theHistory.type = (short)phantdefs.T_ADDRESS;
                Do_save_history(c, theHistory);
            }

            ioclass.Do_send_line(c, "The note has been entered.\n");
            ioclass.Do_more(c);
            ioclass.Do_send_clear(c);
            return;
        }

    }
}
