using UnityEngine;

public class KillerTSkill : AllyUnit
{
    // 킬러 T 세포는 부모(AllyUnit -> UnitBase)의 스탯과 전투 메커니즘을 그대로 상속받습니다.
    // 인스펙터 창에서 Max Health, Move Speed, Damage 등을 자유롭게 조절하시면 됩니다.

    protected override void Start()
    {
        // 부모 클래스의 스폰 위치 기록 및 기본 초기화 실행
        base.Start();
    }




    // ★ 핵심 기획: 특정 적(change_sepo)을 공격하지 않도록 탐색 함수를 오버라이드(재정의)합니다.
    protected override void FindClosestEnemy()
    {
        // "Enemy" 태그를 가진 모든 적 오브젝트 탐색
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            if (enemy == null) continue;

            // 🔥 [기획 반영] 적의 이름에 "change_sepo"가 포함되어 있다면 타겟팅 후보에서 완전히 제외(패스)합니다.
            if (enemy.name.Contains("change_sepo"))
            {
                continue;
            }

            float distance = Vector2.Distance(transform.position, enemy.transform.position);

            // 인식 범위(detectRange) 내에 있는 가장 가까운 적 선별
            if (distance < shortestDistance && distance <= detectRange)
            {
                shortestDistance = distance;
                nearestEnemy = enemy;
            }
        }

        // change_sepo를 제외한 적이 있다면 추적 상태로, 없다면 대기 상태로 전환
        if (nearestEnemy != null)
        {
            currentTarget = nearestEnemy.transform;
            currentState = State.Chasing;
        }
        else
        {
            currentTarget = null;
            currentState = State.Idle;
        }
    }
}