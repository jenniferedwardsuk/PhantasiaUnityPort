using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InputFieldSelectDetector : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public void Start()
    {

    }

    public void Update()
    {

    }


    bool isSelected;
    public void OnSelect(BaseEventData data) //from ISelectHandler
    {
        UnityJavaInterface.IsInputFieldSelected = true;
    }

    public void OnDeselect(BaseEventData data) //from IDeselectHandler
    {
        UnityJavaInterface.IsInputFieldSelected = false;
    }
}
