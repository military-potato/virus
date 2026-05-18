using UnityEngine;
using System.Collections;

public class CellTestFX : MonoBehaviour
{
    [Header("[ Test Mode Setup ]")]
    [Tooltip("좌측 세포(공격자)를 연결하세요.")]
    [SerializeField] private Transform leftCell;

    [Tooltip("우측 세포(피격자)를 연결하세요.")]
    [SerializeField] private Transform rightCell;

    private SpriteRenderer rightSprite;
    private Color originalRightColor;      // 우측 세포의 진짜 원래 색상
    private Vector3 originalLeftPos;
    private Vector3 originalRightPos;

    private float dashDuration = 0.12f;
    private float returnDuration = 0.18f;
    private float hitDuration = 0.22f;

    // 돌진과 피격 코루틴을 독립적으로 제어하기 위한 변수
    private Coroutine dashCoroutine;
    private Coroutine hitCoroutine;

    private void Start()
    {
        if (leftCell != null) originalLeftPos = leftCell.localPosition;
        if (rightCell != null)
        {
            originalRightPos = rightCell.localPosition;
            rightSprite = rightCell.GetComponent<SpriteRenderer>();

            if (rightSprite != null)
            {
                originalRightColor = rightSprite.color;
            }
        }
    }

    private void Update()
    {
        // 숫자 6번 키를 누를 때
        if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6))
        {
            if (leftCell != null && rightCell != null)
            {
                // 1. 돌진(박치기) 중이었다면 기존 돌진만 취소하고 새로 박치기 시작
                if (dashCoroutine != null)
                {
                    StopCoroutine(dashCoroutine);
                }
                dashCoroutine = StartCoroutine(BodySlamRoutine());
            }
        }
    }

    private IEnumerator BodySlamRoutine()
    {
        // 돌진 시작할 때 공격자 위치만 초기화 (피격자는 흔들리는 중일 수 있으므로 건드리지 않음)
        leftCell.localPosition = originalLeftPos;

        float elapsed = 0f;
        Vector3 dir = (rightCell.position - leftCell.position).normalized;
        Vector3 targetDashPos = originalLeftPos + (dir * 1.2f);

        // 1단계: 몸통 박치기 전진
        while (elapsed < dashDuration)
        {
            elapsed += Time.deltaTime;
            leftCell.localPosition = Vector3.Lerp(originalLeftPos, targetDashPos, elapsed / dashDuration);
            yield return null;
        }
        leftCell.localPosition = targetDashPos;

        // [충돌 시점] 피격 연출이 이미 돌고 있다면 취소하고 새 충격 타이머 시작 (연타 대응)
        if (hitCoroutine != null)
        {
            StopCoroutine(hitCoroutine);
        }
        hitCoroutine = StartCoroutine(HitRoutine());

        // 2단계: 원래 자리로 복귀
        elapsed = 0f;
        while (elapsed < returnDuration)
        {
            elapsed += Time.deltaTime;
            leftCell.localPosition = Vector3.Lerp(targetDashPos, originalLeftPos, elapsed / returnDuration);
            yield return null;
        }
        leftCell.localPosition = originalLeftPos;
        dashCoroutine = null;
    }

    private IEnumerator HitRoutine()
    {
        if (rightSprite == null) yield break;

        float elapsed = 0f;

        while (elapsed < hitDuration)
        {
            elapsed += Time.deltaTime;

            // 격렬한 피격 사인파 흔들림
            float shake = Mathf.Sin(elapsed * 65f) * 0.15f;
            rightCell.localPosition = originalRightPos + new Vector3(shake, 0f, 0f);

            // 연타하는 도중에는 계속 빨간색 계열 조건이 걸리도록 원본 색상 대조 유지
            rightSprite.color = (Mathf.FloorToInt(elapsed * 25f) % 2 == 0) ? Color.red : originalRightColor;

            yield return null;
        }

        // 연타가 완전히 끝나서 지속 시간이 종료되면 원래 색상과 위치로 깔끔하게 복구
        rightCell.localPosition = originalRightPos;
        rightSprite.color = originalRightColor;
        hitCoroutine = null;
    }
}