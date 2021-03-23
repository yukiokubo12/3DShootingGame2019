using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneBulletAttachController : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.tag == "PlayerTag")
            {
                Destroy(this.gameObject);
            }
        }
}
