using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fusion;

[AddComponentMenu("")]
public sealed class RunnerVisibilityNode : MonoBehaviour
{
	public enum PreferredRunners
	{
		InputAuthority = 0,
		Server = 1,
		Client = 2
	}

	private enum ComponentType
	{
		None = 0,
		Renderer = 1,
		Behaviour = 2
	}

	private static readonly Type[] _recognizedBehaviourTypes = new Type[6]
	{
		typeof(IRunnerVisibilityRecognizedType),
		typeof(Renderer),
		typeof(AudioListener),
		typeof(Camera),
		typeof(Canvas),
		typeof(Light)
	};

	private static readonly string[] _recognizedBehaviourNames = new string[1] { "EventSystem" };

	[SerializeField]
	public PreferredRunners PreferredRunner;

	public Component Component;

	[SerializeField]
	[EditorDisabled(false)]
	internal string Guid;

	[SerializeField]
	[HideInInspector]
	internal bool _showAtRuntime;

	private NetworkRunner _runner;

	private ComponentType _componentType;

	private bool _originalState;

	private LinkedListNode<RunnerVisibilityNode> _node;

	internal static readonly Dictionary<string, List<RunnerVisibilityNode>> CommonObjectLookup = new Dictionary<string, List<RunnerVisibilityNode>>();

	public bool DefaultState
	{
		get
		{
			return _originalState;
		}
		set
		{
			_originalState = value;
		}
	}

	internal bool Enabled
	{
		get
		{
			return (_componentType == ComponentType.Renderer) ? (Component as Renderer).enabled : (Component as UnityEngine.Behaviour).enabled;
		}
		set
		{
			if (!(Component == null))
			{
				if (_componentType == ComponentType.Renderer)
				{
					(Component as Renderer).enabled = value;
				}
				else
				{
					(Component as UnityEngine.Behaviour).enabled = value;
				}
			}
		}
	}

	internal static bool IsRecognized(Type type)
	{
		Type[] recognizedBehaviourTypes = _recognizedBehaviourTypes;
		foreach (Type type2 in recognizedBehaviourTypes)
		{
			if (type2.IsAssignableFrom(type))
			{
				return true;
			}
		}
		string text = type.Name;
		string[] recognizedBehaviourNames = _recognizedBehaviourNames;
		foreach (string value in recognizedBehaviourNames)
		{
			if (text.Contains(value))
			{
				return true;
			}
		}
		return false;
	}

	private void Reset()
	{
		_showAtRuntime = true;
		Guid = System.Guid.NewGuid().ToString();
	}

	private bool AssociateComponent(Component component)
	{
		Component = component;
		Type type = component.GetType();
		if (component as Renderer != null)
		{
			_componentType = ComponentType.Renderer;
			return true;
		}
		if (component as UnityEngine.Behaviour != null)
		{
			_componentType = ComponentType.Behaviour;
			return true;
		}
		return false;
	}

	private void OnValidate()
	{
		if (Component != null)
		{
			if (Component.transform != base.transform)
			{
				Debug.LogWarning("RunnerVisibilityNode can only be associated with components on the same GameObject.");
				Component = null;
			}
			else if (!AssociateComponent(Component))
			{
				Debug.LogWarning("RunnerVisibilityNode can only be associated with Components that can be enabled/disabled.");
				Component = null;
			}
		}
	}

	private void Awake()
	{
		if (!_showAtRuntime)
		{
			base.hideFlags = HideFlags.HideInInspector;
		}
	}

	private void OnDestroy()
	{
		UnregisterNode(this);
	}

	private void Initialize(Component comp, NetworkRunner runner, LinkedListNode<RunnerVisibilityNode> node)
	{
		_runner = runner;
		if (comp is Renderer renderer)
		{
			_componentType = ComponentType.Renderer;
			_originalState = renderer.enabled;
			renderer.enabled = runner.IsVisible && _originalState;
			_node = node;
			Component = comp;
		}
		else if (comp is UnityEngine.Behaviour behaviour)
		{
			_componentType = ComponentType.Behaviour;
			_originalState = behaviour.enabled;
			behaviour.enabled = runner.IsVisible && _originalState;
			_node = node;
			Component = comp;
		}
	}

