using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneBulletController : MonoBehaviour
{
    public GameObject planebullet;
    public GameObject rightplanebulletPos;
    public GameObject leftplanebulletPos;
    public float speed = 20000f;

    private float interval = 0.5f;

    void Start()
    {
        
    }

    void Update()
    {
        interval += Time.deltaTime;
        if(interval >= 1f)
        {
            GameObject rightplanebulletsPos = Instantiate(rightplanebulletPos) as GameObject;
            GameObject leftplanebulletsPos = Instantiate(leftplanebulletPos) as GameObject;
            GameObject planebullets = Instantiate(planebullet) as GameObject;
            interval = 0f;
            planebullets.transform.position = rightplanebulletPos.transform.position;
            planebullets.transform.position = leftplanebulletPos.transform.position;
            // Debug.Log("弾発射");
            Vector3 force;
            force = this.gameObject.transform.forward * speed;
            planebullets.GetComponent<Rigidbody>().AddForce(force);
            // planebullets.GetComponent<Rigidbody>().velocity = transform.forward * speed;
            // planebullets.transform.position = transform.position;

            Destroy(planebullets, 2f);
        }
    }
}
