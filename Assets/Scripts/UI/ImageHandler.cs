using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Handles the card image zoom
//Move to ScaleHandler?
public class ImageHandler: MonoBehaviour
{

    public RectTransform background;
    public RawImage enlargedImage;
    public RawImage originalImage;

    public void EnlargeImage()
    {
        background.gameObject.SetActive(true);
        originalImage.gameObject.SetActive(false);
        enlargedImage.texture= originalImage.texture;

    }

    public void ShrinkImage()
    {
        originalImage.gameObject.SetActive(true);
        background.gameObject.SetActive(false);
    }

}