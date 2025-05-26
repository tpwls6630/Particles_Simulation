using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Button))]
public abstract class SelectElement : MonoBehaviour
{
    private List<Action> _onClickDelegates;
    private List<Action> onClickDelegates
    {
        get
        {
            if (_onClickDelegates == null)
            {
                _onClickDelegates = new List<Action>();
            }
            return _onClickDelegates;
        }
    }

    private Image _image;
    public Image image
    {
        get
        {
            if (_image == null)
            {
                _image = GetComponent<Image>();
            }
            return _image;
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


    [SerializeField] protected TextMeshProUGUI _textField;
    [SerializeField] private Color _selectedColor;
    [SerializeField] private Color _unselectedColor;

    private bool _selected;
    public bool selected
    {
        get => _selected;
        set
        {
            _selected = value;
            image.color = _selected ? _selectedColor : _unselectedColor;
        }
    }

    protected void Start()
    {
        selected = true;
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

    protected virtual void OnClick()
    {
        foreach (Action onClickDelegate in _onClickDelegates)
        {
            onClickDelegate?.Invoke();
        }
        selected = !selected;
    }
}
