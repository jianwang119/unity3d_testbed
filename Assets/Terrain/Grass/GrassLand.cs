using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Terrain
{
	[System.Serializable]
	public class GrassChunkData
	{
		public ushort[] size = new ushort[TerrainConst.GRASS_CHUNK_MAX_SIZE];
		public byte[] type = new byte[TerrainConst.GRASS_CHUNK_MAX_SIZE];
		public int[] textureIndeics;
		public int seed;
	}

	public class GrassLand : MonoBehaviour 
	{
		// TODO data for brush
		public int chunkX;
		public int chunkZ;
		public GrassChunkData[] chunks;
		public Texture2D[] textures;
		public Color[] colors; 

		public static GrassLand Inst = null;

		private int frameUpdate = 0;
		private List<GrassChunk> grassChunks = new List<GrassChunk>();

		private Texture2D packedTexture;
		private Rect[] packedRect;
		private float[] packedAspectRatio;
		private Material packedMaterial;

		void Awake()
		{
			GrassLand.Inst = this;
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

			for (int i = 0; i < grassChunks.Count; i++)
			{
				grassChunks[i].OnUpdate(frameUpdate);
			}
		}

		void GenerateGrass()
		{
			ClearGrassChunks();

			if (colors == null && textures != null)
			{
				Color clr = new Color(0.5f, 0.5f, 0.5f, 1f);
				colors = new Color[textures.Length];
				for (int i = 0; i < colors.Length; i++)
				{
					colors[i] = clr;
				}
			}

			BuildMaterial();

			BuildGrassChunks();
		}

		private void BuildMaterial(bool useGPU, bool pack)
		{
			packedTexture = null;
			packedRect = null;
			packedAspectRatio = null;
			packedMaterial = null;

			int texCount = textures.Length;
			if (texCount <= 0) 
			{
				return;
			}
			packedTexture = new Texture2D(16, 16, textures[0].format, true);
			if (packedTexture == null)
			{
				return;
			}
			packedRect = packedTexture.PackTextures(textures, 2, 512, true);
			if (packedRect == null || packedRect.Length != texCount)
			{
				Object.Destroy(packedTexture);
				packedTexture = null;
				return;
			}
			packedAspectRatio = new float[texCount];
			for (int i = 0; i < texCount; i++)
			{
				packedAspectRatio[i] = (float)textures[i].width / (float)textures[i].height;
				textures[i] = null;
			}

			packedMaterial = new Material(Shader.Find("Diffuse"));//"Terrain/Grass"
			packedMaterial.mainTexture = packedTexture;
		}

		private void BuildGrassChunks()
		{
			for (int i = 0; i < chunkX; i++)
			{
				for (int j = 0; j < chunkZ; j++)
				{
					GrassChunkData d = chunks[i * chunkX + j];
					if (d != null && d.size.Length == 1024)
					{
						BuildGrassChunk(d, i, j);
					}
				}
			}
		}

		private void BuildGrassChunk(GrassChunkData d, int cx, int cz)
		{
			if (d == null || d.textureIndeics == null)
			{
				return;
			}

			int validGrassCount = 0;
			int grassCount = 0;

			for (int i = 0; i < TerrainConst.GRASS_CHUNK_COUNT; i++)
			{
				for (int j = 0; j < TerrainConst.GRASS_CHUNK_COUNT; j++)
				{
					int idx = i * TerrainConst.GRASS_CHUNK_COUNT + j;

					if (d.textureIndeics != null && d.size[idx] > 0 &&
					    (d.type[idx] & 0xf) > 0 && (int)(d.type[idx] & 0xf) < d.textureIndeics.Length + 1)
					{
						validGrassCount++;
						grassCount += 1 + ((d.type[idx] & 0xf0) >> 4);
					}
				}
			}
			if (validGrassCount <= 0)
			{
				return;
			}

			float x = (float)cx * TerrainConst.TERRAIN_CHUNK_SIZE;
			float z = (float)cz * TerrainConst.TERRAIN_CHUNK_SIZE;
			float y = float.MaxValue;

			Vector3[] vertices = new Vector3[grassCount * 4];
			Color32[] colors = new Color32[grassCount * 4];
			Vector2[] uvs = new Vector2[grassCount * 4];
			Vector3[] normals = new Vector3[grassCount * 4];
			Vector4[] tangents = new Vector4[grassCount * 4];
			Vector2[] effectors = new Vector2[grassCount * 4];
			List<int> triangles = new List<int>();

			//List<CGrassFast.CState> list = new List<CGrassFast.CState>();
			//CGrassFast.CState[,] array8 = new CGrassFast.CState[32, 32];


			CSimpleRandom cSimpleRandom = new CSimpleRandom();

			int vertIndex = 0;
			int num7 = 0;


			for (int k = 0; k < d.textureIndeics.Length; k++)
			{
				Texture2D tex = null;
				Rect rect = default(Rect);
				float aspect = 1f;
				GetTexture(d.textureIndeics[k], ref tex, ref rect, ref aspect);

				Random rnd = new Random();
				rnd.seed = d.seed;

				int num10 = 0;
				int num11 = (aspect != 1f) ? (CGrassFast.s_GrassSkip / 2) : CGrassFast.s_GrassSkip;
				if (num11 < 1)
				{
					num11 = 1;
				}


				for (int l = 0; l < 32; l++)
				{
					for (int m = 0; m < 32; m++)
					{
						int idx = l * 32 + m;

						float rx = rnd.Range(0f, 0.266666681f * (float)num11);
						float rz = rnd.Range(0f, 0.266666681f * (float)num11);
						int rotation = rnd.Range(0, 63);
						int state = rnd.Range(0, 255);

						if ((int)(d.type[idx] & 0xf) == k + 1 && d.size[idx] > 0)
						{
							if (num10++ % num11 == 0)
							{
								float gx = (float)m * 16f / 32f + rx;
								float gz = (float)l * 16f / 32f + rz;
								float gy = GetHeight(gx + x, gz + z);

								if (y == 3.40282347E+38f)
								{
									y = gy;
								}
								gy -= y;

								int density = (d.type[idx] & 0xf0) >> 4;
								density = 0;


								int seed = line * 1000000 + column * 10000 + l * 100 + m;
								cSimpleRandom.Reset(seed);

								FillGrassVB(vertices, vertIndex, gx, gy, gz, rotation, (int)d.size[idx], aspect, density, cSimpleRandom);

								int grassPieceIndex = num7;
								for (int n = 0; n < 1 + density; n++)
								{
									uvs[vertIndex].x = rect.xMin;
									uvs[vertIndex].y = rect.yMax - 0.01f;
									uvs[vertIndex + 1].x = rect.xMax;
									uvs[vertIndex + 1].y = rect.yMax - 0.01f;
									uvs[vertIndex + 2].x = rect.xMin;
									uvs[vertIndex + 2].y = rect.yMin;
									uvs[vertIndex + 3].x = rect.xMax;
									uvs[vertIndex + 3].y = rect.yMin;

									colors[vertIndex] = colors[d.textureIndeics[k]];
									colors[vertIndex + 1] = colors[d.textureIndeics[k]];
									colors[vertIndex + 2] = colors[d.textureIndeics[k]];
									colors[vertIndex + 3] = colors[d.textureIndeics[k]];
	
									normals[vertIndex].x = 0f;
									normals[vertIndex].y = 1f;
									normals[vertIndex].z = 0f;
									normals[vertIndex + 1].x = 0f;
									normals[vertIndex + 1].y = 1f;
									normals[vertIndex + 1].z = 0f;
									normals[vertIndex + 2].x = 0f;
									normals[vertIndex + 2].y = 1f;
									normals[vertIndex + 2].z = 0f;
									normals[vertIndex + 3].x = 0f;
									normals[vertIndex + 3].y = 1f;
									normals[vertIndex + 3].z = 0f;

									float ty = (float)d.size[idx] / 1024f * 40f;
									float tz = 1f + Random.Range(0f, 1f);
									tangents[vertIndex].x = 1f;
									tangents[vertIndex].y = ty;
									tangents[vertIndex].z = tz;
									tangents[vertIndex + 1].x = 1f;
									tangents[vertIndex + 1].y = ty;
									tangents[vertIndex + 1].z = tz;
									tangents[vertIndex + 2].x = 0f;
									tangents[vertIndex + 2].y = ty;
									tangents[vertIndex + 2].z = tz;
									tangents[vertIndex + 3].x = 0f;
									tangents[vertIndex + 3].y = ty;
									tangents[vertIndex + 3].z = tz;

									effectors[vertIndex].x = 1f;
									effectors[vertIndex].y = (float)state;
									effectors[vertIndex + 1].x = 1f;
									effectors[vertIndex + 1].y = (float)state;
									effectors[vertIndex + 2].x = 1f;
									effectors[vertIndex + 2].y = (float)state;
									effectors[vertIndex + 3].x = 1f;
									effectors[vertIndex + 3].y = (float)state;
												
									triangles.Add(vertIndex);
									triangles.Add(vertIndex + 1);
									triangles.Add(vertIndex + 2);
									triangles.Add(vertIndex + 1);
									triangles.Add(vertIndex + 3);
									triangles.Add(vertIndex + 2);
									vertIndex += 4;
									num7++;
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
				}
			}

			// create grass go
			string text = string.Format("GrassChunk({0},{1})", x, z);
			GameObject gameObject = new GameObject (text);
			
			gameObject.transform.parent = transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
			gameObject.hideFlags = HideFlags.DontSaveInEditor;
			gameObject.layer = LayerMask.NameToLayer("Terrain");

			gameObject.transform.localPosition = new Vector3(x, y, z);

			Mesh mesh = new Mesh();
			mesh.vertices = vertices;
			mesh.uv = uvs;
			mesh.colors32 = colors;
			mesh.normals = normals;
			mesh.tangents = tangents;
			mesh.uv3 = effectors;
			mesh.subMeshCount = d.textureIndeics.Length;
			mesh.triangles = triangles.ToArray();

			MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
			if (meshFilter == null) meshFilter = gameObject.AddComponent<MeshFilter>();
			meshFilter.sharedMesh = mesh;

			MeshRenderer meshRenderer = gameObject.GetComponent<MeshFilter>();
			if (meshRenderer == null) meshRenderer = gameObject.AddComponent<MeshRenderer>();
			meshRenderer.sharedMaterial = packedMaterial;
			meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			mesh.MarkDynamic();


			GrassChunk gChunk = gameObject.GetComponent<GrassChunk>();
			if (gChunk == null) gChunk = gameObject.AddComponent<GrassChunk>();
			gChunk.centerX = x;
			gChunk.centerZ = z;
			//gChunk.m_StateList = list.ToArray();
			//gChunk.m_StateTable = array8;
			gChunk.renderer = meshRenderer;
			gChunk.meshFilter = meshFilter;
			gChunk.effector = effectors;
			gChunk.Init();
			grassChunks.Add(gChunk);
		}

		private void FillGrassVB(Vector3[] vb, int vbIndex, float x, float y, float z, int rotation, int size, 
		                        float aspectRatio, int density, Random densityRand)
		{
			aspectRatio /= 2f;

			float r = Mathf.Deg2Rad * ((float)rotation / 64f * -60f + -30f);
			float s = (float)size / 1024f * 40f;

			float[,] verts = new float[4, 3];
			verts[0, 0] = -aspectRatio;
			verts[0, 1] = 1f;
			verts[1, 0] = aspectRatio;
			verts[1, 1] = 1f;
			verts[2, 0] = -aspectRatio;
			verts[3, 0] = aspectRatio;

			for (int i = 0; i < 1 + density; i++)
			{
				float sinr = Mathf.Sin(r);
				float cosr = Mathf.Cos(r);

				for (int j = 0; j < 4; j++)
				{
					verts[j, 2] = verts[j, 0] * sinr;
					verts[j, 0] = verts[j, 0] * cosr;
				}
				for (int k = 0; k < 4; k++)
				{
					vb[vbIndex + k].x = verts[k, 0] * s + x;
					vb[vbIndex + k].y = verts[k, 1] * s + y;
					vb[vbIndex + k].z = verts[k, 2] * s + z;
				}

				r = Mathf.Deg2Rad * (densityRand.Range(0, 360f) * -60f + -30f);
				x = x + densityRand.Range(-1f, 1f) * aspectRatio * s;
				z = z + densityRand.Range(-1f, 1f) * aspectRatio * s;
				vbIndex += 4;
			}
		}

		private void ClearGrassChunks()
		{
			if (chunks == null) 
			{
				return;
			}

			for (int i = 0; i < grassChunks.Count; i++)
			{
				if (!(chunks[i] == null))
				{
					GameObject grassGo = grassChunks[i].gameObject;
					MeshFilter mf = grassGo.GetComponent<MeshFilter>();
					if (mf != null)
					{
						Object.DestroyImmediate(mf.sharedMesh);
					}
					Object.DestroyImmediate(grassGo);
				}
			}
			chunks.Clear();
		}

		private bool GetTexture(int index, ref Texture2D tex, ref Rect rect, ref float aspect)
		{
			if (index >= 0 && index < this.textures.Length)
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
			for (int i = 0; i < grassChunks.Count; i++)
			{
				grassChunks[i].Disturb(pos, strength);
			}
		}
	}
}
