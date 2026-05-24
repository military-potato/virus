using UnityEngine;

public abstract class ActiveSkillBase : MonoBehaviour
{
    [Header("[ 스킬 기본 정보 ]")]
    public string skillName;
    public int currentLevel = 1;
    public int maxLevel = 3;

    [Header("[ 자원 소모 설정 ]")]
    // 인스펙터에서 각 레벨(1, 2, 3레벨)일 때 소모할 염증 수치를 적습니다. (예: 20, 40, 60)
    public int[] costInflammationByLevel = new int[3] { 20, 40, 60 };

    // 인스펙터에서 레벨업할 때(1->2레벨, 2->3레벨) 소모할 염증 수치를 적습니다. (예: 50, 100)
    public int[] upgradeCostInflammation = new int[2] { 50, 100 };

    [Header("[ 쿨타임 설정 ]")]
    public float cooldownTime = 10f;
    protected float currentCooldown = 0f;
    protected bool isCooldown = false;

    protected virtual void Update()
    {
        if (isCooldown)
        {
            currentCooldown -= Time.deltaTime;
            if (currentCooldown <= 0)
            {
                currentCooldown = 0f;
                isCooldown = false;
            }
        }
    }

    // [핵심 1] 스킬 사용 시 자원 검사 및 차감
    public void TryUseSkill()
    {
        if (isCooldown)
        {
            Debug.Log($"{skillName}은(는) 아직 쿨타임입니다!");
            return;
        }

        // 현재 레벨에 맞는 염증 수치 코스트 가져오기
        int requiredCost = costInflammationByLevel[currentLevel - 1];

        // StatusController에서 염증 수치를 성공적으로 차감했다면 스킬 발동
        if (StatusController.Instance.TrySpendInflammation(requiredCost))
        {
            ActivateSkillEffect(); // 각 하위 스킬들이 구현한 진짜 효과 실행
            StartCooldown();
        }
    }

    // [핵심 2] 레벨업 시 자원 검사 및 차감
    public void TryUpgradeSkill()
    {
        if (currentLevel >= maxLevel)
        {
            Debug.Log($"{skillName}은(는) 이미 최고 레벨({maxLevel})입니다.");
            return;
        }

        // 레벨업에 필요한 코스트 가져오기 (1->2렙이면 0번 인덱스, 2->3렙이면 1번 인덱스)
        int upgradeCost = upgradeCostInflammation[currentLevel - 1];

        // StatusController에서 염증 수치를 성공적으로 차감했다면 레벨업 진행
        if (StatusController.Instance.TrySpendInflammation(upgradeCost))
        {
            currentLevel++;
            Debug.Log($"{skillName} 레벨업 성공! 현재 레벨: {currentLevel}");
            OnLevelUp(); // 레벨업 시 추가로 처리할 사항이 있다면 실행
        }
    }

    // 자식 스킬들이 고유의 효과를 채워넣을 추상 함수들
    protected abstract void ActivateSkillEffect();
    protected virtual void OnLevelUp() { }

    protected void StartCooldown()
    {
        currentCooldown = cooldownTime;
        isCooldown = true;
    }
}