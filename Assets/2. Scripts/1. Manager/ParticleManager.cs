using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

[Serializable]
public class ParticleInspector
{
    public ParticleInfo particleInfo;
    public GameObject particlePrefab;
}

public class ParticleManager : Singleton<ParticleManager>
{
    [SerializeField] private List<ParticleInspector> _particleInspectors;
    [SerializeField] private float _particleSpeed;
    [SerializeField] private float _initparticleCount;

    private GameObject _particleGroup;

    private int _particleCount;
    private Dictionary<ParticleInfo, List<Particle>> _particleDict;

    private List<ParticleInfo> _particleInfos;
    private Dictionary<ParticleInfo, GameObject> _particlePrefabs;

    public List<ParticleInfo> ParticleInfos
    {
        get
        {
            if (_particleInfos == null)
            {
                _particleInfos = new List<ParticleInfo>();
                foreach (var inspector in _particleInspectors)
                {
                    _particleInfos.Add(inspector.particleInfo);
                }
            }
            return _particleInfos;
        }
    }

    public Dictionary<ParticleInfo, List<Particle>> ParticleDict => _particleDict;

    public Dictionary<ParticleInfo, GameObject> ParticlePrefabs => _particlePrefabs;

    private void Awake()
    {
        _particleGroup = new GameObject("ParticleGroup");

        _particleCount = 0;

        _particlePrefabs = new Dictionary<ParticleInfo, GameObject>();
        foreach (var inspector in _particleInspectors)
        {
            _particlePrefabs.Add(inspector.particleInfo, inspector.particlePrefab);
        }

        _particleDict = new();
        foreach (var particleInfo in ParticleInfos)
        {
            _particleDict.Add(particleInfo, new List<Particle>());
        }
    }

    public void CreateParticles(ParticleInfo particleInfo, int count)
    {
        for (int i = 0; i < count; i++)
        {
            CreateParticle(particleInfo);
        }
    }

    public void CreateParticle(ParticleInfo particleInfo)
    {
        GameObject particle = Instantiate(
            _particlePrefabs[particleInfo],
            UnityEngine.Random.insideUnitSphere * 5f + GameManager.Instance.BoxCenter,
            Quaternion.identity,
            _particleGroup.transform);

        Particle particleComponent = particle.GetComponent<Particle>();
        particleComponent.SetParticleInfo(particleInfo);
        particleComponent.SetVelocity(UnityEngine.Random.insideUnitSphere * _particleSpeed);

        _particleDict[particleInfo].Add(particleComponent);

        _particleCount++;
    }

    public void DestroyParticles(ParticleInfo particleInfo, int count)
    {
        List<Particle> particles = _particleDict[particleInfo];
        for (int i = 0; i < count; i++)
        {
            if (particles.Count == 0)
                break;

            Particle particle = particles[particles.Count - 1];
            particles.RemoveAt(particles.Count - 1);
            Destroy(particle.gameObject);
        }
    }
}
