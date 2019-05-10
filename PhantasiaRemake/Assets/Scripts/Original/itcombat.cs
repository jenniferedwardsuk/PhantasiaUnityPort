using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace phantasiaclasses
{
    public class itcombat //: MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        static itcombat Instance;
        private itcombat()
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
        public static itcombat GetInstance()
        {
            itcombat instance = null;
            if (Instance != null)
            {
                instance = Instance;
            }
            else
            {
                instance = new itcombat();
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

        /*
     * itcombat.c       Routines to handle inter-terminal combat
     */


        /***************************************************************************
        / FUNCTION NAME: Do_opponent_struct(struct client_t *c, struct opponent_t *theOpponent)
        /
        / FUNCTION: Check to see if a player entered an occupied square
        /
        / AUTHOR:  Brian Kelly, 8/16/99
        /
        / ARGUMENTS: 
        /	struct server_t s - the server strcture
        /	struct game_t this_game - pointer to the player we're checking
        /
        / RETURN VALUE: none
        /
        / DESCRIPTION:
        /       Process arguments, initialize program, and loop forever processing
        /       player input.
        /
        ****************************************************************************/

        void Do_opponent_struct(client_t c, opponent_t theOpponent)
        {
            string string_buffer = ""; //[phantdefs.SZ_LINE];

            c.battle.ring_in_use = false;
            c.battle.melee_damage = 0.0;
            c.battle.skirmish_damage = 0.0;
            c.battle.tried_luckout = false;

            CFUNCTIONS.strcpy(ref theOpponent.name, c.modifiedName);
            theOpponent.processID = c.game.clientPid;

            /* passable experience = all player exp now that nicking is capped */
            /* Kings and higher are immune to nicking */
            if (c.player.special_type >= phantdefs.SC_KING)
            {
                theOpponent.experience = 0.0;
            }
            else
            {
                theOpponent.experience = CFUNCTIONS.floor(c.player.experience /
                        (30 * c.player.level + 1));
            }

            theOpponent.strength = c.player.strength + c.player.sword;
            theOpponent.max_strength = theOpponent.strength;
            theOpponent.energy = c.player.energy;
            theOpponent.max_energy = c.player.max_energy + c.player.shield;
            theOpponent.speed = c.player.quickness + 1;
            theOpponent.max_speed = theOpponent.speed;
            theOpponent.brains = c.player.brains;
            theOpponent.size = c.player.level;

            /* the bigger they are, the harder they fall to all or nothing */
            theOpponent.sin = c.player.sin + CFUNCTIONS.floor(c.player.level / 100);

            CFUNCTIONS.sprintf(ref string_buffer,
                    "%s, %s, Level %.0lf, Sin %.0f has effective Sin of %.0lf\n",
                    c.player.lcname,
                    c.realm.charstats[c.player.type].class_name,
                    c.player.level,
                    c.player.sin,
                    theOpponent.sin);

            fileclass.Do_log(pathnames.COMBAT_LOG, string_buffer);

            theOpponent.shield = 0;
            theOpponent.special_type = phantdefs.SM_IT_COMBAT;
            theOpponent.treasure_type = 0;
            theOpponent.flock_percent = 0;

            /* throw up spells for combat */
            fightclass.Do_starting_spells(c);
        }


        /***************************************************************************
        / FUNCTION NAME: Do_setup_it_combat(struct client_t *c, struct game_t *theGame)
        /
        / FUNCTION: Check to see if a player entered an occupied square
        /
        / AUTHOR:  Brian Kelly, 8/16/99
        /
        / ARGUMENTS: 
        /	struct server_t s - the server strcture
        /	struct game_t this_game - pointer to the player we're checking
        /
        / RETURN VALUE: none
        /
        / DESCRIPTION:
        /       Process arguments, initialize program, and loop forever processing
        /       player input.
        /
        ****************************************************************************/

        internal void Do_setup_it_combat(client_t c, game_t theGame)
        {
            float ftemp;

            /* WARNING: c.realm.realm_lock is passed LOCKED! */
            /*
                miscclass.Do_lock_mutex(c.realm.realm_lock);
            */
            it_combat_t theCombat = new it_combat_t();
            event_t event_ptr = new event_t();
            it_combat_t it_combat_ptr = new it_combat_t();
            string string_buffer = ""; //[phantdefs.SZ_LINE];

            //theCombat = (it_combat_t) Do_malloc(phantdefs.SZ_IT_COMBAT);
            theCombat.opponentFlag[0] = false;
            theCombat.opponentFlag[1] = false;
            theCombat.next_opponent = null;
            theCombat.player_ptr = null;
            miscclass.Do_init_mutex(theCombat.theLock);

            c.game.it_combat = theCombat;

            if (theGame.it_combat == null)
            {

                theGame.it_combat = theCombat;
                miscclass.Do_unlock_mutex(c.realm.realm_lock);

                event_ptr = eventclass.Do_create_event();
                event_ptr.type = (short)phantdefs.IT_COMBAT_EVENT;
                event_ptr.arg4 = theCombat;
                event_ptr.to = theGame;
                event_ptr.from = c.game;


                eventclass.Do_send_event(event_ptr);

                ioclass.Do_send_clear(c);

                if (c.player.blind)
                    CFUNCTIONS.sprintf(ref string_buffer,
                    "You hear another player nearby!\n");
                else
                    CFUNCTIONS.sprintf(ref string_buffer,
                    "You see another player in the distance!\n");
            }
            else
            {

                it_combat_ptr = theGame.it_combat;

                while (it_combat_ptr.next_opponent != null)
                    it_combat_ptr = it_combat_ptr.next_opponent;

                it_combat_ptr.next_opponent = theCombat;
                miscclass.Do_unlock_mutex(c.realm.realm_lock);

                ioclass.Do_send_clear(c);

                if (c.player.blind)
                    CFUNCTIONS.sprintf(ref string_buffer,
                    "You sense that several players are nearby!\n");
                else
                    CFUNCTIONS.sprintf(ref string_buffer,
                    "You see several players in the distance!\n");
            }

            ioclass.Do_send_line(c, string_buffer);

            miscclass.Do_lock_mutex(theCombat.theLock);
            Do_opponent_struct(c, theCombat.opponent[0]);
            c.battle.opponent = theCombat.opponent[1];
            miscclass.Do_unlock_mutex(theCombat.theLock);

            /* for attacking another player, pick up sin */
            statsclass.Do_sin(c, .5);
            string_buffer = CFUNCTIONS.sprintfSinglestring("%s ATTACKED another player.\n", c.player.lcname);
            fileclass.Do_log(pathnames.BATTLE_LOG, string_buffer);

            ioclass.Do_send_line(c, "Waiting for the other player(s)...\n");

            Do_it_combat_turns(c, theCombat, theCombat.opponent[0],
                theCombat.opponentFlag[0], theCombat.opponentFlag[1]);

            return;
        }


        /***************************************************************************
        / FUNCTION NAME: Do_it_combat(struct client_t *c, struct event_t *theEvent)
        /
        / FUNCTION: Check to see if a player entered an occupied square
        /
        / AUTHOR:  Brian Kelly, 8/16/99
        /
        / ARGUMENTS: 
        /	struct server_t s - the server strcture
        /	struct game_t this_game - pointer to the player we're checking
        /
        / RETURN VALUE: none
        /
        / DESCRIPTION:
        /       Process arguments, initialize program, and loop forever processing
        /       player input.
        /
        ****************************************************************************/

        internal void Do_it_combat(client_t c, event_t theEvent, int available)
        {
            it_combat_t theCombat = new it_combat_t();
            string string_buffer = ""; //[phantdefs.SZ_LINE];

            ioclass.Do_send_clear(c);

            theCombat = (it_combat_t)theEvent.arg4;

            miscclass.Do_lock_mutex(theCombat.theLock);

            /* if our flag is set, there is a problem */
            if (!theCombat.opponentFlag[1])
            {

                theCombat.opponentFlag[0] = true;
                theCombat.opponentFlag[1] = false;

                Do_opponent_struct(c, theCombat.opponent[1]);

                if (c.player.energy <= 0)
                {

                    theCombat.message = (short)phantdefs.IT_JUST_DIED;
                    miscclass.Do_unlock_mutex(theCombat.theLock);
                    CFUNCTIONS.kill(theCombat.opponent[0].processID , LinuxLibSIG.SIGIO);
                    miscclass.Do_lock_mutex(c.realm.realm_lock);
                    c.game.it_combat = null;
                    miscclass.Do_unlock_mutex(c.realm.realm_lock);
                    return;
                }
                else if (available == 0)
                {

                    theCombat.message = (short)phantdefs.IT_JUST_LEFT;
                    miscclass.Do_unlock_mutex(theCombat.theLock);
                    CFUNCTIONS.kill(theCombat.opponent[0].processID , LinuxLibSIG.SIGIO); 
                    miscclass.Do_lock_mutex(c.realm.realm_lock);
                    c.game.it_combat = null;
                    miscclass.Do_unlock_mutex(c.realm.realm_lock);
                    return;
                }
                else
                {
                    theCombat.message = (short)phantdefs.IT_REPORT;
                }
            }

            c.battle.opponent = theCombat.opponent[0];

            if (c.player.blind)
            {
                if (c.stuck)
                {
                    c.stuck = false;
                    CFUNCTIONS.sprintf(ref string_buffer,
                   "Before you can move, you hear another player in the area!\n");
                }
                else
                {
                    CFUNCTIONS.sprintf(ref string_buffer,
                    "You hear another player approach!\n");
                }
            }
            else
            {
                if (c.stuck)
                {
                    c.stuck = false;

                    CFUNCTIONS.sprintf(ref string_buffer,
                        "Before you can move, %s enters the area!\n",
                        theCombat.opponent[0].name);
                }
                else
                {

                    CFUNCTIONS.sprintf(ref string_buffer,
                        "You see %s approaching from the distance!\n",
                        theCombat.opponent[0].name);
                }
            }

            miscclass.Do_unlock_mutex(theCombat.theLock);

            CFUNCTIONS.kill(theCombat.opponent[0].processID , LinuxLibSIG.SIGIO);

            ioclass.Do_send_line(c, string_buffer);
            socketclass.Do_send_buffer(c);
            CLibPThread.sleep(2);

            Do_it_combat_turns(c, theCombat, theCombat.opponent[1],
                theCombat.opponentFlag[1], theCombat.opponentFlag[0]);

            return;
        }


        /***************************************************************************
        / FUNCTION NAME: void Do_it_combat_turn(struct client_t *c, struct it_combat_t *theCombat, struct opponent_t *myStats, bool *my Flag, bool *hisFlag)
        /
        / FUNCTION: Check to see if a player entered an occupied square
        /
        / AUTHOR:  Brian Kelly, 8/16/99
        /
        / ARGUMENTS: 
        /	struct server_t s - the server strcture
        /	struct game_t this_game - pointer to the player we're checking
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
        void Do_it_combat_turns(client_t c, it_combat_t theCombat, opponent_t myStats, bool myFlag, bool hisFlag)
        {
            short theMessage;
            double theArg = -1;
            short newMessage;
            double newArg = -1;
            string string_buffer = ""; //[phantdefs.SZ_LINE];
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            event_t event_ptr = new event_t();
            realm_object_t object_ptr = new realm_object_t();
            float ftemp;

            for (; ; )
            {
                if (UnityGameController.StopApplication) //time to quit
                {
                    Debug.Log("Thread " + System.Threading.Thread.CurrentThread.Name + " stopping in Do_it_combat_turns");
                    return;
                }
                else
                {
                    System.Threading.Thread.Sleep(33); //30fps
                }

                c.timeoutFlag = 0;

                ioclass.Do_wait_flag(c, myFlag, theCombat.theLock);

                /* handle our flag  */
                miscclass.Do_lock_mutex(theCombat.theLock);

                /* see if the player timed out */
                if (c.timeoutFlag != 0 && theCombat.message != phantdefs.IT_BORED)
                {

                    theMessage = (short)phantdefs.IT_ABANDON;
                }
                else
                {
                    theMessage = theCombat.message;
                    theArg = theCombat.arg1;

                    myFlag = false;
                }

                /* unlock the flags so the other thread can check it  */
                miscclass.Do_unlock_mutex(theCombat.theLock);

                /* if the other thread received an event, we're server */
                if (theMessage == phantdefs.IT_REPORT)
                {

                    if (c.player.blind)


                        CFUNCTIONS.sprintf(ref string_buffer,
                        "You have encountered another player.\n");

                    else
                    {

                        string_buffer = CFUNCTIONS.sprintfSinglestring("You have encountered %s.\n",
                            c.battle.opponent.name);


                        ioclass.Do_send_line(c, string_buffer);
                    }

                    /* defender always attacks first */
                    theMessage = (short)phantdefs.IT_DEFEND;
                }

                /* battle continues, decide who goes next */
                if (theMessage == phantdefs.IT_CONTINUE)
                {

                    if (macros.RND() * c.battle.opponent.speed > macros.RND() *

                        myStats.speed)
                    {

                        theMessage = (short)phantdefs.IT_DEFEND;
                    }
                    else
                        theMessage = (short)phantdefs.IT_ECHO;
                }

                /* if the opponent just left the game */
                if (theMessage == phantdefs.IT_JUST_LEFT)
                {


                    CFUNCTIONS.sprintf(ref string_buffer,
                        "You arrive just to see %s vanish from the realm.\n",
                        c.battle.opponent.name);


                    ioclass.Do_send_line(c, string_buffer);

                    c.battle.opponent.experience = 0.0;
                    theMessage = (short)phantdefs.IT_DEFEAT;
                }

                /* if the opponent passes on combat */
                if (theMessage == phantdefs.IT_JUST_DIED)
                {


                    string_buffer = CFUNCTIONS.sprintfSinglestring("%s falls dead at your feet.\n",
                        c.battle.opponent.name);


                    ioclass.Do_send_line(c, string_buffer);

                    c.battle.opponent.experience = 0.0;
                    theMessage = (short)phantdefs.IT_DEFEAT;
                }

                /* if the opponent passes on combat */
                if (theMessage == phantdefs.IT_BORED)
                {


                    CFUNCTIONS.sprintf(ref string_buffer,
                        "%s became bored and left the battlefield.\n",
                        c.battle.opponent.name);


                    ioclass.Do_send_line(c, string_buffer);

                    c.battle.opponent.experience = 0.0;
                    theMessage = (short)phantdefs.IT_DEFEAT;
                }

                /* if the opponent is having network trouble */
                if (theMessage == phantdefs.IT_CONCEDE)
                {


                    string_buffer = CFUNCTIONS.sprintfSinglestring("%s concedes the battle.\n",
                        c.battle.opponent.name);


                    ioclass.Do_send_line(c, string_buffer);

                    theMessage = (short)phantdefs.IT_DEFEAT;
                }

                newMessage = (short)phantdefs.IT_CONTINUE;
                string_buffer = "\0";

                switch (theMessage)
                {

                    /* we've encountered a problem - every thread for himself */
                    case phantdefs.IT_ABANDON:

                        error_msg = CFUNCTIONS.sprintfSinglestring(
                        "[%s] now abandoning it_combat in Do_it_combat_turns.\n",
                                c.connection_id);

                        fileclass.Do_log_error(error_msg);

                        /* leave a message for our opponent */
                        theCombat.message = (short)phantdefs.IT_BORED;

                        miscclass.Do_lock_mutex(theCombat.theLock);

                        hisFlag = true;

                        miscclass.Do_unlock_mutex(theCombat.theLock);

                        CFUNCTIONS.kill(c.battle.opponent.processID, LinuxLibSIG.SIGIO);

                        /* clear any spells */
                        statsclass.Do_energy(c, c.player.energy, c.player.max_energy,
                                c.player.shield, 0.0, 0);

                        statsclass.Do_strength(c, c.player.max_strength, c.player.sword, 0, 0);

                        statsclass.Do_speed(c, c.player.max_quickness, c.player.quicksilver, 0,
                                0);

                        /* pull ourselves out of combat */
                        miscclass.Do_lock_mutex(c.realm.realm_lock);
                        c.game.it_combat = null;
                        c.game.virtualvirtual = true;
                        miscclass.Do_unlock_mutex(c.realm.realm_lock);

                        /* get away from this square and this battle */
                        event_ptr = eventclass.Do_create_event();
                        event_ptr.to = c.game;
                        event_ptr.from = c.game;
                        event_ptr.type = (short)phantdefs.MOVE_EVENT;
                        event_ptr.arg3 = phantdefs.A_NEAR;
                        eventclass.Do_file_event(c, event_ptr);

                        return;

                    /* we decided to attack, so have the opponent echo an attack */
                    case phantdefs.IT_ECHO:

                        newMessage = (short)phantdefs.IT_DEFEND;
                        break;

                    /* we are to attack */
                    case phantdefs.IT_ATTACK:

                        /* give the character fatigue */
                        ++c.battle.rounds;


                        statsclass.Do_speed(c, c.player.max_quickness, c.player.quicksilver,
                                c.battle.speedSpell, 0);

                        myStats.speed = c.player.quickness;

                        /* if the player has a ring in use */
                        if (c.battle.ring_in_use)
                        {

                            /* age ring */
                            if (c.player.ring_type != phantdefs.R_DLREG)
                            {
                                --c.player.ring_duration;
                            }

                            /* regardless of ring type, heal the character */
                            statsclass.Do_energy(c, c.player.max_energy + c.player.shield,
                                                            c.player.max_energy, c.player.shield,
                                                            c.battle.force_field, 0);
                        }


                        ioclass.Do_send_line(c,
                                                "You attack this round.  Please choose your attack:\n");


                        fightclass.Do_playerhits(c, newMessage, newArg);

                        /* if the player put up a shield, copy the info over */
                        if (newMessage == phantdefs.IT_SHIELD)
                        {
                            myStats.shield = c.battle.force_field;
                        }

                        break;

                    /* we are to defend */
                    case phantdefs.IT_DEFEND:


                        CFUNCTIONS.sprintf(ref string_buffer,
                           "%s attacks for this round.  Please wait for a decision.\n",
                           c.battle.opponent.name);

                        newMessage = (short)phantdefs.IT_ATTACK;
                        break;

                    /* the other thread is declaring victory */
                    case phantdefs.IT_VICTORY:

                        newMessage = (short)phantdefs.IT_DEFEAT;

                        /* clear any spells */
                        statsclass.Do_energy(c, c.player.energy, c.player.max_energy,
                            c.player.shield, 0.0, 0);


                        statsclass.Do_strength(c, c.player.max_strength, c.player.sword, 0, 0);


                        statsclass.Do_speed(c, c.player.max_quickness, c.player.quicksilver, 0, 0);

                        c.battle.force_field = 0;
                        c.battle.strengthSpell = 0;
                        c.battle.speedSpell = 0;

                        break;

                    /* the other thread stands defeated - we clean up */
                    case phantdefs.IT_DEFEAT:

                        /* award experience if there is any to be had */
                        if (c.battle.opponent.experience != 0)
                        {

                            statsclass.Do_experience(c, c.battle.opponent.experience, 0);

                            /* for killing another player, pick up sin */
                            statsclass.Do_sin(c, CFUNCTIONS.sqrt(c.battle.opponent.size) / 3.1);

                            error_msg = CFUNCTIONS.sprintfSinglestring("%s KILLED %s in IT combat.\n", c.player.lcname,
                            c.battle.opponent.name);
                            fileclass.Do_log(pathnames.BATTLE_LOG, error_msg);

                        }

                        /* clear any spells */
                        statsclass.Do_energy(c, c.player.energy, c.player.max_energy,
                            c.player.shield, 0.0, 0);


                        statsclass.Do_strength(c, c.player.max_strength, c.player.sword, 0, 0);


                        statsclass.Do_speed(c, c.player.max_quickness, c.player.quicksilver, 0, 0);

                        c.battle.force_field = 0;
                        c.battle.strengthSpell = 0;
                        c.battle.speedSpell = 0;

                        /* if we're in the throne room and are not king */
                        /* become king NOW (another battle may be pending) */
                        if (c.player.location == phantdefs.PL_THRONE && c.player.special_type !=
                            phantdefs.SC_KING && c.player.special_type != phantdefs.SC_STEWARD)
                        {

                            /* become steward */
                            if (c.player.level >= 10 && c.player.level < 200)
                            {

                                commandsclass.Do_steward(c);
                            }
                            else if (c.player.level >= 1000 && c.player.level < 2000)
                            {

                                commandsclass.Do_king(c);
                            }
                        }


                        miscclass.Do_lock_mutex(c.realm.realm_lock);

                        /* if there are no more battles to fight */
                        if (theCombat.next_opponent == null)
                        {

                            /* pull ourselves out of combat */
                            c.game.it_combat = null;

                            miscclass.Do_unlock_mutex(c.realm.realm_lock);

                            /* create a body event for ourselves */
                            /* opponent put player struct here if he will die */
                            if (theCombat.player_ptr != null)
                            {
                                event_ptr = eventclass.Do_create_event();
                                event_ptr.to = c.game;
                                event_ptr.from = c.game;
                                event_ptr.type = (short)phantdefs.CORPSE_EVENT;

                                /* the body can be cursed */
                                event_ptr.arg1 = 1;
                                event_ptr.arg4 = theCombat.player_ptr;

                                eventclass.Do_file_event(c, event_ptr);
                            }
                        }

                        /* if there are more opponents to fight */
                        else
                        {
                            event_ptr = eventclass.Do_create_event();
                            event_ptr.to = c.game;
                            event_ptr.from = c.game;
                            event_ptr.type = (short)phantdefs.IT_COMBAT_EVENT;
                            event_ptr.arg4 = theCombat.next_opponent;

                            eventclass.Do_file_event(c, event_ptr);

                            /* put ourselves back in combat */
                            c.game.it_combat = theCombat.next_opponent;

                            /* create a realm object for this kill, if necessary */
                            if (theCombat.player_ptr != null)
                            {

                                object_ptr = new realm_object_t();// (realm_object_t) Do_malloc(phantdefs.SZ_REALM_OBJECT);

                                object_ptr.x = c.player.x;
                                object_ptr.y = c.player.y;
                                object_ptr.type = phantdefs.CORPSE;
                                object_ptr.arg1 = theCombat.player_ptr;
                                object_ptr.next_object = c.realm.objects;
                                c.realm.objects = object_ptr;
                            }


                            miscclass.Do_unlock_mutex(c.realm.realm_lock);
                        }


                        miscclass.Do_unlock_mutex(theCombat.theLock);

                        miscclass.Do_destroy_mutex(theCombat.theLock);

                        theCombat = null;// free((void*) theCombat);


                        ioclass.Do_send_line(c, "You are victorious!\n");

                        ioclass.Do_more(c);

                        ioclass.Do_send_clear(c);

                        return;

                    case phantdefs.IT_MELEE:

                        CFUNCTIONS.sprintf(ref string_buffer,
                            "%s melees with you hitting %.0f times!\n",
                            c.battle.opponent.name, theArg);


                        statsclass.Do_energy(c, myStats.energy, c.player.max_energy,
                            c.player.shield, myStats.shield, 0);

                        break;

                    case phantdefs.IT_SKIRMISH:


                        CFUNCTIONS.sprintf(ref string_buffer,
                            "%s skirmishes with you hitting %.0f times!\n",
                            c.battle.opponent.name, theArg);


                        statsclass.Do_energy(c, myStats.energy, c.player.max_energy,
                            c.player.shield, myStats.shield, 0);

                        break;

                    case phantdefs.IT_NICKED:

                        c.battle.opponent.experience += theArg;
                        theArg *= 30.0 * c.player.level + 1.0;


                        CFUNCTIONS.sprintf(ref string_buffer,
                                                "%s nicks you taking %.0f experience!\n",
                                                c.battle.opponent.name, theArg);


                        statsclass.Do_energy(c, myStats.energy, c.player.max_energy,
                            c.player.shield, myStats.shield, 0);


                        statsclass.Do_speed(c, c.player.max_quickness, c.player.quicksilver,
                            c.battle.speedSpell + 2, 0);

                        /* only subtract positive values, characters managed to
                            get negative experience */

                        if (theArg > 0)
                        {
                            c.player.experience -= theArg;
                        }

                        break;

                    case phantdefs.IT_EVADED:


                        string_buffer = CFUNCTIONS.sprintfSinglestring("%s flees the battle!\n",
                            c.battle.opponent.name);

                        c.battle.opponent.experience = 0;
                        newMessage = (short)phantdefs.IT_VICTORY;
                        break;

                    case phantdefs.IT_WIZEVADE:


                        string_buffer = CFUNCTIONS.sprintfSinglestring("%s laughs at your pitiful attack and teleports away!\n",
                            c.battle.opponent.name);

                        c.battle.opponent.experience = 0;
                        newMessage = (short)phantdefs.IT_VICTORY;
                        break;

                    case phantdefs.IT_NO_EVADE:


                        CFUNCTIONS.sprintf(ref string_buffer,
                            "%s tried to flee, but couldn't shake you!\n",
                            c.battle.opponent.name);

                        break;

                    case phantdefs.IT_LUCKOUT:


                        string_buffer = CFUNCTIONS.sprintfSinglestring("%s successfully lucks out in the battle!\n",
                            c.battle.opponent.name);


                        statsclass.Do_energy(c, myStats.energy, c.player.max_energy,
                            c.player.shield, myStats.shield, 0);

                        break;

                    case phantdefs.IT_NO_LUCKOUT:


                        CFUNCTIONS.sprintf(ref string_buffer,
                            "%s failed to luckout in the battle!\n",
                            c.battle.opponent.name);

                        break;

                    case phantdefs.IT_RING:


                        string_buffer = CFUNCTIONS.sprintfSinglestring("%s puts on a ring!\n",
                            c.battle.opponent.name);

                        break;

                    case phantdefs.IT_NO_RING:


                        string_buffer = CFUNCTIONS.sprintfSinglestring("%s searches in vain for a ring!\n",
                            c.battle.opponent.name);

                        break;

                    case phantdefs.IT_ALL_OR_NOT:


                        CFUNCTIONS.sprintf(ref string_buffer,
                            "%s casts All or Nothing, blasting you for %.0f damage!\n",
                            c.battle.opponent.name, theArg);


                        statsclass.Do_energy(c, myStats.energy, c.player.max_energy,
                            c.player.shield, myStats.shield, 0);

                        break;

                    case phantdefs.IT_NO_ALL_OR_NOT:


                        CFUNCTIONS.sprintf(ref string_buffer,
                            "%s flubs an attempt to cast All or Nothing!\n",
                            c.battle.opponent.name);

                        break;

                    case phantdefs.IT_BOLT:


                        CFUNCTIONS.sprintf(ref string_buffer,
                            "%s hit you with a %.0f damage Mana Bolt!\n",
                            c.battle.opponent.name, theArg);


                        statsclass.Do_energy(c, myStats.energy, c.player.max_energy,
                            c.player.shield, myStats.shield, 0);

                        break;

                    case phantdefs.IT_NO_BOLT:


                        CFUNCTIONS.sprintf(ref string_buffer,
                            "%s casts a Mana Bolt that peters out!\n",
                            c.battle.opponent.name);

                        break;

                    case phantdefs.IT_SHIELD:


                        string_buffer = CFUNCTIONS.sprintfSinglestring("%s throws up a Force Shield!\n",
                            c.battle.opponent.name);

                        break;

                    case phantdefs.IT_NO_SHIELD:


                        string_buffer = CFUNCTIONS.sprintfSinglestring("%s messes up a Force Shield spell!\n",
                            c.battle.opponent.name);

                        break;

                    case phantdefs.IT_TRANSFORM:


                        miscclass.Do_lock_mutex(c.realm.monster_lock);


                        CFUNCTIONS.sprintf(ref string_buffer,
                            "%s casts Transform, turning you into %s!\n",
                            c.battle.opponent.name,
                            c.realm.monster[(int)theArg].name);

                        event_ptr = eventclass.Do_create_event();
                        event_ptr.type = (short)phantdefs.DEATH_EVENT;
                        event_ptr.arg1 = theArg;
                        event_ptr.arg3 = phantdefs.K_TRANSFORMED;


                        miscclass.Do_unlock_mutex(c.realm.monster_lock);

                        eventclass.Do_handle_event(c, event_ptr);

                        newMessage = (short)phantdefs.IT_DEFEAT;
                        break;

                    case phantdefs.IT_NO_TRANSFORM:


                        string_buffer = CFUNCTIONS.sprintfSinglestring("%s fails to cast a Transform spell!\n",
                            c.battle.opponent.name);

                        break;

                    case phantdefs.IT_TRANSFORM_BACK:


                        CFUNCTIONS.sprintf(ref string_buffer,
                            "%s's Transform spell backfires!  %s turns into %s!\n",
                            c.battle.opponent.name, c.battle.opponent.name,
                            c.realm.monster[(int)theArg].name);


                        fightclass.Do_cancel_monster(c.battle.opponent);
                        newMessage = (short)phantdefs.IT_VICTORY;
                        break;

                    case phantdefs.IT_MIGHT:


                        CFUNCTIONS.sprintf(ref string_buffer,
                            "%s buffs up with an Increase Might spell!\n",
                            c.battle.opponent.name);

                        c.battle.opponent.strength += c.player.max_strength;

                        break;

                    case phantdefs.IT_NO_MIGHT:


                        CFUNCTIONS.sprintf(ref string_buffer,
                            "%s flounders with an Increase Might spell!\n",
                            c.battle.opponent.name);

                        break;

                    case phantdefs.IT_HASTE:


                        CFUNCTIONS.sprintf(ref string_buffer,
                            "%s casts a Haste spell and speeds up!\n",
                            c.battle.opponent.name);

                        c.battle.opponent.strength += c.player.max_quickness;

                        break;

                    case phantdefs.IT_NO_HASTE:


                        CFUNCTIONS.sprintf(ref string_buffer,
                            "%s casts an ineffective Haste spell!\n",
                            c.battle.opponent.name);

                        break;

                    case phantdefs.IT_TRANSPORT:


                        CFUNCTIONS.sprintf(ref string_buffer,
                            "%s sends you away with a Transport spell!\n",
                            c.battle.opponent.name);

                        event_ptr = eventclass.Do_create_event();
                        event_ptr.type = (short)phantdefs.MOVE_EVENT;
                        event_ptr.arg3 = phantdefs.A_FAR;

                        eventclass.Do_file_event(c, event_ptr);

                        newMessage = phantdefs.IT_DEFEAT;
                        break;

                    case phantdefs.IT_NO_TRANSPORT:


                        CFUNCTIONS.sprintf(ref string_buffer,
                            "%s misfires a Transport spell!\n",
                            c.battle.opponent.name);

                        break;

                    case phantdefs.IT_TRANSPORT_BACK:


                        CFUNCTIONS.sprintf(ref string_buffer,
                            "%s is transported away from a bounced Transport spell!\n",
                            c.battle.opponent.name);


                        fightclass.Do_cancel_monster(c.battle.opponent);
                        newMessage = (short)phantdefs.IT_VICTORY;
                        break;

                    case phantdefs.IT_PARALYZE:


                        CFUNCTIONS.sprintf(ref string_buffer,
                            "%s casts a Paralyze spell holding you in place!\n",
                            c.battle.opponent.name);

                        break;

                    case phantdefs.IT_NO_PARALYZE:


                        string_buffer = CFUNCTIONS.sprintfSinglestring("%s casts a dud Paralyze spell!\n",
                            c.battle.opponent.name);

                        break;

                    case phantdefs.IT_PASS:


                        string_buffer = CFUNCTIONS.sprintfSinglestring("%s whistles a happy tune.\n",
                            c.battle.opponent.name);

                        break;

                    default:


                        error_msg = CFUNCTIONS.sprintfSinglestring(
                            "[%s] Unknown combat message %d in Do_it_combat_turn.\n",
                            c.connection_id, theMessage);


                        fileclass.Do_log_error(error_msg);

                        theCombat.message = phantdefs.IT_ABANDON;
                        miscclass.Do_lock_mutex(theCombat.theLock);
                        hisFlag = true;
                        miscclass.Do_unlock_mutex(theCombat.theLock);

                        CFUNCTIONS.kill(c.battle.opponent.processID, LinuxLibSIG.SIGIO);

                        miscclass.Do_lock_mutex(c.realm.realm_lock);
                        c.game.it_combat = null;
                        c.game.virtualvirtual = true;
                        miscclass.Do_unlock_mutex(c.realm.realm_lock);
                        c.run_level = phantdefs.EXIT_THREAD;
                        return;
                }

                if (CFUNCTIONS.strlen(string_buffer) != 0)

                    ioclass.Do_send_line(c, string_buffer);

                /* if the character has died */
                if (c.player.energy <= 0.0)
                {

                    newMessage = (short)phantdefs.IT_DEFEAT;
                    event_ptr = eventclass.Do_create_event();
                    event_ptr.type = (short)phantdefs.DEATH_EVENT;
                    event_ptr.arg3 = phantdefs.K_IT_COMBAT;

                    event_ptr.arg4 = "";// (void*) Do_malloc(cfunctions.strlen(c.battle.opponent.name) + 1);
                    
                    string arg4 = (string)event_ptr.arg4;
                    CFUNCTIONS.strcpy(ref arg4, c.battle.opponent.name);
                    event_ptr.arg4 = arg4;

                    /* if we leave a corpse */
                    if (c.player.level < 3000 && c.player.special_type != phantdefs.SC_KING &&

                        (!c.battle.ring_in_use || !(c.player.ring_type ==
                        phantdefs.R_DLREG || c.player.ring_type == phantdefs.R_NAZREG)))
                    {

                        /* create a copy of ourselves for the corpse */
                        theCombat.player_ptr = (player_t)characterclass.Do_copy_record(c.player);//, 0);
                    }


                    eventclass.Do_handle_event(c, event_ptr);
                }

                /* if we lost, remove us from battle completely */
                /* otherwise the winner could encounter us again before we move */
                if (newMessage == phantdefs.IT_DEFEAT || newMessage == phantdefs.IT_EVADED)
                {


                    miscclass.Do_lock_mutex(c.realm.realm_lock);
                    c.game.it_combat = null;
                    c.game.virtualvirtual = true;

                    miscclass.Do_unlock_mutex(c.realm.realm_lock);
                }

                /* send off the new message */
                theCombat.message = newMessage;
                theCombat.arg1 = newArg;

                miscclass.Do_lock_mutex(theCombat.theLock);

                hisFlag = true;

                miscclass.Do_unlock_mutex(theCombat.theLock);


                CFUNCTIONS.kill(c.battle.opponent.processID, LinuxLibSIG.SIGIO);

                if (c.player.poison > 0.0)
                    myStats.energy = c.player.energy;

                if (newMessage == phantdefs.IT_DEFEAT)
                {


                    eventclass.Do_orphan_events(c);

                    ioclass.Do_more(c);

                    ioclass.Do_send_clear(c);
                    return;
                }
            }
        }

    }
}
