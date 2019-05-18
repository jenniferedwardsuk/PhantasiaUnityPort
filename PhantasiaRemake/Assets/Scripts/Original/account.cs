using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using phantasiaclasses;
using System.Text.RegularExpressions;
using System.IO;

namespace phantasiaclasses
{
    public class account //: MonoBehaviour
    {

        phantasiaclasses.io ioclass;
        phantasiaclasses.file fileclass;
        phantasiaclasses.misc miscclass;
        phantasiaclasses.hack hackclass;
        phantasiaclasses.character characterclass;
        phantasiaclasses.tags tagsclass;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        static account Instance;
        private account()
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
        }
        public static account GetInstance()
        {
            account instance = null;
            if (Instance != null)
            {
                instance = Instance;
            }
            else
            {
                instance = new account();
            }
            return instance;
        }

        internal void Do_get_account(client_t c)
        {
            int rc = -1;
            int answer = -1;
            string error_msg = "";//;// = new char = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            button_t buttons = new button_t();

            ioclass.Do_send_line(c, "\n");

            ioclass.Do_send_line(c,
               "Do you need to sign up for an account, or are you ready to log in?\n");

            buttons.button[0] = "Sign Up\n";
            buttons.button[1] = "Log In\n";
            ioclass.Do_clear_buttons(buttons, 2);
            buttons.button[3] = "New Pass\n";
            buttons.button[6] = "Scoreboard\n";
            buttons.button[7] = "Quit\n";

            rc = ioclass.Do_buttons(c, ref answer, buttons);
            ioclass.Do_send_clear(c);

            if (rc != phantdefs.S_NORM)
            {
                answer = 7;
            }

            /* switch on the player's answer */
            switch (answer)
            {
                /* if the player needs a new account */
                case 0:
                    /* Let's make them an account */
                    Do_account_signup(c);
                    break;

                /* the player has an account */
                case 1:
                    /* The wish to log in */
                    Do_account_login(c);
                    break;

                /* the player has forgotten his account password */
                case 3:
                    /* go to account password reset */
                    Do_reset_account_password(c);
                    break;

                /* show the character the scoreboard */
                case 6:
                    miscclass.Do_scoreboard(c, 0);
                    break;

                /* exit if requested */
                case 7:
                    c.run_level = (short)phantdefs.EXIT_THREAD;
                    return;

                /* since it's a push button interface, any other answer is a hacker */
                default:
                    error_msg = c.connection_id + " Returned non-option " + answer + " in Do_get_account.\n";
                    fileclass.Do_log_error(error_msg);
                    hackclass.Do_caught_hack(c, phantdefs.H_SYSTEM);
                    break;
            }
        }

