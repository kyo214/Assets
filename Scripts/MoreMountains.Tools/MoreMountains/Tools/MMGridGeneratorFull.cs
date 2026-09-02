namespace MoreMountains.Tools;

public class MMGridGeneratorFull : MMGridGenerator
{
	public static int[,] Generate(int width, int height, bool full)
	{
		int[,] array = MMGridGenerator.PrepareGrid(ref width, ref height);
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				MMGridGenerator.SetGridCoordinate(array, i, j, full ? 1 : 0);
			}
		}
		return array;
	}
}
