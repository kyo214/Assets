#define DEBUG
#define ENABLE_PROFILER
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Fusion.Sockets;
using UnityEngine;

namespace Fusion;

public abstract class Simulation : ILogBuilder, INetPeerGroupCallbacks
{
	public struct AreaOfInterest
	{
		public struct RadixQuery
		{
			public unsafe void* UserData;

			public Vector3 Position;

			public float RadiusSqr;

			public unsafe NetworkObjectHeader** Hits;

			public int HitsCapacity;

			public int HitsCount;

			public int ActiveIndex;

			public int Mask;
		}

		public struct RadixObject
		{
			public Vector3 Position;

			public Ptr Ptr;

			public int Mask;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public unsafe delegate void BurstInsertAndResolveDelegate(AreaOfInterest* aoi, Accuracy* accuracy, Allocator* allocator, NetworkObjectRefMapPtr* map);

		private const int OFFSET = 8192;

		private int _radixBufferCapacity;

		private unsafe Radix.SortTarget* _radixBuffer;

		private unsafe Radix.SortTarget* _radixBufferTemp;

		private unsafe RadixQuery* _radixQueries;

		private unsafe RadixQuery** _radixQueriesActive;

		private int _radixQueriesCount;

		private int _radixQueriesCapacity;

		private unsafe RadixObject* _radixObjects;

		private int _radixObjectsCount;

		private int _radixObjectsCapacity;

		private unsafe int* _radixP;

		private unsafe int* _radixC;

		public static BurstInsertAndResolveDelegate BurstInsertAndResolve;

		private const int NO_ACC_INVERSE = 1000;

		internal int QueryCount => _radixQueriesCount;

		internal unsafe RadixQuery* GetQuery(int index)
		{
			Assert.Check(index >= 0 && index < _radixQueriesCount);
			return _radixQueries + index;
		}

		internal unsafe static AreaOfInterest* Alloc()
		{
			AreaOfInterest* ptr = Native.MallocAndClear<AreaOfInterest>();
			ptr->_radixP = Native.MallocAndClearArrayMin1<int>(256);
			ptr->_radixC = Native.MallocAndClearArrayMin1<int>(1024);
			EnsureCapacity(ptr, ptr->_radixObjectsCapacity, ptr->_radixQueriesCapacity);
			return ptr;
		}

		internal unsafe static void Reset(AreaOfInterest* aoi, int objects, int queries)
		{
			aoi->_radixQueriesCount = 0;
			aoi->_radixObjectsCount = 0;
			EnsureCapacity(aoi, objects, queries);
		}

		internal unsafe static void Free(AreaOfInterest* aoi)
		{
			for (int i = 0; i < aoi->_radixQueriesCapacity; i++)
			{
				RadixQuery* ptr = aoi->_radixQueries + i;
				if (ptr->Hits != null)
				{
					Native.Free(ptr->Hits);
					ptr->Hits = null;
				}
			}
			if (aoi->_radixP != null)
			{
				Native.Free(aoi->_radixP);
				aoi->_radixP = null;
			}
			if (aoi->_radixC != null)
			{
				Native.Free(aoi->_radixC);
				aoi->_radixC = null;
			}
			if (aoi->_radixObjects != null)
			{
				Native.Free(aoi->_radixObjects);
				aoi->_radixObjects = null;
			}
			if (aoi->_radixQueries != null)
			{
				Native.Free(aoi->_radixQueries);
				aoi->_radixQueries = null;
			}
			if (aoi->_radixQueriesActive != null)
			{
				Native.Free(aoi->_radixQueriesActive);
				aoi->_radixQueriesActive = null;
			}
			if (aoi->_radixBuffer != null)
			{
				Native.Free(aoi->_radixBuffer);
				aoi->_radixBuffer = null;
			}
			if (aoi->_radixBufferTemp != null)
			{
				Native.Free(aoi->_radixBufferTemp);
				aoi->_radixBufferTemp = null;
			}
			*aoi = default;
			Native.Free(aoi);
		}

		private unsafe static void EnsureCapacity(AreaOfInterest* aoi, int objects, int queries)
		{
			if (objects > aoi->_radixObjectsCapacity)
			{
				if (aoi->_radixObjects != null)
				{
					Native.Free(aoi->_radixObjects);
					aoi->_radixObjects = null;
				}
				aoi->_radixObjectsCapacity = objects;
				aoi->_radixObjects = Native.MallocAndClearArrayMin1<RadixObject>(aoi->_radixObjectsCapacity);
			}
			if (queries > aoi->_radixQueriesCapacity)
			{
				if (aoi->_radixQueries != null)
				{
					for (int i = 0; i < aoi->_radixQueriesCapacity; i++)
					{
						RadixQuery* ptr = aoi->_radixQueries + i;
						if (ptr->Hits != null)
						{
							Native.Free(ptr->Hits);
							ptr->Hits = null;
						}
					}
					Native.Free(aoi->_radixQueries);
					aoi->_radixQueries = null;
				}
				if (aoi->_radixQueriesActive != null)
				{
					Native.Free(aoi->_radixQueriesActive);
					aoi->_radixQueriesActive = null;
				}
				aoi->_radixQueriesCapacity = queries;
				aoi->_radixQueries = Native.MallocAndClearArrayMin1<RadixQuery>(aoi->_radixQueriesCapacity);
				aoi->_radixQueriesActive = Native.MallocAndClearPtrArrayMin1<RadixQuery>(aoi->_radixQueriesCapacity);
			}
			int num = objects + queries * 2;
			if (num > aoi->_radixBufferCapacity)
			{
				if (aoi->_radixBuffer != null)
				{
					Native.Free(aoi->_radixBuffer);
					aoi->_radixBuffer = null;
				}
				if (aoi->_radixBufferTemp != null)
				{
					Native.Free(aoi->_radixBufferTemp);
					aoi->_radixBufferTemp = null;
				}
				aoi->_radixBufferCapacity = num;
				aoi->_radixBuffer = Native.MallocAndClearArrayMin1<Radix.SortTarget>(aoi->_radixBufferCapacity);
				aoi->_radixBufferTemp = Native.MallocAndClearArrayMin1<Radix.SortTarget>(aoi->_radixBufferCapacity);
			}
		}

		public unsafe static void Resolve(AreaOfInterest* aoi, Allocator* local)
		{
			int num = aoi->_radixObjectsCount + aoi->_radixQueriesCount * 2;
			Radix.Sort(aoi->_radixBuffer, aoi->_radixBufferTemp, num, aoi->_radixP, aoi->_radixC);
			RadixQuery* radixQueries = aoi->_radixQueries;
			RadixObject* radixObjects = aoi->_radixObjects;
			Radix.SortTarget* radixBuffer = aoi->_radixBuffer;
			RadixQuery** radixQueriesActive = aoi->_radixQueriesActive;
			int num2 = 0;
			int i = 0;
			int num3 = 0;
			for (; i < num; i++)
			{
				int userData = radixBuffer[i].UserData;
				if ((userData & 0x40000000) == 1073741824)
				{
					userData &= -1073741825;
					Assert.Check(userData < aoi->_radixQueriesCount);
					int num4 = num3++;
					RadixQuery* ptr = radixQueries + userData;
					Assert.Check(num4 >= 0);
					ptr->ActiveIndex = num4;
					radixQueriesActive[num4] = ptr;
					continue;
				}
				if ((userData & int.MinValue) == int.MinValue)
				{
					if (++num2 == aoi->_radixQueriesCount)
					{
						break;
					}
					userData &= 0x7FFFFFFF;
					Assert.Check(userData < aoi->_radixQueriesCount);
					int activeIndex = radixQueries[userData].ActiveIndex;
					Assert.Check(activeIndex >= 0, userData, activeIndex);
					if (activeIndex < --num3)
					{
						radixQueriesActive[activeIndex] = radixQueriesActive[num3];
						radixQueriesActive[activeIndex]->ActiveIndex = activeIndex;
					}
					continue;
				}
				RadixObject* ptr2 = radixObjects + userData;
				for (int j = 0; j < num3; j++)
				{
					RadixQuery* ptr3 = radixQueriesActive[j];
					Assert.Check(ptr3->ActiveIndex >= 0, j, ptr3->ActiveIndex);
					if ((ptr3->Mask & ptr2->Mask) == 0)
					{
						continue;
					}
					float num5 = ptr3->Position.z - ptr2->Position.z;
					if (ptr3->RadiusSqr > num5 * num5)
					{
						if (ptr3->HitsCount == ptr3->HitsCapacity)
						{
							ptr3->Hits = Native.DoublePtrArray(ptr3->Hits, ptr3->HitsCapacity);
							ptr3->HitsCapacity *= 2;
						}
						ptr3->Hits[ptr3->HitsCount++] = local->Ptr<NetworkObjectHeader>(ptr2->Ptr);
					}
				}
			}
		}

		internal unsafe static void AddQuery(AreaOfInterest* aoi, Accuracy accuracy, Vector3 position, float radius, int mask, void* userData)
		{
			Assert.Check(aoi->_radixQueriesCount < aoi->_radixQueriesCapacity);
			RadixQuery* ptr = aoi->_radixQueries + aoi->_radixQueriesCount;
			ptr->UserData = userData;
			ptr->Position = position;
			ptr->RadiusSqr = radius * radius;
			ptr->Mask = mask;
			if (ptr->Hits == null)
			{
				ptr->HitsCapacity = 256;
				ptr->Hits = Native.MallocAndClearPtrArrayMin1<NetworkObjectHeader>(ptr->HitsCapacity);
			}
			ptr->HitsCount = 0;
			ptr->ActiveIndex = -1;
			float num = accuracy._inverse;
			int num2 = 0;
			if (accuracy._value == 0f)
			{
				num = 1000f;
				num2 = (int)(position.x * 1000f);
			}
			else
			{
				num2 = ReadWriteUtilsForWeaver.CompressFloat(num, position.x);
			}
			num2 += (int)(8192f * num);
			int num3 = aoi->_radixQueriesCount * 2;
			Radix.SortTarget sortTarget = new Radix.SortTarget
			{
				SortData = num2 - (int)(radius * num),
				UserData = (aoi->_radixQueriesCount | 0x40000000)
			};
			aoi->_radixBuffer[num3] = sortTarget;
			sortTarget.SortData = num2 + (int)(radius * num);
			sortTarget.UserData = aoi->_radixQueriesCount | int.MinValue;
			aoi->_radixBuffer[num3 + 1] = sortTarget;
			aoi->_radixQueriesCount++;
		}

		public unsafe static void InsertObjects(AreaOfInterest* aoi, Accuracy accuracy, Allocator* allocator, NetworkObjectRefMapPtr* map)
		{
			int num = (int)(8192f * accuracy._inverse);
			NetworkObjectRefMapPtr.GetIterateBufferStartCount(map, out var entries, out var start, out var count);
			Radix.SortTarget sortTarget = default;
			int num2 = aoi->_radixQueriesCount * 2;
			for (int i = start; i < count; i++)
			{
				Ptr ptr = entries[i].Ptr;
				if (!ptr)
				{
					continue;
				}
				Assert.Check(entries[i].Id.IsValid);
				NetworkObjectHeader* ptr2 = allocator->Ptr<NetworkObjectHeader>(ptr);
				if (ptr2->TransformOffset > 0)
				{
					int* ptr3 = (int*)ptr2 + ptr2->TransformOffset;
					Vector3 position = ReadWriteUtilsForWeaver.ReadVector3(ptr3, accuracy._value);
					if (accuracy._value != 0f)
					{
						sortTarget.SortData = *ptr3 + num;
					}
					else
					{
						sortTarget.SortData = 8192000 + (int)(*(float*)ptr3 * 1000f);
					}
					sortTarget.UserData = aoi->_radixObjectsCount;
					aoi->_radixObjects[sortTarget.UserData].Mask = ptr2->AreaOfInterestLayerMask;
					aoi->_radixObjects[sortTarget.UserData].Position = position;
					aoi->_radixObjects[sortTarget.UserData].Ptr = ptr;
					aoi->_radixBuffer[num2 + aoi->_radixObjectsCount] = sortTarget;
					aoi->_radixObjectsCount++;
				}
			}
		}
	}

	internal class Client : Simulation
	{
		private PlayerRef _player;

		private unsafe NetConnection* _server;

		private bool _stateReceived;

		private bool _forceClientPredictionReset;

		private SimulationInput.Buffer _inputBuffer;

		private SimulationInput[] _inputArray;

		private double _inputOffsetTarget;

		private double _inputLastAdjustment;

		private Timer _inputAdjustClock;

		private List<SimulationPlayer.AOIQuery> _aoiQueries;

		private SimulationSnapshot.Interpolator _interpolator;

		private Queue<(PlayerRef, bool)> _playerJoinedLeftMessages;

		internal unsafe NetConnection* ServerConnection => _server;

		public double InterpolationTimeScale => _interpolator.TimeScale;

		public unsafe bool IsConnectedToServer => _server != null;

		public unsafe NetAddress ServerAddress => IsConnectedToServer ? _server->RemoteAddress : default(NetAddress);

		public unsafe double RttToServer => (_server == null) ? 0.0 : _server->RoundTripTime;

		public override PlayerRef LocalPlayer => _player;

		public override SimulationSnapshot LatestServerState => (_history.Count > 0) ? _history.Latest : _history.Root;

		public override IEnumerable<PlayerRef> ActivePlayers
		{
			get
			{
				int i = 0;
				while (i < MaxPlayers())
				{
					if (IsPlayerActive(i))
					{
						yield return i;
					}
					int num = i + 1;
					i = num;
				}
				unsafe bool IsPlayerActive(int index)
				{
					return (base.State.GlobalState->PlayersActive[index / 64] & (ulong)(1L << index % 64)) != 0;
				}
				unsafe int MaxPlayers()
				{
					return base.State.GlobalState->MaxPlayers;
				}
			}
		}

		private float GetRelaySlackMin()
		{
			if (ServerAddress.IsRelayAddr)
			{
				return Math.Max(1f, RELAY_SLACK);
			}
			return 1f;
		}

		private float GetRelaySlackMax()
		{
			if (ServerAddress.IsRelayAddr)
			{
				return Math.Max(1f, RELAY_SLACK);
			}
			return 1f;
		}

		internal unsafe override void OnNetworkShutdown()
		{
			if (_server != null)
			{
				NetPeerGroup.Disconnect(_netPeerGroup, _server);
				NetworkSend();
			}
			_server = null;
		}

		internal override double GetPlayerRtt(PlayerRef player)
		{
			if (player == LocalPlayer || player == PlayerRef.None)
			{
				return RttToServer;
			}
			return 0.0;
		}

		internal Client(SimulationArgs args)
			: base(args)
		{
			_interpolator = new SimulationSnapshot.Interpolator(_config, _projectConfig.Interpolation);
			_inputBuffer = new SimulationInput.Buffer(_projectConfig);
			_inputArray = new SimulationInput[_config.TickRate];
			_inputAdjustClock = Timer.StartNew();
			_aoiQueries = new List<SimulationPlayer.AOIQuery>();
			_playerJoinedLeftMessages = new Queue<(PlayerRef, bool)>();
		}

		public unsafe void Connect(NetAddress address, byte[] token = null, byte[] uniqueId = null)
		{
			NetPeerGroup.Connect(_netPeerGroup, address, token, uniqueId);
		}

		public unsafe void Connect(string ip, ushort port, byte[] token = null, byte[] uniqueId = null)
		{
			NetPeerGroup.Connect(_netPeerGroup, ip, port, token, uniqueId);
		}

		protected override void BeforeTick()
		{
			while (_playerJoinedLeftMessages.Count > 0 && _callbacks.CanReceivePlayerJoinLeaveCallbacks)
			{
				(PlayerRef, bool) tuple = _playerJoinedLeftMessages.Dequeue();
				var (player, _) = tuple;
				if (tuple.Item2)
				{
					_callbacks.PlayerJoined(player);
				}
				else
				{
					_callbacks.PlayerLeft(player);
				}
			}
		}

		protected override void OnPlayerJoinedLeftInternalMessage(PlayerRef player, bool joined)
		{
			_playerJoinedLeftMessages.Enqueue((player, joined));
		}

		protected unsafe override void NetworkConnected(NetConnection* connection)
		{
			_server = connection;
			_player = connection->RemoteConnectionId.GroupIndex;
		}

		protected unsafe override void NetworkDisconnected(NetConnection* connection)
		{
			try
			{
				Assert.Check(_server == connection);
				_server = null;
				_player = default;
				_callbacks.OnDisconnectedFromServer();
			}
			catch (Exception exn)
			{
				Log.Exception(this, exn);
			}
		}

		protected override void NetworkReceiveDone()
		{
			if (!_accumulator.Running && _history.Count > 0)
			{
				ResetClientPredictionState(2);
			}
		}

		protected override void NoSimulation()
		{
			UpdateInterpolation();
		}

		private void RunClientSidePredictionLoop(int ticks, SimulationStages stage)
		{
			EngineProfiler.Begin("Simulation.Client.RunClientSidePredictionLoop");
			_callbacks.OnBeforeClientSidePredictionReset();
			InvokeOnBeforeAllTicks(resimulation: true, ticks);
			for (int i = 0; i < ticks; i++)
			{
				StepSimulation(stage, i + 1 == ticks, i == 0, freeInput: false);
			}
			Assert.Check(_state.Tick == (int)_history.Latest.Tick + ticks);
			InvokeOnAfterAllTicks(resimulation: true, ticks);
			_callbacks.OnAfterClientSidePredictionReset();
			EngineProfiler.End();
		}

		protected unsafe override int BeforeSimulation()
		{
			EngineProfiler.Begin("Simulation.Client.BeforeSimulation");
			int num = 0;
			_aoiQueries.Clear();
			UpdateInterpolation();
			if (_stateReceived)
			{
				_stateReceived = false;
				if (IsConnectedToServer)
				{
					EngineProfiler.RoundTripTime((float)_server->RoundTripTime);
				}
				(SimulationInput[], int) sortedInputs = GetSortedInputs();
				SimulationInput[] item = sortedInputs.Item1;
				int item2 = sortedInputs.Item2;
				for (int i = 0; i < item2; i++)
				{
					if (item[i].Header->Tick <= _history.Latest.Tick && _inputBuffer.Remove(item[i].Header->Tick, out var removed))
					{
						_inputPool.Release(removed);
					}
				}
				switch (base.Topology)
				{
				case SimulationConfig.Topologies.Shared:
				{
					int num3 = (int)_state.Tick - (int)_history.Latest.Tick;
					if (num3 <= 0)
					{
						if (num3 >= -5)
						{
							int num4 = Math.Abs(num3) + 2;
							Log.Debug(this, $"added {num4} extra ticks {num3} {_state.Tick} {_history.Latest.Tick}");
							_accumulator.AddTicks(num4);
						}
						else
						{
							_state.SetTick(_history.Latest.Tick);
							_statePrevious.SetTick((int)_history.Latest.Tick - 1);
						}
					}
					break;
				}
				case SimulationConfig.Topologies.ClientServer:
					Assert.Check(base.IsPlayer);
					Assert.Check(_history.Count > 0);
					try
					{
						if (_forceClientPredictionReset)
						{
							_forceClientPredictionReset = false;
							ResetClientPredictionState(timeExtra: _inputOffsetTarget, ticksExtra: 2);
							break;
						}
						num = (int)_state.Tick - (int)_history.Latest.Tick;
						if (num > base.Config.MaxPrediction)
						{
							ResetClientPredictionState(2);
						}
						else if (num > 0)
						{
							_state.CopyFrom(_history.Latest);
							RunClientSidePredictionLoop(num, SimulationStages.Resimulate);
						}
						else if (num >= -5)
						{
							int num2 = Math.Abs(num) + 2;
							Log.Debug(this, $"added {num2} extra ticks {num} {_state.Tick} {_history.Latest.Tick}");
							_accumulator.AddTicks(num2);
						}
						else
						{
							ResetClientPredictionState(2);
						}
					}
					catch (Exception exn)
					{
						Log.Exception(this, exn);
					}
					break;
				}
			}
			EngineProfiler.End();
			return num;
		}

		internal override void AreaOfInterestQueryAdded(PlayerRef player, SimulationPlayer.AOIQuery query)
		{
			if (_config.Topology == SimulationConfig.Topologies.Shared)
			{
				Assert.Check(LocalPlayer == player);
				_aoiQueries.Add(query);
			}
		}

		protected override void BeforeNetworkRecv()
		{
			_stateReplicator.StateUpdateCountThisNetworkRecv = 0;
		}

		internal unsafe override void RecvPacket(NetConnection* connection, NetBitBuffer* buffer)
		{
			_interpolator.StateUpdateReceived(this);
			ReadInputFeedback(buffer);
			_stateReceived = true;
			_stateReplicator.StateUpdateCountThisNetworkRecv++;
			_stateReplicator.RecvPacket(connection, buffer);
		}

		internal unsafe override void SendPacket(NetConnection* connection, NetBitBuffer* buffer, SimulationPacketEnvelope* envelope)
		{
			WriteInput(buffer);
			if (_config.Topology == SimulationConfig.Topologies.Shared)
			{
				int num = Math.Min(4, _aoiQueries.Count);
				buffer->WriteByte((byte)num);
				for (int i = 0; i < num; i++)
				{
					SimulationPlayer.AOIQuery.Write(_aoiQueries[i], buffer);
				}
			}
			if (_stateReplicator.ClientToServer)
			{
				_stateReplicator.SendPacket(connection, buffer, envelope);
			}
		}

