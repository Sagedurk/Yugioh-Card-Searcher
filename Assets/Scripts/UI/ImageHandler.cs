using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Handles the card image zoom
//Move to ScaleHandler?
public class ImageHandler : MonoBehaviour
{
    public CardInfo card;
    public Canvas canvas;
    public RectTransform background;
    RectTransform rectTransform;
    RectTransform canvasTransform;
    float origWidth;
    float origHeight;
    float origPosX;
    float origPosY;
    float imageMult = 0.95f;
    bool imageScaled = false;

    Vector2 minAnchor, maxAnchor;

    void Start()
    {
        rectTransform = card.image.gameObject.GetComponent<RectTransform>();
        canvasTransform = canvas.gameObject.GetComponent<RectTransform>();
        origPosX = rectTransform.anchoredPosition.x;
        //origPosY = rectTransform.anchoredPosition.y;
        origPosY = 0;
        origHeight = rectTransform.sizeDelta.y;
        origWidth = rectTransform.sizeDelta.x;
    }

    //Called by button
    public void ImageScale()
    {
        if (card.image.gameObject.GetComponent<RawImage>().color == new Color(1, 1, 1, 1))
        {
            if (!imageScaled)
            {
                minAnchor = rectTransform.anchorMin; 
                maxAnchor = rectTransform.anchorMax;

                // If X < Y in Aspect Ratio X:Y     (Tall Aspect Ratios)
                if (((canvasTransform.sizeDelta.x * imageMult) * 1.46f) < canvasTransform.sizeDelta.y) { 
                    rectTransform.sizeDelta = new Vector2(canvasTransform.sizeDelta.x * imageMult, (canvasTransform.sizeDelta.x * imageMult) * 1.46f);
                    rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                    rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                    rectTransform.pivot = new Vector2(0.5f, 0.5f);
                    rectTransform.anchoredPosition = new Vector2(0, 0);
                    imageScaled = true;
                }
                // If X > Y in Aspect Ratio X:Y     (Wide Aspect Ratios)
                else if (((canvasTransform.sizeDelta.x * imageMult) * 1.46f) > canvasTransform.sizeDelta.y)
                {
                    rectTransform.sizeDelta = new Vector2((canvasTransform.sizeDelta.y * imageMult) /1.46f, canvasTransform.sizeDelta.y * imageMult);
                    rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                    rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                    rectTransform.pivot = new Vector2(0.5f, 0.5f);
                    rectTransform.anchoredPosition = new Vector2(0, 0);
                    imageScaled = true;
                }
                background.sizeDelta = new Vector2(canvasTransform.sizeDelta.x, canvasTransform.sizeDelta.y);
            }
            else if (imageScaled)
            {
                //Scales card image back to the original size
                background.sizeDelta = new Vector2(0, 0);
                rectTransform.sizeDelta = new Vector2(origHeight/1.46f, origHeight);
                rectTransform.anchorMin = minAnchor;
                rectTransform.anchorMax = maxAnchor;
                rectTransform.pivot = new Vector2(0.5f, 1);
                rectTransform.anchoredPosition = new Vector2(origPosX, origPosY);
                imageScaled = false;
            }
        }
    }
}