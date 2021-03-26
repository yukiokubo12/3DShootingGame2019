using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneEnemyController : MonoBehaviour
{
    private Rigidbody planeRigidbody;
    private float velocityZ = -20f;
    public GameObject explosionPrefab;

    void Start()
    {
        this.planeRigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        this.planeRigidbody.velocity = new Vector3(0, 0, this.velocityZ);
    }
    
    //敵爆発
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "BulletTag" || other.gameObject.tag == "PlayerTag")
        {
            this.gameObject.SetActive(false);
            GameObject explosion = Instantiate(explosionPrefab, this.transform.position, Quaternion.identity);
            var scoreManager = GameObject.Find("GameSystem");
            scoreManager.GetComponent<ScoreManager>().SetScore(1);
            var soundManager = GameObject.Find("SoundManager");
            soundManager.GetComponent<SoundManager>().PlaneExplosionSound();
            Destroy(explosion, 0.4f);
        }
    }
}
