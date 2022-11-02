using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

//Handles the dropdowns (filter parameters and their respective values)
public class DropdownHandler : MonoBehaviour
{
    public Dropdown parameters;
    public Dropdown parameterValues;
    public CardInfo card;
    public SaveManager savemngr;
    public DropValueViewport paramValuesViewport;
    public List<Dropdown.OptionData> cardtype, atk, def, lvl, type, attribute, linkrating, linkmarker, pendulumscale, cardset, archetype, banlist, format;

    public string urlParam0, urlParam1, urlParam2, urlParam3, urlParam4, urlParam5, urlParam6, urlParam7, urlParam8, urlParam9, urlParam10, urlParam11,
        urlParam12, urlParam13, urlParam14, urlParam15, urlParam16, urlParam17, urlParam18;
    
    public string urlMod;

    void Start()
    {
        card = GameObject.FindObjectOfType<CardInfo>();
        parameters = GameObject.FindObjectsOfType<Dropdown>()[0];
        parameterValues = GameObject.FindObjectsOfType<Dropdown>()[1];
        savemngr = GameObject.FindObjectOfType<SaveManager>();

        cardtype.Add(new Dropdown.OptionData(""));
        cardtype.Add(new Dropdown.OptionData("Effect Monster"));
        cardtype.Add(new Dropdown.OptionData("Flip Effect Monster"));
        cardtype.Add(new Dropdown.OptionData("Flip Tuner Effect Monster"));
        cardtype.Add(new Dropdown.OptionData("Gemini Monster"));
        cardtype.Add(new Dropdown.OptionData("Normal Monster"));
        cardtype.Add(new Dropdown.OptionData("Normal Tuner Monster"));
        cardtype.Add(new Dropdown.OptionData("Pendulum Effect Monster"));
        cardtype.Add(new Dropdown.OptionData("Pendulum Flip Effect Monster"));
        cardtype.Add(new Dropdown.OptionData("Pendulum Normal Monster"));
        cardtype.Add(new Dropdown.OptionData("Pendulum Tuner Effect Monster"));
        cardtype.Add(new Dropdown.OptionData("Ritual Effect Monster"));
        cardtype.Add(new Dropdown.OptionData("Ritual Monster"));
        cardtype.Add(new Dropdown.OptionData("Skill Card"));
        cardtype.Add(new Dropdown.OptionData("Spell Card"));
        cardtype.Add(new Dropdown.OptionData("Spirit Monster"));
        cardtype.Add(new Dropdown.OptionData("Toon Monster"));
        cardtype.Add(new Dropdown.OptionData("Trap Card"));
        cardtype.Add(new Dropdown.OptionData("Tuner Monster"));
        cardtype.Add(new Dropdown.OptionData("Union Effect Monster"));
        cardtype.Add(new Dropdown.OptionData("Fusion Monster"));
        cardtype.Add(new Dropdown.OptionData("Link Monster"));
        cardtype.Add(new Dropdown.OptionData("Pendulum Effect Fusion Monster"));
        cardtype.Add(new Dropdown.OptionData("Synchro Monster"));
        cardtype.Add(new Dropdown.OptionData("Synchro Pendulum Effect Monster"));
        cardtype.Add(new Dropdown.OptionData("Synchro Tuner Monster"));
        cardtype.Add(new Dropdown.OptionData("XYZ Monster"));
        cardtype.Add(new Dropdown.OptionData("XYZ Pendulum Effect Monster"));

        AddCombatList(atk);
        atk.Add(new Dropdown.OptionData("4200"));
        atk.Add(new Dropdown.OptionData("4400"));
        atk.Add(new Dropdown.OptionData("4500"));
        atk.Add(new Dropdown.OptionData("4600"));
        atk.Add(new Dropdown.OptionData("5000"));
        AddCombatList(def);
        def.Add(new Dropdown.OptionData("4500"));
        def.Add(new Dropdown.OptionData("5000"));

        AddIntList(lvl);

        type.Add(new Dropdown.OptionData(""));
        type.Add(new Dropdown.OptionData("Aqua"));
        type.Add(new Dropdown.OptionData("Beast"));
        type.Add(new Dropdown.OptionData("Beast-Warrior"));
        type.Add(new Dropdown.OptionData("Creator-God"));
        type.Add(new Dropdown.OptionData("Cyberse"));
        type.Add(new Dropdown.OptionData("Dinosaur"));
        type.Add(new Dropdown.OptionData("Divine-Beast"));
        type.Add(new Dropdown.OptionData("Dragon"));
        type.Add(new Dropdown.OptionData("Fairy"));
        type.Add(new Dropdown.OptionData("Fiend"));
        type.Add(new Dropdown.OptionData("Fish"));
        type.Add(new Dropdown.OptionData("Insect"));
        type.Add(new Dropdown.OptionData("Machine"));
        type.Add(new Dropdown.OptionData("Plant"));
        type.Add(new Dropdown.OptionData("Psychic"));
        type.Add(new Dropdown.OptionData("Pyro"));
        type.Add(new Dropdown.OptionData("Reptile"));
        type.Add(new Dropdown.OptionData("Rock"));
        type.Add(new Dropdown.OptionData("Sea Serpent"));
        type.Add(new Dropdown.OptionData("Spellcaster"));
        type.Add(new Dropdown.OptionData("Thunder"));
        type.Add(new Dropdown.OptionData("Warrior"));
        type.Add(new Dropdown.OptionData("Winged Beast"));
        type.Add(new Dropdown.OptionData("Normal"));
        type.Add(new Dropdown.OptionData("Field"));
        type.Add(new Dropdown.OptionData("Equip"));
        type.Add(new Dropdown.OptionData("Continous"));
        type.Add(new Dropdown.OptionData("Quick-Play"));
        type.Add(new Dropdown.OptionData("Ritual"));
        type.Add(new Dropdown.OptionData("Counter"));

        attribute.Add(new Dropdown.OptionData(""));
        attribute.Add(new Dropdown.OptionData("WATER"));
        attribute.Add(new Dropdown.OptionData("WIND"));
        attribute.Add(new Dropdown.OptionData("FIRE"));
        attribute.Add(new Dropdown.OptionData("EARTH"));
        attribute.Add(new Dropdown.OptionData("LIGHT"));
        attribute.Add(new Dropdown.OptionData("DARK"));

        linkrating.Add(new Dropdown.OptionData(""));
        linkrating.Add(new Dropdown.OptionData("1"));
        linkrating.Add(new Dropdown.OptionData("2"));
        linkrating.Add(new Dropdown.OptionData("3"));
        linkrating.Add(new Dropdown.OptionData("4"));
        linkrating.Add(new Dropdown.OptionData("5"));

        linkmarker.Add(new Dropdown.OptionData(""));
        linkmarker.Add(new Dropdown.OptionData("Top"));
        linkmarker.Add(new Dropdown.OptionData("Bottom"));
        linkmarker.Add(new Dropdown.OptionData("Left"));
        linkmarker.Add(new Dropdown.OptionData("Right"));
        linkmarker.Add(new Dropdown.OptionData("Bottom-Left"));
        linkmarker.Add(new Dropdown.OptionData("Bottom-Right"));
        linkmarker.Add(new Dropdown.OptionData("Top-Left"));
        linkmarker.Add(new Dropdown.OptionData("Top-Right"));

        AddIntList(pendulumscale);
        pendulumscale.Add(new Dropdown.OptionData("13"));

        //Cardset & Archetype gets populated in [CardInfo] (~ line 460)

        banlist.Add(new Dropdown.OptionData(""));
        banlist.Add(new Dropdown.OptionData("TCG"));
        banlist.Add(new Dropdown.OptionData("OCG"));
        banlist.Add(new Dropdown.OptionData("GOAT"));

        format.Add(new Dropdown.OptionData(""));
        format.Add(new Dropdown.OptionData("GOAT"));
        format.Add(new Dropdown.OptionData("OCG GOAT"));
        format.Add(new Dropdown.OptionData("SPEED DUEL"));
        format.Add(new Dropdown.OptionData("RUSH DUEL"));
        format.Add(new Dropdown.OptionData("DUEL LINKS"));
    }
    public void DropdownParams(int index)
    {
        if (!parameterValues.GetComponent<Dropdown>().interactable)
            parameterValues.GetComponent<Dropdown>().interactable = true;


        parameterValues.ClearOptions();

        switch (index)
        {
            case 0:
                parameterValues.GetComponent<Dropdown>().interactable = false;
                break;
            case 1:
                GetParamData(cardtype, "&type=", savemngr.paramIndex0);
                paramValuesViewport.amountOfItems = cardtype.Count;
                break;
            case 2:
                GetParamData(type, "&race=", savemngr.paramIndex1);
                paramValuesViewport.amountOfItems = type.Count;
                break;
            case 3:
                GetParamData(atk, "&atk=", savemngr.paramIndex2);
                paramValuesViewport.amountOfItems = atk.Count;
                break;
            case 4:
                GetParamData(atk, "&atk=lte", savemngr.paramIndex3);
                paramValuesViewport.amountOfItems = atk.Count;
                break;
            case 5:
                GetParamData(atk, "&atk=gte", savemngr.paramIndex4);
                paramValuesViewport.amountOfItems = atk.Count;
                break;
            case 6:
                GetParamData(def, "&def=", savemngr.paramIndex5);
                paramValuesViewport.amountOfItems = def.Count;
                break;
            case 7:
                GetParamData(def, "&def=lte", savemngr.paramIndex6);
                paramValuesViewport.amountOfItems = def.Count;
                break;
            case 8:
                GetParamData(def, "&def=gte", savemngr.paramIndex7);
                paramValuesViewport.amountOfItems = def.Count;
                break;
            case 9:
                GetParamData(lvl, "&level=", savemngr.paramIndex8);
                paramValuesViewport.amountOfItems = lvl.Count;
                break;
            case 10:
                GetParamData(lvl, "&level=lte", savemngr.paramIndex9);
                paramValuesViewport.amountOfItems = lvl.Count;
                break;
            case 11:
                GetParamData(lvl, "&level=gte", savemngr.paramIndex10);
                paramValuesViewport.amountOfItems = lvl.Count;
                break;
            case 12:
                GetParamData(attribute, "&attribute=", savemngr.paramIndex11);
                paramValuesViewport.amountOfItems = attribute.Count;
                break;
            case 13:
                GetParamData(linkrating, "&link=", savemngr.paramIndex12);
                paramValuesViewport.amountOfItems = linkrating.Count;
                break;
            case 14:
                GetParamData(linkmarker, "&linkmarker=", savemngr.paramIndex13);
                paramValuesViewport.amountOfItems = linkmarker.Count;
                break;
            case 15:
                GetParamData(pendulumscale, "&scale=", savemngr.paramIndex14);
                paramValuesViewport.amountOfItems = pendulumscale.Count;
                break;
            case 16:
                GetParamData(cardset, "&cardset=", savemngr.paramIndex15);
                paramValuesViewport.amountOfItems = cardset.Count;
                break;
            case 17:
                GetParamData(archetype, "&archetype=", savemngr.paramIndex16);
                paramValuesViewport.amountOfItems = archetype.Count;
                break;
            case 18:
                GetParamData(banlist, "&banlist=", savemngr.paramIndex17);
                paramValuesViewport.amountOfItems = banlist.Count;
                break;
            case 19:
                GetParamData(format, "&format=", savemngr.paramIndex18);
                paramValuesViewport.amountOfItems = format.Count;
                break;
            default:
                break;
        }
        if (parameterValues.value == 0)
        {
            urlMod = "";
        }
    }

