using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opponent : BaseBehaviourScript{
	Rigidbody rb;

	//Material mat;
	Color c;

	float prob_jump = 0.5f;
	protected override void Start()
	{
		base.Start();
	}

	void Update()
	{
		//c = new Color (c.r, c.g, c.b, 0.5f + 0.5f * Mathf.Sin (Time.time));
		//mat.color = c;

	}

	public void Jump()
	{
		if(Random.value > prob_jump)
			rb.AddForce (50 * Vector3.up, ForceMode.Impulse);
		//Debug.Log("Defender has hunmped!");

	}

	public override void Init()
	{
		//base.Init ();
		init_pos = transform.localPosition;
		init_rot = transform.localRotation;
		rb = GetComponent<Rigidbody> ();
		//mat = GetComponent<Renderer> ().material;
		//c = mat.color;
	}

	public override void Reset()
	{
		//base.Reset ();
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
		transform.localPosition = init_pos;
		transform.localRotation = init_rot;
	}
}
