using UnityEngine;
using UnityEngine.InputSystem; // New Input System 사용 시 필수

public class allySpawn : MonoBehaviour
{
    public static allySpawn Instance { get; private set; }

    // 소환 방식을 에디터에서 편하게 고를 수 있도록 열거형(Enum)을 선언합니다.
    public enum SpawnMechanism
    {
        NearBase,    // 기지 주변에 무작위 소환 (기존 TestSpawner 방식)
        MouseClick   // 마우스로 클릭한 정확한 위치에 소환
    }

    [Header("[ Spawn Mechanism Settings ]")]
    [Tooltip("원하는 소환 메커니즘을 선택하세요.")]
    public SpawnMechanism spawnMechanism = SpawnMechanism.NearBase;

    [Header("[ Base Settings ]")]
    public Transform giziTransform; // 기지 위치

    [Header("[ Selected Unit Info ]")]
    public GameObject selectedPrefab = null; // 현재 UI에서 선택된 프리팹
    public float spawnTimer = 0f; // 현재 선택된 유닛의 쿨타임 타이머

    void Awake()
    {
        // 싱글톤 초기화
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // 쿨타임 계산
        if (spawnTimer > 0) spawnTimer -= Time.deltaTime;

        // 마우스 왼쪽 클릭 감지 + 선택된 유닛이 있을 때만 실행
        if (Mouse.current.leftButton.wasPressedThisFrame && selectedPrefab != null)
        {
            TrySpawnAlly();
        }
    }

    // UI 버튼에 연결할 함수 (UnitSelector 등에서 호출)
    public void SetSelectedUnit(GameObject unitPrefab)
    {
        selectedPrefab = unitPrefab;
        Debug.Log($"{unitPrefab.name} 유닛이 장전되었습니다. 맵을 클릭해 소환하세요.");
    }

    private void TrySpawnAlly()
    {
        if (selectedPrefab == null) return;

        UnitBase unitScript = selectedPrefab.GetComponent<UnitBase>();
        if (unitScript == null)
        {
            Debug.LogError($"{selectedPrefab.name} 프리팹에 UnitBase 스크립트가 없습니다!");
            return;
        }

        // 1. 쿨타임 체크
        if (spawnTimer > 0)
        {
            Debug.Log($"{selectedPrefab.name} 소환 쿨타임 중... 남은 시간: {spawnTimer:F1}초");
            return;
        }

        // 2. 염증 수치 체크 및 소비 함수 호출
        int cost = unitScript.produceCost;
    
        if (StatusController.Instance.TrySpendInflammation(cost))
        {
            // 코스트(염증) 차감에 성공하면 설정된 메커니즘에 따라 소환 진행
            ExecuteSpawn();
            
            // 쿨타임 적용
            spawnTimer = unitScript.produceCooldown;
        }
    }

    /// <summary>
    /// 설정된 인스펙터 값에 따라 분기 처리하여 소환하는 함수
    /// </summary>
    private void ExecuteSpawn()
    {
        Vector3 finalSpawnPosition = Vector3.zero;

        if (spawnMechanism == SpawnMechanism.NearBase)
        {
            if (giziTransform == null)
            {
                Debug.LogError("NearBase 모드이지만 기지(Gizi) 트랜스폼이 없습니다! 원점에 소환합니다.");
                finalSpawnPosition = Vector3.zero;
            }
            else
            {
                // [매커니즘 A] 기지 주변 반경 2단위 내 무작위 스폰 (기존 TestSpawner의 기능 이식)
                Vector2 randomOffset = Random.insideUnitCircle * 30f;
                finalSpawnPosition = giziTransform.position + new Vector3(randomOffset.x, randomOffset.y, 0);
            }
        }
        else if (spawnMechanism == SpawnMechanism.MouseClick)
        {
            // [매커니즘 B] 마우스 스크린 좌표를 월드 게임 좌표로 치환하여 클릭한 위치에 스폰
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mouseWorldPos.z = 0; // 2D 게임이므로 Z축은 0으로 고정
            finalSpawnPosition = mouseWorldPos;
        }

        // 최종 결정된 위치에 프리팹 생성
        Instantiate(selectedPrefab, finalSpawnPosition, Quaternion.identity);
        Debug.Log($"{selectedPrefab.name} 소환 성공! 위치: {finalSpawnPosition}");
    }
}
