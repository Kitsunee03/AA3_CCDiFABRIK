using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public Button playButton;
    public Button exitButton;

    private void Awake()
    {
        if (playButton != null) { playButton.onClick.AddListener(PlayGame); }
        if (exitButton != null) { exitButton.onClick.AddListener(ExitGame); }
    }

    public void PlayGame()
    {
        RemoveListeners();
        SceneManager.LoadScene(1);
    }

    public void ExitGame()
    {
        RemoveListeners();

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    private void RemoveListeners()
    {
        if (playButton != null) { playButton.onClick.RemoveListener(PlayGame); }
        if (exitButton != null) { exitButton.onClick.RemoveListener(ExitGame); }
    }
}