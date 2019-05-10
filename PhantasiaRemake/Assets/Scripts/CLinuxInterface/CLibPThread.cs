using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using phantasiaclasses;
using System.Threading;

public class CLibPThread : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

    }

    internal static Thread unityThread;
    internal static int nextThreadID = 0;
    internal class pid_t //thread ID
    {
        int threadID;
        internal string playerID;

        internal pid_t()
        {
            init(Thread.CurrentThread);
        }
        internal pid_t(Thread sourceThread)
        {
            init(sourceThread);
        }

        void init(Thread sourceThread)
        {
            if (sourceThread != null)
            { 
                //sourceThread.Name is set to either the short networkPlayerID or the string UnityGameController.ServerThreadName
                if (sourceThread.Name == UnityGameController.ServerThreadName)
                {
                    threadID = ServerManager.ServerSocketNum;
                }
                else
                {
                    threadID = Convert.ToInt32(sourceThread.Name); // nextThreadID;
                }
                this.playerID = sourceThread.Name; //pid
                //nextThreadID++;
                Debug.Log("registered new thread with name, ID: " + sourceThread.Name + ", " + threadID);
            }
            else
            {
                Debug.LogError("Thread not specified for new pid");
            }
        }

        //internal int AsInt()
        //{
        //    return threadID;
        //}

        public static implicit operator int(pid_t x)
        {
            return x.threadID;
        }
    }
    internal class pthread_t
    {
        internal pid_t tID;
        internal Thread associatedThread;
        internal LinuxLibSocket.LinuxSocket associatedSocket;
        //internal List<pendingSig> pendingSignals;

        //internal struct pendingSig
        //{
        //    internal int signalNum;
        //    internal bool signalStatus;
        //}

        private bool sIGIO_ISPENDING;
        private bool sIGINT_ISPENDING;
        private bool sIGTERM_ISPENDING;
        private bool sIGUSR1_ISPENDING;
        private bool sIGALRM_ISPENDING;

        internal bool SIGIO_ISPENDING
        {
            get
            {
                if (associatedSocket != null && associatedSocket.waitingMessages.Count > 0) //allow for stacked messages
                    sIGIO_ISPENDING = true;
                if (Thread.CurrentThread.Name != "ServerThread")
                {
                    //Debug.Log("Checking SIGIO for " + Thread.CurrentThread.Name + ": " + sIGIO_ISPENDING);
                    //if (associatedSocket != null)
                    //    Debug.Log("associatedSocket is " + associatedSocket.FileDescriptor + ", waitingMessage count: " + associatedSocket.waitingMessages.Count);
                }

                return sIGIO_ISPENDING;
            }
        }

        internal bool SIGINT_ISPENDING
        {
            get
            {
                return sIGINT_ISPENDING;
            }
        }

        internal bool SIGTERM_ISPENDING
        {
            get
            {
                return sIGTERM_ISPENDING;
            }
        }

        internal bool SIGUSR1_ISPENDING
        {
            get
            {
                return sIGUSR1_ISPENDING;
            }
        }

        internal bool SIGALRM_ISPENDING
        {
            get
            {
                return sIGALRM_ISPENDING;
            }
        }

        internal pthread_t(Thread thread)
        {
            tID = new pid_t(thread);
            associatedThread = thread;

            SetSignal(LinuxLibSIG.SIGIO, false);
            SetSignal(LinuxLibSIG.SIGINT, false);
            SetSignal(LinuxLibSIG.SIGTERM, false);
            SetSignal(LinuxLibSIG.SIGUSR1, false);
            SetSignal(LinuxLibSIG.SIGALRM, false);
            //signals = new bool[5] { SIGIO_ISPENDING, SIGINT_ISPENDING, SIGTERM_ISPENDING, SIGUSR1_ISPENDING, SIGALRM_ISPENDING };

            associatedSocket = LinuxLibSocket.SocketListManager.GetSocket(tID);
        }

        internal void SetSignal(int sig, bool value)
        {
            //pendingSig newsig = new pendingSig();
            //newsig.signalNum = sig;
            //newsig.signalStatus = value;
            //pendingSignals.Add(newsig);

            if (sig == LinuxLibSIG.SIGIO)
            {
                sIGIO_ISPENDING = value;
            }
            if (sig == LinuxLibSIG.SIGINT)
            {
                sIGINT_ISPENDING = value;
            }
            if (sig == LinuxLibSIG.SIGTERM)
            {
                sIGTERM_ISPENDING = value;
            }
            if (sig == LinuxLibSIG.SIGUSR1)
            {
                sIGUSR1_ISPENDING = value;
            }
            if (sig == LinuxLibSIG.SIGALRM)
            {
                sIGALRM_ISPENDING = value;
            }
        }
    }
    public class pthread_mutex_t
    {
    }
    internal class pthread_attr_t
    {
    }

    internal static Dictionary<string, pthread_t> knownThreads = new Dictionary<string, pthread_t> { };
    internal static pid_t gettid()
    {
        /*gettid() returns the caller's thread ID (TID).  In a single-threaded
       process, the thread ID is equal to the process ID (PID, as returned
       by getpid(2)).  In a multithreaded process, all threads have the same
       PID, but each one has a unique TID.*/

        Thread thread = Thread.CurrentThread;
        pid_t pid = knownThreads[thread.Name].tID;
        if (pid == null)
        {
            Debug.LogError("Could not find pid for current thread");
        }
        return pid;
        //throw new NotImplementedException();
    }

    internal static void sleep(int v)
    {
        /*sleep() makes the calling thread sleep until seconds seconds have elapsed or a signal arrives which is not ignored.*/
        //throw new NotImplementedException();

        Thread.Sleep(v * 1000); //todo: make interruptible?
    }
    internal static int pthread_create(ref pthread_t the_thread, pthread_attr_t p, Action<object> startMethod, object methodParams) //LINUX
    {
        /* int pthread_create(pthread_t *thread, const pthread_attr_t *attr, void *(*start_routine) (void *), void *arg);       
       The pthread_create() function starts a new thread in the calling process. The new thread starts execution by invoking start_routine(); 
       arg is passed as the sole argument of start_routine().
         */

        Debug.Log("creating new thread");

        bool useProcess = false; //wip
        if (useProcess)
        {
            //System.Diagnostics.Process myProcess = new System.Diagnostics.Process();
            //myProcess.StartInfo.UseShellExecute = false;
            //// You can start any process, HelloWorld is a do-nothing example.
            //myProcess.StartInfo.FileName = "C:\\HelloWorld.exe";
            //myProcess.StartInfo.CreateNoWindow = true;
            //myProcess.Start();

            return -1;
        }
        else
        {
            Thread newThread = new Thread(new ParameterizedThreadStart(startMethod));

            short networkPlayerID = ServerManager.PickupLatestPlayerID(); //todo: more reliable method pls
            if (networkPlayerID < 0)
            {
                Debug.LogError("Could not find network player for new player thread");
                return -1;
            }
            else
            {
                newThread.Name = networkPlayerID.ToString();// "pthread" + nextThreadID;
                the_thread = new pthread_t(newThread); //sets pid_t //ensuring Thread name has been set before running this
                knownThreads.Add(newThread.Name, the_thread);
                //the_thread.associatedSocket = LinuxLibSocket.SocketListManager.AddSocket(the_thread); //new LinuxLibSocket.LinuxSocket(the_thread.tID, LinuxLibSocket.SocketState.LISTEN); //todo: untested

                newThread.Start(methodParams);

                Debug.Log("created new pthread with ID " + newThread.Name);

                //subscribe thread for SIGALRM signals
                UnityCServerInterface unityintf = UnityCServerInterface.GetInstance();
                if (unityintf == null)
                    Debug.LogError("No CServerInstance available for new pthread");
                else
                {
                    unityintf.AddThreadSIGALRMflag(newThread.Name);
                }

                return 0; //success
            }
        }
    }
    internal static int pthread_attr_init(pthread_attr_t thread_attr)
    {
        return 0; //default success - pthread_attr_t is unused in current version of phantasia and not needed for unity, therefore is an empty class, no init needed
    }
    internal static void pthread_exit(int v)
    {
        Thread.CurrentThread.Abort();
    }
    internal static void pthread_kill(pthread_t the_thread, int sig)
    {
        if (the_thread != null)
        {
            the_thread.SetSignal(sig, true);
        }
        else
        {
            Debug.LogError("Unable to find thread for kill delivery");
        }
    }
    internal static void pthread_join(pthread_t the_thread, object p)
    {
        Thread theThread = the_thread.associatedThread;
        if (theThread != null)
        {
            theThread.Join();
        }
        else
        {
            Debug.LogError("Unable to find thread for kill: " + the_thread.associatedThread.Name);
        }
        //todo: p?
    }


    internal static int pthread_mutex_init(pthread_mutex_t the_mutex, object p)
    {
        //todo  //throw new NotImplementedException();
        return 0;
    }
    internal static int pthread_mutex_destroy(object the_mutex)
    {
        //todo  //throw new NotImplementedException();
        return 0;
    }
    internal static int pthread_mutex_lock(pthread_mutex_t the_mutex)
    {
        //todo  //throw new NotImplementedException();
        return 0;
    }
    internal static int pthread_mutex_unlock(pthread_mutex_t the_mutex)
    {
        //todo  //throw new NotImplementedException();
        return 0;
    }

    internal static int pthread_sigmask(int sIG_BLOCK, LinuxLibSIG.sigset_t sa_mask, LinuxLibSIG.sigset_t newset) //int pthread_sigmask(int how, const sigset_t *restrict set, sigset_t* restrict oset);
    {
        /*The pthread_sigmask() function shall examine or change (or both) the calling thread's signal mask, regardless of the number of threads in the process. 
         * The function shall be equivalent to sigprocmask(), without the restriction that the call be made in a single-threaded process. 
The argument How indicates the way in which the set is changed, and the application shall ensure it consists of one of the following values:
SIG_BLOCK
The resulting set shall be the union of the current set and the signal set pointed to by set.
SIG_SETMASK
The resulting set shall be the signal set pointed to by set.
SIG_UNBLOCK
The resulting set shall be the intersection of the current set and the complement of the signal set pointed to by set.
If the argument oset is not a null pointer, the previous mask shall be stored in the location pointed to by oset.

 Upon successful completion pthread_sigmask() shall return 0; otherwise, it shall return the corresponding error number.*/

        //phantasia usage: once in main. if (CLibPThread.pthread_sigmask(LinuxLibSIGIO.SIG_BLOCK, sigAct.sa_mask, null) < 0)     [...]     CFUNCTIONS.printf("Error blocking signals.\n");    /* this routine will block for created threads as well */
        //i.e. a sanity check
        //unnecessary for unity

        return 0;
    }
}
