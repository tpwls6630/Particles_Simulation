using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(Button))]
public class AmountSelectElement : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textField;

    private List<Action> _onClickDelegates;
    private List<Action> onClickDelegates
    {
        get
        {
            if (_onClickDelegates == null)
            {
                print("_onClickDelegates is null");
                _onClickDelegates = new List<Action>();
            }
            return _onClickDelegates;
        }
    }

    private int _amount;
    public int amount
    {
        get => _amount;
        set
        {
            _amount = value;
            _textField.text = _amount.ToString();
        }
    }

    private Button _button;
    public Button button
    {
        get
        {
            if (_button == null)
            {
                _button = GetComponent<Button>();
            }
            return _button;
        }
    }

    private void Start()
    {
        button.onClick.AddListener(OnClick);
    }

    public void AddClickDelegate(Action onClick)
    {
        onClickDelegates.Add(onClick);
    }

    public void RemoveClickDelegate(Action onClick)
    {
        onClickDelegates.Remove(onClick);
    }

    public void OnClick()
    {
        foreach (Action onClickDelegate in onClickDelegates)
        {
            onClickDelegate?.Invoke();
        }
    }
}