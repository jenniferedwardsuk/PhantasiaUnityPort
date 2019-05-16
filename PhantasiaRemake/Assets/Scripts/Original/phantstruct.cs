using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class phantstruct //: MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}

namespace phantasiaclasses
{
    /*
     * phantstruct.h - structure definitions for Phantasia
     */

    [Serializable]
    public class player_t     /* player attributes */
    {
        /* player information */
        internal string name;  //[phantdefs.SZ_NAME];         /* player name */
        internal string lcname;  //[phantdefs.SZ_NAME];   /* player name in lowercase */
        internal char[] password;//;// = new char[16];     /* password hash */

        /* player location */
        internal double x;           /* x coord */
        internal double y;           /* y coord */
        internal string area;  //[phantdefs.SZ_AREA];     /* name of the area player is in */
        internal double circle;          /* current circle player is in */
        internal bool beyond;            /* is the character past no return */
        internal short location;     /* codes for special all locations */

        /* player stats */
        internal double experience;      /* experience */
        internal double level;           /* level */

        internal double strength;        /* strength */
        internal double max_strength;        /* maximum strength */
        internal double energy;          /* energy */
        internal double max_energy;      /* maximum energy */
        internal float quickness;        /* quickness */
        internal float max_quickness;        /* quickness */
        internal double mana;            /* mana */
        internal double brains;          /* brains */
        internal double magiclvl;        /* magic level */
        internal float poison;           /* poison */
        internal float sin;          /* sin */
        internal bool gender;            /* player is male/female */
        internal short lives;            /* multiple lives for council, valar */
        internal int age;            /* age of player */
        internal int degenerated;        /* age/3000 last degenerated */

        /* player status */
        internal short type;         /* character type */
        internal short special_type;     /* special character type */
        internal bool cloaked;       /* is character cloaked */
        internal bool blind;         /* blindness */
        internal short shield_nf;        /* does player get shield next battle */
        internal short haste_nf;     /* will the player be hasted */
        internal short strong_nf;        /* does player get a strength bonus */

        /* player currency */
        internal double gold;            /* gold */
        internal double gems;            /* gems */

        /* player equipment */
        internal double sword;           /* sword */
        internal double shield;          /* shield */
        internal float quicksilver;      /* quicksilver */
        internal int holywater;      /* holy water */
        internal int amulets;        /* amulets */
        internal int charms;         /* charms */
        internal short crowns;           /* crowns */
        internal bool virgin;            /* virgin */
        internal bool palantir;      /* palantir */
        internal bool blessing;      /* blessing */

        internal short ring_type;        /* type of ring */
        internal int ring_duration;      /* duration of ring */

        /* creation information */
        internal string parent_account;  //[phantdefs.SZ_NAME];   /* created by this account */
        internal string parent_network;  //[phantdefs.SZ_FROM];   /* created from this address */
        internal int date_created;        /* created at this time */
        internal bool faithful;      /* may other accounts load? */

        /* current or previous user information */
        internal string last_IP;  //[phantdefs.SZ_FROM];  /* last IP or DNS address */
        internal string last_account;  //[phantdefs.SZ_NAME]; /* last account accessed */
        internal int last_load;       /* time last accessed */
        internal int last_reset;      /* time password was last reset */
        internal int load_count;     /* times character has been loaded */
        internal int time_played;     /* seconds character has been played */

        /* hack foilers */
        internal int bad_passwords;      /* unsuccessful load attempts */
        internal int muteCount;      /* times caught spamming */
        internal int lastMute;        /* last instance spammed */

        /* purgatory options */
        internal bool purgatoryFlag;
        internal short monsterNumber;

