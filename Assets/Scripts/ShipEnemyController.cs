using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipEnemyController : MonoBehaviour
{
    private Rigidbody shipRigidbody;
    private float velocityZ = 50f;
    //ラスボスステータス
    public int maxShipHp;
    public int currentShipHp;
    //敵の爆発
    public GameObject explosionPrefab;

    void Start()
    {
        this.maxShipHp = 600;
        this.currentShipHp = this.maxShipHp;
        this.shipRigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        this.shipRigidbody.velocity = new Vector3(0, 0, this.velocityZ);
    }
    //敵爆発
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "BulletTag")
        {
            int damage = 10;
            this.currentShipHp -= damage;
        }
        if(other.gameObject.tag == "PlayerTag")
        {
            int damage = 30;
            this.currentShipHp -= damage;
        }
        if(this.currentShipHp <= 0)
        {
            this.gameObject.SetActive(false);
            GameObject explosion = Instantiate(explosionPrefab, this.transform.position, Quaternion.identity);
            var scoreManager = GameObject.Find("GameSystem");
            scoreManager.GetComponent<ScoreManager>().SetScore(1);

            var toMainButton = GameObject.Find("GameSystem");
            toMainButton.GetComponent<GameSystem>().ShowMainButton();
            var toTitleButton = GameObject.Find("GameSystem");
            toTitleButton.GetComponent<GameSystem>().ShowTitleButton();
            var clearText = GameObject.Find("GameSystem");
            clearText.GetComponent<GameSystem>().ShowClearText();

            var soundManager = GameObject.Find("SoundManager");
            soundManager.GetComponent<SoundManager>().ShipExplosionSound();
        }
    }
}
