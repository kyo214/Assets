using UnityEngine;
using UnityEngine.Tilemaps;

namespace MoreMountains.Tools;

public class MMTilemap : MonoBehaviour
{
	public static Vector2 GetRandomPosition(Tilemap targetTilemap, Grid grid, int width, int height, bool shouldBeFilled = true, int maxIterations = 1000)
	{
		int i = 0;
		Vector3Int zero = Vector3Int.zero;
		for (; i < maxIterations; i++)
		{
			zero.x = Random.Range(0, width);
			zero.y = Random.Range(0, height);
			zero += MMTilemapGridRenderer.ComputeOffset(width - 1, height - 1);
			if (targetTilemap.HasTile(zero) == shouldBeFilled)
			{
				return targetTilemap.CellToWorld(zero) + grid.cellSize / 2f;
			}
		}
		return Vector2.zero;
	}

	public static Vector2 GetRandomPositionOnGround(Tilemap targetTilemap, Grid grid, int width, int height, int startingHeight, int xMin, int xMax, bool shouldBeFilled = true, int maxIterations = 1000)
	{
		int i = 0;
		Vector3Int zero = Vector3Int.zero;
		for (; i < maxIterations; i++)
		{
			zero.x = Random.Range(xMin, xMax);
			zero.y = startingHeight;
			zero += MMTilemapGridRenderer.ComputeOffset(width - 1, height - 1);
			for (int num = height; num > 0; num--)
			{
				if (targetTilemap.HasTile(zero) == shouldBeFilled)
				{
					zero.y++;
					return targetTilemap.CellToWorld(zero) + grid.cellSize / 2f;
				}
				zero.y--;
			}
		}
		return Vector2.zero;
	}
}
