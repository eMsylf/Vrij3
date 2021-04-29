using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[HelpURL("https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-numeric-format-strings")]
public class ValueText : MonoBehaviour
{
    public TextMeshProUGUI TMPComponent;
    TextMeshProUGUI GetTMPComponent()
    {
        if (TMPComponent == null) TMPComponent = GetComponent<TextMeshProUGUI>();
        return TMPComponent;
    }

    public enum NumberFormat
    {
        Raw,
        Integer,
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
            case NumberFormat.Integer:
                return "0";
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
    [Tooltip("For more custom number formats, check the help url of this script.")]
    public string CustomFormat = "";

    public void UpdateValue(float value)
    {
        string formattedSliderValue = value.ToString(GetNumberFormat());
        GetTMPComponent().text = formattedSliderValue; 
    }
}
