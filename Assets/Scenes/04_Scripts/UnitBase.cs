using UnityEngine;

public class UnitBase : MonoBehaviour
{
    public float maxHealth; //최대체력
    protected float currentHealth; //현재체력
    public float moveSpeed; //이동속도
    public float detectRange; // 기획서의 UR(세포반지름) 단위, 인지범위
    //public float attackRange; //공격범위, 세포의 크기 //당장필요없음,19번줄 radius랑 용도가 같음
    public float damage; //공격력
    public float attackRate; // 공격 간격
    protected float attackCooldown; // 다음 공격까지 남은 시간

    public int produceCost = 10;        // 생산 비용 (예: 10 골드)
    public float produceCooldown = 3.0f; // 생산 쿨타임 (예: 3초 대기)

    [Header("Size Settings")]
    [Tooltip("유닛의 물리적 반지름 크기입니다. 외곽선 정지 계산에 사용됩니다.")]
    public float radius = 10; // ★ 추가된 변수

    protected virtual void Start()
    {
        currentHealth = maxHealth; //시작하면 최대체력으로 현재체력 설정
    }
    protected virtual void Update()
    {
        // 매 프레임마다 공격 시계(Cooldown) 감소
        if (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
        }
    }

    public virtual void TakeDamage(float damage)
    {
        currentHealth -= damage;  //데미지 입으면 현재체력 감소
        if (currentHealth <= 0)  //현재체력이 0이하가 되면
        {
            Die(); //사멸함수 발동
        }
    }

    protected virtual void Die()
    {
        Destroy(gameObject);  // 체력 0 이하 시 사멸
    }
}