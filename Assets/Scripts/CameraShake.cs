using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    // public GameObject player;

   public void Shake(float duration, float magnitude)
   {
       StartCoroutine(DoShake(duration, magnitude));
   }
   
//    public void Shake(float duration, float magnitude)
//    {
//        StartCoroutine(DoShake(duration, magnitude));
//    }

   private IEnumerator DoShake(float duration, float magnitude)
   {
       var pos = transform.localPosition;
       var elapsed = 0f;
       while(elapsed < duration)
       {
           var x = pos.x + Random.Range(-1f, 1f) * magnitude;
           var y = pos.y + Random.Range(-1f, 1f) * magnitude;
           transform.localPosition = new Vector3(x, y, pos.z);
           elapsed += Time.deltaTime;
           yield return null;
       }
       transform.localPosition = pos;
   }

    // void Start()
    // {
    //     this.player = GameObject.Find("Player");
    // }

    // void Update()
    // {
    //     this.player.GetComponent<PlayerController>().Shakeable();
    // }

    // public void Shakeable()
    // {
    //     Shake(0.25f, 0.1f);
    // }
}