        void Do_account_signup(client_t c)
        {
            int rc = -1;
            int answer = -1;
            int i = -1;
            string string_buffer;// = new char = ""; //[phantdefs.SZ_LINE];
            account_t theAccount = new account_t();
            button_t buttons = new button_t();

            ioclass.Do_send_clear(c);
            ioclass.Do_send_line(c, "To get an account, you will need an account name, a password and an active e-mail address.  Are you ready to provide those things?\n \n- NOTE FROM UNITY PORT: emailing is not yet active, so an email address is unnecessary.\n");

            buttons.button[0] = "Yes\n";
            buttons.button[1] = "No\n";
            ioclass.Do_clear_buttons(buttons, 2);

            buttons.button[7] = "Quit\n";

            rc = ioclass.Do_buttons(c, ref answer, buttons);
            ioclass.Do_send_clear(c);

            if (rc != phantdefs.S_NORM)
            {
                answer = 7;
            }

            /* switch on the player's answer */
            switch (answer)
            {

                /* if the player is ready */
                case 0:
                    /* we continue on */
                    break;

                /* back to main account menu */
                case 1:
                    return;

                /* exit if requested */
                case 7:
                    c.run_level = (short)phantdefs.EXIT_THREAD;
                    return;

                /* since it's a push button interface,
                 any other answer is a hacker */
                default:
                    string_buffer = CFUNCTIONS.sprintfSinglestring("%s Returned non-option %d in Do_account_signup.\n", c.connection_id, answer);
                    fileclass.Do_log_error(string_buffer);
                    hackclass.Do_caught_hack(c, phantdefs.H_SYSTEM);
                    return;
            }

            ioclass.Do_send_line(c, "First we'll need an account name.  This name will be visible to other players and this is important, _characters can not use account names_.  Please only use letters, numbers and underscores, and do not use the name of the character you want to play.\n");

            ioclass.Do_more(c);
            ioclass.Do_send_clear(c);

            /* Get an account name */
            for (; ; )
            {

                /* name the new character */
                if (ioclass.Do_string_dialog(c, ref theAccount.name, phantdefs.SZ_NAME - 1, "What account name would you like?\n"))
                    return;

                /* convert the name to lower case and store it */
                miscclass.Do_lowercase(ref theAccount.lcname, theAccount.name);

                /* Check the name for illegal characters or profanity */
                if (characterclass.Do_approve_name(c, theAccount.lcname, theAccount.name, ref answer) != phantdefs.S_NORM)
                {
                    return;
                }

                if (UnityGameController.StopApplication) //time to quit
                {
                    Debug.Log("Thread " + System.Threading.Thread.CurrentThread.Name + " stopping in Do_account_signup");
                    return;
                }
                else
                {
                    System.Threading.Thread.Sleep(33); //30fps
                }

                /* move on to the next stage */
                if (answer > 0)
                {
                    break;
                }
            }

            ioclass.Do_send_line(c, "Now think of a password.  You may use any characters you like.  Make it a good one so people can't guess it!\n");

            ioclass.Do_more(c);
            ioclass.Do_send_clear(c);

            /* Get a password for the account */
            if (!ioclass.Do_new_password(c, theAccount.password, "account"))
            {
                characterclass.Do_release_name(c, theAccount.lcname);
                return;
            }

            ioclass.Do_send_line(c, "Finally, I need your e-mail address.  I will use it to send you a confirmation code as well as password resets you request, but nothing else.\n \n- NOTE FROM UNITY PORT: emailing is not yet active. Provide any text in 'a@b.com' format to proceed.\n");

            ioclass.Do_more(c);
            ioclass.Do_send_clear(c);

            /* Get an account name */
            for (; ; )
            {
                if (UnityGameController.StopApplication) //time to quit
                {
                    Debug.Log("Thread " + System.Threading.Thread.CurrentThread.Name + " stopping in Do_account_signup");
                    return;
                }
                else
                {
                    System.Threading.Thread.Sleep(33); //30fps
                }

                /* get an e-mail address */
                if (ioclass.Do_string_dialog(c, ref theAccount.email, phantdefs.SZ_FROM - 1, "What is your e-mail address.\n"))
                {
                    characterclass.Do_release_name(c, theAccount.lcname);
                    return;
                }

                if (Do_approve_email(c, theAccount, ref answer) != phantdefs.S_NORM)
                {
                    characterclass.Do_release_name(c, theAccount.lcname);
                    return;
                }

                /* continue on a positive answer */
                if (answer > 0)
                {
                    break;
                }
            }

            ioclass.Do_send_clear(c);
            ioclass.Do_send_line(c, "Please re-check the information.\n");

            string_buffer = CFUNCTIONS.sprintfSinglestring("Account name: %s\n", theAccount.name);
            ioclass.Do_send_line(c, string_buffer);

            string_buffer = CFUNCTIONS.sprintfSinglestring("E-mail address: %s\n", theAccount.email);
            ioclass.Do_send_line(c, string_buffer);

            ioclass.Do_send_line(c, "Do I have everything correct?\n");

            long longAnswer = (long)answer;
            if (ioclass.Do_yes_no(c, ref longAnswer) != phantdefs.S_NORM || longAnswer == 1)
            {
                answer = (int)longAnswer;
                characterclass.Do_release_name(c, theAccount.lcname);
                Do_release_email(c, theAccount.lcemail);
                return;
            }

            ioclass.Do_send_clear(c);

            /* create a confirmation code */
            miscclass.Do_create_password(ref theAccount.confirmation);

            /* call the script to e-mail the confirmation code */
            string_buffer = CFUNCTIONS.sprintfSinglestring("%s %s %s %s\n", pathnames.NEW_ACCOUNT_SCRIPT, theAccount.name, theAccount.confirmation, theAccount.email);

            /* if the mail send fails */
            rc = -1; //added
            if (rc == CFUNCTIONS.system(string_buffer, theAccount.name, new string(theAccount.confirmation), theAccount.email))
            {
                string_buffer = CFUNCTIONS.sprintfSinglestring("[%s] New account e-mail failed with a code of %d in Do_account_signup.\n",
                c.connection_id, rc);

                fileclass.Do_log_error(string_buffer);

                ioclass.Do_send_line(c, "An error occurred while trying to send the confirmation e-mail.  Please contact the game administrator and ask about this message.\n");

                ioclass.Do_more(c);

                characterclass.Do_release_name(c, theAccount.lcname);

                Do_release_email(c, theAccount.lcemail);
                return;
            }

            //TimeSpan TimeSinceStart = DateTime.Now - new DateTime(1970, 01, 01);

            /* start filling in the account strcture */
            theAccount.parent_network = c.network;
            theAccount.date_created = CFUNCTIONS.time(null);
            theAccount.last_load = CFUNCTIONS.time(null);
            theAccount.last_reset = CFUNCTIONS.time(null);
            theAccount.bad_passwords = 0;
            theAccount.login_count = 0;
            theAccount.hackCount = 0;
            theAccount.muteCount = 0;
            theAccount.rejectCount = 0;

            /* save the account */
            Do_save_account(c, theAccount);

            string_buffer = CFUNCTIONS.sprintfSinglestring("[%s] Created account %s with e-mail address %s.\n", c.connection_id, theAccount.lcname, theAccount.email);

            fileclass.Do_log(pathnames.CONNECTION_LOG, string_buffer);

            string formattedConfCode = new string(theAccount.confirmation);
            formattedConfCode = formattedConfCode.Substring(0, CFUNCTIONS.strlen(formattedConfCode));
            ioclass.Do_send_line(c, "Your account has been created and your confirmation code has been e-mailed to the address you provided.  To activate the account, log in with the information you provided, but have the confirmation code handy as you will need it. \n \n- NOTE FROM UNITY PORT: emailing is not yet active. Your confirmation code is: " + formattedConfCode + "\n");

            ioclass.Do_more(c);
            ioclass.Do_send_clear(c);
            ioclass.Do_send_line(c, "Welcome to Phantasia, and good luck in the realm.\n");
            ioclass.Do_more(c);
            ioclass.Do_send_clear(c);
            return;
        }


