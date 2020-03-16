using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


//From Messaging System Tutorial from Unity
public class EventManager : MonoBehaviour {
	private Dictionary <string, UnityEvent> eventDictionary;

	private static EventManager eventManager;

	public static EventManager instance
	{
		get
		{
			if (!eventManager) {
				eventManager = FindObjectOfType (typeof(EventManager)) as EventManager;

				if (!eventManager) {
					Debug.LogError ("There needs to be one active Event Manager script on a  GameObject in the scene.");
				}
				else 
				{
					eventManager.Init ();
				}
			} 

			return eventManager;
		}
	}


	void Init(){
		if (eventDictionary == null) {
			eventDictionary = new Dictionary<string, UnityEvent> ();
		}
	}

	public static void StartListening(string eventName, UnityAction listener)
	{
		UnityEvent thisEvent = null;

		if (instance.eventDictionary.TryGetValue (eventName, out thisEvent)) {
			thisEvent.AddListener (listener); //if there exists an event with this name, add a listener to this event
		} else {
			thisEvent = new UnityEvent ();	//if there doesnt exist an entry, create a new event and add it to the dictionary
			thisEvent.AddListener (listener);	//also add the listener to it
			instance.eventDictionary.Add (eventName, thisEvent);
		}

	}

	public static void StopListening(string eventName, UnityAction listener)
	{
		if (eventManager == null)
			return; //if the eventManager doesnt exist or is destroyed dont do anything.
		
		UnityEvent thisEvent = null; //otherwise create empty event
		if (instance.eventDictionary.TryGetValue (eventName, out thisEvent)) { //find the event by trying to find it from the key eventName
			thisEvent.RemoveListener (listener); //Remove the listener from this event.
		}
	}

	public static void TriggerEvent(string eventName){
		UnityEvent thisEvent = null;
		if (instance.eventDictionary.TryGetValue (eventName, out thisEvent)) {
			thisEvent.Invoke (); //trigger this event, invoke/call all functions that are listening to this event.
		}
	}
		
}
