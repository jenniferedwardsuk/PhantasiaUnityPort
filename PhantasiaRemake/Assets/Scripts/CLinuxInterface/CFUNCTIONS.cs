using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using System.Text.RegularExpressions;
using System.IO;
using phantasiaclasses;

public class CFUNCTIONS
{
    
    public static int F_SETFL { get; internal set; } //todo: move to linuxlibsocket
    /*Set the file status flags to the value specified by arg.  File
              access mode (O_RDONLY, O_WRONLY, O_RDWR) and file creation
              flags (i.e., O_CREAT, O_EXCL, O_NOCTTY, O_TRUNC) in arg are
              ignored.  On Linux, this command can change only the O_APPEND,
              O_ASYNC, O_DIRECT, O_NOATIME, and O_NONBLOCK flags.*/
    public static int O_ASYNC { get; internal set; } //todo: move to linuxlibsocket

    internal static int GetMillisecs(DateTime now)
    {
        string timestr = now.ToString("HHmmssff");
        return Convert.ToInt32(timestr);
    }

    /*The bit that enables asynchronous input mode. If set, then SIGIO signals will be generated when input is available.*/

    public static int ECONNRESET { get; internal set; }
    public static int INADDR_ANY { get; internal set; }
    public static int STATELEN { get; internal set; }

    //main's global variables
    public static int server_hook { get; internal set; }
    internal static random_data randData { get; set; }
    public static string randomStateBuffer { get; internal set; }

    internal static int GetUnixEpoch(DateTime dateTime) //this is for jan 1970. switch to datetime.ticks (since 0001)?
    {
        var unixTime = dateTime.ToUniversalTime() -
            new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        return Convert.ToInt32(unixTime.TotalSeconds);
    }

    internal static bool isalnum(string strToCheck)
    {
        Regex objAlphaNumericPattern = new Regex("[^a-zA-Z0-9]"); //finds a letter that does NOT match a-zA-Z0-9
        return !objAlphaNumericPattern.IsMatch(strToCheck);
    }
    internal static bool isalnum(char charToCheck)
    {
        string strToCheck = charToCheck.ToString();
        return isalnum(strToCheck);
    }
    internal static string strerror(int errno)
    {
        /*The strerror() function returns a pointer to a string that describes
       the error code passed in the argument errnum, possibly using the
       LC_MESSAGES part of the current locale to select the appropriate
       language.*/

        return "(C Error Code " + errno + ")";
    }

    internal static bool isalpha(string strToCheck)
    {
        Regex objAlphaPattern = new Regex("[^a-zA-Z]"); //this is a guess
        return !objAlphaPattern.IsMatch(strToCheck);
    }
    internal static bool isalpha(char charToCheck)
    {
        string strToCheck = charToCheck.ToString();
        return isalpha(strToCheck);
    }

    internal static string tolower(string str)
    {
        return str.ToLower();
    }
    internal static char tolower(char chr)
    {
        //Debug.LogError("tolower: " + new string(new char[] { chr }).Replace('\0','$'));
        string str = chr.ToString();
        return str.ToLower().ToCharArray()[0];
    }

    internal static void initstate_r(uint seed, string randomStateBuffer, int sTATELEN, random_data randData)
    {
        /* seed the random number generator */
        /*int initstate_r(unsigned int seed, char *statebuf, size_t statelen, struct random_data *buf);
          initstate(3) except that it initializes the state in the object pointed to by buf, rather than initializing the global state variable.
          These functions are the reentrant equivalents of the functions described in random(3). They are suitable for use in multithreaded 
          programs where each thread needs to obtain an independent, reproducible sequence of random numbers.*/

        randData = new random_data();
        randData.Seed = (int)seed;
    }

    internal static bool isupper(char chr)
    {
        string str = chr.ToString();
        Regex objAlphaPattern = new Regex("[^A-Z]"); //this is a guess
        return !objAlphaPattern.IsMatch(str);
    }
   
