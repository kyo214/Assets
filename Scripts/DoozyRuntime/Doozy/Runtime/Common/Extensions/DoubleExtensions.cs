using System;

namespace Doozy.Runtime.Common.Extensions;

public static class DoubleExtensions
{
	public static double Round(this double target, int decimals = 1)
	{
		return Math.Round(target, decimals);
	}
}
