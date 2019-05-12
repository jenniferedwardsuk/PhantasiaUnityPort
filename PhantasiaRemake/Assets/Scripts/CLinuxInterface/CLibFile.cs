using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using phantasiaclasses;

public class CLibFile : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

    }

    internal class CFILE
    {
        internal FileStream datafile;
        //internal bool CanRead;
        internal string Name;
        internal int errorCode;
        internal string mode;

        internal CFILE(FileStream stream)
        {
            datafile = stream;
            if (stream == null)
            {
                Name = "";
                //CanRead = false;
                errorCode = 1;
            }
            else
            {
                Name = stream.Name;
                //CanRead = stream.CanRead;
                errorCode = 0;
            }
        }

        internal void Close()
        {
            datafile.Close(); //todo: may be run multiple times. necessary to check whether already closed?
        }
    }

    internal static int ferror(CFILE data_file)
    {
        /*If the error indicator associated with the stream was set, the function returns a non-zero value else, it returns a zero value.*/
        if (data_file.errorCode != 0)
            return 1;
        else
            return 0;
    }
    
    internal static void fclose(CLibFile.CFILE file)
    {
        //Debug.Log("CLibFile.fclose"); 
        file.Close();
    }

    static string filesLocation = Directory.GetCurrentDirectory() + "\\Assets\\Scripts\\Original\\";
    internal static CLibFile.CFILE fopen(string filename, string mode, ref int errno) //CLibFile.fopen(filename, "a") "r+"
    {
        /*
1 "r" Opens a file for reading.The file must exist.
2 "w" Creates an empty file for writing.If a file with the same name already exists, its content is erased and the file is considered as a new empty file.
3 "a" Appends to a file.Writing operations, append data at the end of the file.The file is created if it does not exist.
4 "r+" Opens a file to update both reading and writing.The file must exist.
5 "w+" Creates an empty file for both reading and writing.
6 "a+" Opens a file for reading and appending.
*/
        /*
        "r" Open a text file for reading. 
        "w" Open a text file for writing, truncating an an existing file to zero length, or creating the file if it does not exist.
        "r+" Open a text file for update (that is, for both reading and writing). 
        "w+" Open a text file for update (reading and writing), first truncating the file to zero length if it exists or creating the file if it does not exist.
        */

        //Debug.Log("CLibFile.fopen");
        //Debug.Log("filename " + filename + " mode " + mode);

        //if (!filename.Contains("debug"))
           // Debug.LogError("FOPEN DEBUG: OPENING " + filename);

        filename = filesLocation + filename;
        CLibFile.CFILE file = null;
        try
        {
            if (mode == "r")
            {
                file = new CFILE(File.OpenRead(filename));
                file.mode = mode;
            }
            else if (mode == "w")
            {
                if (!File.Exists(filename) && mode == "w")
                {
                    FileStream filestr = File.Create(filename);
                    filestr.Close();
                }
                file = new CFILE(File.OpenWrite(filename));
                file.mode = mode;
            }
            else if (mode == "w+" || mode == "r+")
            {
                if (!File.Exists(filename) && mode == "w+")
                {
                    FileStream filestr = File.Create(filename);
                    filestr.Close();
                }
                file = new CFILE(File.Open(filename, FileMode.Open, FileAccess.ReadWrite));
                file.mode = mode;
            }
            else if (mode == "a")
            {
                if (!File.Exists(filename))
                {
                    FileStream filestr = File.Create(filename);
                    filestr.Close();
                }
                file = new CFILE(new FileStream(filename, FileMode.Append));
                file.mode = mode;
            }

            if (file != null)
            {
                errno = 0;
                return file;
            }
            else
            {
                errno = 1;
                return null;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("fopen exception: " + e.InnerException + " || " + e.Message + " || " + e.StackTrace);
            errno = 1;
            return null;
        }
    }
    
    internal static void remove(string filepath)
    {
        Debug.Log("CLibFile.remove");
        //Debug.LogError("DEBUG: CLibFile.remove: " + filepath);
        File.Delete(filesLocation + filepath);
    }

    internal static void rename(string oldname, string newname)
    {
        Debug.Log("CLibFile.rename");
        //Debug.LogError("DEBUG: CLibFile.rename: " + oldname + " || to || " + newname);
        if (File.Exists(filesLocation + newname))
        {
            File.Delete(filesLocation + newname);
        }
        File.Move(filesLocation + oldname, filesLocation + newname);
    }

    public const int SEEK_SET = 0;
    public const int SEEK_END = 1;
    internal static void fseek(CLibFile.CFILE file, long offset, int whence)
    {
        /*The C library function int fseek(FILE *stream, long int offset, int whence) sets the file position of the stream to the given offset.
         int fseek(FILE *stream, long int offset, int whence)
        stream − This is the pointer to a FILE object that identifies the stream.
        offset − This is the number of bytes to offset from whence.
        whence − This is the position from where offset is added. It is specified by one of the following constants −
        SEEK_SET Beginning of file
        SEEK_CUR Current position of the file pointer
        SEEK_END End of file
        */

        Debug.Log("CLibFile.fseek");
        long position = 0;
        switch (whence)
        {
            case SEEK_SET:
                file.datafile.Seek(offset, SeekOrigin.Begin);
                break;
            case SEEK_END:
                file.datafile.Seek(offset, SeekOrigin.End);
                break;
        }
    }

    internal static long ftell(CLibFile.CFILE file)
    {
        /*The C library function long int ftell(FILE *stream) returns the current file position of the given stream.*/
        Debug.Log("CLibFile.ftell");
        return file.datafile.Position;
    }

    internal static void fprintf(CLibFile.CFILE file, string toFormat, params object[] formatParams)
    {
        /*The C library function int fprintf(FILE *stream, const char *format, ...) sends formatted output to a stream.*/
        //byte[] bytes = Encoding.ASCII.GetBytes(someString);
        //string someString = Encoding.ASCII.GetString(bytes);

        //Debug.Log("CLibFile.fprintf");

        string formattedStr = CFUNCTIONS.sprintfSinglestring(toFormat, formatParams);
        //Debug.Log("writing to file: " + formattedStr);
        byte[] data = Encoding.ASCII.GetBytes(formattedStr);
        if (file.mode == "a")
            file.datafile.Position = file.datafile.Length;
        file.datafile.Write(data, 0, data.Length);
    }

    internal static string fgets(ref string buffer, int size, CLibFile.CFILE file) //(string_buffer, phantdefs.SZ_FROM, theFile)
    {
        /*The C library function char *fgets(char *str, int n, FILE *stream) reads a line from the specified stream and stores it into the string pointed to by str. 
         * It stops when either (n-1) characters are read, the newline character is read, or the end-of-file is reached, whichever comes first.*/

        //Debug.Log("CLibFile.fgets");
        //Debug.Log("starting string: " + buffer);
        byte[] readData;
        int numBytesToRead = size;// - 1; //the - 1 causes a line skip when reading a size-1 length value in shop items
        int numBytesRead = 0;
        bool newLineread = false;
        while (numBytesToRead > 0 //todo: unitygamecontroller interrupt?
            && file.datafile.Position < file.datafile.Length
            && !newLineread)
        {
            readData = new byte[1];
            int numRead = file.datafile.Read(readData, 0, 1); //read one byte at a time
            //file.datafile.Position += numRead;
            char[] chars = Encoding.ASCII.GetChars(readData);
            char checkByte = chars[0];
            if (checkByte == '\n') //tested: this works fine
            {
                newLineread = true;
            }
            else
            {
                buffer += chars[0].ToString();
            }
            numBytesRead ++;
            numBytesToRead--;
        }
        /*On success, the function returns the same str parameter. If the End-of-File is encountered and no characters have been read, the contents of str remain unchanged and a null pointer is returned.*/
        if (numBytesRead > 0)
        {
            //Debug.Log("fgets returning line: " + buffer);
            return buffer;
        }
        else
            return null;
    }

    //intention is to update vars with extracted values, but this can't be done with param args (can't pass as ref). don't want multiple implementations, so returning extracted values for manual assignment at source
    internal static string[] fscanf(CLibFile.CFILE file, string format, params object[] vars)
    {
        /*The C library function int fscanf(FILE *stream, const char *format, ...) reads formatted input from a stream.
         format: This is the C string that contains one or more of the following items − Whitespace character, Non-whitespace character and Format specifiers. A format specifier will be as [=%[*][width][modifiers]type=]
         1	* This is an optional starting asterisk indicates that the data is to be read from the stream but ignored, i.e. it is not stored in the corresponding argument.
         2	width This specifies the maximum number of characters to be read in the current reading operation.
         3	modifiers Specifies a size different from int (in the case of d, i and n), unsigned int (in the case of o, u and x) or float (in the case of e, f and g) for the data pointed by the corresponding additional argument: 
            h : short int (for d, i and n), or unsigned short int (for o, u and x) l : long int (for d, i and n), or unsigned long int (for o, u and x), or double (for e, f and g) L : long double (for e, f and g)
         4	type A character specifying the type of data to be read and how it is expected to be read.
         additional arguments − Depending on the format string, the function may expect a sequence of additional arguments, each containing one value to be inserted instead of each %-tag specified
         */

        //    CLibFile.fscanf(wizard_file, "%ld %s %s %s %d\n", wizTypeInt, wizNetwork, wizAccount, wizCharacter, exceptionFlag) == 5)
        //if (CLibFile.fscanf(monster_file, "%lf %lf %lf %lf %lf %hd %hd %hd\n",
        //if (CLibFile.fscanf(charstats_file, "%c %lf %lf %lf %lf %d %lf %lf %lf %lf %lf %lf %lf %lf %lf %lf %lf %lf %lf %lf %lf %lf %lf %lf\n",
        //if (CLibFile.fscanf(shopitems_file, "%lf\n", the_realm.shop_item[i].cost)
        //all these files are space delimited

        //Debug.Log("CLibFile.fscanf: num to match = " + vars.Length);

        int itemsMatched = 0;
        int numToMatch = vars.Length;
        byte[] readData;
        bool newLineread = false;
        string currentValue = "";
        string[] extractedValues = new string[numToMatch];
        char prevByte = ' ';

        //get next byte
        //if it's a space and prev char was not, increment itemsmatched and assign associated var       
        while (numToMatch > 0
            && file.datafile.Position < file.datafile.Length
            && !newLineread)
        {
            //debug
            //if (UnityGameController.StopApplication) //time to quit
            //{
            //    Debug.Log("Thread " + System.Threading.Thread.CurrentThread.Name + " stopping in fscanf");
            //    break;
            //}
            //else
            //{
            //    if (file.Name.Contains("wizard"))
            //        System.Threading.Thread.Sleep(33); //30fps
            //}
            //if (file.Name.Contains("wizard"))
            //    Debug.Log("reading in fscanf on file " + file.Name + " with file.datafile.Position and file.datafile.Length: " + file.datafile.Position + " " + file.datafile.Length);

            readData = new byte[1];
            int numRead = file.datafile.Read(readData, 0, 1); //read one byte at a time
            char[] chars = Encoding.ASCII.GetChars(readData);
            char checkByte = chars[0];
            //Debug.Log("read byte " + checkByte.ToString());
            if (checkByte == '\n') //tested: this works fine
            {
                newLineread = true;
            }
            if ((checkByte == ' ' || checkByte == '\n') && prevByte != ' ') //end of value
            {
                //Debug.Log("fscanf: value detected: " + currentValue);
                extractedValues[itemsMatched] = currentValue;
                currentValue = "";
                itemsMatched++;
                numToMatch--;
            }
            else if (checkByte != ' ') //reading a value
            {
                currentValue += chars[0].ToString();
            }
            //else: moving through spaces between values

            prevByte = checkByte;
        }

        //set results to vars 
        if (itemsMatched == vars.Length)
        {
            for (int i = 0; i < vars.Length; i++)
            {
                vars[i] = extractedValues[i];
            }
        }
        //this fails to update value types at source, so now return the values directly

        //update extractedValues so that its length reflects num found - to allow detection of incorrect count
        string[] extractedValuesFinal = new string[itemsMatched];
        for (int i = 0; i < Math.Min(extractedValuesFinal.Length, extractedValues.Length); i++)
        {
            extractedValuesFinal[i] = extractedValues[i];
        }

        /*This function returns the number of input items successfully matched and assigned, which can be fewer than provided for, or even zero in the event of an early matching failure.*/
        return extractedValuesFinal;
    }
    
    internal static int fwrite(object destinationobj, ref int size, int num, CLibFile.CFILE file) //CLibFile.fwrite(entry, phantdefs.SZ_SCOREBOARD, 1, temp_file)
    {
        /*The C library function size_t fwrite(const void *ptr, size_t size, size_t nmemb, FILE *stream) writes data from the array pointed to, by ptr to the given stream.
size_t fwrite(const void *ptr, size_t size, size_t nmemb, FILE *stream)
ptr − This is the pointer to the array of elements to be written.
size − This is the size in bytes of each element to be written.
nmemb − This is the number of elements, each one with a size of size bytes.
stream − This is the pointer to a FILE object that specifies an output stream.*/
        //Debug.Log("CLibFile.fwrite");

        //in phantasia nmemb is always 1

        //todo: some files contain multiple objects. need to write from datafile.position onwards
        try
        {

            byte[] objectData = ObjectToByteArray(destinationobj);
            Debug.Log("serializing object " + destinationobj + " to array, size " + objectData.Length + ", provided size: " + size);
            if (destinationobj.GetType() == typeof(realm_object_t))
            {
                realm_object_t realmobj = (realm_object_t)destinationobj;
                Debug.Log("realm object detected, with type " + realmobj.type + " and loc " + realmobj.x + "," + realmobj.y);
            }
            if (objectData.Length > size)
            {
                Debug.LogError("Size update needed for " + destinationobj + " - from " + size + " to " + objectData.Length);
                size = objectData.Length; //update size - C sizes may not be accurate
            }
            file.datafile.Write(objectData, 0, objectData.Length); //0 is the offset into the array, not the file
            if (size > objectData.Length) //pad extra bytes
            {
                byte[] dummyData = new byte[size - objectData.Length];
                file.datafile.Write(dummyData, 0, dummyData.Length);
            }

            //long sizebefore = file.datafile.Length;
            //ObjectToFile(destinationobj, file.datafile);
            //long sizeafter = file.datafile.Length;
            //size = (int)sizeafter;
            //Debug.Log("wrote " + destinationobj + " to file with size change " + (sizeafter - sizebefore));
        }
        catch (Exception e)
        {
            Debug.LogError("WRITE ERROR: " + e.InnerException + "||" + e.Message + "||" + e.StackTrace);
            Debug.LogError("file position " + file.datafile.Position + ", length " + file.datafile.Length + ", size requested " + size);
            return 0;
        }
        return 1; 
    }

    internal static int fread<T>(ref T destinationobj, int size, int num, CLibFile.CFILE file)
    {
        Debug.Log("CLibFile.fread generic: size: " + size + " || Length: " + file.datafile.Length + " || Position: " + file.datafile.Position);

        //if (file.datafile.Position != 0) //not currently structured to read multiple objects from a file
        //    return 0;

        try
        {
            //Debug.Log("fread debug: size: " + size + " || Length: " + file.datafile.Length + " || Position: " + file.datafile.Position);
            //if size is not set, assume this is a whole-file object
            if (size == 0) //todo: config file?
            {
                size = (int)file.datafile.Length;
            }
            if (size != 0
                && size <= file.datafile.Length - file.datafile.Position)
            {
                //for (int i = 0; i < size - 1; i++)
                //{
                //    Debug.Log("reading byte " + i);
                //    byte[] objectDataByte = new byte[1];
                //    file.datafile.Read(objectDataByte, i, 1);
                //}

                if (destinationobj.GetType() == typeof(realm_state_t))
                {
                    realm_state_t realmobj = (realm_state_t)((object)destinationobj);
                    Debug.Log("before read: realm state detected, with kings_gold " + realmobj.kings_gold + " and king_name " + realmobj.king_name + ", valar_name " + realmobj.valar_name);
                }
                if (destinationobj.GetType() == typeof(realm_object_t))
                {
                    realm_object_t realmobj = (realm_object_t)((object)destinationobj);
                    Debug.Log("before read: realm object detected, with type " + realmobj.type + " and loc " + realmobj.x + "," + realmobj.y);
                }

                byte[] objectData = new byte[size];
                file.datafile.Read(objectData, 0, size); // 0 is the offset into the array, not the file
                object result = ByteArrayToObject(objectData);
                destinationobj = (T)result;

                if (destinationobj.GetType() == typeof(realm_state_t))
                {
                    realm_state_t realmobj = (realm_state_t)((object)destinationobj);
                    Debug.Log("after read: realm state detected, with kings_gold " + realmobj.kings_gold + " and king_name " + realmobj.king_name + ", valar_name " + realmobj.valar_name);
                }
                if (destinationobj.GetType() == typeof(realm_object_t))
                {
                    realm_object_t realmobj = (realm_object_t)((object)destinationobj);
                    Debug.Log("after read: realm object detected, with type " + realmobj.type + " and loc " + realmobj.x + "," + realmobj.y);
                }

                //FileToObject(destinationobj, file.datafile);
                return 1;
            }
            return 0;
        }
        catch (Exception e)
        {
            Debug.LogError("READ ERROR for " + destinationobj + ": " + e.Message + "||" + e.InnerException + "||" + e.StackTrace);
            Debug.LogError("file position " + file.datafile.Position + ", file length " + file.datafile.Length + ", size requested " + size);

            return 0;
        }
    }

    //commented: object subclasses passed as destinationobj don't update at source; using generic method above instead
    //    internal static int fread(object destinationobj, int size, int num, CLibFile.CFILE file) //CLibFile.fread(ref sb, phantdefs.SZ_SCOREBOARD, 1, scoreboard_file)
    //    {
    //        /*The C library function size_t fread(void *ptr, size_t size, size_t nmemb, FILE *stream) reads data from the given stream into the array pointed to, by ptr.
    //size_t fread(void *ptr, size_t size, size_t nmemb, FILE *stream)
    //ptr − This is the pointer to a block of memory with a minimum size of size*nmemb bytes.
    //size − This is the size in bytes of each element to be read.
    //nmemb − This is the number of elements, each one with a size of size bytes.
    //stream − This is the pointer to a FILE object that specifies an input stream.*/

    //        //num is always 1 for phantasia

    //        Debug.Log("CLibFile.fread");

    //        //if (file.datafile.Position != 0) //not currently structured to read multiple objects from a file
    //        //    return 0;

    //        try
    //        {
    //            //if size is not set, assume this is a whole-file object
    //            if (size == 0) //todo: config file?
    //            {
    //                size = (int)file.datafile.Length;
    //            }
    //            if (size != 0 
    //                && size <= file.datafile.Length - file.datafile.Position)
    //            {
    //                //for (int i = 0; i < size - 1; i++)
    //                //{
    //                //    Debug.Log("reading byte " + i);
    //                //    byte[] objectDataByte = new byte[1];
    //                //    file.datafile.Read(objectDataByte, i, 1);
    //                //}

    //                if (destinationobj.GetType() == typeof(realm_state_t))
    //                {
    //                    realm_state_t realmobj = (realm_state_t)destinationobj;
    //                    Debug.Log("before read: realm state detected, with kings_gold " + realmobj.kings_gold + " and king_name " + realmobj.king_name + ", valar_name " + realmobj.valar_name);
    //                }
    //                if (destinationobj.GetType() == typeof(realm_object_t))
    //                {
    //                    realm_object_t realmobj = (realm_object_t)destinationobj;
    //                    Debug.Log("before read: realm object detected, with type " + realmobj.type + " and loc " + realmobj.x + "," + realmobj.y);
    //                }

    //                byte[] objectData = new byte[size];
    //                file.datafile.Read(objectData, 0, size); // 0 is the offset into the array, not the file
    //                destinationobj = ByteArrayToObject(objectData);

    //                if (destinationobj.GetType() == typeof(realm_state_t))
    //                {
    //                    realm_state_t realmobj = (realm_state_t)destinationobj;
    //                    Debug.Log("after read: realm state detected, with kings_gold " + realmobj.kings_gold + " and king_name " + realmobj.king_name + ", valar_name " + realmobj.valar_name);
    //                }
    //                if (destinationobj.GetType() == typeof(realm_object_t))
    //                {
    //                    realm_object_t realmobj = (realm_object_t)destinationobj;
    //                    Debug.Log("after read: realm object detected, with type " + realmobj.type + " and loc " + realmobj.x + "," + realmobj.y);
    //                }

    //                //FileToObject(destinationobj, file.datafile);
    //                return 1;
    //            }
    //            return 0;
    //        }
    //        catch (Exception e)
    //        {
    //            Debug.LogError("READ ERROR for " + destinationobj + ": " + e.Message + "||" + e.InnerException + "||" + e.StackTrace);
    //            Debug.LogError("file position " + file.datafile.Position + ", file length " + file.datafile.Length + ", size requested " + size);

    //            return 0;
    //        }
    //    }
    /**/

    //stackoverflow.com/questions/1446547/how-to-convert-an-object-to-a-byte-array-in-c-sharp
    // Convert an object to a byte array
    public static byte[] ObjectToByteArray(object obj)
    {
        BinaryFormatter bf = new BinaryFormatter();
        using (var ms = new MemoryStream())
        {
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }
    }
    public static void ObjectToFile(object obj, FileStream file)
    {
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, obj);
    }
    // Convert a byte array to an Object
    public static object ByteArrayToObject(byte[] arrBytes)
    {
        using (var memStream = new MemoryStream())
        {
            var binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            var obj = binForm.Deserialize(memStream);
            return obj;
        }
    }
    public static void FileToObject(object obj, FileStream file)
    {
        BinaryFormatter bf = new BinaryFormatter();
        obj = bf.Deserialize(file);
    }
}
