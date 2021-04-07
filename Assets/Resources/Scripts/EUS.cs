using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//Engine Utility System
/* The idea with EUS is to be a smaller library of generally useful functions, which can be used across different projects
 * and functions which can't really be placed somewhere else
 * e.g. destroy all objects with a specific tag */
public class EUS : MonoBehaviour
{
    public static int sceneIndex = 0;
    static SaveManager saveManager;
    private void Awake()
    {
        saveManager = gameObject.GetComponent<SaveManager>();
    }


    public static Transform SetChild(Transform parent, int childDepth, int[] childIndexes)    //Find child of target in X nest depth; with an array of child indexes, of array length X
    {                                                                           //Example : Transform T = SetChild(_transform, 3, new int[] { 1, 4, 7 });
        //SOLVE PROBLEM, YES
        if (childIndexes.Length == childDepth)
            for (int i = 0; i < childDepth; i++)
            {
                parent = parent.GetChild(childIndexes[i]);
            }
        return parent;
    }

    public static RectTransform SetChild(RectTransform parent, int childDepth, int[] childIndexes)    //SetChild, RectTransform variant, for UI
    {                                                                                   //Example : RectTransform RT = SetChild(_rectTransform, 2, new int[] { 0, 5 });
        //SOLVE PROBLEM, YES
        RectTransform child = parent;
        if (childIndexes.Length == childDepth)
            for (int i = 0; i < childDepth; i++)
            {
                child = child.GetChild(childIndexes[i]).GetComponent<RectTransform>();
            }
        return child;
    }


    public static void DestroyAll(string tag)
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag(tag);
        foreach(GameObject target in targets)
        {
            Destroy(target);
        }
    }

    public static void HideUI(string tag)
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject target in targets)
        {
            target.GetComponent<RectTransform>().localScale = new Vector3(0,0,0);
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

    public static void DisableUIComponent(string tag, string component)
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject target in targets)
        {
            if (component == "Button")
                target.GetComponent<Button>().enabled = false;
            else if (component == "Canvas")
                target.GetComponent<Canvas>().enabled = false;
        }
    }
    
    //Called by button
    public static void LoadScene(int index)
    {
        if (sceneIndex != 0) {
            saveManager.SaveData();
        }

        sceneIndex = index;
        SceneManager.LoadScene(index);
    }

    public void LoadSceneBtn(int index)
    {
        LoadScene(index);
    }
   
}
