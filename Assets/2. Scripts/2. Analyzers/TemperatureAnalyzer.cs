using System.Collections.Generic;
using UnityEngine;


public class TemperatureAnalyzer : MonoBehaviour
{
    public void Analyze(Dictionary<ParticleInfo, List<float>> speedData)
    {
        Dictionary<ParticleInfo, float> temperatureData = new();
        foreach (var particleItem in speedData)
        {
            ParticleInfo pInfo = particleItem.Key;
            List<float> speeds = particleItem.Value;
            float rms = MaxwellBoltzmannAnalysis.RMSFromSpeeds(speeds);
            float temperature = MaxwellBoltzmannAnalysis.TemperatureFromSpeeds(rms, pInfo.RelativeAtomicMass);
            temperatureData[pInfo] = temperature;
        }
    }
}