using UnityEngine;
using System.IO;

public class PersistentData : MonoBehaviour
{
    private int topPoints;

    public string playerName;
    private string topPointsName;

    public static PersistentData Instance;
    private void Awake()
    {
        // start of new code
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        // end of new code

        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadTopScore();
    }

    public string GetTopName()
    {
        string topName = topPointsName;
        return topName;
    }

    public int GetTopPoints()
    {
        int highScore = topPoints;
        return highScore;
    }

    public void ScoreUpdate(int currentPoints)
    {
        if (currentPoints > topPoints)
        {
            topPoints = currentPoints;
            topPointsName = playerName;
        }
    }


    [System.Serializable]
    class SaveData
    {
        public int topPoints;
        public string topPointsName;
    }

    public void SaveTopScore()
    {
        SaveData data = new SaveData();
        data.topPoints = topPoints;
        data.topPointsName = topPointsName;

        string json = JsonUtility.ToJson(data);

        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    public void LoadTopScore()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            topPoints = data.topPoints;
            topPointsName = data.topPointsName;
        }
    }

    public void ClearTopScore()
    {
        topPoints = 0;
        topPointsName = "";

    }
}
