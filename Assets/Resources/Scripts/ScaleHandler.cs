using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Handles UI scaling, to percentage of canvas
public class ScaleHandler : MonoBehaviour
{
    Text[] texts;
    public RectTransform canvas;
    public RectTransform NotchBuffert, title, inputField, submitBtn, scrollField, errorText, image, menu, menuBuffert;
    public RectTransform param, paramVal;
    public RectTransform archetype1, archetype2, archetype3;
    public RectTransform artworkBtnNext, artworkBtnPrev;
    public RectTransform CardSetBtn;
    public int fontSize;
    int rtSize = 0;
    //readonly int paramChildAmount;
    readonly float multFont = 0.0153677277716795f;
    readonly float multPPU = 0.1097694840834248f;
    readonly float multArtworkBtn = 0.04419889f;
    
    void Start()
    {
        //set width of menubuttons
        if (EUS.sceneIndex != 0)
        {
            GameObject menu = GameObject.FindGameObjectWithTag("Canvas");
            GameObject[] menuButton = GameObject.FindGameObjectsWithTag("MenuButton");
            RectTransform rectBase = menu.GetComponent<RectTransform>();
            foreach (GameObject button in menuButton)
            {
                RectTransform rect = button.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(rectBase.sizeDelta.x / 4, rect.sizeDelta.y);

            }
        }
        switch (EUS.sceneIndex)
        {
            case 1:
                ScaleCardInfo();
                break;
            case 2:
                ScaleCardSearch();
                break;
            case 3:
                ScaleCardRandom();
                break;
            case 4:
                ScaleArchetype();
                break;
            default:
                break;
        }
        canvas.gameObject.GetComponent<CanvasScaler>().referencePixelsPerUnit = canvas.sizeDelta.y * multPPU;
    }


    void ScaleUIObject(RectTransform transform, RectTransform transformAnchor, bool scaleX, bool scaleY, float percentageX, float percentageY,bool scaleOnFraction = false, bool posY = false, bool posX = false)
    {
        if (scaleOnFraction)
        {
            percentageY = 100 / percentageY;
            percentageX = 100 / percentageX;
        }
        //scale on Y
        if (scaleY && !scaleX)      
        {
            transform.anchoredPosition = new Vector2(transform.anchoredPosition.x, transformAnchor.anchoredPosition.y - transformAnchor.sizeDelta.y);
            transform.sizeDelta = new Vector2(transform.sizeDelta.x, canvas.sizeDelta.y / 100 * percentageY);     
        }
        //scale on X
        else if (scaleX && !scaleY)     
        {
            transform.anchoredPosition = new Vector2(transformAnchor.anchoredPosition.x - transformAnchor.sizeDelta.x, transform.anchoredPosition.y);
            transform.sizeDelta = new Vector2(canvas.sizeDelta.x / 100 * percentageX, transform.sizeDelta.y);
        }
        //scale on both
        else if (scaleY && scaleX)      
        {
            transform.anchoredPosition = new Vector2(transform.anchoredPosition.x, transformAnchor.anchoredPosition.y - transformAnchor.sizeDelta.y);
            transform.sizeDelta = new Vector2(canvas.sizeDelta.x / 100 * percentageX, canvas.sizeDelta.y / 100 * percentageY);
        }
        if (posX && !posY)
        {
            transform.anchoredPosition = new Vector2(transformAnchor.anchoredPosition.x, transform.anchoredPosition.y);
        }
        else if (posY && !posX)
        {
            transform.anchoredPosition = new Vector2(transform.anchoredPosition.x, transformAnchor.anchoredPosition.y - transformAnchor.sizeDelta.y);
        }
        //Set font size
        texts = transform.GetComponentsInChildren<Text>();
        if (texts != null)
        {
            foreach (Text t in texts)
            {
                fontSize = Mathf.RoundToInt((canvas.sizeDelta.y * multFont));
                t.fontSize = fontSize;
                if (t.transform.parent.name == "Card Info")
                {   
                    //Set font size and position of scrollable card info, with the first entry being at the very top of the scrollable box
                    RectTransform tTransform = t.gameObject.GetComponent<RectTransform>();
                    tTransform.sizeDelta = new Vector2(tTransform.sizeDelta.x, t.fontSize * 2);
                    rtSize -= t.fontSize * 2;
                    if (t.transform.name == "ID")   //First entry
                        rtSize = 0;
                    tTransform.anchoredPosition = new Vector2(0, rtSize);
                }
            }
        }
    }