	public void SetEnabled(bool enabled)
	{
		if (enabled)
		{
			if (!_originalState)
			{
				if (!Enabled)
				{
					return;
				}
				_originalState = true;
			}
			Enabled = true;
		}
		else
		{
			Enabled = false;
		}
	}

	public static void AddVisibilityNodes(GameObject go, NetworkRunner runner)
	{
		if ((bool)go.GetComponent<RunnerVisibilityNodeRoot>())
		{
			return;
		}
		go.AddComponent<RunnerVisibilityNodeRoot>();
		bool flag;
		if (runner._visibilityNodes == null)
		{
			runner._visibilityNodes = new LinkedList<RunnerVisibilityNode>();
			flag = true;
		}
		else
		{
			flag = false;
		}
		List<RunnerVisibilityNodes> nestedComponentsInChildren = go.transform.GetNestedComponentsInChildren<RunnerVisibilityNodes, NetworkObject>(null, includeInactive: false);
		foreach (RunnerVisibilityNodes item in nestedComponentsInChildren)
		{
			item.AddNodes();
		}
		List<RunnerVisibilityNode> nestedComponentsInChildren2 = go.transform.GetNestedComponentsInChildren<RunnerVisibilityNode, NetworkObject>(null, includeInactive: false);
		CollectBehavioursAndAddNodes(go, runner, nestedComponentsInChildren2.ToArray());
		if (flag)
		{
			RefreshRunnerVisibility(runner);
		}
	}

	private static void AddNodeToCommonLookup(RunnerVisibilityNode node)
	{
		string guid = node.Guid;
		if (guid != null && !(guid == ""))
		{
			if (!CommonObjectLookup.TryGetValue(guid, out var value))
			{
				value = new List<RunnerVisibilityNode>();
				CommonObjectLookup.Add(guid, value);
			}
			value.Add(node);
		}
	}

	private static void CollectBehavioursAndAddNodes(GameObject go, NetworkRunner runner, RunnerVisibilityNode[] existingNodes)
	{
		if (go == null)
		{
			return;
		}
		bool flag = false;
		List<Component> nestedComponentsInChildren = go.transform.GetNestedComponentsInChildren<Component, NetworkObject>(null);
		foreach (Component item in nestedComponentsInChildren)
		{
			bool flag2 = false;
			if (item == null)
			{
				continue;
			}
			foreach (RunnerVisibilityNode runnerVisibilityNode in existingNodes)
			{
				if (runnerVisibilityNode.Component == item)
				{
					flag2 = true;
					AddNodeToCommonLookup(runnerVisibilityNode);
					RegisterNode(runnerVisibilityNode, runner, item);
					flag = true;
					break;
				}
			}
			if (flag2)
			{
				continue;
			}
			Type type = item.GetType();
			Type[] recognizedBehaviourTypes = _recognizedBehaviourTypes;
			foreach (Type type2 in recognizedBehaviourTypes)
			{
				if (IsRecognized(type))
				{
					RunnerVisibilityNode node = item.gameObject.AddComponent<RunnerVisibilityNode>();
					RegisterNode(node, runner, item);
					break;
				}
			}
		}
		if (flag)
		{
			RefreshCommonObjectVisibilities();
		}
	}

	private static void RegisterNode(RunnerVisibilityNode node, NetworkRunner runner, Component comp)
	{
		if (runner._visibilityNodes.Contains(node))
		{
			Log.Warn("RunnerVisibilityNode on '" + node.name + "' already has been registered.");
		}
		LinkedListNode<RunnerVisibilityNode> node2 = runner._visibilityNodes.AddLast(node);
		node.Initialize(comp, runner, node2);
	}

