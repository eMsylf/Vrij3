using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderScaling : MonoBehaviour
{

    public RectTransform targetRect;

    public enum Direction
    {
        TopBottom,
        BottomTop,
        LeftRight,
        RightLeft
    }
    public Direction fillDirection;


    public void SetValue(int newValue)
    {
        Vector2 anchorMin = Vector2.zero;
        Vector2 anchorMax = Vector2.one;

        switch (fillDirection)
        {
            case Direction.TopBottom:
                //if (m_FillImage != null && m_FillImage.type == Image.Type.Filled)
                //{
                //    m_FillImage.fillAmount = normalizedValue;
                //}
                //else
                //{
                //    if (reverseValue)
                //        anchorMin[(int)axis] = 1 - normalizedValue;
                //    else
                //        anchorMax[(int)axis] = normalizedValue;
                //}

                //m_FillRect.anchorMin = anchorMin;
                //m_FillRect.anchorMax = anchorMax;


                break;
            case Direction.BottomTop:
                break;
            case Direction.LeftRight:
                break;
            case Direction.RightLeft:
                break;
            default:
                break;
        }

        targetRect.anchorMin = anchorMin;
    }
}
