using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;
using System.IO;

namespace phantasiaclasses
{
    public class misc //: MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        static misc Instance;
        private misc()
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
        public static misc GetInstance()
        {
            misc instance = null;
            if (Instance != null)
            {
                instance = Instance;
            }
            else
            {
                instance = new misc();
            }
            return instance;
        }

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

        /*
     * misc.c       Multiple small utility procedures
     */

        //extern randData; //handled in CFUNCTIONS
        //extern server_hook; //handled in CFUNCTIONS

        /***************************************************************************
        / FUNCTION NAME: Do_init_mutex(pthread_mutex_t *the_mutex)
        /
        / FUNCTION: Initializes the passed mutex
        /
        / AUTHOR:  Brian Kelly, 4/19/99
        /
        / ARGUMENTS: 
        /	pthread_mutex_t the_mutex - The mutex to be initialized
        /
        / RETURN VALUE: none
        /
        / DESCRIPTION:
        /       Procedure handles errors from mutex operations.
        /
        ****************************************************************************/

        internal void Do_init_mutex(CLibPThread.pthread_mutex_t the_mutex) 
        {
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            int error;

            /* initialize the passed mutex */
            error = CLibPThread.pthread_mutex_init(the_mutex, null);
            if (error != 0)
            {

                error_msg = CFUNCTIONS.sprintfSinglestring("[0.0.0.0:?] pthread_mutex_init failed with %d return code in Do_init_mutex.\n", error);

                fileclass.Do_log_error(error_msg);
                CFUNCTIONS.exit(phantdefs.MUTEX_INIT_ERROR);
            }
        }


        /***************************************************************************
        / FUNCTION NAME: Do_destroy_mutex(pthread_mutex_t *the_mutex)
        /
        / FUNCTION: Initializes the passed mutex
        /
        / AUTHOR:  Brian Kelly, 4/19/99
        /
        / ARGUMENTS: 
        /	pthread_mutex_t the_mutex - The mutex to be destroyed
        /
        / RETURN VALUE: none
        /
        / DESCRIPTION:
        /       Procedure handles errors from mutex operations.
        /
        ****************************************************************************/

        internal void Do_destroy_mutex(CLibPThread.pthread_mutex_t the_mutex) 
        {
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            int error;

            /* destroy the passed mutex */
            error = CLibPThread.pthread_mutex_destroy(the_mutex);
            if (error != 0)
            {

                error_msg = CFUNCTIONS.sprintfSinglestring("[0.0.0.0:?] pthread_mutex_destroy failed with %d return code in Do_destroy_mutex.\n", error);

                fileclass.Do_log_error(error_msg);
                CFUNCTIONS.exit(phantdefs.MUTEX_DESTROY_ERROR);
            }
        }


        /***************************************************************************
        / FUNCTION NAME: Do_lock_mutex(pthread_mutex_t *the_mutex)
        /
        / FUNCTION: Locks the passed mutex
        /
        / AUTHOR:  Brian Kelly, 4/19/99
        /
        / ARGUMENTS: 
        /	pthread_mutex_t the_mutex - The mutex to be locked
        /
        / RETURN VALUE: none
        /
        / DESCRIPTION:
        /       Procedure handles errors from mutex operations.
        /
        ****************************************************************************/

        internal void Do_lock_mutex(CLibPThread.pthread_mutex_t the_mutex) 
        {
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            int error;
            
            if (UnityGameController.MUTEX_DEBUG)
            {
                error_msg = CFUNCTIONS.sprintfSinglestring("Request mutex %x lock.\n", the_mutex);
                fileclass.Do_log(pathnames.DEBUG_LOG, error_msg);
            }

            /* lock the passed mutex */
            error = CLibPThread.pthread_mutex_lock(the_mutex);
            if (error != 0)
            {

                error_msg = CFUNCTIONS.sprintfSinglestring("[?:?] pthread_mutex_lock failed with %d return code in Do_lock_mutex.\n", error);

                fileclass.Do_log_error(error_msg);
                CFUNCTIONS.exit(phantdefs.MUTEX_LOCK_ERROR);
            }
            if (UnityGameController.MUTEX_DEBUG)
            {
                error_msg = CFUNCTIONS.sprintfSinglestring("Mutex %x locked.\n", the_mutex);
                fileclass.Do_log(pathnames.DEBUG_LOG, error_msg);
            }
        }


        /***************************************************************************
        / FUNCTION NAME: Do_unlock_mutex(pthread_mutex_t *the_mutex)
        /
        / FUNCTION: Unlocks the passed mutex
        /
        / AUTHOR:  Brian Kelly, 4/19/99
        /
        / ARGUMENTS: 
        /	pthread_mutex_t the_mutex - The mutex to be unlocked
        /
        / RETURN VALUE: none
        /
        / DESCRIPTION:
        /       Procedure handles errors from mutex operations.
        /
        ****************************************************************************/

        internal void Do_unlock_mutex(CLibPThread.pthread_mutex_t the_mutex)
        {
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            int error;

            /* unlock the passed mutex */
            error = CLibPThread.pthread_mutex_unlock(the_mutex);
            if (error != 0)
            {

                error_msg = CFUNCTIONS.sprintfSinglestring("[?:?] pthread_mutex_unlock failed with a %d return code in Do_unlock_mutex.\n", error);

                fileclass.Do_log_error(error_msg);
                CFUNCTIONS.exit(phantdefs.MUTEX_UNLOCK_ERROR);
            }
            if (UnityGameController.MUTEX_DEBUG)
            {
                error_msg = CFUNCTIONS.sprintfSinglestring("Mutex %x unlocked.\n", the_mutex);
                fileclass.Do_log(pathnames.DEBUG_LOG, error_msg);
            }
        }


        /***************************************************************************
        / FUNCTION NAME: void *Do_malloc(struct size_t the_size)
        /
        / FUNCTION: Returns the pointer to a new memory space
        /
        / AUTHOR:  Brian Kelly, 4/20/99
        /
        / ARGUMENTS:
        /	struct size_t the_size - The size of the memory space to allocate
        /
        / RETURN VALUE: 
        /	void * - pointer to the new memory space
        /
        / DESCRIPTION:
        /       Procedure handles errors for malloc calls.
        /
        ****************************************************************************/

        internal T Do_malloc<T>(int the_size)
        {
            //unncessary in c#/unity
            Debug.LogError("Attempted to call Do_malloc instead of initialising");
            return default(T);

            //T void_ptr = default(T);
            //string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];

            ///* create the new memory space */
            ////void_ptr = malloc(the_size);
            //if (void_ptr == null)
            //{

            //    error_msg = CFUNCTIONS.sprintfSinglestring("[?:?] malloc failed on size %d in Do_malloc.\n",
            //        the_size);

            //    fileclass.Do_log_error(error_msg);
            //    CFUNCTIONS.exit(phantdefs.MALLOC_ERROR);
            //}
            //return void_ptr;
        }


        /************************************************************************
        /
        / FUNCTION NAME: int Do_random(void)
        /
        / FUNCTION: return a random floating point number from 0.0 < 1.0
        /
        / AUTHOR: E. A. Estes, 2/7/86
        /	  Brian Kelly, 4/20/99
        /
        / ARGUMENTS: none
        /
        / RETURN VALUE: none
        /
        / MODULES CALLED: random()
        /
        / GLOBAL INPUTS: none
        /
        / GLOBAL OUTPUTS: none
        /
        / DESCRIPTION:
        /       Convert random integer from library routine into a floating
        /       point number, and divide by the largest possible random number.
        /       We mask large integers with 32767 to handle sites that return
        /       31 bit random integers.
        /
        *************************************************************************/

        int Do_random()
        {
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            int error = -1, result = -1;

            if (CFUNCTIONS.random_r(CFUNCTIONS.randData, ref result) != 0)
            {


                error_msg = CFUNCTIONS.sprintfSinglestring(
                    "[?:?] random_r failed with error code of %d in Do_random.\n",
                    error);


                fileclass.Do_log_error(error_msg);
            }

            /* return an unsigned 4 byte number */
            return result;
        }

        /************************************************************************
        /
        / FUNCTION NAME: Do_name_location(struct client_t *c)
        /
        / FUNCTION: return a formatted description of location
        /
        / AUTHOR: E. A. Estes, 12/4/85
        /
        / ARGUMENTS:
        /       struct player playerp - pointer to player structure
        /       bool shortflag - set if short form is desired
        /
        / RETURN VALUE: pointer to string containing result
        /
        / MODULES CALLED: Mathf.Abs(), cfunctions.floor(), CFUNCTIONS.sprintf(ref ), distance()
        /
        / DESCRIPTION:
        /       Look at coordinates and return an appropriately formatted
        /       string.
        /
        *************************************************************************/

