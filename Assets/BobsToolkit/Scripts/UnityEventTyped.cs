using UnityEngine;
using UnityEngine.Events;

namespace BobJeltes.Events
{
    [System.Serializable]
    public class UnityEventGameObject : UnityEvent<GameObject> { }

    [System.Serializable]
    public class UnityEventFloat : UnityEvent<float> { }

    [System.Serializable]
    public class UnityEventInt : UnityEvent<int> { }

    [System.Serializable]
    public class UnityEventString : UnityEvent<string> { }

    [System.Serializable]
    public class UnityEventBool : UnityEvent<bool> { }

    [System.Serializable]
    public class UnityEventData
    {
        public string EventString;
        public float EventFloat;
        public int EventInt;
        public GameObject EventObject;
        public Color color;
    }

    [System.Serializable]
    public class UnityEventWithData {
        public UnityEventData eventData = new UnityEventData();
        public DataEvent Event;
    }

    [System.Serializable]
    public class DataEvent : UnityEvent<UnityEventData>
    {

    }
}
