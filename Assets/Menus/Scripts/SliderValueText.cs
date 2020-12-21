using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.Remoting.Messaging;

namespace BobJeltes.Menu
{
    public class SliderValueText : MonoBehaviour
    {
        public Slider slider;
        public TextMeshProUGUI TMPComponent;
        TextMeshProUGUI GetTMPComponent()
        {
            if (TMPComponent == null)
            {
                TMPComponent = GetComponent<TextMeshProUGUI>();
            }
            return TMPComponent;
        }

        public enum NumberFormat
        {
            Raw,
            Percentage,
            ZeroToOne,
            Custom
        }
        public NumberFormat numberFormat = NumberFormat.Percentage;

        public string GetNumberFormat() => GetNumberFormat(numberFormat);

        public string GetNumberFormat(NumberFormat format)
        {
            switch (format)
            {
                case NumberFormat.Raw:
                    return string.Empty;
                case NumberFormat.Percentage:
                    return "##0%";
                case NumberFormat.ZeroToOne:
                    return "0.00";
                case NumberFormat.Custom:
                    return CustomFormat;
                default:
                    return string.Empty;
            }
        }
        public string CustomFormat = "";

        [Tooltip("If auto-update is true, you DO NOT need to reference UpdateValue in the OnValueChanged of the Slider component.")]
        public bool autoUpdate = true;

        void Awake()
        {
            if (autoUpdate)
            {
                UpdateValue();
                slider.onValueChanged.AddListener(_ => UpdateValue());
            }
        }

        public void UpdateValue()
        {
            UpdateValue(slider.value);
        }

        public void UpdateValue(float value)
        {
            string formattedSliderValue = value.ToString(GetNumberFormat());
            GetTMPComponent().text = formattedSliderValue; //TODO https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-numeric-format-strings
            //Debug.Log("Update value to " + formattedSliderValue, this);
        }
    }
}