        int Do_approve_email(client_t c, account_t theAccount, ref int answer)
        {
            int len, i;
            bool at_flag, no_punct, good_address;
            string string_buffer = "";// = new char = ""; //[phantdefs.SZ_LINE];
            account_t readAccount = new account_t();
            linked_list_t list_ptr;
            CLibFile.CFILE account_file;

            /* see if the e-mail address looks okay */
            at_flag = false;
            no_punct = true;
            good_address = true;
            len = CFUNCTIONS.strlen(theAccount.email);//.Length;
            for (i = 0; i < len; i++)
            {
                if (theAccount.email[i] == '@' || theAccount.email[i] == '.' || theAccount.email[i] == '-')
                {

                    /* if the previous was punctuation or the start */
                    if (no_punct)
                    {
                        good_address = false;
                        break;
                    }

                    no_punct = true;

                    /* if this is the last character */
                    if (i + 1 == len)
                    {
                        good_address = false;
                        break;
                    }

                    if (theAccount.email[i] == '@')
                    {
                        /* if we've already seen an '@' */
                        if (at_flag)
                        {
                            good_address = false;
                            break;
                        }
                        else
                        {
                            at_flag = true;
                        }
                    }
                }
                else if (theAccount.email[i] == '_')
                {
                    /* do nothing - here because addresses may have underscores
                    at the beginning or end */
                }
                /* approve only letters and numbers */
                else if (CFUNCTIONS.isalnum(theAccount.email[i]))
                {
                    no_punct = false;
                }
                /* reject anything other than letters, numbers, '.', '_' or '@' */
                else
                {
                    good_address = false;
                    break;
                }
            }

            /* make sure a '@' was there someplace */
            if (!at_flag)
            {
                good_address = false;
            }

            /* if the adress is bad, complain and return */
            if (!good_address)
            {
                string_buffer = CFUNCTIONS.sprintfSinglestring("\"%s\" does not appear to be a proper e-mail address.  Please try another.\n", theAccount.email);

                ioclass.Do_send_line(c, string_buffer);

                ioclass.Do_more(c);

                answer = 0;// false;
                return phantdefs.S_NORM;
            }

            /* it's a good address, convert it to lowercase */
            miscclass.Do_lowercase(ref theAccount.lcemail, theAccount.email);

            /* check the account file for a duplicate address */
            int errno = 0;
            miscclass.Do_lock_mutex(c.realm.account_lock);

            if //((account_file = CLibFile.FileOpenRead(pathnames.ACCOUNT_FILE)) == null)
                ((account_file = CLibFile.fopen(pathnames.ACCOUNT_FILE, "r", ref errno)) == null)
            {
                string_buffer = CFUNCTIONS.sprintfSinglestring("[%s] CLibFile.fopen of %s failed in Do_approve_email: %s\n", c.connection_id, pathnames.ACCOUNT_FILE, CFUNCTIONS.strerror(errno)); //strerror retrieves a C error message from a number

                fileclass.Do_log_error(string_buffer);
            }
            else
            {
                /* run through each entry and compare */
                while (    //loop through file byte chunks per CLibFile.fread
                    CLibFile.fread(ref readAccount, phantdefs.SZ_ACCOUNT, 1, account_file) == 1
                    )
                {
                    if (!CFUNCTIONS.strcmp(readAccount.lcemail,theAccount.lcemail))
                    {
                        CLibFile.fclose(account_file);
                        miscclass.Do_unlock_mutex(c.realm.account_lock);

                        string_buffer = CFUNCTIONS.sprintfSinglestring("The email address \"%s\" already has an account registered to it.  Please choose another.\n", theAccount.email);

                        ioclass.Do_send_line(c, string_buffer);

                        ioclass.Do_more(c);

                        answer = 0;// false;
                        return phantdefs.S_NORM;
                    }
                }

                CLibFile.fclose(account_file);
            }

            /* see if this address is in limbo */
            list_ptr = c.realm.email_limbo;

            /* run through all addresses in limbo */
            while (list_ptr != null)
            {

                if (!CFUNCTIONS.strcmp(list_ptr.name,theAccount.lcemail))
                {
                    miscclass.Do_unlock_mutex(c.realm.account_lock);

                    string_buffer = CFUNCTIONS.sprintfSinglestring("The email address \"%s\" is currently being registered by another player.  Please choose another.\n", theAccount.email);

                    ioclass.Do_send_line(c, string_buffer);

                    ioclass.Do_more(c);

                    answer = 0;// false;
                    return phantdefs.S_NORM;
                }

                list_ptr = list_ptr.next;
            }

            /* e-mail address checks out.  Put ours in limbo */
            list_ptr = new linked_list_t(); // = miscclass.Do_malloc<linked_list_t>(phantdefs.SZ_LINKED_LIST);

            list_ptr.name = theAccount.lcemail;
            list_ptr.next = c.realm.email_limbo;
            c.realm.email_limbo = list_ptr;

            miscclass.Do_unlock_mutex(c.realm.account_lock);

            answer = 1;// true;
            return phantdefs.S_NORM;
        }
        
        void Do_release_email(client_t c, string address)
        {
            string error_msg;// = new char = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            linked_list_t list_ptr, list_ptr_ptr;

            miscclass.Do_lock_mutex(c.realm.account_lock);

            /* start at the first pointer */
            list_ptr_ptr = c.realm.email_limbo;

            /* run through all addresses in limbo */
            while (list_ptr_ptr != null)
            {
                if (!CFUNCTIONS.strcmp(list_ptr_ptr.name, address))
                {
                    /* remove this section of linked list */
                    list_ptr = list_ptr_ptr;
                    list_ptr_ptr = list_ptr.next;

                    CFUNCTIONS.free(ref list_ptr);

                    miscclass.Do_unlock_mutex(c.realm.account_lock);
                    return;
                }

                list_ptr_ptr = list_ptr_ptr.next;
            }

            miscclass.Do_unlock_mutex(c.realm.account_lock);

            error_msg = CFUNCTIONS.sprintfSinglestring("[%s] Passed name not found in email limbo in Do_release_email.\n", c.connection_id, address);

            fileclass.Do_log_error(error_msg);
            return;
        }
        
