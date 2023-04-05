using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using System.Xml.Serialization;
using System;


//Handles Saving and Loading of data
public class SaveManager : EUS.Cat_Systems.Singleton<SaveManager>
{
    public static string cardFileType = ".json";
    public static string idLutFileType= ".txt"; 
    public static string imageFileType = ".jpg";
    public static string parameterFileType = ".txt";
    public static string parameterValuesFileType = ".json";

    public static string rootDirectory;
    public static string cardDirectory;
    public static string[] imageDirectories;
    public static string idLutDirectory;
    public static string parameterDirectory;
    public static string parameterValuesDirectory;
    

    
    public readonly string[] parameterIndices = new string[19];

    ApiCall apiCall;

    public string fileData;
    readonly string cardInput = "CardInput";
    readonly string id = "ID";
    readonly string cardName = "cName";
    readonly string cardType = "cType";
    readonly string monsterType = "mType";
    readonly string atk = "ATK";
    readonly string def = "DEF";
    readonly string level = "lvl";
    readonly string attribute = "attr";
    readonly string pendulumScale = "scale";
    readonly string archetype = "archetype";
    readonly string desc = "desc";
    readonly string Mod_Spec = "S_";
    readonly string Mod_Gen = "G_";
    readonly string Mod_Random = "R_";
    readonly string Mod_Arch = "A_";
    string _cardInput;
    string _id;
    string _trueID;
    string _cardName;
    string _cardType;
    string _monsterType;
    string _atk;              
    string _def;
    string _level;
    string _attribute;
    string _pendulumScale;
    string _archetype;
    string _desc;
    bool whileCoroutine = false;
    public bool isCoroutinePlaying = false;



    protected override void Awake()
    {
        base.Awake();

        SetDirectories();
        //DebugDirectories();
        TryCreateDirectories();
    }

    void Start()
    {
        apiCall = ApiCall.Instance;
        
        
        switch (apiCall.apiType)
        {
            case ApiCall.ApiTypes.CARD_INFO:
                
                LoadPrefs(Mod_Spec);
                LoadData();
                if (_id != "")
                {
                    _trueID = _id.Remove(0, 4);
                    apiCall.LoadImage(int.Parse(_trueID), apiCall.cardInfo.image, ApiCall.ImageTypes.LARGE);
                }

                break;
            case ApiCall.ApiTypes.CARD_SEARCH:

                SetParameterIndices();
                _cardInput = PlayerPrefs.GetString(Mod_Gen + cardInput);
                apiCall.cardSearch.idInputField.text = _cardInput;
                apiCall.cardID = _cardInput;
                LoadPrefs(Mod_Gen);
                //LoadData();
                StartCoroutine(LoadParameters());

                break;
            case ApiCall.ApiTypes.CARD_RANDOM:

                LoadPrefs(Mod_Random);
                LoadData();
                if (_id != "")
                {
                    _trueID = _id.Remove(0, 4);
                    apiCall.LoadImage(int.Parse(_trueID), apiCall.cardInfo.image, ApiCall.ImageTypes.LARGE);
                }

                break;
            default:
                break;
        }


        if (EUS.sceneIndex == 4)
        {
            LoadPrefs(Mod_Arch);
            LoadData();
        }
    }


    #region Saving


    #region PreSaving

    void PreSaveInfo()
    {
        _id = apiCall.cardInfo.id.text;
        _cardName = apiCall.cardInfo.cardName.text;
        _cardType = apiCall.cardInfo.cardType.text;
        _monsterType = apiCall.cardInfo.monsterType.text;
        _atk = apiCall.cardInfo.atk.text;
        _def = apiCall.cardInfo.def.text;
        _level = apiCall.cardInfo.level.text;
        _attribute = apiCall.cardInfo.attribute.text;
        _pendulumScale = apiCall.cardInfo.pendulumScale.text;
        _archetype = apiCall.cardInfo.archetype.text;
        _desc = apiCall.cardInfo.desc.text;
    }

    void PreSaveSearch()
    {
        
        //_id = apiCall.cardSearch.id.text;
        //_cardName = apiCall.cardSearch.cardName.text;
        //_cardType = apiCall.cardSearch.cardType.text;
        //_monsterType = apiCall.cardSearch.monsterType.text;
        //_atk = apiCall.cardSearch.atk.text;
        //_def = apiCall.cardSearch.def.text;
        //_level = apiCall.cardSearch.level.text;
        //_attribute = apiCall.cardSearch.attribute.text;
        //_pendulumScale = apiCall.cardSearch.pendulumScale.text;
        //_archetype = apiCall.cardSearch.archetype.text;
        //_desc = apiCall.cardSearch.desc.text;
    }

