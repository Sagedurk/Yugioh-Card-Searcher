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
using static DropdownHandler;

//Handles the dropdowns (filter parameters and their respective values)
public class DropdownHandler : EUS.Cat_Systems.Singleton<DropdownHandler>
{
    #region Variables


    public Dropdown parameterDropdown;
    public Dropdown parameterValueDropdown;
    public Dropdown comparisonOperatorDropdown;
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
            instance.parameterDropdown = parameterDropdown;
            instance.parameterValueDropdown = parameterValueDropdown;
            instance.comparisonOperatorDropdown = comparisonOperatorDropdown;
        }

        base.Awake();

        apiCall = ApiCall.Instance;

        parameterDropdown.OverrideOnValueChanged(OnChangeParameterDropdown);
        parameterValueDropdown.OverrideOnValueChanged(OnChangeParameterValueDropdown);
        comparisonOperatorDropdown.OverrideOnValueChanged(OnChangeComparisonOperatorDropdown);

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
        List<Dropdown.OptionData> editorOptions = new List<Dropdown.OptionData>(parameterDropdown.options);
        foreach (ParameterInstance instance in parameterInstances)
        {
            if(instance != null)
                optionData.Add(instance.instanceName);
        }

        parameterDropdown.ClearOptions();
        parameterDropdown.AddOptions(editorOptions);
        parameterDropdown.AddOptions(optionData);
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
    public void OnChangeParameterDropdown(int index)
    {
        parameterValueDropdown.ClearOptions();

        if (index == 0)
        {
            parameterValueDropdown.interactable = false;
            comparisonOperatorDropdown.gameObject.SetActive(false);

            return;
        }

        if (!parameterValueDropdown.interactable)
            parameterValueDropdown.interactable = true;

        string [] urlParamModifier = urlParamModifiers[index - 1];
        ParameterInstance parameterInstance = parameterInstances[index - 1];

        SetDropDownData(parameterInstance, urlParamModifier[0], index);

        if (urlParamModifier.Length > 1)
        {
            comparisonOperatorDropdown.gameObject.SetActive(true);
            comparisonOperatorDropdown.value = parameterInstance.tertriaryIndex;
        }
        else
            comparisonOperatorDropdown.gameObject.SetActive(false);
    }

    public void OnChangeParameterValueDropdown(int SecondaryDropdownIndex)
    {
        string value = parameterValueDropdown.options[SecondaryDropdownIndex].text;
        int primaryDropdownIndex = parameterDropdown.value;
        ParameterInstance parameterInstance = parameterInstances[primaryDropdownIndex - 1];
        string[] urlParamModifier = urlParamModifiers[primaryDropdownIndex - 1];
        string newUrlMod = urlParamModifier[0];

        if(urlParamModifier.Length > 1)
            newUrlMod = urlParamModifier[comparisonOperatorDropdown.value];
        
        parameterInstance.optionIndex = SecondaryDropdownIndex;

        parameterInstance.SetURLModifier(SecondaryDropdownIndex, newUrlMod, value);
        
        PlayerPrefs.SetInt(SaveManager.Instance.parameterIndices[primaryDropdownIndex - 1], SecondaryDropdownIndex);


        SetApiCallUrlMod();
    }

    public void OnChangeComparisonOperatorDropdown(int index)
    {
        int primaryDropdownIndex = parameterDropdown.value;
        ParameterInstance parameterInstance = parameterInstances[primaryDropdownIndex - 1];

        string modifier = urlParamModifiers[primaryDropdownIndex - 1][index];
        string value = parameterValueDropdown.options[parameterValueDropdown.value].text;

        parameterInstance.tertriaryIndex = index;

        parameterInstance.SetURLModifier(parameterValueDropdown.value, modifier, value);

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
        parameterValueDropdown.AddOptions(parameterInstance.optionList);
        parameterInstance.optionIndex = PlayerPrefs.GetInt(SaveManager.Instance.parameterIndices[index - 1]);
        parameterValueDropdown.SetValueWithoutNotify(parameterInstance.optionIndex);
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
        /// <summary>
        /// Used to keep track of the tertriary dropdown value
        /// </summary>
        public int tertriaryIndex = 0;

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

        public void SetURLModifier(int dropdownValue ,string modifier, string value)
        {
            urlModifier = "";

            if (dropdownValue > 0)
                urlModifier = modifier + value;
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