using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public enum X_AxisType
{
    Speed,
    KineticEnergy,
    PotentialEnergy,
    MechanicalEnergy
}

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

    private X_AxisType _xAxisType = X_AxisType.Speed;


    public void SetAxisType(X_AxisType xAxisType)
    {
        _xAxisType = xAxisType;
        string label = xAxisType.ToString();
        label += "\n";
        label += AxisUnit(xAxisType);
        _label_text.text = label;
    }

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
            _stepLines[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = (_min + (i + 1) * _valueStep).ToString("F2");
        }

    }

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _stepLines = new List<GameObject>();
    }

    private string AxisUnit(X_AxisType xAxisType)
    {
        switch (xAxisType)
        {
            case X_AxisType.Speed:
                return "m/s";
            case X_AxisType.KineticEnergy:
                return "J";
            case X_AxisType.PotentialEnergy:
                return "J";
            case X_AxisType.MechanicalEnergy:
                return "J";
            default:
                return "";
        }
    }
}
