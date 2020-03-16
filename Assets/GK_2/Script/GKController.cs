using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GKController : BaseBehaviourScript{

	//Vector3 init_pos;
	//Quaternion init_rot;
	public Transform Ball;
	Vector3 init_ball_pos;
	Vector3 GK_normal;

	bool has_jumped;
	private Rigidbody rb;


	//Best results are speed = 0.01 and w = 0.2
	public float speed; //higher than 1 goes faster less goes slower. For a challenge have it around 0.1
	public float w; //angularfrequency for motion. Roughly translated to the acceleration

	// Use this for initialization
	protected override void Start () {

		rb = GetComponent<Rigidbody> ();
		//react = true;

		//Debug.Log (init_pos);
	}

	public override void Init()
	{
		base.Init ();
		has_jumped = false;

		init_ball_pos = Ball.position;
		GK_normal = new Vector3 (-init_ball_pos.z, 0.0f, init_ball_pos.x).normalized;

	}

	//Initialize position and rotation based on the balls new position and rotation.
	public override void Init(Vector3 pos, Quaternion rot)
	{
		transform.rotation = Quaternion.Euler (Vector3.up*180 + rot.eulerAngles);
		//transform.position = ;
		//transform.Rotate (rot);
		transform.position = Vector3.Project (Vector3.back * 3.5f, pos)  + Vector3.up;
		Init ();
	}
	
	// Update is called once per frame
	void Update () {



	}

	void FixedUpdate()
	{
		//Debug.Log (react);
		if (!has_jumped) {


			//Goalie movement is governed by a critically damped system: x(t) = (A + B*t)*e^(-w*t),
			//where A is the initial displacement at each step, B can be seen as the speed where it reaches x(t_eq) = 0 -> -A/t_eq = B.
			//To compute the rate at which the goalie moves, we differentiate x(t) with respect to t.
			// -> x'(t) = [(A + B*t)*(-w) + B]e^(-w*t).

			float A = Vector3.Project ( Ball.transform.position - init_ball_pos, GK_normal).x;
			//Debug.Log (A);
			float B = -A*speed;

			float x_vel = ((A + B * Time.fixedDeltaTime)*(-w) + B) * Mathf.Exp (-w * Time.fixedDeltaTime);

			transform.Translate(Vector3.right * 1.5f*x_vel );
			transform.position = transform.position + Vector3.up * (0.02f * Mathf.Sin (Time.time*20));

			init_ball_pos = Ball.transform.position; //update ball position for next step.
			Vector3 tmp = init_ball_pos - transform.position;
			GK_normal = new Vector3 (-tmp.z, 0.0f, tmp.x).normalized;

			if ((transform.position - Ball.transform.position).magnitude < 8 && Mathf.Abs(Ball.position.x) < 5 && Mathf.Abs(Ball.position.y) < 8) {
				Jump (Mathf.Sign(x_vel));
			}
		}

	}

	void Jump(float dir)
	{
		has_jumped = true;
		//Add torque to rotate:
		rb.AddRelativeTorque(100*Vector3.back*dir*(Ball.position.y-2));
		rb.AddRelativeForce (2*rb.mass*Vector3.Scale(transform.InverseTransformPoint(Ball.position), Vector3.up+Vector3.right), ForceMode.Impulse);

	}


	public override void Reset()
	{
		base.Reset ();

		has_jumped = false;

		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;

		init_ball_pos = Ball.transform.position;


	}
}
