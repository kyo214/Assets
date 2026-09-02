using UnityEngine;

namespace Fusion.LagCompensation;

public struct BoxOverlapQuery
{
	public Vector3 Center;

	public Vector3 Extents;

	public Quaternion Rotation;

	internal Vector3 AabbExtents;

	public static Query CreateQuery(Vector3 center, Vector3 extents, Quaternion rotation, PlayerRef player, int layerMask = -1, HitOptions options = HitOptions.None, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal, PreProcessingDelegate preProcessRoots = null)
	{
		return new Query
		{
			Type = QueryType.BoxOverlap,
			Player = player,
			LayerMask = layerMask,
			Options = options,
			TriggerInteraction = queryTriggerInteraction,
			PreProcessingDelegate = preProcessRoots,
			BoxOverlap = new BoxOverlapQuery
			{
				Center = center,
				Extents = extents,
				Rotation = rotation
			}
		};
	}

	public static Query CreateQuery(Vector3 center, Vector3 extents, Quaternion rotation, int tick, int? tickTo = null, float? alpha = null, int layerMask = -1, HitOptions options = HitOptions.None, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal, PreProcessingDelegate preProcessRoots = null)
	{
		return new Query
		{
			Type = QueryType.BoxOverlap,
			Tick = tick,
			TickTo = tickTo,
			Alpha = alpha,
			LayerMask = layerMask,
			Options = options,
			TriggerInteraction = queryTriggerInteraction,
			PreProcessingDelegate = preProcessRoots,
			BoxOverlap = new BoxOverlapQuery
			{
				Center = center,
				Extents = extents,
				Rotation = rotation
			}
		};
	}
}
