using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct AtomComposition
{
    public AtomInfo AtomInfo;
    public int Count;
}

[CreateAssetMenu(fileName = "ParticleInfo", menuName = "Scriptable Objects/ParticleInfo")]
public class ParticleInfo : ScriptableObject
{
    [Header("입자 이름")]
    [SerializeField] private string _name;

    [Header("원자 구성")]
    [SerializeField] private List<AtomComposition> _atomCompositions = new();

    private float _relativeAtomicMass;

    public string Name => _name;
    public float RelativeAtomicMass => _relativeAtomicMass;
    public double Mass => _relativeAtomicMass * Constants.AtomicMassUnitInKg;

    public Action<Particle> ExternalForce;

    private void OnValidate()
    {
        _relativeAtomicMass = CalculateRelativeAtomicMass();
    }

    private float CalculateRelativeAtomicMass()
    {
        float relativeAtomicMass = 0f;
        foreach (var atomComposition in _atomCompositions)
        {
            relativeAtomicMass += atomComposition.AtomInfo.RelativeAtomicMass * atomComposition.Count;
        }
        return relativeAtomicMass;
    }
}
