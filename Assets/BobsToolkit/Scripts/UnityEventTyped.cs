using System;
using UnityEngine;
using UnityEngine.Events;

namespace BobJeltes.Events
{
    [Serializable]
    public class UnityEventGameObject : UnityEvent<GameObject> { }

    [Serializable]
    public class UnityEventFloat : UnityEvent<float> { }

    [Serializable]
    public class UnityEventInt : UnityEvent<int> { }

    [Serializable]
    public class UnityEventString : UnityEvent<string> { }

    [Serializable]
    public class UnityEventBool : UnityEvent<bool> { }

    [Serializable]
    public class UnityEventCustomType<T> : UnityEvent<T> { }
}
