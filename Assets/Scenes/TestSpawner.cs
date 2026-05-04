using UnityEngine;

public class TestSpawner : MonoBehaviour
{
    public GameObject allyPrefab;   // 아군세포 프리팹
    public GameObject enemyPrefab;  // 적세포 프리팹
    public float enemySpawnRadius = 8f; // 적이 생성될 외곽 원의 반지름

    void Update()
    {
        // 1번 키를 누르면 마우스 커서 위치에 아군 소환
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Instantiate(allyPrefab, mousePos, Quaternion.identity);
        }

        // 2번 키를 누르면 화면 밖(외곽) 랜덤 위치에 적군 소환
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            // 반지름 8f 밖의 랜덤한 방향(원 테두리) 계산
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            Vector2 spawnPos = randomDirection * enemySpawnRadius;
            Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        }
    }
}