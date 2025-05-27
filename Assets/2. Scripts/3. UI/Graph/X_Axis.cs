using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class X_Axis : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _label_text;
    [SerializeField] private GameObject _stepLine_Prefab;
    [SerializeField] private float _min;
    [SerializeField] private float _max;
    [SerializeField] private int _stepCount;

    private float _valueStep => (_max - _min) / _stepCount;
    private float _positionStep => _rectTransform.rect.width / _stepCount;
    private RectTransform _rectTransform;
    private List<GameObject> _stepLines;

    public void SetMin(float min)
    {
        _min = min;
    }

    public void SetMax(float max)
    {
        _max = max;
    }

    public void SetStepCount(int stepCount)
    {
        _stepCount = stepCount;
    }


    public void SetLineThickness(float height)
    {
        _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, height);
    }

    public void SetLineColor(Color color)
    {
        GetComponent<Image>().color = color;
    }

    public float GetPositionStep()
    {
        return _positionStep;
    }

    public void InitializeAxis()
    {
        _stepLine_Prefab.SetActive(false);
        if (_stepLines.Count != _stepCount)
        {
            foreach (GameObject stepLine in _stepLines)
            {
                Destroy(stepLine);
            }
            _stepLines.Clear();
            for (int i = 0; i < _stepCount; i++)
            {
                GameObject stepLine = Instantiate(_stepLine_Prefab, transform);
                stepLine.SetActive(true);
                _stepLines.Add(stepLine);
            }
        }

        for (int i = 0; i < _stepCount; i++)
        {
            _stepLines[i].GetComponent<RectTransform>().anchoredPosition = new Vector2((i + 1) * _positionStep, 0);
            _stepLines[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = (_min + (i + 1) * _valueStep).ToString("F0");
        }

    }

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _stepLines = new List<GameObject>();
    }
}
