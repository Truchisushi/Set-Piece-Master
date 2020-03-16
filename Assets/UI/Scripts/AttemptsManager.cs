using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttemptsManager : MonoBehaviour {

	public static int attempts;
	Text text;

	void Awake()
	{
		text = GetComponent<Text> ();
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		text.text = "A T T E M P T S x  " + attempts;
	}
}
