using UnityEngine;
using System.Collections;

public class SaitoKainTonadoSkill : ActiveSkillBase
{
    [Header("[ 사이토카인 폭풍 특화 설정 ]")]
    [SerializeField] private float damageInterval = 1.0f; // 피해 주기 (1초마다)

    // 레벨별 DoT 데미지 밸런스 설정 (인스펙터창에서 수정 가능)
    [SerializeField] private int[] enemyDamageByLevel = new int[3] { 15, 25, 40 };
    [SerializeField] private int[] baseDamageByLevel = new int[3] { 8, 12, 18 };
    [SerializeField] private int[] allyDamageByLevel = new int[3] { 4, 6, 9 };

    private bool isStormActive = false;
    private Coroutine stormCoroutine;

    // 💡 부모 클래스(ActiveSkillBase)에 Awake가 없으므로 
    // override나 base.Awake() 없이 단독으로 작성하여 초기화합니다!
    private void Awake()
    {
        skillName = "사이토카인 폭풍";
        cooldownTime = 0f; // On/Off 토글형이므로 쿨타임은 사용하지 않음
    }

    // 💡 부모 클래스가 요구하는 필수 추상 함수(ActivateSkillEffect)를 완벽하게 구현합니다.
    // 외부 UI 버튼에서 'TryUseSkill()'을 호출하면 자원이 깎인 뒤 이 함수가 실행됩니다!
    protected override void ActivateSkillEffect()
    {
        // 토글 ON / OFF 스위칭
        isStormActive = !isStormActive;

        if (isStormActive)
        {
            Debug.Log($"<color=red><b>[{skillName} Lv.{currentLevel} 활성화]</b></color> 소모 염증: {costInflammationByLevel[currentLevel - 1]}");
            if (stormCoroutine != null) StopCoroutine(stormCoroutine);
            stormCoroutine = StartCoroutine(CytokineStormTick());
        }
        else
        {
            Debug.Log($"<color=blue><b>[{skillName} 비활성화]</b></color> 폭풍 상태를 종료합니다.");
            if (stormCoroutine != null)
            {
                StopCoroutine(stormCoroutine);
                stormCoroutine = null;
            }
        }
    }

    private IEnumerator CytokineStormTick()
    {
        while (isStormActive)
        {
            // 현재 스킬 레벨(1~3)에 맞는 단계별 데미지 적용
            int currentEnemyDamage = enemyDamageByLevel[currentLevel - 1];
            int currentBaseDamage = baseDamageByLevel[currentLevel - 1];
            int currentAllyDamage = allyDamageByLevel[currentLevel - 1];

            Debug.Log($"[폭풍 DoT 틱 작동 중] Lv.{currentLevel} -> 병원체: {currentEnemyDamage} | 거점: {currentBaseDamage} | 아군: {currentAllyDamage}");

            // TODO: 실제 다른 매니저나 시스템 스크립트 연결부
            // BaseStation.Instance.TakeDamage(currentBaseDamage);

            yield return new WaitForSeconds(damageInterval);
        }
    }

    // 💡 부모의 TryUpgradeSkill을 통해 레벨업에 성공했을 때 실행되는 보너스 함수입니다.
    protected override void OnLevelUp()
    {
        Debug.Log($"<color=green><b>[{skillName} 레벨업 완료]</b></color> 현재 Lv.{currentLevel}.");

        // 폭풍이 켜진 상태에서 레벨업을 한 경우, 실시간으로 데미지를 변경하기 위해 틱 코루틴을 재시작합니다.
        if (isStormActive)
        {
            if (stormCoroutine != null) StopCoroutine(stormCoroutine);
            stormCoroutine = StartCoroutine(CytokineStormTick());
        }
    }
}