        public player_t()
        {
            password = new char[16];
        }
        public player_t(player_t orig)
        {
            /* player information */
             name = orig.name;       /* player name */
             lcname = orig.lcname;   /* player name in lowercase */
             password = orig.password;     /* password hash */

            /* player location */
             x = orig.x;           /* x coord */
             y = orig.y;           /* y coord */
             area = orig.area;    /* name of the area player is in */
             circle = orig.circle;          /* current circle player is in */
             beyond = orig.beyond;            /* is the character past no return */
             location = orig.location;     /* codes for special all locations */

            /* player stats */
             experience = orig.experience;      /* experience */
             level = orig.level;           /* level */

             strength = orig.strength;        /* strength */
             max_strength = orig.max_strength;        /* maximum strength */
             energy = orig.energy;          /* energy */
             max_energy = orig.max_energy;      /* maximum energy */
             quickness = orig.quickness;        /* quickness */
             max_quickness = orig.max_quickness;        /* quickness */
             mana = orig.mana;            /* mana */
             brains = orig.brains;          /* brains */
             magiclvl = orig.magiclvl;        /* magic level */
             poison = orig.poison;           /* poison */
             sin = orig.sin;          /* sin */
             gender = orig.gender;            /* player is male/female */
             lives = orig.lives;            /* multiple lives for council, valar */
             age = orig.age;            /* age of player */
             degenerated = orig.degenerated;        /* age/3000 last degenerated */

            /* player status */
             type = orig.type;         /* character type */
             special_type = orig.special_type;     /* special character type */
             cloaked = orig.cloaked;       /* is character cloaked */
             blind = orig.blind;         /* blindness */
             shield_nf = orig.shield_nf;        /* does player get shield next battle */
             haste_nf = orig.haste_nf;     /* will the player be hasted */
             strong_nf = orig.strong_nf;        /* does player get a strength bonus */

            /* player currency */
             gold = orig.gold;            /* gold */
             gems = orig.gems;            /* gems */

            /* player equipment */
             sword = orig.sword;           /* sword */
             shield = orig.shield;          /* shield */
             quicksilver = orig.quicksilver;      /* quicksilver */
             holywater = orig.holywater;      /* holy water */
             amulets = orig.amulets;        /* amulets */
             charms = orig.charms;         /* charms */
             crowns = orig.crowns;           /* crowns */
             virgin = orig.virgin;            /* virgin */
             palantir = orig.palantir;      /* palantir */
             blessing = orig.blessing;      /* blessing */

             ring_type = orig.ring_type;        /* type of ring */
             ring_duration = orig.ring_duration;      /* duration of ring */

            /* creation information */
             parent_account = orig.parent_account;   /* created by this account */
             parent_network = orig.parent_network;;   /* created from this address */
             date_created = orig.date_created;        /* created at this time */
             faithful = orig.faithful;      /* may other accounts load? */

            /* current or previous user information */
             last_IP = orig.last_IP;   /* last IP or DNS address */
             last_account = orig.last_account; /* last account accessed */
             last_load = orig.last_load;       /* time last accessed */
             last_reset = orig.last_reset;      /* time password was last reset */
             load_count = orig.load_count;     /* times character has been loaded */
             time_played = orig.time_played;     /* seconds character has been played */

            /* hack foilers */
             bad_passwords = orig.bad_passwords;      /* unsuccessful load attempts */
             muteCount = orig.muteCount;      /* times caught spamming */
             lastMute = orig.lastMute;        /* last instance spammed */

            /* purgatory options */
             purgatoryFlag = orig.purgatoryFlag;
             monsterNumber = orig.monsterNumber;
        }
    };


    internal class player_mod_t /* information to modify an account */
    {
        /* new name */
        internal bool newName;       /* is there a new name? */
        internal string name;  //[phantdefs.SZ_NAME];         /* character's new name */
        internal string lcName;  //[phantdefs.SZ_NAME];       /* character's new name lowercase */

        /* new password */
        internal bool newPassword;       /* is there a new password? */
        internal char[] password;//;// = new char[16];     /* hash of the new password */
        internal bool passwordReset;     /* is the password being reset? */

        /* sharing permissions */
        internal bool newPermissions;        /* are we sending a perm change? */
        internal bool faithful;      /* is the character faithful */

        /* info updates */
        internal bool badPassword;       /* load attempt */

        internal player_mod_t() //initialise arrays ready for data loads
        {
            password = new char[16];
        }
    };


    internal class opponent_t   /* opponent attributes for battle */
    {
        /* opponent information */
        internal string name;  //[phantdefs.SZ_MONSTER_NAME]; /* opponent name */
        internal string realName;  //[phantdefs.SZ_MONSTER_NAME]; /* opponent's real name */
        internal short type;         /* opponent type */
        internal int processID;      /* IT combat opponent process ID */

