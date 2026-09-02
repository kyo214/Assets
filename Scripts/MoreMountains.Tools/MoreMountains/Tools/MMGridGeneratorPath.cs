using System;
using UnityEngine;

namespace MoreMountains.Tools;

public class MMGridGeneratorPath : MMGridGenerator
{
	public enum Directions
	{
		TopToBottom = 0,
		BottomToTop = 1,
		LeftToRight = 2,
		RightToLeft = 3
	}

	public static int[,] Generate(int width, int height, int seed, Directions direction, Vector2Int startPosition, int pathMinWidth, int pathMaxWidth, int directionChangeDistance, int widthChangePercentage, int directionChangePercentage)
	{
		int[,] array = MMGridGenerator.PrepareGrid(ref width, ref height);
		array = MMGridGeneratorFull.Generate(width, height, full: true);
		System.Random random = new System.Random(seed);
		UnityEngine.Random.InitState(seed);
		int num = 1;
		int x = startPosition.x;
		int y = startPosition.y;
		MMGridGenerator.SetGridCoordinate(array, x, y, 0);
		switch (direction)
		{
		case Directions.TopToBottom:
		{
			int num5 = x;
			for (int n = -num; n <= num; n++)
			{
				MMGridGenerator.SetGridCoordinate(array, num5 + n, y, 0);
			}
			for (int num6 = y; num6 > 0; num6--)
			{
				num = ComputeWidth(random, widthChangePercentage, pathMinWidth, pathMaxWidth, num);
				num5 = DetermineNextStep(random, num5, directionChangeDistance, directionChangePercentage, pathMaxWidth, width);
				for (int num7 = -num; num7 <= num; num7++)
				{
					MMGridGenerator.SetGridCoordinate(array, num5 + num7, num6, 0);
				}
			}
			break;
		}
		case Directions.BottomToTop:
		{
			int num8 = x;
			for (int num9 = -num; num9 <= num; num9++)
			{
				MMGridGenerator.SetGridCoordinate(array, num8 + num9, y, 0);
			}
			for (int num10 = y; num10 < height; num10++)
			{
				num = ComputeWidth(random, widthChangePercentage, pathMinWidth, pathMaxWidth, num);
				num8 = DetermineNextStep(random, num8, directionChangeDistance, directionChangePercentage, pathMaxWidth, width);
				for (int num11 = -num; num11 <= num; num11++)
				{
					MMGridGenerator.SetGridCoordinate(array, num8 + num11, num10, 0);
				}
			}
			break;
		}
		case Directions.LeftToRight:
		{
			int num4 = y;
			for (int k = -num; k <= num; k++)
			{
				MMGridGenerator.SetGridCoordinate(array, x, num4 + k, 0);
			}
			for (int l = x; l < width; l++)
			{
				num = ComputeWidth(random, widthChangePercentage, pathMinWidth, pathMaxWidth, num);
				num4 = DetermineNextStep(random, num4, directionChangeDistance, directionChangePercentage, pathMaxWidth, width);
				for (int m = -num; m <= num; m++)
				{
					MMGridGenerator.SetGridCoordinate(array, l, num4 + m, 0);
				}
			}
			break;
		}
		case Directions.RightToLeft:
		{
			int num2 = y;
			for (int i = -num; i <= num; i++)
			{
				MMGridGenerator.SetGridCoordinate(array, x, num2 + i, 0);
			}
			for (int num3 = x; num3 > 0; num3--)
			{
				num = ComputeWidth(random, widthChangePercentage, pathMinWidth, pathMaxWidth, num);
				num2 = DetermineNextStep(random, num2, directionChangeDistance, directionChangePercentage, pathMaxWidth, width);
				for (int j = -num; j <= num; j++)
				{
					MMGridGenerator.SetGridCoordinate(array, num3, num2 + j, 0);
				}
			}
			break;
		}
		}
		return array;
	}

	private static int ComputeWidth(System.Random random, int widthChangePercentage, int pathMinWidth, int pathMaxWidth, int pathWidth)
	{
		if (random.Next(0, 100) > widthChangePercentage)
		{
			int num = UnityEngine.Random.Range(-pathMaxWidth, pathMaxWidth);
			pathWidth += num;
			if (pathWidth < pathMinWidth)
			{
				pathWidth = pathMinWidth;
			}
			if (pathWidth > pathMaxWidth)
			{
				pathWidth = pathMaxWidth;
			}
		}
		return pathWidth;
	}

	private static int DetermineNextStep(System.Random random, int x, int directionChangeDistance, int directionChangePercentage, int pathMaxWidth, int width)
	{
		if (random.Next(0, 100) > directionChangePercentage)
		{
			int num = UnityEngine.Random.Range(-directionChangeDistance, directionChangeDistance);
			x += num;
			if (x < pathMaxWidth)
			{
				x = pathMaxWidth;
			}
			if (x > width - pathMaxWidth)
			{
				x = width - pathMaxWidth;
			}
		}
		return x;
	}
}
