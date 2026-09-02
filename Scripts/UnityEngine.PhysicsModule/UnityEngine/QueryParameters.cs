namespace UnityEngine;

public struct QueryParameters(int layerMask = -5, bool hitMultipleFaces = false, QueryTriggerInteraction hitTriggers = QueryTriggerInteraction.UseGlobal, bool hitBackfaces = false)
{
	public int layerMask = layerMask;

	public bool hitMultipleFaces = hitMultipleFaces;

	public QueryTriggerInteraction hitTriggers = hitTriggers;

	public bool hitBackfaces = hitBackfaces;

	public static QueryParameters Default => new QueryParameters(-5, false, QueryTriggerInteraction.UseGlobal, false);
}
