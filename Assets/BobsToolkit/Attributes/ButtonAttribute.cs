using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BobJeltes.Attributes
{
    public class ButtonAttribute : PropertyAttribute
    {
        public string name;

        public ButtonAttribute(string name)
        {
            this.name = name;
        }
    }
}
