using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public Text scoreText;
    private int score = 0;
    public GameObject planeEnemy;

    void Start()
    {
        score = 0;
        SetScore(0);
        // scoreText = GameObject.Find("ScoreText");
    }

    void Update()
    {

    }

    public void SetScore(int addScore)
    {
        this.score = score + addScore;
        this.scoreText.text = string.Format("{0} Hits", score);
    }
}
