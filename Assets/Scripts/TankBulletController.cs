using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankBulletController : MonoBehaviour
{
    public GameObject tankbullet;
    public GameObject tankbulletPos;
    public float speed = 10000f;

    private float interval = 0.5f;

    void Start()
    {
        
    }

    void Update()
    {
        interval += Time.deltaTime;
        if(interval >= 2f)
        {
            GameObject tankbulletsPos = Instantiate(tankbulletPos) as GameObject;
            GameObject tankbullets = Instantiate(tankbullet) as GameObject;
            interval = 0f;
            tankbullets.transform.position = tankbulletPos.transform.position;
            // Debug.Log("弾発射");
            Vector3 force;
            force = this.gameObject.transform.forward * speed;
            tankbullets.GetComponent<Rigidbody>().AddForce(force);
            // tankbullets.GetComponent<Rigidbody>().velocity = transform.forward * speed;
            // tankbullets.transform.position = transform.position;

            Destroy(tankbullets, 2f);
        }
    }
}
