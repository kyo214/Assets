using System;
using UnityEngine;

namespace MoreMountains.Tools;

public class MMGridGeneratorRandomWalkGround : MMGridGenerator
{
	public static int[,] Generate(int width, int height, int seed, int minHeightDifference, int maxHeightDifference, int minFlatDistance, int maxFlatDistance, int maxHeight)
	{
		System.Random random = new System.Random(seed.GetHashCode());
		UnityEngine.Random.InitState(seed);
		int[,] array = MMGridGenerator.PrepareGrid(ref width, ref height);
		int num = UnityEngine.Random.Range(0, maxHeight);
		int num2 = num;
		int num3 = -1;
		for (int i = 0; i < width; i++)
		{
			num = num2;
			int num4 = UnityEngine.Random.Range(minHeightDifference, maxHeightDifference);
			int num5 = UnityEngine.Random.Range(minFlatDistance, maxFlatDistance);
			if (num3 >= num5 - 1)
			{
				if (random.Next(2) > 0)
				{
					num -= num4;
				}
				else if (num2 + num4 < height)
				{
					num += num4;
				}
				num = Mathf.Clamp(num, 1, maxHeight);
				num3 = 0;
			}
			else
			{
				num3++;
			}
			for (int num6 = num; num6 >= 0; num6--)
			{
				array[i, num6] = 1;
			}
			num2 = num;
		}
		return array;
	}
}
