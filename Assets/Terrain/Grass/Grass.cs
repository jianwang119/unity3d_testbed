using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Terrain
{
	public class Grass : MonoBehaviour 
	{
		// TODO data for brush
		public GrassChunkData[] burshChunks;
		public Texture2D[] useTextures;
		public Color[] useColors; 

		public static Grass Inst = null;

		private int frameUpdate = 0;
		private List<GrassChunk> chunkList = new List<GrassChunk>();

		private Texture2D packedTexture;
		private Rect[] packedRect;
		private float[] packedAspectRatio;
		private Material packedMaterial;

		void Awake()
		{
			Grass.Inst = this;
		}

		void OnDestroy()
		{
			ClearGrassChunks();
		}

		void Start () 
		{
			GenerateGrass ();
		}

		void Update()
		{
			frameUpdate++;

			if (packedMaterial != null)
			{
				packedMaterial.SetFloat("_CurrentTime", (float)frameUpdate);
			}

			for (int i = 0; i < chunkList.Count; i++)
			{
				chunkList[i].OnUpdate(frameUpdate);
			}
		}

		void GenerateGrass()
		{
			ClearGrassChunks();

			if (useColors == null && useTextures != null)
			{
				Color clr = new Color(0.5f, 0.5f, 0.5f, 1f);
				useColors = new Color[useTextures.Length];
				for (int i = 0; i < useColors.Length; i++)
				{
					useColors[i] = clr;
				}
			}

			BuildMaterial();

			BuildGrassChunks();
		}

		private void BuildMaterial()
		{
			packedTexture = null;
			packedRect = null;
			packedAspectRatio = null;
			packedMaterial = null;

			int texCount = useTextures.Length;
			if (texCount <= 0) 
			{
				return;
			}
			packedTexture = new Texture2D(16, 16, useTextures[0].format, false);
			if (packedTexture == null)
			{
				return;
			}
			packedRect = packedTexture.PackTextures(useTextures, 2, 512, true);
			if (packedRect == null || packedRect.Length != texCount)
			{
				Object.Destroy(packedTexture);
				packedTexture = null;
				return;
			}
			packedAspectRatio = new float[texCount];
			for (int i = 0; i < texCount; i++)
			{
				packedAspectRatio[i] = (float)useTextures[i].width / (float)useTextures[i].height;
				useTextures[i] = null;
			}

			packedMaterial = new Material(Shader.Find("Terrain/Grass"));
			packedMaterial.mainTexture = packedTexture;
		}

		private void BuildGrassChunks()
		{
			for (int i = 0; i < burshChunks.Length; i++)
			{
				GrassChunkData d = burshChunks[i];
				if (d != null && d.textureIndeics.Length > 0)
				{
					BuildGrassChunk(d);
				}
			}
		}

		private void BuildGrassChunk(GrassChunkData d)
		{
			if (d == null || d.textureIndeics == null)
			{
				return;
			}

			int grassCount = 0;

			for (int i = 0; i < TerrainConst.GRASS_CHUNK_CELL_DIM; i++)
			{
				for (int j = 0; j < TerrainConst.GRASS_CHUNK_CELL_DIM; j++)
				{
					int idx = i * TerrainConst.GRASS_CHUNK_CELL_DIM + j;

					if (d.textureIndeics != null && d.size[idx] > 0 && (d.info[idx] & 0xf) > 0 && (int)(d.info[idx] & 0xf) < d.textureIndeics.Length + 1)
					{
						grassCount += 1 + ((d.info[idx] & 0xf0) >> 4);
					}
				}
			}
			if (grassCount <= 0)
			{
				return;
			}

			Random.seed = d.seed;

			Vector3 chunkPos = new Vector3 ();
			chunkPos.x = (float)d.posX * TerrainConst.TERRAIN_CHUNK_SIZE;
			chunkPos.z = (float)d.posZ * TerrainConst.TERRAIN_CHUNK_SIZE;
			chunkPos.y = GetTerrainHeight(chunkPos.x + TerrainConst.TERRAIN_CHUNK_HALF_SIZE, 
			                              chunkPos.z + TerrainConst.TERRAIN_CHUNK_HALF_SIZE);

			Vector3[] vertices = new Vector3[grassCount * 4];
			Color32[] colors = new Color32[grassCount * 4];
			Vector2[] uvs = new Vector2[grassCount * 4];
			Vector2[] uv3 = new Vector2[grassCount * 4];
			Vector3[] normals = new Vector3[grassCount * 4];
			List<int> triangles = new List<int>();

			//List<CGrassFast.CState> list = new List<CGrassFast.CState>();
			//CGrassFast.CState[,] array8 = new CGrassFast.CState[32, 32];

			int vertIndex = 0;
			int vertStride = 0;
			int grassCounter = 0;
			int grassSkip = 2;

			for (int i = 0; i < d.textureIndeics.Length; i++)
			{
				int texIndex = d.textureIndeics[i];

				Texture2D tex = null;
				Rect rect = default(Rect);
				float aspect = 1f;
				GetTexture(texIndex, ref tex, ref rect, ref aspect);

				for (int l = 0; l < TerrainConst.GRASS_CHUNK_CELL_DIM; l++)
				{
					for (int m = 0; m < TerrainConst.GRASS_CHUNK_CELL_DIM; m++)
					{
						int idx = l * TerrainConst.GRASS_CHUNK_CELL_DIM + m;

						int texPos = (int)(d.info[idx] & 0xf) - 1;
						int density = (int)(d.info[idx] & 0xf0) >> 4;
						int size = (int)d.size[idx];

						if (texPos != i || d.size[idx] <= 0) continue;
						if (grassCounter++ % grassSkip != 0) continue;

						float gx = (float)m * TerrainConst.GRASS_CHUNK_CELL_SIZE + Random.Range(0f, 0.26f);
						float gz = (float)l * TerrainConst.GRASS_CHUNK_CELL_SIZE + Random.Range(0f, 0.26f);
						float gy = GetTerrainHeight(gx + chunkPos.x, gz + chunkPos.z) - chunkPos.y;
						int rotation = Random.Range(0, 63);
						int state = Random.Range(0, 255);

						float sx = gx;
						float sz = gz;
						float r = Mathf.Deg2Rad * ((float)rotation / 64f * -60f + -30f);
						float s = (float)size;
						float hw = aspect / 2f;

						int grassPieceIndex = vertStride;
						for (int n = 0; n < 1 + density; n++)
						{
							float hwsinr = hw * Mathf.Sin(r);
							float hwcosr = hw * Mathf.Cos(r);
							
							vertices[vertIndex + 0].x = -hwcosr * s + sx;
							vertices[vertIndex + 0].y =  1 * s + gy;
							vertices[vertIndex + 0].z = -hwsinr * s + sz;
							vertices[vertIndex + 1].x =  hwcosr * s + sx;
							vertices[vertIndex + 1].y =  1 * s + gy;
							vertices[vertIndex + 1].z =  hwsinr * s + sz;
							vertices[vertIndex + 2].x = -hwcosr * s + sx;
							vertices[vertIndex + 2].y =  0 * s + gy;
							vertices[vertIndex + 2].z = -hwsinr * s + sz;
							vertices[vertIndex + 3].x =  hwcosr * s + sx;
							vertices[vertIndex + 3].y =  0 * s + gy;
							vertices[vertIndex + 3].z =  hwsinr * s + sz;
							
							r = Mathf.Deg2Rad * (Random.Range(0, 360f) * -60f + -30f);
							sx = gx + Random.Range(-1f, 1f) * hw * s;
							sz = gz + Random.Range(-1f, 1f) * hw * s;

							uvs[vertIndex + 0].x = rect.xMin;
							uvs[vertIndex + 0].y = rect.yMax - 0.01f;
							uvs[vertIndex + 1].x = rect.xMax;
							uvs[vertIndex + 1].y = rect.yMax - 0.01f;
							uvs[vertIndex + 2].x = rect.xMin;
							uvs[vertIndex + 2].y = rect.yMin;
							uvs[vertIndex + 3].x = rect.xMax;
							uvs[vertIndex + 3].y = rect.yMin;

							colors[vertIndex + 0] = useColors[texIndex];
							colors[vertIndex + 1] = useColors[texIndex];
							colors[vertIndex + 2] = useColors[texIndex];
							colors[vertIndex + 3] = useColors[texIndex];

							uv3[vertIndex + 0].x = 1f;
							uv3[vertIndex + 0].y = (float)state;
							uv3[vertIndex + 1].x = 1f;
							uv3[vertIndex + 1].y = (float)state;
							uv3[vertIndex + 2].x = 1f;
							uv3[vertIndex + 2].y = (float)state;
							uv3[vertIndex + 3].x = 1f;
							uv3[vertIndex + 3].y = (float)state;
								
							normals[vertIndex + 0].x = 0f;
							normals[vertIndex + 0].y = 1f;
							normals[vertIndex + 0].z = 0f;
							normals[vertIndex + 1].x = 0f;
							normals[vertIndex + 1].y = 1f;
							normals[vertIndex + 1].z = 0f;
							normals[vertIndex + 2].x = 0f;
							normals[vertIndex + 2].y = 1f;
							normals[vertIndex + 2].z = 0f;
							normals[vertIndex + 3].x = 0f;
							normals[vertIndex + 3].y = 1f;
							normals[vertIndex + 3].z = 0f;

							triangles.Add(vertIndex + 0);
							triangles.Add(vertIndex + 1);
							triangles.Add(vertIndex + 2);
							triangles.Add(vertIndex + 1);
							triangles.Add(vertIndex + 3);
							triangles.Add(vertIndex + 2);
							vertIndex += 4;
							vertStride++;
						}
						//CGrassFast.CState cState = new CGrassFast.CState();
						//cState.m_State = (float)num20;
						//cState.m_Step = 1f + Random.Range(0f, 1f);
						//cState.m_Touch = 1f;
						//cState.m_TouchFactor = 0.1f;
						//cState.m_TouchBegin = false;
						//cState.m_SizeOrStartState = ((!useGPU) ? ((int)data.m_Size[num16]) : num20);
						//cState.m_GrassPieceIndex = grassPieceIndex;
						//cState.m_Density = density;
						//list.Add(cState);
						//array8[l, m] = cState;
					}
				}
			}

			// create grass chunk
			string chunkName = string.Format("GrassChunk({0},{1})", d.posX, d.posZ);
			GameObject gameObject = new GameObject (chunkName);
			
			gameObject.transform.parent = transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
			gameObject.hideFlags = HideFlags.DontSaveInEditor;
			gameObject.layer = LayerMask.NameToLayer("Terrain");

			gameObject.transform.localPosition = chunkPos;

			Mesh mesh = new Mesh();
			mesh.vertices = vertices;
			mesh.uv = uvs;
			mesh.colors32 = colors;
			mesh.normals = normals;
			mesh.uv3 = uv3;
			mesh.triangles = triangles.ToArray();

			MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
			if (meshFilter == null) meshFilter = gameObject.AddComponent<MeshFilter>();
			meshFilter.sharedMesh = mesh;

			MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
			if (meshRenderer == null) meshRenderer = gameObject.AddComponent<MeshRenderer>();
			meshRenderer.sharedMaterial = packedMaterial;
			meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			mesh.MarkDynamic();

			GrassChunk gChunk = gameObject.GetComponent<GrassChunk>();
			if (gChunk == null) gChunk = gameObject.AddComponent<GrassChunk>();
			gChunk.basePos = chunkPos;
			//gChunk.m_StateList = list.ToArray();
			//gChunk.m_StateTable = array8;
			gChunk.meshRenderer = meshRenderer;
			gChunk.meshFilter = meshFilter;
			gChunk.uv3 = uv3;
			gChunk.Init();
			chunkList.Add(gChunk);
		}

		private void ClearGrassChunks()
		{
			if (chunkList == null) 
			{
				return;
			}

			for (int i = 0; i < chunkList.Count; i++)
			{
				if (!(chunkList[i] == null))
				{
					GameObject grassGo = chunkList[i].gameObject;
					MeshFilter mf = grassGo.GetComponent<MeshFilter>();
					if (mf != null)
					{
						Object.DestroyImmediate(mf.sharedMesh);
					}
					Object.DestroyImmediate(grassGo);
				}
			}
			chunkList.Clear();
		}

		private bool GetTexture(int index, ref Texture2D tex, ref Rect rect, ref float aspect)
		{
			if (index >= 0 && index < this.useTextures.Length)
			{
				tex = packedTexture;
				rect = packedRect[index];
				aspect = packedAspectRatio[index];
				return true;
			}
			return false;
		}

		// TODO
		private float GetTerrainHeight(float x, float z)
		{
			return 0;
		}

		private void Disturb(Vector3 pos, float strength)
		{
			for (int i = 0; i < chunkList.Count; i++)
			{
				chunkList[i].Disturb(pos, strength);
			}
		}
	}
}
