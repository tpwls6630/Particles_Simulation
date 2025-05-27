using UnityEngine;
using TMPro;
public class TemperatureInput : MonoBehaviour
{
    public TMP_InputField temperatureInputField; // 중력 값을 입력받을 InputField
    private const float MaxTemperature = 10000f; // 중력 최대값
    public float Temperature { get; private set; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Temperature = 300;
        if (temperatureInputField != null)
        {
            // InputField의 onEndEdit 이벤트에 리스너를 등록합니다.
            temperatureInputField.onEndEdit.AddListener(UpdateTemperature);

            temperatureInputField.text = Temperature.ToString();
        }
        else
        {
            Debug.LogError("Temperature InputField가 할당되지 않았습니다.");
        }
    }

    // InputField 입력 완료 시 호출될 메서드
    void UpdateTemperature(string inputText)
    {
        if (float.TryParse(inputText, out float newTemperature))
        {
            // 입력값이 너무 큰 경우 바운딩합니다. (양수/음수 모두 절댓값으로 비교)
            if (Mathf.Abs(newTemperature) > MaxTemperature)
            {
                newTemperature = Mathf.Sign(newTemperature) * MaxTemperature;
            }

            // 음수인 경우 상온 300K로 설정
            if (newTemperature < 0)
            {
                newTemperature = 300;
            }

            // Unity의 중력 값을 업데이트합니다.
            // Physics.gravity는 Vector3이므로, y값만 변경합니다.
            Temperature = newTemperature;
            Debug.Log($"온도가 {Temperature}K로 변경되었습니다.");
        }
        else
        {
            Debug.LogWarning($"입력 값 '{inputText}'(은)는 유효한 실수가 아닙니다. 온도 값은 변경되지 않았습니다.");
        }
    }

    // MonoBehaviour가 파괴될 때 리스너를 제거하는 것이 좋습니다.
    void OnDestroy()
    {
        if (temperatureInputField != null)
        {
            temperatureInputField.onEndEdit.RemoveListener(UpdateTemperature);
        }
    }
}
