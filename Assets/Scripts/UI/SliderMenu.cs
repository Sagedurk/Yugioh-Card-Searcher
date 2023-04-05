using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderMenu : MonoBehaviour
{
    public Canvas canvas;
    public RectTransform menuTransform;
    public RectTransform menuButtons;
    public RectTransform viewPort;
    public Button returnButton;
    public float buttonAlphaValue;
    // Start is called before the first frame update
    void Start()
    {
        menuTransform.sizeDelta = new Vector2(canvas.GetComponent<RectTransform>().sizeDelta.x * 1.5f, 0);
        menuTransform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(canvas.GetComponent<RectTransform>().sizeDelta.x * 0.5f, 0);
    }

    public void OnEndDrag()
    {
        if(menuTransform.anchoredPosition.x < menuTransform.sizeDelta.x/6)
        {
            menuTransform.anchoredPosition = new Vector2(0,0);
            returnButton.interactable = false;
            returnButton.image.raycastTarget = false;
            returnButton.image.color = new Color(0, 0, 0, 0);
            ResizeViewport(false);
        }
        else
        {
            menuTransform.anchoredPosition = new Vector2(menuTransform.sizeDelta.x, 0);
            returnButton.interactable = true;
            returnButton.image.raycastTarget = true;
            returnButton.image.color = new Color(0, 0, 0, 0.5f);
            
        }
        
    }

    public void OnDrag()
    {
        buttonAlphaValue = menuTransform.anchoredPosition.x / canvas.GetComponent<RectTransform>().sizeDelta.x;
        returnButton.image.color = new Color(0, 0, 0, buttonAlphaValue);
    }

    public void OnButtonReturn()
    {
        menuTransform.anchoredPosition = new Vector2(0,0);
        returnButton.image.color = new Color(0, 0, 0, 0);
        returnButton.interactable = false;
        returnButton.image.raycastTarget = false;
        ResizeViewport(false);
    }

    public void ResizeViewport(bool widen)
    {
        if (!widen)
        {
            viewPort.sizeDelta = new Vector2(-canvas.GetComponent<RectTransform>().sizeDelta.x + 15, -menuButtons.sizeDelta.y);
            viewPort.anchoredPosition = new Vector2(0, 0);
        }
        else
        {
            viewPort.sizeDelta = new Vector2(0, 0);
            viewPort.anchoredPosition = new Vector2(0, 0);
        }
    }
}
