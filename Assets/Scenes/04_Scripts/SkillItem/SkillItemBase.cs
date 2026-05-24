using UnityEngine;

public abstract class SkillItemBase : MonoBehaviour
{
    [Header("[ Base Info ]")]
    public string nameString;
    [TextArea] public string description;

    [Header("[ Cooldown ]")]
    public float cooldownTime = 50f;
    protected float currentCooldown = 0f;
    protected bool isCooldown = false;

    protected virtual void Update()
    {
        if (isCooldown)
        {
            currentCooldown -= Time.deltaTime;
            if (currentCooldown <= 0)
            {
                currentCooldown = 0f;
                isCooldown = false;
            }
        }
    }

    // 외부(UI 버튼 등)에서 스킬/아이템을 발동할 때 호출하는 함수
    public void TryUse()
    {
        if (isCooldown)
        {
            Debug.Log($"{nameString}은(는) 아직 쿨타임입니다!");
            return;
        }

        Execute();
    }

    // 각 하위 스킬/아이템에서 실제로 구현할 실행 메커니즘
    protected abstract void Execute();

    // 쿨타임 시작 실행
    protected void StartCooldown()
    {
        currentCooldown = cooldownTime;
        isCooldown = true;
    }
}