        int Do_save_account(client_t c, account_t theAccount)
        {
            string string_buffer;// = new char = ""; //[phantdefs.SZ_LINE];
            //FILE 
            CLibFile.CFILE account_file;

            /* open the account file for writing */
            int errno = 0;
            miscclass.Do_lock_mutex(c.realm.account_lock);
            if ((account_file = CLibFile.fopen(pathnames.ACCOUNT_FILE, "a", ref errno)) == null)
            {
                miscclass.Do_unlock_mutex(c.realm.account_lock);
                string_buffer = CFUNCTIONS.sprintfSinglestring("[%s] CLibFile.fopen of %s failed in Do_save_account: %s.\n", c.connection_id, pathnames.ACCOUNT_FILE, CFUNCTIONS.strerror(errno));

                fileclass.Do_log_error(string_buffer);
                return phantdefs.S_ERROR;
            }

            /* write the new account at the end */
            if (CLibFile.fwrite(theAccount, ref phantdefs.SZ_ACCOUNT, 1, account_file) != 1)
            {
                CLibFile.fclose(account_file);
                miscclass.Do_unlock_mutex(c.realm.account_lock);
                string_buffer = CFUNCTIONS.sprintfSinglestring("[%s] CLibFile.fwrite of %s failed in Do_save_account: %s.\n", c.connection_id, pathnames.ACCOUNT_FILE, CFUNCTIONS.strerror(errno));

                fileclass.Do_log_error(string_buffer);
                return phantdefs.S_ERROR;
            }

            /* close the file */
            CLibFile.fclose(account_file);
            miscclass.Do_unlock_mutex(c.realm.account_lock);

            /* release the names in limbo */
            characterclass.Do_release_name(c, theAccount.lcname);
            Do_release_email(c, theAccount.lcemail);

            return phantdefs.S_NORM;
        }
        
        void Do_account_login(client_t c)
        {
            string string_buffer = "";// = new char = ""; //[phantdefs.SZ_LINE];
            string string_buffer2 = "";// = new char = ""; //[phantdefs.SZ_LINE];
            string accountName = "";// = new char = ""; //[phantdefs.SZ_NAME];
            account_t theAccount = new account_t();
            account_mod_t theMod = new account_mod_t();
            int i;

            /* Inquire the account to load */
            for (; ; )
            {
                if (UnityGameController.StopApplication) //time to quit
                {
                    Debug.Log("Thread " + System.Threading.Thread.CurrentThread.Name + " stopping in Do_account_login");
                    return;
                }
                else
                {
                    System.Threading.Thread.Sleep(33); //30fps
                }

                if (ioclass.Do_string_dialog(c, ref accountName, phantdefs.SZ_NAME - 1, "What is the name of your account?\n"))
                {
                    return;
                }

                if (Do_look_account(c, accountName, ref theAccount) != 0)
                {
                    break;
                }

                ioclass.Do_send_line(c, "I can not find the account you named.  Please check the spelling and try again.\n");

                ioclass.Do_more(c);
                ioclass.Do_send_clear(c);
            }

            /* ask for a password */
            if (ioclass.Do_request_account_password(c, theAccount.password, theAccount.name, theAccount.lcname) == 0)
            {
                /* add a confirmation code reminder if applicable */
                if (theAccount.confirmation != null && theAccount.confirmation[0] != '\0')
                {
                    ioclass.Do_send_line(c, "Perhaps you entered the confirmation code emailed to you instead of your chosen password.  If so, try to log in again and enter the PASSWORD YOU CHOSE when you created the account.\n");

                    ioclass.Do_more(c);
                    ioclass.Do_send_clear(c);
                }

                return;
            }

            /* ask for confirmation code if one exists */
            if (theAccount.confirmation != null && theAccount.confirmation[0] != '\0')
            {

                for (i = 0; i < 2; i++)
                {
                    /* Inquire the account to load */
                    if (ioclass.Do_string_dialog(c, ref string_buffer, phantdefs.SZ_LINE - 1, "Please enter the confirmation code mailed to you.\n"))
                    {
                        return;
                    }

                    /* good code */
                    if (!CFUNCTIONS.strcmp(string_buffer, new string(theAccount.confirmation)))
                    {
                        Do_clear_account_mod(theMod);
                        theMod.confirm = true;
                        if (Do_modify_account(c, accountName, null, theMod) == 0)
                        {
                            ioclass.Do_send_line(c, "The account change request encountered an error.  Please contact the game administrator about the problem.\n");

                            ioclass.Do_more(c);

                            ioclass.Do_send_clear(c);
                            return;
                        }

                        ioclass.Do_send_line(c, "Confirmation code confirmed!  You will never be asked for this code again, so feel free to forget all about it.\n");

                        ioclass.Do_more(c);

                        ioclass.Do_send_clear(c);
                        break;
                    }

                    if (i == 0)
                    {

                        ioclass.Do_send_line(c, "That is not the correct confirmation code.  Please double check the code before you hit \"ok\"\n");

                        ioclass.Do_more(c);

                        ioclass.Do_send_clear(c);
                    }
                    else
                    {
                        ioclass.Do_send_clear(c);

                        ioclass.Do_send_line(c, "That is not the confirmation code that was mailed to you.  Please wait until you have that e-mail before again attempting to log on with this account.\n");

                        ioclass.Do_more(c);
                        return;
                    }
                }
            }

            /* mark us as logged in and get updated information */
            Do_clear_account_mod(theMod);
            theMod.access = true;
            if (Do_modify_account(c, accountName, theAccount, theMod) == 0)
            {
                ioclass.Do_send_line(c, "The program encountered an error while accessing your account information.  Please contact the game administrator about the problem.\n");

                ioclass.Do_more(c);

                ioclass.Do_send_clear(c);
            }

            string_buffer = CFUNCTIONS.sprintfSinglestring("[%s] Logged on with account %s with e-mail address %s.\n", c.connection_id, theAccount.lcname, theAccount.email);

            fileclass.Do_log(pathnames.CONNECTION_LOG, string_buffer);

            c.account = theAccount.name;
            c.lcaccount = theAccount.lcname;
            c.email = theAccount.email;
            c.parentNetwork = theAccount.parent_network;
            c.accountLoaded = true;

            /* put the account name in the game description */
            miscclass.Do_lock_mutex(c.realm.realm_lock);
            c.game.account = theAccount.name;
            miscclass.Do_unlock_mutex(c.realm.realm_lock);

            if (theAccount.bad_passwords != 0)
            {
                string_buffer = CFUNCTIONS.sprintfSinglestring("There have been %d unsuccessful login(s) to this account since last successful login.\n", theAccount.bad_passwords);

                ioclass.Do_send_line(c, string_buffer);
            }

            if (theAccount.login_count == 0)
            {
                ioclass.Do_send_line(c, "Welcome new player.  To create a character, hit the \"New Char\" button just below.\n");
            }
            else
            {
                /* convert the last time and remove the "\n". */
                CFUNCTIONS.ctime_r(theAccount.last_load, ref string_buffer2);
                string_buffer2 = string_buffer2.Substring(0, string_buffer2.Length - 1);

                string_buffer = CFUNCTIONS.sprintfSinglestring("Last login at %s from %s.\n", string_buffer2, theAccount.last_IP);

                ioclass.Do_send_line(c, string_buffer);
            }

            c.run_level = (short)phantdefs.CHAR_SELECTION;

            /* make sure this isn't a banned address */
            tagsclass.Do_check_tags(c);

            return;
        }

