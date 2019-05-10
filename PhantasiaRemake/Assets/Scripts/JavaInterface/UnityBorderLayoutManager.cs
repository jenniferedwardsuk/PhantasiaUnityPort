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

            //todo - include xgap and ygap

            //set size and position
            switch (component.layoutLocation.ToLower())
            {
                case "center": //75% of layout
                    unityObj.sizeDelta = new Vector2(rect.sizeDelta.x * 0.75f, rect.sizeDelta.y * 0.75f);
                    unityObj.position = panelCorners[1];// + new Vector3(unityObj.sizeDelta.x / 2, -1 * unityObj.sizeDelta.y / 2, 0); //top left + size offset
                    unityObj.position += new Vector3(rect.sizeDelta.x / 2, -1 * rect.sizeDelta.y / 2, 0); //move to center
                    break;
                case "north": //12.5% of layout
                    unityObj.sizeDelta = new Vector2(rect.sizeDelta.x, rect.sizeDelta.y * 0.125f);
                    unityObj.position = panelCorners[1];// + new Vector3(unityObj.sizeDelta.x / 2, -1 * unityObj.sizeDelta.y / 2, 0); //top left + size offset
                    unityObj.position += new Vector3(rect.sizeDelta.x / 2, -1 * (rect.sizeDelta.y * 0.125f / 2), 0); //move to top center

                    break;
                case "south":
                    unityObj.sizeDelta = new Vector2(rect.sizeDelta.x, rect.sizeDelta.y * 0.125f);
                    unityObj.position = panelCorners[1];// + new Vector3(unityObj.sizeDelta.x / 2, -1 * unityObj.sizeDelta.y / 2, 0); //top left + size offset
                    unityObj.position += new Vector3(rect.sizeDelta.x / 2, -1 * (rect.sizeDelta.y * (1 - 0.125f / 2)), 0); //move to bottom center
                    break;
                case "east":
                    unityObj.sizeDelta = new Vector2(rect.sizeDelta.x * 0.125f, rect.sizeDelta.y * 0.75f);
                    unityObj.position = panelCorners[1];// + new Vector3(unityObj.sizeDelta.x / 2, -1 * unityObj.sizeDelta.y / 2, 0); //top left + size offset
                    unityObj.position += new Vector3(rect.sizeDelta.x * (1 - 0.125f / 2), -1 * (rect.sizeDelta.y) / 2, 0); //move to left center
                    break;
                case "west":
                    unityObj.sizeDelta = new Vector2(rect.sizeDelta.x * 0.125f, rect.sizeDelta.y * 0.75f);
                    unityObj.position = panelCorners[1];// + new Vector3(unityObj.sizeDelta.x / 2, -1 * unityObj.sizeDelta.y / 2, 0); //top left + size offset
                    unityObj.position += new Vector3(rect.sizeDelta.x * (0.125f / 2), -1 * (rect.sizeDelta.y) / 2, 0); //move to right center
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
