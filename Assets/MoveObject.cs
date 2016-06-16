using UnityEngine;
using System.Collections;

public class MoveObject : MonoBehaviour 
{
	public float radius = 1.0f;
	public float strength = 1.0f;

	void Awake()
	{
		transform.localScale = new Vector3 (radius * 2, 1, radius * 2);
	}

	void Update () 
	{
		if (Terrain.Grass.Inst != null) 
		{
			Terrain.Grass.Inst.Disturb(transform.position, radius, strength);
		}
	}
}
