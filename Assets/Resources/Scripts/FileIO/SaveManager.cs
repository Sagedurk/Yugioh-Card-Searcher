using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using System.Xml.Serialization;

//Handles Saving and Loading of data
public class SaveManager : MonoBehaviour
{
    GameObject cardInfoHolder;
    CardInfo cardInfo;
    StreamWriter writer;
    public DropdownHandler parameterDropdown;
    public readonly string paramIndex0 = "val0";
    public readonly string paramIndex1 = "val1";
    public readonly string paramIndex2 = "val2";
    public readonly string paramIndex3 = "val3";
    public readonly string paramIndex4 = "val4";
    public readonly string paramIndex5 = "val5";
    public readonly string paramIndex6 = "val6";
    public readonly string paramIndex7 = "val7";
    public readonly string paramIndex8 = "val8";
    public readonly string paramIndex9 = "val9";
    public readonly string paramIndex10 = "val10";
    public readonly string paramIndex11 = "val11";
    public readonly string paramIndex12 = "val12";
    public readonly string paramIndex13 = "val13";
    public readonly string paramIndex14 = "val14";
    public readonly string paramIndex15 = "val15";
    public readonly string paramIndex16 = "val16";
    public readonly string paramIndex17 = "val17";
    public readonly string paramIndex18 = "val18";
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
    void Start()
    {
        if (EUS.sceneIndex != 0)
        {
            cardInfoHolder = GameObject.FindGameObjectWithTag("CardInfoHandler");
            cardInfo = cardInfoHolder.GetComponent<CardInfo>();
        }
        if (EUS.sceneIndex == 1)
        {
            LoadPrefs(Mod_Spec);
            LoadData();
            if (_id != "")
            {
                _trueID = _id.Remove(0,4);
                StartCoroutine(cardInfo.LoadImage(int.Parse(_trueID), cardInfo.image));
            }
        }  
        else if (EUS.sceneIndex == 2)
        {
            _cardInput = PlayerPrefs.GetString(Mod_Gen + cardInput);
            cardInfo.idInputField.text = _cardInput;
            cardInfo.cardID = _cardInput;
            LoadPrefs(Mod_Gen);
            LoadData();
            StartCoroutine(LoadParameters());
        }
        else if (EUS.sceneIndex == 3)
        {
            LoadPrefs(Mod_Random);
            LoadData();
            if (_id != "")
            {
                _trueID = _id.Remove(0, 4);
                StartCoroutine(cardInfo.LoadImage(int.Parse(_trueID), cardInfo.image));
            }
        }
        else if (EUS.sceneIndex == 4)
        {
            LoadPrefs(Mod_Arch);
            LoadData();
        }
    }

    void LoadData()
    {
        cardInfo.id.text = _id;
        cardInfo.cardName.text = _cardName;
        cardInfo.cardType.text = _cardType;
        cardInfo.monsterType.text = _monsterType;
        cardInfo.atk.text = _atk;
        cardInfo.def.text = _def;
        cardInfo.level.text = _level;
        cardInfo.attribute.text = _attribute;
        cardInfo.pendulumScale.text = _pendulumScale;
        cardInfo.archetype.text = _archetype;
        cardInfo.desc.text = _desc;
        SetCardInfoTextPos();
        StartCoroutine(cardInfo.CardInfoPosition());
    }

    void PreSaveState()
    {
        cardInfoHolder = GameObject.FindGameObjectWithTag("CardInfoHandler");
        cardInfo = cardInfoHolder.GetComponent<CardInfo>();
        _id = cardInfo.id.text;
        _cardName = cardInfo.cardName.text;
        _cardType = cardInfo.cardType.text;
        _monsterType = cardInfo.monsterType.text;
        _atk = cardInfo.atk.text;
        _def = cardInfo.def.text;
        _level = cardInfo.level.text;
        _attribute = cardInfo.attribute.text;
        _pendulumScale = cardInfo.pendulumScale.text;
        _archetype = cardInfo.archetype.text;
        _desc = cardInfo.desc.text;
    }

