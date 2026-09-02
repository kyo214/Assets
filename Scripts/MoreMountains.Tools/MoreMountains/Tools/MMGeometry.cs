using System.Collections.Generic;

namespace MoreMountains.Tools;

public static class MMGeometry
{
	public struct MMEdge(int aV1, int aV2, int aIndex)
	{
		public int Vertice1 = aV1;

		public int Vertice2 = aV2;

		public int TriangleIndex = aIndex;
	}

	public static List<MMEdge> GetEdges(int[] indices)
	{
		List<MMEdge> list = new List<MMEdge>();
		for (int i = 0; i < indices.Length; i += 3)
		{
			int num = indices[i];
			int num2 = indices[i + 1];
			int num3 = indices[i + 2];
			list.Add(new MMEdge(num, num2, i));
			list.Add(new MMEdge(num2, num3, i));
			list.Add(new MMEdge(num3, num, i));
		}
		return list;
	}

	public static List<MMEdge> FindBoundary(this List<MMEdge> edges)
	{
		List<MMEdge> list = new List<MMEdge>(edges);
		for (int num = list.Count - 1; num > 0; num--)
		{
			for (int num2 = num - 1; num2 >= 0; num2--)
			{
				if (list[num].Vertice1 == list[num2].Vertice2 && list[num].Vertice2 == list[num2].Vertice1)
				{
					list.RemoveAt(num);
					list.RemoveAt(num2);
					num--;
					break;
				}
			}
		}
		return list;
	}

	public static List<MMEdge> SortEdges(this List<MMEdge> edges)
	{
		List<MMEdge> list = new List<MMEdge>(edges);
		for (int i = 0; i < list.Count - 2; i++)
		{
			MMEdge mMEdge = list[i];
			for (int j = i + 1; j < list.Count; j++)
			{
				MMEdge value = list[j];
				if (mMEdge.Vertice2 == value.Vertice1)
				{
					if (j != i + 1)
					{
						list[j] = list[i + 1];
						list[i + 1] = value;
					}
					break;
				}
			}
		}
		return list;
	}
}
