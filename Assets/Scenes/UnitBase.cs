using UnityEngine;

public class UnitBase : MonoBehaviour
{
    public float maxHealth; //최대체력
    protected float currentHealth; //현재체력
    public float moveSpeed; //이동속도
    public float detectRange; // 기획서의 UR(세포반지름) 단위, 사정거리

    protected virtual void Start()
    {
        currentHealth = maxHealth; //시작하면 최대체력으로 현재체력 설정
    }

    public virtual void TakeDamage(float damage)
    {
        currentHealth -= damage; //데미지 입으면 현재체력 감소
        if (currentHealth <= 0) //현재체력이 0이하가 되면
        {
            Die(); //사멸함수 발동
        }
    }

    protected virtual void Die()
    {
        Destroy(gameObject); // 체력 0 이하 시 사멸
    }
}