using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VeolocityAnalyzer : Singleton<VeolocityAnalyzer>
{
    private Dictionary<ParticleInfo, List<Particle>> _particles => ParticleManager.Instance.ParticleDict;
    private float _analyzeTime => GameManager.Instance.AnalyzeTime;

    private void Awake()
    {
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
            Dictionary<ParticleInfo, List<float>> _velocities = new Dictionary<ParticleInfo, List<float>>();
            foreach (var particleItem in _particles)
            {
                ParticleInfo pInfo = particleItem.Key;
                _velocities[pInfo] = new List<float>();

                foreach (Particle particle in particleItem.Value)
                {
                    _velocities[pInfo].Add(particle.GetSpeed());
                }
            }

            ChartManager.Instance.DrawChart(_velocities);

            yield return new WaitForSeconds(_analyzeTime);
        }
    }
}
