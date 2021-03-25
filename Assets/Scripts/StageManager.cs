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

    public GameObject fields;
    public GameObject field1;
    public GameObject field2;
    public GameObject field3;
    public GameObject field4;
    private float offsetZ;

    private bool isAppearShip = false;

    void Start()
    {
        this.delta = 0;
        numberOfPlanes = 0;
        numberOfTanks = 0;
        this.player = GameObject.Find("Player");
        this.fields = GameObject.Find("Fields");
        this.field1 = GameObject.Find("Field1");
        this.field2 = GameObject.Find("Field2");
        this.field3 = GameObject.Find("Field3");
        this.field4 = GameObject.Find("Field4");
        // this.fields.SetActive(false);
        // this.field1.SetActive(false);
        // this.field2.SetActive(false);
        // this.field3.SetActive(false);
        // this.field4.SetActive(false);
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
            Invoke("AppearShip", 20);
        }
        //フィールドの動的生成
        // if(player.transform.position.z > -200)
        // {
        //     this.field1.SetActive(true);
        //     Debug.Log("フィールド1生成");
        // }
        // if(player.transform.position.z > 200)
        // {
        //     this.field2.SetActive(true);
        //     Debug.Log("フィールド2生成");
        // }
        // if(player.transform.position.z > 600)
        // {
        //     this.field1.SetActive(true);
        //     Debug.Log("フィールド3生成");
        // }
        // if(player.transform.position.z > 1000)
        // {
        //     this.field1.SetActive(true);
        //     Debug.Log("フィールド4生成");
        // }

        void MakeField()
        {
            float distance = Vector3.Distance(player.transform.position, field1.transform.position);
            print(distance);
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
            Debug.Log("タンク出現");
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
        Debug.Log("船出現");
        ship.transform.position = new Vector3(offsetX, offsetY, this.player.transform.position.z + offsetZ);
    }
}
