using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletController : MonoBehaviour
{
    public GameObject bullet;
    public GameObject bulletPos;
    public float speed = 20000f;
    AudioSource audioSource;
    public AudioClip playerBulletSound;

    private float interval = 0.2f;
    private float time = 0f;

    public int maxMp;
    public double currentMp;
    public Slider playerMPSlider;
    
    public Text scoreText;
    private bool isShotEnable = true;
    

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        // MP
        this.playerMPSlider.value = 1;
        this.maxMp = 150;
        this.currentMp = this.maxMp;
    }

    void Update()
    {
        //isShotEnableが有効であれば、発射できる。
        if(Input.GetKey(KeyCode.A) && time <= 0f && isShotEnable)
        {
            GameObject bullets = Instantiate(bullet) as GameObject;
            bullets.GetComponent<ScoreController>().SetText(scoreText);
            
            int attack = 10;
            this.currentMp -= attack;
            playerMPSlider.value = (float)currentMp / maxMp;
            time = interval;
            bullets.transform.position = bulletPos.transform.position;
            Debug.Log("プレイヤー弾発射");
            Vector3 force;
            force = this.gameObject.transform.forward * speed;
            bullets.GetComponent<Rigidbody>().AddForce(force);
            audioSource.PlayOneShot(playerBulletSound);
            Destroy(bullets, 1.5f);
        }
        
        if(this.currentMp <= maxMp)
        {
            Invoke("RecoverMp", 1);
            playerMPSlider.value = (float)currentMp / maxMp;
        }
        if(this.currentMp >= maxMp)
        {
            this.currentMp = maxMp;
            this.playerMPSlider.value = 1;
        }
        if(this.currentMp <= 0 && isShotEnable)
        {
            this.currentMp = 0;
            this.playerMPSlider.value = 0;
            StartCoroutine(MPCoroutine());
            Invoke("RecoverMp", 1);
        }
        if(time > 0f)
        {
            time -= Time.deltaTime;
        }
    }
    void RecoverMp()
    {
        this.currentMp += 0.02;
        playerMPSlider.value = (float)currentMp / maxMp;
    }
    IEnumerator MPCoroutine()
    {
        isShotEnable = false;
        yield return new WaitForSeconds(5.0f);
        isShotEnable = true;
    }
}
