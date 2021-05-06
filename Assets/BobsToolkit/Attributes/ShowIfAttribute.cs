using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BobJeltes.Attributes
{
    public class ShowIfAttribute : PropertyAttribute
    {
        private string valueName;
        private bool boolValue;

        private int[] enumIndices;

        public string ValueName { get => valueName; private set => valueName = value; }
        public bool BoolValue { get => boolValue; private set => boolValue = value; }
        public int[] EnumIndices { get => enumIndices; private set => enumIndices = value; }

        /// <summary>
        /// Only shows the parameter that this is used on, if the chosen value (with the value name) matches the chosen value. NOTE: Does not work in combination with other attributes.
        /// </summary>
        /// <param name="boolValueName">The name of the reference value that determines whether the property is shown in the inspector or not</param>
        /// <param name="boolValue">The bool value at which you want the parameter to be shown</param>
        public ShowIfAttribute(string boolValueName, bool boolValue = true)
        {
            ValueName = boolValueName;
            this.BoolValue = boolValue;
        }
        /// <summary>
        /// Only shows the parameter that this is used on, if the chosen parameter matches the chosen value. NOTE: Does not work in combination with other attributes.
        /// </summary>
        /// <param name="enumValueName">The name of the enum value.</param>
        /// <param name="enumIndices">The index of the enum that you want the parameter to be shown at.</param>
        public ShowIfAttribute(string enumValueName, params int[] enumIndices)
        {
            ValueName = enumValueName;
            this.EnumIndices = enumIndices;
        }
    }
}