        /* opponent stats */
        internal double experience;      /* experience */
        internal double strength;        /* strength */
        internal double max_strength;        /* maximum strength */
        internal double energy;          /* energy */
        internal double max_energy;      /* maximum energy */
        internal float speed;            /* speed */
        internal float max_speed;        /* maximum speed */
        internal double brains;          /* brains */
        internal double size;            /* monster size */
        internal float sin;                    /* sin */

        /* battle info */
        internal double shield;          /* shield */
        internal short special_type;     /* special monster flag */
        internal short treasure_type;        /* treasure type */
        internal short flock_percent;        /* percent chance of flocking */
    };

    internal class battle_t     /* battle information */
    {
        /* player information */
        internal double force_field;     /* force field */
        internal double strengthSpell;       /* strength bonus */
        internal double speedSpell;      /* speed bonus */
        internal bool ring_in_use;       /* ring flag */

        /* opponent information */
        internal opponent_t opponent;	/* pointer to opponent info */

        /* battle information */
        internal double melee_damage;        /* damage done in melee */
        internal double skirmish_damage; /* damage done in skirmish */
        internal bool tried_luckout;     /* luckout flag */
        internal int rounds;         /* count of player attacks */
        internal int timeouts;       /* count of player timeouts */
    };

    internal class it_combat_t
    {
        internal CLibPThread.pthread_mutex_t theLock;

        internal opponent_t[] opponent;	/* structures to attack */
        internal bool[] opponentFlag;
        internal short message;
        internal double arg1;
        internal player_t player_ptr;

        /* other battles */
        internal it_combat_t next_opponent;	/* pointer to next player to fight */

        internal it_combat_t() //initialise arrays ready for data loads
        {
            opponent = new opponent_t[2];
            for (int i = 0; i < opponent.Length; i++)
            {
                opponent[i] = new opponent_t();

            }
            opponentFlag = new bool[2];
        }
    };

    internal class client_t : object     /* structure for client variables */
    {
        /* socket variables */
        internal int socket;         /* the client-player socket */
        internal bool socket_up;     /* flag if socket is active */
        internal LinuxLibSocket.sockaddr_in address;		/* player address */   //from #include <netinet/in.h>
        internal string out_buffer = new string(new char[phantdefs.SZ_OUT_BUFFER]); /* storage for outgoing messages */
        internal uint out_buffer_size;     /* number of bytes in out_buffer */
        internal string in_buffer = new string(new char[phantdefs.SZ_IN_BUFFER]);   /* storage for outgoing messages */
        internal uint in_buffer_size;      /* number of bytes in in_buffer */

        /* connection information */
        internal string IP;  //[phantdefs.SZ_FROM];           /* IP or DNS of player */
        internal string network;  //[phantdefs.SZ_FROM];      /* IP or DNS of player's network */
        internal bool addressResolved;       /* is the address a DNS entry? */
        internal string connection_id;  //[phantdefs.SZ_FROM + 15];   /* IP or DNS : process id */
        internal long machineID;         /* Cookie assigned to system */
        internal short run_level;            /* area of game player is in */
        internal string modifiedName;  //[phantdefs.SZ_NAME];     /* tagged name */

        /* account information */
        internal string account;  //[phantdefs.SZ_NAME];      /* account logged in as */
        internal string lcaccount;  //[phantdefs.SZ_NAME];        /* account in lowercase */
        internal string wizaccount;  //[phantdefs.SZ_NAME];       /* wizard account for backdoor */
        internal string wizIP;  //[phantdefs.SZ_FROM];        /* wizard IP for backdoor */
        internal string previousName;  //[phantdefs.SZ_NAME];     /* saves the name when killed */
        internal string email;  //[phantdefs.SZ_FROM];        /* player e-mail from account info */
        internal string parentNetwork;  //[phantdefs.SZ_FROM];    /* network of the account */
        internal int date_connected;      /* time this connection started */

        /* client variables */
        internal int channel;        /* what chat channel is being used */
        internal int timeout;        /* current time player has to act */
        internal int timeoutAt;      /* current time player has to act */
        internal short timeoutFlag;      /* is a timeout response expected? */
        internal double knightEnergy;        /* energy bonus for knighthood */
        internal float knightQuickness;  /* quickness bonus for kinghthood */
        internal double morgothCount;        /* time since the last Morgoth defeated */
        internal int ageCount;           /* one ring counter */
        internal short wizard;           /* administrator level */
        internal bool broadcast;     /* broadcast next chat? */
        internal bool stuck;         /* did the player stay for it-combat */
        internal bool suspended;     /* hold game and only hear wizards */
        internal int muteUntil;       /* time after which player can chat */
        internal int tagUntil;        /* time the player's name tag disappears */
        internal bool hearBroadcasts;        /* are server messages posted? */
        internal bool accountLoaded;     /* do we have an account? */
        internal bool characterLoaded;   /* do we have a character? */
        internal bool characterAnnounced;    /* has the character been accounced? */

