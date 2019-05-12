using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace phantasiaclasses
{
    public class file //: MonoBehaviour
    {
        phantasiaclasses.misc miscclass;
        phantasiaclasses.io ioclass;
        phantasiaclasses.character characterclass;
        phantasiaclasses.info infoclass;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        static file Instance;
        private file()
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
            miscclass = misc.GetInstance();
            characterclass = character.GetInstance();
            infoclass = info.GetInstance();
        }
        public static file GetInstance()
        {
            file instance = null;
            if (Instance != null)
            {
                instance = Instance;
            }
            else
            {
                instance = new file();
            }
            return instance;
        }

        /***************************************************************************
        / FUNCTION NAME: Do_purge_characters(void)
        /
        / FUNCTION: Handles error messages
        /
        / AUTHOR:  Brian Kelly, 4/22/99
        /
        / ARGUMENTS: none
        /
        / RETURN VALUE: none
        /
        / DESCRIPTION:
        /       Process arguments, initialize program, and loop forever processing
        /       player input.
        /
        ****************************************************************************/

        internal void Do_purge_characters()
        {
            Debug.Log("phantasia.file.Do_purge_characters");
            CLibFile.CFILE character_file;
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];

            /* open the character file */
            int errno = 0;
            Debug.LogError("CHAR FILE DEBUG: purging");
            if ((character_file = CLibFile.fopen(pathnames.CHARACTER_FILE, "w", ref errno)) == null)
            {
                error_msg = CFUNCTIONS.sprintfSinglestring(
                "[0.0.0.0:?] CLibFile.fopen of %s failed in Do_purge_characters: %s\n",
                pathnames.CHARACTER_FILE, CFUNCTIONS.strerror(errno));
                
                CLibFile.fclose(character_file);

                Do_log_error(error_msg);
                CFUNCTIONS.exit(phantdefs.CHARACTER_FILE_ERROR);
            }

            return;
        }


        /***************************************************************************
        / FUNCTION NAME: Do_purge_scoreboard(void)
        /
        / FUNCTION: Handles error messages
        /
        / AUTHOR:  Brian Kelly, 4/22/99
        /
        / ARGUMENTS: none
        /
        / RETURN VALUE: none
        /
        / DESCRIPTION:
        /       Process arguments, initialize program, and loop forever processing
        /       player input.
        /
        ****************************************************************************/

        internal void Do_purge_scoreboard()
        {
            Debug.Log("phantasia.file.Do_purge_scoreboard");
            CLibFile.CFILE scoreboard_file;
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];

            /* open the scoreboard file */
            int errno = 0;
            if ((scoreboard_file = CLibFile.fopen(pathnames.SCOREBOARD_FILE, "w", ref errno)) == null)
            {

                error_msg = CFUNCTIONS.sprintfSinglestring(
                "[0.0.0.0:?] CLibFile.fopen of %s failed in Do_purge_scoreboard: %s\n",
                pathnames.SCOREBOARD_FILE, CFUNCTIONS.strerror(errno));
                
                CLibFile.fclose(scoreboard_file);

                Do_log_error(error_msg);
                CFUNCTIONS.exit(phantdefs.SCOREBOARD_FILE_ERROR);
            }


            return;
        }


        /***************************************************************************
        / FUNCTION NAME: fileclass.Do_scoreboard_add(struct client_t *c, struct scoreboard_t *entry)
        /
        / FUNCTION: Handles error messages
        /
        / AUTHOR:  Brian Kelly, 4/22/99
        /
        / ARGUMENTS: none
        /
        / RETURN VALUE: none
        /
        / DESCRIPTION:
        /       Process arguments, initialize program, and loop forever processing
        /       player input.
        /
        ****************************************************************************/

        internal void Do_scoreboard_add(client_t c, scoreboard_t entry, bool showScore)
        {
            Debug.Log("phantasia.file.Do_scoreboard_add");
            CLibFile.CFILE scoreboard_file, temp_file;       /* for updating various files */
            scoreboard_t sb = new scoreboard_t();
            event_t event_ptr = new event_t();
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            int location;
            bool save_flag;

            miscclass.Do_lock_mutex(c.realm.scoreboard_lock);

            /* open the scoreboard file */
            int errno = 0;
            if ((scoreboard_file=CLibFile.fopen(pathnames.SCOREBOARD_FILE, "r", ref errno)) == null)
            {
                miscclass.Do_unlock_mutex(c.realm.scoreboard_lock);

                error_msg = CFUNCTIONS.sprintfSinglestring("[%s] CLibFile.fopen of %s failed in fileclass.Do_scoreboard_add: %s\n",
                   c.connection_id, pathnames.SCOREBOARD_FILE, CFUNCTIONS.strerror(errno));

                Do_log_error(error_msg);
                return;
            }

            errno = 0;
            /* open a tempoary file */
            if ((temp_file=CLibFile.fopen(pathnames.TEMP_SCOREBOARD_FILE, "w", ref errno)) == null) 
            {
                CLibFile.fclose(scoreboard_file);

                miscclass.Do_unlock_mutex(c.realm.scoreboard_lock);

                error_msg = CFUNCTIONS.sprintfSinglestring("[%s] CLibFile.fopen of %s failed in fileclass.Do_scoreboard_add(2): %s\n",
                    c.connection_id, pathnames.TEMP_SCOREBOARD_FILE, CFUNCTIONS.strerror(errno));

                Do_log_error(error_msg);
                return;
            }

            save_flag = true;
            location = 0;

            /* copy records over to the temp file */
            while (CLibFile.fread(ref sb, phantdefs.SZ_SCOREBOARD, 1, scoreboard_file) == 1)
            {

                /* if our player is higher level than our copy, put him in */
                if (save_flag && sb.level <= entry.level)
                {

                    if (CLibFile.fwrite(entry, ref phantdefs.SZ_SCOREBOARD, 1, temp_file) != 1)
                    {


                        CLibFile.fclose(scoreboard_file);

                        CLibFile.fclose(temp_file);

                        CLibFile.remove(pathnames.TEMP_CHARACTER_FILE);

                        miscclass.Do_unlock_mutex(c.realm.scoreboard_lock);

                        error_msg = CFUNCTIONS.sprintfSinglestring(
                 "[%s] CLibFile.fwrite of %s failed in fileclass.Do_scoreboard_add: %s\n",
                                c.connection_id, pathnames.TEMP_SCOREBOARD_FILE,
                     CFUNCTIONS.strerror(errno));

                        Do_log_error(error_msg);
                        return;
                    }
                    save_flag = false;
                }

                /* check that the record to save isn't too old */
                if (sb.level > phantdefs.SB_KEEP_ABOVE || sb.time > CFUNCTIONS.GetUnixEpoch(DateTime.Now) -
                        phantdefs.SB_KEEP_FOR)
                {

                    /* transfer over the record */
                    if (CLibFile.fwrite(sb, ref phantdefs.SZ_SCOREBOARD, 1, temp_file) != 1)
                    {
                        CLibFile.fclose(scoreboard_file);

                        CLibFile.fclose(temp_file);

                        CLibFile.remove(pathnames.TEMP_CHARACTER_FILE);

                        miscclass.Do_unlock_mutex(c.realm.scoreboard_lock);

                        error_msg = CFUNCTIONS.sprintfSinglestring("[%s] CLibFile.fwrite of %s failed in fileclass.Do_scoreboard_add(2): %s\n",
                              c.connection_id, pathnames.TEMP_SCOREBOARD_FILE, CFUNCTIONS.strerror(errno));

                        Do_log_error(error_msg);
                        return;
                    }
                }

                /* count this record if we haven't saved our character yet */
                if (save_flag)
                {
                    ++location;
                }
            }

            /* if no more scoreboard records, and we haven't written ours yet */
            if (save_flag)
            {

                /* write it now */
                if (CLibFile.fwrite(entry, ref phantdefs.SZ_SCOREBOARD, 1, temp_file) != 1)
                {


                    CLibFile.fclose(scoreboard_file);

                    CLibFile.fclose(temp_file);

                    CLibFile.remove(pathnames.TEMP_CHARACTER_FILE);

                    miscclass.Do_unlock_mutex(c.realm.scoreboard_lock);

                    error_msg = CFUNCTIONS.sprintfSinglestring(
                 "[%s] CLibFile.fwrite of %s failed in fileclass.Do_scoreboard_add(3): %s\n",
                            c.connection_id, pathnames.TEMP_SCOREBOARD_FILE, CFUNCTIONS.strerror(errno));

                    Do_log_error(error_msg);
                    return;
                }
            }

            /* close the file handles */
            CLibFile.fclose(scoreboard_file);
            CLibFile.fclose(temp_file);

            /* remove the scoreboard */
            CLibFile.remove(pathnames.SCOREBOARD_FILE); //unity: swapped places with remove call, to avoid sharing violation

            /* replace the old scoreboard file with the new one */
            CLibFile.rename(pathnames.TEMP_SCOREBOARD_FILE, pathnames.SCOREBOARD_FILE);

            miscclass.Do_unlock_mutex(c.realm.scoreboard_lock);

            /* display the scoreboard */
            if (showScore)
            {
                miscclass.Do_scoreboard(c, location, 1);
            }
        }


        /************************************************************************
        /
        / FUNCTION NAME: fileclass.Do_log(char *filename, char *message)
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

        internal void Do_log(string filename, string message)
        {
            if (filename != pathnames.ERROR_LOG) //unity: added
            {
                string msgcopy = message.Replace("\0", "£");
                msgcopy = msgcopy.Replace('\0', '£');
                Debug.Log("Thread " + System.Threading.Thread.CurrentThread.Name + ": phantasia.file.Do_log: || " + msgcopy + " ||");
            }

            CLibFile.CFILE log_file;
            int time_now;
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            string string_buffer = ""; //[phantdefs.SZ_LINE];

            time_now = CFUNCTIONS.GetUnixEpoch(DateTime.Now);
            CFUNCTIONS.ctime_r(time_now, ref string_buffer);
            miscclass.Do_truncstring(string_buffer);

            /* open the error log file */
            int errno = 0;
            if ((log_file = CLibFile.fopen(filename, "a", ref errno)) == null)
            {

                /* if the log file won't open, send and error */
                if (filename != pathnames.ERROR_LOG)
                {

                    error_msg = CFUNCTIONS.sprintfSinglestring(
                    "[?:?] Can not open log file %s in Do_log: %s.\n",
                    filename, CFUNCTIONS.strerror(errno));

                    Debug.Log(error_msg);
                    Do_log_error(error_msg);
                }

                /* unless it is the error log, then go to stdout */
                else
                {

                    CFUNCTIONS.printf("%s [?:?] Can not open error file %s in Do_log: %s.\n", string_buffer, pathnames.ERROR_LOG, CFUNCTIONS.strerror(errno));

                    CFUNCTIONS.printf("%s %s", string_buffer, message);
                }

                return;
            }

            /* print the error to the logfile */
            CLibFile.fprintf(log_file, "%s %s", string_buffer, message);

            /* close the logfile */
            CLibFile.fclose(log_file);

            /* return, regardless of error */
            return;
        }


        /***************************************************************************
        / FUNCTION NAME: Do_log_error(char *message)
        /
        / FUNCTION: Handles error messages
        /
        / AUTHOR:  Brian Kelly, 4/12/99
        /
        / ARGUMENTS:
        /       int error - the error code to be returned
        /       char *message - the error message to be printed
        /
        / RETURN VALUE: none
        /
        / DESCRIPTION:
        /       Process arguments, initialize program, and loop forever processing
        /       player input.
        /
        ****************************************************************************/

        internal void Do_log_error(string message)
        {
            //Debug.Log("phantasia.file.Do_log_error");
            Debug.Log("<color=red>Logging error: " + message + "</color>");

            Do_log(pathnames.ERROR_LOG, message);
        }


        /************************************************************************
        /
        / FUNCTION NAME: int Do_check_protected(struct client_t *c, char *theNetwork)
        /
        / FUNCTION: return a char specifying player type
        /
        / AUTHOR: Brian Kelly, 1/3/01
        /
        / ARGUMENTS:
        /       struct client_t c - pointer to the main client strcture
        /
        / RETURN VALUE:
        /       char - character the describes the character
        /
        / MODULES CALLED: CFUNCTIONS.strcpy(ref )
        /
        / DESCRIPTION:
        /       Return a string describing the player type.
        /       King, council, valar, supercedes other types.
        /       The first character of the string is '*' if the player
        /       has a crown.
        /       If 'shortflag' is true, return a 3 character string.
        /
        *************************************************************************/

        internal int Do_check_protected(client_t c, string theNetwork)
        {
            Debug.Log("phantasia.file.Do_check_protected");
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            string string_buffer = ""; //[phantdefs.SZ_LINE];
            CLibFile.CFILE theFile;

            /* open the protected file */
            int errno = 0;
            if ((theFile = CLibFile.fopen(pathnames.PROTECTED_FILE, "r", ref errno)) == null)
            {

                error_msg = CFUNCTIONS.sprintfSinglestring(
                "[0.0.0.0:?] CLibFile.fopen of %s failed in Do_check_protected: %s\n",
                        pathnames.PROTECTED_FILE, CFUNCTIONS.strerror(errno));

                Do_log_error(error_msg);
                return 0;
            }

            /* loop through the the addresses */
            while (CLibFile.fgets(ref string_buffer, phantdefs.SZ_FROM, theFile) != null)
            {

                miscclass.Do_truncstring(string_buffer);

                if (!CFUNCTIONS.strcmp(string_buffer, theNetwork))
                {

                    CLibFile.fclose(theFile);
                    return 1;
                }
            }

            CLibFile.fclose(theFile);
            return 0;
        }


        /************************************************************************
        /
        / FUNCTION NAME: fileclass.Do_restore_character(struct client_t *c)
        /
        / FUNCTION: find location in player file of given name
        /
        / AUTHOR: Brian Kelly, 01/17/01
        /
        / ARGUMENTS:
        /       char *name - name of character to look for
        /       struct player *playerp - pointer of structure to fill
        /
        / RETURN VALUE: location of player if found, -1 otherwise
        /
        / MODULES CALLED: CLibFile.fread(ref ), fseek(), cfunctions.strcmp()
        /
        / GLOBAL INPUTS: Wizard, *Playersfp
        /
        / GLOBAL OUTPUTS: none
        /
        / DESCRIPTION:
        /       Search the player file for the player of the given name.
        /       If player is found, fill structure with player data.
        /
        *************************************************************************/

        internal void Do_restore_character(client_t c)
        {
            Debug.Log("phantasia.file.Do_restore_character");
            player_t readPlayer = new player_t();
            history_t theHistory = new history_t();
            CLibFile.CFILE character_file;
            string characterName = ""; //[phantdefs.SZ_NAME], 
            string lcCharacterName = ""; //[phantdefs.SZ_NAME];
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            bool char_flag;

            if (ioclass.Do_string_dialog(c, ref characterName, phantdefs.SZ_NAME - 1,
                    "What is the name of the character to restore\n"))
            {


                ioclass.Do_send_clear(c);
                return;
            }

            ioclass.Do_send_clear(c);
            miscclass.Do_lowercase(ref lcCharacterName, characterName);
            if (characterclass.Do_look_character(c, lcCharacterName, ref readPlayer) != 0)
            {


                error_msg = CFUNCTIONS.sprintfSinglestring(
                    "A character named %s is already in the character file.\n",
                    characterName);


                ioclass.Do_send_line(c, error_msg);

                ioclass.Do_more(c);

                ioclass.Do_send_clear(c);
                return;
            }

            int errno = 0;
            if ((character_file = CLibFile.fopen(pathnames.SECONDARY_CHAR_FILE, "r", ref errno)) == null)
            {


                error_msg = CFUNCTIONS.sprintfSinglestring("CLibFile.fopen of %s failed: %s\n",
                    pathnames.SECONDARY_CHAR_FILE, CFUNCTIONS.strerror(errno));


                ioclass.Do_send_line(c, error_msg);

                ioclass.Do_more(c);

                ioclass.Do_send_clear(c);
                return;
            }

            char_flag = false;

            /* read each line of the character file */
            while (CLibFile.fread(ref readPlayer, phantdefs.SZ_PLAYER, 1, character_file) == 1)
            {

                /* if we find our character, copy it over */
                if (!CFUNCTIONS.strcmp(readPlayer.lcname, lcCharacterName))
                {
                    char_flag = true;
                    break;
                }
            }

            CLibFile.fclose(character_file);

            if (!char_flag)
            {


                error_msg = CFUNCTIONS.sprintfSinglestring("A character named %s was not found in %s.\n",
                    characterName, pathnames.SECONDARY_CHAR_FILE);


                ioclass.Do_send_line(c, error_msg);

                ioclass.Do_more(c);

                ioclass.Do_send_clear(c);
                return;
            }

            /* save the character */
            characterclass.Do_save_character(c, readPlayer);

            /* log this restore */
            CFUNCTIONS.sprintf(ref theHistory.description, "%s restored the character named %s\n", c.modifiedName, readPlayer.name);

            theHistory.date = CFUNCTIONS.GetUnixEpoch(DateTime.Now);
            theHistory.type = (short)phantdefs.T_ACCOUNT;
            CFUNCTIONS.strcpy(ref theHistory.name, readPlayer.parent_account);
            infoclass.Do_save_history(c, theHistory);

            theHistory.type = (short)phantdefs.T_ADDRESS;
            CFUNCTIONS.strcpy(ref theHistory.name, readPlayer.parent_network);
            infoclass.Do_save_history(c, theHistory);

            error_msg = CFUNCTIONS.sprintfSinglestring("[%s] %s restored\n", c.connection_id,
                    readPlayer.lcname);

            Do_log(pathnames.GAME_LOG, error_msg);

            ioclass.Do_send_line(c, "The character has been successfully restored.\n");
            ioclass.Do_more(c);
            ioclass.Do_send_clear(c);
            return;
        }

    }
}
