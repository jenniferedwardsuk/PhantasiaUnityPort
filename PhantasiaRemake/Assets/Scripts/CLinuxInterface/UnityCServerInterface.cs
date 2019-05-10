using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

public class UnityCServerInterface : NetworkBehaviour {

    List<UnityPlayerController> players;

    static UnityCServerInterface instance;

    internal static UnityCServerInterface GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            DestroyImmediate(this);
    }

    // Use this for initialization
    void Start ()
    {
        players = new List<UnityPlayerController> { };
    }
	
	// Update is called once per frame
	void Update ()
    {      
        foreach(int value in Sockets.Keys)
        {
            //todo: move to coroutine if performance suffers
            while (Sockets[value].Count > 0)
            {
                string datastring = Encoding.ASCII.GetString(Sockets[value][0]);
                string someStringFiltered = datastring.Replace('\0', '£');
                Debug.Log("................................................UNITYCSERVERINTERFACE DELIVERING DATA: " + someStringFiltered);
                RpcSendDataToSpecificClient((short)value, Sockets[value][0]);
                Sockets[value].RemoveAt(0);
            }
        }

        foreach (string value in ThreadSIGALRMflags.Keys)
        {
            //todo: move to coroutine if performance suffers
            while (ThreadSIGALRMflags[value].Count > 0)
            {
                Debug.Log("<color=green>UNITYCSERVERINTERFACE SCHEDULING ALARMS</color>");
                SIGALRMCountdownThread(ThreadSIGALRMflags[value][0], value); //todo: this allows overlap of SIGALRMs. valid in linux?
                ThreadSIGALRMflags[value].RemoveAt(0);
            }
        }
    }

    public Dictionary<string, List<int>> ThreadSIGALRMflags = new Dictionary<string, List<int>> { };
    public Dictionary<int, List<byte[]>> Sockets = new Dictionary<int, List<byte[]>> { };

    public void AddThreadSIGALRMflag(string threadname)
    {
        ThreadSIGALRMflags.Add(threadname, new List<int> { });
    }
    public void AddSocket(int socketFD)
    {
        Sockets.Add(socketFD, new List<byte[]> { });
    }

    //todo: call
    public void OnNewPlayer(UnityPlayerController newPlayer)
    {
        players.Add(newPlayer);
    }
    public void OnPlayerDisconnect(UnityPlayerController rmPlayer)
    {
        players.Remove(rmPlayer);
    }

    #region emails

    //internal static string NEW_ACCOUNT_SCRIPT = pathname + "bin/new_account_mail.pl";
    //string_buffer = CFUNCTIONS.sprintfSinglestring("{%s} {%s} {%s} {%s}\n", pathnames.NEW_ACCOUNT_SCRIPT, theAccount.name, theAccount.confirmation, theAccount.email);
    //string_buffer = CFUNCTIONS.sprintfSinglestring("%s %s %s %s\n", pathnames.ACCOUNT_PASSWORD_RESET_SCRIPT, theAccount.name, newPassword, theAccount.email);
    //string_buffer = CFUNCTIONS.sprintfSinglestring("%s %s %s %s\n", pathnames.CHARACTER_PASSWORD_RESET_SCRIPT, c.player.name, newPassword, c.email);

    public static void SendNewAccountScriptEmail(string name, string value, string email)
    {
        string Arg0 = name;
        string Arg1 = value;
        string Arg2 = email;

        string emailBody = "Congratulations!  You have sucessfully created a new account on Phantasia\nat http://www.phantasiaold.com. May all your adventures in the realm be\nfun and profitable.\n\n";
        emailBody += "Here is your account information and confirmation code:\n\n";
        emailBody += CFUNCTIONS.sprintfSinglestring("account: %s\n", Arg0);
        emailBody += CFUNCTIONS.sprintfSinglestring("confirmation code: %s\n\n", Arg1);
        emailBody += "To use your account, just go to http://www.phantasiaold.com, click on\n\"The Game\", choose \"Java 1.18 Phantasia Client\" and enter your\naccount, password when confirmation code.  Remember, the password and\nconfirmation code are case sensitive.\n\n";
        emailBody += "If you ever forget your account password, you can request it be changed\n by using the \"New Pass\" button from the login options.  A new\npassword will be generated and e-mailed to this address.\n\n";
        emailBody += "Please keep your account and character passwords private.  If a\nplayer logs on with your account or your account's characters and acts\nin a manner that deserves to be banned, you will be banned.\n\n";
        emailBody += "If you did not request or want an account on Phantasia, you don't\nneed to do anything.  Unless the above confirmation code is used to\nactivate the account, no more information will be sent to you.  The\nonly information kept is your e-mail address so it is not used again.\n\n";
        emailBody += "If you have any questions, please contact the game moderator at\nlink\\@phantasia4.net.\n\n";
        emailBody += "Good luck and happy hunting!\n";

        string emailSubject = "Phantasia Old Account Information";
        string emailRecipient = Arg2;

        string Arg0filtered = Arg0.Replace('\0', '£');
        string Arg1filtered = Arg1.Replace('\0', '£');
        Debug.Log("<color=red>Email sending is not implemented yet.</color> Confirmation code for " + Arg0filtered + " is: " + Arg1filtered + ""); //todo: display on screen
        //todo: send email
        //        system("mail -s \"Phantasia Old Account Information\" $ARGV[2] < /tmp/$ARGV[2].mail");

    }

    //string_buffer = CFUNCTIONS.sprintfSinglestring("%s %s %s %s\n", pathnames.ACCOUNT_PASSWORD_RESET_SCRIPT, theAccount.name, newPassword, theAccount.email);
    //internal static string ACCOUNT_PASSWORD_RESET_SCRIPT = pathname + "bin/account_reset_mail.pl";
    public static void SendAccountPasswordResetScriptEmail(string name, string value, string email)
    {
        string Arg0 = name;
        string Arg1 = value;
        string Arg2 = email;

        string emailBody = CFUNCTIONS.sprintfSinglestring("As you requested, the password has been reset for the Phantasia account\nnamed %s.  Here is the new password and remember it is case sensitive:\n\n", Arg0);
        emailBody += CFUNCTIONS.sprintfSinglestring("account: %s\n", Arg0);
        emailBody += CFUNCTIONS.sprintfSinglestring("password: %s\n\n", Arg1);
        emailBody += "This password can be changed at http://www.phantasiaold.com.  Log in to your\naccount and press \"Account\" and then \"Change Pass\".\n\n";
        emailBody += "If you have any questions, please contact the game moderator at\nlink\\@phantasia4.net.\n\n";
        emailBody += "May the minions tremble at the mention of your name.\n";

        string emailSubject = "Account " + Arg0 + " Password Reset";
        string emailRecipient = Arg2;

        string Arg0filtered = Arg0.Replace('\0', '£');
        string Arg1filtered = Arg1.Replace('\0', '£');
        Debug.Log("<color=red>Email sending is not implemented yet.</color> New password for " + Arg0filtered + " is: " + Arg1filtered + ""); //todo: display on screen
        //todo: send email
        //        system("mail -s \"Account $ARGV[0] Password Reset\" $ARGV[2] < /tmp/$ARGV[2].mail");
    }

    //string_buffer = CFUNCTIONS.sprintfSinglestring("{%s} {%s} {%s} {%s}\n", pathnames.CHARACTER_PASSWORD_RESET_SCRIPT, c.player.name, newPassword, c.email);
    //internal static string CHARACTER_PASSWORD_RESET_SCRIPT = pathname + "bin/character_reset_mail.pl";
    public static void SendCharacterPasswordResetScriptEmail(string name, string value, string email)
    {
        string Arg0 = name;
        string Arg1 = value;
        string Arg2 = email;

        string emailBody = CFUNCTIONS.sprintfSinglestring("As you requested, the password has been reset for the Phantasia character\nnamed %s.  Here is the new password.  Remember that it is case sensitive:\n\n", Arg0);
        emailBody += CFUNCTIONS.sprintfSinglestring("character name: %s\n", Arg0);
        emailBody += CFUNCTIONS.sprintfSinglestring("password: %s\n\n", Arg1);
        emailBody += "This password can be changed at http://www.phantasiaold.com.  Login in to\nyour account and press \"Characters\" and then \"Change Pass\".\n\n";
        emailBody += "If you have any questions, please contact the game moderator at\nlink\\@phantasia4.net.\n\n";
        emailBody += "May your sword be swift and the evil fall quickly.\n";

        string emailSubject = "Character " + Arg0 + " Password Reset";
        string emailRecipient = Arg2;

        string Arg0filtered = Arg0.Replace('\0', '£');
        string Arg1filtered = Arg1.Replace('\0', '£');
        Debug.Log("<color=red>Email sending is not implemented yet.</color> New password for " + Arg0filtered + " is: " + Arg1filtered + ""); //todo: display on screen
        //todo: send email
        //        system("mail -s \"Character $ARGV[0] Password Reset\" $ARGV[2] < /tmp/$ARGV[2].mail");
    }

    //from stackoverflow. wip
    //void SendEmail(string emailaddress)
    //{
    //    MailMessage mail = new MailMessage();

    //    mail.From = new MailAddress(emailaddress);
    //    mail.To.Add(emailaddress);
    //    mail.Subject = "Test Mail";
    //    mail.Body = "This is for testing SMTP mail from GMAIL";

    //    SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
    //    smtpServer.Port = 587;
    //    smtpServer.Credentials = new System.Net.NetworkCredential(emailaddress, "yourpassword") as ICredentialsByHost;
    //    smtpServer.EnableSsl = true;
    //    ServicePointManager.ServerCertificateValidationCallback =
    //        delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    //        { return true; };
    //    smtpServer.Send(mail);
    //    Debug.Log("Email send success");
    //}

    #endregion emails

    #region communication

    //ClientRpc calls are sent from objects with a NetworkIdentity on the server to objects on clients.
    //To make a function into a ClientRpc call, add the [ClientRpc] custom attribute to it, and add the “Rpc” prefix. 
    //This function will now be run on clients when it is called on the server. Any arguments will automatically be passed to the clients

    [ClientRpc]
    public void RpcSendDataToAllClients(byte[] message) //todo: call
    {
        foreach (UnityPlayerController player in players)
        {
            if (player.isLocalPlayer)
            {
                player.ReceiveDataFromServer(message);
            }
        }
    }

    [ClientRpc]
    public void RpcSendDataToSpecificClient(short targetPlayerID, byte[] message)  //this method runs on all clients when called by the server //todo: call
    {
        foreach(UnityPlayerController player in players)
        {
            if (player.playerControllerId == targetPlayerID) //todo: set playerControllerId as thread pid
            {
                player.ReceiveDataFromServer(message);
            }
        }
    }

    //Commands are sent from player objects on the client to player objects on the server.
    //To make a function into a command, add the [Command] custom attribute to it, and add the “Cmd” prefix. 
    //This function will now be run on the server when it is called on the client. Any arguments will automatically be passed to the server with the command.
    
    internal static void ReceiveDataFromClient(short playerID, byte[] message) //called by UnityPlayerController Cmd
    {
        LinuxLibSocket.LinuxSocket socket = LinuxLibSocket.SocketListManager.FileDescriptors.Find(x => x.FileDescriptor == playerID);
        if (socket != null)
        {
            socket.SetMessage(message);
        }
        else
        {
            Debug.LogError("Socket not found for player ID " + playerID);
        }
        if (CLibPThread.knownThreads.ContainsKey(playerID.ToString())) //correct to send SIGIO for any new message?
        {
            CLibPThread.pthread_t playerThread = CLibPThread.knownThreads[playerID.ToString()];
            playerThread.SetSignal(LinuxLibSIG.SIGIO, true);
        }
        else
        {
            Debug.LogError("Server client thread not found for player " + playerID);
        }
        //todo: identify player's C thread, set message as waiting data? and signal it
    }

    internal void DoSIGALRMCountdownForSocket(int time, LinuxLibSocket.LinuxSocket linuxSocket)
    {
        //StartCoroutine(SIGALRMCountdownSocket(time, linuxSocket));
        throw new NotImplementedException();
    }
    internal void DoSIGALRMCountdownForThread(int time, Thread relevantThread)
    {
        if (ThreadSIGALRMflags.ContainsKey(relevantThread.Name))
        {
            ThreadSIGALRMflags[relevantThread.Name].Add(time);
         }
        else
        {
            Debug.LogError("Thread " + relevantThread.Name + " is not registered for SIGALRM signals");
        }
        //StartCoroutine(SIGALRMCountdownThread(time, relevantThread)); //must be done by main thread
    }
    IEnumerator SIGALRMCountdownSocket(int time, LinuxLibSocket.LinuxSocket linuxSocket)
    {
        float fTime = time;
        while (fTime > 0)
        {
            fTime -= Time.deltaTime;
            yield return null;
        }
        //todo: send SIGALRM to socket
    }
    IEnumerator SIGALRMCountdownThread(int time, string relevantThreadName)
    {
        float fTime = time;
        while (fTime > 0)
        {
            fTime -= Time.deltaTime;
            yield return null;
        }
        //send SIGALRM to thread
        if (CLibPThread.knownThreads.ContainsKey(relevantThreadName))
        {
            Debug.Log("Setting SIGIO_ISPENDING on pthread for " + UnityGameController.ServerThreadName);
            CLibPThread.knownThreads[UnityGameController.ServerThreadName].SetSignal(LinuxLibSIG.SIGALRM, true);
        }
        else
        {
            Debug.LogError("Could not find pthread to send SIGALRM for thread " + relevantThreadName);
        }
    }
    

    #endregion communication

}
