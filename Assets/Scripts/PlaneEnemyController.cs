using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneEnemyController : MonoBehaviour
{
    private Rigidbody planeRigidbody;
    private float velocityZ = -20f;

    //敵の爆発
    public GameObject explosionPrefab;

    //爆発音
    AudioSource audioSource;
    public AudioClip planeExplosionSound;

    void Start()
    {
        this.planeRigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
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
            Destroy(this.gameObject);
            GameObject explosion = Instantiate(explosionPrefab, this.transform.position, Quaternion.identity);
            // audioSource.PlayOneShot(planeExplosionSound);
            Destroy(explosion, 0.3f);
            Debug.Log("敵爆発");
        }
    }
}
