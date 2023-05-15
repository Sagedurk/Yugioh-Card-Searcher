using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UpdateData: EUS.Cat_Systems.Singleton<UpdateData>
{

    public const string URL = "db.ygoprodeck.com/api/v6/", cInfo = "cardinfo.php?";
    public string jsonSaveData;
    public CardInfo cardInfo;
    public Text result;

    public UnityWebRequest webRequest;
    public CardInfoParse[] parseList, toJson;

    public Image blockBackground;
    public Text updateProgressText;

    protected override void Awake()
    {
        isDestroyable= true;
        base.Awake();
    }

    //public Button nextImage, previousImage, showCardSets, cardSetCloseBtn;
    public void UpdateAllCards()
    {

        StartCoroutine(ApiCall.Instance.UpdateCards());
    }

    public void UpdateArchetypes()
    {
        StartCoroutine(ApiCall.Instance.UpdateArchetypeList());
    }

    public void UpdateCardSets()
    {
        StartCoroutine(ApiCall.Instance.UpdateCardSets());
    }

    public void UpdateImages()
    {
        StartCoroutine(ApiCall.Instance.UpdateImages());
    }

    public void UpdateSearchData()
    {
        StartCoroutine(ApiCall.Instance.UpdateSearchData());
    }



    public void BlockUIInteraction()
    {
        blockBackground.gameObject.SetActive(true);
    }
    public void UnblockUIInteraction()
    {
        blockBackground.gameObject.SetActive(false);
    }


    


}