        /* hack information */
        internal int[] chatTimes;
        internal int[] chatLength;
        internal short swearCount;

        /* directory to important information */
        internal player_t player;		/* the main player structure */
        internal battle_t battle;		/* structure used for battles */
        internal event_t events;		/* pointer to character events */
        internal realm_t realm;		/* pointer to server realm variables */
        internal game_t game;		/* pointer to game's game_t struct */

        internal client_t()
        { 
            //initialise arrays ready for data loads
            chatTimes = new int[10];
            chatLength = new int[10];

            //initialise objects where needed
            battle = new battle_t();
            address = new LinuxLibSocket.sockaddr_in();
            player = new player_t();
        }
    };

    internal class player_desc_t    /* common information about a player */
    {
        /* Player information */
        internal string name;  //[phantdefs.SZ_NAME];     /* name of the character */
        internal string lcname; //[phantdefs.SZ_NAME];   /* lowercase character name */
        internal string parent_account; //[phantdefs.SZ_NAME]; /* character parent account */
        internal short type;         /* character class */
        internal short special_type;     /* special character type */
        internal bool gender;            /* is the character phantdefs.MALE or female */
        internal double level;           /* the character's current level */
        internal int channel;        /* the chat channel being used */
        internal short wizard;           /* is player game admin */
        internal bool cloaked;       /* is the player cloaked */
        internal bool palantir;      /* does the player have a palantir */
        internal bool blind;         /* is character blind? */
    };

    internal class game_t       /* structure for a linked list of games */
    {
        /* the thread itself */
        internal CLibPThread.pthread_t the_thread;       /* the game thread */
        internal bool cleanup_thread;        /* has the thread ended? */
        internal int the_socket;
        internal CLibPThread.pid_t clientPid;         /* the process ID of the client */ 

        /* player information */
        internal string IP; //[phantdefs.SZ_FROM];       /* IP being used */
        internal string network; //[phantdefs.SZ_FROM];  /* network being used */
        internal string account; //[phantdefs.SZ_NAME];  /* account being used */
        internal long machineID;     /* machine number */

        /* character information */
        internal double x;           /* x-coordinate */
        internal double y;           /* y-coordinate */
        internal bool virtualvirtual;		/* player not at specific coordinates */
        internal string area;  //[phantdefs.SZ_AREA];     /* name of area in the game */
        internal bool useLocationName;   /* always show area over coords */
        internal bool palantir;      /* does the player have a palantir */
        internal double circle;          /* the circle the character is in */
        internal int hearAllChannels;    /* can this player hear all chat? */
        internal bool chatFilter;
        internal bool sendEvents;        /* include this player on broadcasts? */

        internal player_desc_t description;	/* pointer to the player description */
        internal it_combat_t it_combat;	/* pointer to character's it_combat */

        /* event traffic */
        internal event_t events_in;		/* queue for events to the thread */
        internal CLibPThread.pthread_mutex_t events_in_lock; /* lock on events in */

        /* server info */
        internal game_t next_game;		/* pointer to the next game */
    };

    internal class event_t      /* structure to describe actions */
    {
        /* event information */
        internal short type;         /* the type of event */
        internal double arg1, arg2;      /* event arguments */
        internal long arg3;          /* another argument */
        internal object arg4; //void*         /* and more arguments */

        /* addressing information */
        internal game_t from;		/* who created the event */
        internal game_t to;			/* where the event is going */
        internal event_t next_event;		/* pointer to the next event */
    };

    [Serializable]
    internal class scoreboard_t         /* scoreboard entry */
    {
        internal double level;           /* level of player */
        internal string classclass; //[phantdefs.SZ_CLASS_NAME];	/* character type of player */
        internal string name; //[phantdefs.SZ_NAME];     /* name of player */
        internal string from; //[phantdefs.SZ_FROM];     /* ip or DNS of player */
        internal string how_died; //[phantdefs.SZ_HOW_DIED]; /* description of player's fate */
        internal int time;            /* time of death */
    };

