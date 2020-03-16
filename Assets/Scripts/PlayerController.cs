using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseBehaviourScript{
	public Rigidbody rb;
	//Vector3 init_pos;
	//Quaternion init_rot;

	public ParticleSystem Smoke;
	public ParticleSystem explosion;
	float charge_up_time;
	public float max_charge_up_time;
	public float max_curve;
	[Range(0.0f, 1.0f)]
	public float player_curve_multiplier;
	[Range(0.0f, 1.0f)]
	public float C_D;

	//These two are used to create a plane which a raycaster is sent.
	private Plane m_Plane; //create a plane using the x and y from the camera, with a distance from the ball to the goal.
	Vector3 m_DistanceFromBall;

	//Direction and curve the player uses to shoot the ball
	Vector3 direction;
	Vector3 curve;
	Vector3 direction_penalty;
	Vector3 knuckle_ball_force;
	//float shooting_power;

	//true if has kicked, false if not
	private bool has_kicked; 

	public bool Has_kicked {
		get {return this.has_kicked;}
		set{ this.has_kicked = value; }
	}


	protected override void Start () {
		

	}

	// Use this for initialization
	public override void Init()
	{
		base.Init ();

		if (Smoke != null)
			Smoke.Stop ();
		
		has_kicked = false;
		curve = Vector3.zero;
		charge_up_time = 0.0f;
		rb.maxAngularVelocity = max_curve;

		//Initialize camera position:
		Camera.main.transform.rotation = transform.rotation;
		Camera.main.transform.position = transform.position;
		Camera.main.transform.Translate(Vector3.up + 4*Vector3.back);

		//This is how far away from the Camera the plane is placed
		m_DistanceFromBall = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z - transform.position.z);
		//Create a new plane with normal (0,1,0) at the position away from the camera you define in the Inspector. This is the plane that you can click so make sure it is reachable.
		m_Plane = new Plane(transform.TransformDirection(Vector3.forward), m_DistanceFromBall);
	}

	public override void Init(Vector3 pos, Quaternion q)
	{
		base.Init (pos, q);

	}


	// Update is called once per frame
	void Update () {


			//Shoot (); //shoot the ball

	}

	void OnTriggerEnter(Collider col)
	{
		if (col.gameObject.tag == "Goal") {
			EventManager.TriggerEvent ("Goal");
			if(explosion != null)
				explosion.Play ();

		}
			Debug.Log ("Collided inside goal!");
	}

	void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.tag == "Goalframe")
			Debug.Log (col.gameObject.tag);
	}


	//Wind direction, eventual airdrag and Magnus force will be added here:
	void FixedUpdate(){
		if (has_kicked && rb.velocity.sqrMagnitude > 10) {
			//rb.AddForce (new Vector3 (rb.angularVelocity.y, -rb.angularVelocity.x, 0.0f) * 0.1f); //this is just a filler in hack to get the game mechanics done

			//Magnus Force. The model assumes the the form a cylinder.
			float d = 0.223f; 			//diameter of a standard adult football.	(m)
			float p = 1.2041f; 			//density of air at 1atm 20 C degrees. 	(kg/m^3)
			float C = 0.1f*Mathf.PI*d*d;
			float magnus = d * p * C * rb.angularVelocity.magnitude*rb.velocity.magnitude; 			//first part of the magnuseffect, containing the rotational axis and angular velocity.
			Vector3 F_magnus = magnus*Vector3.Cross( rb.angularVelocity, rb.velocity).normalized; 	//Final part of the Magnus effect, containing the direction and velocity. The 
																									//resulting force has the direction of the cross product between angular velocity and velocity.
			//Wind Direction
			//TO BE ADDED
			//Debug.Log("Drag Coefficient and speed (m/s): "+ drag_coefficient() + ", " + rb.velocity.magnitude);

			//Air drag
			Vector3 F_drag = -drag_coefficient()*p*0.4f*Mathf.PI*d*d*rb.velocity.sqrMagnitude*rb.velocity.normalized; //Square air drag. The effective area, drag coefficient and velocity.

			//Noise/wobble effect for knuckle balls. This is only arbitrary and needs some tinkering for good effect.
			Vector3 F_knuckle = Vector3.zero;
			if (rb.angularVelocity.magnitude < 10  && rb.velocity.sqrMagnitude > 100) {
				float mult = Mathf.Min (2.0f, 2.0f / rb.angularVelocity.magnitude);
				F_knuckle = new Vector3 (mult*Mathf.Sin (knuckle_ball_force.x + Time.time*5), 0.3f*mult*Mathf.Sin (knuckle_ball_force.y + Time.time*5), 0.0f);
			}

			//Add the external forces
			rb.AddForce (F_magnus + F_drag + F_knuckle); 
		}
	}

	public void ComputeDirection()
	{
		//Create a ray from the Mouse click position
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		//Initialise the enter variable
		float enter = 0.0f;

		if (m_Plane.Raycast(ray, out enter))
		{
			//Get the point that is clicked
			direction = (ray.GetPoint(enter) - init_pos).normalized;
		}
	}

	public bool UpdatePower(float dt)
	{
		float h = Input.GetAxis("Mouse X");
		float v = Input.GetAxis("Mouse Y");
		curve += new Vector3 (-v, h, 0.0f);
		charge_up_time += dt;
		//IF you power up too much, shoot.
		if (charge_up_time > max_charge_up_time) {
			direction_penalty = 0.2f * (Vector3.up + (new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)))).normalized;
			return false; //otherwise return false
		}
		return true; //return true if could update charge time and curve
	}

	//What should happen when shoot is called:
	public void Shoot(){
		EventManager.TriggerEvent ("Shoot"); //Trigger the shoot event so that AI can react to it.

		//Vector3 dir = new Vector3 (-7.6f, 7.7f, 50.0f);
		rb.AddForce (10*charge_up_time*(direction + direction_penalty), ForceMode.Impulse);

		//Rotate the curve along the direction of the shot:
		curve = Quaternion.Euler(0, Mathf.Atan (direction.x / direction.z) * 180 / Mathf.PI, 0)*curve;

		//Debug.Log(Vector3.Angle(transform.TransformDirection(Vector3.forward), new Vector3(direction.x, 0.0f, direction.z)));
		rb.AddTorque (player_curve_multiplier*curve +  new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), Random.Range(-0.5f,0.5f)));

		//set has kicked to true:
		has_kicked = true;

		//Compute knuckle ball trajectory
		knuckle_ball_force = new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f), 0.0f);

		//Activate particle system if it is attached:
		if (Smoke != null) {
			
			//var main = Smoke.main;
			float c = charge_up_time / (10*max_charge_up_time);
			var main = Smoke.main;
			main.startColor = new Color (c, c, c, 1.0f);
			Smoke.Play ();
			//Edit the initial start color depending on speed.
		}




	}


	//Drag coefficient linearized from results of an experiment of different football's over the years. Source: https://www.ncbi.nlm.nih.gov/pmc/articles/PMC3657093/
	float drag_coefficient(){

		return rb.velocity.magnitude > 16 ? C_D : C_D + 0.7f*(16 - rb.velocity.magnitude)/18;
	}
		
	//Compute the PowerValue, normalized between 0 and 1.
	public float PowerValue()
	{
		return charge_up_time / max_charge_up_time;
	}

	public override void Reset()
	{
		base.Reset ();
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
		has_kicked = false;
		charge_up_time = 0.0f;
		direction_penalty = Vector3.zero;
		curve = Vector3.zero;

		if (Smoke != null)
			Smoke.Stop ();
		if(explosion != null)
			explosion.Stop ();
	}
		
}
