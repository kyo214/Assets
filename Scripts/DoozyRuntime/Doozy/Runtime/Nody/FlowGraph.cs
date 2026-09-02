using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Common.ScriptableObjects;
using Doozy.Runtime.Common.Utils;
using Doozy.Runtime.Nody.Nodes.System;
using Doozy.Runtime.UIManager.ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.Nody;

[CreateAssetMenu(menuName = "Doozy/Flow Graph", fileName = "Flow Graph", order = -1000)]
public class FlowGraph : ScriptableObject
{
	private struct GraphHistory
	{
		public FlowNode previousActiveNode { get; set; }

		public FlowPort previousActivePort { get; set; }

		public FlowNode activeNode { get; set; }

		public GraphHistory(FlowNode previousActiveNode, FlowPort previousActivePort, FlowNode activeNode)
		{
			this.previousActiveNode = previousActiveNode;
			this.previousActivePort = previousActivePort;
			this.activeNode = activeNode;
		}
	}

	[SerializeField]
	public Vector3 EditorPosition = Vector3.zero;

	[SerializeField]
	public Vector3 EditorScale = Vector3.one;

	[SerializeField]
	private string Id;

	[SerializeField]
	private string GraphName;

	[SerializeField]
	private string GraphDescription;

	[SerializeField]
	private bool IsSubGraph;

	[SerializeField]
	protected GraphState GraphState;

	public GraphStateEvent OnStateChanged = new GraphStateEvent();

	public UnityEvent OnStart = new UnityEvent();

	public UnityEvent OnStop = new UnityEvent();

	public UnityEvent OnPause = new UnityEvent();

	public UnityEvent OnResume = new UnityEvent();

	public UnityEvent OnBackFlow = new UnityEvent();

	[SerializeField]
	private List<FlowNode> Nodes;

	[SerializeField]
	private FlowNode RootNode;

	[SerializeField]
	private FlowNode ActiveNode;

	public static UIManagerInputSettings inputSettings => SingletonRuntimeScriptableObject<UIManagerInputSettings>.instance;

	public static bool multiplayerMode => inputSettings.multiplayerMode;

	public static int defaultPlayerIndex => inputSettings.defaultPlayerIndex;

	internal Vector3 editorPosition
	{
		get
		{
			return EditorPosition;
		}
		set
		{
			EditorPosition = value;
		}
	}

	internal Vector3 editorScale
	{
		get
		{
			return EditorScale;
		}
		set
		{
			EditorScale = value;
		}
	}

	public string id
	{
		get
		{
			return Id;
		}
		set
		{
			Id = value;
		}
	}

	public string graphName
	{
		get
		{
			return GraphName;
		}
		set
		{
			GraphName = value;
		}
	}

	public string graphDescription
	{
		get
		{
			return GraphDescription;
		}
		set
		{
			GraphDescription = value;
		}
	}

	public bool isSubGraph
	{
		get
		{
			return IsSubGraph;
		}
		set
		{
			IsSubGraph = value;
		}
	}

	public GraphState graphState
	{
		get
		{
			return GraphState;
		}
		set
		{
			GraphState = value;
			OnStateChanged?.Invoke(value);
		}
	}

	public List<FlowNode> nodes
	{
		get
		{
			return Nodes;
		}
		private set
		{
			Nodes = value;
		}
	}

	public IEnumerable<FlowNode> globalNodes => Nodes.Where((FlowNode node) => node.nodeType == NodeType.Global);

	public FlowNode rootNode
	{
		get
		{
			return RootNode;
		}
		set
		{
			RootNode = value;
		}
	}

	public FlowNode activeNode
	{
		get
		{
			return ActiveNode;
		}
		private set
		{
			ActiveNode = value;
		}
	}

	public FlowNode previousActiveNode { get; private set; }

	public FlowPort previousActivePort { get; private set; }

	public FlowGraph activeSubGraph { get; private set; }

	public FlowGraph parentGraph { get; private set; }

