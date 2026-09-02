using System.Collections.Generic;
using Unity.Mathematics;

namespace UnityEngine.U2D.Common.UTess;

internal struct IntersectionCompare : IComparer<int2>
{
	public Array<double2> points;

	public Array<int2> edges;

	public unsafe fixed double xvasort[4];

	public unsafe fixed double xvbsort[4];

	public unsafe int Compare(int2 a, int2 b)
	{
		int2 int5 = edges[a.x];
		int2 int6 = edges[a.y];
		int2 int7 = edges[b.x];
		int2 int8 = edges[b.y];
		ref double reference = ref xvasort[0];
		reference = points[int5.x].x;
		xvasort[1] = points[int5.y].x;
		xvasort[2] = points[int6.x].x;
		xvasort[3] = points[int6.y].x;
		ref double reference2 = ref xvbsort[0];
		reference2 = points[int7.x].x;
		xvbsort[1] = points[int7.y].x;
		xvbsort[2] = points[int8.x].x;
		xvbsort[3] = points[int8.y].x;
		fixed (double* array = xvasort)
		{
			ModuleHandle.InsertionSort<double, XCompare>(array, 0, 3, default);
		}
		fixed (double* array = xvbsort)
		{
			ModuleHandle.InsertionSort<double, XCompare>(array, 0, 3, default);
		}
		for (int i = 0; i < 4; i++)
		{
			if (xvasort[i] - xvbsort[i] != 0.0)
			{
				if (!(xvasort[i] < xvbsort[i]))
				{
					return 1;
				}
				return -1;
			}
		}
		if (!(points[int5.x].y < points[int5.x].y))
		{
			return 1;
		}
		return -1;
	}
}
