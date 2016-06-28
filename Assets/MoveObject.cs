using UnityEngine;
using System.Collections;

public class MoveObject : MonoBehaviour 
{
	public float radius = 1.0f;
	public float strength = 1.0f;

    private Vector3 lastPos = Vector3.zero;

	void Awake()
	{
		transform.localScale = new Vector3 (radius * 2, 1, radius * 2);
	}

	void Update () 
	{
		if (Terrain.Grass.Inst != null) 
		{
            Vector3 curPos = transform.position;

            if ((lastPos - curPos).magnitude > 0.5f)
            {
                Terrain.Grass.Inst.AddTurbulence(curPos, radius, strength);
                lastPos = curPos;
            }
		}
	}
}
