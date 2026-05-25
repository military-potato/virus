using UnityEngine;

public class gameover : MonoBehaviour
{
    [Header("게임오버 시 켤 반투명 패널 UI")]
    [SerializeField] private GameObject gameOverPanel;

    private void Start()
    {
        // 게임 시작 시에는 판넬을 확실히 꺼두고, 시간을 정상(1)으로 돌립니다.
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        Time.timeScale = 1f; 
    }

    /// <summary>
    /// 게임오버 조건이 충족되었을 때 외부에서 호출할 함수
    /// </summary>
    public void TriggerGameOver()
    {
        Debug.Log("게임 오버 트리거 발동!");

        // 1. 모든 유닛과 물리 연산을 그 자리에 멈춤
        Time.timeScale = 0f;

        // 2. 만들어둔 반투명 게임오버 UI 창을 켬
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("GameOverPanel이 인스펙터에 연결되지 않았습니다!");
        }
    }
}