    public void DropdownParamValues(int index)
    {
        string value = parameterValues.options[index].text;
        switch (parameters.value)
        {
            case 1:
                SetParamData(savemngr.paramIndex0, index, "&type=");
                urlParam0 = urlMod + value;
                break;
            case 2:
                SetParamData(savemngr.paramIndex1, index, "&race=");
                urlParam1 = urlMod + value;
                break;
            case 3:
                SetParamData(savemngr.paramIndex2, index, "&atk=");
                urlParam2 = urlMod + value;
                break;
            case 4:
                SetParamData(savemngr.paramIndex3, index, "&atk=lte");
                urlParam3 = urlMod + value;
                break;
            case 5:
                SetParamData(savemngr.paramIndex4, index, "&atk=gte");
                urlParam4 = urlMod + value;
                break;
            case 6:
                SetParamData(savemngr.paramIndex5, index, "&def=");
                urlParam5 = urlMod + value;
                break;
            case 7:
                SetParamData(savemngr.paramIndex6, index, "&def=lte");
                urlParam6 = urlMod + value;
                break;
            case 8:
                SetParamData(savemngr.paramIndex7, index, "&def=gte");
                urlParam7 = urlMod + value;
                break;
            case 9:
                SetParamData(savemngr.paramIndex8, index, "&level=");
                urlParam8 = urlMod + value;
                break;
            case 10:
                SetParamData(savemngr.paramIndex9, index, "&level=lte");
                urlParam9 = urlMod + value;
                break;
            case 11:
                SetParamData(savemngr.paramIndex10, index, "&level=gte");
                urlParam10 = urlMod + value;
                break;
            case 12:
                SetParamData(savemngr.paramIndex11, index, "&attribute=");
                urlParam11 = urlMod + value;
                break;
            case 13:
                SetParamData(savemngr.paramIndex12, index, "&link=");
                urlParam12 = urlMod + value;
                break;
            case 14:
                SetParamData(savemngr.paramIndex13, index, "&linkmarker=");
                urlParam13 = urlMod + value;
                break;
            case 15:
                SetParamData(savemngr.paramIndex14, index, "&scale=");
                urlParam14 = urlMod + value;
                break;
            case 16:
                SetParamData(savemngr.paramIndex15, index, "&cardset=");
                urlParam15 = urlMod + value;
                break;
            case 17:
                SetParamData(savemngr.paramIndex16, index, "&archetype=");
                urlParam16 = urlMod + value;
                break;
            case 18:
                SetParamData(savemngr.paramIndex17, index, "&banlist=");
                urlParam17 = urlMod + value;
                break;
            case 19:
                SetParamData(savemngr.paramIndex18, index, "&format=");
                urlParam18 = urlMod + value;
                break;
            default:
                break;
        }
        card._URLMod = urlParam0 + urlParam1 + urlParam2 + urlParam3 + urlParam4 + urlParam5 + urlParam6 + urlParam7 + urlParam8
            + urlParam9 + urlParam10 + urlParam11 + urlParam12 + urlParam13 + urlParam14 + urlParam15 + urlParam16 + urlParam17 + urlParam18;
    }

