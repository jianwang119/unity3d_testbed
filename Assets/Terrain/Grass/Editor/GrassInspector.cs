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
				for (int i = 0; i < g.brushChunks.Length; i++)
				{
					Terrain.GrassChunkData data = g.brushChunks[i];
					for (int j = 0; j < Terrain.TerrainConst.GRASS_CHUNK_CELL_COUNT; j++)
					{
						data.size[j] = (ushort)(Random.Range(2, 4));
						data.texPos[j] = (byte)(j > 32*16 ? 1: 2);
					}
				}
			}
		}

		if (GUI.changed) 
		{
			EditorUtility.SetDirty (target);
		}
	}
}
