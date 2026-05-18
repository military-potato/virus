using UnityEngine;

public class LymphocyteSkill : AllyUnit
{
    [Header("Lymphocyte Production Settings")]
    public float resistanceAmount = 10f;       // 내성수치 생산량
    public float firstProductionTime = 15f;   // 첫 내성수치 생산 시간 (15초)
    public float reproductionTime = 30f;      // 이후 내성수치 재생산 시간 (30초)

    [Header("Evade AI Settings (생산 유닛 메커니즘)")]
    public float evadeRangeMultiplier = 6f;   // 감지 범위 배율 (반지름 크기 radius의 6배 = 6UR)
    public float pingInterval = 0.5f;          // 몇 초마다 도망칠 좌표를 갱신할 것인가
    public float pingDistance = 3.0f;         // 한 번 좌표를 찍을 때 얼마나 멀리 찍을 것인가

    private float productionTimer;
    private bool isInitialized = false;

    // 회피 이동(핑 찍기) 관련 변수
    private Vector3 targetPingPosition;       // 현재 찍혀있는 목적지 좌표
    private float pingUpdateTimer;            // 좌표 갱신 주기 타이머
    private bool isEvading = false;           // 현재 도망치는 중인가 플래그

    // 필드 최대 1마리 존재 제한용 스태틱 변수
    private static int activeLymphocyteCount = 0;

    protected override void Start()
    {
        // 1. 중복 생성 차단
        if (activeLymphocyteCount > 0)
        {
            Debug.LogWarning("[림프구 제한] 필드에 이미 림프구가 존재합니다! 생성을 차단합니다.");
            Destroy(gameObject);
            return;
        }

        activeLymphocyteCount++;
        isInitialized = true;

        base.Start();

        // 타이머 및 초기 좌표(현재 위치) 설정
        productionTimer = firstProductionTime;
        targetPingPosition = transform.position;
        pingUpdateTimer = 0f;
    }

    // ★ 부모의 Update를 완전히 오버라이드하여, 적에게 돌격하는 일반 공격 AI를 끄고 회피 AI를 구동합니다.
    protected override void Update()
    {
        if (!isInitialized) return;

        // 1. 내성 수치 자원 생산 타이머 관리
        HandleResistanceProduction();

        // 2. 주변 6UR 범위 적 감지 및 회피(핑) 로직
        HandleEvadeAI();
    }

    private void HandleEvadeAI()
    {
        pingUpdateTimer -= Time.deltaTime;

        // 지정된 주기(pingInterval, 예: 0.5초)마다 주변 적을 탐색해 안전한 좌표(핑)를 새로 계산합니다.
        if (pingUpdateTimer <= 0f)
        {
            pingUpdateTimer = pingInterval;
            CalculateEscapePing();
        }

        // 3. 찍힌 핑(목적지)을 향해 2.5D 보정 속도로 이동합니다.
        if (isEvading)
        {
            Vector3 dir = (targetPingPosition - transform.position).normalized;
            Vector3 velocity = new Vector3(dir.x, dir.y * verticalRatio, 0);
            transform.position += velocity * moveSpeed * Time.deltaTime;

            // 목적지에 거의 도달했다면 회피 모드 일시 정지
            if (Vector2.Distance(transform.position, targetPingPosition) < 0.2f)
            {
                isEvading = false;
            }
        }
    }

    // ★ 수정된 핑 계산 함수 (태그 대신 스크립트 검사)
    private void CalculateEscapePing()
    {
        float detectRadius = radius * evadeRangeMultiplier;

        // 범위 내 모든 콜라이더 탐색
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, detectRadius);

        Vector2 enemyCenterOfMass = Vector2.zero;
        int enemyCount = 0;

        foreach (Collider2D collider in hitColliders)
        {
            if (collider == null) continue;

            // 🔥 [핵심 수정] 태그 대신, 부딪힌 오브젝트에 EnemyUnit 스크립트가 붙어있는지 확인합니다!
            EnemyUnit enemy = collider.GetComponent<EnemyUnit>();

            // EnemyUnit 스크립트를 가지고 있는 유닛이라면 적군으로 판정!
            if (enemy != null)
            {
                enemyCenterOfMass += (Vector2)collider.transform.position;
                enemyCount++;
            }
        }

        // 범위 안에 EnemyUnit을 가진 적이 한 마리라도 있다면 회피
        if (enemyCount > 0)
        {
            enemyCenterOfMass /= enemyCount;
            Vector2 escapeDirection = ((Vector2)transform.position - enemyCenterOfMass).normalized;

            targetPingPosition = transform.position + (Vector3)escapeDirection * pingDistance;
            isEvading = true;

            Debug.Log($"[림프구 감지] EnemyUnit 적 {enemyCount}마리 발견! 왼쪽/안전구역으로 도망칩니다.");
        }
        else
        {
            isEvading = false;
        }
    }

    private void HandleResistanceProduction()
    {
        if (productionTimer > 0)
        {
            productionTimer -= Time.deltaTime;
            if (productionTimer <= 0)
            {
                ProduceResistance();
                productionTimer = reproductionTime; // 다음 생산부터는 재생산 주기(30초) 적용
            }
        }
    }

    private void ProduceResistance()
    {
        Debug.Log($"[내성 수치 생산] 내성 수치를 {resistanceAmount} 만큼 생산했습니다!");
    }

    private void OnDestroy()
    {
        if (isInitialized)
        {
            activeLymphocyteCount--;
            if (activeLymphocyteCount < 0) activeLymphocyteCount = 0;
            Debug.Log("[림프구 사멸] 필드의 림프구가 사라져 이제 새로 생산할 수 있습니다.");
        }
    }

    // 에디터 뷰에서 6UR 회피 감지 범위를 시각적으로 확인 (선택 시 하늘색 원 표시)
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius * evadeRangeMultiplier);

        // 현재 도망치고 있는 목적지(핑)가 있다면 빨간 점으로 선 연결 표시
        if (isEvading)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, targetPingPosition);
            Gizmos.DrawSphere(targetPingPosition, 0.2f);
        }
    }
}