    internal static void printf(string message, string buffer, string pathname, string strerror) //("%s [?:?] Can not open error file %s in Do_log: %s.\n", string_buffer, pathnames.ERROR_LOG, ""); 
    {
        /*The C library function int printf(const char *format, ...) sends formatted output to stdout.*/
        Debug.Log("CFUNCTIONS.printf:" + message);
    }

    internal static void printf(string message, string buffer, string strerror)
    {
        printf(message, buffer, "", strerror);
    }

    internal static void printf(string message)
    {
        Debug.Log("CFUNCTIONS.printf:" + message);
    }
    
    internal static bool isgraph(char cchar)//(cstring[--length])
    {
        /*The isgraph() checks whether a character is a graphic character or not. If the argument passed to isgraph() is a graphic character, 
         * it returns a non-zero integer. If not, it returns 0.
         When character is passed as an argument, corresponding ASCII value of the character is passed instead of that character itself.*/

        if (Char.IsControl(cchar) || cchar == ' ')
        {
            return false;
        }
        return true;

    }

    internal static void sprintf(ref string target, string str, params object[] paramstrs) //(theHistory.description, "%s restored the character named %s\n", c.modifiedName, readPlayer.name);
    { //todo: string target as ref string? causes unity crash
      /*The C library function int sprintf(char *str, const char *format, ...) sends formatted output to a string pointed to, by str.*/
      //todo: str1 purpose?

        //error_msg = CFUNCTIONS.sprintfSinglestring("[%s] Connection on socket %d.\n", c.connection_id, c.socket);

        //if (str != "%s %s" && !str.Contains("Mutex") && !str.Contains("mutex"))
            //Debug.Log("string format debug: str: " + str);

        string readystr = str;
        int paramNum = 0;
        
        //if (str != "%s %s" && !str.Contains("Mutex") && !str.Contains("mutex"))
            //Debug.Log("string format debug: str: " + str);

        foreach (object obj in paramstrs)
        {
            //insert param num
            readystr = ReplaceFirst(readystr, "%", "{" + paramNum + ":");
            //add closing bracket for this paramnum //inefficient but wcyd
            readystr = readystr.Replace("{" + paramNum + ":s", "{" + paramNum + ":s}");
            readystr = readystr.Replace("{" + paramNum + ":d", "{" + paramNum + ":d}");
            readystr = readystr.Replace("{" + paramNum + ":ld", "{" + paramNum + ":ld}");
            readystr = readystr.Replace("{" + paramNum + ":1d", "{" + paramNum + ":1d}");
            readystr = readystr.Replace("{" + paramNum + ":2.0f", "{" + paramNum + ":2.0f}");
            readystr = readystr.Replace("{" + paramNum + ":.01f", "{" + paramNum + ":.01f}");
            readystr = readystr.Replace("{" + paramNum + ":.0lf", "{" + paramNum + ":.0lf}");
            readystr = readystr.Replace("{" + paramNum + ":0.1f", "{" + paramNum + ":0.1f}");
            readystr = readystr.Replace("{" + paramNum + ":0.lf", "{" + paramNum + ":0.lf}");
            readystr = readystr.Replace("{" + paramNum + ":3.0f", "{" + paramNum + ":3.0f}");
            readystr = readystr.Replace("{" + paramNum + ":6.0f", "{" + paramNum + ":6.0f}");
            readystr = readystr.Replace("{" + paramNum + ":9.0f", "{" + paramNum + ":9.0f}");
            readystr = readystr.Replace("{" + paramNum + ":-12s", "{" + paramNum + ":-12s}");
            readystr = readystr.Replace("{" + paramNum + ":-l2s", "{" + paramNum + ":-l2s}");
            readystr = readystr.Replace("{" + paramNum + ":6d", "{" + paramNum + ":6d}");
            readystr = readystr.Replace("{" + paramNum + ":.0f", "{" + paramNum + ":.0f}");
            readystr = readystr.Replace("{" + paramNum + ":lf", "{" + paramNum + ":lf}");
            readystr = readystr.Replace("{" + paramNum + ":1f", "{" + paramNum + ":1f}");
            readystr = readystr.Replace("{" + paramNum + ":hd", "{" + paramNum + ":d}"); //hd is ignored, replaced with d. "[0.0.0.0:%d] bad realm object of type %hd read in Do_load_data_file.\n"
            readystr = readystr.Replace("{" + paramNum + ":.21f", "{" + paramNum + ":.21f}");
            readystr = readystr.Replace("{" + paramNum + ":.2lf", "{" + paramNum + ":.2lf}");
            readystr = readystr.Replace("{" + paramNum + ":2.2d", "{" + paramNum + ":2.2d}");
            readystr = readystr.Replace("{" + paramNum + ":02x", "{" + paramNum + ":02x}"); //todo: likely ignored
            readystr = readystr.Replace("{" + paramNum + ":x", "{" + paramNum + ":s}"); //todo: still ignored as s
            readystr = readystr.Replace("{" + paramNum + ":.4lf", "{" + paramNum + ":.4lf}");
            paramNum++;
        }

        //Debug.Log("string to replace: " + readystr + " with numargs " + paramstrs.Length);
        //foreach (object obj in paramstrs)
        //{
        //    Debug.Log("arg: " + obj.ToString());
        //}

        //process params
        object[] paramStrings = new object[paramstrs.Length];
        for (int i = 0; i < paramstrs.Length; i++)
        {
            if (paramstrs[i] != null)
            {
                //todo: early interrupt avoidance needed elsewhere also?
                if (paramstrs[i].GetType() == typeof(char[])) //convert char arrays to strings //todo: interrupts strings...?
                {
                    ////Debug.Log("converting char[] param to string");
                    //char[] param = (char[])paramstrs[i];
                    //for (int j = 0; j < param.Length; j++)
                    //{
                    //    if (param[i] == '\0') //interrupts strings
                    //    {
                    //        param[i] = '$';
                    //    }
                    //}
                    //paramStrings[i] = new string(param);

                    string paramstr = new string((char[])paramstrs[i]);
                    paramstr = paramstr.Replace('\0', '$'); //avoid early interrupt
                    paramstr = paramstr.Replace("\0", "$"); //avoid early interrupt
                    paramStrings[i] = paramstr;
                }
                else if (paramstrs[i].GetType() == typeof(string)) //avoid early interrupt
                {
                    string paramstr = paramstrs[i].ToString();
                    paramstr = paramstr.Replace('\0', '$'); //avoid early interrupt
                    paramstr = paramstr.Replace("\0", "$"); //avoid early interrupt
                    paramStrings[i] = paramstr;
                }
                else if (paramstrs[i].GetType() == typeof(CLibPThread.pid_t)) //convert pid_t type to its implicit int
                {
                    int param = (CLibPThread.pid_t)paramstrs[i];
                    paramStrings[i] = param.ToString(); //"pid_t " + param.ToString();
                }
                else //otherwise write value or type
                {
                    paramStrings[i] = paramstrs[i].ToString();
                }
            }
            else
            {
                paramStrings[i] = null;
            }
        }

        //return
        //if (str != "%s %s" && !str.Contains("Mutex") && !str.Contains("mutex"))
            //Debug.Log("string format debug: target: " + target + " || readystr: " + readystr + " || paramStrings[0]: " + paramStrings[0] + " ...");
        try
        {
            target = String.Format(readystr, paramStrings); //todo: sufficient?
        }
        catch (Exception e)
        {
            Debug.Log("exception in sprintf: " + e.Message + " || " + e.InnerException + " || " + e.StackTrace);
        }
        //if (str != "%s %s" && !str.Contains("Mutex") && !str.Contains("mutex"))
            //Debug.Log("string format debug: target: " + target + " ...");
    }

