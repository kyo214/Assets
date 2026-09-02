using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.Nody;

[Serializable]
public class FlowPort
{
	[SerializeField]
	private string NodeId;

	[SerializeField]
	private string PortId;

	[SerializeField]
	private PortDirection Direction;

	[SerializeField]
	private PortCapacity Capacity;

	[SerializeField]
	private List<string> Connections;

	[SerializeField]
	private string Value;

	[SerializeField]
	private Type m_ValueType;

	[SerializeField]
	private string ValueTypeQualifiedName;

	[SerializeField]
	private bool CanBeDeleted;

	[SerializeField]
	private bool CanBeReordered;

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

	public string portId => PortId;

	public PortDirection direction
	{
		get
		{
			return Direction;
		}
		internal set
		{
			Direction = value;
		}
	}

	public PortCapacity capacity
	{
		get
		{
			return Capacity;
		}
		internal set
		{
			Capacity = value;
		}
	}

	public List<string> connections => Connections;

	public string firstConnection => connections.FirstOrDefault();

	public bool isConnected => Connections.Count > 0;

	public bool isInput => Direction == PortDirection.Input;

	public bool isOutput => Direction == PortDirection.Output;

	public bool acceptsOnlyOneConnection => Capacity == PortCapacity.Single;

	public bool acceptsMultipleConnections => Capacity == PortCapacity.Multi;

	public string value
	{
		get
		{
			return Value;
		}
		set
		{
			Value = value;
		}
	}

	public Type valueType
	{
		get
		{
			if (m_ValueType != null)
			{
				return m_ValueType;
			}
			if (string.IsNullOrEmpty(ValueTypeQualifiedName))
			{
				return null;
			}
			m_ValueType = Type.GetType(ValueTypeQualifiedName, throwOnError: false);
			return m_ValueType;
		}
		private set
		{
			m_ValueType = value;
			if (!(value == null))
			{
				ValueTypeQualifiedName = value.AssemblyQualifiedName;
			}
		}
	}

	private string valueTypeQualifiedName
	{
		get
		{
			return ValueTypeQualifiedName;
		}
		set
		{
			ValueTypeQualifiedName = value;
			m_ValueType = Type.GetType(value, throwOnError: false);
		}
	}

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

	public bool canBeReordered
	{
		get
		{
			return CanBeReordered;
		}
		internal set
		{
			CanBeReordered = value;
		}
	}

	public UnityAction<FlowDirection> ping { get; set; }

	public UnityAction refreshPortEditor { get; set; }

	public UnityAction refreshPortView { get; set; }

	public UnityAction<FlowPort> onConnected { get; set; }

	public UnityAction<FlowPort> onDisconnected { get; set; }

	public FlowNode node { get; set; }

	public FlowPort()
	{
		PortId = Guid.NewGuid().ToString();
		Connections = new List<string>();
		CanBeDeleted = true;
		CanBeReordered = true;
	}

	public FlowPort(FlowNode node, PortDirection direction, PortCapacity capacity)
		: this()
	{
		NodeId = node.nodeId;
		Direction = direction;
		Capacity = capacity;
		valueType = valueType;
		valueTypeQualifiedName = valueType.AssemblyQualifiedName;
		value = JsonUtility.ToJson(Activator.CreateInstance(valueType));
	}

	public FlowPort(FlowPort other)
	{
		PortId = other.portId;
		NodeId = other.nodeId;
		Direction = other.direction;
		Capacity = other.capacity;
		Connections = new List<string>(other.connections);
		CanBeDeleted = other.CanBeDeleted;
		CanBeReordered = other.CanBeReordered;
		valueType = other.valueType;
		valueTypeQualifiedName = other.valueTypeQualifiedName;
		value = other.value;
	}

	public T GetValue<T>()
	{
		return (T)JsonUtility.FromJson(value, typeof(T));
	}

	public FlowPort SetValue<T>(T data)
	{
		valueType = typeof(T);
		value = JsonUtility.ToJson(data);
		return this;
	}

	public ScriptableObject GetScriptableObjectValue(ScriptableObject targetScriptableObject)
	{
		JsonUtility.FromJsonOverwrite(value, targetScriptableObject);
		return targetScriptableObject;
	}
}