        internal int Do_look_account(client_t c, string accountName, ref account_t theAccount)
        {
            string string_buffer = "";// = new char = ""; //[phantdefs.SZ_LINE];
            CLibFile.CFILE account_file;

            /* open the account file */
            int errno = 0;
            miscclass.Do_lock_mutex(c.realm.account_lock);
            if //((account_file = CLibFile.FileOpenRead(pathnames.ACCOUNT_FILE)) == null)
                ((account_file = CLibFile.fopen(pathnames.ACCOUNT_FILE, "r", ref errno)) == null)
            {
                miscclass.Do_unlock_mutex(c.realm.account_lock);
                string_buffer = CFUNCTIONS.sprintfSinglestring("[%s] CLibFile.fopen of %s failed in Do_look_account: %s.\n", c.connection_id, pathnames.ACCOUNT_FILE, CFUNCTIONS.strerror(errno));

                fileclass.Do_log_error(string_buffer);
                return 0;
            }

            /* convert the name to lower case and store it */
            miscclass.Do_lowercase(ref accountName, accountName);

            /* run through the account entries */
            while (CLibFile.fread(ref theAccount, phantdefs.SZ_ACCOUNT, 1, account_file) == 1)
            {
                string filter1 = theAccount.lcname.Replace('\0', '$');
                string filter2 = accountName.Replace('\0', '$');
                //Debug.LogError("Look account debug: || " + filter1 + " || " + filter2 + " ||");
                /* if this is the account */
                if (!CFUNCTIONS.strcmp(theAccount.lcname, accountName)) //false = a match
                {

                    /* return with the good news */
                    CLibFile.fclose(account_file);
                    miscclass.Do_unlock_mutex(c.realm.account_lock);

                    /* There's an overflow with the mute count somewhere -- EH & BK - 1/6/02 */
                    if (theAccount.muteCount > 6 || theAccount.muteCount < 0)
                    {
                        theAccount.muteCount = 0;
                    }

                    return 1;
                }
            }

            /* close down and return a negative */
            CLibFile.fclose(account_file);
            miscclass.Do_unlock_mutex(c.realm.account_lock);
            return 0;
        }

        internal void Do_clear_account_mod(account_mod_t theMod)
        {
            theMod.newPassword = false;
            theMod.passwordReset = false;
            theMod.newEmail = false;
            theMod.confirm = false;
            theMod.access = false;
            theMod.badPassword = false;
            theMod.hack = false;
            theMod.mute = false;
        }


