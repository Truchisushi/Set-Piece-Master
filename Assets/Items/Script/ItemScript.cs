using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemScript : MonoBehaviour {

	public float rotation_rate;		//rotation speed
	public float bobble_rate;		//bobble speed
	public float bobble_distance;	//bobble offset
	public int extra_score;


	public GameObject particles;		//attached particle system to play.
	public GameObject pickup_explosion; //explosion when picked up


	private Vector3 rot_axis;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		transform.Rotate (Vector3.forward* rotation_rate * Time.deltaTime);
		transform.Translate (bobble_distance*Mathf.Cos(Time.time * bobble_rate)*Vector3.up * Time.deltaTime);

		//rot_axis = (rot_axis * rotation_rate).normalized;

		//Debugging test code:
		//if (Input.GetButton ("Jump")) {
		//	PickedUp ();
		//}
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.tag == "Player") {
			PickedUp ();
			ScoreTextManager.score += extra_score;
		}
	}


	void PickedUp()	//Destroy particle.
	{
		if (pickup_explosion != null) {
			GameObject tmp = Instantiate (pickup_explosion, transform.position, transform.rotation);
			tmp.transform.position = transform.position;
			Destroy(tmp, 2);
		}

		if (particles != null) {
			GameObject tmp = Instantiate (particles, transform.position, transform.rotation);
			tmp.transform.position = transform.position;
			Destroy(tmp, 2);
		}
		Destroy(gameObject); //Destroy Item
	}
}
