﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Handles the card image zoom
//Move to ScaleHandler?
public class ImageHandler: EUS.Cat_Systems.Singleton<ImageHandler>
{

    public RectTransform background;
    public RawImage enlargedImage;
    public RawImage originalImage;

    protected override void Awake()
    {
        isDestroyable = true;
        base.Awake();
    }

    public void EnlargeImage()
    {
        if (originalImage.texture == null)
            return;

        background.gameObject.SetActive(true);
        originalImage.gameObject.SetActive(false);
        enlargedImage.texture = originalImage.texture;
    }

    public void ShrinkImage()
    {
        originalImage.gameObject.SetActive(true);
        background.gameObject.SetActive(false);
    }


}