    internal class monster_t    /* base monster information */
    {
        internal string name; //[phantdefs.SZ_MONSTER_NAME]; /* name of the monster */
        internal short special_type;         /* monster special type */
        internal double experience;      /* monster experience */
        internal double energy;          /* monster energy */
        internal double strength;        /* monster strength */
        internal double brains;          /* monster brains */
        internal double speed;           /* monster speed */
        internal short treasure_type;        /* monster treasure type */
        internal short flock_percent;        /* percent chance of flocking */
    };

    internal class charstats_t          /* character type statistics */
    {
        internal string class_name;  //[phantdefs.SZ_CLASS_NAME];/* name of the character class */
        internal char short_class_name;  /* character class abriviation */
        internal double max_brains;            /* max brains per level */
        internal double max_mana;              /* max mana per level */
        internal double weakness;             /* how strongly poison affects player */
        internal double goldtote;             /* how much gold char can carry */
        internal int ring_duration;         /* bad ring duration */

        internal class statset
        {
            internal double statbase;           /* base for roll */
            internal double interval;       /* interval for roll */
            internal double increase;       /* increment per level */
        }
        internal statset quickness;          /* quickness */
        internal statset strength;           /* strength */
        internal statset mana;               /* mana */
        internal statset energy;             /* energy level */
        internal statset brains;             /* brains */
        internal statset magiclvl;           /* magic level */

        internal charstats_t()
        {
            quickness = new statset();
            strength = new statset();
            mana = new statset();
            energy = new statset();
            brains = new statset();
            magiclvl = new statset();
        }
    };

    internal class shop_item_t			/* menu item for purchase */
    {
        internal string item;  //[phantdefs.SZ_ITEMS];		/* menu item name */
        internal double cost;			/* cost of item */
    };

    [Serializable]
    internal class realm_state_t    /* things to save upon shutdown */
    {
        internal double kings_gold;
        internal string king_name; //[phantdefs.SZ_NAME];	/* name of old king */
        internal string valar_name; //[phantdefs.SZ_NAME];	/* name of old valar */
    };

    [Serializable]
    internal class realm_object_t : object /* things to run into out there */
    {
        internal double x;			/* x coordinate */
        internal double y;			/* y coordinate */
        internal short type;			/* what the object is */
        internal player_t arg1;	//void*		/* item pointer argument */
        internal realm_object_t next_object;	/* pointer to the next object */
    };

    internal class realm_t		/* variables for the entire realm */
    {
        /* server variables */
        internal CLibPThread.pid_t serverPid;            /* the process ID of the server */

        /* the linked list of games */
        internal game_t games;		/* begin linked list of games */
        internal game_t knight;		/* pointer to current knight */
        internal game_t king;		/* pointer to current king */
        internal game_t valar;		/* pointer to current valar */
        internal bool king_flag;			/* true when there is a king */
        internal string king_name;//;//[phantdefs.SZ_NAME];		/* name of old king */
        internal string valar_name;//; //[phantdefs.SZ_NAME];       /* name of old valar */

        /* file mutexes */
        internal CLibPThread.pthread_mutex_t backup_lock;	/* access to the backup file */
        internal CLibPThread.pthread_mutex_t scoreboard_lock;	/* scoreboard file in use */
        internal CLibPThread.pthread_mutex_t character_file_lock; /* locks access to character file */
        internal CLibPThread.pthread_mutex_t log_file_lock;	/* locks all log files */
        internal CLibPThread.pthread_mutex_t network_file_lock;	/* locks the network file */
        internal CLibPThread.pthread_mutex_t tag_file_lock;	/* locks the tag file */
        internal CLibPThread.pthread_mutex_t tagged_file_lock;	/* locks the tagged file */
        internal CLibPThread.pthread_mutex_t history_file_lock;  /* locks the history file */

