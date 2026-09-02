#define DEBUG
using System;
using System.Collections.Generic;
using Fusion.Sockets;

namespace Fusion;

public class SimulationInput
{
	public class Buffer
	{
		private NetworkProjectConfig _cfg;

		private Dictionary<Tick, SimulationInput> _map;

		private Dictionary<Tick, double> _time;

		public int Count => _map.Count;

		public bool Full => _map.Count == _cfg.Simulation.TickRate;

		public Buffer(NetworkProjectConfig cfg)
		{
			_cfg = cfg;
			_map = new Dictionary<Tick, SimulationInput>(new Tick.EqualityComparer());
			_time = new Dictionary<Tick, double>();
		}

		public void Clear()
		{
			_map.Clear();
			_time.Clear();
		}

		public int CopySortedTo(SimulationInput[] array)
		{
			Assert.Always(array.Length >= Count, "Array too small");
			Array.Clear(array, 0, array.Length);
			_map.Values.CopyTo(array, 0);
			ArraySpecialized.Sort(array, 0, _map.Count);
			for (int i = 0; i < _map.Count; i++)
			{
				Assert.Check(array[i]);
			}
			return _map.Count;
		}

		public bool Contains(Tick tick)
		{
			return _map.ContainsKey(tick);
		}

		public bool Remove(Tick tick, out SimulationInput removed)
		{
			_time.Remove(tick);
			_map.TryGetValue(tick, out removed);
			return _map.Remove(tick);
		}

		public double? GetInsertTime(Tick tick)
		{
			if (_time.TryGetValue(tick, out var value))
			{
				return value;
			}
			return null;
		}

		public SimulationInput Get(Tick tick)
		{
			if (_map.TryGetValue(tick, out var value))
			{
				Assert.Check(value);
				return value;
			}
			return null;
		}

		public IEnumerator<KeyValuePair<Tick, SimulationInput>> GetEnumerator()
		{
			foreach (KeyValuePair<Tick, SimulationInput> item in _map)
			{
				yield return item;
			}
		}

		public unsafe bool Add(SimulationInput input, double? insertTime = null)
		{
			Assert.Check(input);
			Assert.Always(_map.Count < _cfg.Simulation.TickRate, "_map.Count < _cfg.Simulation.TickRate");
			if (Contains(input.Header->Tick))
			{
				return false;
			}
			_map.Add(input.Header->Tick, input);
			if (insertTime.HasValue)
			{
				_time.Add(input.Header->Tick, insertTime.Value);
			}
			return true;
		}
	}

	internal class Pool
	{
		private Stack<SimulationInput> _pool;

		private List<SimulationInput> _created;

		private SimulationConfig _config;

		private bool _disposed;

		public Pool(SimulationConfig config)
		{
			_pool = new Stack<SimulationInput>();
			_created = new List<SimulationInput>();
			_config = config;
			_disposed = false;
		}

		public unsafe SimulationInput Acquire()
		{
			Assert.Check(!_disposed);
			SimulationInput simulationInput;
			if (_pool.Count > 0)
			{
				simulationInput = _pool.Pop();
				Assert.Check(simulationInput._pooled);
				simulationInput._pooled = false;
			}
			else
			{
				simulationInput = new SimulationInput();
				simulationInput._ptr = (int*)Native.MallocAndClear(_config.InputTotalWordCount * 4);
				_created.Add(simulationInput);
			}
			Assert.Check(simulationInput._sent == 0);
			Assert.Check(!simulationInput._pooled);
			Assert.Check(simulationInput._player == PlayerRef.None);
			return simulationInput;
		}

		public unsafe void Release(SimulationInput input)
		{
			Assert.Check(!_disposed);
			Assert.Check(!input._pooled);
			Native.MemClear(input.Header, _config.InputTotalWordCount * 4);
			input._sent = 0;
			input._pooled = true;
			input._player = default;
			_pool.Push(input);
		}

		public void Dispose()
		{
			_disposed = true;
			for (int i = 0; i < _created.Count; i++)
			{
				_created[i].Dispose();
			}
			_created = null;
			_pool = null;
		}
	}

	private int _sent;

	private bool _pooled;

	private PlayerRef _player;

	private unsafe int* _ptr;

	internal SimulationInput Prev;

	internal SimulationInput Next;

	public PlayerRef Player
	{
		get
		{
			Assert.Check(!_pooled);
			return _player;
		}
		set
		{
			Assert.Check(!_pooled);
			_player = value;
		}
	}

	public unsafe SimulationInputHeader* Header
	{
		get
		{
			Assert.Check(!_pooled);
			return (SimulationInputHeader*)_ptr;
		}
	}

	public unsafe int* Data
	{
		get
		{
			Assert.Check(!_pooled);
			return _ptr + 4;
		}
	}

	public int Sent
	{
		get
		{
			Assert.Check(!_pooled);
			return _sent;
		}
		set
		{
			Assert.Check(!_pooled);
			_sent = value;
		}
	}

	public unsafe void Clear(int wordCount)
	{
		Assert.Check(!_pooled);
		Native.MemClear(_ptr, wordCount * 4);
	}

	public unsafe void CopyFrom(SimulationInput source, int wordCount)
	{
		Assert.Check(!_pooled);
		Native.MemCpy(_ptr, source._ptr, wordCount * 4);
	}

	internal unsafe void Serialize(SimulationInput previous, SimulationConfig config, NetBitBufferSerializer serializer)
	{
		Assert.Check(!_pooled);
		int* ptr = _ptr;
		int* ptr2 = previous._ptr;
		if (config.Topology == SimulationConfig.Topologies.Shared)
		{
			if (serializer.Writing)
			{
				serializer.Buffer->WriteInt32VarLength(Header->Tick, 8);
			}
			else
			{
				Header->Tick = serializer.Buffer->ReadInt32VarLength(8);
			}
		}
		else if (serializer.Writing)
		{
			for (int i = 0; i < config.InputTotalWordCount; i++)
			{
				if (ptr[i] != ptr2[i])
				{
					serializer.Buffer->WriteBoolean(value: true);
					serializer.Buffer->WriteInt32VarLength(i, 8);
					serializer.Buffer->WriteInt64VarLength(Maths.ZigZagEncode((long)ptr[i] - (long)ptr2[i]), 8);
				}
			}
			serializer.Buffer->WriteBoolean(value: false);
		}
		else
		{
			Native.MemCpy(ptr, ptr2, config.InputTotalWordCount * 4);
			while (serializer.Buffer->ReadBoolean())
			{
				ptr[serializer.Buffer->ReadInt32VarLength(8)] += (int)Maths.ZigZagDecode(serializer.Buffer->ReadInt64VarLength(8));
			}
		}
	}

	internal unsafe void Dispose()
	{
		int* ptr = _ptr;
		_ptr = null;
		if (ptr != null)
		{
			Native.Free(ptr);
		}
	}
}