	public List<FlowPort> inputPorts
	{
		get
		{
			List<FlowPort> list = new List<FlowPort>();
			foreach (FlowNode node in Nodes)
			{
				list.AddRange(node.inputPorts);
			}
			return list;
		}
	}

	public List<FlowPort> outputPorts
	{
		get
		{
			List<FlowPort> list = new List<FlowPort>();
			foreach (FlowNode node in Nodes)
			{
				list.AddRange(node.outputPorts);
			}
			return list;
		}
	}

	public List<FlowPort> ports
	{
		get
		{
			List<FlowPort> list = new List<FlowPort>();
			foreach (FlowNode node in Nodes)
			{
				list.AddRange(node.inputPorts);
				list.AddRange(node.outputPorts);
			}
			return list;
		}
	}

	public FlowController controller { get; internal set; }

	private Stack<GraphHistory> history { get; set; }

	private HashSet<FlowNode> tempNodesSet { get; set; }

	private HashSet<FlowPort> tempPortsSet { get; set; }

	public FlowGraph()
	{
		Id = Guid.NewGuid().ToString();
		GraphName = ObjectNames.NicifyVariableName("FlowGraph");
		Nodes = new List<FlowNode>();
	}

	internal void ResetEditorSettings()
	{
		EditorPosition = Vector3.zero;
		EditorScale = Vector3.one;
	}

	public void ResetGraph()
	{
		ClearHistory();
		previousActiveNode = null;
		activeNode = null;
		nodes.ForEach((FlowNode n) =>
		{
			n.ResetNode();
		});
		CleanGraph();
		graphState = GraphState.Idle;
	}

	private void CleanGraph()
	{
		nodes.ForEach((FlowNode n) =>
		{
			foreach (FlowPort inputPort in n.inputPorts)
			{
				foreach (string item in from otherPortId in inputPort.connections.ToList()
					where GetPortById(otherPortId) == null
					select otherPortId)
				{
					inputPort.connections.Remove(item);
				}
			}
			foreach (FlowPort outputPort in n.outputPorts)
			{
				foreach (string item2 in from otherPortId in outputPort.connections.ToList()
					where GetPortById(otherPortId) == null
					select otherPortId)
				{
					outputPort.connections.Remove(item2);
				}
			}
		});
	}

	public void SetActiveNode(FlowNode node, FlowPort fromPort = null)
	{
		if (!(node == null))
		{
			if (activeNode != null)
			{
				activeNode.OnExit();
			}
			history.Push(new GraphHistory(previousActiveNode, previousActivePort, activeNode));
			previousActiveNode = activeNode;
			previousActivePort = fromPort;
			activeNode = node;
			activeNode.OnEnter();
			FlowNodeExtensions.Ping(activeNode, FlowDirection.Forward);
			if (fromPort != null)
			{
				FlowPortExtensions.Ping(fromPort, FlowDirection.Forward);
			}
		}
	}

	public void GoBack()
	{
		GoBack(defaultPlayerIndex);
	}

