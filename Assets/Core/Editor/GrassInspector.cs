using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Core.Grass))]
public class GrassInspector : Editor 
{
	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();

		if (GUILayout.Button ("Fill All")) 
		{
            Core.Grass g = target as Core.Grass;
			if (g != null)
			{
				for (int i = 0; i < g.brushTiles.Length; i++)
				{
                    Core.GrassTileData data = g.brushTiles[i];
                    data.seed = Random.Range(0, int.MaxValue);
                    data.size = new ushort[Core.TerrainUtils.GRASS_TILE_CELL_COUNT];
                    data.texture = new byte[Core.TerrainUtils.GRASS_TILE_CELL_COUNT];
                    data.textureIndeics = new int[2];
                    data.textureIndeics[0] = 0;
                    data.textureIndeics[0] = 1;
                    for (int j = 0; j < Core.TerrainUtils.GRASS_TILE_CELL_COUNT; j++)
					{
						data.size[j] = (ushort)(Random.Range(2, 4));
						data.texture[j] = (byte)(j > 32*16 ? 0: 1);
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
