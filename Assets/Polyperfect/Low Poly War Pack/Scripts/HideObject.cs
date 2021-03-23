using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PolyPerfect
{
	namespace War
	{
		public class HideObject : MonoBehaviour, IPoolSystem
		{

			public float timer;
			public string objName;
			// Use this for initialization
			public void OnPoolSpawn()
			{
				StartCoroutine("DestroyTimer", timer);
			}
			IEnumerator DestroyTimer(float seconds)
			{
				yield return new WaitForSeconds(seconds);
				transform.parent = null;
				PoolSystem.Instance.poolDictionary[objName].Enqueue(this.gameObject);
				gameObject.SetActive(false);
			}
		}
	}
}