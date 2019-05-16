using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace phantasiaclasses
{
    public class hack //: MonoBehaviour
    {
        phantasiaclasses.misc miscclass;
        phantasiaclasses.account accountclass;
        phantasiaclasses.tags tagsclass;
        phantasiaclasses.file fileclass;
        phantasiaclasses.io ioclass;


        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        static hack Instance;
        private hack()
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
            accountclass = account.GetInstance();
        }
        public static hack GetInstance()
        {
            hack instance = null;
            if (Instance != null)
            {
                instance = Instance;
            }
            else
            {
                instance = new hack();
            }
            return instance;
        }

            /************************************************************************
            /
            / FUNCTION NAME: hackclass.Do_caught_hack(c)
            /
            / FUNCTION: return a char specifying player type
            /
            / AUTHOR: Brian Kelly, 01/06/01
            /
            / ARGUMENTS:
            /       struct client_t c - pointer to the main client strcture
            /
            / RETURN VALUE: 
            /	char - character the describes the character
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

            internal void Do_caught_hack(client_t c, int theReason)
        {
            int hackCount, expiredAttempts;
            account_t theAccount = new account_t();
            account_mod_t theAccountMod = new account_mod_t();
            network_t theNetwork = new network_t();
            tag_t theTag = new tag_t();
            string string_buffer = ""; //[phantdefs.SZ_LINE], error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            int timeNow;
            bool rejectFlag;

            /* ban times in seconds- one hour, one day, one week, one month,
            four months, one year */
            int[] tagTimes = { 0, 3600, 86400, 604800, 2592000, 9676800, 31536000 };
            timeNow = CFUNCTIONS.GetUnixEpoch(DateTime.Now);
            rejectFlag = false;

            /* lock the hack lock to prevent these stats from updating by others */
            miscclass.Do_lock_mutex(c.realm.hack_lock);

            theAccount.hackCount = 0;

            /* find past triggers, start with the character, if it exists */
            /* skip the character, if it exists, it's going to be killed */
            if (c.accountLoaded)
            {

                /* load the account - there may be a hack since last load */
                if (accountclass.Do_look_account(c, c.lcaccount, ref theAccount) != 0)
                {

                    /* if previous offenses, lower count by 1 per month since */
                    if (theAccount.hackCount != 0)
                    {

                        theAccount.hackCount -= (timeNow - theAccount.lastHack) /
                        2592000;

                        if (theAccount.hackCount < 0)
                        {
                            theAccount.hackCount = 0;
                        }
                    }
                }
            }

            /* now get the information on the connection - it may not exist! */
            if (Do_look_network(c, c.network, theNetwork) != 0)
            {

                /* if previous offenses, lower count by 1 per month since */
                expiredAttempts = (timeNow - theNetwork.lastHack) / 2592000;

                if (theNetwork.hackCount != 0)
                {
                    theNetwork.hackCount -= expiredAttempts;

                    if (theNetwork.hackCount < 0)
                    {
                        theNetwork.hackCount = 0;
                    }
                }
            }
            else
            {
                CFUNCTIONS.strcpy(ref theNetwork.address, c.network);
                theNetwork.hackCount = 0;
            }

            /* find the highest number of hack and rejections attempts */
            if (theAccount.hackCount > theNetwork.hackCount)
            {
                hackCount = theAccount.hackCount;
            }
            else
            {
                hackCount = theNetwork.hackCount;
            }

            /* increment the hack count */
            ++hackCount;

            /* max ban is a year */
            if (hackCount > 6)
            {
                hackCount = 6;
            }

            /* save the account info */
            if (c.accountLoaded)
            {
                accountclass.Do_clear_account_mod(theAccountMod);
                theAccountMod.hack = true;
                theAccountMod.hackCount = hackCount;
                accountclass.Do_modify_account(c, c.lcaccount, null, theAccountMod);
            }

            /* update the address info */
            theNetwork.hackCount = hackCount;
            theNetwork.lastHack = timeNow;

            if (theNetwork.expires < timeNow + hackCount * 2592000)
            {
                theNetwork.expires = timeNow + hackCount * 2592000;
            }

            Do_update_network(c, theNetwork);

            /* prepare a tag for the account and address */
            theTag.type = (short)phantdefs.T_BAN;
            theTag.validUntil = timeNow + tagTimes[hackCount];
            theTag.affectNetwork = false;
            Do_get_hack_string(theReason, ref theTag.description);

            /* send it */
            tagsclass.Do_tag_self(c, theTag);
            miscclass.Do_unlock_mutex(c.realm.hack_lock);

            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: int Do_look_network(struct client_t *c, char *networkName, struct network_t *theNetwork)
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

        int Do_look_network(client_t c, string networkName, network_t theNetwork)
        {
            string string_buffer = ""; //[phantdefs.SZ_LINE];
            CLibFile.CFILE network_file;

            /* open the network file */
            int errno = 0;
            miscclass.Do_lock_mutex(c.realm.network_file_lock);
            if ((network_file = CLibFile.fopen(pathnames.NETWORK_FILE, "r", ref errno)) == null)
            {

                miscclass.Do_unlock_mutex(c.realm.network_file_lock);
                CFUNCTIONS.sprintf(ref string_buffer,
                        "[%s] CLibFile.fopen of %s failed in Do_look_network: %s.\n",
                        c.connection_id, pathnames.NETWORK_FILE, CFUNCTIONS.strerror(errno));

                fileclass.Do_log_error(string_buffer);
                return 0;
            }

            /* run through the network entries */
            while (CLibFile.fread(ref theNetwork, phantdefs.SZ_NETWORK, 1, network_file) == 1)
            {

                /* if this is the network */
                if (!CFUNCTIONS.strcmp(networkName, theNetwork.address))
                {

                    /* return with the good news */
                    CLibFile.fclose(network_file);
                    miscclass.Do_unlock_mutex(c.realm.network_file_lock);
                    return 1;
                }
            }

            /* close down and return a negative */
            CLibFile.fclose(network_file);
            miscclass.Do_unlock_mutex(c.realm.network_file_lock);

            theNetwork.hackCount = 0;
            theNetwork.muteCount = 0;

            return 0;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_update_network(struct client_t *c, struct network_t *theNetwork)
        /
        / FUNCTION: find location in player file of given name
        /
        / AUTHOR: Brian Kelly, 01/06/01
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

        bool Do_update_network(client_t c, network_t theNetwork)
        {
            network_t readNetwork = new network_t();
            CLibFile.CFILE network_file, temp_file;
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            int timeNow;
            bool found_flag;

            miscclass.Do_lock_mutex(c.realm.network_file_lock);

            /* get the time now */
            timeNow = CFUNCTIONS.GetUnixEpoch(DateTime.Now);
            found_flag = false;

            /* open a temp file to transfer records to */
            int errno = 0;
            if ((temp_file = CLibFile.fopen(pathnames.TEMP_NETWORK_FILE, "w", ref errno)) == null)
            {

                miscclass.Do_unlock_mutex(c.realm.network_file_lock);
                error_msg = CFUNCTIONS.sprintfSinglestring(
                        "[%s] CLibFile.fopen of %s failed in Do_update_network: %s.\n",
                        c.connection_id, pathnames.TEMP_NETWORK_FILE, CFUNCTIONS.strerror(errno));

                fileclass.Do_log_error(error_msg);
                return false;
            }

            errno = 0;
            if ((network_file = CLibFile.fopen(pathnames.NETWORK_FILE, "r", ref errno)) == null)
            {

                miscclass.Do_unlock_mutex(c.realm.network_file_lock);
                error_msg = CFUNCTIONS.sprintfSinglestring(
                        "[%s] CLibFile.fopen of %s failed in Do_update_network: %s\n",
                        c.connection_id, pathnames.NETWORK_FILE, CFUNCTIONS.strerror(errno));

                fileclass.Do_log_error(error_msg);
            }
            else
            {

                /* run through the network entries */
                while (CLibFile.fread(ref readNetwork, phantdefs.SZ_NETWORK, 1, network_file) == 1)
                {

                    /* if this is the network */
                    if (!CFUNCTIONS.strcmp(readNetwork.address, theNetwork.address))
                    {

                        found_flag = true;

                        /* write the passed strcture */
                        if (CLibFile.fwrite(theNetwork, ref phantdefs.SZ_NETWORK, 1, temp_file) != 1)
                        {

                            CLibFile.fclose(network_file);
                            CLibFile.fclose(temp_file);
                            CLibFile.remove(pathnames.TEMP_NETWORK_FILE);
                            miscclass.Do_unlock_mutex(c.realm.network_file_lock);

                            error_msg = CFUNCTIONS.sprintfSinglestring(
                                "[%s] CLibFile.fwrite of %s failed in Do_update_network: %s.\n",
                                c.connection_id, pathnames.TEMP_NETWORK_FILE, CFUNCTIONS.strerror(errno));

                            fileclass.Do_log_error(error_msg);
                            return false;
                        }

                        continue;
                    }
                    else
                    {

                        /* see if this network is still valid */
                        if (timeNow > readNetwork.expires)
                        {

                            /* log the deletion of the network */
                            error_msg = CFUNCTIONS.sprintfSinglestring(
                        "[%s] Deleted hack info of the network %s.\n",
                                    c.connection_id, readNetwork.address);

                            fileclass.Do_log(pathnames.CONNECTION_LOG, error_msg);

                            /* don't save this network */
                            continue;
                        }
                    }

                    /* write the network to the temp file */
                    if (CLibFile.fwrite(readNetwork, ref phantdefs.SZ_NETWORK, 1, temp_file) != 1)
                    {

                        CLibFile.fclose(network_file);
                        CLibFile.fclose(temp_file);
                        CLibFile.remove(pathnames.TEMP_NETWORK_FILE);
                        miscclass.Do_unlock_mutex(c.realm.network_file_lock);

                        error_msg = CFUNCTIONS.sprintfSinglestring(
                             "[%s] CLibFile.fwrite of %s failed in Do_update_network(2): %s.\n",
                             c.connection_id, pathnames.TEMP_NETWORK_FILE, CFUNCTIONS.strerror(errno));

                        fileclass.Do_log_error(error_msg);
                        return false;
                    }
                }

                CLibFile.fclose(network_file);
            }

            /* if we've not written in our network, we make a new entry */
            if (!found_flag)
            {

                /* write the new network to the temp file */
                if (CLibFile.fwrite(theNetwork, ref phantdefs.SZ_NETWORK, 1, temp_file) != 1)
                {

                    CLibFile.fclose(temp_file);
                    CLibFile.remove(pathnames.TEMP_NETWORK_FILE);
                    miscclass.Do_unlock_mutex(c.realm.network_file_lock);

                    error_msg = CFUNCTIONS.sprintfSinglestring(
                            "[%s] CLibFile.fwrite of %s failed in Do_update_network(3): %s.\n",
                            c.connection_id, pathnames.TEMP_NETWORK_FILE, CFUNCTIONS.strerror(errno));

                    fileclass.Do_log_error(error_msg);
                    return false;
                }
            }

            /* close the two files */
            CLibFile.fclose(temp_file);
            CLibFile.fclose(network_file); //unity: added

            /* delete the old character record */
            CLibFile.remove(pathnames.NETWORK_FILE);

            /* replace it with the temporary file */
            CLibFile.rename(pathnames.TEMP_NETWORK_FILE, pathnames.NETWORK_FILE);

            miscclass.Do_unlock_mutex(c.realm.network_file_lock);
            return true;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_tally_ip(struct client_t *c, bool connection, short badPasswords);
        /
        / FUNCTION: find location in player file of given name
        /
        / AUTHOR: Brian Kelly, 01/06/01
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

        internal void Do_tally_ip(client_t c, bool connection, short badPasswords)
        {
            connection_t connection_ptr = new connection_t();
            connection_t connection_ptr_ptr = new connection_t();
            int timeNow;

            miscclass.Do_lock_mutex(c.realm.connections_lock);
            timeNow = CFUNCTIONS.GetUnixEpoch(DateTime.Now);

            connection_ptr_ptr = c.realm.connections;

            /* run through the list looking for previous entries */
            while (connection_ptr_ptr != null)
            {

                connection_ptr = connection_ptr_ptr;

                /* is this the network we're talking about? */
                if (!CFUNCTIONS.strcmp(connection_ptr.theAddress, c.network))
                {

                    /* if we're adding a connection */
                    if (connection)
                    {

                        /* drop out expired connections */
                        Do_drop_expired(connection_ptr.connections,
                            ref connection_ptr.connectionCount, timeNow - 900);

                        /* see if this connection will be too many */
                        if (connection_ptr.connectionCount > 19)
                        {
                            connection_ptr.connectionCount = 0;

                            miscclass.Do_unlock_mutex(c.realm.connections_lock);

                            Do_caught_hack(c, phantdefs.H_CONNECTIONS);
                            return;
                        }

                        connection_ptr.connections[connection_ptr.connectionCount++]
                            = timeNow;

                        if (connection_ptr.eraseAt < timeNow + 600)
                        {
                            connection_ptr.eraseAt = timeNow + 600;
                        }
                    }

                    if (badPasswords != 0)
                    {

                        /* drop out expired missed passwords */
                        Do_drop_expired(connection_ptr.badPasswords,
                            ref connection_ptr.badPasswordCount, timeNow - 1800);

                        /* see if these additions will be too many */
                        if (connection_ptr.badPasswordCount + badPasswords > 9)
                        {
                            connection_ptr.badPasswordCount = 0;

                            miscclass.Do_unlock_mutex(c.realm.connections_lock);

                            Do_caught_hack(c, phantdefs.H_PASSWORDS);
                            return;
                        }

                        while (badPasswords > 0)
                        {

                            connection_ptr.badPasswords[
                                connection_ptr.badPasswordCount++] = timeNow;

                            --badPasswords;
                        }

                        if (connection_ptr.eraseAt < timeNow + 1800)
                        {
                            connection_ptr.eraseAt = timeNow + 1800;
                        }
                    }


                    miscclass.Do_unlock_mutex(c.realm.connections_lock);
                    return;
                }
                else
                {

                    /* see if this address should be deleted */
                    if (connection_ptr.eraseAt < timeNow)
                    {

                        /* delete the current entry and point to the next */
                        connection_ptr_ptr = connection_ptr.next;

                        //free((void*)connection_ptr);
                        connection_ptr = null;
                    }
                    else
                    {
                        /* increment to the next pointer */
                        connection_ptr_ptr = connection_ptr_ptr.next;
                    }
                }

            }

            /* no connection record for us, so make our own */
            connection_ptr = new connection_t();// (struct connection_t *) Do_malloc(phantdefs.SZ_CONNECTION);
            CFUNCTIONS.strcpy(ref connection_ptr.theAddress, c.network);
            connection_ptr.connectionCount = 0;
            connection_ptr.badPasswordCount = 0;

            /* if we're adding a connection */
            if (connection)
            {

                connection_ptr.connections[0] = timeNow;
                connection_ptr.connectionCount = 1;
                connection_ptr.eraseAt = timeNow + 600;
            }

            if (badPasswords != 0)
            {
                while (badPasswords > 0)
                {
                    connection_ptr.badPasswords[
                    connection_ptr.badPasswordCount++] = timeNow;

                    --badPasswords;
                }

                connection_ptr.eraseAt = timeNow + 1800;
            }

            /* put the new connection record in place */
            connection_ptr.next = c.realm.connections;
            c.realm.connections = connection_ptr;

            miscclass.Do_unlock_mutex(c.realm.connections_lock);
            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_drop_expired(int theArray[], int *elements, int eraseTime)
        /
        / FUNCTION: find location in player file of given name
        /
        / AUTHOR: Brian Kelly, 01/06/01
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

        void Do_drop_expired(int[] theArray, ref int elements, int dropBefore)
        {
            int i, j;

            /* the lowest elements will be bad */
            if (theArray[0] > dropBefore)
            {
                return;
            }

            /* if they're all bad, we don't need to move anything */
            if (theArray[elements - 1] < dropBefore)
            {
                elements = 0;
                return;
            }

            /* run through the elements to find non-expired time */
            for (i = 1; i < elements; i++)
            {
                if (theArray[i] > dropBefore)
                {
                    break;
                }
            }

            /* move the times down */
            for (j = i; j < elements; j++)
            {
                theArray[j - i] = theArray[j];
            }

            elements -= i;

            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_get_hack_string(int number, char *theString);
        /
        / FUNCTION: return a char specifying player type
        /
        / AUTHOR: Brian Kelly, 01/09/01
        /
        / ARGUMENTS:
        /       struct client_t c - pointer to the main client strcture
        /
        / RETURN VALUE: 
        /	char - character the describes the character
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

        void Do_get_hack_string(int number, ref string theString)
        {
            switch (number)
            {

                case phantdefs.H_SYSTEM:
                    CFUNCTIONS.strcpy(ref theString, "hacking the system");
                    break;

                case phantdefs.H_PASSWORDS:
                    CFUNCTIONS.strcpy(ref theString, "hacking passwords");
                    break;

                case phantdefs.H_CONNECTIONS:
                    CFUNCTIONS.strcpy(ref theString, "making excessive connections");
                    break;

                case phantdefs.H_KILLING:
                    CFUNCTIONS.strcpy(ref theString, "killing characters");
                    break;

                case phantdefs.H_PROFANITY:
                    CFUNCTIONS.strcpy(ref theString, "using profanity on channel 1");
                    break;

                case phantdefs.H_DISRESPECTFUL:
                    CFUNCTIONS.strcpy(ref theString, "disrespecting the wizards");
                    break;

                case phantdefs.H_FLOOD:
                    CFUNCTIONS.strcpy(ref theString, "sending too many long messages");
                    break;

                case phantdefs.H_SPAM:
                    CFUNCTIONS.strcpy(ref theString, "sending too many messages at once");
                    break;

                case phantdefs.H_WHIM:
                    CFUNCTIONS.strcpy(ref theString, "the wizard's whim");
                    break;

                default:
                    CFUNCTIONS.strcpy(ref theString, "listening to Neil Diamond");
                    break;
            }

            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_caught_spam(c, int theReason)
        /
        / FUNCTION: return a char specifying player type
        /
        / AUTHOR: Brian Kelly, 01/06/01
        /
        / ARGUMENTS:
        /       struct client_t c - pointer to the main client strcture
        /
        / RETURN VALUE: 
        /	char - character the describes the character
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

        internal void Do_caught_spam(client_t c, int theReason)
        {
            account_t theAccount = new account_t();
            account_mod_t theAccountMod = new account_mod_t();
            tag_t theTag = new tag_t();
            string string_buffer = ""; //[phantdefs.SZ_LINE], error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            int timeNow;
            bool banFlag;

            /* mute times; 1 min, 5 min, 20 min, 1 hour, 4 hours, 1 day, 3 days, 1 week */
            int[] muteTimes = { 60, 300, 1200, 7200, 28800, 86400, 259200, 604800 };

            timeNow = CFUNCTIONS.GetUnixEpoch(DateTime.Now);
            banFlag = false;

            /* lock the hack lock to prevent these stats from updating by others */
            miscclass.Do_lock_mutex(c.realm.hack_lock);

            /* find past triggers, start with the character, if it exists */
            /* skip the character, if it exists, it's going to be killed */
            if (accountclass.Do_look_account(c, c.lcaccount, ref theAccount) != 0)
            {

                /* if previous offenses, lower count by 1 per two weeks since */
                if (theAccount.muteCount != 0)
                {

                    theAccount.muteCount -= (timeNow - theAccount.lastMute) /
                    1296000;

                    if (theAccount.muteCount < 0)
                    {
                        theAccount.muteCount = 0;
                    }
                }
            }

            /* find the highest number of mutes */
            if (theAccount.muteCount > c.player.muteCount)
            {
                c.player.muteCount = theAccount.muteCount;
            }

            /* There's an overflow with the mute count somewhere -- EH */
            if (c.player.muteCount > 10 || c.player.muteCount < 0)
            {
                c.player.muteCount = 1;
            }

            /* increment the mute count */
            ++c.player.muteCount;

            /* max mute punishment is one week */
            if (c.player.muteCount > 7)
            {
                c.player.muteCount = 7;
                banFlag = true;
            }

            /* save the account info */
            if (c.accountLoaded)
            {
                accountclass.Do_clear_account_mod(theAccountMod);
                theAccountMod.mute = true;
                theAccountMod.muteCount = c.player.muteCount;
                accountclass.Do_modify_account(c, c.lcaccount, null, theAccountMod);
            }

            /* prepare a tag */
            theTag.type = (short)phantdefs.T_MUTE;
            theTag.validUntil = timeNow + muteTimes[c.player.muteCount];
            theTag.affectNetwork = false;
            theTag.contagious = false;
            Do_get_hack_string(theReason, ref theTag.description);

            /* send it */
            tagsclass.Do_tag_self(c, theTag);
            miscclass.Do_unlock_mutex(c.realm.hack_lock);

            string_buffer = CFUNCTIONS.sprintfSinglestring("%s has been muted by the server for %s.\n", c.modifiedName, theTag.description);
            ioclass.Do_broadcast(c, string_buffer);

            if (banFlag)
            {

                Do_caught_hack(c, theReason);
            }

            return;
        }
    }
}
