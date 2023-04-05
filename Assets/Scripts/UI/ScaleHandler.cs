using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static ApiCall;

public class ScaleHandler : EUS.Cat_Systems.Singleton<ScaleHandler>
{
    public CanvasScaler scaler;
    public RectTransform canvas;
    [HideInInspector] public static int fontSize;
    readonly float aspect_9_16 = 0.5625f;

    protected override void Awake()
    {
        ScaleHandler instance = TryGetInstance();
        if (instance)
        {
            instance.scaler = scaler;
            instance.canvas = canvas;
        }


        base.Awake();
        SetScaling();

        //Subscribe to screen resize event if one exists
    }

    /// <summary>
    /// Scale the UI in the proper dimensions depending on the Aspect Ratio
    /// </summary>
    void SetScaling()
    {
        if (scaler == null)
            return;

        float aspectRatio = Camera.main.aspect;

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
