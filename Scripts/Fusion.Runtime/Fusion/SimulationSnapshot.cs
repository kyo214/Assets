#define ENABLE_PROFILER
#define DEBUG
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Fusion;

public sealed class SimulationSnapshot
{
	public interface IHistory
	{
		SimulationSnapshot Oldest { get; }

		SimulationSnapshot Latest { get; }

		SimulationSnapshot Root { get; }

		int Count { get; }

		Tick MaxTick { get; }

		SimulationSnapshot this[int index] { get; }

		void Dispose();

		bool TryGet(Tick tick, out SimulationSnapshot snapshot);

		SimulationSnapshot Get(Tick tick);

		SimulationSnapshot Next(Tick tick, bool copyPrevious = true);

		void DisposeAllExcept(HashSet<Tick> save);

		void DisposeOlderThan(Tick tick);
	}

	internal class HistoryServerDeltaSnapshots : IHistory
	{
		private Pool _pool;

		private SimulationSnapshot _root;

		private List<SimulationSnapshot> _snapshots;

		private Dictionary<Tick, SimulationSnapshot> _lookup;

		public SimulationSnapshot Oldest => _snapshots[0];

		public SimulationSnapshot Latest => _snapshots[_snapshots.Count - 1];

		public SimulationSnapshot Root => _root;

		public int Count => _snapshots.Count;

		public Tick MaxTick => Latest.Tick;

		public SimulationSnapshot this[int index]
		{
			get
			{
				throw new NotImplementedException("Indexer not implemented for server-delta-snapshot history");
			}
		}

		public HistoryServerDeltaSnapshots(Pool pool)
		{
			_pool = pool;
			_root = pool.Acquire();
			_root.SetTick(0);
			_snapshots = new List<SimulationSnapshot>();
			_lookup = new Dictionary<Tick, SimulationSnapshot>(new Tick.EqualityComparer());
		}

		public void Dispose()
		{
			_lookup.Clear();
			_snapshots.Clear();
		}

		public bool TryGet(Tick tick, out SimulationSnapshot snapshot)
		{
			return _lookup.TryGetValue(tick, out snapshot);
		}

		public SimulationSnapshot Get(Tick tick)
		{
			SimulationSnapshot snapshot;
			return TryGet(tick, out snapshot) ? snapshot : null;
		}

		public SimulationSnapshot Next(Tick tick, bool copyPrevious = true)
		{
			copyPrevious = copyPrevious && Count > 0;
			SimulationSnapshot simulationSnapshot = _pool.Acquire();
			if (copyPrevious)
			{
				simulationSnapshot.CopyFrom(Latest);
			}
			simulationSnapshot.SetTick(tick);
			_snapshots.Add(simulationSnapshot);
			_lookup.Add(simulationSnapshot.Tick, simulationSnapshot);
			return simulationSnapshot;
		}

		public void DisposeAllExcept(HashSet<Tick> save)
		{
			for (int i = 0; i < _snapshots.Count; i++)
			{
				SimulationSnapshot simulationSnapshot = _snapshots[i];
				if (!save.Contains(simulationSnapshot.Tick))
				{
					_snapshots.RemoveAt(i--);
					_lookup.Remove(simulationSnapshot.Tick);
					_pool.Release(simulationSnapshot);
				}
			}
		}

		public void DisposeOlderThan(Tick tick)
		{
			for (int i = 0; i < _snapshots.Count; i++)
			{
				SimulationSnapshot simulationSnapshot = _snapshots[i];
				if (simulationSnapshot.Tick < tick)
				{
					_snapshots.RemoveAt(i--);
					_lookup.Remove(simulationSnapshot.Tick);
					_pool.Release(simulationSnapshot);
				}
			}
		}
	}

	internal class HistoryLL : IHistory
	{
		private Pool _pool;

		private SimulationSnapshot _root;

		private SimulationSnapshotList _list;

		private Dictionary<Tick, SimulationSnapshot> _lookup;

