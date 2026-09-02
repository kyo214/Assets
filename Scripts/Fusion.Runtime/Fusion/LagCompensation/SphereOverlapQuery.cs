using UnityEngine;

namespace Fusion.LagCompensation;

public struct SphereOverlapQuery
{
	public Vector3 Center;

	public float Radius;

	public static Query CreateQuery(Vector3 center, float radius, PlayerRef player, int layerMask = -1, HitOptions options = HitOptions.None, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal, PreProcessingDelegate preProcessRoots = null)
	{
		return new Query
		{
			Type = QueryType.SphereOverlap,
			Player = player,
			LayerMask = layerMask,
			Options = options,
			TriggerInteraction = queryTriggerInteraction,
			PreProcessingDelegate = preProcessRoots,
			SphereOverlap = new SphereOverlapQuery
			{
				Center = center,
				Radius = radius
			}
		};
	}

	public static Query CreateQuery(Vector3 center, float radius, int tick, int? tickTo = null, float? alpha = null, int layerMask = -1, HitOptions options = HitOptions.None, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal, PreProcessingDelegate preProcessRoots = null)
	{
		return new Query
		{
			Type = QueryType.SphereOverlap,
			Tick = tick,
			TickTo = tickTo,
			Alpha = alpha,
			LayerMask = layerMask,
			Options = options,
			TriggerInteraction = queryTriggerInteraction,
			PreProcessingDelegate = preProcessRoots,
			SphereOverlap = new SphereOverlapQuery
			{
				Center = center,
				Radius = radius
			}
		};
	}
}
