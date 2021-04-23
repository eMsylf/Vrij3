using BobJeltes;
using BobJeltes.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyPool : TriggerEvent
{
    [SerializeField]
    private float energy;
    public float Energy
    {
        get => energy;
        set
        {
            energy = value;
            energyEvents.OnEnergyUpdated.Invoke(energy);
        }
    }
    public bool DeactivateIncomingObject = true;

    [System.Serializable]
    public struct EnergyEvents
    {
        public FloatEvent OnAbsorbEnergy;
        public FloatEvent OnEnergyUpdated;
    }
    public EnergyEvents energyEvents = new EnergyEvents();

    public void AbsorbEnergy(GameObject energyObject)
    {
        Energy energyComponent = energyObject.GetComponent<Energy>();
        float absorbedEnergy;
        if (energyComponent == null)
            absorbedEnergy = 1f;
        else
            absorbedEnergy = energyComponent.amount;
        Energy += absorbedEnergy;
        if (DeactivateIncomingObject) energyObject.SetActive(false);
        energyEvents.OnAbsorbEnergy.Invoke(absorbedEnergy);
    }
}
