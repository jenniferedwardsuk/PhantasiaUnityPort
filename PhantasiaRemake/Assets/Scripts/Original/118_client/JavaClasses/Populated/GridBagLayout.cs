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
using System;

internal class GridBagLayout : JavaLayout
{
    public GridBagLayout()
    {

    }

    internal static void setConstraints(JavaComponent item, GridBagConstraints constraints, string panelType = null)
    {
        item.layoutType = JavaComponent.LayoutType.GridBag;
        item.gridBagConstraints = new GridBagConstraints(constraints);
        item.layoutName = panelType;
    }
}