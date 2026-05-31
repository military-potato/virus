using UnityEngine;

public class OverGrowSkill : EnemyUnit
{
    [Header("Host Cell Spawn Settings")]
    public GameObject powerfulVirusPrefab;   // 소환할 강력 바이러스 프리팹 (인스펙터에서 등록)
    public float firstSpawnTime = 15f;       // 첫 소환 시간 (15초)
    public float respawnTime = 30f;          // 이후 재생산 시간 (30초)

    [Tooltip("숙주세포 중심에서 좌우로 얼마나 떨어진 곳에 소환할 것인가 (몸집이 크므로 1.5 전후 추천)")]
    public float spawnOffsetDistance = 1.5f;

    private float spawnTimer;

    protected override void Start()
    {
        // 부모(EnemyUnit -> UnitBase)의 초기화 실행 (중앙 기지 타겟 탐색 및 체력 설정)
        base.Start();

        // 첫 시계는 15초로 맞춤
        spawnTimer = firstSpawnTime;
    }

    protected override void Update()
    {
        // 부모(EnemyUnit)의 기본 AI 로직 실행 (중앙 기지를 향해 이동 및 아군 방해석 탐색/공격)
        base.Update();

        // 강력 바이러스 소환 시계 관리
        HandleVirusSpawnTimer();
    }

    private void HandleVirusSpawnTimer()
    {
        if (spawnTimer > 0)
        {
            spawnTimer -= Time.deltaTime;

            if (spawnTimer <= 0)
            {
                // 강력 바이러스 2마리 양옆 소환
                SpawnPowerfulViruses();

                // 재생산 시간(30초)으로 타이머 리셋
                spawnTimer = respawnTime;
            }
        }
    }

    private void SpawnPowerfulViruses()
    {
        if (powerfulVirusPrefab == null)
        {
            Debug.LogError("[과성장 숙주세포] 소환할 강력 바이러스 프리팹(Powerful Virus Prefab)이 연결되지 않았습니다!");
            return;
        }

        Debug.Log("[과성장 숙주세포 능력 발동] 양옆에 강력 바이러스를 1마리씩, 총 2마리 소환합니다!");

        // 생성위치 벡터 계산:
        Vector3 rightOffset = Vector3.right * spawnOffsetDistance;
        Vector3 leftOffset = Vector3.left * spawnOffsetDistance;

        // 최종 소환될 월드 좌표 벡터 계산
        Vector3 spawnPosRight = transform.position + rightOffset;
        Vector3 spawnPosLeft = transform.position + leftOffset;

        // 오른쪽과 왼쪽에 각각 1마리씩 총 2마리 복제 생성
        Instantiate(powerfulVirusPrefab, spawnPosRight, Quaternion.identity);
        Instantiate(powerfulVirusPrefab, spawnPosLeft, Quaternion.identity);
    }

    // 에디터 뷰에서 소환될 좌우 위치를 마젠타(분홍)색 점으로 미리 보여주는 기즈모
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Vector3 rightOffset = Vector3.right * spawnOffsetDistance;
        Gizmos.DrawWireSphere(transform.position + rightOffset, 0.3f);
        Gizmos.DrawWireSphere(transform.position - rightOffset, 0.3f);
    }
}