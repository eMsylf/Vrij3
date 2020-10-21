using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class GridLayoutGroupAutoScaler : GridLayoutGroup
{
    public bool fitX = true;
    public bool fitY = true;

    public Vector2 CalculateCellSize()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector2 newCellSize = cellSize;
        if (fitX)
        {
            newCellSize.x = CalculateCellSizeOneAxis(padding.horizontal, spacing.x, rectTransform.sizeDelta.x, startAxis == Axis.Horizontal);
        }
        if (fitY)
        {
            newCellSize.y = CalculateCellSizeOneAxis(padding.vertical, spacing.y, rectTransform.sizeDelta.y, startAxis == Axis.Vertical);
        }

        Debug.Log("Calculated cell size: " + newCellSize);
        return newCellSize;
    }

    public float CalculateCellSizeOneAxis(float padding, float spacing, float sizeDelta, bool childCountDependent)
    {
        float startingPixels;
        float totalSpacing;
        int childCount = (childCountDependent?transform.childCount:1);
        float calculatedCellSize;

        startingPixels = sizeDelta;
        totalSpacing = spacing * childCount;

        float pixelsRemaining = startingPixels - (padding + totalSpacing);
        calculatedCellSize = pixelsRemaining / childCount;
        return calculatedCellSize;
    }

    public void AutoScale()
    {
        cellSize = CalculateCellSize();
        CalculateLayoutInputHorizontal();
        CalculateLayoutInputVertical();
    }
}
