using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScoring : MonoBehaviour
{
    public static int totalScore = 0;
    //public static List<int> levelScore; useful later if I track every level score
    public static int levelScore = 0;

    //// Start is called before the first frame update
    //void Start()
    //{
        
    //}

    //// Update is called once per frame
    //void Update()
    //{
        
    //}

    public static void AddLeveltoTotal()
    {
        totalScore += levelScore;
        levelScore = 0;
    }

    public static void AddtoLevelScore(int points)
    {
        levelScore += points;
    }
    public static void SubtractfromLevelScore(int points)
    {
        levelScore -= points;
    }

    public static void ResetLevelScore()
    {
        levelScore = 0;
    }
    public static void ResetTotalScore()
    {
        totalScore = 0;
    }

}
