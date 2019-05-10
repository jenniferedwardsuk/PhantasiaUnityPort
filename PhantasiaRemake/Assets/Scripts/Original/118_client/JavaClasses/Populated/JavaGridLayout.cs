//import java.awt.JavaGridLayout;
//import java.awt.BorderLayout;
//import java.io.*;
//import java.awt.JavaPanel;
//import java.awt.JavaButton;
//import java.awt.Font;
//import java.awt.event.*;
//import java.lang.Integer;
internal class JavaGridLayout : JavaLayout
{
    internal int rows;
    internal int cols;
    internal int hgap;
    internal int vgap;

    public JavaGridLayout()
    {
        this.rows = 1;
        this.cols = 1;
        this.hgap = 0;
        this.vgap = 0;
    }

    public JavaGridLayout(int rows, int cols)
    {
        this.rows = rows;
        this.cols = cols;
        this.hgap = 0;
        this.vgap = 0;
    }

    public JavaGridLayout(int rows, int cols, int hgap, int vgap) //GridLayout(int rows, int cols, int hgap, int vgap)
    {
        this.rows = rows;
        this.cols = cols;
        this.hgap = hgap;
        this.vgap = vgap;
    }
}