        /* variable/link list mutexes */
        internal CLibPThread.pthread_mutex_t realm_lock;		/* locks the whole realm */
        internal CLibPThread.pthread_mutex_t account_lock;	/* account file and email_limbo */
        internal CLibPThread.pthread_mutex_t hack_lock;		/* locks hack stats */
        internal CLibPThread.pthread_mutex_t monster_lock;	/* monster list lock */
        internal CLibPThread.pthread_mutex_t object_lock;	/* realm object lock */
        internal CLibPThread.pthread_mutex_t kings_gold_lock;	/* lock of the kings gold */
        internal CLibPThread.pthread_mutex_t connections_lock;   /* lock the connection linked list */

        /* changing game variables */
        internal monster_t[] monster; /* list of all monsters */
        internal realm_object_t objects;	/* list of objects in realm */
        internal double kings_gold;		/* amount of gold in kings coffers */
        internal double steward_gold;		/* amount of gold in stewards coffers */
        internal int nextTagNumber;          /* numbers for session tags */

        /* realm-wide fixed information */
        internal charstats_t[] charstats; /* character types */
        internal shop_item_t[] shop_item; /* shop items */

        /* linked lists */
        internal connection_t connections;	/* counts of logins and passwords */

        /* names and e-mail addresses in limbo */
        internal linked_list_t name_limbo;
        internal linked_list_t email_limbo;

        internal realm_t() //initialise arrays ready for data loads
        {
            monster = new monster_t[phantdefs.NUM_MONSTERS];
            for (int i = 0; i < monster.Length; i++)
            {
                monster[i] = new monster_t();

            }
            charstats = new charstats_t[phantdefs.NUM_CHARS];
            for (int i = 0; i < charstats.Length; i++)
            {
                charstats[i] = new charstats_t();

            }
            shop_item = new shop_item_t[phantdefs.NUM_ITEMS];
            for (int i = 0; i < shop_item.Length; i++)
            {
                shop_item[i] = new shop_item_t();

            }
        }
    };

    [Serializable]
    internal class account_t  : object /* structure to store account information */
    {
        /* main information */
        internal string name;  //[phantdefs.SZ_NAME];		/* account name */
        internal string lcname; //[phantdefs.SZ_NAME];	/* name in lowercase */
        internal char[] password;   /* password hash */
        internal int last_reset;		/* time of last password reset */
        internal string email; //[phantdefs.SZ_FROM];	/* creator e-mail */
        internal string lcemail; //[phantdefs.SZ_FROM]; /* e-mail in lowercase */
        internal char[] confirmation;  /* first time confirmation */

        /* creation information */
        internal string parent_IP; //[phantdefs.SZ_FROM];     /* created from this address */
        internal string parent_network; //[phantdefs.SZ_FROM];     /* created from this address */
        internal int date_created;          /* created at this time */

        /* previous user information */
        internal string last_IP; //[phantdefs.SZ_FROM];       /* last IP or DNS address */
        internal string last_network; //[phantdefs.SZ_FROM];  /* last IP or DNS address */
        internal int last_load;		/* time last accessed */
        internal int login_count;        /* number of times account used */

        /* hack foilers */
        internal int bad_passwords;          /* unsuccessful load attempts */
        internal int hackCount;		/* number of alleged hacks */
        internal int rejectCount;		/* times given a reject tag */
        internal int lastHack;		/* time of the last hack */
        internal int muteCount;		/* times given a reject tag */
        internal int lastMute;        /* time of the last hack */

        internal account_t() //initialise arrays ready for data loads
        {
            password = new char[16];
            confirmation = new char[phantdefs.SZ_PASSWORD];
        }

    };

    internal class account_mod_t  : object /* information to modify an account */
    {
        /* new password */
        internal bool newPassword;		/* is there a new password? */
        internal char[] password;       /* hash of the new password */
        internal bool passwordReset;     /* is the password being reset? */

        /* e-mail resets - not used */
        internal bool newEmail;
        internal string email;//[phantdefs.SZ_FROM];
        internal char[] confirmation;

        /* hack information */
        internal bool hack;
        internal int hackCount;
        internal bool mute;
        internal int muteCount;

        /* info updates */
        internal bool confirm;
        internal bool access;
        internal bool badPassword;

        internal account_mod_t() //initialise arrays ready for data loads
        {
            password = new char[16];
            confirmation = new char[phantdefs.SZ_PASSWORD];
        }
    };

    internal class server_t : object		/* structure for top server variables */
    {
        /* server variables */
        internal short run_level;		/* flag set to shutdown server */
        internal int the_socket;     /* socket to listen for connections */

