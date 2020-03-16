using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DistanceTextManager : MonoBehaviour {

	Text text;
	static int distance;
	// Use this for initialization
	void Awake()
	{
		text = GetComponent<Text> ();
	}

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		text.text = distance + "m";
	}

	public static IEnumerator ComputeDistance(float d){
		distance = 0;
		//Debug.Log(tmp);
		while(distance <= d )
		{

			distance++;
			yield return new WaitForSeconds(0);
		}
	}
}
