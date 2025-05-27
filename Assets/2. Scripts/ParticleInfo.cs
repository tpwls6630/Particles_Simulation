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

    [Header("입자 색상")]
    [SerializeField] private Color _color;

    [SerializeField] private float _relativeAtomicMass;

    [SerializeField] private int _degreeOfFreedom;

    public string Name => _name;
    public float RelativeAtomicMass => _relativeAtomicMass;
    public double Mass => _relativeAtomicMass * Constants.AtomicMassUnitInKg;
    public Color Color => _color;
    public int DegreeOfFreedom => _degreeOfFreedom;
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
