using UnityEngine;

public class EnemyUnit : UnitBase
{
    public enum State { MoveToGoal, Chasing, Attacking }

    [Header("AI Settings")]
    public State currentState = State.MoveToGoal;
    public GameObject baseTarget;       // 기지 오브젝트
    public float verticalRatio = 0.7f;  // 2.5D 수직 속도 보정값

    [Header("Combat Settings")]
    public float attackRange = 1.5f;

    private Transform currentTarget;

    protected override void Start()
    {
        base.Start(); 

        GameObject findBase = GameObject.Find("gizi_0");
        if (findBase != null)
        {
            baseTarget = findBase;
        }
        else
        {
            Debug.LogError("씬에서 기지 오브젝트(gizi_0)를 찾을 수 없습니다!");
        }
    }

    protected override void Update()
    {
        if (baseTarget == null) return;

        // ★ [중요] 부모(UnitBase)의 Cooldown 감소 로직 실행
        base.Update(); 

        // 1. 타겟 탐색 및 상태 업데이트
        FindClosestAlly();
        UpdateState();

        // 2. 상태에 따른 행동 수행
        HandleAction();
    }

    protected void FindClosestAlly()
    {
        GameObject[] allies = GameObject.FindGameObjectsWithTag("Ally");
        float shortestDistance = Mathf.Infinity;
        GameObject nearestAlly = null;

        foreach (GameObject ally in allies)
        {
            float distance = Vector2.Distance(transform.position, ally.transform.position);

            if (distance < shortestDistance && distance <= detectRange)
            {
                shortestDistance = distance;
                nearestAlly = ally;
            }
        }

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
        // 🚨 [버그 수정] 매 프레임 Update와 여기서 이중으로 쿨타임을 감소시키던 코드를 삭제했습니다.
        
        if (currentTarget == null) return;

        // 1. 나와 타겟 사이의 실제 거리를 계산
        float distance = Vector2.Distance(transform.position, currentTarget.position);

        // 2. 타겟의 반지름 확인
        float targetRadius = 0f;
        UnitBase targetUnit = currentTarget.GetComponent<UnitBase>();

        if (targetUnit != null)
        {
            targetRadius = targetUnit.radius;
        }
        else
        {
            // 타겟이 기지(gizi_0)일 경우 처리할 예외 반지름 값
            if (currentTarget == baseTarget.transform)
            {
                targetRadius = 2.0f; 
            }
        }

        // 3. 외곽 정지 공식 적용
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
            // 부모가 공평하게 깎아둔 시계를 검사합니다.
            if (attackCooldown <= 0)
            {
                AttackTarget();
                attackCooldown = attackRate; // 인스펙터 지정 수치로 리셋
            }
            return;
        }

        // 이동 상태 수행 (Chasing 또는 MoveToGoal)
        Vector3 dir = (currentTarget.position - transform.position).normalized;
        Vector3 velocity = new Vector3(dir.x, dir.y * verticalRatio, 0);

        transform.position += velocity * moveSpeed * Time.deltaTime;
    }

    protected void AttackTarget()
    {
        if (currentTarget == null) return;

        UnitBase targetUnit = currentTarget.GetComponent<UnitBase>();
        if (targetUnit != null)
        {
            Debug.Log($"[적군] {currentTarget.name}을(를) 공격하여 {damage} 데미지를 입혔습니다.");
            targetUnit.TakeDamage(damage);
        }
    }

    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius + attackRange);
    }
}