		internal unsafe override SimulationInput GetInput(Tick tick, PlayerRef player)
		{
			if (LocalPlayer != player)
			{
				return null;
			}
			if (base.IsResimulation)
			{
				return _inputBuffer.Get(tick);
			}
			if (_inputBuffer.Full)
			{
				if (base.Topology != SimulationConfig.Topologies.Shared)
				{
					return null;
				}
				_inputBuffer.Clear();
			}
			SimulationInput simulationInput = _inputPool.Acquire();
			simulationInput.Player = LocalPlayer;
			simulationInput.Header->Tick = _state.Tick;
			simulationInput.Header->InterpTo = _interpTo.Tick;
			simulationInput.Header->InterpFrom = _interpFrom.Tick;
			simulationInput.Header->InterpAlpha = _interpAlpha;
			_callbacks.OnInput(simulationInput);
			if (_inputBuffer.Add(simulationInput))
			{
				return simulationInput;
			}
			_inputPool.Release(simulationInput);
			return null;
		}

		private (SimulationInput[], int) GetSortedInputs()
		{
			int item = _inputBuffer.CopySortedTo(_inputArray);
			return (_inputArray, item);
		}

		private void UpdateInterpolation()
		{
			_interpolator.Calculate(_updateDelta, _history, this);
			_interpAlpha = _interpolator.Alpha;
			_interpFrom = _interpolator.From;
			_interpTo = _interpolator.To;
		}

		private unsafe void ReadInputFeedback(NetBitBuffer* buffer)
		{
			double num = buffer->ReadDouble();
			double num2 = buffer->ReadDouble();
			double num3 = buffer->ReadDouble();
			double num4 = buffer->ReadDouble();
			EngineProfiler.InputOffset((float)num);
			Stats.GetStatBuffer(Statistics.SimStats.InputOffset).Push((float)num);
			EngineProfiler.InputOffsetDeviation((float)num2);
			Stats.GetStatBuffer(Statistics.SimStats.InputOffsetDeviation).Push((float)num2);
			EngineProfiler.InputRecvDelta((float)num3);
			Stats.GetStatBuffer(Statistics.SimStats.InputReceiveDelta).Push((float)num3);
			EngineProfiler.InputRecvDeltaDeviation((float)num4);
			Stats.GetStatBuffer(Statistics.SimStats.InputReceiveDeltaDeviation).Push((float)num4);
			double roundTripTime = _server->RoundTripTime;
			double num5 = num3 + num4;
			double num6 = num;
			double num7 = Math.Max(0.33, Maths.Clamp01(num2 / _stepDeltaDouble));
			double num8 = (_inputOffsetTarget = Math.Max(_config.ServerDeltaTime, (num2 + num5) * num7) * (double)GetRelaySlackMin());
			double num9 = num8;
			double num10 = num8 * ((1.0 + Maths.Clamp01(Math.Max(1.5, num7))) * (double)GetRelaySlackMax());
			Stats.GetStatBuffer(Statistics.SimStats.InputOffsetTarget).Push((float)num8);
			if (num9 >= num6)
			{
				_accumulator.TimeScale = 0.99;
				if (num < 0.0 && _inputLastAdjustment + roundTripTime * 3.0 < _inputAdjustClock.ElapsedInSeconds)
				{
					_inputLastAdjustment = _inputAdjustClock.ElapsedInSeconds;
					_accumulator.AddTime(Math.Abs(num), _stepDeltaDouble, base.Config.TickRate / 2);
				}
			}
			else if (num10 <= num6)
			{
				_inputLastAdjustment = _inputAdjustClock.ElapsedInSeconds;
				if (num6 >= 5.0 * num10)
				{
					_forceClientPredictionReset = true;
					_accumulator.TimeScale = 1.0;
				}
				else if (num6 >= 3.0 * num10)
				{
					_accumulator.TimeScale = 1.02;
				}
				else
				{
					_accumulator.TimeScale = 1.01;
				}
			}
			else
			{
				_accumulator.TimeScale = 1.0;
			}
			EngineProfiler.SimulationTimeScale((float)_accumulator.TimeScale);
			Stats.GetStatBuffer(Statistics.SimStats.SimulationTimeScale).Push((float)_accumulator.TimeScale);
		}

		private static string NullableToString<T>(T? value) where T : struct
		{
			if (value.HasValue)
			{
				return value.Value.ToString();
			}
			return "null";
		}

		private void ResetClientPredictionState(int? ticksExtra = 2, double? timeExtra = null)
		{
			Log.Debug(this, $"reset client prediction state to {_history.Latest.Tick} (ticks:{NullableToString(ticksExtra)}, time:{NullableToString(timeExtra)})");
			if (_config.Topology == SimulationConfig.Topologies.ClientServer)
			{
				_state.CopyFrom(_history.Latest);
				_statePrevious.CopyFrom(_state);
			}
			_state.SetTick(_history.Latest.Tick);
			_statePrevious.SetTick(_state.Tick);
			foreach (KeyValuePair<Tick, SimulationInput> item in _inputBuffer)
			{
				_inputPool.Release(item.Value);
			}
			_inputBuffer.Clear();
			_stateReceived = false;
			_accumulator = TickAccumulator.StartNew();
			if (timeExtra.HasValue)
			{
				_accumulator.AddTime(timeExtra.Value, _stepDeltaDouble, base.Config.TickRate / 2);
			}
			if (ticksExtra.HasValue)
			{
				_accumulator.AddTicks(ticksExtra.Value);
			}
		}

		private unsafe void WriteInput(NetBitBuffer* buffer)
		{
			int num = Maths.Clamp(Mathf.CeilToInt((float)(_server->Rtt / _config.DeltaTime)), 3, 6);
			(SimulationInput[], int) sortedInputs = GetSortedInputs();
			SimulationInput[] item = sortedInputs.Item1;
			int item2 = sortedInputs.Item2;
			int i = Math.Max(0, item2 - num);
			int num2 = i;
			EngineProfiler.InputQueue(item2 - num2);
			int offsetBits = buffer->OffsetBits;
			NetBitBufferSerializer serializer = NetBitBufferSerializer.Writer(buffer);
			serializer.Buffer->WriteInt32(item2 - num2);
			for (; i < item2; i++)
			{
				while (true)
				{
					int offsetBits2 = serializer.Buffer->OffsetBits;
					if (i == num2)
					{
						item[i].Serialize(_inputRoot, _config, serializer);
					}
					else
					{
						item[i].Serialize(item[i - 1], _config, serializer);
					}
					if (!serializer.Buffer->OverflowOrLessThanOneByteRemaining)
					{
						break;
					}
					serializer.Buffer->OffsetBits = offsetBits2;
					serializer.Buffer->ReplaceDataFromBlockWithTemp(serializer.Buffer->LengthBytes * 2);
				}
			}
			int b = buffer->OffsetBits - offsetBits;
			EngineProfiler.InputSize(Maths.BytesRequiredForBits(b));
		}
	}

	internal interface ICallbacks
	{
		bool IsSharedModeMasterClient { get; }

		bool CanReceivePlayerJoinLeaveCallbacks { get; }

		void OnTick();

		void OnServerStart();

		void OnInput(SimulationInput input);

		void OnInputMissing(SimulationInput input);

		unsafe void OnMessage(SimulationMessage* message);

		void OnAfterClientSidePredictionReset();

		void OnBeforeClientSidePredictionReset();

		void OnAfterTick();

		void OnBeforeTick();

		void OnAfterAllTicks(bool resimulation, int tickCount);

		void OnBeforeAllTicks(bool resimulation, int tickCount);

		void OnAfterSimulation();

		void OnBeforeSimulation();

		void OnBeforeCopyPreviousState();

		void OnConnectedToServer();

		void OnDisconnectedFromServer();

		bool OnConnectionRequest(NetAddress remoteAddress, byte[] token);

		void OnConnectionFailed(NetAddress remoteAddress, NetConnectFailedReason reason);

		void OnReliableData(PlayerRef player, byte[] dataArray);

		void PlayerJoined(PlayerRef player);

		void PlayerLeft(PlayerRef player);

		bool TryBeginUpdateRemotePrefabs();

		void EndUpdateRemotePrefabs();

		unsafe bool CreateRemotePrefab(NetworkObjectHeader* header);

		bool DestroyRemotePrefab(NetworkId id, bool exists);

		void OnInternalConnectionAttempt(int attempt, int totalConnectionAttempts, out bool shouldChange, out NetAddress newAddress);

		void ObjectReceivedUpdate(NetworkId id, int tick);

		void ObjectStateAuthorityChanged(NetworkId id);

		string[] GetDefaultInterestGroups(NetworkId id);
	}

	public interface IDeltaCompressor
	{
		unsafe void Pack(int* current, int* shared, int words, NetBitBuffer* buffer);

		unsafe void Unpack(int* target, int bitCount, NetBitBuffer* buffer);
	}

	private class DeltaCompressorDebug : IDeltaCompressor
	{
		public unsafe void Pack(int* current, int* shared, int words, NetBitBuffer* buffer)
		{
			for (int i = 0; i < words; i++)
			{
				if (current[i] != 0)
				{
					buffer->WriteInt32(i);
					buffer->WriteInt32(current[i]);
				}
			}
		}

		public unsafe void Unpack(int* target, int bitCount, NetBitBuffer* buffer)
		{
			while (buffer->OffsetBits < bitCount)
			{
				target[buffer->ReadInt32()] = buffer->ReadInt32();
			}
		}
	}

	private class DeltaCompressorDefault : IDeltaCompressor
	{
		public unsafe void Pack(int* current, int* shared, int words, NetBitBuffer* buffer)
		{
			int num = 0;
			for (int i = 0; i < words; i++)
			{
				if (shared[i] != current[i])
				{
					long i2 = (long)current[i] - (long)shared[i];
					int num2 = i - num;
					Assert.Check(num2 >= 0);
					buffer->WriteInt32VarLength(num2, 3);
					buffer->WriteInt64VarLength(Maths.ZigZagEncode(i2), 6);
					num = i;
				}
			}
		}

		public unsafe void Unpack(int* target, int bitCount, NetBitBuffer* buffer)
		{
			int num = 0;
			while (buffer->OffsetBits < bitCount)
			{
				num += buffer->ReadInt32VarLength(3);
				long num2 = Maths.ZigZagDecode(buffer->ReadInt64VarLength(6));
				target[num] = (int)(target[num] + num2);
			}
		}
	}

	private enum TargetObjectVerificationResult
	{
		Ok = 0,
		ObjectNotConfirmed = 1,
		TargetNotInterestedInObject = 2
	}

	internal class Server : Simulation
	{
		private bool _joinedHostPlayer;

		private HashSet<Tick> _clientTicks;

		public override PlayerRef LocalPlayer => base.IsPlayer ? ((PlayerRef)(_config.DefaultPlayers - 1)) : PlayerRef.None;

		public override SimulationSnapshot LatestServerState => _state;

		internal unsafe override double GetPlayerRtt(PlayerRef player)
		{
			if (LocalPlayer == player)
			{
				return 0.0;
			}
			SimulationConnection simulationConnection = _connections[(int)player];
			if (simulationConnection.Connection != null)
			{
				return simulationConnection.Connection->RoundTripTime;
			}
			return 0.0;
		}

		internal Server(SimulationArgs args)
			: base(args)
		{
			_clientTicks = new HashSet<Tick>(new Tick.EqualityComparer());
		}

		internal unsafe void Disconnect(PlayerRef player)
		{
			if (player.IsValid)
			{
				Assert.Check<PlayerRef, int, int>((int)player >= 0 && (int)player < _connections.Length, player, (int)player, _connections.Length);
				SimulationConnection simulationConnection = _connections[(int)player];
				if (simulationConnection.Connection != null)
				{
					NetPeerGroup.DisconnectInternal(_netPeerGroup, simulationConnection.Connection, NetDisconnectReason.Requested);
				}
			}
		}

		internal unsafe void Disconnect(NetAddress address)
		{
			for (int i = 0; i < _connections.Length; i++)
			{
				if (_connections[i].Connection != null && _connections[i].Connection->Address.Equals(address))
				{
					NetPeerGroup.DisconnectInternal(_netPeerGroup, _connections[i].Connection, NetDisconnectReason.Requested);
					break;
				}
			}
		}

		protected override void AfterUpdate()
		{
			_clientTicks.Clear();
			for (int i = 0; i < _connections.Length; i++)
			{
				_clientTicks.Add(_connections[i].SharedTick);
				HashSet<Tick>.Enumerator enumerator = _connections[i].PendingTicks.GetEnumerator();
				while (enumerator.MoveNext())
				{
					_clientTicks.Add(enumerator.Current);
				}
				enumerator.Dispose();
			}
			_history.DisposeAllExcept(_clientTicks);
			base.AfterUpdate();
		}

		protected unsafe override void NetworkDisconnected(NetConnection* connection)
		{
		}

		internal unsafe override void RecvPacket(NetConnection* connection, NetBitBuffer* buffer)
		{
			ReadInput(connection, buffer);
			if (_stateReplicator.ClientToServer)
			{
				_stateReplicator.RecvPacket(connection, buffer);
			}
		}

		internal unsafe override void SendPacket(NetConnection* connection, NetBitBuffer* buffer, SimulationPacketEnvelope* envelope)
		{
			WriteInputFeedback(buffer, GetSimulationConnection(connection));
			_stateReplicator.SendPacket(connection, buffer, envelope);
		}

		internal unsafe void CreateInternalStateObject()
		{
			if (_state.TryGetObject(NetworkId.InternalState) == null)
			{
				Log.Debug(this, "Creating internal state network object");
				NetworkObjectHeader* ptr = _state.AllocateObject(NetworkId.InternalState, default, 20 + base.Config.DefaultPlayers * 2);
				ptr->Flags |= NetworkObjectHeaderFlags.NoPrefab;
			}
		}

		private unsafe void InvokePlayerJoined(PlayerRef player)
		{
			_state.GlobalState->PlayersActive[(int)player / 64] |= (ulong)(1L << (int)player % 64);
			_callbacks.PlayerJoined(player);
		}

		protected unsafe override void BeforeTick()
		{
			if (_state.Tick == 1)
			{
				CreateInternalStateObject();
				_callbacks.OnServerStart();
			}
			if (!_callbacks.CanReceivePlayerJoinLeaveCallbacks)
			{
				return;
			}
			if (base.IsPlayer && !_joinedHostPlayer)
			{
				_joinedHostPlayer = true;
				InvokePlayerJoined(LocalPlayer);
			}
			bool flag = false;
			for (int i = 0; i < _connections.Length; i++)
			{
				if (_connections[i].Active)
				{
					flag = true;
					break;
				}
			}
			SimulationMessageInternal_PlayerJoinedLeft buffer = default;
			SimulationMessageInternal_PlayerJoinedLeft buffer2 = default;
			for (int j = 0; j < _connections.Length; j++)
			{
				SimulationConnection simulationConnection = _connections[j];
				if (simulationConnection.InvokeJoined)
				{
					simulationConnection.InvokeJoined = false;
					InvokePlayerJoined(j);
					buffer.Joined = 1;
					buffer.Player = j;
					SendInternalSimulationMessage(SimulationMessageInternalTypes.PlayerJoinedLeft, buffer);
				}
				if (simulationConnection.InvokeLeave)
				{
					simulationConnection.InvokeLeave = false;
					_callbacks.PlayerLeft(j);
					_state.GlobalState->PlayersActive[j / 64] &= (ulong)(~(1L << j % 64));
					if (flag)
					{
						buffer2.Joined = 0;
						buffer2.Player = j;
						SendInternalSimulationMessage(SimulationMessageInternalTypes.PlayerJoinedLeft, buffer2);
					}
				}
			}
		}

		internal unsafe override SimulationInput GetInput(Tick tick, PlayerRef player)
		{
			if (base.IsPlayer && LocalPlayer == player)
			{
				SimulationInput simulationInput = _inputPool.Acquire();
				simulationInput.Player = LocalPlayer;
				simulationInput.Header->Tick = _state.Tick;
				simulationInput.Header->InterpTo = _state.Tick;
				simulationInput.Header->InterpFrom = _statePrevious.Tick;
				simulationInput.Header->InterpAlpha = base.StateAlpha;
				_callbacks.OnInput(simulationInput);
				return simulationInput;
			}
			if (!_connections[(int)player].Active)
			{
				return null;
			}
			SimulationInput simulationInput2 = _connections[(int)player]._inputs.Get(tick);
			if (simulationInput2 == null)
			{
				if (_config.Topology == SimulationConfig.Topologies.ClientServer)
				{
					simulationInput2 = _inputPool.Acquire();
					simulationInput2.Player = player;
					simulationInput2.Header->Tick = tick;
					simulationInput2.Header->InterpTo = tick;
					simulationInput2.Header->InterpFrom = tick;
					simulationInput2.Header->InterpAlpha = 0f;
					try
					{
						_callbacks.OnInputMissing(simulationInput2);
					}
					catch (Exception exn)
					{
						Log.Exception(this, exn);
					}
				}
			}
			else
			{
				double? insertTime = _connections[(int)player]._inputs.GetInsertTime(tick);
				Assert.Check(insertTime.HasValue);
				if (insertTime.HasValue)
				{
					_connections[(int)player].InputReceiveDelta(tick, insertTime.Value, _updateTime);
				}
				_connections[(int)player]._inputs.Remove(tick, out var _);
			}
			if (_config.Topology == SimulationConfig.Topologies.Shared)
			{
				if (simulationInput2 != null)
				{
					_inputPool.Release(simulationInput2);
				}
				return null;
			}
			return simulationInput2;
		}

		private unsafe static void WriteInputFeedback(NetBitBuffer* buffer, SimulationConnection ci)
		{
			buffer->WriteDouble(ci._inputsOffsetDelta.Val);
			buffer->WriteDouble(ci._inputsOffsetDelta.Dev);
			buffer->WriteDouble(ci._packetRecvDelta.Val);
			buffer->WriteDouble(ci._packetRecvDelta.Dev);
		}

		private unsafe void ReadInput(NetConnection* connection, NetBitBuffer* buffer)
		{
			try
			{
				SimulationConnection simulationConnection = GetSimulationConnection(connection);
				NetBitBufferSerializer serializer = NetBitBufferSerializer.Reader(buffer);
				int num = serializer.Buffer->ReadInt32();
				if (num <= 0)
				{
					return;
				}
				SimulationInput inputRoot = _inputRoot;
				inputRoot.Clear(_config.InputTotalWordCount);
				for (int i = 0; i < num; i++)
				{
					SimulationInput simulationInput = _inputPool.Acquire();
					simulationInput.Player = connection->LocalConnectionId.GroupIndex;
					simulationInput.Serialize(inputRoot, _config, serializer);
					inputRoot.CopyFrom(simulationInput, _config.InputTotalWordCount);
					if (simulationInput.Header->Tick > _state.Tick)
					{
						if (simulationConnection._inputs.Full)
						{
							_inputPool.Release(simulationInput);
						}
						else if (!simulationConnection._inputs.Add(simulationInput, _updateTime))
						{
							_inputPool.Release(simulationInput);
						}
					}
					else
					{
						if (_tickUpdateTimes.TryGetValue(simulationInput.Header->Tick, out var value))
						{
							simulationConnection.InputReceiveDelta(simulationInput.Header->Tick, _updateTime, value);
						}
						else
						{
							simulationConnection.InputReceiveDelta(simulationInput.Header->Tick, _updateTime, (double)(int)simulationInput.Header->Tick * _stepDeltaDouble);
						}
						_inputPool.Release(simulationInput);
					}
				}
			}
			finally
			{
			}
		}
	}

	internal abstract class StateReplicator
	{
		protected Simulation Simulation { get; }

		internal int StateUpdateCountThisNetworkRecv { get; set; }

		public virtual bool UseObjectInterest => false;

		public virtual bool ClientToServer => false;

		public virtual Tick SharedTick { get; }

		protected StateReplicator(Simulation simulation)
		{
			Simulation = simulation;
		}

		public unsafe abstract void SendPacket(NetConnection* connection, NetBitBuffer* buffer, SimulationPacketEnvelope* envelope);

		public unsafe abstract void RecvPacket(NetConnection* connection, NetBitBuffer* buffer);

		public abstract void SendBegin();

		public abstract void SendEnd();

		public unsafe abstract void OnPacketLost(NetConnection* c, SimulationPacketEnvelope* envelope);

		public unsafe abstract void OnPacketDelivered(NetConnection* c, SimulationPacketEnvelope* envelope);

		public virtual void Dispose()
		{
		}

		public virtual void UpdateRemotePrefabs()
		{
		}

		public virtual void OnObjectDestroyed(NetworkId id, NetworkObjectDestroyFlags flags)
		{
		}

		public virtual void OnObjectSpawnedLocal(NetworkId id)
		{
		}

		public virtual void OnObjectInterestGroupChange(PlayerRef player, NetworkId id, string group, bool interested)
		{
		}

		internal virtual bool HasObjectInterest(PlayerRef player, NetworkId id)
		{
			return true;
		}

		internal virtual void AssertVerifyNothingIsPending()
		{
		}
	}

	private class StateReplicatorDeltaSnapshot : StateReplicator
	{
		private struct BitBufferPtr
		{
			public unsafe NetBitBuffer* Ptr;
		}

		private HashSet<NetworkId> _removed;

		private HashSet<NetworkId> _created;

		private Tick _sharedTick;

		private Dictionary<Tick, BitBufferPtr> _deltaBuffers;

