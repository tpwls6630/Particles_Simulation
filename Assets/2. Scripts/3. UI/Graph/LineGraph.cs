using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI.Extensions;

[RequireComponent(typeof(UILineRenderer))]
public class LineGraph : GraphBase
{
    private UILineRenderer _lineRenderer;

    private void Awake()
    {
        _lineRenderer = GetComponent<UILineRenderer>();
    }

    // public override void SetParam(GraphParam param)
    // {
    //     _param.xAxisValueRange = new Vector2(0, param.xAxisValueRange.y);
    //     _param.yAxisValueRange = new Vector2(0, param.yAxisValueRange.y);
    //     _param.xAxisPositionRange = new Vector2(0, param.xAxisPositionRange.y);
    //     _param.yAxisPositionRange = new Vector2(0, param.yAxisPositionRange.y);
    // }

    public override void Draw(List<float> xData, List<float> yData, GraphParam param)
    {
        if (xData.Count == 0)
            return;

        if (xData.Count != yData.Count)
            throw new Exception("xData와 yData의 개수가 다릅니다.");

        if (_lineRenderer.Points.Length != xData.Count + 1)
        {
            _lineRenderer.Points = new Vector2[xData.Count + 1];
        }

        _lineRenderer.Points[0] = new Vector2(param.xAxisPositionRange.x, param.yAxisPositionRange.x);
        for (int i = 0; i < xData.Count; i++)
        {
            _lineRenderer.Points[i + 1] = new Vector2(param.xValue2Pos(xData[i] + xData[1]), param.yValue2Pos(yData[i]));
        }
        _lineRenderer.SetAllDirty();
    }

}
