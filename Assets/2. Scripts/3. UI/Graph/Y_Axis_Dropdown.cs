using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System;

[RequireComponent(typeof(TMP_Dropdown))]
public class Y_Axis_Dropdown : MonoBehaviour
{
    private List<Action<Y_AxisType>> _onValueChangedListeners;

    private TMP_Dropdown _dropdown;

    private void Awake()
    {
        _dropdown = GetComponent<TMP_Dropdown>();
        _onValueChangedListeners = new List<Action<Y_AxisType>>();
        InitializeDropdown();
    }

    private void Start()
    {
        ClearListeners();
        _dropdown.onValueChanged.RemoveAllListeners();
        _dropdown.onValueChanged.AddListener(OnValueChanged);
    }

    private void InitializeDropdown()
    {
        _dropdown.ClearOptions();
        List<string> options = new List<string>();

        foreach (Y_AxisType type in System.Enum.GetValues(typeof(Y_AxisType)))
        {
            options.Add(type.ToString());
        }

        _dropdown.AddOptions(options);
    }

    public void SetValue(int value)
    {
        _dropdown.value = value;
    }

    private void OnValueChanged(int index)
    {
        foreach (Action<Y_AxisType> listener in _onValueChangedListeners)
        {
            listener((Y_AxisType)index);
        }
    }

    public void AddListener(Action<Y_AxisType> listener)
    {
        _onValueChangedListeners.Add(listener);
    }

    public void RemoveListener(Action<Y_AxisType> listener)
    {
        _onValueChangedListeners.Remove(listener);
    }

    public void ClearListeners()
    {
        _onValueChangedListeners.Clear();
    }
}
