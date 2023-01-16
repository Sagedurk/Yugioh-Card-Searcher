using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEditor;

//Engine Utility System
/* The idea with EUS is to be a smaller library of generally useful functions, which can be used across different projects
 * and functions which can't really be placed somewhere else
 * e.g. destroy all objects with a specific tag */
public class EUS
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

    public static class Cat_Object_Manipulation { }
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
        public enum Scenes
        {
            Scene1 = 0, 
            Scene2 = 1, 
            Scene3 = 2, 
            Scene4 = 3,
            Scene5 = 4, 
            Scene6 = 5, 
            Scene7 = 6, 
            Scene8 = 7, 
            Scene9 = 8,
        }

        public static int sceneIndex = -1;
        private static float progressThreshold = 0.9f;

        public static IEnumerator LoadingScreen(Scenes sceneToLoad)
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


        

        public static void LoadScene(int index)
        {
            if (sceneIndex != 0)
            {
                saveManager.SaveData();
            }

            sceneIndex = index;
            SceneManager.LoadScene(index);
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
        }  

    }






    public static void DestroyAll(string tag)
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag(tag);
        foreach(GameObject target in targets)
        {
            Object.Destroy(target);
        }
    }

    
   




}
