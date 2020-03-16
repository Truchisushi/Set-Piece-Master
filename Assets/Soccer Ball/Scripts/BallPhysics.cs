using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPhysics : MonoBehaviour {



	[Range(0.0f, 1.0f)]
	public float C_D = 0.16f; //air drag coefficient close to Reynolds number. (at high speeds)
	[Range(0.0f, 1.0f)]
	public float curve_multiplier = 0.27f;
	public float max_curve = 40.0f;

	//public WindScript wind;

	private Rigidbody rb;
	private Vector3 knuckle_ball_force;
	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
		knuckle_ball_force = Vector3.zero;
		rb.maxAngularVelocity = max_curve;
	}


	void FixedUpdate()
	{
		//Only do this if the speed is more than about 3m/s
		if (rb.velocity.sqrMagnitude > 10) {
			//Magnus Force. The model assumes the the form a cylinder.
			float d = 0.223f; 				//diameter of a standard adult football.	(m)
			float p = 1.2041f; 				//density of air at 1atm 20 C degrees. 	(kg/m^3)
			float C = 0.1f*Mathf.PI*d*d;	
			//Resulting force has the direction of the cross product between angular velocity and velocity.
			//first part of the magnuseffect, containing the rotational axis and angular velocity:
			float magnus = d * p * C * rb.angularVelocity.magnitude*rb.velocity.magnitude; 
			//Final part of the Magnus effect, containing the direction and velocity.
			Vector3 F_magnus = magnus*Vector3.Cross( rb.angularVelocity, rb.velocity).normalized; 	

			//Wind Direction
			Vector3 wind_vel;
			//if(wind != null)
				//Pressure of wind on the ball: Fwind = (Effective area of the ball) x (Density of air) x (velocity of speed)^2. We need to take inte account of the balls velocity as well.
				//so the speed at which the wind is hitting the ball will be (velocity ball) + (velocity air). This can be incorprated into the drag part of the calculations. 
				//What is then needed to take into consideration is that the netto velocity needs to be computed first.
				//wind_vel =  WindScript.wind_strength*WindScript.wind_direction;
			//else 
			wind_vel = Vector3.zero;
			//TO BE ADDED

			//Netto speed of ball and air:
			Vector3 netto_vel = rb.velocity - 0.5f*wind_vel;

			//Air drag
			Vector3 F_drag = -drag_coefficient()*p*0.4f*Mathf.PI*d*d*netto_vel.sqrMagnitude*netto_vel.normalized; //Square air drag. The effective area, drag coefficient and velocity.

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

	public void Shoot(Vector3 direction, Vector3 curve, float power){
		//Vector3 dir = new Vector3 (-7.6f, 7.7f, 50.0f);
		rb.AddForce (20*power*direction, ForceMode.Impulse);

		//Transform 
		curve = Quaternion.Euler(0, Mathf.Atan (direction.x / direction.z) * 180 / Mathf.PI, 0)*curve;

		//Add rotation to the ball
		rb.AddTorque (curve_multiplier*curve +  new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), Random.Range(-0.5f,0.5f)));

		//Compute knuckle ball trajectory
		knuckle_ball_force = new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f), 0.0f);

	}

	public void ResetPhysics()
	{
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
	}


	//Drag coefficient linearized from results of an experiment of different football's over the years. Source: https://www.ncbi.nlm.nih.gov/pmc/articles/PMC3657093/
	float drag_coefficient(){

		return rb.velocity.magnitude > 16 ? C_D : C_D + 0.7f*(16 - rb.velocity.magnitude)/18;
	}
}
