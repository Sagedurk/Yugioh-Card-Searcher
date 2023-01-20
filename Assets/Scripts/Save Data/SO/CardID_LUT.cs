using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.AI;

[Serializable]
public class CardID_LUT
{

    public static List<CardID_LUT> TryCreateLUTs(CardInfoParse card)
    {
        List<CardID_LUT> ID_LUTs = new List<CardID_LUT>();

        foreach (CardImageParse image in card.card_images)
        {
            if (File.Exists(SaveManager.idLutDirectory + image.id.ToString() + SaveManager.idLutFileType))
                continue;
            
            CardID_LUT instance = new CardID_LUT();

            instance.cardName = card.name;

            instance.WriteClassToFile(SaveManager.idLutDirectory, image.id.ToString(), SaveManager.idLutFileType);
        }

        return ID_LUTs;
    }

    public static void UpdateName(CardInfoParse card)
    {
        foreach (CardImageParse image in card.card_images)
        {
            CardID_LUT iD_LUT = TryGetInstance(image.id);
            
            if (iD_LUT != null)
                iD_LUT.cardName = card.name;    
        }
    }

    public static CardID_LUT TryGetInstance(int id)
    {
        if (File.Exists(SaveManager.idLutDirectory + id + SaveManager.idLutFileType))
        {
            CardID_LUT[] loadedData = SaveManager.ReadFile<CardID_LUT>(SaveManager.idLutDirectory, id.ToString(), SaveManager.idLutFileType);

            return loadedData[0];
        }

        return null;
    }

    public string GetName()
    {
        return cardName;
    }

    [SerializeField]string cardName;


}
