using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    void Start()
    {
        this.maxShipHp = 100;
        this.currentShipHp = this.maxShipHp;
        this.shipRigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
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
            Destroy(this.gameObject);
            GameObject explosion = Instantiate(explosionPrefab, this.transform.position, Quaternion.identity);
            // audioSource.PlayOneShot(tankExplosionSound);
            // Destroy(explosion, 0.3f);
            Debug.Log("敵爆発");
        }
    }
}
