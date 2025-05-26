using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Rigidbody))]
public class Particle : MonoBehaviour
{
    [SerializeField] private ParticleInfo particleInfo;

    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        UpdateParicleInfo();
    }

    public void SetParticleInfo(ParticleInfo particleInfo)
    {
        this.particleInfo = particleInfo;
        UpdateParicleInfo();
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    /// <summary>
    /// 입자의 속도를 설정합니다.
    /// m/s단위입니다.
    /// </summary>
    /// <param name="velocity">파티클 속도(m/s)</param>
    public void SetVelocity(Vector3 velocity)
    {
        _rb.linearVelocity = velocity / GameManager.Instance.SpeedMultiplier;
    }

    /// <summary>
    /// 입자의 위치를 반환합니다.
    /// </summary>
    /// <returns>파티클 위치(Vector3)</returns>
    public Vector3 GetPosition()
    {
        return transform.position;
    }

    /// <summary>
    /// 입자의 속도를 반환합니다.
    /// x,y,z 방향의 속력은 각각 m/s단위입니다.
    /// </summary>
    /// <returns>파티클 속도(Vector3, m/s)</returns>
    public Vector3 GetVelocity()
    {
        return _rb.linearVelocity * GameManager.Instance.SpeedMultiplier;
    }

    /// <summary>
    /// 입자의 속력을 반환합니다.
    /// m/s단위입니다.
    /// </summary>
    /// <returns>파티클 속력(m/s)</returns>
    public float GetSpeed()
    {
        return _rb.linearVelocity.magnitude * GameManager.Instance.SpeedMultiplier;
    }

    /// <summary>
    /// 입자의 운동 에너지를 반환합니다.
    /// J단위입니다.
    /// </summary>
    /// <returns>파티클 운동 에너지(J)</returns>
    public float GetKineticEnergy()
    {
        float speed = GetSpeed();
        return (float)(speed * speed / 2 * particleInfo.Mass);
    }

    /// <summary>
    /// 입자의 위치에 따른 위치 에너지를 반환합니다.
    /// J단위입니다.
    /// </summary>
    /// <returns>파티클 위치 에너지(J)</returns>
    public float GetPotentialEnergy()
    {
        return (float)(Physics.gravity.y * transform.position.y * particleInfo.Mass);
    }

    /// <summary>
    /// 입자의 역학적 에너지를 반환합니다.
    /// J단위입니다.
    /// </summary>
    /// <returns>파티클 역학적 에너지(J)</returns>
    public float GetTotalEnergy()
    {
        return GetKineticEnergy() + GetPotentialEnergy();
    }

    /// <summary>
    /// 입자의 정보를 반환합니다.
    /// </summary>
    /// <returns>파티클 정보(ParticleInfo)</returns>
    public ParticleInfo GetParticleInfo()
    {
        return particleInfo;
    }

    private void UpdateParicleInfo()
    {
        _rb.mass = particleInfo.RelativeAtomicMass;
        transform.localScale = Vector3.one * (float)Constants.ParticleRenderingMultiplier;
    }
}
