using UnityEngine;

public class NKSkill : AllyUnit
{
    [Header("NK Cell Custom Settings")]
    public GameObject hojunguPrefab;         // 소환할 호중구 프리팹을 연결하는 슬롯
    public float firstSpawnTime = 15f;       // 첫 호중구 소환 시간 (15초)
    public float respawnTime = 30f;          // 이후 호중구 재생성 시간 (30초)
    public float spawnOffsetDistance = 1.2f; // NK세포 중심에서 좌우로 얼마나 떨어진 곳에 소환할 것인가

    private float spawnTimer;
    private bool isFirstSpawnDone = false;   // 첫 소환 완료 여부 플래그

    protected override void Start()
    {
        base.Start();

        // 기획서: 첫 소환 시간은 15초로 시계를 맞춥니다.
        spawnTimer = firstSpawnTime;
    }

    // 부모(AllyUnit)의 Update 로직(AI 탐색, 이동, 공격)을 그대로 유지하면서 호중구 소환 타이머만 추가합니다.
    protected override void Update()
    {
        base.Update(); // 부모의 AI 전투/추적 메커니즘 실행

        // 호중구 소환 시계 관리
        HandleHojunguSpawnTimer();
    }

    private void HandleHojunguSpawnTimer()
    {
        if (spawnTimer > 0)
        {
            spawnTimer -= Time.deltaTime;

            if (spawnTimer <= 0)
            {
                // 호중구 2마리 소환 실행!
                SpawnHojunguUnits();

                // ★ 기획서 반영: 첫 소환(15초)이 끝났으므로, 다음부터는 재생성 시간(30초)으로 시계를 맞춥니다.
                spawnTimer = respawnTime;
            }
        }
    }

    private void SpawnHojunguUnits()
    {
        if (hojunguPrefab == null)
        {
            Debug.LogError("[NK세포] 호중구 프리팹(Hojungu Prefab)이 인스펙터에 등록되지 않았습니다!");
            return;
        }

        Debug.Log("[NK세포 능력 발동] 양옆에 호중구를 각각 1마리씩, 총 2마리 소환합니다!");

        // 1. NK세포 기준 오른쪽과 왼쪽 방향 벡터 계산
        Vector3 rightOffset = transform.right * spawnOffsetDistance;
        Vector3 leftOffset = -transform.right * spawnOffsetDistance;

        // 2. 최종 소환될 세계(World) 좌표 지정
        Vector3 spawnPositionRight = transform.position + rightOffset;
        Vector3 spawnPositionLeft = transform.position + leftOffset;

        // 3. 오른쪽 유닛 소환
        Instantiate(hojunguPrefab, spawnPositionRight, Quaternion.identity);

        // 4. 왼쪽 유닛 소환
        Instantiate(hojunguPrefab, spawnPositionLeft, Quaternion.identity);
    }

    // 에디터 뷰에서 호중구가 대략 어디쯤 소환되는지 노란색 점으로 시각화 (테스트용)
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 rightOffset = transform.right * spawnOffsetDistance;
        Gizmos.DrawSphere(transform.position + rightOffset, 0.2f);
        Gizmos.DrawSphere(transform.position - rightOffset, 0.2f);
    }
}
