using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace phantasiaclasses
{
    public class main //: MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        static main Instance;
        private main()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.Log("Warning: attempt to create duplicate instance of singleton " + this);
                ////Destroy(this);
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
            initclass = init.GetInstance();
        }
        public static main GetInstance()
        {
            main instance = null;
            if (Instance != null)
            {
                instance = Instance;
            }
            else
            {
                instance = new main();
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
        phantasiaclasses.restore restoreclass;
        phantasiaclasses.eventsrc eventclass;
        phantasiaclasses.hack hackclass;
        phantasiaclasses.character characterclass;
        phantasiaclasses.account accountclass;
        phantasiaclasses.init initclass;

        /* global variables */
        //int server_hook; //handled in CFUNCTIONS
        //string randomStateBuffer; //[STATELEN]; //handled in CFUNCTIONS
        //CFUNCTIONS.random_data randData; //handled in CFUNCTIONS


        /***************************************************************************
        / FUNCTION NAME: main()
        /
        / FUNCTION: initialize state, and call main process
        /
        / AUTHOR: E. A. Estes, 12/4/85
        / MODIFIED:  Brian Kelly, 4/6/99
        /
        / ARGUMENTS:
        /       int     argc - argument count
        /       char    **argv - argument vector
        /
        / RETURN VALUE: none
        /
        / MODULES CALLED: monstlist(), checkenemy(), activelist(),
        /       throneroom(), checkbattle(), readmessage(), changestats(), writerecord()
        ,
        /       tradingpost(), adjuststats(), recallplayer(), displaystats(), checktampe
        red(),
        /       Mathf.Abs(), rollnewplayer(), time(), exit(), cfunctions.sqrt(), cfunctions.floor(), wmove(),
        /       signal(), cfunctions.strcat(), purgeoldplayers(), getuid(), isatty(), wclear(),
        /       CFUNCTIONS.strcpy(ref ), system(), altercoordinates(), cleanup(), waddstr(), procmain()
        ,
        /       playinit(), leavegame(), localtime(), getanswer(), neatstuff(), initials
        tate(),
        /       scorelist(), titlelist()
        /
        / GLOBAL INPUTS: *Login, Throne, Wizard, Player, *stdscr, Changed, Databuf[],
        /       Fileloc, Stattable[]
        /
        / GLOBAL OUTPUTS: Wizard, Player, Changed, Fileloc, Timeout, *Statptr
        /
        / DESCRIPTION:
        /       Process arguments, initialize program, and loop forever processing
        /       player input.
        /
        ****************************************************************************/

        //int     argc - argument count
        //while loop steps down until count = 0
        //char** argv - argument vector, e.g. '-c-q-s'
        // c = purge characters
        // q = initialize, then bring down server
        // s = purge scoreboard

        public void mainmain(int argc, char[] argv)
        {
            Debug.Log("phantasia.main.mainmain");
            fileclass.Do_log_error("UNITY: STARTING GAME\n");

            /* set up the main server structure */
            server_t server = new server_t();

            /* set up a signal structre */
            //sigaction 
            LinuxLibSIG.sigaction sigAct = new LinuxLibSIG.sigaction();

            /* set the program to run */
            server.run_level = (short)phantdefs.RUN_SERVER;

            /* process arguments */
            while (--argc != 0 && argv[0] == '-') //originally ++argv
            {

                switch (argv[1])
                {

                    /* purge character file */
                    case 'c':
                        fileclass.Do_purge_characters();
                        break;

                    /* initialize, then bring down server */
                    case 'q':
                        server.run_level = (short)phantdefs.FAST_SHUTDOWN;
                        break;

                    /* purge scoreboard file */
                    case 's':
                        fileclass.Do_purge_scoreboard();
                        break;

                    default:    /* for all other answers */
                        CFUNCTIONS.printf("usage: phantasia [-c][-q][-s]\n");
                        CFUNCTIONS.printf("  -c: Purge character roster\n");
                        CFUNCTIONS.printf("  -q: Quit as soon as possible\n");
                        CFUNCTIONS.printf("  -s: Purge scoreboard\n");
                        CFUNCTIONS.exit(1);
                        break;
                }

                //added for unity
                string argvIncrementor = new string(argv);
                argvIncrementor = argvIncrementor.Substring(2);
                argv = argvIncrementor.ToCharArray();
            }

            /* block SIGINT, SIGTERM, SIGIO, SIGALRM and SIGUSR1 til we're ready */
            LinuxLibSIG.sigemptyset(ref sigAct.sa_mask);
            LinuxLibSIG.sigaddset(sigAct.sa_mask, LinuxLibSIG.SIGINT);
            LinuxLibSIG.sigaddset(sigAct.sa_mask, LinuxLibSIG.SIGTERM);
            LinuxLibSIG.sigaddset(sigAct.sa_mask, LinuxLibSIG.SIGIO);
            LinuxLibSIG.sigaddset(sigAct.sa_mask, LinuxLibSIG.SIGALRM);
            LinuxLibSIG.sigaddset(sigAct.sa_mask, LinuxLibSIG.SIGUSR1);

            /* this routine will block for created threads as well */
            if (CLibPThread.pthread_sigmask(LinuxLibSIG.SIG_BLOCK, sigAct.sa_mask, null) < 0)
            {
                CFUNCTIONS.printf("Error blocking signals.\n");
                CFUNCTIONS.exit(1);
            }

            /* set up structures, variables and files */
            initclass.Init_server(server);

            /* run the main loop */
            Do_main_loop(server);

            /* destroy structures and close files */
            initclass.Do_close(server);

            fileclass.Do_log(pathnames.SERVER_LOG, "Server is shut down.\n");
            CFUNCTIONS.exit(0); /* thanks for playing */
        }


        /***************************************************************************
        / FUNCTION NAME: Do_main_loop(struct server_t *server)
        /
        / FUNCTION: initialize state, and call main process
        /
        / AUTHOR:  Brian Kelly, 4/12/99
        /
        / ARGUMENTS: 
        /	struct server_t *s - address of the sever's main data structure
        /
        / RETURN VALUE: none
        /
        / MODULES CALLED: monstlist(), checkenemy(), activelist(),
        /       throneroom(), checkbattle(), readmessage(), changestats(), writerecord()
        ,
        /       tradingpost(), adjuststats(), recallplayer(), displaystats(), checktampe
        red(),
        /       Mathf.Abs(), rollnewplayer(), time(), exit(), cfunctions.sqrt(), cfunctions.floor(), wmove(),
        /       signal(), cfunctions.strcat(), purgeoldplayers(), getuid(), isatty(), wclear(),
        /       CFUNCTIONS.strcpy(ref ), system(), altercoordinates(), cleanup(), waddstr(), procmain()
        ,
        /       playinit(), leavegame(), localtime(), getanswer(), neatstuff(), initials
        tate(),
        /       scorelist(), titlelist()
        /
        / DESCRIPTION:
        /       Process arguments, initialize program, and loop forever processing
        /       player input.
        /
        ****************************************************************************/

        void Do_main_loop(server_t s)
        {
            Debug.Log("phantasia.main.Do_main_loop");
            LinuxLibSIG.sigset_t sigMask = new LinuxLibSIG.sigset_t();
            it_combat_t combat_ptr = new it_combat_t();
            game_t game_ptr = new game_t(), game_ptr_ptr = new game_t();
            event_t event_ptr = new event_t();
            int error = -1, theSignal = -1;
            int shutdownStart = -1;
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];

            error_msg = CFUNCTIONS.sprintfSinglestring("Server started up.  pid=%d\n",
                            (int)s.realm.serverPid);

            fileclass.Do_log(pathnames.SERVER_LOG, error_msg);


            LinuxLibSIG.sigemptyset(ref sigMask);
            LinuxLibSIG.sigaddset(sigMask, LinuxLibSIG.SIGINT);
            LinuxLibSIG.sigaddset(sigMask, LinuxLibSIG.SIGTERM);
            LinuxLibSIG.sigaddset(sigMask, LinuxLibSIG.SIGIO);
            /*
                sigaddset(&sigMask, SIGALRM);
            */
            LinuxLibSIG.sigaddset(sigMask, LinuxLibSIG.SIGUSR1);

            while (s.run_level != phantdefs.FAST_SHUTDOWN)
            {
                if (UnityGameController.StopApplication)
                {
                    Debug.Log("stopping server thread gracefully");
                    fileclass.Do_log_error("UNITY: STOPPING GAME\n");

                    s.run_level = (short)phantdefs.FAST_SHUTDOWN;
                    break;
                }
                else
                {
                    System.Threading.Thread.Sleep(33); //30fps
                }


                /* wait for a signal to do anything */
                LinuxLibSIG.sigwait(sigMask, ref theSignal);
                /*
                    sleep(.1);
                    theSignal = SIGIO;
                */

                /* on SIGIO, check for new connections and inactive games */
                if (theSignal == LinuxLibSIG.SIGIO)
                {
                    Debug.Log("DEBUG: MAIN LOOP: new connection detected");
                    /* if we're not shutting down check for new connections */
                    if (s.run_level == phantdefs.RUN_SERVER)
                    {
                        error = socketclass.Do_accept_connections(s, this);
                        if (error != 0)
                        {


                            error_msg = CFUNCTIONS.sprintfSinglestring(
                           "[0.0.0.0:%d] Do_accept_connections returned an error of %d.\n",
                           (int)s.realm.serverPid, error);


                            fileclass.Do_log_error(error_msg);
                            CFUNCTIONS.server_hook = phantdefs.SHUTDOWN;
                        }
                    }
                }

                /* a SIGUSER says to clean up a thread */
                else if (theSignal == LinuxLibSIG.SIGUSR1)
                {
                    Debug.Log("DEBUG: MAIN LOOP: cleanup detected");

                    /* check for inactive games */
                    miscclass.Do_lock_mutex(s.realm.realm_lock);
                    game_ptr_ptr = s.realm.games;

                    while (game_ptr_ptr != null)
                    {

                        if ((game_ptr_ptr).cleanup_thread)
                        {

                            game_ptr = game_ptr_ptr;

                            game_ptr_ptr = game_ptr.next_game;


                            LinuxLibSocket.close(game_ptr.the_socket);

                            CLibPThread.pthread_join(game_ptr.the_thread, null);

                            miscclass.Do_destroy_mutex(game_ptr.events_in_lock);


                            game_ptr = null; // free((void*) game_ptr);
                            --s.num_games;
                        }
                        else
                        {
                            game_ptr_ptr = ((game_ptr_ptr).next_game);
                        }
                    }


                    miscclass.Do_unlock_mutex(s.realm.realm_lock);
                }

                /* SIGTERM shuts the server down quickly */
                else if (theSignal == LinuxLibSIG.SIGTERM)
                {
                    Debug.Log("DEBUG: MAIN LOOP: term shutdown detected");
                    CFUNCTIONS.server_hook = phantdefs.FAST_SHUTDOWN;
                }

                /* SIGIO is a normal shutdown */
                else if (theSignal == LinuxLibSIG.SIGINT)
                {
                    CFUNCTIONS.server_hook = phantdefs.SHUTDOWN;
                }
                else
                {
                    
                    error_msg = CFUNCTIONS.sprintfSinglestring(
                        "[0.0.0.0:%d] sigwait caught bad signal %d in Do_main_loop.\n",
                        (int)s.realm.serverPid, theSignal);


                    fileclass.Do_log_error(error_msg);
                }

                /* check to see if a thread wants the server down */
                if (CFUNCTIONS.server_hook != s.run_level)
                {

                    if (CFUNCTIONS.server_hook == phantdefs.LEISURE_SHUTDOWN && s.num_games == 0)
                    {


                        fileclass.Do_log(pathnames.SERVER_LOG, "Starting a leisure shutdown.\n");
                        CFUNCTIONS.server_hook = phantdefs.SHUTDOWN;
                        shutdownStart = CFUNCTIONS.GetUnixEpoch(DateTime.Now);
                    }

                    /* if requested to die now */
                    else if (CFUNCTIONS.server_hook == phantdefs.HARD_SHUTDOWN)
                    {


                        fileclass.Do_log(pathnames.SERVER_LOG, "Hard shutdown ordered.  Exiting Now.\n");
                        /* hasta la vista, baby */
                        CFUNCTIONS.exit(0);
                    }

                    /* check for a quick shutdown */
                    else if (CFUNCTIONS.server_hook == phantdefs.FAST_SHUTDOWN)
                    {


                        fileclass.Do_log(pathnames.SERVER_LOG, "Starting a fast shutdown.\n");
                        s.run_level = (short)phantdefs.FAST_SHUTDOWN;
                    }

                    /* normal shutdown */
                    else if (CFUNCTIONS.server_hook == phantdefs.SHUTDOWN)
                    {


                        fileclass.Do_log(pathnames.SERVER_LOG, "Starting a normal shutdown.\n");

                        s.run_level = (short)phantdefs.SHUTDOWN;
                        shutdownStart = CFUNCTIONS.GetUnixEpoch(DateTime.Now);
                    }
                }

                /* If shutting down,  quit if nobody's on or time's expired */
                if (s.run_level == phantdefs.SHUTDOWN && (s.num_games == 0 ||

                CFUNCTIONS.GetUnixEpoch(DateTime.Now) - shutdownStart > 300))
                {

                    s.run_level = (short)phantdefs.FAST_SHUTDOWN;
                }
            }

            return;
        }


        /***************************************************************************
        / FUNCTION NAME: Do_start_thread(struct client_t *c)
        /
        / FUNCTION: Handle thread startup
        /
        / AUTHOR:  Brian Kelly, 4/23/99
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

        internal void Do_start_thread(object objC)//client_t c)    //unity: changed for c# thread-starting purposes
        {
            try //added for unity: debugging purposes
            {
                Debug.Log("Thread " + System.Threading.Thread.CurrentThread.Name + ": phantasia.main.Do_start_thread");

                client_t c = (client_t)objC; //added for unity per above

                int error;
                string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE], 
                string string_buffer = ""; //[phantdefs.SZ_LINE];
                char outout;
                LinuxLibSIG.sigaction sigAct;
                event_t eventPtr = new event_t();

                /* set the thread's process id */
                miscclass.Do_lock_mutex(c.realm.realm_lock);
                c.game.clientPid = miscclass.gettid();
                miscclass.Do_unlock_mutex(c.realm.realm_lock);

                //Debug.Log("record connection");
                /* record this connection */
                CFUNCTIONS.sprintf(ref c.connection_id, "?:%s:%d", c.IP, c.game.clientPid); //todo: debug

                //Debug.Log("log connection");
                error_msg = CFUNCTIONS.sprintfSinglestring("[%s] Connection on socket %d.\n",
                    c.connection_id, c.socket);
                //crashed sprintf due to overload confusion, and c.connection_id being null

                fileclass.Do_log(pathnames.CONNECTION_LOG, error_msg);

                /* set the socket so I/O notifies this thread */
                int errno = 0;
                if (LinuxLibSocket.fcntl(c.socket, LinuxLibSocket.F_SETOWN, c.game.clientPid, ref errno) < 0)
                {


                    error_msg = CFUNCTIONS.sprintfSinglestring(
                        "Thread %d returned an error on fcntl F_SETOWN.\n",
                        c.socket);


                    fileclass.Do_log_error(error_msg);

                    socketclass.Do_send_error(c, error_msg);
                    c.run_level = phantdefs.EXIT_THREAD;
                    c.socket_up = false;
                }

                if (LinuxLibSocket.fcntl(c.socket, CFUNCTIONS.F_SETFL, CFUNCTIONS.O_ASYNC, ref errno) < 0)
                {


                    error_msg = CFUNCTIONS.sprintfSinglestring(
                        "Thread %d returned an error on fcntl F_SETFL.\n",
                        c.socket);


                    fileclass.Do_log_error(error_msg);

                    socketclass.Do_send_error(c, error_msg);
                    c.run_level = phantdefs.EXIT_THREAD;
                    c.socket_up = false;
                }

                /* handshake the client */
                ioclass.Do_handshake(c);

                /* see if this socket should be rejected */
                tagsclass.Do_check_tags(c);

                /* modify stats from this ip - (checks for excessive connections) */
                hackclass.Do_tally_ip(c, true, 0);

                /* find all the current players and list them */
                miscclass.Do_lock_mutex(c.realm.realm_lock);
                characterclass.Do_starting_spec(c);
                c.game.sendEvents = true;
                miscclass.Do_unlock_mutex(c.realm.realm_lock);

                /* call the main thread loop */
                Do_thread_loop(c);

                /* clean up any events we may have */
                miscclass.Do_lock_mutex(c.realm.realm_lock);
                c.game.sendEvents = false;
                miscclass.Do_unlock_mutex(c.realm.realm_lock);
                eventclass.Do_check_events_in(c);

                /* pull death name out of limbo if necessary */
                if (c.previousName != null && c.previousName[0] != '\0')
                {

                    characterclass.Do_release_name(c, c.previousName);
                }

                /* clean up socket connection */
                if (c.socket_up)
                {
                    socketclass.Do_send_int(c, phantdefs.CLOSE_CONNECTION_PACKET);
                    socketclass.Do_send_buffer(c);
                }

                /* log the connection */
                error_msg = CFUNCTIONS.sprintfSinglestring("[%s] Leaving the game after %d seconds.\n",
                    c.connection_id, CFUNCTIONS.GetUnixEpoch(DateTime.Now) - c.date_connected);

                fileclass.Do_log(pathnames.CONNECTION_LOG, error_msg);

                /* tell the server to clean up */
                c.game.cleanup_thread = true;
                //c = null; // free((void*) c);   //why is this freed up when it's immediately used in the kill command...
                CFUNCTIONS.kill(c.realm.serverPid, LinuxLibSIG.SIGUSR1);

                /* leave the game */
                CLibPThread.pthread_exit(0);
            }
            catch (Exception e)
            {
                if (!e.Message.Contains("Thread was being aborted")) //todo: this is for debug, filtering out 'thread aborted while sleeping' exception
                {
                    Debug.LogError("Exception in client C thread: " + e.Message + " || " + e.InnerException);
                    Debug.LogError(e.StackTrace);
                }
                else
                {
                    Debug.Log("<color=red>Exception in client C thread: " + e.Message + " || " + e.InnerException + " ||</color>");
                    Debug.Log(e.StackTrace);
                }
            }
        }


        /***************************************************************************
        / FUNCTION NAME: Do_thread_loop(struct client_t *c)
        /
        / FUNCTION: The main loop for threads
        /
        / AUTHOR:  Brian Kelly, 4/23/99
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

        void Do_thread_loop(client_t c)
        {
            Debug.Log("phantasia.main.Do_thread_loop");
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];

            /* start the main loop */
            while (c.run_level != phantdefs.EXIT_THREAD)
            {
                if (UnityGameController.StopApplication) //time to quit
                {
                    Debug.Log("Thread " + System.Threading.Thread.CurrentThread.Name + " stopping in Do_thread_loop");
                    break;
                }
                else
                {
                    System.Threading.Thread.Sleep(33); //30fps
                }

                /* make characters log in first */
                if (c.run_level == phantdefs.SIGNING_IN)
                {

                    /* print out the title page */
                    miscclass.Do_title_page(c);

                    /* have the user log in */
                    accountclass.Do_get_account(c);

                    /* see if the game is shutting down */
                    miscclass.Do_shutdown_check(c);
                }

                /* if the player has no character - get one */
                if (c.run_level == phantdefs.CHAR_SELECTION)
                {

                    /* empty out the player strcture */
                    statsclass.Do_init_player(c);

                    /* Get a character */
                    characterclass.Do_get_character(c);
                }

                if (c.run_level == phantdefs.PLAY_GAME)
                {
                    
                    /* approve the entrance */
                    characterclass.Do_approve_entrance(c);
                }

                /* if ready to play, enter the game */
                if (c.run_level == phantdefs.PLAY_GAME)
                {
                    /* announce character entrance */
                    characterclass.Do_entering_character(c);
                    
                    System.Threading.Thread.Sleep(33);
                    /* play the game */
                    Do_play_loop(c);

                    /* remove the character from play */
                    characterclass.Do_leaving_character(c);
                }

                /* see if the character needs to be saved */
                if (c.run_level == phantdefs.SAVE_AND_CONTINUE ||
                    c.run_level == phantdefs.SAVE_AND_EXIT)
                {
                    
                    characterclass.Do_handle_save(c);
                }

                /* regardless, remove the backup */
                else
                {

                    characterclass.Do_backup_save(c, 0);
                }
                
                /* ask if player wishes to go again */
                if (c.run_level == phantdefs.GO_AGAIN)
                {
                    /* ask if the player wants to continue */
                    ioclass.Do_ask_continue(c);
                }
                
                /* see if the game is shutting down */
                miscclass.Do_shutdown_check(c);
            }
            return;
        }


        /***************************************************************************
        / FUNCTION NAME: Do_play_loop(struct client_t *c)
        /
        / FUNCTION: Default activity when no events are pending
        /
        / AUTHOR:  Brian Kelly, 4/23/99
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

        void Do_play_loop(client_t c)
        {
            Debug.Log("phantasia.main.Do_play_loop");
            event_t event_ptr = new event_t();
            
            while (c.run_level == phantdefs.PLAY_GAME
                   || (c.events != null && c.player.energy > 0))
            {
                if (UnityGameController.StopApplication) //time to quit
                {
                    Debug.Log("Thread " + System.Threading.Thread.CurrentThread.Name + " stopping in Do_play_loop");
                    break;
                }
                else
                {
                    System.Threading.Thread.Sleep(33); //30fps
                }
               
                /* age the player */
                statsclass.Do_age(c);

                /* if there are events to handle */
                if (c.events != null)
                {

                    /* remove the next event */
                    event_ptr = c.events;
                    c.events = event_ptr.next_event;

                    /* take care of it */
                    eventclass.Do_handle_event(c, event_ptr);
                }
                else
                {

                    /* If no events, start a new turn */
                    Do_game_turn(c);

                    /* check for monsters, guru's, etc. */
                    if (!c.player.cloaked
                               && (c.wizard < 3)
                               && (c.player.location == phantdefs.PL_REALM || c.player.location == phantdefs.PL_EDGE)
                               && (c.run_level == phantdefs.PLAY_GAME))


                        Do_random_events(c);
                }
                
                /* file any events from outside the thread */
                eventclass.Do_check_events_in(c);
                
                /* see if the game is shutting down */
                miscclass.Do_shutdown_check(c);

                /* if the socket is down, leave the game */
                if (!c.socket_up && c.run_level == phantdefs.PLAY_GAME)
                {
                    c.run_level = (short)phantdefs.SAVE_AND_EXIT;
                }
            }

            return;
        }


        /***************************************************************************
        / FUNCTION NAME: Do_game_turn(struct client_t *c)
        /
        / FUNCTION: Default activity when no events are pending
        /
        / AUTHOR:  E. A. Estes, 12/4/85
        /	   Brian Kelly, 5/8/99
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

        void Do_game_turn(client_t c)
        {
            Debug.Log("phantasia.main.Do_game_turn");
            event_t event_ptr = new event_t();
            button_t buttons = new button_t();                     /* input */
            long ch = -1;
            int rc;
            int loop;
            string string_buffer = ""; //[phantdefs.SZ_LINE];
            double dtemp = -1;
            bool postFlag;
            float ftemp;

            if (c.player.location == phantdefs.PL_EDGE)
            {


                ioclass.Do_send_line(c,
                   "An old wooden sign here says, 'This is the edge of the realm.\n");


                ioclass.Do_send_line(c, "I would strongly advise going no further.'\n");
            }

            /* see if the player is on a trading post  and not cloaked */
            postFlag = false;
            if (Mathf.Abs((float)c.player.x) == Mathf.Abs((float)c.player.y) && c.player.location !=
                        phantdefs.PL_THRONE && !c.player.cloaked)
            {

                dtemp = CFUNCTIONS.sqrt(Mathf.Abs((float)c.player.x) / 100.0);

                if (CFUNCTIONS.floor(dtemp) == dtemp)
                {
                    postFlag = true;

                    ioclass.Do_send_line(c, "You stand in front of a trading post.\n");
                }
            }

            ioclass.Do_clear_buttons(buttons, 0);

            /*
                if (c.player.special_type != phantdefs.SC_VALAR) {
            */
            CFUNCTIONS.strcpy(ref buttons.button[0], "Rest\n");
            /*
                }
            */

            /* print status line */
            CFUNCTIONS.strcpy(ref buttons.button[1], "Move To\n");
            buttons.compass = '1';// true; //todo
            CFUNCTIONS.strcpy(ref buttons.button[2], "Info\n");
            CFUNCTIONS.strcpy(ref buttons.button[7], "Quit\n");
            /*
            examine
            rest
            do nothing
            */
            if (postFlag)
            {
                CFUNCTIONS.strcpy(ref buttons.button[3], "Enter Post\n");
            }
            else if (!c.player.cloaked && c.player.location == phantdefs.PL_REALM)
            {
                CFUNCTIONS.strcpy(ref buttons.button[3], "Hunt\n");
            }

            if (c.player.cloaked)
            {
                CFUNCTIONS.strcpy(ref buttons.button[4], "Uncloak\n");
            }
            else if (c.player.level >= phantdefs.MEL_CLOAK && c.player.magiclvl >= phantdefs.ML_CLOAK)
            {
                CFUNCTIONS.strcpy(ref buttons.button[4], "Cloak\n");
            }

            /*
                if (c.player.level >= MEL_TELEPORT && c.player.magiclvl >= ML_TELEPORT &&
            	    c.player.special_type != phantdefs.SC_VALAR) {
            */
            if (c.player.level >= phantdefs.MEL_TELEPORT && c.player.magiclvl >= phantdefs.ML_TELEPORT &&
                !c.player.cloaked)
            {

                CFUNCTIONS.strcpy(ref buttons.button[5], "Teleport\n");
            }


            if (c.wizard == 2)
            {
                CFUNCTIONS.strcpy(ref buttons.button[6], "Cantrip\n");
            }
            else if (c.wizard > 3)
            {
                CFUNCTIONS.strcpy(ref buttons.button[6], "Administrate\n");
            }
            else if (c.wizard != 0)
            {
                CFUNCTIONS.strcpy(ref buttons.button[6], "Moderate\n");
            }
            else if (c.player.location == phantdefs.PL_THRONE && c.player.special_type
                == phantdefs.SC_STEWARD && !c.player.cloaked)
            {

                CFUNCTIONS.strcpy(ref buttons.button[6], "Enact\n");
            }
            else if (c.player.location == phantdefs.PL_THRONE && c.player.special_type
                == phantdefs.SC_KING && !c.player.cloaked)
            {

                CFUNCTIONS.strcpy(ref buttons.button[6], "Decree\n");
            }
            else if ((c.player.special_type == phantdefs.SC_COUNCIL || c.player.special_type
            == phantdefs.SC_EXVALAR) && !c.player.cloaked)
            {

                CFUNCTIONS.strcpy(ref buttons.button[6], "Intervene\n");
            }
            else if (c.player.special_type == phantdefs.SC_VALAR && !c.player.cloaked)
            {
                CFUNCTIONS.strcpy(ref buttons.button[6], "Command\n");
            }
            else if (c.player.level < 10)
            {
                CFUNCTIONS.strcpy(ref buttons.button[6], "Help\n");
            }

            if (ioclass.Do_buttons(c, ref ch, buttons) != phantdefs.S_NORM)
            {
                ioclass.Do_send_clear(c);
                return;
            }

            ioclass.Do_send_clear(c);


            /* if the valar asks to move, change his request to a null */
            /*
                if (c.player.special_type == phantdefs.SC_VALAR && (ch == 0 || (ch >= 8 &&
            	    ch != 12))) { 

            	ch = 127;	
                }
            */

            /* create an event to handle the player action */
            event_ptr = eventclass.Do_create_event();
            event_ptr.to = c.game;
            event_ptr.from = c.game;

            switch (ch)
            {

                case 8:               /* move north-west */
                    event_ptr.type = (short)phantdefs.MOVE_EVENT;
                    event_ptr.arg1 = c.player.x - miscclass.Do_anglemove(c);
                    event_ptr.arg2 = c.player.y + miscclass.Do_anglemove(c);
                    event_ptr.arg3 = phantdefs.A_SPECIFIC;
                    break;

                case 9:               /* move up/north */
                    event_ptr.type = (short)phantdefs.MOVE_EVENT;
                    event_ptr.arg1 = c.player.x;
                    event_ptr.arg2 = c.player.y + miscclass.Do_maxmove(c);
                    event_ptr.arg3 = phantdefs.A_SPECIFIC;
                    break;

                case 10:               /* move north-east */
                    event_ptr.type = (short)phantdefs.MOVE_EVENT;
                    event_ptr.arg1 = c.player.x + miscclass.Do_anglemove(c);
                    event_ptr.arg2 = c.player.y + miscclass.Do_anglemove(c);
                    event_ptr.arg3 = phantdefs.A_SPECIFIC;
                    break;

                case 11:               /* move left/west */
                    event_ptr.type = (short)phantdefs.MOVE_EVENT;
                    event_ptr.arg1 = c.player.x - miscclass.Do_maxmove(c);
                    event_ptr.arg2 = c.player.y;
                    event_ptr.arg3 = phantdefs.A_SPECIFIC;
                    break;

                case 13:               /* move right/east */
                    event_ptr.type = (short)phantdefs.MOVE_EVENT;
                    event_ptr.arg1 = c.player.x + miscclass.Do_maxmove(c);
                    event_ptr.arg2 = c.player.y;
                    event_ptr.arg3 = phantdefs.A_SPECIFIC;
                    break;

                case 14:               /* move south-west */
                    event_ptr.type = (short)phantdefs.MOVE_EVENT;
                    event_ptr.arg1 = c.player.x - miscclass.Do_anglemove(c);
                    event_ptr.arg2 = c.player.y - miscclass.Do_anglemove(c);
                    event_ptr.arg3 = phantdefs.A_SPECIFIC;
                    break;

                case 15:               /* move down/south */
                    event_ptr.type = (short)phantdefs.MOVE_EVENT;
                    event_ptr.arg1 = c.player.x;
                    event_ptr.arg2 = c.player.y - miscclass.Do_maxmove(c);
                    event_ptr.arg3 = phantdefs.A_SPECIFIC;
                    break;

                case 16:               /* move south-west */
                    event_ptr.type = (short)phantdefs.MOVE_EVENT;
                    event_ptr.arg1 = c.player.x + miscclass.Do_anglemove(c);
                    event_ptr.arg2 = c.player.y - miscclass.Do_anglemove(c);
                    event_ptr.arg3 = phantdefs.A_SPECIFIC;
                    break;

                case 1:               /* move */

                    if (ioclass.Do_coords_dialog(c, event_ptr.arg1, event_ptr.arg2,
                    "Where do you wish to move to?\n") != 0)
                    {

                        break;
                    }

                    miscclass.Do_distance(c.player.x, event_ptr.arg1, c.player.y,
                    event_ptr.arg2, ref dtemp);

                    if (dtemp > miscclass.Do_maxmove(c))
                    {
                        ioclass.Do_send_line(c, "That's too far to move in 1 step.  Get closer by using the compass buttons in the lower-right corner.\n");

                        ioclass.Do_more(c);

                        ioclass.Do_send_clear(c);
                    }
                    else
                    {
                        event_ptr.type = (short)phantdefs.MOVE_EVENT;
                        event_ptr.arg3 = phantdefs.A_SPECIFIC;
                    }

                    break;

                /* get information */
                case 2:
                    event_ptr.type = (short)phantdefs.INFORMATION_EVENT;
                    break;

                case 7:               /* good-bye */
                    c.run_level = (short)phantdefs.SAVE_AND_CONTINUE;
                    break;

                case 4:               /* cloak */

                    if (c.player.cloaked || (c.player.level >= phantdefs.MEL_CLOAK &&
                    c.player.magiclvl >= phantdefs.ML_CLOAK))
                    {

                        event_ptr.type = (short)phantdefs.CLOAK_EVENT;
                    }
                    break;

                case 5:         /* teleport */
                    event_ptr.type = (short)phantdefs.TELEPORT_EVENT;
                    event_ptr.arg2 = 0;        /* not Gwaihir */
                    if (c.player.location == phantdefs.PL_THRONE)
                    {
                        event_ptr.arg3 = 0; /* teleport costs no mana */
                    }
                    else
                    {
                        event_ptr.arg3 = 1; /* teleport costs mana */
                    }
                    break;

                case 6:               /* decree and intervention */

                    /* cantrips handled first since all other actions are available
                       off the cantrip menu */
                    if (c.wizard == 2)
                    {
                        event_ptr.type = (short)phantdefs.CANTRIP_EVENT;
                    }
                    else if (c.wizard > 3)
                    {
                        event_ptr.type = (short)phantdefs.ADMINISTRATE_EVENT;
                    }
                    else if (c.wizard != 0)
                    {
                        event_ptr.type = (short)phantdefs.MODERATE_EVENT;
                    }
                    /* see if the player is allowed to enact */
                    else if (c.player.special_type == phantdefs.SC_STEWARD && c.player.location
                    == phantdefs.PL_THRONE && !c.player.cloaked)
                    {

                        event_ptr.type = (short)phantdefs.ENACT_EVENT;
                    }
                    /* see if the player is allowed to decree */
                    else if (c.player.special_type == phantdefs.SC_KING && c.player.location
                    == phantdefs.PL_THRONE && !c.player.cloaked)
                    {

                        event_ptr.type = (short)phantdefs.DECREE_EVENT;
                    }
                    else if ((c.player.special_type == phantdefs.SC_COUNCIL ||
                    c.player.special_type == phantdefs.SC_EXVALAR) && !c.player.cloaked)
                    {

                        event_ptr.type = (short)phantdefs.INTERVENE_EVENT;
                    }
                    else if (c.player.special_type == phantdefs.SC_VALAR && !c.player.cloaked)
                    {
                        event_ptr.type = (short)phantdefs.COMMAND_EVENT;
                    }
                    else if (c.player.level < 10)
                    {
                        event_ptr.type = (short)phantdefs.HELP_EVENT;
                    }
                    break;

                case 3:               /* hunt */

                    /* the valar cannot call monsters and no monsters on throne */
                    /* being cloaked is right out too */
                    if (postFlag)
                    {
                        event_ptr.type = (short)phantdefs.TRADING_EVENT;
                    }
                    else if (!c.player.cloaked && c.player.location == phantdefs.PL_REALM)
                    {


                        statsclass.Do_sin(c, .001);
                        event_ptr.type = (short)phantdefs.MONSTER_EVENT;
                        event_ptr.arg1 = phantdefs.MONSTER_CALL;
                        event_ptr.arg3 = phantdefs.SM_RANDOM;	/* pick a monster normally */
                    }
                    break;

                case 0:
                case 12:            /* rest */
                    event_ptr.type = (short)phantdefs.REST_EVENT;
                    break;

                    /*    default: phantdefs.NULL_EVENT	stupid people deserve no reward */
            }

            if (event_ptr.type != phantdefs.NULL_EVENT)
            {

                eventclass.Do_handle_event(c, event_ptr);
            }
            else
            {

                event_ptr = null; // free((void*) event_ptr);
            }

            return;
        }


        /***************************************************************************
        / FUNCTION NAME: mainclass.Do_random_events(struct client_t *c)
        /
        / FUNCTION: Default activity when no events are pending
        /
        / AUTHOR:  Brian Kelly, 8/17/99
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

        internal void Do_random_events(client_t c)
        {
            Debug.Log("phantasia.main.Do_random_events");
            event_t event_ptr = new event_t();

            /* to shred "backups" */
            if ((c.player.quickness == 0)
                 && (macros.RND() >= .1 * c.player.degenerated))
            {
                event_ptr = eventclass.Do_create_event();
                event_ptr.type = (short)phantdefs.MONSTER_EVENT;
                event_ptr.arg1 = phantdefs.MONSTER_RANDOM;
                event_ptr.arg3 = 16;

                eventclass.Do_file_event(c, event_ptr);
            }

            if (c.player.blind && macros.RND() <= 0.0075)
            {

                ioclass.Do_send_line(c, "You've regained your sight!\n");
                c.player.blind = false;

                /* update the player description */
                miscclass.Do_lock_mutex(c.realm.realm_lock);

                characterclass.Do_player_description(c);

                miscclass.Do_unlock_mutex(c.realm.realm_lock);


                ioclass.Do_more(c);

                ioclass.Do_send_clear(c);
            }

            if (macros.RND() <= 0.0133)
            {

                eventclass.Do_send_self_event(c, phantdefs.MEDIC_EVENT);
            }

            if (macros.RND() <= 0.0075)
            {

                eventclass.Do_send_self_event(c, phantdefs.GURU_EVENT);
            }

            if (macros.RND() <= 0.005)
            {
                event_ptr = (event_t)eventclass.Do_create_event();
                event_ptr.type = (short)phantdefs.PLAGUE_EVENT;
                event_ptr.arg3 = 0;

                eventclass.Do_file_event(c, event_ptr);
            }

            if (macros.RND() <= 0.0075)
            {

                eventclass.Do_send_self_event(c, phantdefs.VILLAGE_EVENT);
            }

            if (c.player.level < 3000)
            {
                if (macros.RND() <= 0.0033 + (c.player.level * .00000125))
                {

                    eventclass.Do_send_self_event(c, phantdefs.TAX_EVENT);
                }
            }
            else if (macros.RND() <= 0.0033)
            {

                eventclass.Do_send_self_event(c, phantdefs.TAX_EVENT);
            }

            if (macros.RND() <= 0.015)
            {
                event_ptr = eventclass.Do_create_event();
                event_ptr.type = (short)phantdefs.TREASURE_EVENT;
                event_ptr.arg1 = c.player.circle;
                event_ptr.arg3 = 1;

                eventclass.Do_file_event(c, event_ptr);
            }

            if (macros.RND() <= 0.0075)
            {
                event_ptr = eventclass.Do_create_event();
                event_ptr.type = (short)phantdefs.TREASURE_EVENT;
                event_ptr.arg1 = c.player.circle;
                event_ptr.arg3 = 2;

                eventclass.Do_file_event(c, event_ptr);
            }

            if (macros.RND() <= 0.20)
            {
                event_ptr = eventclass.Do_create_event();
                event_ptr.type = (short)phantdefs.MONSTER_EVENT;
                event_ptr.arg1 = phantdefs.MONSTER_RANDOM;
                event_ptr.arg3 = phantdefs.SM_RANDOM;

                eventclass.Do_file_event(c, event_ptr);
            }
        }

    }
}
