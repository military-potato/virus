using UnityEngine;
using System.Collections;

public class ItemHangsengje : ItemBase
{
    [Header("[ 항생제 DoT 설정 ]")]
    [Tooltip("초당 적 최대체력의 몇 %만큼 깎을 것인가? (0.05f면 초당 5% 피해)")]
    [SerializeField] private float percentDamagePerSecond = 0.05f;
    [SerializeField] private float damageInterval = 1.0f; // DoT 주기 (1초 마다)

    private void Start()
    {
        nameString = "항생제";
        description = "필드에 나와있는 모든 병원체(적)에게 지속적인 피해를 준다. 지속시간 내 새로 나온 적도 포함.";

        // 기획서 반영용 기본 세팅 (인스펙터창에서 변경 가능)
        duration = 30f;
        cooldownTime = 50f;
    }

    protected override void ActivateItemEffect()
    {
        StartCoroutine(AntibioticsDamageRoutine());
    }

    // 💡여기에 코드를 추가했습니다! 버튼 이벤트 연결용 public 함수
    /*public void OnButtonClick()
    {
        // 버튼을 누르면 이 함수가 호출되고, 내부에서 실제 아이템 효과를 실행합니다.
        //ActivateItemEffect();
        TryUse();
    }*/

    private IEnumerator AntibioticsDamageRoutine()
    {
        Debug.Log("<color=green><b>[아이템] 항생제 작동 시작!</b></color> 모든 병원체 최대 체력 비례 DoT 피해 가동.");
        float elapsed = 0f;

        // 30초 지속시간에 도달할 때까지 루프 작동
        while (elapsed < duration)
        {
            // 💡 [신버전 에러 해결] FindObjectsOfType 대신 FindObjectsByType 사용
            // 틱이 돌 때마다 새로 감지하므로 중간에 새로 스폰된 적들도 실시간 적용됩니다!
            EnemyUnit[] currentEnemies = Object.FindObjectsByType<EnemyUnit>(FindObjectsSortMode.None);

            foreach (EnemyUnit enemy in currentEnemies)
            {
                if (enemy != null)
                {
                    // 적의 maxHealth(최대체력) 기준 % 피해량 연산
                    float calculatedDamage = enemy.maxHealth * percentDamagePerSecond;

                    // 유닛베이스에 장착된 데미지 연산 기능 호출
                    enemy.TakeDamage(calculatedDamage);
                }
            }

            // 1초 쉬고 시간 누적
            yield return new WaitForSeconds(damageInterval);
            elapsed += damageInterval;
        }

        Debug.Log("<color=green><b>[아이템] 항생제 효과 만료.</b></color> 병원체 지속 피해가 종료됩니다.");
    }
}