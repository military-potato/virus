/*using UnityEngine;
using TMPro; // TextMesh Pro 컴포넌트 제어용
using System.Collections; // Coroutine(IEnumerator) 사용을 위해 필수

public class StatusController : MonoBehaviour
{
    [Header("[ UI Text References ]")]
    [SerializeField] private TextMeshProUGUI inflammationText;
    [SerializeField] private TextMeshProUGUI painText;
    [SerializeField] private TextMeshProUGUI toleranceText;

    [Header("[ Popup UI Reference ]")]
    [SerializeField] private GameObject warningPopup; // 생성한 팝업 오브젝트 연결용
    [SerializeField] private float popupDuration = 1.0f; // 팝업이 유지될 시간(초)

    // 각 수치를 저장할 내부 변수
    private int inflammationValue = 0;
    private int painValue = 0;
    private int toleranceValue = 0;

    private Coroutine activePopupCoroutine; // 실행 중인 팝업 코루틴을 추적하기 위한 변수

    private void Start()
    {
        UpdateAllUI();

        // 시작 시 팝업이 확실히 꺼져 있도록 설정
        if (warningPopup != null)
            warningPopup.SetActive(false);
    }

    private void Update()
    {
        // --- 100씩 증가 로직 (1, 2, 3) ---
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            inflammationValue += 100;
            UpdateAllUI();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            painValue += 100;
            UpdateAllUI();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        {
            toleranceValue += 100;
            UpdateAllUI();
        }


        // --- 50씩 감소 로직 및 부족 시 팝업 처리 (4, 5, 6) ---

        // 염증 수치 감소 (4)
        if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
        {
            if (inflammationValue >= 50)
            {
                inflammationValue -= 50;
                UpdateAllUI();
            }
            else
            {
                TriggerWarning();
            }
        }

        // 고통 수치 감소 (5)
        if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
        {
            if (painValue >= 50)
            {
                painValue -= 50;
                UpdateAllUI();
            }
            else
            {
                TriggerWarning();
            }
        }

        // 내성 수치 감소 (6)
        if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6))
        {
            if (toleranceValue >= 50)
            {
                toleranceValue -= 50;
                UpdateAllUI();
            }
            else
            {
                TriggerWarning();
            }
        }
    }

    /// <summary>
    /// 수치가 부족할 때 경고 팝업 제어를 요청하는 함수
    /// </summary>
    private void TriggerWarning()
    {
        if (warningPopup == null) return;

        // 이미 팝업이 떠 있는 상태에서 다시 작동하면, 기존 타이머를 취소하고 새로 시작
        if (activePopupCoroutine != null)
        {
            StopCoroutine(activePopupCoroutine);
        }

        activePopupCoroutine = StartCoroutine(ShowAndHidePopup());
    }

    /// <summary>
    /// 팝업을 활성화하고 지정된 시간 후 비활성화하는 코루틴 함수
    /// </summary>
    private IEnumerator ShowAndHidePopup()
    {
        warningPopup.SetActive(true); // 팝업 켜기

        yield return new WaitForSeconds(popupDuration); // 설정한 시간만큼 대기

        warningPopup.SetActive(false); // 팝업 끄기
        activePopupCoroutine = null;
    }

    /// <summary>
    /// UI 텍스트 동기화 함수
    /// </summary>
    private void UpdateAllUI()
    {
        if (inflammationText != null)
            inflammationText.text = inflammationValue.ToString();

        if (painText != null)
            painText.text = painValue.ToString();

        if (toleranceText != null)
            toleranceText.text = toleranceValue.ToString();
    }
}*/