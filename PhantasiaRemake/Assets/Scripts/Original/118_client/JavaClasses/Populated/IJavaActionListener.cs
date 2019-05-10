//import java.awt.JavaGridLayout;
//import java.awt.BorderLayout;
//import java.io.*;
//import java.awt.JavaPanel;
//import java.awt.JavaButton;
//import java.awt.Font;
//import java.awt.event.*;
//import java.lang.Integer;
internal interface IJavaActionListener
{
    /*
     * extends EventListener
The listener interface for receiving action events. 
The class that is interested in processing an action event implements this interface, and the object created with that class 
is registered with a component, using the component's addActionListener method. 
When the action event occurs, that object's actionPerformed method is invoked.

Methods 
Modifier and Type	Method and Description
void	actionPerformed(ActionEvent e)
Invoked when an action occurs.
     */
    void actionPerformed(ActionEvent e);
}