using System.Collections.Generic;
using UnityEngine;

public class ChartManager : Singleton<ChartManager>
{
    [SerializeField] private Chart _chart;

    public void DrawChart(Dictionary<ParticleInfo, List<float>> data)
    {
        if (data.Count > 0)
        {
            _chart.UpdateGraph(data);
        }
    }
}
