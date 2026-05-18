using UnityEngine;
using UnityEngine.UI;

public class UnitSelect : MonoBehaviour
{
    [Header("[ Unit Info ]")]
    public GameObject unitPrefab; // 이 버튼이 담당할 아군 유닛 프리팹 (프로젝트 창에서 드래그)
    
    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            // 버튼 클릭 시 유닛을 장전하는 함수를 자동으로 연결합니다.
            button.onClick.AddListener(SelectThisUnit);
        }
    }

    public void SelectThisUnit()
    {
        if (unitPrefab == null)
        {
            Debug.LogWarning($"{gameObject.name} 버튼에 유닛 프리팹이 연결되지 않았습니다!");
            return;
        }

        // AllySpawner에게 "이 유닛을 소환할 준비를 해라" 하고 넘겨줍니다.
        allySpawn.Instance.SetSelectedUnit(unitPrefab);
    }
}