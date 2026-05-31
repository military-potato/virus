using UnityEngine;

public class AllyUnit : UnitBase
{
    public enum State { Idle, Chasing, Attacking }

    [Header("AI Settings")]
    public State currentState = State.Idle;
    public float verticalRatio = 0.7f;  // 2.5D 수직 속도 보정값

    [Header("Combat Settings")]
    public float attackRange = 1.5f;    // 멈춰서 공격할 사거리

    protected Transform currentTarget;
    private float lastAttackTime;
    private Vector3 spawnPosition;      // 원래 대기하던 위치 기록용


    protected override void Start()
    {
        base.Start();
        spawnPosition = transform.position; // 스폰된 위치를 집(대기소)으로 지정
    }

    protected override void Update()
    {
        // 1. 적 탐색 및 상태 업데이트
        FindClosestEnemy();
        UpdateState();

        // 2. 행동 수행 (이동 및 공격)
        HandleAction();
    }

    protected virtual void FindClosestEnemy()
    {
        // "Enemy" 태그를 가진 모든 적 오브젝트 탐색
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);

            // UnitBase의 detectRange(인식 범위) 내에 있는지 확인
            if (distance < shortestDistance && distance <= detectRange)
            {
                shortestDistance = distance;
                nearestEnemy = enemy;
            }
        }

        // 범위 내 적이 있다면 추적, 없으면 제자리(혹은 스폰 위치)로 복귀
        if (nearestEnemy != null)
        {
            currentTarget = nearestEnemy.transform;
            currentState = State.Chasing;
        }
        else
        {
            currentTarget = null;
            currentState = State.Idle;
        }
    }

    // ================= [이 부분이 수정되었습니다] =================
    protected void UpdateState()
    {
        if (attackCooldown > 0)
            attackCooldown -= Time.deltaTime;
        
        if (currentTarget == null) return;

        // 1. 나와 타겟 사이의 실제 중심점 간 거리 계산
        float distance = Vector2.Distance(transform.position, currentTarget.position);

        // 2. 타겟(적)의 UnitBase 컴포넌트를 가져와 반지름 확인
        float targetRadius = 0f;
        UnitBase targetUnit = currentTarget.GetComponent<UnitBase>();
        if (targetUnit != null)
        {
            targetRadius = targetUnit.radius;
        }

        // 3. [핵심] 외곽 정지 공식 적용: 내 반지름 + 상대 반지름 + 내 공격 사거리
        float stopDistance = this.radius + targetRadius + attackRange;

        // 4. 계산된 동적 거리 기준으로 상태 전환
        if (distance <= stopDistance)
        {
            currentState = State.Attacking;
        }
        else
        {
            currentState = State.Chasing;
        }
    }

    protected void HandleAction()
    {
        // 1. 대기 상태 (적이 없을 때)
        if (currentState == State.Idle)
        {
            // 원래 스폰 되었던 자리로 복귀하는 로직 (기지 주변을 지키게 함)
            float distToSpawn = Vector2.Distance(transform.position, spawnPosition);
            if (distToSpawn > 0.2f)
            {
                Vector3 dir = (spawnPosition - transform.position).normalized;
                Vector3 velocity = new Vector3(dir.x, dir.y * verticalRatio, 0);
                transform.position += velocity * moveSpeed * Time.deltaTime;
            }
            return;
        }

        // 2. 공격 상태
        if (currentState == State.Attacking)
        {
            if (currentTarget == null) return;

            // 부모(UnitBase)가 깎아주는 쿨타임 타이머가 0 이하가 되었는지 확인
            if (attackCooldown <= 0)
            {
                AttackTarget();

                // 공격 후, 부모가 가진 attackRate(공격 간격) 수치로 타이머를 다시 채움
                attackCooldown = attackRate;
            }
            return;
        }

        // 3. 추적 상태 (이동)
        if (currentState == State.Chasing && currentTarget != null)
        {
            Vector3 dir = (currentTarget.position - transform.position).normalized;
            Vector3 velocity = new Vector3(dir.x, dir.y * verticalRatio, 0);
            transform.position += velocity * moveSpeed * Time.deltaTime;
        }
    }

    protected void AttackTarget()
    {
        if (currentTarget == null) return;

        // 상대방의 UnitBase 컴포넌트를 가져와서 데미지를 줌
        UnitBase targetUnit = currentTarget.GetComponent<UnitBase>();
        if (targetUnit != null)
        {
            Debug.Log($"[아군] {currentTarget.name}에게 {damage} 데미지를 주었습니다.");
            targetUnit.TakeDamage(damage);
        }
    }

    // 디버그 기즈모 시각화도 외곽선 기준으로 수정하면 좋습니다.
    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectRange); // 인식 범위 (초록색)

        // 사거리 기즈모에 내 반지름을 더해서 표현해 주면 에디터에서 보기 편합니다.
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius + attackRange); // 공격 사거리 (빨간색)
    }
}