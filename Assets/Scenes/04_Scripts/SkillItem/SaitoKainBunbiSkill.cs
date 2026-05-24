using UnityEngine;
using System.Collections;

public class SaitoKainBunbiSkill : ActiveSkillBase
{
    [Header("[ 사이토카인 분비 특화 설정 ]")]
    [SerializeField] private GameObject neutrophilPrefab;
    [SerializeField] private float spawnRadius = 3f;       // 10UR 이내 원형 배치를 위한 반지름
    [SerializeField] private LayerMask groundLayer;

    // 레벨별 소환 마리수 규칙 [cite: 91]
    private int[] spawnCountByLevel = new int[3] { 5, 7, 9 };
    private bool isWaitingForClick = false;

    private void Start()
    {
        skillName = "사이토카인 분비";
        cooldownTime = 10f; // 기획서 고정 쿨타임 10초
    }

    protected override void ActivateSkillEffect()
    {
        StartCoroutine(WaitForPlayerClick());
    }

    private IEnumerator WaitForPlayerClick()
    {
        isWaitingForClick = true;
        Debug.Log($"[사이토카인 분비 Lv.{currentLevel}] 발동 (소모 염증: {costInflammationByLevel[currentLevel - 1]})");
        Debug.Log("마우스 좌클릭으로 소환할 구역을 지정하세요.");

        while (isWaitingForClick)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
                {
                    SpawnNeutrophilsCircular(hit.point);
                    isWaitingForClick = false;
                }
            }
            yield return null;
        }
    }

    private void SpawnNeutrophilsCircular(Vector3 centerPosition)
    {
        // 현재 레벨에 맞는 마리수 선택
        int spawnCount = spawnCountByLevel[currentLevel - 1];

        for (int i = 0; i < spawnCount; i++)
        {
            // n각형 꼭짓점 모양 둥근 소환 알고리즘
            float angle = i * Mathf.PI * 2 / spawnCount;
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * spawnRadius;
            Vector3 spawnPos = centerPosition + offset;

            if (neutrophilPrefab != null)
            {
                Instantiate(neutrophilPrefab, spawnPos, Quaternion.identity);
            }
        }
        Debug.Log($"[소환 완료] 호중구 {spawnCount}마리가 생성되었습니다.");
    }
}