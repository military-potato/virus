using UnityEngine;

public class CombinedCell : EnemyUnit
{
    [Header("Combined Cell Cloning Settings")]
    public GameObject combinedCellPrefab; // ★ 중요: 인스펙터에서 자기 자신의 프리팹을 연결해야 합니다.
    public float firstCloneTime = 15f;    // 첫 복제 시간 (15초)
    public float recloneTime = 30f;       // 이후 재복제 시간 (30초)
    public float spawnOffsetRange = 0.6f; // 복제될 때 서로 완전히 겹쳐서 물리 버그가 나지 않도록 줄 오프셋 거리

    private float cloneTimer;

    // 복제본 전용 예외 처리 변수
    private bool isClone = false;
    private float inheritedHealth;

    // 원본 세포가 복제본을 만들고 나서 체력을 강제로 주입하기 위한 함수
    public void SetupClone(float health)
    {
        isClone = true;
        inheritedHealth = health;
        currentHealth = health; // 혹시 모를 타이밍 오류 방지를 위해 즉시 대입
    }

    protected override void Start()
    {
        // 1. 부모(EnemyUnit -> UnitBase)의 초기화 실행 (기지 찾기, currentHealth = maxHealth 설정)
        base.Start();

        // 2. [유니티 Start 함정 방지] 만약 이 유닛이 복제본이라면, 
        // 방금 base.Start()가 최대 체력으로 되돌려놓은 체력을 원본의 '현재 체력'으로 다시 덮어씁니다.
        if (isClone)
        {
            currentHealth = inheritedHealth;
            Debug.Log($"[합체세포 태어남] 원본의 체력 {currentHealth}을 물려받은 복제본이 스폰되었습니다.");
        }

        // 3. 첫 복제 시간(15초)으로 타이머를 맞춥니다.
        cloneTimer = firstCloneTime;
    }

    protected override void Update()
    {
        // 부모(EnemyUnit)의 AI 로직 실행 (중앙 기지 추적, 아군 탐색 및 공격)
        base.Update();

        // 복제 타이머 관리
        HandleCloningTimer();
    }

    private void HandleCloningTimer()
    {
        if (cloneTimer > 0)
            cloneTimer -= Time.deltaTime;

        if (cloneTimer <= 0)
        {
            // 복제 능력 발동!
            CloneSelf();

            // ★ 기획서 반영: 다음 복제부터는 재복제 주기(30초)로 시계를 리셋합니다.
            cloneTimer = recloneTime;
        }
    }

    private void CloneSelf()
    {
        if (combinedCellPrefab == null)
        {
            Debug.LogError("[합체세포] 인스펙터 창에 합체세포 프리팹(Combined Cell Prefab)이 등록되지 않았습니다!");
            return;
        }

        // ➔ 질문하신 생성위치 벡터 계산:
        // 유닛들이 완전히 똑같은 좌표에 겹쳐서 소환되면 리지드바디 물리 엔진 충돌 때문에 툭 튕겨 나가거나 겹치는 버그가 생깁니다.
        // 이를 막기 위해 원본 위치에서 상하좌우로 살짝(0.6f) 빗겨나간 무작위 '오프셋 벡터'를 더해줍니다.
        Vector3 spawnOffset = new Vector3(
            Random.Range(-spawnOffsetRange, spawnOffsetRange),
            Random.Range(-spawnOffsetRange, spawnOffsetRange),
            0
        );
        Vector3 spawnPosition = transform.position + spawnOffset;

        Debug.Log($"[합체세포 분열] 복제 실행! 현재 원본 체력: {currentHealth}");

        // 1. 새로운 합체 세포 오브젝트 생성 (동일한 프리팹이므로 복제본도 똑같이 이 복제 스크립트를 가집니다.)
        GameObject cloneObj = Instantiate(combinedCellPrefab, spawnPosition, Quaternion.identity);

        // 2. 생성된 복제본의 스크립트 컴포넌트를 가져옴
        CombinedCell cloneScript = cloneObj.GetComponent<CombinedCell>();

        if (cloneScript != null)
        {
            // 3. ★ 핵심 기획 구현: 원본의 '현재 체력(currentHealth)'을 복제본에게 주입합니다.
            cloneScript.SetupClone(currentHealth);
        }
    }
}