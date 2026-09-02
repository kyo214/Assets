using System.Collections.Generic;

namespace Doozy.Runtime.Common.Extensions;

public static class EnumerableExtensions
{
	public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source, IEqualityComparer<T> comparer = null)
	{
		return new HashSet<T>(source, comparer);
	}
}
