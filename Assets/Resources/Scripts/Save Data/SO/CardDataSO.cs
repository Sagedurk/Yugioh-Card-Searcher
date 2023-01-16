using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CardDataSO : ScriptableObject
{

    public static string loadPath = "ScriptableObjects/Cards/";
    static string savePath = "Assets/Resources/" + loadPath;
    static string fileType = ".asset";
    static string fileName;
    public static CardDataSO TryCreateInstance(CardInfoParse card)
    {
        #region SetFileName
        ApiCall.Instance.PrepToFileName(card.name);
        fileName = ApiCall.Instance.fileName;

        #endregion

        if (TryGetNonNullInstance(fileName, out CardDataSO instance))
                return instance;

        instance = CreateInstance<CardDataSO>();

        instance.SaveValues(card);

        //AssetDatabase.CreateAsset(instance, savePath + fileName + fileType);

        //AssetDatabase.SaveAssets();
        //AssetDatabase.Refresh();

        return instance;
    }

    public static List<CardDataSO> TryCreateInstances(CardInfoParse[] cards)
    {
        List<CardDataSO> instances = new List<CardDataSO>();

        foreach (CardInfoParse card in cards)
        {
            TryCreateInstance(card);
        }

        return instances;
    }

    private static CardDataSO TryGetInstance(string cardName)
    {
        return Resources.Load<CardDataSO>(loadPath + cardName);
    }

    public static bool TryGetNonNullInstance(string cardName, out CardDataSO instance)
    {
        instance = TryGetInstance(cardName);

        if (instance != null)
            return true;

        Debug.LogWarning("CardData Instance not found!");
        return false;
    }

    public void SaveValues(CardInfoParse card) 
    { 
        id = card.id;
        cardName = card.name;
        cardType = card.type;
        desc = card.desc;
        atk = card.atk;
        def = card.def;
        level = card.level;
        race = card.race;
        attribute = card.attribute;
        archetype = card.archetype;
        linkRating = card.linkval;
        linkMarkers = card.linkMarkers;
        pendulumScale = card.pendScale;

        cardSets = card.card_sets.ToList();
        cardImages = card.card_images.ToList();
        cardPrices = card.card_prices.ToList();
    }
    
    public CardInfoParse LoadValues()
    {
        CardInfoParse cardInfo = new CardInfoParse();
    
        cardInfo.id = id;
        cardInfo.name = cardName;
        cardInfo.type = cardType; 
        cardInfo.desc = desc;
        cardInfo.atk = atk;
        cardInfo.def = def;
        cardInfo.level = level;
        cardInfo.race = race;
        cardInfo.attribute = attribute;
        cardInfo.archetype = archetype;
        cardInfo.linkval = linkRating; 
        cardInfo.linkMarkers = linkMarkers;
        cardInfo.pendScale = pendulumScale;
        cardInfo.card_sets = cardSets.ToArray();
        cardInfo.card_images = cardImages.ToArray();
        cardInfo.card_prices = cardPrices.ToArray();
        

        return cardInfo;   
    }

    [SerializeField] int id = -1;
    [SerializeField] string cardName = "";
    [SerializeField] string cardType = "";
    [SerializeField] string desc = "" ;
    [SerializeField] int atk = -1;
    [SerializeField] int def = -1 ;
    [SerializeField] int level = -1;
    [SerializeField] string race = "";
    [SerializeField] string attribute = "";
    [SerializeField] string archetype = "";
    [SerializeField] int linkRating = -1;
    [SerializeField] string[] linkMarkers;
    [SerializeField] int pendulumScale = -1;
    [SerializeField] List<CardSetParse> cardSets;
    [SerializeField] List<CardImageParse> cardImages;
    [SerializeField] List<CardPriceParse> cardPrices;

}
