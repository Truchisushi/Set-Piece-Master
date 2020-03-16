using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Base behaviour for dynamic objects in the scene. Has an initialization and Reset functions
//for resetting and setting transforms.
public class BaseBehaviourScript : MonoBehaviour, IReset {

	protected Vector3 init_pos;
	protected Quaternion init_rot;


	protected virtual void Start()
	{
		Init ();

	}

	//Store initial position
	public virtual void Init()
	{
		init_pos = transform.position;
		init_rot = transform.rotation;
	}

	//initialize at a new position
	public virtual void Init(Vector3 pos, Quaternion rot)
	{
		transform.position = pos;
		transform.rotation = rot;
		Init ();

	}

	public virtual void Reset()
	{
		transform.position = init_pos;
		transform.rotation = init_rot;

	}
	//abstract public void Reset();

}
