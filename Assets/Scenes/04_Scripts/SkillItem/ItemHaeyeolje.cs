using UnityEngine;
using System.Collections;

public class ItemHaeyeolje : ItemBase
{
    // 다른 스킬 스크립트들이 실시간으로 "지금 해열제 감면 효과 중인가?" 확인하게 해주는 스위치
    public static bool IsAntipyreticActive { get; private set; } = false;

    private void Start()
    {
        nameString = "해열제";
        description = "잠시동안 염증수치의 재화 소모율이 30%로 감소한다.";

        // 기획서 반영용 기본 세팅 (인스펙터창에서 변경 가능)
        duration = 30f;
        cooldownTime = 50f;
    }

    protected override void ActivateItemEffect()
    {
        StartCoroutine(AntipyreticRoutine());
    }

    private IEnumerator AntipyreticRoutine()
    {
        Debug.Log("<color=cyan><b>[아이템] 해열제 투여!</b></color> 30초간 액티브 스킬 염증수치 소모량 30%로 감소.");

        IsAntipyreticActive = true;

        // 지정된 지속시간(30초) 만큼 대기
        yield return new WaitForSeconds(duration);

        IsAntipyreticActive = false;
        Debug.Log("<color=cyan><b>[아이템] 해열제 효과 종료.</b></color> 염증수치 소모율이 정상으로 복구됩니다.");
    }
}