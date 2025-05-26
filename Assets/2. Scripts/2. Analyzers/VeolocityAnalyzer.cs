using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VeolocityAnalyzer : Singleton<VeolocityAnalyzer>
{
    private Dictionary<ParticleInfo, List<Particle>> _particles => ParticleManager.Instance.ParticleDict;
    private Dictionary<ParticleInfo, List<float>> _velocities;
    private float _analyzeTime => GameManager.Instance.AnalyzeTime;

    private void Awake()
    {
        _velocities = new Dictionary<ParticleInfo, List<float>>();
    }

    private void Start()
    {
        AnalyzeHelper();
    }

    private void AnalyzeHelper()
    {
        StartCoroutine(AnalyzeCoroutine());
    }

    private IEnumerator AnalyzeCoroutine()
    {

        while (true)
        {
            int totalParticleCount = 0;
            foreach (var particleItem in _particles)
            {
                totalParticleCount += particleItem.Value.Count;
            }

            int batchCount = (int)(_analyzeTime / Time.deltaTime);
            int batchSize = totalParticleCount / batchCount;

            if (_particles.Count == 0)
            {
                yield return null;
                continue;
            }

            int particleGroupIndex = 0;
            ParticleInfo particleInfo = _particles.ElementAt(particleGroupIndex).Key;
            int particleIndex = 0;

            for (int i = 0; i < batchCount; i++)
            {
                for (int j = 0; j < batchSize; j++)
                {
                    while (true)
                    {
                        if (particleGroupIndex >= _particles.Count)
                            break;
                        if (particleIndex >= _particles[particleInfo].Count)
                        {
                            particleGroupIndex++;
                            particleInfo = _particles.ElementAt(particleGroupIndex).Key;
                            particleIndex = 0;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (particleGroupIndex >= _particles.Count)
                        break;

                    if (!_velocities.ContainsKey(particleInfo))
                    {
                        _velocities.Add(particleInfo, new List<float>());
                    }

                    if (_velocities[particleInfo].Count != _particles[particleInfo].Count)
                    {
                        _velocities[particleInfo].Clear();
                        for (int k = 0; k < _particles[particleInfo].Count; k++)
                        {
                            _velocities[particleInfo].Add(0);
                        }
                    }

                    _velocities[particleInfo][particleIndex] = _particles[particleInfo][particleIndex].GetSpeed();
                    particleIndex++;
                }

                yield return null;
            }
            ChartManager.Instance.DrawChart(_velocities);
        }
    }
}
