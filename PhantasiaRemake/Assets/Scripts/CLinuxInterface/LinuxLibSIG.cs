using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class LinuxLibSIG : MonoBehaviour
{

    public static int SIGIO = 0; //This signal is sent when a file descriptor is ready to perform input or output.
    public static int SIGINT = 1; //SIGINT is a program interrupt signal, which is sent when an user presses Ctrl+C. 
    public static int SIGTERM = 2;    /* SIGTERM shuts the server down quickly */
    public static int SIGUSR1 = 3; //The SIGUSR1 and SIGUSR2 signals are set aside for you to use any way you want. //phantasia: /* a SIGUSER says to clean up a thread */ (removes inactive 'games' (players))
    public static int SIGALRM = 4; // The SIGALRM signal is raised when a time interval specified in a call to the alarm or alarmd function expires. 
    
    public static int SIG_BLOCK { get; internal set; } //SIG_BLOCK: specifies that the signals in the new signal mask should be blocked.

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

    }

    internal class sigset_t
    {
        internal List<int> blockingSignals;

        internal sigset_t()
        {
            blockingSignals = new List<int>();
        }
        /*Data Type: sigset_t
The sigset_t data type is used to represent a signal set. Internally, it may be implemented as either an integer or structure type.
There are two ways to initialize a signal set. You can initially specify it to be empty with sigemptyset and then add specified signals individually. 
Or you can specify it to be full with sigfillset and then delete specified signals individually.
You must always initialize the signal set with one of these two functions before using it in any other way. 
Don’t try to set all the signals explicitly because the sigset_t object might include some other information (like a version field) that needs to be initialized as well. 
(In addition, it’s not wise to put into your program an assumption that the system has no signals aside from the ones you know about.)*/
    }

    internal static void sigemptyset(ref sigset_t sigMask)
    {
        //sigemptyset() initializes the signal set given by set to empty, with all signals excluded from the set.

        sigMask = new sigset_t();
    }

    internal static void sigaddset(sigset_t sigMask, int signo) //int sigaddset(sigset_t *set, int signo);
    {
        /*The sigaddset() function adds the individual signal specified by the signo to the signal set pointed to by set.
Applications must call either sigemptyset() or sigfillset() at least once for each object of type sigset_t prior to any other use of that object. 
If such an object is not initialised in this way, but is nonetheless supplied as an argument to any of sigaction(), sigaddset(), sigdelset(), 
sigismember(), sigpending() or sigprocmask(), the results are undefined.*/

        if (!sigMask.blockingSignals.Contains(signo))
            sigMask.blockingSignals.Add(signo);
    }

    internal static void sigprocmask(int v, sigset_t set, ref sigset_t oldset) //int sigprocmask(int how, const sigset_t *set, sigset_t *oldset);
    {
        /*sigprocmask() is used to fetch and/or change the signal mask of the calling thread.  The signal mask is the set of signals whose delivery
       is currently blocked for the caller (see also signal(7) for more details).
The argument How indicates the way in which the set is changed, and the application shall ensure it consists of one of the following values:
SIG_BLOCK
The resulting set shall be the union of the current set and the signal set pointed to by set.
SIG_SETMASK
The resulting set shall be the signal set pointed to by set.
SIG_UNBLOCK
The resulting set shall be the intersection of the current set and the complement of the signal set pointed to by set.

 If the argument oset is not a null pointer, the previous mask shall be stored in the location pointed to by oset. 

 If set is a null pointer, the value of the argument how is not significant and the thread's signal mask shall be unchanged; thus the call can be used to enquire about currently blocked signals.*/

        //phantasia use: once in io: LinuxLibSIG.sigprocmask(0, null, ref sigMask);
        oldset = set;
    }

    internal static void sigdelset(sigset_t sigMask, int signo) //int sigdelset(sigset_t *set, int signo);
    {
        /*The sigdelset() function deletes the individual signal specified by signo from the signal set pointed to by set.
Applications should call either sigemptyset() or sigfillset() at least once for each object of type sigset_t prior to any other use of that object. 
If such an object is not initialised in this way, but is nonetheless supplied as an argument to any of sigaction(), sigaddset(), sigdelset(), 
sigismember(), sigpending() or sigprocmask(), the results are undefined.*/

        sigMask.blockingSignals.Remove(signo);
    }

    internal static void sigwait(sigset_t sigMask, ref int sig)
    {
        /*int sigwait(const sigset_t *set, int *sig);     
       The sigwait() function suspends execution of the calling thread until one of the signals specified in the signal set set becomes pending.
       The function accepts the signal (removes it from the pending list of signals), and returns the signal number in sig.*/

        //check for a pending signal which matches an element in blockedSignals //todo: set pending signals from unitycserverinterface

        //public static int SIGIO = 0; //This signal is sent when a file descriptor is ready to perform input or output.
        //public static int SIGINT = 1; //SIGINT is a program interrupt signal, which is sent when an user presses Ctrl+C. 
        //public static int SIGTERM = 2;    /* SIGTERM shuts the server down quickly */
        //public static int SIGUSR1 = 3; //The SIGUSR1 and SIGUSR2 signals are set aside for you to use any way you want. //phantasia: /* a SIGUSER says to clean up a thread */ (removes inactive 'games' (players))
        //public static int SIGALRM = 4; // The SIGALRM signal is raised when a time interval specified in a call to the alarm or alarmd function expires. 

        Thread theThread = Thread.CurrentThread;
        if (CLibPThread.knownThreads.ContainsKey(theThread.Name))
        {
            CLibPThread.pthread_t pthread = CLibPThread.knownThreads[theThread.Name];
            
            bool[] signals = new bool[] { pthread.SIGIO_ISPENDING, pthread.SIGINT_ISPENDING, pthread.SIGTERM_ISPENDING, pthread.SIGUSR1_ISPENDING, pthread.SIGALRM_ISPENDING };

            bool anySignal = false;
            int count = 0;
            do
            {
                signals = new bool[] { pthread.SIGIO_ISPENDING, pthread.SIGINT_ISPENDING, pthread.SIGTERM_ISPENDING, pthread.SIGUSR1_ISPENDING, pthread.SIGALRM_ISPENDING };
                foreach (int SIG in sigMask.blockingSignals)
                {
                    if (signals[SIG])
                    {
                        anySignal = true;
                        signals[SIG] = false;
                        sig = SIG;
                    }
                }
                
                count++;
                if (count > 50000000)
                {
                    count = 0;
                    string msg = "Thread " + System.Threading.Thread.CurrentThread.Name + " waiting for SIG with mask ";
                    foreach (int SIG in sigMask.blockingSignals)
                    {
                        msg += SIG + " "; 
                    }
                    msg += ", and pthread's SIGIO_ISPENDING " + pthread.SIGIO_ISPENDING;
                    Debug.Log(msg);
                }
                
                if (UnityGameController.StopApplication) //time to quit
                {
                    Debug.Log("Thread " + System.Threading.Thread.CurrentThread.Name + " stopping in sigwait");
                    sig = -1;
                    break;
                }
                else
                {
                    System.Threading.Thread.Sleep(33); //30fps
                }
            }
            while (!anySignal);

            if (sig >= 0 && sig < 5)
            {
                //reset source signal //todo: not working?
                Debug.Log("SIGWAIT RESETTING SIGNAL " + sig); //todo: what about stacked socket messages?
                pthread.SetSignal(sig, false);
            }

            Debug.Log("SIGWAIT FINISHED");
        }
        else
        {
            Debug.LogError("Could not find pthread for sigwait on thread " + theThread.Name);
        }
        
        //wait over, returns to main loop etc
    }

    internal class sigaction //int sigaction(int signum, const struct sigaction *act, struct sigaction *oldact);
    {
        /*           struct sigaction {
               void     (*sa_handler)(int);
               void     (*sa_sigaction)(int, siginfo_t *, void *);
               sigset_t   sa_mask;
               int        sa_flags;
               void     (*sa_restorer)(void);
           };*/

        /*The sigaction() system call is used to change the action taken by a
   process on receipt of a specific signal.  (See signal(7) for an
   overview of signals.)

   signum specifies the signal and can be any valid signal except
   SIGKILL and SIGSTOP.

   If act is non-NULL, the new action for signal signum is installed
   from act.  If oldact is non-NULL, the previous action is saved in
   oldact.*/
        public sigset_t sa_mask;
    }
}