    //internal static string sprintf(string str1, params object[] paramstrs)
    //{
    //    return sprintf(str1, str1, paramstrs);
    //}
    //if first param is a string, the sprintf two-string overload can be called incorrectly. force-corrected by calling this instead
    internal static string sprintfSinglestring(string str1, params object[] paramstrs)
    {
        sprintf(ref str1, str1, paramstrs);
        return str1;
    }

    // stackoverflow.com/questions/8809354/replace-first-occurrence-of-pattern-in-a-string
    public static string ReplaceFirst(string text, string search, string replace)
    {
        //Debug.Log("null check: text: " + text + " || search: " + search + " || replace: " + replace);
        int pos = text.IndexOf(search);
        if (pos < 0)
        {
            return text;
        }
        return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
    }
    
    internal static string strncpy(ref string dest, string src, int num) //(client_ptr.network, client_ptr.IP, phantdefs.SZ_FROM - 1);
    {
        /*The C library function char *strncpy(char *dest, const char *src, size_t n) copies up to n characters from the string pointed to, by src to dest. 
         * In a case where the length of src is less than that of n, the remainder of dest will be padded with null bytes.*/

        //Debug.LogError("strncpy debug: dest: " + dest + " || src: " + src + " || num: " + num); //IP debug: current IP:  || string_ptr: 0 || phantdefs.SZ_FROM - 1: 80

        if (src.Length >= num)
            dest = src.Substring(0, num);
        else
        {
            dest = src;
            //for (int i = 0; i < (num - src.Length); i++) //unnecessary?
            //{
            //    dest += "";
            //}
        }

        //Debug.LogError("strncpy debug done: dest: " + dest + " || src: " + src + " || num: " + num); //IP debug: current IP:  || string_ptr: 0 || phantdefs.SZ_FROM - 1: 80
        
        return dest; 
    }