	public void GoBack(int playerIndex)
	{
		if (history.Count == 0)
		{
			return;
		}
		FlowNode flowNode = previousActiveNode;
		if (flowNode is StartNode || flowNode is EnterNode || (multiplayerMode && playerIndex != defaultPlayerIndex && controller.hasMultiplayerInfo && playerIndex != controller.multiplayerInfo.playerIndex))
		{
			return;
		}
		tempNodesSet.Clear();
		tempPortsSet.Clear();
		tempPortsSet.Add(previousActivePort);
		if (history.All((GraphHistory item) => item.activeNode.passthrough))
		{
			return;
		}
		GraphHistory graphHistory = history.Peek();
		if ((graphHistory.activeNode == activeNode) | graphHistory.activeNode.passthrough)
		{
			while (history.Count > 0)
			{
				graphHistory = history.Peek();
				if (graphHistory.activeNode == activeNode)
				{
					tempNodesSet.Clear();
					tempPortsSet.Clear();
					history.Pop();
					continue;
				}
				if (!graphHistory.activeNode.passthrough)
				{
					break;
				}
				tempNodesSet.Add(graphHistory.activeNode);
				tempPortsSet.Add(graphHistory.previousActivePort);
				history.Pop();
			}
		}
		if (history.Count == 0)
		{
			return;
		}
		if (activeNode != null)
		{
			activeNode.OnExit();
		}
		tempNodesSet.Remove(null);
		foreach (FlowNode item in tempNodesSet)
		{
			FlowNodeExtensions.Ping(item, FlowDirection.Back);
		}
		tempPortsSet.Remove(null);
		foreach (FlowPort item2 in tempPortsSet)
		{
			FlowPortExtensions.Ping(item2, FlowDirection.Back);
		}
		previousActiveNode = history.Peek().previousActiveNode;
		previousActivePort = history.Peek().previousActivePort;
		activeNode = history.Peek().activeNode;
		history.Pop();
		activeNode.OnEnter();
		tempNodesSet.Clear();
		tempPortsSet.Clear();
		OnBackFlow?.Invoke();
	}

	public void SetActiveNodeByNodeName(string nodeName)
	{
		SetActiveNode(GetNodeByName(nodeName));
	}

	public void SetActiveNodeByNodeId(string nodeId)
	{
		SetActiveNode(GetNodeById(nodeId));
	}

	public void Restart()
	{
	}

	public void Start()
	{
		if (graphState == GraphState.Paused)
		{
			Resume();
			return;
		}
		ResetGraph();
		UpdateNodes();
		StartGlobalNodes();
		SetActiveNode(RootNode);
		graphState = GraphState.Running;
		OnStart?.Invoke();
	}

	public void Resume()
	{
		if (graphState == GraphState.Idle)
		{
			Start();
			return;
		}
		graphState = GraphState.Running;
		StartGlobalNodes();
		if (activeNode != null)
		{
			activeNode.OnEnter();
		}
		OnResume?.Invoke();
	}

	public void Pause()
	{
		if (graphState == GraphState.Running)
		{
			graphState = GraphState.Paused;
			StopGlobalNodes();
			if (activeNode != null)
			{
				activeNode.OnExit();
			}
			OnPause?.Invoke();
		}
	}

	public void SetPaused(bool paused)
	{
		if (paused)
		{
			Pause();
		}
		else
		{
			Resume();
		}
	}

	public void Stop()
	{
		StopGlobalNodes();
		if (activeNode != null)
		{
			activeNode.OnExit();
		}
		activeNode = null;
		graphState = GraphState.Idle;
		OnStop?.Invoke();
	}

	public void StartGlobalNodes()
	{
		for (int i = 0; i < nodes.Count; i++)
		{
			FlowNode flowNode = nodes[i];
			if (flowNode.nodeType == NodeType.Global)
			{
				flowNode.Start();
			}
		}
		if (activeSubGraph != null)
		{
			activeSubGraph.StartGlobalNodes();
		}
	}

	public virtual void StopGlobalNodes()
	{
		for (int i = 0; i < nodes.Count; i++)
		{
			FlowNode flowNode = nodes[i];
			if (flowNode.nodeType == NodeType.Global)
			{
				flowNode.Stop();
			}
		}
		if (activeSubGraph != null)
		{
			activeSubGraph.StopGlobalNodes();
		}
	}

	public void FixedUpdate()
	{
		if (graphState != GraphState.Running)
		{
			return;
		}
		if (activeNode != null && activeNode.runFixedUpdate)
		{
			activeNode.FixedUpdate();
		}
		if (activeSubGraph != null)
		{
			activeSubGraph.FixedUpdate();
		}
		for (int i = 0; i < nodes.Count; i++)
		{
			FlowNode flowNode = nodes[i];
			if (flowNode.nodeType == NodeType.Global && flowNode.runFixedUpdate)
			{
				flowNode.FixedUpdate();
			}
		}
	}

