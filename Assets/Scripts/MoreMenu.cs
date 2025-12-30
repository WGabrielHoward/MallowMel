using UnityEngine;

public class MoreMenu : MonoBehaviour
{
    public GameObject optionsCanvas;
    public GameObject backstoryCanvas;
    public GameObject infoCanvas;
    public GameObject creditsCanvas;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        backstoryCanvas.SetActive(false);
        infoCanvas.SetActive(false);
        creditsCanvas.SetActive(false);
        optionsCanvas.SetActive(true);
    }


    public void Options()
    {
        backstoryCanvas.SetActive(false);
        infoCanvas.SetActive(false);
        creditsCanvas.SetActive(false);
        optionsCanvas.SetActive(true);
    }
    public void Info()
    {
        optionsCanvas.SetActive(false);
        infoCanvas.SetActive(true);
    }

    public void Backstory()
    {
        optionsCanvas.SetActive(false);
        backstoryCanvas.SetActive(true);
    }

    public void Credits()
    {
        optionsCanvas.SetActive(false);
        creditsCanvas.SetActive(true);
    }
}
