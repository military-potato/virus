using UnityEngine;
using System.Collections;

public class CellActionFX : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color originalColor;           // 연타 버그 방지용 원본 색상 저장 장부
    private Vector3 originalLocalPos;      // 돌진 전 원본 로컬 위치

    [Header("[ FX Animation Settings ]")]
    [SerializeField] private float dashDuration = 0.12f;  // 돌진하는 시간 (속도)
    [SerializeField] private float returnDuration = 0.18f; // 복귀하는 시간
    [SerializeField] private float hitDuration = 0.22f;    // 피격 흔들림 지속 시간

    private Coroutine dashCoroutine;
    private Coroutine hitCoroutine;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 게임 시작 시점의 순수한 원본 색상을 안전하게 보관합니다.
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    /// <summary>
    /// 외부 AI 스크립트에서 공격 성공 시 호출하는 함수 (몸통 박치기 시작)
    /// </summary>
    public void PlayBodySlam(Transform target)
    {
        if (target == null) return;

        // 이전 돌진 연출이 끝나기 전에 다음 공격 주기가 오면, 기존 돌진을 취소하고 새로 전진
        if (dashCoroutine != null)
        {
            StopCoroutine(dashCoroutine);
        }
        dashCoroutine = StartCoroutine(BodySlamRoutine(target));
    }

    /// <summary>
    /// 내가 충돌 당했을 때 스스로 발동하는 피격 연출 함수
    /// </summary>
    public void PlayHitEffect()
    {
        // 연타로 얻어맞는 중이면 기존 피격 연출을 취소하고 타이머를 새로 갱신 (붉은 상태 유지)
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

        // 상대방 방향 벡터 계산 후 살짝 앞 좌표를 타겟 포인트로 지정
        Vector3 dir = (target.position - transform.position).normalized;
        Vector3 targetDashPos = originalLocalPos + (dir * 1.2f);

        // 1단계: 몸통 박치기 전진 (Lerp)
        while (elapsed < dashDuration)
        {
            elapsed += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(originalLocalPos, targetDashPos, elapsed / dashDuration);
            yield return null;
        }
        transform.localPosition = targetDashPos;

        // [충돌 시점] 실시간으로 타겟의 몸뚱이에 붙어있는 CellActionFX 컴포넌트를 찾아 피격 효과를 트리거
        CellActionFX targetFX = target.GetComponent<CellActionFX>();
        if (targetFX != null)
        {
            targetFX.PlayHitEffect();
        }

        // 2단계: 제자리로 복귀 (Lerp)
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

            // 격렬한 피격 사인파 흔들림 연산
            float shake = Mathf.Sin(elapsed * 65f) * 0.15f;
            transform.localPosition = hitOriginPos + new Vector3(shake, 0f, 0f);

            // 최초 등록된 originalColor 장부를 대조하여 연타 시 색상이 고정되는 현상 원천 차단
            spriteRenderer.color = (Mathf.FloorToInt(elapsed * 25f) % 2 == 0) ? Color.red : originalColor;

            yield return null;
        }

        // 연출 종료 시 완벽하게 복구
        transform.localPosition = hitOriginPos;
        spriteRenderer.color = originalColor;
        hitCoroutine = null;
    }
}