using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static RectTransformExt;
using Button = UnityEngine.UI.Button;

public class ApiCall : EUS.Cat_Systems.Singleton<ApiCall>
{
    #region Variables

    public CardInfo cardInfo;
    public CardSearch cardSearch;

    public SaveManager saveManager;
    public ApiTypes apiType;
    public LoadTypes loadType;

    public UnityWebRequest webRequest;


   

    protected const string URL = "db.ygoprodeck.com/api/v7/", endpointCard = "cardinfo.php?", endpointArchetype = "archetypes.php";
    public const string imageURL = "https://images.ygoprodeck.com/images/";
    [HideInInspector] public string dropdownUrlMod, cardID, jsonSaveData, fileName, nameUrlMod;


    private string cardName;

    public enum ApiTypes
    {
        CARD_INFO,
        CARD_SEARCH,
        CARD_RANDOM
    }

    public enum LoadTypes
    {
        API,
        FILE
    }

    public enum ImageTypes
    {
        LARGE,
        SMALL,
        CROPPED
    }

    #endregion

    protected override void Awake()
    {
        base.Awake();


    }

    public void Execute()
    {
        SetID();

     
        //Needs to be redone to work with more than Card Info
        if (cardID == null)
            return;

        switch (apiType)
        {
            case ApiTypes.CARD_INFO:

                StartCoroutine(CardInfoExecute());
                break;

            case ApiTypes.CARD_SEARCH:

                CardSearchExecute();
                break;

            case ApiTypes.CARD_RANDOM:


                break;

            default:
                break;
        }
    }

    //TODO: Rename function to CardInfo
    private IEnumerator CardInfoExecute()
    {
        if (cardID == "")
            yield break;

        cardName = cardID;

        
        if (int.TryParse(cardID, out int id))
        {
            CardID_LUT id_LUT = CardID_LUT.TryGetInstance(id);
            Debug.Log("LUT ATTEMPT");

            //Fetch card name from API if ID_LUT doesn't exist yet
            if (id_LUT == null)
            {
                Debug.Log(URL + endpointCard + "id=" + cardID);
                webRequest = UnityWebRequest.Get(URL + endpointCard + "id=" + cardID);

                yield return StartCoroutine(APIRequestID());
            }
            else
            {
                cardName = id_LUT.GetName();
            }
            
        }

        PrepToFileName(cardName);
        CardDataSO instance = saveManager.TryGetCardDataInstance(fileName);


        //Get data from local file
        if (instance != null)
        {
            Debug.Log("Load card data!");
            loadType = LoadTypes.FILE;

            CardInfoParse loadedData = instance.LoadValues();

            string loadedDataJson = JsonParser.ToJson(loadedData);
            saveManager.fileData = loadedDataJson;

            cardInfo.ClearTextInfo(cardInfo.errorText, true);
            StartCoroutine(LoadCardInfo()); 
        }

        //Get data from API
        else
        {
            loadType = LoadTypes.API;
            nameUrlMod = "name=";
            webRequest = UnityWebRequest.Get(URL + endpointCard + nameUrlMod + cardName);
            StartCoroutine(APIRequest());
        }

    }

    private void CardSearchExecute()
    {
        PrepToFileName(cardID);
       
        if (File.Exists(Application.persistentDataPath + "/" + fileName.ToLower() + dropdownUrlMod.ToLower() + " search" + ".txt"))
        {
            saveManager.ReadFile(fileName + dropdownUrlMod + " search");
            cardSearch.ResetPrefab();
            cardInfo.ClearTextInfo(new TextExtension[] { cardInfo.errorText }, true);
                
            loadType = LoadTypes.FILE;
            StartCoroutine(LoadCardSearch());
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(cardID) || !string.IsNullOrWhiteSpace(dropdownUrlMod))
            {
                webRequest = UnityWebRequest.Get(URL + endpointCard + "fname=" + cardID + dropdownUrlMod);
                Debug.Log(URL + endpointCard + "fname=" + cardID + dropdownUrlMod);
                StartCoroutine(APIRequest());
            }
            else
            {
                //Add errortext
            }
        }
            
