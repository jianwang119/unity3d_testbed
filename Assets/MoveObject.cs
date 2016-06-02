using UnityEngine;
using System.Collections;

public class MoveObject : MonoBehaviour 
{
	void Update () 
	{
		if (Terrain.Grass.Inst != null) 
		{
			Terrain.Grass.Inst.Disturb(transform.position, 1f, 1f);
		}
	}
}
