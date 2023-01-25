using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;

//Engine Utility System
/* The idea with EUS is to be a smaller library of generally useful functions, which can be used across different projects
 * and functions which can't really be placed somewhere else
 * e.g. destroy all objects with a specific tag */
public static class EUS
{
    public static int sceneIndex = 0;
    static SaveManager saveManager;


    /// <summary>
    /// <see cref="Cat_Transform"/> is a category containing functions related to <see cref="Transform"/> and <see cref="RectTransform"/>. <br/>
    ///These functions are intended to improve the workflow of using <see cref="Transform"/> and <see cref="RectTransform"/>.
    /// </summary>
    public static class Cat_Transform
    {
        /// <summary>
        /// Finds a child of a given <see cref="Transform"/> at a specified nest depth, using an array of child indices to specify the path to the child.
        /// </summary>
        /// <param name="parent"> The parent <see cref="Transform"/> to search for the child in.</param>
        /// <param name="childDepth"> The nest depth of the child.</param>
        /// <param name="childIndices"> An array of child indices specifying the path to the child. The length of the array must match or exceed the <paramref name="childDepth"/> parameter.</param>
        /// <returns>The <see cref="Transform"/> of the child that was found. If an invalid index is encountered, the function returns null and logs an error message.</returns>
        /// <remarks> Example : Transform child = GetChildFromIndices(transform, 3, new int[] { 1, 4, 7 }); </remarks>
        public static Transform GetChildFromIndices(Transform parent, int childDepth, int[] childIndices)
        {
            Transform child = parent;
            if (childIndices.Length >= childDepth)
                for (int i = 0; i < childDepth; i++)
                {
                    if(HelperFunctions.IsValidChildIndex(child, childIndices[i]))
                        child = child.GetChild(childIndices[i]);

                    else
                    {
                        Debug.LogError("Invalid index at depth: " + i + ". Return NULL");
                        return null;
                    }
                }

            return child;
        }

        /// <summary>
        /// Finds the child of a given <see cref="RectTransform"/> at a specific depth using an array of child indices to specify the path to the child.
        /// </summary>
        /// <param name="parent">The parent <see cref="RectTransform"/> to search for the child in.</param>
        /// <param name="childDepth">The nest depth of the child.</param>
        /// <param name="childIndices">An array of child indices specifying the path to the child. The length of the array must match or exceed the <paramref name="childDepth"/> parameter.</param>
        /// <returns>The <see cref="RectTransform"/> of the child that was found. If an invalid index is encountered, the function returns null and logs an error message.</returns>
        public static RectTransform GetChildFromIndices(RectTransform parent, int childDepth, int[] childIndices)
        {                                  
            RectTransform child = parent;
            if (childIndices.Length < childDepth)
                return null;


            for (int i = 0; i < childDepth; i++)
            {
                if (HelperFunctions.IsValidChildIndex(child, childIndices[i]))
                    child = child.GetChild(childIndices[i]).GetComponent<RectTransform>();

                else
                {
                    Debug.LogError("Invalid index at depth: " + i + ". Return NULL");
                    return null;
                }
            }

            return child;
        }



        /// <summary>
        /// Helper functions for the Transform category
        /// </summary>
        private static class HelperFunctions
        {
            public static bool IsValidChildIndex(Transform parent, int index)
            {
                if (index >= parent.childCount || index < 0)
                    return false;

                return true;
            }
            public static bool IsValidChildIndex(RectTransform parent, int index)
            {
                if (index >= parent.childCount || index < 0)
                    return false;

                return true;
            }
        }
    
    }

