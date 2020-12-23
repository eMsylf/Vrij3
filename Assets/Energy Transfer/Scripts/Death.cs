using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnergyBeing))]
public class Death : MonoBehaviour
{
    [Tooltip("This is turned off upon death")]
    public MeshRenderer Visual;
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
    private bool IsDead = false;

    public void Die()
    {
        if (IsDead) return;
        IsDead = true;
        if (Visual != null)
            Visual.enabled = false;

        ReleaseEnergy();
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
            // Divide the absorbed energy over the cycles
            absorbedBurst.count = absorbedEnergy / cycles;
            absorbedBurst.cycleCount = cycles;
            absorbedBurst.repeatInterval = .1f;
        }
    }

    public void ReleaseEnergy()
    {
        CalculateBursts(GetComponent<EnergyBeing>().Energy, out ParticleSystem.Burst nonAbsorbedBurst, out ParticleSystem.Burst absorbedBurst);
        if (InstantiateParticles)
        {
            ParticleSystem instantiatedAbsorb = Instantiate(AbsorbedPrefab, transform.position, Quaternion.identity);
            ParticleSystem instantiatedFree = Instantiate(FreePrefab, transform.position, Quaternion.identity);
            instantiatedAbsorb.emission.SetBurst(0, absorbedBurst);
            instantiatedFree.emission.SetBursts(new ParticleSystem.Burst[] { nonAbsorbedBurst });
            instantiatedAbsorb.Play();
            instantiatedFree.Play();
            GameObjectEmitter energyEmitter = Instantiate(EnergyEmitter, transform.position, Quaternion.identity);

            energyEmitter.numberOfObjects = GetComponent<EnergyBeing>().Energy;
            energyEmitter.Emit();
        }
        else
        {
            freeParticles?.emission.SetBurst(0, nonAbsorbedBurst);
            absorbedParticles?.emission.SetBursts(new ParticleSystem.Burst[] { absorbedBurst });
            GetNonAbsorbedParticles()?.Play();
        }
    }

    public void Resurrect()
    {
        if (!IsDead) return;
        IsDead = false;
        if (Visual != null)
            Visual.enabled = true;
    }
}
