using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UnitSelect : MonoBehaviour
{
    [Header("[ Unit Info ]")]
    public GameObject unitPrefab; // 이 버튼이 담당할 아군 유닛 프리팹 (프로젝트 창)
 
    [Header("[ Click Animation Settings ]")]
    [SerializeField] private float shrinkScale = 0.85f;  // 클릭했을 때 작아질 크기 비율 (기본 85%)
    [SerializeField] private float shrinkDuration = 0.1f; // 작아지는 데 걸리는 시간(초)
    [SerializeField] private float returnDuration = 0.1f; // 원래대로 돌아오는 데 걸리는 시간(초)
    private Vector3 originalScale;                        // 버튼의 원래 크기 기억용 변수
    private Coroutine activeScaleCoroutine;               // 중복 클릭 시 충돌을 방지하기 위한 변수

    private Button button;

    void Start()
    {
        // 나중에 크기가 꼬이지 않도록 게임 시작 시 버튼의 순수 크기를 저장
        originalScale = transform.localScale;

        button = GetComponent<Button>();
        if (button != null)
        {
            // 버튼 클릭 시 유닛을 장전하는 함수를 자동으로 연결
            button.onClick.AddListener(SelectThisUnit);
        }
    }

    public void SelectThisUnit()
    {
        if (unitPrefab == null)
        {
            Debug.LogWarning($"{gameObject.name} 버튼에 유닛 프리팹이 연결되지 않았습니다!");
            return;
        }

        // 원본 코드 작동 직전에 크기 찌그러짐 효과 발동
        if (activeScaleCoroutine != null) StopCoroutine(activeScaleCoroutine);
        activeScaleCoroutine = StartCoroutine(ClickScaleRoutine());

        // AllySpawner에게 "이 유닛을 소환할 준비를 해라" 하고 넘김
        allySpawn.Instance.SpawnUnitImmediate(unitPrefab);
    }

    private IEnumerator ClickScaleRoutine()
    {
        Vector3 targetShrinkScale = originalScale * shrinkScale;
        float elapsedTime = 0f;

        // 1. 순식간에 지정한 크기(shrinkScale)로 작아지기
        while (elapsedTime < shrinkDuration)
        {
            elapsedTime += Time.deltaTime;
            transform.localScale = Vector3.Lerp(originalScale, targetShrinkScale, elapsedTime / shrinkDuration);
            yield return null;
        }
        transform.localScale = targetShrinkScale; // 확실하게 목적지 크기 고정

        elapsedTime = 0f;

        // 2. 다시 부드럽게 원래 크기(originalScale)로 되돌아가기
        while (elapsedTime < returnDuration)
        {
            elapsedTime += Time.deltaTime;
            transform.localScale = Vector3.Lerp(targetShrinkScale, originalScale, elapsedTime / returnDuration);
            yield return null;
        }
        transform.localScale = originalScale; // 원래 크기로 안전하게 마무리

        activeScaleCoroutine = null;
    }
}