        internal int Do_modify_account(client_t c, string accountName, account_t theAccount, account_mod_t theMod)
        {
            string string_buffer = ""; // new char = ""; //[phantdefs.SZ_LINE];
            account_t readAccount = new account_t();
            CLibFile.CFILE account_file, temp_file;
            int timeNow;
            bool found_flag;

            /* open the account file */
            int errno = 0;
            miscclass.Do_lock_mutex(c.realm.account_lock);
            if //((account_file = CLibFile.FileOpenRead(pathnames.ACCOUNT_FILE)) == null)
                ((account_file = CLibFile.fopen(pathnames.ACCOUNT_FILE, "r", ref errno)) == null)
            {
                miscclass.Do_unlock_mutex(c.realm.account_lock);
                string_buffer = CFUNCTIONS.sprintfSinglestring("[%s] CLibFile.fopen of %s failed in Do_modify_account: %s.\n", c.connection_id, pathnames.ACCOUNT_FILE, CFUNCTIONS.strerror(errno));

                fileclass.Do_log_error(string_buffer);
                return 0;
            }

            /* open a temp file to transfer records to */
            errno = 0;
            if //((temp_file = CLibFile.FileOpenWrite(pathnames.TEMP_ACCOUNT_FILE)) == null)
                ((temp_file = CLibFile.fopen(pathnames.TEMP_ACCOUNT_FILE, "w", ref errno)) == null)
            {
                CLibFile.fclose(account_file);
                miscclass.Do_unlock_mutex(c.realm.account_lock);
                string_buffer = CFUNCTIONS.sprintfSinglestring("[%s] CLibFile.fopen of %s failed in Do_modify_account(2): %s.\n", c.connection_id, pathnames.TEMP_ACCOUNT_FILE, CFUNCTIONS.strerror(errno));

                fileclass.Do_log_error(string_buffer);
                return 0;
            }

            /* get the time now */
            timeNow = CFUNCTIONS.time(null);
            found_flag = false;

            /* run through the account entries */
            while (CLibFile.fread(ref readAccount, phantdefs.SZ_ACCOUNT, 1, account_file) == 1)
            {

                /* if this is the account */
                if (readAccount.lcname == accountName)
                {

                    found_flag = true;

                    /* copy the record to the passed pointer */
                    if (theAccount != null)
                    {
                        CFUNCTIONS.memcpy(ref theAccount, readAccount, phantdefs.SZ_ACCOUNT);
                    }

                    /* make the modifications */
                    if (theMod.newPassword)
                    {

                        readAccount.password = theMod.password;
                    }

                    if (theMod.passwordReset)
                    {
                        readAccount.last_reset = timeNow;
                    }

                    if (theMod.newEmail)
                    {
                        readAccount.email = theMod.email;

                        readAccount.confirmation = theMod.confirmation;
                    }

                    if (theMod.confirm)
                    {
                        readAccount.confirmation[0] = '\0';
                    }

                    if (theMod.access)
                    {

                        readAccount.last_IP = c.IP;

                        readAccount.last_network = c.network;
                        readAccount.last_load = timeNow;
                        ++readAccount.login_count;
                        readAccount.bad_passwords = 0;
                    }

                    if (theMod.badPassword)
                    {
                        ++readAccount.bad_passwords;
                    }

                    if (theMod.hack)
                    {
                        readAccount.hackCount = theMod.hackCount;
                        readAccount.lastHack = timeNow;
                    }

                    if (theMod.mute)
                    {
                        readAccount.muteCount = theMod.muteCount;
                        readAccount.lastMute = timeNow;
                    }
                }
                else
                {

                    /* see if this account is still valid */
                    if (timeNow - readAccount.last_load > phantdefs.ACCOUNT_KEEP_TIME)
                    {
                        /* log the deletion of the account */
                        string_buffer = CFUNCTIONS.sprintfSinglestring("[%s] Deleted the account %s.\n",
                        c.connection_id, readAccount.lcname);
                        
                        fileclass.Do_log(pathnames.CONNECTION_LOG, string_buffer);

                        /* don't save this account */
                        continue;
                    }
                }

                /* write the account to the temp file */
                if (CLibFile.fwrite(readAccount, ref phantdefs.SZ_ACCOUNT, 1, temp_file) != 1)
                {
                    CLibFile.fclose(account_file);
                    CLibFile.fclose(temp_file);
                    
                    CLibFile.remove(pathnames.TEMP_ACCOUNT_FILE);
                    miscclass.Do_unlock_mutex(c.realm.account_lock);

                    string_buffer = CFUNCTIONS.sprintfSinglestring("[%s] CLibFile.fwrite of %s failed in Do_modify_account: %s.\n", c.connection_id, pathnames.TEMP_ACCOUNT_FILE, CFUNCTIONS.strerror(errno));

                    fileclass.Do_log_error(string_buffer);
                    return 0;
                }
            }

            /* close the two files */
            CLibFile.fclose(account_file);
            CLibFile.fclose(temp_file);

            /* delete the old character record */
            CLibFile.remove(pathnames.ACCOUNT_FILE);

            /* replace it with the temporary file */
            CLibFile.rename(pathnames.TEMP_ACCOUNT_FILE, pathnames.ACCOUNT_FILE);

            miscclass.Do_unlock_mutex(c.realm.account_lock);

            if (found_flag)
            {
                return 1;
            }

            string_buffer = CFUNCTIONS.sprintfSinglestring("[%s] Couldn't find account \"%s\" to modify in Do_modify_account.\n", c.connection_id, accountName);

            fileclass.Do_log_error(string_buffer);
            return 0;
        }
        
        internal void Do_account_options(client_t c)
        {
            string error_msg = "";// new char = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            string string_buffer = "";// new char = ""; //[phantdefs.SZ_LINE];
            account_t theAccount = new account_t();
            button_t buttons = new button_t();
            int rc;
            long answer = -1;

            if (Do_look_account(c, c.lcaccount, ref theAccount) == 0)
            {
                error_msg = CFUNCTIONS.sprintfSinglestring("[%s] Unable to load own account %s in Do_account_options.\n", c.connection_id, c.lcaccount);

                fileclass.Do_log_error(error_msg);
            }

            string_buffer = CFUNCTIONS.sprintfSinglestring("Connecting From: %s\n", c.IP);
            ioclass.Do_send_line(c, string_buffer);
            string_buffer = CFUNCTIONS.sprintfSinglestring("Account Name: %s\n", theAccount.name);
            ioclass.Do_send_line(c, string_buffer);
            string_buffer = CFUNCTIONS.sprintfSinglestring("Account E-Mail: %s\n", theAccount.email);
            ioclass.Do_send_line(c, string_buffer);
            string_buffer = CFUNCTIONS.sprintfSinglestring("Created From: %s\n", theAccount.parent_network);
            ioclass.Do_send_line(c, string_buffer);
            CFUNCTIONS.ctime_r(theAccount.date_created, ref error_msg);
            string_buffer = CFUNCTIONS.sprintfSinglestring("Created On: %s", error_msg);
            ioclass.Do_send_line(c, string_buffer);
            string_buffer = CFUNCTIONS.sprintfSinglestring("Successful Logins: %d\n", theAccount.login_count);
            ioclass.Do_send_line(c, string_buffer);

            buttons.button[0] = "Change Pass\n";
            ioclass.Do_clear_buttons(buttons, 1);
            buttons.button[7] = "Go Back\n";

            /* I was thinking od adding a "change email" option, but I then realized that
            all of a players characters could become compromized if just the account was */

            rc = ioclass.Do_buttons(c, ref answer, buttons);
            ioclass.Do_send_clear(c);

            if (rc != phantdefs.S_NORM)
            {
                answer = 7;
            }

            /* switch on the player's answer */
            switch (answer)
            {
                /* Change the account password */
                case 0:

                    Do_change_account_password(c);
                    break;

                /* Return to previous state */
                case 7:
                    return;

                default:

                    error_msg = CFUNCTIONS.sprintfSinglestring("[%s] Returned non-option %ld in Do_account_options.\n", c.connection_id, answer);

                    fileclass.Do_log_error(error_msg);

                    hackclass.Do_caught_hack(c, phantdefs.H_SYSTEM);
                    return;
            }

            return;
        }
        
