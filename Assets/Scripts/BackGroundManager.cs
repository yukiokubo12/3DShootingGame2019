using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundManager : MonoBehaviour
{
    public GameObject leftGrounds;
    public GameObject rightGrounds;
    public GameObject player;

    void Start()
    {
        this.leftGrounds = GameObject.Find("LeftGrounds");
        // this.leftGrounds.SetActive(false);
        this.rightGrounds = GameObject.Find("RightGrounds");
        // this.rightGrounds.SetActive(false);
        DestroyLeftGrounds();
        DestroyRightGrounds();
    }

    void Update()
    {
        Invoke("AppearLeftGrounds", 26);
        Invoke("AppearRightGrounds", 26);
        if(this.leftGrounds.transform.position.z < player.transform.position.z + 400)
        {
            Invoke("DestroyLeftGrounds", 10);
            // this.leftGrounds.SetActive(true);
            AppearLeftGrounds();
            this.leftGrounds.transform.position = new Vector3(this.leftGrounds.transform.position.x, this.leftGrounds.transform.position.y, player.transform.position.z + 900);
        }
        if(this.rightGrounds.transform.position.z < player.transform.position.z + 400)
        {
            Invoke("DestroyRightGrounds", 10);
            // this.rightGrounds.SetActive(true);
            AppearRightGrounds();
            this.rightGrounds.transform.position = new Vector3(this.rightGrounds.transform.position.x, this.rightGrounds.transform.position.y, player.transform.position.z + 900);
        }
    }

    private void AppearLeftGrounds()
    {
        this.leftGrounds.SetActive(true);
    }

    private void DestroyLeftGrounds()
    {
        this.leftGrounds.SetActive(false);
    }

    private void AppearRightGrounds()
    {
        this.rightGrounds.SetActive(true);
    }

    private void DestroyRightGrounds()
    {
        this.rightGrounds.SetActive(false);
    }
}
