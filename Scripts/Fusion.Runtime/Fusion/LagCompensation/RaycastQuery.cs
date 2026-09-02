using UnityEngine;

namespace Fusion.LagCompensation;

public struct RaycastQuery
{
	public Vector3 Origin;

	public Vector3 Direction;

	public float Length;

	public bool HitAll;

	public static Query CreateQuery(Vector3 origin, Vector3 direction, float length, bool hitAll, PlayerRef player, int layerMask = -1, HitOptions options = HitOptions.None, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal, PreProcessingDelegate preProcessRoots = null)
	{
		return new Query
		{
			Type = QueryType.Raycast,
			Player = player,
			LayerMask = layerMask,
			Options = options,
			TriggerInteraction = queryTriggerInteraction,
			PreProcessingDelegate = preProcessRoots,
			Raycast = new RaycastQuery
			{
				Origin = origin,
				Direction = direction,
				Length = length,
				HitAll = hitAll
			}
		};
	}

	public static Query CreateQuery(Vector3 origin, Vector3 direction, float length, bool hitAll, int tick, int? tickTo = null, float? alpha = null, int layerMask = -1, HitOptions options = HitOptions.None, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal, PreProcessingDelegate preProcessRoots = null)
	{
		return new Query
		{
			Type = QueryType.Raycast,
			Tick = tick,
			TickTo = tickTo,
			Alpha = alpha,
			LayerMask = layerMask,
			Options = options,
			TriggerInteraction = queryTriggerInteraction,
			PreProcessingDelegate = preProcessRoots,
			Raycast = new RaycastQuery
			{
				Origin = origin,
				Direction = direction,
				Length = length,
				HitAll = hitAll
			}
		};
	}
}
