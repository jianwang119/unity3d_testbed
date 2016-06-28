using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Terrain
{
	[System.Serializable]
	public class GrassTileData
	{
		public int posX;
		public int posZ;
		public int seed;
		public ushort[] size = new ushort[Utils.GRASS_TILE_CELL_COUNT];
		public byte[] texture = new byte[Utils.GRASS_TILE_CELL_COUNT];
		public int[] textureIndeics;
	}

	public class GrassTileCell
	{
		public int vertStart;
		public int waveSpeedStart;
		public float waveSpeed;
		public float waveSpeedStep;
		public float turbulenceScale;
		public float turbulenceScaleFactor;
		public bool isTurbulence;
	}

	public class GrassTile : MonoBehaviour 
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
		public List<GrassTileCell> cellList;

		[HideInInspector]
		public GrassTileCell[,] cellMap;

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
					GrassTileCell cell = cellList[i];

					if (cell.isTurbulence || cell.turbulenceScale > 1)
					{
						updateMesh = true;

						float n = (!cell.isTurbulence) ? cell.turbulenceScale : cell.turbulenceScale * cell.turbulenceScaleFactor;
						cell.waveSpeed += cell.waveSpeedStep * ((n - 1) / 2 + 1);

						uv3[cell.vertStart + 0].x = n;
						uv3[cell.vertStart + 0].y = cell.waveSpeed - ((float)cell.waveSpeedStart + cell.waveSpeedStep * (float)updateFrame);
						uv3[cell.vertStart + 1].x = uv3[cell.vertStart + 0].x;
						uv3[cell.vertStart + 1].y = uv3[cell.vertStart + 0].y;

						if (cell.isTurbulence)
						{
							cell.turbulenceScaleFactor += 0.15f;
							if (cell.turbulenceScaleFactor > 1)
							{
								cell.turbulenceScaleFactor = 1;
								cell.isTurbulence = false;
							}
						}
						else
						{
							cell.turbulenceScale = n > 1 ? n * 0.95f : 1;
							cell.turbulenceScaleFactor *= 0.95f;
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

		public void AddTurbulence(Vector3 pos, float radius, float scale)
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
			if (x1 > Utils.TERRAIN_TILE_SIZE || x2 < 0f || z1 > Utils.TERRAIN_TILE_SIZE || z2 < 0f) 
			{
				return;
			}

            int cx1 = Utils.PosToGrid(x1, Utils.GRASS_TILE_CELL_SIZE, Utils.GRASS_TILE_CELL_DIM);
            int cx2 = Utils.PosToGrid(x2, Utils.GRASS_TILE_CELL_SIZE, Utils.GRASS_TILE_CELL_DIM);
            int cz1 = Utils.PosToGrid(z1, Utils.GRASS_TILE_CELL_SIZE, Utils.GRASS_TILE_CELL_DIM);
            int cz2 = Utils.PosToGrid(z2, Utils.GRASS_TILE_CELL_SIZE, Utils.GRASS_TILE_CELL_DIM);

			if (scale < 0)
				scale = 0;

			if (scale > 1f)
				radius *= scale;

			for (int i = cz1; i <= cz2; i++)
			{
				for (int j = cx1; j <= cx2; j++)
				{
					GrassTileCell cell = cellMap[j, i];

					if (cell != null)
					{
                        float cx = Utils.GridToPos(j, Utils.GRASS_TILE_CELL_SIZE) + Utils.GRASS_TILE_CELL_HALF_SIZE;
                        float cz = Utils.GridToPos(i, Utils.GRASS_TILE_CELL_SIZE) + Utils.GRASS_TILE_CELL_HALF_SIZE;

						float dis = Mathf.Sqrt((cx - posx) * (cx - posx) + (cz - posz) * (cz - posz));
						if (dis > radius)
						{
                            continue;
                        }

                        float f = (1f - dis / radius) * 8f + 1f;
                        f *= scale;
                        if (cell.turbulenceScale < f)
                        {
                            cell.turbulenceScale = f;
                            float ff = Mathf.Acos((cx - posx) / dis);
                            if (cz < posz)
                            {
                                ff = 2 * Mathf.PI - ff;
                            }
                            cell.waveSpeed = 256 * ff / (2 * Mathf.PI);
                            if (cell.turbulenceScaleFactor < 0.1f)
                            {
                                cell.turbulenceScaleFactor = 0.1f;
                            }
                            cell.isTurbulence = true;
                            isDirty = true;
                        }
					}
				}
			}
		}

		void OnDrawGizmos()
		{
			if (isVisible && UnityEditor.Selection.activeGameObject == gameObject)
			{
				Gizmos.matrix = transform.localToWorldMatrix;
				Gizmos.color = Color.grey;

                Vector3 size = new Vector3(Utils.TERRAIN_TILE_SIZE, 2, Utils.TERRAIN_TILE_SIZE);
                Vector3 center = size / 2;
				Gizmos.DrawWireCube(center, size);

                Vector3 csize = new Vector3(Utils.GRASS_TILE_CELL_SIZE, 1.5f, Utils.GRASS_TILE_CELL_SIZE);
                for (int i = 0; i < Utils.GRASS_TILE_CELL_DIM; i++)
				{
                    for (int j = 0; j < Utils.GRASS_TILE_CELL_DIM; j++)
					{
						GrassTileCell cell = cellMap[i, j];
						
						if (cell != null)
						{
                            Vector3 ccenter = new Vector3(i * Utils.GRASS_TILE_CELL_SIZE, 0, j * Utils.GRASS_TILE_CELL_SIZE) + csize / 2;
							Gizmos.DrawWireCube(ccenter, csize);
						}
					}
				}

				Gizmos.color = Color.clear;
			}
		}
	}
}
