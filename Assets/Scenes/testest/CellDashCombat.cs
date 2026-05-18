using UnityEngine;
using System.Collections;

public class CellActionFX : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color originalColor;           // 세포의 진짜 최초 원본 색상 기록용
    private Vector3 originalLocalPos;

    private float dashDuration = 0.12f;  // 돌진 속도
    private float returnDuration = 0.18f; // 복귀 속도
    private float hitDuration = 0.22f;    // 피격 흔들림 시간

    // 돌진과 피격 연출 타이머를 독립적으로 제어하기 위한 핸들러
    private Coroutine dashCoroutine;
    private Coroutine hitCoroutine;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // [핵심] 게임이 처음 시작할 때 붉어지지 않은 원본 색상을 안전하게 딱 한 번 기록합니다.
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    /// <summary>
    /// 공격자(나)가 피격자(타겟)를 향해 몸통 박치기를 하는 함수
    /// </summary>
    public void PlayBodySlam(Transform target)
    {
        if (target == null) return;

        // 이전 돌진이 끝나기 전에 다시 공격 주기(연타)가 오면 기존 돌진만 끊고 새로 돌진
        if (dashCoroutine != null)
        {
            StopCoroutine(dashCoroutine);
        }
        dashCoroutine = StartCoroutine(BodySlamRoutine(target));
    }

    /// <summary>
    /// 내가 맞았을 때 스스로 흔들리고 붉어지는 함수 (상대방의 충돌 시점에 의해 실시간 호출됨)
    /// </summary>
    public void PlayHitEffect()
    {
        // 연타로 얻어맞는 중이면 기존 피격 대기 타이머를 리셋하여 연출 지속시간을 갱신
        if (hitCoroutine != null)
        {
            StopCoroutine(hitCoroutine);
        }
        hitCoroutine = StartCoroutine(HitRoutine());
    }

    private IEnumerator BodySlamRoutine(Transform target)
    {
        originalLocalPos = transform.localPosition;
        float elapsed = 0f;

        // 타겟 방향으로 약간 앞까지 돌진할 위치 계산
        Vector3 dir = (target.position - transform.position).normalized;
        Vector3 targetDashPos = originalLocalPos + (dir * 1.2f);

        // 1. 박치기 전진
        while (elapsed < dashDuration)
        {
            elapsed += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(originalLocalPos, targetDashPos, elapsed / dashDuration);
            yield return null;
        }
        transform.localPosition = targetDashPos;

        // [충돌하는 순간] 상대방 몸뚱이에 있는 피격 효과를 실시간으로 찾아서 실행 (상대방 연타 타이머 발동)
        CellActionFX targetFX = target.GetComponent<CellActionFX>();
        if (targetFX != null)
        {
            targetFX.PlayHitEffect();
        }

        // 2. 원래 자리로 복귀
        elapsed = 0f;
        while (elapsed < returnDuration)
        {
            elapsed += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(targetDashPos, originalLocalPos, elapsed / returnDuration);
            yield return null;
        }
        transform.localPosition = originalLocalPos;
        dashCoroutine = null;
    }

    private IEnumerator HitRoutine()
    {
        if (spriteRenderer == null) yield break;

        Vector3 hitOriginPos = transform.localPosition;
        float elapsed = 0f;

        while (elapsed < hitDuration)
        {
            elapsed += Time.deltaTime;

            // 격렬한 피격 사인파 흔들림
            float shake = Mathf.Sin(elapsed * 65f) * 0.15f;
            transform.localPosition = hitOriginPos + new Vector3(shake, 0f, 0f);

            // 항상 고정된 최초 원본 색상(originalColor)을 기준으로 번갈아 대조하므로 색상이 굳지 않음
            spriteRenderer.color = (Mathf.FloorToInt(elapsed * 25f) % 2 == 0) ? Color.red : originalColor;

            yield return null;
        }

        // 연타 타격이 완전히 멈추고 시간이 종료되면 원상 복구
        transform.localPosition = hitOriginPos;
        spriteRenderer.color = originalColor;
        hitCoroutine = null;
    }
}