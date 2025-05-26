using UnityEngine;

[CreateAssetMenu(fileName = "RenderingSetting", menuName = "Scriptable Objects/RenderingSetting")]
public class RenderingSetting : ScriptableObject
{
    [SerializeField] public float ParticleSize;
}
