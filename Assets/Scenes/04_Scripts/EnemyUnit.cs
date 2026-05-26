using UnityEngine;


// UnitBase를 상속받아 기본 체력, 속도, 사거리 변수를 그대로 활용합니다.
public class EnemyUnit : UnitBase
{

    public enum State { MoveToGoal, Chasing, Attacking }

    [Header("AI Settings")]
    public State currentState = State.MoveToGoal;
    public GameObject baseTarget;       // 기지 위치 (인스펙터에서 할당 혹은 Start에서 자동 검색)
    public float verticalRatio = 0.7f;  // 2.5D 수직 속도 보정값

    [Header("Combat Settings")]
    public float attackRange = 1.5f;



    private float lastAttackTime;       // 마지막 공격 시간 기록 - 추가    // 1.0f 대신 인스펙터에서 조절 가능한 변수로 승격!

    private Transform currentTarget;

    // 부모인 UnitBase의 Start()를 실행하면서 추가 설정을 진행합니다.
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
            // 만약 기지(gizi_0)에 UnitBase가 없고 콜라이더만 있다면?
            // 기지의 CircleCollider2D 등을 활용하거나, 기지용 스크립트(UnitBase 상속)를 기지에 달아주는 것이 좋습니다.
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
            // ★ [수정] 부모(UnitBase)가 Update에서 알아서 깎아주는 attackCooldown 시계가 0 이하가 되었는지 확인합니다.
            if (attackCooldown <= 0)
            {
                AttackTarget();

                // ★ [중요] 공격을 했으니, 인스펙터 창에서 설정한 Attack Rate(공격 간격) 수치로 쿨타임을 다시 가득 채워줍니다!
                attackCooldown = attackRate;
            }
            return;
        }

        // [이동 상태] 방향 계산 및 2.5D 보정 (기존 코드 유지)
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

    // 에디터에서 범위를 시각적으로 보여주는 기즈모
    protected void OnDrawGizmosSelected()
    {
        // 1. 노란색 원: 아군을 포착하는 센서 범위 (UnitBase의 detectRange)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        // 2. 빨간색 원: 타겟 앞에서 멈추는 공격 사거리 범위 (attackRange)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}