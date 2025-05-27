using UnityEngine;
using TMPro;
public class Gravity : MonoBehaviour
{
    public TMP_InputField gravityInputField; // 중력 값을 입력받을 InputField
    private const float MaxGravity = 20f; // 중력 최대값

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (gravityInputField != null)
        {
            // InputField의 onEndEdit 이벤트에 리스너를 등록합니다.
            gravityInputField.onEndEdit.AddListener(UpdateGravity);
            // 초기 중력 값으로 InputField를 설정할 수도 있습니다.
            // gravityInputField.text = Physics.gravity.y.ToString();
        }
        else
        {
            Debug.LogError("Gravity InputField가 할당되지 않았습니다.");
        }
    }

    // InputField 입력 완료 시 호출될 메서드
    void UpdateGravity(string inputText)
    {
        if (float.TryParse(inputText, out float newGravityY))
        {
            // 입력값이 너무 큰 경우 바운딩합니다. (양수/음수 모두 절댓값으로 비교)
            if (Mathf.Abs(newGravityY) > MaxGravity)
            {
                newGravityY = Mathf.Sign(newGravityY) * MaxGravity;
                // 사용자에게 바운딩 되었음을 알리기 위해 InputField 값을 업데이트 할 수도 있습니다.
                // gravityInputField.text = newGravityY.ToString();
            }

            // Unity의 중력 값을 업데이트합니다.
            // Physics.gravity는 Vector3이므로, y값만 변경합니다.
            Physics.gravity = new Vector3(Physics.gravity.x, -newGravityY, Physics.gravity.z);
            Debug.Log($"중력 값이 {Physics.gravity.y}(으)로 변경되었습니다.");
        }
        else
        {
            Debug.LogWarning($"입력 값 '{inputText}'(은)는 유효한 실수가 아닙니다. 중력 값은 변경되지 않았습니다.");
            // 이전 값으로 InputField를 되돌리거나, 기본값으로 설정할 수 있습니다.
            // gravityInputField.text = Physics.gravity.y.ToString();
        }
    }

    // MonoBehaviour가 파괴될 때 리스너를 제거하는 것이 좋습니다.
    void OnDestroy()
    {
        if (gravityInputField != null)
        {
            gravityInputField.onEndEdit.RemoveListener(UpdateGravity);
        }
    }
}
