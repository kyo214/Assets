using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Common.ScriptableObjects;
using Doozy.Runtime.Common.Utils;
using Doozy.Runtime.UIManager.ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.Nody;

[Serializable]
public abstract class FlowNode : ScriptableObject
{
	[SerializeField]
	private string FlowGraphId;

	[SerializeField]
	private string NodeId;

	[SerializeField]
	private NodeType NodeType;

	[SerializeField]
	private string NodeName;

	[SerializeField]
	private string NodeDescription;

	[SerializeField]
	private List<FlowPort> InputPorts;

	[SerializeField]
	private List<FlowPort> OutputPorts;

	[SerializeField]
	private bool RunUpdate;

	[SerializeField]
	private bool RunFixedUpdate;

	[SerializeField]
	private bool RunLateUpdate;

	[SerializeField]
	protected NodeState NodeState;

	[SerializeField]
	private bool CanBeDeleted;

	[SerializeField]
	private bool Passthrough;

	[SerializeField]
	private bool ClearGraphHistory;

	[SerializeField]
	private Vector2 Position = Vector2.zero;

	public bool multiplayerMode => SingletonRuntimeScriptableObject<UIManagerInputSettings>.instance.multiplayerMode & flowGraph.controller.hasMultiplayerInfo;

	public int playerIndex => flowGraph.controller.playerIndex;

	public string flowGraphId
	{
		get
		{
			return FlowGraphId;
		}
		internal set
		{
			FlowGraphId = value;
		}
	}

	public FlowGraph flowGraph { get; internal set; }

	public string nodeId
	{
		get
		{
			return NodeId;
		}
		internal set
		{
			NodeId = value;
		}
	}

	public NodeType nodeType => NodeType;

	public string nodeName
	{
		get
		{
			return NodeName;
		}
		internal set
		{
			NodeName = value;
		}
	}

	public string nodeDescription
	{
		get
		{
			return NodeDescription;
		}
		internal set
		{
			NodeDescription = value;
		}
	}

	public List<FlowPort> inputPorts
	{
		get
		{
			return InputPorts;
		}
		internal set
		{
			InputPorts = value;
		}
	}

	public FlowPort firstInputPort => inputPorts.FirstOrDefault();

	public FlowPort lastInputPort => inputPorts.Last();

	public List<string> inputConnections
	{
		get
		{
			List<string> list = new List<string>();
			foreach (FlowPort inputPort in inputPorts)
			{
				list.AddRange(inputPort.connections);
			}
			return list;
		}
	}

	public List<FlowPort> outputPorts
	{
		get
		{
			return OutputPorts;
		}
		internal set
		{
			OutputPorts = value;
		}
	}

	public FlowPort firstOutputPort => outputPorts.FirstOrDefault();

	public FlowPort lastOutputPort => outputPorts.Last();

	public List<string> outputConnections
	{
		get
		{
			List<string> list = new List<string>();
			foreach (FlowPort outputPort in outputPorts)
			{
				list.AddRange(outputPort.connections);
			}
			return list;
		}
	}

	public List<FlowPort> ports
	{
		get
		{
			List<FlowPort> list = new List<FlowPort>();
			list.AddRange(inputPorts);
			list.AddRange(outputPorts);
			return list;
		}
	}

	public List<string> connections
	{
		get
		{
			List<string> list = new List<string>();
			list.AddRange(inputConnections);
			list.AddRange(outputConnections);
			return list;
		}
	}

	public virtual int minNumberOfInputPorts => 0;

	public virtual int minNumberOfOutputPorts => 0;

	public bool runUpdate
	{
		get
		{
			return RunUpdate;
		}
		set
		{
			RunUpdate = value;
		}
	}

	public bool runFixedUpdate
	{
		get
		{
			return RunFixedUpdate;
		}
		set
		{
			RunFixedUpdate = value;
		}
	}

	public bool runLateUpdate
	{
		get
		{
			return RunLateUpdate;
		}
		set
		{
			RunLateUpdate = value;
		}
	}

