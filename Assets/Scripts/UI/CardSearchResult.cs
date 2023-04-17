using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ApiCall;
using static UnityEngine.Networking.UnityWebRequest;

public class CardSearchResult : MonoBehaviour
{

    public Button copyButton, cardImageButton;
    public Text cardName;
    public RawImage background, cardImage;
    public int imageID;
    private bool hasLoadedLargeImage = false;


    private void Start()
    {
        cardImageButton.OverrideOnClick(OnCardImage);

    }

    public void CopyCardName()
    {
        //Send to Card Information? Automate the process?

        //copies the card name to the clipboard
        GUIUtility.systemCopyBuffer = cardName.text;
    }


    public void OnCardImage()
    {
        if (!hasLoadedLargeImage)
        {
            ApiCall.Instance.LoadImage(imageID, cardImage, ImageTypes.LARGE);
            hasLoadedLargeImage = true;
        }

        ImageHandler.Instance.originalImage = cardImage;
        ImageHandler.Instance.EnlargeImage();
    }
    


}
