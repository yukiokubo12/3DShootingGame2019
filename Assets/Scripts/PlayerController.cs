using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerController : MonoBehaviour
{
   //自機の爆発
   public GameObject explosionPrefab;
   //爆発音
   AudioSource audioSource;
   public AudioClip planeExplosionSound;
   //移動速度
   private Rigidbody myRigidbody;
   private float velocityX = 50f;
   private float velocityY = 50f;
   private float velocityZ = 50f;
   //移動範囲
   private float movableRangeX = 15f;
   private float movableRangeY = 11f;
   private float coefficient = 0.3f;
   private bool isEnd = false;
   //ステータス
   public int maxHp;
   public int currentHp;
   public Slider playerHPSlider;
   //ゲームオーバー処理
   private GameObject stateText;
   public GameObject retryButton;
   public GameObject titleButton;
   //カメラシェイク
   public CameraShake shake;

   private bool isRotate = true;

   void Start()
   {
       //プレイヤーHP 
       this.playerHPSlider.value = 1;
       this.maxHp = 120;
       this.currentHp = this.maxHp;
       this.myRigidbody = GetComponent<Rigidbody>();
       audioSource = GetComponent<AudioSource>();
       this.stateText = GameObject.Find("GameOverText");
       this.retryButton = GameObject.Find("ToMainButton");
       this.titleButton = GameObject.Find("ToTitleButton");
       retryButton.SetActive(false);
       titleButton.SetActive(false);
   }

   void Update()
   {
       this.myRigidbody.velocity = new Vector3(0, 0, this.velocityZ);
       float speed = 20f;
       float step = speed * Time.deltaTime;
       float inputVelocityX = 0;
       if(Input.GetKey(KeyCode.LeftArrow) && -this.movableRangeX < this.transform.position.x && isRotate)
       {
           inputVelocityX = -this.velocityX;
           transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, 20f), step);
       }
       else if(Input.GetKey(KeyCode.RightArrow) && this.transform.position.x < this.movableRangeX)
       {
           inputVelocityX = this.velocityX;
           transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, -20f), step);
       }
       if(Input.GetKeyUp(KeyCode.LeftArrow))
       {
           transform.rotation = Quaternion.Euler(0, 0, 0);
       }
       if(Input.GetKeyUp(KeyCode.RightArrow))
       {
           transform.rotation = Quaternion.Euler(0, 0, 0);
       }

       float inputVelocityY = 0;
       if(Input.GetKey(KeyCode.DownArrow) && -this.movableRangeY < this.transform.position.y)
       {
           inputVelocityY = -this.velocityY;
       }
       else if(Input.GetKey(KeyCode.UpArrow) && this.transform.position.y < this.movableRangeY)
       {
           inputVelocityY = this.velocityY;
       }

       this.myRigidbody.velocity = new Vector3(inputVelocityX, inputVelocityY, velocityZ);

       if(this.isEnd)
       {
           this.velocityX *= this.coefficient;
           this.velocityY *= this.coefficient;
           this.velocityZ *= this.coefficient;
       }
   }

   //自機消滅、爆発
   public void OnTriggerEnter(Collider other)
   {
       //飛行機、建物と衝突
       if(other.gameObject.tag == "PlaneTag" || other.gameObject.tag == "BuildingTag")
       {
           shake.Shake(0.2f, 0.2f);
           int damage = 10;
           this.currentHp -= damage;
           playerHPSlider.value = (float)currentHp / maxHp;
           var soundManager = GameObject.Find("SoundManager");
           soundManager.GetComponent<SoundManager>().PlayerDamageSound();
       }
       //飛行機弾との衝突
       if(other.gameObject.tag == "PlaneBulletTag")
       {
           shake.Shake(0.2f, 0.2f);
           int damage = 5;
           this.currentHp -= damage;
           playerHPSlider.value = (float)currentHp / maxHp;
           var soundManager = GameObject.Find("SoundManager");
           soundManager.GetComponent<SoundManager>().PlayerDamageSound();
       }
       //タンクと衝突
       if(other.gameObject.tag == "TankTag" || other.gameObject.tag == "TankBulletTag")
       {
           shake.Shake(0.2f, 0.2f);
           int damage = 20;
           this.currentHp -= damage;
           playerHPSlider.value = (float)currentHp / maxHp;
           var soundManager = GameObject.Find("SoundManager");
           soundManager.GetComponent<SoundManager>().PlayerDamageSound();
       }
       if(other.gameObject.tag == "ShipBulletTag")
       {
           shake.Shake(0.2f, 0.2f);
           int damage = 20;
           this.currentHp -= damage;
           playerHPSlider.value = (float)currentHp / maxHp;
           var soundManager = GameObject.Find("SoundManager");
           soundManager.GetComponent<SoundManager>().PlayerDamageSound();
       }
       //自機消滅、爆発
       if(this.currentHp <= 0)
       {
           retryButton.SetActive(true);
           titleButton.SetActive(true);
           this.isEnd = true;
           this.stateText.GetComponent<Text>().text = "Game Over";
           this.retryButton.GetComponent<Button>();
           this.gameObject.SetActive(false);
           GameObject explosion = Instantiate(explosionPrefab, this.transform.position, Quaternion.identity);
           Destroy(explosion, 0.3f);
           playerHPSlider.value = (float)currentHp / (float)maxHp; ;
           var soundManager = GameObject.Find("SoundManager");
           soundManager.GetComponent<SoundManager>().PlayerDamageSound();
       }
   }
}