    //Want to add the possibility to easily find a child, even if it has a lot of parents (with different childIndexes), in the future

    

    void ScaleUIChild(RectTransform parent, int childDepth, int[] childIndexes, float anchoredPosX, float anchoredPosY, float scaleX, float scaleY)
    {
        EUS.SetChild(parent, childDepth, childIndexes).GetComponent<RectTransform>().anchoredPosition = new Vector2(anchoredPosX, anchoredPosY);
        EUS.SetChild(parent, childDepth, childIndexes).GetComponent<RectTransform>().sizeDelta = new Vector2(scaleX, scaleY);
    }

    //If ScaleUIChild gets a better solution (see comment on said function), ScaleUIDropdown won't be as complex
    public void ScaleUIDropdown(RectTransform transform, float labelP, float labelS, float arrowP, float arrowS, float viewPort, float iLabel, float scrollBar, float amountOfItems)
    {
        ScaleUIChild(transform, 1, new int[] { 0 }, transform.sizeDelta.x * labelP, 0, transform.sizeDelta.x * labelS, 0);
        ScaleUIChild(transform, 1, new int[] { 1 }, transform.sizeDelta.x * arrowP, 0, transform.sizeDelta.x * arrowS, transform.sizeDelta.y * 0.6666666666666667f);
        float arrowY = EUS.SetChild(transform, 1, new int[] { 1 }).sizeDelta.y;
        //child = SetChild(transform, 1, new int[] {1});
 /*Arrow scale*/    EUS.SetChild(transform, 1, new int[] {1}).sizeDelta = new Vector2(arrowY, arrowY);
 /*Arrow position*/ EUS.SetChild(transform, 1, new int[] {1}).anchoredPosition = new Vector2(arrowY* -0.75f, 0);
 /*Label scale*/    EUS.SetChild(transform, 1, new int[] {0}).anchoredPosition = new Vector2(arrowY, 0);
 /*Label position*/ EUS.SetChild(transform, 1, new int[] {0}).sizeDelta = new Vector2(- (arrowY * 2) + (arrowY* -0.75f / 3), 0);

        ScaleUIChild(transform, 1, new int[] { 2 }, 0, transform .sizeDelta.y * 0.0666667f, 0, transform.sizeDelta.y * 5f);
        float itemHeightMod = 0.6666666666666667f;
        EUS.SetChild(transform, 1, new int[] { 2 }).sizeDelta = new Vector2(EUS.SetChild(transform, 1, new int[] { 2 }).sizeDelta.x, transform.sizeDelta.y * itemHeightMod * amountOfItems + ((EUS.SetChild(transform, 3, new int[] { 2, 0, 0 }).sizeDelta.y - EUS.SetChild(transform, 4, new int[] { 2, 0, 0, 0 }).sizeDelta.y)) + 1);
        EUS.SetChild(transform, 2, new int[] { 2, 0 }).sizeDelta = new Vector2(transform.sizeDelta.x * viewPort, transform.sizeDelta.y * 5f);
        EUS.SetChild(transform, 3, new int[] { 2, 0, 0 }).sizeDelta = new Vector2(0, transform.sizeDelta.y * 0.9333333333333333f);
        EUS.SetChild(transform, 4, new int[] { 2, 0, 0, 0 }).sizeDelta = new Vector2(0, transform.sizeDelta.y * itemHeightMod);
        EUS.SetChild(transform, 5, new int[] { 2, 0, 0, 0, 1 }).sizeDelta = new Vector2(arrowY, arrowY);
        ScaleUIChild(transform, 5, new int[] { 2, 0, 0, 0, 2 }, EUS.SetChild(transform, 5, new int[] { 2, 0, 0, 0, 1 }).sizeDelta.x, 0, transform.sizeDelta.x * iLabel, transform.sizeDelta.y * 0.5666666666666667f);
        EUS.SetChild(transform, 5, new int[] { 2, 0, 0, 0, 2 }).GetComponent<Text>().fontSize = fontSize;
        EUS.SetChild(transform, 2, new int[] { 2, 1 }).sizeDelta = new Vector2(arrowY /*+ (arrowY/10.0f)*/, 0);//transform.sizeDelta.x * scrollBar
        ScaleUIChild(transform, 3, new int[] { 2, 1, 0 }, transform.sizeDelta.x * (scrollBar / 2), transform.sizeDelta.x * (scrollBar / 2), transform.sizeDelta.x * (scrollBar / 2), transform.sizeDelta.x * (scrollBar / 2));      //Same Value
        ScaleUIChild(transform, 4, new int[] { 2, 1, 0, 0 }, -transform.sizeDelta.x * (scrollBar / 2), -transform.sizeDelta.x * (scrollBar / 2), -transform.sizeDelta.x * (scrollBar / 2), -transform.sizeDelta.x * (scrollBar / 2));   //Same Value
    }

