using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

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

            instance.TryWriteClassToFile(SaveManager.idLutDirectory, image.id.ToString(), SaveManager.idLutFileType);
        }

        return ID_LUTs;
    }

    public static CardID_LUT TryGetLUT(int id)
    {
        if (!File.Exists(SaveManager.idLutDirectory + id.ToString() + SaveManager.idLutFileType))
          return null;

        CardID_LUT iD_LUT = new CardID_LUT().TryReadFileToClass(SaveManager.idLutDirectory, id.ToString(), SaveManager.idLutFileType);
        return iD_LUT;
    }

    public static void UpdateLUTs(List<CardInfoParse> cards)
    {
        SaveManager.DeleteFilesInDirectory(SaveManager.idLutDirectory);

        foreach (CardInfoParse card in cards)
        {
            TryCreateLUTs(card);
        }
    }

    public static void UpdateName(CardInfoParse card)
    {
        foreach (CardImageParse image in card.card_images)
        {
            CardID_LUT iD_LUT = TryGetLUT(image.id);
            
            if (iD_LUT != null)
                iD_LUT.cardName = card.name;    
        }
    }

    public string GetName()
    {
        return cardName;
    }

    [SerializeField]string cardName;


}