    #endregion

    public void SaveData()
    {
        switch (apiCall.apiType)
        {
            case ApiCall.ApiTypes.CARD_INFO:
                PreSaveInfo();
                SetPrefs(Mod_Spec);
                break;
                
            case ApiCall.ApiTypes.CARD_SEARCH:
                
                PreSaveSearch();
                _cardInput = apiCall.cardInfo.idInput.text;
                PlayerPrefs.SetInt("parameterStartVal", DropdownHandler.Instance.primaryDropdown.value);
                PlayerPrefs.SetString(Mod_Gen + cardInput, _cardInput);
                SetPrefs(Mod_Gen);
                break;
                
            case ApiCall.ApiTypes.CARD_RANDOM:
                PreSaveInfo();
                SetPrefs(Mod_Random);
                break;
                
            default:
                break;
        }

        //else if (EUS.sceneIndex == 4)
        //    SetPrefs(Mod_Arch);

    }

    void SetPrefs(string Modifier)
    {
        PlayerPrefs.SetString(Modifier + id, _id);
        PlayerPrefs.SetString(Modifier + cardName, _cardName);
        PlayerPrefs.SetString(Modifier + cardType, _cardType);
        PlayerPrefs.SetString(Modifier + monsterType, _monsterType);
        PlayerPrefs.SetString(Modifier + atk, _atk);
        PlayerPrefs.SetString(Modifier + def, _def);
        PlayerPrefs.SetString(Modifier + level, _level);
        PlayerPrefs.SetString(Modifier + attribute, _attribute);
        PlayerPrefs.SetString(Modifier + pendulumScale, _pendulumScale);
        PlayerPrefs.SetString(Modifier + archetype, _archetype);
        PlayerPrefs.SetString(Modifier + desc, _desc);
    }
    



    #region ID_LUT

    public static void CreateID_LUTs(CardInfoParse card)
    {
        CardID_LUT.TryCreateLUTs(card);
    }

    public void CreateID_LUTs(CardInfoParse[] cards)
    {
        foreach (CardInfoParse card in cards)
        {
            CardID_LUT.TryCreateLUTs(card);
        }
    }

    public IEnumerator OverwriteID_LUTs(CardInfoParse card)
    {
        yield return null;
        //yield return StartCoroutine(ApiCall.Instance.TryDownloadImages(ApiCall.imageURL, card));
    }

    public IEnumerator OverwriteID_LUTs(CardInfoParse[] cards)
    {
        foreach (CardInfoParse card in cards)
        {
            yield return StartCoroutine(OverwriteID_LUTs(card));
        }
    }

    #endregion

    #region CardData

    public CardInfoParse TryGetSavedCard(string cardName)
    {
        if (File.Exists(cardDirectory + cardName + cardFileType))
        {
            CardInfoParse[] loadedData = ReadFile<CardInfoParse>(cardDirectory, cardName, cardFileType);
            return loadedData[0];
        }

        return null;
    }

    public static void SaveCard(CardInfoParse card, bool isOverwriting = false)
    {
        string cardFileName = card.name.ConvertToValidFileName();
        card.TryWriteClassToFile(cardDirectory, cardFileName, cardFileType, isOverwriting);
    }

    #endregion


    #endregion

    #region Loading

    void LoadData()
    {

        apiCall.cardInfo.id.SetText(_id);
        apiCall.cardInfo.cardName.SetText(_cardName);
        apiCall.cardInfo.cardType.SetText(_cardType);
        apiCall.cardInfo.monsterType.SetText(_monsterType);
        apiCall.cardInfo.atk.SetText(_atk);
        apiCall.cardInfo.def.SetText(_def);
        apiCall.cardInfo.level.SetText(_level);
        apiCall.cardInfo.attribute.SetText(_attribute);
        apiCall.cardInfo.pendulumScale.SetText(_pendulumScale);
        apiCall.cardInfo.archetype.SetText(_archetype);
        apiCall.cardInfo.desc.SetText(_desc);
        SetCardInfoTextPos();
        StartCoroutine(apiCall.cardInfo.ResizeTransform());
    }
    
