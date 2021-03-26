using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField] float appearNextTime;
    [SerializeField] int maxNumOfPlanes;
    [SerializeField] int maxNumOfTanks;
    public GameObject player;
    public GameObject planePrefab;
    public GameObject tankPrefab;
    public GameObject shipPrefab;

    private float delta;
    private int numberOfPlanes;
    private int numberOfTanks;

    //フィールドの動的生成
    public GameObject field1;
    public GameObject field2;
    public GameObject field3;
    private float offsetZ;

    private bool isAppearShip = false;

    void Start()
    {
        this.delta = 0;
        numberOfPlanes = 0;
        numberOfTanks = 0;
        this.player = GameObject.Find("Player");

        this.field1 = GameObject.Find("Field1");
        this.field2 = GameObject.Find("Field2");
        this.field3 = GameObject.Find("Field3");
        this.field1.SetActive(true);
        this.field2.SetActive(false);
        this.field3.SetActive(false);
    }

    void Update()
    {
        if(numberOfPlanes >= maxNumOfPlanes)
        {
            return;
        }
        if(numberOfTanks >= maxNumOfTanks)
        {
            return;
        }
        delta += Time.deltaTime;
        //敵の生成間隔
        if(delta > appearNextTime)
        {
            delta = 0f;
            AppearPlane();
            Invoke("AppearTank", 10);
        }
        if(!isAppearShip)
        {
            isAppearShip = true;
            Invoke("AppearShip", 26);
        }

        //フィールドの動的生成
        Invoke("MakeField2", 5);
        Invoke("MakeField3", 10);
        if(player.transform.position.z > 400)
        {
            Destroy(this.field1);
        }
        if(player.transform.position.z > 800)
        {
            Destroy(this.field2);
        }
        if(player.transform.position.z > 1200)
        {
            Destroy(this.field3);
        }

        void MakeField2()
        {
            this.field2.GetComponent<GameObject>();
        }
        void MakeField3()
        {
            this.field3.SetActive(true);
        }
    }

    //敵の生成（飛行機）
    void AppearPlane()
    {
        //プレイヤー爆発のとき
        if(player == null) return;

        //飛行機の数指定
        for(int i = 0; i <= 1; i++)
        {
            int offsetX = Random.Range(-10, 13);
            int offsetY = Random.Range(-8, 13);
            int offsetZ = Random.Range(250, 350);
            GameObject plane = Instantiate(planePrefab);
            // Debug.Log("敵出現");
            numberOfPlanes++;
            delta = 0f;
            plane.transform.position = new Vector3(offsetX, offsetY, this.player.transform.position.z + offsetZ);
        }
    }
    //敵の生成（タンク）
    void AppearTank()
    {
        //プレイヤー爆発のとき
        if(player == null) return;

        //タンクの数指定
        for(int a = 0; a <= 1; a++)
        {
            int offsetX = Random.Range(-10, 15);
            int offsetY = -15;
            int offsetZ = Random.Range(250, 350);
            GameObject tank = Instantiate(tankPrefab);
            // Debug.Log("タンク出現");
            numberOfTanks++;
            tank.transform.position = new Vector3(offsetX, offsetY, this.player.transform.position.z + offsetZ);
        }
    }
    void AppearShip()
    {
        //プレイヤー爆発のとき
        if(player == null) return;
        int offsetX = 0;
        int offsetY = -15;
        int offsetZ = 300;
        GameObject ship = Instantiate(shipPrefab);
        // Debug.Log("船出現");
        ship.transform.position = new Vector3(offsetX, offsetY, this.player.transform.position.z + offsetZ);
    }
}
