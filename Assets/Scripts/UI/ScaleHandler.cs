using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//Handles UI scaling, to percentage of canvas
public class ScaleHandler : EUS.Cat_Systems.Singleton<ScaleHandler>
{
    Text[] texts;
    public CanvasScaler scaler;
    [Space(40)]
    public RectTransform canvas;
    public RectTransform param, paramVal;
    [HideInInspector] public static int fontSize;
    int rtSize = 0;
    //readonly int paramChildAmount;
    readonly float multFont = 0.0153677277716795f;
    readonly float aspect_9_16 = 0.5625f;

    protected override void Awake()
    {
        base.Awake();


        SetScaling();
    }


    void Start()
    {
        SetScaling();
        return;

        EUS.Cat_Scene.currentScene = (SceneNames)UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;

        //set width of menubuttons
        if (EUS.Cat_Scene.currentScene != SceneNames.PRE_SETUP)
        {
            GameObject menu = GameObject.FindGameObjectWithTag("Canvas");
            GameObject[] menuButton = GameObject.FindGameObjectsWithTag("MenuButton");
            RectTransform rectBase = menu.GetComponent<RectTransform>();
            foreach (GameObject button in menuButton)
            {
                RectTransform rect = button.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(rectBase.sizeDelta.x / menuButton.Length, rect.sizeDelta.y);

            }
        }
    }


    float GetAspectRatio()
    {
        Resolution currentRes = Screen.currentResolution;
        float aspectRatio = Camera.main.aspect;
        Debug.Log("AR: " + aspectRatio);
        return aspectRatio;
    }

    /// <summary>
    /// Scale the UI in the proper dimensions depending on the Aspect Ratio
    /// </summary>
    void SetScaling()
    {
        if (scaler == null)
            return;

        float aspectRatio = GetAspectRatio();

        if(aspectRatio < aspect_9_16)
            scaler.matchWidthOrHeight = 0.0f;
        else
            scaler.matchWidthOrHeight = 1.0f;
        
    }

    public static Vector2 GetCanvasSize()
    {
        return Instance.canvas.sizeDelta;
    }


}
