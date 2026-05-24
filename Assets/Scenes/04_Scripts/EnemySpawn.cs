using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawn : MonoBehaviour
{
    // 1. 단일 유닛 소환 정보 구조체
    [System.Serializable]
    public struct EnemySpawnData
    {
        public string enemyName;       // 에디터 확인용 이름 (예: 슬라임, 보스 등)
        public GameObject enemyPrefab;  // 소환할 적 프리팹
        public int spawnCount;         // 이 유닛을 몇 마리 소환할 것인가
    }

    // 2. ★ [추가] 각 웨이브별 설정을 통째로 묶어줄 기획 구조체
    [System.Serializable]
    public struct WaveSettings
    {
        public string waveName;                      // 에디터 확인용 이름 (예: 웨이브 1 - 초반 러시)
        public List<EnemySpawnData> enemySpawnList;  // 이 웨이브에 등장할 적 무리 목록
        public float spawnInterval;                  // 유닛 간의 스폰 간격 (예: 0.5초당 한 마리씩)
        public float postWaveDelay;                  // 이 웨이브가 끝난 뒤 다음 웨이브까지 쉬는 시간
    }

    [Header("Global Spawn Settings")]
    public float enemySpawnRadius = 40f; // 적이 생성될 외곽 원의 반지름

    [Header("Wave Sequence Settings")]
    // 💡 인스펙터에서 이 리스트의 칸을 늘려 총 웨이브 수와 종류를 마음대로 디자인할 수 있습니다!
    public List<WaveSettings> waves = new List<WaveSettings>();

    private int currentWaveIndex = 0; // 현재 몇 번째 웨이브 인덱스인지 기록

    void Start()
    {
        if (waves.Count > 0)
        {
            StartCoroutine(WaveRoutine());
        }
        else
        {
            Debug.LogWarning("EnemySpawn: 기획된 웨이브 설정(Waves)이 비어있습니다!");
        }
    }

    private IEnumerator WaveRoutine()
    {
        // 총 설정된 웨이브 갯수만큼만 순차적으로 순회합니다 (무한 루프 제거)
        for (currentWaveIndex = 0; currentWaveIndex < waves.Count; currentWaveIndex++)
        {
            WaveSettings currentWave = waves[currentWaveIndex];
            
            Debug.Log($"<color=cyan>=== [{currentWave.waveName}] 시작 (단계: {currentWaveIndex + 1}/{waves.Count}) ===</color>");

            // 현재 웨이브에 묶인 적 데이터 목록을 하나씩 순회
            foreach (EnemySpawnData enemyData in currentWave.enemySpawnList)
            {
                if (enemyData.enemyPrefab == null) continue;

                string nameToDisplay = string.IsNullOrEmpty(enemyData.enemyName) ? enemyData.enemyPrefab.name : enemyData.enemyName;
                Debug.Log($"[{currentWave.waveName}] {nameToDisplay} 적을 {enemyData.spawnCount}마리 스폰합니다.");

                // 설정된 spawnCount만큼 반복해서 하나씩 소환
                for (int i = 0; i < enemyData.spawnCount; i++)
                {
                    SpawnEnemyOuterCircle(enemyData.enemyPrefab);
                    yield return new WaitForSeconds(currentWave.spawnInterval); // 해당 웨이브의 지정 속도로 대기
                }
            }

            // 마지막 웨이브까지 클리어했다면 루프 밖 종료 처리로 유도하기 위해 딜레이 스킵
            if (currentWaveIndex == waves.Count - 1)
                break;

            Debug.Log($"[{currentWave.waveName}] 종료. {currentWave.postWaveDelay}초 후 다음 웨이브가 시작됩니다.");
            yield return new WaitForSeconds(currentWave.postWaveDelay);
        }

        // 모든 웨이브를 성공적으로 마친 시점
        Debug.Log("<color=lime><b>★ 모든 지정 웨이브 소환 종료! 올 클리어! ★</b></color>");
        yield break; 
    }

    // 전방향 외곽 무작위 소환 로직
    private void SpawnEnemyOuterCircle(GameObject prefab)
    {
        if (prefab == null) return;

        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        Vector2 spawnCenter = transform.position;
        Vector2 spawnPos = spawnCenter + (randomDirection * enemySpawnRadius);

        Instantiate(prefab, spawnPos, Quaternion.identity);
    }
}