using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class DropdownLoader : MonoBehaviour
{
    public DropdownHandler dropdownHandler;
    public Color color;
    ApiCall apiCall;
    Dropdown primaryDropdown;
    Dropdown secondaryDropdown;


    private void Start()
    {
        apiCall = ApiCall.Instance;
        primaryDropdown = dropdownHandler.primaryDropdown;
        secondaryDropdown = dropdownHandler.secondaryDropdown;

        StartCoroutine(LoadDropdowns());
    }

    private IEnumerator LoadDropdowns()
    {
        int parameterValue = primaryDropdown.value;
        for (int i = 0; i < dropdownHandler.GetDropOptionCount(); i++)
        {
            secondaryDropdown.ClearOptions();
            primaryDropdown.value = i;

            if(i == (int)DropdownHandler.DropOptions.NONE)
            {
                secondaryDropdown.interactable = false;
            }
            
            else if (i == (int)DropdownHandler.DropOptions.CARD_SET)
            {
                dropdownHandler.OnChangePrimaryDropdown(i);
                yield return StartCoroutine(apiCall.GetCardSet());
                secondaryDropdown.value = PlayerPrefs.GetInt(apiCall.saveManager.parameterIndices[i - 1]);
                //index out of range
                //parameterHandler.DropdownParamValues(PlayerPrefs.GetInt(parameterHandler.savemngr.paramIndex15));
            }

            else if (i == (int)DropdownHandler.DropOptions.ARCHETYPE)
            {
                //dropdownHandler.OnChangePrimaryDropdown(i);
                yield return StartCoroutine(apiCall.GetArchetypes());
                SetSecondaryDropdownValue(i);
            }

            else
            {
                SetSecondaryDropdownValue(i);
            }
            
            if (secondaryDropdown.value != 0)
                primaryDropdown.transform.GetChild(3).GetChild(0).GetChild(0).GetChild(i + 1).GetChild(0).GetComponent<Image>().color = color;
            
        }
        primaryDropdown.value = parameterValue;
    }

    void SetSecondaryDropdownValue(int index)
    {
        dropdownHandler.OnChangePrimaryDropdown(index);
        secondaryDropdown.value = PlayerPrefs.GetInt(apiCall.saveManager.parameterIndices[index - 1]);
        dropdownHandler.OnChangeSecondaryDropdown(PlayerPrefs.GetInt(apiCall.saveManager.parameterIndices[index - 1]));
    }

}