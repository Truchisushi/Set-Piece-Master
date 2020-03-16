using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreTextManager : MonoBehaviour {

	public static int score = 0;
	Text text;
	// Use this for initialization
	void Start () {
		text = GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
		text.text = "S C O R E : " + score;

		//if (Input.GetButtonDown ("Jump"))
		//	StartCoroutine (ComputeScore(10));
	}

	public static IEnumerator ComputeScore(int extraScore){
		//distance = 0;
		int newScore = score+extraScore;
		//Debug.Log(tmp);
		while(score < newScore )
		{
			score++;
			yield return new WaitForSeconds(0);
		}
	}
}
