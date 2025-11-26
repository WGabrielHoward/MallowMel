using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI; // Assign the PauseMenu Canvas here
    private bool isPaused = false;
    public TextMeshProUGUI pausedMessage;

    private void Start()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;
        pausedMessage.text = "Paused\n" + sceneName;
    }

    void Update()
    {
        // Toggle pause with Escape or Controller Start button
        if (Input.GetKeyDown(KeyCode.P) ) //|| Input.GetButtonDown("Start"))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        AudioListener.pause = false;
        isPaused = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        AudioListener.pause = true;
        isPaused = true;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        PlayerScoring.ResetLevelScore();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene(0, LoadSceneMode.Single);
        PlayerScoring.ResetLevelScore();
        PlayerScoring.ResetTotalScore();
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stop in editor
#elif UNITY_WEBGL
        // Refresh the page in WebGL
        Application.OpenURL("javascript:location.reload();");
#else
        UnityEngine.Application.Quit(); // Quit in build
        // alternative was: System.Net.Mime.MediaTypeNames.Application
#endif
    }
}
