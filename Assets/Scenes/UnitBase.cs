using UnityEngine;

public class UnitBase : MonoBehaviour
{
    public float maxHealth; 
    protected float currentHealth; 
    public float moveSpeed; 
    public float detectRange; 

    [Header("Size Settings")]
    [Tooltip("유닛의 물리적 반지름 크기입니다. 외곽선 정지 계산에 사용됩니다.")]
    public float radius = 10; // ★ 추가된 변수

    protected virtual void Start()
    {
        currentHealth = maxHealth; 
    }

    public virtual void TakeDamage(float damage)
    {
        currentHealth -= damage; 
        if (currentHealth <= 0) 
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Destroy(gameObject); 
    }
}