    public static class Cat_Object_Manipulation 
    {
        public static void DestroyAll(string tag)
        {
            GameObject[] targets = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject target in targets)
            {
                Object.Destroy(target);
            }
        }



    }
    public static class Cat_UI
    {
        public static void HideUI(string tag)
        {
            GameObject[] targets = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject target in targets)
            {
                target.GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
            }
        }

        public static void ShowUI(string tag)
        {
            GameObject[] targets = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject target in targets)
            {
                target.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            }

        }

        public static void DisableUIComponent<T>(string tag) where T : Graphic
        {
            GameObject[] targets = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject target in targets)
            {
                target.GetComponent<T>().enabled = false;
            }
        }
        public static void DisableUICanvasComponent(string tag)
        {
            GameObject[] targets = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject target in targets)
            {
                target.GetComponent<Canvas>().enabled = false;
            }
        }




    }
    public static class Cat_EventSystem { }
    public static class Cat_Physics { }
    public static class Cat_Scene 
    {
        public static SceneNames currentScene;
        public enum LoadType
        {
            /// <summary>
            /// If using single type loading and the scene is small
            /// <br/> Use this to avoid unnecessary loading screens
            /// </summary>
            SINGLE,
            /// <summary>
            /// If using single type loading and the scene isn't small
            /// <br/> Use this to avoid bad user experience
            /// </summary>
            SINGLE_LOADING_SCREEN,
            /// <summary>
            /// Use this for additive type loading
            /// </summary>
            ADDITIVE,
        }


        private static float progressThreshold = 0.9f;

        public static void LoadScene(SceneNames sceneToLoad, LoadType loadType)
        {
            int sceneIndex = (int)sceneToLoad;

            if (sceneIndex == -1)
            {
                Debug.LogError("Attempting to load disabled scene: " + sceneToLoad.ToString());
                return;
            }

            currentScene = sceneToLoad;
            switch (loadType)
            {

                case LoadType.SINGLE:
                    SceneManager.LoadScene(sceneIndex);
                    break;

                case LoadType.SINGLE_LOADING_SCREEN:
                    SceneManager.LoadSceneAsync(sceneIndex);
                    break;

                case LoadType.ADDITIVE:

                    SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
                    break;
                default:
                    break;
            }




        }

        public static void UnloadScene(SceneNames sceneToUnload)
        {
            int sceneIndex = (int)sceneToUnload;

            if (sceneIndex == -1)
            {
                Debug.LogError("Attempting to unLoad disabled scene: " + sceneToUnload.ToString());
                return;
            }



        }




        public static IEnumerator LoadingScreen(SceneNames sceneToLoad)
        {

            AsyncOperation operation = SceneManager.LoadSceneAsync((int)sceneToLoad);
            operation.allowSceneActivation = false;

            // Wait until the scene is fully loaded
            while (operation.isDone == false)
            {
                // Update the loading progress bar
        //        loadingBar.localScale = new Vector3(operation.progress, 1f, 1f);

                // If the scene is loaded and ready to activate
                if (operation.progress >= progressThreshold)
                {
                    // Hide the loading screen
        //            loadingScreen.SetActive(false);

                    // Allow the scene to activate
                    operation.allowSceneActivation = true;
                }

                yield return null;
            }

        }




    }
    public static class Cat_Systems
    {
        public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
        {
            private static T _instance;
            public static T Instance
            {
                get
                {
                    if (_instance == null)
                    {
                        _instance = (T)FindObjectOfType(typeof(T));
                        if (_instance == null)
                        {
                            GameObject singleton = new GameObject();
                            _instance = singleton.AddComponent<T>();
                            singleton.name = "(singleton) " + typeof(T).ToString();
                        }
                    }

                    return _instance;
                }
            }

            protected virtual void Awake()
            {
                if (_instance == null)
                {
                    _instance = this as T;
                    DontDestroyOnLoad(this);
                }
                else
                {
                    Destroy(gameObject);
                }
            }

            protected T TryGetInstance()
            {
                return _instance;
            }
        }


        [InitializeOnLoad]
        public class GenerateEnum
        {
            static string enumFilePath = "Assets/Scripts/DynamicEnums/";
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
                        WriteEnumEntriesToFile(streamWriter, disabledEnumEntries, true);
                    }

                    streamWriter.WriteLine("}");
                }
                AssetDatabase.Refresh();
            }


            private static void OnSceneListChanged()
            {
                GenerateSceneEnum();
            }


            private static Dictionary<int, string> GetBuildScenes(out Dictionary<int, string> disabledScenes)
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


    }
    


}
public static class ExtensionMethods
{
    public static void CheckAndReplace(this string stringRef, string toReplace, string replaceTo)
    {
        if (stringRef.Contains(toReplace))
        {
            stringRef = stringRef.Replace(toReplace, replaceTo);
        }
    }

    public static string ConvertToValidFileName(this string stringRef)
    {
        string returnString = stringRef;
        returnString.CheckAndReplace(":", "_");
        returnString.CheckAndReplace("/", "=");
        returnString.CheckAndReplace("?", "ʔ");
        returnString.CheckAndReplace("%", "¤");
        returnString.CheckAndReplace("\"", "^");

        return returnString;
    }

    public static string PrepFromFileName(this string stringRef)
    {
        string returnString = stringRef;
        returnString.CheckAndReplace("_", ":");
        returnString.CheckAndReplace("=", "/");
        returnString.CheckAndReplace("ʔ", "?");
        returnString.CheckAndReplace("¤", "%");
        returnString.CheckAndReplace("^", "\"");
        return returnString;
    }

}