using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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

        //Subscribe to screen resize event if one exists
    }



    float GetAspectRatio()
    {
        return Camera.main.aspect;
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
