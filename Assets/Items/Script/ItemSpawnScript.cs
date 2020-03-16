using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawnScript : MonoBehaviour {
	
	public GameObject item;		//Item to spawn.
	private GameObject[] items;	//array of spawned items.
	public Vector3 area;		//How far from spawn center it is allowed to deviate.
	[Range(0, 100)]
	public int spawn_chance;	

	public int max = 0;	//max number if these items at a time.
	//private int num_items;

	// Use this for initialization
	void Start () {
		//num_items = 0;	//number of items is 0.
		items = new GameObject[max];
		//Debug.Log (items.Length);
	}


	public void Spawn()
	{
		for(int i = 0; i < max; i++) {
			if (items[i] == null && Random.Range (0, 100) < spawn_chance ) {
				
				Vector3 pos = new Vector3 (Random.Range (-10, 10) * area.x, Random.Range (-10, 10) * area.y, Random.Range (-10, 10) * area.z)/20;
				items[i] = Instantiate(item, transform.position + pos, transform.rotation );
			}
		}
	}

	public void DestroyItems()
	{
		for(int i = 0; i < max; i++) {
			if (items[i] != null ) {
				Destroy (items [i]);
			}
		}
	}
}
