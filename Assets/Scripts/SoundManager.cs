using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    //爆発音
    public AudioSource audioSource;
    public AudioClip planeExplosionSound;
    public AudioClip tankExplosionSound;
    public AudioClip shipExplosionSound;
    public AudioClip playerDamageSound;
    public AudioClip playerBulletSound;

    public float volume;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaneExplosionSound()
    {
        audioSource.PlayOneShot(planeExplosionSound);
    }
    public void TankExplosionSound()
    {
        audioSource.PlayOneShot(tankExplosionSound);
        audioSource.volume = volume;
    }
    public void ShipExplosionSound()
    {
        audioSource.PlayOneShot(shipExplosionSound);
        audioSource.volume = volume;
    }
    public void PlayerDamageSound()
    {
        audioSource.PlayOneShot(playerDamageSound);
        audioSource.volume = volume;
    }
    public void PlayerBulletSound()
    {
        audioSource.PlayOneShot(playerBulletSound);
        audioSource.volume = volume;
    }
}
