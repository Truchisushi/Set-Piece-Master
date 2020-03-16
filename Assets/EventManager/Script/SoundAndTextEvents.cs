using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SoundAndTextEvents : MonoBehaviour {



	private UnityAction goalListener;
	private UnityAction resetListener;
	private UnityAction closeOneListener;
	private UnityAction missListener;
	private UnityAction graveMissListener;
	private UnityAction gameOverListener;
	private AudioSource sound;

	public GameObject goalText;
	public GameObject closeOneText;
	public GameObject missText;
	public GameObject graveMissText;
	public GameObject gameOverText;
	public AudioClip goalClip;
	//public Collider goalZone;
	// Use this for initialization
	void Start () {
		sound = GetComponent<AudioSource> ();
		if (goalText != null) {
			goalText.SetActive (false);
		}
		if (closeOneText != null) {
			closeOneText.SetActive(false);

		}
		if (missText != null) {
			missText.SetActive(false);

		}
		if (graveMissText != null) {
			graveMissText.SetActive(false);

		}

		if (gameOverText != null) {
			gameOverText.SetActive (false);
		}

	}

	void Awake()
	{
		goalListener = new UnityAction (Goal);
		resetListener = new UnityAction (Reset);
		closeOneListener = new UnityAction (CloseOne);
		missListener = new UnityAction (Miss);
		graveMissListener = new UnityAction (GraveMiss);
		gameOverListener = new UnityAction (GameOver);
	}

	void OnEnable()
	{
		EventManager.StartListening ("Goal", goalListener);
		EventManager.StartListening ("Reset", resetListener);
		EventManager.StartListening ("CloseOne", closeOneListener);
		EventManager.StartListening ("Miss", missListener);
		EventManager.StartListening ("GraveMiss", graveMissListener);
		EventManager.StartListening ("GameOver", gameOverListener);
	}

	void OnDisable(){
		EventManager.StopListening ("Goal", goalListener);
		EventManager.StopListening ("Reset", resetListener);
		EventManager.StopListening ("CloseOne", closeOneListener);
		EventManager.StopListening ("GraveMiss", graveMissListener);
		EventManager.StopListening ("GameOver", gameOverListener);
	}

	void CloseOne()
	{
		if(closeOneText != null)
			closeOneText.SetActive (true);
		//Debug.Log ("Close One!");
	}

	void Miss()
	{
		if(missText != null)
			missText.SetActive (true);
		//Debug.Log ("Missed!");

	}

	void GraveMiss()
	{
		if(graveMissText != null)
			graveMissText.SetActive (true);
		//Debug.Log ("Ouch!");

	}


	void Goal()
	{
		//Debug.Log ("Goal!");
		//Code for when a goal is made:
		if (sound.isPlaying) {
			//Debug.Log ("PLaying something in audio!" + sound.time);
			//audio.PlayScheduled (0.5);
			sound.time = 4/sound.time ;
		} else {
			sound.clip = goalClip;
			sound.Play ();
		}
		if (goalText != null) {
			goalText.SetActive(true);
		}
		//goalText.enabled = true;				
	}

	void GameOver()
	{
		//Debug.Log ("Game Over!");
		if (gameOverText != null) {
			gameOverText.SetActive (true);
		}
	}

	void Reset()
	{
		//Code for what the reset Goal is:
		if ( goalText != null && goalText.activeSelf) {
			//goalText.SetTime (0.0f);
			goalText.SetActive(false);
			//GC.Spawn ();
		}

		if (closeOneText != null && closeOneText.activeSelf) {
			closeOneText.SetActive(false);
		}
		if (missText != null && missText.activeSelf) {
			missText.SetActive(false);

		}
		if (graveMissText != null && graveMissText.activeSelf) {
			graveMissText.SetActive(false);
		}

		if (gameOverText != null && gameOverText.activeSelf) {
			gameOverText.SetActive (false);
		}
	}
}

