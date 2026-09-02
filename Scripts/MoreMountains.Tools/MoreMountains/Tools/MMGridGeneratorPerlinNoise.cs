using UnityEngine;

namespace MoreMountains.Tools;

public class MMGridGeneratorPerlinNoise : MMGridGenerator
{
	public static int[,] Generate(int width, int height, float seed)
	{
		int[,] array = MMGridGenerator.PrepareGrid(ref width, ref height);
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				int value = Mathf.RoundToInt(Mathf.PerlinNoise((float)i * seed, (float)j * seed));
				MMGridGenerator.SetGridCoordinate(array, i, j, value);
			}
		}
		return array;
	}
}
