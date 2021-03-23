using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCameraController : MonoBehaviour
{
    private GameObject player;
    private float difference;
    private Vector3 offset;

    void Start()
    {
        this.player = GameObject.Find("Player");
        this.difference = player.transform.position.z - this.transform.position.z;
        this.offset = transform.position - player.transform.position; 
    }

    void Update()
    {
        if(player != null)
        {
        this.transform.position = new Vector3(0, this.transform.position.y, this.player.transform.position.z - difference);
        transform.position = player.transform.position + offset;
        transform.rotation = player.transform.rotation;
        }
    }
}
