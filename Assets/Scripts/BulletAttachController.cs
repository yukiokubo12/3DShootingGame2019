using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletAttachController : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "PlaneTag" || other.gameObject.tag == "TankTag")
        {
            Destroy(this.gameObject);
        }
    }
}
