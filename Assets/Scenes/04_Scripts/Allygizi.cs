using UnityEngine;

public class Allygizi : UnitBase
{
    public enum State { Idle, Chasing, Attacking }

    [Header("AI Settings")]
    public State currentState = State.Idle;
    public float verticalRatio = 0.7f;  // 2.5D 수직 속도 보정값

    [Header("Combat Settings")]
    public float attackRange = 1.5f;    // 무기 자체의 순수 사거리

    protected Transform currentTarget;
    private Vector3 spawnPosition;      // 원래 대기하던 위치 기록용

    protected override void Start()
    {
        base.Start();
        spawnPosition = transform.position; 
    }

    protected override void Update()
    {
        // 부모(UnitBase)의 Update를 실행시켜 attackCooldown 시계를 매 프레임 깎음
        base.Update(); 

        // 1. 적 탐색 및 상태 업데이트
        FindClosestEnemy();
        UpdateState();

        // 2. 행동 수행 (이동 및 공격)
        HandleAction();
    }

    protected virtual void FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);

            if (distance < shortestDistance && distance <= detectRange)
            {
                shortestDistance = distance;
                nearestEnemy = enemy;
            }
        }

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

        // 1. 중심점 간의 거리 계산
        float distance = Vector2.Distance(transform.position, currentTarget.position);

        // 2. 타겟의 반지름 값 가져오기
        float targetRadius = 0f;
        UnitBase targetUnit = currentTarget.GetComponent<UnitBase>();
        if (targetUnit != null)
        {
            targetRadius = targetUnit.radius;
        }

        // 3. 아군 유닛에도 [외곽 정지 공식] 동적 적용
        float stopDistance = this.radius + targetRadius + attackRange;

        // 4. 거리에 따른 상태 전환
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
        if (currentState == State.Idle)
        {
            float distToSpawn = Vector2.Distance(transform.position, spawnPosition);
            if (distToSpawn > 0.2f)
            {
                Vector3 dir = (spawnPosition - transform.position).normalized;
                Vector3 velocity = new Vector3(dir.x, dir.y * verticalRatio, 0);
                transform.position += velocity * moveSpeed * Time.deltaTime;
            }
            return;
        }

        if (currentState == State.Attacking)
        {
            if (currentTarget == null) return;

            // 부모(UnitBase)가 깎아놓은 타이머가 0 이하인지 확인
            if (attackCooldown <= 0)
            {
                AttackTarget();
                attackCooldown = attackRate; // 쿨타임 리셋
            }
            return;
        }

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
        Gizmos.DrawWireSphere(transform.position, detectRange);

        // 기즈모도 내 반지름을 포함한 기본 저지선 범위를 그리도록 시각화 보정
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius + attackRange); 
    }
    // 만약 부모(UnitBase)에 데미지를 받거나 죽는 가상 함수가 있다면 오버라이드
    // (부모 스크립트의 죽는 함수 이름이 'Die' 또는 'OnDeath'인지 확인)
    public override void TakeDamage(float amount)
    {
        // 부모의 원래 데미지 계산(체력 감소 등)을 먼저 실행
        base.TakeDamage(amount);

        // 만약 부모에 구현된 현재 체력 변수(예: currentHp 등)가 0 이하가 되었다면
        // 체력 변수 이름은 수정할 것(예: hp, currentHealth)
        if (currentHealth <= 0) 
        {
            // 전역에 있는 GameOverManager를 찾아서 게임오버
            gameover gameOverManager = FindObjectOfType<gameover>();
            if (gameOverManager != null)
            {
                gameOverManager.TriggerGameOver();
            }
        }
    }
}