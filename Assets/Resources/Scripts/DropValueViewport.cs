using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropValueViewport : MonoBehaviour
{
    ScaleHandler scaleMngr;
    public int amountOfItems;
    int maxAmountOfItems = 36;
    bool update = false;

    void Start()
    {   //called when changing [parameterValue].value
        
        scaleMngr = FindObjectOfType<ScaleHandler>();        
        if (amountOfItems > maxAmountOfItems) {
            amountOfItems = maxAmountOfItems;
        }

        Transform viewportDelta = scaleMngr.paramVal.GetChild(2).GetChild(0).GetChild(0);
        scaleMngr.paramVal.GetChild(3).GetComponent<RectTransform>().sizeDelta = 
            new Vector2(scaleMngr.paramVal.GetChild(2).GetComponent<RectTransform>().sizeDelta.x, 
            scaleMngr.paramVal.sizeDelta.y * 0.6666666666666667f * amountOfItems + 
            (viewportDelta.GetComponent<RectTransform>().sizeDelta.y - viewportDelta.GetChild(0).GetComponent<RectTransform>().sizeDelta.y));
    }
}