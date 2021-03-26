using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextUIController : MonoBehaviour
{
    public GameObject player;
    public GameObject shipEnemy;
    private GameObject clearText;
    public GameObject retryButton;
    public GameObject titleButton;
    public float currentShipHp;

    void Start()
    {
        this.clearText = GameObject.Find("ClearText");
        this.retryButton = GameObject.Find("ToMainButton");
        this.titleButton = GameObject.Find("ToTitleButton");
    }

    public void OnTriggerEnter(Collider collider)
    {
        if(gameObject.tag == "ShipEnemyTag" && currentShipHp <= 0)
        {
           retryButton.SetActive(true);
           titleButton.SetActive(true);
           this.clearText.GetComponent<Text>().text = "Game Clear!!";
           this.retryButton.GetComponent<Button>();
           this.gameObject.SetActive(false);
        }
    }
}
