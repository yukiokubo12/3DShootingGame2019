using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveByDistance : MonoBehaviour
{
    private GameObject player;
    public float distance = 0;
    
    void Start()
    {
        this.player = GameObject.Find("Player");
    }

    void Update()
    {
        // if(Vector3.Distance(transform.position, player.transform.position) < distance)
        // {
        //     // GetComponent<MeshRenderer>().enabled = true;
        //     // GetComponent<BoxCollider>().enabled = true;
        // }else{
        //     // GetComponent<MeshRenderer>().enabled = false;
        //     // GetComponent<BoxCollider>().enabled = false;
        // }
    }
}