    public void SaveData()
    {
        PreSaveState();
        if (EUS.sceneIndex == 1)
            SetPrefs(Mod_Spec);
        else if (EUS.sceneIndex == 2)
        {
            _cardInput = cardInfo.idInput.text;
            PlayerPrefs.SetInt("parameterStartVal", parameterDropdown.parameters.value);
            PlayerPrefs.SetString(Mod_Gen + cardInput, _cardInput);
            SetPrefs(Mod_Gen);
        }
        else if (EUS.sceneIndex == 3)
            SetPrefs(Mod_Random);
        else if (EUS.sceneIndex == 4)
            SetPrefs(Mod_Arch);
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
            if (cardInfo.cardType.text.Contains("Pendulum"))
                cardInfo.archetype.rectTransform.anchoredPosition = new Vector2(0,cardInfo.cardName.rectTransform.anchoredPosition.y * 9);
            else
                cardInfo.archetype.rectTransform.anchoredPosition = new Vector2(0, cardInfo.cardName.rectTransform.anchoredPosition.y * 8);
        }
        else
            cardInfo.archetype.rectTransform.anchoredPosition = new Vector2(0, cardInfo.cardName.rectTransform.anchoredPosition.y * 4);
        if (cardInfo.archetype.text != "")
            cardInfo.desc.rectTransform.anchoredPosition = cardInfo.archetype.rectTransform.anchoredPosition - new Vector2(0, 25);
        else
            cardInfo.desc.rectTransform.anchoredPosition = cardInfo.archetype.rectTransform.anchoredPosition;
        cardInfo.desc.rectTransform.anchoredPosition = cardInfo.archetype.rectTransform.anchoredPosition - new Vector2(0, -cardInfo.cardName.rectTransform.anchoredPosition.y);
    }
    public void WriteFile(string path, string content)
    {
        writer = new StreamWriter(Application.persistentDataPath + "/" + path.ToLower() + ".txt", true);
        writer.Write(content);
        writer.Close();
    }

    public void ReadFile(string path)
    {
        fileData = File.ReadAllText(Application.persistentDataPath + "/" + path.ToLower() + ".txt");
    }

    public void ReplaceFileWithNew(string path, string content)
    {
        File.Delete(Application.persistentDataPath + "/" + path.ToLower() + ".txt");
        WriteFile(path, content);
    }

    private IEnumerator LoadParameters()   //Load the search parameters and their values (i.e. format, Rush Duel) in scene[2]
    {
        for (int i = 1; i < 20; i++)
        {
            parameterDropdown.parameterValues.ClearOptions();
            parameterDropdown.parameters.value = i;
            switch (i)
            {
                case 0:
                    parameterDropdown.parameterValues.GetComponent<Dropdown>().interactable = false;
                    break;
                case 1:
                    parameterDropdown.parameterValues.value = PlayerPrefs.GetInt(paramIndex0);
                    parameterDropdown.DropdownParamValues(PlayerPrefs.GetInt(paramIndex0));
                    break;
                case 2:
                    parameterDropdown.parameterValues.value = PlayerPrefs.GetInt(paramIndex1);
                    parameterDropdown.DropdownParamValues(PlayerPrefs.GetInt(paramIndex1));
                    break;
                case 3:
                    parameterDropdown.parameterValues.value = PlayerPrefs.GetInt(paramIndex2);
                    parameterDropdown.DropdownParamValues(PlayerPrefs.GetInt(paramIndex2));
                    break;
                case 4:
                    parameterDropdown.parameterValues.value = PlayerPrefs.GetInt(paramIndex3);
                    parameterDropdown.DropdownParamValues(PlayerPrefs.GetInt(paramIndex3));
                    break;
                case 5:
                    parameterDropdown.parameterValues.value = PlayerPrefs.GetInt(paramIndex4);
                    parameterDropdown.DropdownParamValues(PlayerPrefs.GetInt(paramIndex4));
                    break;
                case 6:
                    parameterDropdown.parameterValues.value = PlayerPrefs.GetInt(paramIndex5);
                    parameterDropdown.DropdownParamValues(PlayerPrefs.GetInt(paramIndex5));
                    break;
                case 7:
                    parameterDropdown.parameterValues.value = PlayerPrefs.GetInt(paramIndex6);
                    parameterDropdown.DropdownParamValues(PlayerPrefs.GetInt(paramIndex6));
                    break;
                case 8:
                    parameterDropdown.parameterValues.value = PlayerPrefs.GetInt(paramIndex7);
                    parameterDropdown.DropdownParamValues(PlayerPrefs.GetInt(paramIndex7));
                    break;
                case 9:
                    parameterDropdown.parameterValues.value = PlayerPrefs.GetInt(paramIndex8);
                    parameterDropdown.DropdownParamValues(PlayerPrefs.GetInt(paramIndex8));
                    break;
                case 10:
                    parameterDropdown.parameterValues.value = PlayerPrefs.GetInt(paramIndex9);
                    parameterDropdown.DropdownParamValues(PlayerPrefs.GetInt(paramIndex9));
                    break;
                case 11:
                    //Debug.Log();
                    parameterDropdown.parameterValues.value = PlayerPrefs.GetInt(paramIndex10);
                    parameterDropdown.DropdownParamValues(PlayerPrefs.GetInt(paramIndex10));
                    break;
                case 12:
                    parameterDropdown.parameterValues.value = PlayerPrefs.GetInt(paramIndex11);
                    parameterDropdown.DropdownParamValues(PlayerPrefs.GetInt(paramIndex11));
                    break;
                case 13:
                    parameterDropdown.parameterValues.value = PlayerPrefs.GetInt(paramIndex12);
                    parameterDropdown.DropdownParamValues(PlayerPrefs.GetInt(paramIndex12));
                    break;
                case 14:
                    parameterDropdown.parameterValues.value = PlayerPrefs.GetInt(paramIndex13);
                    parameterDropdown.DropdownParamValues(PlayerPrefs.GetInt(paramIndex13));
                    break;
                case 15:
                    parameterDropdown.parameterValues.value = PlayerPrefs.GetInt(paramIndex14);
                    parameterDropdown.DropdownParamValues(PlayerPrefs.GetInt(paramIndex14));
                    break;
                case 16:
                    yield return StartCoroutine(cardInfo.GetCardSet());
                    parameterDropdown.DropdownParams(i);
                    parameterDropdown.parameterValues.value = PlayerPrefs.GetInt(paramIndex15);
                    //parameterDropdown.DropdownParamValues(PlayerPrefs.GetInt(paramIndex15));
                    break;
                case 17:
                    yield return StartCoroutine(cardInfo.GetArchetypes());
                    parameterDropdown.DropdownParams(i);
                    parameterDropdown.parameterValues.value = PlayerPrefs.GetInt(paramIndex16);
                    //parameterDropdown.DropdownParamValues(PlayerPrefs.GetInt(paramIndex16));
                    break;
                case 18:
                    parameterDropdown.parameterValues.value = PlayerPrefs.GetInt(paramIndex17);
                    parameterDropdown.DropdownParamValues(PlayerPrefs.GetInt(paramIndex17));
                    break;
                case 19:
                    parameterDropdown.parameterValues.value = PlayerPrefs.GetInt(paramIndex18);
                    parameterDropdown.DropdownParamValues(PlayerPrefs.GetInt(paramIndex18));
                    break;
                default:
                    break;
            }
            
        }
        parameterDropdown.parameters.value = PlayerPrefs.GetInt("parameterStartVal");
        
        if ( !CheckIfOnlySpaces(_cardInput)|| !CheckIfOnlySpaces(parameterDropdown.card._URLMod))
        {
            Debug.Log( "URLMOD LOAD" + _cardInput + cardInfo._URLMod);
            cardInfo.PrepToFileName(cardInfo.cardID);
            Debug.Log(cardInfo.fileName.ToLower() + cardInfo._URLMod.ToLower() + " search" + ".txt");
            if (System.IO.File.Exists(Application.persistentDataPath + "/" + cardInfo.fileName.ToLower() + cardInfo._URLMod.ToLower() + " search" + ".txt"))
            {
                Debug.Log("yes");
                ReadFile(cardInfo.fileName + cardInfo._URLMod + " search");
                cardInfo.ResetPrefab();
                cardInfo.ClearCardInfo(false, cardInfo.errorText);
                StartCoroutine(cardInfo.LoadCardInfo(null, cardInfo.saveManager.fileData));
            }
        }
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

}
