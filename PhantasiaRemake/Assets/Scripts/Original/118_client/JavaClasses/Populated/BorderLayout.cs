//import java.awt.JavaGridLayout;
//import java.awt.BorderLayout;
//import java.io.*;
//import java.awt.JavaPanel;
//import java.awt.JavaButton;
//import java.awt.Font;
//import java.awt.event.*;
//import java.lang.Integer;
internal class BorderLayout : JavaLayout
{
    /*
     * A border layout lays out a container, arranging and resizing its components to fit in five regions: north, south, east, west, and center. 
     * Each region may contain no more than one component, and is identified by a corresponding constant: NORTH, SOUTH, EAST, WEST, and CENTER.
     */

    //static string CENTER;
    //static string EAST;
    //static string NORTH;
    //static string SOUTH;
    //static string WEST;

    public int XGap;
    public int YGap;

    public BorderLayout()
    {
        XGap = 0;
        YGap = 0;
    }

    public BorderLayout(int v1, int v2) //Constructs a border layout with the specified gaps between components.
    {
        XGap = v1;
        YGap = v2;
    }
}