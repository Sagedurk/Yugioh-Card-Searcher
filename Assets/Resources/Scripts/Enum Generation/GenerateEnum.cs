#if UNITY_EDITOR
using UnityEditor;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using static UnityEngine.Networking.UnityWebRequest;

[InitializeOnLoad]
public class GenerateEnum
{
    static string enumFilePath = "Assets/DynamicEnums/";
    static GenerateEnum()
    {
        EditorBuildSettings.sceneListChanged += OnSceneListChanged;
    }

    public static void GenerateDynamicEnum(string enumName, string filePath, string[] enumEntries, string[] disabledEnumEntries = null)
    {
        if (!Directory.Exists(filePath))
            Directory.CreateDirectory(filePath);

        using (StreamWriter streamWriter = new StreamWriter(filePath + enumName + "DynamicEnum.cs"))
        {
            streamWriter.WriteLine("public enum " + enumName);
            streamWriter.WriteLine("{");

            WriteEnumEntriesToFile(streamWriter, enumEntries);

            if (disabledEnumEntries != null)
            {
                WriteEnumEntriesToFile(streamWriter, disabledEnumEntries, isDisabled: true);
            }

            streamWriter.WriteLine("}");
        }
        AssetDatabase.Refresh();
    }


    public static void OnSceneListChanged()
    {
        GenerateSceneEnum();
    }   


    public static Dictionary<int, string> GetBuildScenes(out Dictionary<int, string> disabledScenes)
    {
        Dictionary<int, string> sceneData = new Dictionary<int, string>();
        Dictionary<int, string> sceneDataDisabled = new Dictionary<int, string>();
        EditorBuildSettingsScene[] buildSettingsScenes = EditorBuildSettings.scenes;
        int disabledSceneCounter = 0;

        for (int i = 0; i < buildSettingsScenes.Length; i++)
        {
            SceneAsset sceneAsset = (SceneAsset)AssetDatabase.LoadAssetAtPath(buildSettingsScenes[i].path, typeof(SceneAsset));

            if (!buildSettingsScenes[i].enabled)
            {
                sceneDataDisabled.Add(disabledSceneCounter, sceneAsset.name);
                disabledSceneCounter++;
                continue;
            }
            
            sceneData.Add(i - disabledSceneCounter, sceneAsset.name);
        }

        disabledScenes = sceneDataDisabled;
        return sceneData;
    }


    private static void GenerateSceneEnum()
    {
        Dictionary<int, string> scenes = GetBuildScenes(out Dictionary<int, string> disabledScenes);


        string[] sceneNames = new string[scenes.Count];

        for (int i = 0; i < scenes.Count; i++)
        {
            scenes.TryGetValue(i, out string sceneName);
            sceneNames[i] = sceneName;
        }

        string[] disabledSceneNames = new string[disabledScenes.Count];
        for (int i = 0; i < disabledScenes.Count; i++)
        {
            disabledScenes.TryGetValue(i, out string sceneName);
            disabledSceneNames[i] = sceneName;
        }

        GenerateDynamicEnum("SceneNames", enumFilePath, sceneNames, disabledSceneNames);

    }
    
    #region Helper Functions
    private static void WriteEnumEntriesToFile(StreamWriter streamWriter, string[] entries, bool isDisabled = false)
    {
        for (int i = 0; i < entries.Length; i++)
        {
            string currentEntry = ConvertToValidEnumName(entries[i]);
            int value = isDisabled ? -1 : i;

            streamWriter.WriteLine($"\t{currentEntry} = {value}{(isDisabled ? ",    //DISABLED SCENE" : ",")}");
        }
    }

    private static string ConvertToValidEnumName(string input)
    {
        //Remove Numbers
        input = Regex.Replace(input, @"[\d-]", string.Empty);

        //Remove Leading Whitespace
        input = Regex.Replace(input, @"^\s+", "");

        //Replace Whitespace With Underscores
        input = Regex.Replace(input, @"\s+", "_");

        //Convert To Uppercase
        input = input.ToUpper();

        return input;
    }

    #endregion

}
#endif