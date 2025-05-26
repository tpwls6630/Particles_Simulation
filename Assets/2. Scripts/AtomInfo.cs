using System;
using UnityEngine;

[CreateAssetMenu(fileName = "AtomInfo", menuName = "Scriptable Objects/AtomInfo")]
public class AtomInfo : ScriptableObject
{
    [Header("원소 이름")]
    [SerializeField] private string _name;

    [Header("원자 번호")]
    [SerializeField] private int _atomicNumber;

    [Header("원자량")]
    [SerializeField] private float _relativeAtomicMass;

    [Header("반데르발스 반지름 (pm)")]
    [SerializeField] private float _radius;

    [Header("머티리얼")]
    [SerializeField] private Material _material;

    public string Name => _name;
    public int AtomicNumber => _atomicNumber;
    public float RelativeAtomicMass => _relativeAtomicMass;
    public float Radius => _radius * (float)Constants.AtomRadiusRegulizer;
    public Color Color => _material.color;
    public double Mass => _relativeAtomicMass * Constants.AtomicMassUnitInKg;
    public Material Material => _material;
}
