using UnityEngine;
using TMPro; // TextMesh Pro 컴포넌트 제어용
using System.Collections; // Coroutine(IEnumerator) 사용을 위해 필수

public class StatusController : MonoBehaviour
{
    // --- [싱글톤 추가] 어디서나 StatusController.Instance 로 접근할 수 있게 합니다 ---
    public static StatusController Instance { get; private set; }
    
    

    [Header("[ UI Text References ]")]
    [SerializeField] private TextMeshProUGUI inflammationText;
    [SerializeField] private TextMeshProUGUI painText;
    [SerializeField] private TextMeshProUGUI toleranceText;

    [Header("[ Popup UI Reference ]")]
    [SerializeField] private GameObject warningPopup; // 생성한 팝업 오브젝트 연결용
    [SerializeField] private float popupDuration = 1.0f; // 팝업이 유지될 시간(초)

    // 각 수치를 저장할 내부 변수 (외부에서 읽을 수만 있게 public getter 제공)
    public int InflammationValue { get; private set; } = 100; // 테스트를 위해 초기값 100 세팅
    public int PainValue { get; private set; } = 100;
    public int ToleranceValue { get; private set; } = 100;

    private Coroutine activePopupCoroutine; // 실행 중인 팝업 코루틴을 추적하기 위한 변수

    private void Awake()
    {
        // 싱글톤 인스턴스 설정
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateAllUI();

        // 시작 시 팝업이 확실히 꺼져 있도록 설정
        if (warningPopup != null)
            warningPopup.SetActive(false);
    }

    // --- [코스트 관리 핵심 함수 추가] ---
    
    /// <summary>
    /// 아군 소환 시 코스트(내성 수치)를 차감할 수 있는지 확인하고 소모하는 함수
    /// </summary>
    /// <param name="amount">소모할 내성 수치</param>
    /// <returns>소모 성공 여부</returns>
    public bool TrySpendInflammation(int amount)
    {
        // 현재 염증 수치가 내려갈 수 있는 최소치(코스트)보다 여유가 있는지 확인
        if (InflammationValue >= amount)
        {
            InflammationValue -= amount; // 염증 수치 감소(소모)
            UpdateAllUI();
            return true;
        }
        else
        {
            // 염증 수치가 부족하면(코스트보다 낮으면) 경고 팝업을 띄웁니다.
            TriggerWarning();
            Debug.LogWarning("염증 수치가 부족하여 아군을 소환할 수 없습니다!");
            return false;
        }
    }

    /// <summary>
    /// 외부에서 수치를 회복시켜줄 때 사용하는 함수 (예: 적 처치 시 염증/고통 완화 등)
    /// </summary>
    public void AddValues(int inflammation, int pain, int tolerance)
    {
        InflammationValue += inflammation;
        PainValue += pain;
        ToleranceValue += tolerance;
        UpdateAllUI();
    }


    private void Update()
    {
        // --- 100씩 증가 로직 (1, 2, 3) ---
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            InflammationValue += 100;
            UpdateAllUI();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            PainValue += 100;
            UpdateAllUI();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        {
            ToleranceValue += 100;
            UpdateAllUI();
        }


        // --- 50씩 감소 로직 및 부족 시 팝업 처리 (4, 5, 6) ---

        // 염증 수치 감소 (4)
        if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
        {
            if (InflammationValue >= 50)
            {
                InflammationValue -= 50;
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
            if (PainValue >= 50)
            {
                PainValue -= 50;
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
            if (ToleranceValue >= 50)
            {
                ToleranceValue -= 50;
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
            inflammationText.text = InflammationValue.ToString();

        if (painText != null)
            painText.text = PainValue.ToString();

        if (toleranceText != null)
            toleranceText.text = ToleranceValue.ToString();
    }
}