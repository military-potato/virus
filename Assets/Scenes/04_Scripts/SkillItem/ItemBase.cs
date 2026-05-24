using UnityEngine;

public abstract class ItemBase : SkillItemBase
{
    [Header("[ Item Settings ]")]
    public int costPain = 30; // 기본 고통수치 소모량 (미정 상태이므로 기획서 대비 기본값 세팅) [cite: 1091, 1094, 1098]
    public int reductionTolerance = 0; // 내성수치 감소량 

    protected override void Execute()
    {
        // StatusController의 고통수치 검사 및 차감
        if (StatusController.Instance.TrySpendPain(costPain))
        {
            // 내성 수치 감소 수치가 있다면 차감 
            if (reductionTolerance > 0)
            {
                StatusController.Instance.SubTolerance(reductionTolerance);
            }

            ActivateItemEffect();
            StartCooldown();
        }
    }

    // 실제 아이템 고유의 효과 구현부
    protected abstract void ActivateItemEffect();
}