    void AddIntList(List<Dropdown.OptionData> listData)
    {
        listData.Add(new Dropdown.OptionData(""));
        listData.Add(new Dropdown.OptionData("0"));
        listData.Add(new Dropdown.OptionData("1"));
        listData.Add(new Dropdown.OptionData("2"));
        listData.Add(new Dropdown.OptionData("3"));
        listData.Add(new Dropdown.OptionData("4"));
        listData.Add(new Dropdown.OptionData("5"));
        listData.Add(new Dropdown.OptionData("6"));
        listData.Add(new Dropdown.OptionData("7"));
        listData.Add(new Dropdown.OptionData("8"));
        listData.Add(new Dropdown.OptionData("9"));
        listData.Add(new Dropdown.OptionData("10"));
        listData.Add(new Dropdown.OptionData("11"));
        listData.Add(new Dropdown.OptionData("12"));
    }
    void AddCombatList(List<Dropdown.OptionData> listData)
    {
        listData.Add(new Dropdown.OptionData(""));
        listData.Add(new Dropdown.OptionData("0"));
        listData.Add(new Dropdown.OptionData("50"));
        listData.Add(new Dropdown.OptionData("100"));
        listData.Add(new Dropdown.OptionData("150"));
        listData.Add(new Dropdown.OptionData("200"));
        listData.Add(new Dropdown.OptionData("250"));
        listData.Add(new Dropdown.OptionData("300"));
        listData.Add(new Dropdown.OptionData("350"));
        listData.Add(new Dropdown.OptionData("400"));
        listData.Add(new Dropdown.OptionData("450"));
        listData.Add(new Dropdown.OptionData("500"));
        listData.Add(new Dropdown.OptionData("550"));
        listData.Add(new Dropdown.OptionData("600"));
        listData.Add(new Dropdown.OptionData("650"));
        listData.Add(new Dropdown.OptionData("700"));
        listData.Add(new Dropdown.OptionData("750"));
        listData.Add(new Dropdown.OptionData("800"));
        listData.Add(new Dropdown.OptionData("850"));
        listData.Add(new Dropdown.OptionData("900"));
        listData.Add(new Dropdown.OptionData("950"));
        listData.Add(new Dropdown.OptionData("1000"));
        listData.Add(new Dropdown.OptionData("1100"));
        listData.Add(new Dropdown.OptionData("1200"));
        listData.Add(new Dropdown.OptionData("1300"));
        listData.Add(new Dropdown.OptionData("1400"));
        listData.Add(new Dropdown.OptionData("1500"));
        listData.Add(new Dropdown.OptionData("1600"));
        listData.Add(new Dropdown.OptionData("1700"));
        listData.Add(new Dropdown.OptionData("1800"));
        listData.Add(new Dropdown.OptionData("1900"));
        listData.Add(new Dropdown.OptionData("2000"));
        listData.Add(new Dropdown.OptionData("2100"));
        listData.Add(new Dropdown.OptionData("2200"));
        listData.Add(new Dropdown.OptionData("2300"));
        listData.Add(new Dropdown.OptionData("2400"));
        listData.Add(new Dropdown.OptionData("2500"));
        listData.Add(new Dropdown.OptionData("2600"));
        listData.Add(new Dropdown.OptionData("2700"));
        listData.Add(new Dropdown.OptionData("2800"));
        listData.Add(new Dropdown.OptionData("2900"));
        listData.Add(new Dropdown.OptionData("3000"));
        listData.Add(new Dropdown.OptionData("3100"));
        listData.Add(new Dropdown.OptionData("3200"));
        listData.Add(new Dropdown.OptionData("3300"));
        listData.Add(new Dropdown.OptionData("3400"));
        listData.Add(new Dropdown.OptionData("3500"));
        listData.Add(new Dropdown.OptionData("3600"));
        listData.Add(new Dropdown.OptionData("3800"));
        listData.Add(new Dropdown.OptionData("4000"));
    }

    public void GetParamData(List<Dropdown.OptionData> option, string urlModifier, string parameterIndex)
    {
        parameterValues.AddOptions(option);
        urlMod = urlModifier;
        parameterValues.value = PlayerPrefs.GetInt(parameterIndex);
    }

    public void SetParamData(string parameterIndex, int prefsValue, string setUrlMod)
    {
        if (parameterValues.value != 0)
        {
            urlMod = setUrlMod;
        }
        else
        {
            urlMod = "";
        }
        PlayerPrefs.SetInt(parameterIndex, prefsValue);
    }
}