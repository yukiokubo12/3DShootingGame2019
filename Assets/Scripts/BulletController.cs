using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletController : MonoBehaviour
{
    public GameObject bullet;
    public GameObject bulletPos;
    public float speed = 20000f;
    private float interval = 0.2f;
    private float time = 0f;

    public int maxMp;
    public double currentMp;
    public Slider playerMPSlider;
    
    public Text scoreText;
    private bool isShotEnable = true;
    
    void Start()
    {
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
            int attack = 10;
            this.currentMp -= attack;
            playerMPSlider.value = (float)currentMp / maxMp;
            time = interval;
            bullets.transform.position = bulletPos.transform.position;
            Vector3 force;
            force = this.gameObject.transform.forward * speed;
            bullets.GetComponent<Rigidbody>().AddForce(force);
            var soundManager = GameObject.Find("SoundManager");
            soundManager.GetComponent<SoundManager>().PlayerBulletSound();
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
    //MP回復
    void RecoverMp()
    {
        this.currentMp += 0.12;
        playerMPSlider.value = (float)currentMp / maxMp;
    }
    IEnumerator MPCoroutine()
    {
        isShotEnable = false;
        yield return new WaitForSeconds(5.0f);
        isShotEnable = true;
    }
}
