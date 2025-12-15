using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DefaultExecutionOrder(1000)]
public class MenuUI : MonoBehaviour
{

    public Text TopScore;
    public InputField playerName;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (PersistentData.Instance != null)
        {

            TopScore.text = $"Top Score: {PersistentData.Instance.GetTopName()} {PersistentData.Instance.GetTopPoints()}";
        }
    }

    public void StartPlay()
    {
        if (PersistentData.Instance != null)
        {
            PersistentData.Instance.playerName = playerName.text.ToString();
        }
        SceneManager.LoadScene(1);
    }

    public void Exit()
    {
        PersistentData.Instance.SaveTopScore();     // enable this to autoSave any changes, else it will go with whatever was last saved
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit(); // original code to quit Unity player
#endif
    }
}
