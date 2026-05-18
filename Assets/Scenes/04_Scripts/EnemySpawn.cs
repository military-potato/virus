using UnityEngine;
using System.Collections;
using System.Collections.Generic; // List를 사용하기 위해 추가 필수!!

public class EnemySpawn : MonoBehaviour
{
    // 유니티 인스펙터 창에서 적 종류와 갯수를 묶어서 세팅할 수 있게 해주는 구조체입니다.
    [System.Serializable]
    public struct EnemySpawnData
    {
        public string enemyName;      // 에디터 확인용 이름 (예: 일반좀비, 보스 등)
        public GameObject enemyPrefab; // 적 프리팹
        public int spawnCount;        // 이 적을 몇 마리 소환할 것인가
    }

    [Header("Enemy Variety Settings")]
    // 기존 public GameObject enemy1 대신 리스트를 사용하여 여러 적을 등록합니다.
    public List<EnemySpawnData> enemyWaveList = new List<EnemySpawnData>();

    public float enemySpawnRadius = 40f; // 적이 생성될 외곽 원의 반지름
    
    [Header("Wave Settings")]
    public int currentWave = 1;
    public float spawnInterval = 0.5f; // 적 유닛 간의 소환 간격
    public float waveInterval = 10f;

    void Start()
    {
        StartCoroutine(WaveRoutine());
    }

    private IEnumerator WaveRoutine()
    {
        while (true)
        {
            Debug.Log($"=== 웨이브 {currentWave} 시작! ===");
            
            // 리스트에 등록된 모든 적 종류를 하나씩 순회합니다.
            foreach (EnemySpawnData enemyData in enemyWaveList)
            {
                if (enemyData.enemyPrefab == null) continue;

                string nameToDisplay = string.IsNullOrEmpty(enemyData.enemyName) ? enemyData.enemyPrefab.name : enemyData.enemyName;
                Debug.Log($"{nameToDisplay} 적을 {enemyData.spawnCount}마리 소환합니다.");

                // 설정된 spawnCount만큼 반복해서 하나씩 소환
                for (int i = 0; i < enemyData.spawnCount; i++)
                {
                    SpawnEnemyOuterCircle(enemyData.enemyPrefab);
                    yield return new WaitForSeconds(spawnInterval);
                }
            }

            // 다음 웨이브 준비
            currentWave++;

            // [선택 사항] 다음 웨이브 때 난이도를 높이고 싶다면, 아래 주석을 해제하여 다음 웨이브엔 갯수가 늘어나게 만들 수 있습니다.
            /*
            for (int i = 0; i < enemyWaveList.Count; i++)
            {
                var data = enemyWaveList[i];
                data.spawnCount += 2; // 다음 웨이브 시 모든 적 종류의 소환 수를 2마리씩 증가
                enemyWaveList[i] = data;
            }
            */

            Debug.Log($"{waveInterval}초 후 다음 웨이브가 시작됩니다.");
            yield return new WaitForSeconds(waveInterval);
        }
    }

    // 전방향 외곽 무작위 소환 로직 (매개변수로 어떤 프리팹을 소환할지 받습니다)
    private void SpawnEnemyOuterCircle(GameObject prefab)
    {
        if (prefab == null) return;

        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        Vector2 spawnCenter = transform.position;
        Vector2 spawnPos = spawnCenter + (randomDirection * enemySpawnRadius);

        Instantiate(prefab, spawnPos, Quaternion.identity);
    }

    void Update()
    {
        
    }
}