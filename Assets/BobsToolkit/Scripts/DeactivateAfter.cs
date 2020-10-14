using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateAfter : Timer
{
    public override void OnTimeOver()
    {
        base.OnTimeOver();
        gameObject.SetActive(false);
    }
}
