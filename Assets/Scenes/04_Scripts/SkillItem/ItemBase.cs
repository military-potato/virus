using UnityEngine;

public abstract class ItemBase : SkillItemBase
{
    [Header("[ Item Settings ]")]
    public int costPain = 30;           // 사용할 때 소모되는 고통 수치
    public int reductionTolerance = 10; // 사용할 때 감소시켜줄 내성 수치

    [Header("[ Duration Settings ]")]
    public float duration = 30f;        // 기획서: 모든 아이템 30초 지속

    // 외부(UI 버튼 등)에서 사용 시 실행되는 최상위 로직 (부모의 Execute 오버라이드)
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

            // 각 아이템 고유 효과 실행
            ActivateItemEffect();

            // 쿨타임 가동
            StartCooldown();
        }
    }

    // 실제 아이템 고유의 효과 구현부 (자식들이 채워넣음)
    protected abstract void ActivateItemEffect();
}