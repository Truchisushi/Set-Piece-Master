using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallController : BaseBehaviourScript {


	public ParticleSystem smoke;
	public ParticleSystem explosion;
	public GameObject sparks;
	public float max_charge_up_time = 2.0f;
	public bool isInGoal;
	public int start_lives = 5; //starting lives.

	public Slider powerBar; //Slider for UI.

	public AudioClip hit;
	public AudioClip groundHit;
	public AudioClip postHit;

	private float charge_up_time;
	private Vector3 direction;
	private Vector3 direction_penalty;
	private Vector3 curve;
	private BallPhysics b_physics;
	private Plane m_Plane; //create a plane using the x and y from the camera, with a distance from the ball to the goal.


	private AudioSource ballAudio; //Audio for the ball.

	private int lives; //How many lives are left.
	public int Lives{get{return lives;}}	//Read only on lives.

	//true if has kicked, false if not
	private bool has_kicked; 

	public bool Has_kicked {
		get {return this.has_kicked;}
		set{ this.has_kicked = value; }
	}


	// Use this for initialization
	protected override void Start () {
		//base.Start();
		lives = start_lives; //standard 
		isInGoal = false;
		b_physics = GetComponent<BallPhysics> ();
		ballAudio = GetComponent<AudioSource> ();
	}

	// Use this for initialization
	public override void Init()
	{
		base.Init ();

		isInGoal = false;
		Stop ();
		if (smoke != null)
			smoke.Stop ();
		if (explosion != null)
			explosion.Stop ();

		has_kicked = false;
		curve = Vector3.zero;
		charge_up_time = 0.0f;


		//Initialize camera position:
		Camera.main.transform.rotation = transform.rotation;
		Camera.main.transform.position = transform.position;
		Camera.main.transform.Translate(Vector3.up + 4*Vector3.back);

		//Compute the point in which the plane should be at.
		Vector3 m_DistanceFromBall = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z - transform.position.z);
		//Create a new plane with normal (0,1,0) at the position away from the camera you define in the Inspector. This is the plane that you can click so make sure it is reachable.
		m_Plane = new Plane(transform.TransformDirection(Vector3.forward), m_DistanceFromBall);
	}

	//initialize at a new position.
	public override void Init(Vector3 pos, Quaternion q)
	{
		base.Init (pos, q);
		//lives = start_lives;
	}
		
	//Reset paremeters, position, physics and particle systems.
	public override void Reset()
	{
		base.Reset ();
		isInGoal = false;
		has_kicked = false;
		charge_up_time = 0.0f;
		direction_penalty = Vector3.zero;
		curve = Vector3.zero;

		if(b_physics != null)
			b_physics.ResetPhysics ();
		
		if (smoke != null)
			smoke.Stop ();

		if (explosion != null)
			explosion.Stop ();

	}

	public void ResetLives()
	{
		lives = start_lives;
	}

	//Stop movement of the ball.
	public void Stop()
	{
		if (has_kicked)
			b_physics.ResetPhysics ();

	}

	
	// Update is called once per frame
	void Update () {
		AttemptsManager.attempts = Lives;
		powerBar.value = PowerValue (); //update the value of the power bar


		//Change so that all the player input is handled in GameController.
		//Update kick direction when the player klicks on the button
		if (Input.GetMouseButtonDown (0) && !has_kicked) {
			ComputeDirection ();
		}

		//Power up the shot as long as the button is held down.
		if (Input.GetMouseButton (0) && !has_kicked) {
			if (!UpdatePower (Time.deltaTime))
				PlayerShoot ();
		}

		//When player releases shooting button, kick the ball
		if (Input.GetMouseButtonUp (0) && !has_kicked) {
			PlayerShoot ();
		}
	}



	void ComputeDirection()
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

	bool UpdatePower(float dt)
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

	void PlayerShoot()
	{
		ballAudio.clip = hit;
		ballAudio.volume = charge_up_time / max_charge_up_time;
		ballAudio.Play ();//PLay audio effect.
		//Debug.Log (lives);
		if (b_physics != null)
			b_physics.Shoot (direction + direction_penalty, curve, charge_up_time/max_charge_up_time); //shoot the ball.

		//Activate particle system if it is attached:
		if (smoke != null) {

			//var main = Smoke.main;

			float c = charge_up_time / (10*max_charge_up_time);
			var main = smoke.main;
			main.startColor = new Color (c, c, c, 1.0f);
			smoke.Play ();
			//Edit the initial start color depending on speed.
		}



		has_kicked = true;
	}

	//Compute the PowerValue, normalized between 0 and 1.
	public float PowerValue()
	{
		return charge_up_time / max_charge_up_time;
	}

	public void RemoveLife()
	{
		lives--;
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.tag == "Goal") {
			//EventManager.TriggerEvent ("Goal");
			isInGoal = true;
			if (explosion != null)
				explosion.Play ();
		}

		if (col.tag == "LifeBall")
			lives++; //Add one life.

		//Debug.Log ("Collided inside goal!");
	}

	void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.tag == "GoalFrame") {
			ballAudio.clip = postHit;
			ballAudio.volume = col.impulse.magnitude / 10;
			ballAudio.Play ();
			GameObject tmp = Instantiate (sparks, transform.position, Quaternion.Euler(Vector3.left*45));
			Destroy (tmp, 1);
		}
			//Debug.Log (col.gameObject.tag);
		if (col.gameObject.tag == "Ground") {
			//ballAudio.clip = groundHit;
			//ballAudio.volume = col.impulse.magnitude/5;
			//ballAudio.Play ();//PLay audio effect.
		}
			//Debug.Log (col.gameObject.tag);
	}
}
