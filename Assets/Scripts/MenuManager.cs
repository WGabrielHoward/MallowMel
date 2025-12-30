using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject mainMenuUI; 
    public GameObject moreMenu;
    public GameObject quitCanvas;

    public void Play()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene(2, LoadSceneMode.Single);
    }

    public void Menu()
    {
        moreMenu.SetActive(false);
        mainMenuUI.SetActive(true);
    }

    public void More()
    {
        mainMenuUI.SetActive(false);
        moreMenu.SetActive(true);
    }

    public void Tutorial()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        mainMenuUI.SetActive(false);
        quitCanvas.SetActive(true);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stop in editor
//#elif UNITY_WEBGL
//        // Refresh the page in WebGL
//        UnityEngine.Application.OpenURL("javascript:location.reload();");
#else
        UnityEngine.Application.Quit(); // Quit in build
#endif
    }
}
