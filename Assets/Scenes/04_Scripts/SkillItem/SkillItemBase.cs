using UnityEngine;
using System.Collections; // 💡 코루틴 사용을 위해 필수 추가

public abstract class SkillItemBase : MonoBehaviour
{
    [Header("[ Base Info ]")]
    public string nameString;
    [TextArea] public string description;

    // --- [ 원본 유지 외 추가된 연출 세팅 ] ---
    [Header("[ Click Animation Settings ]")]
    [SerializeField] private float shrinkScale = 0.85f;  // 클릭했을 때 작아질 크기 비율 (기본 85%)
    [SerializeField] private float shrinkDuration = 0.1f; // 작아지는 데 걸리는 시간(초)
    [SerializeField] private float returnDuration = 0.1f; // 원래대로 돌아오는 데 걸리는 시간(초)
    private Vector3 originalScale;                        // 버튼의 원래 크기 기억용 변수
    private Coroutine activeScaleCoroutine;               // 중복 클릭 시 연출 꼬임 방지용
    // ------------------------------------------

    [Header("[ Cooldown ]")]
    public float cooldownTime = 50f;
    protected float currentCooldown = 0f;
    protected bool isCooldown = false;

    // 💡 [연출용 추가] 최상위 부모이므로 Awake를 정의하여 원래 크기를 기억하게 합니다.
    protected virtual void Awake()
    {
        originalScale = transform.localScale;
    }

    protected virtual void Update()
    {
        if (isCooldown)
        {
            currentCooldown -= Time.deltaTime;
            if (currentCooldown <= 0)
            {
                currentCooldown = 0f;
                isCooldown = false;
            }
        }
    }

    // 외부(UI 버튼 등)에서 스킬/아이템을 발동할 때 호출하는 함수
    public void TryUse()
    {
        if (isCooldown)
        {
            Debug.Log($"{nameString}은(는) 아직 쿨타임입니다!");
            return;
        }

        // 💡 [추가된 연출 로직] 쿨타임이 아닐 때만 시각 애니메이션을 발동시킵니다.
        if (activeScaleCoroutine != null) StopCoroutine(activeScaleCoroutine);
        activeScaleCoroutine = StartCoroutine(ClickScaleRoutine());

        Execute();
    }

    // 각 하위 스킬/아이템에서 실제로 구현할 실행 메커니즘
    protected abstract void Execute();

    // 쿨타임 시작 실행
    protected void StartCooldown()
    {
        currentCooldown = cooldownTime;
        isCooldown = true;
    }

    // 💡 [추가된 핵심 기능] 버튼이 부드럽게 줄어들었다가 원래대로 돌아오는 시각 연출 코루틴
    private IEnumerator ClickScaleRoutine()
    {
        Vector3 targetShrinkScale = originalScale * shrinkScale;
        float elapsedTime = 0f;

        // 1. 지정한 크기(targetShrinkScale)로 부드럽게 작아지기
        while (elapsedTime < shrinkDuration)
        {
            elapsedTime += Time.deltaTime;
            transform.localScale = Vector3.Lerp(originalScale, targetShrinkScale, elapsedTime / shrinkDuration);
            yield return null;
        }
        transform.localScale = targetShrinkScale; // 크기 고정

        elapsedTime = 0f;

        // 2. 다시 부드럽게 원래 크기(originalScale)로 되돌아가기
        while (elapsedTime < returnDuration)
        {
            elapsedTime += Time.deltaTime;
            transform.localScale = Vector3.Lerp(targetShrinkScale, originalScale, elapsedTime / returnDuration);
            yield return null;
        }
        transform.localScale = originalScale; // 원래 크기로 마무리

        activeScaleCoroutine = null;
    }
}