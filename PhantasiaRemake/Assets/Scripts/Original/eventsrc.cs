using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

namespace phantasiaclasses
{
    public class eventsrc //: MonoBehaviour
    {
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

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        static eventsrc Instance;
        private eventsrc()
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
            tagsclass = tags.GetInstance();
            statsclass = stats.GetInstance();
            commandsclass = commands.GetInstance();
            socketclass = socket.GetInstance();
            infoclass = info.GetInstance();
            treasureclass = treasure.GetInstance();
            restoreclass = restore.GetInstance();
            itcombatclass = itcombat.GetInstance();
            fightclass = fight.GetInstance();
        }
        public static eventsrc GetInstance()
        {
            eventsrc instance = null;
            if (Instance != null)
            {
                instance = Instance;
            }
            else
            {
                instance = new eventsrc();
            }
            return instance;
        }


        /************************************************************************
        /
        / FUNCTION NAME: eventclass.Do_create_event(void)
        /
        / FUNCTION: To create event structures
        /
        / AUTHOR: Brian Kelly, 4/16/99
        /
        / ARGUMENTS: none
        /
        / RETURN VALUE:
        /       event_t - pointer to the new event
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

        internal event_t Do_create_event()
        {
            event_t temp_event_ptr;

            /* create an event structure */
            temp_event_ptr = new event_t();// (event_t)Do_malloc(phantdefs.SZ_EVENT);

            /* initialize event information */
            temp_event_ptr.type = (short)phantdefs.NULL_EVENT;
            temp_event_ptr.arg1 = 0.0;
            temp_event_ptr.arg2 = 0.0;
            temp_event_ptr.arg3 = 0;
            temp_event_ptr.arg4 = null;
            temp_event_ptr.next_event = null;
            temp_event_ptr.from = null;
            temp_event_ptr.to = null;

            /* return the address of the new event */
            return temp_event_ptr;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_duplicate_event(struct event_t *event)
        /
        / FUNCTION: To duplicate event structures
        /
        / AUTHOR: Brian Kelly, 4/12/99
        /
        / ARGUMENTS:
        /       event_t *orig - the source event
        /
        / RETURN VALUE:
        /       event_t - pointer to the new event
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

        event_t Do_duplicate_event(event_t the_event)
        {
            event_t event_ptr;

            /* create a duplicate event structure */
            event_ptr = new event_t();// (struct event_t *)eventclass.Do_create_event();

            /* duplicate event information */
            event_ptr = the_event;// memcpy((void*) event_ptr, (void*) the_event, phantdefs.SZ_EVENT);

            /* copy an attached object, if necessary */
            if (the_event.arg4 != null)
            {
                event_ptr.arg4 = new event_t(); //(void*) Do_malloc((int) the_event.arg3);
                event_ptr.arg4 = the_event.arg4;// memcpy(event_ptr.arg4, the_event.arg4, (int) the_event.arg3);
            }

            /* return the address of the new event */
            return event_ptr;
        }


        /************************************************************************
        /
        / FUNCTION NAME: eventclass.Do_send_event(event_t *event_ptr)
        /
        / FUNCTION: Send an event to its proper destination
        /
        / AUTHOR: Brian Kelly, 4/12/99
        /
        / ARGUMENTS:
        /       event_t *event - the event to be placed
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

        internal void Do_send_event(event_t the_event)
        {
            /*
                struct event_t **event_ptr_ptr;
            */

            /* lock the destination event queue */
            miscclass.Do_lock_mutex(the_event.to.events_in_lock);

            /* point the current list after this event */
            the_event.next_event = the_event.to.events_in;

            /* point the list to start with this event */
            the_event.to.events_in = the_event;

            /* move to the last event in the queue */
            /*
                event_ptr_ptr = &the_event.to.events_in;
                while (*event_ptr_ptr != null) {
                event_ptr_ptr = &(*event_ptr_ptr).next_event;
                }
            */

