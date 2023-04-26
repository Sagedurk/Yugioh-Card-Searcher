using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Reflection;
using UnityEditor;
using System.Globalization;
using static ApiCall;

//Handles the dropdowns (filter parameters and their respective values)
public class DropdownHandler : EUS.Cat_Systems.Singleton<DropdownHandler>
{
    #region Variables


    public Dropdown primaryDropdown;
    public Dropdown secondaryDropdown;
    public Dropdown tertriaryDropdown;
    [HideInInspector] public DropOptions dropOptions;

    [HideInInspector] public List<ParameterInstance> parameterInstances = new List<ParameterInstance>();
    [HideInInspector] private ParameterInstance cardtype, atk, def, lvl, type, attribute, linkrating, linkmarker, pendulumscale, cardset, archetype, banlist, format;

    private string[][] urlParamModifiers = new string[][]{ 
        new string[]{ "&type=" }, 
        new string[] { "&race=" }, 
        new string[] { "&atk=", "&atk=lt", "&atk=lte", "&atk=gt", "&atk=gte" }, 
        new string[] { "&def=", "&def=lt", "&def=lte", "&def=gt", "&def=gte" }, 
        new string[] { "&level=", "&level=lt", "&level=lte", "&level=gt", "&level=gte" }, 
        new string[] { "&attribute=" }, 
        new string[] { "&link=" }, 
        new string[] { "&linkmarker=" }, 
        new string[] { "&scale=" }, 
        new string[] { "&cardset=" }, 
        new string[] { "&archetype=" }, 
        new string[] { "&banlist=" }, 
        new string[] { "&format=" }
    };


    private static Dictionary<DropOptions, string> enumStringMap = new Dictionary<DropOptions, string>()
    {
        { DropOptions.MONSTER_TYPE, "M/S/T Type" },
        { DropOptions.ATK, "ATK"},
        { DropOptions.DEF, "DEF"}
    };

    ApiCall apiCall;


    public enum DropOptions
    {
        CARD_TYPE,
        /// <summary>
        /// Monster/Spell/Trap Type
        /// </summary>
        MONSTER_TYPE,
        ATK,
        DEF,
        LEVEL,
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

    protected override void Awake()
    {
        DropdownHandler instance = TryGetInstance();
        if (instance)
        {
            instance.primaryDropdown = primaryDropdown;
            instance.secondaryDropdown = secondaryDropdown;
        }

        base.Awake();

        apiCall = ApiCall.Instance;

        primaryDropdown.OverrideOnValueChanged(OnChangePrimaryDropdown);
        secondaryDropdown.OverrideOnValueChanged(OnChangeSecondaryDropdown);
        tertriaryDropdown.OverrideOnValueChanged(OnChangeTertriaryDropdown);

        CreateParameterInstances();

        SetDropdownUI();
    }

    string ConvertEnumToString(DropOptions enumValue)
    {
        if (enumStringMap.TryGetValue(enumValue, out string mappedValue))
            return mappedValue;
        

        string enumString = enumValue.ToString();
        enumString = enumString.CheckAndReplace("_", " ");
        enumString = enumString.ToLower();
        enumString = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(enumString);
        return enumString;
    }

    void CreateParameterInstances()
    {
        for (int i = 0; i < GetDropOptionCount(); i++)
        {
            dropOptions = (DropOptions)i;

            ParameterInstance newInstance = PopulateParamList(dropOptions);
            parameterInstances.Add(newInstance);
        }
    }

    void SetDropdownUI()
    {
        List<string> optionData = new List<string>();
        foreach (ParameterInstance instance in parameterInstances)
        {
            if(instance != null)
                optionData.Add(instance.instanceName);
        }

        primaryDropdown.AddOptions(optionData);
    }





    #region Populate Dropdown Lists




    string[] LoadParameterOptions(string fileName)
    {
        DropdownParameterParse[] dropdownData = SaveManager.ReadFile<DropdownParameterParse>(SaveManager.parameterValuesDirectory, fileName, SaveManager.parameterValuesFileType);

        if (dropdownData != null)
        {
            return dropdownData[0].options;
        }
        else
        {
            ParameterOptionSO paramOption = Resources.Load("Scriptables/" + fileName) as ParameterOptionSO;

            if (paramOption == null)
                return null;

            DropdownParameterParse saveData = new DropdownParameterParse();
            saveData.options = paramOption.options;
            saveData.TryWriteClassToFile(SaveManager.parameterValuesDirectory, fileName, SaveManager.parameterValuesFileType);
            return paramOption.options;
        }
    }

