using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UpdateLocalCards: MonoBehaviour
{

    public const string URL = "db.ygoprodeck.com/api/v6/", cInfo = "cardinfo.php?";
    public string jsonSaveData;
    public CardInfo cardInfo;
    public Text result;

    public SaveManager saveManager;
    public UnityWebRequest webRequest;
    public CardInfoParse[] parseList, toJson;


    //public Button nextImage, previousImage, showCardSets, cardSetCloseBtn;
    public void RequestCheck()
    {
                webRequest = UnityWebRequest.Get(URL + cInfo);         //General Search
                StartCoroutine(APIrequest());
    }

    public IEnumerator APIrequest(bool resetButtons = true)
    {
        yield return webRequest.SendWebRequest();
        if (!webRequest.isNetworkError)
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


    public IEnumerator LoadCardInfo(UnityWebRequest req = null)
    {
        string json;
        
        yield return req.downloadHandler.text;
        json = "{ \"Items\":" + req.downloadHandler.text + "}";
        parseList = JsonHelper.FromJson<CardInfoParse>(json);
        

        for (int i = 0; i < parseList.Length; i++)
        {
            //JSON CONVERSION!
            toJson = new CardInfoParse[1];
            toJson[0] = parseList[i];
            jsonSaveData = JsonHelper.ToJson<CardInfoParse>(toJson);
            jsonSaveData = jsonSaveData.Replace("{\"Items\":", "");
            jsonSaveData = jsonSaveData.Replace(",\"archetype_name\":\"\"", "");
            if (!parseList[i].type.Contains("Pendulum"))
            {
                jsonSaveData = jsonSaveData.Replace(",\"scale\":0", "");
            }
            if (!parseList[i].type.Contains("Link"))
            {
                jsonSaveData = jsonSaveData.Replace(",\"linkval\":0", "");
                jsonSaveData = jsonSaveData.Replace(",\"linkmarkers\":[]", "");
            }
            jsonSaveData = jsonSaveData.Substring(0, jsonSaveData.Length - 1);

            cardInfo.PrepToFileName(parseList[i].name);
            if (System.IO.File.Exists(Application.persistentDataPath + "/" + cardInfo.fileName.ToLower() + ".txt"))
                saveManager.ReplaceFileWithNew(cardInfo.fileName, jsonSaveData);
            
            if(i == parseList.Length - 1)
            {
                result.text = "Update done!";
            }
        }
    }



}
