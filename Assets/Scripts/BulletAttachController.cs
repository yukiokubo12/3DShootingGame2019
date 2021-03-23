using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletAttachController : MonoBehaviour
{
    // public Text scoreText;
    // private int score = 0;

    void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.tag == "PlaneTag" || other.gameObject.tag == "TankTag")
            {
                // score += 1;
                Destroy(this.gameObject);
            }
            // SetScore();
        }

    // void SetScore()
    // {
    //     this.scoreText.text = string.Format("{0} Hits", score);
    // }
}