        void Do_change_account_password(client_t c)
        {
            string error_msg = "";// new char = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            string string_buffer = ""; // new char = ""; //[phantdefs.SZ_LINE];
            string newPassword = ""; // new char = ""; //[phantdefs.SZ_PASSWORD];
            button_t theButtons = new button_t();
            account_t theAccount = new account_t();
            account_mod_t theMod = new account_mod_t();
            int rc;
            long answer = -1;

            ioclass.Do_send_line(c, "This option allows you to change the password of your account.  Do you wish to continue?\n");

            theButtons.button[0] = "Continue\n";
            ioclass.Do_clear_buttons(theButtons, 1);
            theButtons.button[7] = "Go Back\n";

            rc = ioclass.Do_buttons(c, ref answer, theButtons);
            ioclass.Do_send_clear(c);

            if (rc != phantdefs.S_NORM)
            {
                answer = 7;
            }

            /* switch on the player's answer */
            switch (answer)
            {
                /* Continiue */
                case 0:
                    break;

                /* Go Back */
                case 7:
                    return;

                default:
                    error_msg = CFUNCTIONS.sprintfSinglestring("[%s] Returned non-option %ld in Do_change_account_password.\n", c.connection_id, answer);

                    fileclass.Do_log_error(error_msg);

                    hackclass.Do_caught_hack(c, phantdefs.H_SYSTEM);
                    return;
            }

            /* load the account information */
            if (Do_look_account(c, c.account, ref theAccount) == 0)
            {
                error_msg = CFUNCTIONS.sprintfSinglestring("[%s] Unable to load own account %s in Do_change_account_password.\n", c.connection_id, c.lcaccount);

                fileclass.Do_log_error(error_msg);

                ioclass.Do_send_line(c, "I can not load your account information.  Please contact the game administrator about the problem.\n");

                ioclass.Do_more(c);
                ioclass.Do_send_clear(c);
                return;
            }

            /* confirm the player knows his current password */
            if (ioclass.Do_request_account_password(c, theAccount.password, c.account, c.lcaccount) == 0)
            {
                return;
            }

            Do_clear_account_mod(theMod);
            theMod.newPassword = true;

            /* Get the new password from the player */
            if (!ioclass.Do_new_password(c, theMod.password, "account"))
            {
                return;
            }

            if (Do_modify_account(c, c.lcaccount, null, theMod) == 0)
            {
                /* if false returns, the modify did not occur */
                ioclass.Do_send_line(c, "The account change request encountered an error.  Please write down the timestamp at the end and contact the game administrator about the problem.  The password has NOT been changed.\n");


                ioclass.Do_more(c);

                ioclass.Do_send_clear(c);
                return;
            }

            string_buffer = CFUNCTIONS.sprintfSinglestring( "[%s] Changed the password to account %s.\n", c.connection_id, c.lcaccount);

            fileclass.Do_log(pathnames.CONNECTION_LOG, string_buffer);

            ioclass.Do_send_line(c, "The password to your account has been successfully changed.\n");

            ioclass.Do_more(c);
            ioclass.Do_send_clear(c);
            return;
        }
        
