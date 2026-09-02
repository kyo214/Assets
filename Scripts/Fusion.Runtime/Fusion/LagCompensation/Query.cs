using UnityEngine;

namespace Fusion.LagCompensation;

public struct Query
{
	public QueryType Type;

	public HitOptions Options;

	public QueryTriggerInteraction TriggerInteraction;

	public int LayerMask;

	public PlayerRef Player;

	public int Tick;

	public int? TickTo;

	public float? Alpha;

	public PreProcessingDelegate PreProcessingDelegate;

	public object UserArgs;

	public RaycastQuery Raycast;

	public SphereOverlapQuery SphereOverlap;

	public BoxOverlapQuery BoxOverlap;

	public PositionRotationQuery PositionRotation;
}
