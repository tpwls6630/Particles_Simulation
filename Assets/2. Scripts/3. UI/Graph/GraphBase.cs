using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct GraphParam
{
    public Vector2 xAxisValueRange;
    public Vector2 yAxisValueRange;
    public Vector2 xAxisPositionRange;
    public Vector2 yAxisPositionRange;
    [HideInInspector]
    public ParticleInfo TargetParticleInfo;
    public float xValue2Pos(float value) => (value - xAxisValueRange.x) / (xAxisValueRange.y - xAxisValueRange.x) * (xAxisPositionRange.y - xAxisPositionRange.x) + xAxisPositionRange.x;
    public float yValue2Pos(float value) => (value - yAxisValueRange.x) / (yAxisValueRange.y - yAxisValueRange.x) * (yAxisPositionRange.y - yAxisPositionRange.x) + yAxisPositionRange.x;

    public GraphParam(Vector2 xAxisValueRange, Vector2 yAxisValueRange, Vector2 xAxisPositionRange, Vector2 yAxisPositionRange, ParticleInfo targetParticleInfo)
    {
        this.xAxisValueRange = xAxisValueRange;
        this.yAxisValueRange = yAxisValueRange;
        this.xAxisPositionRange = xAxisPositionRange;
        this.yAxisPositionRange = yAxisPositionRange;
        this.TargetParticleInfo = targetParticleInfo;
    }

    public GraphParam(GraphParam param)
    {
        this.xAxisValueRange = param.xAxisValueRange;
        this.yAxisValueRange = param.yAxisValueRange;
        this.xAxisPositionRange = param.xAxisPositionRange;
        this.yAxisPositionRange = param.yAxisPositionRange;
        this.TargetParticleInfo = param.TargetParticleInfo;
    }
}

public abstract class GraphBase : MonoBehaviour
{
    public virtual void Draw(List<float> xData, List<float> yData, GraphParam param)
    {

    }
}