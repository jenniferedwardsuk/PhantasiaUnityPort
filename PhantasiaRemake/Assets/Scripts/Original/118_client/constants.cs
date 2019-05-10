//import java.awt.Color;
//import java.awt.JavaFont;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

internal static class constants
{

    internal static  String clientVersion= "1004";

    /* socket to client protocol */
    internal const  int HANDSHAKE_PACKET = 2;
    internal const int CLOSE_CONNECTION_PACKET = 3;
    internal const int PING_PACKET = 4;
    internal const int ADD_PLAYER_PACKET = 5;
    internal const int REMOVE_PLAYER_PACKET = 6;
    internal const int SHUTDOWN_PACKET = 7;
    internal const int ERROR_PACKET = 8;

    internal const int CLEAR_PACKET = 10;
    internal const int WRITE_LINE_PACKET = 11;

    internal const int BUTTONS_PACKET = 20;
    internal const int FULL_BUTTONS_PACKET = 21;
    internal const int STRING_DIALOG_PACKET = 22;
    internal const int COORDINATES_DIALOG_PACKET = 23;
    internal const int PLAYER_DIALOG_PACKET = 24;
    internal const int PASSWORD_DIALOG_PACKET = 25;
    internal const int SCOREBOARD_DIALOG_PACKET = 26;
    internal const int DIALOG_PACKET = 27;

    internal const int CHAT_PACKET = 30;
    internal const int ACTIVATE_CHAT_PACKET = 31;
    internal const int DEACTIVATE_CHAT_PACKET = 32;
    internal const int PLAYER_INFO_PACKET = 33;
    internal const int CONNECTION_DETAIL_PACKET = 34;

    internal const  int NAME_PACKET = 40;
    internal const  int LOCATION_PACKET = 41;
    internal const  int ENERGY_PACKET = 42;
    internal const  int STRENGTH_PACKET = 43;
    internal const  int SPEED_PACKET = 44;
    internal const  int SHIELD_PACKET = 45;
    internal const  int SWORD_PACKET = 46;
    internal const  int QUICKSILVER_PACKET = 47;
    internal const  int MANA_PACKET = 48;
    internal const  int LEVEL_PACKET = 49;
    internal const  int GOLD_PACKET = 50;
    internal const  int GEMS_PACKET = 51;
    internal const  int CLOAK_PACKET = 52;
    internal const  int BLESSING_PACKET = 53;
    internal const  int CROWN_PACKET = 54;
    internal const  int PALANTIR_PACKET = 55;
    internal const  int RING_PACKET = 56;
    internal const  int VIRGIN_PACKET = 57;

    internal const String C_RESPONSE_PACKET = "1\0";
    internal const String C_CANCEL_PACKET = "2\0";
    internal const String C_PING_PACKET = "3\0";
    internal const String C_CHAT_PACKET = "4\0";
    internal const String C_EXAMINE_PACKET = "5\0";
    internal const String C_ERROR_PACKET = "6\0";
    internal const String C_SCOREBOARD_PACKET = "7\0";

    /* various colors */
    /*
        internal static  Color foregroundColor = new Color(10, 70, 17);
        internal static  Color backgroundColor = new Color(240, 236, 199);
        internal static  Color highlightColor = new Color(232, 220, 183);
    */
    internal static  Color foregroundColor = new Color(000, 51, 000);
    internal static  Color backgroundColor = new Color(255, 255, 204);
    internal static  Color highlightColor = new Color(204, 204, 153);

    /* i/o identifiers */
    internal const  int NO_REQUEST = 0;
    internal const int BUTTONS = 1;
    internal const int STRING_DLOG = 2;
    internal const int COORD_DLOG = 3;
    internal const int PLAYER_DLOG = 4;

    /* general text */
    internal static  String OK_LABEL = "OK";
    internal static  String CANCEL_LABEL = "Cancel";

    /* fonts */
    internal static  JavaFont MsgJavaFont = new JavaFont("TimesRoman", JavaFont.PLAIN, 12);
    internal static  JavaFont ChatJavaFont = new JavaFont("TimesRoman", JavaFont.PLAIN, 12);
    internal static  JavaFont JavaButtonJavaFont = new JavaFont("TimesRoman", JavaFont.PLAIN, 9);
    internal static  JavaFont UserJavaFont = new JavaFont("TimesRoman", JavaFont.PLAIN, 12);
    internal static  JavaFont CompassJavaFont = new JavaFont("Helvetica", JavaFont.BOLD, 12);
}
