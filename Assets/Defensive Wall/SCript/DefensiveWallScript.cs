using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefensiveWallScript : BaseBehaviourScript {

	//Keep an empty list of opponents.
	public GameObject Defender;
	private List<Opponent> opponents;

	protected override void Start()
	{
		opponents = new List<Opponent> ();
	}

	public override void Init (Vector3 pos, Quaternion rot)
	{
		if (opponents != null && opponents.Count > 0) {
			foreach (Opponent op in opponents) {
				Destroy (op.gameObject);

			}
			opponents.Clear ();
		}


		this.transform.rotation = rot;
		this.transform.position = pos;




		this.transform.Translate (9.1f * Vector3.forward);
		if (rot.y > 0)
			transform.Translate (Random.Range (0, 2) * Vector3.left);
		else
			transform.Translate (Random.Range (0, 2) * Vector3.right);

		int extra = Random.Range (0, 2);
		int num_opps;
		//Debug.Log (pos.magnitude);
		//Depending on the distance spawn different number of opponents.

		if (pos.magnitude < 22) //18-22m 6-5 men
			num_opps = 5 + extra;

		else if(pos.magnitude <33) //23-26 4-3 men
			num_opps = 4 + extra;
		else //33+m 2-1 men
			num_opps = 1 + extra;

		float wall_length = 0.4f * num_opps + 0.1f * (num_opps - 1); //wall length is the number of opponents and distance between them.

		for (int i = 0; i < num_opps; i++) {
			GameObject tmp = Instantiate (Defender, transform.position + transform.right * (-wall_length / 2 + i * 0.6f) + Vector3.up*0.8f, rot);
			tmp.transform.parent = transform;
			opponents.Add (tmp.GetComponent<Opponent>());
		}


	}


	public override void Reset()
	{
		//Opponent[] ops = GetComponentsInChildren<Opponent> ();
		foreach (Opponent op in opponents) {
			op.Reset ();
		}

	}
}
