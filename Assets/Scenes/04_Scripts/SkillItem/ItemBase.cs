using UnityEngine;

public abstract class ItemBase : SkillItemBase
{
    [Header("[ Item Settings ]")]
    public int costPain = 30;           // 사용할 때 소모되는 고통 수치
    public int reductionTolerance = 10; // 사용할 때 감소시켜줄 내성 수치

    [Header("[ Duration Settings ]")]
    public float duration = 30f;        // 기획서: 모든 아이템 30초 지속

    protected override void Execute()
    {
        // 함수가 시작되자마자 무조건 찍혀야 하는 로그
        Debug.Log($"<color=yellow>★ [ItemBase] Execute() 진입 성공! 아이템 이름: {gameObject.name}</color>");

        // 안전장치
        if (StatusController.Instance == null)
        {
            Debug.LogError("ItemBase: 씬에 StatusController 인스턴스가 존재하지 않습니다!");
            return;
        }

        Debug.Log($"[아이템 사용 시도] 차감 전 고통 수치: {StatusController.Instance.PainValue}, 필요 수치: {costPain}");

        // StatusController의 고통수치 검사 및 차감
        if (StatusController.Instance.TrySpendPain(costPain))
        {
            Debug.Log($"[아이템 사용 성공] 고통 수치 차감 완료! 남은 고통 수치: {StatusController.Instance.PainValue}");

            if (reductionTolerance > 0)
            {
                StatusController.Instance.SubTolerance(reductionTolerance);
            }

            ActivateItemEffect();
            StartCooldown();
        }
        else
        {
            Debug.LogWarning("[아이템 사용 실패] 고통 수치가 부족합니다.");
        }
    }

    protected abstract void ActivateItemEffect();
}