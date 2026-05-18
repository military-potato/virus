using UnityEngine;

public class HojunguSkill : AllyUnit
{
    [Header("HojunguSkill Custom Settings")]
    public float lifetime = 60f;            // 생존 시간 (60초)
    public float deathDropInflammation = 5f; // 사망 시 드랍할 염증 수치 (기획에 맞게 수정 가능)

    [Header("Toxic Fluid (독성 체액) Settings")]
    public float toxicDamage = 10f;          // 독성 체액 피해량
    public float toxicRangeMultiplier = 3f;  // 피해 범위 (반지름 크기 radius의 3배 = 3UR)

    [Header("Visual Effects")]
    public GameObject toxicEffectPrefab;     // ★ 추가: 사멸 시 생성할 독성 이미지 프리팹
    public float effectDuration = 2f;        // ★ 추가: 이미지 지속시간 2초

    private float survivalTimer;
    private bool isDead = false; // 중복 사멸 방지 플래그

    protected override void Start()
    {
        base.Start();
        survivalTimer = lifetime;
    }

    // ★ void Update() 대신 protected override void Update()를 사용하여 하단 노란색 경고(CS0114)를 해결합니다.
    protected override void Update()
    {
        // 1. AllyUnit의 기존 AI 로직 수행 (탐색, 추적, 공격)
        FindClosestEnemy();
        UpdateState();
        HandleAction();

        // 2. 시한부 생존 시간 체크
        if (!isDead)
        {
            survivalTimer -= Time.deltaTime;
            if (survivalTimer <= 0)
            {
                Die(); // 시간이 다 되면 체력 상관없이 사멸
            }
        }
    }

    // 체력이 다 닳거나, 시간이 다 되어 사멸할 때 호출됨
    protected override void Die()
    {
        if (isDead) return;
        isDead = true;

        // 1. 독성 체액 폭발 효과 및 이미지 생성 발동
        ExplodeToxicFluid();

        // 2. 염증 수치 드랍 로직
        DropInflammation();

        // 3. 실제 오브젝트 파괴 (UnitBase의 Destroy 호출)
        base.Die();
    }

    private void ExplodeToxicFluid()
    {
        // 기획서: 피해범위 3UR (유닛 반지름 radius * 3)
        float explosionRadius = radius * toxicRangeMultiplier;

        Debug.Log($"[호중구 사멸] 독성 체액 폭발! 범위: {explosionRadius}");

        // ★★★ [핵심 수정] 누락되었던 독성 체액 이미지를 화면에 생성하고 2초 뒤 삭제하는 로직입니다.
        if (toxicEffectPrefab != null)
        {
            // 호중구가 있던 자리에 독성 이펙트 프리팹 생성
            GameObject effect = Instantiate(toxicEffectPrefab, transform.position, Quaternion.identity);

            // effectDuration(2초) 뒤에 이펙트 오브젝트를 자동으로 파괴함
            Destroy(effect, effectDuration);
        }

        // 폭발 범위 내의 모든 콜라이더 탐색 (2D 게임 기준 Physics2D 사용)
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D collider in hitColliders)
        {
            // 적 태그를 가진 오브젝트인지 확인
            if (collider.CompareTag("Enemy"))
            {
                UnitBase enemyUnit = collider.GetComponent<UnitBase>();
                if (enemyUnit != null)
                {
                    Debug.Log($"[독성 피해] {collider.name}에게 {toxicDamage}의 피해를 입혔습니다.");
                    enemyUnit.TakeDamage(toxicDamage);
                }
            }
        }
    }

    private void DropInflammation()
    {
        Debug.Log($"[염증 드랍] 염증 수치가 {deathDropInflammation} 만큼 변동합니다.");
    }

    // 에디터 뷰에서 독성 폭발 범위를 시각적으로 확인하기 위함
    protected void OnDrawGizmos()
    {
        Gizmos.color = Color.purple; // 독성은 보라색으로 표현
        Gizmos.DrawWireSphere(transform.position, radius * toxicRangeMultiplier);
    }
}