    internal static bool strcmp(string string1, string string2) //todo: check whether accidentally reversed signs anywhere (str1==str2 => 0, in C => false as condition, not true)
    {
        //todo
        /*This function return values that are as follows −    
if Return value < 0 then it indicates str1 is less than str2.
if Return value > 0 then it indicates str2 is less than str1.
if Return value = 0 then it indicates str1 is equal to str2.*/

        bool compare = true;
        if (string1 == null || string2 == null) // todo: null strings are not a match?
        {
            compare = false;
            return !compare;
        }

        int str1end = CFUNCTIONS.strlen(string1);
        int str2end = CFUNCTIONS.strlen(string2);
        if (str1end == 0 || str2end == 0) // todo: empty strings are not a match?
        {
            compare = false;
            return !compare;
        }
        if (str1end != str2end)
        {
            compare = false;
            return !compare;
        }

        char[] str1 = string1 != null ? string1.Substring(0, str1end).ToCharArray() : new char[] { };
        char[] str2 = string2 != null ? string2.Substring(0, str2end).ToCharArray() : new char[] { };
        for (int i = 0; i < macros.MIN(str1.Length, str2.Length); i++)
        {
            if (str1[i] != str2[i])
            {
                compare = false;
                break;
            }
        }
        return !compare; //false = 0 = a match, in C
    }

    internal static string strcpy(ref string str1, string str2)
    {
        return str1 = str2;
    }

    internal static int floor(float expr)
    {
        int result = (int)Mathf.Floor((float)expr);
        return result;
    }

    internal static int time(object p)
    {
        /*The C library function time_t time(time_t *seconds) returns the time since the Epoch (00:00:00 UTC, January 1, 1970), measured in seconds. 
         * If seconds is not NULL, the return value is also stored in variable seconds.*/

        //TimeSpan TimeSinceStart = DateTime.Now - new DateTime(1970, 01, 01);
        //Convert.ToInt32(TimeSinceStart.TotalSeconds);// time(null);

        if (p == null)
            return GetUnixEpoch(DateTime.Now);
        else
            throw new NotImplementedException(); //p is always null in phantasia
    }

    internal static int floor(double expr)
    {
        int result = Mathf.FloorToInt((float)expr);
        return result;
    }

