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

/*
 A flow layout arranges components in a directional flow, much like lines of text in a paragraph. 
 The flow direction is determined by the container's componentOrientation property and may be one of two values:
ComponentOrientation.LEFT_TO_RIGHT
ComponentOrientation.RIGHT_TO_LEFT
Flow layouts are typically used to arrange buttons in a panel. It arranges buttons horizontally until no more buttons fit on the same line. 
The line alignment is determined by the align property. The possible values are:
LEFT
RIGHT
CENTER
LEADING
TRAILING
 */
internal class FlowLayout : JavaLayout
{
    private int positioning;
    private int hgap;
    private int vgap;

    public FlowLayout(int positioning, int hgap, int vgap) // Creates a new flow layout manager with the indicated alignment and the indicated horizontal and vertical gaps.
    {
        this.positioning = positioning;
        this.Hgap = hgap;
        this.Vgap = vgap;
    }

    public static int CENTER = 0;

    public int Hgap
    {
        get
        {
            return hgap;
        }

        set
        {
            hgap = value;
        }
    }

    public int Vgap
    {
        get
        {
            return vgap;
        }

        set
        {
            vgap = value;
        }
    }
}