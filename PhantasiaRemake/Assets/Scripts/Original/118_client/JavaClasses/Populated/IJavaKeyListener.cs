//import java.awt.JavaPanel;
//import java.awt.JavaButton;
//import java.awt.TextField;
//import java.awt.TextArea;
//import java.awt.List;
//import java.awt.BorderLayout;
//import java.io.DataInputStream;
//import java.awt.Color;
//import java.awt.event.*;
internal interface IJavaKeyListener
{
    /*
     * public interface KeyListener
extends EventListener
The listener interface for receiving keyboard events (keystrokes). 
The class that is interested in processing a keyboard event either implements this interface 
(and all the methods it contains) or extends the abstract KeyAdapter class (overriding only the methods of interest).
The listener object created from that class is then registered with a component using the component's addKeyListener method. 
A keyboard event is generated when a key is pressed, released, or typed. 
The relevant method in the listener object is then invoked, and the KeyEvent is passed to it.

Modifier and Type	Method and Description
void	keyPressed(KeyEvent e)
Invoked when a key has been pressed.
void	keyReleased(KeyEvent e)
Invoked when a key has been released.
void	keyTyped(KeyEvent e)
Invoked when a key has been typed.
     */

    void keyPressed(KeyEvent e);
    void keyReleased(KeyEvent e);
    void keyTyped(KeyEvent e);
}