using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class Atom : MonoBehaviour
{
    [SerializeField] private AtomInfo _atomInfo;

    private void OnValidate()
    {
        if (_atomInfo != null)
        {
            float radius = _atomInfo.Radius;
            transform.localScale = new Vector3(radius, radius, radius);

            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            meshRenderer.material = _atomInfo.Material;
        }
    }
}
