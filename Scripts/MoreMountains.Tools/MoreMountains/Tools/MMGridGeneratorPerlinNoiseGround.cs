using UnityEngine;

namespace MoreMountains.Tools;

public class MMGridGeneratorPerlinNoiseGround : MMGridGenerator
{
	public static int[,] Generate(int width, int height, float seed)
	{
		int[,] array = MMGridGenerator.PrepareGrid(ref width, ref height);
		for (int i = 0; i < width; i++)
		{
			for (int num = Mathf.FloorToInt((Mathf.PerlinNoise(i, seed) - 0.5f) * (float)height) + height / 2; num >= 0; num--)
			{
				MMGridGenerator.SetGridCoordinate(array, i, num, 1);
			}
		}
		return array;
	}
}