    ParameterInstance PopulateParamList(DropOptions enumName)
    {
        string paramName = ConvertEnumToString(enumName);
        string fileName = paramName.ConvertToValidFileName();

        string[] options = LoadParameterOptions(fileName);

        if (options == null)
            return null;

        return new ParameterInstance(paramName, options);
    }

    

    #endregion

    #region Dropdowns
    public void OnChangePrimaryDropdown(int index)
    {
        secondaryDropdown.ClearOptions();

        if (index == 0)
        {
            secondaryDropdown.interactable = false;
            tertriaryDropdown.gameObject.SetActive(false);

            return;
        }

        if (!secondaryDropdown.interactable)
            secondaryDropdown.interactable = true;

        string [] urlParamModifier = urlParamModifiers[index - 1];
        ParameterInstance parameterInstance = parameterInstances[index - 1];


        if (urlParamModifier.Length > 1)
            tertriaryDropdown.gameObject.SetActive(true);
        else
            tertriaryDropdown.gameObject.SetActive(false);
        
        SetDropDownData(parameterInstance, urlParamModifier[0], index);
    }

    public void OnChangeSecondaryDropdown(int SecondaryDropdownIndex)
    {
        string value = secondaryDropdown.options[SecondaryDropdownIndex].text;
        int primaryDropdownIndex = primaryDropdown.value;
        ParameterInstance parameterInstance = parameterInstances[primaryDropdownIndex - 1];
        string[] urlParamModifier = urlParamModifiers[primaryDropdownIndex - 1];
        string newUrlMod = urlParamModifier[0];

        if(urlParamModifier.Length > 1)
            newUrlMod = urlParamModifier[tertriaryDropdown.value];
        

        parameterInstance.optionIndex = SecondaryDropdownIndex;
        parameterInstance.urlModifier = "";
        
        if (SecondaryDropdownIndex > 0)
            parameterInstance.urlModifier = newUrlMod + value;
        
        PlayerPrefs.SetInt(SaveManager.Instance.parameterIndices[primaryDropdownIndex - 1], SecondaryDropdownIndex);


        SetApiCallUrlMod();
    }

    public void OnChangeTertriaryDropdown(int index)
    {
        int primaryDropdownIndex = primaryDropdown.value;
        ParameterInstance parameterInstance = parameterInstances[primaryDropdownIndex - 1];

        string modifier = urlParamModifiers[primaryDropdownIndex - 1][index];
        string value = secondaryDropdown.options[secondaryDropdown.value].text;

        parameterInstance.urlModifier = "";

        if(secondaryDropdown.value > 0)
            parameterInstance.urlModifier = modifier + value;

        SetApiCallUrlMod();
    }

    #endregion

    void SetApiCallUrlMod()
    {
        apiCall.dropdownUrlMod = "";
        foreach (ParameterInstance instance in parameterInstances)
        {
            apiCall.dropdownUrlMod += instance.urlModifier;
        }
    }

    void SetDropDownData(ParameterInstance parameterInstance, string urlModifier, int index)
    {
        secondaryDropdown.AddOptions(parameterInstance.optionList);
        parameterInstance.optionIndex = PlayerPrefs.GetInt(SaveManager.Instance.parameterIndices[index - 1]);
        secondaryDropdown.SetValueWithoutNotify(parameterInstance.optionIndex);
    }


    public int GetDropOptionCount()
    {
        return Enum.GetValues(typeof(DropOptions)).Length;
    }


    public class ParameterInstance
    {
        public string instanceName;
        public int optionIndex = 0;
        public List<Dropdown.OptionData> optionList = new List<Dropdown.OptionData>();
        public string urlModifier = "";

        public ParameterInstance(string name, string[] options, Action OnCreate = null) 
        { 
            instanceName = name;
            optionList.Add(new Dropdown.OptionData(""));
            AddToDropdown(optionList, options);

            if (OnCreate != null)
                OnCreate.Invoke();
        }

        private void AddToDropdown(List<Dropdown.OptionData> listData, string[] options)
        {
            foreach (string optionName in options)
            {
                listData.Add(new Dropdown.OptionData(optionName));
            }
        }
    }

    #region Old Stuff
    ParameterInstance PopulateCardSetList()
    {
        string[] options = {"TCG", "OCG", "GOAT" };
        return new ParameterInstance("Card Set", options);
    }
    ParameterInstance PopulateArchetypeList()
    {
        string[] options = {"TCG", "OCG", "GOAT" };
        return new ParameterInstance("Archetype", options);
    }
    #endregion

}