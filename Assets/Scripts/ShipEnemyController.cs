﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipEnemyController : MonoBehaviour
{
    private Rigidbody shipRigidbody;
    private float velocityZ = 50f;

    public int maxShipHp;
    public int currentShipHp;

    //敵の爆発
    public GameObject explosionPrefab;

    //爆発音
    AudioSource audioSource;
    public AudioClip shipExplosionSound;

    //クリア処理
    // public GameObject clearText;
    // public GameObject titleButton;
    // public GameObject retryButton;

    void Start()
    {
        this.maxShipHp = 600;
        this.currentShipHp = this.maxShipHp;
        this.shipRigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        
        //クリア処理
        // this.clearText = GameObject.Find("ClearText");
        // this.retryButton = GameObject.Find("ToMainButton");
        // this.titleButton = GameObject.Find("ToTitleButton");
        // this.clearText.GetComponent<Text>().text = "Game Clear!!";
        // this.clearText.SetActive(false);
        // retryButton.SetActive(false);
        // titleButton.SetActive(false);
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
            Debug.Log("ゲームクリア");
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

            // audioSource.PlayOneShot(tankExplosionSound);
            // Destroy(explosion, 0.3f);

            Debug.Log("敵爆発");
        }
    }
}
