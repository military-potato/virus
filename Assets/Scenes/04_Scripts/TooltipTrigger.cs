using UnityEngine;
using UnityEngine.EventSystems; // 마우스 이벤트를 받기 위해 필수!

// 이 스크립트가 붙으면 마우스 진입/이탈 이벤트를 감지할 수 있습니다.
public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Tooltip Data")]
    [SerializeField] private Sprite itemSprite;       // 띄울 이미지
    [SerializeField] private string itemName;         // 이름 (예: "아군 유닛 A")
    [TextArea(3, 5)] 
    [SerializeField] private string itemDescription;  // 설명 (예: "강력한 근접 공격을 합니다.")

    // 마우스 커서가 버튼 영역 안으로 들어왔을 때 유니티가 자동으로 호출함
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Tooltip.Instance != null)
        {
            // 왼쪽 UI 매니저에게 내 데이터를 넘겨주며 켜달라고 요청
            Tooltip.Instance.ShowDescription(itemSprite, itemName, itemDescription);
        }
    }

    // 마우스 커서가 버튼 영역 밖으로 나갔을 때 유니티가 자동으로 호출함
    public void OnPointerExit(PointerEventData eventData)
    {
        if (Tooltip.Instance != null)
        {
            // 왼쪽 UI 매ni저에게 창을 닫아달라고 요청
            Tooltip.Instance.HideDescription();
        }
    }

    // 오브젝트가 도중에 비활성화되면 툴팁도 안전하게 닫히도록 예외처리
    private void OnDisable()
    {
        if (Tooltip.Instance != null)
        {
            Tooltip.Instance.HideDescription();
        }
    }
}