using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Chart : MonoBehaviour
{



    [Header("X축")]
    [SerializeField] private string _xAxisLabel;
    [SerializeField] private X_Axis _xAxis;
    [SerializeField] private int _xAxisCount;
    [Header("Y축")]
    [SerializeField] private string _yAxisLabel;
    [SerializeField] private Y_Axis _yAxis;
    [SerializeField] private int _yAxisCount;

    [SerializeField] private GraphParam _graphParam;
    [SerializeField] private GameObject _barGraphGroup;
    [SerializeField] private GameObject _lineGraphGroup;

    [SerializeField] private int _lineXAxisCount;

    private float _drawTime => GameManager.Instance.DrawTime;
    private List<int> _bar_xValueCount;
    private List<float> _bar_xData, _bar_yData;
    private List<float> _line_xData, _line_yData;

    private BarGraph _barGraphTemplate;
    private LineGraph _lineGraphTemplate;

    private Dictionary<ParticleInfo, BarGraph> _barGraphs;
    private Dictionary<ParticleInfo, LineGraph> _lineGraphs;

    private void Start()
    {
        _barGraphTemplate = transform.Find("BarGraphTemplate").GetComponent<BarGraph>();
        _lineGraphTemplate = transform.Find("LineGraphTemplate").GetComponent<LineGraph>();

        _barGraphs = new Dictionary<ParticleInfo, BarGraph>();
        _lineGraphs = new Dictionary<ParticleInfo, LineGraph>();

        RectTransform rectTransform = GetComponent<RectTransform>();
        _graphParam.xAxisPositionRange = new Vector2(0, rectTransform.rect.width);
        _graphParam.yAxisPositionRange = new Vector2(0, rectTransform.rect.height);
        SetGraphParam();
    }

    private void SetGraphParam()
    {
        _xAxis.SetMin(_graphParam.xAxisValueRange.x);
        _xAxis.SetMax(_graphParam.xAxisValueRange.y);
        _xAxis.SetStepCount(10);
        _xAxis.InitializeAxis();

        _yAxis.SetMin(_graphParam.yAxisValueRange.x);
        _yAxis.SetMax(_graphParam.yAxisValueRange.y);
        _yAxis.SetStepCount(10);
        _yAxis.InitializeAxis();
    }

    private bool AdjustGraphView(float xMax, float yMax)
    {
        if (xMax > _graphParam.xAxisValueRange.y || Mathf.Abs(xMax - _graphParam.xAxisValueRange.y) / _graphParam.xAxisValueRange.y > 0.25f)
        {
            _graphParam.xAxisValueRange = new Vector2(_graphParam.xAxisValueRange.x, xMax);
            _xAxis.SetMax(_graphParam.xAxisValueRange.y);
            _xAxis.InitializeAxis();
            return true;
        }

        if (yMax > _graphParam.yAxisValueRange.y || Mathf.Abs(yMax - _graphParam.yAxisValueRange.y) / _graphParam.yAxisValueRange.y > 0.25f)
        {
            _graphParam.yAxisValueRange = new Vector2(_graphParam.yAxisValueRange.x, yMax);
            _yAxis.SetMax(_graphParam.yAxisValueRange.y);
            _yAxis.InitializeAxis();
            return true;
        }
        return false;
    }

    public void UpdateGraph(Dictionary<ParticleInfo, List<float>> data)
    {
        foreach (var particle in data)
        {
            if (particle.Value.Count == 0)
                continue;

            ParticleInfo particleInfo = particle.Key;
            if (!_barGraphs.ContainsKey(particleInfo))
            {
                BarGraph barGraph = Instantiate(_barGraphTemplate, _barGraphGroup.transform);
                barGraph.gameObject.SetActive(true);
                _barGraphs.Add(particleInfo, barGraph);
            }

            if (!_lineGraphs.ContainsKey(particleInfo))
            {
                LineGraph lineGraph = Instantiate(_lineGraphTemplate, _lineGraphGroup.transform);
                lineGraph.gameObject.SetActive(true);
                _lineGraphs.Add(particleInfo, lineGraph);
            }

            StartCoroutine(DrawBarGraph(particleInfo, particle.Value));
            StartCoroutine(DrawLineGraph(particleInfo, particle.Value));
        }
    }

    private IEnumerator DrawBarGraph(ParticleInfo particleInfo, List<float> data)
    {

        float xMax = 0;
        float yMax = 0;


        int batchCount = (int)(_drawTime / Time.deltaTime);
        int batchSize = data.Count / batchCount;

        // 입자 개수 카운트를 위한 리스트 초기화
        if (_bar_xValueCount == null || _bar_xValueCount.Count != _xAxisCount)
        {
            _bar_xValueCount = new List<int>(_xAxisCount);
            for (int i = 0; i < _xAxisCount; i++)
            {
                _bar_xValueCount.Add(0);
            }
        }
        else
        {
            for (int i = 0; i < _xAxisCount; i++)
            {
                _bar_xValueCount[i] = 0;
            }
        }

        float xStep = (_graphParam.xAxisValueRange.y - _graphParam.xAxisValueRange.x) / _xAxisCount;
        // 구간별 입자 개수 카운트
        for (int i = 0; i < batchCount; i++)
        {
            for (int j = 0; j < batchSize; j++)
            {
                int index = i * batchSize + j;
                if (index >= data.Count) break;

                int xValue = Mathf.FloorToInt(data[index] / xStep);
                if (xValue >= _xAxisCount)
                {
                    xValue = _xAxisCount - 1;
                }

                _bar_xValueCount[xValue]++;
                if (data[index] > xMax) xMax = data[index];
            }
            yield return null;
        }

        // 최대 입자 개수 찾기
        for (int i = 0; i < _xAxisCount; i++)
        {
            if (_bar_xValueCount[i] > yMax) yMax = _bar_xValueCount[i];
        }

        bool graphViewAdjusted = AdjustGraphView(xMax, yMax);
        xStep = (_graphParam.xAxisValueRange.y - _graphParam.xAxisValueRange.x) / _xAxisCount;

        // x좌표용 xData 초기화
        if (_bar_xData == null || _bar_xData.Count != _xAxisCount)
        {
            _bar_xData = new List<float>(_xAxisCount);
            for (int i = 0; i < _xAxisCount; i++)
            {
                _bar_xData.Add(i * xStep);
            }
        }
        else
        {
            for (int i = 0; i < _xAxisCount; i++)
            {
                _bar_xData[i] = i * xStep;
            }

        }



        // y좌표용 yData 초기화
        if (_bar_yData == null || _bar_yData.Count != _xAxisCount)
        {
            _bar_yData = new List<float>(_xAxisCount);
            for (int i = 0; i < _xAxisCount; i++)
            {
                _bar_yData.Add(_bar_xValueCount[i]);
            }
        }
        else
        {
            for (int i = 0; i < _xAxisCount; i++)
            {
                _bar_yData[i] = _bar_xValueCount[i];
            }
        }

        _barGraphs[particleInfo].Draw(_bar_xData, _bar_yData, _graphParam);
    }

    private IEnumerator DrawLineGraph(ParticleInfo particleInfo, List<float> data)
    {

        float temperature = MaxwellBoltzmannAnalysis.InferTemperatureFromSpeeds(data, particleInfo.RelativeAtomicMass);

        Func<double, double> particleDensityFunction = MaxwellBoltzmannAnalysis.MaxwellBoltzmannParticleDensityFunction(data.Count, temperature, particleInfo.RelativeAtomicMass);

        float xStep = (_graphParam.xAxisValueRange.y - _graphParam.xAxisValueRange.x) / _lineXAxisCount;
        if (_line_xData == null || _line_xData.Count != _lineXAxisCount)
        {
            _line_xData = new List<float>(_lineXAxisCount);
            for (int i = 0; i < _lineXAxisCount; i++)
            {
                _line_xData.Add(i * xStep);
            }
        }
        else
        {
            for (int i = 0; i < _lineXAxisCount; i++)
            {
                _line_xData[i] = i * xStep;
            }
        }

        if (_line_yData == null || _line_yData.Count != _lineXAxisCount)
        {
            _line_yData = new List<float>(_lineXAxisCount);
            for (int i = 0; i < _lineXAxisCount; i++)
            {
                _line_yData.Add(0);
            }
        }

        float step = (_graphParam.xAxisValueRange.y - _graphParam.xAxisValueRange.x) / _lineXAxisCount;
        for (int i = 0; i < _lineXAxisCount; i++)
        {
            _line_yData[i] = (float)MaxwellBoltzmannAnalysis.IntegratePaticleDensity(particleDensityFunction, _line_xData[i], _line_xData[i] + step);
        }

        _lineGraphs[particleInfo].Draw(_line_xData, _line_yData, _graphParam);
        yield return null;
    }

}
