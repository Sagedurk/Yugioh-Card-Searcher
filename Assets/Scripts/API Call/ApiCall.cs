﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEditor;
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

    public ApiTypes apiType;
    public LoadTypes loadType;

    public UnityWebRequest webRequest;

    public ArchetypeParse[] archetypes;

    Coroutine CardSearchResultsCoroutine;

    protected const string URL = "db.ygoprodeck.com/api/v7/", endpointCard = "cardinfo.php?", endpointArchetype = "archetypes.php", endpointCardSet = "cardsets.php";
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
        ApiCall instance = TryGetInstance();
        if (instance)
        {
            instance.apiType = apiType;
            instance.cardInfo = cardInfo;
            instance.cardSearch = cardSearch;

            if (instance.cardInfo)
                instance.cardInfo.SetSubmitButtonListener();

            if (instance.cardSearch)
                instance.cardSearch.SetSubmitButtonListener();
        }

        base.Awake();

        if(Instance.cardInfo)
            Instance.cardInfo.SetSubmitButtonListener();


        if (Instance.cardSearch)
            Instance.cardSearch.SetSubmitButtonListener();
    }


    public void Execute()
    {

        SetID();
     
        //Needs to be redone to work with more than Card Info
        if (cardID == null)
            return;

        ErrorManager.Instance.ClearError();

        switch (apiType)
        {
            case ApiTypes.CARD_INFO:

                if(cardID != null)
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


    private IEnumerator CardInfoExecute()
    {
        if (cardID == "")
            yield break;

        cardName = cardID;

        
        if (int.TryParse(cardID, out int id))
        {
            CardID_LUT id_LUT = CardID_LUT.TryGetLUT(id);

            //Fetch card name from API if ID_LUT doesn't exist yet
            if (id_LUT == null)
            {
                webRequest = UnityWebRequest.Get(URL + endpointCard + "id=" + cardID);
                yield return StartCoroutine(APIRequestID());
            }
            else
            {
                cardName = id_LUT.GetName();
            }
            
        }

        string cardFileName = cardName.ConvertToValidFileName();
        CardInfoParse loadedData = SaveManager.Instance.TryGetSavedCard(cardFileName);


        //Get data from local file
        if (loadedData != null)
        {
            Debug.Log("Load card data!");
            loadType = LoadTypes.FILE;

            string loadedDataJson = JsonParser.ToJson(loadedData);
            SaveManager.Instance.fileData = loadedDataJson;

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
        string searchQuery = cardID.ConvertToValidFileName();
        string fileName = searchQuery + dropdownUrlMod.ConvertToValidFileName();
        string fileContent = SaveManager.ReadFileAsString(SaveManager.parameterDirectory, fileName, SaveManager.parameterFileType);


        if(CardSearchResultsCoroutine != null)
        {
            StopCoroutine(CardSearchResultsCoroutine);
            CardSearchResultsCoroutine = null;
        }

        cardSearch.DestroySearchResults();

        if (fileContent != ""){

            SaveManager.Instance.fileData = fileContent;
            loadType = LoadTypes.FILE;
            StartCoroutine(LoadCardSearch());
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(cardID) || !string.IsNullOrWhiteSpace(dropdownUrlMod))
            {
                webRequest = UnityWebRequest.Get(URL + endpointCard + "fname=" + cardID + dropdownUrlMod);
                //Debug.Log(URL + endpointCard + "fname=" + cardID + dropdownUrlMod);
                StartCoroutine(APIRequest());
            }
            else
            {
                //Add errortext
            }
        }

    }

    public IEnumerator APIRequestID()
    {
        Debug.Log("ID First Pass!");
        yield return webRequest.SendWebRequest();

        if (webRequest.downloadHandler.text.Contains("No card matching your query was found in the database."))
            OnApiNoDataFound();

        else if (webRequest.downloadHandler.text.Contains("\"error\":"))
            OnApiError();

            yield return webRequest.downloadHandler.text;
        string jsonData = webRequest.downloadHandler.text;

  
        CardInfoParse[] cards = JsonParser.FromJson<CardInfoParse>(jsonData);
        cardName = cards[0].name;
    }

    public IEnumerator APIRequest()
    {
        yield return webRequest.SendWebRequest();

        //If there is a connection error to the API, stop
        //TODO: Set up some error message if connection error happens
        if (webRequest.result == UnityWebRequest.Result.ConnectionError)
            yield break;

        #region API Responses
        if (webRequest.downloadHandler.text.Contains("No card matching your query was found in the database."))
            OnApiNoDataFound();

        else if (webRequest.downloadHandler.text.Contains("\"error\":"))
            OnApiError();

        else
            OnApiSuccess();
        #endregion

    }
    
    #region API Data Managing
    void OnApiNoDataFound()
    {
        ErrorManager.Instance.SetError("No matching card was found.");
    }

    void OnApiError()
    {
        ErrorManager.Instance.SetError("An API error has occurred.");
    }

    void OnApiSuccess()
    {
        loadType = LoadTypes.API;

       
        //Load data based on the API type set
        switch (apiType)
        {
            case ApiTypes.CARD_INFO:
                
                StartCoroutine(LoadCardInfo());
                break;
            case ApiTypes.CARD_SEARCH:
                

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
                yield return SaveManager.Instance.fileData;
                jsonData = SaveManager.Instance.fileData;
                break;
        }

        cardInfo.fetchedCards = JsonParser.FromJson<CardInfoParse>(jsonData);
        #endregion

        CardInfoParse card = cardInfo.fetchedCards[0];

        SaveAPIData(cardInfo.fetchedCards);
        SaveManager.CreateID_LUTs(card);
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

                SaveManager.SaveSearchData(jsonData, cardID + dropdownUrlMod);
                break;

            case LoadTypes.FILE:
                yield return SaveManager.Instance.fileData;
                jsonData = SaveManager.Instance.fileData;
                break;
        }

        cardSearch.fetchedCards = JsonParser.FromJson<CardInfoParse>(jsonData);
        #endregion
        
        CardSearchResultsCoroutine = StartCoroutine(cardSearch.ConvertData(cardSearch.fetchedCards));
    }

    #endregion


    #region Misc



    
    //TODO: Set Private?

    public void SetID()
    {
        switch (apiType)
        {
            case ApiTypes.CARD_INFO:
                if (cardInfo.idInput != null)
                    cardID = cardInfo.idInput.text;
                break;
            case ApiTypes.CARD_SEARCH:
                if (cardSearch.idInput != null)
                    cardID = cardSearch.idInput.text;
                break;
            case ApiTypes.CARD_RANDOM:
                if (cardInfo.idInput != null)
                    cardID = cardInfo.idInput.text;
                break;
            default:
                break;
        }

    }

    #endregion


    #region File Name Functions
    
    

    void SaveAPIData(CardInfoParse[] cards)
    {
        foreach (CardInfoParse card in cards)
        {
            if (loadType != LoadTypes.API)
                break;

            SaveManager.SaveCard(card);
        }
    }
    #endregion


    #region Image Functions

    byte[] imageData;

    public IEnumerator TryDownloadImages(string baseUrl, CardInfoParse card, bool overwriteData = false)
    {
        foreach (CardImageParse cardImage in card.card_images)
        {
            ImageData imageDataInstance = ImageData.TryGetImageData(cardImage.id);


            if (!overwriteData && imageDataInstance != null && imageDataInstance.HasImages())
                continue;

            else if (imageDataInstance == null)
                imageDataInstance = ImageData.CreateImageData(cardImage.id);
    

            yield return TryDownloadImage(baseUrl + "cards/", ImageTypes.LARGE, imageDataInstance, overwriteData);
            imageDataInstance.SetImage(ImageTypes.LARGE, imageData);

            yield return TryDownloadImage(baseUrl + "cards_small/", ImageTypes.SMALL, imageDataInstance, overwriteData);
            imageDataInstance.SetImage(ImageTypes.SMALL, imageData);

            //yield return TryDownloadImage(baseUrl + "cards_cropped/", ImageTypes.CROPPED, imageDataInstance, overwriteData);
            //imageDataInstance.SetImage(ImageTypes.CROPPED, imageData);
            
            //Save image data
            StartCoroutine(imageDataInstance.SaveImages());
        }
    }


    private IEnumerator TryDownloadImage(string url, ImageTypes imageType, ImageData instance, bool overwriteData = false)
    {
        if (!overwriteData)
        {
            switch (imageType)
            {
                case ImageTypes.LARGE:
                    imageData = instance.GetLargeImage();
                    break;

                case ImageTypes.SMALL:
                    imageData = instance.GetSmallImage();
                    break;

                case ImageTypes.CROPPED:
                    imageData = instance.GetCroppedImage();
                    break;

                default:
                    break;
            }

            if (imageData != null)
                yield break;
        }


        UnityWebRequest imageDownload = UnityWebRequestTexture.GetTexture(url + instance.id.ToString() + ImageData.fileType);
        yield return imageDownload.SendWebRequest();

        try
        {
            imageData = DownloadHandlerTexture.GetContent(imageDownload).EncodeToJPG(75);
        }
        catch (System.Exception ex)
        {
            Debug.LogWarningFormat("Failed to download image type {0} for card ID {1}: \n{2}", imageType, instance.id, ex.Message);
        }
    }

    public void LoadImage(int id, RawImage img, ImageTypes imageType)
    {
        ImageData imageData = ImageData.TryGetImageData(id);
    
        if(imageData == null)
        {
            img.color = Color.clear;
            img.texture = null;
            return;
        }

        byte[] imageBytes = null;

        switch (imageType)
        {
            case ImageTypes.LARGE:
            imageBytes = imageData.GetLargeImage();
                break;
            case ImageTypes.SMALL:
            imageBytes = imageData.GetSmallImage();
                break;
            case ImageTypes.CROPPED:
            imageBytes = imageData.GetCroppedImage();
                break;
        }

        Texture2D texture = new Texture2D(1, 1);
        texture.LoadImage(imageBytes);
        
        img.texture = texture;
        img.color = Color.white;

        if (apiType != ApiTypes.CARD_SEARCH)
            cardInfo.ShowImageButtons();
    }

    public IEnumerator UpdateImages()
    {
        UpdateData.Instance.updateProgressText.text = "0.00%";
        UpdateData.Instance.BlockUIInteraction();

        float percentageProgress = 0;

        string[] imageFilePaths = Directory.GetFiles(SaveManager.imageDirectories[0]);

        for (int i = 0; i < imageFilePaths.Length; i++)
        {
            string fileName = Path.GetFileNameWithoutExtension(imageFilePaths[i]);

            if (int.TryParse(fileName, out int imageID))
            {
                Debug.Log("Parse Successful: " + imageID);
                ImageData imageData = ImageData.CreateImageData(imageID);

                yield return StartCoroutine(TryDownloadImage(imageURL + "cards/", ImageTypes.LARGE, imageData, true));   
                yield return StartCoroutine(TryDownloadImage(imageURL + "cards_small/", ImageTypes.SMALL, imageData, true));

                yield return StartCoroutine(imageData.SaveImages(true));
            }

            percentageProgress = (float)i / imageFilePaths.Length * 100;

            UpdateData.Instance.updateProgressText.text = percentageProgress.ToString("F2") + "%";
        }
        UpdateData.Instance.UnblockUIInteraction();
    }

    #endregion

    public IEnumerator UpdateCards( )
    {
        webRequest = UnityWebRequest.Get(URL + endpointCard);
        yield return StartCoroutine(CardUpdateRequest());
    }
    private IEnumerator CardUpdateRequest()
    {
        UpdateData.Instance.updateProgressText.text = "0%";
        UpdateData.Instance.BlockUIInteraction();

        yield return webRequest.SendWebRequest();
        yield return webRequest.downloadHandler.text;


        string jsonData = webRequest.downloadHandler.text;

        CardInfoParse[] cardData = JsonParser.FromJson<CardInfoParse>(jsonData);

        List<CardInfoParse> updatedCards = new List<CardInfoParse>();
        float percentageProgress = 0;
        int cardsUpdated = 0;


        foreach (CardInfoParse card in cardData)
        {
            string cardFileName = card.name.ConvertToValidFileName();

            if (!File.Exists(SaveManager.cardDirectory + cardFileName + SaveManager.cardFileType))
            {
                cardsUpdated++;
                continue;
            }

            Debug.Log("card Updated");

            SaveManager.SaveCard(card, true);
            updatedCards.Add(card);

            yield return new WaitForSeconds(0.0025f);

            percentageProgress = (float)cardsUpdated / cardData.Length * 100;
            UpdateData.Instance.updateProgressText.text = percentageProgress.ToString("F0") + "%";

            cardsUpdated++;
        }
        UpdateData.Instance.UnblockUIInteraction();
        CardID_LUT.UpdateLUTs(updatedCards);
    }

    public IEnumerator UpdateSearchData()
    {
        UpdateData.Instance.updateProgressText.text = "0%";
        UpdateData.Instance.BlockUIInteraction();
        int updateCounter = 0;

        string[] files = Directory.GetFiles(SaveManager.parameterDirectory);

        float percentageProgress = 0;

        foreach (string file in files)
        {
            string fileName = Path.GetFileNameWithoutExtension(file);
            fileName = fileName.ConvertFromFileName();

            webRequest = UnityWebRequest.Get(URL + endpointCard + fileName);
            yield return StartCoroutine(SearchUpdateRequest(fileName));

            updateCounter++;
            percentageProgress = (float)updateCounter / files.Length * 100;
            UpdateData.Instance.updateProgressText.text = percentageProgress.ToString("F0") + "%";
            
            //Due to a new API call happening each iteration,
            //fastest rate allowed is 20 iterations per second
            yield return new WaitForSeconds(0.066f);    //0.066 = 1/15
        }

        UpdateData.Instance.UnblockUIInteraction();
    }

    public IEnumerator SearchUpdateRequest(string fileName)
    {
        yield return webRequest.SendWebRequest();
        yield return webRequest.downloadHandler.text;

        webRequest.downloadHandler.text.WriteStringToFile(SaveManager.parameterDirectory, fileName, SaveManager.parameterFileType, true);
        Debug.Log(webRequest.downloadHandler.text);
    }

    

    #region CardSets
    private IEnumerator CardSetRequest()
    {
        UpdateData.Instance.updateProgressText.text = "0%";
        UpdateData.Instance.BlockUIInteraction();

        yield return webRequest.SendWebRequest();
        yield return webRequest.downloadHandler.text;

        string json = "{\"data\":" + webRequest.downloadHandler.text + "}";

        CardSetInfo[] cardSets = JsonParser.FromJson<CardSetInfo>(json);

        string cardSetFilePath = "card set";

        DropdownParameterParse dropdownData = new DropdownParameterParse().TryReadFileToClass(SaveManager.parameterValuesDirectory, cardSetFilePath, SaveManager.parameterValuesFileType);

        float percentageProgress = 0;

        dropdownData.options = new string[cardSets.Length];
        for (int i = 0; i < cardSets.Length; i++)
        {
            dropdownData.options[i] = cardSets[i].set_name;

            percentageProgress = (float)i / cardSets.Length * 100;
            UpdateData.Instance.updateProgressText.text = percentageProgress.ToString("F0") + "%";

            yield return new WaitForSeconds(0.0025f);
        }

        dropdownData.TryWriteClassToFile(SaveManager.parameterValuesDirectory, cardSetFilePath, SaveManager.parameterValuesFileType, true);
        UpdateData.Instance.UnblockUIInteraction();
    }

    public IEnumerator UpdateCardSets()
    {
        webRequest = UnityWebRequest.Get(URL + endpointCardSet);                  //Randomcard
        yield return StartCoroutine(CardSetRequest());
    }


    #endregion



    #region Archetypes

    public IEnumerator UpdateArchetypeList()
    {
        webRequest = UnityWebRequest.Get(URL + endpointArchetype);
        yield return StartCoroutine(ArchetypeRequest());
    }

    public IEnumerator ArchetypeRequest()
    {
        UpdateData.Instance.updateProgressText.text = "0%";
        UpdateData.Instance.BlockUIInteraction();

        yield return webRequest.SendWebRequest();
        yield return webRequest.downloadHandler.text;

        string json = "{\"data\":" + webRequest.downloadHandler.text + "}";
        archetypes = JsonParser.FromJson<ArchetypeParse>(json);

        string archetypeFilePath = "archetype";

        DropdownParameterParse dropdownData = new DropdownParameterParse().TryReadFileToClass(SaveManager.parameterValuesDirectory, archetypeFilePath, SaveManager.parameterValuesFileType);

        float percentageProgress = 0;

        dropdownData.options = new string[archetypes.Length];
        for (int i = 0; i < archetypes.Length; i++)
        {
            dropdownData.options[i] = archetypes[i].archetype_name;

            percentageProgress = (float)i / archetypes.Length * 100;
            UpdateData.Instance.updateProgressText.text = percentageProgress.ToString("F0") + "%";
            yield return new WaitForSeconds(0.01f);
        }

        dropdownData.TryWriteClassToFile(SaveManager.parameterValuesDirectory, archetypeFilePath, SaveManager.parameterValuesFileType, true);
        UpdateData.Instance.UnblockUIInteraction();
    }

    #endregion


}
