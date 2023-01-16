using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Done
//Handles the things that should only run on boot
public class Index : MonoBehaviour
{
    void Awake()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("parameterStartVal", 0);
        EUS.Cat_Scene.LoadScene(1);
    }
}