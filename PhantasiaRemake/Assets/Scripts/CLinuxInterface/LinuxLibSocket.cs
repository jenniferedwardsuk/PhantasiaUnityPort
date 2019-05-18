using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Text;
using phantasiaclasses;

public class LinuxLibSocket : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

    }

    internal class in_addr
    {
        public int s_addr { get; internal set; }
    }
    internal class sockaddr_in : sockaddr
    {
        //struct in_addr sin_addr;   /* internet address */
        //sa_family_t sin_family; /* address family: AF_INET */
        //in_port_t sin_port;   /* port in network byte order */

        public in_addr sin_addr { get; internal set; }
        public uint sin_family { get; internal set; } //sa_family_t is uint
        public int sin_port { get; internal set; }
        //sin_addr is the IP address in the socket (the socket structure also contains other data, such as a port). 
        //The type of sin_addr is a union, so it can be accessed in three different ways : 
        //as s_un_b (four 1-byte integers), s_un_w (two 2-bytes integers) or as s_addr (one 4-bytes integer).

        internal sockaddr_in()
        {
            sin_addr = new in_addr();
        }
    }
    internal class sockaddr
    {
        /*    unsigned short sa_family;   // address family, AF_xxx
                char           sa_data[14]; // 14 bytes of protocol address*/
    }
    public class fd_set //LINUX
    {
        /*void FD_SET(int fd, fd_set *set);*/

        //private List<int> FileDescriptorsToWatch;
        public List<LinuxSocket> linuxSockets;

        public void AddDescriptor(int newDescriptor)
        {
            LinuxSocket linkedSocket = SocketListManager.GetSocket(newDescriptor);
            if (linkedSocket != null)
            {
                linuxSockets.Add(linkedSocket);

            }
            else
            {
                Debug.LogError("Socket not found for fd_set add");
            }
        }
        public bool CanRead(int fDescriptor)
        {
            LinuxSocket linkedSocket = SocketListManager.GetSocket(fDescriptor);
            if (linkedSocket != null)
            {
                return linkedSocket.CanRead;
            }
            else
            {
                Debug.LogError("Socket not found for fd_set check read");
                return false;
            }
        }
        public bool CanWrite(int fDescriptor)
        {
            LinuxSocket linkedSocket = SocketListManager.GetSocket(fDescriptor);
            if (linkedSocket != null)
            {
                return linkedSocket.CanWrite;
            }
            else
            {
                Debug.LogError("Socket not found for fd_set check read");
                return false;
            }
        }
        public bool IsExceptioned(int fDescriptor)
        {
            LinuxSocket linkedSocket = SocketListManager.GetSocket(fDescriptor);
            if (linkedSocket != null)
            {
                return linkedSocket.isExceptioned;
            }
            else
            {
                Debug.LogError("Socket not found for fd_set check read");
                return false;
            }
        }
    }

    public static class SocketListManager
    {
        public static List<LinuxSocket> FileDescriptors = new List<LinuxSocket> { };
        //static int nextSocketNum = 0;

        public static LinuxSocket AddSocket()
        {
            return AddSocket(null, SocketState.LISTEN);
        }
        internal static LinuxSocket AddSocket(CLibPThread.pthread_t linkedthread) //defunct
        {
            return AddSocket(linkedthread, SocketState.LISTEN);
        }

        internal static LinuxSocket AddSocket(CLibPThread.pthread_t linkedthread, SocketState startState)
        {
            if (ServerManager.latestPlayerID == ServerManager.ServerSocketNum) //no players joined yet, so this is the server socket
            {
                int socketNum = ServerManager.latestPlayerID;
                LinuxSocket newSocket = new LinuxSocket(socketNum, startState, linkedthread);
                FileDescriptors.Add(newSocket);
                return newSocket;
            }
            else
            {
                if (FileDescriptors.Find(x => x.FileDescriptor == ServerManager.latestPlayerID) != null)
                {
                    Debug.LogError("Attempt to reuse a playerID for new socket");
                    return null;
                }
                else
                {
                    Debug.Log("creating socket for network player " + ServerManager.latestPlayerID);
                    int socketNum = ServerManager.latestPlayerID;
                    LinuxSocket newSocket = new LinuxSocket(socketNum, startState, linkedthread);
                    FileDescriptors.Add(newSocket);
                    return newSocket;
                }
            }
        }

        public static LinuxSocket GetSocket(int descriptorNum)
        {
            LinuxSocket linkedSocket = FileDescriptors.Find(x => x.FileDescriptor == descriptorNum);
            return linkedSocket;
        }

        internal static LinuxSocket GetSocketForThread(Thread currentThread)
        {
            LinuxSocket linkedSocket = FileDescriptors.Find(x => x.sourceThread.associatedThread == currentThread); //todo: don't know whether this will work
            return linkedSocket;
        }
    }

    public enum SocketState { LISTEN, ESTABLISHED, SYN_SENT, SYN_RECV, LAST_ACK, CLOSE_WAIT, TIME_WAIT, CLOSED, CLOSING, FIN_WAIT1, FIN_WAIT2 }
    /*State	Description
LISTEN	accepting connections
ESTABLISHED	connection up and passing data
SYN_SENT	TCP; session has been requested by us; waiting for reply from remote endpoint
SYN_RECV	TCP; session has been requested by a remote endpoint for a socket on which we were listening
LAST_ACK	TCP; our socket is closed; remote endpoint has also shut down; we are waiting for a final acknowledgement
CLOSE_WAIT	TCP; remote endpoint has shut down; the kernel is waiting for the application to close the socket
TIME_WAIT	TCP; socket is waiting after closing for any packets left on the network
CLOSED	socket is not being used (FIXME. What does mean?)
CLOSING	TCP; our socket is shut down; remote endpoint is shut down; not all data has been sent
FIN_WAIT1	TCP; our socket has closed; we are in the process of tearing down the connection
FIN_WAIT2	TCP; the connection has been closed; our socket is waiting for the remote endpoint to shut down*/

    public class LinuxSocket
    {
        public int FileDescriptor;
        private bool canRead; //A file descriptor is considered ready if it is possible to perform a corresponding I/O operation(e.g., read(2), or a sufficiently small write(2)) without blocking.
        public bool CanWrite;
        public bool isExceptioned;
        public SocketState currentState;
        public bool reuseAddr;
        internal sockaddr assignedAddress;
        public List<int> pendingConnRequests;
        internal CLibPThread.pthread_t sourceThread;
        internal int maxPendingConns;
        //internal bool messageWaiting;
        internal List<byte[]> waitingMessages;

        public bool CanRead
        {
            get
            {
                if (FileDescriptor == ServerManager.ServerSocketNum && pendingConnRequests.Count > 0
                    && currentState != SocketState.CLOSED)
                {
                    canRead = true; // to allow accept of new connections
                }
                else
                {
                    if (currentState != SocketState.CLOSED)
                        canRead = waitingMessages.Count > 0;
                    else
                        canRead = false;
                }
                return canRead;
            }

            set
            {
                canRead = value;
            }
        }

        internal LinuxSocket(int socketNum, SocketState startState, CLibPThread.pthread_t linkedthread)
        {
            FileDescriptor = socketNum;
            CanRead = true;
            CanWrite = true;
            isExceptioned = false;
            //messageWaiting = false;
            waitingMessages = new List<byte[]> { };
            currentState = startState;
            pendingConnRequests = new List<int> { };

            UnityCServerInterface unityintf = UnityCServerInterface.GetInstance();
            if (unityintf)
            {
                unityintf.AddSocket(FileDescriptor);
            }
            else
            {
                Debug.LogError("Failed to add new LinuxSocket to UnityCServerInterface socket dictionary");
            }

            if (linkedthread != null)
            {
                sourceThread = linkedthread;
            }
            else
            {
                //defunct - thread is created after socket
                //foreach (CLibPThread.pthread_t thread in CLibPThread.knownThreads.Values)
                //{
                //    if (thread.associatedThread == Thread.CurrentThread)
                //    {
                //        sourceThread = thread;
                //    }
                //}
            }
            //if (sourceThread == null) //defunct - thread is created after socket
            //Debug.LogError("Could not trace p_thread for pid on new socket");
        }
        //public void Open()
        //{
        //    CanRead = true;
        //    CanWrite = true;
        //    currentState = SocketState.LISTEN;
        //}
        public void Close()
        {
            CanRead = false;
            CanWrite = false;
            currentState = SocketState.CLOSED;
        }
        public int AddPendingConnection(int newConn) //todo: must be called
        {
            if (pendingConnRequests.Count < maxPendingConns)
            {
                pendingConnRequests.Add(newConn);
                return 0;
            }
            else
                return -1;
        }

        internal void SetMessage(byte[] message)
        {
            //todo: locking
            waitingMessages.Add(message);
            Debug.Log("socket " + FileDescriptor + ": message added, count: " + waitingMessages.Count);
            //latestMessage = message;
            //messageWaiting = true;
        }
        internal byte[] GetMessage()
        {
            //todo: locking
            byte[] message = null;
            if (waitingMessages.Count > 0)
            {
                message = waitingMessages[0];
                waitingMessages.RemoveAt(0);
            }
            //latestMessage = new byte[] { };
            //messageWaiting = false;
            return message;
        }
    }

    //these values are all guesses. actual value for ints is likely to be irrelevant
    internal static uint AF_INET = 1;     //sa_family_t is uint
    public static int SOCK_STREAM = 0;
    public static int SOMAXCONN = 10;
    public static int SOL_SOCKET = 0;
    public static bool SO_REUSEADDR = false;

    internal static int socket(int aF_INET, int sOCK_STREAM, int protocol, ref int errno)
    {
        /*socket() creates an endpoint for communication and returns a file descriptor that refers to that endpoint.  
         * The file descriptor returned by a successful call will be the lowest-numbered file descriptor not currently open for the process.
       The domain argument specifies a communication domain; this selects the protocol family which will be used for communication.  These families are defined in <sys/socket.h>.
       SOCK_STREAM to open a tcp(7) socket. Provides sequenced, reliable, two-way, connection-based byte streams.  An out-of-band data transmission mechanism may be supported.
       AF_INET      IPv4 Internet protocols                    ip(7)

         int socket(int domain, int type, int protocol);
         
         On success, a file descriptor for the new socket is returned.  On error, -1 is returned, and errno is set appropriately.*/

        //if ((the_socket = LinuxLibSocket.socket(LinuxLibSocket.AF_INET, LinuxLibSocket.SOCK_STREAM, 0)) == -1)

        LinuxSocket newSocket = SocketListManager.AddSocket();
        return newSocket.FileDescriptor;
    }
    internal static int setsockopt(int the_socket, int sOL_SOCKET, int sO_REUSEADDR, char on, int memsize, ref int errno)
    {
        /*getsockopt() and setsockopt() manipulate options for the socket referred to by the file descriptor sockfd. 
         * To manipulate options at the sockets API level, level is specified as SOL_SOCKET.
         * int argument for optval. For setsockopt(), the argument should be nonzero to enable a boolean option, or zero if the option is to be disabled.
         The arguments optval and optlen are used to access option values for setsockopt().

         int getsockopt(int sockfd, int level, int optname, void *optval, socklen_t *optlen);
         int setsockopt(int sockfd, int level, int optname, const void *optval, socklen_t optlen);
         
         For example, suppose we want to set the socket option to reuse the address to 1 (on/true), we pass in the "level" SOL_SOCKET and the value we want it set to.
        int value = 1;            setsockopt(mysocket, SOL_SOCKET, SO_REUSEADDR, &value, sizeof(value));

        On success, zero is returned. On error, -1 is returned, and errno is set appropriately.
         */

        //error = LinuxLibSocket.setsockopt(the_socket, LinuxLibSocket.SOL_SOCKET, LinuxLibSocket.SO_REUSEADDR, (char)on, LinuxLibSocket.Csizeof(on));

        if (LinuxLibSocket.SOL_SOCKET == sOL_SOCKET)
            LinuxLibSocket.SO_REUSEADDR = on == 0 ? false : true;
        else
            Debug.LogError("Unanticipated setsockopt call");

        //unnecessary for unity
        return 0;

    }

    internal static void close(int the_socket)
    {
        //todo: confirm against API

        LinuxSocket requestedSocket = SocketListManager.GetSocket(the_socket);
        if (requestedSocket != null)
        {
            requestedSocket.Close();
        }
        else
            Debug.LogError("Could not find socket to close");

    }
    
    internal class hostent
    {
        public string h_name { get; internal set; }
    }

    internal static int SOCKETselect(int nfds, fd_set readfds, int writefds, int exceptfds, int[] timeout, ref int errno) //LINUX
    {
        /*select() and pselect() allow a program to monitor multiple file descriptors, waiting until one or more of the file descriptors become
       "ready" for some class of I/O operation (e.g., input possible).  A file descriptor is considered ready if it is possible to perform a
       corresponding I/O operation (e.g., read(2), or a sufficiently small write(2)) without blocking.
         int select(int nfds, fd_set *readfds, fd_set *writefds, fd_set *exceptfds, struct timeval *timeout);*/

        /* check for connections to be accepted */
        //error = LinuxLibSocket.SOCKETselect(the_socket + 1, rmask, 0, 0, timeout);

        //phantasia only calls this once and only with a readfds set, and with no timeout

        List<LinuxSocket> theSockets = readfds.linuxSockets;
        bool anySocketReady = false;
        int numReady = 0;
        bool usingTimeout = timeout[0] > 0 ? true : false; //this only uses the first element of timeout since phantasia only checks readfds //timeout should always be false for phantasia but have prepared code anyway
        long millisecondsAtStart = 0;
        if (usingTimeout)
            millisecondsAtStart = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

        int count = 0;
        //wait for a socket to be ready
        try
        {
            do
            {
                numReady = 0;
                
                count++;
                if (count > 50000000)
                {
                    count = 0;
                    string msg = System.Threading.Thread.CurrentThread.Name + " checking socket(s) for select. sockets available: " 
                        + theSockets.Count;
                    Debug.Log(msg);
                    if (theSockets.Count > 0)
                    {
                        //msg = "first socket is " + theSockets[0].FileDescriptor + " with canread " + theSockets[0].CanRead;
                        //Debug.Log(msg);
                    }
                }

                foreach (LinuxSocket socket in theSockets)
                {
                    //Debug.Log("socket canread: " + socket.CanRead + " with fd: " + socket.FileDescriptor + " and nfds: " + nfds);
                    if (socket.CanRead && socket.FileDescriptor <= nfds)
                    {
                        //Debug.Log("socket ready for select!");
                        anySocketReady = true;
                        numReady++;
                    }
                }
                if (usingTimeout)
                {
                    long timeNow = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                    int timeSinceStart = (int)(timeNow - millisecondsAtStart);
                    timeout[0] -= timeSinceStart;
                }

                if (UnityGameController.StopApplication) //time to quit
                {
                    Debug.Log("Thread " + System.Threading.Thread.CurrentThread.Name + " stopping in SOCKETselect");
                    return -1;
                }
                else
                {
                    System.Threading.Thread.Sleep(33); //30fps
                }

            }
            while (!anySocketReady && (!usingTimeout || (usingTimeout && timeout[0] > 0))); //todo: don't use while? caller allows for a '0 sockets ready' response
        }
        catch
        {
            errno = -1;
            numReady = -1;

            if (UnityGameController.StopApplication)
            {
                Debug.Log("Thread " + System.Threading.Thread.CurrentThread.Name + " stopping in SOCKETselect");
            }
        }

        return numReady;

        /*On success, select() and pselect() return the number of file descriptors contained in the three returned descriptor sets (that is,
       the total number of bits that are set in readfds, writefds, exceptfds) which may be zero if the timeout expires before anything
       interesting happens.  On error, -1 is returned, and errno is set to indicate the error; the file descriptor sets are unmodified, and timeout becomes undefined.*/
    }
    internal static void FD_ZERO(fd_set rmask) //LINUX
    {
        /* FD_ZERO() clears a set. 
         void FD_ZERO(fd_set *set);*/

        rmask.linuxSockets = new List<LinuxSocket>();
    }
    public static void FD_SET(int the_socket, fd_set rmask) //LINUX
    {
        /*FD_SET() and FD_CLR() respectively add and remove a given file descriptor from a set.
         void FD_SET(int fd, fd_set *set);*/

        LinuxSocket requestedSocket = SocketListManager.GetSocket(the_socket);
        if (requestedSocket != null)
            rmask.linuxSockets.Add(requestedSocket);
        else
            Debug.LogError("Could not find socket to add to fd_set");
    }
    internal static bool FD_ISSET(int the_socket, fd_set rmask) //LINUX
    {
        /*FD_ISSET() tests to see if a file descriptor is part of the set; this is useful after select() returns.
         int  FD_ISSET(int fd, fd_set *set);*/

        bool isIncluded = false;
        LinuxSocket requestedSocket = SocketListManager.GetSocket(the_socket);
        if (requestedSocket != null)
        {
            if (rmask.linuxSockets.Contains(requestedSocket))
            {
                isIncluded = true;
            }
        }
        else
            Debug.LogError("Could not find socket to check in fd_set");

        return isIncluded;
    }

    internal static int SOCKETsend(int socket, string out_buffer, uint out_buffer_size, int v, ref int errno)
    {
        /* ssize_t send(int sockfd, const void *buf, size_t len, int flags);
         * The system calls send(), sendto(), and sendmsg() are used to transmit a message to another socket.

       The send() call may be used only when the socket is in a connected state (so that the intended recipient is known).  The only difference
       between send() and write(2) is the presence of flags.  With a zero flags argument, send() is equivalent to write(2).
       
         On success, these calls return the number of bytes sent.  On error, -1 is returned, and errno is set appropriately.*/

        int result = 0;

        LinuxSocket requestedSocket = SocketListManager.GetSocket(socket);
        if (requestedSocket != null)
        {
            string msg = out_buffer.Substring(0, (int)out_buffer_size);//.Replace('\0','$');
            //Debug.LogError("Questionmark debug: " + msg.Replace('\0', '$'));
            byte[] data = Encoding.ASCII.GetBytes(msg);
            UnityCServerInterface unityintf = UnityCServerInterface.GetInstance();
            if (unityintf)// && requestedSocket.sourceThread != null)
            {
                //Rpc method has to happen on main thread
                if (unityintf.Sockets.ContainsKey(requestedSocket.FileDescriptor))
                {
                    unityintf.Sockets[requestedSocket.FileDescriptor].Add(data);

                    //unityintf.RpcSendDataToSpecificClient((short)requestedSocket.FileDescriptor, data); //player network ID is set as FileDescriptor
                    //unityintf.RpcSendDataToSpecificClient((short)requestedSocket.sourceThread.tID, data); //defunct
                }
                else
                {
                    Debug.LogError("Could not find socket " + requestedSocket.FileDescriptor + " in UnityCServerInterface instance");
                    errno = -1;
                    result = -1;
                }
            }
            else
            {
                if (!UnityGameController.StopApplication)
                {
                    Debug.LogError("Could not find UnityCServerInterface instance to fcntl setown");
                }
                errno = -1;
                result = -1;
            }
            result = data.Length;
        }
        else
        {
            Debug.LogError("Could not find socket to fcntl setown");
            errno = -1;
            result = -1;
        }

        //return (int)out_buffer_size; //phantasia expects number of bytes sent to equal out_buffer_size, but out_buffer_size is the length of the string sent
        return result;
    }
    internal static int SOCKETrecv(int socket, ref string buf, uint bufWriteIndex, long len, int flags, ref int errno)
    {
        /*ssize_t recv(int sockfd, void *buf, size_t len, int flags);
        The recv(), recvfrom(), and recvmsg() calls are used to receive messages from a socket. They may be used to receive data on both connectionless and connection-oriented sockets. 
        This page first describes common features of all three system calls, and then describes the differences between the calls.
        The only difference between recv() and read(2) is the presence of flags. With a zero flags argument, recv() is generally equivalent to read(2) (but see NOTES). Also, the following call
            recv(sockfd, buf, len, flags);
        is equivalent to
            recvfrom(sockfd, buf, len, flags, NULL, NULL);
        All three calls return the length of the message on successful completion. If a message is too long to fit in the supplied buffer, excess bytes may be discarded depending on the type
        of socket the message is received from.
        If no messages are available at the socket, the receive calls wait for a message to arrive, unless the socket is nonblocking (see fcntl(2)), in which case the value -1 is returned and 
        the external variable errno is set to EAGAIN or EWOULDBLOCK. The receive calls normally return any data available, up to the requested amount, rather than waiting for receipt of the full amount requested.
        An application can use select(2), poll(2), or epoll(7) to determine when more data arrives on a socket.  
        
       The recv() call is normally used only on a connected socket (see connect(2)).  It is equivalent to the call:   recvfrom(fd, buf, len, flags, NULL, 0);
       recvfrom() places the received message into the buffer buf.  The caller must specify the size of the buffer in len.

       ssize_t recv(int sockfd, void *buf, size_t len, int flags);
       
         These calls return the number of bytes received, or -1 if an error occurred.  In the event of an error, errno is set to indicate the error.*/

        int resultLength = 0;

        //bool messageWaiting = false;
        byte[] message = new byte[] { };
        LinuxSocket requestedSocket = SocketListManager.GetSocket(socket);
        if (requestedSocket != null)
        {
            while (requestedSocket.waitingMessages.Count == 0) //assuming that waiting blocks the thread by default
            {
                //wait for a message to arrive
                if (UnityGameController.StopApplication) //time to quit
                {
                    Debug.Log("Thread " + System.Threading.Thread.CurrentThread.Name + " stopping in SOCKETrecv");
                    errno = -1;
                    return -1;
                }
                else
                {
                    System.Threading.Thread.Sleep(33); //30fps
                }
            }
            message = requestedSocket.GetMessage();
            resultLength = message.Length;

            string messagestr = Encoding.ASCII.GetString(message);
            //buf += messagestr; //not sufficient, must write from provided index, which is not always append; sometimes overwrites
            char[] bufAsChar = new char[phantdefs.SZ_IN_BUFFER];
            if (buf != null && buf.Length > 0)
                buf.ToCharArray().CopyTo(bufAsChar, 0);
            for (int i = 0; i < messagestr.Length; i++)
            {
                bufAsChar[bufWriteIndex + i] = messagestr[i];
            }
            buf = new string(bufAsChar);
        }
        else
        {
            Debug.LogError("Socket not found for recv " + socket);
        }

        return resultLength;
    }

    internal static int htonl(int hostlong)
    {
        /* The htonl() function converts the unsigned integer hostlong from host byte order to network byte order.

        On the i386 the host byte order is Least Significant Byte first, whereas the network byte order, as used on the Internet, is Most Significant Byte first.*/

        //unnecessary in unity
        return hostlong;
    }

    internal static int ntohl(int netlong) //socket
    {
        /*The ntohl() function converts the unsigned integer netlong from network byte order to host byte order.*/

        //unnecessary in unity?
        return netlong;
    }
    internal static string inet_ntoa(in_addr sin_addr)
    {
        /*The inet_ntoa() function converts the Internet host address in, given in network byte order, to a string in IPv4 dotted-decimal notation. 
         * The string is returned in a statically allocated buffer, which subsequent calls will overwrite.*/

        //unnecessary in unity?

        //Debug.LogError("IP debug: " + sin_addr.s_addr.ToString());
        return sin_addr.s_addr.ToString();
    }
    public static int F_SETOWN = 1; //value likely irrelevant
    /*Set the process ID or process group ID that will receive SIGIO
              and SIGURG signals for events on the file descriptor fd.  The
              target process or process group ID is specified in arg.  A
              process ID is specified as a positive value; a process group
              ID is specified as a negative value.  Most commonly, the call‐
              ing process specifies itself as the owner (that is, arg is
              specified as getpid(2)).*/
    internal static int fcntl(int socket, int f_SETOWN, int clientPid, ref int errno)
    {
        /*int fcntl(int fd, int cmd, ... [arg] );
       fcntl() performs one of the operations described below on the open file descriptor fd.The operation is determined by cmd.
       
         F_SETOWN (int)
              Set the process ID or process group ID that will receive SIGIO and SIGURG signals for events on the file descriptor fd.  The
              target process or process group ID is specified in arg.  A process ID is specified as a positive value; a process group
              ID is specified as a negative value.  Most commonly, the calling process specifies itself as the owner (that is, arg is
              specified as getpid(2)).

        For a successful call, the return value [for f_SETOWN is 0]. On error, -1 is returned, and errno is set appropriately.
*/
        int result = 0;
        LinuxSocket requestedSocket = SocketListManager.GetSocket(socket);
        if (requestedSocket != null)
        {
            //unnecessary for unity; signals are received on the relevant pthread; threads are already linked to linuxsockets at linuxsocket creation
            //requestedSocket.sourceThread = CLibPThread.knownThreads[clientPid.ToString()].associatedThread; //if replacement is wanted. nb: can't recall if clientPid is knownThreads key, check if needed
            result = 0;
        }
        else
        {
            Debug.LogError("Could not find socket to fcntl setown");
            errno = -1;
            result = -1;
        }

        return result;
    }
    internal static void alarm(int time)
    {
        /*The alarm() function shall cause the system to generate a SIGALRM signal for the process after the number of realtime seconds specified by seconds have elapsed. 
         * Processor scheduling delays may prevent the process from handling the signal as soon as it is generated.*/

        UnityCServerInterface unityintf = UnityCServerInterface.GetInstance();
        if (unityintf == null)
            Debug.LogError("No CServerInstance available for alarm function");
        else
        {
            //unityintf.DoSIGALRMCountdownSocket(time, SocketListManager.GetSocketForThread(Thread.CurrentThread)); //todo: link thread to socket on creation
            unityintf.DoSIGALRMCountdownForThread(time, Thread.CurrentThread);
        }
    }
    internal static int bind(int the_socket, sockaddr bind_address, int addrlen, ref int errno)
    {
        /*When a socket is created with socket(2), it exists in a name space (address family) but has no address assigned to it.  bind() assigns
       the address specified by addr to the socket referred to by the file descriptor sockfd.  addrlen specifies the size, in bytes, of the
       address structure pointed to by addr.  Traditionally, this operation is called “assigning a name to a socket”.

         int bind(int sockfd, const struct sockaddr *addr, socklen_t addrlen);
         
         On success, zero is returned.  On error, -1 is returned, and errno is set appropriately.*/

        int result = 0;
        LinuxSocket requestedSocket = SocketListManager.GetSocket(the_socket);
        if (requestedSocket != null)
        {
            requestedSocket.assignedAddress = bind_address;
        }
        else
        {
            Debug.LogError("Could not find socket to bind");
            errno = -1;
            result = -1;
        }

        return result;
    }
    internal static int listen(int the_socket, int sOMAXCONN, ref int errno)
    {
        /*listen() marks the socket referred to by sockfd as a passive socket, that is, as a socket that will be used to accept incoming connection requests using accept(2).
       The sockfd argument is a file descriptor that refers to a socket of type SOCK_STREAM or SOCK_SEQPACKET.
       The backlog argument defines the maximum length to which the queue of pending connections for sockfd may grow.  If a connection request
       arrives when the queue is full, the client may receive an error with an indication of ECONNREFUSED or, if the underlying protocol supports
       retransmission, the request may be ignored so that a later reattempt at connection succeeds.
       
       int listen(int sockfd, int backlog);
       
        On success, zero is returned.  On error, -1 is returned, and errno is set appropriately.
         */

        int result = 0;
        LinuxSocket requestedSocket = SocketListManager.GetSocket(the_socket);
        if (requestedSocket != null)
        {
            requestedSocket.currentState = SocketState.LISTEN;
            requestedSocket.CanRead = false;
            requestedSocket.CanWrite = false;
            requestedSocket.maxPendingConns = sOMAXCONN;
        }
        else
        {
            Debug.LogError("Could not find socket to listen to");
            errno = -1;
            result = -1;
        }

        return result;
    }
    internal static int accept(int the_socket, sockaddr address, int addrlen, ref int errno)
    {
        /*The accept() system call is used with connection-based socket types (SOCK_STREAM, SOCK_SEQPACKET).  It extracts the first connection
       request on the queue of pending connections for the listening socket, sockfd, creates a new connected socket, and returns a new file
       descriptor referring to that socket.  The newly created socket is not in the listening state.  The original socket sockfd is unaffected by
       this call.
         int accept(int sockfd, struct sockaddr *addr, socklen_t *addrlen);
       On error, -1 is returned, and errno is set appropriately.*/

        int newFD = 0;
        LinuxSocket requestedSocket = SocketListManager.GetSocket(the_socket);
        if (requestedSocket != null)
        {
            if (requestedSocket.pendingConnRequests.Count > 0)
            {
                LinuxSocket newSocket = SocketListManager.AddSocket(null, SocketState.ESTABLISHED);
                newSocket.CanRead = true;
                newSocket.CanWrite = true;

                newFD = newSocket.FileDescriptor;
                requestedSocket.pendingConnRequests.RemoveAt(0); //todo: populate pendingConnRequests somewhere
            }
            else
            {
                Debug.LogError("Socket to accept from does not contain a pending connection");
                errno = -1;
                newFD = -1;
            }
        }
        else
        {
            Debug.LogError("Could not find socket to accept a pending connection from");
            errno = -1;
            newFD = -1;
        }

        return newFD;
    }



    //only used in socket.cs
    internal static bool finite(double theDouble)
    {
        /*The finite() functions return a nonzero value if x is neither infinite nor a "not-a-number" (NaN) value, and 0 otherwise.*/
        bool result = true; // nonzero
        if (Double.IsInfinity(theDouble) || Double.IsNaN(theDouble))
        {
            result = false;
        }
        return result;
    }
    internal static bool isxdigit(char string_ptr)
    {
        /*The isxdigit() function checks whether a character is a hexadecimal digit character (0-9, a-f, A-F) or not.
         Return Value	            Remarks
        Non-zero integer (x > 0)	Argument is a hexadecimal character.
        Zero (0)	                Argument is not a hexadecimal character.*/

        Regex objHexPattern = new Regex("[^a-fA-F0-9]"); //finds a letter that does NOT match a-fA-F0-9
        bool isNotHex = objHexPattern.IsMatch(string_ptr.ToString());
        bool isHex = !isNotHex;
        return isHex; // ismatch true => not a hex char, but function expects true => hex char, so have to switch to false = not hex, true = hex
    }
    internal static int Csizeof<T>(T value)
    {
        /*generates the size of a variable or datatype, measured in the number of char-sized storage units required for the type*/

        //c# equivalent = only for unmanaged types. if an object is passed this will need to be enhanced        //c# ints have size 4
        Type typ = typeof(T);
        if (typ == typeof(int))
        {
            return sizeof(int);
        }
        else if (typ == typeof(sockaddr_in))
        {
            return sizeof(int); //System.Runtime.InteropServices.Marshal.SizeOf(value); //todo: this errors: Type sockaddr_in cannot be marshaled as an unmanaged structure.
            //sizeof's result is ignored by unity interface anyway, so this shouldn't cause problems as is
        }
        //no further implementations needed for phantasia
        return sizeof(int);
    }
}