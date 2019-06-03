using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace phantasiaclasses
{
    public class init //: MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        static init Instance;
        private init()
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
            socketclass = socket.GetInstance();
            tagsclass = tags.GetInstance();
        }
        public static init GetInstance()
        {
            init instance = null;
            if (Instance != null)
            {
                instance = Instance;
            }
            else
            {
                instance = new init();
            }
            return instance;
        }

        phantasiaclasses.misc miscclass;
        phantasiaclasses.file fileclass;
        phantasiaclasses.socket socketclass;
        phantasiaclasses.tags tagsclass;


        //internal init()
        //{
        //    miscclass = new misc();
        //    fileclass = new file();

        //    socketclass = new socket();
        //    tagsclass = new tags();
        //}
        /*
     * init.c - startup/shutdown routines for Phantasia
     */
        int server_hook;
        //extern randomStateBuffer; //handled in CFUNCTIONS
        //extern randData; //handled in CFUNCTIONS

        /************************************************************************
        /
        / FUNCTION NAME: Do_initialize(struct server_t *server)
        /
        / FUNCTION: To initialize the program's variables
        /
        / AUTHOR: Brian Kelly, 4/6/99
        /
        / ARGUMENTS:
        /       struct server_t *s - address of the server's main data strcture
        /
        / RETURN VALUE: short error
        /
        / MODULES CALLED: wmove(), _filbuf(), clearok(), waddstr(), wrefresh(),
        /       wclrtoeol()
        /
        / GLOBAL INPUTS: Echo, _iob[], Wizard, *stdscr
        /
        / GLOBAL OUTPUTS: _iob[]
        /
        / DESCRIPTION:
        /       Read a string from the keyboard.
        /       This routine is specially designed to:
        /
        /           - strip non-printing characters (unless Wizard)
        /           - echo, if desired
        /           - redraw the screen if CH_REDRAW is entered
        /           - read in only 'mx - 1' characters or less characters
        /           - nul-terminate string, and throw away newline
        /
        /       'mx' is assumed to be at least 2.
        /
        *************************************************************************/

        internal void Init_server(server_t s)
        {
            Debug.Log("phantasia.init.Init_server");
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            int error;
            realm_object_t object_ptr = new realm_object_t();

            /* seed the random number generator */
            CFUNCTIONS.initstate_r((uint)CFUNCTIONS.time(null), CFUNCTIONS.randomStateBuffer, CFUNCTIONS.STATELEN, CFUNCTIONS.randData);

            /* initialize realm variables */
            server_hook = s.run_level;
            s.realm.serverPid = CLibPThread.gettid();
            s.realm.objects = null;
            s.realm.games = null;
            s.realm.king = null;
            s.realm.valar = null;
            s.realm.king_flag = false;
            s.realm.king_name = "\0";
            s.realm.valar_name = "\0";
            s.realm.name_limbo = null;
            s.realm.email_limbo = null;
            s.realm.connections = null;
            s.realm.steward_gold = 0;

            /* initialize all the realm mutexes */
            miscclass.Do_init_mutex(s.realm.realm_lock);

            miscclass.Do_init_mutex(s.realm.backup_lock);
            miscclass.Do_init_mutex(s.realm.scoreboard_lock);
            miscclass.Do_init_mutex(s.realm.account_lock);
            miscclass.Do_init_mutex(s.realm.character_file_lock);
            miscclass.Do_init_mutex(s.realm.log_file_lock);
            miscclass.Do_init_mutex(s.realm.network_file_lock);
            miscclass.Do_init_mutex(s.realm.tag_file_lock);
            miscclass.Do_init_mutex(s.realm.tagged_file_lock);
            miscclass.Do_init_mutex(s.realm.history_file_lock);

            miscclass.Do_init_mutex(s.realm.monster_lock);
            miscclass.Do_init_mutex(s.realm.object_lock);
            miscclass.Do_init_mutex(s.realm.kings_gold_lock);
            miscclass.Do_init_mutex(s.realm.hack_lock);
            miscclass.Do_init_mutex(s.realm.connections_lock);

            /* set the number to tags to the time */
            s.realm.nextTagNumber = Do_get_next_tag();

            /* load the realm objects and the king's gold */
            s.realm.objects = null;
            error = Do_load_data_file(s.realm);

            /* if there's an error, we can continue, just with no objects */
            if (error != 0)
            {

                error_msg = CFUNCTIONS.sprintfSinglestring("[0.0.0.0:%d] Phantasia will continue with no objects from previous games.\n", (int)s.realm.serverPid);

                fileclass.Do_log_error(error_msg);

                /* set kings and stewards gold to zero */
                s.realm.kings_gold = 0;

                /* only the grail should exist */
                while (s.realm.objects != null)
                {
                    object_ptr = s.realm.objects;
                    s.realm.objects = object_ptr.next_object;

                    object_ptr = null; // free((void*) object_ptr);
                }

                Do_hide_grail(s.realm, 5000);
                Do_hide_trove(s.realm);
            }

            /* load the monsters into the realm array */
            Do_load_monster_file(s.realm);

            /* load the charstats into the realm array */
            Do_load_charstats_file(s.realm);

            /* load the shop items into the realm array */
            Do_load_shopitems_file(s.realm);

            /* restore any characters in the backup file */
            Do_backup_restore();

            /* initialize the server variables */
            s.num_games = 0;

            /* start up the socket connection */
            s.the_socket = socketclass.Do_init_server_socket();

            /* no problems */
            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_load_data_file(struct *realm_t)
        /
        / FUNCTION: To load saved configuration information
        /
        / AUTHOR: Brian Kelly, 4/8/99
        /
        / ARGUMENTS:
        /       struct realm_t *the_realm - pointer to the realm
        /
        / RETURN VALUE: short error
        /
        / MODULES CALLED: wmove(), _filbuf(), clearok(), waddstr(), wrefresh(),
        /       wclrtoeol()
        /
        / DESCRIPTION:
        /       Read a string from the keyboard.
        /       This routine is specially designed to:
        /
        /           - strip non-printing characters (unless Wizard)
        /           - echo, if desired
        /           - redraw the screen if CH_REDRAW is entered
        /           - read in only 'mx - 1' characters or less characters
        /           - nul-terminate string, and throw away newline
        /
        /       'mx' is assumed to be at least 2.
        /
        *************************************************************************/

        int Do_load_data_file(realm_t the_realm)
        {
            Debug.Log("phantasia.init.Do_load_data_file");
            CLibFile.CFILE data_file;
            realm_object_t object_ptr = new realm_object_t();
            realm_state_t state_ptr = new realm_state_t();
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            int error;
            int grailCount = 0, troveCount = 0;
            // (realm_state_t) Do_malloc(phantdefs.SZ_REALM_STATE);

            /* open the data file - for king's gold and corpses */
            int errno = 0;
            if ((data_file = CLibFile.fopen(pathnames.DATA_FILE, "r", ref errno)) == null)
            {

                error_msg = CFUNCTIONS.sprintfSinglestring(
                "[0.0.0.0:%d] CLibFile.fopen of %s failed in Do_load_data_file: %s\n",
                the_realm.serverPid, pathnames.DATA_FILE, CFUNCTIONS.strerror(errno));

                fileclass.Do_log_error(error_msg);
                return phantdefs.DATA_FILE_ERROR;
            }
            
            if (CLibFile.fread(ref state_ptr, phantdefs.SZ_REALM_STATE, 1, data_file) != 1)
            {

                error_msg = CFUNCTIONS.sprintfSinglestring(
                "[0.0.0.0:%d] CLibFile.fread of %s failed in Do_load_data_file: %d\n",
                (int)the_realm.serverPid, pathnames.DATA_FILE, CLibFile.ferror(data_file));

                fileclass.Do_log_error(error_msg);
                CLibFile.fclose(data_file);
                return phantdefs.DATA_FILE_ERROR;
            }

            the_realm.kings_gold = state_ptr.kings_gold;
            CFUNCTIONS.strcpy(ref the_realm.king_name, state_ptr.king_name);
            CFUNCTIONS.strcpy(ref the_realm.valar_name, state_ptr.valar_name);

            /* destroy the temporary realm state object */
            state_ptr = null; // free((void*) state_ptr);

            /* first entry is king's gold as double float */
            /*
                if (CLibFile.fread(ref (void *) the_realm.kings_gold, sizeof(the_realm.kings_gold),
                    1, data_file) != 1) {

                    error_msg = CFUNCTIONS.sprintf(ref 
                    "[0.0.0.0:%d] CLibFile.fread of %s failed in Do_load_data_file: %d\n",
                    the_realm.serverPid, pathnames.DATA_FILE, ferror(data_file));

                    fileclass.Do_log_error(error_msg);
                    CLibFile.fclose(data_file);
                    return phantdefs.DATA_FILE_ERROR;
                }
            */

            /* all remaining items are realm objects */
            /* create a structure to read into */
            object_ptr = new realm_object_t();// (struct realm_object_t *) Do_malloc(phantdefs.SZ_REALM_OBJECT);
            
            /* read next object while not at EOF */
            while (CLibFile.fread(ref object_ptr, phantdefs.SZ_REALM_OBJECT, 1, data_file) == 1) 
            {
                //if (object_ptr.type == 0)
                //    Debug.LogError("REALM READ ERROR: type: " + object_ptr.type + " (x,y = " + object_ptr.x + "," + object_ptr.y + ")");

                switch (object_ptr.type)
                {
                    case phantdefs.CORPSE:

                        /* create an strcture to hold the character information */
                        object_ptr.arg1 = null;// (void*) malloc(phantdefs.SZ_PLAYER);

                        /* and read in the dead player's information */
                        if (CLibFile.fread(ref object_ptr.arg1, phantdefs.SZ_PLAYER, 1, data_file) == 0)
                        {

                            error_msg = CFUNCTIONS.sprintfSinglestring(
                       "[0.0.0.0:%d] CLibFile.fread of %s failed in Do_load_data_file(2): %d\n",
                       the_realm.serverPid, pathnames.DATA_FILE, CLibFile.ferror(data_file));

                            fileclass.Do_log_error(error_msg);
                            CLibFile.fclose(data_file);

                            object_ptr.arg1 = null; // free((void*) object_ptr.arg1);

                            object_ptr = null; // free((void*) object_ptr);
                            return phantdefs.DATA_FILE_ERROR;
                        }

                        /* used to be phantdefs.CORPSE_LIFE */
                        if (86400 * CFUNCTIONS.sqrt(((player_t)object_ptr.arg1).level)
                                < CFUNCTIONS.GetUnixEpoch(DateTime.Now) - ((player_t)object_ptr.arg1).last_load)
                        {

                            /* delete the object */
                            object_ptr.arg1 = null; // free((void*) object_ptr.arg1);

                            object_ptr = null; // free((void*) object_ptr);
                        }
                        else
                        {
                            /* put the new corpse in the realm object list */
                            object_ptr.next_object = the_realm.objects;
                            the_realm.objects = object_ptr;
                        }

                        break;

                    /* if the object is the holy grail, we do nothing special */
                    case phantdefs.HOLY_GRAIL:

                        /* put the item in the realm object list */
                        object_ptr.next_object = the_realm.objects;
                        the_realm.objects = object_ptr;
                        ++grailCount;
                        break;

                    /* if the object is the treasure trove, we do nothing special */
                    case phantdefs.TREASURE_TROVE:

                        /* put the item in the realm object list */
                        object_ptr.next_object = the_realm.objects;
                        the_realm.objects = object_ptr;
                        ++troveCount;
                        break;

                    /* anything else is an error */
                    default:

                        error_msg = CFUNCTIONS.sprintfSinglestring(
                  "[0.0.0.0:%d] bad realm object of type %hd read in Do_load_data_file.\n",
                  the_realm.serverPid, object_ptr.type);

                        fileclass.Do_log_error(error_msg);
                        CLibFile.fclose(data_file);

                        object_ptr = null; // free((void*) object_ptr);
                        return phantdefs.DATA_FILE_ERROR;
                }

                /* create a new temporary realm object */
                object_ptr = new realm_object_t();// (realm_object_t)malloc(phantdefs.SZ_REALM_OBJECT);
            }

            /* destroy the remaining temporary realm object */
            object_ptr = null; // free((void*) object_ptr);

            /* if we found no grail, hide a new one */
            if (grailCount == 0)
            {


                error_msg = CFUNCTIONS.sprintfSinglestring(
                 "[0.0.0.0:%d] No grail found in Do_load_data_file. Creating a new one.\n",
                 the_realm.serverPid);


                fileclass.Do_log_error(error_msg);
                Do_hide_grail(the_realm, 5000);
            }
            else if (grailCount != 1)
            {


                error_msg = CFUNCTIONS.sprintfSinglestring(
                   "[0.0.0.0:%d] Read in %d grails in Do_load_data_file.\n",
                   the_realm.serverPid, grailCount);


                fileclass.Do_log_error(error_msg);
                return phantdefs.DATA_FILE_ERROR;
            }

            /* if we found no treasure trove, hide a new one */
            if (troveCount == 0)
            {


                error_msg = CFUNCTIONS.sprintfSinglestring("[0.0.0.0:%d] No treasure trove found in Do_load_data_file. Creating a new one.\n", the_realm.serverPid);


                fileclass.Do_log_error(error_msg);
                Do_hide_trove(the_realm);
            }
            else if (troveCount != 1)
            {


                error_msg = CFUNCTIONS.sprintfSinglestring(
                   "[0.0.0.0:%d] Read in %d treasure troves in Do_load_data_file.\n",
                   the_realm.serverPid, troveCount);


                fileclass.Do_log_error(error_msg);
                return phantdefs.DATA_FILE_ERROR;
            }

            CLibFile.fclose(data_file); //added for Unity
            return 0;   /* no problems */
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_load_monster_file(struct realm_t *the_realm)
        /
        / FUNCTION: To load saved configuration information
        /
        / AUTHOR: Brian Kelly, 4/8/99
        /
        / ARGUMENTS:
        /       struct realm_t *the_realm - pointer to the realm strcture
        /
        / RETURN VALUE: none
        /
        / MODULES CALLED: wmove(), _filbuf(), clearok(), waddstr(), wrefresh(),
        /       wclrtoeol()
        /
        / DESCRIPTION:
        /       Read a string from the keyboard.
        /       This routine is specially designed to:
        /
        /           - strip non-printing characters (unless Wizard)
        /           - echo, if desired
        /           - redraw the screen if CH_REDRAW is entered
        /           - read in only 'mx - 1' characters or less characters
        /           - nul-terminate string, and throw away newline
        /
        /       'mx' is assumed to be at least 2.
        /
        *************************************************************************/

        void Do_load_monster_file(realm_t the_realm)
        {
            Debug.Log("phantasia.init.Do_load_monster_file");
            CLibFile.CFILE monster_file;
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            int i;

            /* load monster information */
            int errno = 0;

            /* open the monster file */
            if ((monster_file = CLibFile.fopen(pathnames.MONSTER_FILE, "r", ref errno)) == null)
            {

                error_msg = CFUNCTIONS.sprintfSinglestring(
                   "[0.0.0.0:%d] CLibFile.fopen of %s failed in Do_load_monster_file: %s\n",
                   the_realm.serverPid, pathnames.MONSTER_FILE, CFUNCTIONS.strerror(errno));

                fileclass.Do_log_error(error_msg);
                CFUNCTIONS.exit(phantdefs.MONSTER_FILE_ERROR);
            }

            /* read each line of the monster file */
            for (i = 0; i < phantdefs.NUM_MONSTERS; i++)
            {

                /* read the monster name which is on its own line */
                if (CLibFile.fgets(ref the_realm.monster[i].name, phantdefs.SZ_MONSTER_NAME, monster_file) == null)
                {
                    error_msg = CFUNCTIONS.sprintfSinglestring("[0.0.0.0:%d] CLibFile.fgets of %s failed on call number %d in Do_load_monster_file.\n", the_realm.serverPid, pathnames.MONSTER_FILE, i);

                    fileclass.Do_log_error(error_msg);
                    CFUNCTIONS.exit(phantdefs.MONSTER_FILE_ERROR);
                }

                /* reEmove trailing blanks */
                miscclass.Do_truncstring(ref the_realm.monster[i].name);

                /* read the stat line for each monster */
                string[] extractedValues;
                if ((extractedValues = CLibFile.fscanf(monster_file, "%lf %lf %lf %lf %lf %hd %hd %hd\n",
                                                            the_realm.monster[i].strength, the_realm.monster[i].brains,
                                                            the_realm.monster[i].speed, the_realm.monster[i].energy,
                                                            the_realm.monster[i].experience,
                                                            the_realm.monster[i].treasure_type,
                                                            the_realm.monster[i].special_type,
                                                            the_realm.monster[i].flock_percent)).Length != 8)
                {

                    /* exit if we didn't read 8 items */
                    error_msg = CFUNCTIONS.sprintfSinglestring("[0.0.0.0:%d] fscanf of %s failed on call number %d in Do_load_monster_file.\n", the_realm.serverPid, pathnames.MONSTER_FILE, i);

                    fileclass.Do_log_error(error_msg);
                    CFUNCTIONS.exit(phantdefs.MONSTER_FILE_ERROR);
                }
                //unity: populating extracted values manually due to param/ref argument restrictions
                //exceptionFlag is unused anyway
                the_realm.monster[i].strength = Convert.ToDouble(extractedValues[0]);
                the_realm.monster[i].brains = Convert.ToDouble(extractedValues[1]);
                the_realm.monster[i].speed = Convert.ToDouble(extractedValues[2]);
                the_realm.monster[i].energy = Convert.ToDouble(extractedValues[3]);
                the_realm.monster[i].experience = Convert.ToDouble(extractedValues[4]);
                the_realm.monster[i].treasure_type = (short)Convert.ToInt32(extractedValues[5]);
                the_realm.monster[i].special_type = (short)Convert.ToInt32(extractedValues[6]);
                the_realm.monster[i].flock_percent = (short)Convert.ToInt32(extractedValues[7]);
            }

            /* close the monster file */
            CLibFile.fclose(monster_file);

            /* all done here */
            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_load_charstats_file(struct realm_t *the_realm)
        /
        / FUNCTION: To load saved configuration information
        /
        / AUTHOR: Brian Kelly, 5/7/99
        /
        / ARGUMENTS:
        /       struct realm_t *the_realm - pointer to the realm strcture
        /
        / RETURN VALUE: none
        /
        / MODULES CALLED: wmove(), _filbuf(), clearok(), waddstr(), wrefresh(),
        /       wclrtoeol()
        /
        / DESCRIPTION:
        /       Read a string from the keyboard.
        /       This routine is specially designed to:
        /
        /           - strip non-printing characters (unless Wizard)
        /           - echo, if desired
        /           - redraw the screen if CH_REDRAW is entered
        /           - read in only 'mx - 1' characters or less characters
        /           - nul-terminate string, and throw away newline
        /
        /       'mx' is assumed to be at least 2.
        /
        *************************************************************************/

        void Do_load_charstats_file(realm_t the_realm)
        {
            Debug.Log("phantasia.init.Do_load_charstats_file");
            CLibFile.CFILE charstats_file;
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            int i;

            /* load charstats information */
            int errno = 0;

            /* open the charstats file */
            if ((charstats_file = CLibFile.fopen(pathnames.CHARSTATS_FILE, "r", ref errno)) == null)
            {

                error_msg = CFUNCTIONS.sprintfSinglestring(
                "[0.0.0.0:%d] CLibFile.fopen of %s failed Do_load_charstats_file: %s\n",
                the_realm.serverPid, pathnames.CHARSTATS_FILE, CFUNCTIONS.strerror(errno));

                fileclass.Do_log_error(error_msg);
                CFUNCTIONS.exit(phantdefs.CHARSTATS_FILE_ERROR);
            }

            /* read each line of the charstats file */
            for (i = 0; i < phantdefs.NUM_CHARS; i++)
            {

                /* read the character name which is on its own line */
                if (CLibFile.fgets(ref the_realm.charstats[i].class_name, phantdefs.SZ_CLASS_NAME, charstats_file) == null)
                {

                    error_msg = CFUNCTIONS.sprintfSinglestring("[0.0.0.0:%d] CLibFile.fgets of %s failed on call number %d in Do_load_charstats_file.\n", the_realm.serverPid, pathnames.CHARSTATS_FILE, i);

                    fileclass.Do_log_error(error_msg);
                    CFUNCTIONS.exit(phantdefs.CHARSTATS_FILE_ERROR);
                }

                /* reEmove trailing blanks */
                miscclass.Do_truncstring(ref the_realm.charstats[i].class_name);

                /* read the stat line for each charstats */
                string[] extractedValues;
                if ((
                    extractedValues = 
                    CLibFile.fscanf(
                        charstats_file,
                        "%c %lf %lf %lf %lf %d %lf %lf %lf %lf %lf %lf %lf %lf %lf %lf %lf %lf %lf %lf %lf %lf %lf %lf\n",
                the_realm.charstats[i].short_class_name,
                the_realm.charstats[i].max_brains,
                the_realm.charstats[i].max_mana,
                the_realm.charstats[i].weakness,
                the_realm.charstats[i].goldtote,
                the_realm.charstats[i].ring_duration,
                the_realm.charstats[i].quickness.statbase,
                the_realm.charstats[i].quickness.interval,
                the_realm.charstats[i].quickness.increase,
                the_realm.charstats[i].strength.statbase,
                the_realm.charstats[i].strength.interval,
                the_realm.charstats[i].strength.increase,
                the_realm.charstats[i].mana.statbase,
                the_realm.charstats[i].mana.interval,
                the_realm.charstats[i].mana.increase,
                the_realm.charstats[i].energy.statbase,
                the_realm.charstats[i].energy.interval,
                the_realm.charstats[i].energy.increase,
                the_realm.charstats[i].brains.statbase,
                the_realm.charstats[i].brains.interval,
                the_realm.charstats[i].brains.increase,
                the_realm.charstats[i].magiclvl.statbase,
                the_realm.charstats[i].magiclvl.interval,
                the_realm.charstats[i].magiclvl.increase)).Length != 24)
                {

                    /* exit if we didn't read 24 items */
                    error_msg = CFUNCTIONS.sprintfSinglestring("[0.0.0.0:%d] fscanf of %s failed on call number %d in Do_load_charstats_file.\n", the_realm.serverPid, pathnames.CHARSTATS_FILE, i);

                    fileclass.Do_log_error(error_msg);
                    CFUNCTIONS.exit(phantdefs.CHARSTATS_FILE_ERROR);
                }
                //unity: populating extracted values manually due to param/ref argument restrictions
                //exceptionFlag is unused anyway
                the_realm.charstats[i].short_class_name = extractedValues[5].ToCharArray()[0];
                the_realm.charstats[i].max_brains = Convert.ToDouble(extractedValues[1]);
                the_realm.charstats[i].max_mana = Convert.ToDouble(extractedValues[2]);
                the_realm.charstats[i].weakness = Convert.ToDouble(extractedValues[3]);
                the_realm.charstats[i].goldtote = Convert.ToDouble(extractedValues[4]);
                the_realm.charstats[i].ring_duration = Convert.ToInt32(extractedValues[5]);
                the_realm.charstats[i].quickness.statbase = Convert.ToDouble(extractedValues[6]);
                the_realm.charstats[i].quickness.interval = Convert.ToDouble(extractedValues[7]);
                the_realm.charstats[i].quickness.increase = Convert.ToDouble(extractedValues[8]);
                the_realm.charstats[i].strength.statbase = Convert.ToDouble(extractedValues[9]);
                the_realm.charstats[i].strength.interval = Convert.ToDouble(extractedValues[10]);
                the_realm.charstats[i].strength.increase = Convert.ToDouble(extractedValues[11]);
                the_realm.charstats[i].mana.statbase = Convert.ToDouble(extractedValues[12]);
                the_realm.charstats[i].mana.interval = Convert.ToDouble(extractedValues[13]);
                the_realm.charstats[i].mana.increase = Convert.ToDouble(extractedValues[14]);
                the_realm.charstats[i].energy.statbase = Convert.ToDouble(extractedValues[15]);
                the_realm.charstats[i].energy.interval = Convert.ToDouble(extractedValues[16]);
                the_realm.charstats[i].energy.increase = Convert.ToDouble(extractedValues[17]);
                the_realm.charstats[i].brains.statbase = Convert.ToDouble(extractedValues[18]);
                the_realm.charstats[i].brains.interval = Convert.ToDouble(extractedValues[19]);
                the_realm.charstats[i].brains.increase = Convert.ToDouble(extractedValues[20]);
                the_realm.charstats[i].magiclvl.statbase = Convert.ToDouble(extractedValues[21]);
                the_realm.charstats[i].magiclvl.interval = Convert.ToDouble(extractedValues[22]);
                the_realm.charstats[i].magiclvl.increase = Convert.ToDouble(extractedValues[23]);

            }

            /* close the charstats file */
            CLibFile.fclose(charstats_file);

            /* all done here */
            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_load_shopitems_file(struct realm_t *the_realm)
        /
        / FUNCTION: To load saved configuration information
        /
        / AUTHOR: Brian Kelly, 6/29/99
        /
        / ARGUMENTS:
        /       struct realm_t *the_realm - pointer to the realm strcture
        /
        / RETURN VALUE: none
        /
        / MODULES CALLED: wmove(), _filbuf(), clearok(), waddstr(), wrefresh(),
        /       wclrtoeol()
        /
        / DESCRIPTION:
        /       Read a string from the keyboard.
        /       This routine is specially designed to:
        /
        /           - strip non-printing characters (unless Wizard)
        /           - echo, if desired
        /           - redraw the screen if CH_REDRAW is entered
        /           - read in only 'mx - 1' characters or less characters
        /           - nul-terminate string, and throw away newline
        /
        /       'mx' is assumed to be at least 2.
        /
        *************************************************************************/

        void Do_load_shopitems_file(realm_t the_realm)
        {
            Debug.Log("phantasia.init.Do_load_shopitems_file");
            CLibFile.CFILE shopitems_file;
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            int i;

            /* load shopitems information */
            int errno = 0;

            /* open the shopitems file */
            if ((shopitems_file = CLibFile.fopen(pathnames.SHOPITEMS_FILE, "r", ref errno)) == null)
            {

                error_msg = CFUNCTIONS.sprintfSinglestring(
                 "[0.0.0.0:%d] CLibFile.fopen of %s failed in Do_load_shopitems_file: %s\n",
                 the_realm.serverPid, pathnames.SHOPITEMS_FILE, CFUNCTIONS.strerror(errno));

                fileclass.Do_log_error(error_msg);
                CFUNCTIONS.exit(phantdefs.SHOPITEMS_FILE_ERROR);
            }

            /* read each line of the monster file */
            for (i = 0; i < phantdefs.NUM_ITEMS; i++)
            {

                /* read the character name which is on its own line */
                if (CLibFile.fgets(ref the_realm.shop_item[i].item, phantdefs.SZ_ITEMS, shopitems_file)
                == null)
                {

                    error_msg = CFUNCTIONS.sprintfSinglestring("[0.0.0.0:%d] CLibFile.fgets of %s failed on call number %d in Do_load_shopitems_file.\n", the_realm.serverPid, pathnames.SHOPITEMS_FILE, i);

                    fileclass.Do_log_error(error_msg);
                    CFUNCTIONS.exit(phantdefs.SHOPITEMS_FILE_ERROR);
                }

                /* reEmove trailing blanks */
                miscclass.Do_truncstring(ref the_realm.shop_item[i].item);

                //CLibFile.FscanfObjectWrapper costWrapper = new CLibFile.FscanfObjectWrapper(the_realm.shop_item[i].cost);
                /* read the stat line for each item */
                string[] extractedValues;
                if ((extractedValues = CLibFile.fscanf(shopitems_file, "%lf\n", the_realm.shop_item[i].cost)).Length
                != 1)
                {

                    /* exit if we didn't read 1 item */
                    error_msg = CFUNCTIONS.sprintfSinglestring("[0.0.0.0:%d] fscanf of %s failed on call number %d in Do_load_shopitems_file.\n", the_realm.serverPid, pathnames.SHOPITEMS_FILE, i);

                    fileclass.Do_log_error(error_msg);
                    CFUNCTIONS.exit(phantdefs.SHOPITEMS_FILE_ERROR);
                }
                //Debug.Log("extracted item cost value: " + extractedValues[0]);
                the_realm.shop_item[i].cost = Convert.ToDouble(extractedValues[0]);

            }

            /* close the charstats file */
            CLibFile.fclose(shopitems_file);

            /* all done here */
            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: void Do_close(struct server_t *server)
        /
        / FUNCTION: To close the program's data files 
        /
        / AUTHOR: Brian Kelly, 4/22/99
        /
        / ARGUMENTS:
        /       struct server_t *s - address of the server's main data strcture
        /
        / RETURN VALUE: none
        /
        / DESCRIPTION:
        /       Read a string from the keyboard.
        /       This routine is specially designed to:
        /
        /           - strip non-printing characters (unless Wizard)
        /           - echo, if desired
        /           - redraw the screen if CH_REDRAW is entered
        /           - read in only 'mx - 1' characters or less characters
        /           - nul-terminate string, and throw away newline
        /
        /       'mx' is assumed to be at least 2.
        /
        *************************************************************************/

        internal void Do_close(server_t s)
        {
            Debug.Log("phantasia.init.Do_close");
            CLibFile.CFILE data_file;
            realm_object_t object_ptr = new realm_object_t();
            realm_object_t object_ptr2 = new realm_object_t();
            realm_state_t state_ptr = new realm_state_t();
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            int error;

            //state_ptr = (realm_state_t) Do_malloc(phantdefs.SZ_REALM_STATE);

            state_ptr.kings_gold = s.realm.kings_gold;
            CFUNCTIONS.strcpy(ref state_ptr.king_name, s.realm.king_name);
            CFUNCTIONS.strcpy(ref state_ptr.valar_name, s.realm.valar_name);

            /* open the data file - for saving kings, valars, and corpses */
            int errno = 0;
            if ((data_file = CLibFile.fopen(pathnames.DATA_FILE, "w", ref errno)) == null)
            {

                error_msg = CFUNCTIONS.sprintfSinglestring("[0.0.0.0:%d] CLibFile.fopen of %s failed in Do_close: %s\n",
                (int)s.realm.serverPid, pathnames.DATA_FILE, CFUNCTIONS.strerror(errno));

                fileclass.Do_log_error(error_msg);
                CFUNCTIONS.exit(phantdefs.DATA_FILE_ERROR);
            }

            if (CLibFile.fwrite(state_ptr, ref phantdefs.SZ_REALM_STATE, 1, data_file)
                    != 1)
            {

                error_msg = CFUNCTIONS.sprintfSinglestring(
                "[0.0.0.0:%d] CLibFile.fwrite of %s's realm state failed in Do_close: %d\n",
                (int)s.realm.serverPid, pathnames.DATA_FILE, CLibFile.ferror(data_file));

                fileclass.Do_log_error(error_msg);
                CLibFile.fclose(data_file);
                CFUNCTIONS.exit(phantdefs.DATA_FILE_ERROR);
            }

            /* destroy the temporary realm state object */
            state_ptr = null; // free((void*) state_ptr);

            /* first entry is king's gold as double float */
            /*
                if (CLibFile.fwrite((void *)s.realm.kings_gold, sizeof(double), 1, data_file)
                        != 1) {

                    error_msg = CFUNCTIONS.sprintf(ref 
                    "[0.0.0.0:%d] CLibFile.fwrite of %s failed in Do_close: %d\n",
                    (int)s.realm.serverPid, pathnames.DATA_FILE, ferror(data_file));

                    fileclass.Do_log_error(error_msg);
                    CLibFile.fclose(data_file);
                    exit(phantdefs.DATA_FILE_ERROR);
                }
            */

            /* all remaining items are realm objects */
            object_ptr = s.realm.objects;

            /* while there is an item in the list of realm objects */
            while (object_ptr != null)
            {

                /* write the object if it is a corpse, holy grail or treasure */
                if (object_ptr.type == phantdefs.HOLY_GRAIL || object_ptr.type == phantdefs.CORPSE ||
                    object_ptr.type == phantdefs.TREASURE_TROVE)
                {

                    /* write the object to the data file */
                    if (CLibFile.fwrite(object_ptr, ref phantdefs.SZ_REALM_OBJECT, 1, data_file) != 1)
                    {

                        error_msg = CFUNCTIONS.sprintfSinglestring(
                       "[0.0.0.0:%d] CLibFile.fwrite of %s failed in Do_close(2): %d\n",
                       (int)s.realm.serverPid, pathnames.DATA_FILE, CLibFile.ferror(data_file));

                        fileclass.Do_log_error(error_msg);
                        CLibFile.fclose(data_file);
                        CFUNCTIONS.exit(phantdefs.DATA_FILE_ERROR);
                    }

                    /* if the object is a corpse object */
                    if (object_ptr.type == phantdefs.CORPSE)
                    {

                        /* write the character record with it */
                        if (CLibFile.fwrite(object_ptr.arg1, ref phantdefs.SZ_PLAYER, 1, data_file)
                    != 1)
                        {

                            error_msg = CFUNCTIONS.sprintfSinglestring(
                       "[0.0.0.0:%d] CLibFile.fwrite of %s failed in Do_close(3): %d\n",
                       (int)s.realm.serverPid, pathnames.DATA_FILE, CLibFile.ferror(data_file));

                            fileclass.Do_log_error(error_msg);
                            CLibFile.fclose(data_file);
                            CFUNCTIONS.exit(phantdefs.DATA_FILE_ERROR);
                        }


                        object_ptr.arg1 = null; // free((void*) object_ptr.arg1);
                    }
                }

                /* point to the next realm object */
                object_ptr2 = object_ptr;
                object_ptr = object_ptr.next_object;

                object_ptr2 = null; // free((void*) object_ptr2);
            }

            /* close the data file */
            CLibFile.fclose(data_file);

            return; /* no problems */
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_hide_grail(struct *realm_t, int level)
        /
        / FUNCTION: To load saved configuration information
        /
        / AUTHOR: E. A. Estes, 12/4/85
        /	  Brian Kelly, 8/17/99
        /
        / ARGUMENTS:
        /       struct realm_t *the_realm - pointer to the realm
        /
        / RETURN VALUE: short error
        /
        / MODULES CALLED: wmove(), _filbuf(), clearok(), waddstr(), wrefresh(),
        /       wclrtoeol()
        /
        / DESCRIPTION:
        /       Read a string from the keyboard.
        /       This routine is specially designed to:
        /
        /           - strip non-printing characters (unless Wizard)
        /           - echo, if desired
        /           - redraw the screen if CH_REDRAW is entered
        /           - read in only 'mx - 1' characters or less characters
        /           - nul-terminate string, and throw away newline
        /
        /       'mx' is assumed to be at least 2.
        /
        *************************************************************************/

        internal int Do_hide_grail(realm_t the_realm, int level)
        {
            Debug.Log("phantasia.init.Do_hide_grail");
            double distance = 0;
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];

            /* WARNING: Realm should already be locked when function is called */
            /* or it shouldn't matter (startup) */

            realm_object_t object_ptr = new realm_object_t();

            object_ptr = the_realm.objects;

            while (object_ptr != null)
            {

                if (object_ptr.type == phantdefs.HOLY_GRAIL)
                {


                    error_msg = CFUNCTIONS.sprintfSinglestring(
                 "[0.0.0.0:%d] Do_hide_grail found a holy grail already in the realm.",
                 the_realm.serverPid);


                    fileclass.Do_log_error(error_msg);
                    return 0;
                }

                object_ptr = object_ptr.next_object;
            }

            if (level < 3000)
            {
                level = 3000;
            }

            /* create a holy grail realm object */
            object_ptr = new realm_object_t();// (realm_object_t) Do_malloc(phantdefs.SZ_REALM_OBJECT);
            object_ptr.type = phantdefs.HOLY_GRAIL;

            /* place grail within point of no return */
            for (; ; )
            {
                if (UnityGameController.StopApplication) //time to quit
                {
                    Debug.Log("Thread " + System.Threading.Thread.CurrentThread.Name + " stopping in Do_hide_grail");
                    return 0;
                }
                else
                {
                    System.Threading.Thread.Sleep(33); //30fps
                }

                object_ptr.x = 0.0;
                object_ptr.y = 0.0;
                miscclass.Do_move_close(ref object_ptr.x, ref object_ptr.y, 1000000.0);
                
                miscclass.Do_distance(0.0, object_ptr.x, 0.0, object_ptr.y, ref distance);

                if (distance >= level * 100)
                {
                    break;
                }
            }

            object_ptr.next_object = the_realm.objects;
            the_realm.objects = object_ptr;
            return 1;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_hide_trove(struct *realm_t)
        /
        / FUNCTION: To load saved configuration information
        /
        / AUTHOR: E. A. Estes, 12/4/85
        /	  Brian Kelly, 5/9/01
        /
        / ARGUMENTS:
        /       struct realm_t *the_realm - pointer to the realm
        /
        / RETURN VALUE: short error
        /
        / MODULES CALLED: wmove(), _filbuf(), clearok(), waddstr(), wrefresh(),
        /       wclrtoeol()
        /
        / DESCRIPTION:
        /       Read a string from the keyboard.
        /       This routine is specially designed to:
        /
        /           - strip non-printing characters (unless Wizard)
        /           - echo, if desired
        /           - redraw the screen if CH_REDRAW is entered
        /           - read in only 'mx - 1' characters or less characters
        /           - nul-terminate string, and throw away newline
        /
        /       'mx' is assumed to be at least 2.
        /
        *************************************************************************/

        internal int Do_hide_trove(realm_t the_realm)
        {
            Debug.Log("phantasia.init.Do_hide_trove");
            double distance = 0;
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            realm_object_t object_ptr = new realm_object_t();

            /* WARNING: Realm should already be locked when function is called */
            /* or it shouldn't matter (startup) */

            object_ptr = the_realm.objects;

            while (object_ptr != null)
            {

                if (object_ptr.type == phantdefs.TREASURE_TROVE)
                {


                    error_msg = CFUNCTIONS.sprintfSinglestring(
                 "[0.0.0.0:%d] Do_hide_trove found a trove already in the realm.\n",
                 the_realm.serverPid);


                    fileclass.Do_log_error(error_msg);
                    return 0;
                }

                object_ptr = object_ptr.next_object;
            }

            /* create a treasure_trove realm object */
            object_ptr = new realm_object_t();// (realm_object_t) Do_malloc(phantdefs.SZ_REALM_OBJECT);
            object_ptr.type = phantdefs.TREASURE_TROVE;

            for (; ; )
            {
                if (UnityGameController.StopApplication) //time to quit
                {
                    Debug.Log("Thread " + System.Threading.Thread.CurrentThread.Name + " stopping in Do_hide_trove");
                    return 0;
                }
                else
                {
                    System.Threading.Thread.Sleep(33); //30fps
                }

                object_ptr.x = 0.0;
                object_ptr.y = 0.0;
                miscclass.Do_move_close(ref object_ptr.x, ref object_ptr.y, 1400.0);


                miscclass.Do_distance(0.0, object_ptr.x, 0.0, object_ptr.y, ref distance);

                if (distance >= 600)
                {
                    break;
                }
            }

            object_ptr.next_object = the_realm.objects;
            the_realm.objects = object_ptr;

            return 1;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_backup_restore()
        /
        / FUNCTION: To load saved configuration information
        /
        / AUTHOR: Brian Kelly, 11/26/99
        /
        / ARGUMENTS:
        /       struct realm_t *the_realm - pointer to the realm
        /
        / RETURN VALUE: short error
        /
        / MODULES CALLED: wmove(), _filbuf(), clearok(), waddstr(), wrefresh(),
        /       wclrtoeol()
        /
        / DESCRIPTION:
        /       Read a string from the keyboard.
        /       This routine is specially designed to:
        /
        /           - strip non-printing characters (unless Wizard)
        /           - echo, if desired
        /           - redraw the screen if CH_REDRAW is entered
        /           - read in only 'mx - 1' characters or less characters
        /           - nul-terminate string, and throw away newline
        /
        /       'mx' is assumed to be at least 2.
        /
        *************************************************************************/

        void Do_backup_restore()
        {
            Debug.Log("phantasia.init.Do_backup_restore");
            CLibFile.CFILE backup_file;
            CLibFile.CFILE character_file;
            player_t the_player = new player_t();
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            int restoreCount = 0;

            /* open the backup file */
            int errno = 0;
            if ((backup_file = CLibFile.fopen(pathnames.BACKUP_FILE, "r", ref errno)) == null)
            {

                /* No backup file?  Cool! */
                return;
            }

            /* open the character file */
            errno = 0;
            if ((character_file = CLibFile.fopen(pathnames.CHARACTER_FILE, "a", ref errno)) == null)
            {

                error_msg = CFUNCTIONS.sprintfSinglestring(
                "[0.0.0.0:?] CLibFile.fopen of %s failed in Do_backup_restore: %s\n",
                pathnames.CHARACTER_FILE, CFUNCTIONS.strerror(errno));

                fileclass.Do_log_error(error_msg);

                CLibFile.fclose(backup_file);
                return;
            }

            /* read each line of the backup file */
            errno = 0;
            while (CLibFile.fread(ref the_player, phantdefs.SZ_PLAYER, 1, backup_file) == 1)
            {

                /* write the character to the charcter file */
                //Debug.LogError("CHAR FILE DEBUG: appending");
                if (CLibFile.fwrite(the_player, ref phantdefs.SZ_PLAYER, 1, character_file) != 1)
                {

                    error_msg = CFUNCTIONS.sprintfSinglestring(
                          "[0.0.0.0:?] CLibFile.fwrite of %s failed in Do_backup_restore: %s\n",
                          pathnames.CHARACTER_FILE, CFUNCTIONS.strerror(errno));

                    fileclass.Do_log_error(error_msg);
                    CLibFile.fclose(backup_file);
                    CLibFile.fclose(character_file);
                    return;
                }
                else
                {

                    error_msg = CFUNCTIONS.sprintfSinglestring("[0.0.0.0:?] %s restored\n", the_player.lcname);
                    fileclass.Do_log(pathnames.SERVER_LOG, error_msg);

                    fileclass.Do_log(pathnames.GAME_LOG, error_msg);

                    ++restoreCount;
                }
            }

            /* delete the old backup file */
            CLibFile.fclose(backup_file);
            CLibFile.remove(pathnames.BACKUP_FILE);   //unity: moved below close call to avoid sharing violation
            CLibFile.fclose(character_file);

            if (restoreCount != 0)
            {
                error_msg = CFUNCTIONS.sprintfSinglestring("Server restored %d characters.\n", restoreCount);
                fileclass.Do_log(pathnames.SERVER_LOG, error_msg);
            }

            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_get_next_tag()
        /
        / FUNCTION: To load saved configuration information
        /
        / AUTHOR: Brian Kelly, 01/18/01
        /
        / ARGUMENTS:
        /       struct realm_t *the_realm - pointer to the realm
        /
        / RETURN VALUE: short error
        /
        / MODULES CALLED: wmove(), _filbuf(), clearok(), waddstr(), wrefresh(),
        /       wclrtoeol()
        /
        / DESCRIPTION:
        /       Read a string from the keyboard.
        /       This routine is specially designed to:
        /
        /           - strip non-printing characters (unless Wizard)
        /           - echo, if desired
        /           - redraw the screen if CH_REDRAW is entered
        /           - read in only 'mx - 1' characters or less characters
        /           - nul-terminate string, and throw away newline
        /
        /       'mx' is assumed to be at least 2.
        /
        *************************************************************************/

        int Do_get_next_tag()
        {
            Debug.Log("phantasia.init.Do_get_next_tag");
            CLibFile.CFILE the_file;
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            tag_t readTag = new tag_t();
            tagged_t readTagged = new tagged_t();
            int lastTagNumber = 0;

            int errno = 0;
            if ((the_file = CLibFile.fopen(pathnames.TAG_FILE, "r", ref errno)) != null)
            {

                /* run through the tag entries */
                while (CLibFile.fread(ref readTag, phantdefs.SZ_TAG, 1, the_file) == 1)
                {

                    /* see if this is the highest numbered tag */
                    if (readTag.number > lastTagNumber)
                    {
                        lastTagNumber = readTag.number;
                    }
                }


                CLibFile.fclose(the_file);
            }

            errno = 0;
            if ((the_file = CLibFile.fopen(pathnames.TAGGED_FILE, "r", ref errno)) != null)
            {

                /* run through the tag entries */
                while (CLibFile.fread(ref readTagged, phantdefs.SZ_TAGGED, 1, the_file) == 1)
                {

                    /* see if this is the highest numbered tag */
                    if (readTagged.tagNumber > lastTagNumber)
                    {
                        lastTagNumber = readTagged.tagNumber;
                    }
                }


                CLibFile.fclose(the_file);
            }

            return lastTagNumber + 1;
        }
    }
}
