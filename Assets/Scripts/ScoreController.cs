using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ScoreController : MonoBehaviour
{
    public Text scoreText;
    private int score = 0;

    void Start()
    {
        score = 0;
        SetScore();
    }

    // /スコアの参照を渡す
    public void SetText(Text scoreText)
    {
        this.scoreText = scoreText;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "PlaneTag" || other.gameObject.tag == "TankTag")
        {
           score++;
        }
        SetScore();
    }

    public void SetScore()
    {
        this.scoreText.text = string.Format("{0} Hits", score);
    }
}
