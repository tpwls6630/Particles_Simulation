using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class Y_Axis : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _label_text;
    [SerializeField] private GameObject _stepLine_Prefab;
    [SerializeField] private float _min;
    [SerializeField] private float _max;
    [SerializeField] private int _stepCount;


    private string _label;
    private float _step => (_max - _min) / _stepCount;
    private float _positionStep => _rectTransform.rect.height / _stepCount;
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


    public void SetLineThickness(float width)
    {
        _rectTransform.sizeDelta = new Vector2(width, _rectTransform.sizeDelta.y);
    }

    public void SetLineColor(Color color)
    {
        GetComponent<Image>().color = color;
    }

    public void InitializeAxis()
    {
        _stepLine_Prefab.SetActive(false);
        if (_stepLines.Count > 0)
        {
            foreach (GameObject stepLine in _stepLines)
            {
                Destroy(stepLine);
            }
        }
        _stepLines.Clear();

        for (int i = 0; i < _stepCount; i++)
        {
            GameObject stepLine = Instantiate(_stepLine_Prefab, transform);
            stepLine.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, (i + 1) * _positionStep);
            stepLine.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = (_min + (i + 1) * _step).ToString("F0");
            stepLine.SetActive(true);
            _stepLines.Add(stepLine);
        }
    }

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _stepLines = new List<GameObject>();
    }

}
