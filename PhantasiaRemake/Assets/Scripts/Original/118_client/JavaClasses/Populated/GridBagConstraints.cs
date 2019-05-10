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

internal class GridBagConstraints
{
    public const int BOTH = 0; //Resize the component both horizontally and vertically.
    public const int CENTER = 1; //Put the component in the center of its display area.
    public const int HORIZONTAL = 2; //Resize the component horizontally but not vertically.
    public const int NONE = 3; //Do not resize the component.

    internal Boundary insets; //This field specifies the external padding of the component, the minimum amount of space between the component and the edges of its display area.
    public int gridx { get; internal set; }
    public int gridy { get; internal set; }
    public int gridwidth { get; internal set; }
    public int gridheight { get; internal set; }
    public int weightx { get; internal set; } //Specifies how to distribute extra horizontal space.
    public int weighty { get; internal set; } //Specifies how to distribute extra vertical space.
    public int fill { get; internal set; } //This field is used when the component's display area is larger than the component's requested size.
    public int anchor { get; internal set; } //This field is used when the component is smaller than its display area.

    public GridBagConstraints()
    {
        insets = new Boundary();
        gridwidth = 1;
        gridheight = 1;
        weightx = 0;
        weighty = 0;
        fill = BOTH;
        anchor = CENTER;
    }

    public GridBagConstraints(GridBagConstraints copy)
    {
        insets = new Boundary(copy.insets);
        gridx = copy.gridx;
        gridy = copy.gridy;
        gridwidth = copy.gridwidth;
        gridheight = copy.gridheight;
        weightx = copy.weightx;
        weighty = copy.weighty;
        fill = copy.fill;
        anchor = copy.anchor;
    }
}
