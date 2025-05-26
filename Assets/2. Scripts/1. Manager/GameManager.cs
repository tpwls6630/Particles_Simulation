using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] public float SpeedMultiplier;
    [SerializeField] public float AnalyzeTime;
    [SerializeField] public float DrawTime;

    [SerializeField] public BoxCamera BoxCamera;

    [SerializeField] private GameObject _box;
    public Vector3 BoxCenter => new Vector3(0, _box.transform.localScale.y / 2 * 10, 0);

}
