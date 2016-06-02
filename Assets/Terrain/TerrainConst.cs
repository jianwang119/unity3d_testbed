using UnityEngine;
using System.Collections;

namespace Terrain
{
	public static class TerrainConst
	{
		public const float TERRAIN_CHUNK_SIZE = 16f;
		public const float TERRAIN_CHUNK_HALF_SIZE = TERRAIN_CHUNK_SIZE / 2f;
		public const int GRASS_CHUNK_CELL_DIM = 32;
		public const int GRASS_CHUNK_CELL_COUNT = GRASS_CHUNK_CELL_DIM * GRASS_CHUNK_CELL_DIM;
		public const float GRASS_CHUNK_CELL_SIZE = TERRAIN_CHUNK_SIZE / GRASS_CHUNK_CELL_DIM;
	}
}
