using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSearchResult : MonoBehaviour
{

    public void CopyCardName()
    {
        //Send to Card Information? Automate the process?

        //copies the card name to the clipboard
        GUIUtility.systemCopyBuffer = gameObject.name;
    }



}
