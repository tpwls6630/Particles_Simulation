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
    [SerializeField] private ParticleCreateSelector _particleCreateSelector;

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

    public void CreateParticles(ParticleInfo particleInfo, int count, float temperature)
    {
        for (int i = 0; i < count; i++)
        {
            CreateParticle(particleInfo, temperature);
        }
        UpdateParticleCount(particleInfo);
    }

    public void CreateParticle(ParticleInfo particleInfo, float temperature)
    {
        GameObject particle = Instantiate(
            _particlePrefabs[particleInfo],
            UnityEngine.Random.insideUnitSphere * 5f + GameManager.Instance.BoxCenter,
            Quaternion.identity,
            _particleGroup.transform);

        Particle particleComponent = particle.GetComponent<Particle>();
        particleComponent.SetParticleInfo(particleInfo);

        // 맥스웰-볼츠만 분포의 RMS 속도 계산
        double mass = particleInfo.Mass; // kg 단위
        double rmsSpeed = Math.Sqrt(3 * Constants.BoltzmannConstant * temperature / mass);

        // 랜덤한 방향으로 RMS 속력 적용
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere.normalized;
        Vector3 initialVelocity = randomDirection * (float)rmsSpeed;
        print(initialVelocity.magnitude);
        particleComponent.SetVelocity(initialVelocity);

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
        UpdateParticleCount(particleInfo);
    }

    private void UpdateParticleCount(ParticleInfo particleInfo)
    {
        if (_particleCreateSelector != null)
        {
            _particleCreateSelector.SetParticleCount(particleInfo, _particleDict[particleInfo].Count);
        }
    }
}
