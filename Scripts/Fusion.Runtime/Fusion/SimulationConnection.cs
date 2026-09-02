using System.Collections.Generic;
using Fusion.Sockets;

namespace Fusion;

internal class SimulationConnection
{
	public const int INTEGRATOR_HISTORY_MULT = 10;

	private bool? _active;

	public Tick SharedTick;

	public int PendingSnapshots;

	public ulong MessagesInSequence;

	public ulong MessagesOutSequence;

	public SimulationMessageList MessagesIn;

	public SimulationMessageList MessagesOut;

	internal double LastSend;

	internal bool InvokeJoined;

	internal bool InvokeLeave;

	internal Ema _packetRecvDelta;

	internal TimerDelta _packetRecvDeltaTimer;

	internal SimulationInput.Buffer _inputs;

	internal Ema _inputsOffsetDelta;

	internal Tick _inputsOffsetDeltaMax;

	internal SimulationConnectionObjectData ObjectData;

	internal NetworkObjectPriorityHeap ObjectPriorityHeap;

	internal unsafe NetConnection* Connection;

	internal NetConnectionId ConnectionId;

	internal SimulationGlobalState GlobalState;

	internal HashSet<Tick> PendingTicks;

	internal List<SimulationPlayer.AOIQuery> AreaOfInterestQueries;

	public bool Active
	{
		get
		{
			return _active == true;
		}
		set
		{
			if (!_active.HasValue || _active.Value != value)
			{
				_active = value;
				if (value)
				{
					InvokeJoined = true;
				}
				else
				{
					InvokeLeave = true;
				}
			}
		}
	}

	internal SimulationConnection(Simulation simulation)
	{
		_inputs = new SimulationInput.Buffer(simulation.ProjectConfig);
		_inputsOffsetDelta = default;
		_inputsOffsetDeltaMax = default;
		_packetRecvDelta = default;
		_packetRecvDeltaTimer = default;
		ObjectData = new SimulationConnectionObjectData();
		ObjectPriorityHeap = new NetworkObjectPriorityHeap();
		AreaOfInterestQueries = new List<SimulationPlayer.AOIQuery>();
		PendingTicks = new HashSet<Tick>(new Tick.EqualityComparer());
	}

	public unsafe void Reset()
	{
		Active = false;
		GlobalState = default;
		SharedTick = default;
		PendingSnapshots = 0;
		MessagesInSequence = 0uL;
		MessagesOutSequence = 0uL;
		MessagesIn = default;
		MessagesOut = default;
		LastSend = 0.0;
		Connection = default;
		ConnectionId = default;
		ObjectData.Clear();
		ObjectPriorityHeap.Clear();
		PendingTicks.Clear();
		_inputs.Clear();
		_inputsOffsetDelta = default;
		_inputsOffsetDeltaMax = default;
		_packetRecvDelta = default;
		_packetRecvDeltaTimer = default;
		AreaOfInterestQueries.Clear();
	}

	public void PacketReceiveDelta()
	{
		if (!_packetRecvDeltaTimer.IsRunning)
		{
			_packetRecvDeltaTimer = TimerDelta.StartNew();
		}
		else if (!(_packetRecvDeltaTimer.Peek() < 0.01))
		{
			_packetRecvDelta.Add(_packetRecvDeltaTimer.Consume());
		}
	}

	public void InputReceiveDelta(Tick tick, double receive, double expected)
	{
		if (!(tick <= _inputsOffsetDeltaMax))
		{
			double num = expected - receive;
			if (num < 0.0)
			{
			}
			_inputsOffsetDeltaMax = tick;
			_inputsOffsetDelta.Add(expected - receive);
		}
	}
}
