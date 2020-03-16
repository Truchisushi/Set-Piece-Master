using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq; //Used to find objects that have implemented a certain interface: https://answers.unity.com/questions/863509/how-can-i-find-all-objects-that-have-a-script-that.html


public class GameController : MonoBehaviour {


	public BallController ball;

	//public BaseBehaviourScript DefensiveWall;

	//public GKController GK;

	public BaseBehaviourScript[] objs;

	public ItemSpawnScript[] itemSpawns;

	//public Text attemptsText;

	//public Text distanceText;



	//private bool isResetActive;
	private bool isBallInGame; //if the ball is still in the game or not. 
	private bool isRestartReady;
	//private List<IReset> objs = new List<IReset>();



	private float t;

	void Awake()
	{
		
	}

	void Start()
	{
		isRestartReady = false;
		var resetables = FindObjectsOfType<MonoBehaviour> ().OfType<IReset> ();
		//foreach (IReset resetable in resetables) {
		//	objs.Add (resetable);
		//}
		//attempts = 5;
		Spawn ();
		//t = 0.0f;

	}


	void Update () {
		//attemptsText.text = attempt_string + ball.Lives;

		if (Input.GetKey ("escape"))
			Application.Quit ();

				
		if ((Input.GetKeyDown ("space") || Input.GetKeyDown ("r")) && isRestartReady) {
				//EventManager.TriggerEvent ("Reset");
			Restart ();
		}
	}

	void FixedUpdate() {
		if (ball.Has_kicked && isBallInGame) {
			//GK.React ();
			//distanceText.text = ball.transform.position.magnitude + "m";
			CheckState ();
			t += Time.deltaTime;
		} 
	}

	//Called at the beginning of each game or after you've scored to place the ball randomly and defensive wall in an appropriate position.
	public void Spawn()
	{
		//EventManager.TriggerEvent ("Reset");
		t = 0.0f;
		//attempts = 5;

		//Ball initialization here These are some nasty dependencies that need to be get rid of.
		isBallInGame = true;
		//Compute new position and rotations for ball:
		Vector3 new_pos = new Vector3 (Random.Range (-18.0f, 18.0f), 0.12f, Random.Range (-16.5f, -33.0f));
		Vector3 rot = new Vector3 (0.0f, Mathf.Atan (new_pos.x / new_pos.z) * 180 / Mathf.PI, 0.0f);	//The ball should be facing the center of the goal. Always.
		Quaternion q = Quaternion.Euler(rot);


		foreach (BaseBehaviourScript obj in objs)
			obj.Init (new_pos, q);

		StartCoroutine (DistanceTextManager.ComputeDistance(ball.transform.position.magnitude));

		//Spawn new items
		if (itemSpawns != null) {
			foreach (ItemSpawnScript itemSpawn in itemSpawns) {
				itemSpawn.Spawn ();
			}
		}
			
		//WindScript.GenWind ();


	}

	//Check the current state of the game. 
	void CheckState()
	{
		if (ball.isInGoal) { // Is ball in goal
				//attempts++; //add extra attempt
				isBallInGame = false;
				StartCoroutine (TriggerEventAndRetry ("Goal", 2));
				StartCoroutine (ScoreTextManager.ComputeScore (10));
			} 

			//Has ball missed?
			else if (ball.transform.position.z > 0.5) {
				//close one
				if (Mathf.Abs (ball.transform.position.x) < 5 && ball.transform.position.y < 3) {
					//Debug.Log ("Close One!");
					StartCoroutine (TriggerEventAndRetry ("CloseOne", 1));
				}
					

				//Killed a bird with that one
				else if (Mathf.Abs (ball.transform.position.y) > 6) {
					//Debug.Log ("Killed a bird with that one");
					StartCoroutine(TriggerEventAndRetry("GraveMiss", 1.5f));
				}
				//That ball is going way out west
				else if (Mathf.Abs (ball.transform.position.x) > 10) {
					//Debug.Log ("That ball is going way out west");
					StartCoroutine(TriggerEventAndRetry("GraveMiss", 1.5f));
				}
				//Average miss
				else {
					//Debug.Log ("Missed!");
					StartCoroutine(TriggerEventAndRetry("Miss", 1));
				}
				
				
				isBallInGame = false;
				//isResetActive = true;
			}
			//Poor shot
			else if (t > 3 ) {
				//Debug.Log ("Poor Attempt!");
				StartCoroutine(TriggerEventAndRetry("GraveMiss", 1.5f));
				ball.Stop ();
				isBallInGame = false;
				//isResetActive = true;
			}

	}

	void Retry(){
		EventManager.TriggerEvent ("Reset");
		//attempts--;
		ball.RemoveLife ();

		t = 0.0f;
		isBallInGame = true;

		foreach (BaseBehaviourScript obj in objs)
			obj.Reset();

		if (itemSpawns != null) {
			foreach (ItemSpawnScript itemSpawn in itemSpawns) {
				itemSpawn.Spawn ();
			}
		}
	}

	void Restart()
	{
		EventManager.TriggerEvent ("Reset");
		ball.enabled = true;
		isRestartReady = false;
		ScoreTextManager.score = 0;
		//attempts = 5;
		if (itemSpawns != null) {
			foreach (ItemSpawnScript itemSpawn in itemSpawns) {
				itemSpawn.DestroyItems ();
			}
		}
		ball.ResetLives ();
		Spawn ();


	}

	IEnumerator TriggerEventAndRetry(string e, float s)
	{

		EventManager.TriggerEvent (e);
		yield return new WaitForSecondsRealtime(s);
		if(e == "GameOver")
			isRestartReady = true;
		else {
			EventManager.TriggerEvent ("Reset");
			if (e == "Goal")
				Spawn ();
			else if(ball.Lives > 0)
				Retry ();
			else
				StartCoroutine (TriggerEventAndRetry ("GameOver", 3)); //Game over!
		}


	}



}
