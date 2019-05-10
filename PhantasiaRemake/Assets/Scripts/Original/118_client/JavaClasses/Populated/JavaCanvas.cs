using System;
using UnityEngine;
using UnityEngine.UI;

//import java.awt.*;
public abstract class JavaCanvas : JavaComponent
{
    public static JavaColor highlightColor = new JavaColor(new Color(1, 1, 1));
    public static JavaColor foregroundColor = new JavaColor(new Color(0.784f, 0.796f, 0.588f));
    public static JavaColor backgroundColor = new JavaColor(new Color(1, 1, 0.8f));

    //A Canvas component represents a blank rectangular area of the screen onto which the application can draw or from which the application can trap input events from the user.

    public JavaCanvas(string name, bool parent)
    {
        highlightColor = new JavaColor(new Color(1,1,1));
        foregroundColor = new JavaColor(new Color(0.784f, 0.796f, 0.588f));
        backgroundColor = new JavaColor(new Color(1, 1, 0.8f));

        AddPanelToCanvas("-Canvas" + name, parent);
        //unityComponentGroup = UnityJavaInterface.AddPanel("-Canvas" + name, parent);
        //if (!unityComponentGroup.rectComponent) //deferred to main thread
        //{
        //    Debug.LogError("Unity object not created");
        //}
    }

    //internal void AddPanelToCanvas(string v1, bool v2)
    //{
    //    UnityJavaUIFuncQueue.GetInstance().QueueUIMethod(M_AddPanelToCanvas, v1, v2);
    //}
    internal void AddPanelToCanvas(string v1, bool v2)
    {
        UnityJavaInterface.AddPanel(v1, v2, this);
    }

    public void super()
    {
        //runs constructor of parent class - this is done automatically in c#
    }

    public void setBackground(JavaColor highlightColor)
    {
        UnityJavaUIFuncQueue.GetInstance().QueueUIMethod(M_setBackground, highlightColor);
    }
    public void M_setBackground(JavaColor highlightColor)
    {
        if (((UnityPanelComponents)unityComponentGroup).imageComponent)
        {
            if (highlightColor != null)
                ((UnityPanelComponents)unityComponentGroup).imageComponent.color = highlightColor.GetUnityColor();
            else
                ((UnityPanelComponents)unityComponentGroup).imageComponent.color = new Color(1, 1, 1, 1);
        }
        else
        {
            Debug.LogError("Unity Component doesn't exist");
        }
    }
    
    public Rectangle getBounds()
    {
        Rectangle rect = new Rectangle();
        if (unityComponentGroup.rectComponent)
        {
            rect.width = Mathf.FloorToInt(unityComponentGroup.rectComponent.sizeDelta.x);
            rect.height = Mathf.FloorToInt(unityComponentGroup.rectComponent.sizeDelta.y);
            rect.x = Mathf.FloorToInt(unityComponentGroup.rectComponent.position.x);
            rect.y = Mathf.FloorToInt(unityComponentGroup.rectComponent.position.y);
        }
        else
        {
            Debug.LogError("Unity Component doesn't exist");
        }
        return rect;
    }

    FontMetrics fontMetrics;
    public FontMetrics getFontMetrics(JavaFont theFont)
    {
        if (fontMetrics == null)
            fontMetrics = new FontMetrics(theFont);
        return fontMetrics;
    }
    JavaGraphics graphics;
    internal JavaGraphics getGraphics()
    {
        if (graphics == null)
        {
            graphics = new JavaGraphics(this);
        }
        return graphics;
    }

    //internal virtual void repaint()
    //{
    //    //If this component is a lightweight component, this method causes a call to this component's paint method as soon as possible. 
    //    //Otherwise, this method causes a call to this component's update method as soon as possible.
    //    if (componentGraphics == null)
    //        componentGraphics = new JavaGraphics(this);
    //    paint(componentGraphics);
    //}
    //internal abstract void paint() //commented: force graphics reference
    //{
    //
    //}
    //internal abstract void paint(JavaGraphics g);
    //{
    //subclasses to override
    //}

    internal void setSize(int v1, int v2)
    {
        UnityJavaUIFuncQueue.GetInstance().QueueUIMethod(M_setSize, v1, v2);
    }
    internal void M_setSize(int v1, int v2)
    {
        if (unityComponentGroup.rectComponent)
        {
            unityComponentGroup.rectComponent.sizeDelta = new Vector2(v1, v2);
        }
        else
        {
            Debug.LogError("Unity Component doesn't exist");
        }
    }
}