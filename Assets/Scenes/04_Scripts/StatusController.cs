using UnityEngine;
using TMPro;
using System.Collections;

public class StatusController : MonoBehaviour
{
    public static StatusController Instance { get; private set; }

    [Header("[ UI Text References ]")]
    [SerializeField] private TextMeshProUGUI inflammationText;
    [SerializeField] private TextMeshProUGUI painText;
    [SerializeField] private TextMeshProUGUI toleranceText;

    [Header("[ Popup UI Reference ]")]
    [SerializeField] private GameObject warningPopup;
    [SerializeField] private float popupDuration = 1.0f;


    [Header("[ Auto Increase Settings ]")]
    [SerializeField] private bool isAutoIncreasing = true;
    [SerializeField] private int inflammationIncreaseAmount = 5;
    [SerializeField] private int painIncreaseAmount = 5;
    [SerializeField] private float increaseInterval = 1.0f;

    // 각 수치를 저장할 내부 변수
    public int InflammationValue { get; private set; } = 50;
    public int PainValue { get; private set; } = 0;
    public int ToleranceValue { get; private set; } = 0;

    private Coroutine activePopupCoroutine;

    private void Awake()
    {
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


        if (warningPopup != null)
            warningPopup.SetActive(false);

        StartCoroutine(AutoIncreaseStatusRoutine());
    }


    private IEnumerator AutoIncreaseStatusRoutine()
    {
        while (true)
        {

            yield return new WaitForSeconds(increaseInterval);
            
            if (isAutoIncreasing)
            {
                InflammationValue += inflammationIncreaseAmount;
                PainValue += painIncreaseAmount;
                
                UpdateAllUI();
            }
        }
    }
    
    public bool TrySpendInflammation(int amount)
    {
        if (InflammationValue >= amount)
        {
            InflammationValue -= amount; 
            UpdateAllUI();
            return true;
        }
        else
        {
            TriggerWarning();
            Debug.LogWarning("염증 수치가 부족하여 아군을 소환할 수 없습니다!");
            return false;
        }
    }
    
    public bool TrySpendPain(int amount)
    {
        if (PainValue >= amount)
        {
            PainValue -= amount; 
            UpdateAllUI();
            return true;
        }
        else
        {
            TriggerWarning();
            Debug.LogWarning("고통 수치가 부족하여 아이템을 사용할 수 없습니다!");
            return false;
        }
    }
    
    public void SubTolerance(int amount)
    {
        if (ToleranceValue >= amount)
        {
            ToleranceValue -= amount; 
            UpdateAllUI();
        }
    }
    
    public void AddValues(int inflammation, int pain, int tolerance)
    {
        InflammationValue += inflammation;
        PainValue += pain;
        ToleranceValue += tolerance;
        UpdateAllUI();
    }

    private void TriggerWarning()
    {
        if (warningPopup == null) return;

        if (activePopupCoroutine != null)
        {
            StopCoroutine(activePopupCoroutine);
        }

        activePopupCoroutine = StartCoroutine(ShowAndHidePopup());
    }


    private IEnumerator ShowAndHidePopup()
    {
        warningPopup.SetActive(true); 

        yield return new WaitForSeconds(popupDuration); 

        warningPopup.SetActive(false); 
        activePopupCoroutine = null;
    }
    
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