using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace phantasiaclasses
{
    public class socket //: MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        static socket Instance;
        private socket()
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
            infoclass = info.GetInstance();
            treasureclass = treasure.GetInstance();
            restoreclass = restore.GetInstance();
            itcombatclass = itcombat.GetInstance();
            fightclass = fight.GetInstance();
            initclass = init.GetInstance();
        }
        public static socket GetInstance()
        {
            socket instance = null;
            if (Instance != null)
            {
                instance = Instance;
            }
            else
            {
                instance = new socket();
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
        phantasiaclasses.stats statsclass;
        phantasiaclasses.itcombat itcombatclass;
        phantasiaclasses.fight fightclass;
        phantasiaclasses.treasure treasureclass;
        phantasiaclasses.eventsrc eventclass;
        phantasiaclasses.hack hackclass;
        phantasiaclasses.character characterclass;
        phantasiaclasses.account accountclass;
        phantasiaclasses.init initclass;

        //extern void Do_start_thread(client_t c); //using main.Do_start_thread via reference instead
        //  extern object server_hook; //handled in CFUNCTIONS
        // extern object randData; //handled in CFUNCTIONS

        /************************************************************************
        /
        / FUNCTION NAME: Do_initialize_socket(struct server_t *server)
        /
        / FUNCTION: To initialize the program's socket
        /
        / AUTHOR: Brian Kelly, 4/12/99
        /
        / ARGUMENTS:
        /       struct server_t *s - address of the server's main data strcture
        /
        / RETURN VALUE: 
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

        internal int Do_init_server_socket()
        {
            Debug.Log("Thread " + System.Threading.Thread.CurrentThread.Name + ": phantasia.socket.Do_init_server_socket");

            LinuxLibSocket.sockaddr_in bind_address = new LinuxLibSocket.sockaddr_in(); 
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            int the_socket = 0, error = 0, on = 1;

            /* create a socket */
            int errno = 0;
            if ((the_socket = LinuxLibSocket.socket((int)LinuxLibSocket.AF_INET, LinuxLibSocket.SOCK_STREAM, 0, ref errno)) == -1)
            {

                error_msg = CFUNCTIONS.sprintfSinglestring(
               "[0.0.0.0:?] Socket creation failed in Do_init_server_socket: %s\n",
                CFUNCTIONS.strerror(errno));

                fileclass.Do_log_error(error_msg);
                CFUNCTIONS.exit(phantdefs.SOCKET_CREATE_ERROR);
            }

            error = LinuxLibSocket.setsockopt(the_socket, LinuxLibSocket.SOL_SOCKET, (LinuxLibSocket.SO_REUSEADDR ? 1 : 0), (char)on, LinuxLibSocket.Csizeof(on), ref errno);

            if (error != 0)
            {

                error_msg = CFUNCTIONS.sprintfSinglestring("[0.0.0.0:?] setsockopt failed with error code of %d in Do_init_server_socket.\n", error);

                fileclass.Do_log_error(error_msg);
                CFUNCTIONS.exit(phantdefs.SOCKET_CREATE_ERROR);
            }

            /* set up the bind address */
            bind_address.sin_family = LinuxLibSocket.AF_INET;
            bind_address.sin_addr.s_addr = CFUNCTIONS.INADDR_ANY;
            bind_address.sin_port = phantdefs.PHANTASIA_PORT;

            /* bind to that socket */
            error = LinuxLibSocket.bind(the_socket, (LinuxLibSocket.sockaddr)bind_address, LinuxLibSocket.Csizeof(bind_address), ref errno); //C#: inherited sockaddr_in from sockaddr as a workaround

            if (error != 0)
            {

                error_msg = CFUNCTIONS.sprintfSinglestring("[0.0.0.0:?] bind to socket failed with error code of %d in Do_init_server_socket.\n", error);

                fileclass.Do_log_error(error_msg);
                CFUNCTIONS.exit(phantdefs.SOCKET_BIND_ERROR);
            }

            /* start listening on the socket */
            error = LinuxLibSocket.listen(the_socket, LinuxLibSocket.SOMAXCONN, ref errno);
            if (error != 0)
            {

                error_msg = CFUNCTIONS.sprintfSinglestring("[0.0.0.0:?] listen command failed with error code of %d in Do_init_server_socket\n", error);

                fileclass.Do_log_error(error_msg);
                CFUNCTIONS.exit(phantdefs.SOCKET_LISTEN_ERROR);
            }

            if ((error = LinuxLibSocket.fcntl(the_socket, LinuxLibSocket.F_SETOWN, CLibPThread.gettid(), ref errno)) < 0)
            {


                error_msg = CFUNCTIONS.sprintfSinglestring("[0.0.0.0:?] fcntl F_SETOWN failed with error code of %d in Do_init_server_socket.\n", error);

                fileclass.Do_log_error(error_msg);

                CFUNCTIONS.exit(phantdefs.SOCKET_BIND_ERROR);
            }

            if ((error = LinuxLibSocket.fcntl(the_socket, CFUNCTIONS.F_SETFL, CFUNCTIONS.O_ASYNC, ref errno)) < 0) //todo: O_ASYNC? currently read as pid, likely different overload expected
            {


                error_msg = CFUNCTIONS.sprintfSinglestring("[0.0.0.0:?] fcntl F_SETFL failed with error code of %d in Do_init_server_socket.\n", error);

                fileclass.Do_log_error(error_msg);

                CFUNCTIONS.exit(phantdefs.SOCKET_BIND_ERROR);
            }

            return the_socket; /* no problems */
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_accept_connections(struct server_t *s)
        /
        / FUNCTION: Create new games for new connections on the socket
        /
        / AUTHOR: Brian Kelly, 4/12/99
        /
        / ARGUMENTS:
        /       struct server_t *s - address of the server's main data strcture
        /
        / RETURN VALUE: int error
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

        internal int Do_accept_connections(server_t s, main source) //added ref to main to give access to do_start_thread
        {
            Debug.Log("Thread " + System.Threading.Thread.CurrentThread.Name + ": phantasia.socket.Do_accept_connections");
            
            game_t game_ptr = new game_t();
            client_t client_ptr = new client_t();
            CLibPThread.pthread_attr_t thread_attr = new CLibPThread.pthread_attr_t();
            int addrlen = 0;
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            int theError, on = 1, terms, itemp;
            char[] gethost_buffer;//[16384]
            char[] string_ptr, string_ptr_two;
            LinuxLibSocket.hostent host_info, host_buffer;
            LinuxLibSocket.in_addr theNetwork = new LinuxLibSocket.in_addr();

            while (Do_check_socket(s.the_socket) != 0)
            {
                //if (UnityGameController.stopApplication)
                //{
                //    Debug.Log("stopping in Do_accept_connections");
                //    return 1;
                //}

                /* on a new connection, seed the random number generator */
                CFUNCTIONS.srandom_r(CFUNCTIONS.time(null), CFUNCTIONS.randData);

                /* create a structure for the thread information */
                client_ptr = new client_t();// (client_t) Do_malloc(phantdefs.SZ_CLIENT);

                /* accept the new connection */
                addrlen = LinuxLibSocket.Csizeof(client_ptr.address);

                int errno = 0;
                client_ptr.socket = LinuxLibSocket.accept(s.the_socket, (LinuxLibSocket.sockaddr)client_ptr.address, addrlen, ref errno);

                if (client_ptr.socket == -1)
                {

                    error_msg = CFUNCTIONS.sprintfSinglestring(
               "[0.0.0.0:%d] accept socket failed in Do_accept_connections: %s.\n",
               (int)s.realm.serverPid, CFUNCTIONS.strerror(errno));


                    fileclass.Do_log_error(error_msg);
                    client_ptr = null; // free((void*) client_ptr);
                    return phantdefs.SOCKET_ACCEPT_ERROR;
                }

                /* log the connection */
                error_msg = CFUNCTIONS.sprintfSinglestring("New connection on socket %d.\n",
                                client_ptr.socket);


                fileclass.Do_log(pathnames.SERVER_LOG, error_msg);

                /* set the socket variables */
                client_ptr.socket_up = true;
                client_ptr.out_buffer_size = 0;
                client_ptr.in_buffer_size = 0;
                client_ptr.date_connected = CFUNCTIONS.GetUnixEpoch(DateTime.Now);
                client_ptr.run_level = (short)phantdefs.SIGNING_IN;
                client_ptr.machineID = 0;
                client_ptr.events = null;
                client_ptr.channel = 1;
                client_ptr.timeout = 120;
                client_ptr.timeoutFlag = 0;
                client_ptr.broadcast = false;
                client_ptr.stuck = false;
                client_ptr.knightEnergy = 0;
                client_ptr.knightQuickness = 0;
                client_ptr.ageCount = 0;
                client_ptr.morgothCount = 0;
                client_ptr.suspended = false;
                client_ptr.accountLoaded = false;
                client_ptr.characterLoaded = false;
                client_ptr.characterAnnounced = false;
                client_ptr.muteUntil = 0;
                client_ptr.tagUntil = 0;
                client_ptr.hearBroadcasts = true;
                client_ptr.swearCount = 0;
                client_ptr.battle.rounds = 0;
                client_ptr.battle.timeouts = 0;
                client_ptr.previousName = "\0";
                client_ptr.wizaccount = "\0";
                client_ptr.wizIP = "\0";

                for (terms = 0; terms < 10; terms++)
                {
                    client_ptr.chatTimes[terms] = 0;
                    client_ptr.chatLength[terms] = 0;
                }


                /* set the from and name fields */
                /* determine the dns entry of this connection */
                /* I've had major problems getting the reentrant fuction
                working with linux as it is not standard.  I hope the normal
                function run in main process is okay */
                host_info = null;
                theError = 0;
                /*
                        gethostbyaddr_r((char *)&client_ptr.address.sin_addr,
                                sizeof(client_ptr.address.sin_addr), AF_INET, &host_buffer,
                        gethost_buffer, 16384, &host_info, &theError);
                */

                /*
                    errno = 0;
                    host_info = gethostbyaddr((char *)&client_ptr.address.sin_addr, 
                        sizeof(client_ptr.address.sin_addr), AF_INET);
                    theError = errno;
                */

                /* if we received host information */
                if (host_info != null)
                {

                    /* copy over the hostname */
                    CFUNCTIONS.strncpy(ref client_ptr.IP, host_info.h_name, phantdefs.SZ_FROM - 1);
                    client_ptr.IP = client_ptr.IP.Substring(0, phantdefs.SZ_FROM - 1);
                    char[] IPChar = new char[client_ptr.IP.Length + 1];
                    client_ptr.IP.ToCharArray().CopyTo(IPChar, 0);
                    IPChar[client_ptr.IP.Length] = '\0';
                    client_ptr.IP = new string(IPChar); //[phantdefs.SZ_FROM - 1] = '\0';

                    /* determine the network address of this connection */
                    string_ptr = new char[client_ptr.IP.Length + 1];
                    client_ptr.IP.ToCharArray().CopyTo(string_ptr, 0);
                    string_ptr[client_ptr.IP.Length] = '\0';

                    /* skip the first term (assume host name) */
                    int currentIndex = 0;
                    while (string_ptr[currentIndex] != '\0' && string_ptr[currentIndex++] != '.')
                    { }

                    /* if we found a null, there is no network (local machine) */
                    if (string_ptr == null || string_ptr[currentIndex] == '\0')
                    {
                        CFUNCTIONS.strncpy(ref client_ptr.network, client_ptr.IP, phantdefs.SZ_FROM - 1);
                        char[] networkChar = new char[client_ptr.network.Length + 1];
                        client_ptr.network.ToCharArray().CopyTo(networkChar, 0);
                        networkChar[client_ptr.network.Length] = '\0';
                        client_ptr.network = new string(networkChar); //[phantdefs.SZ_FROM - 1] = '\0';
                        client_ptr.addressResolved = false;
                    }
                    else
                    {
                        /* count the number or remaining terms */
                        terms = 1;
                        string_ptr_two = string_ptr;
                        int newCurrentIndex = currentIndex;
                        while (string_ptr_two[newCurrentIndex] != '\0')
                        {
                            if (string_ptr_two[newCurrentIndex + 1] == '.')
                                ++terms;
                            newCurrentIndex++;
                        }

                        /* remove terms until we find one without numbers or hex or
                        we have only two terms left */

                        string_ptr_two = string_ptr;
                        while (string_ptr[currentIndex] != '\0' || terms > 2)
                        {
                            if (LinuxLibSocket.isxdigit(string_ptr[currentIndex]))
                            {
                                //++string_ptr; 
                            }
                            else if (string_ptr[currentIndex] == '.')
                            {
                                string_ptr_two = new string(string_ptr).Substring(currentIndex + 1).ToCharArray();//++string_ptr;
                                terms -= 1; //this wasn't in the original - bug?
                            }
                            else
                            {
                                break;
                            }
                            currentIndex++; //this wasn't in the original - bug?
                        }

                        /* put this shortened hostname into place */
                        CFUNCTIONS.strncpy(ref client_ptr.network, new string(string_ptr_two), phantdefs.SZ_FROM - 1);
                        char[] networkChar = new char[client_ptr.network.Length + 1];
                        client_ptr.network.ToCharArray().CopyTo(networkChar, 0);
                        networkChar[client_ptr.network.Length] = '\0';
                        client_ptr.network = new string(networkChar); //[phantdefs.SZ_FROM - 1] = '\0';
                        client_ptr.addressResolved = true;
                    }
                }
                else
                {
                    /* use the IP address */
                    string_ptr = LinuxLibSocket.inet_ntoa(client_ptr.address.sin_addr).ToCharArray();

                    //Debug.LogError("IP debug: " + new string(string_ptr));
                    //Debug.LogError("IP debug: current IP: " + client_ptr.IP + " || string_ptr: " + new string(string_ptr) + " || phantdefs.SZ_FROM - 1: " + (phantdefs.SZ_FROM - 1));

                    CFUNCTIONS.strncpy(ref client_ptr.IP, new string(string_ptr), phantdefs.SZ_FROM - 1);

                    char[] IPChar = new char[client_ptr.IP.Length + 1];
                    client_ptr.IP.ToCharArray().CopyTo(IPChar, 0);
                    IPChar[client_ptr.IP.Length] = '\0';
                    client_ptr.IP = new string(IPChar); //[phantdefs.SZ_FROM - 1] = '\0'; //CFUNCTIONS.CharArrayToString(IPChar);  to remove \0?

                    /* get the class C network address */
                    theNetwork.s_addr = client_ptr.address.sin_addr.s_addr;// & 0x00FFFFFF;  //yikes. //todo???

                    client_ptr.addressResolved = false;

                    string_ptr = LinuxLibSocket.inet_ntoa(theNetwork).ToCharArray();
                    CFUNCTIONS.strncpy(ref client_ptr.network, new string(string_ptr), phantdefs.SZ_FROM - 1);
                    char[] networkChar = new char[client_ptr.network.Length + 1];
                    client_ptr.network.ToCharArray().CopyTo(networkChar, 0);
                    networkChar[client_ptr.network.Length] = '\0';
                    client_ptr.network = new string(networkChar); //[phantdefs.SZ_FROM - 1] = '\0';

                    /* stop logging this an an error - too common */
                    /*
                                error_msg = CFUNCTIONS.sprintf(ref 
                           "[%s:?] gethostbyaddress returned error %d in Do_accept_connections.\n",
                           client_ptr.IP, theError);

                                fileclass.Do_log_error(error_msg);
                    */
                }

                error_msg = CFUNCTIONS.sprintfSinglestring("Connection, IP=%s, Network=%s.\n", client_ptr.IP, client_ptr.network);

                fileclass.Do_log(pathnames.DEBUG_LOG, error_msg);

                /* create a new game object */
                game_ptr = new game_t();// (game_t) Do_malloc(phantdefs.SZ_GAME);

                /* set up the game variables */
                game_ptr.cleanup_thread = false;
                game_ptr.virtualvirtual = false;
                game_ptr.hearAllChannels = phantdefs.HEAR_SELF;
                game_ptr.chatFilter = true;
                game_ptr.sendEvents = false;
                game_ptr.the_socket = client_ptr.socket;
                game_ptr.description = null;
                game_ptr.it_combat = null;
                game_ptr.events_in = null;
                game_ptr.account = "\0";

                CFUNCTIONS.strcpy(ref game_ptr.IP, client_ptr.IP);

                CFUNCTIONS.strcpy(ref game_ptr.network, client_ptr.network);
                game_ptr.machineID = 0;

                /* initialize the event queue locks */
                miscclass.Do_init_mutex(game_ptr.events_in_lock);

                /* lock the game list */
                miscclass.Do_lock_mutex(s.realm.realm_lock);

                /* put the temp game into the list of games */
                game_ptr.next_game = s.realm.games;
                s.realm.games = game_ptr;

                /* unlock the game list */
                miscclass.Do_unlock_mutex(s.realm.realm_lock);

                /* init the pthread_att strcture */
                theError = CLibPThread.pthread_attr_init(thread_attr);
                if (theError != 0)
                {

                    error_msg = CFUNCTIONS.sprintfSinglestring("[0.0.0.0:%d] init of pthread_attr_t failed with error code of %d in Do_accept_connections.\n", (int)s.realm.serverPid, theError);


                    fileclass.Do_log_error(error_msg);
                    client_ptr = null; // free((void*) client_ptr);
                    return phantdefs.PTHREAD_ATTR_ERROR;
                }
                /*
                    pthread_attr_setdetachstate(&thread_attr, PTHREAD_CREATE_DETACHED);
                    pthread_attr_setstacksize(&thread_attr, 0xfffffff);
                */

                /* set up all information to be passed to the thread */
                client_ptr.realm = s.realm;
                client_ptr.game = game_ptr;

                /* create the new thread */
                theError = CLibPThread.pthread_create(ref game_ptr.the_thread, null, source.Do_start_thread, client_ptr);

                if (theError != 0)
                {

                    error_msg = CFUNCTIONS.sprintfSinglestring("[0.0.0.0:%d] thread creation failed with an error code of %d in Do_accept_connections.\n", (int)s.realm.serverPid, theError);


                    fileclass.Do_log_error(error_msg);
                    client_ptr = null;// free((void*) client_ptr);
                    return phantdefs.PTHREAD_CREATE_ERROR;
                }

                ++s.num_games;
            }
            return 0;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_check_socket(int the_socket)
        /
        / FUNCTION: Checks for data waiting on the socket
        /
        / AUTHOR: Brian Kelly, 4/23/99
        /
        / ARGUMENTS:
        /       int the_socket - the socket to check for data
        /
        / RETURN VALUE: 
        /	bool - true if there is data waiting on the socket
        /
        / MODULES CALLED: wmove(), _filbuf(), clearok(), waddstr(), wrefresh(),
        /       wclrtoeol()
        /
        / DESCRIPTION:
        /       Read a string from the keyboard.
        /       This routine is specially designed to:
        /
        *************************************************************************/

        int Do_check_socket(int the_socket)
        {
            Debug.Log("Thread " + System.Threading.Thread.CurrentThread.Name + ": phantasia.socket.Do_check_socket");

            LinuxLibSocket.fd_set rmask = new LinuxLibSocket.fd_set();
                          //static timeval
            int[] timeout = { 0, 0 };   /* no timeout */
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            int error = 0;
            int errno = 0;

            /* set rmask to check our socket */
            LinuxLibSocket.FD_ZERO(rmask);
            //Debug.Log("setting socket " + the_socket + " to rmask");
            LinuxLibSocket.FD_SET(the_socket,rmask);

            /* check for connections to be accepted */
            error = LinuxLibSocket.SOCKETselect(the_socket + 1, rmask, 0, 0, timeout, ref errno);
            if (error < 0)
            {

                error_msg = CFUNCTIONS.sprintfSinglestring("[0.0.0.0:?] select on socket failed with error code of %d in Do_check_socket\n", error);


                fileclass.Do_log_error(error_msg);
                CFUNCTIONS.exit(phantdefs.SOCKET_SELECT_ERROR);
                return 0; //added; exit does not halt thread
            }

            /* if select found no matches, return */
            if (error == 0)
            {
                return 0;
            }

            /* if our socket flag is not set, something is wrong */
            if (!LinuxLibSocket.FD_ISSET(the_socket, rmask))
            {

                error_msg = CFUNCTIONS.sprintfSinglestring("[0.0.0.0:?] select returned %d, but socket flag is off in Do_check_socket.\n", error);


                fileclass.Do_log_error(error_msg);
                CFUNCTIONS.exit(phantdefs.SOCKET_SELECT_ERROR);
                return 0; //added; exit does not halt thread //todo: apply to other exit calls
            }
            return 1;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_send_buffer(int the_socket)
        /
        / FUNCTION: Send data over the socket to the player
        /
        / AUTHOR: Brian Kelly, 4/23/99
        /
        / ARGUMENTS:
        /	int the_socket - the socket to send the information on
        /	size_t the_size - the number of byest to send
        /	void *the_data - a pointer to the data to be sent
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
        *************************************************************************/

        internal void Do_send_buffer(client_t c)
        {
            Debug.Log("Thread " + System.Threading.Thread.CurrentThread.Name + ": phantasia.socket.Do_send_buffer");

            /*
                string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            */
            string error_msg;//[2048];
            int bytes_sent;

            if (c.socket_up && c.out_buffer_size > 0)
            {

                /* send off the data */
                int errno = 0;
                
                char[] dummybuffer = c.out_buffer.ToCharArray();
                char[] charbuffer = new char[dummybuffer.Length + 1];
                dummybuffer.CopyTo(charbuffer, 0);
                charbuffer[(int)c.out_buffer_size] = '\0';
                c.out_buffer = new string(charbuffer);
                
                if (UnityGameController.SEND_DEBUG)
                {
                    error_msg = CFUNCTIONS.sprintfSinglestring("[%s] sending %d bytes\n", c.connection_id, c.out_buffer_size);

                    fileclass.Do_log(pathnames.DEBUG_LOG, error_msg);
                }
                
                if (UnityGameController.SEND_PACKET_DEBUG)
                {
                    error_msg = CFUNCTIONS.sprintfSinglestring("[%s] (%s)\n", c.connection_id, c.out_buffer);

                    fileclass.Do_log(pathnames.DEBUG_LOG, error_msg);
                }
                
                bytes_sent = LinuxLibSocket.SOCKETsend(c.socket, c.out_buffer, c.out_buffer_size, 0, ref errno); 

                if (bytes_sent != c.out_buffer_size)
                {

                    error_msg = CFUNCTIONS.sprintfSinglestring(
            "[%s] send on socket sent %d out of %d bytes in Do_send_buffer: %s.\n",
            c.connection_id, bytes_sent, c.out_buffer_size, CFUNCTIONS.strerror(errno));


                    fileclass.Do_log_error(error_msg);
                    c.socket_up = false;

                    error_msg = CFUNCTIONS.sprintfSinglestring("[%s] Error on socket while sending.\n",
                    c.connection_id);

                    fileclass.Do_log(pathnames.CONNECTION_LOG, error_msg);

                }
                c.out_buffer_size = 0;
            }

            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_send_out(struct client_t *c, void *the_data, size_t the_size)
        /
        / FUNCTION: Send data over the socket to the player
        /
        / AUTHOR: Brian Kelly, 5/6/99
        /
        / ARGUMENTS:
        /	int the_socket - the socket to send the information on
        /	size_t the_size - the number of byest to send
        /	void *the_data - a pointer to the data to be sent
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
        *************************************************************************/

        void Do_send_out(client_t c, string the_data, int the_size)
        {
            string filteredString = the_data.Replace('\0', '$');
            Debug.Log("Thread " + System.Threading.Thread.CurrentThread.Name + ": phantasia.socket.Do_send_out: || data: " + filteredString + " || size: " + the_size + " ||");

            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            string string_buffer = ""; //[phantdefs.SZ_LINE], 
            string string_buffer2 = ""; //[phantdefs.SZ_LINE];

            if (c.socket_up)
            {

                if (the_size > phantdefs.SZ_OUT_BUFFER)
                {

                    error_msg = CFUNCTIONS.sprintfSinglestring(
                "[%s] buffer_out overflow with %d bytes in Do_send_out.\n",
                c.connection_id, the_size);


                    fileclass.Do_log_error(error_msg);

                    Do_send_error(c, error_msg);
                    c.socket_up = false;
                    return;
                }

                if (the_size + c.out_buffer_size > phantdefs.SZ_OUT_BUFFER)
                {
                    Do_send_buffer(c);

                    if (the_size + c.out_buffer_size > phantdefs.SZ_OUT_BUFFER)
                    {
                        return;
                    }
                }
                
                if (UnityGameController.SEND_QUEUE_DEBUG)
                {
                    CFUNCTIONS.memcpy(ref string_buffer, the_data, the_size);

                    /* remove the "\n" */
                    char[] stringbufferchar = string_buffer.ToCharArray();
                    if (the_size > 0)
                    {
                        stringbufferchar[the_size - 1] = '\0'; //this should be fine as we're replacing, not adding
                    }
                    string_buffer = new string(stringbufferchar);
                    
                    CFUNCTIONS.sprintf(ref string_buffer2, "[%s] Queued (%s)\n", c.connection_id,
                 string_buffer);


                    fileclass.Do_log(pathnames.DEBUG_LOG, string_buffer2);
                }

                string bufstr = c.out_buffer.Replace('\0', '$');
                //Debug.Log("debug: c.out_buffer: " + bufstr + " || c.out_buffer_size: " + c.out_buffer_size);
                
                //string target = "";
                //CFUNCTIONS.memcpy(ref target, (string)the_data, the_size);

                bufstr = the_data.Replace('\0', '$');
                //Debug.Log("debug: the_data: " + the_data + " || c.out_buffer_size: " + c.out_buffer_size);

                //c.out_buffer += target; //nope. needs to sometimes overwrite instead of append

                string newbuf = c.out_buffer.Substring((int)c.out_buffer_size);
                CFUNCTIONS.memcpy(ref newbuf, the_data, the_size); //memcpy(&c->out_buffer[c->out_buffer_size], the_data, the_size);
                c.out_buffer = c.out_buffer.Substring(0, (int)c.out_buffer_size) + newbuf;

                c.out_buffer_size += (uint)the_size;

                bufstr = c.out_buffer.Replace('\0', '$');
                //Debug.Log("debug: c.out_buffer: " + bufstr + " || c.out_buffer_size: " + c.out_buffer_size);

                //original:

                //c->out_buffer_size += the_size;

            }

            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_send_string(struct client_t *c, char *theMessage)
        /
        / FUNCTION: Send data over the socket to the player
        /
        / AUTHOR: Brian Kelly, 8/11/99
        /
        / ARGUMENTS:
        /	int the_socket - the socket to send the information on
        /	size_t the_size - the number of byest to send
        /	void *the_data - a pointer to the data to be sent
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
        *************************************************************************/

        internal void Do_send_string(client_t c, string theMessage)
        {
            //Debug.Log("Thread " + System.Threading.Thread.CurrentThread.Name + ": phantasia.socket.Do_send_string");

            //if (theMessage != null && theMessage.Length > 0
            //    && theMessage[theMessage.Length - 1] == '\0') //this is a quick fix for nullbroken strings
            //{
            //    Debug.LogError("Fixing string " + theMessage.Replace('\0', '$'));
            //    if (theMessage.Length == 1)
            //        theMessage = new string(new char[] { '\n' });
            //    else
            //        theMessage = theMessage.Substring(0, theMessage.Length - 1)  + '\n';
            //}

            if (theMessage != null && theMessage.Length > 0 && theMessage[theMessage.Length - 1] != '\n') //todo: this is a quick fix for unlinebroken strings
            {
                theMessage += '\n';
            }

            int theSize;

            /* determine the string size */
            theSize = CFUNCTIONS.strlen(theMessage);

            //added for unity debug
            if (theSize == 0 && (theMessage == null || theMessage.Length == 0))
                Debug.LogError("Warning: sending message of size 0");

            /* send the data */
            Do_send_out(c, theMessage, theSize);

            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_send_double(struct client_t *c, double theDouble)
        /
        / FUNCTION: Send data over the socket to the player
        /
        / AUTHOR: Brian Kelly, 8/13/99
        /
        / ARGUMENTS:
        /	int the_socket - the socket to send the information on
        /	size_t the_size - the number of byest to send
        /	void *the_data - a pointer to the data to be sent
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
        *************************************************************************/

        internal void Do_send_double(client_t c, double theDouble)
        {
            string tmpDouble = ""; //[phantdefs.SZ_NUMBER];

            /* determine the string size */
            CFUNCTIONS.sprintf(ref tmpDouble, "%0.lf\n", theDouble);

            /* send the data */
            Do_send_string(c, tmpDouble);

            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_send_float(struct client_t *c, float theFloat)
        /
        / FUNCTION: Send data over the socket to the player
        /
        / AUTHOR: Brian Kelly, 8/13/99
        /
        / ARGUMENTS:
        /	int the_socket - the socket to send the information on
        /	size_t the_size - the number of byest to send
        /	void *the_data - a pointer to the data to be sent
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
        *************************************************************************/

        internal void Do_send_float(client_t c, double theFloat)
        {
            string tmpFloat = ""; //[phantdefs.SZ_NUMBER];

            /* determine the string size */
            CFUNCTIONS.sprintf(ref tmpFloat, "%0.lf\n", theFloat);

            /* send the data */
            Do_send_string(c, tmpFloat);

            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_send_fpfloat(struct client_t *c, float theFloat)
        /
        / FUNCTION: Send a full-precision float to the client
        /
        / AUTHOR: Brian Kelly, 8/25/99
        /
        / ARGUMENTS:
        /	int the_socket - the socket to send the information on
        /	size_t the_size - the number of byest to send
        /	void *the_data - a pointer to the data to be sent
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
        *************************************************************************/

        internal void Do_send_fpfloat(client_t c, double theFloat)
        {
            string tmpFloat = ""; //[phantdefs.SZ_NUMBER];

            /* determine the string size */
            CFUNCTIONS.sprintf(ref tmpFloat, "%.4lf\n", theFloat);

            /* send the data */
            Do_send_string(c, tmpFloat);

            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_send_int(struct client_t *c, int theInt)
        /
        / FUNCTION: Send data over the socket to the player
        /
        / AUTHOR: Brian Kelly, 8/13/99
        /
        / ARGUMENTS:
        /	int the_socket - the socket to send the information on
        /	size_t the_size - the number of byest to send
        /	void *the_data - a pointer to the data to be sent
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
        *************************************************************************/

        internal void Do_send_int(client_t c, int theInt)
        {
            string tmpInt = ""; //[phantdefs.SZ_NUMBER];

            /* determine the string size */
            CFUNCTIONS.sprintf(ref tmpInt, "%d\n", theInt);

            /* send the data */
            Do_send_string(c, tmpInt);

            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_send_short(struct client_t *c, short theShort)
        /
        / FUNCTION: Send data over the socket to the player
        /
        / AUTHOR: Brian Kelly, 8/13/99
        /
        / ARGUMENTS:
        /	int the_socket - the socket to send the information on
        /	size_t the_size - the number of byest to send
        /	void *the_data - a pointer to the data to be sent
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
        *************************************************************************/

        void Do_send_short(client_t c, short theShort)
        {
            string tmpShort = ""; //[phantdefs.SZ_NUMBER];

            /* determine the string size */
            CFUNCTIONS.sprintf(ref tmpShort, "%hd\n", theShort);

            /* send the data */
            Do_send_string(c, tmpShort);

            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_send_bool(struct client_t *c, bool theBool)
        /
        / FUNCTION: Send data over the socket to the player
        /
        / AUTHOR: Brian Kelly, 8/13/99
        /
        / ARGUMENTS:
        /	int the_socket - the socket to send the information on
        /	size_t the_size - the number of byest to send
        /	void *the_data - a pointer to the data to be sent
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
        *************************************************************************/

        internal void Do_send_bool(client_t c, short theBool)
        {
            if (theBool == 0)

                /* send the data */
                Do_send_string(c, "No\n");
            else

                /* send the data */
                Do_send_string(c, "Yes\n");

            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_send_error(struct client_t *c, char *theError)
        /
        / FUNCTION: Send data over the socket to the player
        /
        / AUTHOR: Brian Kelly, 10/2/99
        /
        / ARGUMENTS:
        /	int the_socket - the socket to send the information on
        /	size_t the_size - the number of byest to send
        /	void *the_data - a pointer to the data to be sent
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
        *************************************************************************/

        internal void Do_send_error(client_t c, string theError)
        {
            Do_send_int(c, phantdefs.ERROR_PACKET);
            Do_send_string(c, theError);
            Do_send_buffer(c);
            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_read_socket(struct client_t *c)
        /
        / FUNCTION: Send data over the socket to the player
        /
        / AUTHOR: Brian Kelly, 4/24/99
        /
        / ARGUMENTS:
        /       int the_socket - the socket to read data from
        /	size_t the_size - the number of bytes to read from the socket
        /	void *the_data - a pointer to the data to be read
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
        *************************************************************************/

        void Do_read_socket(client_t c)
        {
            Debug.Log("Thread " + System.Threading.Thread.CurrentThread.Name + ": phantasia.socket.Do_read_socket");

            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            int bytes_read;
            event_t event_ptr = new event_t();

            int errno = 0;

            /* read the data from the socket */
            bytes_read = 0;
            bytes_read = LinuxLibSocket.SOCKETrecv(c.socket, ref c.in_buffer, c.in_buffer_size, //c.in_buffer[c.in_buffer_size]
                (phantdefs.SZ_IN_BUFFER - c.in_buffer_size), 0, ref errno);

            if (bytes_read <= 0)
            {

                /* the socket is no longer any good */
                c.socket_up = false;

                /* if client closed abruptly, the connection will be reset */
                if (errno == CFUNCTIONS.ECONNRESET)
                {

                    /* Error too common for the error log */
                    /*
                            error_msg = CFUNCTIONS.sprintf(ref 
                                   "[%s] Received a ECONNRESET on socket in Do_read_socket.\n",
                               c.connection_id);

                                fileclass.Do_log_error(error_msg);
                    */
                }
                else
                {

                    error_msg = CFUNCTIONS.sprintfSinglestring(
                   "[%s] read on socket returned %d bytes in Do_read_socket: %s\n",
                   c.connection_id, bytes_read, CFUNCTIONS.strerror(errno));

                    fileclass.Do_log_error(error_msg);
                }


                error_msg = CFUNCTIONS.sprintfSinglestring("[%s] Received an error on the socket.\n",
                    c.connection_id);


                fileclass.Do_log(pathnames.CONNECTION_LOG, error_msg);
            }

            else if (bytes_read + c.in_buffer_size > phantdefs.SZ_IN_BUFFER)
            {

                /* the socket is no longer any good */
                c.socket_up = false;

                /* log an error */
                error_msg = CFUNCTIONS.sprintfSinglestring("[%s] added %d to %d bytes for in_buffer overvlow in Do_read_socket.\n", c.connection_id, bytes_read, c.in_buffer_size);


                fileclass.Do_log_error(error_msg);
            }

            else
            {
                c.in_buffer_size += (uint)bytes_read;

                //Debug.Log("Thread " + System.Threading.Thread.CurrentThread.Name + ": socket read successful");
                string bufcopy = c.in_buffer.Replace('\0', '$');
                //Debug.Log("Thread " + System.Threading.Thread.CurrentThread.Name + ": || buffer contents: " + bufcopy + " ||");
            }

            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_get_socket_string(struct client_t *c, void *the_data, size_t the_size)
        /
        / FUNCTION: reads the next packet type off the socket
        /
        / AUTHOR: Brian Kelly, 4/23/99
        /
        / ARGUMENTS:
        /	int the_socket - the socket to read the packet type
        /
        / RETURN VALUE: 
        /	int - the type of packet next on the socket
        /
        / MODULES CALLED: wmove(), _filbuf(), clearok(), waddstr(), wrefresh(),
        /       wclrtoeol()
        /
        / DESCRIPTION:
        /       Read a string from the keyboard.
        /
        /************************************************************************/

        int Do_get_socket_string(client_t c, ref string theString, int theSize)
        {
            int theLength;
            string string_buffer = ""; //[phantdefs.SZ_LINE], 
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE + phantdefs.SZ_OUT_BUFFER];
            LinuxLibSIG.sigset_t sigMask = new LinuxLibSIG.sigset_t();
            int theSignal = 0;

            /* prepare to unblock SIGIO */
            LinuxLibSIG.sigemptyset(ref sigMask);
            LinuxLibSIG.sigaddset(sigMask, LinuxLibSIG.SIGIO);
            LinuxLibSIG.sigaddset(sigMask, LinuxLibSIG.SIGALRM);

            string test1 = "1\0";
            //Debug.Log("STRING LENGTH CHECK: LENGTH OF 1$ = " + test1.Length); // 2

            for (; ; )
            {
                if (UnityGameController.StopApplication) //time to quit
                {
                    Debug.Log("Thread " + System.Threading.Thread.CurrentThread.Name + " stopping in Do_get_socket_string");
                    return phantdefs.S_ERROR;
                }
                else
                {
                    System.Threading.Thread.Sleep(33); //30fps
                }

                /* if the socket is down, return an error */
                if (!c.socket_up)
                {
                    return phantdefs.S_ERROR;
                }

                /* if we have information in the buffer */
                if (c.in_buffer_size != 0)
                {

                    /* see if the entire string has been downloaded */
                    theLength = CFUNCTIONS.strlen(c.in_buffer);

                    //Debug.Log("!!!Info present in buffer!!! theLength " + theLength + " c.in_buffer_size " + c.in_buffer_size);
                    
                    if (theLength < c.in_buffer_size)
                    {
                        //Debug.Log("!!!Received info less than full buffer!!!");

                        /* check that the passed pointer can handle the size */
                        if (theSize - 1 < theLength) //i.e. other messgaes are waiting in the buffer after this one
                        {

                            /* log the error */
                            error_msg = CFUNCTIONS.sprintfSinglestring("[%s] Client returned a string of %d bytes, %d max in Do_get_socket_string.\n", c.connection_id, theLength, theSize);


                            fileclass.Do_log_error(error_msg);

                            /* get as much of the string as we can */
                            CFUNCTIONS.strncpy(ref theString, c.in_buffer, theSize - 1);
                            char[] thestringchar = new char[theString.Length + 1];//theString.ToCharArray();
                            theString.ToCharArray().CopyTo(thestringchar, 0);
                            thestringchar[theSize - 1] = '\0'; //todo: remove later part of string? also other instance earlier in this file
                            theString = new string(thestringchar);
                            
                            int firstNull = theString.IndexOf('\0'); //cut off at first null
                            theString = theString.Substring(0, firstNull + 1);

                        }
                        else
                        {

                            //Debug.Log("!!!Copying info to string!!!");
                            CFUNCTIONS.strcpy(ref theString, c.in_buffer);

                            int firstNull = theString.IndexOf('\0'); //cut off at first null
                            theString = theString.Substring(0, firstNull + 1);
                        }

                        /* add the terminating null to the string */
                        ++theLength;

                        /* move up information in the buffer */
                        if (theLength < c.in_buffer_size)
                        {
                            //Debug.Log("!!!Received info + 1 still less than full buffer!!!");

                            c.in_buffer_size -= (uint)theLength;

                            CFUNCTIONS.memmove(ref c.in_buffer, c.in_buffer.Substring(theLength),//c.in_buffer[theLength], 
                                c.in_buffer_size); //moves c.in_buffer data from after theLength up to in_buffer_size, back to [0] (overwriting thelength data but leaving other data)
                        }
                        else
                        {
                            c.in_buffer_size = 0;
                        }
                        
                        if (UnityGameController.RECEIVE_DEBUG)
                        {
                            error_msg = CFUNCTIONS.sprintfSinglestring("[%s] received %d bytes\n",
                        c.connection_id, theLength);

                            fileclass.Do_log(pathnames.DEBUG_LOG, error_msg);
                        }
                        
                        if (UnityGameController.RECEIVE_PACKET_DEBUG)
                        {
                            error_msg = CFUNCTIONS.sprintfSinglestring("[%s] (%s)\n", c.connection_id, theString);

                            fileclass.Do_log(pathnames.DEBUG_LOG, error_msg);
                        }
                        
                        string msgcopy = c.in_buffer.Replace("\0", "$");
                        msgcopy = msgcopy.Replace('\0', '$');
                        //Debug.Log("Thread " + System.Threading.Thread.CurrentThread.Name + ": buffer content after get: " + msgcopy);
                        //Debug.Log("!!!returning!!!");
                        return phantdefs.S_NORM;
                    }

                    /* the the buffer is maxed and we're here,
            			the string is too large for the buffer */

                    if (c.in_buffer_size == phantdefs.SZ_IN_BUFFER)
                    {


                        error_msg = CFUNCTIONS.sprintfSinglestring("[%s] Request for a string, %d bytes, larger than buffer in Do_get_socket_string.\n", c.connection_id, theLength);


                        fileclass.Do_log_error(error_msg);

                        c.socket_up = false;
                        return phantdefs.S_ERROR;
                    }
                }

                /* We need to wait for information, so pause */
                if (c.socket_up)
                {
                    
                    if (UnityGameController.SUSPEND_DEBUG)
                    {
                        error_msg = CFUNCTIONS.sprintfSinglestring(
                        "[%s] now sleeping with alarm set for %d seconds.\n",
                        c.connection_id, c.timeout);


                        fileclass.Do_log(pathnames.DEBUG_LOG, error_msg);
                    }
                    LinuxLibSIG.sigwait(sigMask, ref theSignal);
                    
                    if (UnityGameController.SUSPEND_DEBUG)
                    {
                        error_msg = CFUNCTIONS.sprintfSinglestring("[%s] awoken on signal %d.\n",
                        c.connection_id, theSignal);


                        fileclass.Do_log(pathnames.DEBUG_LOG, error_msg);
                    }
                    /*
                            sleep(.1);
                    */

                }
                else
                {
                    theSignal = LinuxLibSIG.SIGIO; 
                }

                /* check events and the socket on a SIGIO */
                if (theSignal == LinuxLibSIG.SIGIO)
                {

                    /* if the socket is up, we have room in the buffer and
                        there is info waiting */

                    if (c.socket_up && c.in_buffer_size < (uint)phantdefs.SZ_IN_BUFFER
                        && Do_check_socket(c.socket) != 0)
                    {
                        Do_read_socket(c);
                    }

                    /* see if any other threads have sent us an event */
                    eventclass.Do_check_events_in(c);
                }

                /* see if the tread alarm went off */
                else if (theSignal == LinuxLibSIG.SIGALRM)
                {
                    /*
                        if (time(null) > c.timeoutAt) {
                    */

                    switch (++c.timeoutFlag)
                    {

                        /* alarm has gone off once */
                        case 1:

                            Do_send_int(c, phantdefs.PING_PACKET);

                            Do_send_buffer(c);

                            /* give the client 15 seconds to respond */
                            LinuxLibSocket.alarm(15);
                            c.timeoutAt = CFUNCTIONS.GetUnixEpoch(DateTime.Now) + 15;
                            break;

                        /* gone off twice */
                        case 2:

                            error_msg = CFUNCTIONS.sprintfSinglestring("[%s] Socket connection timed out.\n",
                            c.connection_id);

                            fileclass.Do_log(pathnames.CONNECTION_LOG, error_msg);

                            Do_send_error(c, "The socket connection timed out.\n");

                            Do_send_buffer(c);

                            /* assume the network connection is down */
                            error_msg = CFUNCTIONS.sprintfSinglestring(
                         "[%s] Socket connection timed out in Do_get_socket_string.\n",
                         c.connection_id);


                            fileclass.Do_log_error(error_msg);
                            c.socket_up = false;
                            return phantdefs.S_ERROR;
                    }
                }

                /* unknown signal */
                else
                {

                    error_msg = CFUNCTIONS.sprintfSinglestring("[%s] Received unknown signal %d.\n",
                        c.connection_id, theSignal);


                    fileclass.Do_log_error(error_msg);
                }
            }
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_get_string(struct client_t *c, char *theString,
        /	int maxSize);
        /
        / FUNCTION: Wait until char is returned from player
        /
        / AUTHOR: Brian Kelly, 8/11/99
        /
        / ARGUMENTS:
        /	client_t c - the client data strcture
        /	size_t the_size - the number of bytes fo data expected
        /	void *the_data - a pointer to where the data should be written
        /	bool exact - Does there need to be exactly the_size bytes?
        /
        / RETURN VALUE: 
        /	bool - Was the data or a timeout registered
        /
        / MODULES CALLED: wmove(), _filbuf(), clearok(), waddstr(), wrefresh(),
        /       wclrtoeol()
        /
        / DESCRIPTION:
        /       Read a string from the keyboard.
        /
        /************************************************************************/

        internal int Do_get_string(client_t c, ref string theString, int maxSize)
        {
            Debug.Log("Thread " + System.Threading.Thread.CurrentThread.Name + ": phantasia.socket.Do_get_string");

            string packetTmp = ""; //[phantdefs.SZ_PACKET_TYPE], 
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            int packetType;
            int returnCode;

            Do_send_buffer(c);

            c.timeoutFlag = 0;
            LinuxLibSocket.alarm(c.timeout); 
            c.timeoutAt = CFUNCTIONS.GetUnixEpoch(DateTime.Now) + c.timeout;

            for (; ; )
            {
                if (UnityGameController.StopApplication) //time to quit
                {
                    Debug.Log("Thread " + System.Threading.Thread.CurrentThread.Name + " stopping in Do_get_string");
                    return phantdefs.S_ERROR;
                }
                else
                {
                    System.Threading.Thread.Sleep(33); //30fps
                }

                /* get the header of the next packet */
                int errno = 0;
                if (Do_get_socket_string(c, ref packetTmp, phantdefs.SZ_PACKET_TYPE) == phantdefs.S_ERROR)
                {

                    LinuxLibSocket.alarm(0); 
                    return phantdefs.S_ERROR;
                }

                /* convert the string to an integer */
                packetType = (int)CFUNCTIONS.strtol(packetTmp, null, 10);

                Debug.Log("Thread " + System.Threading.Thread.CurrentThread.Name + " IDENTIFYING PACKET " + packetType);
                switch (packetType)
                {

                    /* if the packet type is returning an answer */
                    case phantdefs.C_RESPONSE_PACKET:

                        /* read the string */
                        Debug.Log("Thread " + System.Threading.Thread.CurrentThread.Name + " fetching packet content");
                        returnCode = Do_get_socket_string(c, ref theString, maxSize);

                        LinuxLibSocket.alarm(0); 
                        return returnCode;

                    case phantdefs.C_CANCEL_PACKET:


                        LinuxLibSocket.alarm(0);
                        return phantdefs.S_CANCEL;

                    case phantdefs.C_PING_PACKET:

                        if (c.timeoutFlag == 1)
                        {
                            LinuxLibSocket.alarm(0);
                            return phantdefs.S_TIMEOUT;
                        }
                        else
                        {

                            error_msg = CFUNCTIONS.sprintfSinglestring("[%s] Received unexpected ping packet.\n",
                            c.connection_id);


                            fileclass.Do_log_error(error_msg);
                        }
                        break;

                    default:

                        if (Do_packet(c, packetType) == phantdefs.S_ERROR)
                        {

                            LinuxLibSocket.alarm(0);
                            return phantdefs.S_ERROR;
                        }
                        break;
                }
            }
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_get_double(struct client_t *c, double *theDouble);
        /
        / FUNCTION: Wait until char is returned from player
        /
        / AUTHOR: Brian Kelly, 8/11/99
        /
        / ARGUMENTS:
        /	client_t c - the client data strcture
        /	size_t the_size - the number of bytes fo data expected
        /	void *the_data - a pointer to where the data should be written
        /	bool exact - Does there need to be exactly the_size bytes?
        /
        / RETURN VALUE: 
        /	bool - Was the data or a timeout registered
        /
        / MODULES CALLED: wmove(), _filbuf(), clearok(), waddstr(), wrefresh(),
        /       wclrtoeol()
        /
        / DESCRIPTION:
        /       Read a string from the keyboard.
        /
        /************************************************************************/

        internal int Do_get_double(client_t c, ref double theDouble)
        {
            string tmpDouble = ""; //[phantdefs.SZ_NUMBER];
            int rc;
            rc = Do_get_string(c, ref tmpDouble, phantdefs.SZ_NUMBER) != phantdefs.S_NORM ? 1 : 0;
            if (rc != 0)
            {
                return rc;
            }

            //Debug.LogError("Retrieved tmp debug: " + tmpDouble);
            theDouble = CFUNCTIONS.floor(CFUNCTIONS.strtod(tmpDouble, null));
            //Debug.LogError("Retrieved double debug: " + theDouble);

            /* insure that the number is finite */
            if (!LinuxLibSocket.finite(theDouble))
            {

                /* fix the number */
                theDouble = 0;
            }

            return phantdefs.S_NORM;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_get_long(struct client_t *c, long *theLong);
        /
        / FUNCTION: Wait until char is returned from player
        /
        / AUTHOR: Brian Kelly, 10/2/99
        /
        / ARGUMENTS:
        /	client_t c - the client data strcture
        /	size_t the_size - the number of bytes fo data expected
        /	void *the_data - a pointer to where the data should be written
        /	bool exact - Does there need to be exactly the_size bytes?
        /
        / RETURN VALUE: 
        /	bool - Was the data or a timeout registered
        /
        / MODULES CALLED: wmove(), _filbuf(), clearok(), waddstr(), wrefresh(),
        /       wclrtoeol()
        /
        / DESCRIPTION:
        /       Read a string from the keyboard.
        /
        /************************************************************************/

        internal int Do_get_long(client_t c, ref long theLong)
        {
            string tmpLong = ""; //[phantdefs.SZ_NUMBER];
            int returnCode;

            returnCode = Do_get_string(c, ref tmpLong, phantdefs.SZ_NUMBER);

            if (returnCode != phantdefs.S_NORM)
            {
                return returnCode;
            }

            theLong = CFUNCTIONS.strtol(tmpLong, null, 10);

            return phantdefs.S_NORM;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_get_nothing(struct client_t *c);
        /
        / FUNCTION: Wait until char is returned from player
        /
        / AUTHOR: Brian Kelly, 11/3/99
        /
        / ARGUMENTS:
        /	client_t c - the client data strcture
        /	size_t the_size - the number of bytes fo data expected
        /	void *the_data - a pointer to where the data should be written
        /	bool exact - Does there need to be exactly the_size bytes?
        /
        / RETURN VALUE: 
        /	bool - Was the data or a timeout registered
        /
        / MODULES CALLED: wmove(), _filbuf(), clearok(), waddstr(), wrefresh(),
        /       wclrtoeol()
        /
        / DESCRIPTION:
        /       Read a string from the keyboard.
        /
        /************************************************************************/

        internal int Do_get_nothing(client_t c)
        {
            string packetTmp = ""; //[phantdefs.SZ_PACKET_TYPE];
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            int packetType;

            for (; ; )
            {
                if (UnityGameController.StopApplication) //time to quit
                {
                    Debug.Log("Thread " + System.Threading.Thread.CurrentThread.Name + " stopping in Do_get_nothing");
                    return phantdefs.S_ERROR;
                }
                else
                {
                    System.Threading.Thread.Sleep(33); //30fps
                }

                /* if the socket is up, we have room in the buffer and
                    there is info waiting */

                if (c.socket_up && c.in_buffer_size < phantdefs.SZ_IN_BUFFER
                    && Do_check_socket(c.socket) != 0)
                {

                    Do_read_socket(c);
                }

                /* if the socket is up, and the buffer has data */
                if (c.socket_up && c.in_buffer_size != 0)
                {

                    c.timeoutAt = CFUNCTIONS.GetUnixEpoch(DateTime.Now) + c.timeout;

                    /* get the header of the next packet */
                    if (Do_get_socket_string(c, ref packetTmp, phantdefs.SZ_PACKET_TYPE) == phantdefs.S_ERROR)
                        return phantdefs.S_ERROR;

                    /* convert the string to an integer */
                    packetType = (int)CFUNCTIONS.strtol(packetTmp, null, 10);

                    switch (packetType)
                    {

                        /* we're not expecting an answer */
                        case phantdefs.C_RESPONSE_PACKET:
                        case phantdefs.C_CANCEL_PACKET:
                        case phantdefs.C_PING_PACKET:

                            /* print an error message */
                            error_msg = CFUNCTIONS.sprintfSinglestring(
                        "[%s] Unexpected packet type %d returned in Do_get_nothing.\n",
                        c.connection_id, packetType);


                            fileclass.Do_log_error(error_msg);

                            Do_send_error(c, error_msg);
                            c.socket_up = false;

                            return phantdefs.S_ERROR;

                        default:

                            if (Do_packet(c, packetType) == phantdefs.S_ERROR)
                            {
                                return phantdefs.S_ERROR;
                            }
                            break;
                    }
                }
                else
                {
                    return phantdefs.S_NORM;
                }
            }
        }


        /************************************************************************
        /
        / FUNCTION NAME: int Do_packet(client_t *c, char packetType);
        /
        / FUNCTION: Wait until char is returned from player
        /
        / AUTHOR: Brian Kelly, 8/11/99
        /
        / ARGUMENTS:
        /	client_t c - the client data strcture
        /	size_t the_size - the number of bytes fo data expected
        /	void *the_data - a pointer to where the data should be written
        /	bool exact - Does there need to be exactly the_size bytes?
        /
        / RETURN VALUE: 
        /	bool - Was the data or a timeout registered
        /
        / MODULES CALLED: wmove(), _filbuf(), clearok(), waddstr(), wrefresh(),
        /       wclrtoeol()
        /
        / DESCRIPTION:
        /       Read a string from the keyboard.
        /
        /************************************************************************/

        int Do_packet(client_t c, int thePacket)
        {
            Debug.Log("Thread " + System.Threading.Thread.CurrentThread.Name + ": phantasia.socket.Do_packet");

            char numChars;
            int timeNow;
            event_t event_ptr = new event_t();
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            string player_name = ""; //[phantdefs.SZ_NAME];
            string string_buffer = ""; //[phantdefs.SZ_CHAT - 20];
            long theLong;

            /* switch on the type of event */
            switch (thePacket)
            {

                case phantdefs.C_CHAT_PACKET:

                    /* get the string to be sent and quit on error*/
                    if (Do_get_socket_string(c, ref string_buffer, phantdefs.SZ_CHAT - 20) == phantdefs.S_ERROR)
                        return phantdefs.S_ERROR;

                    /* mute null chat messages */
                    if (CFUNCTIONS.strlen(string_buffer) > 0)
                    {

                        /* if the player is muted */
                        if (c.muteUntil < CFUNCTIONS.GetUnixEpoch(DateTime.Now))
                        {

                            /* send the chat message */
                            ioclass.Do_chat(c, string_buffer);
                        }
                    }

                    /* get rid of old tags */
                    if (c.tagUntil < CFUNCTIONS.GetUnixEpoch(DateTime.Now))
                    {

                        tagsclass.Do_remove_prefix_suffix(c);
                        /* if(c.characterLoaded)
                        {
                            CFUNCTIONS.strcpy(ref string_buffer, c.player.name);
                            if(c.characterAnnounced)
                            {
                                characterclass.Do_send_specification(c, phantdefs.REMOVE_PLAYER_EVENT);
                            }
                            cfunctions.strncpy(c.modifiedName, string_buffer, phantdefs.SZ_NAME - 1);
                            if(c.characterAnnounced)
                            {
                                characterclass.Do_send_specification(c, phantdefs.ADD_PLAYER_EVENT);
                                Do_name(c);
                            }
                        } */
                    }

                    /* finished */
                    return phantdefs.S_NORM;

                case phantdefs.C_EXAMINE_PACKET:

                    /* read player name and quit if there's an error */
                    if (Do_get_socket_string(c, ref player_name, phantdefs.SZ_NAME) == phantdefs.S_ERROR)
                        return phantdefs.S_ERROR;

                    /* request the info from the player */
                    event_ptr = eventclass.Do_create_event();
                    event_ptr.type = (short)phantdefs.REQUEST_RECORD_EVENT;
                    event_ptr.from = c.game;

                    if (eventclass.Do_send_character_event(c, event_ptr, player_name) == 0)
                    {

                        event_ptr = null; // free((void*)event_ptr);
                    }

                    /* finished */
                    return phantdefs.S_NORM;

                case phantdefs.C_SCOREBOARD_PACKET:

                    /* read starting record and quit if there's an error */
                    if (Do_get_socket_string(c, ref string_buffer, phantdefs.SZ_NAME) == phantdefs.S_ERROR)
                        return phantdefs.S_ERROR;

                    theLong = CFUNCTIONS.strtol(string_buffer, null, 10);
                    /* convert the string to an int */
                    miscclass.Do_scoreboard(c, (int)theLong, 1);

                    /* finished */
                    return phantdefs.S_NORM;

                case phantdefs.C_ERROR_PACKET:

                    /* see if the client included a message */
                    if (Do_get_socket_string(c, ref error_msg, phantdefs.SZ_ERROR_MESSAGE) !=
                        phantdefs.S_NORM)
                    {

                        error_msg = "\0";
                    }

                    /* log the error */
                    CFUNCTIONS.sprintf(ref string_buffer,
                        "[%s] Client returned an error packet in Do_packet: %s.\n",
                        c.connection_id, error_msg);


                    fileclass.Do_log_error(string_buffer);

                    c.socket_up = false;
                    return phantdefs.S_ERROR;

                default:

                    /* print an error message */
                    error_msg = CFUNCTIONS.sprintfSinglestring(
                        "[%s] Received unknown packet type %d in Do_packet.\n",
                        c.connection_id, thePacket);


                    fileclass.Do_log_error(error_msg);

                    Do_send_error(c, error_msg);
                    c.socket_up = false;

                    /* exit gracefully */
                    return phantdefs.S_ERROR;
            }
        }


        /************************************************************************
        /
        / FUNCTION NAME: int Do_get_network_address(client_t *c, int hostAddress, int subnetMask);
        /
        / FUNCTION: Wait until char is returned from player
        /
        / AUTHOR: Brian Kelly, 6/8/00
        /
        / ARGUMENTS:
        /	client_t c - the client data strcture
        /	size_t the_size - the number of bytes fo data expected
        /	void *the_data - a pointer to where the data should be written
        /	bool exact - Does there need to be exactly the_size bytes?
        /
        / RETURN VALUE: 
        /	bool - Was the data or a timeout registered
        /
        / MODULES CALLED: wmove(), _filbuf(), clearok(), waddstr(), wrefresh(),
        /       wclrtoeol()
        /
        / DESCRIPTION:
        /       Read a string from the keyboard.
        /
        /************************************************************************/

        void Do_get_network_address(LinuxLibSocket.in_addr hostAddress, int subnetMask)
        {
            uint bitMask;

            /* create a bitmask.  Ex: 8 mask = 0xffffff00 */
            bitMask = (uint)CFUNCTIONS.floor(CFUNCTIONS.pow(2, subnetMask) - 1);
            bitMask = (uint)0xffffffff ^ bitMask;

            /* return only the network portion of the hostAddress */
            hostAddress.s_addr = (int)bitMask & LinuxLibSocket.htonl(hostAddress.s_addr);
            hostAddress.s_addr = LinuxLibSocket.ntohl(hostAddress.s_addr);
            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: int Do_check_host_address(int hostAddress, int networkAddress, int subnetMask);
        /
        / FUNCTION: Wait until char is returned from player
        /
        / AUTHOR: Brian Kelly, 6/8/00
        /
        / ARGUMENTS:
        /	client_t c - the client data strcture
        /	size_t the_size - the number of bytes fo data expected
        /	void *the_data - a pointer to where the data should be written
        /	bool exact - Does there need to be exactly the_size bytes?
        /
        / RETURN VALUE: 
        /	bool - Was the data or a timeout registered
        /
        / MODULES CALLED: wmove(), _filbuf(), clearok(), waddstr(), wrefresh(),
        /       wclrtoeol()
        /
        / DESCRIPTION:
        /       Read a string from the keyboard.
        /
        /************************************************************************/

        int Do_check_host_address(LinuxLibSocket.in_addr hostAddress, LinuxLibSocket.in_addr networkAddress, int subnetMask)
        {
            LinuxLibSocket.in_addr checkAddress = new LinuxLibSocket.in_addr();

            if (subnetMask < 0 || subnetMask > 24)
            {
                return 0;
            }

            checkAddress.s_addr = hostAddress.s_addr;

            Do_get_network_address(checkAddress, subnetMask);

            if (networkAddress.s_addr == checkAddress.s_addr)
            {
                return 1;
            }

            return 0;
        }

    }
}
