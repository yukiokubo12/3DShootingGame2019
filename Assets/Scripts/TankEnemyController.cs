using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankEnemyController : MonoBehaviour
{
    private Rigidbody tankRigidbody;
    private float velocityZ = -5f;
    public int maxTankHp;
    public int currentTankHp;
    public GameObject explosionPrefab;

    void Start()
    {
        this.maxTankHp = 100;
        this.currentTankHp = this.maxTankHp;
        this.tankRigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        this.tankRigidbody.velocity = new Vector3(0, 0, this.velocityZ);
    }
    
    //敵爆発
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "BulletTag")
        {
            int damage = 10;
            this.currentTankHp -= damage;
        }
        if(other.gameObject.tag == "PlayerTag")
        {
            int damage = 30;
            this.currentTankHp -= damage;
        }
        if(this.currentTankHp <= 0)
        {
            Destroy(this.gameObject);
            GameObject explosion = Instantiate(explosionPrefab, this.transform.position, Quaternion.identity);
            var soundManager = GameObject.Find("SoundManager");
            soundManager.GetComponent<SoundManager>().TankExplosionSound();
            Destroy(explosion, 0.3f);
            var scoreManager = GameObject.Find("GameSystem");
            scoreManager.GetComponent<ScoreManager>().SetScore(1);
        }
    }
}
