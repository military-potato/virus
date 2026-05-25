using UnityEngine;
using System.Collections;

public class ItemOrangeJuice : ItemBase
{
    private void Start()
    {
        nameString = "오렌지 착즙 주스";
        description = "비타민 작용으로 아군 세포들의 공격속도를 50% 증가시킨다.";
        reductionTolerance = 0;
    }

    protected override void ActivateItemEffect()
    {
        StartCoroutine(OrangeJuiceRoutine());
    }

    private IEnumerator OrangeJuiceRoutine()
    {
        Debug.Log("[아이템] 오렌지 주스 발동!");

        AllyUnit[] allies = Object.FindObjectsByType<AllyUnit>(FindObjectsSortMode.None);
        foreach (AllyUnit ally in allies)
        {
            ally.attackRate *= 0.5f;
        }

        yield return new WaitForSeconds(duration);

        Debug.Log("[아이템] 오렌지 주스 종료!");

        AllyUnit[] alliesEnd = Object.FindObjectsByType<AllyUnit>(FindObjectsSortMode.None);
        foreach (AllyUnit ally in alliesEnd)
        {
            ally.attackRate /= 0.5f;
        }
    }
}