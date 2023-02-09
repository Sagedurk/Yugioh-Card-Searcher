using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Reflection;
using UnityEditor;

//Handles the dropdowns (filter parameters and their respective values)
public class DropdownHandler : MonoBehaviour
{
    #region Variables


    public Dropdown primaryDropdown;
    public Dropdown secondaryDropdown;
    [HideInInspector]public List<Dropdown.OptionData> cardtype, atk, def, lvl, type, attribute, linkrating, linkmarker, pendulumscale, cardset, archetype, banlist, format;
    [HideInInspector]public string[] urlParams = new string [19];
    [HideInInspector]public string urlMod;
    [HideInInspector]public DropOptions dropOptions;

    ApiCall apiCall;


    public enum DropOptions
    {
        NONE,
        CARD_TYPE,
        MONSTER_TYPE,
        ATK,
        ATK_LESS,
        ATK_GREATER,
        DEF,
        DEF_LESS,
        DEF_GREATER,
        LEVEL,
        LEVEL_LESS,
        LEVEL_GREATER,
        ATTRIBUTE,
        LINK_RATING,
        LINK_MARKER,
        PENDULUM_SCALE,
        CARD_SET,
        ARCHETYPE,
        BANLIST,
        FORMAT
    }

    #endregion

    void Start()
    {
        apiCall = ApiCall.Instance;
        primaryDropdown = GameObject.FindObjectsOfType<Dropdown>()[0];
        secondaryDropdown = GameObject.FindObjectsOfType<Dropdown>()[1];

        PopulateCardTypeList();
        PopulateAtkAndDefLists();
        PopulateLvlAndScaleLists();
        PopulateTypeList();
        PopulateAttributeList();
        PopulateLinkRatingList();
        PopulateLinkMarkerList();
        PopulateBanList();
        PopulateFormatList();

        //Cardset & Archetype gets populated in [CardInfo] (~ line 460)
    }


    #region Populate Dropdown Lists

    void PopulateCardTypeList()
    {
        string[] options = { "", "Effect Monster", "Flip Effect Monster", "Flip Tuner Effect Monster", "Gemini Monster", 
            "Normal Monster", "Normal Tuner Monster", "Pendulum Effect Monster", "Pendulum Flip Effect Monster", 
            "Pendulum Normal Monster", "Pendulum Tuner Effect Monster", "Ritual Effect Monster", "Ritual Monster", 
            "Skill Card", "Spell Card", "Spirit Monster", "Toon Monster", "Trap Card", "Tuner Monster", "Union Effect Monster", 
            "Fusion Monster", "Link Monster", "Pendulum Effect Fusion Monster", "Synchro Monster", 
            "Synchro Pendulum Effect Monster", "Synchro Tuner Monster", "XYZ Monster", "XYZ Pendulum Effect Monster" };

        AddToDropdown(cardtype, options);
    }

    void PopulateTypeList()
    {
        string[] options = { "", "Aqua", "Beast", "Beast-Warrior", "Creator-God", "Cyberse", "Dinosaur", 
            "Divine-Beast", "Dragon", "Fairy", "Fiend", "Fish", "Insect", "Machine", "Plant", "Psychic", "Pyro", 
            "Reptile", "Rock", "Sea Serpent", "Spellcaster", "Thunder", "Warrior", "Winged Beast", "Normal", 
            "Field", "Equip", "Continuous", "Quick-Play", "Ritual", "Counter" };

        AddToDropdown(type, options);
    }

    void PopulateAttributeList()
    {
        string[] options = { "", "WATER", "WIND", "FIRE", "EARTH", "LIGHT", "DARK" };
        AddToDropdown(attribute, options);
    }

    void PopulateLinkRatingList()
    {
        string[] options = { "", "1", "2", "3", "4", "5" };
        AddToDropdown(linkrating, options);
    }

    void PopulateLinkMarkerList()
    {
        string[] options = { "", "Top", "Bottom", "Left", "Right", "Bottom-Left", "Bottom-Right", "Top-Left", "Top-Right" };
        AddToDropdown(linkmarker, options);
    }

    void PopulateFormatList()
    {
        string[] options = { "", "GOAT", "OCG GOAT", "SPEED DUEL", "RUSH DUEL", "DUEL LINKS" };
        AddToDropdown(format, options);
    }

    void PopulateBanList()
    {
        string[] options = { "", "TCG", "OCG", "GOAT"};
        AddToDropdown(banlist, options);
    }

