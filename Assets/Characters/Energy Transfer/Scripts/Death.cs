using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnergyPool))]
public class Death : MonoBehaviour
{
    [Tooltip("This is turned off upon death")]
    public MeshRenderer Visual;
    private bool IsDead = false;

    public EnergyPool energyPool;

    private void Awake()
    {
        energyPool = GetComponent<EnergyPool>();
    }

    [ContextMenu("Kill")]
    public void Die()
    {
        if (IsDead) return;
        IsDead = true;
        if (Visual != null)
            Visual.enabled = false;

        if (energyPool != null)
            energyPool.ReleaseEnergy();
    }

    [ContextMenu("Resurrect")]
    public void Resurrect()
    {
        if (!IsDead) return;
        IsDead = false;
        if (Visual != null)
            Visual.enabled = true;
    }
}
