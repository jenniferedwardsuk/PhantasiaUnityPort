using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace phantasiaclasses
{
    public class tags //: MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        static tags Instance;
        private tags()
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
        public static tags GetInstance()
        {
            tags instance = null;
            if (Instance != null)
            {
                instance = Instance;
            }
            else
            {
                instance = new tags();
            }
            return instance;
        }

        phantasiaclasses.misc miscclass;
        phantasiaclasses.file fileclass;
        phantasiaclasses.restore restoreclass;
        phantasiaclasses.stats statsclass;
        phantasiaclasses.info infoclass;
        phantasiaclasses.io ioclass;
        phantasiaclasses.commands commandsclass;
        phantasiaclasses.socket socketclass;
        phantasiaclasses.itcombat itcombatclass;
        phantasiaclasses.fight fightclass;
        phantasiaclasses.treasure treasureclass;
        phantasiaclasses.eventsrc eventclass;
        phantasiaclasses.hack hackclass;
        phantasiaclasses.character characterclass;
        phantasiaclasses.account accountclass;
        phantasiaclasses.init initclass;


        string[] taggedTypes = { "machine #", "account ", "address ", "network " };
        string[] tagTypes = { "reject", "ban", "suicide", "mute", "prefix", "suffix" };
        string[] tagDescs = { "rejected", "banned", "ordered to commit suicide", "muted", "tagged", "tagged" };

        /************************************************************************
        /
        / FUNCTION NAME: tagsclass.Do_tag_self(client_t c, struct tag_t *theTag)
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

        internal void Do_tag_self(client_t c, tag_t theTag)
        {
            tagged_t theTagged = new tagged_t();
            history_t theHistory = new history_t();
            char[] error_msg = new char[phantdefs.SZ_ERROR_MESSAGE]; // = ""; //[phantdefs.SZ_ERROR_MESSAGE], 
            string string_buffer = ""; //[phantdefs.SZ_LINE];

            /* get the next tag number */
            miscclass.Do_lock_mutex(c.realm.tag_file_lock);
            theTag.number = c.realm.nextTagNumber++;
            miscclass.Do_unlock_mutex(c.realm.tag_file_lock);

            /* write the tag */
            Do_save_tag(c, theTag);

            /* get the time the ban expires */
            string err = new string(error_msg);
            CFUNCTIONS.ctime_r(theTag.validUntil, ref err);
            error_msg[CFUNCTIONS.strlen(new string(error_msg)) - 1] = '\0';

            /* log the tag creation */
            CFUNCTIONS.sprintf(ref string_buffer,
                "[%s] Created %s tag #%d for \"%s\" effective until %s.\n",
                c.connection_id, tagTypes[theTag.type], theTag.number,
                theTag.description, error_msg);

            fileclass.Do_log(pathnames.HACK_LOG, string_buffer);

            /* prepare a history entry */
            theHistory.date = CFUNCTIONS.GetUnixEpoch(DateTime.Now);

            CFUNCTIONS.sprintf(ref theHistory.description,
                "New %s tag #%d for \"%s\" effective until %s.\n",
                tagTypes[theTag.type], theTag.number, theTag.description,
                error_msg);

            /* prepare the tagged entry */
            theTagged.tagNumber = theTag.number;
            theTagged.validUntil = theTag.validUntil;

            /* tag the machine if we have one */
            if (c.machineID != 0)
            {

                /* associate the machine to the tag */
                theTagged.type = (short)phantdefs.T_MACHINE;

                CFUNCTIONS.sprintf(ref theTagged.name, "%ld", c.machineID);

                Do_save_tagged(c, theTagged);

                /* log the tag creation */
                string_buffer = CFUNCTIONS.sprintfSinglestring("[%s] Attached machine #%d to tag #%d.\n",
                            c.connection_id, c.machineID, theTag.number);


                fileclass.Do_log(pathnames.HACK_LOG, string_buffer);

                /* add this to the history file */
                theHistory.type = (short)phantdefs.T_MACHINE;

                CFUNCTIONS.sprintf(ref theHistory.name, "%ld", c.machineID);

                infoclass.Do_save_history(c, theHistory);
            }

            /* if there is an account */
            if (c.accountLoaded)
            {

                /* associate the account to the tag */
                theTagged.type = (short)phantdefs.T_ACCOUNT;

                CFUNCTIONS.strcpy(ref theTagged.name, c.lcaccount);

                Do_save_tagged(c, theTagged);

                /* log the tag creation */
                string_buffer = CFUNCTIONS.sprintfSinglestring("[%s] Attached account %s to tag #%d.\n",
                                c.connection_id, c.lcaccount, theTag.number);


                fileclass.Do_log(pathnames.HACK_LOG, string_buffer);

                /* add this to the history file */
                theHistory.type = (short)phantdefs.T_ACCOUNT;

                CFUNCTIONS.strcpy(ref theHistory.name, c.lcaccount);

                infoclass.Do_save_history(c, theHistory);
            }

            /* see if this tag should affect network */
            if (theTag.affectNetwork)
            {

                /* associate the network to the tag */
                theTagged.type = (short)phantdefs.T_NETWORK;

                CFUNCTIONS.strcpy(ref theTagged.name, c.network);

                Do_save_tagged(c, theTagged);

                /* log the tag creation */
                string_buffer = CFUNCTIONS.sprintfSinglestring("[%s] Attached network %s to tag #%d.\n",
                                    c.connection_id, c.network, theTag.number);


                fileclass.Do_log(pathnames.HACK_LOG, string_buffer);

                /* add this to the history file */
                theHistory.type = (short)phantdefs.T_NETWORK;

                CFUNCTIONS.strcpy(ref theHistory.name, c.network);

                infoclass.Do_save_history(c, theHistory);
            }

            /* associate the IP to the tag */
            theTagged.type = (short)phantdefs.T_ADDRESS;
            CFUNCTIONS.strcpy(ref theTagged.name, c.IP);

            /* address can be associated for 1 hour maximum */
            if (theTagged.validUntil > theHistory.date + 3600)
            {
                theTagged.validUntil = theHistory.date + 3600;
            }

            Do_save_tagged(c, theTagged);

            /* log the tag creation */
            string_buffer = CFUNCTIONS.sprintfSinglestring("[%s] Attached address %s to tag #%d.\n",
                                    c.connection_id, c.IP, theTag.number);

            fileclass.Do_log(pathnames.HACK_LOG, string_buffer);

            /* add this to the history file */
            theHistory.type = (short)phantdefs.T_ADDRESS;
            CFUNCTIONS.strcpy(ref theHistory.name, c.IP);
            infoclass.Do_save_history(c, theHistory);

            /* now implement the tag */
            Do_implement_tag(c, theTag);

            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_save_tag(client_t c, struct tag_t *theTag)
        /
        / FUNCTION: find location in player file of given name
        /
        / AUTHOR: Brian Kelly, 01/09/01
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

        int Do_save_tag(client_t c, tag_t theTag)
        {
            CLibFile.CFILE tag_file;
            bool found_flag;
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];

            miscclass.Do_lock_mutex(c.realm.tag_file_lock);

            int errno = 0;
            if ((tag_file = CLibFile.fopen(pathnames.TAG_FILE, "a", ref errno)) == null)
            {

                miscclass.Do_unlock_mutex(c.realm.tag_file_lock);
                error_msg = CFUNCTIONS.sprintfSinglestring(
                        "[%s] CLibFile.fopen of %s failed in Do_save_tag: %s\n",
                        c.connection_id, pathnames.TAG_FILE, CFUNCTIONS.strerror(errno));

                fileclass.Do_log_error(error_msg);
                return 0;
            }

            /* write the tag to the tag file */
            if (CLibFile.fwrite(theTag, ref phantdefs.SZ_TAG, 1, tag_file) != 1)
            {

                CLibFile.fclose(tag_file);
                CLibFile.remove(pathnames.TAG_FILE);
                miscclass.Do_unlock_mutex(c.realm.tag_file_lock);

                error_msg = CFUNCTIONS.sprintfSinglestring(
                        "[%s] CLibFile.fwrite of %s failed in Do_save_tag: %s.\n",
                        c.connection_id, pathnames.TEMP_TAG_FILE, CFUNCTIONS.strerror(errno));

                fileclass.Do_log_error(error_msg);
                return 0;
            }

            /* close the two files */
            CLibFile.fclose(tag_file);
            miscclass.Do_unlock_mutex(c.realm.tag_file_lock);

            return 1;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_look_tag(client_t c, struct tag_t *theTag, int tagNumber)
        /
        / FUNCTION: find location in player file of given name
        /
        / AUTHOR: Brian Kelly, 01/09/01
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

        int Do_look_tag(client_t c, tag_t theTag, int tagNumber)
        {
            tag_t readTag = new tag_t();
            CLibFile.CFILE tag_file, temp_file;
            int timeNow;
            bool found_flag;
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];

            miscclass.Do_lock_mutex(c.realm.tag_file_lock);

            /* get the time now */
            timeNow = CFUNCTIONS.GetUnixEpoch(DateTime.Now);
            found_flag = false;

            /* open a temp file to transfer records to */
            int errno = 0;
            if ((temp_file = CLibFile.fopen(pathnames.TEMP_TAG_FILE, "w", ref errno)) == null)
            {

                miscclass.Do_unlock_mutex(c.realm.tag_file_lock);
                error_msg = CFUNCTIONS.sprintfSinglestring(
                        "[%s] CLibFile.fopen of %s failed in Do_look_tag: %s.\n",
                        c.connection_id, pathnames.TEMP_TAG_FILE, CFUNCTIONS.strerror(errno));

                fileclass.Do_log_error(error_msg);
                return 0;
            }

            errno = 0;
            if ((tag_file = CLibFile.fopen(pathnames.TAG_FILE, "r", ref errno)) == null)
            {

                CLibFile.fclose(temp_file);
                miscclass.Do_unlock_mutex(c.realm.tag_file_lock);
                error_msg = CFUNCTIONS.sprintfSinglestring(
                        "[%s] CLibFile.fopen of %s failed in Do_look_tag: %s\n",
                        c.connection_id, pathnames.TAG_FILE, CFUNCTIONS.strerror(errno));

                fileclass.Do_log_error(error_msg);
                return 0;
            }

            /* run through the tag entries */
            while (CLibFile.fread(ref readTag, phantdefs.SZ_TAG, 1, tag_file) == 1)
            {

                /* see if this tag is outdated */
                if (readTag.validUntil < timeNow)
                {

                    /* don't rewrite it */
                    continue;
                }

                /* this is the tag? */
                else if (readTag.number == tagNumber)
                {

                    /* copy over the tag information */
                    CFUNCTIONS.memcpy(ref theTag, readTag, phantdefs.SZ_TAG);
                    found_flag = true;
                }

                /* write the tag to the temp file */
                if (CLibFile.fwrite(readTag, ref phantdefs.SZ_TAG, 1, temp_file) != 1)
                {

                    CLibFile.fclose(tag_file);
                    CLibFile.fclose(temp_file);
                    CLibFile.remove(pathnames.TEMP_TAG_FILE);
                    miscclass.Do_unlock_mutex(c.realm.tag_file_lock);

                    error_msg = CFUNCTIONS.sprintfSinglestring(
                            "[%s] CLibFile.fwrite of %s failed in Do_look_tag: %s.\n",
                            c.connection_id, pathnames.TEMP_TAG_FILE, CFUNCTIONS.strerror(errno));

                    fileclass.Do_log_error(error_msg);
                    return 0;
                }
            }

            /* close the two files */
            CLibFile.fclose(temp_file);
            CLibFile.fclose(tag_file);

            /* delete the old character record */
            CLibFile.remove(pathnames.TAG_FILE);

            /* replace it with the temporary file */
            CLibFile.rename(pathnames.TEMP_TAG_FILE, pathnames.TAG_FILE);

            miscclass.Do_unlock_mutex(c.realm.tag_file_lock);

            if (found_flag)
            {
                return 1;
            }

            return 0;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_save_tagged(client_t c, struct tag_t *theTagged)
        /
        / FUNCTION: find location in player file of given name
        /
        / AUTHOR: Brian Kelly, 01/11/01
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

        int Do_save_tagged(client_t c, tagged_t theTagged)
        {
            CLibFile.CFILE tagged_file;
            bool found_flag;
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            int newTime;

            miscclass.Do_lock_mutex(c.realm.tagged_file_lock);

            int errno = 0;
            if ((tagged_file = CLibFile.fopen(pathnames.TAGGED_FILE, "a", ref errno)) == null)
            {

                miscclass.Do_unlock_mutex(c.realm.tagged_file_lock);
                error_msg = CFUNCTIONS.sprintfSinglestring(
                        "[%s] CLibFile.fopen of %s failed in Do_save_tagged: %s\n",
                        c.connection_id, pathnames.TAGGED_FILE, CFUNCTIONS.strerror(errno));

                fileclass.Do_log_error(error_msg);
                return 0;
            }

            /* write the tagged to the tag file */
            if (CLibFile.fwrite(theTagged, ref phantdefs.SZ_TAGGED, 1, tagged_file) != 1)
            {

                CLibFile.fclose(tagged_file);
                CLibFile.remove(pathnames.TAGGED_FILE);
                miscclass.Do_unlock_mutex(c.realm.tagged_file_lock);

                error_msg = CFUNCTIONS.sprintfSinglestring(
                        "[%s] CLibFile.fwrite of %s failed in Do_save_tagged: %s.\n",
                        c.connection_id, pathnames.TEMP_TAGGED_FILE, CFUNCTIONS.strerror(errno));

                fileclass.Do_log_error(error_msg);
                return 0;
            }

            /* close the two files */
            CLibFile.fclose(tagged_file);
            miscclass.Do_unlock_mutex(c.realm.tagged_file_lock);

            return 1;
        }


        /************************************************************************
        /
        / FUNCTION NAME: struct tagged_list_t *Do_look_tagged(client_t c)
        /
        / FUNCTION: find location in player file of given name
        /
        / AUTHOR: Brian Kelly, 01/09/01
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

        tagged_list_t Do_look_tagged(client_t c)
        {
            tagged_t readTagged = new tagged_t();
            tagged_list_t taggedList = new tagged_list_t(), taggedList_ptr = new tagged_list_t();
            CLibFile.CFILE tagged_file, temp_file;
            int timeNow;
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE], 
            string machineID = "";//[16];

            miscclass.Do_lock_mutex(c.realm.tagged_file_lock);

            /* get the time now */
            timeNow = CFUNCTIONS.GetUnixEpoch(DateTime.Now);
            taggedList = null;
            CFUNCTIONS.sprintf(ref machineID, "%ld", c.machineID);

            /* open a temp file to transfer records to */
            int errno = 0;
            if ((temp_file = CLibFile.fopen(pathnames.TEMP_TAGGED_FILE, "w", ref errno)) == null)
            {

                miscclass.Do_unlock_mutex(c.realm.tagged_file_lock);
                error_msg = CFUNCTIONS.sprintfSinglestring(
                        "[%s] CLibFile.fopen of %s failed in Do_look_tagged: %s.\n",
                        c.connection_id, pathnames.TEMP_TAGGED_FILE, CFUNCTIONS.strerror(errno));

                fileclass.Do_log_error(error_msg);
                return null;
            }

            /* open the tagged file */
            errno = 0;
            if ((tagged_file = CLibFile.fopen(pathnames.TAGGED_FILE, "r", ref errno)) == null)
            {

                CLibFile.fclose(temp_file);
                CLibFile.remove(pathnames.TEMP_TAGGED_FILE);
                miscclass.Do_unlock_mutex(c.realm.tagged_file_lock);
                error_msg = CFUNCTIONS.sprintfSinglestring(
                        "[%s] CLibFile.fopen of %s failed in Do_look_tagged: %s\n",
                        c.connection_id, pathnames.TAGGED_FILE, CFUNCTIONS.strerror(errno));

                fileclass.Do_log_error(error_msg);
                return null;
            }

            /* run through the tag entries */
            while (CLibFile.fread(ref readTagged, phantdefs.SZ_TAGGED, 1, tagged_file) == 1)
            {

                /* see if this tag is outdated */
                if (readTagged.validUntil < timeNow)
                {

                    /* don't rewrite it */
                    continue;
                }

                /* is this tagged entry pertain to us? */
                else if (

                    /* see if this machine is us */
                    (readTagged.type == phantdefs.T_MACHINE && c.machineID > 0 &&

                    !CFUNCTIONS.strcmp(readTagged.name, machineID)) ||

                    /* see if this account is ours */
                    (readTagged.type == phantdefs.T_ACCOUNT && c.accountLoaded &&

                    !CFUNCTIONS.strcmp(readTagged.name, c.account)) ||

                    /* see if this is the character's account */
                    (readTagged.type == phantdefs.T_ACCOUNT && c.characterLoaded &&

                    !CFUNCTIONS.strcmp(readTagged.name, c.player.parent_account)) ||

                    /* see if this is our address */
                    (readTagged.type == phantdefs.T_ADDRESS && !CFUNCTIONS.strcmp(readTagged.name, c.IP)) ||

                    /* see if this is our network */
                    (readTagged.type == phantdefs.T_NETWORK && !CFUNCTIONS.strcmp(readTagged.name, c.network)))
                {

                    /* create a new list strcture to pass the info back */
                    taggedList_ptr = new tagged_list_t();// (tagged_list_t) Do_malloc(phantdefs.SZ_TAGGED_LIST);

                    CFUNCTIONS.memcpy(ref taggedList_ptr.theTagged, readTagged, phantdefs.SZ_TAGGED);

                    /* put the strcture into the list */
                    taggedList_ptr.next = taggedList;
                    taggedList = taggedList_ptr;
                }

                /* write the tag to the temp file */
                if (CLibFile.fwrite(readTagged, ref phantdefs.SZ_TAGGED, 1, temp_file) != 1)
                {

                    CLibFile.fclose(tagged_file);
                    CLibFile.fclose(temp_file);
                    CLibFile.remove(pathnames.TEMP_TAGGED_FILE);
                    miscclass.Do_unlock_mutex(c.realm.tagged_file_lock);

                    error_msg = CFUNCTIONS.sprintfSinglestring(
                            "[%s] CLibFile.fwrite of %s failed in Do_look_tagged: %s.\n",
                            c.connection_id, pathnames.TEMP_TAGGED_FILE, CFUNCTIONS.strerror(errno));

                    fileclass.Do_log_error(error_msg);
                    return taggedList;
                }
            }

            /* close the two files */
            CLibFile.fclose(temp_file);
            CLibFile.fclose(tagged_file);

            /* delete the old character record */
            CLibFile.remove(pathnames.TAGGED_FILE);

            /* replace it with the temporary file */
            CLibFile.rename(pathnames.TEMP_TAGGED_FILE, pathnames.TAGGED_FILE);

            miscclass.Do_unlock_mutex(c.realm.tagged_file_lock);
            return taggedList;
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_check_tags(client_t c)
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

        internal int Do_check_tags(client_t c)
        {
            tagged_list_t taggedList = new tagged_list_t(), taggedList_ptr = new tagged_list_t();
            tag_t theTag = new tag_t();
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE], 
            string string_buffer = ""; //[phantdefs.SZ_LINE];

            /* read all the tags that pertain to us */
            taggedList = Do_look_tagged(c);

            /* see if this player inherits any tags, skip if wiz backdoor */
            if (c.wizaccount == null || c.wizaccount[0] == '\0')
            {
                Do_check_inherit(c, taggedList);
            }

            /* run through the list of tagged recalled */
            while (taggedList != null)
            {

                if (Do_look_tag(c, theTag, taggedList.theTagged.tagNumber) != 0)
                {

                    /* implement the tag */
                    Do_implement_tag(c, theTag);
                }

                taggedList_ptr = taggedList;
                taggedList = taggedList.next;

                taggedList_ptr = null; //free((void*) taggedList_ptr);
            }

            return 0;
        }


        /************************************************************************
        /
        / FUNCTION NAME: struct tagged_list_t *Do_check_inherit(client_t c)
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

        void Do_check_inherit(client_t c, tagged_list_t list_ptr)
        {
            tagged_sort_t the_sort, sort_ptr;

            the_sort = null;

            /* run though the tagged list */
            while (list_ptr != null)
            {

                /* run through the sort list */
                sort_ptr = the_sort;
                while (sort_ptr != null)
                {

                    /* see if this list entry is this sort structure */
                    if (sort_ptr.tag == list_ptr.theTagged.tagNumber)
                    {

                        /* point the sort strcture to this entry */
                        sort_ptr.tagged[list_ptr.theTagged.type] =
                            list_ptr.theTagged;

                        break;
                    }

                    sort_ptr = sort_ptr.next;
                }

                /* if we found no sort structure */
                if (sort_ptr == null)
                {

                    /* create one */
                    sort_ptr = new tagged_sort_t(); // (tagged_sort_t) Do_malloc(phantdefs.SZ_TAGGED_SORT);
                    sort_ptr.tag = list_ptr.theTagged.tagNumber;
                    sort_ptr.tagged[0] = null;
                    sort_ptr.tagged[1] = null;
                    sort_ptr.tagged[2] = null;
                    sort_ptr.tagged[3] = null;
                    sort_ptr.tagged[list_ptr.theTagged.type] = list_ptr.theTagged;

                    /* put it in place */
                    sort_ptr.next = the_sort;
                    the_sort = sort_ptr;
                }

                list_ptr = list_ptr.next;
            }

            /* run through the sorted lists */
            while (the_sort != null)
            {

                /* accounts pass to IP and Machine */
                if (the_sort.tagged[phantdefs.T_ACCOUNT] != null)
                {

                    /* pass to the address if needed */
                    if (the_sort.tagged[phantdefs.T_ADDRESS] == null)
                    {

                        Do_inherit_tag(c, the_sort.tagged[phantdefs.T_ACCOUNT], phantdefs.T_ADDRESS);
                    }

                    /* pass to the machine if needed */
                    if (c.machineID != 0 && the_sort.tagged[phantdefs.T_MACHINE] == null)
                    {

                        Do_inherit_tag(c, the_sort.tagged[phantdefs.T_ACCOUNT], phantdefs.T_MACHINE);
                    }
                }

                /* IP passes to the machine */
                else if (the_sort.tagged[phantdefs.T_ADDRESS] != null)
                {

                    /* pass to the machine if needed */
                    if (c.machineID != 0 && the_sort.tagged[phantdefs.T_MACHINE] == null)
                    {

                        Do_inherit_tag(c, the_sort.tagged[phantdefs.T_ADDRESS], phantdefs.T_MACHINE);
                    }
                }

                sort_ptr = the_sort.next;

                the_sort = null; // free((void*) the_sort);
                the_sort = sort_ptr;
            }
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_inherit_tag(client_t c, struct tagged_t *source, int targetType)
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

        void Do_inherit_tag(client_t c, tagged_t source, int targetType)
        {
            tagged_t theTagged = new tagged_t();
            tag_t theTag = new tag_t();
            history_t theHistory = new history_t();
            char[] string_buffer = new char[phantdefs.SZ_LINE]; //[phantdefs.SZ_LINE], 
            string sourceName = ""; //[phantdefs.SZ_FROM + 10];
            string targetName = ""; //[phantdefs.SZ_FROM + 10];
            int maxDuration;

            /* load the tag */
            Do_look_tag(c, theTag, source.tagNumber);

            /* create a new tagged */
            theTagged.tagNumber = source.tagNumber;
            theTagged.type = (short)targetType;

            /* prepare the history entry */
            theHistory.date = CFUNCTIONS.GetUnixEpoch(DateTime.Now);
            theHistory.type = (short)targetType;

            /* Find the name for the target machine */
            switch (targetType)
            {

                case phantdefs.T_MACHINE:


                    CFUNCTIONS.sprintf(ref targetName, "Machine #%d", c.machineID);

                    CFUNCTIONS.sprintf(ref theTagged.name, "%d", c.machineID);

                    CFUNCTIONS.sprintf(ref theHistory.name, "%d", c.machineID);
                    theTagged.validUntil = source.validUntil;
                    break;

                case phantdefs.T_ACCOUNT:


                    CFUNCTIONS.sprintf(ref targetName, "Account %s", c.lcaccount);

                    CFUNCTIONS.strcpy(ref theTagged.name, c.lcaccount);

                    CFUNCTIONS.strcpy(ref theHistory.name, c.lcaccount);
                    theTagged.validUntil = source.validUntil;
                    break;

                case phantdefs.T_ADDRESS:


                    CFUNCTIONS.sprintf(ref targetName, "Address %s", c.IP);

                    CFUNCTIONS.strcpy(ref theTagged.name, c.IP);

                    CFUNCTIONS.strcpy(ref theHistory.name, c.IP);
                    maxDuration = theHistory.date + 3600;

                    /* IPs can only be tagged for an hour */
                    if (source.validUntil < maxDuration)
                    {
                        theTagged.validUntil = source.validUntil;
                    }
                    else
                    {
                        theTagged.validUntil = maxDuration;
                    }
                    break;

                case phantdefs.T_NETWORK:


                    CFUNCTIONS.sprintf(ref targetName, "Network %s", c.network);

                    CFUNCTIONS.strcpy(ref theTagged.name, c.network);

                    CFUNCTIONS.strcpy(ref theHistory.name, c.network);
                    theTagged.validUntil = source.validUntil;
                    break;
            }

            Do_save_tagged(c, theTagged);

            /* get the time the ban expires */
            string str = new string(string_buffer);
            CFUNCTIONS.ctime_r(theTag.validUntil, ref str);
            string_buffer[CFUNCTIONS.strlen(new string(string_buffer)) - 1] = '\0';

            CFUNCTIONS.sprintf(ref theHistory.description,
                    "Inherited %d tag #%d from %s%s for \"%s\" effective until %s.\n",
                    tagTypes[theTag.type], theTag.number, taggedTypes[source.type],
                source.name, theTag.description, string_buffer);

            infoclass.Do_save_history(c, theHistory);

            string_buffer = CFUNCTIONS.sprintfSinglestring("[%s] %s inherited %s tag #%d from %s%s.\n",
                c.connection_id, targetName, tagTypes[theTag.type],
                theTag.number, taggedTypes[source.type], source.name).ToCharArray();

            fileclass.Do_log(pathnames.HACK_LOG, new string(string_buffer));
        }


        /************************************************************************
        /
        / FUNCTION NAME: Do_implement_tag(client_t c, struct tag_t *theTag)
        /
        / FUNCTION: return a char specifying player type
        /
        / AUTHOR: Brian Kelly, 01/14/01
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

        void Do_implement_tag(client_t c, tag_t theTag)
        {
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            string string_buffer = ""; //[phantdefs.SZ_LINE];
            event_t event_ptr = new event_t();

            /* now implement the tag */
            switch (theTag.type)
            {

                case phantdefs.T_BAN:

                    /* add this event to the connection log */
                    error_msg = CFUNCTIONS.sprintfSinglestring("[%s] Connection banned by tag #%d.\n",
                        c.connection_id, theTag.number);

                    fileclass.Do_log(pathnames.CONNECTION_LOG, error_msg);

                    /* send a message to the user */
                    socketclass.Do_send_error(c,
                    "This location is no longer permitted to play Phantasia.\n");

                    if (c.characterLoaded == true)
                    {
                        c.run_level = (short)phantdefs.SAVE_AND_EXIT;
                    }
                    else
                    {
                        c.run_level = phantdefs.EXIT_THREAD;
                    }

                    break;

                case phantdefs.T_SUICIDE:

                    if (c.characterLoaded)
                    {
                        event_ptr = eventclass.Do_create_event();
                        event_ptr.type = (short)phantdefs.DEATH_EVENT;
                        event_ptr.to = c.game;
                        event_ptr.from = c.game;
                        event_ptr.arg3 = phantdefs.K_SUICIDE;
                        eventclass.Do_send_event(event_ptr);
                    }
                    break;

                case phantdefs.T_MUTE:

                    if (c.characterLoaded)
                    {

                        /* send a message to the user */
                        string_buffer = CFUNCTIONS.sprintfSinglestring("Your characters are muted due to %s.\n",
                        theTag.description);

                        /* send off an error dialog */
                        ioclass.Do_dialog(c, string_buffer);
                    }

                    c.muteUntil = theTag.validUntil;
                    break;

                case phantdefs.T_PREFIX:

                    if (c.characterLoaded)
                    {
                        CFUNCTIONS.strcpy(ref string_buffer, theTag.description);
                        CFUNCTIONS.strcat(ref string_buffer, " ");

                        CFUNCTIONS.strcat(ref string_buffer, c.player.name);

                        if (c.characterAnnounced)
                        {
                            characterclass.Do_send_specification(c, phantdefs.REMOVE_PLAYER_EVENT);
                        }

                        CFUNCTIONS.strncpy(ref c.modifiedName, string_buffer, phantdefs.SZ_NAME - 1);

                        if (c.characterAnnounced)
                        {
                            characterclass.Do_send_specification(c, phantdefs.ADD_PLAYER_EVENT);

                            statsclass.Do_name(c);
                        }
                    }
                    c.tagUntil = theTag.validUntil;
                    break;

                case phantdefs.T_SUFFIX:

                    if (c.characterLoaded)
                    {

                        CFUNCTIONS.strcpy(ref string_buffer, c.player.name);
                        CFUNCTIONS.strcat(ref string_buffer, " ");
                        CFUNCTIONS.strcat(ref string_buffer, theTag.description);

                        if (c.characterAnnounced)
                        {
                            characterclass.Do_send_specification(c, phantdefs.REMOVE_PLAYER_EVENT);
                        }

                        CFUNCTIONS.strncpy(ref c.modifiedName, string_buffer, phantdefs.SZ_NAME - 1);

                        if (c.characterAnnounced)
                        {
                            characterclass.Do_send_specification(c, phantdefs.ADD_PLAYER_EVENT);

                            statsclass.Do_name(c);
                        }
                    }
                    c.tagUntil = theTag.validUntil;
                    break;
            }

            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: tagsclass.Do_create_tag(client_t c, int tagType)
        /
        / FUNCTION: return a char specifying player type
        /
        / AUTHOR: Brian Kelly, 01/14/01
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

        internal void Do_create_tag(client_t c, int tagType)
        {
            button_t buttons = new button_t();
            event_t eventPtr = new event_t();
            tag_t theTag = new tag_t(), tagPtr = new tag_t();
            tagged_t theTagged = new tagged_t();
            history_t theHistory = new history_t();
            long answer = -1;
            char[] error_msg = new char[phantdefs.SZ_ERROR_MESSAGE]; //[phantdefs.SZ_ERROR_MESSAGE];
            string string_buffer = ""; //[phantdefs.SZ_LINE];
            string tagee = ""; //[phantdefs.SZ_NAME];
            int i;
            int timeNow;

            int[] tagTimes = { 3600, 10800, 86400, 259200, 604800, 2592000, 7776000, 31536000 };

            /* start filling out the tag */
            timeNow = CFUNCTIONS.GetUnixEpoch(DateTime.Now);
            theTag.type = (short)tagType;
            theTag.affectNetwork = false;
            theTag.contagious = false;

            string_buffer = CFUNCTIONS.sprintfSinglestring("What life would you like this %s to have? (shorter times under cantrip)\n",
                tagTypes[tagType]);

            ioclass.Do_send_line(c, string_buffer);

            CFUNCTIONS.strcpy(ref buttons.button[0], "1 hour\n");
            CFUNCTIONS.strcpy(ref buttons.button[1], "3 hours\n");
            CFUNCTIONS.strcpy(ref buttons.button[2], "1 day\n");
            CFUNCTIONS.strcpy(ref buttons.button[3], "3 days\n");
            CFUNCTIONS.strcpy(ref buttons.button[4], "1 week\n");
            CFUNCTIONS.strcpy(ref buttons.button[5], "1 month\n");
            CFUNCTIONS.strcpy(ref buttons.button[6], "3 months\n");
            CFUNCTIONS.strcpy(ref buttons.button[7], "1 year\n");
            ioclass.Do_clear_buttons(buttons, 8);

            if (ioclass.Do_buttons(c, ref answer, buttons) != phantdefs.S_NORM)
            {
                ioclass.Do_send_clear(c);
                return;
            }

            ioclass.Do_send_clear(c);

            if (answer > 7 || answer < 0)
            {

                error_msg = CFUNCTIONS.sprintfSinglestring(
                        "[%s] Returned non-option in tagsclass.Do_create_tags.\n",
                        c.connection_id).ToCharArray();

                fileclass.Do_log_error(new string(error_msg));
                hackclass.Do_caught_hack(c, phantdefs.H_SYSTEM);
                return;
            }

            /* convert to a time */
            theTag.validUntil = timeNow + tagTimes[answer];

            if (tagType == phantdefs.T_PREFIX || tagType == phantdefs.T_SUFFIX)
            {


                CFUNCTIONS.sprintf(ref string_buffer,
                        "What %s would you like to add to this character's name?\n",
                        tagTypes[tagType]);
            }
            else
            {


                CFUNCTIONS.sprintf(ref string_buffer,
                    "This %s is being created for: (e.g. \"claiming N'Sync rules\".\n",
                    tagTypes[tagType]);
            }

            if (ioclass.Do_string_dialog(c, ref theTag.description, phantdefs.SZ_LINE - 1, string_buffer))
            {
                return;
            }

            /* find out where it goes */
            string_buffer = CFUNCTIONS.sprintfSinglestring("What do you wish to %s?\n", tagTypes[tagType]);
            ioclass.Do_send_line(c, string_buffer);

            CFUNCTIONS.strcpy(ref buttons.button[0], "Player\n");
            CFUNCTIONS.strcpy(ref buttons.button[1], "Machine\n");
            CFUNCTIONS.strcpy(ref buttons.button[2], "Account\n");
            CFUNCTIONS.strcpy(ref buttons.button[3], "Address\n");
            CFUNCTIONS.strcpy(ref buttons.button[4], "Network\n");
            ioclass.Do_clear_buttons(buttons, 5);
            CFUNCTIONS.strcpy(ref buttons.button[7], "Cancel\n");

            if (ioclass.Do_buttons(c, ref answer, buttons) != phantdefs.S_NORM || answer == 7)
            {

                ioclass.Do_send_clear(c);
                return;
            }

            ioclass.Do_send_clear(c);
            if (answer > 5 || answer < 0)
            {

                error_msg = CFUNCTIONS.sprintfSinglestring(
                        "[%s] Returned non-option in tagsclass.Do_create_tags(2).\n",
                        c.connection_id).ToCharArray();

                fileclass.Do_log_error(new string(error_msg));
                hackclass.Do_caught_hack(c, phantdefs.H_SYSTEM);
                return;
            }

            switch (answer)
            {

                case 0:

                    string_buffer = CFUNCTIONS.sprintfSinglestring("Who do you wish to %s?\n", tagTypes[tagType]);

                    if (ioclass.Do_player_dialog(c, string_buffer, tagee) != phantdefs.S_NORM)
                    {
                        return;
                    }

                    /* convert the tag to an object */
                    tagPtr = new tag_t();// (tag_t) Do_malloc(phantdefs.SZ_TAG);

                    CFUNCTIONS.memcpy(ref tagPtr, theTag, phantdefs.SZ_TAG);

                    /* create the event */
                    eventclass.Do_create_event();
                    eventPtr.type = (short)phantdefs.TAG_EVENT;
                    eventPtr.from = c.game;
                    if (tagPtr.type == phantdefs.T_MUTE)
                    {
                        eventPtr.arg3 = phantdefs.T_MUTE;
                    }
                    eventPtr.arg4 = tagPtr;

                    /* send off the event */
                    if (eventclass.Do_send_character_event(c, eventPtr, tagee) == 0)
                    {
                        eventPtr = null; // free((void*) eventPtr);

                        tagPtr = null; // free((void*) tagPtr);
                        ioclass.Do_send_line(c, "That character just left the game.\n");
                        ioclass.Do_more(c);

                        ioclass.Do_send_clear(c);
                        return;
                    }

                    ioclass.Do_send_line(c, "The tag has been sent.\n");
                    ioclass.Do_more(c);
                    ioclass.Do_send_clear(c);
                    return;

                case 1:

                    string_buffer = CFUNCTIONS.sprintfSinglestring("What machine number do you wish to %s?\n",
                    tagTypes[tagType]);

                    /* associate the machine to the tag */
                    theTagged.type = phantdefs.T_MACHINE;
                    theHistory.type = phantdefs.T_MACHINE;

                    break;

                case 2:

                    string_buffer = CFUNCTIONS.sprintfSinglestring("What account do you wish to %s?\n",
                    tagTypes[tagType]);

                    /* associate the account to the tag */
                    theTagged.type = phantdefs.T_ACCOUNT;
                    theHistory.type = phantdefs.T_ACCOUNT;

                    break;

                case 3:

                    string_buffer = CFUNCTIONS.sprintfSinglestring("What address do you wish to %s?\n",
                    tagTypes[tagType]);

                    /* associate the address to the tag */
                    theTagged.type = phantdefs.T_ADDRESS;
                    theHistory.type = phantdefs.T_ADDRESS;

                    break;

                case 4:

                    string_buffer = CFUNCTIONS.sprintfSinglestring("What network do you wish to %s?\n",
                    tagTypes[tagType]);

                    /* associate the network to the tag */
                    theTagged.type = phantdefs.T_NETWORK;
                    theHistory.type = phantdefs.T_NETWORK;

                    break;

                default:

                    error_msg = CFUNCTIONS.sprintfSinglestring("[%s] Returned non-option %ld in tagsclass.Do_create_tag.\n",
                            c.connection_id, answer).ToCharArray();

                    fileclass.Do_log_error(new string(error_msg));
                    hackclass.Do_caught_hack(c, phantdefs.H_SYSTEM);
                    return;
            }

            /* get the item to tag */
            if ((ioclass.Do_string_dialog(c, ref tagee, phantdefs.SZ_NAME - 1, string_buffer) ? 1 : 0)
                != phantdefs.S_NORM)
            {

                return;
            }

            /* get the next tag number */
            miscclass.Do_lock_mutex(c.realm.tag_file_lock);
            theTag.number = c.realm.nextTagNumber++;
            miscclass.Do_unlock_mutex(c.realm.tag_file_lock);

            /* write the tag */
            Do_save_tag(c, theTag);

            /* fill out the tagged and file it */
            miscclass.Do_lowercase(ref theTagged.name, tagee);
            theTagged.tagNumber = theTag.number;
            theTagged.validUntil = theTag.validUntil;
            Do_save_tagged(c, theTagged);

            /* put together something for the logs */
            string str = new string(error_msg);
            CFUNCTIONS.ctime_r(theTagged.validUntil, ref str);
            error_msg[CFUNCTIONS.strlen(new string(error_msg)) - 1] = '\0';
            theHistory.date = timeNow;
            CFUNCTIONS.strcpy(ref theHistory.name, theTagged.name);

            CFUNCTIONS.sprintf(ref theHistory.description,
                "%s created %s tag #%d for \"%s\" effective until %s.\n",
                    c.modifiedName, tagTypes[tagType], theTag.number,
                theTag.description, error_msg);

            infoclass.Do_save_history(c, theHistory);

            CFUNCTIONS.sprintf(ref string_buffer,
            "[%s] %s created %s tag #%d for \"%s\" effective until %s.\n",
            c.connection_id, c.modifiedName, tagTypes[tagType],
            theTag.number, theTag.description, error_msg);

            fileclass.Do_log(pathnames.HACK_LOG, string_buffer);

            string_buffer = CFUNCTIONS.sprintfSinglestring("[%s] Attached %s%s to tag #%d.\n", c.connection_id,
                taggedTypes[tagType], theTagged.name, theTag.number);

            fileclass.Do_log(pathnames.HACK_LOG, string_buffer);

            ioclass.Do_send_line(c, "The tag has been sent.\n");
            ioclass.Do_more(c);
            ioclass.Do_send_clear(c);
            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: tagsclass.Do_create_minitag(client_t c, int tagType)
        /
        / FUNCTION: allow trusted players to tag other players for short periods
        /
        / AUTHOR: Brian Kelly, 01/14/01
        /
        / ARGUMENTS:
        /       struct client_t c - pointer to the main client strcture
        /
        / RETURN VALUE: 
        /
        / MODULES CALLED: CFUNCTIONS.strcpy(ref )
        /
        / DESCRIPTION:
        /
        *************************************************************************/

        internal void Do_create_minitag(client_t c, int tagType)
        {
            button_t buttons = new button_t();
            event_t eventPtr = new event_t();
            tag_t theTag = new tag_t(), tagPtr = new tag_t();
            tagged_t theTagged = new tagged_t();
            history_t theHistory = new history_t();
            long answer = -1;
            char[] error_msg = new char[phantdefs.SZ_ERROR_MESSAGE]; // = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            string string_buffer = ""; //[phantdefs.SZ_LINE];
            string tagee = ""; //[phantdefs.SZ_NAME];
            int i;
            int timeNow;

            int[] tagTimes = { 60, 300, 900, 1800, 3600, 10800 };

            /* start filling out the tag */
            timeNow = CFUNCTIONS.GetUnixEpoch(DateTime.Now);
            theTag.type = (short)tagType;
            theTag.affectNetwork = false;
            theTag.contagious = false;

            string_buffer = CFUNCTIONS.sprintfSinglestring("How long would you like this %s to be?\n",
                tagTypes[tagType]);

            ioclass.Do_send_line(c, string_buffer);

            CFUNCTIONS.strcpy(ref buttons.button[0], "1 min\n");
            CFUNCTIONS.strcpy(ref buttons.button[1], "5 min\n");
            CFUNCTIONS.strcpy(ref buttons.button[2], "15 min\n");
            CFUNCTIONS.strcpy(ref buttons.button[3], "30 min\n");
            if (tagType != phantdefs.T_BAN)
            {
                CFUNCTIONS.strcpy(ref buttons.button[4], "1 hour\n");
                CFUNCTIONS.strcpy(ref buttons.button[5], "3 hours\n");
                ioclass.Do_clear_buttons(buttons, 6);
            }
            else
            {
                ioclass.Do_clear_buttons(buttons, 4);
            }
            CFUNCTIONS.strcpy(ref buttons.button[7], "Cancel\n");

            if (ioclass.Do_buttons(c, ref answer, buttons) != phantdefs.S_NORM || answer == 7)
            {
                ioclass.Do_send_clear(c);
                return;
            }

            ioclass.Do_send_clear(c);

            if (answer > 7 || answer < 0)
            {

                error_msg = CFUNCTIONS.sprintfSinglestring(
                        "[%s] Returned non-option in tagsclass.Do_create_minitags.\n",
                        c.connection_id).ToCharArray();

                fileclass.Do_log_error(new string(error_msg));
                hackclass.Do_caught_hack(c, phantdefs.H_SYSTEM);
                return;
            }

            /* convert to a time */
            theTag.validUntil = timeNow + tagTimes[answer];

            if (tagType == phantdefs.T_PREFIX || tagType == phantdefs.T_SUFFIX)
            {


                CFUNCTIONS.sprintf(ref string_buffer,
                        "What %s would you like to add to this character's name?\n",
                        tagTypes[tagType]);
            }
            else
            {


                CFUNCTIONS.sprintf(ref string_buffer,
                    "This %s is being created for: (e.g. \"being a moron\".\n",
                    tagTypes[tagType]);
            }

            if (ioclass.Do_string_dialog(c, ref theTag.description, phantdefs.SZ_LINE - 1, string_buffer))
            {
                return;
            }

            string_buffer = CFUNCTIONS.sprintfSinglestring("Who do you wish to %s?\n", tagTypes[tagType]);

            if (ioclass.Do_player_dialog(c, string_buffer, tagee) != phantdefs.S_NORM)
            {
                return;
            }

            /* convert the tag to an object */
            tagPtr = new tag_t();// (tag_t) Do_malloc(phantdefs.SZ_TAG);
            CFUNCTIONS.memcpy(ref tagPtr, theTag, phantdefs.SZ_TAG);

            /* create the event */
            eventclass.Do_create_event();
            eventPtr.type = (short)phantdefs.TAG_EVENT;
            eventPtr.from = c.game;
            if (tagPtr.type == phantdefs.T_MUTE)
            {
                eventPtr.arg3 = phantdefs.T_MUTE;
            }
            eventPtr.arg4 = tagPtr;

            /* send off the event */
            if (eventclass.Do_send_character_event(c, eventPtr, tagee) == 0)
            {
                eventPtr = null; // free((void*) eventPtr);
                tagPtr = null; // free((void*) tagPtr);
                ioclass.Do_send_line(c, "That character just left the game.\n");
                ioclass.Do_more(c);
                ioclass.Do_send_clear(c);
                return;
            }

            /* get the time the ban expires */
            string err = new string(error_msg);
            CFUNCTIONS.ctime_r(theTag.validUntil, ref err);
            error_msg[CFUNCTIONS.strlen(new string(error_msg)) - 1] = '\0';

            CFUNCTIONS.sprintf(ref string_buffer,
            "[%s] %s created %s tag on %s for \"%s\" effective until %s.\n",
            c.connection_id, c.modifiedName, tagTypes[tagType], tagee,
            theTag.description, error_msg);

            fileclass.Do_log(pathnames.CANTRIP_LOG, string_buffer);

            ioclass.Do_send_line(c, "The cantrip has been sent.\n");

            if ((tagType != phantdefs.T_PREFIX) && (tagType != phantdefs.T_SUFFIX))
            {

                CFUNCTIONS.sprintf(ref string_buffer,
                "%s was %s by %s for %s.\n",
                    tagee, tagDescs[tagType], c.modifiedName, theTag.description);
                ioclass.Do_broadcast(c, string_buffer);

            }
            return;
        }

        /************************************************************************
        /
        / FUNCTION NAME: int Do_remove_tagged(client_t c, int tageeType, char *tageeName, int tagNumber)
        /
        / FUNCTION: find location in player file of given name
        /
        / AUTHOR: Brian Kelly, 01/19/01
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

        int Do_remove_tagged(client_t c, int tageeType, string tageeName, int tagNumber)
        {
            tagged_t readTagged = new tagged_t();
            CLibFile.CFILE tagged_file, temp_file;
            int timeNow;
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            bool foundTagged;

            miscclass.Do_lock_mutex(c.realm.tagged_file_lock);

            /* get the time now */
            timeNow = CFUNCTIONS.GetUnixEpoch(DateTime.Now);

            /* open a temp file to transfer records to */
            int errno = 0;
            if ((temp_file = CLibFile.fopen(pathnames.TEMP_TAGGED_FILE, "w", ref errno)) == null)
            {

                miscclass.Do_unlock_mutex(c.realm.tagged_file_lock);
                error_msg = CFUNCTIONS.sprintfSinglestring(
                        "[%s] CLibFile.fopen of %s failed in Do_remove_tagged: %s.\n",
                        c.connection_id, pathnames.TEMP_TAGGED_FILE, CFUNCTIONS.strerror(errno));

                fileclass.Do_log_error(error_msg);
                return 0;
            }

            errno = 0;
            if ((tagged_file = CLibFile.fopen(pathnames.TAGGED_FILE, "r", ref errno)) == null)
            {

                CLibFile.fclose(temp_file);
                miscclass.Do_unlock_mutex(c.realm.tagged_file_lock);
                error_msg = CFUNCTIONS.sprintfSinglestring(
                        "[%s] CLibFile.fopen of %s failed in Do_remove_tagged: %s\n",
                        c.connection_id, pathnames.TAGGED_FILE, CFUNCTIONS.strerror(errno));

                fileclass.Do_log_error(error_msg);
                return 0;
            }

            foundTagged = false;

            /* run through the tag entries */
            while (CLibFile.fread(ref readTagged, phantdefs.SZ_TAGGED, 1, tagged_file) == 1)
            {

                /* this is the a tagged strcture we want? */
                if (readTagged.tagNumber == tagNumber && readTagged.type == tageeType
                    && !CFUNCTIONS.strcmp(readTagged.name, tageeName))
                {

                    /* mark it as deleted */
                    foundTagged = true;

                    /* and don't re-write it */
                    continue;
                }

                /* see if this tag is outdated */
                if (readTagged.validUntil < timeNow)
                {

                    /* don't rewrite it */
                    continue;
                }

                /* write the tag to the temp file */
                if (CLibFile.fwrite(readTagged, ref phantdefs.SZ_TAGGED, 1, temp_file) != 1)
                {

                    CLibFile.fclose(tagged_file);
                    CLibFile.fclose(temp_file);
                    CLibFile.remove(pathnames.TEMP_TAGGED_FILE);
                    miscclass.Do_unlock_mutex(c.realm.tagged_file_lock);

                    error_msg = CFUNCTIONS.sprintfSinglestring(
                            "[%s] CLibFile.fwrite of %s failed in Do_remove_tagged: %s.\n",
                            c.connection_id, pathnames.TEMP_TAGGED_FILE, CFUNCTIONS.strerror(errno));

                    fileclass.Do_log_error(error_msg);
                    return 0;
                }
            }

            /* close the two files */
            CLibFile.fclose(temp_file);
            CLibFile.fclose(tagged_file);

            /* delete the old character record */
            CLibFile.remove(pathnames.TAGGED_FILE);

            /* replace it with the temporary file */
            CLibFile.rename(pathnames.TEMP_TAGGED_FILE, pathnames.TAGGED_FILE);

            miscclass.Do_unlock_mutex(c.realm.tagged_file_lock);

            if (foundTagged)
            {
                return 1;
            }

            return 0;
        }


        /************************************************************************
        /
        / FUNCTION NAME: tagsclass.Do_untag(client_t c)
        /
        / FUNCTION: return a char specifying player type
        /
        / AUTHOR: Brian Kelly, 01/18/01
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

        internal void Do_untag(client_t c)
        {
            button_t buttons = new button_t();
            history_t theHistory = new history_t();
            long tageeType = 0, tagNumber = 0;
            string error_msg = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            string string_buffer = ""; //[phantdefs.SZ_LINE];
            string tagee = ""; //[phantdefs.SZ_NAME];
            int i;

            ioclass.Do_send_line(c, "Do you wish to untag an account or an address?\n");

            CFUNCTIONS.strcpy(ref buttons.button[0], "Machine\n");
            CFUNCTIONS.strcpy(ref buttons.button[1], "Account\n");
            CFUNCTIONS.strcpy(ref buttons.button[2], "Address\n");
            CFUNCTIONS.strcpy(ref buttons.button[3], "Network\n");
            ioclass.Do_clear_buttons(buttons, 4);
            CFUNCTIONS.strcpy(ref buttons.button[7], "Cancel\n");

            if (ioclass.Do_buttons(c, ref tageeType, buttons) != phantdefs.S_NORM)
            {
                ioclass.Do_send_clear(c);
                return;
            }

            ioclass.Do_send_clear(c);
            switch (tageeType)
            {

                case 0:
                    tageeType = phantdefs.T_MACHINE;

                    if ((ioclass.Do_string_dialog(c, ref tagee, phantdefs.SZ_NAME - 1,
                        "What machine do you wish to untag?\n") ? 1 : 0) != phantdefs.S_NORM)
                    {

                        return;
                    }
                    break;

                case 1:
                    tageeType = phantdefs.T_ACCOUNT;

                    if ((ioclass.Do_string_dialog(c, ref tagee, phantdefs.SZ_NAME - 1,
                        "What account do you wish to untag?\n") ? 1 : 0) != phantdefs.S_NORM)
                    {

                        return;
                    }
                    break;

                case 2:
                    tageeType = phantdefs.T_ADDRESS;

                    if ((ioclass.Do_string_dialog(c, ref tagee, phantdefs.SZ_NAME - 1,
                        "What address do you wish to untag?\n") ? 1 : 0) != phantdefs.S_NORM)
                    {

                        return;
                    }
                    break;

                case 3:
                    tageeType = phantdefs.T_NETWORK;

                    if ((ioclass.Do_string_dialog(c, ref tagee, phantdefs.SZ_NAME - 1,
                        "What network do you wish to untag?\n") ? 1 : 0) != phantdefs.S_NORM)
                    {

                        return;
                    }
                    break;

                case 7:
                    return;
                    break;

                default:

                    error_msg = CFUNCTIONS.sprintfSinglestring("[%s] Returned non-option %ld in tagsclass.Do_untag.\n",
                            c.connection_id, tageeType);

                    fileclass.Do_log_error(error_msg);
                    hackclass.Do_caught_hack(c, phantdefs.H_SYSTEM);
                    return;
            }

            miscclass.Do_lowercase(ref tagee, tagee);

            if (ioclass.Do_long_dialog(c, ref tagNumber, "What is the tag number to remove?\n") != 0)
            {
                return;
            }

            if (Do_remove_tagged(c, (int)tageeType, tagee, (int)tagNumber) != 0)
            {

                theHistory.date = CFUNCTIONS.GetUnixEpoch(DateTime.Now);
                theHistory.type = (short)tageeType;
                CFUNCTIONS.strcpy(ref theHistory.name, tagee);


                CFUNCTIONS.sprintf(ref theHistory.description, "%s removed connection to tag #%d.\n",
                            c.modifiedName, tagNumber);

                infoclass.Do_save_history(c, theHistory);


                ioclass.Do_send_line(c, "The tag was successfully removed.\n");
            }
            else
            {


                ioclass.Do_send_line(c,
                    "I could not find the tag number or the account/address.\n");
            }

            ioclass.Do_more(c);
            ioclass.Do_send_clear(c);
            return;
        }


        /************************************************************************
        /
        / FUNCTION NAME: tagsclass.Do_remove_prefix_suffix(client_t c)
        /
        / FUNCTION: Remove a prefix or suffix tag from a player.
        /
        / AUTHOR: Renee Gehlbach, 8/6/2002
        /
        / ARGUMENTS:
        /       struct client_t c - pointer to the main client strcture
        /
        / RETURN VALUE: 
        /
        / MODULES CALLED: CFUNCTIONS.strcpy(ref ), strncpy(), characterclass.Do_send_specification(), statsclass.Do_name()
        /
        / DESCRIPTION:
        /       Remove a prefix or suffix from a player.
        /
        *************************************************************************/

        internal void Do_remove_prefix_suffix(client_t c)
        {
            string string_buffer = ""; //[phantdefs.SZ_NAME];

            if (c.characterLoaded)
            {
                if (CFUNCTIONS.strcmp(c.player.name, c.modifiedName))
                {

                    CFUNCTIONS.strcpy(ref string_buffer, c.player.name);
                    if (c.characterAnnounced)
                    {

                        characterclass.Do_send_specification(c, phantdefs.REMOVE_PLAYER_EVENT);
                    }

                    CFUNCTIONS.strncpy(ref c.modifiedName, string_buffer, phantdefs.SZ_NAME - 1);
                    if (c.characterAnnounced)
                    {

                        characterclass.Do_send_specification(c, phantdefs.ADD_PLAYER_EVENT);

                        statsclass.Do_name(c);
                    }
                }
            }
            return;
        }



        /************************************************************************
        /
        / FUNCTION NAME: int tagsclass.Do_tag_muted(client_t c, stuct tag_t *theMute)
        /
        / FUNCTION: Give muted players a tag of "the Silent" which lasts for the
        /           duration of their mute
        /
        / AUTHOR: Renee Gehlbach, 6/24/2002
        /
        / ARGUMENTS: client_t c - stu
        /            
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


        internal int Do_tag_muted(client_t c, tag_t theMute)
        {

            event_t eventPtr = new event_t();
            tag_t theTag = new tag_t(), tagPtr = new tag_t();
            string string_buffer = ""; //[phantdefs.SZ_LINE], 
            char[] error_msg = new char[phantdefs.SZ_LINE]; // ""; //[phantdefs.SZ_LINE];
            string tagee = ""; //[phantdefs.SZ_NAME];


            CFUNCTIONS.sprintf(ref tagee, c.player.name);

            /* fill out the tag */
            theTag.type = phantdefs.T_SUFFIX;
            theTag.validUntil = theMute.validUntil;
            theTag.affectNetwork = theMute.affectNetwork;
            theTag.contagious = theMute.contagious;

            CFUNCTIONS.sprintf(ref theTag.description, "the Silent");

            /* convert the tag to an object */
            tagPtr = new tag_t();// (tag_t) Do_malloc(phantdefs.SZ_TAG);

            CFUNCTIONS.memcpy(ref tagPtr, theTag, phantdefs.SZ_TAG);

            /* create the event */
            eventclass.Do_create_event();
            eventPtr.type = (short)phantdefs.TAG_EVENT;
            eventPtr.from = c.game;
            if (tagPtr.type == phantdefs.T_MUTE)
            {
                eventPtr.arg3 = phantdefs.T_MUTE;
            }
            eventPtr.arg4 = tagPtr;

            /* send off the event */
            if (eventclass.Do_send_character_event(c, eventPtr, tagee) == 0)
            {

                eventPtr = null; // free((void*) eventPtr);

                tagPtr = null; // free((void*) tagPtr);

                ioclass.Do_more(c);

                ioclass.Do_send_clear(c);
                return 1;
            }

            string err = new string(error_msg);
            CFUNCTIONS.ctime_r(theTag.validUntil, ref err);
            error_msg[CFUNCTIONS.strlen(new string(error_msg)) - 1] = '\0';

            return 0;
        }
    }
}
