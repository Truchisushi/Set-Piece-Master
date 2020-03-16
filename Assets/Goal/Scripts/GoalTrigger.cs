using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class GoalTrigger : MonoBehaviour{
	//public ParticleSystem explosion;

	public GameObject netColliderIn;
	public GameObject netColliderOut;
	//bool isIn = false;

	void Start()
	{
		//Collider to keep ball outside is active
		netColliderIn.SetActive(false);
		netColliderOut.SetActive(true);
	}
		

	void OnTriggerEnter(Collider col)
	{
		if (col.tag == "Player") {


			//set so that the colliders to keep ball inside is active
			netColliderIn.SetActive(true);
			netColliderOut.SetActive(false);

			GetComponent<BoxCollider> ().size = new Vector3 (8.5f, 3.5f, 4.0f);
		}
	}


	void OnTriggerExit(Collider col)
	{
		if (col.tag == "Player") {
			//Collider to keep ball outside is active
			netColliderIn.SetActive (false);
			netColliderOut.SetActive (true);
			GetComponent<BoxCollider> ().size = new Vector3 (7.3f, 2.5f, 0.1f);
		}
	}
		
		
}
