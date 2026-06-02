using UnityEngine;
using UnityEngine.UI; // Image 컴포넌트용
using TMPro;        // TextMeshPro용

public class Tooltip : MonoBehaviour
{
    public static Tooltip Instance { get; private set; }

    [Header("UI Elements")]
    [SerializeField] private GameObject displayPanel; // 왼쪽 UI 전체를 담고 있는 패널
    [SerializeField] private Image contentImage;       // 이미지가 표시될 컴포넌트
    [SerializeField] private TextMeshProUGUI titleText; // 제목/이름 텍스트
    [SerializeField] private TextMeshProUGUI descText;  // 설명 텍스트

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // 게임 시작 시에는 설명창을 꺼둡니다.
        HideDescription();
    }

    // 마우스를 올렸을 때 호출할 함수
    public void ShowDescription(Sprite imageSprite, string title, string description)
    {
        if (displayPanel == null) return;

        // 데이터 반영
        if (contentImage != null && imageSprite != null)
        {
            contentImage.sprite = imageSprite;
            contentImage.gameObject.SetActive(true);
        }
        else if (contentImage != null)
        {
            contentImage.gameObject.SetActive(false); // 이미지가 없을 때 처리
        }

        if (titleText != null) titleText.text = title;
        if (descText != null) descText.text = description;

        // UI 켜기
        displayPanel.SetActive(true);
    }

    // 마우스가 나갔을 때 호출할 함수
    public void HideDescription()
    {
        if (displayPanel != null)
        {
            displayPanel.SetActive(false);
        }
    }
}