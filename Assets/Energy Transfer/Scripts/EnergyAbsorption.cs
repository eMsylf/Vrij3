using BobJeltes;
using BobJeltes.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyAbsorption : TriggerEvent
{
    [SerializeField]
    private float energy;
    public float Energy
    {
        get => energy;
        set
        {
            Debug.Log("Energy value updated from " + energy + " to " + value);
            energy = value;
            OnEnergyUpdated.Invoke(energy);
        }
    }
    public FloatEvent OnAbsorbEnergy;
    public bool DeactivateIncomingObject = true;
    public FloatEvent OnEnergyUpdated;

    public void AbsorbEnergy(GameObject energyObject)
    {
        //Debug.Log("Absorb energy", energyObject);
        Energy energyComponent = energyObject.GetComponent<Energy>();
        float absorbedEnergy;
        if (energyComponent == null)
        {
            absorbedEnergy = 1f;
        }
        else
        {
            absorbedEnergy = energyComponent.amount;
        }
        Energy += absorbedEnergy;
        if (DeactivateIncomingObject) energyObject.SetActive(false);
        OnAbsorbEnergy.Invoke(absorbedEnergy);
    }
}
