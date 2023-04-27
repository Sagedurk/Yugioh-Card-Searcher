using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class DropdownLoader : MonoBehaviour
{
    public Color color;
    Dropdown primaryDropdown;
    Dropdown secondaryDropdown;


    private void Start()
    {
        primaryDropdown = DropdownHandler.Instance.primaryDropdown;
        secondaryDropdown = DropdownHandler.Instance.secondaryDropdown;

        MarkPrimaryDropdownOptions();
    }

    private void MarkPrimaryDropdownOptions()
    {
        List<DropdownHandler.ParameterInstance>parameterInstances = DropdownHandler.Instance.parameterInstances;

        int parameterValue = primaryDropdown.value;

        for (int i = 0; i < parameterInstances.Count; i++)
        {
            if(parameterInstances[i].optionIndex != 0)
                primaryDropdown.transform.GetChild(3).GetChild(0).GetChild(0).GetChild(i + 2).GetChild(0).GetComponent<Image>().color = color;

            //primaryDropdown.SetValueWithoutNotify(i);
        }

        //primaryDropdown.SetValueWithoutNotify(parameterValue);
    }

    void SetSecondaryDropdownValue(int index)
    {
        DropdownHandler.Instance.OnChangePrimaryDropdown(index);
        secondaryDropdown.value = PlayerPrefs.GetInt(SaveManager.Instance.parameterIndices[index - 1]);
        DropdownHandler.Instance.OnChangeSecondaryDropdown(PlayerPrefs.GetInt(SaveManager.Instance.parameterIndices[index - 1]));
    }

}