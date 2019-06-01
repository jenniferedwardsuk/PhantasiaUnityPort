//import java.awt.*;
using System;
using UnityEngine;
using UnityEngine.UI;

internal class JavaGraphics
{
    JavaCanvas source;
    RectTransform sourceTransform;
    Image sourceImage;
    Text sourceText;
    JavaColor currentColor;

    internal JavaGraphics(JavaCanvas sourceCanvas)
    {
        source = sourceCanvas;
        sourceTransform = source.unityComponentGroup.rectComponent;

        if (source.unityComponentGroup.GetType() == typeof(UnityPanelComponents))
            sourceImage = ((UnityPanelComponents)source.unityComponentGroup).imageComponent;
        if (source.unityComponentGroup.GetType() == typeof(UnityTextComponents))
            sourceText = ((UnityTextComponents)source.unityComponentGroup).textComponent;
        if (source.unityComponentGroup.GetType() == typeof(UnityButtonComponents))
            sourceText = ((UnityButtonComponents)source.unityComponentGroup).textComponent;
    }

    internal void setColor(JavaColor contextColor)
    {
        currentColor = contextColor;
    }

    //clearRect(int x, int y, int width, int height)    //Clears the specified rectangle by filling it with the background color of the current drawing surface.
    internal void clearRect(int x, int y, int canvasWidth, int canvasHeight)
    {
        if (sourceImage) //assumes x/y/width/height are equivalent to component's bounds (always the case for phantasia)
        {
            sourceImage.color = JavaCanvas.foregroundColor.GetUnityColor();// backgroundColor.GetUnityColor();// constants.backgroundColor;
        }
    }

    //fillRect(int x, int y, int width, int height)    //Fills the specified rectangle.
    internal void fillRect(int v1, int v2, int canvasWidth, int canvasHeight, float percentFill) //The rectangle is filled using the graphics context's current color. //percentFill added for unity
    {
        if (sourceImage)
        {
            sourceImage.color = currentColor.GetUnityColor();

            if (!sourceImage.sprite)
            {
                sourceImage.sprite = Resources.Load("EmptySprite", typeof(Sprite)) as Sprite;
                sourceImage.type = Image.Type.Filled;
                sourceImage.fillMethod = Image.FillMethod.Horizontal;

                GameObject borderChild = new GameObject("BorderChild");
                RectTransform childRect = borderChild.gameObject.AddComponent<RectTransform>();
                childRect.parent = sourceImage.transform;
                childRect.position = sourceImage.transform.position;
                childRect.rotation = sourceImage.transform.rotation;
                childRect.localScale = sourceImage.transform.localScale;
                childRect.anchorMin = new Vector2(0, 0);
                childRect.anchorMax = new Vector2(1, 1);
                childRect.offsetMax = new Vector2(0, 0);
                childRect.offsetMin = new Vector2(0, 0);
                Image childImg = borderChild.gameObject.AddComponent<Image>();
                childImg.sprite = Resources.Load("BorderSprite", typeof(Sprite)) as Sprite;
                childImg.type = Image.Type.Sliced;

                Outline border = sourceImage.gameObject.AddComponent<Outline>();
                border.effectDistance = new Vector2(1,1);
                border.useGraphicAlpha = false;
            }
            sourceImage.fillAmount = percentFill;

            //if (percentFill >= 0) //scale-based version - unused
            //{
            //    Vector3 imageScale = sourceImage.GetComponent<RectTransform>().localScale;
            //    imageScale.x = percentFill;
            //    sourceImage.GetComponent<RectTransform>().localScale = imageScale;
            //    for (int i = 0; i < sourceImage.gameObject.transform.childCount; i++)
            //    {
            //        Vector3 childScale = sourceImage.gameObject.transform.GetChild(i).localScale;
            //        if (percentFill > 0)
            //        {
            //            childScale.x = 1 / percentFill;
            //        }
            //        else
            //        {
            //            childScale.x = 1;
            //        }
            //        sourceImage.gameObject.transform.GetChild(i).localScale = childScale;
            //    }
            //}

        }
        else
        {
            Debug.LogError("fillRect called without an Image reference");
        }
    }

    //drawLine(int x1, int y1, int x2, int y2)    //Draws a line, using the current color, between the points(x1, y1) and(x2, y2) in this graphics context's coordinate system.
    internal void drawLine(int x1, int y1, int x2, int y2)
    {
        UnityJavaInterface.DrawLine(sourceTransform, x1, y1, x2, y2, currentColor);
    }

    //drawRect(int x, int y, int width, int height)    //Draws the outline of the specified rectangle.
    internal void drawRect(int x, int y, int width, int height)
    {
        UnityJavaInterface.DrawRect(sourceTransform, x, y, width, height, currentColor);
    }

    /*
        img - the specified image to be drawn. This method does nothing if img is null.
        x - the x coordinate.
        y - the y coordinate.
        observer - object to be notified as more of the image is converted.
     */
    internal void drawImage(Sprite image, int v1, int v2, JavaComponent component)
    {
        if (sourceImage)
        {
            sourceImage.sprite = image;
            sourceImage.preserveAspect = true;
        }
        else
        {
            Debug.LogError("drawImage called without an Image reference");
        }
    }

    internal void setFont(JavaFont theFont)
    {
        if (sourceText && theFont != null && theFont.UnityFont)
        {
            sourceText.fontSize = theFont.fontSize;
            sourceText.fontStyle = theFont.fontStyle;
            sourceText.font = theFont.UnityFont;
        }
        else
        {
            if (theFont != null && !theFont.UnityFont)
            {
                Debug.Log("Warning: unityfont does not exist for graphics setfont.");
            }
        }
    }

    //Draws the text given by the specified string, using this graphics context's current font and color. 
    //The baseline of the leftmost character is at position (x, y) in this graphics context's coordinate system.
    internal void drawString(string theValue, int x, int y, UnityTextComponents alternativeText) 
    {
        if (sourceText)
        {
            sourceText.text = theValue;
            //sourceText.transform.position += new Vector3(x, y, 0); //todo: doesn't work as intended
        }
        else if (alternativeText != null)
        {
            alternativeText.textComponent.text = theValue;
            //alternativeText.textComponent.transform.position += new Vector3(x, y, 0); //todo: doesn't work as intended
        }
        else
            Debug.LogError("drawString called without a Text reference");
    }
}