	public NodeState nodeState
	{
		get
		{
			return NodeState;
		}
		set
		{
			NodeState = value;
			onStateChanged?.Invoke(value);
		}
	}

	public UnityAction<NodeState> onStateChanged { get; set; }

	public bool canBeDeleted
	{
		get
		{
			return CanBeDeleted;
		}
		internal set
		{
			CanBeDeleted = value;
		}
	}

	public virtual bool showPassthroughInEditor => false;

	public bool passthrough
	{
		get
		{
			return Passthrough;
		}
		set
		{
			Passthrough = value;
		}
	}

	public virtual bool showClearGraphHistoryInEditor => false;

	public bool clearGraphHistory
	{
		get
		{
			return ClearGraphHistory;
		}
		set
		{
			ClearGraphHistory = value;
		}
	}

	public Vector2 position
	{
		get
		{
			return Position;
		}
		internal set
		{
			Position = value;
		}
	}

	public UnityAction<FlowDirection> ping { get; set; }

	public UnityAction refreshNodeEditor { get; set; }

	public UnityAction refreshNodeView { get; set; }

	public UnityAction onEnter { get; set; }

	public UnityAction onExit { get; set; }

	public UnityAction onStart { get; set; }

	public UnityAction onStop { get; set; }

	protected FlowNode(NodeType type)
	{
		FlowGraphId = string.Empty;
		NodeId = Guid.NewGuid().ToString();
		NodeType = type;
		NodeName = ObjectNames.NicifyVariableName(GetType().Name.Replace("Node", ""));
		NodeDescription = string.Empty;
		InputPorts = new List<FlowPort>();
		OutputPorts = new List<FlowPort>();
		RunUpdate = false;
		RunFixedUpdate = false;
		RunLateUpdate = false;
		CanBeDeleted = true;
		Passthrough = true;
		ClearGraphHistory = false;
	}

	public virtual void Start()
	{
		nodeState = NodeState.Running;
		onStart?.Invoke();
	}

	public virtual void Stop()
	{
		nodeState = NodeState.Idle;
		onStop?.Invoke();
	}

	public virtual void OnEnter(FlowNode previousNode = null, FlowPort previousPort = null)
	{
		nodeState = NodeState.Active;
		onEnter?.Invoke();
		if (clearGraphHistory)
		{
			flowGraph.ClearHistory();
		}
	}

	public virtual void OnExit()
	{
		nodeState = NodeState.Idle;
		onExit?.Invoke();
	}

	public virtual void Update()
	{
	}

	public virtual void FixedUpdate()
	{
	}

	public virtual void LateUpdate()
	{
	}

	public virtual FlowNode Clone()
	{
		return UnityEngine.Object.Instantiate(this);
	}

	protected virtual void GoToNextNode(FlowPort outputPort)
	{
		if (!(flowGraph == null))
		{
			FlowPort portById = flowGraph.GetPortById(outputPort.firstConnection);
			if (portById != null && !(portById.node == null))
			{
				flowGraph.SetActiveNode(portById.node, outputPort);
			}
		}
	}

	public virtual void ResetNode()
	{
		nodeState = NodeState.Idle;
	}

	public virtual FlowPort AddPort(PortDirection direction, PortCapacity capacity)
	{
		FlowPort flowPort = new FlowPort().SetNodeId(nodeId).SetDirection(direction).SetCapacity(capacity);
		switch (direction)
		{
		case PortDirection.Input:
			inputPorts.Add(flowPort);
			break;
		case PortDirection.Output:
			outputPorts.Add(flowPort);
			break;
		default:
			throw new ArgumentOutOfRangeException("direction", direction, null);
		}
		return flowPort;
	}

	public virtual FlowPort AddInputPort(PortCapacity capacity = PortCapacity.Multi)
	{
		return AddPort(PortDirection.Input, capacity);
	}

	public virtual FlowPort AddOutputPort(PortCapacity capacity = PortCapacity.Single)
	{
		return AddPort(PortDirection.Output, capacity);
	}
}
