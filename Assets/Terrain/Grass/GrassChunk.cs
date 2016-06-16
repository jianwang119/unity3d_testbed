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
		public byte[] texPos = new byte[TerrainConst.GRASS_CHUNK_CELL_COUNT];
		public int[] textureIndeics;
	}

	public class GrassChunkCell
	{
		public int vertStart;
		public int waveSpeedStart;
		public float waveSpeed;
		public float waveSpeedStep;
		public float strength;
		public float strengthFactor;
		public bool isDisturb;
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

		[HideInInspector]
		public List<GrassChunkCell> cellList;

		[HideInInspector]
		public GrassChunkCell[,] cellMap;

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

				for (int i = 0; i < cellList.Count; i++)
				{
					GrassChunkCell cell = cellList[i];

					if (cell.isDisturb || cell.strength > 1)
					{
						updateMesh = true;

						float n = (!cell.isDisturb) ? cell.strength : cell.strength * cell.strengthFactor;
						cell.waveSpeed += cell.waveSpeedStep * ((n - 1) * 4 / 7 + 1);

						uv3[cell.vertStart + 0].x = n;
						uv3[cell.vertStart + 0].y = cell.waveSpeed - ((float)cell.waveSpeedStart + cell.waveSpeedStep * (float)updateFrame);
						uv3[cell.vertStart + 1].x = uv3[cell.vertStart + 0].x;
						uv3[cell.vertStart + 1].y = uv3[cell.vertStart + 0].y;

						if (cell.isDisturb)
						{
							cell.strengthFactor += 0.15f;
							if (cell.strengthFactor > 1)
							{
								cell.strengthFactor = 1;
								cell.isDisturb = false;
							}
						}
						else
						{
							cell.strength = n > 1 ? n * 0.95f : 1;
							cell.strengthFactor *= 0.95f;
						}
					}
				}

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
			if (x1 > TerrainConst.TERRAIN_CHUNK_SIZE || x2 < 0f || z1 > TerrainConst.TERRAIN_CHUNK_SIZE || z2 < 0f) 
			{
				return;
			}

			int cx1 = Mathf.Clamp(Mathf.FloorToInt(x1 / TerrainConst.GRASS_CHUNK_CELL_SIZE), 0, TerrainConst.GRASS_CHUNK_CELL_DIM - 1);
			int cx2 = Mathf.Clamp(Mathf.FloorToInt(x2 / TerrainConst.GRASS_CHUNK_CELL_SIZE), 0, TerrainConst.GRASS_CHUNK_CELL_DIM - 1);
			int cz1 = Mathf.Clamp(Mathf.FloorToInt(z1 / TerrainConst.GRASS_CHUNK_CELL_SIZE), 0, TerrainConst.GRASS_CHUNK_CELL_DIM - 1);
			int cz2 = Mathf.Clamp(Mathf.FloorToInt(z2 / TerrainConst.GRASS_CHUNK_CELL_SIZE), 0, TerrainConst.GRASS_CHUNK_CELL_DIM - 1);

			if (strength < 0)
				strength = 0;

			if (strength > 1f)
				radius *= strength;

			for (int i = cz1; i <= cz2; i++)
			{
				for (int j = cx1; j <= cx2; j++)
				{
					GrassChunkCell cell = cellMap[j, i];

					if (cell != null)
					{
						float cx = (float)j * TerrainConst.GRASS_CHUNK_CELL_SIZE;
						float cz = (float)i * TerrainConst.GRASS_CHUNK_CELL_SIZE;

						float dis = Mathf.Sqrt((cx - posx) * (cx - posx) + (cz - posz) * (cz - posz));
						if (dis < radius)
						{
							float f = (1f - dis / radius) * 7f + 1f;
							f *= strength;
							if (cell.strength < f)
							{
								cell.strength = f;
								float ff = Mathf.Acos((cx - posx) / dis);
								if (cz < posz)
								{
									ff = 2 * Mathf.PI - ff;
								}
								cell.waveSpeed = 256 * ff / (2 * Mathf.PI);
								cell.isDisturb = true;
								if (cell.strengthFactor < 0.1f)
								{
									cell.strengthFactor = 0.1f;
								}
								isDirty = true;
							}
						}
					}
				}
			}
		}

//		void OnDrawGizmos()
//		{
//			if (isVisible)
//			{
//				Vector3 size = new Vector3(TerrainConst.TERRAIN_CHUNK_SIZE, 2, TerrainConst.TERRAIN_CHUNK_SIZE);
//				Vector3 center = transform.position + size / 2;
//				Gizmos.matrix = transform.localToWorldMatrix;
//				Gizmos.color = (UnityEditor.Selection.activeGameObject == gameObject) ? Color.grey : Color.green;
//				Gizmos.DrawWireCube(center, size);
//
//				Vector3 csize = new Vector3(TerrainConst.GRASS_CHUNK_CELL_SIZE, TerrainConst.GRASS_CHUNK_CELL_SIZE, TerrainConst.GRASS_CHUNK_CELL_SIZE);
//				for (int i = 0; i < TerrainConst.GRASS_CHUNK_CELL_DIM; i++)
//				{
//					for (int j = 0; j < TerrainConst.GRASS_CHUNK_CELL_DIM; j++)
//					{
//						GrassChunkCell cell = cellMap[i, j];
//						
//						if (cell != null)
//						{ 
//							Vector3 ccenter = new Vector3(i * TerrainConst.GRASS_CHUNK_CELL_SIZE, 0, j * TerrainConst.GRASS_CHUNK_CELL_SIZE) + csize / 2;
//							Gizmos.DrawWireCube(ccenter, csize);
//						}
//					}
//				}
//
//				Gizmos.color = Color.clear;
//			}
//		}
	}
}
