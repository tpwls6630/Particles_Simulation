using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
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

        InitializeGraphs();
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
        if (float.IsNaN(xMax) || float.IsNaN(yMax))
        {
            return false;
        }
        if (xMax == 0)
        {
            xMax = 1;
        }
        if (yMax == 0)
        {
            yMax = 1;
        }

        if (xMax > _graphParam.xAxisValueRange.y || Mathf.Abs(xMax - _graphParam.xAxisValueRange.y) / _graphParam.xAxisValueRange.y > 0.25f)
        {
            print($"그래프 파라미터 변경 : ({_graphParam.xAxisValueRange.x}, {_graphParam.xAxisValueRange.y}) -> ({_graphParam.xAxisValueRange.x}, {xMax})");
            _graphParam.xAxisValueRange = new Vector2(_graphParam.xAxisValueRange.x, xMax);
            _xAxis.SetMax(_graphParam.xAxisValueRange.y);
            _xAxis.InitializeAxis();
            return true;
        }

        if (yMax > _graphParam.yAxisValueRange.y || Mathf.Abs(yMax - _graphParam.yAxisValueRange.y) / _graphParam.yAxisValueRange.y > 0.25f)
        {
            print($"그래프 파라미터 변경 : ({_graphParam.yAxisValueRange.x}, {_graphParam.yAxisValueRange.y}) -> ({_graphParam.yAxisValueRange.x}, {yMax})");
            _graphParam.yAxisValueRange = new Vector2(_graphParam.yAxisValueRange.x, yMax);
            _yAxis.SetMax(_graphParam.yAxisValueRange.y);
            _yAxis.InitializeAxis();
            return true;
        }
        return false;
    }

    private void InitializeGraphs()
    {
        foreach (var particleInfo in ParticleManager.Instance.ParticleInfos)
        {
            BarGraph barGraph = Instantiate(_barGraphTemplate, _barGraphGroup.transform);
            barGraph.gameObject.SetActive(true);
            barGraph.SetParticleInfo(particleInfo);
            _barGraphs.Add(particleInfo, barGraph);

            LineGraph lineGraph = Instantiate(_lineGraphTemplate, _lineGraphGroup.transform);
            lineGraph.gameObject.SetActive(true);
            lineGraph.SetParticleInfo(particleInfo);
            _lineGraphs.Add(particleInfo, lineGraph);
        }
    }

    public void SetParticleView(ParticleInfo particleInfo, bool isVisible)
    {
        if (isVisible)
        {
            if (_barGraphs.ContainsKey(particleInfo))
            {
                _barGraphs[particleInfo].gameObject.SetActive(true);
            }

            if (_lineGraphs.ContainsKey(particleInfo))
            {
                _lineGraphs[particleInfo].gameObject.SetActive(true);
            }
        }
        else
        {
            if (_barGraphs.ContainsKey(particleInfo))
            {
                _barGraphs[particleInfo].gameObject.SetActive(false);
            }

            if (_lineGraphs.ContainsKey(particleInfo))
            {
                _lineGraphs[particleInfo].gameObject.SetActive(false);
            }
        }
    }

    public void UpdateGraph(Dictionary<ParticleInfo, List<float>> data)
    {
        float xMaxCon = 0;
        float yMaxCon = 0;
        foreach (var particle in data)
        {
            ParticleInfo particleInfo = particle.Key;

            if (_barGraphs[particleInfo].enabled == false)
                continue;

            DrawBarGraph(particleInfo, particle.Value, out float xMax, out float yMax);
            DrawLineGraph(particleInfo, particle.Value);

            if (xMax > xMaxCon) xMaxCon = xMax;
            if (yMax > yMaxCon) yMaxCon = yMax;
        }

        AdjustGraphView(xMaxCon, yMaxCon);
    }


    private void DrawBarGraph(ParticleInfo particleInfo, List<float> data, out float xMax, out float yMax)
    {
        BarGraph targetBarGraph = _barGraphs[particleInfo];
        xMax = 0;
        yMax = 0;

        Dictionary<float, int> valueCounts = new Dictionary<float, int>();
        float xStep = (_graphParam.xAxisValueRange.y - _graphParam.xAxisValueRange.x) / _xAxisCount;


        for (int i = 0; i < _xAxisCount; i++)
        {
            valueCounts.Add(i * xStep, 0);
        }

        foreach (float value in data)
        {
            if (value > xMax) xMax = value;

            int bucketIndex = Mathf.FloorToInt(value / xStep);
            if (bucketIndex >= _xAxisCount)
            {
                bucketIndex = _xAxisCount - 1;
            }
            if (bucketIndex < 0)
            {
                bucketIndex = 0;
            }
            float bucketStartValue = bucketIndex * xStep;
            valueCounts[bucketStartValue]++;
        }

        List<float> xData = new List<float>();
        List<float> yData = new List<float>();

        foreach (var pair in valueCounts.OrderBy(p => p.Key))
        {
            xData.Add(pair.Key);
            yData.Add(pair.Value);
            if (pair.Value > yMax) yMax = pair.Value;
        }

        targetBarGraph.Draw(xData, yData, _graphParam);
    }


    private void DrawLineGraph(ParticleInfo particleInfo, List<float> data)
    {

        float xStep = (_graphParam.xAxisValueRange.y - _graphParam.xAxisValueRange.x) / _lineXAxisCount;
        List<float> _line_xData = new List<float>(_lineXAxisCount);
        List<float> _line_yData = new List<float>(_lineXAxisCount);


        for (int i = 0; i < _lineXAxisCount; i++)
        {
            _line_xData.Add(i * xStep);
            _line_yData.Add(0);
        }

        if (data.Count == 0)
        {
            _lineGraphs[particleInfo].Draw(_line_xData, _line_yData, _graphParam);
            return;
        }

        float rms = MaxwellBoltzmannAnalysis.InferRMSFromSpeeds(data);

        float temperature = MaxwellBoltzmannAnalysis.InferTemperatureFromSpeeds(data, particleInfo.RelativeAtomicMass, particleInfo.DegreeOfFreedom);

        Func<double, double> particleDensityFunction = MaxwellBoltzmannAnalysis.MaxwellBoltzmannParticleDensityFunction(data.Count, temperature, particleInfo.RelativeAtomicMass);



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
            float integral = (float)MaxwellBoltzmannAnalysis.IntegratePaticleDensity(particleDensityFunction, _line_xData[i], _line_xData[i] + step);
            if (float.IsNaN(integral))
            {
                _line_yData[i] = 0;
            }
            else
            {
                _line_yData[i] = integral;
            }
        }

        _lineGraphs[particleInfo].Draw(_line_xData, _line_yData, rms, _graphParam);

    }

}
