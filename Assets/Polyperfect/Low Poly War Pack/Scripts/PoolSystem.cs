using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PolyPerfect
{
    namespace War
    {
        public class PoolSystem : MonoBehaviour
        {
            [System.Serializable]
            public class Pool
            {
                public GameObject prefab;
                public int size;
                public Pool()
                {

                }
                public Pool(string _name, GameObject _prefab, int _size)
                {
                    prefab = _prefab;
                    size = _size;
                }
            }
            public List<Pool> pools;
            public GameObject audioProp;
            public Dictionary<string, Queue<GameObject>> poolDictionary;

            #region Singleton
            public static PoolSystem Instance
            {
                get
                {
                    if (s_Instance == null)
                    {
                        GameObject pool = GameObject.Find("PoolSystem");
                        if (pool == null)
                        {
                            pool = new GameObject("PoolSystem");
                            pool.AddComponent<PoolSystem>();
                        }
                        else
                            if (pool.GetComponent<PoolSystem>() == null)
                            pool.AddComponent<PoolSystem>();
                        s_Instance = pool.GetComponent<PoolSystem>();
                    }
                    return s_Instance;
                }
            }
            static PoolSystem s_Instance;
            #endregion

            private void Start()
            {
                poolDictionary = new Dictionary<string, Queue<GameObject>>();
                pools.Add(new Pool {prefab = audioProp,size = 64 });
                foreach (Pool pool in pools)
                {
                    Queue<GameObject> queue = new Queue<GameObject>();
                    for (int i = 0; i < pool.size; i++)
                    {
                        GameObject obj = Instantiate(pool.prefab);
                        obj.SetActive(false);
                        queue.Enqueue(obj);
                    }
                    poolDictionary.Add(pool.prefab.name, queue);
                }
            }
            public GameObject Spawn(GameObject targetObject, Vector3 pos, Quaternion rotation)
            {
                if (!poolDictionary.ContainsKey(targetObject.name))
                {
                    SetUpPool(targetObject);
                    Debug.LogWarning("Pool whith name " + targetObject.name + " doesn´t exist, create it in editor to prevent lags");
                    //return ;
                }
                GameObject obj;
                if (poolDictionary[targetObject.name].Count == 0)
                {
                    Debug.LogWarning("Pool whith name " + targetObject.name + " has empty queue, try increase number of spawned objects");
                    obj = Instantiate(targetObject);
                }
                else
                    obj = poolDictionary[targetObject.name].Dequeue();
                obj.transform.position = pos;
                obj.transform.rotation = rotation;
                obj.SetActive(true);
                IPoolSystem pooledObj = obj.GetComponent<IPoolSystem>();
                if (pooledObj != null)
                    pooledObj.OnPoolSpawn();
                return obj;
            }
            public void PlaySound(AudioClip clip, Vector3 pos, Quaternion rotation)
            {
                GameObject obj;
                if (poolDictionary["AudioProp"].Count == 0)
                {
                    Debug.LogWarning("Pool whith name AudioProp has empty queue, try increase number of spawned objects");
                    obj = Instantiate(audioProp);
                }
                else
                    obj = poolDictionary["AudioProp"].Dequeue();
                obj.GetComponent<AudioSource>().clip = clip;
                obj.transform.position = pos;
                obj.transform.rotation = rotation;
                obj.SetActive(true);
                IPoolSystem pooledObj = obj.GetComponent<IPoolSystem>();
                if (pooledObj != null)
                    pooledObj.OnPoolSpawn();
            }
            public void SetUpPool(GameObject objToAdd)
            {
                Queue<GameObject> queue = new Queue<GameObject>();
                for (int i = 0; i < 16; i++)
                {
                    GameObject obj = Instantiate(objToAdd);
                    obj.SetActive(false);
                    queue.Enqueue(obj);
                }
                poolDictionary.Add(objToAdd.name, queue);
                pools.Add(new Pool(objToAdd.name, objToAdd, 16));
            }
        }
    }
}