        cardInfo.ClearTextInfo(new TextExtension[] { cardInfo.id, cardInfo.cardName, cardInfo.cardType, cardInfo.monsterType, cardInfo.atk, cardInfo.def, cardInfo.level, cardInfo.attribute, cardInfo.pendulumScale, cardInfo.archetype, cardInfo.desc }, true);

    }

    public IEnumerator APIRequestID()
    {
        Debug.Log("ID First Pass!");
        yield return webRequest.SendWebRequest();

        if (webRequest.downloadHandler.text.Contains("No card matching your query was found in the database."))
            OnApiNoDataFound(true);

        else if (webRequest.downloadHandler.text.Contains("\"error\":"))
            OnApiError(true);

            yield return webRequest.downloadHandler.text;
        string jsonData = webRequest.downloadHandler.text;

  
        CardInfoParse[] cards = JsonParser.FromJson<CardInfoParse>(jsonData);
        cardName = cards[0].name;
    }

    public IEnumerator APIRequest(bool resetButtons = true)
    {
        yield return webRequest.SendWebRequest();

        //If there is a connection error to the API, stop
        //TODO: Set up some error message if connection error happens
        if (webRequest.result == UnityWebRequest.Result.ConnectionError)
            yield break;

        #region API Responses
        if (webRequest.downloadHandler.text.Contains("No card matching your query was found in the database."))
            OnApiNoDataFound(resetButtons);

        else if (webRequest.downloadHandler.text.Contains("\"error\":"))
            OnApiError(resetButtons);

        else
            OnApiSuccess(resetButtons);
        #endregion

    }
    
    #region API Data Managing
    void OnApiNoDataFound(bool resetButtons)
    {
        if(cardSearch)
            cardSearch.ResetPrefab();
        cardInfo.ClearTextInfo(new TextExtension[] { cardInfo.id, cardInfo.cardName, cardInfo.cardType, cardInfo.monsterType, cardInfo.atk, cardInfo.def, cardInfo.level, cardInfo.attribute, cardInfo.pendulumScale, cardInfo.archetype, cardInfo.desc }, resetButtons);
        cardInfo.errorText.SetText("No matching card was found.");

        cardInfo.image.color = Vector4.zero;
        cardInfo.showCardSets.interactable = false;
        cardInfo.showCardSets.gameObject.SetActive(false);
        cardInfo.artworkButtons.SetActive(false);
    }

    void OnApiError(bool resetButtons)
    {
        cardSearch.ResetPrefab();
        cardInfo.ClearTextInfo(new TextExtension[] { cardInfo.id, cardInfo.cardName, cardInfo.cardType, cardInfo.monsterType, cardInfo.atk, cardInfo.def, cardInfo.level, cardInfo.attribute, cardInfo.pendulumScale, cardInfo.archetype, cardInfo.desc }, resetButtons);
        cardInfo.errorText.SetText("An error has occurred.");
    }

    void OnApiSuccess(bool resetButtons)
    {
        Debug.Log("API Success!");

        loadType = LoadTypes.API;

        if (cardInfo.errorText != null)
        {
            cardInfo.ClearTextInfo(cardInfo.errorText, resetButtons);
        }


        //Load data based on the API type set
        switch (apiType)
        {
            case ApiTypes.CARD_INFO:
                
                StartCoroutine(LoadCardInfo());
                break;
            case ApiTypes.CARD_SEARCH:
                
                cardSearch.ResetPrefab();
                StartCoroutine(LoadCardSearch());
                break;
            case ApiTypes.CARD_RANDOM:
                
                
                break;
            default:
                break;
        }


    }

    #endregion




    
    #region Load Data
    public IEnumerator LoadCardInfo()
    {
        #region Parse Data
        string jsonData = "";

        switch (loadType)
        {
            case LoadTypes.API:
                yield return webRequest.downloadHandler.text;
                jsonData = webRequest.downloadHandler.text;
                break;

            case LoadTypes.FILE:
                yield return saveManager.fileData;
                jsonData = saveManager.fileData;
                break;
        }

        cardInfo.fetchedCards = JsonParser.FromJson<CardInfoParse>(jsonData);
        #endregion

        CardInfoParse card = cardInfo.fetchedCards[0];
        SaveAPIData(cardInfo.fetchedCards);

        saveManager.CreateID_LUTs(card);
        StartCoroutine(cardInfo.ConvertData(card));
    }


    public IEnumerator LoadCardSearch()
    {

        #region Parse Data
        string jsonData = "";

        switch (loadType)
        {
            case LoadTypes.API:
                yield return webRequest.downloadHandler.text;
                jsonData = webRequest.downloadHandler.text;
                break;

            case LoadTypes.FILE:
                yield return saveManager.fileData;
                jsonData = saveManager.fileData;
                break;
        }

        cardSearch.fetchedCards = JsonParser.FromJson<CardInfoParse>(jsonData);
        #endregion

        cardSearch.ConvertData(cardSearch.fetchedCards);

    }

    #endregion


    #region Misc



    public IEnumerator GetArchetypes()
    {
        webRequest = UnityWebRequest.Get(URL + endpointArchetype);
        yield return StartCoroutine(ArchetypeRequest());
    }

    //TODO: Set Private?
    public void SetID()
    {
        if (cardInfo.idInput != null)
            cardID = cardInfo.idInput.text;
    }

    #endregion


    #region File Name Functions
    public void PrepToFileName(string fileNameInsert)
    {
        fileName = fileNameInsert;
        ReplaceToValidFileName(":", "_");
        ReplaceToValidFileName("/", "=");
        ReplaceToValidFileName("?", "ʔ");
        ReplaceToValidFileName("%", "¤");
        ReplaceToValidFileName("\"", "^");
    }

    public void PrepFromFileName()
    {
        ReplaceToValidFileName("_", ":");
        ReplaceToValidFileName("=", "/");
        ReplaceToValidFileName("ʔ", "?");
        ReplaceToValidFileName("¤", "%");
        ReplaceToValidFileName("^", "\"");
    }
    private void ReplaceToValidFileName(string toReplace, string replaceTo)
    {
        if (fileName.Contains(toReplace))
        {
            fileName = fileName.Replace(toReplace, replaceTo);
        }
    }

    void SaveAPIData(CardInfoParse[] cards)
    {
        foreach (CardInfoParse card in cards)
        {
            if (loadType != LoadTypes.API)
                break;

            PrepToFileName(card.name);


            saveManager.CreateCardDataInstance(card);
            
            //saveManager.WriteFile(fileName, webRequest.downloadHandler.text);
        }
        
    }
    #endregion


    #region Image Functions
    public IEnumerator DownloadImages(string baseUrl, CardInfoParse card, bool overwriteData = false)
    {
        Debug.Log("DOWNLOADING IMAGES");

        string fileType = ".jpg";

        foreach (CardImageParse cardImage in card.card_images)
        {
            ImageData imageDataInstance = ImageData.TryGetImageData(cardImage.id);

            //If all images exist and overwriting shouldn't happen, check next ID_LUT
            if (!overwriteData && imageDataInstance != null)
                continue;



            //Download image data
            UnityWebRequest imageRequestLarge = UnityWebRequestTexture.GetTexture(baseUrl + "cards/" + cardImage.id.ToString() + fileType);
            UnityWebRequest imageRequestSmall = UnityWebRequestTexture.GetTexture(baseUrl + "cards_small/" + cardImage.id.ToString() + fileType);
            UnityWebRequest imageRequestCropped = UnityWebRequestTexture.GetTexture(baseUrl + "cards_cropped/" + cardImage.id.ToString() + fileType);
            yield return imageRequestLarge.SendWebRequest();
            yield return imageRequestSmall.SendWebRequest();
            yield return imageRequestCropped.SendWebRequest();

            //Encode image data
            byte[] largeTextureBytes = DownloadHandlerTexture.GetContent(imageRequestLarge).EncodeToJPG(100);
            byte[] smallTextureBytes = DownloadHandlerTexture.GetContent(imageRequestSmall).EncodeToJPG(100);
            byte[] croppedTextureBytes = DownloadHandlerTexture.GetContent(imageRequestCropped).EncodeToJPG(100);

            //Save image data
            ImageData.CreateImageData(cardImage.id, largeTextureBytes, smallTextureBytes, croppedTextureBytes);
        }
    }
    public void LoadImage(int id, RawImage img, ImageTypes imageType)
    {
        ImageData imageData = ImageData.TryGetImageData(id);
    
        if(imageData == null)
        {
            img.color = Color.clear;
            return;
        }

        byte[] imageBytes = null;

        if (imageType == ImageTypes.LARGE)
            imageBytes = imageData.GetLargeImage();
        else if (imageType == ImageTypes.SMALL)
            imageBytes = imageData.GetSmallImage();
        else if (imageType == ImageTypes.CROPPED)
            imageBytes = imageData.GetCroppedImage();


        Texture2D texture = new Texture2D(1, 1);
        texture.LoadImage(imageBytes);
        img.texture = texture;
        img.color = Color.white;
        
       

        if (apiType == ApiTypes.CARD_INFO || apiType == ApiTypes.CARD_RANDOM)
            cardInfo.artworkButtons.SetActive(true);
    }
    #endregion


    #region CardSets
    private IEnumerator CardSetRequest()
    {
        yield return webRequest.SendWebRequest();

        if (cardInfo.errorText != null)
            cardInfo.ClearTextInfo(new TextExtension[] { cardInfo.errorText });
        if (System.IO.File.Exists(Application.persistentDataPath + "/cardsets.txt"))
        {
            saveManager.ReadFile("cardsets");
            yield return StartCoroutine(LoadCardSetInfo(null, saveManager.fileData));
        }
        else
        {
            yield return StartCoroutine(LoadCardSetInfo(webRequest));
        }
    }

    public IEnumerator GetCardSet()
    {
        webRequest = UnityWebRequest.Get("db.ygoprodeck.com/api/v7/cardsets.php");                  //Randomcard
        yield return StartCoroutine(CardSetRequest());
    }

    public IEnumerator LoadCardSetInfo(UnityWebRequest req = null, string cachedData = "")
    {

        string json;
        if (req != null)
        {
            yield return req.downloadHandler.text;
            json = "{ \"data\":" + req.downloadHandler.text + "}";
            cardInfo.parseSetList = JsonParser.FromJson<CardSetInfo>(json);
        }
        else if (cachedData != "")
        {
            yield return cachedData;
            json = "{ \"data\":" + cachedData + "}";
            cardInfo.parseSetList = JsonParser.FromJson<CardSetInfo>(json);
        }

        if (cardInfo.parseSetList == null)
            yield break;

        cardInfo.dropDownMenu.cardset.Clear();
        cardInfo.dropDownMenu.cardset.Add(new Dropdown.OptionData(""));
        for (int i = 0; i < cardInfo.parseSetList.Length; i++)
        {
            cardInfo.dropDownMenu.cardset.Add(new Dropdown.OptionData(cardInfo.parseSetList[i].set_name));
        }

        if (req != null)
        {
            req.downloadHandler.text.WriteStringToFile(SaveManager.rootDirectory, "cardsets", SaveManager.parameterFileType);
            //saveManager.WriteFile("cardsets", req.downloadHandler.text);
        }
    }

    #endregion


    #region Archetypes
    public IEnumerator ArchetypeRequest()
    {
        yield return webRequest.SendWebRequest();

        if (cardInfo.errorText != null)
            cardInfo.ClearTextInfo(new TextExtension[] { cardInfo.errorText });
        if (System.IO.File.Exists(Application.persistentDataPath + "/archetypes.txt"))
        {
            saveManager.ReadFile("archetypes");
            yield return StartCoroutine(LoadArchetypeInfo(null, saveManager.fileData));
        }
        else
        {
            yield return StartCoroutine(LoadArchetypeInfo(webRequest));
        }
    }

    public IEnumerator LoadArchetypeInfo(UnityWebRequest req = null, string cachedData = "")
    {
        string json;

        if (req != null)
        {
            yield return req.downloadHandler.text;
            json = "{ \"data\":" + req.downloadHandler.text + "}";
            cardInfo.parseArchList = JsonParser.FromJson<ArchetypeParse>(json);
        }
        else if (cachedData != "")
        {
            yield return cachedData;
            json = "{ \"data\":" + cachedData + "}";
            cardInfo.parseArchList = JsonParser.FromJson<ArchetypeParse>(json);
        }

        if (cardInfo.dropDownMenu != null)
        {
            cardInfo.dropDownMenu.archetype.Clear();
            cardInfo.dropDownMenu.archetype.Add(new Dropdown.OptionData(""));
        }

        for (int i = 0; i < cardInfo.parseArchList.Length; i++)
        {
            cardInfo.dropDownMenu.archetype.Add(new Dropdown.OptionData(cardInfo.parseArchList[i].archetype_name));
        }
        if (req != null)
            req.downloadHandler.text.WriteStringToFile(SaveManager.rootDirectory, "archetypes", SaveManager.parameterFileType);
            //saveManager.WriteFile("archetypes", req.downloadHandler.text);
    }

    #endregion


    #region MoveToAPI

    


    //Called by button
    public void RequestCheck()
    {
        SetID();

        if (cardID != "")
        {
            if (cardID != null)
            {
                //Card Information
                if (EUS.sceneIndex == 2)
                {
                   
                }
            }
        }
       
    }

    

    #endregion





}
