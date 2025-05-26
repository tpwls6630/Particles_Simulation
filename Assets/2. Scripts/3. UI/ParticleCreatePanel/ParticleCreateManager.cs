using UnityEngine;

public class ParticleCreateManager : MonoBehaviour
{
    [SerializeField] private ParticleCreateSelector _particleCreateSelector;
    [SerializeField] private AmountSelector _amountSelector;
    [SerializeField] private ParticleCreateButton _particleCreateButton;
    [SerializeField] private ParticleDestroyButton _particleDestroyButton;

    private void Start()
    {
        _particleCreateButton.AddClickDelegate(CreateParticle);
        _particleDestroyButton.AddClickDelegate(DestroyParticle);
    }

    private void CreateParticle()
    {
        ParticleInfo particleInfo = _particleCreateSelector.GetSelectedParticleInfo();
        int amount = _amountSelector.selectedAmount;
        ParticleManager.Instance.CreateParticles(particleInfo, amount);
    }

    private void DestroyParticle()
    {
        ParticleInfo particleInfo = _particleCreateSelector.GetSelectedParticleInfo();
        int amount = _amountSelector.selectedAmount;
        ParticleManager.Instance.DestroyParticles(particleInfo, amount);
    }
}