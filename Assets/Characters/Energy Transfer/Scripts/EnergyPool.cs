using BobJeltes;
using BobJeltes.Events;
using System;
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
    [Tooltip("Absorbed energy objects will be turned off.")]
    public bool DeactivateIncomingObjects = true;

    [Tooltip("The part of the energy that can be absorbed by energy collectors. The rest gets released into the air.")]
    [Range(0f, 1f)]
    public float EnergyAbsorbedRatio = .1f;
    [Tooltip("This creates new objects from given particle system prefabs. Useful for if this object or its parent is deactivated when 'release energy' is called")]
    public bool InstantiateParticles;
    [Header("Instantiate")]
    public ParticleSystem FreePrefab;
    public ParticleSystem AbsorbedPrefab;
    public GameObjectEmitter EnergyEmitter;
    [Header("No instantiation")]
    [SerializeField] private ParticleSystem freeParticles = null;

    [System.Serializable]
    public struct EnergyEvents
    {
        public FloatEvent OnEnergyAbsorbed;
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
        if (DeactivateIncomingObjects) energyObject.SetActive(false);
        energyEvents.OnEnergyAbsorbed.Invoke(absorbedEnergy);
    }

    public ParticleSystem GetNonAbsorbedParticles()
    {
        if (freeParticles == null)
        {
            Debug.LogError("Death particles have not been assigned");
        }
        return freeParticles;
    }
    [SerializeField] private ParticleSystem absorbedParticles = null;
    public ParticleSystem GetFreeParticles()
    {
        if (absorbedParticles == null)
        {
            Debug.LogError("Death particles have not been assigned");
        }
        return absorbedParticles;
    }

    private void CalculateBursts(int totalEnergy, out ParticleSystem.Burst nonAbsorbedBurst, out ParticleSystem.Burst absorbedBurst)
    {
        nonAbsorbedBurst = new ParticleSystem.Burst();
        absorbedBurst = new ParticleSystem.Burst();
        if (totalEnergy <= 0)
        {
            Debug.LogWarning("Boss has no, or negative energy.");
            return;
        }
        // Distribute energy
        int absorbedEnergy = Mathf.RoundToInt(totalEnergy * EnergyAbsorbedRatio);
        int releasedEnergy = totalEnergy - absorbedEnergy;
        Debug.Log("Boss energy released. Total energy: " + totalEnergy + ". Absorbed energy: " + absorbedEnergy + ". Released energy: " + releasedEnergy);

        // Non-absorbed energy
        nonAbsorbedBurst.count = releasedEnergy;

        // Absorbed energy
        if (absorbedEnergy > 0)
        {
            // 2 energy orbs per cycle
            int cycles = absorbedEnergy / 2;
            if (cycles > 0)
            {
                // Divide the absorbed energy over the cycles
                absorbedBurst.count = absorbedEnergy / cycles;
                absorbedBurst.cycleCount = cycles;
                absorbedBurst.repeatInterval = .1f;
            }
        }
    }

    [ContextMenu("Release energy")]
    public void ReleaseEnergy()
    {
        CalculateBursts(Mathf.RoundToInt(Energy), out ParticleSystem.Burst nonAbsorbedBurst, out ParticleSystem.Burst absorbedBurst);
        if (InstantiateParticles)
        {
            ParticleSystem instantiatedAbsorb = Instantiate(AbsorbedPrefab, transform.position, Quaternion.identity);
            ParticleSystem instantiatedFree = Instantiate(FreePrefab, transform.position, Quaternion.identity);
            instantiatedAbsorb.emission.SetBurst(0, absorbedBurst);
            instantiatedFree.emission.SetBursts(new ParticleSystem.Burst[] { nonAbsorbedBurst });
            instantiatedAbsorb.Play();
            instantiatedFree.Play();
            GameObjectEmitter energyEmitter = Instantiate(EnergyEmitter, transform.position, Quaternion.identity);

            energyEmitter.numberOfObjects = Mathf.RoundToInt(GetComponent<EnergyPool>().Energy);
            energyEmitter.Emit();
        }
        else
        {
            freeParticles?.emission.SetBurst(0, nonAbsorbedBurst);
            absorbedParticles?.emission.SetBursts(new ParticleSystem.Burst[] { absorbedBurst });
            GetNonAbsorbedParticles()?.Play();
        }
    }
}
