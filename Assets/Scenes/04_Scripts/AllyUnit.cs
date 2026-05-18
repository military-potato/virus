using UnityEngine;

public class AllyUnit : UnitBase
{
    public enum State { Idle, Chasing, Attacking }

    [Header("AI Settings")]
    public State currentState = State.Idle;
    public float verticalRatio = 0.7f;  // 2.5D 수직 속도 보정값

    [Header("Combat Settings")]
    public float attackRange = 1.5f;    // 멈춰서 공격할 사거리
    public float attackCooldown = 1.0f; // 공격 속도 (초)
    public float damage = 10f;          // 공격력

    private Transform currentTarget;
    private float lastAttackTime;
    private Vector3 spawnPosition;      // 원래 대기하던 위치 기록용

    // [추가] 몸통 박치기 연출 컴포넌트 참조 변수
    private CellActionFX actionFX;

    protected override void Start()
    {
        base.Start();
        spawnPosition = transform.position; // 스폰된 위치를 집(대기소)으로 지정
        
        // [추가] 내 몸에 붙어있는 연출 컴포넌트를 가져옴
        actionFX = GetComponent<CellActionFX>();

    }

    void Update()
    {
        // 1. 적 탐색 및 상태 업데이트
        FindClosestEnemy();
        UpdateState();

        // 2. 행동 수행 (이동 및 공격)
        HandleAction();
    }

    void FindClosestEnemy()
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

    void UpdateState()
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

    void HandleAction()
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

            // 주기적으로 타겟에게 데미지를 입힘
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                AttackTarget();
                lastAttackTime = Time.time;
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

    void AttackTarget()
    {
        if (currentTarget == null) return;
        // [연출 이어붙이기] 데미지를 주기 직전에 타겟을 향해 몸통 박치기 연출을 실행합니다.
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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectRange); // 인식 범위 (초록색)

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange); // 공격 사거리 (빨간색)
    }
}