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
            float temperature = MaxwellBoltzmannAnalysis.InferTemperatureFromSpeeds(speeds, pInfo.RelativeAtomicMass, pInfo.DegreeOfFreedom);
            temperatureData[pInfo] = temperature;
        }
    }
}