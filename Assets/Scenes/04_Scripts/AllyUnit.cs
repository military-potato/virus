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

    // [추가] 분리된 연출 스크립트를 연결하기 위한 참조 변수
    private CellActionFX actionFX;

    protected override void Start()
    {
        base.Start();
        spawnPosition = transform.position; // 스폰된 위치를 집(대기소)으로 지정

        // [추가] 내 몸뚱이에 함께 붙어있을 연출 컴포넌트를 가져옵니다.
        actionFX = GetComponent<CellActionFX>();
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

    protected void UpdateState()
    {
        if (currentTarget == null) return;

        float distance = Vector2.Distance(transform.position, currentTarget.position);

        if (distance <= attackRange)
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
            // 원래 스폰되었던 자리로 복귀하는 로직 (기지 주변을 지키게 함)
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

                // 공격 후, 부모가 가진 attackRate(공격 간격) 수치로 타이머를 다시 채웁니다!
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

        // [신호 연동] 공격 주기가 도래하여 때리는 타이밍에 분리된 연출 컴포넌트로 타겟 정보를 토스합니다.
        if (actionFX != null)
        {
            actionFX.PlayBodySlam(currentTarget);
        }

        // 상대방의 UnitBase 컴포넌트를 가져와서 데미지를 줍니다.
        UnitBase targetUnit = currentTarget.GetComponent<UnitBase>();
        if (targetUnit != null)
        {
            Debug.Log($"[아군] {currentTarget.name}에게 {damage} 데미지를 주었습니다.");
            targetUnit.TakeDamage(damage);
        }
    }

    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectRange); // 인식 범위 (초록색)

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange); // 공격 사거리 (빨간색)
    }
}