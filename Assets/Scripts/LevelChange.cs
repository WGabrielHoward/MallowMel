using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

// This would probably be better as a singleton of some sort
public class LevelChange : MonoBehaviour
{
    //public int sceneBuildIndex;
    [SerializeField] private float transitionTime;
    [SerializeField] private Animator transition;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            NextLevel();
        }
    }

    private void OnColliderEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            NextLevel();
        }
    }


    public void NextLevel()
    {
        transition.SetTrigger("End");
        PlayerScoring.AddLeveltoTotal();
        int nextScene = SceneManager.GetActiveScene().buildIndex + 1;
        StartCoroutine(GotoNextLevel(nextScene));
    }

    private IEnumerator GotoNextLevel(int sceneNumber)
    {
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(sceneNumber);

    }

    public void MMScreen()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene(0, LoadSceneMode.Single);
        PlayerScoring.totalScore = 0;
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
#endif
    }

}