        void Do_reset_account_password(client_t c)
        {
            string error_msg = "";// new char = ""; //[phantdefs.SZ_ERROR_MESSAGE];
            string accountName = "";// new char = ""; //[phantdefs.SZ_NAME];
            string string_buffer = "";
            string lcAccountName = "";
            char[] newPassword = new char[16];
            button_t theButtons = new button_t();
            account_t theAccount = new account_t();
            account_mod_t theMod = new account_mod_t();
            int rc;
            long answer = -1;
            md5c.MD5_CTX context = new md5c.MD5_CTX();
            uint len;

            ioclass.Do_send_line(c, "With this option a random password will be created for your account and e-mailed to the account address.  You are required to be logged in at the same location at which the account was created.  This is the only way to gain access to an account whose password you've forgotten.\n");

            theButtons.button[0] = "Continue\n";
            ioclass.Do_clear_buttons(theButtons, 1);
            theButtons.button[7] = "Go Back\n";

            rc = ioclass.Do_buttons(c, ref answer, theButtons);
            ioclass.Do_send_clear(c);

            if (rc != phantdefs.S_NORM)
            {
                answer = 7;
            }

            /* switch on the player's answer */
            switch (answer)
            {
                /* Continiue */
                case 0:
                    break;

                /* Go Back */
                case 7:
                    return;

                default:

                    error_msg = CFUNCTIONS.sprintfSinglestring("[%s] Returned non-option %ld in Do_reset_account_password.\n", c.connection_id, answer);

                    fileclass.Do_log_error(error_msg);

                    hackclass.Do_caught_hack(c, phantdefs.H_SYSTEM);
                    return;
            }

            for (; ; )
            {
                if (UnityGameController.StopApplication) //time to quit
                {
                    Debug.Log("Thread " + System.Threading.Thread.CurrentThread.Name + " stopping in Do_reset_account_password");
                    return;
                }
                else
                {
                    System.Threading.Thread.Sleep(33); //30fps
                }

                /* prompt for the account name */
                if (ioclass.Do_string_dialog(c, ref accountName, phantdefs.SZ_NAME - 1, "What is the name of the account that needs a new password?\n"))
                {
                    return;
                }

                /* load the account information */
                miscclass.Do_lowercase(ref lcAccountName, accountName);
                if (Do_look_account(c, lcAccountName, ref theAccount) != 0)
                {
                    break;
                }
                
                string_buffer = CFUNCTIONS.sprintfSinglestring("I can not find an account named \"%s\".  Please check the spelling and try again.\n", accountName);

                ioclass.Do_send_line(c, string_buffer);
                ioclass.Do_more(c);

                ioclass.Do_send_clear(c);
            }

            /* compare network addresses */
            if (CFUNCTIONS.strcmp(theAccount.parent_network,c.network))
            {
                ioclass.Do_send_line(c, "You are in a different internet domain than the one used to create this account.  The account password reset can not continue.\n");


                ioclass.Do_more(c);

                ioclass.Do_send_clear(c);
                return;
            }

            /* only confirmed accounts can be reset */
            if (theAccount.confirmation != null && theAccount.confirmation[0] != '\0')
            {
                ioclass.Do_send_line(c, "Password resets can not be done on accounts with a e-mail address that has not been confirmed.\n");


                ioclass.Do_more(c);

                ioclass.Do_send_clear(c);
                return;
            }

            DateTime timeNow = DateTime.Now;
            /* allow only one reset every 24 hours */
            if (CFUNCTIONS.GetUnixEpoch(timeNow) - theAccount.last_reset < 86400)
            {
                ioclass.Do_send_line(c, "The password for this account was reset in the last 24 hours.  You must wait before resetting it again.\n");

                ioclass.Do_more(c);

                ioclass.Do_send_clear(c);
                return;
            }

            string_buffer = CFUNCTIONS.sprintfSinglestring("Are you certain you wish to reset the password for the account \"%s\"?\n", theAccount.name);

            ioclass.Do_send_line(c, string_buffer);

            if (ioclass.Do_yes_no(c, ref answer) != phantdefs.S_NORM || answer == 1)
            {
                ioclass.Do_send_clear(c);

                string_buffer = CFUNCTIONS.sprintfSinglestring("Password reset aborted.  The password to the account \"%s\" has NOT been changed.\n", theAccount.name);

                ioclass.Do_send_line(c, string_buffer);

                ioclass.Do_more(c);

                ioclass.Do_send_clear(c);
                return;
            }

            ioclass.Do_send_clear(c);

            /* create a new password */
            miscclass.Do_create_password(ref newPassword);

            /* call the script to e-mail this new password */
            string_buffer = CFUNCTIONS.sprintfSinglestring("%s %s %s %s\n", pathnames.ACCOUNT_PASSWORD_RESET_SCRIPT, theAccount.name, newPassword, theAccount.email);

            /* if the mail send fails */
            rc = -1; //added
            if (rc == CFUNCTIONS.system(string_buffer, theAccount.name, new string(newPassword), theAccount.email)) //todo: rc is set to the chosen button?? email fail code is -1
            {

                string_buffer = CFUNCTIONS.sprintfSinglestring("[%s] Account password reset e-mail failed with a code of %d in Do_reset_account_password.", c.connection_id, rc);

                fileclass.Do_log_error(string_buffer);

                ioclass.Do_send_line(c, "An error occured while trying to send e-mail containing the new password.  The account password has NOT been changed.  Please write down the timestamp at the end and contact the game administrator about this problem.\n");

                ioclass.Do_more(c);

                ioclass.Do_send_clear(c);
                return;
            }

            /* prepare the modification */
            Do_clear_account_mod(theMod);
            theMod.newPassword = true;
            theMod.passwordReset = true;

            /* run the password through a MD5 hash */
            len = (uint)newPassword.Length;
            md5c.MD5Init(ref context);
            md5c.MD5Update(ref context, newPassword, len);

            /* put the password hash into the change struct */
            md5c.MD5Final(ref theMod.password, ref context);

                /* put the modification in place */
            if (Do_modify_account(c, lcAccountName, null, theMod) == 0)
            {
                ioclass.Do_send_line(c, "The account change request encountered an error.  The password has NOT been changed and the e-mail sent to you contains incorrect information.  Please contact the game administrator about the problem.\n");

                ioclass.Do_more(c);

                ioclass.Do_send_clear(c);
                return;
            }

            string_buffer = CFUNCTIONS.sprintfSinglestring("[%s] Reset the password to account %s.\n", c.connection_id, theAccount.lcname);

            fileclass.Do_log(pathnames.CONNECTION_LOG, string_buffer);

            string_buffer = CFUNCTIONS.sprintfSinglestring("The password to the account \"%s\" has been successfully changed.  An e-mail with the new password is in transit.\n", theAccount.name);

            ioclass.Do_send_line(c, string_buffer);
            ioclass.Do_more(c);
            ioclass.Do_send_clear(c);
        }
    }
}
