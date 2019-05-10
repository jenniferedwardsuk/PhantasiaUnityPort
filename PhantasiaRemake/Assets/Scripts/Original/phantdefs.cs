using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace phantasiaclasses
{
    public static class phantdefs
    {


        /*
     * h - important constants for Phantasia
     */

        /* testig stuff - should be delete before production */

        /* configurables */
        internal static int PHANTASIA_PORT = 9898;

        /* boolean identifiers */
        internal static int MALE = 0;
        internal static int FEMALE = 1;

        /* server run_level constants */
        internal static int RUN_SERVER = 0; /* continue through main loop */
        internal static int HARD_SHUTDOWN = 1; /* kill server NOW */
        internal static int FAST_SHUTDOWN = 2; /* just save the realm */
        internal static int SHUTDOWN = 3; /* wait on threads, then save */
        internal static int LEISURE_SHUTDOWN = 4; /* wait until no one is logged on */

        /* thread run_level constants */
        internal static int SIGNING_IN = 0; /* connection has no account */
        internal static int CHAR_SELECTION = 1; /* player needs a character */
        internal static int PLAY_GAME = 2; /* in play */
        internal static int SAVE_AND_CONTINUE = 3; /* save char and ask to continue */
        internal static int SAVE_AND_EXIT = 4; /* save char and leave game */
        internal static int GO_AGAIN = 5; /* save char and leave game */
        internal const int EXIT_THREAD = 6; /* leave the main loop */
        internal static int LEAVING_THREAD = 7; /* Final thread exit */

        /* socket return values */
        internal static int S_NORM = 0; /* socket returned data */
        internal static int S_CANCEL = 1; /* player canceled selection */
        internal static int S_TIMEOUT = 2; /* player ran out of time */
        internal static int S_ERROR = 3; /* socket is closed */

        /* tag types */
        internal const int T_REJECT = 0; /* hang up on address ASAP */
        internal const int T_BAN = 1; /* kick when character loaded, ban */
        internal const int T_SUICIDE = 2; /* all loaded characters hari-kari */
        internal const int T_MUTE = 3; /* player may not chat */
        internal const int T_PREFIX = 4; /* add a prefix to the player */
        internal const int T_SUFFIX = 5; /* add a suffix to the player */

        /* tagged types */
        internal const int T_MACHINE = 0; /* tag of a specific machine */
        internal const int T_ACCOUNT = 1; /* tag of an account */
        internal const int T_ADDRESS = 2; /* tag of an address */
        internal const int T_NETWORK = 3; /* tag of an network */

        /* hearing constants */
        internal static int HEAR_SELF = 0;      /* hear the channel you're in */
        internal static int HEAR_ONE = 1;     /* hear channel 1 -- apprentice */
        internal static int HEAR_ALL = 11;    /* hear all channels */

        /* realm objects */
        internal const int ENERGY_VOID = 0; /* object is an energy void */
        internal const int CORPSE = 1; /* object is a corpse */
        internal const int TREASURE_TROVE = 2; /* opject is the treasure trove */
        internal const int HOLY_GRAIL = 3; /* object is the holy grail */

        /* total number of things */
        internal static int NUM_MONSTERS = 100; /* monsters in the game */
        internal static int NUM_CHARS = 6; /* number of character types */
        internal static int NUM_ITEMS = 7; /* items in the shops */

        /* event types */
        internal const int NULL_EVENT = 0; /* blank event */
                                           /* immediate events */
        internal const int KICK_EVENT = 1; /* get off my game */
        internal const int TAG_EVENT = 2; /* tag yourself */
        internal const int REQUEST_DETAIL_EVENT = 3; /* get player info */
        internal const int CONNECTION_DETAIL_EVENT = 4; /* another player's info */
        internal const int REQUEST_RECORD_EVENT = 5; /* get player info */
        internal const int PLAYER_RECORD_EVENT = 6; /* another player's info */
        internal const int ADD_PLAYER_EVENT = 7; /* add player info */
        internal const int REMOVE_PLAYER_EVENT = 8; /* remove player info */
        internal const int CHANGE_PLAYER_EVENT = 9; /* change player info */
        internal const int CHAT_EVENT = 10; /* chat message */
        internal const int REPRIMAND_EVENT = 11; /* Apprentices reprimand */
        internal const int UNTAG_EVENT = 12; /* remove a prefix or suffix */
        internal const int UNSUSPEND_EVENT = 13; /* resume player's game */
        internal static int GAME_MARKER = 14; /* only game events follow */
                                              /* ASAP events */
        internal const int DEATH_EVENT = 20; /* player died */
        internal const int IT_COMBAT_EVENT = 21; /* enter interterminal-combat */
        internal const int EXPERIENCE_EVENT = 23; /* award the player experience */
                                                  /* tampering events */
        internal const int SUSPEND_EVENT = 25; /* stop player's game */
        internal const int CANTRIP_EVENT = 26; /* apprentice options */
        internal const int MODERATE_EVENT = 27; /* wizard options */
        internal const int ADMINISTRATE_EVENT = 28; /* administrative options */
        internal const int VALAR_EVENT = 29; /* become/lose valar */
        internal const int KING_EVENT = 30; /* become king */
        internal const int STEWARD_EVENT = 31; /* become steward */
        internal const int DETHRONE_EVENT = 32; /* lose king or steward */
        internal const int SEX_CHANGE_EVENT = 34;  /* toggle player's sex */
        internal const int RELOCATE_EVENT = 35; /* order player to location */
        internal const int TRANSPORT_EVENT = 36; /* transport player */
        internal const int CURSE_EVENT = 37; /* curse player */
        internal const int SLAP_EVENT = 38; /* slap player */
        internal const int BLIND_EVENT = 39; /* blind player */
        internal const int BESTOW_EVENT = 40; /* king bestowed gold */
        internal const int SUMMON_EVENT = 41; /* summon monster for player */
        internal const int BLESS_EVENT = 42;
        internal const int HEAL_EVENT = 43;
        internal const int STRONG_NF_EVENT = 44; /* set the player's strength_nf flag */
        internal const int KNIGHT_EVENT = 45; /* this player has been knighted */
        internal const int DEGENERATE_EVENT = 46;
        internal const int HELP_EVENT = 47; /* player is asking for information */

        /* command events */
        internal const int COMMAND_EVENT = 48; /* the valar uses a power */
        internal const int SAVE_EVENT = 49; /* save the game and quit */
        internal const int MOVE_EVENT = 50; /* move the character */
        internal const int EXAMINE_EVENT = 51; /* examine the stats on a character */
        internal const int DECREE_EVENT = 52; /* make a decree */
        internal const int ENACT_EVENT = 53; /* the steward enacts something */
        internal const int LIST_PLAYER_EVENT = 54; /* list the players in the game */
        internal const int CLOAK_EVENT = 55; /* cloak/uncloak */
        internal const int TELEPORT_EVENT = 56; /* teleport player */
        internal const int INTERVENE_EVENT = 57; /* a council uses a power */
        internal const int REST_EVENT = 58; /* rest player */
        internal const int INFORMATION_EVENT = 59; /* go to information screen */
        internal const int FORCEAGE_EVENT = 60;  /* forcefully age a player */

        /* normal events */
        /* events after this are destroyed on orphan */
        internal static int DESTROY_MARKER = 69;
        internal const int ENERGY_VOID_EVENT = 70; /* create/hit energy void */
        internal const int TROVE_EVENT = 71; /* find the treasure trove */
        internal const int MONSTER_EVENT = 72; /* encounter monster */
        internal const int PLAGUE_EVENT = 73; /* hit with plague */
        internal const int MEDIC_EVENT = 74; /* encounter medic */
        internal const int GURU_EVENT = 75; /* encounter guru */
        internal const int TRADING_EVENT = 76; /* find a trading post */
        internal const int TREASURE_EVENT = 77; /* find treasure */
        internal const int VILLAGE_EVENT = 78; /* found a village or a volcano */
        internal const int TAX_EVENT = 79; /* encounter tax collector */

        /* other events*/
        internal const int NATINC_EVENT = 84;     /* restoration of natural stats*/
        internal const int EQINC_EVENT = 85;    /* restoration of equipment */
        internal const int CINC_EVENT = 86; /* restoration of currency */
        internal const int AGING_EVENT = 87;     /* Age restore */
        internal const int ITEMINC_EVENT = 88; /* Restores items */


        /* realm objects */
        /* events after this are made realm objects on orphan */
        internal static int REALM_MARKER = 90;
        internal const int CORPSE_EVENT = 91; /* find a corpse */
        internal const int GRAIL_EVENT = 92; /* find the holy grail */
        internal const int LAST_EVENT = 93; /* used to find bad events */

        /* combat messages */
        internal const int IT_OPPONENT_BUSY = 1; /* currently on another combat */
        internal const int IT_REPORT = 2; /* report in */
        internal const int IT_JUST_DIED = 3; /* player has just been killed */
        internal const int IT_JUST_LEFT = 42; /* player just left the game */
        internal const int IT_ATTACK = 4; /* player is attacker */
        internal const int IT_DEFEND = 5; /* player is defender */
        internal const int IT_MELEE = 6; /* attacker meleed */
        internal const int IT_SKIRMISH = 7; /* attacker skirmished */
        internal const int IT_NICKED = 8; /* attacker nicked */
        internal const int IT_EVADED = 9; /* attacker evaded */
        internal const int IT_NO_EVADE = 10; /* attacker failed to evade */
        internal const int IT_LUCKOUT = 11; /* attacker lucked-out (luckouted?) */
        internal const int IT_NO_LUCKOUT = 12; /* attacker failed to luckout */
        internal const int IT_RING = 13; /* attacker put on a ring */
        internal const int IT_NO_RING = 14; /* attacker failed to put on a ring */
        internal const int IT_ALL_OR_NOT = 15; /* attacker cast all or nothing */
        internal const int IT_NO_ALL_OR_NOT = 16; /* attacker blew all or nothing */
        internal const int IT_BOLT = 17; /* attacker cast magic bolt */
        internal const int IT_NO_BOLT = 18; /* attacker blew magic bolt */
        internal const int IT_SHIELD = 19; /* attacker cast force field */
        internal const int IT_NO_SHIELD = 20; /* attacker blew force field */
        internal const int IT_TRANSFORM = 21; /* attacker transformed defender */
        internal const int IT_NO_TRANSFORM = 22; /* attacker blew transform */
        internal const int IT_TRANSFORM_BACK = 23; /* attacker's transform backfired */
        internal const int IT_MIGHT = 24; /* attacker cast increase might */
        internal const int IT_NO_MIGHT = 25; /* attacker blew increase might */
        internal const int IT_HASTE = 26; /* attacker cast haste */
        internal const int IT_NO_HASTE = 27; /* attacker blew haste */
        internal const int IT_TRANSPORT = 28; /* attacker cast transport */
        internal const int IT_NO_TRANSPORT = 29; /* attacker blew transport */
        internal const int IT_TRANSPORT_BACK = 30; /* attacker's transport backfired */
        internal const int IT_PARALYZE = 31; /* attaker cast paralyze */
        internal const int IT_NO_PARALYZE = 32; /* attaker blew paralyze */
        internal const int IT_PASS = 33; /* attacker passed the turn */
        internal const int IT_CONTINUE = 34; /* defender continues the battle */
        internal const int IT_CONCEDE = 35; /* defender surrenders */
        internal const int IT_DEFEAT = 36; /* the sender stands defeated */
        internal const int IT_VICTORY = 37; /* the sender claims victory */
        internal const int IT_DONE = 38; /* This is Kang, cease hostilities */
        internal const int IT_ECHO = 39; /* tell me to attack */
        internal const int IT_ABANDON = 40; /* player quiting and saving self  */
        internal const int IT_BORED = 41; /* player quit after timeout */
        internal const int IT_WIZEVADE = 43; /* player used wiz powers to evade */
        
        /* client>player packet headers */
        internal static int HANDSHAKE_PACKET = 2; /* used when connecting */
        internal static int CLOSE_CONNECTION_PACKET = 3; /* last message before close */
        internal static int PING_PACKET = 4; /* used for timeouts */
        internal static int ADD_PLAYER_PACKET = 5; /* add a player to the list */
        internal static int REMOVE_PLAYER_PACKET = 6; /* remove a player from the list */
        internal static int SHUTDOWN_PACKET = 7; /* the server is going down */
        internal static int ERROR_PACKET = 8; /* server has encountered an error */

        internal static int CLEAR_PACKET = 10; /* clears the message screen */
        internal static int WRITE_LINE_PACKET = 11; /* write a line on message screen */

        internal static int BUTTONS_PACKET = 20; /* use the interfaces buttons */
        internal static int FULL_BUTTONS_PACKET = 21; /* use buttons and compass */
        internal static int STRING_DIALOG_PACKET = 22; /* request a message response */
        internal static int COORDINATES_DIALOG_PACKET = 23; /* request player coordinates */
        internal static int PLAYER_DIALOG_PACKET = 24; /* request a player name */
        internal static int PASSWORD_DIALOG_PACKET = 25; /* string dialog with hidden text */
        internal static int SCOREBOARD_DIALOG_PACKET = 26; /* pull up the scoreboard */
        internal static int DIALOG_PACKET = 27; /* okay dialog with next line */

        internal static int CHAT_PACKET = 30; /* chat message */
        internal static int ACTIVATE_CHAT_PACKET = 31; /* turn on the chat window */
        internal static int DEACTIVATE_CHAT_PACKET = 32; /* turn off the chat window */
        internal static int PLAYER_INFO_PACKET = 33; /* display a player's info */
        internal static int CONNECTION_DETAIL_PACKET = 34; /* display connection info */

        internal static int NAME_PACKET = 40; /* set the player's name */
        internal static int LOCATION_PACKET = 41; /* refresh the player's energy */
        internal static int ENERGY_PACKET = 42; /* refresh the player's energy */
        internal static int STRENGTH_PACKET = 43; /* refresh the player's strength */
        internal static int SPEED_PACKET = 44; /* refresh the player's speed */
        internal static int SHIELD_PACKET = 45; /* refresh the player's shield */
        internal static int SWORD_PACKET = 46; /* refresh the player's sword */
        internal static int QUICKSILVER_PACKET = 47; /* refresh the player's quicksilver */
        internal static int MANA_PACKET = 48; /* refresh the player's mana */
        internal static int LEVEL_PACKET = 49; /* refresh the player's level */
        internal static int GOLD_PACKET = 50; /* refresh the player's gold */
        internal static int GEMS_PACKET = 51; /* refresh the player's gems */
        internal static int CLOAK_PACKET = 52; /* refresh the player's cloak */
        internal static int BLESSING_PACKET = 53; /* refresh the player's blessing */
        internal static int CROWN_PACKET = 54; /* refresh the player's crowns */
        internal static int PALANTIR_PACKET = 55; /* refresh the player's palantir */
        internal static int RING_PACKET = 56; /* refresh the player's ring */
        internal static int VIRGIN_PACKET = 57; /* refresh the player's virgin */


        /* player>client packet headers */
        internal const int C_RESPONSE_PACKET = 1; /* player feedback for game */
        internal const int C_CANCEL_PACKET = 2; /* player canceled question */
        internal const int C_PING_PACKET = 3; /* response to a ping */
        internal const int C_CHAT_PACKET = 4; /* chat message from player */
        internal const int C_EXAMINE_PACKET = 5; /* examine a player */
        internal const int C_ERROR_PACKET = 6; /* client is lost */
        internal const int C_SCOREBOARD_PACKET = 7; /* show the scoreboard */

        /* locations within the realm */
        internal static int PL_REALM = 0; /* normal coordinates */
        internal static int PL_THRONE = 1; /* In the lord's chamber */
        internal static int PL_EDGE = 2; /* On the edge of the realm */
        internal static int PL_VALHALLA = 3; /* In Valhalla */
        internal static int PL_PURGATORY = 4; /* In purgatory fighting */

        /* size of many structures */ //todo  //current sizes are based on byte sizes of written files
        internal static int SZ_PLAYER = 1500; // actual test: 1075, 1073, 1102      // sizeof(player_t); /* size of player_t */
        internal static int SZ_GAME;// sizeof(game_t); /* size of game_t */
        internal static int SZ_IT_COMBAT;// sizeof(it_combat_t); /* size of it_combat_t */
        internal static int SZ_PLAYER_DESC;// sizeof(player_desc_t); /* size of player_desc_t */
        internal static int SZ_PLAYER_SPEC;// sizeof(player_spec_t); /* size of player_spec_t */
        internal static int SZ_EVENT;// sizeof(event_t); /* size of event_t */
        internal static int SZ_REALM_STATE = 250; // actual test: 129, 5163 //todo     // sizeof(realm_state_t); /* size of realm_state_t */
        internal static int SZ_REALM_OBJECT = 250; // actual test: 237, 204      // sizeof(realm_object_t); /* size of realm_object_t */
        internal static int SZ_SCOREBOARD;// sizeof(scoreboard_t); /* size of scoreboard_t */
        internal static int SZ_CLIENT;// sizeof(client_t); /* size of client_t */
        internal static int SZ_OPPONENT;// sizeof(opponent_t); /* size of opponent_t */
        internal static int SZ_BUTTON;// sizeof(button_t); /* size of button_t */
        internal static int SZ_ACCOUNT = 550; // actual test: 516, 502, 508      // sizeof(account_t); /* size of account_t */
        internal static int SZ_LINKED_LIST;// sizeof(linked_list_t); /* size of linked_list_t */
        internal static int SZ_EXAMINE;// sizeof(examine_t);
        internal static int SZ_TAG;// sizeof(tag_t);
        internal static int SZ_TAGGED;// sizeof(tagged_t);
        internal static int SZ_TAGGED_LIST;// sizeof(tagged_list_t);
        internal static int SZ_NETWORK;// sizeof(network_t);
        internal static int SZ_CONNECTION;// sizeof(connection_t);
        internal static int SZ_HISTORY;// sizeof(history_t);
        internal static int SZ_HISTORY_LIST;// sizeof(history_list_t);
        internal static int SZ_DETAIL;// sizeof(detail_t);
        internal static int SZ_TAGGED_SORT;// sizeof(tagged_sort_t);

        /* string sizes */
        internal static int SZ_IN_BUFFER = 1024; /* largest possible client message */
        internal static int SZ_OUT_BUFFER = 1024; /* largest possible server message */
        internal static int SZ_NAME = 33; /* player name field (incl. trailing null) */
        internal static int MAX_NAME_LEN = 16; /* actual player name */
        internal static int SZ_PASSWORD = 16; /* 128 bit MD5 hash of the password */
        internal static int SZ_FROM = 81; /* ip or dns login (incl. null) */
        internal static int SZ_MONSTER_NAME = 49; /* characters in monster names */
        internal static int SZ_AREA = 24; /* name of player location */
        internal static int SZ_HOW_DIED = 78; /* string describing character death */
        internal static int SZ_CLASS_NAME = 13; /* longest class name */
        internal static int SZ_ITEMS = 12; /* longest shop item description */
        internal static int SZ_ERROR_MESSAGE = 256; /* max length of error message */
        internal static int SZ_LINE = 256; /* length of one line on terminal */
        internal static int SZ_LABEL = 22; /* length of interface button text */
        internal static int SZ_NUMBER = 25; /* characters describing number */
        internal static int SZ_CHAT = 512; /* largest chat message */
        internal static int SZ_PACKET_TYPE = 2; /* maximum packet size */
        internal static int SZ_SPEC = 7; /* 5 chars, newline and null */

        /* possible errors */
        internal static int MALLOC_ERROR = 1001;
        internal static int DATA_FILE_ERROR = 1002;
        internal static int MONSTER_FILE_ERROR = 1003;
        internal static int CHARACTER_FILE_ERROR = 1004;
        internal static int CHARSTATS_FILE_ERROR = 1004;
        internal static int SHOPITEMS_FILE_ERROR = 1004;
        internal static int SCOREBOARD_FILE_ERROR = 1005;
        internal static int CHAT_LOG_FILE_ERROR = 1006;
        internal static int SOCKET_CREATE_ERROR = 1007;
        internal static int SOCKET_BIND_ERROR = 1008;
        internal static int SOCKET_LISTEN_ERROR = 1009;
        internal static int SOCKET_SELECT_ERROR = 1010;
        internal static int SOCKET_ACCEPT_ERROR = 1011;
        internal static int MUTEX_INIT_ERROR = 1012;
        internal static int MUTEX_DESTROY_ERROR = 1013;
        internal static int MUTEX_LOCK_ERROR = 1014;
        internal static int MUTEX_UNLOCK_ERROR = 1015;
        internal static int PTHREAD_ATTR_ERROR = 1016;
        internal static int PTHREAD_CREATE_ERROR = 1017;
        internal static int GENERAL_EVENT_ERROR = 1021;
        internal static int IMPOSSIBLE_EVENT_ERROR = 1022;
        internal static int EVENT_ADDRESS_ERROR = 1023;
        internal static int UNDEFINED_OBJECT_ERROR = 1024;
        internal static int BATTLE_PHASE_ERROR = 1025;
        internal static int DEFENDER_MESSAGE_ERROR = 1026;
        internal static int BATTLE_MESSAGE_ERROR = 1027;
        internal static int SEND_SOCKET_ERROR = 1028;
        internal static int READ_SOCKET_ERROR = 1029;

        /* ring constants */
        internal const int R_NONE = 0;             /* no ring */
        internal const int R_NAZREG = 1;          /* regular Nazgul ring (expires) */
        internal const int R_DLREG = 2;             /* regular Dark Lord ring (does not expire) */
        internal const int R_BAD = 3;            /* bad ring */
        internal const int R_SPOILED = 4;       /* ring which has gone bad */
        internal const int R_YES = 5; /* masked ring type */

        /* constants for character types */
        internal static int C_MAGIC = 0;               /* magic user */
        internal static int C_FIGHTER = 1;              /* fighter */
        internal static int C_ELF = 2;             /* elf */
        internal static int C_DWARF = 3;            /* dwarf */
        internal static int C_HALFLING = 4;           /* halfling */
        internal static int C_EXPER = 5;          /* experimento */

        /* constants for special character types */
        internal const int SC_NONE = 0;               /* not a special character */
        internal const int SC_KNIGHT = 1;              /* knight */
        internal const int SC_STEWARD = 2;             /* steward */
        internal const int SC_KING = 3;            /* king */
        internal const int SC_COUNCIL = 4;           /* council of the wise */
        internal const int SC_EXVALAR = 5;          /* past valar - now council */
        internal const int SC_VALAR = 6;         /* valar */

        /* means of death */
        internal const int K_OLD_AGE = 0; /* old age */
        internal const int K_MONSTER = 1; /* combat with monster */
        internal const int K_IT_COMBAT = 2; /* combat with another player */
        internal const int K_GHOSTBUSTERS = 3; /* lost connection */
        internal const int K_VAPORIZED = 4; /* vaporized by another player */
        internal const int K_RING = 5; /* killed by a cursed ring */
        internal const int K_NO_ENERGY = 6; /* player ran out of energy */
        internal const int K_FELL_OFF = 7; /* fell off the edge of the world */
        internal const int K_TRANSFORMED = 8; /* turned into another monster */
        internal const int K_SEGMENTATION = 9; /* bad internal error */
        internal const int K_SUICIDE = 10; /* character did something bad */
        internal const int K_SQUISH = 11; /* new wizard kill option */
        internal const int K_GREED = 12; /* killed when carrying too much gold */
        internal const int K_FATIGUE = 13; /* killed when speed = 0 from fatigue */
        internal const int K_SIN = 14; /* goes to hell for too much evil */

        /* special monster constants */
        internal const int SM_RANDOM = -1; /* pick a monster by normal means */
        internal const int SM_NONE = 0;          /* nothing special */
        internal const int SM_UNICORN = 1;               /* unicorn */
        internal const int SM_MODNAR = 2;             /* Modnar */
        internal const int SM_MIMIC = 3;             /* mimic */
        internal const int SM_DARKLORD = 4;               /* Dark Lord */
        internal const int SM_LEANAN = 5;            /* Leanan-Sidhe */
        internal const int SM_SARUMAN = 6;               /* Saruman */
        internal const int SM_THAUMATURG = 7;               /* thaumaturgist */
        internal const int SM_BALROG = 8;          /* balrog */
        internal const int SM_VORTEX = 9;              /* vortex */
        internal const int SM_NAZGUL = 10;              /* nazgul */
        internal const int SM_TIAMAT = 11;              /* Tiamat */
        internal const int SM_KOBOLD = 12;             /* kobold */
        internal const int SM_SHELOB = 13;            /* Shelob */
        internal const int SM_FAERIES = 14;           /* assorted faeries */
        internal const int SM_LAMPREY = 15;          /* lamprey */
        internal const int SM_SHRIEKER = 16;         /* shrieker */
        internal const int SM_BONNACON = 17;        /* bonnacon */
        internal const int SM_SMEAGOL = 18;       /* Smeagol */
        internal const int SM_SUCCUBUS = 19;      /* succubus */
        internal const int SM_CERBERUS = 20;     /* Cerberus */
        internal const int SM_UNGOLIANT = 21;    /* Ungoliant */
        internal const int SM_JABBERWOCK = 22;   /* jabberwock */
        internal const int SM_MORGOTH = 23;  /* Morgoth */
        internal const int SM_TROLL = 24; /* troll */
        internal const int SM_WRAITH = 25;/* wraith */
        internal const int SM_TITAN = 26;/* titan */
        internal const int SM_IT_COMBAT = 27;/* fighting another player */
        internal const int SM_IDIOT = 28;/* idiot */
        internal const int SM_SMURF = 29;/* smurf */
        internal const int SM_MORON = 30;/* moron */

        /* encounter constants */
        internal const int MONSTER_RANDOM = 0; /* monster was wandering */
        internal const int MONSTER_CALL = 1; /* monster was hunted */
        internal const int MONSTER_FLOCKED = 2; /* another monster in herd */
        internal const int MONSTER_SHRIEKER = 3; /* called by shrieker */
        internal const int MONSTER_JABBERWOCK = 4; /* called by jabberwock */
        internal const int MONSTER_TRANSFORM = 5; /* monster was polymorphed */
        internal const int MONSTER_SUMMONED = 6; /* another player threw monster */
        internal const int MONSTER_SPECIFY = 7; /* player requested monster */
        internal const int MONSTER_PURGATORY = 8; /* encounter in purgatory */

        /* scoreboard constants */
        internal static int SB_KEEP_ABOVE = 1000; /* below this level, delete chars */
        internal static int SB_KEEP_FOR = 2592000;/* seconds to keep low chars */

        /* other constants */
        internal static int CORPSE_LIFE = 2592000; /* seconds corpses stay in game */
        internal static int KEEP_TIME = 2592000;/* base secs to keep saved characters */
        internal static int NEWBIE_KEEP_TIME = 259200; /* base secs to keep saved characters */
        internal static int ACCOUNT_KEEP_TIME = 7776000; /* secs to keep accounts */

        /* constants for altering coordinates */
        internal const int A_SPECIFIC = 0;             /* coordinates specified, non-TP */
        internal const int A_FORCED = 1;            /* coordinates specified, ignore Beyond */
        internal const int A_NEAR = 2;           /* coordinates not specified, move near */
        internal const int A_FAR = 3;          /* coordinates not specified, move far */
        internal const int A_TRANSPORT = 4;     /* distant teleport */
        internal const int A_OUST = 5;     /* more distant teleport */
        internal const int A_BANISH = 6; /* move player to beyond */
        internal const int A_TELEPORT = 7;            /* moved by teleport */

        /* spell constants */
        internal static int P_HEAL = 0; /* steward heals a player */
        internal static int P_CURE = 1; /* council heals with poison cure */
        internal static int P_RESTORE = 2; /* valar restores a character */
        internal static int P_CURSE = 0; /* steward curse */
        internal static int P_EXECRATE = 1; /* king's stronger curse */
        internal static int P_SMITE = 2; /* valar decimates a character */

        /* constants for spells */
        internal static float ML_ALLORNOTHING = 0.0f;             /* magic level for 'all or nothing' */
        internal static float MM_ALLORNOTHING = 1.0f;             /* mana used for 'all or nothing' */
        internal static float ML_MAGICBOLT = 5.0f;             /* magic level for 'magic bolt' */
        internal static float ML_INCRMIGHT = 15.0f;            /* magic level for 'increase might' */
        internal static float MM_INCRMIGHT = 30.0f;            /* mana used for 'increase might' */
        internal static float ML_HASTE = 25.0f;            /* magic level for 'haste' */
        internal static float MM_HASTE = 35.0f;            /* mana used for 'haste' */
        internal static float ML_FORCEFIELD = 35.0f;            /* magic level for 'force field' */
        internal static float MM_FORCEFIELD = 60.0f;            /* mana used for 'force field' */
        internal static float ML_XPORT = 45.0f;            /* magic level for 'transport' */
        internal static float MM_XPORT = 100.0f;           /* mana used for 'transport' */
        internal static float ML_PARALYZE = 60.0f;            /* magic level for 'paralyze' */
        internal static float MM_PARALYZE = 125.0f;           /* mana used for 'paralyze' */
        internal static float ML_XFORM = 75.0f;            /* magic level for 'transform' */
        internal static float MM_XFORM = 150.0f;           /* mana used for 'transform' */
        internal static float MM_SPECIFY = 1000.0f;          /* mana used for 'specify' */
        internal static float ML_CLOAK = 20.0f;            /* magic level for 'cloak' */
        internal static float MEL_CLOAK = 7.0f;             /* experience level for 'cloak' */
        internal static float MM_CLOAK = 35.0f;            /* mana used for 'cloak' */
        internal static float ML_TELEPORT = 40.0f;            /* magic level for 'teleport' */
        internal static float MEL_TELEPORT = 12.0f;            /* experience level for 'teleport' */
        internal static float MM_INTERVENE = 3000.0f;          /* mana used to 'intervene' */
        internal static float MM_COMMAND = 15000.0f;         /* mana used to 'command' */

        /* some miscellaneous constants */
        internal static int N_DAYSOLD = 30;            /* number of days old for purge */
        internal static int N_AGE = 750;             /* age to degenerate ratio */
        internal static float N_GEMVALUE = 1000.0f;        /* number of gold pieces to gem ratio */
        internal static int N_FATIGUE = 50; /* rounds of combat before -1 speed */
        internal static float N_SWORDPOWER = 0.04f;             /* percentage of strength swords increase */

        internal static double D_BEYOND = 1.0e6;         /* distance to beyond point of no return */
        internal static float D_EXPER = 2000.0f;        /* distance experimentos are allowed */
        internal static float D_EDGE = 100000000.0f; /* edge of the world */
        internal static float D_CIRCLE = 125.0f; /* distance for each circle */
        internal static int STATELEN = 256; /* random number state buffer */
        internal static float MIN_STEWARD = 10.0f;       /* minimum level for steward */
        internal static float MAX_STEWARD = 200.0f;        /* maximum level for steward */
        internal static float MIN_KING = 1000.0f;          /* minimum level for king */
        internal static float MAX_KING = 2000.0f;          /* maximum level for king */

        /* hacking constants */
        internal const int H_SYSTEM = 0; /* hacking the system */
        internal const int H_PASSWORDS = 1; /* hacking passwords */
        internal const int H_CONNECTIONS = 2; /* excessive connections */
        internal const int H_KILLING = 3; /* killing rampage */
        internal const int H_PROFANITY = 4; /* using profanity */
        internal const int H_DISRESPECTFUL = 5; /* disrespectful to wizards */
        internal const int H_FLOOD = 6; /* flooding chat */
        internal const int H_SPAM = 7; /* spamming chat */
        internal const int H_WHIM = 8; /* wizard's whim */

    }
}
