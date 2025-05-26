using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Button))]
public class ParticleSelectElement : SelectElement
{

    private ParticleInfo _particleInfo;

    private new void Start()
    {
        base.Start();
    }

    public void SetParticleInfo(ParticleInfo particleInfo)
    {
        _particleInfo = particleInfo;
        _textField.text = _particleInfo.Name;
    }

    public ParticleInfo GetParticleInfo()
    {
        return _particleInfo;
    }
}
