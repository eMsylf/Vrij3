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


        void Awake()
        {
            UpdateValue();
            slider.onValueChanged.AddListener(_ => UpdateValue());
        }

        void UpdateValue()
        {
            string formattedSliderValue = slider.value.ToString("##0%");
            GetTMPComponent().text = formattedSliderValue; //TODO https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-numeric-format-strings
            Debug.Log("Update value to " + formattedSliderValue);
        }
    }
}