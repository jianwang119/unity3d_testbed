using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Terrain
{
	[System.Serializable]
	public class GrassChunkData
	{
		public int posX;
		public int posZ;
		public int seed;
		public ushort[] size = new ushort[TerrainConst.GRASS_CHUNK_CELL_COUNT];
		public byte[] info = new byte[TerrainConst.GRASS_CHUNK_CELL_COUNT]; // density(8) | texPos(8)
		public int[] textureIndeics;
	}

	public class GrassChunkCell
	{

	}

	public class GrassChunk : MonoBehaviour 
	{
		[HideInInspector]
		public Vector3 basePos;
				
		[HideInInspector]
		public MeshRenderer meshRenderer;
		
		[HideInInspector]
		public MeshFilter meshFilter;
		
		[HideInInspector]
		public Vector2[] uv3;

		private bool isDirty = false;
		private bool isVisible;
		
		public void Init()
		{
			isVisible = meshRenderer.isVisible;
		}
		
		private void OnBecameVisible()
		{
			isVisible = true;
		}
		
		private void OnBecameInvisible()
		{
			isVisible = false;
		}

		public void OnUpdate(int updateFrame)
		{
			if (!isVisible) 
			{
				return;
			}

			if (isDirty)
			{
				bool updateMesh = false;

				//

				if (updateMesh)
				{
					meshFilter.sharedMesh.uv3 = uv3;
				}
				else
				{
					isDirty = false;
				}
			}
		}

		public void Disturb(Vector3 pos, float radius, float strength)
		{
			if (!gameObject.activeSelf)
			{
				return;
			}

			float posx = pos.x - basePos.x;
			float posz = pos.z - basePos.z;

			float x1 = posx - radius;
			float x2 = posx + radius;

			float z1 = posz - radius;
			float z2 = posz + radius;

			// out of bounds
			if (x1 > TerrainConst.TERRAIN_CHUNK_SIZE || x2 < 0f || 
				z1 > TerrainConst.TERRAIN_CHUNK_SIZE || z2 < 0f) 
			{
				return;
			}

			int cx1 = Mathf.Clamp(Mathf.CeilToInt(x1 / TerrainConst.GRASS_CHUNK_CELL_SIZE), 0, TerrainConst.GRASS_CHUNK_CELL_DIM - 1);
			int cx2 = Mathf.Clamp(Mathf.FloorToInt(x2 / TerrainConst.GRASS_CHUNK_CELL_SIZE), 0, TerrainConst.GRASS_CHUNK_CELL_DIM - 1);
			int cz1 = Mathf.Clamp(Mathf.CeilToInt(z1 / TerrainConst.GRASS_CHUNK_CELL_SIZE), 0, TerrainConst.GRASS_CHUNK_CELL_DIM - 1);
			int cz2 = Mathf.Clamp(Mathf.FloorToInt(z2 / TerrainConst.GRASS_CHUNK_CELL_SIZE), 0, TerrainConst.GRASS_CHUNK_CELL_DIM - 1);

			Debug.Log (cx1 + " to " + cx2 + ", " + cz1 + " to " + cz2);

			if (strength < 0f)
			{
				strength = 0f;
			}

			float disRef = 1.75f;
			if (strength > 1f)
			{
				disRef *= strength;
			}

			float cx = (float)cx1 * TerrainConst.GRASS_CHUNK_CELL_SIZE;
			float cz = (float)cz1 * TerrainConst.GRASS_CHUNK_CELL_SIZE;

			for (int i = cz1; i <= cz2; i++)
			{
				for (int j = cx1; j <= cx2; j++)
				{
					//CGrassFast.CState cState = this.m_StateTable[i, j];
					//if (cState != null)
					{
						float dis = Mathf.Sqrt((cx - posx) * (cx - posx) + (cz - posz) * (cz - posz));
						if (dis < disRef)
						{
							float num13 = (1f - dis / disRef) * 7f + 1f;
							num13 *= strength;
							//if (cState.m_Touch < num13)
							{
								//cState.m_Touch = num13;
								float num14 = Mathf.Acos((cx - posx) / dis);
								if (cz < posz)
								{
									num14 = 2*Mathf.PI- num14;
								}
								//cState.m_State = num14 / (2*Mathf.PI) * 256f;
								//cState.m_TouchBegin = true;
								//if (cState.m_TouchFactor < 0.1f)
								//{
								//		cState.m_TouchFactor = 0.1f;
								//}
								isDirty = true;
							}
						}
					}
					cx += TerrainConst.GRASS_CHUNK_CELL_SIZE;
				}
				cz += TerrainConst.GRASS_CHUNK_CELL_SIZE;
			}
		}

		void OnDrawGizmos()
		{
			if (isVisible)
			{
				Vector3 size = new Vector3(TerrainConst.TERRAIN_CHUNK_SIZE, 5, TerrainConst.TERRAIN_CHUNK_SIZE);
				Vector3 center = transform.position + size / 2;
				Gizmos.matrix = transform.localToWorldMatrix;
				Gizmos.color = (UnityEditor.Selection.activeGameObject == gameObject) ? Color.grey : Color.green;
				Gizmos.DrawWireCube(center, size);
				Gizmos.color = Color.clear;
			}
		}
	}
}
