using System;

namespace MoreMountains.Tools;

public class MMGridGeneratorRandom : MMGridGenerator
{
	public static int[,] Generate(int width, int height, int seed, int fillPercentage)
	{
		int[,] array = MMGridGenerator.PrepareGrid(ref width, ref height);
		array = MMGridGeneratorFull.Generate(width, height, full: true);
		Random random = new Random(seed);
		for (int i = 0; i <= width; i++)
		{
			for (int j = 0; j <= height; j++)
			{
				int value = ((random.Next(0, 100) < fillPercentage) ? 1 : 0);
				MMGridGenerator.SetGridCoordinate(array, i, j, value);
			}
		}
		return array;
	}
}
