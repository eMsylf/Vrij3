using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class UnityEventCustom : MonoBehaviour
{
    public OnClickData onClickData;
    public MyOnClickEvent myOnClickEvent;

    public void OnClick()
    {
        myOnClickEvent.Invoke(onClickData);
    }
}

[System.Serializable]
public class OnClickData
{
    public Color myColor;
    public int myInt;
    public float myFloat;
    //etc.
}

[System.Serializable]
public class MyOnClickEvent : UnityEvent<OnClickData> { }