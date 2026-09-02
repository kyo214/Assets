using System;
using System.Linq;

namespace Doozy.Runtime.Nody;

public static class FlowPortExtensions
{
	public static T RemoveConnection<T>(this T target, string portId) where T : FlowPort
	{
		if (target.connections.Contains(portId))
		{
			target.connections.Remove(portId);
		}
		return target;
	}

	public static bool IsConnectedToPort<T>(this T target, string otherPortId) where T : FlowPort
	{
		return target.direction switch
		{
			PortDirection.Input => target.connections.Any((string c) => c.Equals(otherPortId)), 
			PortDirection.Output => target.connections.Any((string c) => c.Equals(otherPortId)), 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}

	public static T SetNodeId<T>(this T target, string nodeId) where T : FlowPort
	{
		target.nodeId = nodeId;
		return target;
	}

	public static T SetDirection<T>(this T target, PortDirection direction) where T : FlowPort
	{
		target.direction = direction;
		return target;
	}

	public static T SetCapacity<T>(this T target, PortCapacity capacity) where T : FlowPort
	{
		target.capacity = capacity;
		return target;
	}

	public static T SetCanBeDeleted<T>(this T target, bool canBeDeleted) where T : FlowPort
	{
		target.canBeDeleted = canBeDeleted;
		return target;
	}

	public static T SetCanBeReordered<T>(this T target, bool canBeReordered) where T : FlowPort
	{
		target.canBeReordered = canBeReordered;
		return target;
	}

	public static T Ping<T>(this T target, FlowDirection flowDirection) where T : FlowPort
	{
		target.ping?.Invoke(flowDirection);
		return target;
	}

	public static T RefreshPortEditor<T>(this T target) where T : FlowPort
	{
		target.refreshPortEditor?.Invoke();
		return target;
	}

	public static T RefreshPortView<T>(this T target) where T : FlowPort
	{
		target.refreshPortView?.Invoke();
		return target;
	}
}
