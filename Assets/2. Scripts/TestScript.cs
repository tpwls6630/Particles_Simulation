using UnityEngine;

public class TestScript : MonoBehaviour
{

    private RectTransform _rectTransform;
    private GameObject _findObject;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _findObject = _rectTransform.Find("FindThis").gameObject;
        print(_findObject.name);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