        internal void Do_name_location(client_t c)
        {
            int quadrant;       /* quandrant of grid */
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            string[][] nametable = new string[4][]    /* names of places */
                    {
                    new string[4] { "Anorien",      "Ithilien",     "Rohan",        "Lorien" },
                    new string[4] { "Gondor",       "Mordor",       "Dunland",      "Rovanion" },
                    new string[4] { "South Gondor", "Khand",        "Eriador",      "The Iron Hills" },
                    new string[4] { "Far Harad",    "Near Harad",   "The Northern Waste", "Rhun" }
                    };

            if (c.player.location == phantdefs.PL_REALM)
            {

                if (c.player.beyond)
                {
                    CFUNCTIONS.strcpy(ref c.player.area, "The Point of No Return");
                }
                else if (c.player.circle >= 400.0)
                    CFUNCTIONS.strcpy(ref c.player.area, "The Ashen Mountains");
                else if (c.player.circle >= 100)
                    CFUNCTIONS.strcpy(ref c.player.area, "Kennaquahir");
                else if (c.player.circle >= 36)
                    CFUNCTIONS.strcpy(ref c.player.area, "Morannon");
                else if ((c.player.circle == 27) || (c.player.circle == 28))
                    CFUNCTIONS.strcpy(ref c.player.area, "The Cracks of Doom");
                else if ((c.player.circle > 24) && (c.player.circle < 31))
                    CFUNCTIONS.strcpy(ref c.player.area, "The Plateau of Gorgoroth");
                else if (c.player.circle >= 20)
                    CFUNCTIONS.strcpy(ref c.player.area, "The Dead Marshes");
                else if (c.player.circle >= 10)
                    CFUNCTIONS.strcpy(ref c.player.area, "The Outer Waste");
                else if (c.player.circle >= 5)
                    CFUNCTIONS.strcpy(ref c.player.area, "The Moors Adventurous");
                else
                {

                    /* this expression is split to prevent compiler loop with some compilers */
                    quadrant = ((c.player.x > 0.0) ? 1 : 0);
                    quadrant += ((c.player.y >= 0.0) ? 2 : 0);

                    CFUNCTIONS.strcpy(ref c.player.area,
                    nametable[((int)c.player.circle) - 1][quadrant]);
                }
            }
            else if (c.player.location == phantdefs.PL_THRONE)
            {
                CFUNCTIONS.strcpy(ref c.player.area, "The Lord's Chamber");
            }
            else if (c.player.location == phantdefs.PL_EDGE)
            {
                CFUNCTIONS.strcpy(ref c.player.area, "Edge Of The Realm");
            }
            else if (c.player.location == phantdefs.PL_VALHALLA)
            {
                CFUNCTIONS.strcpy(ref c.player.area, "Valhalla");
            }
            else if (c.player.location == phantdefs.PL_PURGATORY)
            {
                CFUNCTIONS.strcpy(ref c.player.area, "Purgatory");
            }
            /* no other places to be */
            else
            {
                CFUNCTIONS.strcpy(ref c.player.area, "State Of Insanity");

                error_msg = CFUNCTIONS.sprintfSinglestring(
                "[%s] Bad c.player.area of %hd in Do_name_location.\n",
                c.connection_id, c.player.area);
            }
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_truncstring()
        /
        / FUNCTION: truncate trailing blanks off a string
        /
        / AUTHOR: E. A. Estes, 12/4/85
        /	  Brian Kelly, 5/4/99
        /
        / ARGUMENTS:
        /       char *string - pointer to null terminated string
        /
        / RETURN VALUE: none
        /
        / MODULES CALLED: cfunctions.strlen()
        /
        / DESCRIPTION:
        /       Put nul characters in place of spaces at the end of the string.
        /
        *************************************************************************/

        internal void Do_truncstring(string cstring)
        {
            try
            {
                DateTime dateonly = Convert.ToDateTime(cstring);
            }
            catch
            {
                //Debug.Log("phantasia.misc.Do_truncstring: " + cstring);
            }

            int length;         /* length of string */

            length = CFUNCTIONS.strlen(cstring);

            while (length != 0 && !CFUNCTIONS.isgraph(cstring[--length]))
            {
                cstring = cstring.Substring(0, length); //cstring[length] = '\0'; //length - 1 unnecessary due to -- above


            }
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_distance()
        /
        / FUNCTION: calculate distance between two points
        /
        / AUTHOR: E. A. Estes, 12/4/85
        /         Brian Kelly, 6/20/99
        /
        / ARGUMENTS:
        /       double x1, y1 - x, y coordinates of first point
        /       double x2, y2 - x, y coordinates of second point
        /
        / RETURN VALUE: distance between the two points
        /
        / MODULES CALLED: cfunctions.sqrt()
        /
        / GLOBAL INPUTS: none
        /
        / GLOBAL OUTPUTS: none
        /
        / DESCRIPTION:
        /       This function is provided because someone's hypot() library function
        /       fails if x1 == x2 && y1 == y2.
        /
        *************************************************************************/

        internal void Do_distance(double x1, double x2, double y1, double y2, ref double answer)
        {
            double deltax, deltay;

            deltax = x1 - x2;
            deltay = y1 - y2;

            answer = CFUNCTIONS.sqrt(deltax * deltax + deltay * deltay);
            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_move(struct client_t c, float new_x, float new_y, int operation)
        /
        / FUNCTION: truncate trailing blanks off a string
        /
        / AUTHOR: E. A. Estes, 12/4/85
        /	  Brian Kelly, 5/8/99
        /
        / ARGUMENTS:
        /       char *string - pointer to null terminated string
        /
        / RETURN VALUE: none
        /
        / MODULES CALLED: cfunctions.strlen()
        /
        / DESCRIPTION:
        /       Put nul characters in place of spaces at the end of the string.
        /
        *************************************************************************/

        internal void Do_move(client_t c, event_t the_event)
        {
            event_t event_ptr = new event_t(), event_ptr_two = new event_t(), event_ptr_ptr = new event_t();
            realm_object_t object_ptr = new realm_object_t(), object_ptr_ptr = new realm_object_t();
            game_t game_ptr = new game_t();
            double dtemp, x = 0, y = 0;
            double distance;
            bool pause = false;
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            long answer = -1;
            short oldLocation;

            /* determine the destination coordinates */
            switch (the_event.arg3)
            {

                case phantdefs.A_NEAR:
                    the_event.arg1 = c.player.x;
                    the_event.arg2 = c.player.y;
                    dtemp = (double)Do_maxmove(c);

                    Do_move_close(ref the_event.arg1, ref the_event.arg2, dtemp);
                    break;

                case phantdefs.A_FAR:     /* pick random coordinates far */

                    the_event.arg1 = macros.SGN(c.player.x) * (phantdefs.D_CIRCLE + Mathf.Abs((float)c.player.x)) *
                        (2 * macros.RND() + 2);

                    the_event.arg2 = macros.SGN(c.player.y) * (phantdefs.D_CIRCLE + Mathf.Abs((float)c.player.y)) *
                        (2 * macros.RND() + 2);
                    break;

                case phantdefs.A_TRANSPORT: /* send out aways */


                    Do_move_close(ref x, ref y, 2000 * macros.RND());

                    /* use whichever x is larger of old and new */
                    if (Mathf.Abs((float)c.player.x) > Mathf.Abs((float)x))
                    {
                        the_event.arg1 = c.player.x;
                    }
                    else
                    {
                        the_event.arg1 = x;
                    }

                    /* use whichever y is larger of old and new */
                    if (Mathf.Abs((float)c.player.y) > Mathf.Abs((float)y))
                    {
                        the_event.arg2 = c.player.y;
                    }
                    else
                    {
                        the_event.arg2 = y;
                    }

                    break;

                case phantdefs.A_OUST:  /* send out aways more */


                    Do_move_close(ref x, ref y, 50000 * macros.RND());

                    /* use whichever x is larger of old and new */
                    if (Mathf.Abs((float)c.player.x) > Mathf.Abs((float)x))
                    {
                        the_event.arg1 = c.player.x;
                    }
                    else
                    {
                        the_event.arg1 = x;
                    }

                    /* use whichever y is larger of old and new */
                    if (Mathf.Abs((float)c.player.y) > Mathf.Abs((float)y))
                    {
                        the_event.arg2 = c.player.y;
                    }
                    else
                    {
                        the_event.arg2 = y;
                    }

                    break;

                /* send beyond */
                case phantdefs.A_BANISH:

                    if (Mathf.Abs((float)c.player.x) > Mathf.Abs((float)c.player.y))
                    {

                        the_event.arg1 = c.player.x;
                        the_event.arg2 = c.player.y;

                        if (Mathf.Abs((float)c.player.x) < phantdefs.D_BEYOND)
                        {
                            the_event.arg1 = macros.SGN(c.player.x) * phantdefs.D_BEYOND;
                        }

                    }
                    else
                    {

                        the_event.arg1 = c.player.x;
                        the_event.arg2 = c.player.y;

                        if (Mathf.Abs((float)c.player.y) < phantdefs.D_BEYOND)
                        {
                            the_event.arg2 = macros.SGN(c.player.y) * phantdefs.D_BEYOND;
                        }
                    }
                    break;
            }

            /* round the new location down */
            the_event.arg1 = CFUNCTIONS.floor(the_event.arg1);
            the_event.arg2 = CFUNCTIONS.floor(the_event.arg2);

            /* lock the realm */
            Do_lock_mutex(c.realm.realm_lock);

            /* check to make sure there are no it_combat events not received */
            if (eventclass.Do_check_encountered(c) != 0)
            {

                Do_unlock_mutex(c.realm.realm_lock);
                c.stuck = true;
                return;
            }

            /* the move is successful - handle any events in the queue */
            /* only orphan events if the player is leaving the square -
            this closes the king safe send through deliberate itcombat */
            if (the_event.arg1 != c.player.x || the_event.arg2 != c.player.y)
            {

                eventclass.Do_orphan_events(c);
            }

            /* if returning from beyond */
            if (the_event.arg3 != phantdefs.A_FORCED && c.player.beyond &&

                Mathf.Abs((float)the_event.arg1) < phantdefs.D_BEYOND && Mathf.Abs((float)the_event.arg2)
                < phantdefs.D_BEYOND)
            {

                /* cannot move back from point of no return */
                /* pick the largest coordinate to remain unchanged */
                if (Mathf.Abs((float)c.player.x) > Mathf.Abs((float)c.player.y))
                {

                    the_event.arg1 = macros.SGN(c.player.x) * phantdefs.D_BEYOND;
                    /*
                            if (Mathf.Abs((float)c.player.x) < phantdefs.D_BEYOND) {
                            the_event.arg1 = macros.SGN(c.player.x) * phantdefs.D_BEYOND;
                            }
                    */
                }
                else
                {

                    the_event.arg2 = macros.SGN(c.player.y) * phantdefs.D_BEYOND;
                    /*
                            if (Mathf.Abs((float)c.player.y) < phantdefs.D_BEYOND) {
                            the_event.arg2 = macros.SGN(c.player.y) * phantdefs.D_BEYOND;
                            }
                    */
                }
            }

            /* see if the player is beyond */
            c.player.beyond = false;
            oldLocation = c.player.location;

            if (Mathf.Abs((float)the_event.arg1) >= phantdefs.D_BEYOND || Mathf.Abs((float)the_event.arg2)
                >= phantdefs.D_BEYOND)
            {

                c.player.beyond = true;
            }

            /* if moving off the board's edge */
            if (Mathf.Abs((float)the_event.arg1) >= phantdefs.D_EDGE || Mathf.Abs((float)the_event.arg2) >= phantdefs.D_EDGE)
            {

                /* stop a character at the edge */
                /* send over if they move that way again */
                if ((Mathf.Abs((float)the_event.arg1) > phantdefs.D_EDGE || Mathf.Abs((float)the_event.arg2) > phantdefs.D_EDGE)
                && c.player.location == phantdefs.PL_EDGE &&
                        (the_event.arg3 == phantdefs.A_SPECIFIC ||
                         the_event.arg3 == phantdefs.A_TELEPORT))
                {


                    Do_unlock_mutex(c.realm.realm_lock);
                    event_ptr = eventclass.Do_create_event();
                    event_ptr.type = (short)phantdefs.DEATH_EVENT;
                    event_ptr.arg3 = phantdefs.K_FELL_OFF;

                    eventclass.Do_file_event(c, event_ptr);
                    return;
                }

                c.player.location = (short)phantdefs.PL_EDGE;

                if (Mathf.Abs((float)the_event.arg1) > phantdefs.D_EDGE)
                {
                    the_event.arg1 = macros.SGN(the_event.arg1) * phantdefs.D_EDGE;
                }

                if (Mathf.Abs((float)the_event.arg2) > phantdefs.D_EDGE)
                {
                    the_event.arg2 = macros.SGN(the_event.arg2) * phantdefs.D_EDGE;
                }
            }
            else
            {
                c.player.location = (short)phantdefs.PL_REALM;
            }

            /* see if we're in the throne room */
            if (the_event.arg1 == 0 && the_event.arg2 == 0)
            {

                /* if this player is not the king/steward */
                if (c.game != c.realm.king)
                {

                    /* if this player believes himself to be, quit */
                    /* we will have an event pending */
                    if (c.player.special_type == phantdefs.SC_STEWARD || c.player.special_type
                        == phantdefs.SC_KING)
                    {


                        Do_unlock_mutex(c.realm.realm_lock);
                        return;
                    }

                    /* if this character is to be steward */
                    if (c.player.level >= 10 && c.player.level < 200)
                    {

                        /* and if this player has a staff */
                        if (c.player.crowns != 0)
                        {

                            /* and there is no king */
                            if (!c.realm.king_flag)
                            {

                                /* enter the throne room */
                                c.player.location = (short)phantdefs.PL_THRONE;

                                /* if there is a steward currently on the throne */
                                if (c.realm.king != null && c.realm.king.x == 0 &&
                            c.realm.king.y == 0)
                                {

                                    /* do nothing.  If victorious, he'll be crowned in
                                    the it_combat routines */
                                }
                                else
                                {

                                    /* become steward */
                                    eventclass.Do_send_self_event(c, phantdefs.STEWARD_EVENT);
                                }
                            }

                            /* there is a king when we're trying to be steward */
                            else
                            {


                                ioclass.Do_send_line(c,
                            "There is no need for a steward while a king presides!\n");

                                pause = true;


                                Do_move_close(ref the_event.arg1, ref the_event.arg2,
                                    Do_maxmove(c));
                            }
                        }

                        /* if the (10-200) player has no staff */
                        else
                        {


                            ioclass.Do_send_line(c,
                             "You require a staff to enter The Lord's Chamber.\n");

                            pause = true;


                            Do_move_close(ref the_event.arg1, ref the_event.arg2,
                                Do_maxmove(c));
                        }
                    }

                    /* if this character is to be king */
                    else if (c.player.level >= phantdefs.MIN_KING && c.player.level < phantdefs.MAX_KING)
                    {

                        /* and if this player has a crown */
                        if (c.player.crowns != 0)
                        {

                            /* enter the throne room */
                            c.player.location = (short)phantdefs.PL_THRONE;

                            /* if there is a king currently on the throne */
                            if (c.realm.king != null && c.realm.king.x == 0 &&
                        c.realm.king.y == 0)
                            {

                                /* if that character is a king */
                                if (c.realm.king_flag)
                                {

                                    /* do nothing.  If victorious, he'll be crowned
            				in the it_combat routines */
                                }
                                else
                                {

                                    /* make the curent steward virtual */
                                    c.realm.king.virtualvirtual = true;

                                    /* take the throne */
                                    eventclass.Do_send_self_event(c, phantdefs.KING_EVENT);
                                }
                            }
                            else
                            {

                                /* become king */
                                eventclass.Do_send_self_event(c, phantdefs.KING_EVENT);
                            }
                        }

                        /* if the (1000-2000) player has no crown */
                        else
                        {


                            ioclass.Do_send_line(c,
                             "You require a crown to enter The Lord's Chamber.\n");

                            pause = true;


                            Do_move_close(ref the_event.arg1, ref the_event.arg2,
                                Do_maxmove(c));
                        }
                    }

                    /* the character is above the king level */
                    else if (c.player.level >= 2000)
                    {


                        ioclass.Do_send_line(c, "The head page says, 'Get out of here, you greedy bastard.' and throws you out of The Lord's Chamber.\n");

                        pause = true;

                        Do_move_close(ref the_event.arg1, ref the_event.arg2, Do_maxmove(c));
                    }

                    /* the character is of the wrong level */
                    else
                    {


                        ioclass.Do_send_line(c, "The head page stops you and says, 'Characters of your level may not enter The Lord's Chamber.'\n");

                        pause = true;

                        Do_move_close(ref the_event.arg1, ref the_event.arg2, Do_maxmove(c));
                    }
                }
                /* if the player already is king or steward */
                else
                {
                    c.player.location = (short)phantdefs.PL_THRONE;
                }
            }

            /* check for possible combat */
            dtemp = 0;
            game_ptr = c.realm.games;
            while (game_ptr != null)
            {

                if (game_ptr.description != null && !game_ptr.virtualvirtual &&
                    game_ptr.x == the_event.arg1 && game_ptr.y ==
                    the_event.arg2 && game_ptr != c.game)
                {

                    dtemp = 1;
                    break;
                }
                game_ptr = game_ptr.next_game;
            }

            /* if we've encountered someone */
            if (dtemp != 0)
            {

                /* confirm the move if player moved under own power */
                if (event_ptr.arg3 == phantdefs.A_SPECIFIC || event_ptr.arg3 == phantdefs.A_TELEPORT)
                {

                    /* release the realm */
                    Do_unlock_mutex(c.realm.realm_lock);
                    pause = false;


                    ioclass.Do_send_line(c, "The square you're moving into is currently occupied by another player.  Do you still wish to move there?\n");

                    if (ioclass.Do_yes_no(c, ref answer) != phantdefs.S_NORM || answer == 1)
                    {

                        /* abort the move if the player says "no" */
                        ioclass.Do_send_clear(c);
                        c.player.location = oldLocation;
                        return;
                    }

                    ioclass.Do_send_clear(c);

                    /* relock the realm */
                    Do_lock_mutex(c.realm.realm_lock);

                    /* see if someone else has attacked this player */
                    if (eventclass.Do_check_encountered(c) != 0)
                    {

                        Do_unlock_mutex(c.realm.realm_lock);
                        c.stuck = true;
                        return;
                    }

                    /* make sure a player is still there */
                    dtemp = 0;
                    game_ptr = c.realm.games;
                    while (game_ptr != null)
                    {

                        if (game_ptr.description != null && !game_ptr.virtualvirtual &&
                            game_ptr.x == the_event.arg1 && game_ptr.y ==
                            the_event.arg2 && game_ptr != c.game)
                        {

                            dtemp = 1;
                            break;
                        }

                        game_ptr = game_ptr.next_game;
                    }
                }
            }

            /* set public and private variables to the new coordinates */
            if (c.wizard > 2)
            {
                c.player.location = (short)phantdefs.PL_VALHALLA;
            }

            statsclass.Do_location(c, the_event.arg1, the_event.arg2, 0);

            /* move the grail if player teleported close to it */
            /*
                if (the_event.arg3 == phantdefs.A_TELEPORT) {
            	object_ptr = c.realm.objects;

            	while (object_ptr != null) {

            	    if (object_ptr.type == phantdefs.HOLY_GRAIL) {
            		break;
            	    }

            	    object_ptr = object_ptr.next_object;
            	}

            	if (object_ptr == null) {

            	    Do_unlock_mutex(c.realm.realm_lock);

            	    error_msg = CFUNCTIONS.sprintf(ref 
            		    "[%s] No grail found in realm objects in Do_move.\n",
            		    c.connection_id);

            	    fileclass.Do_log_error(error_msg);

            	    return;
            	}

            	Do_distance(c.player.x, object_ptr.x, c.player.y, object_ptr.y,
            		distance);


                    if (distance <= c.player.level / 500.0) {

            	    error_msg = CFUNCTIONS.sprintf(ref 
            		    "Teleporting grail.\n");

            	    fileclass.Do_log_error(error_msg);

                        free((void *)object_ptr);
            	    Do_hide_grail(c.realm, c.player.level);
                    }

                }
            */


            /* look for realm objects if destination has no players */
            if (dtemp == 0)
            {

                /* if a player is cloaked, he/she can't find realm objects */
                if (!c.player.cloaked)
                {

                    /* check for realm objects in the new location */
                    object_ptr_ptr = c.realm.objects;

                    while (object_ptr_ptr != null)
                    {

                        if ((object_ptr_ptr).x == c.player.x && (object_ptr_ptr).y
                        == c.player.y)
                        {

                            object_ptr = object_ptr_ptr;

                            object_ptr_ptr = object_ptr.next_object;

                            event_ptr = eventclass.Do_create_event();

                            switch (object_ptr.type)
                            {

                                case phantdefs.CORPSE:
                                    event_ptr.type = (short)phantdefs.CORPSE_EVENT;

                                    /* the corpse can be cursed */
                                    event_ptr.arg1 = 1;
                                    event_ptr.arg4 = object_ptr.arg1;
                                    break;

                                case phantdefs.HOLY_GRAIL:
                                    event_ptr.type = (short)phantdefs.GRAIL_EVENT;

                                    initclass.Do_hide_grail(c.realm, CFUNCTIONS.floor(c.player.level));
                                    break;

                                case phantdefs.ENERGY_VOID:
                                    event_ptr.type = (short)phantdefs.ENERGY_VOID_EVENT;
                                    break;

                                case phantdefs.TREASURE_TROVE:
                                    event_ptr.type = (short)phantdefs.TROVE_EVENT;

                                    initclass.Do_hide_trove(c.realm);
                                    break;

                                default:


                                    error_msg = CFUNCTIONS.sprintfSinglestring(
                              "[%s] encountered realm object of type %d in Do_move.\n",
                                  c.connection_id, object_ptr.type);


                                    fileclass.Do_log_error(error_msg);
                                    break;
                            }


                            eventclass.Do_file_event(c, event_ptr);


                            object_ptr = null; // free((void*) object_ptr);
                        }
                        else
                        {
                            object_ptr_ptr = ((object_ptr_ptr).next_object);
                        }
                    }
                }


                Do_unlock_mutex(c.realm.realm_lock);

                if (pause)
                {

                    ioclass.Do_more(c);
                    ioclass.Do_send_clear(c);
                }
            }

            /* else we have found a player to fight */
            else
            {

                /* pass this function with realm locked! */
                itcombatclass.Do_setup_it_combat(c, game_ptr);
            }

            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_check_weight(c)
        /
        / FUNCTION: truncate trailing blanks off a string
        /
        / AUTHOR: E. A. Estes, 12/4/85
        /	  Brian Kelly, 5/16/99
        /
        / ARGUMENTS:
        /       char *string - pointer to null terminated string
        /
        / RETURN VALUE: none
        /
        / MODULES CALLED: cfunctions.strlen()
        /
        / DESCRIPTION:
        /       Put nul characters in place of spaces at the end of the string.
        /
        *************************************************************************/

        internal void Do_check_weight(client_t c)
        {
            statsclass.Do_speed(c, c.player.max_quickness, c.player.quicksilver,
                c.battle.speedSpell, 0);
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_medic(struct client_t *c)
        /
        / FUNCTION: do random stuff
        /
        / AUTHOR: E. A. Estes, 3/3/86
        /         Brian Kelly, 6/20/99
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

        internal void Do_medic(client_t c)
        {
            double gold = 0;
            string string_buffer = ""; //[phantdefs.SZ_LINE];
            float ftemp;

            if (c.player.blind)
            {

                ioclass.Do_send_line(c, "You've found a medic!\n");

                if (ioclass.Do_double_dialog(c, gold, "How much will you offer to heal your blindness?\n") != 0)
                {

                    ioclass.Do_send_clear(c);
                    return;
                }

                /* negative gold, or more than available */
                if (gold < 0.0 || gold > c.player.gold)
                {
                    ioclass.Do_send_line(c, "He was not amused, and gave you a disease.\n");
                    statsclass.Do_adjusted_poison(c, 1.0);

                    statsclass.Do_strength(c, c.player.max_strength, c.player.sword, 0, 0);
                }
                else if (gold < c.player.level * 10)
                {
                    ioclass.Do_send_line(c, "Sorry, he thinks you're too powerful an adventurer to be offering him so little.\n");
                }
                else if (macros.RND() > (gold + 1.0) / (c.player.gold + 4.0))
                {

                    /* medic wants nearly all of available gold */
                    ioclass.Do_send_line(c, "Sorry, he wasn't interested.\n");
                }
                else
                {
                    ioclass.Do_send_line(c, "He accepted.\n");
                    c.player.blind = false;

                    statsclass.Do_gold(c, -gold, 0);
                }
                ioclass.Do_more(c);
                ioclass.Do_send_clear(c);
            }
            else if (c.player.poison > 0.0)
            {

                ioclass.Do_send_line(c, "You've found a medic!\n");

                if (ioclass.Do_double_dialog(c, gold, "How much will you offer to be cured?\n") != 0)
                {

                    ioclass.Do_send_clear(c);
                    return;
                }

                /* negative gold, or more than available */
                if (gold < 0.0 || gold > c.player.gold)
                {
                    ioclass.Do_send_line(c, "He was not amused, and made you worse.\n");
                    statsclass.Do_adjusted_poison(c, 1.0);

                    statsclass.Do_strength(c, c.player.max_strength, c.player.sword, 0, 0);
                }
                /* medic needs at least a certain amount of gold to cure */
                else if (gold < c.player.level)
                {
                    ioclass.Do_send_line(c, "Sorry, he thinks you're too powerful an adventurer to be offering him so little.\n");
                }
                else if (gold < CFUNCTIONS.pow(c.player.poison, 2.5))
                {
                    ioclass.Do_send_line(c, "Sorry, you didn't offer enough to treat your virulent poison.\n");
                }
                else if (macros.RND() > (1 / c.player.poison) * CFUNCTIONS.sqrt(c.player.circle))
                {
                    ioclass.Do_send_line(c, "Sorry, the medic says the poison is beyond his ability to cure.  Find a better one.\n");
                }
                else if (macros.RND() > (1 + CFUNCTIONS.MAX(0, (1 - .1 * c.player.poison)))
                                  * (gold + 1.0) /
                                  (c.player.gold + 2.0))
                {

                    /* medic wants 1/2 of available gold for 1 poison */
                    /* as poison goes to infinity, the medic will only cure
                       25% of the time when player gives all gold */

                    ioclass.Do_send_line(c, "Sorry, he wasn't interested.\n");
                }
                else
                {
                    ioclass.Do_send_line(c, "He accepted.\n");

                    statsclass.Do_poison(c, (double)-c.player.poison);

                    statsclass.Do_gold(c, -gold, 0);
                }
                ioclass.Do_more(c);
                ioclass.Do_send_clear(c);
            }
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_tax(struct client_t *c)
        /
        / FUNCTION: Tax time!
        /
        / AUTHOR: Eugene Hung, 6/16/01
        /
        / ARGUMENTS: none
        /
        / RETURN VALUE: none
        /
        / MODULES CALLED: none
        /
        / GLOBAL INPUTS: none
        /
        / GLOBAL OUTPUTS: none
        /
        / DESCRIPTION:
        /       Handle tax collecting
        /
        *************************************************************************/

        internal void Do_tax(client_t c)
        {
            double gold, gems;
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];

            /* no tax collectors in the PoNR */
            if (c.player.beyond)
            {
                return;
            }

            ioclass.Do_send_line(c, "You've encountered a tax collector!\n");

            if ((c.player.special_type == phantdefs.SC_VALAR) || (c.wizard >= 3))
            {
                ioclass.Do_send_line(c, "He takes one look at you, screams, and tries to run away!\n");

                ioclass.Do_more(c);
                ioclass.Do_send_line(c, "You reach out, and squish him with your little finger!\n");

                ioclass.Do_more(c);

                ioclass.Do_send_clear(c);
                return;
            }
            else if (c.player.special_type >= phantdefs.SC_COUNCIL)
            {
                ioclass.Do_send_line(c, "He takes one look at you and runs away screaming in terror!\n");

                ioclass.Do_more(c);

                ioclass.Do_send_clear(c);
                return;
            }
            else if (c.player.special_type >= phantdefs.SC_STEWARD)
            {
                ioclass.Do_send_line(c, "He bows before you and walks off.\n");

                ioclass.Do_more(c);

                ioclass.Do_send_clear(c);
                return;
            }

            /* at 7% we'll only take if there's at least 15 of something */
            if (c.player.gems >= 15.0)
            {


                ioclass.Do_send_line(c, "His eyes glow at the sight of your gems.  He swipes a portion of your wealth and disappears!\n");

            }
            /* at 10% we'll only take if there's at least 10 of something */
            else if (c.player.gold >= 10.0)
            {


                ioclass.Do_send_line(c, "He cackles gleefully and takes some of your gold before disappearing!\n");

            }
            else if (c.player.gems == 0.0)
            {


                ioclass.Do_send_line(c, "'Ah,' he says, 'A candidate for our welfare program.'  He flips you a gold piece, gives a satisfied smile, and disappears!\n");


                ioclass.Do_more(c);

                ioclass.Do_send_clear(c);

                statsclass.Do_gold(c, 1.0, 0);
                return;
            }
            else
            {


                ioclass.Do_send_line(c, "He lectures, 'How can our kingdom survive when you are making such a pittance?  Work harder!' then disappears!\n");


                ioclass.Do_more(c);

                ioclass.Do_send_clear(c);
                return;
            }

            ioclass.Do_more(c);
            ioclass.Do_send_clear(c);

            gold = CFUNCTIONS.floor(c.player.gold * 0.1);
            statsclass.Do_gold(c, -gold, 0);

            gems = CFUNCTIONS.floor(c.player.gems * 0.07);
            statsclass.Do_gems(c, -gems, 0);

            gold += gems * phantdefs.N_GEMVALUE;

            /* see if there is a steward */
            /*
            Do_lock_mutex(c.realm.realm_lock);

            if (c.realm.king != null && c.realm.king_flag) {
            */

            Do_unlock_mutex(c.realm.realm_lock);
            Do_lock_mutex(c.realm.kings_gold_lock);

            /* steward coffers are capped at 40K */
            if (macros.RND() > c.realm.steward_gold / 40000.0)
            {

                /* stewards get a maximum of 1K donation */
                if (gold > 1000.0)
                {
                    c.realm.steward_gold += 1000.0;
                    gold -= 1000.0;
                }
                else
                {
                    c.realm.steward_gold += gold;

                    Do_unlock_mutex(c.realm.kings_gold_lock);
                    return;
                }
            }


            Do_unlock_mutex(c.realm.kings_gold_lock);
            /*
            }
            else {
            Do_unlock_mutex(c.realm.realm_lock);
            }
            */

            /* add to the king's coffers */
            Do_lock_mutex(c.realm.kings_gold_lock);

            /* kings coffers are capped at 2M */
            if (macros.RND() > c.realm.kings_gold / 2000000.0)
            {

                /* maximum contribution of 20K */
                if (gold > 20000.0)
                {
                    c.realm.kings_gold += 20000.0;
                    gold -= 20000.0;
                }
                else
                {
                    c.realm.kings_gold += gold;
                    gold = 0.0;
                }
            }

            Do_unlock_mutex(c.realm.kings_gold_lock);

            if (gold > 0.0)
            {
                error_msg = CFUNCTIONS.sprintfSinglestring("[%s] Do_tax did not allocate %lf in wealth.\n", c.connection_id, gold);
                fileclass.Do_log(pathnames.DEBUG_LOG, error_msg);
            }

            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_guru(struct client_t *c)
        /
        / FUNCTION: do random stuff
        /
        / AUTHOR: E. A. Estes, 3/3/86
        /         Brian Kelly, 6/20/99
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

        internal void Do_guru(client_t c)
        {
            float ftemp;

            ioclass.Do_send_line(c, "You've met a Guru. . .\n");

            if (macros.RND() * c.player.sin > 1.0)
            {
                ioclass.Do_send_line(c, "You disgusted him with your sins!\n");
                ioclass.Do_more(c);
                ioclass.Do_send_clear(c);

            }
            else if (c.player.poison > 0.0)
            {
                ioclass.Do_send_line(c, "He looked kindly upon you, and cured you.\n");
                ioclass.Do_more(c);
                ioclass.Do_send_clear(c);

                statsclass.Do_poison(c, (double)-c.player.poison);
            }
            else if ((macros.RND() / 10 > c.player.sin) && (c.player.circle > 1))
            {
                ioclass.Do_send_line(c, "He slips something into your charm pouch as a reward for your saintly behavior!\n");

                statsclass.Do_mana(c, 40.0 + 15 * c.player.circle, 0);

                ioclass.Do_more(c);
                ioclass.Do_send_clear(c);

                c.player.charms += 1 + CFUNCTIONS.floor((c.player.circle / 20)); //floor added
            }
            else
            {
                ioclass.Do_send_line(c, "He rewarded you for your virtue.\n");

                ioclass.Do_more(c);
                ioclass.Do_send_clear(c);

                statsclass.Do_mana(c, 40.0 + 10 * c.player.circle, 0);


                statsclass.Do_energy(c, c.player.energy + 2 + c.player.circle / 5,
                    c.player.max_energy, c.player.shield + 2 + c.player.circle
                    / 5, 0, 0);
            }
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_village(struct client_t *c)
        /
        / FUNCTION: do random stuff
        /
        / AUTHOR: E. A. Estes, 3/3/86
        /         Brian Kelly, 6/20/99
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

        internal void Do_village(client_t c)
        {
            event_t event_ptr = new event_t();
            long answer = -1;
            float ftemp;

            if (c.player.beyond || (c.player.level + 1 < c.player.circle))
            {
                ioclass.Do_send_line(c, "You have found a village.  But the inhabitants are too strong for you!\n");
                ioclass.Do_more(c);
                ioclass.Do_send_clear(c);
                return;
            }



            ioclass.Do_send_line(c, "You have found a village.  Do you wish to pillage it?\n");

            if (ioclass.Do_yes_no(c, ref answer) != phantdefs.S_NORM || answer == 1)
            {

                /* return if the player says no */
                ioclass.Do_send_clear(c);
                return;
            }

            ioclass.Do_send_clear(c);

            statsclass.Do_sin(c, 0.5);
            statsclass.Do_experience(c, (.75 + macros.RND() / 2) * 1000.0 * c.player.circle, 0);
            statsclass.Do_gold(c, (.75 + macros.RND() / 2) * 50.0 * c.player.circle, 0);
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_plague(struct client_t *c, struct event_t *the_event)
        /
        / FUNCTION: do random stuff
        /
        / AUTHOR: E. A. Estes, 3/3/86
        /         Brian Kelly, 6/20/99
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
        /       Handle plague.  event.arg3 contains 1000x the amount of poison to
        /       increase the player by.
        /
        *************************************************************************/

        internal void Do_plague(client_t c, event_t the_event)
        {
            float ftemp;

            if (the_event.arg3 == 0)
            {
                if (c.player.poison < CFUNCTIONS.sqrt(c.player.circle))
                {
                    ioclass.Do_send_line(c, "You've been hit with a plague!\n");

                    if (c.player.type == phantdefs.C_ELF)
                    {
                        ioclass.Do_send_line(c, "But as an elf, you are unaffected!\n");
                    }
                    else if (c.player.charms > 0)
                    {
                        ioclass.Do_send_line(c, "But your charm saved you!\n");
                        --c.player.charms;
                    }
                    else
                    {
                        statsclass.Do_adjusted_poison(c, 1.0);
                    }

                    if (c.player.virgin && macros.RND() < 0.2)
                    {
                        ioclass.Do_send_line(c, "Your virgin catches it and perishes!\n");
                        statsclass.Do_virgin(c, 0, 0);
                    }

                    ioclass.Do_more(c);
                    ioclass.Do_send_clear(c);
                }
            }
            else
            {
                ioclass.Do_send_line(c, "You absorb some of the poison you cured!\n");
                statsclass.Do_poison(c, (double)the_event.arg3 / 1000);
            }
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_volcano(struct client_t *c)
        /
        / FUNCTION: generate a place for the One Ring to be thrown into
        /
        / AUTHOR: Eugene Hung, 8/2/01
        /
        / ARGUMENTS: c
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
        /       Gives crowns
        /
        *************************************************************************/

        internal void Do_volcano(client_t c)
        {
            long answer = -1;
            float ftemp;
            string string_buffer = ""; //[phantdefs.SZ_LINE];


            ioclass.Do_send_line(c, "You've come upon a pool of lava!\n");

            if (c.player.ring_type != phantdefs.R_NONE)
            {
                ioclass.Do_send_line(c, "Do you wish to destroy your ring?\n");
            }
            else
            {
                ioclass.Do_send_line(c, "It bubbles and hisses, and you wisely avoid it.\n");
                ioclass.Do_more(c);
                ioclass.Do_send_clear(c);
                return;
            }

            if (ioclass.Do_yes_no(c, ref answer) != phantdefs.S_NORM || answer == 1)
            {

                /* return if the player says no */
                ioclass.Do_send_line(c, "You hang on to Your Precious Ring.  You hear a roar of evil laughter in the distance.\n");
                statsclass.Do_sin(c, 2.5);
                ioclass.Do_more(c);
                ioclass.Do_send_clear(c);
                return;
            }

            ioclass.Do_send_line(c, "Sweating, you attempt to throw the Ring in . . .\n");

            socketclass.Do_send_buffer(c);
            CLibPThread.sleep(3);

            ftemp = (float)macros.RND();


            /* certain races have more willpower */
            if (((c.player.type == phantdefs.C_HALFLING) && (c.player.sin <= ftemp + .25)) ||
                ((c.player.type == phantdefs.C_DWARF) && (c.player.sin * 2 <= ftemp + .25)) ||
                ((c.player.type == phantdefs.C_ELF) && (c.player.sin * 3 <= ftemp + .25)) ||
                (c.player.sin * 4 <= ftemp + .25)
               )
            {

                ioclass.Do_send_line(c, "You throw the ring into the pool...\n");
                ioclass.Do_more(c);
                ioclass.Do_send_clear(c);


                if (c.player.ring_type == phantdefs.R_DLREG)
                {

                    ioclass.Do_send_line(c, "and watch as the symbol of ultimate evil melts in its birth place!\n");
                    ioclass.Do_more(c);
                    ioclass.Do_send_line(c, "Having rid yourself of the One Ring, you feel a great burden lift from your heart!\n");
                    ioclass.Do_more(c);
                    ioclass.Do_send_clear(c);



                    statsclass.Do_sin(c, -c.player.sin);
                    statsclass.Do_experience(c, 50000.0 * c.player.circle, 0);
                    string_buffer = CFUNCTIONS.sprintfSinglestring("%s has thrown the One Ring into the Fire!  Praise %s with great praise!\n",
                            c.modifiedName, (c.player.gender ? "her" : "him"));
                    ioclass.Do_broadcast(c, string_buffer);
                    statsclass.Do_ring(c, phantdefs.R_NONE, 0);

                    if ((c.player.level > phantdefs.MIN_KING) && (c.player.level < phantdefs.MAX_KING))
                    {
                        ioclass.Do_send_line(c, "For your great deed, you have been given a golden crown!\n");
                        statsclass.Do_crowns(c, 1, 0);
                        ioclass.Do_more(c);
                    }
                }
                else
                {
                    ioclass.Do_send_line(c, "Unfortunately, nothing else seems to happen.\n");
                    statsclass.Do_ring(c, phantdefs.R_NONE, 0);
                    ioclass.Do_more(c);
                }

            }
            else
            {
                ioclass.Do_send_line(c, "You lack the willpower to do so!  You hear a sinister cackle in the distance.\n");
                statsclass.Do_sin(c, 1.0);

                if (c.player.ring_type == phantdefs.R_BAD)
                {
                    c.player.ring_duration = 0;
                    statsclass.Do_ring(c, phantdefs.R_SPOILED, 0);
                }
                ioclass.Do_more(c);
            }

            ioclass.Do_send_clear(c);

            return;
        }

        /************************************************************************
        /
        / FUNCTION NAME: Do_shutdown_check(struct client_t *c)
        /
        / FUNCTION: do random stuff
        /
        / AUTHOR: E. A. Estes, 3/3/86
        /         Brian Kelly, 6/20/99
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

        internal void Do_shutdown_check(client_t c)
        {
            if (CFUNCTIONS.server_hook != phantdefs.RUN_SERVER && CFUNCTIONS.server_hook != phantdefs.LEISURE_SHUTDOWN)
            {
                socketclass.Do_send_int(c, phantdefs.SHUTDOWN_PACKET);
                socketclass.Do_send_buffer(c);
                c.socket_up = false;
            }
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_information(struct client_t *c)
        /
        / FUNCTION: do random stuff
        /
        / AUTHOR: Brian Kelly, 11/8/99
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

        internal void Do_information(client_t c)
        {
            event_t event_ptr = new event_t();
            button_t buttons = new button_t();
            long answer = -1;
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            string string_buffer = ""; //[phantdefs.SZ_LINE];

            ioclass.Do_clear_buttons(buttons, 0);

            CFUNCTIONS.strcpy(ref buttons.button[0], "Stats\n");
            CFUNCTIONS.strcpy(ref buttons.button[1], "Examine\n");
            CFUNCTIONS.strcpy(ref buttons.button[2], "Players\n");
            CFUNCTIONS.strcpy(ref buttons.button[3], "Scoreboard\n");
            CFUNCTIONS.strcpy(ref buttons.button[4], "Channel\n");

            if (c.hearBroadcasts)
            {
                CFUNCTIONS.strcpy(ref buttons.button[5], "No Announce\n");
            }
            else
            {
                CFUNCTIONS.strcpy(ref buttons.button[5], "Announce\n");
            }


            if (c.game.chatFilter)
            {
                CFUNCTIONS.strcpy(ref buttons.button[6], "Filter Off\n");
            }
            else
            {
                CFUNCTIONS.strcpy(ref buttons.button[6], "Filter On\n");
            }


            CFUNCTIONS.strcpy(ref buttons.button[7], "Cancel\n");

            if (ioclass.Do_buttons(c, ref answer, buttons) != phantdefs.S_NORM || answer == 7)
            {
                ioclass.Do_send_clear(c);
                return;
            }

            ioclass.Do_send_clear(c);

            event_ptr = eventclass.Do_create_event();
            event_ptr.to = c.game;
            event_ptr.from = c.game;

            switch (answer)
            {

                /* see this player's stats */
                case 0:
                    event_ptr.type = (short)phantdefs.EXAMINE_EVENT;
                    event_ptr.arg3 = 0;
                    break;

                case 1:
                    event_ptr.type = (short)phantdefs.EXAMINE_EVENT;
                    event_ptr.arg3 = 1;
                    break;

                case 2:
                    event_ptr.type = (short)phantdefs.LIST_PLAYER_EVENT;
                    break;

                case 3:

                    Do_scoreboard(c, 0, 1);
                    break;

                case 4:


                    string_buffer = CFUNCTIONS.sprintfSinglestring("You are currently on channel %d.\n",
                        c.channel);


                    ioclass.Do_send_line(c, string_buffer);


                    ioclass.Do_send_line(c, "Channel 8 is reserved for players with a palantir.\n");

                    ioclass.Do_send_line(c, "Which channel do you want to change to?\n");

                    ioclass.Do_clear_buttons(buttons, 0);

                    CFUNCTIONS.strcpy(ref buttons.button[0], "1\n");
                    CFUNCTIONS.strcpy(ref buttons.button[1], "2\n");
                    CFUNCTIONS.strcpy(ref buttons.button[2], "3\n");
                    CFUNCTIONS.strcpy(ref buttons.button[3], "4\n");
                    CFUNCTIONS.strcpy(ref buttons.button[4], "5\n");
                    CFUNCTIONS.strcpy(ref buttons.button[5], "6\n");
                    CFUNCTIONS.strcpy(ref buttons.button[6], "7\n");

                    if (c.player.palantir || c.wizard > 3)
                    {
                        CFUNCTIONS.strcpy(ref buttons.button[7], "8\n");
                    }

                    if (ioclass.Do_buttons(c, ref answer, buttons) != phantdefs.S_NORM)
                    {

                        ioclass.Do_send_clear(c);

                        event_ptr = null; // free((void*) event_ptr);
                        return;
                    }


                    ioclass.Do_send_clear(c);

                    /* update the player info, if necessary */
                    if (c.channel != (int)answer + 1)
                    {

                        /* if we were palantiring, turn off snooping  */
                        if (c.channel == 8)
                        {
                            c.game.hearAllChannels = phantdefs.HEAR_SELF;
                        }


                        c.channel = (int)answer + 1;

                        /* if we used our palantir, turn on snooping */
                        if (c.channel == 8)
                        {
                            c.game.hearAllChannels = phantdefs.HEAR_ONE;
                        }

                        Do_lock_mutex(c.realm.realm_lock);
                        characterclass.Do_player_description(c);
                        Do_unlock_mutex(c.realm.realm_lock);


                        characterclass.Do_send_specification(c, phantdefs.CHANGE_PLAYER_EVENT);
                    }

                    break;

                /*
                    case 5:
                    free((void *)event_ptr);

                    string_buffer = CFUNCTIONS.sprintfSinglestring("/var/phantasia/backup2/%s", c.player.lcname);

                    errno = 0;
                        if ((character_file=CLibFile.fopen(string_buffer, "w")) == null) {

                            error_msg = CFUNCTIONS.sprintf(ref 
                            "[%s] CLibFile.fopen of %s failed in Do_information: %s\n",
                            c.connection_id, string_buffer, CFUNCTIONS.strerror(errno));

                            fileclass.Do_log_error(error_msg);
                        return;
                        }

                      int errno = 0;
                        if (CLibFile.fwrite((void *)c.player, phantdefs.SZ_PLAYER, 1, character_file) != 1) {

                            error_msg = CFUNCTIONS.sprintf(ref 
                            "[%s] CLibFile.fwrite of %s failed in Do_information: %s\n",
                                    c.connection_id, string_buffer, CFUNCTIONS.strerror(errno));

                            fileclass.Do_log_error(error_msg);
                            CLibFile.fclose(character_file);
                            return;
                        }

                        CLibFile.fclose(character_file);

                        string_buffer = CFUNCTIONS.sprintfSinglestring("%s has been backed up for a server crash.\n",
                        c.modifiedName);

                    ioclass.Do_send_line(c, string_buffer);
                    ioclass.Do_send_line(c,
                        "Backing up this character again will destroy the old backup.\n");

                    ioclass.Do_more(c);
                    return;
                */

                case 5:

                    event_ptr = null; // free((void*) event_ptr);

                    if (c.hearBroadcasts)
                    {

                        ioclass.Do_send_line(c, "Server announcements are now blocked.\n");
                        c.hearBroadcasts = false;
                    }
                    else
                    {

                        ioclass.Do_send_line(c, "You will now hear server announcements.\n");
                        c.hearBroadcasts = true;
                    }


                    ioclass.Do_more(c);

                    ioclass.Do_send_clear(c);
                    return;

                case 6:
                    c.game.chatFilter = !c.game.chatFilter;
                    return;

                default:


                    error_msg = CFUNCTIONS.sprintfSinglestring(
                        "[%s] Returned non-option %ld in Do_information.\n",
                        c.connection_id, answer);


                    fileclass.Do_log_error(error_msg);

                    event_ptr = null; //free((void*) event_ptr);

                    hackclass.Do_caught_hack(c, phantdefs.H_SYSTEM);
                    return;
            }

            if (event_ptr.type != phantdefs.NULL_EVENT)
            {
                eventclass.Do_handle_event(c, event_ptr);
            }
            else
            {

                event_ptr = null; //free((void*) event_ptr);
            }
            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_scoreboard(struct client_t *c)
        /
        / FUNCTION: do random stuff
        /
        / AUTHOR: Brian Kelly, 12/30/99
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

        internal void Do_scoreboard(client_t c, int start, int newi = 0)
        {
            string string_buffer = ""; //[phantdefs.SZ_LINE];
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            scoreboard_t scoreboard = new scoreboard_t();
            CLibFile.CFILE scoreboard_file;
            long theEOF = -1;
            int i = -1, records = -1, EOF_flag = -1;

            socketclass.Do_send_int(c, phantdefs.SCOREBOARD_DIALOG_PACKET);

            Do_lock_mutex(c.realm.scoreboard_lock);

            /* open the scoreboard file */
            int errno = 0;

            if (newi != 0)
            {
                if ((scoreboard_file = CLibFile.fopen(pathnames.SCOREBOARD_FILE, "r", ref errno)) == null)
                {

                    error_msg = CFUNCTIONS.sprintfSinglestring("[%s] CLibFile.fopen of %s failed in Do_scoreboard: %s\n",
                    c.connection_id, pathnames.SCOREBOARD_FILE, CFUNCTIONS.strerror(errno));

                    fileclass.Do_log_error(error_msg);

                    //ioclass.Do_send_line(c,
                    //"1\nCurrently, there is nobody on the scoreboard.\n"); //bugfix: split into two messages instead
                    ioclass.Do_send_line(c, "1\n");
                    ioclass.Do_send_line(c, "Currently, there is nobody on the scoreboard.\n");

                    Do_unlock_mutex(c.realm.scoreboard_lock);
                    return;
                }
                else
                {

                    /* drop to next part */
                }

            }
            else
            {
                if ((scoreboard_file = CLibFile.fopen(pathnames.OLD_SCOREBOARD_FILE, "w+", ref errno)) == null) //changed r to w+ to allow creation. bug in original?
                {

                    error_msg = CFUNCTIONS.sprintfSinglestring("[%s] CLibFile.fopen of %s failed in Do_scoreboard: %s\n",
                    c.connection_id, pathnames.OLD_SCOREBOARD_FILE, CFUNCTIONS.strerror(errno));

                    fileclass.Do_log_error(error_msg);

                    //ioclass.Do_send_line(c,
                    //"1\nCurrently, there is nobody on the scoreboard.\n");
                    ioclass.Do_send_line(c, "1\n");
                    ioclass.Do_send_line(c, "Currently, there is nobody on the scoreboard.\n");

                    Do_unlock_mutex(c.realm.scoreboard_lock);
                    return;
                }
                else
                {

                    /* drop to next part */
                }
            }


            /* find the end of the scoreboard file */
            CLibFile.fseek(scoreboard_file, 0, CLibFile.SEEK_END);
            theEOF = CLibFile.ftell(scoreboard_file);

            /* calculate how many records are in the file */
            if (phantdefs.SZ_SCOREBOARD > 0)
            {
                records = CFUNCTIONS.floor(theEOF / phantdefs.SZ_SCOREBOARD);
            }
            else
            {
                Debug.Log("<color=red>Warning: scoreboard size is 0</color>");
                records = 0;
            }

            /* the earliest we can start is the beginning */
            if (start < 0)
            {
                start = 0;
            }

            /* we'll only show 50 records maximum */
            records = 50;

            /* tell the client number of records and their start */
            socketclass.Do_send_int(c, start);

            socketclass.Do_send_int(c, records);

            /* move to the starting record */
            EOF_flag = 0;
            start = phantdefs.SZ_SCOREBOARD;
            CLibFile.fseek(scoreboard_file, start, CLibFile.SEEK_SET);
            /* read the next 50 or so records */
            for (i = start + 1; i < records + start + 1; i++)
            {

                if (EOF_flag == 0 && CLibFile.fread(ref scoreboard, phantdefs.SZ_SCOREBOARD, 1,
                scoreboard_file) == 1)
                {

                    /*
                                CFUNCTIONS.sprintf(ref string_buffer,
                                "%d> %s, the level %0.lf %s, was %s at ", i,
                                    scoreboard.name, scoreboard.level, scoreboard.class,
                                    scoreboard.how_died);
                    */
                    if (scoreboard.level == 9999)
                    {


                        CFUNCTIONS.sprintf(ref string_buffer,
                     "%d> %s the %s ascended to the position of Valar on ",
                 i, scoreboard.name, scoreboard.classclass);


                        socketclass.Do_send_string(c, string_buffer);


                        CFUNCTIONS.ctime_r(scoreboard.time, ref string_buffer);

                        Do_truncstring(string_buffer);

                        CFUNCTIONS.strcat(string_buffer, ".\n");

                        socketclass.Do_send_string(c, string_buffer);
                    }
                    else
                    {

                        CFUNCTIONS.sprintf(ref string_buffer,
                    "%d> %s, the level %0.lf %s, %s on ", i,
                        scoreboard.name, scoreboard.level, scoreboard.classclass,
                        scoreboard.how_died);
                        /*
                                        CFUNCTIONS.sprintf(ref string_buffer,
                                        "%d> %s, the level %0.lf %s,", i,
                                            scoreboard.name, scoreboard.level,
                                        scoreboard.class);
                        */

                        socketclass.Do_send_string(c, string_buffer);


                        CFUNCTIONS.ctime_r(scoreboard.time, ref string_buffer);

                        Do_truncstring(string_buffer);

                        CFUNCTIONS.strcat(string_buffer, ".\n");

                        socketclass.Do_send_string(c, string_buffer);
                    }
                }
                else
                {
                    EOF_flag = 1;

                    string_buffer = CFUNCTIONS.sprintfSinglestring("%d> No entry.\n", i);
                    socketclass.Do_send_string(c, string_buffer);
                }
            }

            /* close the file handles */
            CLibFile.fclose(scoreboard_file);

            socketclass.Do_send_buffer(c);

            Do_unlock_mutex(c.realm.scoreboard_lock);
        }


        /************************************************************************
        /
        / FUNCTION NAME: int Do_profanity_check(char *theString)
        /
        / FUNCTION: do random stuff
        /
        / AUTHOR: Brian Kelly, 1/2/01
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

        internal int Do_profanity_check(string theString)
        {
            string[] swears = { "shit", "fuck", "fuc", "fuk", "fock", "piss", "cunt", "cock", "ass", "dick", "penis", "pussy", "clit", "bitch", "butt", "twat", "vagina", "dork", "dildo", "masturbat", "fag", "homo", "lesbian", "screw", "anal", "poo", "nigger", "nigga", "chink", "jap", "wop", "kike", "bitch", "whore", "crap", "hell", "damn", "rape", "suck", "abortion", "mute", "sex", "kkk", "balls", "bush", "tits", "dammit", "snatch", "sauron", "morgoth", "osama", "alatar", "stfu", "wtf", "" };

            char currentChar;
            int stringPtr, currentSwear, swearPtr, wordBeginning, swearBeginning, spaceCount;

            if (theString == null || theString == "" || theString == "\0" || (theString.Length > 0 && theString.ToCharArray()[0] == '\0'))
            {
                return 0;
            }

            currentSwear = 0;

            /* loop through all the swear words */
            while (swears[currentSwear] != "")
            {

                stringPtr = swearPtr = wordBeginning = swearBeginning = spaceCount = 0;

                /* look for the next letter in the swear */
                while (theString.Length > stringPtr)//theString[stringPtr] != "\0")
                {

                    currentChar = theString.ToCharArray()[stringPtr];

                    /* replace numbers that could be letters */
                    if (currentChar == '1' && swears[currentSwear].ToCharArray()[swearPtr] ==
                        'i')
                    {

                        currentChar = 'i';
                    }

                    else if (currentChar == '1' && swears[currentSwear].ToCharArray()[swearPtr] ==
                        'l')
                    {

                        currentChar = 'l';
                    }
                    else if (currentChar == 'z' && swears[currentSwear].ToCharArray()[swearPtr] ==
                        's')
                    {

                        currentChar = 's';
                    }
                    else if (currentChar == '5' && swears[currentSwear].ToCharArray()[swearPtr] ==
                        's')
                    {

                        currentChar = 's';
                    }
                    else if (currentChar == '0' && swears[currentSwear].ToCharArray()[swearPtr] ==
                        'o')
                    {

                        currentChar = 'o';
                    }
                    else if (currentChar == '$' && swears[currentSwear].ToCharArray()[swearPtr] ==
                        's')
                    {

                        currentChar = 's';
                    }
                    else if (currentChar == '@' && swears[currentSwear].ToCharArray()[swearPtr] ==
                        'a')
                    {

                        currentChar = 'a';
                    }

                    /* Is this character a space? */
                    if (currentChar == '_')
                    {

                        if (swearPtr == 0)
                        {
                            wordBeginning = stringPtr + 1;
                        }
                        else
                        {
                            ++spaceCount;
                        }
                    }

                    /* If the character is not a letter, skip it */
                    else if (CFUNCTIONS.isalpha(currentChar))
                    {

                        if (currentChar == swears[currentSwear].ToCharArray()[swearPtr])
                        {
                            ++swearPtr;

                            /* See if we've found a complete swear */
                            if (swears[currentSwear].ToCharArray()[swearPtr] == '\0') //todo: append \0 to swears?
                            {

                                /* if the swear did not start at the beginning
                                of a word and contains one space, let it pass.
                                "is_hit" would be caught */
                                if (wordBeginning != swearBeginning || spaceCount
                                    != 1)
                                {

                                    return 1;
                                }

                                swearPtr = 0;
                                spaceCount = 0;
                                stringPtr = swearBeginning;
                                ++swearBeginning;
                            }
                        }

                        /* Start over if anything but previous swear letter */
                        /* This is to catch "ffuucckk" */
                        else if (swearPtr == 0 || currentChar !=
                            swears[currentSwear].ToCharArray()[swearPtr - 1])
                        {

                            swearPtr = 0;
                            spaceCount = 0;
                            stringPtr = swearBeginning;
                            ++swearBeginning;
                        }
                    }

                    /* move to the next letter */
                    ++stringPtr;
                }

                ++currentSwear;
            }

            return 0;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_replace_profanity(struct client_t *c, char *theString)
        /
        / FUNCTION: do random stuff
        /
        / AUTHOR: Brian Kelly, 10/11/00
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

        int Do_replace_profanity(string theString)
        {
            string[] swears = { "shit", "fock", "fuk", "fuc", "cunt", "nigger", "crap", "damn", "dammit", "@ss", "a$$", "" };
            string replacements = "smurf ";
            char currentChar;
            int stringPos = 0, currentSwear = 0, swearPos = 0, wordStart = 0, swearStart = 0, smurfPos = 0;
            int spaceCount = 0, bleepPos = 0;
            bool profane, spaceExceptionPossible;

            if (theString == "\0")
            {
                return 0;
            }

            profane = false;
            currentSwear = 0;

            /* loop through all the swear words */
            while (swears[currentSwear][0] != '\0')
            {

                stringPos = swearPos = wordStart = swearStart = 0;
                spaceExceptionPossible = false;

                /* look until the we reach the end of the string */
                while (theString[stringPos] != '\0')
                {

                    currentChar = CFUNCTIONS.tolower(theString[stringPos]);

                    /* Convert similar symbols */
                    if (currentChar == '!' && swears[currentSwear][swearPos] == 'i')
                    {
                        currentChar = 'i';
                    }

                    else if (currentChar == '!' && swears[currentSwear][swearPos] ==
                        'l')
                    {

                        currentChar = 'l';
                    }

                    else if (currentChar == '1' && swears[currentSwear][swearPos] ==
                        'i')
                    {

                        currentChar = 'i';
                    }

                    else if (currentChar == '1' && swears[currentSwear][swearPos] ==
                        'l')
                    {

                        currentChar = 'l';
                    }

                    else if (currentChar == '#' && swears[currentSwear][swearPos] ==
                        'h')
                    {

                        currentChar = 'h';
                    }

                    else if (currentChar == '$' && swears[currentSwear][swearPos] ==
                        's')
                    {

                        currentChar = 's';
                    }

                    else if (currentChar == '(' && swears[currentSwear][swearPos] ==
                        'c')
                    {

                        currentChar = 'c';
                    }


                    /* Is this character a space? */
                    if (currentChar == ' ')
                    {

                        /* if this space is within a possible swear */
                        if (swearPos > 0)
                        {

                            /* count the space for possible exception */
                            ++spaceCount;
                        }

                        /* if we've found no swear letters */
                        else
                        {

                            /* we won't find anything before this */
                            wordStart = stringPos + 1;
                            swearStart = wordStart;

                            /* since the swear starts a word, no exception */
                            spaceExceptionPossible = false;
                        }

                        /* move to the next letter of the string */
                        ++stringPos;
                        continue;
                    }

                    /* If the character is not a letter, skip it */
                    else if (!CFUNCTIONS.isalpha(currentChar))
                    {
                        ++stringPos;
                        continue;
                    }

                    /* Is this character the next of this swear? */
                    else if (currentChar == swears[currentSwear][swearPos])
                    {

                        /* start looking for the next letter */
                        ++swearPos;

                        /* have we found all the swear letters? */
                        if (swears[currentSwear][swearPos] == '\0')
                        {

                            /* we don't want to bleep "is hit" or "if u c", 
                                        so pass over if swear didn't start a word and there 
                                        was 1 or 2 spaces */

                            if (!spaceExceptionPossible || spaceCount != 1)
                            {

                                /* bleep out the whole word */
                                bleepPos = wordStart;
                                profane = true;

                                /* loop until space past current location */
                                while (bleepPos < stringPos ||
                                    (theString[bleepPos] != ' ' &&
                                    theString[bleepPos] != '\0'))
                                {

                                    /* replace the letter */

                                    smurfPos = bleepPos - wordStart;
                                    if (smurfPos > 4)
                                    {
                                        smurfPos = 5;
                                    }

                                    char[] theStringChar = theString.ToCharArray();
                                    theStringChar[bleepPos++] = replacements[smurfPos];
                                    theString = new string(theStringChar);
                                }

                                if (smurfPos == 3)
                                {
                                    char[] theStringChar = theString.ToCharArray();
                                    theStringChar[bleepPos + 1] = theString[bleepPos];
                                    theStringChar[bleepPos] = replacements[smurfPos + 1];
                                    theString = new string(theStringChar);
                                }

                                /* start again at the last replacement */
                                /* bleepPos is a space or null, so the
                                spaceExceptionPossible will be handled */
                                stringPos = bleepPos;
                                swearPos = 0;
                                continue;
                            }

                            /* this is a swear, but we're passing it over */
                            else
                            {

                                /* start one after where we did last time */
                                stringPos = swearStart + 1;
                                swearPos = 0;
                                continue;
                            }
                        }

                        /* found the next swear letter */
                        ++stringPos;
                        continue;
                    }

                    /* See if this letter is the same as the last */
                    /* This is to catch "ffuucckk" */
                    else if (swearPos != 0 && currentChar ==
                        swears[currentSwear][swearPos - 1])
                    {

                        /* move to the next letter */
                        ++stringPos;
                        continue;
                    }

                    /* different letter - start one after where we started before */
                    stringPos = swearStart = swearStart + 1;
                    swearPos = 0;
                    spaceExceptionPossible = true;
                    spaceCount = 0;
                }

                ++currentSwear;
            }

            return profane ? 1 : 0;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_censor(char *destString, char *sourceString)
        /
        / FUNCTION: do random stuff
        /
        / AUTHOR: Brian Kelly, 06/25/02
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


        internal int Do_censor(string destString, string sourceString)
        {
            string[] swears = { "shit", "fuck", "cunt", "nigger", "cock", "dick", "penis", "pussy", "clit", "vagina", "dildo", "masturbate", "masturbation", "anal", "chink", "kike", "tits", "" };

            char[][] substitute = new char[][] {
                    new char[5]{ 'a', '@', '\0', '\0','\0' },
                    new char[5]{ 'c', '(', '\0', '\0', '\0' },
                    new char[5]{ 'h', '#', '\0', '\0', '\0' },
                    new char[5]{ 'i', 'l', '!', '1', '\0' },
                    new char[5]{ 'l', 'i', '!', '1', '\0' },
                    new char[5]{ 'o', '\0', '\0', '\0', '\0' },
                    new char[5]{ 's', '$', '5', 'z', '\0' },
                    new char[5]{ '\0', '\0', '\0', '\0', '\0' }
                    };

            int sourceLetter, swearNumber, swearLetter, letterPtr = 0, row, column, count;
            bool newWordFlag, matchFlag, spaceFlag, swearFlag;
            char[] lcSource = new char[sourceString.Length]; //[phantdefs.SZ_CHAT + 64], 
            char currentLetter;
            char[] destStringchar = new char[destString.Length];
            int currchar = 0;

            /* make a lowercase version of the string */
            sourceLetter = 0;
            do
            {
                lcSource[sourceLetter] = CFUNCTIONS.tolower(sourceString[sourceLetter]);
            }
            while (sourceString[sourceLetter++] != '\0');

            /* start at the first letter of the source */
            sourceLetter = 0;
            newWordFlag = true;
            count = 0;

            /* run through all the characters of the source string */
            while (sourceString[sourceLetter] != '\0')
            {

                /* if this character is a space */
                if (sourceString[sourceLetter] == ' ')
                {

                    /* no swears start with a space */
                    destStringchar[currchar++] = ' ';
                    newWordFlag = true;
                    ++sourceLetter;
                    continue;
                }

                /* start at the front of the list of swears */
                swearFlag = false;
                swearNumber = 0;

                /* loop through all the swear words */
                while (swears[swearNumber][0] != '\0')
                {

                    /* start with the first letter of this swear */
                    swearLetter = 0;

                    /* we start checking from our current position */
                    letterPtr = sourceLetter;
                    spaceFlag = false;

                    /* check the swear from this letter */
                    do
                    {

                        /* assume no match */
                        matchFlag = false;

                        /* convert the current character to lowercase */
                        currentLetter = lcSource[letterPtr];

                        /* if the current letter is a space and
                        we're in the middle of a swear */
                        if (currentLetter == ' ' && swears[swearNumber][swearLetter] != '\0')
                        {

                            /* set the space flag */
                            spaceFlag = true;

                            /* increment to the next string character and re-loop */
                            ++letterPtr;
                            continue;
                        }

                        /* if the letter is the next in the swear */
                        if (swears[swearNumber][swearLetter] == currentLetter)
                        {

                            /* swear matches */
                            matchFlag = true;
                        }

                        /* see if a substitute character was used (swear isn't null) */
                        else if (swears[swearNumber][swearLetter] != '\0')
                        {

                            row = 0;

                            /* run through the substitute list */
                            while (substitute[row][0] != '\0')
                            {

                                /* if this sub char is the next in the swear */
                                if (substitute[row][0] == swears[swearNumber][swearLetter])
                                {

                                    column = 1;

                                    /* run through the list */
                                    while (substitute[row][column] != '\0')
                                    {

                                        /* is the string letter this replacement character? */
                                        if (currentLetter == substitute[row][column])
                                        {

                                            /* swear matches */
                                            matchFlag = true;
                                            break;
                                        }

                                        ++column;
                                    }

                                    /* a letter is only listed as a sub once */
                                    break;
                                }

                                ++row;
                            }
                        }

                        /* if this letter matches the next in the swear */
                        if (matchFlag)
                        {

                            /* move to the next letter of the string and swear */
                            ++letterPtr;
                            ++swearLetter;
                        }

                        /* otherwise, if we're in the middle of a swear */
                        else if (swearLetter != 0)
                        {

                            /* if the letter is a repeat of the last */
                            if (swears[swearNumber][swearLetter - 1] == currentLetter)
                            {

                                /* swear is a repeat */
                                matchFlag = true;
                            }

                            /* see if it could be a substitute character repeat */
                            else
                            {

                                row = 0;

                                /* run through the substitute list */
                                while (substitute[row][0] != '\0')
                                {

                                    /* if this sub char is the previous one in the swear */
                                    if (substitute[row][0] ==
                                    swears[swearNumber][swearLetter - 1])
                                    {

                                        column = 1;

                                        /* run through the list */
                                        while (substitute[row][column] != '\0')
                                        {

                                            /* is the string letter this replacement character? */
                                            if (currentLetter == substitute[row][column])
                                            {

                                                /* swear matches */
                                                matchFlag = true;
                                                break;
                                            }

                                            ++column;
                                        }

                                        /* a letter is only listed as a sub once */
                                        break;
                                    }

                                    ++row;
                                }
                            }

                            /* if this letter matches the previous in the swear */
                            if (matchFlag)
                            {

                                /* move to the next letter of the string only */
                                ++letterPtr;
                            }
                        }
                    }
                    while (matchFlag);

                    /* see if a complete swear was found */
                    if (swears[swearNumber][swearLetter] == '\0')
                    {

                        /* if the swear contains spaces and didn't start a word */
                        if (!newWordFlag && spaceFlag)
                        {

                            /* reject the word for replacement */
                            swearLetter = 0;
                        }

                        /* if the swear is valid */
                        else
                        {

                            swearFlag = true;

                            /* break out of the search, we'll replace it below */
                            break;
                        }
                    }

                    ++swearNumber;
                }

                /* if a complete swear was found */
                if (swearFlag)
                {

                    /* if the original was capitalized */
                    if (CFUNCTIONS.isupper(sourceString[sourceLetter]))
                    {

                        /* if the second letter was uppercase */
                        if (CFUNCTIONS.isupper(sourceString[sourceLetter + 1]))
                        {

                            /* assume it was all uppercase */
                            CFUNCTIONS.strcpy(ref destString, "SMURF");
                        }
                        else
                        {

                            /* just capitalize it */
                            CFUNCTIONS.strcpy(ref destString, "Smurf");
                        }
                    }
                    /* the word was lowercase */
                    else
                    {

                        /* replace the word */
                        CFUNCTIONS.strcpy(ref destString, "smurf");
                    }

                    /* move the pointer over the replacement */
                    destString += 5;

                    /* the we continue from the end of the swear */
                    sourceLetter = letterPtr;

                    /* the next character is never the start of a new word */
                    newWordFlag = false;

                    /* increment the counter */
                    ++count;
                }

                /* no swear was found */
                else
                {

                    /* if this letter is a space */
                    if (sourceString[sourceLetter] == ' ')
                    {

                        /* the next character will be the start of a new word */
                        newWordFlag = true;
                    }
                    else
                    {
                        newWordFlag = false;
                    }

                    /* copy over this letter and move to the next letter */
                    destStringchar[currchar++] = sourceString[sourceLetter++];

                }

            }

            /* add a null */
            destString = "\0";
            destString = new string(destStringchar); //added

            /* see if this was an buffer overflow attempt */
            if (count > 20)
            {

                return 1;
            }

            return 0;
        }


        /************************************************************************
        /
        / FUNCTION NAME: int Do_spam_check(struct client_t *c, char *message)
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

        internal int Do_spam_check(client_t c, string message)
        {
            int i, count;

            /* move up all the previous chat times and length */
            for (i = 8; i >= 0; i--)
            {
                c.chatTimes[i + 1] = c.chatTimes[i];
                c.chatLength[i + 1] = c.chatLength[i];
            }

            /* add the current moment into the lists */
            c.chatTimes[0] = CFUNCTIONS.GetUnixEpoch(DateTime.Now);
            c.chatLength[0] = (int)CFUNCTIONS.floor(CFUNCTIONS.strlen(message) / 80.0);

            /* see if we can find a chat infraction */
            for (i = 3; i < 9; i++)
            {

                /* player can send 3 + n messages every n^2 seconds */
                if (c.chatTimes[0] - c.chatTimes[i] <= (i - 2) * (i - 2))
                {

                    /* tag this player as muted */
                    hackclass.Do_caught_spam(c, phantdefs.H_SPAM);
                    return 1;
                }
            }

            /* add up the lines of text from the last nine messages */
            count = 0;
            for (i = 0; i <= 9; i++)
            {
                count += c.chatLength[i];
            }

            CFUNCTIONS.sprintfSinglestring("count check: %d.\n", count);
            /* if the number of lines exceeds 14 */
            if (count >= 14)
            {

                /* clear the history so we don't nail them forever */
                for (i = 0; i <= 9; i++)
                {
                    c.chatLength[i] = 0;
                }

                /* tag this player as muted */
                hackclass.Do_caught_spam(c, phantdefs.H_FLOOD);
                return 1;
            }

            /* no problems */
            return 0;
        }


        /******************************************************************
        /
        / FUNCTION NAME: Do_title_page(struct client_t *c)
        /
        / FUNCTION: print title page
        /
        / AUTHOR: E. A. Estes, 12/4/85
        /	  Brian Kelly, 5/3/99
        /
        / ARGUMENTS: none
        /
        / RETURN VALUE: none
        /
        / MODULES CALLED: CLibFile.fread(ref ), fseek(), CLibFile.fopen(), CLibFile.fgets(), wmove(), CFUNCTIONS.strcpy(ref ),
        /       CLibFile.fclose(), cfunctions.strlen(), waddstr(), CFUNCTIONS.sprintf(ref ), wrefresh()
        /
        / DESCRIPTION:
        /       Print important information about game, players, etc.
        /
        *************************************************************************/

        internal void Do_title_page(client_t c)
        {
            game_t game_ptr = new game_t(), first_ptr = new game_t(), second_ptr = new game_t();
            short councilfound;   /* set if we find a member of the council */
            string string_buffer = ""; //[phantdefs.SZ_LINE];
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            int error;

            /* print a header */
            ioclass.Do_send_line(c,
                        "W e l c o m e   t o   P h a n t a s i a (vers. 4.03)!\n");
            ioclass.Do_send_line(c,
                " "); //"http://Your address here\n"); //todo: commented address for now
            ioclass.Do_send_line(c,
                "\n");
            /* lock the realm */
            Do_lock_mutex(c.realm.realm_lock);

            /* display the current king, if any */
            if (c.realm.king == null || !c.realm.king_flag)
            {

                if (c.realm.king == null)
                {
                    ioclass.Do_send_line(c, "There is no ruler at this time.\n");
                }
                else
                {
                    if (c.realm.king.description != null)
                    {

                        CFUNCTIONS.sprintf(ref string_buffer,
                        "The present steward is %s  Level: %.0lf\n",
                                c.realm.king.description.name,
                        c.realm.king.description.level);


                        ioclass.Do_send_line(c, string_buffer);
                    }
                    else
                    {

                        CFUNCTIONS.sprintf(ref string_buffer,
                     "[%s] Information on present steward unavailable in Do_title_page.\n",
                       c.connection_id);


                        fileclass.Do_log_error(string_buffer);
                    }
                }


            }
            else
            {
                /* make sure the character has a description in place */
                if (c.realm.king.description != null)
                {

                    /* print out the present king/queen */
                    if (c.realm.king.description.gender == false)
                    {

                        CFUNCTIONS.sprintf(ref string_buffer,
                        "The present ruler is King %s  Level: %.0lf\n",
                                c.realm.king.description.name,
                        c.realm.king.description.level);
                    }
                    else
                    {

                        CFUNCTIONS.sprintf(ref string_buffer,
                        "The present ruler is Queen %s  Level: %.0lf\n",
                                c.realm.king.description.name,
                        c.realm.king.description.level);
                    }
                }
                else
                {

                    CFUNCTIONS.sprintf(ref string_buffer,
                   "[%s] Information on present ruler unavailable in Do_title_page.\n",
                   c.connection_id);


                    fileclass.Do_log_error(string_buffer);
                }


                ioclass.Do_send_line(c, string_buffer);
            }

            /* display the current valar, in any */
            if (c.realm.valar_name != null && c.realm.valar_name[0] != '\0')
            {

                string_buffer = CFUNCTIONS.sprintfSinglestring("The Valar is %s\n",
                        c.realm.valar_name);

                /*
                    if (c.realm.valar.description != null) {

                            string_buffer = CFUNCTIONS.sprintfSinglestring("The Valar is %s   Level:  %.0lf\n",
                                    c.realm.valar.description.name,
                            c.realm.valar.description.level);
                    }
                    else {
                        CFUNCTIONS.sprintf(ref string_buffer,
                           "[%s] Information on valar unavailable in Do_title_page.\n",
                           c.connection_id);

                        fileclass.Do_log_error(string_buffer);
                    }

                */
                ioclass.Do_send_line(c, string_buffer);
            }


            /* search for council members */
            councilfound = 0;
            game_ptr = c.realm.games;

            /* loop through all the games */
            while (game_ptr != null && councilfound < 5)
            {

                /* if the player is playing and is on the council */
                if (game_ptr.description != null &&
                    (game_ptr.description.special_type == phantdefs.SC_COUNCIL ||
                    game_ptr.description.special_type == phantdefs.SC_EXVALAR))
                {

                    /* print a header if this is the first council member */
                    if (councilfound == 0)
                    {
                        ioclass.Do_send_line(c, "Council of the Wise:\n");
                    }

                    /* print out the member */

                    string_buffer = CFUNCTIONS.sprintfSinglestring("%s  Level: %.0lf\n",
                    game_ptr.description.name,
                    game_ptr.description.level);


                    ioclass.Do_send_line(c, string_buffer);

                    /* indicate that we've found another member */

                    councilfound++;
                }


                /* look at the next game */

                game_ptr = game_ptr.next_game;
            }


            /* search for the two highest players */
            first_ptr = null;
            second_ptr = null;
            game_ptr = c.realm.games;

            while (game_ptr != null)
            {

                /* if the player is in the game */
                if (game_ptr.description != null)
                {

                    /* see if the current game is higher level than the current */
                    if ((first_ptr == null || (game_ptr.description.level > first_ptr.description.level)) &&
                          (game_ptr.description.special_type < phantdefs.SC_STEWARD) &&
                          (game_ptr.description.wizard < 3))
                    {



                        /* put the current game into first position */
                        second_ptr = first_ptr;
                        first_ptr = game_ptr;
                    }
                    else if ((second_ptr == null ||
                          (game_ptr.description.level > second_ptr.description.level)) &&
                          (game_ptr.description.special_type < phantdefs.SC_STEWARD) &&
                          (game_ptr.description.wizard < 3))
                    {

                        /* put the current game as the second */
                        second_ptr = game_ptr;
                    }
                }
                /* point to the next game */
                game_ptr = game_ptr.next_game;
            }

            /* print out the search results */
            if (first_ptr != null)
            {
                if (second_ptr != null)
                {


                    string_buffer = CFUNCTIONS.sprintfSinglestring("Highest commoners are: %s  Level: %.0lf  and  %s  Level: %.0lf\n",
                        first_ptr.description.name,
                        first_ptr.description.level,
                        second_ptr.description.name,
                        second_ptr.description.level);
                }
                else
                {


                    string_buffer = CFUNCTIONS.sprintfSinglestring("Highest commoner is: %s  Level:%.0lf\n",
                        first_ptr.description.name,
                        first_ptr.description.level);
                }

                ioclass.Do_send_line(c, string_buffer);
            }

            /* print last to die */

            /*
                if ((fp = CLibFile.fopen(_PATH_LASTDEAD,"r")) != null
                    && CLibFile.fgets(Databuf, phantdefs.SZ_DATABUF, fp) != null)
                    {
                    mvaddstr(19, 25, "The last character to die was:\n");
                    mvaddstr(20, 40 - cfunctions.strlen(Databuf) / 2,Databuf);
                    CLibFile.fclose(fp);
                    }

            */

            Do_unlock_mutex(c.realm.realm_lock);

        }


        /***************************************************************************
        / FUNCTION NAME: Do_lowercase(char *dest, char *source);
        /
        / FUNCTION: Handles error messages
        /
        / AUTHOR:  Brian Kelly, 1/1/01
        /
        / ARGUMENTS: 
        /	int error - the error code to be returned
        /	char *message - the error message to be printed
        /
        / RETURN VALUE: none
        /
        / DESCRIPTION:
        /       Process arguments, initialize program, and loop forever processing
        /       player input.
        /
        ****************************************************************************/

        internal void Do_lowercase(ref string dest, string source)
        {
            //int len, i;

            //len = cfunctions.strlen(source);

            //for (i = 0; i < len; i++)
            //{
            //    dest[i] = cfunctions.tolower(source[i]);
            //}

            //dest[len] = '\0';

            dest = source.ToLower();
            return;
        }


        /***************************************************************************
        / FUNCTION NAME: Do_create_password(char *string);
        /
        / FUNCTION: Handles error messages
        /
        / AUTHOR:  Brian Kelly, 1/1/01
        /
        / ARGUMENTS: 
        /	int error - the error code to be returned
        /	char *message - the error message to be printed
        /
        / RETURN VALUE: none
        /
        / DESCRIPTION:
        /       Process arguments, initialize program, and loop forever processing
        /       player input.
        /
        ****************************************************************************/

        internal void Do_create_password(ref char[] cstring)
        {
            int len, i, letter;
            char[] cstringchar = cstring;

            /* choose a password length */
            len = 5 * CFUNCTIONS.floor(macros.RND()) + 8;

            /* pick a random character */
            for (i = 0; i < len; i++)
            {
                letter = 50 + CFUNCTIONS.floor(macros.RND()) * 55;

                /* skip over non characters and numbers plus "01IOilo" */
                if (letter > 57)
                    letter += 7;
                if (letter > 72)
                    ++letter;
                if (letter > 78)
                    ++letter;
                if (letter > 90)
                    letter += 6;
                if (letter > 104)
                    ++letter;
                if (letter > 107)
                    ++letter;
                if (letter > 110)
                    ++letter;

                cstringchar[i] = (char)letter;
            }
            
            cstring = cstringchar;
            return;
        }


        /***************************************************************************
        / FUNCTION NAME: Do_move_close(double *x, double *y, float distance);
        /
        / FUNCTION: Handles error messages
        /
        / AUTHOR:  Brian Kelly, 1/5/01
        /
        / ARGUMENTS: 
        /	int error - the error code to be returned
        /	char *message - the error message to be printed
        /
        / RETURN VALUE: none
        /
        / DESCRIPTION:
        /       Process arguments, initialize program, and loop forever processing
        /       player input.
        /
        ****************************************************************************/

        internal void Do_move_close(ref double x, ref double y, double maxDistance)
        {
            double angle, distance;

            angle = macros.RND() * 2 * 3.14159;

            distance = macros.RND() * maxDistance;
            if (distance < 1.0)
            {
                distance = 1.0;
            }

            /* add half a point because cfunctions.floor(-3.25) = -4 */
            x += CFUNCTIONS.floor(Mathf.Cos((float)angle) * distance + .5);
            y += CFUNCTIONS.floor(Mathf.Sin((float)angle) * distance + .5);

            return;
        }

        /************************************************************************
        /
        / FUNCTION NAME: Do_replace_repetition(char *theString)
        /
        / FUNCTION: do random stuff
        /
        / AUTHOR: Brian Kelly, 1/2/01
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

        internal void Do_replace_repetition(string theString)
        {
            char previousChar, thisChar;
            char[] writePtr, readPtr;
            int count;

            if (theString[0] == '\0')
            {
                return;
            }

            writePtr = readPtr = theString.ToCharArray();

            int currchar = 0;
            while (readPtr[currchar] == ' ')
            {
                ++currchar;
            }

            currchar = 0;

            count = 1;
            previousChar = CFUNCTIONS.tolower(readPtr[currchar]);
            writePtr[currchar++] = readPtr[currchar++];

            while (currchar < readPtr.Length)
            {

                thisChar = CFUNCTIONS.tolower(readPtr[currchar]);

                /* if this is the same character */
                if (thisChar == previousChar)
                {

                    ++count;

                    /* see if this is the fourth character that's the same */
                    if (count > 4)
                    {

                        /* skip over it */
                        currchar++;
                        continue;
                    }

                }
                else if (thisChar != ' ')
                {
                    previousChar = thisChar;
                    count = 1;
                }

                /* copy the character over */
                writePtr[currchar++] = readPtr[currchar++];
            }

            /* add a null */
            writePtr[currchar] = '\0';
        }


        /***************************************************************************
        / FUNCTION NAME: Do_direction(struct client_t *c, double *x, double *y, char *direction)
        /
        / FUNCTION: Derives the direction of a target coordinate from current location
        /
        / AUTHOR:  Brian Kelly, 5/9/01
        /
        / ARGUMENTS: 
        /	
        /
        / RETURN VALUE: none
        /
        / DESCRIPTION:
        /       
        /
        ****************************************************************************/

        internal void Do_direction(client_t c, double x, double y, string direction)
        {
            double radians;

            /* if we're on the X coordinate, get radians manually */
            if (x - c.player.x == 0)
            {

                if (y - c.player.y > 0)
                {
                    radians = 1.5708;
                }
                else if (y - c.player.y < 0)
                {
                    radians = 4.7124;
                }
                else
                {

                    CFUNCTIONS.strcat(direction, "down");
                    return;
                }
            }
            else
            {

                /* find the angle */
                radians = Mathf.Atan((float)((y - c.player.y) / (x - c.player.x)));

                /* add 180 degrees if on the other side of the plane */
                if (x - c.player.x < 0)
                {
                    radians += 3.1416;
                }
            }

            /* run around the circle */
            if (radians > 4.3197)
            {

                CFUNCTIONS.strcat(direction, "south");
            }
            else if (radians > 3.5343)
            {

                CFUNCTIONS.strcat(direction, "south-west");
            }
            else if (radians > 2.7489)
            {

                CFUNCTIONS.strcat(direction, "west");
            }
            else if (radians > 1.9635)
            {

                CFUNCTIONS.strcat(direction, "north-west");
            }
            else if (radians > 1.1781)
            {

                CFUNCTIONS.strcat(direction, "north");
            }
            else if (radians > .3927)
            {

                CFUNCTIONS.strcat(direction, "north-east");
            }
            else if (radians > -.3927)
            {

                CFUNCTIONS.strcat(direction, "east");
            }
            else if (radians > -1.1781)
            {

                CFUNCTIONS.strcat(direction, "south-east");
            }
            else
            {

                CFUNCTIONS.strcat(direction, "south");
            }

            return;
        }

        internal int Do_maxmove(client_t c)
        {
            if ((c.player.circle > 19) && (c.player.circle < 36))
            {
                return (int)CFUNCTIONS.MIN(CFUNCTIONS.ceil(c.player.level / 50.0) + 1.5, 10.0);
            }
            else
            {
                return (int)CFUNCTIONS.MIN(CFUNCTIONS.floor((c.player.level * 1.5) + 1.5), 100.0);
            }
        }

        internal int Do_anglemove(client_t c)
        {
            return (int)CFUNCTIONS.MAX(1.0, CFUNCTIONS.floor(Do_maxmove(c) * .707106781));
        }

        /************************************************************************
        /
        / FUNCTION NAME: gettid()
        /
        / FUNCTION: get the child processes id
        /
        / ARGUMENTS:
        /
        /
        *************************************************************************/
        internal CLibPThread.pid_t gettid()
        {
            //CLibPThread.pid_t ret = new CLibPThread.pid_t();
            //__asm__("int $0x80" : "=a"(ret) : "0"(224)       
            ///* SYS_gettid */
            //);
            //if ((int)ret < 0)
            //{
            //    ret = -1;
            //}

            //this method is run by the new thread. its pid is already set by CLibPThread, so pull from there
            CLibPThread.pid_t ret = CLibPThread.gettid();
            return ret;
        }

    }

}
