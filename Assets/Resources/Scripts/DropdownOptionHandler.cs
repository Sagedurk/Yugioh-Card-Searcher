using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropdownOptionHandler : MonoBehaviour
{
    public DropdownHandler parameterHandler;
    public Color color;
    public CardInfo cardInfo;
    private void Start()
    {
        cardInfo = GameObject.FindObjectOfType<CardInfo>();
        StartCoroutine(loadParameters());
    }

    private IEnumerator loadParameters()
    {

        int parameterValue = parameterHandler.parameters.value;
        for (int i = 0; i < 20; i++)
        {
            parameterHandler.parameterValues.ClearOptions();
            parameterHandler.parameters.value = i;
            switch (i)
            {
                case 0:
                    parameterHandler.parameterValues.GetComponent<Dropdown>().interactable = false;
                    break;
                case 1:
                    parameterHandler.DropdownParams(i);
                    parameterHandler.parameterValues.value = PlayerPrefs.GetInt(parameterHandler.savemngr.paramIndex0);
                    parameterHandler.DropdownParamValues(PlayerPrefs.GetInt(parameterHandler.savemngr.paramIndex0));
                    break;
                case 2:
                    parameterHandler.DropdownParams(i);
                    parameterHandler.parameterValues.value = PlayerPrefs.GetInt(parameterHandler.savemngr.paramIndex1);
                    parameterHandler.DropdownParamValues(PlayerPrefs.GetInt(parameterHandler.savemngr.paramIndex1));
                    break;
                case 3:
                    parameterHandler.DropdownParams(i);
                    parameterHandler.parameterValues.value = PlayerPrefs.GetInt(parameterHandler.savemngr.paramIndex2);
                    parameterHandler.DropdownParamValues(PlayerPrefs.GetInt(parameterHandler.savemngr.paramIndex2));
                    break;
                case 4:
                    parameterHandler.DropdownParams(i);
                    parameterHandler.parameterValues.value = PlayerPrefs.GetInt(parameterHandler.savemngr.paramIndex3);
                    parameterHandler.DropdownParamValues(PlayerPrefs.GetInt(parameterHandler.savemngr.paramIndex3));
                    break;
                case 5:
                    parameterHandler.DropdownParams(i);
                    parameterHandler.parameterValues.value = PlayerPrefs.GetInt(parameterHandler.savemngr.paramIndex4);
                    parameterHandler.DropdownParamValues(PlayerPrefs.GetInt(parameterHandler.savemngr.paramIndex4));
                    break;
                case 6:
                    parameterHandler.DropdownParams(i);
                    parameterHandler.parameterValues.value = PlayerPrefs.GetInt(parameterHandler.savemngr.paramIndex5);
                    parameterHandler.DropdownParamValues(PlayerPrefs.GetInt(parameterHandler.savemngr.paramIndex5));
                    break;
                case 7:
                    parameterHandler.DropdownParams(i);
                    parameterHandler.parameterValues.value = PlayerPrefs.GetInt(parameterHandler.savemngr.paramIndex6);
                    parameterHandler.DropdownParamValues(PlayerPrefs.GetInt(parameterHandler.savemngr.paramIndex6));
                    break;
                case 8:
                    parameterHandler.DropdownParams(i);
                    parameterHandler.parameterValues.value = PlayerPrefs.GetInt(parameterHandler.savemngr.paramIndex7);
                    parameterHandler.DropdownParamValues(PlayerPrefs.GetInt(parameterHandler.savemngr.paramIndex7));
                    break;
                case 9:
                    parameterHandler.DropdownParams(i);
                    parameterHandler.parameterValues.value = PlayerPrefs.GetInt(parameterHandler.savemngr.paramIndex8);
                    parameterHandler.DropdownParamValues(PlayerPrefs.GetInt(parameterHandler.savemngr.paramIndex8));
                    break;
                case 10:
                    parameterHandler.DropdownParams(i);
                    parameterHandler.parameterValues.value = PlayerPrefs.GetInt(parameterHandler.savemngr.paramIndex9);
                    parameterHandler.DropdownParamValues(PlayerPrefs.GetInt(parameterHandler.savemngr.paramIndex9));
                    break;
                case 11:
                    parameterHandler.DropdownParams(i);
                    parameterHandler.parameterValues.value = PlayerPrefs.GetInt(parameterHandler.savemngr.paramIndex10);
                    parameterHandler.DropdownParamValues(PlayerPrefs.GetInt(parameterHandler.savemngr.paramIndex10));
                    break;
                case 12:
                    parameterHandler.DropdownParams(i);
                    parameterHandler.parameterValues.value = PlayerPrefs.GetInt(parameterHandler.savemngr.paramIndex11);
                    parameterHandler.DropdownParamValues(PlayerPrefs.GetInt(parameterHandler.savemngr.paramIndex11));
                    break;
                case 13:
                    parameterHandler.DropdownParams(i);
                    parameterHandler.parameterValues.value = PlayerPrefs.GetInt(parameterHandler.savemngr.paramIndex12);
                    parameterHandler.DropdownParamValues(PlayerPrefs.GetInt(parameterHandler.savemngr.paramIndex12));
                    break;
                case 14:
                    parameterHandler.DropdownParams(i);
                    parameterHandler.parameterValues.value = PlayerPrefs.GetInt(parameterHandler.savemngr.paramIndex13);
                    parameterHandler.DropdownParamValues(PlayerPrefs.GetInt(parameterHandler.savemngr.paramIndex13));
                    break;
                case 15:
                    parameterHandler.DropdownParams(i);
                    parameterHandler.parameterValues.value = PlayerPrefs.GetInt(parameterHandler.savemngr.paramIndex14);
                    parameterHandler.DropdownParamValues(PlayerPrefs.GetInt(parameterHandler.savemngr.paramIndex14));
                    break;
                case 16:
                    parameterHandler.DropdownParams(i);
                    yield return StartCoroutine(cardInfo.GetCardSet());
                    parameterHandler.parameterValues.value = PlayerPrefs.GetInt(parameterHandler.savemngr.paramIndex15);
                    //index out of range
                    //parameterHandler.DropdownParamValues(PlayerPrefs.GetInt(parameterHandler.savemngr.paramIndex15));
                    break;
                case 17:
                    parameterHandler.DropdownParams(i);
                    yield return StartCoroutine(cardInfo.GetArchetypes());
                    parameterHandler.DropdownParams(i);
                    parameterHandler.parameterValues.value = PlayerPrefs.GetInt(parameterHandler.savemngr.paramIndex16);
                    parameterHandler.DropdownParamValues(PlayerPrefs.GetInt(parameterHandler.savemngr.paramIndex16));
                    break;
                case 18:
                    parameterHandler.DropdownParams(i);
                    parameterHandler.parameterValues.value = PlayerPrefs.GetInt(parameterHandler.savemngr.paramIndex17);
                    parameterHandler.DropdownParamValues(PlayerPrefs.GetInt(parameterHandler.savemngr.paramIndex17));
                    break;
                case 19:
                    parameterHandler.DropdownParams(i);
                    parameterHandler.parameterValues.value = PlayerPrefs.GetInt(parameterHandler.savemngr.paramIndex18);
                    parameterHandler.DropdownParamValues(PlayerPrefs.GetInt(parameterHandler.savemngr.paramIndex18));
                    break;
                default:
                    break;
            }
            if (parameterHandler.parameterValues.value != 0)
            {
                parameterHandler.parameters.transform.GetChild(3).GetChild(0).GetChild(0).GetChild(i + 1).GetChild(0).GetComponent<Image>().color = color;
            }
        }
        parameterHandler.parameters.value = parameterValue;
    }








}