	public void LateUpdate()
	{
		if (graphState != GraphState.Running)
		{
			return;
		}
		if (activeNode != null && activeNode.runLateUpdate)
		{
			activeNode.LateUpdate();
		}
		if (activeSubGraph != null)
		{
			activeSubGraph.LateUpdate();
		}
		for (int i = 0; i < nodes.Count; i++)
		{
			FlowNode flowNode = nodes[i];
			if (flowNode.nodeType == NodeType.Global && flowNode.runLateUpdate && flowNode.nodeState != NodeState.Idle)
			{
				flowNode.LateUpdate();
			}
		}
	}

	public void Update()
	{
		if (graphState != GraphState.Running)
		{
			return;
		}
		if (activeNode != null && activeNode.runUpdate)
		{
			activeNode.Update();
		}
		if (activeSubGraph != null)
		{
			activeSubGraph.Update();
		}
		for (int i = 0; i < nodes.Count; i++)
		{
			FlowNode flowNode = nodes[i];
			if (flowNode.nodeType == NodeType.Global && flowNode.runUpdate && flowNode.nodeState != NodeState.Idle)
			{
				flowNode.Update();
			}
		}
	}

	public void UpdateNodes()
	{
		Nodes.RemoveNulls();
		for (int i = 0; i < nodes.Count; i++)
		{
			nodes[i].SetFlowGraph(this);
		}
	}

	public FlowGraph Clone()
	{
		FlowGraph flowGraph = UnityEngine.Object.Instantiate(this);
		flowGraph.RootNode = RootNode.Clone().SetFlowGraph(flowGraph);
		flowGraph.nodes = nodes.ConvertAll((FlowNode n) => n.Clone());
		flowGraph.UpdateNodes();
		return flowGraph;
	}

	public bool ContainsNode(FlowNode node)
	{
		if (node != null)
		{
			return nodes.Contains(node);
		}
		return false;
	}

	public bool ContainsNodeById(string nodeId)
	{
		return nodes.Any((FlowNode node) => node.nodeId.Equals(nodeId));
	}

	public bool ContainsNodeByName(string nodeName)
	{
		return nodes.Any((FlowNode node) => node.nodeName.Equals(nodeName));
	}

	public StartNode GetStartNode()
	{
		return (StartNode)nodes.FirstOrDefault((FlowNode n) => n is StartNode);
	}

	public EnterNode GetEnterNode()
	{
		return (EnterNode)nodes.FirstOrDefault((FlowNode n) => n is EnterNode);
	}

	public ExitNode GetExitNode()
	{
		return (ExitNode)nodes.FirstOrDefault((FlowNode n) => n is ExitNode);
	}

	public FlowNode GetNodeByName(string nodeName)
	{
		return nodes.FirstOrDefault((FlowNode node) => node.nodeName.Equals(nodeName));
	}

	public FlowNode GetNodeById(string nodeId)
	{
		return nodes.FirstOrDefault((FlowNode node) => node.nodeId.Equals(nodeId));
	}

	public List<T> GetNodeByType<T>() where T : FlowNode
	{
		return (List<T>)nodes.Where((FlowNode node) => node is T);
	}

	public FlowPort GetPortById(string portId)
	{
		return nodes.Select((FlowNode node) => node.GetPortFromId(portId)).FirstOrDefault((FlowPort port) => port != null);
	}

	public void ClearHistory()
	{
		if (history == null)
		{
			Stack<GraphHistory> stack = (history = new Stack<GraphHistory>());
		}
		if (tempNodesSet == null)
		{
			HashSet<FlowNode> hashSet = (tempNodesSet = new HashSet<FlowNode>());
		}
		if (tempPortsSet == null)
		{
			HashSet<FlowPort> hashSet3 = (tempPortsSet = new HashSet<FlowPort>());
		}
		history.Clear();
	}
}
