using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawn : MonoBehaviour
{
    [System.Serializable]
    public struct EnemySpawnData
    {
        public string enemyName;      // 에디터 확인용 이름
        public GameObject enemyPrefab; // 적 프리팹
        public int spawnCount;        // 이 적을 몇 마리 소환할 것인가
    }

    [Header("Enemy Variety Settings")]
    // 여러 적을 등록
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
            
            // 리스트에 등록된 모든 적 종류를 하나씩 순회
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

            Debug.Log($"{waveInterval}초 후 다음 웨이브가 시작됩니다.");
            yield return new WaitForSeconds(waveInterval);
        }
    }

    // 전방향 외곽 무작위 소환 로직 (매개변수로 어떤 프리팹을 소환할지 받음)
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