    void PopulateLvlAndScaleLists()
    {
        string[] options = { "", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12" }; 

        AddToDropdown(lvl, options);

        AddToDropdown(pendulumscale, options);
        pendulumscale.Add(new Dropdown.OptionData("13"));
    }

    void PopulateAtkAndDefLists()
    {
        string[] options = { "", "?", "0","50", "100", "150", "200", "250", "300", "350", 
            "400", "450", "500", "550", "600", "650", "700", "750", "800", "850", "900", 
            "950", "1000", "1100", "1200", "1300" ,"1400" ,"1500", "1600" ,"1700", "1800",
            "1900", "2000", "2100", "2200", "2300", "2400", "2500", "2600", "2700", "2800", 
            "2900", "3000", "3100", "3200", "3300", "3400", "3500", "3600", "3800", "4000", "4500", "5000" };

        AddToDropdown(atk, options);
        AddToDropdown(def, options);
    }

    void AddToDropdown(List<Dropdown.OptionData> listData, string[] options)
    {
        foreach (string optionName in options)
        {
            listData.Add(new Dropdown.OptionData(optionName));
        }
    }

    #endregion

    #region Dropdowns
    public void OnChangePrimaryDropdown(int index)
    {
        if (!secondaryDropdown.interactable)
            secondaryDropdown.interactable = true;

        secondaryDropdown.ClearOptions();

        dropOptions = (DropOptions)index;

        switch (dropOptions)
        {
            case DropOptions.NONE:
                secondaryDropdown.interactable = false;
                break;
            case DropOptions.CARD_TYPE:
                SetDropDownData(cardtype, "&type=", index);
                break;
            case DropOptions.MONSTER_TYPE:
                SetDropDownData(type, "&race=", index);
                break;
            case DropOptions.ATK:
                SetDropDownData(atk, "&atk=", index);
                break;
            case DropOptions.ATK_LESS:
                SetDropDownData(atk, "&atk=lte", index);
                break;
            case DropOptions.ATK_GREATER:
                SetDropDownData(atk, "&atk=gte", index);
                break;
            case DropOptions.DEF:
                SetDropDownData(def, "&def=", index);
                break;
            case DropOptions.DEF_LESS:
                SetDropDownData(def, "&def=lte", index);
                break;
            case DropOptions.DEF_GREATER:
                SetDropDownData(def, "&def=gte", index);
                break;
            case DropOptions.LEVEL:
                SetDropDownData(lvl, "&level=", index);
                break;
            case DropOptions.LEVEL_LESS:
                SetDropDownData(lvl, "&level=lte", index);
                break;
            case DropOptions.LEVEL_GREATER:
                SetDropDownData(lvl, "&level=gte", index);
                break;
            case DropOptions.ATTRIBUTE:
                SetDropDownData(attribute, "&attribute=", index);
                break;
            case DropOptions.LINK_RATING:
                SetDropDownData(linkrating, "&link=", index);
                break;
            case DropOptions.LINK_MARKER:
                SetDropDownData(linkmarker, "&linkmarker=", index);
                break;
            case DropOptions.PENDULUM_SCALE:
                SetDropDownData(pendulumscale, "&scale=", index);
                break;
            case DropOptions.CARD_SET:
                SetDropDownData(cardset, "&cardset=", index);
                break;
            case DropOptions.ARCHETYPE:
                SetDropDownData(archetype, "&archetype=", index);
                break;
            case DropOptions.BANLIST:
                SetDropDownData(banlist, "&banlist=", index);
                break;
            case DropOptions.FORMAT:
                SetDropDownData(format, "&format=", index);
                break;
            default:
                break;
        }

        if (secondaryDropdown.value == 0)
            urlMod = "";
    }

    public void OnChangeSecondaryDropdown(int SecondaryDropdownIndex)
    {
        string value = secondaryDropdown.options[SecondaryDropdownIndex].text;
        int PrimaryDropdownIndex = primaryDropdown.value;
        string newUrlMod = "";

        switch (PrimaryDropdownIndex)
        {
            case 1:
                newUrlMod = "&type=";
                break;
            case 2:
                newUrlMod = "&race=";
                break;
            case 3:
                newUrlMod = "&atk=";
                break;
            case 4:
                newUrlMod = "&atk=lte";
                break;
            case 5:
                newUrlMod = "&atk=gte";
                break;
            case 6: 
                newUrlMod = "&def=";
                break;
            case 7:
                newUrlMod = "&def=lte";
                break;
            case 8:
                newUrlMod = "&def=gte";
                break;
            case 9:
                newUrlMod = "&level=";
                break;
            case 10:
                newUrlMod = "&level=lte";
                break;
            case 11:
                newUrlMod = "&level=gte";
                break;
            case 12:
                newUrlMod = "&attribute=";
                break;
            case 13:
                newUrlMod = "&link=";
                break;
            case 14:
                newUrlMod = "&linkmarker=";
                break;
            case 15:
                newUrlMod = "&scale=";
                break;
            case 16:
                newUrlMod = "&cardset=";
                break;
            case 17:
                newUrlMod = "&archetype=";
                break;
            case 18:
                newUrlMod = "&banlist=";
                break;
            case 19:
                newUrlMod = "&format=";
                break;
            default:
                break;
        }

        SetParamData(SaveManager.Instance.parameterIndices[PrimaryDropdownIndex - 1], SecondaryDropdownIndex, newUrlMod);

        urlParams[PrimaryDropdownIndex - 1] = urlMod + value;


        apiCall.dropdownUrlMod = "";
        foreach (string urlParam in urlParams)
        {
            apiCall.dropdownUrlMod += urlParam;
        }
    }

    #endregion

    void SetDropDownData(List<Dropdown.OptionData> optionList, string urlModifier, int index)
    {
        GetParamData(optionList, urlModifier, SaveManager.Instance.parameterIndices[index - 1]);
    }

    public void GetParamData(List<Dropdown.OptionData> option, string urlModifier, string parameterIndex)
    {
        secondaryDropdown.AddOptions(option);
        urlMod = urlModifier;
        secondaryDropdown.value = PlayerPrefs.GetInt(parameterIndex);
    }

    public void SetParamData(string parameterIndex, int prefsValue, string setUrlMod)
    {
        urlMod = "";

        if (secondaryDropdown.value > 0)
            urlMod = setUrlMod;
        
        PlayerPrefs.SetInt(parameterIndex, prefsValue);
    }

    public int GetDropOptionCount()
    {
        return Enum.GetValues(typeof(DropOptions)).Length;
    }

}