using UnityEngine;

public class ParticleSystemStopHandler : MonoBehaviour
{
    public FuelingStation fuelingStation; // Reference to the FuelingStation script

    void OnParticleSystemStopped()
    {
        if (fuelingStation != null)
        {
            fuelingStation.OnParticleSystemStopped();
        }
    }
}
