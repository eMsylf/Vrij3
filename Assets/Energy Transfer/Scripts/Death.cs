using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnergyBeing))]
public class Death : MonoBehaviour
{
    [Range(0f, 1f)]
    public float EnergyAbsorbedRatio = .1f;
    public MeshRenderer Visual;
    [SerializeField] private ParticleSystem nonAbsorbedParticles = null;
    public ParticleSystem GetNonAbsorbedParticles()
    {
        if (nonAbsorbedParticles == null)
        {
            Debug.LogError("Death particles have not been assigned");
        }
        return nonAbsorbedParticles;
    }
    [SerializeField] private ParticleSystem absorbedParticles = null;
    public ParticleSystem GetAbsorbedParticles()
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
        IsDead = true;
        if (Visual != null)
            Visual.enabled = false;

        ReleaseEnergy();
    }

    public void ReleaseEnergy()
    {
        int totalEnergy = GetComponent<EnergyBeing>().Energy;
        if (totalEnergy <= 0)
        {
            Debug.LogWarning("Boss has no, or negative energy.");
            return;
        }
        // Distribute energy
        int absorbedEnergy = Mathf.RoundToInt(totalEnergy * EnergyAbsorbedRatio);
        int releasedEnergy = totalEnergy - absorbedEnergy;
        Debug.Log("Boss energy released. Total energy: " + totalEnergy + ". Absorbed energy: " + absorbedEnergy + ". Released energy: " + releasedEnergy);

        // Set burst settings for non absorbed energy
        ParticleSystem.Burst nonAbsorbedBurst = new ParticleSystem.Burst();
        nonAbsorbedBurst.count = releasedEnergy;
        nonAbsorbedParticles?.emission.SetBurst(0, nonAbsorbedBurst);

        // Set burst settings for absorbed energy
        ParticleSystem.Burst absorbedBurst = new ParticleSystem.Burst();
        if (absorbedEnergy < 1)
        {
            absorbedParticles.emission.SetBursts(new ParticleSystem.Burst[0]);
        }
        else
        {
            // 2 energy orbs per cycle
            int cycles = absorbedEnergy / 2;
            // Divide the absorbed energy over the cycles
            absorbedBurst.count = absorbedEnergy / cycles;
            absorbedBurst.cycleCount = cycles;
            absorbedBurst.repeatInterval = .1f;
            absorbedParticles?.emission.SetBursts(new ParticleSystem.Burst[] { absorbedBurst });
        }

        GetNonAbsorbedParticles()?.Play();
    }

    public void Resurrect()
    {
        IsDead = false;
        if (Visual != null)
            Visual.enabled = true;
    }
}
