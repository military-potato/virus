using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class EnemySpawn : MonoBehaviour
{
    public static EnemySpawn Instance { get; private set; }

    [System.Serializable]
    public struct EnemySpawnData
    {
        public string enemyName;      
        public GameObject enemyPrefab; 
        public int spawnCount;        
    }

    [System.Serializable]
    public struct WaveData
    {
        public string waveName; 
        public List<EnemySpawnData> enemyList; 
    }

    [Header("Wave Configuration")]
    public List<WaveData> waves = new List<WaveData>();
    public float enemySpawnRadius = 40f; 
    
    [Header("Wave Settings")]
    public int currentWaveIndex = 0; 
    public float spawnInterval = 0.5f; 
    public float waveInterval = 2f; 

    [Header("UI Settings")]
    public TextMeshProUGUI waveText; 

    [Header("Runtime Monitor")]
    public int aliveEnemyCount = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (waves.Count == 0) return;
        StartCoroutine(WaveRoutine());
    }

    private IEnumerator WaveRoutine()
    {
        while (currentWaveIndex < waves.Count)
        {
            UpdateWaveUI(currentWaveIndex + 1);
            Debug.Log($"=== 웨이브 {currentWaveIndex + 1} 시작! ===");
            
            WaveData currentWaveData = waves[currentWaveIndex];

            foreach (EnemySpawnData enemyData in currentWaveData.enemyList)
            {
                if (enemyData.enemyPrefab == null) continue;

                for (int i = 0; i < enemyData.spawnCount; i++)
                {
                    SpawnEnemyOuterCircle(enemyData.enemyPrefab);
                    yield return new WaitForSeconds(spawnInterval);
                }
            }

            Debug.Log("모든 적이 소환되었습니다. 남은 적을 소탕하세요!");
            while (aliveEnemyCount > 0)
            {
                yield return null; 
            }

            Debug.Log($"웨이브 {currentWaveIndex + 1} 클리어!");
            currentWaveIndex++;

            if (currentWaveIndex >= waves.Count)
            {
                // 모든 웨이브를 클리어했을 때 처리 함수 실행
                HandleGameClear();
                yield break; 
            }

            Debug.Log($"{waveInterval}초 후 다음 웨이브가 시작됩니다.");
            yield return new WaitForSeconds(waveInterval);
        }
    }

    // 모든 웨이브 클리어 시 처리
    private void HandleGameClear()
    {
        Debug.Log("모든 웨이브가 끝났습니다! 승리!");
        if (waveText != null) waveText.text = "ALL WAVE CLEAR!";

        // [핵심 연동] 보내주신 gameclear 스크립트의 화면 작동 함수를 실행시킵니다.
        if (gameclear.Instance != null)
        {
            gameclear.Instance.TriggerGameOver();
        }
        else
        {
            Debug.LogError("씬에 gameclear 스크립트가 붙은 오브젝트가 없습니다!");
        }
    }

    private void SpawnEnemyOuterCircle(GameObject prefab)
    {
        if (prefab == null) return;

        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        Vector2 spawnCenter = transform.position;
        Vector2 spawnPos = spawnCenter + (randomDirection * enemySpawnRadius);

        Instantiate(prefab, spawnPos, Quaternion.identity);
        aliveEnemyCount++; 
    }

    private void UpdateWaveUI(int waveNumber)
    {
        if (waveText != null) waveText.text = $"WAVE {waveNumber}";
    }
}