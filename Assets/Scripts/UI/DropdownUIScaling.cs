using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class DropdownUIScaling : MonoBehaviour
{
    [SerializeField] RectTransform rectTransform;
    [SerializeField] RectTransform canvasTransform;
    Vector2 newSize;

    void Start()
    {

        newSize = new Vector2(rectTransform.sizeDelta.x, canvasTransform.sizeDelta.y + rectTransform.anchoredPosition.y - Mathf.Abs(transform.parent.localPosition.y - 50));
        rectTransform.sizeDelta = newSize;

    }



}
