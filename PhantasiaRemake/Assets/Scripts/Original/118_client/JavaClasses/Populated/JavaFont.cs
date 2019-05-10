//import java.awt.Color;
//import java.awt.JavaFont;
using UnityEngine;

public class JavaFont
{
    private Font jFont;

    private string fontName;
    public UnityEngine.FontStyle fontStyle;
    public int fontSize;

    public static UnityEngine.FontStyle PLAIN = UnityEngine.FontStyle.Normal;
    public static UnityEngine.FontStyle BOLD = UnityEngine.FontStyle.Bold;
    public Font UnityFont { get; internal set; }

    public JavaFont() //default values
    {
        this.fontName = "default";
        this.fontStyle = PLAIN;
        this.fontSize = 12;
        jFont = UnityJavaInterface.GetFontHelvetica();
        UnityFont = jFont;
    }

    public JavaFont(string vfontName, UnityEngine.FontStyle vfontStyle, int vfontSize)
    {
        this.fontName = vfontName;
        this.fontStyle = vfontStyle;
        this.fontSize = vfontSize;

        if (vfontName.Equals("Helvetica"))
        {
            jFont = UnityJavaInterface.GetFontHelvetica(); 
            UnityFont = jFont;
        }
        else if (vfontName.Equals("TimesRoman"))
        {
            jFont = UnityJavaInterface.GetFontTimesRoman();
            UnityFont = jFont;
        }
        else
        {
            Debug.LogError("font not found: " + vfontName);
        }
        // style and size are linked to Unity Text element in setFont methods
    }
}