using UnityEngine;

public class allySpawn : MonoBehaviour
{
    public static allySpawn Instance { get; private set; }

    [Header("[ Base Settings ]")]
    public Transform giziTransform; // 기지 위치

    [Header("[ Selected Unit Info ]")]
    public float spawnTimer = 0f; // 소환 쿨타임 타이머

    void Awake()
    {
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
        // 쿨타임은 매 프레임 계속 깎음
        if (spawnTimer > 0) spawnTimer -= Time.deltaTime;
    }

    // UI 버튼에 연결할 함수 
    // 버튼을 누르면 쿨타임 없이 즉시 소환을 시도함.
 
    public void SpawnUnitImmediate(GameObject unitPrefab)
    {
        if (unitPrefab == null) return;

        UnitBase unitScript = unitPrefab.GetComponent<UnitBase>();
        if (unitScript == null)
        {
            Debug.LogError($"{unitPrefab.name} 프리팹에 UnitBase 스크립트가 없습니다!");
            return;
        }

        // 1. 쿨타임 체크
        if (spawnTimer > 0)
        {
            Debug.Log($"{unitPrefab.name} 소환 쿨타임 중... 남은 시간: {spawnTimer:F1}초");
            return;
        }

        // 2. 코스트(염증 수치) 체크
        int cost = unitScript.produceCost;
    
        if (StatusController.Instance.TrySpendInflammation(cost))
        {
            // 코스트 차감 성공 시 바로 위치 계산 후 소환
            ExecuteImmediateSpawn(unitPrefab);
            
            // 프리팹에 설정된 쿨타임 적용
            spawnTimer = unitScript.produceCooldown;
        }
    }

 
    // 기지 주변 무작위 위치를 계산하여 즉시 프리팹을 생성하는 함수
  
    private void ExecuteImmediateSpawn(GameObject unitPrefab)
    {
        Vector3 finalSpawnPosition = Vector3.zero;

        if (giziTransform == null)
        {
            Debug.LogError("기지(Gizi) 트랜스폼이 지정되지 않았습니다! 원점(0,0,0)에 소환합니다.");
            finalSpawnPosition = Vector3.zero;
        }
        else
        {
            // 기지 주변 반경 내 무작위 스폰
            Vector2 randomOffset = Random.insideUnitCircle * 30f; // 반경 수치
            finalSpawnPosition = giziTransform.position + new Vector3(randomOffset.x, randomOffset.y, 0);
        }

        // 즉시 프리팹 생성
        Instantiate(unitPrefab, finalSpawnPosition, Quaternion.identity);
        Debug.Log($"{unitPrefab.name} 즉시 소환 성공! 위치: {finalSpawnPosition}");
    }
}