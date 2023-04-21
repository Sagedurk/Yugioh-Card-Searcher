using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UpdateData: EUS.Cat_Systems.Singleton<UpdateData>
{

    public const string URL = "db.ygoprodeck.com/api/v6/", cInfo = "cardinfo.php?";
    public string jsonSaveData;
    public CardInfo cardInfo;
    public Text result;

    public SaveManager saveManager;
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

        StartCoroutine(ApiCall.Instance.FetchAllCards());
    }

    public void UpdateArchetypes()
    {
        StartCoroutine(ApiCall.Instance.UpdateArchetypeList());
    }

    public void UpdateCardSets()
    {
        StartCoroutine(ApiCall.Instance.UpdateCardSets());
    }

    public void BlockUIInteraction()
    {
        Debug.Log("BLOCKED");
        blockBackground.gameObject.SetActive(true);
    }
    public void UnblockUIInteraction()
    {
        Debug.Log("UNBLOCKED");
        blockBackground.gameObject.SetActive(false);
    }

    public IEnumerator APIrequest(bool resetButtons = true)
    {
        yield return webRequest.SendWebRequest();
        if (webRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            if (webRequest.downloadHandler.text.Contains("No card matching your query was found in the database."))
            {
                result.text = "No matching card was found.";
            }
            else if (webRequest.downloadHandler.text.Contains("\"error\":"))
            {
                result.text = "An error has occurred.";
            }
            else
            {
                StartCoroutine(LoadCardInfo(webRequest));
            }
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    public IEnumerator LoadCardInfo(UnityWebRequest req = null)
    {
        string json;
        
        yield return req.downloadHandler.text;
        json = "{ \"data\":" + req.downloadHandler.text + "}";
        parseList = JsonParser.FromJson<CardInfoParse>(json);
        

        for (int i = 0; i < parseList.Length; i++)
        {
            SaveManager.CreateID_LUTs(parseList[i]);
            parseList[i].TryWriteClassToFile(SaveManager.cardDirectory, parseList[i].name, SaveManager.cardFileType);

            //Old Update!

            ////JSON CONVERSION!
            //toJson = new CardInfoParse[1];
            //toJson[0] = parseList[i];
            //jsonSaveData = JsonParser.ToJson<CardInfoParse>(toJson);
            //jsonSaveData = jsonSaveData.Replace("{\"data\":", "");
            //jsonSaveData = jsonSaveData.Replace(",\"archetype_name\":\"\"", "");
            //if (!parseList[i].type.Contains("Pendulum"))
            //{
            //    jsonSaveData = jsonSaveData.Replace(",\"scale\":0", "");
            //}
            //if (!parseList[i].type.Contains("Link"))
            //{
            //    jsonSaveData = jsonSaveData.Replace(",\"linkval\":0", "");
            //    jsonSaveData = jsonSaveData.Replace(",\"linkmarkers\":[]", "");
            //}
            //jsonSaveData = jsonSaveData.Substring(0, jsonSaveData.Length - 1);

            //ApiCall.Instance.PrepToFileName(parseList[i].name);
            //if (System.IO.File.Exists(Application.persistentDataPath + "/" + ApiCall.Instance.fileName.ToLower() + ".txt"))
            //    saveManager.OverwriteStringFile(ApiCall.Instance.fileName, jsonSaveData);

            //if(i == parseList.Length - 1)
            //{
            //    result.text = "Update done!";
            //}



        }
    }



}
