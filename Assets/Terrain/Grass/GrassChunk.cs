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

		private bool isDirty;
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

		public void Disturb(Vector3 pos, float strength)
		{
//			if (!gameObject.activeSelf)
//			{
//				return;
//			}
//
//			float x = pos.x - centerX;
//			float z = pos.z - centerZ;
//
//			float num = x - 1.75f;
//			float num2 = x + 1.75f;
//			float num3 = z - 1.75f;
//			float num4 = z + 1.75f;
//			if (num > 16f || num2 < 0f || num3 > 16f || num4 < 0f)
//			{
//				return;
//			}
//
//			int num5 = Mathf.Clamp(Mathf.CeilToInt(num / 0.5f), 0, 31);
//			int num6 = Mathf.Clamp(Mathf.FloorToInt(num2 / 0.5f), 0, 31);
//			int num7 = Mathf.Clamp(Mathf.CeilToInt(num3 / 0.5f), 0, 31);
//			int num8 = Mathf.Clamp(Mathf.FloorToInt(num4 / 0.5f), 0, 31);
//			if (strength < 0f)
//			{
//				strength = 0f;
//			}
//			float num9 = 1.75f;
//			if (strength > 1f)
//			{
//				num9 *= strength;
//			}
//			bool flag = false;
//			float num10 = (float)num7 * 0.5f;
//			for (int i = num7; i <= num8; i++)
//			{
//				float num11 = (float)num5 * 0.5f;
//				for (int j = num5; j <= num6; j++)
//				{
//					CGrassFast.CState cState = this.m_StateTable[i, j];
//					if (cState != null)
//					{
//						float num12 = Mathf.Sqrt((num11 - x) * (num11 - x) + (num10 - z) * (num10 - z));
//						if (num12 < num9)
//						{
//							float num13 = (1f - num12 / num9) * 7f + 1f;
//							num13 *= strength;
//							if (cState.m_Touch < num13)
//							{
//								cState.m_Touch = num13;
//								float num14 = Mathf.Acos((num11 - x) / num12);
//								if (num10 < z)
//								{
//									num14 = 6.28318548f - num14;
//								}
//								cState.m_State = num14 / 6.28318548f * 256f;
//								cState.m_TouchBegin = true;
//								if (cState.m_TouchFactor < 0.1f)
//								{
//									cState.m_TouchFactor = 0.1f;
//								}
//								flag = true;
//							}
//						}
//					}
//					num11 += 0.5f;
//				}
//				num10 += 0.5f;
//			}
//			if (flag)
//			{
//				this.m_Dirty = true;
//			}
		}
	}
}
