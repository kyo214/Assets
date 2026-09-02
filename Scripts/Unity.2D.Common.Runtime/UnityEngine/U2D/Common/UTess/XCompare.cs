using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace UnityEngine.U2D.Common.UTess;

[StructLayout(LayoutKind.Sequential, Size = 1)]
internal struct XCompare : IComparer<double>
{
	public int Compare(double a, double b)
	{
		if (!(a < b))
		{
			return 1;
		}
		return -1;
	}
}