    internal static int floor(long expr)
    {
        int result = Mathf.FloorToInt((float)expr);
        return result;
    }

    internal static int ceil(float expr)
    {
        int result = (int)Mathf.Ceil((float)expr);
        return result;
    }
    internal static int ceil(double expr)
    {
        int result = (int)Mathf.Ceil((float)expr);
        return result;
    }
    internal static int ceil(long expr)
    {
        int result = (int)Mathf.Ceil((float)expr);
        return result;
    }

    internal static int strlen(string str)
    {
        //string strFiltered = str.Replace('\0', '$');
        //Debug.Log("CFUNCTIONS.strlen called on " + strFiltered);

        //interprets string as a C string, and returns the count of characters before the first '\0'
        int strlength = 0;
        for (int i = 0; i < str.Length; i++)
        {
            if (str[i] == '\0')
            {
                break;
            }
            else
            {
                strlength++;
            }
        }
        return strlength;
    }

    internal static double sqrt(double numb)
    {
        //cfunctions.sqrt(cfunctions.pow(event_ptr.arg1, 2)
        return Mathf.Sqrt((float)numb);
    }

    internal static double pow(double numb, double power)
    {
        return Mathf.Pow((float)numb, (float)power);
    }

    internal static double MIN(double num1, double num2)
    {
        return Mathf.Min((float)num1, (float)num2);
    }

    internal static double MAX(double num1, double num2)
    {
        return Mathf.Max((float)num1, (float)num2);
    }

    internal static void strcat(ref string str1, string str2)
    {
        str1 = str1.Length == 0 ? "" : str1.Substring(0, CFUNCTIONS.strlen(str1)); //remove \0 if it exists
        str1 = str1 + str2;
    }

    internal static void exit(int PHANTDEFS_ERROR_NUM)
    {
        Debug.Log("CFUNCTIONS.exit");
        if (Thread.CurrentThread == CLibPThread.unityThread)
        {
            Debug.Log("quitting application");
            UnityGameController.StopApplication = true;
            Application.Quit(); //not thread compatible
        }
        else
        {
            //Debug.Log("setting UnityGameController stop flag");
            UnityGameController.StopApplication = true;
            //Thread.CurrentThread.Interrupt(); //this fails to stop current thread, as it waits for the thread to block
            Thread.CurrentThread.Abort(); //this works sometimes but throws a catch-dodging exception
            //letting threads manage themselves
        }
    }


    internal class random_data
    {
        internal System.Random rand;
        private int seed;

        internal int Seed
        {
            get
            {
                return seed;
            }

            set
            {
                seed = value;
                rand = new System.Random(seed);
            }
        }

        //todo: set as global variable in main
    }
    internal static int random_r(random_data randData, ref int result)
    {
        //linux
        /*The random_r() function is like random(3), except that instead of using state information maintained in a global variable, it uses the
       state information in the argument pointed to by buf, which must have been previously initialized by initstate_r().  The generated random
       number is returned in the argument result.
       The random() function uses a nonlinear additive feedback random number generator employing a default table of size 31 long integers
       to return successive pseudo-random numbers in the range from 0 to RAND_MAX.  The period of this random number generator is very large,
       approximately 16 * ((2^31) - 1).

       int random_r(struct random_data *buf, int32_t *result);

         All of these functions return 0 on success.  On error, -1 is returned, with errno set to indicate the cause of the error.*/
         
        if (randData.rand != null)
        {
            result = randData.rand.Next();
            return 0;
        }
        else
        {
            Debug.LogError("Rand not initialised for random_r");
            return -1;
        }
    }

