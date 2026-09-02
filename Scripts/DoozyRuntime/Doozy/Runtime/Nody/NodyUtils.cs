using System;
using System.Linq;

namespace Doozy.Runtime.Nody;

public static class NodyUtils
{
	public static bool IsPortConnectedToNode(FlowPort port, FlowNode node)
	{
		if (port == null)
		{
			return false;
		}
		if (node == null)
		{
			return false;
		}
		return (port.direction switch
		{
			PortDirection.Input => node.outputConnections, 
			PortDirection.Output => node.inputConnections, 
			_ => throw new ArgumentOutOfRangeException(), 
		}).Contains(port.portId);
	}

	public static bool IsNodeConnectedToNode(FlowNode n1, FlowNode n2)
	{
		if (n1 == null)
		{
			return false;
		}
		if (n2 == null)
		{
			return false;
		}
		if (!n1.ports.Any((FlowPort port) => IsPortConnectedToNode(port, n2)))
		{
			return n2.ports.Any((FlowPort port) => IsPortConnectedToNode(port, n1));
		}
		return true;
	}

	public static bool CanConnect(FlowPort p1, FlowPort p2)
	{
		if (p1 == null)
		{
			return false;
		}
		if (p2 == null)
		{
			return false;
		}
		if (p1 == p2)
		{
			return false;
		}
		if (FlowPortExtensions.IsConnectedToPort(p1, p2.portId))
		{
			return false;
		}
		if (p1.direction == p2.direction)
		{
			return false;
		}
		if (p1.portId == p2.portId)
		{
			return false;
		}
		if (p1.nodeId == p2.nodeId)
		{
			return false;
		}
		return true;
	}

	public static bool DisconnectPort(FlowPort port, FlowGraph graph)
	{
		if (port == null)
		{
			return false;
		}
		if (graph == null)
		{
			return false;
		}
		(port.direction switch
		{
			PortDirection.Input => graph.outputPorts, 
			PortDirection.Output => graph.inputPorts, 
			_ => throw new ArgumentOutOfRangeException(), 
		}).ForEach((FlowPort p) =>
		{
			p.RemoveConnection(port.portId);
			FlowPort portById = graph.GetPortById(port.portId);
			p.onDisconnected?.Invoke(portById);
			portById.onDisconnected?.Invoke(p);
		});
		port.connections.Clear();
		return true;
	}

	public static bool DisconnectPortFromPort(FlowPort p1, FlowPort p2)
	{
		if (p1 == null)
		{
			return false;
		}
		if (p2 == null)
		{
			return false;
		}
		if (!FlowPortExtensions.IsConnectedToPort(p1, p2.portId))
		{
			return false;
		}
		p1.RemoveConnection(p2.portId);
		p1.onDisconnected?.Invoke(p2);
		p2.RemoveConnection(p1.portId);
		p2.onDisconnected?.Invoke(p1);
		return true;
	}

	public static bool DisconnectPortFromNode(FlowPort port, FlowNode node)
	{
		if (port == null)
		{
			return false;
		}
		if (node == null)
		{
			return false;
		}
		(port.direction switch
		{
			PortDirection.Input => node.outputPorts, 
			PortDirection.Output => node.inputPorts, 
			_ => throw new ArgumentOutOfRangeException(), 
		}).ForEach((FlowPort nodePort) =>
		{
			nodePort.RemoveConnection(port.portId);
			nodePort.onDisconnected?.Invoke(port);
			port.RemoveConnection(nodePort.portId);
			port.onDisconnected?.Invoke(nodePort);
		});
		return true;
	}

	public static bool DisconnectNode(FlowNode node, FlowGraph graph)
	{
		if (node == null)
		{
			return false;
		}
		if (graph == null)
		{
			return false;
		}
		node.ports.ForEach((FlowPort port) =>
		{
			DisconnectPort(port, graph);
		});
		return true;
	}

	public static bool DisconnectNodeFromNode(FlowNode n1, FlowNode n2)
	{
		if (n1 == null)
		{
			return false;
		}
		if (n2 == null)
		{
			return false;
		}
		n1.ports.ForEach((FlowPort port) =>
		{
			DisconnectPortFromNode(port, n2);
		});
		n2.ports.ForEach((FlowPort port) =>
		{
			DisconnectPortFromNode(port, n1);
		});
		return true;
	}
}
