using System.Collections.Generic;
using UnityEngine;

public class ParticleCreateSelector : MonoBehaviour
{
    [SerializeField] private GameObject _elementGroup;

    private List<ParticleSelectElement> _particleSelectElements;

    private GameObject _selectElementTemplate;

    private int _selectedParticleIndex;
    private bool _selectionDirtyBit;

    private void Start()
    {
        Initialize();
        _selectionDirtyBit = true;
    }

    private void Initialize()
    {
        int particleCount = ParticleManager.Instance.ParticleInfos.Count;
        _selectElementTemplate = transform.Find("SelectionTemplate").gameObject;
        if (_selectElementTemplate == null)
        {
            Debug.LogError("GameObject (\"SelectionTemplate\") is not found\n" +
            "Please check the scene hierarchy.");
            return;
        }
        _selectElementTemplate.SetActive(false);

        _particleSelectElements = new List<ParticleSelectElement>();

        for (int i = 0; i < particleCount; i++)
        {
            int c = i;
            ParticleSelectElement particleSelectElement = Instantiate(_selectElementTemplate, _elementGroup.transform).GetComponent<ParticleSelectElement>();
            particleSelectElement.SetParticleInfo(ParticleManager.Instance.ParticleInfos[i]);
            particleSelectElement.AddClickDelegate(() =>
            {
                _selectionDirtyBit = true;
                _selectedParticleIndex = c;
            });

            particleSelectElement.gameObject.SetActive(true);

            _particleSelectElements.Add(particleSelectElement);
        }

    }

    private void ApplySelection()
    {
        if (!_selectionDirtyBit)
            return;

        print($"selectedParticleIndex: {_selectedParticleIndex}");
        for (int i = 0; i < _particleSelectElements.Count; i++)
        {
            _particleSelectElements[i].selected = i == _selectedParticleIndex;
        }

        _selectionDirtyBit = false;
    }

    private void Update()
    {
        ApplySelection();
    }

    public ParticleInfo GetSelectedParticleInfo()
    {
        return ParticleManager.Instance.ParticleInfos[_selectedParticleIndex];
    }
}