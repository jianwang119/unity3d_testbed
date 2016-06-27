using UnityEngine;
using System.Collections;

namespace Terrain
{
	public static class Utils
	{
		public const float TERRAIN_TILE_SIZE = 16f;
		public const float TERRAIN_TILE_HALF_SIZE = TERRAIN_TILE_SIZE / 2f;
		public const int GRASS_TILE_CELL_DIM = 32;
		public const int GRASS_TILE_CELL_COUNT = GRASS_TILE_CELL_DIM * GRASS_TILE_CELL_DIM;
		public const float GRASS_TILE_CELL_SIZE = TERRAIN_TILE_SIZE / GRASS_TILE_CELL_DIM;

        public static int PosToGrid(float p, float s, int d)
        {
            return Mathf.Clamp(Mathf.FloorToInt(p / s), 0, d - 1);
        }

        public static float GridToPos(int g, float s)
        {
            return (float)g * s;
        }
	}
}
