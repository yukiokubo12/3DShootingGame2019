using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDestroyController : MonoBehaviour
{
    public GameObject player;
    
    void Start()
    {
        this.player = GameObject.Find("Player");
    }
    void Update()
    {
        //敵の消去
        if(player == null || this.transform.position.z < player.transform.position.z)
        {
            Destroy(this.gameObject);
            // Debug.Log("敵消去");
        }
    }
}
