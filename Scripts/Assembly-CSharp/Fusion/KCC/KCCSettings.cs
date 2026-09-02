using System;
using UnityEngine;

namespace Fusion.KCC;

[Serializable]
public sealed class KCCSettings
{
	[Header("Networked")]
	[Tooltip("Defines KCC physics behavior.\nNone - Skips almost all execution including processors, collider is despawned.\nCapsule - Full processing with capsule collider spawned.\nVoid - Skips internal physics query, collider is despawned, processors are executed.")]
	public EKCCShape Shape = EKCCShape.Capsule;

	[Tooltip("Sets collider isTrigger.")]
	public bool IsTrigger;

	[Tooltip("Sets collider radius.")]
	public float Radius = 0.35f;

	[Tooltip("Sets collider height.")]
	public float Height = 1.8f;

	[Tooltip("Defines additional radius extent for ground detection and processors tracking. Recommended range is 1-2% of radius.\nLow values decreases stability and has potential performance impact when executing additional checks.\nHigh values increases stability at the cost of increased sustained performance impact.")]
	public float Extent = 0.035f;

	[Tooltip("Mass used in calculations with dynamic forces.")]
	public float Mass = 1f;

	[Tooltip("Sets layer of collider game object.")]
	[KCCLayer]
	public int ColliderLayer;

	[Tooltip("Layer mask the KCC collides with.")]
	public LayerMask CollisionLayerMask = 1;

	[Tooltip("Defines KCC render behavior for input/state authority.\nNone - Skips render completely. Useful when render update is perfectly synchronized with fixed update or debugging.\nPredict - Full processing and physics query.\nInterpolate - Interpolation between last two fixed updates.")]
	public EKCCRenderBehavior RenderBehavior = EKCCRenderBehavior.Predict;

	[Tooltip("Default KCC features.")]
	public EKCCFeatures Features = EKCCFeatures.All;

	[Header("Local")]
	[Tooltip("Enable to always check collisions against non-convex mesh collider.")]
	public bool SuppressConvexMeshColliders;

	[Tooltip("Used to skip collider creation on proxies.")]
	public bool SpawnColliderOnProxy = true;

	[Tooltip("Allows input authority to call Teleport RPC. Use with caution.")]
	public bool AllowClientTeleports;

	[Tooltip("Defines minimum distance the KCC must move in single tick to treat the movement as instant (teleport). Affects interpolation and other KCC features.")]
	public float TeleportThreshold = 1f;

	[Tooltip("Single Move/CCD step is split into multiple smaller sub-steps which results in higher overall depenetration quality.")]
	[Range(1f, 16f)]
	public int MaxMoveSteps = 1;

	[Tooltip("Controls maximum distance the KCC moves in a single CCD step. Valid range is 25% - 75% of the radius. Use lower values if the character passes through geometry.\nThis setting is valid only when EKCCFeature.CCD is enabled. CCD Max Step Distance = Radius * CCD Radius Multiplier")]
	[Range(0.25f, 0.75f)]
	public float CCDRadiusMultiplier = 0.75f;

	[Tooltip("Defines render position distance to smooth out jitter. Higher values may introduce noticeable delay when switching move direction.\nX = Horizontal axis.\nY = Vertical axis.")]
	public Vector2 AntiJitterDistance;

	[Tooltip("Maximum ground check distance for snapping.")]
	public float GroundSnapDistance = 0.25f;

	[Tooltip("Extra snapping speed per second.")]
	public float GroundSnapSpeed = 4f;

	[Tooltip("Maximum obstacle height to step on it.")]
	public float StepHeight = 0.3f;

	[Tooltip("Maximum depth of the step check.")]
	public float StepDepth = 0.2f;

	[Tooltip("Multiplier of unapplied movement projected to step up. This helps traversing obstacles faster.")]
	public float StepSpeed = 1f;

	[Tooltip("Perform single overlap query during move. Hits are tracked on position before depenetration. This is a performance optimization for non-player characters at the cost of possible errors in movement.")]
	public bool ForceSingleOverlapQuery;

	[Tooltip("Default processors, propagated to KCC.LocalProcessors upon initialization.")]
	public BaseKCCProcessor[] Processors;

	[Space(4f)]
	[Tooltip("Default position accuracy.")]
	public Accuracy PositionAccuracy = new Accuracy("Position");

	[Tooltip("Default rotation accuracy.")]
	public Accuracy RotationAccuracy = new Accuracy("Rotation");

	[Space(4f)]
	[Tooltip("Maximum count of collisions synchronized over network.")]
	public int MaxNetworkedCollisions = 4;

	[Tooltip("Maximum count of modifiers synchronized over network.")]
	public int MaxNetworkedModifiers = 4;

	[Tooltip("Maximum count of ignored colliders synchronized over network.")]
	public int MaxNetworkedIgnores = 4;

	public void CopyFromOther(KCCSettings other)
	{
		Shape = other.Shape;
		IsTrigger = other.IsTrigger;
		Radius = other.Radius;
		Height = other.Height;
		Extent = other.Extent;
		Mass = other.Mass;
		ColliderLayer = other.ColliderLayer;
		CollisionLayerMask = other.CollisionLayerMask;
		RenderBehavior = other.RenderBehavior;
		Features = other.Features;
		SuppressConvexMeshColliders = other.SuppressConvexMeshColliders;
		SpawnColliderOnProxy = other.SpawnColliderOnProxy;
		AllowClientTeleports = other.AllowClientTeleports;
		TeleportThreshold = other.TeleportThreshold;
		MaxMoveSteps = other.MaxMoveSteps;
		CCDRadiusMultiplier = other.CCDRadiusMultiplier;
		AntiJitterDistance = other.AntiJitterDistance;
		GroundSnapDistance = other.GroundSnapDistance;
		GroundSnapSpeed = other.GroundSnapSpeed;
		StepHeight = other.StepHeight;
		StepDepth = other.StepDepth;
		StepSpeed = other.StepSpeed;
		ForceSingleOverlapQuery = other.ForceSingleOverlapQuery;
		if (other.Processors != null && other.Processors.Length != 0)
		{
			Processors = new BaseKCCProcessor[other.Processors.Length];
			Array.Copy(other.Processors, Processors, Processors.Length);
		}
		else
		{
			Processors = null;
		}
		PositionAccuracy = other.PositionAccuracy;
		RotationAccuracy = other.RotationAccuracy;
		MaxNetworkedCollisions = other.MaxNetworkedCollisions;
		MaxNetworkedModifiers = other.MaxNetworkedModifiers;
		MaxNetworkedIgnores = other.MaxNetworkedIgnores;
	}
}
