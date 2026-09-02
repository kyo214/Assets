using System;
using System.Linq;
using UnityEngine;

namespace Doozy.Runtime.Nody;

public static class FlowNodeExtensions
{
	public static FlowPort GetInputPortFromId<T>(this T target, string portId) where T : FlowNode
	{
		return target.inputPorts.FirstOrDefault((FlowPort port) => port.portId.Equals(portId));
	}

	public static FlowPort GetOutputPortFromId<T>(this T target, string portId) where T : FlowNode
	{
		return target.outputPorts.FirstOrDefault((FlowPort port) => port.portId.Equals(portId));
	}

	public static FlowPort GetPortFromId<T>(this T target, string portId) where T : FlowNode
	{
		return target.ports.FirstOrDefault((FlowPort port) => port.portId.Equals(portId));
	}

	public static bool IsConnected<T>(this T target) where T : FlowNode
	{
		if (!target.inputPorts.Any((FlowPort p) => p.isConnected))
		{
			return target.outputPorts.Any((FlowPort p) => p.isConnected);
		}
		return true;
	}

	public static bool IsConnectedToPort<T>(this T target, string portId) where T : FlowNode
	{
		if (!target.inputPorts.Any((FlowPort port) => FlowPortExtensions.IsConnectedToPort(port, portId)))
		{
			return target.outputPorts.Any((FlowPort port) => FlowPortExtensions.IsConnectedToPort(port, portId));
		}
		return true;
	}

	public static bool CanDeletePort<T>(this T target, string portId) where T : FlowNode
	{
		FlowPort portFromId = target.GetPortFromId(portId);
		if (portFromId == null)
		{
			return false;
		}
		if (!portFromId.canBeDeleted)
		{
			return false;
		}
		switch (portFromId.direction)
		{
		case PortDirection.Input:
			if (target.inputPorts.Count <= target.minNumberOfInputPorts)
			{
				return false;
			}
			break;
		case PortDirection.Output:
			if (target.outputPorts.Count <= target.minNumberOfOutputPorts)
			{
				return false;
			}
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		return true;
	}

	public static bool DeletePort<T>(this T target, string portId) where T : FlowNode
	{
		if (!target.CanDeletePort(portId))
		{
			return false;
		}
		FlowPort portFromId = target.GetPortFromId(portId);
		switch (portFromId.direction)
		{
		case PortDirection.Input:
			target.inputPorts.Remove(portFromId);
			break;
		case PortDirection.Output:
			target.outputPorts.Remove(portFromId);
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		return true;
	}

	public static T SetFlowGraph<T>(this T target, FlowGraph flowGraph) where T : FlowNode
	{
		target.flowGraph = flowGraph;
		target.flowGraphId = ((flowGraph != null) ? flowGraph.id : string.Empty);
		target.ports.ForEach((FlowPort port) =>
		{
			port.node = target;
		});
		return target;
	}

	public static T SetNodeName<T>(this T target, string nodeName) where T : FlowNode
	{
		target.nodeName = nodeName;
		return target;
	}

	public static T SetNodeDescription<T>(this T target, string nodeDescription) where T : FlowNode
	{
		target.nodeDescription = nodeDescription;
		return target;
	}

	public static T SetPosition<T>(this T target, Vector2 position) where T : FlowNode
	{
		target.position = position;
		return target;
	}

	public static T Ping<T>(this T target, FlowDirection flowDirection) where T : FlowNode
	{
		target.ping?.Invoke(flowDirection);
		return target;
	}

	public static T RefreshNodeEditor<T>(this T target) where T : FlowNode
	{
		target.refreshNodeEditor?.Invoke();
		return target;
	}

	public static T RefreshNodeView<T>(this T target) where T : FlowNode
	{
		target.refreshNodeView?.Invoke();
		return target;
	}
}
