using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace phantasiaclasses
{
    public class io //: MonoBehaviour
    {

        // Use this for initialization
        internal void Start()
        {

        }

        // Update is called once per frame
        internal void Update()
        {

        }

        static io Instance;
        private io()
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
        public static io GetInstance()
        {
            io instance = null;
            if (Instance != null)
            {
                instance = Instance;
            }
            else
            {
                instance = new io();
            }
            return instance;
        }

        phantasiaclasses.misc miscclass;
        phantasiaclasses.file fileclass;
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

        /*
     * io.c - routines to handle all socket matters for Phantasia
     */


        /************************************************************************
        /
        / FUNCTION NAME: ioclass.Do_send_clear(struct client_t *c)
        /
        / FUNCTION: Send a clear command to the player
        /
        / AUTHOR: Brian Kelly, 4/24/99
        /
        / ARGUMENTS:
        /       struct client_t *c - address of the client's main data strcture
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

        internal void Do_send_clear(client_t c)
        {
            /* send the message */
            socketclass.Do_send_int(c, phantdefs.CLEAR_PACKET);

            /* all done here */
            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: ioclass.Do_send_line(struct client_t *c, char *message)
        /
        / FUNCTION: Send data over the socket to the player
        /
        / AUTHOR: Brian Kelly, 4/23/99
        /
        / ARGUMENTS:
        /       struct client_t *c - address of the client's main data strcture
        /	void *message - a pointer to the data to be sent
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

        internal void Do_send_line(client_t c, string message)
        {
            /* send the message */
            socketclass.Do_send_int(c, phantdefs.WRITE_LINE_PACKET);
            socketclass.Do_send_string(c, message);

            /* all done here */
            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: ioclass.Do_string_dialog(struct client_t *c, char *the_string, int the_size, char *theMessage);)
        /
        / FUNCTION: Sends a request for a charcater, and returns it
        /
        / AUTHOR: Brian Kelly, 5/4/99
        /
        / ARGUMENTS:
        /       struct client_t *c - address of the client's main data strcture
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

        internal bool Do_string_dialog(client_t c, ref string the_string, int the_size, string theMessage)
        {
            /* send the player a string request packet */
            socketclass.Do_send_int(c, phantdefs.STRING_DIALOG_PACKET);

            /* send the prompt string */
            socketclass.Do_send_string(c, theMessage);

            /* wait for the player to send back an answer */
            if (socketclass.Do_get_string(c, ref the_string, the_size) == phantdefs.S_NORM)
                return false;

            /* no more tales to tell */
            return true;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_password_dialog(struct client_t *c, char *the_string, int the_size, char *theMessage);)
        /
        / FUNCTION: Sends a request for a charcater, and returns it
        /
        / AUTHOR: Brian Kelly, 5/4/99
        /
        / ARGUMENTS:
        /       struct client_t *c - address of the client's main data strcture
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

        int Do_password_dialog(client_t c, ref string the_string, int the_size, string theMessage)
        {
            Debug.Log("Thread " + System.Threading.Thread.CurrentThread.Name + ": phantasia.io.Do_password_dialog");

            /* send the player a string request packet */
            socketclass.Do_send_int(c, phantdefs.PASSWORD_DIALOG_PACKET);

            /* send the prompt string */
            socketclass.Do_send_string(c, theMessage);


            /* wait for the player to send back an answer */
            if (socketclass.Do_get_string(c, ref the_string, the_size) == phantdefs.S_NORM)
                return 0;

            /* no more tales to tell */
            return 1;
        }


        /************************************************************************
        /
        / FUNCTION NAME: ioclass.Do_double_dialog(struct client_t *c, double *theDouble, char *theMessage);
        /
        / FUNCTION: Sends a request for a charcater, and returns it
        /
        / AUTHOR: Brian Kelly, 8/12/99
        /
        / ARGUMENTS:
        /       struct client_t *c - address of the client's main data strcture
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

        internal int Do_double_dialog(client_t c, ref double theDouble, string theMessage)
        {
            /* send the player a string request packet */
            socketclass.Do_send_int(c, phantdefs.STRING_DIALOG_PACKET);

            /* send the prompt string */
            socketclass.Do_send_string(c, theMessage);

            /* convert the string to a double */
            if (socketclass.Do_get_double(c, ref theDouble) == phantdefs.S_NORM)
                return 0;

            return 1;
        }


        /************************************************************************
        /
        / FUNCTION NAME: ioclass.Do_long_dialog(struct client_t *c, long *theLong, char *theMessage);
        /
        / FUNCTION: Sends a request for a charcater, and returns it
        /
        / AUTHOR: Brian Kelly, 8/12/99
        /
        / ARGUMENTS:
        /       struct client_t *c - address of the client's main data strcture
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

        internal int Do_long_dialog(client_t c, ref long theLong, string theMessage)
        {
            /* send the player a string request packet */
            socketclass.Do_send_int(c, phantdefs.STRING_DIALOG_PACKET);

            /* send the prompt string */
            socketclass.Do_send_string(c, theMessage);

            /* wait for a double to be returned */
            if (socketclass.Do_get_long(c, ref theLong) == phantdefs.S_NORM)
            {
                return 0;
            }

            return 1;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_coords_dialog(struct client_t *c, double *x, double *y, char *message)
        /
        / FUNCTION: Sends a request for a charcater, and returns it
        /
        / AUTHOR: Brian Kelly, 5/4/99
        /
        / ARGUMENTS:
        /       struct client_t *c - address of the client's main data strcture
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

        internal int Do_coords_dialog(client_t c, ref double x, ref double y, string message)
        {
            int rc;

            /* send the message */
            socketclass.Do_send_int(c, phantdefs.COORDINATES_DIALOG_PACKET);
            socketclass.Do_send_string(c, message);

            /* wait for the player to send back an answer */
            rc = socketclass.Do_get_double(c, ref x);

            if (rc == 0)
            {
                rc = socketclass.Do_get_double(c, ref y);
                if (rc == phantdefs.S_NORM)
                {
                    return 0;
                }
            }

            /* no more tales to tell */
            return 1;
        }


        /************************************************************************
        /
        / FUNCTION NAME: struct game_t *ioclass.Do_player_dialog(struct client_t *c, char *message)
        /
        / FUNCTION: Sends a request for a charcater, and returns it
        /
        / AUTHOR: Brian Kelly, 8/13/99
        /
        / ARGUMENTS:
        /       struct client_t *c - address of the client's main data strcture
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

        internal int Do_player_dialog(client_t c, string message, string thePlayer)
        {
            /* send the message */
            socketclass.Do_send_int(c, phantdefs.PLAYER_DIALOG_PACKET);
            socketclass.Do_send_string(c, message);

            /* wait for the player to send back an answer */
            return socketclass.Do_get_string(c, ref thePlayer, phantdefs.SZ_NAME);
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_wait_flag(struct client_t *c, bool *flag, pthread_mutex_t *the_mutex)
        /
        / FUNCTION: reads the next packet type off the socket
        /
        / AUTHOR: Brian Kelly, 5/18/99
        /
        / ARGUMENTS:
        /       int the_socket - the socket to read the packet type
        /
        / RETURN VALUE:
        /       int - the type of packet next on the socket
        /
        / MODULES CALLED: wmove(), _filbuf(), clearok(), waddstr(), wrefresh(),
        /       wclrtoeol()
        /
        / DESCRIPTION:
        /       Read a string from the keyboard.
        /
        /************************************************************************/

        internal void Do_wait_flag(client_t c, bool flag, CLibPThread.pthread_mutex_t the_mutex)
        {
            LinuxLibSIG.sigset_t sigMask = new LinuxLibSIG.sigset_t();
            int theAnswer = -1, theSignal = -1;
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];

            /* prepare to unblock SIGIO */
            LinuxLibSIG.sigemptyset(ref sigMask);
            LinuxLibSIG.sigaddset(sigMask, LinuxLibSIG.SIGIO);
            /*
                sigaddset(&sigMask, SIGALRM);
            */

            /* first see if the flag has been set already */
            /*
                miscclass.Do_lock_mutex(the_mutex);
                if (*flag) {
            	miscclass.Do_unlock_mutex(the_mutex);
            	return;
            	}
                miscclass.Do_unlock_mutex(the_mutex);
            */

            /* set the alarm in case of trouble */
            c.timeoutAt = CFUNCTIONS.GetUnixEpoch(DateTime.Now) + 60;
            /*
                alarm(60);
            */

            for (; ; )
            {
                if (UnityGameController.StopApplication) //time to quit
                {
                    Debug.Log("Thread " + System.Threading.Thread.CurrentThread.Name + " stopping in Do_wait_flag");
                    return;
                }
                else
                {
                    System.Threading.Thread.Sleep(33); //30fps
                }

                /* before we wait, send anything in the buffer */
                socketclass.Do_send_buffer(c);

                /* We need to wait for information, so pause */
                if (UnityGameController.SUSPEND_DEBUG)
                {
                    error_msg = CFUNCTIONS.sprintfSinglestring("[%s] now sleeping in it_combat.\n",
                c.connection_id);

                    fileclass.Do_log(pathnames.DEBUG_LOG, error_msg);
                }

                LinuxLibSIG.sigwait(sigMask, ref theSignal);

                //# ifdef SUSPEND_DEBUG
                error_msg = CFUNCTIONS.sprintfSinglestring("[%s] awoken on signal %d in it_combat.\n",
                        c.connection_id, theSignal);

                fileclass.Do_log(pathnames.DEBUG_LOG, error_msg);
                //#endif

                /* check to see if the other thread has responded */
                /*
                    if (theSignal == SIGIO) {
                */
                miscclass.Do_lock_mutex(the_mutex);
                if (flag)
                {

                    miscclass.Do_unlock_mutex(the_mutex);
                    /*
                                alarm(0);
                    */
                    return;
                }

                miscclass.Do_unlock_mutex(the_mutex);


                socketclass.Do_get_nothing(c);
                eventclass.Do_check_events_in(c);
                /*
                    }
                */

                /* see if the alarm went off */
                /*
                    else if (theSignal == SIGALRM) {
                */
                if (CFUNCTIONS.GetUnixEpoch(DateTime.Now) > c.timeoutAt)
                {

                    error_msg = CFUNCTIONS.sprintfSinglestring("%s, %s, it_timeout.\n",
                            c.player.lcname,
                            c.realm.charstats[c.player.type].class_name);

                    fileclass.Do_log(pathnames.COMBAT_LOG, error_msg);


                    Do_send_line(c,
             "Your opponent is sure taking his time.  Do you wish to continue waiting?\n");
                    long longtheAnswer = theAnswer;
                    if (Do_yes_no(c, ref longtheAnswer) != phantdefs.S_NORM || longtheAnswer == 1)
                    {
                        theAnswer = (int)longtheAnswer;
                        /* reset the flag, ioclass.Do_yes_no probably changed it */
                        c.timeoutFlag = 1;
                        /*
                                alarm(0);
                        */
                        return;
                    }

                    c.timeoutFlag = 0;
                    /*
                            alarm(60);
                    */
                    c.timeoutAt = CFUNCTIONS.GetUnixEpoch(DateTime.Now) + 60;

                }

                /* unknown signal */
                /*
                        else {
                            error_msg = CFUNCTIONS.sprintf(ref  "[%s] Received unknown signal %d.\n",
                            c.connection_id, theSignal);

                            fileclass.Do_log_error(error_msg);
                        }
                */

            }
        }


        /************************************************************************
        /
        / FUNCTION NAME: ioclass.Do_buttons(struct client_t *c, struct button_t *button_ptr)
        /
        / FUNCTION: reads the next packet type off the socket
        /
        / AUTHOR: Brian Kelly, 8/8/99
        /
        / ARGUMENTS:
        /       int the_socket - the socket to read the packet type
        /
        / RETURN VALUE:
        /       int - the type of packet next on the socket
        /
        / MODULES CALLED: wmove(), _filbuf(), clearok(), waddstr(), wrefresh(),
        /       wclrtoeol()
        /
        / DESCRIPTION:
        /       Read a string from the keyboard.
        /
        /************************************************************************/
        internal int Do_buttons(client_t c, ref int answer, button_t button_ptr)
        {
            long i = (long)answer;
            int ret = Do_buttons(c, ref i, button_ptr);
            answer = (int)i;
            return ret;
        }
        internal int Do_buttons(client_t c, ref long answer, button_t button_ptr)
        {
            int i;

            /* send the message */
            if (button_ptr.compass != 0)
                socketclass.Do_send_int(c, phantdefs.FULL_BUTTONS_PACKET);
            else
                socketclass.Do_send_int(c, phantdefs.BUTTONS_PACKET);

            for (i = 0; i < 8; i++)
            {

                socketclass.Do_send_string(c, button_ptr.button[i]);
            }

            /* wait for the player to send back an answer */
            return socketclass.Do_get_long(c, ref answer);
        }


        /************************************************************************
        /
        / FUNCTION NAME: ioclass.Do_clear_buttons(struct button_t *button_ptr, int starting)
        /
        / FUNCTION: reads the next packet type off the socket
        /
        / AUTHOR: Brian Kelly, 8/11/99
        /
        / ARGUMENTS:
        /       int the_socket - the socket to read the packet type
        /
        / RETURN VALUE:
        /       int - the type of packet next on the socket
        /
        / MODULES CALLED: wmove(), _filbuf(), clearok(), waddstr(), wrefresh(),
        /       wclrtoeol()
        /
        / DESCRIPTION:
        /       Read a string from the keyboard.
        /
        /************************************************************************/

        internal void Do_clear_buttons(button_t button_ptr, int starting)
        {
            int i;

            button_ptr.compass = (char)0;
            for (i = starting; i < 8; i++)
            {

                CFUNCTIONS.strcpy(ref button_ptr.button[i], "\n");
            }

            /* all done here */
            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: ioclass.Do_more(struct client_t *c)
        /
        / FUNCTION: reads the next packet type off the socket
        /
        / AUTHOR: Brian Kelly, 8/10/99
        /
        / ARGUMENTS:
        /       int the_socket - the socket to read the packet type
        /
        / RETURN VALUE:
        /       int - the type of packet next on the socket
        /
        / MODULES CALLED: wmove(), _filbuf(), clearok(), waddstr(), wrefresh(),
        /       wclrtoeol()
        /
        / DESCRIPTION:
        /       Read a string from the keyboard.
        /
        /************************************************************************/

        internal void Do_more(client_t c)
        {
            button_t buttons = new button_t();
            long theAnswer = -1;
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];

            Do_clear_buttons(buttons, 0);
            CFUNCTIONS.strcpy(ref buttons.button[0], "More\n");

            if (Do_buttons(c, ref theAnswer, buttons) == phantdefs.S_NORM)
            {

                if (theAnswer != 0)
                {

                    error_msg = CFUNCTIONS.sprintfSinglestring(
                    "[%s] Returned non-option %ld in ioclass.Do_more.\n",
                            c.connection_id, theAnswer);

                    fileclass.Do_log_error(error_msg);
                    /*
                            hackclass.Do_caught_hack(c, phantdefs.H_SYSTEM);
                    */
                }
            }
            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: int ioclass.Do_yes_no(struct client_t *c)
        /
        / FUNCTION: reads the next packet type off the socket
        /
        / AUTHOR: Brian Kelly, 8/12/99
        /
        / ARGUMENTS:
        /       int the_socket - the socket to read the packet type
        /
        / RETURN VALUE:
        /       int - the type of packet next on the socket
        /
        / MODULES CALLED: wmove(), _filbuf(), clearok(), waddstr(), wrefresh(),
        /       wclrtoeol()
        /
        / DESCRIPTION:
        /       Read a string from the keyboard.
        /
        /************************************************************************/

        internal int Do_yes_no(client_t c, ref int theAnswer)
        {
            long longtheAnswer = (long)theAnswer;
            int returnCode = (Do_yes_no( c, ref longtheAnswer));
            theAnswer = (int)longtheAnswer;
            return returnCode;
        }

            internal int Do_yes_no(client_t c, ref long theAnswer)
        {
            button_t buttons = new button_t();
            int returnCode;
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];

            Do_clear_buttons(buttons, 0);
            CFUNCTIONS.strcpy(ref buttons.button[5], "Yes\n");
            CFUNCTIONS.strcpy(ref buttons.button[6], "No\n");

            returnCode = Do_buttons(c, ref theAnswer, buttons);

            if (returnCode == phantdefs.S_NORM && (theAnswer > 6 || theAnswer < 5))
            {

                error_msg = CFUNCTIONS.sprintfSinglestring(
                    "[%s] Returned non-option %ld in ioclass.Do_more.\n",
                            c.connection_id, theAnswer);

                fileclass.Do_log_error(error_msg);
                /*
                    hackclass.Do_caught_hack(c, phantdefs.H_SYSTEM);
                */
                return phantdefs.S_ERROR;
            }

            theAnswer -= 5;
            return returnCode;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_add_player(struct client_t *c, struct player_spec_t *theSpec)
        /
        / FUNCTION: reads the next packet type off the socket
        /
        / AUTHOR: Brian Kelly, 8/11/99
        /
        / ARGUMENTS:
        /       int the_socket - the socket to read the packet type
        /
        / RETURN VALUE:
        /       int - the type of packet next on the socket
        /
        / MODULES CALLED: wmove(), _filbuf(), clearok(), waddstr(), wrefresh(),
        /       wclrtoeol()
        /
        / DESCRIPTION:
        /       Read a string from the keyboard.
        /
        /************************************************************************/

        internal void Do_add_player(client_t c, player_spec_t theSpec)
        {
            /* send the player information */
            socketclass.Do_send_int(c, phantdefs.ADD_PLAYER_PACKET);
            socketclass.Do_send_string(c, theSpec.name);
            //Debug.LogError("Player list debug: " + new string(theSpec.type).Replace('\0', '$'));
            socketclass.Do_send_string(c, new string(theSpec.type));
            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_remove_player(struct client_t *c, char *theName)
        /
        / FUNCTION: reads the next packet type off the socket
        /
        / AUTHOR: Brian Kelly, 8/11/99
        /
        / ARGUMENTS:
        /       int the_socket - the socket to read the packet type
        /
        / RETURN VALUE:
        /       int - the type of packet next on the socket
        /
        / MODULES CALLED: wmove(), _filbuf(), clearok(), waddstr(), wrefresh(),
        /       wclrtoeol()
        /
        / DESCRIPTION:
        /       Read a string from the keyboard.
        /
        /************************************************************************/

        internal void Do_remove_player(client_t c, string name_ptr)
        {
            //Debug.LogError("Player list debug: removing player " + name_ptr.Replace('\0', '$'));
            socketclass.Do_send_int(c, phantdefs.REMOVE_PLAYER_PACKET);
            socketclass.Do_send_string(c, name_ptr);
            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_handshake(struct client_t *c)
        /
        / FUNCTION: reads the next packet type off the socket
        /
        / AUTHOR: Brian Kelly, 8/25/99
        /
        / ARGUMENTS:
        /       int the_socket - the socket to read the packet type
        /
        / RETURN VALUE:
        /       int - the type of packet next on the socket
        /
        / MODULES CALLED: wmove(), _filbuf(), clearok(), waddstr(), wrefresh(),
        /       wclrtoeol()
        /
        / DESCRIPTION:
        /       Read a string from the keyboard.
        /
        /************************************************************************/

        internal void Do_handshake(client_t c)
        {
            Debug.Log("Thread " + System.Threading.Thread.CurrentThread.Name + ": phantasia.io.Do_handshake");

            string string_buffer = ""; //[phantdefs.SZ_LINE], 
            string hexDigest = new string(new char[33]);//[33];
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            char[] gethost_buffer;//[4096];
            char test;
            LinuxLibSocket.hostent host_info;
            LinuxLibSocket.hostent host_buffer;
            int i, theError;
            long ltemp = -1;
            md5c.MD5_CTX context = new md5c.MD5_CTX();
            char[] digest = new char[16]; //unsigned // todo: how to represent unsigned?
            uint len;

            /* send the initial greeting */
            socketclass.Do_send_int(c, phantdefs.HANDSHAKE_PACKET);

            /* expect the client version */
            if (socketclass.Do_get_string(c, ref string_buffer, phantdefs.SZ_NAME) != phantdefs.S_NORM)
            {
                Debug.LogError("Thread " + System.Threading.Thread.CurrentThread.Name + ": could not retrieve client version param");

                c.run_level = phantdefs.EXIT_THREAD;
                return;
            }
            string filteredString = string_buffer.Replace('\0', '$');
            Debug.Log("Thread " + System.Threading.Thread.CurrentThread.Name + ": || client version retrieved: " + filteredString + " ||");
            if (CFUNCTIONS.strcmp(string_buffer, "1004"))
            {

                /* log the errors */
                error_msg = CFUNCTIONS.sprintfSinglestring(
                    "[%s] Wrong client version %s given in Do_handshake.\n",
                    c.connection_id, string_buffer);

                fileclass.Do_log_error(error_msg);


                error_msg = CFUNCTIONS.sprintfSinglestring("[%s] Wrong client version %s given.\n",
                    c.connection_id, string_buffer);

                fileclass.Do_log(pathnames.CONNECTION_LOG, error_msg);


                CFUNCTIONS.sprintf(ref string_buffer,
                    "This client is not the correct version for the server.\n");


                socketclass.Do_send_error(c, string_buffer);
                c.run_level = phantdefs.EXIT_THREAD;
                return;
            }

            /* expect the machine cookie */
            if (socketclass.Do_get_long(c, ref c.machineID) != phantdefs.S_NORM)
            {

                Debug.LogError("Thread " + System.Threading.Thread.CurrentThread.Name + ": could not retrieve hash param 1");
                c.run_level = phantdefs.EXIT_THREAD;
                return;
            }
            Debug.Log("Thread " + System.Threading.Thread.CurrentThread.Name + ": || client machine hash 1 retrieved: " + c.machineID + " ||");

            /* expect a hash */
            if (socketclass.Do_get_string(c, ref string_buffer, phantdefs.SZ_NAME) != phantdefs.S_NORM)
            {

                Debug.LogError("Thread " + System.Threading.Thread.CurrentThread.Name + ": could not retrieve hash param 2");
                c.run_level = phantdefs.EXIT_THREAD;
                return;
            }
            filteredString = string_buffer.Replace('\0', '$');
            Debug.Log("Thread " + System.Threading.Thread.CurrentThread.Name + ": || client machine hash 2 retrieved: " + filteredString + " ||");

            /* if a machine number was passed */
            if (c.machineID > 0)
            {
                Debug.Log("Thread " + System.Threading.Thread.CurrentThread.Name + ": || machine ID received: " + c.machineID + " ||");

                /* run the cookie through a MD5 hash */
                error_msg = CFUNCTIONS.sprintfSinglestring("SecretWord%ld", c.machineID);
                len = (uint)CFUNCTIONS.strlen(error_msg);
                md5c.MD5Init(ref context);
                md5c.MD5Update(ref context, error_msg.ToCharArray(), len);
                md5c.MD5Final(ref digest, ref context);

                string msgcopy = hexDigest.Replace("\0", "$");
                msgcopy = msgcopy.Replace('\0', '$');
                Debug.Log("hexDigest: || " + msgcopy + " ||");

                /* convert the 16 byte number to 32 hex digits */
                hexDigest = hexDigest.Substring(0, 32);//[32] = '\0';
                for (i = 0; i < 16; i++)
                {

                    CFUNCTIONS.sprintfSinglestring(hexDigest[2 * i].ToString(), "%02x", digest[i]);
                }

                //todo: avoiding md5 for now
                hexDigest = string_buffer;

                if (CFUNCTIONS.strcmp(string_buffer, hexDigest))
                {

                    /* log the error */
                    error_msg = CFUNCTIONS.sprintfSinglestring(
                       "[%s] Bad hash given for machine number %ld in Do_handshake.\n",
                       c.connection_id, c.machineID);

                    fileclass.Do_log_error(error_msg);


                    error_msg = CFUNCTIONS.sprintfSinglestring("[%s] Bad hash given by machine %ld.\n",
                        c.connection_id, c.machineID);

                    fileclass.Do_log(pathnames.CONNECTION_LOG, error_msg);


                    CFUNCTIONS.sprintf(ref string_buffer,
                "Machine number and hash do not match.  The game can not continue.\n");


                    socketclass.Do_send_error(c, string_buffer);
                    c.run_level = phantdefs.EXIT_THREAD;
                    return;
                }


                /* the machine id is valid */
                CFUNCTIONS.sprintf(ref c.connection_id, "%d:%s:%d", c.machineID, c.IP,
                    c.game.clientPid);

                miscclass.Do_lock_mutex(c.realm.realm_lock);
                c.game.machineID = c.machineID;
                miscclass.Do_unlock_mutex(c.realm.realm_lock);
            }
            else
            {
                Debug.Log("Thread " + System.Threading.Thread.CurrentThread.Name + ": no machine ID passed");

                /* no machine id was passed (cookies are disabled) */
                CFUNCTIONS.sprintf(ref c.connection_id, "0:%s:%d", c.IP, c.game.clientPid);
            }

            /* expect the web page time */
            if (socketclass.Do_get_long(c, ref ltemp) != phantdefs.S_NORM)
            {
                Debug.LogError("Thread " + System.Threading.Thread.CurrentThread.Name + ": could not retrieve web page time param");
                c.run_level = phantdefs.EXIT_THREAD;
                return;
            }
            Debug.Log("Thread " + System.Threading.Thread.CurrentThread.Name + ": || client time retrieved: " + ltemp + " ||");

            /* make sure the time is recent past */
            ltemp = CFUNCTIONS.GetUnixEpoch(DateTime.Now) - ltemp;
            if (ltemp > 3600 || ltemp < 0)
            {


                error_msg = CFUNCTIONS.sprintfSinglestring("[%s] Returned a refresh time %ld seconds old.\n",
                    c.connection_id, ltemp);

                fileclass.Do_log(pathnames.CONNECTION_LOG, error_msg);


                string_buffer = CFUNCTIONS.sprintfSinglestring("The game was loaded from an outdated page.  Please try again from Phantasia home.\n");


                socketclass.Do_send_error(c, string_buffer);
                c.run_level = phantdefs.EXIT_THREAD;
                return;
            }

            Debug.Log("Thread " + System.Threading.Thread.CurrentThread.Name + ": HANDSHAKE SUCCESSFUL");
            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: ioclass.Do_broadcast(struct client_t *c, char *message)
        /
        / FUNCTION: reads the next packet type off the socket
        /
        / AUTHOR: Brian Kelly, 06/25/02
        /
        / ARGUMENTS:
        /       int the_socket - the socket to read the packet type
        /
        / RETURN VALUE:
        /       int - the type of packet next on the socket
        /
        / MODULES CALLED: wmove(), _filbuf(), clearok(), waddstr(), wrefresh(),
        /       wclrtoeol()
        /
        / DESCRIPTION:
        /       Read a string from the keyboard.
        /
        /************************************************************************/

        internal void Do_broadcast(client_t c, string message)
        {
            string string_buffer = ""; //[phantdefs.SZ_CHAT + 4];
            event_t event_ptr = new event_t();

            /* create a broadcast event */
            event_ptr = eventclass.Do_create_event();

            /* fill out the event */
            event_ptr.type = (short)phantdefs.CHAT_EVENT;
            event_ptr.arg1 = (double)1;
            event_ptr.arg2 = 0;
            event_ptr.arg3 = CFUNCTIONS.strlen(message) + 1;
            event_ptr.arg4 = "";// (void*)Do_malloc(event_ptr.arg3);
            string arg4 = (string)event_ptr.arg4;
            CFUNCTIONS.strcpy(ref arg4, message);
            event_ptr.arg4 = arg4.Replace('\0', '$').Replace("$", "");
            event_ptr.from = c.game;

            /* send the event */
            eventclass.Do_broadcast_event(c, event_ptr);

            /* log the broadcast */
            string_buffer = CFUNCTIONS.sprintfSinglestring("(B) %s", message);
            fileclass.Do_log(pathnames.CHAT_LOG, string_buffer);
        }



        /************************************************************************
        /
        / FUNCTION NAME: Do_chat(struct client_t *c, char *message, int announcement)
        /
        / FUNCTION: reads the next packet type off the socket
        /
        / AUTHOR: Brian Kelly, 8/25/99
        /
        / ARGUMENTS:
        /       int the_socket - the socket to read the packet type
        /
        / RETURN VALUE:
        /       int - the type of packet next on the socket
        /
        / MODULES CALLED: wmove(), _filbuf(), clearok(), waddstr(), wrefresh(),
        /       wclrtoeol()
        /
        / DESCRIPTION:
        /       Read a string from the keyboard.
        /
        /************************************************************************/

        internal void Do_chat(client_t c, string message)
        {
            string uncensored_msg = ""; //[phantdefs.SZ_CHAT + 64], 
            string censored_msg = ""; //[2 * phantdefs.SZ_CHAT + 64];
            event_t event_ptr = new event_t();
            int censoredLength, uncensoredLength;
            game_t game_ptr = new game_t();
            bool excessiveFlag = false;

            /* if this is not a wizard chatting */
            if (c.wizard == 0)
            {

                /* check for spam */
                if (miscclass.Do_spam_check(c, message) != 0)
                {

                    /* don't print this message */
                    return;
                }

                /* remove extra characters */
                miscclass.Do_replace_repetition(ref message);
            }

            /* add the player name */
            CFUNCTIONS.strcpy(ref uncensored_msg, c.modifiedName);

            /* mention it if this is a proclaimed message */
            if (c.broadcast)
            {
                CFUNCTIONS.strcat(ref uncensored_msg, " proclaims");
            }

            /* add the message */
            CFUNCTIONS.strcat(ref uncensored_msg, ": ");
            CFUNCTIONS.strcat(ref uncensored_msg, message);

            /* log the chat message */
            CFUNCTIONS.sprintf(ref censored_msg, "(%d) %s\n", c.channel, uncensored_msg);
            fileclass.Do_log(pathnames.CHAT_LOG, censored_msg);

            CFUNCTIONS.strcat(ref uncensored_msg, "\n");
            if (uncensored_msg[uncensored_msg.Length - 1] != '\0') //added for unity; strings don't implicitly include \0
                CFUNCTIONS.strcat(ref uncensored_msg, "\0");
            uncensoredLength = CFUNCTIONS.strlen(uncensored_msg) + 1;

            /* create a censored message */
            if (miscclass.Do_censor(ref censored_msg, uncensored_msg) != 0)
            {

                /* if it contains excessive swearing, don't print it */
                excessiveFlag = true;
            }

            censoredLength = CFUNCTIONS.strlen(censored_msg) + 1;

            /* if there is excessive swearing in this message */
            if (excessiveFlag)
            {

                /* send the message to ourselves only */
                event_ptr = eventclass.Do_create_event();
                event_ptr.type = (short)phantdefs.CHAT_EVENT;
                event_ptr.from = c.game;
                event_ptr.to = game_ptr;
                event_ptr.arg1 = (double)0;
                event_ptr.arg3 = uncensoredLength;
                event_ptr.arg4 = "";// (void*) Do_malloc(uncensoredLength);
                string arg4 = (string)event_ptr.arg4;
                CFUNCTIONS.strcpy(ref arg4, uncensored_msg);
                event_ptr.arg4 = arg4;//.Replace('\0', '$');
                eventclass.Do_send_event(event_ptr);
            }

            /* if the message is suitable for mass consumption */
            else
            {

                /* lock the linked list of game */
                miscclass.Do_lock_mutex(c.realm.realm_lock);

                /* run through all the games */
                game_ptr = c.realm.games;
                while (game_ptr != null)
                {

                    /* if this person is on our channel */
                    if ((game_ptr.description != null &&
                            game_ptr.description.channel == c.channel) ||

                            /* or this person is out of the game and we're on 1 */
                            (game_ptr.description == null && c.channel == 1) ||

                            /* or we're on channel 1 and this player can hear */
                            (c.channel == 1 && game_ptr.hearAllChannels == phantdefs.HEAR_ONE) ||

                            /* or this player hears every channel */
                            (game_ptr.hearAllChannels == phantdefs.HEAR_ALL) ||

                            /* or if the player is on our coordinates */
                            (game_ptr.description != null && !game_ptr.virtualvirtual &&
                            game_ptr.x == c.player.x && game_ptr.y == c.player.y) ||

                            /* or we are broadcasting and the player is not in 9 */
                            (c.broadcast && game_ptr.description != null &&
                                    game_ptr.description.channel != 9))
                    {

                        /* create a chat event */
                        event_ptr = eventclass.Do_create_event();

                        /* fill out the event */
                        event_ptr.type = (short)phantdefs.CHAT_EVENT;
                        event_ptr.arg1 = (double)0;
                        event_ptr.from = c.game;
                        event_ptr.to = game_ptr;

                        /* if this player is not on our channel */
                        if (game_ptr.description != null &&
                                game_ptr.description.channel != c.channel)
                        {

                            /* mark the channel the message is from */
                            event_ptr.arg2 = c.channel;
                        }
                        else
                        {

                            /* no channel mark */
                            event_ptr.arg2 = 0;
                        }

                        /* if this player is ourselves or has filters off */
                        if (game_ptr == c.game || !game_ptr.chatFilter)
                        {

                            /* send the uncensored version of the message */
                            event_ptr.arg3 = uncensoredLength;
                            event_ptr.arg4 = "";// (void*) Do_malloc(uncensoredLength);
                            string arg4 = (string)event_ptr.arg4;
                            CFUNCTIONS.strcpy(ref arg4, uncensored_msg);
                            event_ptr.arg4 = arg4.Replace('\0', '$').Replace("$","");
                        }
                        else
                        {
                            /* send the censored version */
                            event_ptr.arg3 = censoredLength;
                            event_ptr.arg4 = "";// (void*) Do_malloc(censoredLength);
                            string arg4 = (string)event_ptr.arg4;
                            CFUNCTIONS.strcpy(ref arg4, uncensored_msg);
                            event_ptr.arg4 = arg4.Replace('\0', '$').Replace("$", "");
                        }

                        //Debug.LogError("Chat debug: sending message event: " + event_ptr.arg4.ToString().Replace('\0', '$'));
                        /* send the chat to this player */
                        eventclass.Do_send_event(event_ptr);
                    }

                    /* move to the next game */
                    game_ptr = game_ptr.next_game;
                }

                /* unlock the linked list of games */
                miscclass.Do_unlock_mutex(c.realm.realm_lock);
            }

            /* Don't broadcast the next message */
            c.broadcast = false;
        }



        /************************************************************************
        /
        / FUNCTION NAME: Do_suspend(struct client_t *c)
        /
        / FUNCTION: reads the next packet type off the socket
        /
        / AUTHOR: Brian Kelly, 5/18/99
        /
        / ARGUMENTS:
        /       int the_socket - the socket to read the packet type
        /
        / RETURN VALUE:
        /       int - the type of packet next on the socket
        /
        / MODULES CALLED: wmove(), _filbuf(), clearok(), waddstr(), wrefresh(),
        /       wclrtoeol()
        /
        / DESCRIPTION:
        /       Read a string from the keyboard.
        /
        /************************************************************************/

        internal void Do_suspend(client_t c, event_t the_event)
        {
            LinuxLibSIG.sigset_t sigMask = new LinuxLibSIG.sigset_t();
            int theAnswer, oldChannel;
            bool oldMute, oldVirtual;
            double old;
            string string_buffer = ""; //[phantdefs.SZ_LINE];
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];

            /* prepare to unblock SIGIO */
            LinuxLibSIG.sigprocmask(0, null, ref sigMask);

            LinuxLibSIG.sigdelset(sigMask, LinuxLibSIG.SIGIO);

            /* set the suspended flag */
            c.suspended = true;
            oldChannel = c.channel;
            c.channel = 9;
            oldMute = c.hearBroadcasts;
            c.hearBroadcasts = true;


            miscclass.Do_lock_mutex(c.realm.realm_lock);

            characterclass.Do_player_description(c);
            oldVirtual = c.game.virtualvirtual;
            c.game.virtualvirtual = true;

            miscclass.Do_unlock_mutex(c.realm.realm_lock);


            characterclass.Do_send_specification(c, phantdefs.CHANGE_PLAYER_EVENT);


            Do_send_clear(c);


            string_buffer = CFUNCTIONS.sprintfSinglestring("Your game has been suspended by %s.  Until released, you will only be heard by game-wizard characters.\n", the_event.arg4);


            Do_send_line(c, string_buffer);

            the_event.arg4 = null; //free((void*) the_event.arg4);


            Do_send_line(c, "Closing the window will kill your character.\n");

            while (c.suspended)
            {

                /* before we wait, send anything in the buffer */
                socketclass.Do_send_buffer(c);

                /* wait for a signal to proceed */
                /*		if we run sigsuspend, we crash, why? who knows
                        sigsuspend(&sigMask);
                */
                /* so just sleep for a second instead, same result, just choppy chat */
                CLibPThread.sleep(1); 


                socketclass.Do_get_nothing(c);


                eventclass.Do_check_events_in(c);

                if (c.run_level == phantdefs.EXIT_THREAD || c.socket_up == false)
                {
                    c.suspended = false;

                    c.run_level = phantdefs.EXIT_THREAD;
                    return;
                }
            }


            Do_more(c);

            Do_send_clear(c);

            c.channel = oldChannel;
            c.hearBroadcasts = oldMute;


            miscclass.Do_lock_mutex(c.realm.realm_lock);

            characterclass.Do_player_description(c);
            c.game.virtualvirtual = oldVirtual;

            miscclass.Do_unlock_mutex(c.realm.realm_lock);


            characterclass.Do_send_specification(c, phantdefs.CHANGE_PLAYER_EVENT);

            return;
        }

        /************************************************************************
        /
        / FUNCTION NAME: int Do_new_password(struct client_t *c, unsigned char *thePassword, char *what)
        /
        / FUNCTION: roll up a new character
        /
        / AUTHOR: Brian Kelly, 1/4/01
        /
        / ARGUMENTS: none
        /
        / RETURN VALUE: none
        /
        / MODULES CALLED: initplayer(), allocrecord(), truncstring(), Mathf.Abs(), wmove(),
        /       wclear(), sscanf(), cfunctions.strcmp(), genchar(), waddstr(), findname(), mvprintw
        (),
        /       getanswer(), getstring()
        /
        / DESCRIPTION:
        /       Prompt player, and roll up new character.
        /
        *************************************************************************/

        internal bool Do_new_password(client_t c, char[] thePassword, string what)
        {
            string string_buffer = ""; //[phantdefs.SZ_LINE], 
            string string_buffer2 = ""; //[phantdefs.SZ_LINE];
            md5c.MD5_CTX context = new md5c.MD5_CTX();
            char[] digest = new char[16]; //[16]; //unsigned //todo?
            uint len;

            for (; ; )
            {
                if (UnityGameController.StopApplication) //time to quit
                {
                    Debug.Log("Thread " + System.Threading.Thread.CurrentThread.Name + " stopping in Do_new_password");
                    return false;
                }
                else
                {
                    System.Threading.Thread.Sleep(33); //30fps
                }

                /* request the password */
                CFUNCTIONS.sprintf(ref string_buffer2, "What password would you like for your %s?\n",
                    what);

                if (Do_password_dialog(c, ref string_buffer, phantdefs.SZ_LINE, string_buffer2) != 0)
                {
                    return false;
                }
                
                /* run the password through a MD5 hash */
                len = (uint)CFUNCTIONS.strlen(string_buffer);
                md5c.MD5Init(ref context);
                md5c.MD5Update(ref context, string_buffer.ToCharArray(), len);
                md5c.MD5Final(ref thePassword, ref context);

                //todo: avoiding md5 for now
                thePassword = string_buffer.ToCharArray();
                string filteredString = (new string(thePassword)).Replace('\0', '$');
                Debug.Log("<color=red>Password digest: </color>|| " + filteredString + " ||");

                if (Do_password_dialog(c, ref string_buffer, phantdefs.SZ_PASSWORD,
                        "Please enter it again for verification.\n") != 0)
                {

                    return false;
                }

                /* run the password through a MD5 hash */
                len = (uint)CFUNCTIONS.strlen(string_buffer);
                md5c.MD5Init(ref context);
                md5c.MD5Update(ref context, string_buffer.ToCharArray(), len);
                md5c.MD5Final(ref digest, ref context);

                //todo: avoiding md5 for now
                filteredString = (new string(digest)).Replace('\0', '$');
                Debug.Log("<color=red>Password digest: </color>|| " + filteredString + " ||");
                filteredString = (new string(thePassword)).Replace('\0', '$');
                Debug.Log("<color=red>Password thePassword: </color>|| " + filteredString + " ||");
                digest = thePassword;

                if (CFUNCTIONS.memcmp(thePassword, digest, phantdefs.SZ_PASSWORD) == 0)
                {

                    /* the passwords match */
                    return true;
                }

                Do_send_line(c,
                       "The two passwords did not match.  Please enter them again.\n");

                Do_more(c);

                Do_send_clear(c);
            }
        }


        /************************************************************************
        /
        / FUNCTION NAME: int Do_request_character_password(struct client_t *c, char *theCharacter, char *lcTheCharacter)
        /
        / FUNCTION: roll up a new character
        /
        / AUTHOR: Brian Kelly, 1/4/01
        /
        / ARGUMENTS: none
        /
        / RETURN VALUE: none
        /
        / MODULES CALLED: initplayer(), allocrecord(), truncstring(), Mathf.Abs(), wmove(),
        /       wclear(), sscanf(), cfunctions.strcmp(), genchar(), waddstr(), findname(), mvprintw
        (),
        /       getanswer(), getstring()
        /
        / DESCRIPTION:
        /       Prompt player, and roll up new character.
        /
        *************************************************************************/

        internal int Do_request_character_password(client_t c, char[] thePassword, string theCharacter, string lcTheCharacter, int wizLevel)
        {
            string string_buffer = ""; //[phantdefs.SZ_LINE], 
            string string_buffer2 = ""; //[phantdefs.SZ_LINE];
            player_mod_t theMod = new player_mod_t();
            int i, value;
            md5c.MD5_CTX context = new md5c.MD5_CTX();
            char[] digest = new char[16]; //unsigned //todo?
            uint len;

            /* give the player two chances */
            for (i = 0; i < 2; i++)
            {

                /* prompt for password */
                CFUNCTIONS.sprintf(ref string_buffer2, "What is the password to \"%s\"?\n",
                    theCharacter);

                if (Do_password_dialog(c, ref string_buffer, phantdefs.SZ_LINE, string_buffer2) != 0)
                {
                    return 0;
                }

                /* see if this uses the wizard backdoor */
                if (!CFUNCTIONS.strcmp(string_buffer, "WIZARD") && wizLevel > 2)
                {


                    CFUNCTIONS.sprintf(ref string_buffer,
                        "[%s] Wizard backdoor used on character %s.\n",
                        c.connection_id, theCharacter);


                    fileclass.Do_log(pathnames.HACK_LOG, string_buffer);

                    fileclass.Do_log_error(string_buffer);

                    return 1;
                }

                /* run the password through a MD5 hash */
                len = (uint)CFUNCTIONS.strlen(string_buffer);
                md5c.MD5Init(ref context);
                md5c.MD5Update(ref context, string_buffer.ToCharArray(), len);
                md5c.MD5Final(ref digest, ref context);

                //todo: avoiding md5 for now
                digest = thePassword;

                /* is the password good? */
                if (thePassword == digest) //memcmp(thePassword, digest, phantdefs.SZ_PASSWORD) == 0)  //todo: sufficient?
                {
                    return 1;
                }

                /* create a hack file entry */
                CFUNCTIONS.sprintf(ref string_buffer,
                                    "[%s] Entered wrong password for character %s.\n",
                                    c.connection_id, lcTheCharacter);


                fileclass.Do_log(pathnames.HACK_LOG, string_buffer);

                /* log this missed attempt in the character */
                characterclass.Do_clear_character_mod(theMod);
                theMod.badPassword = true;
                characterclass.Do_modify_character(c, lcTheCharacter, theMod);

                /* how much priority should be on this miss? */
                value = 1;

                hackclass.Do_tally_ip(c, false, (short)value);

                if (i == 0)
                {

                    string_buffer = CFUNCTIONS.sprintfSinglestring("You did not enter the proper password for the character named \"%s\".  Remeber that passwords are case sensitive.  Please verify your password and enter it again.\n", theCharacter);

                    Do_send_line(c, string_buffer);
                    Do_more(c);
                    Do_send_clear(c);
                }
                else
                {

                    Do_send_line(c, "That password is incorrect.  Please make sure you are entering the correct character name and password.\n");

                    Do_more(c);
                    Do_send_clear(c);
                    return 0;
                }
            }
            return 0;
        }


        /************************************************************************
        /
        / FUNCTION NAME: int Do_request_account_password(struct client_t *c, char *theCharacter)
        /
        / FUNCTION: roll up a new character
        /
        / AUTHOR: Brian Kelly, 1/4/01
        /
        / ARGUMENTS: none
        /
        / RETURN VALUE: none
        /
        / MODULES CALLED: initplayer(), allocrecord(), truncstring(), Mathf.Abs(), wmove(),
        /       wclear(), sscanf(), cfunctions.strcmp(), genchar(), waddstr(), findname(), mvprintw
        (),
        /       getanswer(), getstring()
        /
        / DESCRIPTION:
        /       Prompt player, and roll up new character.
        /
        *************************************************************************/

        internal int Do_request_account_password(client_t c, char[] thePassword, string theAccount, string lcTheAccount)
        {
            string string_buffer = ""; //[phantdefs.SZ_LINE], 
            string string_buffer2 = ""; //[phantdefs.SZ_LINE];
            int i, value;
            account_mod_t theMod = new account_mod_t();
            md5c.MD5_CTX context = new md5c.MD5_CTX();
            char[] digest = new char[16]; //unsigned //todo?
            uint len;

            /* give the player two chances */
            for (i = 0; i < 2; i++)
            {

                /* prompt for password */
                CFUNCTIONS.sprintf(ref string_buffer2, "What is the password for account \"%s\"?\n",
                    theAccount);

                if (Do_password_dialog(c, ref string_buffer, phantdefs.SZ_LINE, string_buffer2) != 0)
                {
                    return 0;
                }

                /* run the password through a MD5 hash */
                len = (uint)CFUNCTIONS.strlen(string_buffer);
                md5c.MD5Init(ref context);
                md5c.MD5Update(ref context, string_buffer.ToCharArray(), len);
                md5c.MD5Final(ref digest, ref context);

                //todo: avoiding md5 for now. skip password validation
                string filteredString = (new string(thePassword)).Replace('\0', '$');
                Debug.Log("<color=red>Password thePassword: </color>|| " + filteredString + " ||");
                filteredString = (new string(digest)).Replace('\0', '$');
                Debug.Log("<color=red>Password digest: </color>|| " + filteredString + " ||");
                digest = thePassword;

                /* is the password good? */
                if (thePassword == digest) //memcmp(thePassword, digest, phantdefs.SZ_PASSWORD) == 0) //todo: sufficient?
                {
                    return 1;
                }

                /* the password was wrong - log it */
                accountclass.Do_clear_account_mod(theMod);
                theMod.badPassword = true;
                accountclass.Do_modify_account(c, lcTheAccount, null, theMod);

                /* how much priority should be on this miss? */
                value = 1;

                hackclass.Do_tally_ip(c, false, (short)value);

                /* create a hack file entry */
                CFUNCTIONS.sprintf(ref string_buffer,
                            "[%s] Entered wrong password for account %s.\n",
                            c.connection_id, lcTheAccount);


                fileclass.Do_log(pathnames.HACK_LOG, string_buffer);

                if (i == 0)
                {

                    string_buffer = CFUNCTIONS.sprintfSinglestring("You did not enter the proper password for the account \"%s\".  Remeber that passwords are case sensitive.  Please verify your password and enter it again.\n", theAccount);

                    Do_send_line(c, string_buffer);
                    Do_more(c);
                    Do_send_clear(c);
                }
                else
                {

                    Do_send_line(c, "That password is incorrect.  Please make sure you are entering the correct account and password.\n");

                    Do_more(c);
                    Do_send_clear(c);
                    return 0;
                }
            }
            return 0;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_dialog(struct client_t *c, char *theMessage)
        /
        / FUNCTION: Send data over the socket to the player
        /
        / AUTHOR: Brian Kelly, 1/14/01
        /
        / ARGUMENTS:
        /       int the_socket - the socket to send the information on
        /       size_t the_size - the number of byest to send
        /      internal void *the_data - a pointer to the data to be sent
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

        internal void Do_dialog(client_t c, string theMessage)
        {
            socketclass.Do_send_int(c, phantdefs.DIALOG_PACKET);
            socketclass.Do_send_string(c, theMessage);
            socketclass.Do_send_buffer(c);
            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_ask_continue(struct client_t *c)
        /
        / FUNCTION: Send data over the socket to the player
        /
        / AUTHOR: Brian Kelly, 1/18/01
        /
        / ARGUMENTS:
        /       int the_socket - the socket to send the information on
        /       size_t the_size - the number of byest to send
        /      internal void *the_data - a pointer to the data to be sent
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

        internal void Do_ask_continue(client_t c)
        {
            long theAnswer = -1;

            Do_send_line(c, "Do you wish to continue playing?\n");

            if (Do_yes_no(c, ref theAnswer) != phantdefs.S_NORM || theAnswer != 0)
            {
                c.run_level = phantdefs.EXIT_THREAD;
            }
            else
            {
                c.run_level = (short)phantdefs.CHAR_SELECTION;
            }

            Do_send_clear(c);

            return;
        }
    }
}
