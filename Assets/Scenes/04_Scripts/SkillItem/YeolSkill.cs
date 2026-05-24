using UnityEngine;
using System.Collections;

public class YeolSkill : ActiveSkillBase
{
    [Header("[ 열 스킬 특화 설정 ]")]
    [SerializeField] private float duration = 15f;

    // 현재 레벨에 따른 버프 비율 계산 (1레벨: 5%, 2레벨: 10%, 3레벨: 15%) [cite: 74]
    private float GetBuffPercentage() => currentLevel * 0.05f;

    protected override void ActivateSkillEffect()
    {
        StartCoroutine(FeverRoutine());
    }

    private IEnumerator FeverRoutine()
    {
        float buffRatio = GetBuffPercentage();
        Debug.Log($"[열] 스킬 발동 (소모 염증: {costInflammationByLevel[currentLevel - 1]})");
        Debug.Log($"[열 Lv.{currentLevel} 효과] 아군 공격력 {buffRatio * 100}% 증가, 기지 피해 {buffRatio * 100}% 감소");

        // TODO: 스탯/기지 매니저 연동
        // BaseStation.Instance.SetDamageReduction(buffRatio);
        // UnitManager.Instance.ApplyAttackDamageBuff(buffRatio);

        yield return new WaitForSeconds(duration);

        Debug.Log($"[열 Lv.{currentLevel}] 종료. 버프가 해제됩니다.");
        // BaseStation.Instance.SetDamageReduction(0f);
        // UnitManager.Instance.RemoveAttackDamageBuff(buffRatio);
    }

    protected override void OnLevelUp()
    {
        // 레벨업 시 수치 변화 확인용 로그
        Debug.Log($"[열 레벨업 완료] 다음 스킬 사용 시 공격력/방어력 버프가 {GetBuffPercentage() * 100}%로 강화됩니다.");
    }
}