		public SimulationSnapshot Oldest => _list?.Head;

		public SimulationSnapshot Latest => _list?.Tail;

		public SimulationSnapshot Root => _root;

		public int Count => (_list?.Count).GetValueOrDefault();

		public Tick MaxTick => (_list?.Tail?.Tick).GetValueOrDefault();

		public SimulationSnapshot this[int index]
		{
			get
			{
				SimulationSnapshot simulationSnapshot = _list.Head;
				while (--index >= 0)
				{
					simulationSnapshot = simulationSnapshot.Next;
				}
				if (simulationSnapshot == null)
				{
					throw new IndexOutOfRangeException();
				}
				return simulationSnapshot;
			}
		}

		public HistoryLL(int history, Pool pool)
		{
			_pool = pool;
			_list = new SimulationSnapshotList();
			_lookup = new Dictionary<Tick, SimulationSnapshot>();
			_root = pool.Acquire();
			_root.SetTick(0);
		}

		public void Dispose()
		{
			_list = null;
			_pool = null;
			_root = null;
		}

		public bool TryGet(Tick tick, out SimulationSnapshot snapshot)
		{
			if (tick == 0)
			{
				snapshot = _root;
				return true;
			}
			return _lookup.TryGetValue(tick, out snapshot);
		}

		public SimulationSnapshot Get(Tick tick)
		{
			return (tick == 0) ? _root : _lookup[tick];
		}

		public SimulationSnapshot Next(Tick tick, bool copyPrevious = true)
		{
			SimulationSnapshot simulationSnapshot = _pool.Acquire(!copyPrevious);
			if (copyPrevious && Count > 0)
			{
				simulationSnapshot.CopyFrom(_list.Tail);
			}
			simulationSnapshot.SetTick(tick);
			simulationSnapshot._interp = false;
			_list.AddLast(simulationSnapshot);
			_lookup.Add(simulationSnapshot.Tick, simulationSnapshot);
			return simulationSnapshot;
		}

		public SimulationSnapshot ReplaceLatest(int newTick)
		{
			SimulationSnapshot latest = Latest;
			Tick tick = latest.Tick;
			latest.SetTick(newTick);
			_lookup.Add(newTick, latest);
			return latest;
		}

		public void DisposeAllExcept(HashSet<Tick> save)
		{
		}

		public void DisposeOlderThan(Tick tick)
		{
			while (_list.Count > 0 && _list.Head.Tick < tick)
			{
				SimulationSnapshot simulationSnapshot = _list.RemoveHead();
				_lookup.Remove(simulationSnapshot.Tick);
				_pool.Release(simulationSnapshot);
			}
		}
	}

	internal class Interpolator
	{
		private double _time;

		private double _timeScale;

		private float _alpha;

		private double _multiplier;

		private Ema _diffAvg;

		private Ema _uncertainAvg;

		private Tick _statsTick;

		private TimerDelta _stateDeltaTimer;

		private Ema _stateDeltaAvg;

		private SimulationSnapshot _to;

		private SimulationSnapshot _from;

		private InterpolationConfiguration _interpConfig;

		private SimulationConfig _simulationConfig;

		public SimulationSnapshot To => _to;

		public SimulationSnapshot From => _from;

		public float Alpha => _alpha;

		public double TimeScale => _timeScale;

		public double Uncertainty => _uncertainAvg.Val;

		public double Offset => CalculateInterpOffset();

		public Interpolator(SimulationConfig simulationConfig, InterpolationConfiguration interpConfig)
		{
			_interpConfig = interpConfig;
			_timeScale = 1.0;
			_simulationConfig = simulationConfig;
			_multiplier = GetDefaultMultiplier();
		}

