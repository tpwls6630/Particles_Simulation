using System.Collections.Generic;
using UnityEngine;

public class ParticleCreateSelector : MonoBehaviour
{
    [SerializeField] private GameObject _elementGroup;

    private Dictionary<ParticleInfo, ParticleSelectElement> _particleSelectElements;

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

        _particleSelectElements = new Dictionary<ParticleInfo, ParticleSelectElement>();

        for (int i = 0; i < particleCount; i++)
        {
            int c = i;
            ParticleInfo particleInfo = ParticleManager.Instance.ParticleInfos[i];
            ParticleSelectElement particleSelectElement = Instantiate(_selectElementTemplate, _elementGroup.transform).GetComponent<ParticleSelectElement>();
            particleSelectElement.SetParticleInfo(particleInfo);
            particleSelectElement.SetParticleCount(0);
            particleSelectElement.AddClickDelegate(() =>
            {
                _selectionDirtyBit = true;
                _selectedParticleIndex = c;
            });

            particleSelectElement.gameObject.SetActive(true);

            _particleSelectElements.Add(particleInfo, particleSelectElement);
        }
    }

    private void ApplySelection()
    {
        if (!_selectionDirtyBit)
            return;

        print($"selectedParticleIndex: {_selectedParticleIndex}");
        int index = 0;
        foreach (var element in _particleSelectElements.Values)
        {
            element.selected = index == _selectedParticleIndex;
            index++;
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

    public void SetParticleCount(ParticleInfo particleInfo, int count)
    {
        if (_particleSelectElements.TryGetValue(particleInfo, out var element))
        {
            element.SetParticleCount(count);
        }
    }
}