            /* point the last event at this new one */
            /*
                *event_ptr_ptr = the_event;
            */

            /* send an interrupt to the thread */
            CLibPThread.pthread_kill(the_event.to.the_thread, LinuxLibSIG.SIGIO);

            /* unlock the destination event queue */
            miscclass.Do_unlock_mutex(the_event.to.events_in_lock);

            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: eventclass.Do_send_self_event(struct client_t *c, int type)
        /
        / FUNCTION: Send an event to its proper destination
        /
        / AUTHOR: Brian Kelly, 5/8/99
        /
        / ARGUMENTS:
        /       event_t *event - the event to be placed
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

        internal void Do_send_self_event(client_t c, int type)
        {
            event_t event_ptr = new event_t();

            /* create a new event */
            event_ptr = Do_create_event();

            /* fill out the necessary information */
            event_ptr.type = (short)type;
            event_ptr.from = c.game;
            event_ptr.to = c.game;

            /* send off the event */
            Do_file_event(c, event_ptr);

            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_broadcast_event()
        /
        / FUNCTION: Send an event to every game 
        /
        / AUTHOR: Brian Kelly, 4/26/99
        /
        / ARGUMENTS:
        /	struct client_t c - a pointer to the client strcture
        /       struct event_t *event - the event to be placed
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

        internal void Do_broadcast_event(client_t c, event_t the_event)
        {
            game_t game_ptr = new game_t();
            event_t event_ptr = new event_t();

            /* lock the linked list of game */
            miscclass.Do_lock_mutex(c.realm.realm_lock);

            /* run thorough all the games */
            game_ptr = c.realm.games;
            while (game_ptr != null)
            {

                /* if we're not point to ourselves and this game hears */
                if (game_ptr != c.game && game_ptr.sendEvents)
                {

                    /* duplicate the event */
                    event_ptr = Do_duplicate_event(the_event);

                    /* address the event to the current game */
                    event_ptr.to = game_ptr;

                    /* send off the event */
                    Do_send_event(event_ptr);
                }

                /* move to the next game */
                game_ptr = game_ptr.next_game;
            }

            /* unlock the linked list of games */
            miscclass.Do_unlock_mutex(c.realm.realm_lock);

            /* send the original event to this game */
            the_event.to = c.game;
            Do_file_event(c, the_event);

            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: eventclass.Do_file_event(struct client_t *c, struct event_t *event_ptr)
        /
        / FUNCTION: Send an event from a client
        /
        / AUTHOR: Brian Kelly, 4/24/99
        /
        / ARGUMENTS:
        /       client_t *c - the pointer to the main client strcture
        /       event_t *event - the event to be placed
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

        internal void Do_file_event(client_t c, event_t the_event)
        {
            event_t event_ptr = new event_t();
            event_t event_ptr_ptr = new event_t();
            //player_info_t pinfo_ptr = new player_info_t(); //unused
            bool flag;
            string string_buffer = ""; //[phantdefs.SZ_LINE], 
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            CLibFile.CFILE ban_file;

            /* switch on the kind of event */
            switch (the_event.type)
            {

                /* kick this player off the game NOW */
                case phantdefs.KICK_EVENT:

                    CFUNCTIONS.strcpy(ref error_msg, (string)the_event.arg4);
                    CFUNCTIONS.strcat(ref error_msg, " has kicked you out of the game.\n");

                    /* send an error packet */
                    socketclass.Do_send_error(c, error_msg);

                    /* accept no more input from this player */
                    c.socket_up = false;

                    /* get out of the game ASAP */
                    c.run_level = phantdefs.EXIT_THREAD;

                    /* delete the event */
                    the_event.arg4 = null; //free(the_event.arg4);
                    the_event = null; //free((void*) the_event);

                    /* nothing more to do here */
                    break;

                /* if the game is being tagged */
                case phantdefs.TAG_EVENT:

                    tagsclass.Do_tag_self(c, (tag_t)the_event.arg4);

                    if (the_event.arg3 == phantdefs.T_MUTE)
                    {

                        tagsclass.Do_tag_muted(c, (tag_t)the_event.arg4);
                    }

                    /* delete the event */
                    the_event.arg4 = null; //free(the_event.arg4);

                    the_event = null; //free((void*) the_event);

                    break;

                /* remove a prefix or suffix tag from the game */
                case phantdefs.UNTAG_EVENT:

                    tagsclass.Do_remove_prefix_suffix(c);

                    the_event = null; //free((void*) the_event);

                    break;

                /* chat messages go straight to the player */
                case phantdefs.CHAT_EVENT:

                    //Debug.LogError("Chat debug: collecting chat event: " + the_event.arg4.ToString().Replace('\0', '$'));

                    /* if this message is from another channel */
                    if (the_event.arg2 > 0)
                    {


                        string_buffer = CFUNCTIONS.sprintfSinglestring("(%.0lf) - ", the_event.arg2);

                        /* send the message */
                        socketclass.Do_send_int(c, phantdefs.CHAT_PACKET);

                        socketclass.Do_send_string(c, string_buffer);

                        socketclass.Do_send_string(c, (string)the_event.arg4);

                        socketclass.Do_send_buffer(c);

                    }
                    /* unless it's an announcement and we're muted */
                    /* or if we're suspended and this isn't from a wizard or himself */
                    else if (c.hearBroadcasts || (the_event.arg1 == 0 ? true : false))
                    {

                        /* send it marked as such */
                        socketclass.Do_send_int(c, phantdefs.CHAT_PACKET);

                        socketclass.Do_send_string(c, (string)the_event.arg4);

                        socketclass.Do_send_buffer(c);
                    }

                    /* delete the string */
                    the_event.arg4 = null; //free(the_event.arg4);

                    /* delete the event */
                    the_event = null; //free((void*) the_event);

                    /* nothing more to do here */
                    break;

                /* bad player, no cookie */
                case phantdefs.REPRIMAND_EVENT:


                    commandsclass.Do_reprimand(c, the_event);
                    break;

                case phantdefs.REQUEST_DETAIL_EVENT:

                    /* create an detail record on arg4 */
                    the_event.arg4 = infoclass.Do_create_detail(c);

                    /* take the event and return it with the information */
                    the_event.type = phantdefs.CONNECTION_DETAIL_EVENT;
                    the_event.to = the_event.from;
                    the_event.from = c.game;

                    /* send off the information */
                    Do_send_event(the_event);

                    /* all done here */
                    break;

                case phantdefs.CONNECTION_DETAIL_EVENT:


                    infoclass.Do_detail_connection(c, (detail_t)the_event.arg4);

                    socketclass.Do_send_buffer(c);

                    /* delete the event */
                    the_event.arg4 = null; //free((void*) the_event.arg4);

                    the_event = null; //free((void*) the_event);

                    /* that's all folkes */
                    break;

                case phantdefs.REQUEST_RECORD_EVENT:

                    /* respond only if we're still in the game */
                    if (c.game.description == null)
                    {
                        break;
                    }

                    /* create an examine record on arg4 */
                    the_event.arg4 = infoclass.Do_create_examine(c, the_event.from);

                    /* take the event and return it with the information */
                    the_event.type = phantdefs.PLAYER_RECORD_EVENT;
                    the_event.to = the_event.from;
                    the_event.from = c.game;

                    /* send off the information */
                    Do_send_event(the_event);

                    /* all done here */
                    break;

                case phantdefs.PLAYER_RECORD_EVENT:


                    infoclass.Do_examine_character(c, (examine_t)the_event.arg4);

                    socketclass.Do_send_buffer(c);

                    /* delete the event */
                    the_event.arg4 = null; //free((void*) the_event.arg4);

                    the_event = null; //free((void*) the_event);

                    /* that's all folkes */
                    break;

                case phantdefs.ADD_PLAYER_EVENT:

                    /* send the packet */
                    ioclass.Do_add_player(c, (player_spec_t)the_event.arg4);

                    socketclass.Do_send_buffer(c);

                    /* delete the event */
                    the_event.arg4 = null; //free((void*) the_event.arg4);

                    the_event = null; //free((void*) the_event);

                    /* Nothing else to do */
                    break;

                case phantdefs.REMOVE_PLAYER_EVENT:

                    /* send the packet */
                    ioclass.Do_remove_player(c, ((player_spec_t)the_event.arg4).name);

                    socketclass.Do_send_buffer(c);

                    /* delete the event */
                    the_event.arg4 = null; //free((void*) the_event.arg4);

                    the_event = null; //free((void*) the_event);


                    /* Nothing else to do */
                    break;

                case phantdefs.CHANGE_PLAYER_EVENT:

                    /* send the packet */
                    ioclass.Do_add_player(c, (player_spec_t)the_event.arg4);

                    ioclass.Do_remove_player(c, ((player_spec_t)the_event.arg4).name);

                    socketclass.Do_send_buffer(c);

                    /* delete the event */
                    the_event.arg4 = null; //free((void*) the_event.arg4);

                    the_event = null; //free((void*) the_event);

                    /* Nothing else to do */
                    break;

                /* remove a player from suspended status */
                case phantdefs.UNSUSPEND_EVENT:

                    /* if this character is suspended */
                    if (c.suspended)
                    {

                        string_buffer = CFUNCTIONS.sprintfSinglestring("You have been released by %s.\n",
                        the_event.arg4);


                        ioclass.Do_send_line(c, string_buffer);

                        /* unsuspend the player */
                        c.suspended = false;
                    }

                    /* delete the event */
                    the_event.arg4 = null; //free(the_event.arg4);

                    the_event = null; //free((void*) the_event);

                    /* nothing more to do here */
                    break;

                case phantdefs.NULL_EVENT:


                    error_msg = CFUNCTIONS.sprintfSinglestring("[%s] attempted to file a null event.\n",
                        c.connection_id);


                    fileclass.Do_log_error(error_msg);

                    event_ptr = null; // free((void*) event_ptr);

                    break;

                /* all other events get queued for the future */
                default:

                    /* getting some impossible events - checking here */
                    if (the_event.type > phantdefs.LAST_EVENT || the_event.type <= phantdefs.NULL_EVENT)
                    {


                        error_msg = CFUNCTIONS.sprintfSinglestring(
                                    "[%s] bad event number %hd in eventclass.Do_file_event.\n",
                            c.connection_id, the_event.type);


                        fileclass.Do_log_error(error_msg);

                        event_ptr = null; //free((void*) event_ptr);

                        break;
                    }

                    /* point to the local event queue */
                    event_ptr_ptr = c.events;

                    /* find the place to put the event in */
                    while (event_ptr_ptr != null && (event_ptr_ptr).type <=
                        the_event.type)
                    {

                        event_ptr_ptr = (event_ptr_ptr).next_event;
                    }
                    
                    /* put the event in that location */
                    the_event.next_event = event_ptr_ptr;
                    event_ptr_ptr = the_event;
                    
                    if (c.events == null) //added for unity; allow for empty queue
                        c.events = event_ptr_ptr;

                    break;
            }
            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: eventclass.Do_file_event_first(struct client_t *c, struct event_t *event_ptr)
        /
        / FUNCTION: Send an event from a client
        /
        / AUTHOR: Brian Kelly, 01/21/01
        /
        / ARGUMENTS:
        /       client_t *c - the pointer to the main client strcture
        /       event_t *event - the event to be placed
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

        internal void Do_file_event_first(client_t c, event_t the_event)
        {
            event_t event_ptr = new event_t();
            event_t event_ptr_ptr = new event_t();

            /* point to the local event queue */
            event_ptr_ptr = c.events;

            /* find the place to put the event in */
            while (event_ptr_ptr != null && (event_ptr_ptr).type <
                  the_event.type)
            {

                event_ptr_ptr = (event_ptr_ptr).next_event;
            }

            /* put the event in that location */
            the_event.next_event = event_ptr_ptr == null ? null : new event_t(event_ptr_ptr);
            event_ptr_ptr = the_event;

            if (c.events == null) //added for unity; allow for empty queue
                c.events = event_ptr_ptr;

            //Debug.LogError("Event debug: the_event.next_event " + the_event.next_event + " event_ptr_ptr " + event_ptr_ptr + " with type " + event_ptr_ptr.type);

            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_check_events_in(struct client_t *c)
        /
        / FUNCTION: Send an event from a client
        /
        / AUTHOR: Brian Kelly, 4/24/99
        /
        / ARGUMENTS:
        /       client_t *c - the pointer to the main client strcture
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

        internal void Do_check_events_in(client_t c)
        {
            event_t event_ptr = new event_t();
            event_t event_ptr_two = new event_t();
            player_spec_t spec_ptr;
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];

            /* lock the events_in queue */
            miscclass.Do_lock_mutex(c.game.events_in_lock);

            /* copy over all the events */
            event_ptr = c.game.events_in;
            c.game.events_in = null;

            /* unlock the events_in queue */
            miscclass.Do_unlock_mutex(c.game.events_in_lock);

            /* run though every event */
            while (event_ptr != null)
            {

                /* save the next event since this one may end up deleted */
                event_ptr_two = event_ptr.next_event;

                if (event_ptr.type <= phantdefs.NULL_EVENT || event_ptr.type > phantdefs.LAST_EVENT)
                {


                    error_msg = CFUNCTIONS.sprintfSinglestring(
                        "[%s] bad event number %hd in Do_check_events_in.\n",
                        c.connection_id, event_ptr.type);


                    fileclass.Do_log_error(error_msg);

                    event_ptr = null; // free((void*) event_ptr);
                }
                else
                {


                    Do_file_event(c, event_ptr);
                }

                /* move to the next event */
                event_ptr = event_ptr_two;
            }
        }


        /************************************************************************
        /
        / FUNCTION NAME: eventclass.Do_handle_event(struct client_t *c, struct event_t *the_event)
        /
        / FUNCTION: Send an event from a client
        /
        / AUTHOR: Brian Kelly, 4/24/99
        /
        / ARGUMENTS:
        /       client_t *c - the pointer to the main client strcture
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

        internal void Do_handle_event(client_t c, event_t the_event)
        {
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            double dtemp;

            error_msg = CFUNCTIONS.sprintfSinglestring("%s: event %hd (1 = %.2lf, 2 = %.2lf, 3 = %ld)\n",
                c.player.lcname, the_event.type, the_event.arg1, the_event.arg2,
                the_event.arg3);

            fileclass.Do_log(pathnames.EVENT_LOG, error_msg);

            //Debug.LogError("Event debug: handling event type " + the_event.type);
            switch (the_event.type)
            {

                /* player died */
                case phantdefs.DEATH_EVENT:


                    commandsclass.Do_death(c, the_event);
                    break;

                case phantdefs.IT_COMBAT_EVENT:


                    itcombatclass.Do_it_combat(c, the_event, 1);
                    break;

                case phantdefs.EXPERIENCE_EVENT:


                    statsclass.Do_experience(c, the_event.arg1, 0);
                    break;

                case phantdefs.SUSPEND_EVENT:


                    ioclass.Do_suspend(c, the_event);
                    break;

                case phantdefs.CANTRIP_EVENT:


                    commandsclass.Do_cantrip(c);
                    break;

                case phantdefs.MODERATE_EVENT:


                    commandsclass.Do_moderate(c);
                    break;

                case phantdefs.ADMINISTRATE_EVENT:


                    commandsclass.Do_administrate(c);
                    break;

                case phantdefs.VALAR_EVENT:


                    commandsclass.Do_valar(c);
                    break;

                case phantdefs.KING_EVENT:


                    commandsclass.Do_king(c);
                    break;

                case phantdefs.STEWARD_EVENT:


                    commandsclass.Do_steward(c);
                    break;

                case phantdefs.DETHRONE_EVENT:


                    commandsclass.Do_dethrone(c);
                    break;

                case phantdefs.RELOCATE_EVENT:


                    commandsclass.Do_relocate(c, the_event);
                    break;

                case phantdefs.TRANSPORT_EVENT:


                    commandsclass.Do_transport(c, the_event);
                    break;

                case phantdefs.CURSE_EVENT:


                    commandsclass.Do_curse(c, the_event);
                    break;

                case phantdefs.SLAP_EVENT:


                    commandsclass.Do_slap(c, the_event);
                    break;

                case phantdefs.REPRIMAND_EVENT:


                    commandsclass.Do_reprimand(c, the_event);
                    break;

                case phantdefs.BLIND_EVENT:


                    commandsclass.Do_blind(c, the_event);
                    break;

                case phantdefs.DEGENERATE_EVENT:


                    commandsclass.Do_caused_degenerate(c, the_event);
                    break;

                case phantdefs.HELP_EVENT:


                    commandsclass.Do_help(c);
                    break;

                case phantdefs.BESTOW_EVENT:


                    commandsclass.Do_bestow(c, the_event);
                    break;

                case phantdefs.SUMMON_EVENT:


                    commandsclass.Do_summon(c, the_event);
                    break;

                case phantdefs.BLESS_EVENT:


                    commandsclass.Do_bless(c, the_event);
                    break;

                case phantdefs.HEAL_EVENT:


                    commandsclass.Do_heal(c, the_event);
                    break;

                case phantdefs.STRONG_NF_EVENT:


                    commandsclass.Do_strong_nf(c, the_event);
                    break;

                case phantdefs.SAVE_EVENT:

                    c.run_level = (short)phantdefs.SAVE_AND_CONTINUE;
                    break;

                case phantdefs.SEX_CHANGE_EVENT:

                    if (c.player.gender == (phantdefs.MALE != 0 ? true : false))
                    {
                        c.player.gender = (phantdefs.FEMALE == 1 ? true : false);
                    }
                    else
                    {
                        c.player.gender = (phantdefs.MALE == 0 ? true : false);
                    }
                    ioclass.Do_send_line(c, "You feel an odd change come over you...\n");

                    break;

                case phantdefs.MOVE_EVENT:

                    miscclass.Do_move(c, the_event);
                    break;

                case phantdefs.EXAMINE_EVENT:


                    commandsclass.Do_examine(c, the_event);
                    break;

                case phantdefs.DECREE_EVENT:


                    commandsclass.Do_decree(c);
                    break;

                case phantdefs.ENACT_EVENT:


                    commandsclass.Do_enact(c);
                    break;

                case phantdefs.KNIGHT_EVENT:


                    commandsclass.Do_knight(c, the_event);
                    break;

                case phantdefs.LIST_PLAYER_EVENT:


                    infoclass.Do_list_characters(c);
                    break;

                case phantdefs.CLOAK_EVENT:


                    statsclass.Do_cloak(c, (!c.player.cloaked ? 1 : 0), 0);
                    break;

                case phantdefs.TELEPORT_EVENT:


                    commandsclass.Do_teleport(c, the_event);
                    break;

                case phantdefs.INTERVENE_EVENT:


                    commandsclass.Do_intervene(c);
                    break;

                case phantdefs.COMMAND_EVENT:


                    commandsclass.Do_command(c);
                    break;

                case phantdefs.REST_EVENT:


                    commandsclass.Do_rest(c);
                    break;

                case phantdefs.INFORMATION_EVENT:


                    miscclass.Do_information(c);
                    break;

                case phantdefs.ENERGY_VOID_EVENT:


                    commandsclass.Do_energy_void(c);
                    break;

                case phantdefs.MONSTER_EVENT:


                    fightclass.Do_monster(c, the_event);
                    break;

                case phantdefs.MEDIC_EVENT:


                    miscclass.Do_medic(c);
                    break;

                case phantdefs.GURU_EVENT:


                    miscclass.Do_guru(c);
                    break;

                case phantdefs.PLAGUE_EVENT:


                    miscclass.Do_plague(c, the_event);
                    break;

                case phantdefs.VILLAGE_EVENT:

                    if ((c.player.circle < 26) || (c.player.circle > 29))
                    {

                        miscclass.Do_village(c);
                    }
                    break;

                case phantdefs.TRADING_EVENT:


                    commandsclass.Do_trading_post(c);
                    break;

                case phantdefs.TREASURE_EVENT:


                    treasureclass.Do_treasure(c, the_event);
                    break;

                case phantdefs.TROVE_EVENT:


                    treasureclass.Do_treasure_trove(c);
                    break;

                case phantdefs.TAX_EVENT:


                    miscclass.Do_tax(c);
                    break;

                case phantdefs.CORPSE_EVENT:


                    treasureclass.Do_corpse(c, the_event);
                    break;

                case phantdefs.GRAIL_EVENT:


                    commandsclass.Do_holy_grail(c, the_event);
                    break;

                case phantdefs.NATINC_EVENT:

                    restoreclass.Do_natincreceive(c, the_event);
                    break;

                case phantdefs.EQINC_EVENT:

                    restoreclass.Do_eqincreceive(c, the_event);
                    break;

                case phantdefs.CINC_EVENT:

                    restoreclass.Do_cincreceive(c, the_event);
                    break;

                case phantdefs.AGING_EVENT:

                    restoreclass.Do_ageincreceive(c, the_event);
                    break;

                case phantdefs.ITEMINC_EVENT:

                    restoreclass.Do_itemincreceive(c, the_event);
                    break;


                default:

                    error_msg = CFUNCTIONS.sprintfSinglestring("[%s] bad event number %hd in Do_handle_event.\n",
                            c.connection_id, the_event.type);

                    fileclass.Do_log_error(error_msg);
                    break;
            }

            the_event = null; //free((void*) the_event);
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_orphan_events(struct client_t *c)
        /
        / FUNCTION: Send an event from a client
        /
        / AUTHOR: Brian Kelly, 8/25/99
        /
        / ARGUMENTS:
        /       client_t *c - the pointer to the main client strcture
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

        internal void Do_orphan_events(client_t c)
        {
            event_t event_ptr = new event_t();
            event_t event_ptr_ptr;
            realm_object_t object_ptr = new realm_object_t();

            /* handle any events in the queue */
            event_ptr_ptr = c.events;
            while (event_ptr_ptr != null)
            {

                /* set the event_ptr to the event pointed to */
                event_ptr = event_ptr_ptr;

                /* if the event is a realm object */
                if (event_ptr.type > phantdefs.REALM_MARKER)
                {

                    /* create a new realm object */
                    object_ptr = new realm_object_t();// (struct realm_object_t *) Do_malloc(phantdefs.SZ_REALM_OBJECT);
                    object_ptr.x = c.player.x;
                    object_ptr.y = c.player.y;

                    /* if we're dealing with a corpse */
                    if (event_ptr.type == phantdefs.CORPSE_EVENT)
                    {

                        /* identify it as such */
                        object_ptr.type = (short)phantdefs.CORPSE;

                        /* point the object to the player_t */
                        object_ptr.arg1 = (player_t)event_ptr.arg4;

                    }
                    else if (event_ptr.type == phantdefs.GRAIL_EVENT)
                    {

                        /* identify it as a grail object */
                        event_ptr.type = (short)phantdefs.HOLY_GRAIL;
                    }
                    else if (event_ptr.type == phantdefs.TROVE_EVENT)
                    {

                        /* identify it as a grail object */
                        event_ptr.type = (short)phantdefs.TREASURE_TROVE;
                    }

                    /* put the object in the list of realm objects */
                    object_ptr.next_object = c.realm.objects;
                    c.realm.objects = object_ptr;
                }

                if (event_ptr.type == phantdefs.IT_COMBAT_EVENT)
                {

                    itcombatclass.Do_it_combat(c, event_ptr, 0);
                }

                if (event_ptr.type > phantdefs.DESTROY_MARKER || event_ptr.type ==
                phantdefs.IT_COMBAT_EVENT)
                {

                    /* delete normal and realm object events */
                    event_ptr_ptr = event_ptr.next_event;
                    event_ptr = null;//free((void*) event_ptr);
                }
                else
                {
                    /* move to the next event */
                    event_ptr_ptr = event_ptr.next_event;
                }
            }
        }


        /************************************************************************
        /
        / FUNCTION NAME: int eventclass.Do_send_character_event(struct client_t *c, struct event_t *theEvent, char *characterName)
        /
        / FUNCTION: Send an event from a client
        /
        / AUTHOR: Brian Kelly, 01/03/01
        /
        / ARGUMENTS:
        /       client_t *c - the pointer to the main client strcture
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

        internal int Do_send_character_event(client_t c, event_t theEvent, string characterName)
        {

            game_t game_ptr = new game_t();

            if (theEvent.type == phantdefs.UNTAG_EVENT)

                fileclass.Do_log(pathnames.DEBUG_LOG, "eventclass.Do_send_character_event reached with untag event\n");

            /* lock the realm */
            miscclass.Do_lock_mutex(c.realm.realm_lock);

            /* run though all the games and check the names */
            game_ptr = c.realm.games;
            while (game_ptr != null)
            {

                /* check for a name match */
                if (game_ptr.description != null &&
                        !CFUNCTIONS.strcmp(characterName, game_ptr.description.name))
                {

                    theEvent.to = game_ptr;
                    Do_send_event(theEvent);

                    miscclass.Do_unlock_mutex(c.realm.realm_lock);
                    return 1;
                }

                game_ptr = game_ptr.next_game;
            }

            /* the character was not found */
            miscclass.Do_unlock_mutex(c.realm.realm_lock);
            return 0;
        }


        /************************************************************************
        /
        / FUNCTION NAME: int Do_check_encountered(struct client_t *c)
        /
        / FUNCTION: Send an event from a client
        /
        / AUTHOR: Brian Kelly, 05/08/01
        /
        / ARGUMENTS:
        /       client_t *c - the pointer to the main client strcture
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

        internal int Do_check_encountered(client_t c)
        {
            event_t event_ptr = new event_t();

            /* Realm must be locked when this function is called! */

            /*      Don't check events in!  If examine request, we'll deadlock
                Do_check_events_in(c);
            */
            event_ptr = c.events;

            while (event_ptr != null)
            {
                if (event_ptr.type == phantdefs.IT_COMBAT_EVENT)
                {
                    return 1;
                }
                event_ptr = event_ptr.next_event;
            }

            /* now check the event-in queue */
            miscclass.Do_lock_mutex(c.game.events_in_lock);
            event_ptr = c.game.events_in;

            while (event_ptr != null)
            {
                if (event_ptr.type == phantdefs.IT_COMBAT_EVENT)
                {
                    miscclass.Do_unlock_mutex(c.game.events_in_lock);
                    return 1;
                }
                event_ptr = event_ptr.next_event;
            }

            miscclass.Do_unlock_mutex(c.game.events_in_lock);
            return 0;
        }

    }
}
