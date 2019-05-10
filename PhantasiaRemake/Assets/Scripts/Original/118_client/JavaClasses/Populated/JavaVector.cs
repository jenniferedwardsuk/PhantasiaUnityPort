//import java.awt.JavaPanel;
//import java.awt.List;
//import java.awt.BorderLayout;
//import java.io.DataInputStream;
//import java.util.Vector;
//import java.awt.event.*;
using System;
using System.Collections.Generic;
using UnityEngine;

public class JavaVector
{
    private List<string> vectorList = new List<string> { };

    public int Count { get; internal set; }


    internal void addElement(string name)
    {
        vectorList.Add(name);
        Count += 1;
    }

    internal int indexOf(string v)
    {
        return vectorList.IndexOf(v);
    }

    internal void removeElementAt(int index)
    {
        vectorList.RemoveAt(index);
    }

    internal string elementAt(int p)
    {
        if (p < vectorList.Count)
            return vectorList[p];
        else
        {
            Debug.LogError("Error: JavaVector elementAt() referenced nonexistent index at " + p);
            return null;
        }
    }
}