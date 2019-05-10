//import java.awt.*;
//import java.net.*;
//import java.io.*;
//import java.applet.*;
//import java.awt.event.*;
//import java.lang.Thread;
//import java.awt.event.KeyEvent;

/*
 An InputStreamReader is a bridge from byte streams to character streams: It reads bytes and decodes them into characters using a specified charset. 
 The charset that it uses may be specified by name or may be given explicitly, or the platform's default charset may be accepted.
Each invocation of one of an InputStreamReader's read() methods may cause one or more bytes to be read from the underlying byte-input stream. 
To enable the efficient conversion of bytes to characters, more bytes may be read ahead from the underlying stream than are necessary to satisfy the current read operation.
For top efficiency, consider wrapping an InputStreamReader within a BufferedReader. 
 */
internal class InputStreamReader
{
    private InputStream p;
    
    public InputStreamReader(InputStream p) // InputStreamReader(InputStream in)  Creates an InputStreamReader that uses the default charset.
    {
        this.p = p;
    }
}