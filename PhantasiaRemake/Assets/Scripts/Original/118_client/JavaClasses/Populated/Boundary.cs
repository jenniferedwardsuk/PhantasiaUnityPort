//import java.awt.JavaPanel;
//import java.awt.JavaLabel;
//import java.awt.Canvas;
//import java.awt.Color;
//import java.awt.BorderLayout;
//import java.awt.FlowLayout;
//import java.awt.JavaGridLayout;
//import java.awt.GridBagLayout;
//import java.awt.GridBagConstraints;
//import java.awt.MediaTracker;
//import java.io.DataInputStream;
//import java.awt.Font;

internal class Boundary
{
    public int top { get; internal set; }
    public int bottom { get; internal set; }
    public int left { get; internal set; }
    public int right { get; internal set; }

    public Boundary()
    {
        top = 0;
        bottom = 0;
        left = 0;
        right = 0;
    }

    public Boundary(Boundary copy)
    {
        top = copy.top;
        bottom = copy.bottom;
        left = copy.left;
        right = copy.right;
    }
}
