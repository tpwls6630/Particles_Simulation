using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ParticleDestroyButton : MonoBehaviour
{
    private Button _button;

    private void Start()
    {
        _button = GetComponent<Button>();
    }

    public void AddClickDelegate(Action onClick)
    {
        _button.onClick.AddListener(() => onClick());
    }
}