using BobJeltes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScalingSliderWithCallbacks : SliderExtension
{
    public RectTransform targetRect;
    public enum Direction
    {
        /// <summary>
        /// From the left to the right
        /// </summary>
        LeftToRight,

        /// <summary>
        /// From the right to the left
        /// </summary>
        RightToLeft,

        /// <summary>
        /// From the bottom to the top.
        /// </summary>
        BottomToTop,

        /// <summary>
        /// From the top to the bottom.
        /// </summary>
        TopToBottom,
    }
    [SerializeField]
    private Direction m_Direction = Direction.LeftToRight;

    public enum Axis
    {
        Horizontal = 0,
        Vertical = 1
    }
    public Axis axis { get { return (m_Direction == Direction.LeftToRight || m_Direction == Direction.RightToLeft) ? Axis.Horizontal : Axis.Vertical; } }
    bool reverseValue { get { return m_Direction == Direction.RightToLeft || m_Direction == Direction.TopToBottom; } }

    public float normalizedValue
    {
        get
        {
            if (Mathf.Approximately(0f, maxValue))
                return 0;
            return Mathf.InverseLerp(0f, maxValue, this.value);
        }
        set
        {
            this.value = (int)Mathf.Lerp(0f, maxValue, value);
        }
    }

    public override void SetValue(float newValue)
    {
        base.SetValue(newValue);
        Vector2 anchorMin = Vector2.zero;
        Vector2 anchorMax = Vector2.one;

        Image m_FillImage = targetRect.GetComponent<Image>();
        if (m_FillImage != null && m_FillImage.type == Image.Type.Filled)
        {
            Debug.Log("Fill image found");
            m_FillImage.fillAmount = normalizedValue;
        }
        else
        {
            Debug.Log("Fill image not found");
            if (reverseValue)
                anchorMin[(int)axis] = 1 - normalizedValue;
            else
                anchorMax[(int)axis] = normalizedValue;
        }
        Debug.Log("Normalized value: " + normalizedValue);

        targetRect.anchorMin = anchorMin;
        targetRect.anchorMax = anchorMax;
        Debug.Log("Set anchors: " + anchorMin + " " + anchorMax);
    }
}
