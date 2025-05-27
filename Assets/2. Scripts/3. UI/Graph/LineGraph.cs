using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI.Extensions;
using TMPro;

[RequireComponent(typeof(UILineRenderer))]
public class LineGraph : GraphBase
{
    public TextMeshProUGUI _temperatureText;
    private ParticleInfo _particleInfo;
    private UILineRenderer _lineRenderer;

    private void Awake()
    {
        _lineRenderer = GetComponent<UILineRenderer>();
    }

    public void SetParticleInfo(ParticleInfo particleInfo)
    {
        _particleInfo = particleInfo;
        _lineRenderer.color = _particleInfo.Color;
        _temperatureText.text = "0 K";
        _temperatureText.color = _particleInfo.Color;
        _temperatureText.gameObject.SetActive(false);
    }

    public override void Draw(List<float> xData, List<float> yData, GraphParam param)
    {
        _temperatureText.gameObject.SetActive(false);
        _lineRenderer.gameObject.SetActive(false);
    }

    public void Draw(List<float> xData, List<float> yData, float rms, GraphParam param)
    {
        _temperatureText.gameObject.SetActive(true);
        _lineRenderer.gameObject.SetActive(true);
        if (xData.Count == 0)
            return;

        if (xData.Count != yData.Count)
            throw new Exception("xData와 yData의 개수가 다릅니다.");

        if (_lineRenderer.Points.Length != xData.Count + 1)
        {
            _lineRenderer.Points = new Vector2[xData.Count + 1];
        }
        float textXPos = param.xAxisPositionRange.x;
        float textYPos = param.yAxisPositionRange.x;
        float minSlope = float.MaxValue;

        _lineRenderer.Points[0] = new Vector2(param.xAxisPositionRange.x, param.yAxisPositionRange.x);
        for (int i = 0; i < xData.Count; i++)
        {
            float xPos = param.xValue2Pos(xData[i]);
            float yPos = param.yValue2Pos(yData[i]);

            if (float.IsNaN(xPos) || float.IsNaN(yPos))
            {
                _lineRenderer.Points[i + 1] = new Vector2(param.xAxisPositionRange.x, param.yAxisPositionRange.x);
            }
            else
            {
                _lineRenderer.Points[i + 1] = new Vector2(xPos, yPos);
            }

            if (i > 0)
            {
                float prevXPos = _lineRenderer.Points[i].x;
                float prevYPos = _lineRenderer.Points[i].y;
                float currentXPos = _lineRenderer.Points[i + 1].x;
                float currentYPos = _lineRenderer.Points[i + 1].y;

                if (currentXPos - prevXPos != 0)
                {
                    float slope = (currentYPos - prevYPos) / (currentXPos - prevXPos);
                    if (slope < 0 && slope < minSlope)
                    {
                        minSlope = slope;
                        textXPos = prevXPos;
                        textYPos = prevYPos;
                    }
                }
            }
        }

        if (xData.Count == 1 && _lineRenderer.Points.Length > 1)
        {
            textXPos = _lineRenderer.Points[1].x;
            textYPos = _lineRenderer.Points[1].y;
        }

        _lineRenderer.SetAllDirty();

        // 그래프에 속도 표시
        string txt = $"{_particleInfo.Name}\nv_rms = {rms.ToString("F0")} m/s";
        _temperatureText.text = txt;
        RectTransform textRect = _temperatureText.GetComponent<RectTransform>();
        if (textRect != null)
        {
            textRect.anchoredPosition = new Vector2(textXPos, textYPos);
        }
    }

}
