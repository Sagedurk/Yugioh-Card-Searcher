using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static RectTransformExt;

public class TextExtension : Text
{
    #region Variables
    public new string text { get => base.text; private set => base.text = value; }

    

    #endregion


    #region SetText();
    public void SetText(string newText)
    {
        text = newText;
    }
    public void SetText(string constPrefixText, string newText)
    {
        text = constPrefixText + newText;
    }
    #endregion


    #region SetAnchoredPosition();

    public void SetAnchoredPosition(Directions2D vectorDir, RectTransform relativeTransform, float multiplier = 1.0f)
    {
        Vector2 directionVector = GetDirectionAsVector2(vectorDir);

        rectTransform.anchoredPosition = (directionVector * relativeTransform.anchoredPosition) * multiplier;
    }

    public void SetAnchoredPosition(Directions2D vectorDir, Vector2 relativePosition, float multiplier = 1.0f)
    {
        Vector2 directionVector = GetDirectionAsVector2(vectorDir);

        rectTransform.anchoredPosition = (directionVector * relativePosition) * multiplier;
    }
    #endregion



    #region Helper Functions
    
    #endregion
}
