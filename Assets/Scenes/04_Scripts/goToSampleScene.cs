using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomSceneChanger : MonoBehaviour
{
    [Header("이동할 씬 이름을 입력하세요")]
    [SerializeField] private string targetSceneName;

    // 인스펙터에 설정된 targetSceneName으로 이동
    public void ChangeToTargetScene()
    {
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            SceneManager.LoadScene(targetSceneName);
        }
        else
        {
            Debug.LogError("인스펙터에서 Target Scene Name을 지정하지 않았습니다!");
        }
    }
}