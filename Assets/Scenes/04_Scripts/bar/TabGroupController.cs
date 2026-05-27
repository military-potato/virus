using UnityEngine;

public class TabGroupController : MonoBehaviour
{
    [System.Serializable]
    public class TabGroup
    {
        public string tabName;              // 인스펙터 구분을 위한 이름 (예: Blue, Orange, Green)
        public GameObject groupObject;      // 부모 폴더 (Group_Blue, Group_Orange, Group_Green)
        public SpriteRenderer tabBarSprite; // 배경 바 오브젝트 (blue_bar, orange_bar, green_bar)
    }

    [Header("Tab Group Register")]
    [SerializeField] private TabGroup[] allTabs; // [수정] 탭들을 배열로 관리하여 수동 누락을 방지합니다.

    private void Start()
    {
        // 게임 시작 시 첫 번째 탭(기본 파란색 유닛 탭)을 활성화
        if (allTabs != null && allTabs.Length > 0)
        {
            SelectTab(0);
        }
    }

    // 하위 호환성을 위해 기존 메서드 명칭과 기능을 그대로 유지합니다.
    public void SelectBlueTab() { SelectTab(0); }
    public void SelectOrangeTab() { SelectTab(1); }
    public void SelectGreenTab() { SelectTab(2); } // [추가] 초록색 스킬 바 메서드

    // [핵심 로직] 선택된 인덱스에 따라 모든 탭의 레이어와 콘텐츠를 유기적으로 제어합니다.
    private void SelectTab(int targetIndex)
    {
        if (allTabs == null || targetIndex < 0 || targetIndex >= allTabs.Length) return;

        for (int i = 0; i < allTabs.Length; i++)
        {
            TabGroup tab = allTabs[i];
            if (tab == null || tab.groupObject == null || tab.tabBarSprite == null) continue;

            if (i == targetIndex)
            {
                // 1. 선택된 탭을 가장 앞으로 (바: 20, 하위 요소: 29)
                tab.tabBarSprite.sortingOrder = 20;
                SetChildrenSpritesLayer(tab.groupObject, tab.tabBarSprite, 29);

                // 2. 물리적 클릭 순위 보정을 위해 최하단 배치
                tab.groupObject.transform.SetAsLastSibling();

                // 3. 내부 요소 활성화
                ToggleGroupContents(tab, true);
            }
            else
            {
                // 선택되지 않은 탭들은 뒤로 숨김 (바: 10, 하위 요소: 11)
                tab.tabBarSprite.sortingOrder = 10;
                SetChildrenSpritesLayer(tab.groupObject, tab.tabBarSprite, 11);

                // 내부 요소 비활성화
                ToggleGroupContents(tab, false);
            }
        }
    }

    // 내부 자식 스프라이트들의 레이어를 일괄 변경하는 유틸리티 메서드
    private void SetChildrenSpritesLayer(GameObject parent, SpriteRenderer rootBar, int order)
    {
        SpriteRenderer[] sprites = parent.GetComponentsInChildren<SpriteRenderer>(true);
        foreach (SpriteRenderer sprite in sprites)
        {
            if (sprite == rootBar) continue;
            if (!sprite.transform.IsChildOf(rootBar.transform))
            {
                sprite.sortingOrder = order;
            }
        }
    }

    private void ToggleGroupContents(TabGroup tabGroup, bool isActive)
    {
        if (tabGroup.groupObject == null || tabGroup.tabBarSprite == null) return;

        Transform[] allChildren = tabGroup.groupObject.GetComponentsInChildren<Transform>(true);
        GameObject protectedBar = tabGroup.tabBarSprite.gameObject;

        foreach (Transform child in allChildren)
        {
            if (child.gameObject == tabGroup.groupObject) continue;

            if (child.gameObject == protectedBar || child.IsChildOf(protectedBar.transform))
            {
                child.gameObject.SetActive(true);
                continue;
            }

            child.gameObject.SetActive(isActive);
        }
    }
}