		public bool Calculate(double dt, IHistory history, Simulation simulation)
		{
			_to = null;
			_from = null;
			_alpha = 0f;
			if (history.Count == 0)
			{
				return false;
			}
			_time += dt * _timeScale;
			if (!history.Latest._interp)
			{
				history.Latest._interp = true;
				double num = CalculateInterpOffset();
				double num2 = num - num * _interpConfig.SmoothAdjustRange;
				double num3 = num + num * _interpConfig.SmoothAdjustRange;
				double num4 = num - num * _interpConfig.SnapAdjustRange;
				double num5 = num + num * _interpConfig.SnapAdjustRange;
				double num6 = history.Latest.Time - _time;
				if (num6 <= num4 || num6 >= num5 || num6 <= 0.0 || _time >= history.Latest.Time)
				{
					_time = history.Latest.Time - num;
					_timeScale = 1.0;
					_multiplier = GetDefaultMultiplier();
				}
				else
				{
					_diffAvg.Add(num6);
					if (_diffAvg.Val <= num2)
					{
						_timeScale = 1.0 - _interpConfig.TimeAdjust;
					}
					else if (_diffAvg.Val >= num3)
					{
						_timeScale = 1.0 + _interpConfig.TimeAdjust;
					}
					else
					{
						_timeScale = 1.0;
					}
				}
				simulation.Stats.GetStatBuffer(Simulation.Statistics.SimStats.InterpDiff).Push((float)num6);
			}
			for (int i = 0; i < history.Count - 1; i++)
			{
				if (history[i].Time <= _time && history[i + 1].Time >= _time)
				{
					double num7 = history[i + 1].Time - history[i].Time;
					double num8 = _time - history[i].Time;
					_alpha = (float)Maths.Clamp01(num8 / num7);
					_from = history[i];
					_to = history[i + 1];
					break;
				}
			}
			if (_from == null)
			{
				Assert.Check(_to == null);
				if (history.Count == 1)
				{
					_alpha = 0f;
					_to = (_from = history.Latest);
				}
				else if (history.Count > 1)
				{
					if (_time <= history.Oldest.Time)
					{
						_alpha = 0f;
						_to = (_from = history.Oldest);
					}
					else if (_time >= history.Latest.Time)
					{
						_alpha = 0f;
						_to = (_from = history.Latest);
					}
					else
					{
						Assert.AlwaysFail($"_interpTime:{_time} _history.Oldest.Time:{history.Oldest.Time} _history.Latest.Time:{history.Latest.Time} _history.Count:{history.Count}");
					}
				}
			}
			else
			{
				Assert.Check(_to);
			}
			if (_statsTick != simulation.Tick)
			{
				EngineProfiler.InterpolationOffset((float)Offset);
				simulation.Stats.GetStatBuffer(Simulation.Statistics.SimStats.InterpOffset).Push((float)Offset);
				EngineProfiler.InterpolationTimeScale((float)TimeScale);
				simulation.Stats.GetStatBuffer(Simulation.Statistics.SimStats.InterpTimescale).Push((float)TimeScale);
				EngineProfiler.InterpolationMultiplier((float)_multiplier);
				simulation.Stats.GetStatBuffer(Simulation.Statistics.SimStats.InterpMultiplier).Push((float)_multiplier);
			}
			_statsTick = simulation.Tick;
			return true;
		}

		public bool StateUpdateReceived(Simulation simulation)
		{
			if (_stateDeltaTimer.IsRunning)
			{
				double num = _stateDeltaTimer.Peek();
				if (!(num >= _simulationConfig.ServerDeltaTime))
				{
					return false;
				}
				num = _stateDeltaTimer.Consume();
				simulation.Stats.GetStatBuffer(Simulation.Statistics.SimStats.InterpStateDelta).Push((float)num);
				_stateDeltaAvg.Add(num);
				_uncertainAvg.Add(_stateDeltaAvg.Dev);
				double num2 = _uncertainAvg.Val / _simulationConfig.ServerPacketDeltaTime;
				EngineProfiler.InterpolationUncertainty(1f - (float)num2);
				simulation.Stats.GetStatBuffer(Simulation.Statistics.SimStats.InterpUncertainty).Push(1f - (float)num2);
				_multiplier = 1.0 + num2;
				_multiplier = Maths.Clamp(Math.Round(_multiplier, 2), _interpConfig.MultiplierMin, _interpConfig.MultiplierMax);
			}
			else
			{
				_stateDeltaTimer = TimerDelta.StartNew();
				_stateDeltaAvg.Add(_simulationConfig.ServerPacketDeltaTime * _multiplier);
			}
			return true;
		}

