using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityBorderLayoutManager : MonoBehaviour {

    RectTransform rect;

    string compLocation;
    BorderLayout compLayout;

    public JavaComponent sourceComponent;
    public List<JavaComponent> components;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    // Use this for initialization
    void Start () {
		
	}

    public bool refreshLayout = false;
    bool isPopup = false;
    // Update is called once per frame
    void Update()
    {
        //debug
        if (refreshLayout)
        {
            UpdateLayout(isPopup);
            refreshLayout = false;
        }
    }

    public void UpdateLayout(bool isPopup)
    {
        this.isPopup = isPopup;

        //Debug.Log("updating border layout on " + sourceComponent.unityComponentGroup.rectComponent.gameObject.name + " with rect on object " + rect.gameObject.name + " sized " + rect.sizeDelta);
        //if (components != null)
        //Debug.Log("child component count for setlayout: " + sourceComponent.childComponents.Count);

        string allLocations = "";
        foreach (JavaComponent component in components)
        {
            allLocations += component.layoutLocation.ToLower();
        }
        bool hasCenter = allLocations.Contains("center");
        bool hasNorth = allLocations.Contains("north");
        bool hasSouth = allLocations.Contains("south");
        bool hasEast = allLocations.Contains("east");
        bool hasWest = allLocations.Contains("west");

        foreach (JavaComponent component in components)
        {
            RectTransform unityObj;
            if (isPopup)
            {
                Transform popupPanel = component.unityComponentGroup.rectComponent.Find("Panel");
                if (popupPanel)
                    rect = popupPanel.GetComponent<RectTransform>();
            }
            unityObj = component.unityComponentGroup.rectComponent;
            
            Vector3[] panelCorners = new Vector3[4];
            rect.GetWorldCorners(panelCorners);

            // xgap and ygap excluded

            //set size and position
            switch (component.layoutLocation.ToLower())
            {
                case "center": //75% of layout
                    unityObj.sizeDelta = new Vector2( //stretch if any component missing
                        rect.sizeDelta.x * 0.75f,
                        rect.sizeDelta.y * 0.75f
                        );
                    unityObj.position = panelCorners[1]; //top left
                    unityObj.position += new Vector3(rect.sizeDelta.x / 2, -1 * rect.sizeDelta.y / 2, 0); //move to center

                    //fill bilaterally empty space
                    if (!hasEast && !hasWest)
                    {
                        float newW = rect.sizeDelta.x;
                        unityObj.sizeDelta = new Vector3(newW, unityObj.sizeDelta.y);
                    }
                    if (!hasNorth && !hasSouth)
                    {
                        float newH = rect.sizeDelta.y;
                        unityObj.sizeDelta = new Vector3(unityObj.sizeDelta.x, newH);
                    }

                    break;

                case "north": //12.5% of layout
                    //if (rect.gameObject.name.Contains("PanelButton"))
                    //{
                    //    Debug.LogError("Layout debug: north button with hasSouth " + hasSouth + " and panel size " + rect.sizeDelta.x + "," + rect.sizeDelta.y);
                    //}

                    unityObj.sizeDelta = new Vector2(
                        rect.sizeDelta.x,
                        rect.sizeDelta.y * 0.125f //+ ((!hasCenter && !hasEast && !hasWest) ? (!hasSouth ? rect.sizeDelta.y * 0.75f : rect.sizeDelta.y * 0.75f / 2) : 0) //stretch if no middle components
                        );
                    unityObj.position = panelCorners[1]; //top left
                    unityObj.position += new Vector3(rect.sizeDelta.x / 2, -1 * (rect.sizeDelta.y * 0.125f / 2), 0); //move to top center

                    //fill any empty vertical space
                    if (!hasCenter && !hasEast && !hasWest)
                    {
                        //reposition and resize
                        float newY = unityObj.position.y;
                        float newH = unityObj.sizeDelta.y;
                        if (!hasSouth)
                        {
                            newH = rect.sizeDelta.y;
                            newY = -1 * rect.sizeDelta.y / 2; //move to center
                            //if (rect.gameObject.name.Contains("PanelButton"))
                            //{
                            //    Debug.LogError("Layout debug: 1: setting height of north child " + unityObj.name + " to " + newH);
                            //}
                        }
                        else
                        {
                            newH = rect.sizeDelta.y / 2;
                            newY = -1 * rect.sizeDelta.y / 4; //move to middle upper half
                            //if (rect.gameObject.name.Contains("PanelButton"))
                            //{
                            //    Debug.LogError("Layout debug: 2: setting height of north child " + unityObj.name + " to " + newH);
                            //}
                        }
                        unityObj.position = new Vector3(unityObj.position.x, newY + panelCorners[1].y, unityObj.position.z);
                        unityObj.sizeDelta = new Vector3(unityObj.sizeDelta.x, newH);
                    }
                    
                    break;

                case "south":
                    //if (rect.gameObject.name.Contains("PanelButton"))
                    //{
                    //    Debug.LogError("Layout debug: south button with hasNorth " + hasNorth + " and panel size " + rect.sizeDelta.x + "," + rect.sizeDelta.y);
                    //}

                    unityObj.sizeDelta = new Vector2(
                        rect.sizeDelta.x, 
                        rect.sizeDelta.y * 0.125f //+ ((!hasCenter && !hasEast && !hasWest) ? (!hasSouth ? rect.sizeDelta.y * 0.75f : rect.sizeDelta.y * 0.75f / 2) : 0) //stretch if no middle components
                        );
                    unityObj.position = panelCorners[1]; //top left
                    unityObj.position += new Vector3(rect.sizeDelta.x / 2, -1 * (rect.sizeDelta.y * (1 - 0.125f / 2)), 0); //move to bottom center

                    //fill any empty vertical space
                    if (!hasCenter && !hasEast && !hasWest)
                    {
                        //reposition and resize
                        float newY = unityObj.position.y;
                        float newH = unityObj.sizeDelta.y;
                        if (!hasNorth)
                        {
                            newH = rect.sizeDelta.y;
                            newY = -1 * rect.sizeDelta.y / 2; //move to center
                        }
                        else
                        {
                            newH = rect.sizeDelta.y / 2;
                            newY = -1 * rect.sizeDelta.y * 3 / 4; //move to middle lower half
                        }
                        unityObj.position = new Vector3(unityObj.position.x, newY + panelCorners[1].y, unityObj.position.z);
                        unityObj.sizeDelta = new Vector3(unityObj.sizeDelta.x, newH);
                    }
                    break;

                case "east":
                    unityObj.sizeDelta = new Vector2(rect.sizeDelta.x * 0.125f, rect.sizeDelta.y * 0.75f);
                    unityObj.position = panelCorners[1]; //top left
                    unityObj.position += new Vector3(rect.sizeDelta.x * (1 - 0.125f / 2), -1 * (rect.sizeDelta.y) / 2, 0); //move to left center

                    if (!hasCenter && !hasNorth && !hasSouth)
                    {
                        //reposition and resize
                        float newX = unityObj.position.x;
                        float newW = unityObj.sizeDelta.x;
                        if (!hasWest)
                        {
                            newW = rect.sizeDelta.x;
                            newX = rect.sizeDelta.x / 2; //move to center
                        }
                        else
                        {
                            newW = rect.sizeDelta.x / 2;
                            newX = rect.sizeDelta.x / 4; //move to middle left half
                        }
                        unityObj.position = new Vector3(newX + panelCorners[1].x, unityObj.position.y, unityObj.position.z);
                        unityObj.sizeDelta = new Vector3(newW, unityObj.sizeDelta.y);
                    }

                    break;

                case "west":
                    unityObj.sizeDelta = new Vector2(rect.sizeDelta.x * 0.125f, rect.sizeDelta.y * 0.75f);
                    unityObj.position = panelCorners[1]; //top left
                    unityObj.position += new Vector3(rect.sizeDelta.x * (0.125f / 2), -1 * (rect.sizeDelta.y) / 2, 0); //move to right center

                    if (!hasCenter && !hasNorth && !hasSouth)
                    {
                        //reposition and resize
                        float newX = unityObj.position.x;
                        float newW = unityObj.sizeDelta.x;
                        if (!hasEast)
                        {
                            newW = rect.sizeDelta.x;
                            newX = rect.sizeDelta.x / 2; //move to center
                        }
                        else
                        {
                            newW = rect.sizeDelta.x / 2;
                            newX = rect.sizeDelta.x * 3 / 4; //move to middle right half
                        }
                        unityObj.position = new Vector3(newX + panelCorners[1].x, unityObj.position.y, unityObj.position.z);
                        unityObj.sizeDelta = new Vector3(newW, unityObj.sizeDelta.y);
                    }

                    break;

                default:
                    break;
            }

            if (component.unityComponentGroup.GetType() == typeof(UnityScrollComponents))
            {
                UnityScrollComponents compgroup = (UnityScrollComponents)component.unityComponentGroup;
                compgroup.contentTextComponent.GetComponent<RectTransform>().sizeDelta
                    = compgroup.contentTextComponent.transform.parent.gameObject.GetComponent<RectTransform>().sizeDelta;
            }
        }

    }
}