    void LoadPrefs(string Modifier)
    {
        _id = PlayerPrefs.GetString(Modifier + id);
        _cardName = PlayerPrefs.GetString(Modifier + cardName);
        _cardType = PlayerPrefs.GetString(Modifier + cardType);
        _monsterType = PlayerPrefs.GetString(Modifier + monsterType);
        _atk = PlayerPrefs.GetString(Modifier + atk);
        _def = PlayerPrefs.GetString(Modifier + def);
        _level = PlayerPrefs.GetString(Modifier + level);
        _attribute = PlayerPrefs.GetString(Modifier + attribute);
        _pendulumScale = PlayerPrefs.GetString(Modifier + pendulumScale);
        _archetype = PlayerPrefs.GetString(Modifier + archetype);
        _desc = PlayerPrefs.GetString(Modifier + desc);
    }
    void SetCardInfoTextPos()
    {
        if (_cardType != "Card Type: Spell Card" && _cardType != "Card Type: Trap Card")
        {
            if (apiCall.cardInfo.cardType.text.Contains("Pendulum"))
                apiCall.cardInfo.archetype.rectTransform.anchoredPosition = new Vector2(0,apiCall.cardInfo.cardName.rectTransform.anchoredPosition.y * 9);
            else
                apiCall.cardInfo.archetype.rectTransform.anchoredPosition = new Vector2(0, apiCall.cardInfo.cardName.rectTransform.anchoredPosition.y * 8);
        }
        else
            apiCall.cardInfo.archetype.rectTransform.anchoredPosition = new Vector2(0, apiCall.cardInfo.cardName.rectTransform.anchoredPosition.y * 4);
        if (apiCall.cardInfo.archetype.text != "")
            apiCall.cardInfo.desc.rectTransform.anchoredPosition = apiCall.cardInfo.archetype.rectTransform.anchoredPosition - new Vector2(0, 25);
        else
            apiCall.cardInfo.desc.rectTransform.anchoredPosition = apiCall.cardInfo.archetype.rectTransform.anchoredPosition;
        apiCall.cardInfo.desc.rectTransform.anchoredPosition = apiCall.cardInfo.archetype.rectTransform.anchoredPosition - new Vector2(0, -apiCall.cardInfo.cardName.rectTransform.anchoredPosition.y);
    }
    public static string ReadFileAsString(string directory, string fileName, string fileType)
    {
        string fileContent = "";
        string fullPath = directory + fileName.ToLower() + fileType;

        if (File.Exists(fullPath))
        {
            fileContent = File.ReadAllText(fullPath);
        }

        return fileContent;

    }

    public static T[] ReadFile<T> (string directory, string fileName, string fileType)
    {
        try
        {
            string jsonData = File.ReadAllText(directory + fileName.ToLower() + fileType);
            T[] convertedData = JsonParser.FromJson<T>(jsonData);

            return convertedData;

        }
        catch (Exception)
        {
            return null;
        }

    }

    private IEnumerator LoadParameters()   //Load the card search parameters and their values 
    {
        yield return null;
        for (int i = 1; i < parameterIndices.Length; i++)
        {
            DropdownHandler.Instance.secondaryDropdown.ClearOptions();
            DropdownHandler.Instance.primaryDropdown.value = i;

            if(i == 0)
            {
                DropdownHandler.Instance.secondaryDropdown.interactable = false;
                continue;
            }

            //else if (i == (int)DropdownHandler.DropOptions.CARD_SET)
            //{
            //    yield return StartCoroutine(apiCall.GetCardSet());
            //    DropdownHandler.Instance.OnChangePrimaryDropdown(i);
            //    DropdownHandler.Instance.secondaryDropdown.value = PlayerPrefs.GetInt(parameterIndices[i - 1]);
            //    //parameterDropdown.DropdownParamValues(PlayerPrefs.GetInt(paramIndex15));
            //    continue;
            //}
            //else if (i == (int)DropdownHandler.DropOptions.ARCHETYPE)
            //{
            //    yield return StartCoroutine(apiCall.GetArchetypes());
            //    DropdownHandler.Instance.OnChangePrimaryDropdown(i);
            //    DropdownHandler.Instance.secondaryDropdown.value = PlayerPrefs.GetInt(parameterIndices[i - 1]);
            //    //parameterDropdown.DropdownParamValues(PlayerPrefs.GetInt(paramIndex16));
            //    continue;
            //}

            DropdownHandler.Instance.secondaryDropdown.value = PlayerPrefs.GetInt(parameterIndices[i-1]);
            DropdownHandler.Instance.OnChangeSecondaryDropdown(PlayerPrefs.GetInt(parameterIndices[i - 1]));

        }

        DropdownHandler.Instance.primaryDropdown.value = PlayerPrefs.GetInt("parameterStartVal");
        
        if ( !string.IsNullOrWhiteSpace(_cardInput)|| !string.IsNullOrWhiteSpace(apiCall.dropdownUrlMod))
        {
            string fileNameCardID = apiCall.cardID.ConvertToValidFileName();

            string fileContent = ReadFileAsString(parameterDirectory,fileNameCardID + apiCall.dropdownUrlMod, parameterFileType);

            if (fileContent != "")
            {
                apiCall.cardSearch.ResetPrefab();
                apiCall.cardInfo.ClearTextInfo(apiCall.cardInfo.errorText);

                apiCall.loadType = ApiCall.LoadTypes.FILE;
                StartCoroutine(apiCall.LoadCardSearch());
            } 
        }
    }


