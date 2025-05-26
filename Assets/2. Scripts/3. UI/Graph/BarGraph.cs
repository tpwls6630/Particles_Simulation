using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BarGraph : GraphBase
{
    private string _barGraphName = "BarGraph (Default)";
    private GameObject _barGroup;
    private GameObject _barPrefab;
    private List<GameObject> _bars;

    private void Awake()
    {
        _bars = new List<GameObject>();
    }

    private void Start()
    {
        _barGroup = gameObject;
        _barPrefab = _barGroup.transform.Find("BarTemplate").gameObject;
        _barPrefab.SetActive(false);
    }

    public override void Draw(List<float> xData, List<float> yData, GraphParam param)
    {

        if (xData.Count == 0)
            return;

        if (xData.Count != yData.Count)
            throw new Exception("xData와 yData의 개수가 다릅니다.");

        float barWidth = (param.xAxisPositionRange.y - param.xAxisPositionRange.x) / xData.Count;

        if (_bars.Count != xData.Count)
        {
            foreach (GameObject bar in _bars)
            {
                Destroy(bar);
            }
            _bars.Clear();
            for (int i = 0; i < xData.Count; i++)
            {
                GameObject bar = Instantiate(_barPrefab, _barGroup.transform);
                bar.SetActive(true);
                _bars.Add(bar);
            }
        }

        for (int i = 0; i < xData.Count; i++)
        {
            GameObject bar = _bars[i];
            bar.GetComponent<RectTransform>().anchoredPosition = new Vector2(param.xValue2Pos(xData[i]), param.yAxisPositionRange.x);
            bar.GetComponent<RectTransform>().sizeDelta = new Vector2(barWidth, param.yValue2Pos(yData[i]));
            // bar.GetComponentInChildren<Image>().color = param.TargetParticleInfo.Color;
        }
    }
}