		private double GetDefaultMultiplier()
		{
			return Maths.Lerp(_interpConfig.MultiplierMin, _interpConfig.MultiplierMax, 0.5);
		}

		private double CalculateInterpOffset()
		{
			return _stateDeltaAvg.Val * _multiplier;
		}
	}

	internal class Pool
	{
		private const int MAX_POOL_COUNT = 5;

		private Simulation _simulation;

		private NetworkProjectConfig _config;

		private Stack<SimulationSnapshot> _pool;

		private List<SimulationSnapshot> _created;

		public Pool(Simulation simulation, NetworkProjectConfig config)
		{
			_simulation = simulation;
			_config = config;
			_pool = new Stack<SimulationSnapshot>();
			_created = new List<SimulationSnapshot>();
		}

		public void Dispose()
		{
			_pool = null;
			for (int i = 0; i < _created.Count; i++)
			{
				_created[i].Dispose();
			}
			_created.Clear();
		}

		public void Release(SimulationSnapshot snapshot)
		{
			Assert.Always(snapshot.Prev == null, "prev");
			Assert.Always(snapshot.Next == null, "next");
			Assert.Always(!snapshot._pooled, "_pooled");
			if (_pool.Count >= 5)
			{
				_created.Remove(snapshot);
				snapshot.Dispose();
			}
			else
			{
				snapshot._pooled = true;
				_pool.Push(snapshot);
			}
		}

		public SimulationSnapshot Create()
		{
			SimulationSnapshot simulationSnapshot = new SimulationSnapshot(_config, _simulation.IsServer);
			_created.Add(simulationSnapshot);
			return simulationSnapshot;
		}

		public unsafe SimulationSnapshot Acquire(bool clearAllocator = true)
		{
			SimulationSnapshot simulationSnapshot;
			if (_pool.Count > 0)
			{
				simulationSnapshot = _pool.Pop();
				Assert.Check(simulationSnapshot._pooled);
				simulationSnapshot._pooled = false;
				if (clearAllocator)
				{
					Native.MemClear(simulationSnapshot.Allocator->Replicate, simulationSnapshot.Allocator->ReplicateByteLength);
				}
			}
			else
			{
				simulationSnapshot = Create();
			}
			simulationSnapshot._tick = default;
			return simulationSnapshot;
		}
	}

	private double _dt;

	private Tick _tick;

	private bool _pooled;

	private bool _interp;

	private unsafe Allocator* _allocator;

	internal SimulationSnapshot Prev;

	internal SimulationSnapshot Next;

