using UnityEngine;

public class EnemyUnit : UnitBase
{
    public enum State { MoveToGoal, Chasing, Attacking }

    [Header("AI Settings")]
    public State currentState = State.MoveToGoal;
    public GameObject baseTarget;       // 기지 위치 (인스펙터에서 할당 혹은 Start에서 자동 검색)
    public float verticalRatio = 0.7f;  // 2.5D 수직 속도 보정값

    [Header("Combat Settings")]
    public float attackRange = 1.5f;

    private float lastAttackTime;       // 마지막 공격 시간 기록 - 추가
    private Transform currentTarget;

    // 부모인 UnitBase의 Start()를 실행하면서 추가 설정 진행
    protected override void Start()
    {
        base.Start(); // currentHealth = maxHealth 설정 실행

        GameObject findBase = GameObject.Find("gizi_0");

        if (findBase != null)
        {
            baseTarget = findBase;
        }
        else
        {
            Debug.LogError("씬에서 기지 오브젝트를 찾을 수 없습니다! 이름을 확인하세요.");
        }
    }

    protected override void Update()
    {
        if (attackCooldown > 0)
            attackCooldown -= Time.deltaTime;
        
        // 기지가 설정되지 않았다면 로직을 실행하지 않음
        if (baseTarget == null)
        {
            Debug.LogWarning($"{gameObject.name}: Base Target이 할당되지 않았습니다!");
            return;
        }

        // 1. 타겟 탐색 및 상태 업데이트
        FindClosestAlly();
        UpdateState();

        // 2. 상태에 따른 행동 수행
        HandleAction();
    }

    protected void FindClosestAlly()
    {
        // "Ally" 태그를 가진 모든 아군 오브젝트 탐색
        GameObject[] allies = GameObject.FindGameObjectsWithTag("Ally");
        float shortestDistance = Mathf.Infinity;
        GameObject nearestAlly = null;

        foreach (GameObject ally in allies)
        {
            float distance = Vector2.Distance(transform.position, ally.transform.position);

            // UnitBase의 detectRange(인식 범위) 내에 있는지 확인
            if (distance < shortestDistance && distance <= detectRange)
            {
                shortestDistance = distance;
                nearestAlly = ally;
            }
        }

        // 범위 내 아군이 있다면 추적 상태로 전환, 없으면 기지로 향함
        if (nearestAlly != null)
        {
            currentTarget = nearestAlly.transform;
            currentState = State.Chasing;
        }
        else
        {
            currentTarget = baseTarget.transform;
            currentState = State.MoveToGoal;
        }
    }

    protected void UpdateState()
    {
        if (currentTarget == null) return;

        // 1. 나와 타겟 사이의 실제 중심점 간 거리 계산
        float distance = Vector2.Distance(transform.position, currentTarget.position);

        // 2. 타겟의 UnitBase 컴포넌트를 가져와 반지름 확인
        float targetRadius = 0f;
        UnitBase targetUnit = currentTarget.GetComponent<UnitBase>();

        if (targetUnit != null)
        {
            targetRadius = targetUnit.radius;
        }
        else
        {
            // 임시 예외 처리: 타겟이 기지일 경우 대략적인 기지 반지름 지정
            if (currentTarget == baseTarget.transform)
            {
                targetRadius = 2.0f; // 기지의 대략적인 반지름 크기
            }
        }

        // 3. 외곽 정지 공식: 내 반지름 + 상대 반지름 + 공격 사거리(무기 길이)
        float stopDistance = this.radius + targetRadius + attackRange;

        // 4. 계산된 동적 거리 기준으로 상태 전환
        if (distance <= stopDistance)
        {
            currentState = State.Attacking;
        }
        else
        {
            if (currentTarget == baseTarget.transform)
                currentState = State.MoveToGoal;
            else
                currentState = State.Chasing;
        }
    }

    protected void HandleAction()
    {
        if (currentTarget == null) return;

        if (currentState == State.Attacking)
        {
            // 부모(UnitBase)가 attackCooldown 시계가 0 이하인지 확인
            if (attackCooldown <= 0)
            {
                AttackTarget();

                // 공격 후 인스펙터 창에서 설정한 Attack Rate(공격 간격) 수치로 쿨타임 다시 채움
                attackCooldown = attackRate;
            }
            return;
        }

        // [이동 상태] 방향 계산 및 2.5D 보정
        Vector3 dir = (currentTarget.position - transform.position).normalized;
        Vector3 velocity = new Vector3(dir.x, dir.y * verticalRatio, 0);

        transform.position += velocity * moveSpeed * Time.deltaTime;
    }

    protected void AttackTarget()
    {
        if (currentTarget == null) return;

        // 타겟이 아군 유닛(UnitBase 상속)이거나 기지인 경우 데미지 전달
        UnitBase targetUnit = currentTarget.GetComponent<UnitBase>();
        if (targetUnit != null)
        {
            Debug.Log($"[적군] {currentTarget.name}을(를) 공격하여 {damage} 데미지를 입혔습니다.");
            targetUnit.TakeDamage(damage);
        }
    }

    // ================= [핵심 추가 부분] =================
    // 부모(UnitBase)의 사멸 함수를 적군용으로 재정의합니다.
    protected override void Die()
    {
        // 씬이 플레이 중이고, 웨이브 매니저(EnemySpawn)가 존재하는지 확인
        if (Application.isPlaying && EnemySpawn.Instance != null)
        {
            // [중요] 죽을 때 현재 살아있는 적의 숫자를 하나 줄입니다.
            EnemySpawn.Instance.aliveEnemyCount--;
        }

        // 부모 클래스의 원래 Die() 로직(Destroy(gameObject))을 실행합니다.
        base.Die();
    }
}