    #endregion

    #region Directories

    void SetDirectories()
    {
        SetRootDirectory();
        SetCardDirectory();
        SetIdLutDirectory();
        SetImageDirectory();
        SetParameterDirectory();
        SetParameterValuesDirectory();
    }

    void SetRootDirectory()
    {
        rootDirectory = Application.persistentDataPath + "/";
    }

    void SetCardDirectory()
    {
        cardDirectory = rootDirectory + "Cards/";
    }

    void SetIdLutDirectory()
    {
        idLutDirectory = rootDirectory + "ID_LUTs/";
    }

    void SetImageDirectory()
    {
        imageDirectories = new string[3];

        imageDirectories[0] = rootDirectory + "Images/Large/";
        imageDirectories[1] = rootDirectory + "Images/Small/";
        imageDirectories[2] = rootDirectory + "Images/Cropped/";
    }

    void SetParameterDirectory()
    {
        parameterDirectory = rootDirectory + "Search/";
    }

    void SetParameterValuesDirectory()
    {
        parameterValuesDirectory = rootDirectory + "Parameters/";
    }

    void DebugDirectories()
    {
        Debug.Log("Root: " + rootDirectory);
        Debug.Log("Card: " + cardDirectory);
        Debug.Log("ID LUT: " + idLutDirectory);

        for (int i = 0; i < imageDirectories.Length; i++)
        {
            Debug.Log("Image " + i + ": " + imageDirectories[i]);
        }

        Debug.Log("Parameters: " + parameterDirectory);
    }

    void TryCreateDirectories()
    {
        TryCreateDirectory(cardDirectory);
        TryCreateDirectory(idLutDirectory);

        foreach (string imageDirectory in imageDirectories)
        {
            TryCreateDirectory(imageDirectory);
        }

        TryCreateDirectory(parameterDirectory);
        TryCreateDirectory(parameterValuesDirectory);
    }

    void TryCreateDirectory(string directory)
    {
        if(!Directory.Exists(directory))
            Directory.CreateDirectory(directory);
    }

    #endregion


    #region Misc

    public void OverwriteStringFile(string content, string directory, string fileName, string fileType)
    {
        content.WriteStringToFile(directory, fileName, fileType, true);
    }

    public bool CheckIfOnlySpaces(string text)
    {
        foreach (char c in text)
        {
            if (char.IsWhiteSpace(c) == false)
            {
                return false;
            }

        }
        return true;
        
    }

    public void SetParameterIndices()
    {
        for (int i = 0; i < parameterIndices.Length; i++)
        {
            parameterIndices[i] = "val" + i.ToString();
        }
    }



    #endregion

}
public static class SaveManagerExtensions
{
    static StreamWriter writer;

    public static void TryWriteClassToFile<T>(this T content, string directory, string fileName, string fileType, bool isOverwriting = false)
    {
        string fullPath = directory + fileName.ToLower() + fileType;

        //Check if File exists, and if it does, if it should be overwritten
        if (!isOverwriting && File.Exists(fullPath))
            return;

        //Create/Overwrite File
        string convertedData = JsonParser.ToJson<T>(content);

        try
        {
            using (StreamWriter writer = new StreamWriter(fullPath, false))
            {
                writer.Write(convertedData);
                writer.Close();
            }
        }
        catch (Exception ex)
        {
            throw new Exception(string.Format("Error writing to file: {0}. {1}", fullPath, ex.Message));
        }
    }

    public static void WriteStringToFile(this string content, string directory, string fileName, string fileType, bool isOverwriting = false)
    {

        fileName = fileName.ConvertToValidFileName();
        //Check if File exists, and if it does, if it should be overwritten
        string fullPath = directory + fileName.ToLower() + fileType;

        if (!isOverwriting && File.Exists(fullPath))
            return;

        writer = new StreamWriter(fullPath, false);
        writer.Write(content);
        writer.Close();
    }

    

    public static void SaveImage(this byte[] imageBytes, string directory, string fileName, string fileType, bool isOverwriting = false)
    {
        //Check if File exists, and if it does, if it should be overwritten
        string fullPath = directory + fileName.ToLower() + fileType;
        
        if (!isOverwriting && File.Exists(fullPath))
            return;

        File.WriteAllBytes(fullPath, imageBytes);
    }

}