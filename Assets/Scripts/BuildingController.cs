using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingController : MonoBehaviour
{ 
    //建物消去
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "PlayerTag")
        {
            Destroy(this.gameObject);
        }
    }
}
