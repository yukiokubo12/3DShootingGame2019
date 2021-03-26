using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipRightBulletController : MonoBehaviour
{
    public GameObject shipbullet;
    public GameObject shipbulletPos;
    public float speed = 10000f;
    private float interval = 0.5f;

    void Update()
    {
        interval += Time.deltaTime;
        if(interval >= 2f)
        {
            GameObject shipbulletsPos = Instantiate(shipbulletPos) as GameObject;
            GameObject shipbullets = Instantiate(shipbullet) as GameObject;
            interval = 0f;
            shipbullets.transform.position = shipbulletPos.transform.position;
            Vector3 force;
            force = this.gameObject.transform.forward * speed;
            shipbullets.GetComponent<Rigidbody>().AddForce(force);
            Destroy(shipbullets, 2f);
        }
    }
}

