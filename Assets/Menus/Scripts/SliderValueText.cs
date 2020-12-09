using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BobJeltes.Menu
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class SliderValueText : MonoBehaviour
    {
        public Slider slider;
        TextMeshProUGUI TMPComponent;
        TextMeshProUGUI GetTMPComponent()
        {
            if (TMPComponent == null)
            {
                TMPComponent = GetComponent<TextMeshProUGUI>();
            }
            return TMPComponent;
        }

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
            string formattedSliderValue = value.ToString("##0%");
            GetTMPComponent().text = formattedSliderValue; //TODO https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-numeric-format-strings
            //Debug.Log("Update value to " + formattedSliderValue, this);
        }
    }
}