using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipBulletAttatchController : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "PlayerTag" || other.gameObject.tag == "BuildintTag")
        {
            Destroy(this.gameObject);
        }
    }
}
