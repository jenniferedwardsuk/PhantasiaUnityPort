//import java.awt.*;
using System;

public class FontMetrics //The FontMetrics class defines a font metrics object, which encapsulates information about the rendering of a particular font on a particular screen.
{
    JavaFont theFont;

    public FontMetrics()
    {

    }

    public FontMetrics(JavaFont theFont)
    {
        this.theFont = theFont;
    }

    internal int stringWidth(string theValue) //Returns the total advance width for showing the specified String in this Font.
    {
        return 0; //unnecessary for Unity text components
    }
}