	public Tick Tick
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return _tick;
		}
	}

	public double Time
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return (double)(int)_tick * _dt;
		}
	}

	internal unsafe Allocator* Allocator
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return _allocator;
		}
	}

	internal unsafe int ReplicateWordLength => Allocator->ReplicateWordLength;

	internal unsafe int ObjectCount
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return ObjectTable->Count;
		}
	}

	internal unsafe NetworkObjectRefMapPtr* ObjectTable
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			Assert.Check(!_pooled);
			Assert.Check(sizeof(SimulationGlobalState) == 128);
			return (NetworkObjectRefMapPtr*)((byte*)_allocator->Globals + 128);
		}
	}

	internal unsafe NetworkObjectRefMapPtr.Enumerable Objects
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return new NetworkObjectRefMapPtr.Enumerable(ObjectTable);
		}
	}

	internal unsafe SimulationGlobalState* GlobalState
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			Assert.Check(!_pooled);
			Assert.Check(sizeof(SimulationGlobalState) == 128);
			return (SimulationGlobalState*)Allocator->Globals;
		}
	}

	public unsafe byte[] GetStateBytes()
	{
		byte[] array = new byte[_allocator->ReplicateByteLength];
		fixed (byte* destination = array)
		{
			Native.MemCpy(destination, _allocator->Replicate, _allocator->ReplicateByteLength);
		}
		return array;
	}

	internal unsafe SimulationSnapshot(NetworkProjectConfig config, bool server)
	{
		_dt = config.Simulation.DeltaTime;
		_allocator = Fusion.Allocator.Create(config.Heap.ToAllocatorConfig());
		NetworkObjectRefMapPtr.InitializeMemory(ObjectTable, config.MaxNetworkedObjectCount);
	}

	internal unsafe void Dispose()
	{
		Fusion.Allocator.Dispose(_allocator);
		_allocator = null;
	}

	internal void SetTick(Tick tick)
	{
		_tick = tick;
	}

	internal unsafe void CopyFrom(SimulationSnapshot from, bool onlyUsedMemory = false)
	{
		EngineProfiler.Begin("SimulationSnapshot.CopyFrom");
		Fusion.Allocator.Copy(from.Allocator, Allocator, onlyUsedMemory);
		_tick = from._tick;
		EngineProfiler.End();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe NetworkObjectHeader* AllocateObject(NetworkId id, NetworkPrefabId type, int wordCount, NetworkId? nestingRoot = null, NetworkObjectNestingKey? nestingKey = null)
	{
		Assert.Check<NetworkId, NetworkPrefabId, int>(!_pooled, id, type, wordCount);
		void* ptr = Fusion.Allocator.AllocAndClear(_allocator, wordCount * 4);
		Ptr ptr2 = _allocator->Ptr(ptr);
		NetworkObjectHeader* ptr3 = (NetworkObjectHeader*)ptr;
		ptr3->Id = id;
		ptr3->Type = type;
		ptr3->WordCount = wordCount;
		ptr3->AreaOfInterestLayerMask = -1;
		if (nestingRoot.HasValue)
		{
			Assert.Check(nestingKey.HasValue);
			ptr3->NestingRoot = nestingRoot.Value;
			ptr3->NestingKey = nestingKey.Value;
		}
		NetworkObjectRefMapPtr.Add(ObjectTable, id, ptr2);
		return ptr3;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe bool ContainsObject(NetworkId id)
	{
		return id.IsValid && NetworkObjectRefMapPtr.Contains(ObjectTable, id);
	}

	internal unsafe (Dictionary<NetworkId, NetworkObjectHeaderPtr>, Dictionary<NetworkId, List<NetworkId>>) GetObjectHeaderPtrs()
	{
		Dictionary<NetworkId, NetworkObjectHeaderPtr> dictionary = new Dictionary<NetworkId, NetworkObjectHeaderPtr>();
		Dictionary<NetworkId, List<NetworkId>> dictionary2 = new Dictionary<NetworkId, List<NetworkId>>();
		NetworkObjectRefMapPtr.GetIterateBufferStartCount(ObjectTable, out var entries, out var start, out var count);
		for (int i = start; i < count; i++)
		{
			NetworkObjectRefMapPtr.Entry entry = entries[i];
			if (!entry.Id.IsValid || !entry.Ptr)
			{
				continue;
			}
			NetworkObjectHeader* ptr = GetObject(entry.Id);
			dictionary.Add(ptr->Id, new NetworkObjectHeaderPtr
			{
				Ptr = ptr
			});
			if (ptr->NestingRoot.IsValid)
			{
				if (!dictionary2.TryGetValue(ptr->NestingRoot, out var value))
				{
					dictionary2.Add(ptr->NestingRoot, value = new List<NetworkId>());
				}
				value.Add(ptr->Id);
			}
		}
		return (dictionary, dictionary2);
	}

	internal unsafe void FreeObject(NetworkId id)
	{
		Assert.Check(!_pooled);
		Allocator* allocator = Allocator;
		NetworkObjectRefMapPtr* objectTable = ObjectTable;
		if (NetworkObjectRefMapPtr.TryGet(objectTable, id, out var ptr))
		{
			NetworkObjectRefMapPtr.Remove(objectTable, id);
			NetworkObjectHeader* ptr2 = (NetworkObjectHeader*)allocator->Ptr(ptr);
			*ptr2 = default;
			Fusion.Allocator.Free(allocator, ptr2);
		}
		else
		{
			Assert.AlwaysFail($"Failed to find {id}");
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe NetworkObjectHeader* GetObject(NetworkId id)
	{
		Assert.Check(!_pooled, id);
		if (NetworkObjectRefMapPtr.TryGet(ObjectTable, id, out var ptr))
		{
			return (NetworkObjectHeader*)_allocator->Ptr(ptr);
		}
		Assert.AlwaysFail($"Failed to find {id}");
		return null;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe NetworkObjectHeader* TryGetObject(NetworkId id)
	{
		Assert.Check(!_pooled);
		if (NetworkObjectRefMapPtr.TryGet(ObjectTable, id, out var ptr))
		{
			return (NetworkObjectHeader*)_allocator->Ptr(ptr);
		}
		return null;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe bool TryGetObject(NetworkId id, out NetworkObjectHeader* header)
	{
		Assert.Check(!_pooled);
		if (NetworkObjectRefMapPtr.TryGet(ObjectTable, id, out var ptr))
		{
			header = (NetworkObjectHeader*)_allocator->Ptr(ptr);
			return true;
		}
		header = null;
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe bool TryGetObjectEntry(NetworkId id, out NetworkObjectRefMapPtr.Entry* entry)
	{
		Assert.Check(!_pooled);
		return NetworkObjectRefMapPtr.TryGetEntry(ObjectTable, id, out entry);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe NetworkObjectHeader* GetObjectFromEntry(NetworkObjectRefMapPtr.Entry* entry)
	{
		Assert.Check(!_pooled);
		return (NetworkObjectHeader*)_allocator->Ptr(entry->Ptr);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe bool TryGetBehaviourPointer(NetworkBehaviour behaviour, out int* ptr)
	{
		return (ptr = TryGetBehaviourPointer(behaviour)) != null;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe int* TryGetBehaviourPointer(NetworkBehaviour behaviour)
	{
		if (TryGetObject(behaviour.Object.Id, out var header))
		{
			return (int*)header + behaviour.WordOffset;
		}
		return null;
	}

	internal unsafe string BuildReport()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine($"SimulationSnapshot.ObjectCount: {ObjectCount}");
		if (Allocator != null)
		{
			stringBuilder.AppendLine($"Allocator.GetByteLengthForReplication: {Fusion.Allocator.GetByteLengthForReplication(Allocator)}");
			stringBuilder.AppendLine("Allocator.PrintDebugInfo: " + Fusion.Allocator.PrintDebugInfo(Allocator));
			stringBuilder.AppendLine($"Allocator.ReplicateByteLength: {Allocator->ReplicateByteLength}");
		}
		return stringBuilder.ToString();
	}

	internal unsafe string PrintObjects()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(PrintAllocator(_allocator));
		return stringBuilder.ToString();
	}

	private unsafe string PrintAllocator(Allocator* allocator)
	{
		NetworkObjectRefMapPtr* objectTable = ObjectTable;
		StringBuilder stringBuilder = new StringBuilder();
		NetworkObjectRefMapPtr.GetIterateBufferStartCount(objectTable, out var entries, out var start, out var count);
		for (int i = start; i < count; i++)
		{
			NetworkObjectRefMapPtr.Entry entry = entries[i];
			if (entry.Id.IsValid && (bool)entry.Ptr)
			{
				stringBuilder.AppendLine(entry.Id.ToString());
			}
		}
		return stringBuilder.ToString();
	}

	public static int LevelExperience(int level, int experiencePerStep)
	{
		return level * (level + 1) / 2 * experiencePerStep;
	}
}
