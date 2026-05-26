using UnityEngine;

public class TabGroupController : MonoBehaviour
{
    [System.Serializable]
    public class TabGroup
    {
        public GameObject groupObject;      // Group_Blue 또는 Group_Orange 부모 폴더
        public SpriteRenderer tabBarSprite; // blue_bar 또는 orange_bar 오브젝트
    }

    [Header("Group Settings")]
    [SerializeField] private TabGroup blueGroup;
    [SerializeField] private TabGroup orangeGroup;

    private void Start()
    {
        // 게임 시작 시 파란색 유닛 탭을 기본 활성화
        SelectBlueTab();
    }

    public void SelectBlueTab()
    {
        // 1. 바 이미지 자체의 Sorting Order 우선 조절
        if (blueGroup.tabBarSprite != null) blueGroup.tabBarSprite.sortingOrder = 20;
        if (orangeGroup.tabBarSprite != null) orangeGroup.tabBarSprite.sortingOrder = 10;

        // 2. 물리적 클릭 순위 보정을 위해 최하단 sibling 배치
        if (blueGroup.groupObject != null) blueGroup.groupObject.transform.SetAsLastSibling();

        // 3. 내부 요소 On/Off 제어
        ToggleGroupContents(blueGroup, true);
        ToggleGroupContents(orangeGroup, false);
    }

    public void SelectOrangeTab()
    {
        // 1. 바 이미지 자체의 Sorting Order 우선 조절
        if (orangeGroup.tabBarSprite != null) orangeGroup.tabBarSprite.sortingOrder = 20;
        if (blueGroup.tabBarSprite != null) blueGroup.tabBarSprite.sortingOrder = 10;

        // 2. 물리적 클릭 순위 보정을 위해 최하단 sibling 배치
        if (orangeGroup.groupObject != null) orangeGroup.groupObject.transform.SetAsLastSibling();

        // 3. 내부 요소 On/Off 제어
        ToggleGroupContents(orangeGroup, true);
        ToggleGroupContents(blueGroup, false);
    }

    private void ToggleGroupContents(TabGroup tabGroup, bool isActive)
    {
        if (tabGroup.groupObject == null || tabGroup.tabBarSprite == null) return;

        // 부모 그룹 하위의 모든 Transform 요소를 계층 깊이에 상관없이 전수 조사합니다.
        Transform[] allChildren = tabGroup.groupObject.GetComponentsInChildren<Transform>(true);
        GameObject protectedBar = tabGroup.tabBarSprite.gameObject;

        foreach (Transform child in allChildren)
        {
            // 최상위 부모 폴더 자체는 끄지 않습니다.
            if (child.gameObject == tabGroup.groupObject) continue;

            // 현재 검사 중인 오브젝트가 등록된 '배경 바(Bar)' 본인이거나, 
            // 혹은 그 배경 바의 하위에 종속된 자식 노드(글자, 데코 등)라면 강제로 활성화하고 스킵합니다.
            if (child.gameObject == protectedBar || child.IsChildOf(protectedBar.transform))
            {
                child.gameObject.SetActive(true);
                continue;
            }

            // 위의 보호 대상(바 관련 오브젝트 전체)이 아닌 순수 버튼 콘텐츠 오브젝트들만 타겟으로 삼아 켜고 끕니다.
            child.gameObject.SetActive(isActive);
        }
    }
}