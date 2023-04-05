using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ParameterOptionSO : ScriptableObject
{
    public string[] options;
    //CHANGE TO FILE IO!!!

#if UNITY_EDITOR
    [MenuItem("Assets/Create/Parameter Options")]
    public static void CreateAsset()
    {
        ParameterOptionSO newAsset = ScriptableObject.CreateInstance<ParameterOptionSO>();

        string uniquePath = AssetDatabase.GenerateUniqueAssetPath("Assets/Resources/Scriptables/Options.asset");
        AssetDatabase.CreateAsset(newAsset, uniquePath);    
        AssetDatabase.SaveAssets();
    }

    public static void CreateAsset(string fileName, string[] options)
    {
        ParameterOptionSO newAsset = ScriptableObject.CreateInstance<ParameterOptionSO>();
        newAsset.options = options;

        string resourceFilePath = "Scriptables/" + fileName;
        string filePath = "Assets/Resources/" + resourceFilePath + ".asset";
        ParameterOptionSO loadedAsset = Resources.Load<ParameterOptionSO>(resourceFilePath);

        if (loadedAsset != null)
            return;
        
        AssetDatabase.CreateAsset(newAsset, filePath);
        AssetDatabase.SaveAssets();
    }
#endif
}