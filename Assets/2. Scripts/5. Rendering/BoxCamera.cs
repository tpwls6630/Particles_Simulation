using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class BoxCamera : MonoBehaviour
{
    private Camera _camera;
    private Dictionary<ParticleInfo, int> _particleCullingBits;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        _particleCullingBits = new Dictionary<ParticleInfo, int>();
        foreach (var particle in ParticleManager.Instance.ParticlePrefabs)
        {
            ParticleInfo particleInfo = particle.Key;
            GameObject particlePrefab = particle.Value;
            _particleCullingBits.Add(particleInfo, particlePrefab.layer);
        }
    }

    public void SetParticleView(ParticleInfo particleInfo, bool isContainView)
    {
        if (_particleCullingBits.TryGetValue(particleInfo, out int layer))
        {
            if (isContainView)
            {
                _camera.cullingMask |= (1 << layer);
            }
            else
            {
                _camera.cullingMask &= ~(1 << layer);
            }
        }
        else
        {
            Debug.LogWarning($"ParticleInfo {particleInfo} not found in _particleCullingBits.");
        }
    }

}