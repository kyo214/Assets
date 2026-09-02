#define DEBUG
#define ENABLE_PROFILER
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Fusion.LagCompensation;
using UnityEngine;
using UnityEngine.Profiling;

namespace Fusion;

[DisallowMultipleComponent]
[AddComponentMenu("Fusion/Lag Compensation/Hitbox Manager")]
[OrderAfter(new Type[] { typeof(HitboxRoot) })]
public sealed class HitboxManager : SimulationBehaviour
{
	[EditorDisabled(false)]
	[InlineHelp]
	[MultiPropertyDrawersFix]
	public int BVHDepth;

	[EditorDisabled(false)]
	[InlineHelp]
	[MultiPropertyDrawersFix]
	public int BVHNodes;

	[EditorDisabled(false)]
	[InlineHelp]
	[MultiPropertyDrawersFix]
	public int TotalHitboxes;

	private LagCompensationSettings _settings;

	private HitboxBuffer _hitboxBuffer;

	private List<HitboxHit> _lagCompensatedHits = new List<HitboxHit>();

	private Collider[] _physXOverlapHits = new Collider[64];

	private RaycastHit[] _physXRaycastHits = new RaycastHit[64];

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int ResolveQuery(ref Query query, List<LagCompensatedHit> hits, bool clearHits = true)
	{
		if (clearHits)
		{
			hits.Clear();
		}
		if (query.Tick == 0)
		{
			GetPlayerTickAndAlpha(query.Player, out query.Tick, out query.TickTo, out query.Alpha);
		}
		switch (query.Type)
		{
		case QueryType.Raycast:
		{
			if (RaycastInternal(ref query, out var hit))
			{
				hits.Add(hit);
				return 1;
			}
			return 0;
		}
		case QueryType.SphereOverlap:
			return OverlapSphereInternal(ref query, hits);
		case QueryType.BoxOverlap:
			return OverlapBoxInternal(ref query, hits);
		default:
			throw new ArgumentException($"Query type '{query.Type}' is not supported by this overload.");
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Raycast(Vector3 origin, Vector3 direction, float length, PlayerRef player, out LagCompensatedHit hit, int layerMask = -1, HitOptions options = HitOptions.None, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal, PreProcessingDelegate preProcessRoots = null)
	{
		Query query = RaycastQuery.CreateQuery(origin, direction, length, hitAll: false, player, layerMask, options, queryTriggerInteraction, preProcessRoots);
		GetPlayerTickAndAlpha(player, out query.Tick, out query.TickTo, out query.Alpha);
		return RaycastInternal(ref query, out hit);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Raycast(Vector3 origin, Vector3 direction, float length, int tick, int? tickTo, float? alpha, out LagCompensatedHit hit, int layerMask = -1, HitOptions options = HitOptions.None, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal, PreProcessingDelegate preProcessRoots = null)
	{
		Query query = RaycastQuery.CreateQuery(origin, direction, length, hitAll: false, tick, tickTo, alpha, layerMask, options, queryTriggerInteraction, preProcessRoots);
		return RaycastInternal(ref query, out hit);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int RaycastAll(Vector3 origin, Vector3 direction, float length, PlayerRef player, List<LagCompensatedHit> hits, int layerMask = -1, bool clearHits = true, HitOptions options = HitOptions.None, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal, PreProcessingDelegate preProcessRoots = null)
	{
		if (clearHits)
		{
			hits.Clear();
		}
		Query query = RaycastQuery.CreateQuery(origin, direction, length, hitAll: true, player, layerMask, options, queryTriggerInteraction, preProcessRoots);
		GetPlayerTickAndAlpha(player, out query.Tick, out query.TickTo, out query.Alpha);
		return RaycastAllInternal(ref query, hits);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int RaycastAll(Vector3 origin, Vector3 direction, float length, int tick, int? tickTo, float? alpha, List<LagCompensatedHit> hits, int layerMask = -1, bool clearHits = true, HitOptions options = HitOptions.None, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal, PreProcessingDelegate preProcessRoots = null)
	{
		if (clearHits)
		{
			hits.Clear();
		}
		Query query = RaycastQuery.CreateQuery(origin, direction, length, hitAll: true, tick, tickTo, alpha, layerMask, options, queryTriggerInteraction, preProcessRoots);
		return RaycastAllInternal(ref query, hits);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int OverlapSphere(Vector3 origin, float radius, PlayerRef player, List<LagCompensatedHit> hits, int layerMask = -1, HitOptions options = HitOptions.None, bool clearHits = true, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal, PreProcessingDelegate preProcessRoots = null)
	{
		if (clearHits)
		{
			hits.Clear();
		}
		Query query = SphereOverlapQuery.CreateQuery(origin, radius, player, layerMask, options, queryTriggerInteraction, preProcessRoots);
		GetPlayerTickAndAlpha(player, out query.Tick, out query.TickTo, out query.Alpha);
		return OverlapSphereInternal(ref query, hits);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int OverlapSphere(Vector3 origin, float radius, int tick, int? tickTo, float? alpha, List<LagCompensatedHit> hits, int layerMask = -1, HitOptions options = HitOptions.None, bool clearHits = true, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal, PreProcessingDelegate preProcessRoots = null)
	{
		if (clearHits)
		{
			hits.Clear();
		}
		Query query = SphereOverlapQuery.CreateQuery(origin, radius, tick, tickTo, alpha, layerMask, options, queryTriggerInteraction, preProcessRoots);
		return OverlapSphereInternal(ref query, hits);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int OverlapBox(Vector3 center, Vector3 extents, Quaternion orientation, PlayerRef player, List<LagCompensatedHit> hits, int layerMask = -1, HitOptions options = HitOptions.None, bool clearHits = true, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal, PreProcessingDelegate preProcessRoots = null)
	{
		if (clearHits)
		{
			hits.Clear();
		}
		Query query = BoxOverlapQuery.CreateQuery(center, extents, orientation, player, layerMask, options, queryTriggerInteraction, preProcessRoots);
		GetPlayerTickAndAlpha(player, out query.Tick, out query.TickTo, out query.Alpha);
		return OverlapBoxInternal(ref query, hits);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int OverlapBox(Vector3 center, Vector3 extents, Quaternion orientation, int tick, int? tickTo, float? alpha, List<LagCompensatedHit> hits, int layerMask = -1, HitOptions options = HitOptions.None, bool clearHits = true, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal, PreProcessingDelegate preProcessRoots = null)
	{
		if (clearHits)
		{
			hits.Clear();
		}
		Query query = BoxOverlapQuery.CreateQuery(center, extents, orientation, tick, tickTo, alpha, layerMask, options, queryTriggerInteraction, preProcessRoots);
		return OverlapBoxInternal(ref query, hits);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void PositionRotation(Hitbox hitbox, int tick, out Vector3 position, out Quaternion rotation, bool subtickAccuracy = false, int? tickTo = null, float? alpha = null)
	{
		Query query = PositionRotationQuery.CreateQuery(hitbox, tick, tickTo, alpha);
		query.Options = (subtickAccuracy ? HitOptions.SubtickAccuracy : HitOptions.None);
		PositionRotationInternal(ref query, out position, out rotation);
	}

	public void PositionRotation(Hitbox hitbox, PlayerRef player, out Vector3 position, out Quaternion rotation, bool subtickAccuracy = false)
	{
		Query query = PositionRotationQuery.CreateQuery(hitbox, player, subtickAccuracy);
		GetPlayerTickAndAlpha(player, out query.Tick, out query.TickTo, out query.Alpha);
		PositionRotationInternal(ref query, out position, out rotation);
	}

	[Obsolete("Use either the overload that receives a PlayerRef (default value will use the head tick) or the one that receives optional 'tickTo' and 'alpha' parameters.")]
	public bool Raycast(Vector3 origin, Vector3 direction, float length, int tick, out LagCompensatedHit hit, int layerMask = -1, HitOptions options = HitOptions.None, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
	{
		Query query = RaycastQuery.CreateQuery(origin, direction, length, hitAll: false, tick, null, null, layerMask, options, queryTriggerInteraction);
		return RaycastInternal(ref query, out hit);
	}

	[Obsolete("Use either the overload that receives a PlayerRef (default value will use the head tick) or the one that receives optional 'tickTo' and 'alpha' parameters.")]
	public int RaycastAll(Vector3 origin, Vector3 direction, float length, int tick, List<LagCompensatedHit> hits, int layerMask = -1, bool clearHits = true, HitOptions options = HitOptions.None, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
	{
		if (clearHits)
		{
			hits.Clear();
		}
		Query query = RaycastQuery.CreateQuery(origin, direction, length, hitAll: true, tick, null, null, layerMask, options, queryTriggerInteraction);
		return RaycastAllInternal(ref query, hits);
	}

	[Obsolete("Use either the overload that receives a PlayerRef (default value will use the head tick) or the one that receives optional 'tickTo' and 'alpha' parameters.")]
	public int OverlapSphere(Vector3 origin, float radius, int tick, List<LagCompensatedHit> hits, int layerMask = -1, HitOptions options = HitOptions.None, bool clearHits = true, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
	{
		if (clearHits)
		{
			hits.Clear();
		}
		Query query = SphereOverlapQuery.CreateQuery(origin, radius, tick, null, null, layerMask, options, queryTriggerInteraction);
		return OverlapSphereInternal(ref query, hits);
	}

	[Obsolete("Use either the overload that receives a PlayerRef (default value will use the head tick) or the one that receives optional 'tickTo' and 'alpha' parameters.")]
	public int OverlapBox(Vector3 center, Vector3 extents, Quaternion orientation, int tick, List<LagCompensatedHit> hits, int layerMask = -1, HitOptions options = HitOptions.None, bool clearHits = true, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
	{
		if (clearHits)
		{
			hits.Clear();
		}
		Query query = BoxOverlapQuery.CreateQuery(center, extents, orientation, tick, null, null, layerMask, options, queryTriggerInteraction);
		return OverlapBoxInternal(ref query, hits);
	}

	private unsafe void GetPlayerTickAndAlpha(PlayerRef player, out int tickFrom, out int? tickTo, out float? alpha)
	{
		SimulationInput inputForPlayer = Runner.Simulation.GetInputForPlayer(player);
		if (inputForPlayer == null)
		{
			tickFrom = _hitboxBuffer?.Current.Tick ?? ((int)Runner.Simulation.Tick);
			tickTo = null;
			alpha = null;
		}
		else if (Runner.IsClient)
		{
			tickFrom = inputForPlayer.Header->Tick;
			tickTo = null;
			alpha = null;
		}
		else
		{
			tickFrom = inputForPlayer.Header->InterpFrom;
			tickTo = inputForPlayer.Header->InterpTo;
			alpha = inputForPlayer.Header->InterpAlpha;
		}
	}

	private bool RaycastInternal(ref Query query, out LagCompensatedHit hit)
	{
		Assert.Check(query.Type == QueryType.Raycast);
		Assert.Check(!query.Raycast.HitAll);
		if (Runner.IsClient)
		{
			query.TickTo = null;
			query.Alpha = null;
		}
		else if ((query.Options & HitOptions.SubtickAccuracy) == 0 && query.Alpha.HasValue && query.Alpha.Value > 0.5f)
		{
			query.Tick++;
		}
		bool flag = _hitboxBuffer.Raycast(ref query, out var hit2);
		bool flag2 = false;
		RaycastHit hitInfo = default;
		if ((query.Options & HitOptions.IncludePhysX) == HitOptions.IncludePhysX && Runner.GetPhysicsScene().Raycast(query.Raycast.Origin, query.Raycast.Direction, out hitInfo, query.Raycast.Length, query.LayerMask, query.TriggerInteraction))
		{
			flag2 = true;
			if (flag && hit2.Distance > hitInfo.distance)
			{
				flag = false;
			}
		}
		if (flag)
		{
			hit = LagCompensatedHit.FromHitboxHit(ref hit2);
		}
		else if (flag2)
		{
			hit = (LagCompensatedHit)hitInfo;
		}
		else
		{
			hit = default;
		}
		return flag | flag2;
	}

	private int RaycastAllInternal(ref Query query, List<LagCompensatedHit> hits)
	{
		Assert.Check(query.Type == QueryType.Raycast);
		Assert.Check(query.Raycast.HitAll);
		if (Runner.IsClient)
		{
			query.TickTo = null;
			query.Alpha = null;
		}
		else if ((query.Options & HitOptions.SubtickAccuracy) == 0 && query.Alpha.HasValue && query.Alpha.Value > 0.5f)
		{
			query.Tick++;
		}
		_lagCompensatedHits.Clear();
		_hitboxBuffer.RaycastAll(ref query, _lagCompensatedHits);
		int num = 0;
		for (int i = 0; i < _lagCompensatedHits.Count; i++)
		{
			HitboxHit hitboxHit = _lagCompensatedHits[i];
			num++;
			hits.Add(LagCompensatedHit.FromHitboxHit(ref hitboxHit));
		}
		if ((query.Options & HitOptions.IncludePhysX) == HitOptions.IncludePhysX)
		{
			int num2 = Runner.GetPhysicsScene().Raycast(query.Raycast.Origin, query.Raycast.Direction, _physXRaycastHits, query.Raycast.Length, query.LayerMask, query.TriggerInteraction);
			for (int j = 0; j < num2; j++)
			{
				num++;
				hits.Add((LagCompensatedHit)_physXRaycastHits[j]);
			}
		}
		return num;
	}

	private int OverlapSphereInternal(ref Query query, List<LagCompensatedHit> hits)
	{
		Assert.Check(query.Type == QueryType.SphereOverlap);
		if (Runner.IsClient)
		{
			query.TickTo = null;
			query.Alpha = null;
		}
		else if ((query.Options & HitOptions.SubtickAccuracy) == 0 && query.Alpha.HasValue && query.Alpha.Value > 0.5f)
		{
			query.Tick++;
		}
		_lagCompensatedHits.Clear();
		_hitboxBuffer.OverlapSphere(ref query, _lagCompensatedHits);
		int num = 0;
		for (int i = 0; i < _lagCompensatedHits.Count; i++)
		{
			HitboxHit hitboxHit = _lagCompensatedHits[i];
			num++;
			hits.Add(LagCompensatedHit.FromHitboxHit(ref hitboxHit));
		}
		if ((query.Options & HitOptions.IncludePhysX) == HitOptions.IncludePhysX)
		{
			int num2 = Runner.GetPhysicsScene().OverlapSphere(query.SphereOverlap.Center, query.SphereOverlap.Radius, _physXOverlapHits, query.LayerMask, query.TriggerInteraction);
			for (int j = 0; j < num2; j++)
			{
				Collider collider = _physXOverlapHits[j];
				LagCompensatedHit item = new LagCompensatedHit
				{
					Collider = collider,
					Normal = default,
					Distance = 0f,
					GameObject = collider.gameObject,
					Type = HitType.PhysX
				};
				num++;
				hits.Add(item);
			}
		}
		return num;
	}

	private int OverlapBoxInternal(ref Query query, List<LagCompensatedHit> hits)
	{
		Assert.Check(query.Type == QueryType.BoxOverlap);
		if (Runner.IsClient)
		{
			query.TickTo = null;
			query.Alpha = null;
		}
		else if ((query.Options & HitOptions.SubtickAccuracy) == 0 && query.Alpha.HasValue && query.Alpha.Value > 0.5f)
		{
			query.Tick++;
		}
		_lagCompensatedHits.Clear();
		_hitboxBuffer.OverlapBox(ref query, computeDetailedInfo: true, _lagCompensatedHits);
		int num = 0;
		for (int i = 0; i < _lagCompensatedHits.Count; i++)
		{
			HitboxHit hitboxHit = _lagCompensatedHits[i];
			num++;
			hits.Add(LagCompensatedHit.FromHitboxHit(ref hitboxHit));
		}
		if ((query.Options & HitOptions.IncludePhysX) == HitOptions.IncludePhysX)
		{
			int num2 = Runner.GetPhysicsScene().OverlapBox(query.BoxOverlap.Center, query.BoxOverlap.Extents, _physXOverlapHits, query.BoxOverlap.Rotation, query.LayerMask, query.TriggerInteraction);
			for (int j = 0; j < num2; j++)
			{
				Collider collider = _physXOverlapHits[j];
				Vector3 normal = default;
				LagCompensatedHit item = new LagCompensatedHit
				{
					Collider = collider,
					Normal = normal,
					Distance = 0f,
					GameObject = collider.gameObject,
					Type = HitType.PhysX
				};
				num++;
				hits.Add(item);
			}
		}
		return num;
	}

	private void PositionRotationInternal(ref Query query, out Vector3 position, out Quaternion rotation)
	{
		if (Runner.IsClient)
		{
			query.TickTo = null;
			query.Alpha = null;
		}
		else if ((query.Options & HitOptions.SubtickAccuracy) == 0 && query.Alpha.HasValue && query.Alpha.Value > 0.5f)
		{
			query.Tick++;
		}
		_hitboxBuffer.PositionQueryInternal(ref query, out position, out rotation);
	}

	private void Init()
	{
		_settings = Runner.Config.LagCompensation;
		Init(GetObjects(Runner));
	}

	private void Init(List<HitboxRoot> initialObjects)
	{
		int bufferSize = Mathf.Max(_settings.HitboxBufferSize, 2);
		int hitboxCapacity = ((_settings.HitboxCapacity < 16) ? 16 : _settings.HitboxCapacity);
		_hitboxBuffer = new HitboxBuffer(initialObjects, bufferSize, hitboxCapacity, _settings.ExpansionFactor);
	}

	private List<HitboxRoot> GetObjects(NetworkRunner runner)
	{
		List<HitboxRoot> list = new List<HitboxRoot>();
		SimulationBehaviour[] allBehaviours = runner.GetAllBehaviours(typeof(HitboxRoot));
		for (int i = 0; i < allBehaviours.Length; i++)
		{
			SimulationBehaviour simulationBehaviour = allBehaviours[i];
			while (BehaviourUtils.IsNotNull(simulationBehaviour))
			{
				if (simulationBehaviour.CanReceiveCallback)
				{
					HitboxRoot hitboxRoot = (HitboxRoot)simulationBehaviour;
					hitboxRoot.Manager = this;
					list.Add(hitboxRoot);
				}
				simulationBehaviour = simulationBehaviour.Next;
			}
		}
		return list;
	}

	public override void FixedUpdateNetwork()
	{
		if (Runner.IsShutdown)
		{
			return;
		}
		if (_hitboxBuffer == null)
		{
			Init();
		}
		Assert.Check(_hitboxBuffer != null);
		if (Runner.IsServer)
		{
			Profiler.BeginSample("Server Hitbox Manager");
			AdvanceAndRegister(Runner.Simulation.Tick, Runner.Simulation.Tick);
			Profiler.EndSample();
		}
		else
		{
			if (Runner.Simulation.InterpFrom == null || Runner.Stage == SimulationStages.Resimulate)
			{
				return;
			}
			Profiler.BeginSample("Client Hitbox Manager");
			AdvanceAndRegister(Runner.Simulation.Tick, Runner.Simulation.InterpFrom.Tick);
			Profiler.EndSample();
		}
		if (_hitboxBuffer.BVH != null)
		{
			BVHDepth = _hitboxBuffer.BVH.maxDepth;
			BVHNodes = _hitboxBuffer.BVH.UsedNodesCount;
		}
		TotalHitboxes = _hitboxBuffer.Current.CollidersCount;
	}

	private void AdvanceAndRegister(int tick, int dataTick)
	{
		Runner.InvokeOnBeforeHitboxRegistration();
		_hitboxBuffer.Advance(tick, dataTick);
		SimulationBehaviour[] allBehaviours = Runner.GetAllBehaviours(typeof(HitboxRoot));
		for (int i = 0; i < allBehaviours.Length; i++)
		{
			SimulationBehaviour simulationBehaviour = allBehaviours[i];
			while (BehaviourUtils.IsNotNull(simulationBehaviour))
			{
				if (simulationBehaviour.CanReceiveCallback)
				{
					HitboxRoot hitboxRoot = (HitboxRoot)simulationBehaviour;
					if (!hitboxRoot.Registered)
					{
						hitboxRoot.Manager = this;
						_hitboxBuffer.Add(hitboxRoot);
					}
					else
					{
						_hitboxBuffer.Update(hitboxRoot);
					}
				}
				simulationBehaviour = simulationBehaviour.Next;
			}
		}
		if (_settings.Optimize)
		{
			_hitboxBuffer.Optimize();
		}
	}

	internal bool Remove(HitboxRoot root)
	{
		return _hitboxBuffer.Remove(root);
	}

	private void OnDrawGizmos()
	{
		if (_hitboxBuffer != null)
		{
			Color color = ((BehaviourUtils.IsAlive(Runner) && Runner.IsClient) ? _settings.ClientDebugColor : _settings.DebugColor);
			_hitboxBuffer.DebugDraw(color, _settings.HistoryDebugColor, _settings.DebugBroadphase, _settings.DebugHistory);
		}
	}
}
