using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MoreMountains.Tools;

public class MMTilemapGridRenderer
{
	public static void RenderGrid(int[,] grid, MMTilemapGeneratorLayer layer, bool slowRender = false, float slowRenderDuration = 1f, MMTweenType slowRenderTweenType = null, MonoBehaviour slowRenderSupport = null)
	{
		if (layer.FusionMode == MMTilemapGeneratorLayer.FusionModes.Normal)
		{
			ClearTilemap(layer.TargetTilemap);
		}
		TileBase tile = layer.Tile;
		if (layer.FusionMode == MMTilemapGeneratorLayer.FusionModes.Combine)
		{
			grid = MMGridGenerator.InvertGrid(grid);
			tile = null;
		}
		if (layer.FusionMode == MMTilemapGeneratorLayer.FusionModes.Subtract)
		{
			grid = MMGridGenerator.InvertGrid(grid);
		}
		if (!slowRender || !Application.isPlaying)
		{
			DrawGrid(grid, layer.TargetTilemap, tile, 0, TotalFilledBlocks(grid));
		}
		else
		{
			slowRenderSupport.StartCoroutine(SlowRenderGrid(grid, layer.TargetTilemap, tile, slowRenderDuration, slowRenderTweenType, 60));
		}
		if (!Application.isPlaying & slowRender)
		{
			Debug.LogWarning("Rendering maps in SlowRender mode is only supported at runtime.");
		}
	}

	public static IEnumerator SlowRenderGrid(int[,] grid, Tilemap tilemap, TileBase tile, float slowRenderDuration, MMTweenType slowRenderTweenType, int frameRate)
	{
		int totalBlocks = TotalFilledBlocks(grid);
		totalBlocks = ((totalBlocks == 0) ? 1 : totalBlocks);
		frameRate = ((frameRate == 0) ? 1 : frameRate);
		float refreshFrequency = 1f / (float)frameRate;
		float startedAt = Time.unscaledTime;
		float lastWaitAt = startedAt;
		int drawnBlocks = 0;
		int lastIndex = 0;
		while (Time.unscaledTime - startedAt < slowRenderDuration)
		{
			while (Time.unscaledTime - lastWaitAt < refreshFrequency)
			{
				yield return null;
			}
			int num = totalBlocks - drawnBlocks;
			float num2 = Time.unscaledTime - startedAt;
			float num3 = slowRenderDuration - num2;
			float num4 = MMMaths.Remap(num2, 0f, slowRenderDuration, 0f, 1f);
			float num5 = MMTween.Tween(num4, 0f, 1f, 0f, 1f, slowRenderTweenType);
			float num6 = 1f - (num4 - num5);
			int num7 = Mathf.RoundToInt((float)num / num3 * refreshFrequency * num6);
			lastIndex = DrawGrid(grid, tilemap, tile, lastIndex, num7);
			drawnBlocks += num7;
			lastWaitAt = Time.unscaledTime;
		}
		DrawGrid(grid, tilemap, tile, lastIndex, totalBlocks - lastIndex);
	}

	public static int TotalFilledBlocks(int[,] grid)
	{
		int upperBound = grid.GetUpperBound(0);
		int upperBound2 = grid.GetUpperBound(1);
		int num = 0;
		for (int i = 0; i <= upperBound; i++)
		{
			for (int j = 0; j <= upperBound2; j++)
			{
				if (grid[i, j] == 1)
				{
					num++;
				}
			}
		}
		return num;
	}

	private static int DrawGrid(int[,] grid, Tilemap tilemap, TileBase tile, int startIndex, int numberOfTilesToDraw)
	{
		int upperBound = grid.GetUpperBound(0);
		int upperBound2 = grid.GetUpperBound(1);
		tilemap.RefreshAllTiles();
		int num = 0;
		int num2 = 0;
		for (int i = 0; i <= upperBound; i++)
		{
			for (int j = 0; j <= upperBound2; j++)
			{
				if (grid[i, j] == 1)
				{
					if (num >= startIndex)
					{
						Vector3Int position = new Vector3Int(i, j, 0);
						position += ComputeOffset(upperBound, upperBound2);
						tilemap.SetTile(position, tile);
						num2++;
					}
					if (num2 > numberOfTilesToDraw)
					{
						return num;
					}
					num++;
				}
			}
		}
		return num;
	}

	public static Vector3Int ComputeOffset(int width, int height)
	{
		Vector3Int vector3Int = new Vector3Int(width + 2, height + 2, 0);
		return -(vector3Int - vector3Int / 2);
	}

	public static void ClearTilemap(Tilemap tilemap)
	{
		tilemap.ClearAllTiles();
		tilemap.RefreshAllTiles();
	}
}