        /* game variables */
        internal short num_games;		/* current count of players */
        internal realm_t realm;		/* realm variables */

        internal server_t()
        {
            realm = new realm_t();

            if (CLibPThread.knownThreads.ContainsKey(System.Threading.Thread.CurrentThread.Name))
            {
                CLibPThread.pthread_t pthread = CLibPThread.knownThreads[System.Threading.Thread.CurrentThread.Name];
                //the_socket = pthread.associatedSocket.FileDescriptor; //thread is created after socket, socket is set in caller
            }
            else
            {
                Debug.LogError("No socket found for thread " + System.Threading.Thread.CurrentThread.Name + " in server_t creation");
            }
        }
    };

    internal class player_spec_t : object /* info for java client interface */
    {
        /* the data */
        internal string name;//[phantdefs.SZ_NAME + 1];			/* character name */
        internal char[] type; /* character type and special type */

        internal player_spec_t() //initialise arrays ready for data loads
        {
            type = new char[phantdefs.SZ_SPEC];
        }
    };

    internal class player_stats_t  : object /* main display information */
    {
        /* player location */
        internal uint x1;			/* x coord */
        internal uint x2;			/* x coord */
        internal uint y1;			/* y coord */
        internal uint y2;            /* y coord */

        /* player stats */
        internal uint strength1;		/* strength */
        internal uint strength2;		/* strength */
        internal uint max_str1;		/* maximum strength */
        internal uint max_str2;		/* maximum strength */
        internal uint energy1;		/* energy */
        internal uint energy2;		/* energy */
        internal uint max_ener1;		/* maximum energy */
        internal uint max_ener2;		/* maximum energy */
        internal uint quickness1;		/* quickness */
        internal uint quickness2;		/* quickness */
        internal uint max_quick1;		/* quickness */
        internal uint max_quick2;		/* quickness */
        internal uint mana1;			/* mana */
        internal uint mana2;			/* mana */
        internal uint level1;		/* level */
        internal uint level2;        /* level */

        /* player currency */
        internal uint gold1;			/* gold */
        internal uint gold2;         /* gold */

        /* player equipment */
        internal uint sword1;		/* sword */
        internal uint sword2;		/* sword */
        internal uint shield1;		/* shield */
        internal uint shield2;		/* shield */
        internal uint quicksilver1;		/* quicksilver */
        internal uint quicksilver2;		/* quicksilver */
        internal bool crowns;			/* crowns */
        internal bool virgin;			/* virgin */
        internal bool palantir;		/* palantir */
        internal bool blessing;		/* blessing */
        internal bool ring;          /* ring */

        /* player information */
        internal string name;  //[phantdefs.SZ_NAME + 1]; 	/* player name */
        internal string area;  //[phantdefs.SZ_AREA + 1]; /* name of the area player is in */
    };

    internal class button_t : object		/* client button configuration */
    {
        /* compass buttons */
        internal char compass;

        /* interface buttons */
        internal string[] button;// = new char[8,phantdefs.SZ_LABEL];

        internal button_t() //initialise arrays ready for data loads
        {
            button = new string[8];
        }
    };

    internal class linked_list_t  : object /* generic linked list */
    {
        internal string name;  //[phantdefs.SZ_FROM];
        internal linked_list_t next; //*next;
    };

    internal class examine_t : object		/* examine strcture */
    {
        internal string title = new string(new char[phantdefs.SZ_NAME + 30]);	/* player name */
        internal string location = new string(new char[50]);    /* name of the area player is in */

        /* player information */
        internal string account = new string(new char[phantdefs.SZ_NAME]);	/* account player is using */
        internal string network = new string(new char[phantdefs.SZ_FROM]);	/* player's domain */
        internal short channel;      /* channel player is on */

        /* player stats */
        internal double level;			/* level */
        internal double experience;		/* experience */
        internal double nextLevel;		/* experience */

        internal double strength;		/* strength */
        internal double max_strength;		/* maximum strength */
        internal double energy;			/* energy */
        internal double max_energy;		/* maximum energy */
        internal float quickness;		/* quickness */
        internal float max_quickness;		/* quickness */
        internal double mana;			/* mana */
        internal double brains;			/* brains */
        internal double magiclvl;		/* magic level */
        internal float poison;			/* poison */
        internal float sin;			/* sin */
        internal string gender = new string(new char[7]);			/* player is male/female */
        internal short lives;            /* multiple lives for council, valar */

