using UnityEngine;

public class TestSpawner : MonoBehaviour
{
    public GameObject hojunguPrefab; // 아군세포 프리팹
    public GameObject enemyPrefab; // 적세포 프리팹
    public GameObject daesikPrefab; 
    public GameObject killerTPrefab;
    public GameObject limpguPrefab;
    public GameObject NKsepoPrefab;
    public float enemySpawnRadius = 40; // 적이 생성될 외곽 원의 반지름

    public Transform giziTransform;     // 기지 위치 (자동 생성 지점)
    private float timer1, timer3, timer4, timer5, timer6;

    void Update()
    {
        if (timer1 > 0) timer1 -= Time.deltaTime;
        if (timer3 > 0) timer3 -= Time.deltaTime;
        if (timer4 > 0) timer4 -= Time.deltaTime;
        if (timer5 > 0) timer5 -= Time.deltaTime;
        if (timer6 > 0) timer6 -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Alpha1)) TrySpawn(hojunguPrefab, ref timer1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) TrySpawn(daesikPrefab, ref timer3);
        if (Input.GetKeyDown(KeyCode.Alpha4)) TrySpawn(killerTPrefab, ref timer4);
        if (Input.GetKeyDown(KeyCode.Alpha5)) TrySpawn(NKsepoPrefab, ref timer5);
        if (Input.GetKeyDown(KeyCode.Alpha6)) TrySpawn(limpguPrefab, ref timer6);
        /*
        // 1번 키를 누르면 마우스 커서 위치에 호중구 소환
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Instantiate(hojunguPrefab, mousePos, Quaternion.identity);
        }
        
        // 3번 키를 누르면 마우스 커서 위치에 대식 소환
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Instantiate(daesikPrefab, mousePos, Quaternion.identity);
        }
        //4번 키를 누르면 마우스 커서 위치에 킬러T 소환
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Instantiate(killerTPrefab, mousePos, Quaternion.identity);
        }
        // 5번 키를 누르면 마우스 커서 위치에 림프구 소환
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Instantiate(limpguPrefab, mousePos, Quaternion.identity);
        }
        // 6번 키를 누르면 마우스 커서 위치에 NK세포 소환
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Instantiate(NKsepoPrefab, mousePos, Quaternion.identity);
        }
        */
        // 2번 키를 누르면 화면 밖(외곽) 랜덤 위치에 적군 소환
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Vector2 randomDirection = Random.insideUnitCircle.normalized;

            // 핵심 수정: 현재 Spawner 오브젝트의 위치(transform.position)를 기준으로 반지름만큼 더해줍니다.
            Vector2 spawnCenter = transform.position;
            Vector2 spawnPos = spawnCenter + (randomDirection * enemySpawnRadius);

            Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        }
    }
    private void TrySpawn(GameObject prefab, ref float timer)
    {
        if (prefab == null)
        {
            Debug.LogWarning("프리팹이 연결되지 않았습니다!");
            return;
        }

        if (giziTransform == null)
        {
            Debug.LogError("Gizi(기지) 트랜스폼이 연결되지 않았습니다!");
            return;
        }

        if (timer <= 0)
        {
            SpawnNearBase(prefab);
            // 유닛의 스탯에서 쿨타임 정보를 가져와 타이머 설정
            UnitBase unitScript = prefab.GetComponent<UnitBase>();
            timer = unitScript.produceCooldown;
            Debug.Log($"{prefab.name} 생성! 다음 생성까지 {timer}초 대기");
        }
        else
        {
            Debug.Log($"{prefab.name} 생성 중... 남은 시간: {timer:F1}초");
        }
    }

    private void SpawnNearBase(GameObject prefab)
    {
        // 기지 위치에서 반경 2단위 내의 무작위 지점 계산
        Vector2 randomOffset = Random.insideUnitCircle * 2f;
        Vector3 spawnPos = giziTransform.position + new Vector3(randomOffset.x, randomOffset.y, 0);

        Instantiate(prefab, spawnPos, Quaternion.identity);
    }
}