    internal static int srandom_r(int seed, random_data randData)
    {
        //linux
        /*suitable for use in multithreaded programs where each thread needs to obtain an independent, reproducible sequence of random numbers.
         The srandom_r() function is like srandom(3), except that it initializes the seed for the random number generator whose state is
       maintained in the object pointed to by buf, which must have been previously initialized by initstate_r(), instead of the seed
       associated with the global state variable.
       The srandom() function sets its argument as the seed for a new sequence of pseudo-random integers to be returned by random().  These
       sequences are repeatable by calling srandom() with the same seed value.  If no seed value is provided, the random() function is
       automatically seeded with a value of 1.
       
         int srandom_r(unsigned int seed, struct random_data *buf);
         
         All of these functions return 0 on success.  On error, -1 is returned, with errno set to indicate the cause of the error.*/

        if (randData != null)
        {
            randData.Seed = seed;
            return 0;
        }
        else
        {
            //meh, initstate_r is supposed to do this. let this call fail if necessary, to find issues
            //if (CFUNCTIONS.randData == null) //for avoidance of referencing issues. phantasia only has this one random_data in use, safe to shortcut to it
            //{
            //    Debug.Log("CFUNCTIONS random number generator does not exist, creating...");
            //    CFUNCTIONS.randData = new random_data();
            //    CFUNCTIONS.randData.Seed = seed;
            //}
            //randData = new random_data();
            //randData.Seed = seed;
            return -1;
        }
    }

    internal static void ctime_r(int time, ref string string_buffer2)
    {
        /*Convert Time to Character String (Restartable)*/

        //time = CFUNCTIONS.GetUnixEpoch(DateTime.Now);
        //var unixTime = dateTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        //return Convert.ToInt32(unixTime.TotalSeconds);

        DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        DateTime dtws = dt.AddSeconds(time);
        string_buffer2 = dtws.ToString("dd-MMM-yyyy HH:mm:ss");

        //string_buffer2 = time.ToString();

        return;
    }

    internal static bool isnan(float sin)
    {
        if (float.IsNaN(sin))
            return true;

        return false;
    }

    internal static long strtol(string packetTmp, object indexOfEndOfLong, int baseint)
    {
        /*The C library function long int strtol(const char *str, char **endptr, int base) converts the initial part of the string in str to 
         * a long int value according to the given base, which must be between 2 and 36 inclusive, or be the special value 0. 
         * 
         endptr − This is the reference to an object of type char*, whose value is set by the function to the next character in str after the numerical value.
         
         This function returns the converted integral number as a long int value, else zero value is returned.*/

        //object indexOfEndOfLong - object type chosen because always null in phantasia

        long ret;
        if (indexOfEndOfLong != null)
            throw new NotImplementedException(); //indexOfEndOfLong is always null in phantasia
        else
        {
            int numberLength = 0;
            for (int i = 0; i < packetTmp.Length; i++)
            {
                if (Char.IsDigit(packetTmp[i]))
                {
                    numberLength++;
                }
                else
                {
                    break;
                }
            }
            if (numberLength > 0)
                ret = long.Parse(packetTmp.Substring(0, numberLength));
            else
                ret = 0;
        }
        return ret;
    }

    internal static double strtod(string tmpDouble, object endptr)
    {
        /*The C library function double strtod(const char *str, char **endptr) converts the string pointed to by the argument str to a floating-point number (type double). 
         * If endptr is not NULL, a pointer to the character after the last character used in the conversion is stored in the location referenced by endptr.*/
         
        double ret;
        if (endptr != null)
            throw new NotImplementedException(); //endptr is always null in phantasia
        else
        {
            int numberLength = 0;
            for (int i = 0; i < tmpDouble.Length; i++)
            {
                if (Char.IsDigit(tmpDouble[i]) || (i == 0 && tmpDouble[i] == '-'))
                {
                    numberLength++;
                }
                else
                {
                    break;
                }
            }
            if (numberLength > 0)
                ret = long.Parse(tmpDouble.Substring(0, numberLength));
            else
                ret = 0;
        }
        return ret;
    }
    