		private unsafe NetBitBufferBlock* _deltaBuffersBlock;

		public override Tick SharedTick => _sharedTick;

		public override bool UseObjectInterest => false;

		public unsafe StateReplicatorDeltaSnapshot(Simulation simulation)
			: base(simulation)
		{
			_created = new HashSet<NetworkId>(new NetworkId.EqualityComparer());
			_removed = new HashSet<NetworkId>();
			int num = simulation.ProjectConfig.Heap.PageCount * (1 << (int)simulation.ProjectConfig.Heap.PageShift) + simulation.ProjectConfig.Heap.GlobalsSize;
			_deltaBuffers = new Dictionary<Tick, BitBufferPtr>(new Tick.EqualityComparer());
			_deltaBuffersBlock = NetBitBufferBlock.Create(num * 2);
		}

		public unsafe override void Dispose()
		{
			NetBitBufferBlock.Dispose(_deltaBuffersBlock);
			_deltaBuffersBlock = null;
		}

		public unsafe override void UpdateRemotePrefabs()
		{
			if (base.Simulation.IsServer)
			{
				return;
			}
			NetworkObjectRefMapPtr.GetIterateBufferStartCount(base.Simulation.State.ObjectTable, out var entries, out var start, out var count);
			for (int i = start; i < count; i++)
			{
				NetworkId id = entries[i].Id;
				if ((bool)id && !_created.Contains(id))
				{
					Assert.Check(base.Simulation._history.Latest.ContainsObject(id));
					if (base.Simulation._callbacks.CreateRemotePrefab(base.Simulation.State.GetObject(id)))
					{
						_created.Add(id);
					}
				}
			}
			HashSet<NetworkId>.Enumerator enumerator = _created.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					NetworkId current = enumerator.Current;
					if (!base.Simulation.State.TryGetObject(current, out var _))
					{
						_removed.Add(current);
						base.Simulation._callbacks.DestroyRemotePrefab(current, exists: false);
					}
				}
			}
			finally
			{
				enumerator.Dispose();
			}
			HashSet<NetworkId>.Enumerator enumerator2 = _removed.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					_created.Remove(enumerator2.Current);
				}
			}
			finally
			{
				enumerator2.Dispose();
				_removed.Clear();
			}
		}

		public override void OnObjectDestroyed(NetworkId id, NetworkObjectDestroyFlags flags)
		{
			if (flags.Get(NetworkObjectDestroyFlags.DestroyState))
			{
				base.Simulation.State.FreeObject(id);
			}
			else if (flags.Get(NetworkObjectDestroyFlags.DestroyedByEngine) && !_created.Remove(id))
			{
			}
		}

		public unsafe override void SendPacket(NetConnection* connection, NetBitBuffer* buffer, SimulationPacketEnvelope* envelope)
		{
			SimulationConnection simulationConnection = base.Simulation.GetSimulationConnection(connection);
			if (simulationConnection.SharedTick == 0 && simulationConnection.PendingSnapshots > 0)
			{
				envelope->HasSnapshot = false;
				buffer->WriteBoolean(value: false);
				return;
			}
			envelope->HasSnapshot = true;
			SimulationSnapshot simulationSnapshot = base.Simulation.FindSnapshot(simulationConnection.SharedTick, defaultToRoot: true);
			Assert.Check(envelope->Tick == base.Simulation._history.Latest.Tick);
			Assert.Check(simulationSnapshot.Tick == 0 || simulationSnapshot.Tick == simulationConnection.SharedTick);
			Assert.Check(base.Simulation._history.Latest.Tick > simulationSnapshot.Tick);
			buffer->WriteBoolean(value: true);
			buffer->WriteInt32VarLength(base.Simulation._history.Latest.Tick);
			buffer->WriteInt32VarLength((int)base.Simulation._history.Latest.Tick - (int)simulationSnapshot.Tick);
			simulationConnection.PendingTicks.Add(envelope->Tick);
			if (!_deltaBuffers.TryGetValue(simulationSnapshot.Tick, out var value))
			{
				NetBitBuffer* ptr = _deltaBuffersBlock->TryAcquire();
				Allocator.DeltaPack(base.Simulation._deltaCompressor, base.Simulation._history.Latest.Allocator, simulationSnapshot.Allocator, ptr);
				_deltaBuffers.Add(simulationSnapshot.Tick, value = new BitBufferPtr
				{
					Ptr = ptr
				});
			}
			if (value.Ptr->OffsetBytes >= buffer->BytesRemaining)
			{
				buffer->ReplaceDataFromBlockWithTemp(value.Ptr->OffsetBytes + buffer->LengthBytes);
			}
			buffer->WriteBytesAligned(value.Ptr->Data, value.Ptr->OffsetBytes);
			simulationConnection.PendingSnapshots++;
		}

		public unsafe override void RecvPacket(NetConnection* connection, NetBitBuffer* buffer)
		{
			if (buffer->ReadBoolean())
			{
				bool flag = base.Simulation._history.Count == 0;
				int num = buffer->ReadInt32VarLength();
				int num2 = num - buffer->ReadInt32VarLength();
				SimulationSnapshot simulationSnapshot = base.Simulation.FindSnapshot(num2, defaultToRoot: false);
				if (simulationSnapshot == null)
				{
					Log.Error(base.Simulation, $"received snapshot for tick #{num} compressed against #{num2} but could not find it locally, can't decompress snapshot.");
					return;
				}
				_sharedTick = simulationSnapshot.Tick;
				Assert.Check(num > base.Simulation._history.MaxTick);
				Assert.Check(simulationSnapshot.Tick == num2);
				int num3 = Maths.BytesRequiredForBits(buffer->LengthBytes * 8 - buffer->OffsetBits);
				EngineProfiler.WorldSnapshotSize(num3);
				base.Simulation.Stats.GetStatBuffer(Statistics.SimStats.PacketSize).Push(num3);
				SimulationSnapshot simulationSnapshot2 = base.Simulation._history.Next(num);
				Allocator.DeltaUnpack(simulationSnapshot2.Allocator, simulationSnapshot.Allocator, buffer);
			}
		}

		public override void SendBegin()
		{
			if (base.Simulation.IsServer)
			{
				Assert.Check(_deltaBuffers.Count == 0);
				SimulationSnapshot simulationSnapshot = base.Simulation._history.Next(base.Simulation._state.Tick, copyPrevious: false);
				simulationSnapshot.CopyFrom(base.Simulation._state, onlyUsedMemory: true);
			}
		}

		public unsafe override void SendEnd()
		{
			foreach (KeyValuePair<Tick, BitBufferPtr> deltaBuffer in _deltaBuffers)
			{
				NetBitBuffer.Release(deltaBuffer.Value.Ptr);
			}
			_deltaBuffers.Clear();
		}

		public unsafe override void OnPacketLost(NetConnection* c, SimulationPacketEnvelope* envelope)
		{
			if (base.Simulation.IsServer)
			{
				SimulationConnection simulationConnection = base.Simulation.GetSimulationConnection(c);
				if (envelope->HasSnapshot)
				{
					Assert.Check(envelope->Tick > 0);
					simulationConnection.PendingTicks.Remove(envelope->Tick);
					simulationConnection.PendingSnapshots--;
				}
				Assert.Check(simulationConnection.PendingSnapshots >= 0);
			}
		}

		public unsafe override void OnPacketDelivered(NetConnection* c, SimulationPacketEnvelope* envelope)
		{
			if (base.Simulation.IsServer)
			{
				SimulationConnection simulationConnection = base.Simulation.GetSimulationConnection(c);
				if (envelope->HasSnapshot)
				{
					Assert.Check(envelope->Tick > 0);
					simulationConnection.PendingTicks.Remove(envelope->Tick);
					simulationConnection.PendingSnapshots--;
					simulationConnection.SharedTick = Math.Max(simulationConnection.SharedTick, envelope->Tick);
				}
				Assert.Check(simulationConnection.PendingSnapshots >= 0);
			}
		}
	}

	private abstract class StateReplicatorEventualConsistencyBase : StateReplicator
	{
		private struct ConnectionBitSet
		{
			public unsafe uint* Words;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public unsafe void Set(int bit)
			{
				Words[bit / 32] |= (uint)(1 << bit % 32);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public unsafe bool IsSet(int bit)
			{
				return (Words[bit / 32] & (uint)(1 << bit % 32)) != 0;
			}
		}

		protected const int SET_WORD_BITS = 32;

		protected const int DATA_BLOCK_SIZE = 6;

		protected const int OFFSET_BLOCK_SIZE = 4;

		protected const int HEADER_BLOCK_SIZE = 8;

		protected const int GLOBAL_BLOCK_SIZE = 8;

		public const int MAX_SNAPSHOTS_CREATED_PER_NETWORKRECV = 3;

		public const int MAX_HISTORY_SNAPSHOTS = 8;

		protected unsafe int* _changed;

		protected SimulationSnapshot _delta;

		private unsafe Allocator* _setsAllocator;

		private Dictionary<NetworkId, ConnectionBitSet> _sets;

		private List<NetworkObjectPriorityHeap.Item> _priorityPopped;

		protected Queue<NetworkId> _create;

		protected Queue<NetworkId> _createNested;

		protected Queue<NetworkId> _destroy;

		protected int SetWordCount => Native.RoundBitsUpTo32(base.Simulation.Config.DefaultPlayers) / 32;

		public override bool UseObjectInterest => base.Simulation.Config.ObjectInterest;

		public virtual int PacketSizeLimitInBits => 32768;

		protected unsafe StateReplicatorEventualConsistencyBase(Simulation simulation)
			: base(simulation)
		{
			Assert.Check(condition: true);
			_changed = Native.MallocAndClearArray<int>(simulation.State.ReplicateWordLength);
			_create = new Queue<NetworkId>();
			_createNested = new Queue<NetworkId>();
			_destroy = new Queue<NetworkId>();
			_priorityPopped = new List<NetworkObjectPriorityHeap.Item>();
			if (base.Simulation.IsServer || base.Simulation.Topology == SimulationConfig.Topologies.Shared)
			{
				_sets = new Dictionary<NetworkId, ConnectionBitSet>();
				_setsAllocator = Allocator.Create(new Allocator.Config(PageSizes._8Kb, 256, 8));
				_delta = simulation._historyPool.Acquire();
			}
		}

		protected unsafe static void WriteFooter(NetBitBuffer* buffer)
		{
			buffer->WriteBoolean(value: false);
		}

		private unsafe void WriteGlobals(SimulationConnection sc, NetBitBuffer* buffer, SimulationPacketEnvelope* envelope)
		{
			SimulationGlobalState globalState = sc.GlobalState;
			int* ptr = (int*)(&globalState);
			int* globalState2 = (int*)base.Simulation.State.GlobalState;
			for (int i = 0; i < 32; i++)
			{
				if (buffer->WriteBoolean(globalState2[i] != ptr[i]))
				{
					buffer->WriteInt32VarLength(globalState2[i], 8);
				}
			}
			envelope->GlobalState = *base.Simulation.State.GlobalState;
		}

		protected unsafe static void ReadGlobals(SimulationConnection sc, SimulationGlobalState* globals, NetBitBuffer* buffer)
		{
			for (int i = 0; i < 32; i++)
			{
				if (buffer->ReadBoolean())
				{
					((int*)globals)[i] = buffer->ReadInt32VarLength(8);
				}
			}
		}

		protected unsafe void ReadDestroys(PlayerRef player, NetBitBuffer* buffer)
		{
			if (buffer->ReadBoolean())
			{
				int num = buffer->ReadInt32VarLength(8);
				for (int i = 0; i < num; i++)
				{
					NetworkId item = NetworkId.Read(buffer);
					_destroy.Enqueue(item);
				}
			}
		}

		public unsafe override void UpdateRemotePrefabs()
		{
			if (base.Simulation._history.Count == 0)
			{
				return;
			}
			if (_destroy.Count > 0 || _create.Count > 0 || _createNested.Count > 0)
			{
			}
			SimulationSnapshot latest = base.Simulation._history.Latest;
			SimulationSnapshot state = base.Simulation.State;
			int count = _create.Count;
			while (count-- > 0)
			{
				NetworkId networkId = _create.Dequeue();
				if (state.TryGetObject(networkId, out var header) && !base.Simulation._callbacks.CreateRemotePrefab(header))
				{
					_create.Enqueue(networkId);
				}
			}
			int count2 = _createNested.Count;
			NetworkObjectHeader* header3;
			while (count2-- > 0)
			{
				NetworkId networkId2 = _createNested.Dequeue();
				if (!state.TryGetObject(networkId2, out var header2))
				{
					continue;
				}
				if (state.TryGetObject(header2->NestingRoot, out header3))
				{
					if (!base.Simulation._callbacks.CreateRemotePrefab(header2))
					{
						_createNested.Enqueue(networkId2);
					}
				}
				else
				{
					_createNested.Enqueue(networkId2);
				}
			}
			while (_destroy.Count > 0)
			{
				NetworkId id = _destroy.Dequeue();
				base.Simulation._callbacks.DestroyRemotePrefab(id, exists: true);
				if (state.TryGetObject(id, out header3))
				{
					state.FreeObject(id);
				}
				if (latest.TryGetObject(id, out header3))
				{
					latest.FreeObject(id);
				}
			}
		}

		protected unsafe NetworkObjectHeader* ReadObjectHeader(NetConnection* connection, NetBitBuffer* buffer, SimulationSnapshot snapshot, out int bitOffset, out bool created, out bool skip)
		{
			created = false;
			skip = false;
			bitOffset = buffer->OffsetBits - 1;
			NetworkId networkId = NetworkId.Read(buffer);
			bool flag = snapshot.TryGetObject(networkId, out var header);
			if (ReadHeaderData(buffer, out var type, out var wordCount, out var transformOffset, out var flags, out var nestingRoot, out var nestingKey, out var sceneGuid))
			{
				Assert.Check(!buffer->DoneOrOverflow);
				if (flag)
				{
					Assert.Check(header->Type.Equals(type));
					Assert.Check(header->WordCount.Equals(wordCount));
					Assert.Check(header->TransformOffset.Equals(transformOffset));
					Assert.Check(header->NestingRoot.Equals(nestingRoot));
					Assert.Check(header->NestingKey.Equals(nestingKey));
					Assert.Check(header->SceneGuid.Equals(sceneGuid), header->SceneGuid, sceneGuid);
					Assert.Check(header->Flags.Equals(flags));
				}
				else
				{
					if (IsLocalDestroyWaitingForConfirmation(networkId))
					{
						skip = true;
						return null;
					}
					header = snapshot.AllocateObject(networkId, type, wordCount);
					header->Flags = flags;
					header->NestingRoot = nestingRoot;
					header->NestingKey = nestingKey;
					header->SceneGuid = sceneGuid;
					header->TransformOffset = transformOffset;
					created = true;
					if (nestingRoot.IsValid && _createNested != null)
					{
						Assert.Check<string, NetworkId, Guid>(sceneGuid == default(Guid), "Nested object has a scene guid", networkId, sceneGuid);
						_createNested.Enqueue(networkId);
					}
					else
					{
						_create.Enqueue(networkId);
					}
				}
			}
			else
			{
				Assert.Check(!buffer->DoneOrOverflow, "buffer->DoneOrOverflow");
				if (!flag)
				{
					if (base.Simulation.State.TryGetObject(networkId, out var header2))
					{
						header = snapshot.AllocateObject(header2->Id, header2->Type, header2->WordCount);
						Native.MemCpy(header, header2, header2->WordCount * 4);
					}
					else
					{
						Assert.AlwaysFail($"Expected to find object in the simulation state ({networkId})");
					}
				}
			}
			base.Simulation._callbacks.ObjectReceivedUpdate(header->Id, snapshot.Tick);
			return header;
		}

		protected unsafe void WriteHeader(SimulationConnection sc, NetBitBuffer* buffer, SimulationPacketEnvelope* envelope)
		{
			if (base.Simulation.IsServer)
			{
				Assert.Check(sizeof(SimulationGlobalState) == 128);
				buffer->WriteInt32VarLength(envelope->Tick);
				WriteGlobals(sc, buffer, envelope);
			}
			if (buffer->WriteBoolean(sc.ObjectData.DestroyedCount > 0))
			{
				int destroyedCount = sc.ObjectData.DestroyedCount;
				buffer->WriteInt32VarLength(destroyedCount, 8);
				for (int i = 0; i < destroyedCount; i++)
				{
					NetworkId id = sc.ObjectData.DestroyedNextId();
					id.Write(buffer);
					envelope->AddObjectPacketData(base.Simulation, id, default, NetworkObjectPacketFlags.Destroy);
				}
			}
		}

		protected unsafe void WriteUsingAllObjects(SimulationConnection sc, PlayerRef player, NetConnection* connection, SimulationPacketEnvelope* envelope, NetBitBuffer* buffer)
		{
			_priorityPopped.Clear();
			EngineProfiler.Begin("WriteUsingAllObjects");
			if (sc.ObjectPriorityHeap.IsEmpty)
			{
				sc.ObjectPriorityHeap.BuildFromMap(base.Simulation.State.ObjectTable);
			}
			else
			{
				NetworkObjectRefMapPtr.GetIterateBufferStartCount(base.Simulation.State.ObjectTable, out var entries, out var start, out var count);
				for (int i = start; i < count; i++)
				{
					if ((bool)entries[i].Id)
					{
						sc.ObjectPriorityHeap.PushIfNotContains(entries[i].Id, 1f);
					}
				}
			}
			NetworkObjectPriorityHeap.Item item;
			while (sc.ObjectPriorityHeap.TryPop(out item))
			{
				if (base.Simulation.State.TryGetObjectEntry(item.Value, out var entry))
				{
					_priorityPopped.Add(item);
					WriteObjectFromEntry(entry, null, sc, player, connection, envelope, buffer);
					if (buffer->OffsetBits >= PacketSizeLimitInBits)
					{
						break;
					}
				}
			}
			sc.ObjectPriorityHeap.IncreasePriorities();
			for (int j = 0; j < _priorityPopped.Count; j++)
			{
				sc.ObjectPriorityHeap.Push(_priorityPopped[j].Value, 1f);
			}
			EngineProfiler.End();
		}

		internal override bool HasObjectInterest(PlayerRef player, NetworkId id)
		{
			SimulationPlayer simulationPlayer = base.Simulation.GetSimulationPlayer(player);
			if (simulationPlayer.AlwaysInterested.Contains(id))
			{
				return true;
			}
			if (base.Simulation._globalInterestObjects.BinarySearchSpecialized(id) >= 0)
			{
				return true;
			}
			return simulationPlayer.AOIResult.Contains(id);
		}

		protected unsafe void WriteUsingInterestManagement(SimulationConnection sc, PlayerRef player, NetConnection* connection, SimulationPacketEnvelope* envelope, NetBitBuffer* buffer)
		{
			_priorityPopped.Clear();
			Assert.Check(base.Simulation.IsServer);
			EngineProfiler.Begin("WriteUsingInterestManagement");
			EngineProfiler.Begin("WriteGlobal");
			if (base.Simulation.State.TryGetObjectEntry(NetworkId.InternalState, out var entry))
			{
				WriteObjectFromEntry(entry, null, sc, player, connection, envelope, buffer);
			}
			int count = base.Simulation._globalInterestObjects.Count;
			for (int num = count - 1; num >= 0; num--)
			{
				if (base.Simulation.State.TryGetObjectEntry(base.Simulation._globalInterestObjects[num], out var entry2))
				{
					WriteObjectFromEntry(entry2, null, sc, player, connection, envelope, buffer);
					if (buffer->OffsetBits >= PacketSizeLimitInBits)
					{
						break;
					}
				}
				else
				{
					base.Simulation._globalInterestObjects.RemoveAt(num);
				}
			}
			EngineProfiler.End();
			EngineProfiler.Begin("WriteAlwaysInterested");
			SimulationPlayer simulationPlayer = base.Simulation.GetSimulationPlayer(player);
			HashSet<NetworkId>.Enumerator enumerator = simulationPlayer.AlwaysInterested.GetEnumerator();
			while (enumerator.MoveNext())
			{
				NetworkId current = enumerator.Current;
				if (base.Simulation.State.TryGetObjectEntry(current, out var entry3))
				{
					WriteObjectFromEntry(entry3, null, sc, player, connection, envelope, buffer);
					if (buffer->OffsetBits >= PacketSizeLimitInBits)
					{
						break;
					}
				}
			}
			enumerator.Dispose();
			EngineProfiler.End();
			EngineProfiler.Begin("WriteAreaOfInterest");
			NetworkObjectPriorityHeap.Item item;
			while (sc.ObjectPriorityHeap.TryPop(out item))
			{
				if (base.Simulation.State.TryGetObjectEntry(item.Value, out var entry4))
				{
					WriteObjectFromEntry(entry4, null, sc, player, connection, envelope, buffer);
					if (buffer->OffsetBits >= PacketSizeLimitInBits)
					{
						break;
					}
				}
			}
			sc.ObjectPriorityHeap.IncreasePriorities();
			EngineProfiler.End();
			EngineProfiler.End();
		}

		public unsafe override void Dispose()
		{
			if (_changed != null)
			{
				Native.Free(_changed);
				_changed = null;
			}
			if (_setsAllocator != null)
			{
				Allocator.Dispose(_setsAllocator);
				_setsAllocator = null;
			}
		}

		public unsafe override void OnObjectDestroyed(NetworkId id, NetworkObjectDestroyFlags flags)
		{
			if (base.Simulation.IsServer)
			{
				Assert.Check(flags.Get(NetworkObjectDestroyFlags.DestroyState));
				if (_sets != null && _sets.TryGetValue(id, out var value))
				{
					for (int i = 0; i < base.Simulation.MaxConnections; i++)
					{
						if (value.IsSet(i))
						{
							base.Simulation.GetSimulationConnectionByIndex(i)?.ObjectData.SetDestroyed(id);
						}
					}
					if (_setsAllocator != null)
					{
						Allocator.Free(_setsAllocator, value.Words);
					}
					_sets.Remove(id);
				}
				base.Simulation.State.FreeObject(id);
			}
			else if (flags.Get(NetworkObjectDestroyFlags.DestroyState))
			{
				base.Simulation.GetSimulationConnectionByIndex(0)?.ObjectData.SetDestroyed(id);
				base.Simulation.State.FreeObject(id);
			}
			else if (flags.Get(NetworkObjectDestroyFlags.DestroyedByEngine))
			{
				NetworkObjectHeader* ptr = base.Simulation.State.GetObject(id);
				if (ptr->NestingRoot.IsValid)
				{
					_createNested.Enqueue(id);
				}
				else
				{
					_create.Enqueue(id);
				}
			}
			else if (flags.Get(NetworkObjectDestroyFlags.DestroyedByReplicator))
			{
				base.Simulation.GetSimulationConnectionByIndex(0)?.ObjectData.SetDestroyed(id);
			}
		}

		protected unsafe virtual int* InitDefaultInterestGroups(NetworkObjectHeader* header, SimulationConnection sc)
		{
			throw new NotImplementedException();
		}

		protected virtual bool IsLocalDestroyWaitingForConfirmation(NetworkId id)
		{
			return false;
		}

		protected unsafe bool WriteObjectFromEntry(NetworkObjectRefMapPtr.Entry* entry, NetworkObjectHeader* header, SimulationConnection sc, PlayerRef player, NetConnection* connection, SimulationPacketEnvelope* envelope, NetBitBuffer* buffer)
		{
			if (header == null)
			{
				header = base.Simulation._state.GetObjectFromEntry(entry);
			}
			if (entry->CheckedTick < base.Simulation.Tick.Raw)
			{
				entry->CheckedTick = base.Simulation.Tick.Raw;
				int wordCount = header->WordCount;
				int* ptr = _changed + base.Simulation.State.Allocator->GetReplicateWordOffset(header);
				int num = entry->ChangedTick;
				for (int i = 11; i < wordCount; i++)
				{
					if (ptr[i] > num)
					{
						num = ptr[i];
					}
				}
				entry->ChangedTick = num;
			}
			return WriteObject(header, sc, player, connection, envelope, buffer, entry->ChangedTick);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe int FindChangedWord(int* changes, int startWord, int maxWord, int minTick, int* groups, int* interestGroups)
		{
			int i;
			for (i = startWord; i < maxWord && (changes[i] <= minTick || (interestGroups != null && interestGroups[i] != 0 && !Int32BitSetUtils.IsBitSetOrNull(groups, interestGroups[i]))); i++)
			{
			}
			return i;
		}

		private unsafe static void WriteWord(NetBitBuffer* buffer, int* ptr, int w, int previous)
		{
			Assert.Check(w - previous >= 0);
			long num = ptr[w];
			num = (num >> 63) ^ (num << 1);
			uint num2 = (uint)(w - previous);
			ulong num3 = 1uL;
			int num4 = 1;
			int num5 = (Maths.BitScanReverse(num2) + 4) / 4;
			num3 |= (uint)(1 << num5 - 1 << num4);
			num4 += num5;
			num3 |= num2 << num4;
			num4 += num5 * 4;
			num5 = (Maths.BitScanReverse(num) + 6) / 6;
			num3 |= (uint)(1 << num5 - 1 << num4);
			num4 += num5;
			num3 |= (ulong)(num << num4);
			num4 += num5 * 6;
			if (num4 > 64)
			{
				buffer->WriteBoolean(value: true);
				buffer->WriteInt32VarLength(w - previous, 4);
				buffer->WriteInt64VarLength(Maths.ZigZagEncode((long)ptr[w]), 6);
			}
			else
			{
				buffer->WriteUInt64(num3, num4);
			}
			Assert.Check(!buffer->Overflow);
		}

		protected unsafe bool WriteObject(NetworkObjectHeader* header, SimulationConnection sc, PlayerRef player, NetConnection* connection, SimulationPacketEnvelope* envelope, NetBitBuffer* buffer, int changedTick = 0)
		{
			sc.ObjectData.EnsureExist(header->Id, out var sentTick, out var isCreateUnconfirmed, out var interestGroups);
			if (sentTick.Raw == base.Simulation.State.Tick.Raw)
			{
				return false;
			}
			if (changedTick > 0 && changedTick < sentTick.Raw)
			{
				return false;
			}
			int* ptr = _changed + base.Simulation.State.Allocator->GetReplicateWordOffset(header);
			if (interestGroups == null && (header->Flags & NetworkObjectHeaderFlags.HasDefaultInterestGroups) == NetworkObjectHeaderFlags.HasDefaultInterestGroups)
			{
				Assert.Check(sentTick == default(Tick));
				interestGroups = InitDefaultInterestGroups(header, sc);
			}
			int offsetBits = buffer->OffsetBits;
			int num = sentTick.Raw;
			int num2 = 11;
			int* ptr2 = (int*)((interestGroups != null) ? base.Simulation._stateInterestGroups.Allocator->Ptr(base.Simulation.State.Allocator->Ptr(header)) : null);
			Assert.Check(!buffer->Overflow);
			buffer->WriteBoolean(value: true);
			header->Id.Write(buffer);
			int wordCount = header->WordCount;
			if (buffer->WriteBoolean(isCreateUnconfirmed))
			{
				Assert.Check(header->WordCount != 0);
				buffer->WriteUInt32VarLength(header->Type.Value, 8);
				buffer->WriteInt32VarLength(header->WordCount, 8);
				buffer->WriteInt32VarLength(header->TransformOffset, 8);
				buffer->WriteInt32VarLength((int)header->Flags, 8);
				if (buffer->WriteBoolean(header->NestingRoot != default(NetworkId)))
				{
					buffer->WriteUInt32VarLength(header->NestingRoot.Raw, 8);
					buffer->WriteInt32VarLength(header->NestingKey.Value, 8);
				}
				if (buffer->WriteBoolean(header->SceneGuid != default(Guid)))
				{
					Assert.Check(sizeof(Guid) == 16);
					buffer->WriteBytesAligned(&header->SceneGuid, 16);
				}
				if (!_sets.TryGetValue(header->Id, out var value))
				{
					value = new ConnectionBitSet
					{
						Words = Allocator.AllocAndClearArray<uint>(_setsAllocator, SetWordCount)
					};
					_sets.Add(header->Id, value);
				}
				value.Set(connection->LocalConnectionId.GroupIndex);
				int num3 = FindChangedWord(ptr, num2, wordCount, num, interestGroups, ptr2);
				if (num3 < wordCount)
				{
					num = 0;
				}
			}
			int num4 = 11;
			for (int i = num2; i < wordCount; i++)
			{
				if (ptr[i] > num && (ptr2 == null || ptr2[i] == 0 || Int32BitSetUtils.IsBitSetOrNull(interestGroups, ptr2[i])))
				{
					WriteWord(buffer, (int*)header, i, num4);
					num4 = i;
				}
			}
			if (num4 == 11)
			{
				buffer->OffsetBits = offsetBits;
				sc.ObjectData.SetSentTick(header->Id, base.Simulation.State.Tick);
				return false;
			}
			envelope->AddObjectPacketData(base.Simulation, header->Id, sentTick, (NetworkObjectPacketFlags)0);
			sc.ObjectData.SetSentTick(header->Id, base.Simulation.State.Tick);
			buffer->WriteBoolean(value: false);
			return true;
		}

		protected unsafe static bool ReadHeaderData(NetBitBuffer* buffer, out NetworkPrefabId type, out int wordCount, out int transformOffset, out NetworkObjectHeaderFlags flags, out NetworkId nestingRoot, out NetworkObjectNestingKey nestingKey, out Guid sceneGuid)
		{
			Assert.Check(!buffer->DoneOrOverflow);
			if (buffer->ReadBoolean())
			{
				type.Value = buffer->ReadUInt32VarLength(8);
				wordCount = buffer->ReadInt32VarLength(8);
				transformOffset = buffer->ReadInt32VarLength(8);
				flags = (NetworkObjectHeaderFlags)buffer->ReadInt32VarLength(8);
				if (buffer->ReadBoolean())
				{
					nestingRoot.Raw = buffer->ReadUInt32VarLength(8);
					nestingKey.Value = buffer->ReadInt32VarLength(8);
				}
				else
				{
					nestingRoot = default;
					nestingKey = default;
				}
				Guid guid = default;
				if (buffer->ReadBoolean())
				{
					Assert.Check(sizeof(Guid) == 16);
					buffer->ReadBytesAligned(&guid, 16);
				}
				sceneGuid = guid;
				return true;
			}
			type = default;
			transformOffset = 0;
			flags = (NetworkObjectHeaderFlags)0;
			wordCount = 0;
			nestingRoot = default;
			nestingKey = default;
			sceneGuid = default;
			return false;
		}

		public unsafe override void SendBegin()
		{
			if (base.Simulation.IsServer || base.Simulation.Topology == SimulationConfig.Topologies.Shared)
			{
				TrackChanges(_delta.Allocator->Replicate, base.Simulation.State.Allocator->Replicate, Allocator.GetWordLengthForReplication(base.Simulation.State.Allocator), base.Simulation.State.Tick);
			}
		}

		public override void SendEnd()
		{
		}

		public unsafe override void OnPacketLost(NetConnection* c, SimulationPacketEnvelope* envelope)
		{
			SimulationConnection simulationConnection = base.Simulation.GetSimulationConnection(c);
			for (int i = 0; i < envelope->ObjectDataCount; i++)
			{
				NetworkObjectPacketData networkObjectPacketData = envelope->ObjectData[i];
				if ((networkObjectPacketData.Flags & NetworkObjectPacketFlags.Destroy) == NetworkObjectPacketFlags.Destroy)
				{
					simulationConnection.ObjectData.SetDestroyed(networkObjectPacketData.Id);
				}
				else
				{
					simulationConnection.ObjectData.SetSentTick(networkObjectPacketData.Id, networkObjectPacketData.ResetTick);
				}
			}
		}

		public unsafe override void OnPacketDelivered(NetConnection* c, SimulationPacketEnvelope* envelope)
		{
			SimulationConnection simulationConnection = base.Simulation.GetSimulationConnection(c);
			for (int i = 0; i < envelope->ObjectDataCount; i++)
			{
				NetworkObjectPacketData networkObjectPacketData = envelope->ObjectData[i];
				if ((networkObjectPacketData.Flags & NetworkObjectPacketFlags.Destroy) == NetworkObjectPacketFlags.Destroy)
				{
					simulationConnection.ObjectData.SetDestroyConfirmed(networkObjectPacketData.Id);
				}
				else
				{
					simulationConnection.ObjectData.SetCreateConfirmed(networkObjectPacketData.Id);
				}
			}
		}

		protected unsafe void TrackChanges(int* oldState, int* newState, int words, int tick)
		{
			EngineProfiler.Begin("TrackChanges");
			Assert.Check(Native.IsPointerAligned(newState, 4));
			Assert.Check(Native.IsPointerAligned(oldState, 4));
			try
			{
				if (base.Simulation.IsServer)
				{
					for (int i = 0; i < words; i++)
					{
						if (newState[i] != oldState[i])
						{
							Assert.Check(_changed[i] < tick);
							_changed[i] = tick;
							oldState[i] = newState[i];
						}
					}
					return;
				}
				for (int j = 0; j < words; j++)
				{
					if (newState[j] != oldState[j] && _changed[j] < tick)
					{
						_changed[j] = tick;
						oldState[j] = newState[j];
					}
				}
			}
			finally
			{
				EngineProfiler.End();
			}
		}

		protected SimulationSnapshot InsertReceivedSnapshotInHistory(int tick)
		{
			if (base.StateUpdateCountThisNetworkRecv <= 3 && base.Simulation._history.Count <= 8)
			{
				return base.Simulation._history.Next(tick);
			}
			return ((SimulationSnapshot.HistoryLL)base.Simulation._history).ReplaceLatest(tick);
		}

		internal override void AssertVerifyNothingIsPending()
		{
			base.AssertVerifyNothingIsPending();
			Assert.Always(_create.Count == 0, "Create queue not empty", BehaviourUtils.Join(_create));
			Assert.Always(_createNested.Count == 0, "Create nested queue not empty", BehaviourUtils.Join(_createNested));
			Assert.Always(_destroy.Count == 0, "Destroy queue not empty", BehaviourUtils.Join(_destroy));
		}
	}

	private class StateReplicatorEventualConsistencyClientServer : StateReplicatorEventualConsistencyBase
	{
		private unsafe Allocator* _clientInterestGroupsAllocator;

		public unsafe StateReplicatorEventualConsistencyClientServer(Simulation simulation)
			: base(simulation)
		{
			_clientInterestGroupsAllocator = Allocator.Create(new Allocator.Config(PageSizes._8Kb, 1024, 4));
		}

		public unsafe override void Dispose()
		{
			base.Dispose();
			Allocator.Dispose(_clientInterestGroupsAllocator);
			_clientInterestGroupsAllocator = null;
		}

		protected unsafe override int* InitDefaultInterestGroups(NetworkObjectHeader* header, SimulationConnection sc)
		{
			string[] defaultInterestGroups = base.Simulation._callbacks.GetDefaultInterestGroups(header->Id);
			int* orAllocGroups = sc.ObjectData.GetOrAllocGroups(header->Id, _clientInterestGroupsAllocator);
			for (int i = 0; i < defaultInterestGroups.Length; i++)
			{
				if (NetworkBehaviourUtils.TryGetInterestGroupKeyFromGroup(defaultInterestGroups[i], out var key))
				{
					Int32BitSetUtils.SetBit(orAllocGroups, key);
				}
			}
			return orAllocGroups;
		}

		public unsafe override void OnObjectInterestGroupChange(PlayerRef player, NetworkId id, string group, bool enabled)
		{
			SimulationConnection simulationConnectionByIndex = base.Simulation.GetSimulationConnectionByIndex(player);
			if (!simulationConnectionByIndex.Active)
			{
				return;
			}
			int* orAllocGroups = simulationConnectionByIndex.ObjectData.GetOrAllocGroups(id, _clientInterestGroupsAllocator);
			int bit = NetworkBehaviourUtils.GetnterestGroupKeyFromGroup(group);
			if (Int32BitSetUtils.IsBitSet(orAllocGroups, bit) != enabled)
			{
				if (enabled)
				{
					simulationConnectionByIndex.ObjectData.SetSentTick(id, default);
					Int32BitSetUtils.SetBit(orAllocGroups, bit);
				}
				else
				{
					Int32BitSetUtils.ClearBit(orAllocGroups, bit);
				}
			}
		}

		public unsafe override void SendPacket(NetConnection* connection, NetBitBuffer* buffer, SimulationPacketEnvelope* envelope)
		{
			SimulationConnection simulationConnection = base.Simulation.GetSimulationConnection(connection);
			PlayerRef player = connection->LocalConnectionId.GroupIndex;
			WriteHeader(simulationConnection, buffer, envelope);
			if (base.Simulation.Config.ObjectInterest)
			{
				WriteUsingInterestManagement(simulationConnection, player, connection, envelope, buffer);
			}
			else
			{
				WriteUsingAllObjects(simulationConnection, player, connection, envelope, buffer);
			}
			StateReplicatorEventualConsistencyBase.WriteFooter(buffer);
		}

		public unsafe override void RecvPacket(NetConnection* connection, NetBitBuffer* buffer)
		{
			Assert.Check(base.Simulation.IsClient);
			int offsetBits = buffer->OffsetBits;
			int tick = buffer->ReadInt32VarLength();
			SimulationSnapshot simulationSnapshot = InsertReceivedSnapshotInHistory(tick);
			StateReplicatorEventualConsistencyBase.ReadGlobals(base.Simulation.GetSimulationConnection(connection), simulationSnapshot.GlobalState, buffer);
			ReadDestroys(connection->LocalId.GroupIndex, buffer);
			while (buffer->ReadBoolean())
			{
				Assert.Check(!buffer->DoneOrOverflow);
				NetworkObjectHeader* ptr = ReadObjectHeader(connection, buffer, simulationSnapshot, out var bitOffset, out var _, out var _);
				int* ptr2 = (int*)ptr;
				int num = 11;
				while (buffer->ReadBoolean())
				{
					Assert.Check(!buffer->DoneOrOverflow);
					num += buffer->ReadInt32VarLength(4);
					ptr2[num] = (int)Maths.ZigZagDecode(buffer->ReadInt64VarLength(6));
				}
				Statistics.TickSample<Statistics.FloatSample> item = new Statistics.TickSample<Statistics.FloatSample>(simulationSnapshot.Tick, (float)base.Simulation.Stats.Timer.ElapsedInSeconds, (float)buffer->OffsetBits - (float)bitOffset);
				base.Simulation.Stats.GetObjectBandwidthBuffer(ptr->Id, createIfMissing: true).Push(item);
			}
			int offsetBits2 = buffer->OffsetBits;
			base.Simulation.Stats.GetStatBuffer(Statistics.SimStats.PacketSize).Push((offsetBits2 - offsetBits) / 8);
		}
	}

	private class StateReplicatorEventualConsistencyShared : StateReplicatorEventualConsistencyBase
	{
		private HashSet<NetworkId> _stateAuthSet;

		public override bool ClientToServer => true;

		public StateReplicatorEventualConsistencyShared(Simulation simulation)
			: base(simulation)
		{
			_stateAuthSet = new HashSet<NetworkId>(new NetworkId.EqualityComparer());
		}

		private void StateAuthAdd(NetworkId id)
		{
			if (_stateAuthSet.Add(id))
			{
				Log.Debug(base.Simulation, $"State Authority Gained({id})");
				base.Simulation._callbacks.ObjectStateAuthorityChanged(id);
			}
		}

		private bool StateAuthRemove(NetworkId id)
		{
			if (_stateAuthSet.Remove(id))
			{
				Log.Debug(base.Simulation, $"State Authority Lost({id})");
				base.Simulation._callbacks.ObjectStateAuthorityChanged(id);
				return true;
			}
			return false;
		}

		public override void OnObjectDestroyed(NetworkId id, NetworkObjectDestroyFlags flags)
		{
			Assert.Check(!base.Simulation.IsServer);
			base.OnObjectDestroyed(id, flags);
			StateAuthRemove(id);
		}

		public unsafe override void OnObjectSpawnedLocal(NetworkId id)
		{
			Assert.Check(base.Simulation.IsClient);
			Assert.Check(base.Simulation.State.GetObject(id)->StateAuthority == base.Simulation.LocalPlayer, base.Simulation.State.GetObject(id)->StateAuthority, base.Simulation.LocalPlayer);
			StateAuthAdd(id);
		}

		protected override bool IsLocalDestroyWaitingForConfirmation(NetworkId id)
		{
			return ((base.Simulation.GetSimulationConnectionByIndex(0)?.ObjectData)?.IsDestroyUnconfirmed(id) == true) ?? false;
		}

		public unsafe override void SendPacket(NetConnection* connection, NetBitBuffer* buffer, SimulationPacketEnvelope* envelope)
		{
			SimulationConnection simulationConnection = base.Simulation.GetSimulationConnection(connection);
			PlayerRef player = connection->LocalConnectionId.GroupIndex;
			WriteHeader(simulationConnection, buffer, envelope);
			Assert.Check(base.Simulation.IsClient);
			HashSet<NetworkId>.Enumerator enumerator = _stateAuthSet.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (base.Simulation.State.TryGetObject(enumerator.Current, out var header))
				{
					WriteObject(header, simulationConnection, player, connection, envelope, buffer);
					if (buffer->OffsetBits >= PacketSizeLimitInBits)
					{
						break;
					}
				}
			}
			enumerator.Dispose();
			StateReplicatorEventualConsistencyBase.WriteFooter(buffer);
		}

		public unsafe override void RecvPacket(NetConnection* connection, NetBitBuffer* buffer)
		{
			SimulationConnection simulationConnection = base.Simulation.GetSimulationConnection(connection);
			int offsetBits = buffer->OffsetBits;
			int tick = buffer->ReadInt32VarLength();
			SimulationSnapshot simulationSnapshot = InsertReceivedSnapshotInHistory(tick);
			StateReplicatorEventualConsistencyBase.ReadGlobals(simulationConnection, simulationSnapshot.GlobalState, buffer);
			*base.Simulation.State.GlobalState = *simulationSnapshot.GlobalState;
			ReadDestroys(connection->LocalId.GroupIndex, buffer);
			while (buffer->ReadBoolean())
			{
				Assert.Check(!buffer->DoneOrOverflow);
				NetworkObjectHeader* ptr = ReadObjectHeader(connection, buffer, simulationSnapshot, out var bitOffset, out var _, out var skip);
				int* ptr2 = (int*)ptr;
				int num = 11;
				if (skip)
				{
					while (buffer->ReadBoolean())
					{
						buffer->ReadInt32VarLength(4);
						buffer->ReadInt64VarLength(6);
					}
					continue;
				}
				Tick sentTick;
				while (buffer->ReadBoolean())
				{
					Assert.Check(!buffer->DoneOrOverflow);
					int num2 = buffer->ReadInt32VarLength(4);
					num += num2;
					Assert.Always(num < ptr->WordCount, "out of bounds", num, ptr->WordCount, num2);
					Assert.Always(simulationSnapshot.Allocator->IsPointerInHeap(ptr2 + num), "pointer is not inside of heap");
					if (num == 13 && ptr->StateAuthority == base.Simulation.LocalPlayer)
					{
						ptr2[num] = (int)Maths.ZigZagDecode(buffer->ReadInt64VarLength(6));
						if (!(ptr->StateAuthority != base.Simulation.LocalPlayer))
						{
							continue;
						}
						NetworkObjectHeader* ptr3 = base.Simulation.State.TryGetObject(ptr->Id);
						if (ptr3 == null || !(ptr3->StateAuthority == base.Simulation.LocalPlayer))
						{
							continue;
						}
						Tick tick2;
						if (base.Simulation.InterpFrom != null)
						{
							tick2 = base.Simulation.InterpFrom.Tick;
						}
						else
						{
							sentTick = default;
							tick2 = sentTick;
						}
						Tick tick3 = tick2;
						SimulationSnapshot simulationSnapshot2 = simulationSnapshot;
						while (simulationSnapshot2 != null && simulationSnapshot2.Tick >= tick3)
						{
							NetworkObjectHeader* ptr4 = simulationSnapshot2.GetObject(ptr->Id);
							if (ptr4 == null)
							{
								break;
							}
							Assert.Check(ptr3->WordCount == ptr->WordCount, "Word count mismatch during lost-auth-fixup");
							Native.MemCpy((byte*)ptr4 + (nint)20 * (nint)4, (byte*)ptr3 + (nint)20 * (nint)4, (ptr->WordCount - 20) * 4);
							simulationSnapshot2 = simulationSnapshot2.Next;
						}
					}
					else
					{
						ptr2[num] = (int)Maths.ZigZagDecode(buffer->ReadInt64VarLength(6));
					}
				}
				Statistics.TickSample<Statistics.FloatSample> item = new Statistics.TickSample<Statistics.FloatSample>(simulationSnapshot.Tick, (float)base.Simulation.Stats.Timer.ElapsedInSeconds, buffer->OffsetBits - bitOffset);
				base.Simulation.Stats.GetObjectBandwidthBuffer(ptr->Id, createIfMissing: true).Push(item);
				NetworkObjectHeader* ptr5 = base.Simulation.State.TryGetObject(ptr->Id);
				if (ptr5 == null)
				{
					ptr5 = base.Simulation.State.AllocateObject(ptr->Id, ptr->Type, ptr->WordCount);
				}
				Assert.Check(ptr5);
				if (ptr->StateAuthority == base.Simulation.LocalPlayer)
				{
					if (ptr->StateAuthority == ptr5->StateAuthority)
					{
						Native.MemCpy(ptr, ptr5, ptr5->WordCount * 4);
					}
					else
					{
						Native.MemCpy(ptr5, ptr, ptr->WordCount * 4);
					}
					simulationConnection.ObjectData.EnsureExist(ptr->Id, out sentTick, out var isCreateUnconfirmed, out var _);
					if (isCreateUnconfirmed)
					{
						simulationConnection.ObjectData.SetCreateConfirmed(ptr->Id);
					}
					StateAuthAdd(ptr->Id);
				}
				else
				{
					Native.MemCpy(ptr5, ptr, ptr->WordCount * 4);
					StateAuthRemove(ptr->Id);
				}
			}
			int offsetBits2 = buffer->OffsetBits;
			base.Simulation.Stats.GetStatBuffer(Statistics.SimStats.PacketSize).Push((offsetBits2 - offsetBits) / 8);
		}
	}

	public sealed class Statistics
	{
		public enum StatSourceTypes
		{
			Simulation = 0,
			NetworkObject = 1,
			NetConnection = 2
		}

		[Flags]
		public enum StatsPer
		{
			Individual = 1,
			Tick = 2,
			Second = 4
		}

		[Flags]
		public enum StatFlags
		{
			ValidOnServer = 1,
			ValidOnClient = 2,
			ValidInShared = 4,
			ValidWithDeltaSnapshot = 8,
			ValidWithEventualConsistency = 0x10,
			ValidOnStateAuthority = 0x20,
			ValidForBuildType = 0x40
		}

		public struct StatSourceInfo(string longname, string shortname, double multiplier, int decimals, float warnThreshold = float.PositiveInfinity, float errorThreshold = float.PositiveInfinity, StatsPer perDefault = StatsPer.Individual, StatsPer perFlags = (StatsPer)0, StatFlags statFlags = StatFlags.ValidOnClient | StatFlags.ValidInShared | StatFlags.ValidWithDeltaSnapshot | StatFlags.ValidWithEventualConsistency | StatFlags.ValidForBuildType, int histoBucketCount = 1020, double histoMaxValue = 1020.0)
		{
			public string LongName = longname;

			public string ShortName = shortname;

			public string InvalidReason = null;

			public double Multiplier = multiplier;

			public int Decimals = decimals;

			public int HistoBucketCount = histoBucketCount;

			public double HistogMaxValue = histoMaxValue;

			public float WarnThreshold = warnThreshold;

			public float ErrorThreshold = errorThreshold;

			public StatsPer PerFlags = perFlags;

			public StatsPer PerDefault = perDefault;

			public StatFlags Flags = statFlags;
		}

		public enum NetStats
		{
			RoundTripTime = 0,
			SentPacketSizes = 1,
			ReceivedPacketSizes = 2
		}

		[Flags]
		public enum NetStatFlags
		{
			RoundTripTime = 1,
			SentPacketSizes = 2,
			ReceivedPacketSizes = 4
		}

		public enum ObjStats
		{
			Bandwidth = 0,
			RPC = 1
		}

		[Flags]
		public enum ObjStatFlags
		{
			Buffer = 1,
			RPC = 2
		}

		public enum SimStats
		{
			FrameTime = 0,
			ForwardSimCount = 1,
			ResimCount = 2,
			PacketSize = 3,
			InterpStateDelta = 4,
			InterpTimescale = 5,
			InterpOffset = 6,
			InterpDiff = 7,
			InterpUncertainty = 8,
			InterpMultiplier = 9,
			SimulationTimeScale = 10,
			InputOffsetTarget = 11,
			InputOffset = 12,
			InputOffsetDeviation = 13,
			InputReceiveDelta = 14,
			InputReceiveDeltaDeviation = 15
		}

		[Flags]
		public enum SimStatFlags
		{
			FrameTime = 1,
			ForwardSimCount = 2,
			ResimCount = 4,
			PacketSize = 8,
			InterpStateDelta = 0x10,
			InterpTimescale = 0x20,
			InterpOffset = 0x40,
			InterpDiff = 0x80,
			InterpUncertainty = 0x100,
			InterpMultiplier = 0x200,
			SimulationTimeScale = 0x400,
			InputOffsetTarget = 0x800,
			InputOffset = 0x1000,
			InputOffsetDeviation = 0x2000,
			InputReceiveDelta = 0x4000,
			InputReceiveDeltaDeviation = 0x8000
		}

		public struct RPCSample : ISampleData
		{
			public ushort Behaviour;

			public ushort Method;

			public int TickValue => 0;

			public float TimeValue => 0f;

			public float FloatValue => 0f;
		}

		public struct FloatSample : ISampleData
		{
			private float _value;

			public int TickValue => 0;

			public float TimeValue => 0f;

			public float FloatValue => _value;

			public static implicit operator FloatSample(float value)
			{
				return new FloatSample
				{
					_value = value
				};
			}

			public static explicit operator float(FloatSample sample)
			{
				return sample._value;
			}
		}

		public struct TickSample<T>(Tick tick, float time, T value) : ISampleData where T : ISampleData
		{
			public int Tick = tick;

			public float Time = time;

			public T Value = value;

			public int TickValue => Tick;

			public float TimeValue => Time;

			public float FloatValue => Value.FloatValue;
		}

		public struct TimeSample<T>(float time, T value) : ISampleData where T : ISampleData
		{
			public float Time = time;

			public T Value = value;

			public int TickValue => 0;

			public float TimeValue => Time;

			public float FloatValue => Value.FloatValue;
		}

		public abstract class StatsBufferBase<T> where T : ISampleData
		{
			private int _head;

			private int _tail;

			private int _count;

			private readonly T[] _array;

			private readonly bool _overwrite;

			public int Count => _count;

			public int Capacity => _array.Length;

			public bool IsFull => _count == _array.Length;

			public bool IsEmpty => _count == 0;

			public bool Paused { get; set; }

			public T First => this[0];

			public T Head => this[_head];

			public T Tail => this[_tail];

			public T this[int index]
			{
				get
				{
					if (index < 0 || index >= _count)
					{
						throw new IndexOutOfRangeException();
					}
					return _array[(_tail + index) % _array.Length];
				}
				set
				{
					if (index < 0 || index >= _count)
					{
						throw new IndexOutOfRangeException();
					}
					_array[(_tail + index) % _array.Length] = value;
				}
			}

			public ISampleData GetSampleAtIndex(int index)
			{
				return this[index];
			}

			public StatsBufferBase(int size, bool overwrite)
			{
				_array = new T[size];
				_overwrite = overwrite;
			}

			[Conditional("ENABLE_PROFILER")]
			public void Push(T item)
			{
				if (Paused)
				{
					return;
				}
				if (IsFull)
				{
					if (!_overwrite)
					{
						throw new InvalidOperationException();
					}
					Pop();
				}
				_array[_head] = item;
				_head = (_head + 1) % _array.Length;
				_count++;
				Assert.Check(_count >= 0 && _count <= _array.Length);
			}

			public void Clear()
			{
				_head = 0;
				_tail = 0;
				_count = 0;
				Array.Clear(_array, 0, _array.Length);
			}

			private T Pop()
			{
				if (IsEmpty)
				{
					throw new InvalidOperationException();
				}
				T result = _array[_tail];
				_array[_tail] = default;
				_tail = (_tail + 1) % _array.Length;
				_count--;
				Assert.Check(_count >= 0 && _count <= _array.Length);
				return result;
			}
		}

		public class FloatStatsBuffer : StatsBufferBase<FloatSample>, IStatsBuffer
		{
			public FusionGraphVisualization DefaultVisualization => FusionGraphVisualization.ContinuousTick;

			public FusionGraphVisualization VisualizationFlags => FusionGraphVisualization.ContinuousTick | FusionGraphVisualization.ValueHistogram;

			public FloatStatsBuffer(int size, bool overwrite)
				: base(size, overwrite)
			{
			}
		}

		public class RPCStatsBuffer : StatsBufferBase<TickSample<RPCSample>>, IStatsBuffer
		{
			public FusionGraphVisualization DefaultVisualization => FusionGraphVisualization.CountHistogram;

			public FusionGraphVisualization VisualizationFlags => FusionGraphVisualization.CountHistogram;

			public RPCStatsBuffer(int size, bool overwrite)
				: base(size, overwrite)
			{
			}
		}

		public class TickFloatBuffer : StatsBufferBase<TickSample<FloatSample>>, IStatsBuffer
		{
			public FusionGraphVisualization DefaultVisualization => FusionGraphVisualization.ValueHistogram;

			public FusionGraphVisualization VisualizationFlags => FusionGraphVisualization.IntermittentTick | FusionGraphVisualization.ValueHistogram;

			public TickFloatBuffer(int size, bool overwrite)
				: base(size, overwrite)
			{
			}
		}

		public class TimeFloatBuffer : StatsBufferBase<TimeSample<FloatSample>>, IStatsBuffer
		{
			public FusionGraphVisualization DefaultVisualization => FusionGraphVisualization.ValueHistogram;

			public FusionGraphVisualization VisualizationFlags => FusionGraphVisualization.IntermittentTick | FusionGraphVisualization.ValueHistogram;

			public TimeFloatBuffer(int size, bool overwrite)
				: base(size, overwrite)
			{
			}
		}

		private const StatFlags StatFlagsAllTrue = (StatFlags)(-1);

		private const StatFlags StatFlagsDefault = StatFlags.ValidOnClient | StatFlags.ValidInShared | StatFlags.ValidWithDeltaSnapshot | StatFlags.ValidWithEventualConsistency | StatFlags.ValidForBuildType;

		private const StatFlags StatFlagsEC_Clnt = StatFlags.ValidOnClient | StatFlags.ValidInShared | StatFlags.ValidWithEventualConsistency;

		private const StatFlags StatFlagReleaseOnly = StatFlags.ValidForBuildType;

		public const int NET_STAT_TYPE_COUNT = 3;

		public static readonly StatSourceInfo[] NetStatSourceInfo = new StatSourceInfo[3]
		{
			new StatSourceInfo("Round Trip Time (ms)", "RTT (ms)", 1000.0, 0, 120f, 500f, StatsPer.Individual, (StatsPer)0, StatFlags.ValidOnClient | StatFlags.ValidInShared | StatFlags.ValidWithDeltaSnapshot | StatFlags.ValidWithEventualConsistency | StatFlags.ValidForBuildType, 200, 1000.0),
			new StatSourceInfo("Sent Packet Sizes (bytes)", "Sent Packet (bytes)", 1.0, 1, float.PositiveInfinity, float.PositiveInfinity, StatsPer.Individual, StatsPer.Individual | StatsPer.Second),
			new StatSourceInfo("Rcvd Packet Sizes (bytes)", "Rcvd Packet (bytes)", 1.0, 1, float.PositiveInfinity, float.PositiveInfinity, StatsPer.Individual, StatsPer.Individual | StatsPer.Second)
		};

		public const int OBJ_STAT_TYPE_COUNT = 2;

		public static readonly StatSourceInfo[] ObjStatSourceInfo = new StatSourceInfo[2]
		{
			new StatSourceInfo("Rcvd Object State (bytes)", "Rcvd Obj State (bytes)", 0.125, 1, 20f, 100f, StatsPer.Tick, StatsPer.Tick | StatsPer.Second, StatFlags.ValidOnClient | StatFlags.ValidInShared | StatFlags.ValidWithEventualConsistency | StatFlags.ValidOnStateAuthority | StatFlags.ValidForBuildType),
			new StatSourceInfo("RPCs", "RPCs", 1.0, 1, float.PositiveInfinity, float.PositiveInfinity, StatsPer.Tick, StatsPer.Tick | StatsPer.Second, StatFlags.ValidOnClient | StatFlags.ValidInShared | StatFlags.ValidWithEventualConsistency | StatFlags.ValidOnStateAuthority | StatFlags.ValidForBuildType)
		};

		public const int SIM_STAT_TYPE_COUNT = 16;

		public static readonly StatSourceInfo[] SimStatSourceInfo = new StatSourceInfo[16]
		{
			new StatSourceInfo("FrameTime (ms)", null, 1000.0, 0, float.PositiveInfinity, float.PositiveInfinity, StatsPer.Individual, (StatsPer)0, (StatFlags)(-1)),
			new StatSourceInfo("Forward Simulation Count", "Fwd Sim Count", 1.0, 1, 5f, 10f, StatsPer.Second, (StatsPer)0, (StatFlags)(-1)),
			new StatSourceInfo("Resimulation Count", "Resim Count", 1.0, 1, 5f, 10f, StatsPer.Second, (StatsPer)0, (StatFlags)(-1)),
			new StatSourceInfo("Snapshot Size (bytes)", null, 1.0, 1),
			new StatSourceInfo("Snapshot Delta (ms)", null, 1000.0, 0),
			new StatSourceInfo("Interpolation Timescale (%)", "Interp Timescale (%)", 100.0, 1),
			new StatSourceInfo("Interpolation Offset (ms)", "Interp Offset (ms)", 1000.0, 0),
			new StatSourceInfo("Interpolation Diff (ms)", "Interp Diff (ms)", 1000.0, 0),
			new StatSourceInfo("Interpolation Uncertainty (%)", "Interp Uncertainty (%)", 100.0, 1),
			new StatSourceInfo("Interpolation Multiplier", "Interp Multiplier", 1.0, 1),
			new StatSourceInfo("Prediction Timescale (%)", "Predict Timescale (%)", 100.0, 1),
			new StatSourceInfo("Prediction Offset Target (ms)", "Predict Offset Targ (ms)", 1000.0, 0),
			new StatSourceInfo("Prediction Offset (ms)", "Predict Offset (ms)", 1000.0, 0),
			new StatSourceInfo("Prediction Offset Deviation (ms)", "Predict Offset Dev (ms)", 1000.0, 0),
			new StatSourceInfo("Prediction Delta (ms)", "Predict Delta (ms)", 1000.0, 0),
			new StatSourceInfo("Prediction Delta Deviation (ms)", "Predict Delta Dev (ms)", 1000.0, 0)
		};

		public const int SAMPLE_SIZE = 512;

		public const int OBJECT_SAMPLE_SIZE = 16;

		private Timer _timer;

		private bool _paused;

		private FloatStatsBuffer[] _buffers;

		private Dictionary<NetworkId, TickFloatBuffer> _objectBandwidthBuffers;

		private Dictionary<NetworkId, RPCStatsBuffer> _objectRpcBuffers;

		public Timer Timer => _timer;

		public IStatsBuffer GetObjectBuffer(NetworkId id, ObjStats statId, bool createIfMissing = false)
		{
			return statId switch
			{
				ObjStats.Bandwidth => GetObjectBandwidthBuffer(id, createIfMissing), 
				ObjStats.RPC => GetObjectRpcBuffer(id, createIfMissing), 
				_ => null, 
			};
		}

		public TickFloatBuffer GetObjectBandwidthBuffer(NetworkId id, bool createIfMissing = false)
		{
			if (!_objectBandwidthBuffers.TryGetValue(id, out var value))
			{
				if (!createIfMissing)
				{
					return null;
				}
				_objectBandwidthBuffers.Add(id, value = new TickFloatBuffer(16, overwrite: true));
			}
			return value;
		}

		public RPCStatsBuffer GetObjectRpcBuffer(NetworkId id, bool createIfMissing = false)
		{
			if (!_objectRpcBuffers.TryGetValue(id, out var value))
			{
				if (!createIfMissing)
				{
					return null;
				}
				_objectRpcBuffers.Add(id, value = new RPCStatsBuffer(16, overwrite: true));
			}
			return value;
		}

		public FloatStatsBuffer GetStatBuffer(SimStats statistic)
		{
			return _buffers[(int)statistic];
		}

		public unsafe IStatsBuffer GetStatBuffer(NetStats statistic, NetworkRunner runner)
		{
			if (!runner.IsRunning || !runner.IsClient)
			{
				return null;
			}
			NetConnection* serverConnection = (runner.Simulation as Client).ServerConnection;
			if (serverConnection == null)
			{
				return null;
			}
			return statistic switch
			{
				NetStats.RoundTripTime => serverConnection->StatsRoundTripTime, 
				NetStats.SentPacketSizes => serverConnection->StatsSentPacketSizes, 
				NetStats.ReceivedPacketSizes => serverConnection->StatsReceivedPacketSizes, 
				_ => null, 
			};
		}

		internal Statistics()
		{
			_paused = false;
			int[] array = (int[])Enum.GetValues(typeof(SimStats));
			_buffers = new FloatStatsBuffer[array.Length];
			_objectBandwidthBuffers = new Dictionary<NetworkId, TickFloatBuffer>();
			_objectRpcBuffers = new Dictionary<NetworkId, RPCStatsBuffer>();
			_timer = Timer.StartNew();
			for (int i = 0; i < array.Length; i++)
			{
				CreateBuffer((SimStats)i);
			}
		}

		public void Pause(bool paused)
		{
			_paused = paused;
			FloatStatsBuffer[] buffers = _buffers;
			foreach (FloatStatsBuffer floatStatsBuffer in buffers)
			{
				floatStatsBuffer.Paused = _paused;
			}
		}

		public void Clear()
		{
			FloatStatsBuffer[] buffers = _buffers;
			foreach (FloatStatsBuffer floatStatsBuffer in buffers)
			{
				floatStatsBuffer.Clear();
			}
		}

		private FloatStatsBuffer CreateBuffer(SimStats statId)
		{
			FloatStatsBuffer floatStatsBuffer = new FloatStatsBuffer(512, overwrite: true);
			floatStatsBuffer.Paused = _paused;
			_buffers[(int)statId] = floatStatsBuffer;
			return floatStatsBuffer;
		}

		public static StatSourceInfo GetDescription(SimStatFlags statFlag)
		{
			return GetDescription(StatSourceTypes.Simulation, (int)Math.Log((double)statFlag, 2.0));
		}

		public static StatSourceInfo GetDescription(SimStats statId)
		{
			return GetDescription(StatSourceTypes.Simulation, (int)statId);
		}

		public static StatSourceInfo GetDescription(ObjStatFlags statFlag)
		{
			return GetDescription(StatSourceTypes.NetworkObject, (int)Math.Log((double)statFlag, 2.0));
		}

		public static StatSourceInfo GetDescription(ObjStats statId)
		{
			return GetDescription(StatSourceTypes.NetworkObject, (int)statId);
		}

		public static StatSourceInfo GetDescription(NetStatFlags statFlag)
		{
			return GetDescription(StatSourceTypes.NetConnection, (int)Math.Log((double)statFlag, 2.0));
		}

		public static StatSourceInfo GetDescription(NetStats statId)
		{
			return GetDescription(StatSourceTypes.NetConnection, (int)statId);
		}

		public static StatSourceInfo GetDescription(StatSourceTypes statSource, int statId)
		{
			return ((StatSourceInfo[])(statSource switch
			{
				StatSourceTypes.NetworkObject => ObjStatSourceInfo, 
				StatSourceTypes.Simulation => SimStatSourceInfo, 
				_ => NetStatSourceInfo, 
			}))[statId];
		}
	}

	public static float RELAY_SLACK = 2f;

	private bool _isShutdown;

	private bool _isWaitingForShutdown;

	private unsafe Allocator* _tempAllocator;

	private ICallbacks _callbacks;

	private IDeltaCompressor _deltaCompressor;

	private SimulationModes _mode;

	private SimulationStages _stage;

	private SimulationConfig _config;

	private NetworkProjectConfig _projectConfig;

	private SimulationSnapshot.IHistory _history;

	private SimulationSnapshot.Pool _historyPool;

	private SimulationSnapshot _state;

	private SimulationSnapshot _statePrevious;

	private SimulationSnapshot _stateInterestGroups;

	private SimulationSnapshot _stateResume;

	private SimulationSnapshot _interpTo;

	private SimulationSnapshot _interpFrom;

	private float _interpAlpha;

	private SimulationInput _inputRoot;

	private SimulationInput.Pool _inputPool;

	private SimulationInputCollection _inputCollection;

	private StateReplicator _stateReplicator;

	private SimulationConnection[] _connections;

	private SimulationPlayer[] _players;

	private TickAccumulator _accumulator;

	private int _stepSize;

	private float _stepDeltaFloat;

	private double _stepDeltaDouble;

	private double _updateTime;

	private double _updateDelta;

	private Ema _updateDeltaAvg;

	private bool _isLastTick;

	private bool _isFirstTick;

	private bool _isResimulation;

	private bool _isResume;

	private bool _isInTick;

	private bool? _isPaused;

	private unsafe AreaOfInterest* _aoi;

	private Dictionary<Tick, double> _tickUpdateTimes;

	private List<NetworkId> _globalInterestObjects;

	private int _ticksWithoutSend = 0;

	public const int DEFAULT_COMPRESSOR_OFFSET_BLOCK_SIZE = 3;

	public const int DEFAULT_COMPRESSOR_VALUE_BLOCK_SIZE = 6;

	internal INetSocket _netSocket;

	internal unsafe NetPeer* _netPeer;

	private unsafe NetPeerGroup* _netPeerGroup;

	private System.Random _netPeerRng;

	public Statistics Stats = new Statistics();

	public bool IsShutdown => _isShutdown;

	public bool IsResimulation => _isResimulation;

	public bool IsLastTick => _isLastTick;

	public bool IsFirstTick => _isFirstTick;

	public bool IsForward => !_isResimulation;

	public bool IsLocalPlayerFirstExecution => _stage == SimulationStages.Forward;

	public SimulationSnapshot State => _state;

	public SimulationSnapshot StatePrevious => _statePrevious;

	public float StateAlpha => IsPlayer ? ((float)Maths.Clamp01(_accumulator.Remainder / (double)DeltaTime)) : 0f;

	public SimulationSnapshot StateResume => _stateResume;

	public Tick Tick => _state.Tick;

	public int InputCount => _inputCollection.Count;

	public int MaxConnections => _connections.Length;

	public SimulationConfig.Topologies Topology => _config.Topology;

	public SimulationSnapshot.IHistory SnapshotHistory => _history;

	public SimulationModes Mode => _mode;

	public SimulationStages Stage => _stage;

	public SimulationConfig Config => _config;

	public NetworkProjectConfig ProjectConfig => _projectConfig;

	public float DeltaTime => _stepDeltaFloat;

	public SimulationSnapshot InterpTo => _interpTo;

	public SimulationSnapshot InterpFrom => _interpFrom;

	public float InterpAlpha => _interpAlpha;

	public bool IsClient => this is Client;

	public bool IsServer => this is Server;

	public bool IsPlayer => _mode == SimulationModes.Client || _mode == SimulationModes.Host;

	public bool IsSinglePlayer => _mode == SimulationModes.Host && _config.DefaultPlayers == 1;

	public bool IsMasterClient => _callbacks.IsSharedModeMasterClient;

	public virtual IEnumerable<PlayerRef> ActivePlayers
	{
		get
		{
			if (IsPlayer)
			{
				yield return LocalPlayer;
			}
			if (!IsServer)
			{
				yield break;
			}
			int i = 0;
			while (i < _connections.Length)
			{
				if (_connections[i].Active)
				{
					yield return i;
				}
				int num = i + 1;
				i = num;
			}
		}
	}

	public bool IsRunning => !_isShutdown;

	internal StateReplicator Replicator => _stateReplicator;

	internal ICallbacks Callbacks => _callbacks;

	internal bool IsResume => _isResume;

	internal bool IsInTick => _isInTick;

	internal bool IsPaused => _isPaused.HasValue && _isPaused.Value;

	public unsafe NetAddress LocalAddress => _netPeer->Address;

	public unsafe NetConfig* NetConfigPointer => NetPeer.GetConfigPointer(_netPeer);

	public abstract PlayerRef LocalPlayer { get; }

	public abstract SimulationSnapshot LatestServerState { get; }

	internal abstract double GetPlayerRtt(PlayerRef player);

	internal unsafe abstract void RecvPacket(NetConnection* connection, NetBitBuffer* buffer);

	internal unsafe abstract void SendPacket(NetConnection* connection, NetBitBuffer* buffer, SimulationPacketEnvelope* envelope);

	internal abstract SimulationInput GetInput(Tick tick, PlayerRef player);

	internal unsafe Simulation(SimulationArgs args)
	{
		Assert.Check(sizeof(NetworkObjectHeader) == 80, "NetworkObjectHeader size != WORD_COUNT * REPLICATE_WORD_SIZE");
		if (args.Config.Simulation.ReplicationMode == SimulationConfig.StateReplicationModes.DeltaSnapshots)
		{
			_deltaCompressor = args.DeltaCompressor;
			Log.Debug(this, "Using Delta Compressor: " + _deltaCompressor.GetType().FullName);
		}
		_mode = args.Mode;
		_config = args.Config.Simulation;
		_projectConfig = args.Config;
		_callbacks = args.Callbacks;
		_isShutdown = false;
		_isWaitingForShutdown = false;
		_tempAllocator = Allocator.Create(new Allocator.Config(PageSizes._64Kb, 256, 1024));
		_inputPool = new SimulationInput.Pool(_config);
		_inputRoot = _inputPool.Acquire();
		_inputCollection = new SimulationInputCollection(_config.DefaultPlayers);
		_aoi = AreaOfInterest.Alloc();
		_historyPool = new SimulationSnapshot.Pool(this, args.Config);
		if (IsServer && args.Config.Simulation.ReplicationMode == SimulationConfig.StateReplicationModes.DeltaSnapshots)
		{
			_history = new SimulationSnapshot.HistoryServerDeltaSnapshots(_historyPool);
		}
		else
		{
			_history = new SimulationSnapshot.HistoryLL(_config.TickRate * 10, _historyPool);
		}
		_state = _historyPool.Create();
		_players = new SimulationPlayer[_config.DefaultPlayers];
		for (int i = 0; i < _players.Length; i++)
		{
			_players[i] = new SimulationPlayer();
		}
		if (IsServer && Config.ReplicationMode == SimulationConfig.StateReplicationModes.EventualConsistency)
		{
			_stateInterestGroups = _historyPool.Create();
		}
		if (IsServer && (_isResume = args.State != null))
		{
			Assert.Check(condition: true, "Invalid Resume Tick");
			Assert.Check(args.State.Length == _state.Allocator->ReplicateByteLength, "Invalid Resume State Size");
			_stateResume = _historyPool.Acquire();
			fixed (byte* state = args.State)
			{
				Native.MemCpy(_stateResume.Allocator->Replicate, state, args.State.Length);
			}
			_stateResume.SetTick(args.Tick);
			_state.SetTick(args.Tick);
			(this as Server).CreateInternalStateObject();
		}
		if (!IsPlayer)
		{
			Assert.Check(IsServer, "Only the Server can reset the Resume State");
			_statePrevious = _state;
		}
		else
		{
			_statePrevious = _historyPool.Create();
		}
		NetworkInit(args.Socket, args.Address);
		switch (args.Config.Simulation.ReplicationMode)
		{
		case SimulationConfig.StateReplicationModes.DeltaSnapshots:
			_stateReplicator = new StateReplicatorDeltaSnapshot(this);
			break;
		case SimulationConfig.StateReplicationModes.EventualConsistency:
			if (_config.Topology == SimulationConfig.Topologies.Shared)
			{
				_stateReplicator = new StateReplicatorEventualConsistencyShared(this);
			}
			else
			{
				_stateReplicator = new StateReplicatorEventualConsistencyClientServer(this);
			}
			break;
		}
		if (_stateReplicator.UseObjectInterest)
		{
			_globalInterestObjects = new List<NetworkId>();
		}
		_tickUpdateTimes = new Dictionary<Tick, double>();
		if (IsServer)
		{
			_accumulator = TickAccumulator.StartNew();
			if (IsPlayer)
			{
				StepDeltaInit(1);
			}
			else
			{
				StepDeltaInit(_config.ServerTickMultiplier);
			}
		}
		else
		{
			StepDeltaInit(1);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void CalculateUpdateTime()
	{
		if (!IsClient)
		{
			double updateTime = _updateTime;
			_updateTime = (double)(int)Tick * _stepDeltaDouble + (double)_accumulator.Pending * _stepDeltaDouble + _accumulator.Remainder;
			Assert.Check(_updateTime > updateTime, $"Current Update Time must be bigger than previous Update Time ({_updateTime} > {updateTime})");
		}
	}

	private void StepDeltaInit(int multiplier)
	{
		multiplier = Math.Max(multiplier, 1);
		_stepSize = multiplier;
		_stepDeltaDouble = _config.DeltaTime * (double)_stepSize;
		_stepDeltaFloat = (float)_stepDeltaDouble;
	}

	private void StepSimulation(SimulationStages stage, bool lastTick, bool firstTick, bool freeInput)
	{
		EngineProfiler.Begin("Simulation.StepSimulation");
		try
		{
			bool isResimulation = stage == SimulationStages.Resimulate;
			_isLastTick = lastTick;
			_isFirstTick = firstTick;
			_isResimulation = isResimulation;
			if (IsLastTick && IsPlayer && !IsResimulation)
			{
				Assert.Check(!IsResimulation, "IsResimulation should be false");
				_callbacks.OnBeforeCopyPreviousState();
				_statePrevious.CopyFrom(_state);
			}
			_state.SetTick(_state.Tick.Next(_stepSize));
			if (IsServer)
			{
				_tickUpdateTimes.Remove((int)_state.Tick - _config.TickRate);
				_tickUpdateTimes.Add(_state.Tick, _updateTime);
			}
			InvokeTick(stage, freeInput);
		}
		catch (Exception exn)
		{
			Log.Exception(this, exn);
		}
		finally
		{
			_isLastTick = false;
			_isFirstTick = false;
			_isResimulation = false;
			EngineProfiler.End();
		}
	}

	protected virtual void OnPlayerJoinedLeftInternalMessage(PlayerRef player, bool joined)
	{
	}

	protected virtual void AfterUpdate()
	{
	}

	protected unsafe virtual void NetworkConnected(NetConnection* connection)
	{
	}

	protected unsafe virtual void NetworkDisconnected(NetConnection* connection)
	{
	}

	protected virtual void NetworkReceiveDone()
	{
	}

	protected virtual void NoSimulation()
	{
	}

	protected virtual int BeforeSimulation()
	{
		return 0;
	}

	protected virtual void BeforeTick()
	{
	}

	protected virtual void AfterTick()
	{
	}

	protected virtual void AfterSimulation()
	{
	}

	internal void SinglePlayerSetPaused(bool paused)
	{
		if (IsSinglePlayer)
		{
			_isPaused = paused;
		}
	}

	internal virtual void AreaOfInterestQueryAdded(PlayerRef player, SimulationPlayer.AOIQuery query)
	{
	}

	internal unsafe NetworkObjectHeader* AllocateObject(NetworkId id, NetworkPrefabId type, int wordCount, out int* groups, NetworkId? nestingRoot = null, NetworkObjectNestingKey? nestingKey = null)
	{
		NetworkObjectHeader* ptr = _state.AllocateObject(id, type, wordCount, nestingRoot, nestingKey);
		if (IsServer && Config.ReplicationMode == SimulationConfig.StateReplicationModes.EventualConsistency)
		{
			groups = (int*)_stateInterestGroups.Allocator->Ptr(_state.Allocator->Ptr(ptr));
			Native.MemClear(groups, ptr->WordCount * 4);
		}
		else
		{
			groups = null;
		}
		return ptr;
	}

	internal unsafe void RequestStateAuthority(NetworkId id, bool wants)
	{
		if (Topology == SimulationConfig.Topologies.Shared)
		{
			Assert.Check(sizeof(SimulationMessageInternal_SharedModeRequestStateAuthority) == 8, "SharedModeRequestStateAuthority unexpected size");
			SimulationMessageInternal_SharedModeRequestStateAuthority buffer = default;
			buffer.Acquire = (wants ? 1 : 0);
			buffer.Object = id;
			SendInternalSimulationMessage(SimulationMessageInternalTypes.SharedModeRequestStateAuthority, buffer);
		}
	}

	internal unsafe void SetPlayerAlwaysInterested(PlayerRef player, NetworkId id, bool alwaysInterested)
	{
		if (player.IsValid && Config.ObjectInterest)
		{
			if (Topology == SimulationConfig.Topologies.Shared)
			{
				Assert.Check(sizeof(SimulationMessageInternal_SharedModeSetAlwaysInterested) == 8, "SharedModeSetAlwaysInterested unexpected size");
				SimulationMessageInternal_SharedModeSetAlwaysInterested buffer = default;
				buffer.Interested = (alwaysInterested ? 1 : 0);
				buffer.Object = id;
				SendInternalSimulationMessage(SimulationMessageInternalTypes.SharedModeSetAlwaysInterested, buffer);
			}
			else if (alwaysInterested)
			{
				_players[(int)player].AlwaysInterested.Add(id);
			}
			else
			{
				_players[(int)player].AlwaysInterested.Remove(id);
			}
		}
	}

	internal void AddPlayerAreaOfInterest(PlayerRef player, Vector3 position, float extent, int layerMask = -1)
	{
		if (IsLastTick && player.IsValid && !IsResimulation && Config.ObjectInterest && (!IsClient || !(player != LocalPlayer)))
		{
			SimulationPlayer.AOIQuery aOIQuery = new SimulationPlayer.AOIQuery
			{
				Position = position,
				Radius = extent,
				LayerMask = layerMask
			};
			_players[(int)player].AOIQueries.Add(aOIQuery);
			AreaOfInterestQueryAdded(player, aOIQuery);
		}
	}

	internal unsafe void TempFree(void* ptr)
	{
		if (_tempAllocator->IsPointerInHeap(ptr))
		{
			Allocator.Free(_tempAllocator, ptr);
		}
		else
		{
			Assert.AlwaysFail("Pointer not part of temp allocator");
		}
	}

	internal unsafe void* TempAlloc(int size)
	{
		if (_tempAllocator->CanAllocSizeAssert_Temp(size))
		{
			return Allocator.AllocAndClear(_tempAllocator, size);
		}
		return null;
	}

	internal unsafe void* TempAllocNoClear(int size)
	{
		if (_tempAllocator->CanAllocSizeAssert_Temp(size))
		{
			return Allocator.Alloc(_tempAllocator, size);
		}
		return null;
	}

	internal unsafe T* TempAlloc<T>() where T : unmanaged
	{
		return (T*)TempAlloc(sizeof(T));
	}

	internal unsafe T* TempAllocArray<T>(int length) where T : unmanaged
	{
		return (T*)TempAlloc(sizeof(T) * length);
	}

	internal unsafe T* TempDoubleArray<T>(T* oldArray, int oldLength) where T : unmanaged
	{
		int length = oldLength * 2;
		T* ptr = TempAllocArray<T>(length);
		Native.MemCpy(ptr, oldArray, sizeof(T) * oldLength);
		TempFree(oldArray);
		return ptr;
	}

	internal void ShutdownNativeSocket()
	{
		if (!_isShutdown)
		{
			NetworkShutdown();
		}
	}

	internal unsafe void Dispose()
	{
		if (!_isShutdown)
		{
			_isShutdown = true;
			Allocator.Dispose(_tempAllocator);
			_tempAllocator = null;
			_history.Dispose();
			_history = null;
			_historyPool.Dispose();
			_historyPool = null;
			_state = null;
			_statePrevious = null;
			_stateInterestGroups = null;
			_stateReplicator.Dispose();
			_interpTo = null;
			_interpFrom = null;
			_inputPool.Dispose();
			_inputPool = null;
			_inputRoot = null;
			AreaOfInterest.Free(_aoi);
			_aoi = null;
		}
	}

	internal void Destroy(NetworkId id, NetworkObjectDestroyFlags flags)
	{
		Replicator.OnObjectDestroyed(id, flags);
	}

	internal bool PlayerValid(PlayerRef player)
	{
		return (int)player >= 0 && (int)player < _players.Length;
	}

	internal bool PlayerActive(PlayerRef player)
	{
		return PlayerValid(player) && GetSimulationConnectionByIndex(player)?.Active == true;
	}

	internal unsafe byte[] GetPlayerConnectionToken(PlayerRef player)
	{
		if (PlayerActive(player))
		{
			SimulationConnection simulationConnectionByIndex = GetSimulationConnectionByIndex(player);
			if (simulationConnectionByIndex.Connection->ConnectionToken != null && simulationConnectionByIndex.Connection->ConnectionTokenLength > 0)
			{
				byte[] array = new byte[simulationConnectionByIndex.Connection->ConnectionTokenLength];
				fixed (byte* destination = array)
				{
					Native.MemCpy(destination, simulationConnectionByIndex.Connection->ConnectionToken, simulationConnectionByIndex.Connection->ConnectionTokenLength);
				}
				return array;
			}
		}
		return null;
	}

	internal unsafe NetAddress GetPlayerAddress(PlayerRef player)
	{
		if (PlayerActive(player))
		{
			SimulationConnection simulationConnectionByIndex = GetSimulationConnectionByIndex(player);
			if (simulationConnectionByIndex != null)
			{
				return simulationConnectionByIndex.Connection->Address;
			}
		}
		return default;
	}

	internal unsafe long GetPlayerUniqueId(PlayerRef player)
	{
		if (PlayerActive(player))
		{
			SimulationConnection simulationConnectionByIndex = GetSimulationConnectionByIndex(player);
			if (simulationConnectionByIndex != null)
			{
				return simulationConnectionByIndex.Connection->UniqueIdHash;
			}
		}
		return 0L;
	}

	public SimulationInput GetInputForPlayer(int player)
	{
		return _inputCollection.GetByPlayer(player);
	}

	public SimulationInput GetInputByIndex(int index)
	{
		return _inputCollection.GetByIndex(index);
	}

	internal unsafe NetworkId GetPlayerObjectId(PlayerRef player)
	{
		uint* playerObjectIdTable = GetPlayerObjectIdTable();
		if (playerObjectIdTable != null)
		{
			return new NetworkId(playerObjectIdTable[(int)player]);
		}
		return default;
	}

	internal unsafe int? GetPlayerActorId(PlayerRef player)
	{
		if (_config.Topology == SimulationConfig.Topologies.ClientServer)
		{
			return null;
		}
		if (!PlayerValid(player))
		{
			return null;
		}
		int* playerActorIdTable = GetPlayerActorIdTable();
		if (playerActorIdTable != null && playerActorIdTable[(int)player] > 0)
		{
			return playerActorIdTable[(int)player] - 1;
		}
		return null;
	}

	internal unsafe void SetPlayerObjectId(PlayerRef player, NetworkId id)
	{
		if (!PlayerValid(player))
		{
			return;
		}
		if (Topology == SimulationConfig.Topologies.ClientServer)
		{
			if (!IsClient)
			{
				GetPlayerObjectIdTable()[(int)player] = id.Raw;
				Log.Debug($"SetPlayerObjectId: {player}={GetPlayerObjectIdTable()[(int)player]}");
			}
		}
		else if (IsClient)
		{
			if (State.TryGetObject(id, out var header) && header->StateAuthority == LocalPlayer && player == LocalPlayer)
			{
				SimulationMessageInternal_SetPlayerObject buffer = default;
				buffer.Object = id;
				SendInternalSimulationMessage(SimulationMessageInternalTypes.SetPlayerObject, buffer);
			}
		}
		else
		{
			GetPlayerObjectIdTable()[(int)player] = id.Raw;
		}
	}

	private unsafe uint* GetPlayerObjectIdTable()
	{
		if (State.TryGetObject(NetworkId.InternalState, out var header))
		{
			return (uint*)header + 20;
		}
		return null;
	}

	private unsafe int* GetPlayerActorIdTable()
	{
		if (State.TryGetObject(NetworkId.InternalState, out var header))
		{
			return (int*)((byte*)header + (nint)20 * (nint)4) + Config.DefaultPlayers;
		}
		return null;
	}

	public unsafe bool HasAnyActiveConnections()
	{
		NetConnectionMap.Iterator iterator = NetPeerGroup.ConnectionIterator(_netPeerGroup);
		while (iterator.Next())
		{
			if (iterator.Current->ConnectionStatus != NetConnectionStatus.Connected)
			{
				continue;
			}
			return true;
		}
		return false;
	}

	private void InvokeOnBeforeAllTicks(bool resimulation, int ticks)
	{
		EngineProfiler.Begin("InvokeOnBeforeAllTicks");
		try
		{
			_callbacks.OnBeforeAllTicks(resimulation, ticks);
		}
		catch (Exception exn)
		{
			Log.Exception(this, exn);
		}
		EngineProfiler.End();
	}

	private void InvokeOnAfterAllTicks(bool resimulation, int ticks)
	{
		EngineProfiler.Begin("InvokeOnAfterAllTicks");
		try
		{
			_callbacks.OnAfterAllTicks(resimulation, ticks);
		}
		catch (Exception exn)
		{
			Log.Exception(this, exn);
		}
		EngineProfiler.End();
	}

	protected virtual void BeforeNetworkRecv()
	{
	}

	public int Update(double dt)
	{
		if (_isShutdown || dt == 0.0)
		{
			return 0;
		}
		EngineProfiler.Begin("Simulation.Update");
		Stats.GetStatBuffer(Statistics.SimStats.FrameTime).Push((float)dt);
		_updateDeltaAvg.Add(dt);
		_updateDelta = dt;
		_accumulator.AddTime(_updateDelta, _stepDeltaDouble, Config.TickRate / 2);
		if (IsServer && _accumulator.Running)
		{
			CalculateUpdateTime();
		}
		BeforeNetworkRecv();
		NetworkRecv();
		_interpAlpha = 0f;
		_interpFrom = null;
		_interpTo = null;
		int num = 0;
		if (!_isWaitingForShutdown && _accumulator.Pending > 0)
		{
			EngineProfiler.Begin("BeforeSimulation");
			int num2 = BeforeSimulation();
			_callbacks.OnBeforeSimulation();
			EngineProfiler.End();
			if (_accumulator.Pending > 0)
			{
				try
				{
					num = _accumulator.Pending;
					_ticksWithoutSend += num;
					InvokeOnBeforeAllTicks(resimulation: false, num);
					bool firstTick = true;
					bool last;
					while (_accumulator.ConsumeTick(out last))
					{
						StepSimulation(SimulationStages.Forward, last, firstTick, IsServer);
						firstTick = false;
					}
					InvokeOnAfterAllTicks(resimulation: false, num);
				}
				catch (Exception exn)
				{
					Log.Exception(this, exn);
				}
				EngineProfiler.Begin("AfterSimulation");
				_callbacks.OnAfterSimulation();
				AfterSimulation();
				EngineProfiler.End();
				try
				{
					if (_config.EnableHalfNetworkTick && Topology == SimulationConfig.Topologies.Shared && _config.TickRate > 30)
					{
						if (_ticksWithoutSend >= 2)
						{
							_ticksWithoutSend = 0;
							PreparePackets();
						}
					}
					else
					{
						PreparePackets();
					}
				}
				catch (Exception exn2)
				{
					Log.Exception(this, exn2);
				}
			}
			Stats.GetStatBuffer(Statistics.SimStats.ForwardSimCount).Push(num);
			Stats.GetStatBuffer(Statistics.SimStats.ResimCount).Push(num2);
		}
		else
		{
			NoSimulation();
		}
		NetworkSend();
		Assert.Check(_stage == (SimulationStages)0, "Invalid Simulation.Stage", _stage);
		EngineProfiler.End();
		AfterUpdate();
		return num;
	}

	private SimulationPlayer GetSimulationPlayer(PlayerRef player)
	{
		return _players[(int)player];
	}

	private unsafe void UpdateAreaOfInterest()
	{
		if (!_projectConfig.AccuracyDefaults.TryGetAccuracy("Position", out var accuracy))
		{
			Assert.AlwaysFail("Accuracy not found: Position");
		}
		try
		{
			int num = 0;
			for (int i = 0; i < _players.Length; i++)
			{
				num += _players[i].AOIQueries.Count;
			}
			if (num == 0)
			{
				return;
			}
			NetworkObjectRefMapPtr* objectTable = State.ObjectTable;
			int count = objectTable->Count;
			if (count == 0)
			{
				return;
			}
			AreaOfInterest.Reset(_aoi, count, num);
			for (int j = 0; j < _players.Length; j++)
			{
				SimulationPlayer simulationPlayer = _players[j];
				for (int k = 0; k < simulationPlayer.AOIQueries.Count; k++)
				{
					SimulationPlayer.AOIQuery aOIQuery = simulationPlayer.AOIQueries[k];
					if (aOIQuery.Radius > 0f)
					{
						AreaOfInterest.AddQuery(_aoi, accuracy, aOIQuery.Position, aOIQuery.Radius, aOIQuery.LayerMask, (void*)j);
					}
				}
				simulationPlayer.AOIQueries.Clear();
				simulationPlayer.AOIResult.Clear();
			}
			if (AreaOfInterest.BurstInsertAndResolve == null)
			{
				AreaOfInterest.InsertObjects(_aoi, accuracy, State.Allocator, objectTable);
				AreaOfInterest.Resolve(_aoi, State.Allocator);
			}
			else
			{
				AreaOfInterest.BurstInsertAndResolve(_aoi, &accuracy, State.Allocator, objectTable);
			}
			int queryCount = _aoi->QueryCount;
			for (int l = 0; l < queryCount; l++)
			{
				AreaOfInterest.RadixQuery* query = _aoi->GetQuery(l);
				if (query->HitsCount <= 0)
				{
					continue;
				}
				int num2 = (int)query->UserData;
				SimulationPlayer simulationPlayer2 = _players[num2];
				SimulationConnection simulationConnectionByIndex = GetSimulationConnectionByIndex(num2);
				if (simulationConnectionByIndex != null)
				{
					for (int m = 0; m < query->HitsCount; m++)
					{
						NetworkObjectHeader* ptr = query->Hits[m];
						simulationPlayer2.AOIResult.Add(ptr->Id);
						if (!simulationConnectionByIndex.ObjectPriorityHeap.Contains(ptr->Id))
						{
							if (ptr->TransformOffset > 0)
							{
								int* data = (int*)ptr + ptr->TransformOffset;
								Vector3 vector = ReadWriteUtilsForWeaver.ReadVector3(data, accuracy._value);
								simulationConnectionByIndex.ObjectPriorityHeap.Push(ptr->Id, 1f / (vector - query->Position).sqrMagnitude);
							}
							else
							{
								simulationConnectionByIndex.ObjectPriorityHeap.Push(ptr->Id, 0.1f);
							}
						}
					}
				}
				else
				{
					for (int n = 0; n < query->HitsCount; n++)
					{
						simulationPlayer2.AOIResult.Add(query->Hits[n]->Id);
					}
				}
			}
		}
		catch (Exception exn)
		{
			Log.Exception(this, exn);
		}
	}

	private unsafe void PreparePackets()
	{
		if (Replicator.UseObjectInterest)
		{
			EngineProfiler.Begin("UpdateAreaOfInterest");
			UpdateAreaOfInterest();
			EngineProfiler.End();
		}
		EngineProfiler.Begin("SendPackets");
		_stateReplicator.SendBegin();
		NetConnectionMap.Iterator iterator = NetPeerGroup.ConnectionIterator(_netPeerGroup);
		while (iterator.Next())
		{
			NetConnection* current = iterator.Current;
			SimulationConnection simulationConnection = GetSimulationConnection(current);
			if (simulationConnection == null)
			{
				continue;
			}
			double connectionIdleTime = NetPeerGroup.GetConnectionIdleTime(_netPeerGroup, current);
			if (connectionIdleTime >= 1.0 && _netPeerGroup->Time - simulationConnection.LastSend < 0.5)
			{
				continue;
			}
			bool flag = true;
			NetBitBuffer* buffer;
			while (NetworkGetBuffer(current, out buffer))
			{
				SimulationPacketEnvelope* ptr = SimulationPacketEnvelope.Alloc(this);
				ptr->Tick = State.Tick;
				if (flag)
				{
					buffer->WriteBoolean(value: true);
					SendPacket(current, buffer, ptr);
				}
				else
				{
					buffer->WriteBoolean(value: false);
				}
				bool flag2 = simulationConnection.MessagesOut.Count > 0;
				int num = 9088 - buffer->OffsetBits;
				if (num > 0)
				{
					int num2 = ConsumeAndWriteMessagesIntoBuffer(ref simulationConnection.MessagesOut, buffer, num, ref ptr->Messages, !flag);
					if (flag2 && !flag && num2 == 0)
					{
						SimulationMessageEnvelope* head = simulationConnection.MessagesOut.Head;
						Assert.Always(head->Message->Offset > 0, "Message offset invalid", head->Message->Offset);
						Assert.Always(!head->Message->GetFlag(256), "Message has FLAG_DUMMY");
						Log.Error(this, $"Message {*head->Message} (sequence: {head->Sequence}) is too large to be serialized and will be discarded");
						head->Message->SetDummy();
					}
				}
				else if (!flag2)
				{
				}
				simulationConnection.LastSend = _netPeerGroup->Time;
				NetworkSendBuffer(current, buffer, ptr);
				if (simulationConnection.MessagesOut.Count == 0)
				{
					break;
				}
				flag = false;
			}
		}
		_stateReplicator.SendEnd();
		EngineProfiler.End();
	}

	internal void UpdateRemotePrefabs()
	{
		if (!_callbacks.TryBeginUpdateRemotePrefabs())
		{
			return;
		}
		try
		{
			_stateReplicator.UpdateRemotePrefabs();
		}
		finally
		{
			_callbacks.EndUpdateRemotePrefabs();
		}
	}

	private void InvokeTick(SimulationStages stage, bool releaseAllInputs)
	{
		try
		{
			Assert.Check(_inputCollection.Count == 0, "InputCollection Size should be 0");
			_stage = stage;
			for (int i = 0; i < _config.DefaultPlayers; i++)
			{
				SimulationInput input = GetInput(_state.Tick, i);
				if (input != null)
				{
					_inputCollection.AddInput(input);
				}
			}
			if (IsClient && IsFirstTick && (IsResimulation || Config.Topology == SimulationConfig.Topologies.Shared))
			{
				SimulationStages stage2 = _stage;
				try
				{
					_stage = SimulationStages.Forward;
					UpdateRemotePrefabs();
				}
				finally
				{
					_stage = stage2;
				}
			}
			EngineProfiler.Begin("Simulation.BeforeTick");
			BeforeTick();
			_callbacks.OnBeforeTick();
			EngineProfiler.End();
			DeliverMessages(_state.Tick);
			try
			{
				_isInTick = true;
				_callbacks.OnTick();
			}
			catch (Exception exn)
			{
				Log.Error(this, "OnTick Threw Exception");
				Log.Exception(this, exn);
			}
			finally
			{
				_isInTick = false;
			}
			EngineProfiler.Begin("Simulation.AfterTick");
			AfterTick();
			_callbacks.OnAfterTick();
			EngineProfiler.End();
		}
		catch (Exception exn2)
		{
			Log.Exception(this, exn2);
		}
		finally
		{
			_stage = (SimulationStages)0;
			try
			{
				if (releaseAllInputs)
				{
					for (int j = 0; j < _inputCollection.Count; j++)
					{
						_inputPool.Release(_inputCollection.GetByIndex(j));
					}
				}
			}
			finally
			{
				_inputCollection.Clear();
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe SimulationMessageInternalTypes GetMessageInternalType(SimulationMessage* message)
	{
		Assert.Check(condition: true, "SimulationMessageInternalTypes size should be 4");
		return *(SimulationMessageInternalTypes*)SimulationMessage.GetData(message);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe void* GetMessageInternalData(SimulationMessage* message)
	{
		Assert.Check(condition: true, "SimulationMessageInternalTypes size should be 4");
		return SimulationMessage.GetData(message) + 4;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe T GetMessageInternalData<T>(SimulationMessage* message) where T : unmanaged
	{
		Assert.Check(condition: true, "SimulationMessageInternalTypes size should be 4");
		return *(T*)GetMessageInternalData(message);
	}

	private unsafe void OnMessageInternal(SimulationMessage* message)
	{
		SimulationMessageInternalTypes messageInternalType = GetMessageInternalType(message);
		SimulationMessageInternalTypes simulationMessageInternalTypes = messageInternalType;
		if (simulationMessageInternalTypes == SimulationMessageInternalTypes.PlayerJoinedLeft)
		{
			SimulationMessageInternal_PlayerJoinedLeft messageInternalData = GetMessageInternalData<SimulationMessageInternal_PlayerJoinedLeft>(message);
			OnPlayerJoinedLeftInternalMessage(messageInternalData.Player, messageInternalData.Joined == 1);
		}
	}

	private unsafe void DeliverMessages(int tick)
	{
		EngineProfiler.Begin("Simulation.DeliverMessages");
		NetConnectionMap.Iterator iterator = NetPeerGroup.ConnectionIterator(_netPeerGroup);
		while (iterator.Next())
		{
			if (iterator.Current->ConnectionStatus != NetConnectionStatus.Connected)
			{
				continue;
			}
			SimulationConnection simulationConnection = GetSimulationConnection(iterator.Current);
			while (simulationConnection.MessagesIn.Count > 0)
			{
				SimulationMessageEnvelope* head = simulationConnection.MessagesIn.Head;
				if (head == null || head->Message == null)
				{
					simulationConnection.MessagesIn.Remove(head);
					continue;
				}
				if (head->Message->Tick > tick && !head->Message->GetFlag(128) && Topology != SimulationConfig.Topologies.Shared)
				{
					break;
				}
				if (head->Message->GetFlag(8))
				{
					Assert.Check(head->Sequence == 0, "Head Sequence must be 0");
				}
				else
				{
					if (head->Sequence != simulationConnection.MessagesInSequence + 1)
					{
						break;
					}
					simulationConnection.MessagesInSequence++;
				}
				simulationConnection.MessagesIn.Remove(head);
				try
				{
					if (head->Message->GetFlag(64))
					{
						OnMessageInternal(head->Message);
					}
					else
					{
						_callbacks.OnMessage(head->Message);
					}
				}
				finally
				{
					SimulationMessageEnvelope.Free(this, head);
				}
			}
		}
		EngineProfiler.End();
	}

	private unsafe void FreeMessages(ref SimulationMessageList list)
	{
		while (list.Count > 0)
		{
			SimulationMessageEnvelope.Free(this, list.RemoveHead());
		}
		list = default;
	}

	private unsafe int ConsumeAndWriteMessagesIntoBuffer(ref SimulationMessageList inList, NetBitBuffer* buffer, int bitCapacity, ref SimulationMessageList outList, bool allowFirstMessageOverflow = true)
	{
		int num = buffer->OffsetBits + bitCapacity;
		buffer->PadToByteBoundary();
		int offsetBits = buffer->OffsetBits;
		buffer->WriteInt32(0, 16);
		int num2 = 0;
		bool flag = allowFirstMessageOverflow;
		bool isServer = IsServer;
		while (inList.Count > 0)
		{
			SimulationMessageEnvelope* head = inList.Head;
			int offsetBits2 = buffer->OffsetBits;
			PlayerRef? playerRef = null;
			if (isServer && head->Message->IsTargeted())
			{
				playerRef = head->Message->Target;
				head->Message->Target = default;
			}
			try
			{
				int bitCount = SimulationMessageEnvelope.GetBitCount(head, buffer);
				if (buffer->CheckBitCount(bitCount))
				{
					SimulationMessageEnvelope.Write(head, buffer);
					if (buffer->OffsetBits < num)
					{
						goto IL_0126;
					}
					Assert.Check(!buffer->Overflow, "Buffer should not overflow");
					if (flag)
					{
						goto IL_0126;
					}
					buffer->OffsetBits = offsetBits2;
				}
			}
			finally
			{
				if (playerRef.HasValue)
				{
					head->Message->Target = playerRef.Value;
				}
			}
			break;
			IL_0126:
			flag = false;
			num2++;
			SimulationMessageEnvelope* ptr = inList.RemoveHead();
			Assert.Check(ptr == head, "SimulationMessageList Head != Msg Head");
			if (head->Message->GetFlag(8))
			{
				SimulationMessageEnvelope.Free(this, head);
			}
			else
			{
				outList.AddLast(head);
			}
		}
		EngineProfiler.RpcOut(num2);
		buffer->WriteInt32AtOffset(num2, offsetBits, 16);
		buffer->PadToByteBoundary();
		return num2;
	}

	private unsafe void ResolveMessageSourceAndTarget(SimulationMessage* msg, PlayerRef sourcePlayer)
	{
		if (IsServer)
		{
			Assert.Check(msg->Source.IsNone, "Messages arriving to server should not have Source set");
			msg->Source = sourcePlayer;
			if (msg->GetFlag(32))
			{
				Assert.Check(msg->Target.IsNone, "Messages to the server should not have target set");
			}
			else if (msg->GetFlag(16))
			{
				Assert.Check(msg->Target.IsValid, "Messages to a player should have target set");
			}
			else
			{
				Assert.Check(msg->Target.IsNone, "Messages without a target should not have target set");
			}
		}
		else
		{
			Assert.Check(!msg->GetFlag(32), "Got forwared to a client? With server?");
			Assert.Check(msg->Target.IsNone, "If a message reaches a client, it should have it's target set");
			if (msg->GetFlag(16))
			{
				msg->Target = LocalPlayer;
			}
		}
	}

	private unsafe void ReadMessagesFromBuffer(NetBitBuffer* buffer, ulong minSequence, PlayerRef sourcePlayer, ref SimulationMessageList outList)
	{
		buffer->SeekToByteBoundary();
		if (!buffer->CanRead(16))
		{
			return;
		}
		int num = buffer->ReadInt32(16);
		Assert.Check(!buffer->Overflow, "Buffer should not overflow");
		EngineProfiler.RpcIn(num);
		if (num > 0)
		{
		}
		while (--num >= 0)
		{
			SimulationMessageEnvelope* ptr = SimulationMessageEnvelope.Read(this, buffer);
			Assert.Check(!buffer->Overflow, "Buffer should not overflow");
			if (ptr->Message->IsUnreliable)
			{
				ResolveMessageSourceAndTarget(ptr->Message, sourcePlayer);
				outList.AddLast(ptr);
			}
			else if (ptr->Sequence > minSequence)
			{
				ResolveMessageSourceAndTarget(ptr->Message, sourcePlayer);
				bool flag = true;
				if (outList.Count > 0)
				{
					for (SimulationMessageEnvelope* ptr2 = outList.Tail; ptr2 != null; ptr2 = ptr2->Prev)
					{
						if (!ptr2->Message->IsUnreliable)
						{
							flag = ptr2->Sequence < ptr->Sequence;
							break;
						}
					}
				}
				if (flag)
				{
					outList.AddLast(ptr);
					continue;
				}
				SimulationMessageEnvelope* ptr3 = outList.Head;
				while (ptr3 != null && (ptr3->Message->IsUnreliable || ptr3->Sequence < ptr->Sequence))
				{
					ptr3 = ptr3->Next;
				}
				if (ptr3 == null)
				{
					Assert.Fail("Expected to have found a reliable message with at least same sequence number", ptr->Sequence);
				}
				else if (ptr3->Sequence == ptr->Sequence)
				{
					SimulationMessageEnvelope.Free(this, ptr);
				}
				else
				{
					outList.AddBefore(ptr, ptr3);
				}
			}
			else
			{
				SimulationMessageEnvelope.Free(this, ptr);
			}
		}
		buffer->SeekToByteBoundary();
	}

	private SimulationSnapshot FindSnapshot(Tick tick, bool defaultToRoot)
	{
		if (_history.TryGet(tick, out var snapshot))
		{
			return snapshot;
		}
		if (!defaultToRoot)
		{
			throw new InvalidOperationException($"Not Found: {tick}");
		}
		return _history.Root;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe SimulationConnection GetSimulationConnectionByIndex(int index)
	{
		if (index >= 0 && index < _connections.Length)
		{
			SimulationConnection simulationConnection = _connections[index];
			if (simulationConnection.Active && simulationConnection.Connection != null)
			{
				return simulationConnection;
			}
		}
		return null;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe SimulationConnection GetSimulationConnection(NetConnection* c)
	{
		SimulationConnection simulationConnection = _connections[c->LocalConnectionId.GroupIndex];
		if (simulationConnection.ConnectionId == c->LocalConnectionId)
		{
			Assert.Check(simulationConnection.Connection == c, "SimulationConnection.Connection != NetConnection");
			return simulationConnection;
		}
		return null;
	}

	public unsafe void SetActiveScene(SceneRef scene)
	{
		if (IsRunning)
		{
			if (IsServer)
			{
				State.GlobalState->Scene = scene;
			}
			else if (_callbacks.IsSharedModeMasterClient)
			{
				SendInternalSimulationMessage(SimulationMessageInternalTypes.SharedModeSceneLoad, scene);
			}
		}
	}

	internal void AddToGlobalObjectInterest(NetworkId id)
	{
		if (_globalInterestObjects != null && !_globalInterestObjects.AddUnique(id))
		{
			Assert.AlwaysFail($"Already added {id}");
		}
	}

	internal void RemoveFromGlobalObjectInterest(NetworkId id)
	{
		if (_globalInterestObjects != null)
		{
			_globalInterestObjects.RemoveUnique(id);
		}
	}

	public unsafe void SendReliableData(int connection, int key, byte[] data)
	{
		SimulationConnection simulationConnectionByIndex = GetSimulationConnectionByIndex(connection);
		if (simulationConnectionByIndex.Active)
		{
			fixed (byte* data2 = data)
			{
				NetPeerGroup.SendReliable(_netPeerGroup, simulationConnectionByIndex.Connection, key, data2, data.Length);
			}
		}
	}

	internal void NotifyWaitingForShutdown()
	{
		_isWaitingForShutdown = true;
	}

	void ILogBuilder.BuildLogMessage(StringBuilder builder, string message, in LogOptions options)
	{
		if (Callbacks is ILogBuilder logBuilder)
		{
			logBuilder.BuildLogMessage(builder, message, in options);
		}
		else
		{
			builder.Append(message);
		}
	}

	public static IDeltaCompressor GetDefaultDeltaCompressor()
	{
		return new DeltaCompressorDefault();
	}

	public static IDeltaCompressor GetDebugDeltaCompressor()
	{
		return new DeltaCompressorDebug();
	}

	private unsafe void NetworkInit(INetSocket socket, NetAddress address)
	{
		NetConfig config = _projectConfig.Network.ToNetConfig(address);
		config.Simulation = _projectConfig.NetworkConditions.Create();
		config.PacketSize = 8192;
		config.ConnectionGroups = 1;
		if (IsSinglePlayer)
		{
			config.MaxConnections = 0;
		}
		else if (IsClient)
		{
			config.MaxConnections = 1;
		}
		else
		{
			Assert.Check(IsServer);
			if (IsPlayer)
			{
				config.MaxConnections = _config.DefaultPlayers - 1;
			}
			else
			{
				config.MaxConnections = _config.DefaultPlayers;
			}
		}
		_connections = new SimulationConnection[config.MaxConnections];
		for (int i = 0; i < _connections.Length; i++)
		{
			_connections[i] = new SimulationConnection(this);
		}
		_netSocket = socket;
		_netPeer = NetPeer.Initialize(config, _netSocket);
		_netPeerGroup = NetPeer.GetGroup(_netPeer, 0);
		_netPeerRng = new System.Random(Environment.TickCount);
	}

	private unsafe void NetworkSend()
	{
		if (_netPeer != null)
		{
			EngineProfiler.Begin("Simulation.NetworkSend");
			NetPeer.Send(_netPeer, _netSocket);
			EngineProfiler.End();
		}
	}

	private unsafe void NetworkRecv()
	{
		if (_netPeer != null)
		{
			EngineProfiler.Begin("Simulation.NetworkRecv");
			if (_netPeerRng == null)
			{
				_netPeerRng = new System.Random(Environment.TickCount);
			}
			NetPeer.Recv(_netPeer, _netSocket, _netPeerRng);
			NetPeerGroup.Update(_netPeerGroup, this);
			NetworkReceiveDone();
			EngineProfiler.End();
		}
	}

	private unsafe void NetworkShutdown()
	{
		OnNetworkShutdown();
		for (int i = 0; i < _connections.Length; i++)
		{
			_connections[i].Reset();
			_connections[i] = new SimulationConnection(this);
		}
		NetPeer.Destroy(_netPeer, _netSocket, this);
		_netPeer = null;
		_netPeerGroup = null;
		_netSocket = null;
	}

	internal virtual void OnNetworkShutdown()
	{
	}

	private unsafe bool NetworkGetBuffer(NetConnection* connection, out NetBitBuffer* buffer)
	{
		if (_netPeer == null)
		{
			buffer = null;
			return false;
		}
		return NetPeerGroup.GetNotifyDataBuffer(_netPeerGroup, connection, out buffer);
	}

	private unsafe bool NetworkSendBuffer(NetConnection* connection, NetBitBuffer* buffer, SimulationPacketEnvelope* envelope)
	{
		if (_netPeer == null)
		{
			return false;
		}
		bool flag = NetPeerGroup.SendNotifyDataBuffer(_netPeerGroup, connection, buffer, envelope);
		if (!flag)
		{
			Log.DebugError("SendNotifyDataBuffer failed");
		}
		return flag;
	}

	internal unsafe bool NetworkSendPing(NetAddress address, void* data, int length)
	{
		if (_netPeer == null)
		{
			return false;
		}
		return NetPeerGroup.SendUnconnectedData(_netPeerGroup, address, data, length);
	}

	unsafe void INetPeerGroupCallbacks.OnConnectionAttempt(NetConnection* connection, int attempt, int totalConnectionAttempts)
	{
		Assert.Check(IsClient);
		_callbacks.OnInternalConnectionAttempt(attempt, totalConnectionAttempts, out var shouldChange, out var newAddress);
		if (shouldChange)
		{
			NetPeerGroup.ChangeConnectionAddressDuringConnecting(_netPeerGroup, connection, newAddress);
		}
	}

	unsafe void INetPeerGroupCallbacks.OnUnconnectedData(NetBitBuffer* buffer)
	{
	}

	unsafe void INetPeerGroupCallbacks.OnConnected(NetConnection* connection)
	{
		SimulationConnection simulationConnection = (_connections[connection->LocalConnectionId.GroupIndex] = new SimulationConnection(this));
		Assert.Check(simulationConnection.Connection == null);
		Assert.Check(simulationConnection.ConnectionId == default(NetConnectionId));
		simulationConnection.Connection = connection;
		simulationConnection.ConnectionId = connection->LocalConnectionId;
		simulationConnection.Active = true;
		_players[connection->LocalConnectionId.GroupIndex] = new SimulationPlayer();
		NetworkConnected(connection);
		try
		{
			if (IsClient)
			{
				_callbacks.OnConnectedToServer();
			}
		}
		catch (Exception exn)
		{
			Log.Exception(this, exn);
		}
	}

	unsafe void INetPeerGroupCallbacks.OnDisconnected(NetConnection* connection, NetDisconnectReason reason)
	{
		Log.Debug(this, $"Disconnected: Address={connection->Address}, Reason={reason}");
		SimulationConnection simulationConnection = GetSimulationConnection(connection);
		FreeMessages(ref simulationConnection.MessagesIn);
		FreeMessages(ref simulationConnection.MessagesOut);
		NetworkDisconnected(connection);
		_players[connection->LocalConnectionId.GroupIndex] = new SimulationPlayer();
		_connections[connection->LocalConnectionId.GroupIndex] = new SimulationConnection(this);
		_connections[connection->LocalConnectionId.GroupIndex].Active = false;
	}

	unsafe void INetPeerGroupCallbacks.OnReliableData(NetConnection* connection, int key, byte* data, int length)
	{
		SimulationConnection simulationConnection = GetSimulationConnection(connection);
		if (!simulationConnection.Active)
		{
			return;
		}
		if (IsServer)
		{
			if (key != -1 && key != LocalPlayer)
			{
				if ((ProjectConfig.Network.ReliableDataTransferModes & NetworkConfiguration.ReliableDataTransfers.ClientToClientWithServerProxy) == NetworkConfiguration.ReliableDataTransfers.ClientToClientWithServerProxy)
				{
					SimulationConnection simulationConnection2 = _connections[key];
					if (simulationConnection2.Active)
					{
						NetPeerGroup.SendReliable(_netPeerGroup, simulationConnection2.Connection, key, data, length);
					}
				}
				else
				{
					Log.DebugError(this, "Disconnecting client for sending server-proxied reliable data when not allowed");
					NetPeerGroup.Disconnect(_netPeerGroup, connection);
				}
				return;
			}
			if ((ProjectConfig.Network.ReliableDataTransferModes & NetworkConfiguration.ReliableDataTransfers.ClientToServer) != NetworkConfiguration.ReliableDataTransfers.ClientToServer)
			{
				NetPeerGroup.Disconnect(_netPeerGroup, connection);
				Log.DebugError(this, "Disconnecting client for sending reliable data when not allowed");
				return;
			}
		}
		byte[] array = new byte[length];
		fixed (byte* destination = array)
		{
			Native.MemCpy(destination, data, length);
		}
		_callbacks.OnReliableData(connection->LocalId.GroupIndex, array);
	}

	bool INetPeerGroupCallbacks.OnConnectionRequest(NetAddress remoteAddres, byte[] token)
	{
		return _callbacks.OnConnectionRequest(remoteAddres, token);
	}

	void INetPeerGroupCallbacks.OnConnectionFailed(NetAddress address, NetConnectFailedReason reason)
	{
		try
		{
			_callbacks.OnConnectionFailed(address, reason);
		}
		catch (Exception exn)
		{
			Log.Exception(this, exn);
		}
	}

	unsafe void INetPeerGroupCallbacks.OnUnreliableData(NetConnection* connection, NetBitBuffer* buffer)
	{
		Assert.AlwaysFail("Not implemented");
	}

	unsafe void INetPeerGroupCallbacks.OnNotifyData(NetConnection* connection, NetBitBuffer* buffer)
	{
		SimulationConnection simulationConnection = GetSimulationConnection(connection);
		if (buffer->ReadBoolean())
		{
			simulationConnection.PacketReceiveDelta();
			RecvPacket(connection, buffer);
		}
		try
		{
			ReadMessagesFromBuffer(buffer, simulationConnection.MessagesInSequence, connection->LocalConnectionId.GroupIndex, ref simulationConnection.MessagesIn);
		}
		catch (Exception exn)
		{
			Log.Exception(this, exn);
		}
	}

	private unsafe void OnEnvelopeLost(NetConnection* connection, SimulationPacketEnvelope* envelope)
	{
		if (connection->ConnectionStatus == NetConnectionStatus.Connected)
		{
			SimulationConnection simulationConnection = GetSimulationConnection(connection);
			if (envelope->Messages.Count > 0)
			{
				while (envelope->Messages.Count > 0)
				{
					SimulationMessageEnvelope* item = envelope->Messages.RemoveHead();
					simulationConnection.MessagesOut.AddFirst(item);
				}
			}
			_stateReplicator.OnPacketLost(connection, envelope);
		}
		else
		{
			FreeMessages(ref envelope->Messages);
		}
		SimulationPacketEnvelope.Free(this, envelope);
	}

	private unsafe void OnEnvelopeDelivered(NetConnection* connection, SimulationPacketEnvelope* envelope)
	{
		FreeMessages(ref envelope->Messages);
		GetSimulationConnection(connection).GlobalState = envelope->GlobalState;
		_stateReplicator.OnPacketDelivered(connection, envelope);
		SimulationPacketEnvelope.Free(this, envelope);
	}

	unsafe void INetPeerGroupCallbacks.OnNotifyDispose(NetSendEnvelope envelope)
	{
		if (envelope.PacketType == NetPacketType.NotifyReliableData)
		{
			Native.Free(envelope.UserData);
			return;
		}
		FreeMessages(ref ((SimulationPacketEnvelope*)envelope.UserData)->Messages);
		SimulationPacketEnvelope.Free(this, (SimulationPacketEnvelope*)envelope.UserData);
	}

	unsafe void INetPeerGroupCallbacks.OnNotifyLost(NetConnection* connection, NetSendEnvelope envelope)
	{
		switch (envelope.PacketType)
		{
		case NetPacketType.NotifyData:
			OnEnvelopeLost(connection, (SimulationPacketEnvelope*)envelope.UserData);
			break;
		case NetPacketType.NotifyReliableData:
			Native.Free(envelope.UserData);
			break;
		}
	}

	unsafe void INetPeerGroupCallbacks.OnNotifyDelivered(NetConnection* connection, NetSendEnvelope envelope)
	{
		switch (envelope.PacketType)
		{
		case NetPacketType.NotifyData:
			OnEnvelopeDelivered(connection, (SimulationPacketEnvelope*)envelope.UserData);
			break;
		case NetPacketType.NotifyReliableData:
			Native.Free(envelope.UserData);
			break;
		}
	}

	internal unsafe RpcTargetStatus GetRpcTargetStatus(PlayerRef target)
	{
		if (target == LocalPlayer)
		{
			return RpcTargetStatus.Self;
		}
		if (IsServer)
		{
			if (target.IsNone)
			{
				return RpcTargetStatus.Self;
			}
			if (NetPeerGroup.TryGetConnectionByIndex(_netPeerGroup, target, out var connection) && connection->Active && connection->ConnectionStatus == NetConnectionStatus.Connected)
			{
				return RpcTargetStatus.Remote;
			}
			return RpcTargetStatus.Unreachable;
		}
		if (target.IsValid || target.IsNone)
		{
			return RpcTargetStatus.Remote;
		}
		return RpcTargetStatus.Unreachable;
	}

	internal unsafe RpcSendMessageResult SendMessage(SimulationMessage* message, PlayerRefSet* clientsSent = null, PlayerRefSet* clientsCulled = null)
	{
		int num = 0;
		try
		{
			NetworkId messageTargetObjectIdForVerification = GetMessageTargetObjectIdForVerification(message);
			message->Tick = _state.Tick;
			if (IsClient)
			{
				PlayerRef none = PlayerRef.None;
				NetConnection* connectionByIndex = NetPeerGroup.GetConnectionByIndex(_netPeerGroup, 0);
				Assert.Check(_netPeerGroup->ConnectionCount == 1);
				if (!connectionByIndex->Active || connectionByIndex->ConnectionStatus != NetConnectionStatus.Connected)
				{
					Log.Error(string.Format("Failed to send {0} to {1}: connection not active and/or connected", "SimulationMessage", none));
					return RpcSendMessageResult.NotSentTargetClientNotAvailable;
				}
				if (!VerifyMessageTargetObject(connectionByIndex, messageTargetObjectIdForVerification, out var result))
				{
					Log.DebugWarn(message, $"Message not sent to {none}. Reason: {result}");
					return VerifyResultToSendMessageResult(result);
				}
				SendMessageInternal(message, connectionByIndex);
				num = 1;
				return RpcSendMessageResult.SentToServerForForwarding;
			}
			if (message->IsTargeted())
			{
				Assert.Check(message->GetFlag(16));
				Assert.Check(message->Target.IsValid);
				PlayerRef target = message->Target;
				message->Target = default;
				if (!NetPeerGroup.TryGetConnectionByIndex(_netPeerGroup, target, out var connection))
				{
					Log.Error(string.Format("Failed to send {0} to {1}: connection not found", "SimulationMessage", target));
					return RpcSendMessageResult.NotSentTargetClientNotAvailable;
				}
				if (!connection->Active || connection->ConnectionStatus != NetConnectionStatus.Connected)
				{
					Log.Error(string.Format("Failed to send {0} to {1}: connection not active and/or connected", "SimulationMessage", target));
					return RpcSendMessageResult.NotSentTargetClientNotAvailable;
				}
				if (!VerifyMessageTargetObject(connection, messageTargetObjectIdForVerification, out var result2))
				{
					Log.DebugWarn(message, $"Message not sent to {target}. Reason: {result2}");
					if (clientsCulled != null)
					{
						clientsCulled->Set(target);
					}
					return VerifyResultToSendMessageResult(result2);
				}
				SendMessageInternal(message, connection);
				if (clientsSent != null)
				{
					clientsSent->Set(target);
				}
				num = 1;
				return RpcSendMessageResult.SentToTargetClient;
			}
			NetConnectionMap.Iterator iterator = NetPeerGroup.ConnectionIterator(_netPeerGroup);
			bool flag = false;
			while (iterator.Next())
			{
				if (iterator.Current->ConnectionStatus != NetConnectionStatus.Connected)
				{
					continue;
				}
				flag = true;
				PlayerRef bit = iterator.Current->LocalConnectionId.GroupIndex;
				if (!VerifyMessageTargetObject(iterator.Current, messageTargetObjectIdForVerification, out var _))
				{
					if (clientsCulled != null)
					{
						clientsCulled->Set(bit);
					}
					continue;
				}
				SendMessageInternal(message, iterator.Current);
				num++;
				if (clientsSent != null)
				{
					clientsSent->Set(bit);
				}
			}
			return (!flag) ? RpcSendMessageResult.NotSentBroadcastNoActiveConnections : ((num == 0) ? RpcSendMessageResult.NotSentBroadcastNoConfirmedNorInterestedClients : RpcSendMessageResult.SentBroadcast);
		}
		finally
		{
			if (num == 0)
			{
				SimulationMessage.Free(this, message);
			}
		}
		static RpcSendMessageResult VerifyResultToSendMessageResult(TargetObjectVerificationResult status)
		{
			return status switch
			{
				TargetObjectVerificationResult.ObjectNotConfirmed => RpcSendMessageResult.NotSentTargetObjectNotConfirmed, 
				TargetObjectVerificationResult.TargetNotInterestedInObject => RpcSendMessageResult.NotSentTargetObjectNotInPlayerInterest, 
				_ => throw new ArgumentOutOfRangeException("status"), 
			};
		}
	}

	internal unsafe bool ForwardMessage(SimulationMessage* message, PlayerRef target, bool required)
	{
		Assert.Check(IsServer, "Only server can forward messages");
		Assert.Check(message->GetFlag(2), "Only received messages are to be forwarded");
		if (!NetPeerGroup.TryGetConnectionByIndex(_netPeerGroup, target, out var connection))
		{
			if (required)
			{
				Log.DebugError<LogBuilderUtils.CombinedLogBuilder_Value_Ptr<Simulation, SimulationMessage>>(LogBuilderUtils.Combine(this, message), $"Failed to forward to {target}: connection not found");
			}
			return false;
		}
		if (!connection->Active || connection->ConnectionStatus != NetConnectionStatus.Connected)
		{
			if (required)
			{
				Log.DebugError<LogBuilderUtils.CombinedLogBuilder_Value_Ptr<Simulation, SimulationMessage>>(LogBuilderUtils.Combine(this, message), $"Failed to forward to {target}: connection not active and/or connected");
			}
			return false;
		}
		NetworkId messageTargetObjectIdForVerification = GetMessageTargetObjectIdForVerification(message);
		if (!VerifyMessageTargetObject(connection, messageTargetObjectIdForVerification, out var result))
		{
			if (required)
			{
				Log.DebugWarn<LogBuilderUtils.CombinedLogBuilder_Value_Ptr<Simulation, SimulationMessage>>(LogBuilderUtils.Combine(this, message), $"Failed to forward to {target} to {messageTargetObjectIdForVerification}: {result}");
			}
			return false;
		}
		message->Tick = _state.Tick;
		if (message->IsTargeted())
		{
			Assert.Check(message->Target == target, "When forwarding a targeted message, target should match the target player");
			message->Target = PlayerRef.None;
		}
		if (message->Offset == 0)
		{
			Assert.Check(message->Offset == 0 || message->Offset == message->Capacity);
			message->Offset = message->Capacity;
		}
		SendMessageInternal(message, connection);
		return true;
	}

	internal unsafe NetworkId GetMessageTargetObjectIdForVerification(SimulationMessage* message)
	{
		if (message->GetFlag(1) || message->GetFlag(4) || message->GetFlag(64))
		{
			return default;
		}
		if (_config.ReplicationMode == SimulationConfig.StateReplicationModes.DeltaSnapshots)
		{
			return default;
		}
		int size;
		return RpcHeader.Read(SimulationMessage.GetData(message), out size).Object;
	}

	internal unsafe void SendInternalSimulationMessage<T>(SimulationMessageInternalTypes type, T buffer, PlayerRef? target = null) where T : unmanaged
	{
		SendInternalSimulationMessage(type, &buffer, sizeof(T), target);
	}

	internal unsafe void SendInternalSimulationMessage(SimulationMessageInternalTypes type, void* buffer, int bufferLength, PlayerRef? target = null)
	{
		Assert.Check(buffer);
		Assert.Check(bufferLength > 0);
		Assert.Check(type > (SimulationMessageInternalTypes)0);
		SimulationMessage* ptr = SimulationMessage.Allocate(this, 4 + bufferLength);
		ptr->Flags |= 64;
		byte* data = SimulationMessage.GetData(ptr);
		*(SimulationMessageInternalTypes*)data = type;
		Native.MemCpy(data + 4, buffer, bufferLength);
		if (target.HasValue)
		{
			Assert.Check(IsServer);
			ptr->Target = target.Value;
			ptr->Flags |= 16;
		}
		else
		{
			ptr->Target = default;
			if (IsClient)
			{
				ptr->Flags |= 32;
			}
		}
		ptr->Offset = ptr->Capacity;
		SendMessage(ptr, null, null);
	}

	private unsafe bool VerifyMessageTargetObject(NetConnection* netConnection, NetworkId id, out TargetObjectVerificationResult result)
	{
		if (!id.IsValid)
		{
			result = TargetObjectVerificationResult.Ok;
			return true;
		}
		SimulationConnection simulationConnection = GetSimulationConnection(netConnection);
		if (simulationConnection.ObjectData.IsCreateUnconfirmed(id) == true)
		{
			result = TargetObjectVerificationResult.ObjectNotConfirmed;
			return false;
		}
		if (Replicator.UseObjectInterest && IsServer)
		{
			short groupIndex = netConnection->LocalId.GroupIndex;
			if (!Replicator.HasObjectInterest(groupIndex, id))
			{
				result = TargetObjectVerificationResult.TargetNotInterestedInObject;
				return false;
			}
		}
		result = TargetObjectVerificationResult.Ok;
		return true;
	}

	private unsafe void SendMessageInternal(SimulationMessage* message, NetConnection* netConnection)
	{
		SimulationConnection simulationConnection = GetSimulationConnection(netConnection);
		ulong sequence = (message->GetFlag(8) ? 0 : (++simulationConnection.MessagesOutSequence));
		simulationConnection.MessagesOut.AddLast(SimulationMessageEnvelope.Allocate(this, message, sequence));
	}
}
