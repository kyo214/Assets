using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MoreMountains.Tools;

public class MMGridGenerator
{
	public static int[,] PrepareGrid(ref int width, ref int height)
	{
		return new int[width, height];
	}

	public static bool SetGridCoordinate(int[,] grid, int x, int y, int value)
	{
		if (x >= 0 && x <= grid.GetUpperBound(0) && y >= 0 && y <= grid.GetUpperBound(1))
		{
			grid[x, y] = value;
			return true;
		}
		return false;
	}

	public static int[,] TilemapToGrid(Tilemap tilemap, int width, int height)
	{
		if (tilemap == null)
		{
			Debug.LogError("[MMGridGenerator] You're trying to convert a tilemap into a grid but didn't specify what tilemap to convert.");
			return null;
		}
		int[,] array = new int[width, height];
		Vector3Int zero = Vector3Int.zero;
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				zero.x = i;
				zero.y = j;
				zero += MMTilemapGridRenderer.ComputeOffset(width - 1, height - 1);
				array[i, j] = ((!(tilemap.GetTile(zero) == null)) ? 1 : 0);
			}
		}
		return array;
	}

	public static void DebugGrid(int[,] grid, int width, int height)
	{
		string text = "";
		for (int num = height - 1; num >= 0; num--)
		{
			text = text + "line " + num + " [";
			for (int i = 0; i < width; i++)
			{
				text += grid[i, num];
				if (i < width - 1)
				{
					text += ", ";
				}
			}
			text += "]\n";
		}
		Debug.Log(text);
	}

	public static int GetValueAtGridCoordinate(int[,] grid, int x, int y, int errorValue)
	{
		if (x >= 0 && x <= grid.GetUpperBound(0) && y >= 0 && y <= grid.GetUpperBound(1))
		{
			return grid[x, y];
		}
		return errorValue;
	}

	public static int[,] InvertGrid(int[,] grid)
	{
		for (int i = 0; i <= grid.GetUpperBound(0); i++)
		{
			for (int j = 0; j <= grid.GetUpperBound(1); j++)
			{
				grid[i, j] = ((grid[i, j] == 0) ? 1 : 0);
			}
		}
		return grid;
	}

	public static int[,] SmoothenGrid(int[,] grid)
	{
		int upperBound = grid.GetUpperBound(0);
		int upperBound2 = grid.GetUpperBound(1);
		for (int i = 0; i <= upperBound; i++)
		{
			for (int j = 0; j <= upperBound2; j++)
			{
				int adjacentWallsCount = GetAdjacentWallsCount(grid, i, j);
				if (adjacentWallsCount > 4)
				{
					grid[i, j] = 1;
				}
				else if (adjacentWallsCount < 4)
				{
					grid[i, j] = 0;
				}
			}
		}
		return grid;
	}

	public static int[,] ApplySafeSpots(int[,] grid, List<MMTilemapGeneratorLayer.MMTilemapGeneratorLayerSafeSpot> safeSpots)
	{
		foreach (MMTilemapGeneratorLayer.MMTilemapGeneratorLayerSafeSpot safeSpot in safeSpots)
		{
			Vector2Int start = safeSpot.Start;
			int x = start.x;
			start = safeSpot.End;
			int num = Mathf.Min(x, start.x);
			start = safeSpot.Start;
			int x2 = start.x;
			start = safeSpot.End;
			int num2 = Mathf.Max(x2, start.x);
			start = safeSpot.Start;
			int y = start.y;
			start = safeSpot.End;
			int num3 = Mathf.Min(y, start.y);
			start = safeSpot.Start;
			int y2 = start.y;
			start = safeSpot.End;
			int num4 = Mathf.Max(y2, start.y);
			for (int i = num; i < num2; i++)
			{
				for (int j = num3; j < num4; j++)
				{
					SetGridCoordinate(grid, i, j, 0);
				}
			}
		}
		return grid;
	}

	public static int[,] BindGrid(int[,] grid, bool top, bool bottom, bool left, bool right)
	{
		int upperBound = grid.GetUpperBound(0);
		int upperBound2 = grid.GetUpperBound(1);
		if (top)
		{
			for (int i = 0; i <= upperBound; i++)
			{
				grid[i, upperBound2] = 1;
			}
		}
		if (bottom)
		{
			for (int j = 0; j <= upperBound; j++)
			{
				grid[j, 0] = 1;
			}
		}
		if (left)
		{
			for (int k = 0; k <= upperBound2; k++)
			{
				grid[0, k] = 1;
			}
		}
		if (right)
		{
			for (int l = 0; l <= upperBound2; l++)
			{
				grid[upperBound, l] = 1;
			}
		}
		return grid;
	}

	public static int GetAdjacentWallsCount(int[,] grid, int x, int y)
	{
		int upperBound = grid.GetUpperBound(0);
		int upperBound2 = grid.GetUpperBound(1);
		int num = 0;
		for (int i = x - 1; i <= x + 1; i++)
		{
			for (int j = y - 1; j <= y + 1; j++)
			{
				if (i >= 0 && i <= upperBound && j >= 0 && j <= upperBound2)
				{
					if (i != x || j != y)
					{
						num += grid[i, j];
					}
				}
				else
				{
					num++;
				}
			}
		}
		return num;
	}
}