    internal static int sscanf(string str, string format, ref int minutes)
    {
        /*The C library function int sscanf(const char *str, const char *format, ...) reads formatted input from a string.
         other arguments − This function expects a sequence of pointers as additional arguments, each one pointing to an object of the type specified by their corresponding %-tag within the format string, in the same order.

         On success, the function returns the number of variables filled. In the case of an input failure before any data could be successfully read, EOF is returned.
         */

        //Only called once in phantasia: if (CFUNCTIONS.sscanf(char_ptr, "%d", minutes) != 0)
        //accordingly, can use this very specific implementation:
        try
        {
            minutes = Convert.ToInt32(str);
            return 1;
        }
        catch
        {
            return 0;
        }
    }
    
    internal static int strstr(string haystack, string needle)
    {
        /*The C library function char *strstr(const char *haystack, const char *needle) function finds the first occurrence of the 
         * substring needle in the string haystack. The terminating '\0' characters are not compared.
         
         This function returns a pointer to the first occurrence in haystack of any of the entire sequence of characters specified in needle, or a null pointer if the sequence is not present in haystack.*/

        int index = haystack.IndexOf(needle); // -1 = not found, 0 = haystack is empty
        return index;
    }






    internal static int system(string string_buffer, string name, string value, string email)
    {
        //The C library function int system(const char *command) passes the command name or program name specified by command 
        //to the host environment to be executed by the command processor and returns after the command has been completed.
        //The value returned is -1 on error, and the return status of the command otherwise.

        //used to send email via NEW_ACCOUNT_SCRIPT / ACCOUNT_PASSWORD_RESET_SCRIPT / CHARACTER_PASSWORD_RESET_SCRIPT, in account.Do_account_signup / account.Do_reset_account_password / character.Do_reset_character_password

        //internal static string NEW_ACCOUNT_SCRIPT = pathname + "bin/new_account_mail.pl";
        //internal static string ACCOUNT_PASSWORD_RESET_SCRIPT = pathname + "bin/account_reset_mail.pl";
        //internal static string CHARACTER_PASSWORD_RESET_SCRIPT = pathname + "bin/character_reset_mail.pl";
        try
        {
            string type = "";
            switch (string_buffer.Substring(0, 20))
            {
                case "bin/new_account_mail":
                    type = "NEW_ACCOUNT_SCRIPT";
                    break;
                case "bin/account_reset_ma":
                    type = "ACCOUNT_PASSWORD_RESET_SCRIPT";
                    break;
                case "bin/character_reset_":
                    type = "CHARACTER_PASSWORD_RESET_SCRIPT";
                    break;
            }

            if (type.Equals("NEW_ACCOUNT_SCRIPT"))
                UnityCServerInterface.SendNewAccountScriptEmail(name, value, email);
            else if (type.Equals("ACCOUNT_PASSWORD_RESET_SCRIPT"))
                UnityCServerInterface.SendAccountPasswordResetScriptEmail(name, value, email);
            else if (type.Equals("CHARACTER_PASSWORD_RESET_SCRIPT"))
                UnityCServerInterface.SendCharacterPasswordResetScriptEmail(name, value, email);
            else
            {
                string filteredString = string_buffer.Replace('\0', '$');
                Debug.LogError("CFUNCTIONS.system failed to call unity interface, with type " + type + ", and string_buffer: || " + string_buffer + " ||");
                return -1;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Exception in CFUNCTIONS.system: " + e.Message + " || " + e.InnerException + " || " + e.StackTrace);
            return -1;
        }
        return 0;
    }

    internal static void kill(int processID, int SIG)
    {
        //linux
        /*The kill() function shall send a signal to a process or a group of processes specified by pid. 
         * The signal to be sent is specified by sig and is either one from the list given in <signal.h> or 0. 
         * If sig is 0 (the null signal), error checking is performed but no signal is actually sent. The null signal can be used to check the validity of pid.*/

        //used at end of client thread loop (SIGUSR1) to tell server thread to clean up, and in itcombat (SIGIO) to contact opponent's process

        //todo: want to use pid to activate sig on appropriate thread. check that pid is accurate
        //todo: do this more efficiently
        bool delivered = false;
        foreach(CLibPThread.pthread_t pthread in CLibPThread.knownThreads.Values)
        {
            if (pthread.tID == processID)
            {
                if (SIG == LinuxLibSIG.SIGIO || SIG == LinuxLibSIG.SIGUSR1)
                {
                    pthread.SetSignal(SIG, true);
                }
                else
                {
                    Debug.LogError("Unexpected signal in kill call for phantasia");
                }
                delivered = true;
            }
        }
        if (!delivered)
        {
            Debug.LogError("Could not find thread by pid for signal delivery via kill");
        }
    }

    internal static void memcpy<T>( ref T obj1, T obj2, int size)
    {
        /*The C library function void *memcpy(void *str1, const void *str2, size_t n) copies n characters from memory area str2 to memory area str1.*/
        
        obj1 = obj2; //todo: sufficient? runtime object size can't be quantified in c#

        //alternative option: serialize, copy bytes over, deserialize. but int size isn't reliable since it came from C version
        //byte[] str1bytes = CLibFile.ObjectToByteArray(str1);
        //byte[] str2bytes = CLibFile.ObjectToByteArray(str2);
    }
    internal static void memcpy(ref string str1, string str2, int size)
    {
        /*The C library function void *memcpy(void *str1, const void *str2, size_t n) copies n characters from memory area str2 to memory area str1.*/
        
        memmove(ref str1, str2, (uint)size);
    }

    internal static void memmove(ref string str1, string str2, uint numToCopy)
    {
        /*The C library function void *memmove(void *str1, const void *str2, size_t n) copies n characters from str2 to str1, but for overlapping memory blocks, memmove() is a safer approach than memcpy().*/

        //only used in socket.Do_get_socket_string, to remove the first part of a string
        //CFUNCTIONS.memmove(c.in_buffer, c.in_buffer[theLength], c.in_buffer_size);

        int minsize = Math.Max(str1.Length, (int)numToCopy);
        char[] str1char = new char[minsize];
        str1.ToCharArray().CopyTo(str1char, 0);

        char[] str2char = str2.ToCharArray();

        for (int i = 0; i < numToCopy; i++)
        {
            //'Valhalla' with 9 -> 'Valhalla\0'
            //3W -4\n\0 with 6 -> 3W -4\n\0

            int endstr2 = Math.Min(str2char.Length, (int)numToCopy);

            int indexToAddNullAt = (str2char[endstr2 - 1] != '\0') ? //doesn't have a null at end of numToCopy?
                (int)numToCopy - 1 //add one there
                : str2char.Length - 1; //add one at end of string in case string ends before numToCopy

            if (i < indexToAddNullAt) 
                //CFUNCTIONS.strlen(str2char.ToString())) //causes index issue
                //numToCopy - 1)    //cuts off last char of strings that don't have \0 already
                //str2char.Length - 1)  //doesn't always ensure \0 is added. had issues with no \0 being sent on a str2 that had length 7 (already had a \0) but numtocopy 6.
                str1char[i] = str2char[i];
            else
                str1char[i] = '\0';
        }
        str1 = new string(str1char);
    }

    internal static void free(ref linked_list_t list_ptr)
    {
        //The C library function void free(void *ptr) deallocates the memory previously allocated by a call to calloc, malloc, or realloc.

        list_ptr = null;
    }

    internal static void free(ref player_desc_t description)
    {
        //The C library function void free(void *ptr) deallocates the memory previously allocated by a call to calloc, malloc, or realloc.

        description = null;
    }

    internal static int memcmp(char[] thePassword, char[] digest, int size)
    {
        // 0 is a match, 1 otherwise

        if (thePassword.Length != digest.Length)
            return 1;

        int maxIndex = Math.Min(Math.Min(size, thePassword.Length), digest.Length);
        int ismatch = 0;
        for (int i = 0; i < maxIndex; i++)
        {
            if (thePassword[i] != digest[i])
                ismatch = 1;
        }
        return ismatch;
    }
}
