using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class AmountSelector : MonoBehaviour
{
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private int[] _amountsConfig;
    [SerializeField] private GameObject _selectElementGroup;

    private GameObject _amountSelectElementTemplate;

    private List<AmountSelectElement> _amountSelectElements;

    private int _selectedAmount;
    public int selectedAmount
    {
        get => _selectedAmount;
    }


    private void Start()
    {
        Initialize();
    }


    private void Initialize()
    {
        _selectedAmount = 50;
        _inputField.text = _selectedAmount.ToString();

        _amountSelectElementTemplate = transform.Find("SelectionTemplate").gameObject;
        if (_amountSelectElementTemplate == null)
        {
            Debug.LogError("GameObject (\"SelectionTemplate\") is not found\n" +
            "Please check the scene hierarchy.");
            return;
        }
        _amountSelectElementTemplate.SetActive(false);

        _amountSelectElements = new List<AmountSelectElement>();

        for (int i = 0; i < _amountsConfig.Length; i++)
        {
            GameObject amountSelectElement = Instantiate(_amountSelectElementTemplate, _selectElementGroup.transform);
            AmountSelectElement amountSelectElementComponent = amountSelectElement.GetComponent<AmountSelectElement>();
            _amountSelectElements.Add(amountSelectElementComponent);
            amountSelectElementComponent.amount = _amountsConfig[i];
            amountSelectElementComponent.AddClickDelegate(() =>
            {
                _inputField.text = amountSelectElementComponent.amount.ToString();
                _selectedAmount = amountSelectElementComponent.amount;
            });

            amountSelectElement.SetActive(true);
        }

        _inputField.onEndEdit.AddListener(OnEndEdit);
    }

    private void OnEndEdit(string value)
    {
        if (int.TryParse(value, out int amount))
        {
            _selectedAmount = amount;
        }
    }
}