        /* player status */
        internal string cloaked = new string(new char[5]);			/* is character cloaked */
        internal string blind = new string(new char[5]);	          /* blindness */

        /* player currency */
        internal double gold;			/* gold */
        internal double gems;            /* gems */

        /* player equipment */
        internal double sword;			/* sword */
        internal double shield;			/* shield */
        internal float quicksilver;		/* quicksilver */
        internal int holywater;		/* holy water */
        internal int amulets;		/* amulets */
        internal int charms;			/* charms */
        internal short crowns;			/* crowns */
        internal string virgin = new string(new char[5]);			/* virgin */
        internal string palantir = new string(new char[5]);			/* palantir */
        internal string blessing = new string(new char[5]);			/* blessing */
        internal string ring = new string(new char[5]);	      /* ring */

        /* creation information */
        internal int age;			/* age of player */
        internal int degenerated;		/* age/3000 last degenerated */
        internal string date_loaded = new string(new char[30]);			/* loaded on this date */
        internal string date_created = new string(new char[30]);		/* created at this time */
        internal string time_played = new string(new char[12]);		/* seconds character has been played */

    };

    [Serializable]
    internal class tag_t  : object /* saved struct of bans, mutes, etc. */
    {
        internal int number;
        internal short type;
        internal int validUntil;
        internal bool affectNetwork;
        internal bool contagious;
        internal string description;  //[phantdefs.SZ_FROM];
    };

    [Serializable]
    internal class tagged_t  : object /* saved struct of bans, mutes, etc. */
    {
        internal int tagNumber;
        internal short type;
        internal string name;  //[phantdefs.SZ_FROM];
        internal int validUntil;
    };

    [Serializable]
    internal class network_t  : object /* strcture to save hack info for networks */
    {
        internal string address;  //[phantdefs.SZ_FROM];
        internal int hackCount;
        internal int lastHack;
        internal int muteCount;
        internal int lastMute;
        internal int expires;
    };

    internal class connection_t  : object /* short term struct to watch for hacks by connection */
    {
        internal string theAddress;//[phantdefs.SZ_FROM];
        internal int[] connections;
        internal int connectionCount;
        internal int[] badPasswords;
        internal int badPasswordCount;
        internal int eraseAt;
        internal connection_t next;

        internal connection_t() //initialise arrays ready for data loads
        {
            connections = new int[10];
            badPasswords = new int[8];
        }
    };

    internal class tagged_list_t 	/* to create a list of tags */
    {
        internal tagged_t theTagged;
        internal tagged_list_t next;
    };

    [Serializable]
    internal class history_t  : object /* list of past administrative events */
    {
        internal int date;
        internal short type;
        internal string name;  //[phantdefs.SZ_FROM];
        internal string description;  //[phantdefs.SZ_LINE + 50];
    };

    internal class history_list_t 	/* to create a list of history */
    {
        internal history_t theHistory;
        internal history_list_t next;
    };

    internal class detail_t		/* information on player's connection */
    {
        internal string modifiedName;  //[phantdefs.SZ_NAME];
        internal string name;  //[phantdefs.SZ_NAME];
        internal string faithful;// = new char[5];
        internal string parentAccount;  //[phantdefs.SZ_NAME];
        internal string charParentNetwork;  //[phantdefs.SZ_FROM];
        internal int playerMutes;

        internal string account;  //[phantdefs.SZ_NAME];
        internal string email;  //[phantdefs.SZ_FROM];
        internal string accParentNetwork;  //[phantdefs.SZ_FROM];
        internal int accountMutes;

        internal string IP;  //[phantdefs.SZ_FROM];
        internal string network;  //[phantdefs.SZ_FROM];
        internal int machineID;
        internal string dateConnected;// = new char[30];
        internal int networkMutes;

    };

    internal class tagged_sort_t  : object /* strcture to help tag inheritance */
    {
        internal int tag;
        internal tagged_t[] tagged;
        internal tagged_sort_t next;

        internal tagged_sort_t() //initialise arrays ready for data loads
        {
            tagged = new tagged_t[4];
            for (int i = 0; i < tagged.Length; i++)
            {
                tagged[i] = new tagged_t();
            }
        }
    };

}
