using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RectTransformExt
{
    public enum Directions2D
    {
        UP,
        DOWN,
        LEFT,
        RIGHT,
        UP_LEFT,
        UP_RIGHT,
        DOWN_LEFT,
        DOWN_RIGHT
    }

    public static Vector2 GetDirectionAsVector2(Directions2D directions)
    {
        Vector2 direction = Vector2.zero;

        switch (directions)
        {
            case Directions2D.UP:
                direction = Vector2.up;
                break;
            case Directions2D.DOWN:
                direction = Vector2.down;
                break;
            case Directions2D.LEFT:
                direction = Vector2.left;
                break;
            case Directions2D.RIGHT:
                direction = Vector2.right;
                break;
            case Directions2D.UP_LEFT:
                direction = Vector2.up + Vector2.left;
                break;
            case Directions2D.UP_RIGHT:
                direction = Vector2.up + Vector2.right;
                break;
            case Directions2D.DOWN_LEFT:
                direction = Vector2.down + Vector2.left;
                break;
            case Directions2D.DOWN_RIGHT:
                direction = Vector2.down + Vector2.right;
                break;
            default:
                break;
        }

        return direction;
    }

    #region Extension Methods
    public static void SetAnchoredPosition(this RectTransform transform, Directions2D vectorDir, Vector2 relativePosition, float multiplier = 1.0f)
    {
        Vector2 directionVector = GetDirectionAsVector2(vectorDir);

        transform.anchoredPosition = (directionVector * relativePosition) * multiplier;
    }

    public static void SetSize(this RectTransform transform, Directions2D vectorDir, Vector2 newSize, float multiplier = 1.0f)
    {
        Vector2 directionVector = GetDirectionAsVector2(vectorDir);

        transform.sizeDelta = (directionVector * newSize) * multiplier;
    }
    public static void SetSize(this RectTransform transform, Directions2D vectorDir, float newValue, float multiplier = 1.0f)
    {
        Vector2 directionVector = GetDirectionAsVector2(vectorDir);

        transform.sizeDelta = directionVector * newValue * multiplier;
    }
    public static void AddSize(this RectTransform transform, Directions2D vectorDir, Vector2 addedSize, float multiplier = 1.0f)
    {
        Vector2 directionVector = GetDirectionAsVector2(vectorDir);

        transform.sizeDelta += (directionVector * addedSize) * multiplier;
    }
    public static void AddSize(this RectTransform transform, Directions2D vectorDir, float addedValue, float multiplier = 1.0f)
    {
        Vector2 directionVector = GetDirectionAsVector2(vectorDir);

        transform.sizeDelta += (directionVector * addedValue) * multiplier;
    }



    #endregion


}
