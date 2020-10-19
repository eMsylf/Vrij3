using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
[RequireComponent(typeof(RectTransform))]
public class GridLayoutGroupAutoScaler : MonoBehaviour
{
    public Vector2 CalculateCellSize()
    {

        return CalculateCellSize(GetComponent<GridLayoutGroup>().startAxis);
    }


    public Vector2 CalculateCellSize(GridLayoutGroup.Axis axis)
    {
        GridLayoutGroup gridLayoutGroup = GetComponent<GridLayoutGroup>();
        RectTransform rectTransform = GetComponent<RectTransform>();

        float startingPixels;
        float padding;
        float totalSpacing;
        int childCount = transform.childCount;
        float pixelsRemaining = 0f;
        float calculatedCellSize;
        Vector2 newCellSize = gridLayoutGroup.cellSize;

        switch (axis)
        {
            case GridLayoutGroup.Axis.Horizontal:
                startingPixels = rectTransform.sizeDelta.x;
                padding = gridLayoutGroup.padding.horizontal;
                totalSpacing = gridLayoutGroup.spacing.x * childCount;

                pixelsRemaining = startingPixels - (padding + totalSpacing);
                calculatedCellSize =  pixelsRemaining / childCount;
                newCellSize.x = calculatedCellSize;
                break;
            case GridLayoutGroup.Axis.Vertical:
                startingPixels = rectTransform.sizeDelta.y;
                padding = gridLayoutGroup.padding.vertical;
                totalSpacing = gridLayoutGroup.spacing.y * childCount;

                pixelsRemaining = startingPixels - (padding + totalSpacing);
                calculatedCellSize =  pixelsRemaining / childCount;
                newCellSize.y = calculatedCellSize;
                break;
        }


        Debug.Log("Pixels remaining on the selected axis: " + pixelsRemaining + ". Calculated cell size: " + newCellSize);
        return newCellSize;
    }

    public void AutoScale()
    {
        GridLayoutGroup gridLayoutGroup = GetComponent<GridLayoutGroup>();
        
        gridLayoutGroup.cellSize = CalculateCellSize();
        gridLayoutGroup.CalculateLayoutInputHorizontal();
        gridLayoutGroup.CalculateLayoutInputVertical();
    }
}
