using UnityEngine;
using UnityEngine.UI; // Image 컴포넌트를 사용하기 위해 필요합니다.

public class BaseHPViewer : MonoBehaviour
{
    [Header("Target Base")]
    [SerializeField] private UnitBase targetBase; // 체력을 가져올 기지 오브젝트

    [Header("UI Components")]
    [SerializeField] private Image hpImage;       //Slider 대신 Image로 변경합니다.

    private void Start()
    {
        // 만약 인스펙터에서 할당하지 않았다면 오브젝트에서 자동으로 Image를 찾음
        if (hpImage == null)
        {
            hpImage = GetComponent<Image>();
        }

        UpdateHPUI();
    }

    private void Update()
    {
        UpdateHPUI();
    }

    private void UpdateHPUI()
    {
        if (targetBase == null || hpImage == null) return;

        float max = targetBase.maxHealth;
        float current = targetBase.GetCurrentHealth();

        if (max > 0)
        {
            // Image의 Fill Amount 속성을 0 ~ 1 사이의 비율로 조절합니다.
            hpImage.fillAmount = current / max;
        }
    }
}