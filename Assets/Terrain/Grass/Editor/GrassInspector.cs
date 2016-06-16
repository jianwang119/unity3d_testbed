using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Terrain.Grass))]
public class GrassInspector : Editor 
{
	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();

		if (GUILayout.Button ("Fill All")) 
		{
			Terrain.Grass g = target as Terrain.Grass;
			if (g != null)
			{
				Terrain.GrassChunkData data = g.burshChunks[0];
				for (int i = 0; i < Terrain.TerrainConst.GRASS_CHUNK_CELL_COUNT; i++)
				{
					data.size[i] = 2;
					data.texPos[i] = (byte)(i > 32*16 ? 1: 2);
				}
			}
		}

		if (GUI.changed) 
		{
			EditorUtility.SetDirty (target);
		}
	}
}
