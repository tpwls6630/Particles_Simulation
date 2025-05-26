using System.Collections.Generic;
using UnityEngine;

public class ParticleViewSelector : MonoBehaviour
{
    [SerializeField] private GameObject _elementGroup;
    private GameObject _particleSelectElementTemplate;

    private List<ParticleSelectElement> _particleSelectElements;

    private bool _selectionDirtyBit;

    private void Start()
    {
        Initialize();
        _selectionDirtyBit = true;
    }

    private void Initialize()
    {
        int particleCount = ParticleManager.Instance.ParticleInfos.Count;
        _particleSelectElementTemplate = transform.Find("SelectionTemplate").gameObject;
        if (_particleSelectElementTemplate == null)
        {
            Debug.LogError("GameObject (\"SelectionTemplate\") is not found\n" +
            "Please check the scene hierarchy.");
            return;
        }
        _particleSelectElementTemplate.SetActive(false);

        _particleSelectElements = new List<ParticleSelectElement>();

        for (int i = 0; i < particleCount; i++)
        {
            ParticleSelectElement particleSelectElement = Instantiate(_particleSelectElementTemplate, _elementGroup.transform).GetComponent<ParticleSelectElement>();
            particleSelectElement.SetParticleInfo(ParticleManager.Instance.ParticleInfos[i]);
            particleSelectElement.AddClickDelegate(() =>
            {
                _selectionDirtyBit = true;
            });

            particleSelectElement.gameObject.SetActive(true);

            _particleSelectElements.Add(particleSelectElement);
        }
    }

    private void ApplySelection()
    {
        if (!_selectionDirtyBit)
            return;

        bool[] selectedParticles = new bool[_particleSelectElements.Count];
        for (int i = 0; i < _particleSelectElements.Count; i++)
        {
            selectedParticles[i] = _particleSelectElements[i].selected;
        }

        // 선택된 입자 정보 전달
        for (int i = 0; i < _particleSelectElements.Count; i++)
        {
            ParticleInfo particleInfo = _particleSelectElements[i].GetParticleInfo();
            GameManager.Instance.BoxCamera.SetParticleView(particleInfo, selectedParticles[i]);
        }

        _selectionDirtyBit = false;
    }

    private void Update()
    {
        ApplySelection();
    }
}
