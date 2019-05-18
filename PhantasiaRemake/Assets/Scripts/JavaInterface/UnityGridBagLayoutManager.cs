using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityGridBagLayoutManager : MonoBehaviour
{

    /*
    public static int BOTH { get; internal set; } //Resize the component both horizontally and vertically.
    public static int CENTER { get; internal set; } //Put the component in the center of its display area.
    public static int HORIZONTAL { get; internal set; } //Resize the component horizontally but not vertically.
    public static int NONE { get; internal set; } //Do not resize the component.

    internal Boundary insets = new Boundary(); //This field specifies the external padding of the component, the minimum amount of space between the component and the edges of its display area.
                public int gridx { get; internal set; }
                public int gridy { get; internal set; }
                public int gridwidth { get; internal set; }
                public int gridheight { get; internal set; }
    public int weightx { get; internal set; } //Specifies how to distribute extra horizontal space.
    public int weighty { get; internal set; } //Specifies how to distribute extra vertical space.
                public int fill { get; internal set; } //This field is used when the component's display area is larger than the component's requested size.
                public int anchor { get; internal set; } //This field is used when the component is smaller than its display area.
     */

    RectTransform rect;

    public JavaComponent sourceComponent;
    public List<JavaComponent> components;

    int gridSizeX;
    int gridSizeY;
    float cellSizeX;
    float cellSizeY;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    // Use this for initialization
    void Start ()
    {

    }

    public bool refreshLayout = false;
    // Update is called once per frame
    void Update ()
    {
		//debug
        if (refreshLayout)
        {
            UpdateLayout();
            refreshLayout = false;
        }
	}

    public void UpdateLayout()
    {
        //Debug.Log("updating gridbag layout on " + sourceComponent.unityComponentGroup.rectComponent.gameObject.name + " with rect on object " + rect.gameObject.name + " sized " + rect.sizeDelta);
        //if (components != null)
            //Debug.Log("child component count for setlayout: " + sourceComponent.childComponents.Count);

        //foreach (JavaComponent comp in components)
        //{
        //    //Debug.Log(comp.unityComponentGroup.rectComponent.gameObject.name + " has constraint grid position " + comp.gridBagConstraints.gridx + "," + comp.gridBagConstraints.gridy);
        //}

        DeduceGridAndCellSize();
        PositionAndResizeComponents();

        //DistributeWeight(); //todo

        OverrideSpecialComponents();
    }

    private void OverrideSpecialComponents()
    {
        foreach (JavaComponent comp in components)
        {
            if (comp.layoutName != null)
            {
                RectTransform unityObj = comp.unityComponentGroup.rectComponent;
                Vector2 hardcodedSize = unityObj.sizeDelta;
                Vector3 hardcodedPosition = unityObj.position;
                bool changed = false;
                switch (comp.layoutName)
                {
                    case "status":
                        //approx original game's proportions
                        //hardcodedSize = new Vector2(960.05f, 272.6f);
                        //hardcodedPosition = new Vector3(480.02f, -136.32f, 0);
                        break;
                    case "messages":
                        //approx original game's proportions
                        //hardcodedSize = new Vector2(730.8f, 138.4f);
                        //hardcodedPosition = new Vector3(365.4f, -350.57f, 0);

                        //taller, wider
                        hardcodedSize = new Vector2(624f, 217.1f);
                        hardcodedPosition = new Vector3(314.15f, -260.55f, 0);

                        changed = true;
                        break;
                    case "buttons":
                        //approx original game's proportions
                        //hardcodedSize = new Vector2(730.8f, 47.7f);
                        //hardcodedPosition = new Vector3(365.4f, -443.62f, 0);

                        //shorter, wider
                        hardcodedSize = new Vector2(624f, 50f);
                        hardcodedPosition = new Vector3(314.15f, -394.1f, 0);

                        changed = true;
                        break;
                    case "chat":
                        //approx original game's proportions
                        //hardcodedSize = new Vector2(730.8f, 110.3f);
                        //hardcodedPosition = new Vector3(365.4f, -526.27f, 0);

                        //taller, wider
                        hardcodedSize = new Vector2(624f, 182.9f);
                        hardcodedPosition = new Vector3(314.15f, -510.55f, 0);

                        changed = true;
                        break;
                    case "right":
                        //approx original game's proportions
                        //hardcodedSize = new Vector2(229.2f, 327.36f);
                        //hardcodedPosition = new Vector3(845.4f, -436.32f, 0);

                        //narrower
                        hardcodedSize = new Vector2(336f, 450f);
                        hardcodedPosition = new Vector3(794.3f, -377f, 0);

                        changed = true;
                        break;
                }
                if (changed)
                {
                    Vector3[] panelCorners = new Vector3[4];
                    rect.GetWorldCorners(panelCorners);

                    unityObj.sizeDelta = hardcodedSize;
                    unityObj.position = hardcodedPosition + new Vector3(0, panelCorners[1].y, 0);
                    //+ new Vector3(unityObj.sizeDelta.x / 2, -1 * (unityObj.sizeDelta.y / 2), 0); //default to top left corner
                }
            }
        }
    }

    GridBagConstraints constraints;

    private void DeduceGridAndCellSize()
    {
        constraints = null;
        foreach (JavaComponent component in components)
        {
            constraints = component.gridBagConstraints;
            if (constraints != null)
            {
                gridSizeX = Mathf.Max(gridSizeX, constraints.gridx + 1); //add 1 to include the 0 index
                gridSizeY = Mathf.Max(gridSizeY, constraints.gridy + 1);
            }
        }

        cellSizeX = rect.sizeDelta.x / gridSizeX;
        cellSizeY = rect.sizeDelta.y / gridSizeY;

        //Debug.Log("cellsizeX " + cellSizeX + " cellsizeY " + cellSizeY);
        //Debug.Log("gridSizeX " + gridSizeX + " gridSizeY " + gridSizeY);
    }

    private void PositionAndResizeComponents()
    {
        //float mainCanvasHeight = UnityJavaInterface.mainCanvas.GetComponent<RectTransform>().sizeDelta.y;
        constraints = null;
        foreach (JavaComponent component in components)
        {
            constraints = component.gridBagConstraints;
            if (constraints != null)
            {
                RectTransform unityObj = component.unityComponentGroup.rectComponent;

                Vector3[] panelCorners = new Vector3[4];
                rect.GetWorldCorners(panelCorners);

                float cellSpaceX = cellSizeX * constraints.gridwidth;// - (constraints.insets.left + constraints.insets.right);
                float cellSpaceY = cellSizeY * constraints.gridheight;// - (constraints.insets.top + constraints.insets.bottom);

                //fill and anchor - todo
                if (unityObj.sizeDelta.x < cellSpaceX || unityObj.sizeDelta.y < cellSpaceY)
                {
                    //use fill
                    bool placeInCenter = true;
                    switch (constraints.fill)
                    {
                        case GridBagConstraints.BOTH:
                            //size:
                            unityObj.sizeDelta = new Vector2(cellSpaceX, cellSpaceY);
                            break;
                        case GridBagConstraints.CENTER:
                            //don't resize
                            break;
                        case GridBagConstraints.HORIZONTAL:
                            unityObj.sizeDelta = new Vector2(cellSpaceX, unityObj.sizeDelta.y);
                            break;
                        case GridBagConstraints.NONE:
                            placeInCenter = false;
                            unityObj.position = panelCorners[1]
                                + new Vector3(
                                    //constraints.insets.left + 
                                    unityObj.sizeDelta.x / 2, //constraints.insets.left is already inset/2 basically
                                    -1 * (//constraints.insets.top + 
                                    unityObj.sizeDelta.y / 2), //constraints.insets.top is already inset/2 basically
                                    0); //default to top left corner
                            break;
                    }

                    //still small? use anchor
                    if (unityObj.sizeDelta.x < cellSpaceX || unityObj.sizeDelta.y < cellSpaceY)
                    {
                        switch (constraints.anchor)
                        {
                            case GridBagConstraints.CENTER: //it's always center in phantasia
                                placeInCenter = true;
                                break;
                        }

                    }

                    //place in center
                    if (placeInCenter)
                    {
                        unityObj.position = panelCorners[1]
                                + new Vector3(
                                    //constraints.insets.left + 
                                    unityObj.sizeDelta.x / 2, //constraints.insets.left is already inset/2 basically
                                    -1 * (//constraints.insets.top + 
                                    unityObj.sizeDelta.y / 2), //constraints.insets.top is already inset/2 basically
                                    0);
                    }
                }
                else //component is larger than cell
                {
                    //set size (shrink)
                    unityObj.sizeDelta = new Vector2(cellSpaceX, cellSizeY);
                }

                //todo - TEMP OVERWRITE FILL AND ANCHOR
                //---------------------------------------------------
                //set size(fill both)
                unityObj.sizeDelta = new Vector2(cellSpaceX, cellSpaceY);
                //(shrink if necessary, but don't grow): Mathf.Min(cellSpaceX, unityObj.sizeDelta.x), Mathf.Min(cellSpaceY, unityObj.sizeDelta.y)); 

                //set relative position(center)
                unityObj.position = (panelCorners[1] //start from top left corner of parent rect
                    + new Vector3(
                        constraints.insets.left + cellSpaceX / 2, //constraints.insets.left is already inset/2 basically
                        -1 * (constraints.insets.top + cellSpaceY / 2), //constraints.insets.top is already inset/2 basically
                        0));
                //---------------------------------------------------

                //move to correct cell
                unityObj.position += new Vector3(cellSizeX * constraints.gridx, -1 * cellSizeY * constraints.gridy, 0);

            }
            else
            {
                Debug.LogError("no constraints on gridbag component " + component);
            }
        }
    }

    private void DistributeWeight()
    {
        float[] xTotalSpaceUsed = new float[gridSizeX];
        float[] yTotalSpaceUsed = new float[gridSizeY];
        for (int i = 0; i < gridSizeX; i++)
        {
            foreach (JavaComponent component in components)
            {
                constraints = component.gridBagConstraints;
                if (constraints != null && constraints.gridx == i)
                {
                    RectTransform unityObj = component.unityComponentGroup.rectComponent;
                    xTotalSpaceUsed[i] += unityObj.sizeDelta.x;
                }
            }
        }
        for (int j = 0; j < gridSizeY; j++)
        {
            foreach (JavaComponent component in components)
            {
                constraints = component.gridBagConstraints;
                if (constraints != null && constraints.gridy == j)
                {
                    RectTransform unityObj = component.unityComponentGroup.rectComponent;
                    yTotalSpaceUsed[j] += unityObj.sizeDelta.y;
                }
            }
        }

        float[] xMaxWeight = new float[gridSizeX];
        float[] yMaxWeight = new float[gridSizeY];
        for (int i = 0; i < gridSizeX; i++)
        {
            if (xTotalSpaceUsed[i] < rect.sizeDelta.x)
            {
                //there is extra horizontal space in the column - divide it between the components using weight
                xMaxWeight[i] = GetMaxWeightForRow(i); //todo
            }
            if (xMaxWeight[i] > 0)
            {
                float extraSpace = rect.sizeDelta.x - xTotalSpaceUsed[i];
                List<RectTransform> unityObjs = GetObjsInGrid(i, true);
                List<GridBagConstraints> constraints = GetConstraintsInGrid(i, true);
                ResizeByWeight(extraSpace, unityObjs, GetWeights(constraints, true), true);
            }
            else 
            {
                //all weights are 0 => don't reassign space
            }
        }
        for (int i = 0; i < gridSizeY; i++)
        {
            if (yTotalSpaceUsed[i] < rect.sizeDelta.y)
            {
                //there is extra horizontal space in the column - divide it between the components using weight
                yMaxWeight[i] = GetMaxWeightForRow(i); //todo
            }
            if (yMaxWeight[i] > 0)
            {
                float extraSpace = rect.sizeDelta.y - yTotalSpaceUsed[i];
                List<RectTransform> unityObjs = GetObjsInGrid(i, false);
                List<GridBagConstraints> constraints = GetConstraintsInGrid(i, false);
                ResizeByWeight(extraSpace, unityObjs, GetWeights(constraints, false), false);
            }
            else
            {
                //all weights are 0 => don't reassign space
            }
        }

    }

    private float[] GetWeights(List<GridBagConstraints> objs, bool isX)
    {
        float[] weights = new float[objs.Count];
        int count = 0;
        foreach (GridBagConstraints obj in objs)
        {
            if (isX)
                weights[count] = obj.weightx;
            else
                weights[count] = obj.weighty;
        }
        return weights;
    }

    private List<GridBagConstraints> GetConstraintsInGrid(int gridNum, bool isX)
    {
        List<GridBagConstraints> objs = new List<GridBagConstraints>();
        foreach (JavaComponent component in components)
        {
            constraints = component.gridBagConstraints;
            if (constraints != null
                && ((isX && constraints.gridx == gridNum) || (!isX && constraints.gridy == gridNum)))
            {
                objs.Add(constraints);
            }
        }
        return objs;
    }

    private List<RectTransform> GetObjsInGrid(int gridNum, bool isX)
    {
        List<RectTransform> objs = new List<RectTransform>();
        foreach (JavaComponent component in components)
        {
            constraints = component.gridBagConstraints;
            if (constraints != null 
                && ((isX && constraints.gridx == gridNum) || (!isX && constraints.gridy == gridNum)))
            {
                objs.Add(component.unityComponentGroup.rectComponent);
            }
        }
        return objs;
    }

    private void ResizeByWeight(float extraSpace, List<RectTransform> unityObjsInvolved, float[] weights, bool isX)
    {
        float totalWeight = 0;
        for (int i = 0; i < weights.Length; i++)
        {
            totalWeight += weights[i];
        }
        float[] proportions = new float[weights.Length];
        for (int i = 0; i < weights.Length; i++)
        {
            proportions[i] = weights[i] / totalWeight;
            unityObjsInvolved[i].sizeDelta = new Vector2(
                    isX ? unityObjsInvolved[i].sizeDelta.x * proportions[i] : unityObjsInvolved[i].sizeDelta.x,
                    isX ? unityObjsInvolved[i].sizeDelta.y : unityObjsInvolved[i].sizeDelta.y * proportions[i]);
            unityObjsInvolved[i].position += new Vector3(
                isX ? unityObjsInvolved[i].sizeDelta.x * proportions[i] / 2 : 0,
                isX ? 0 : -1 * unityObjsInvolved[i].sizeDelta.y * proportions[i] / 2,
                0);
        }
    }

    private float GetMaxWeightForRow(int rownum)
    {
        float maxWeight = 0;
        foreach (JavaComponent component in components)
        {
            constraints = component.gridBagConstraints;
            if (constraints != null)
            {
                RectTransform unityObj = component.unityComponentGroup.rectComponent;
                if (constraints.gridx == rownum)
                {
                    maxWeight = Mathf.Max(maxWeight, constraints.weightx);
                }
            }
        }
        return maxWeight;
    }

    private float GetMaxWeightForColumn(int colnum)
    {
        float maxWeight = 0;
        foreach (JavaComponent component in components)
        {
            constraints = component.gridBagConstraints;
            if (constraints != null)
            {
                RectTransform unityObj = component.unityComponentGroup.rectComponent;
                if (constraints.gridy == colnum)
                {
                    maxWeight = Mathf.Max(maxWeight, constraints.weighty);
                }
            }
        }
        return maxWeight;
    }
}
