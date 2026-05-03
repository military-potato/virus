using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    // 1. 게임 시작 (SampleScene 이동)
    public void ChangeToSampleScene()
    {
        SceneManager.LoadScene("SampleScene");
    }

    // 2. 세팅 화면 이동
    public void ChangeToSettingsScene()
    {
        SceneManager.LoadScene("Settings");
    }

    // 3. 메인 메뉴 화면 이동 (새로 추가할 내용)
    public void ChangeToMenuScene()
    {
        // 실제 메뉴 씬 파일 이름인 "Menu"를 입력합니다.
        SceneManager.LoadScene("Menu");
    }

    // 4. 게임 종료
    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}