	private static void UnregisterNode(RunnerVisibilityNode node)
	{
		if (node == null)
		{
			return;
		}
		NetworkRunner runner = node._runner;
		bool flag = BehaviourUtils.IsNotAlive(runner);
		if (!flag && !node._runner._visibilityNodes.Contains(node))
		{
			Log.Warn("RunnerVisibilityNode cannot be unregistered, as it never was initially registered.");
		}
		if (!flag && runner._visibilityNodes.Contains(node))
		{
			runner._visibilityNodes.Remove(node);
		}
		if ((object)node != null && node._node != null && node._node.List != null)
		{
			node._node.List.Remove(node);
		}
		if (node.Guid != null && CommonObjectLookup.TryGetValue(node.Guid, out var value))
		{
			if (value.Contains(node))
			{
				value.Remove(node);
			}
			if (value.Count == 0)
			{
				CommonObjectLookup.Remove(node.Guid);
			}
		}
	}

	public static void RefreshAllRunnerVisibilities()
	{
		List<NetworkRunner>.Enumerator instancesEnumerator = NetworkRunner.GetInstancesEnumerator();
		while (instancesEnumerator.MoveNext())
		{
			NetworkRunner current = instancesEnumerator.Current;
			if (current.IsRunning)
			{
				RefreshRunnerVisibility(current, refreshCommonObjects: false);
			}
		}
		RefreshCommonObjectVisibilities();
	}

	internal static void RefreshRunnerVisibility(NetworkRunner runner, bool refreshCommonObjects = true)
	{
		if (runner._visibilityNodes == null)
		{
			return;
		}
		bool isVisible = runner.IsVisible;
		foreach (RunnerVisibilityNode visibilityNode in runner._visibilityNodes)
		{
			if (!(visibilityNode == null))
			{
				visibilityNode.SetEnabled(isVisible);
			}
		}
		if (refreshCommonObjects)
		{
			RefreshCommonObjectVisibilities();
		}
	}

	internal static void RefreshCommonObjectVisibilities()
	{
		List<NetworkRunner>.Enumerator instancesEnumerator = NetworkRunner.GetInstancesEnumerator();
		NetworkRunner networkRunner = null;
		NetworkRunner networkRunner2 = null;
		NetworkRunner networkRunner3 = null;
		while (instancesEnumerator.MoveNext())
		{
			NetworkRunner current = instancesEnumerator.Current;
			if (current.IsRunning && current.IsVisible && !current.IsShutdown)
			{
				if (current.IsServer)
				{
					networkRunner = current;
				}
				if (BehaviourUtils.IsNotAlive(networkRunner2) && current.IsClient)
				{
					networkRunner2 = current;
				}
				if (BehaviourUtils.IsNotAlive(networkRunner3) && current.ProvideInput)
				{
					networkRunner3 = current;
				}
			}
		}
		if (BehaviourUtils.IsNotAlive(networkRunner))
		{
			networkRunner = (BehaviourUtils.IsAlive(networkRunner3) ? networkRunner3 : networkRunner2);
		}
		if (BehaviourUtils.IsNotAlive(networkRunner2))
		{
			networkRunner2 = (BehaviourUtils.IsAlive(networkRunner) ? networkRunner : networkRunner3);
		}
		if (BehaviourUtils.IsNotAlive(networkRunner3))
		{
			networkRunner3 = (BehaviourUtils.IsAlive(networkRunner) ? networkRunner : networkRunner2);
		}
		foreach (KeyValuePair<string, List<RunnerVisibilityNode>> item in CommonObjectLookup)
		{
			List<RunnerVisibilityNode> value = item.Value;
			if (value.Count <= 0)
			{
				continue;
			}
			NetworkRunner networkRunner4 = value[0].PreferredRunner switch
			{
				PreferredRunners.Server => networkRunner, 
				PreferredRunners.Client => networkRunner2, 
				PreferredRunners.InputAuthority => networkRunner3, 
				_ => null, 
			};
			foreach (RunnerVisibilityNode item2 in value)
			{
				item2.Enabled = (object)item2._runner == networkRunner4;
			}
		}
	}

	internal static void ResetStatics()
	{
		CommonObjectLookup.Clear();
	}
}