    //Scales all the UI elements for the different scenes
    void ScaleCardInfo()
    {
        NotchBuffert.sizeDelta = new Vector2(0, canvas.sizeDelta.y / 100 * 2.5f);
        ScaleUIObject(title, NotchBuffert, false, true, 0, 30, true);
        ScaleUIObject(inputField, title, false, true, 0, 30, true);
        ScaleUIObject(submitBtn, inputField, false, true, 0, 30, true);     
        ScaleUIObject(scrollField, submitBtn, false, true, 0, 37.5f);
        ScaleUIObject(errorText, submitBtn, false, true, 0, 37.5f);
        ScaleUIObject(image, scrollField, false, true, 0, 45);
        
        ScaleUIObject(menu, image, false, true, 0, 5);

        SetSizeAndPosUI(image, 0, 0, image.sizeDelta.y / 1.46f, image.sizeDelta.y);
        SetSizeAndPosUI(artworkBtnNext, (image.sizeDelta.x/2) + (image.sizeDelta.x/100*2), -image.sizeDelta.y / 2, menu.sizeDelta.y, menu.sizeDelta.y);
        SetSizeAndPosUI(artworkBtnPrev, -(image.sizeDelta.x/2) - (image.sizeDelta.x / 100 * 2), -image.sizeDelta.y / 2, menu.sizeDelta.y, menu.sizeDelta.y);
        SetSizeAndPosUI(CardSetBtn, -(image.sizeDelta.x /2) - (image.sizeDelta.x / 100 * 2), menu.sizeDelta.y*3, canvas.sizeDelta.x/2 + (-(image.sizeDelta.x / 2) - (image.sizeDelta.x / 100 * 2)) - 15, menu.sizeDelta.y);

        if (CardSetBtn.sizeDelta.x < artworkBtnNext.sizeDelta.x)
        {
            CardSetBtn.sizeDelta = new Vector2(artworkBtnNext.sizeDelta.x, CardSetBtn.sizeDelta.y);
        }

        artworkBtnPrev.GetChild(0).GetComponent<Text>().fontSize = fontSize;
        artworkBtnNext.GetChild(0).GetComponent<Text>().fontSize = fontSize;
        CardSetBtn.GetChild(0).GetComponent<Text>().fontSize = fontSize;
    }
    void ScaleCardSearch()
    {
        NotchBuffert.sizeDelta = new Vector2(0, canvas.sizeDelta.y / 100 * 2.5f);
        ScaleUIObject(title, NotchBuffert, false, true, 0, 30, true, true);
        ScaleUIObject(inputField, title, true, true, 95, (100/30f));            //Using 100/30 because it's easier to do, than to calculate 95% into fraction
        ScaleUIObject(param, inputField, true, true, 38, (100 / 30f));          //100/3/10 = 100/10/3 = 100/30 = 3.33333333%          
        ScaleUIObject(paramVal, param, true, true, 57, (100 / 30f));
        ScaleUIObject(submitBtn, param, true, true, 95, (100/30f));        
        ScaleUIObject(scrollField, submitBtn, true, true, 95, (100/30f)*2 + 72.5f);
        ScaleUIObject(menu, scrollField, false, true, 0, 5);

        scrollField.GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector2(canvas.sizeDelta.y / 1080 * 5, 0);
        //scrollField.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(- (2 * scrollField.sizeDelta.x/100), scrollField.GetChild(0).GetComponent<RectTransform>().sizeDelta.y);
        scrollField.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(-canvas.sizeDelta.y/1080*5*2, scrollField.GetChild(0).GetComponent<RectTransform>().sizeDelta.y);

        //Make sure everything has a percentual margin from the side of the canvas, this needs to be made into a function and added to the other scenes??
        //Does the other scenes need CanvasMargin??
        CanvasMargin(inputField);

        //Scale the dropdown lists

        ScaleUIDropdown(param, 0.0625f, 0.15625f, -0.09375f, 0.125f, 0.89375f, 0.70625f, 0.125f, 20);
        ScaleUIDropdown(paramVal, 0.043478f, 0.108695f, -0.065217f, 0.086957f, 0.921739f, 0.791304f, 0.086957f, 2);
    }
    void ScaleCardRandom()
    {
        NotchBuffert.sizeDelta = new Vector2(0, canvas.sizeDelta.y / 100 * 2.5f);
        ScaleUIObject(title, NotchBuffert, false, true, 0, 30, true);
        ScaleUIObject(submitBtn, title, false, true, 0, 30, true);              
        ScaleUIObject(scrollField, submitBtn, false, true, 0, (100/30f) + 37.5f);
        ScaleUIObject(image, scrollField, false, true, 0, 45);
        ScaleUIObject(menu, image, false, true, 0, 5);
        SetSizeAndPosUI(image, 0, 0, image.sizeDelta.y / 1.46f, image.sizeDelta.y);
    }
    void ScaleArchetype()
    {
        NotchBuffert.sizeDelta = new Vector2(0, canvas.sizeDelta.y / 100 * 2.5f);
        ScaleUIObject(title, NotchBuffert, false, true, 0, 30, true);
        ScaleUIObject(submitBtn, title, false, true, 0, 30, true);
        ScaleUIObject(archetype1, submitBtn, false, true, 0, 30, true);
        ScaleUIObject(archetype2, archetype1, false, true, 0, 30, true);
        ScaleUIObject(archetype3, archetype2, false, true, 0, 30, true);
        ScaleUIObject(menuBuffert, archetype3, false, true, 0, (100f/30f) + 7.5f + 65);             
        ScaleUIObject(menu, menuBuffert, false, true, 0, 5);               
    }
    
    void CanvasMargin(RectTransform transform)
    {
        transform.anchoredPosition = new Vector2(canvas.sizeDelta.x / 100 * 2.5f, transform.anchoredPosition.y);
        submitBtn.anchoredPosition = new Vector2(canvas.sizeDelta.x / 100 * 2.5f, submitBtn.anchoredPosition.y);
        param.anchoredPosition = new Vector2(canvas.sizeDelta.x / 100 * 2.5f, param.anchoredPosition.y);
        paramVal.anchoredPosition = new Vector2(-(canvas.sizeDelta.x / 100 * 2.5f), param.anchoredPosition.y);
        scrollField.anchoredPosition = new Vector2((canvas.sizeDelta.x / 100 * 2.5f), scrollField.anchoredPosition.y);
    }

    void SetSizeAndPosUI(RectTransform rTransform, float xPos, float yPos, float xSize, float ySize)
    {
        rTransform.anchoredPosition = new Vector2(xPos, yPos);
        rTransform.sizeDelta = new Vector2(xSize, ySize);
    }
}
