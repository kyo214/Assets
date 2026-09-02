using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public class BGCalcValueInput : BGCalcPort, BGCalcValueInputI, BGCalcPortI
{
	private BGCalcValueOutputI connectedPort;

	private readonly BGCalcTypeCode typeCode;

	private object defaultValue;

	public override bool IsConnected => connectedPort != null;

	public BGCalcValueOutputI ConnectedPort => connectedPort;

	public override BGCalcTypeCode TypeCode => typeCode;

	public override List<BGCalcPortI> ConnectedPorts
	{
		get
		{
			if (connectedPort != null)
			{
				return new List<BGCalcPortI> { connectedPort };
			}
			return null;
		}
	}

	public object DefaultValue
	{
		get
		{
			object obj = defaultValue;
			if (obj != null)
			{
				if (obj is BGObjectI bGObjectI)
				{
					BGObjectI dbObject = bGObjectI;
					if (BGCalcVarA.RefreshDbValue(ref dbObject))
					{
						defaultValue = dbObject;
					}
				}
				return defaultValue;
			}
			return defaultValue;
		}
		set
		{
			if (!object.Equals(defaultValue, value))
			{
				defaultValue = value;
				OnChange?.Invoke();
				FireOnAnyChange();
			}
		}
	}

	public bool HasDefaultValue
	{
		get
		{
			if (IsConnected)
			{
				return false;
			}
			if (TypeCode != null && TypeCode.SupportDefaultValue)
			{
				return !TypeCode.AreEqual(defaultValue, TypeCode.DefaultValue);
			}
			return false;
		}
	}

	public bool SupportDefaultValue
	{
		get
		{
			if (typeCode != null)
			{
				return typeCode.SupportDefaultValue;
			}
			return false;
		}
	}

	public event Action OnChange;

	public BGCalcValueInput(BGCalcUnit graph, string name, string id, BGCalcTypeCode typeCode)
		: base(graph, name, id, BGCalcPortTypeEnum.ValueIn, typeCode.Type)
	{
		this.typeCode = typeCode;
		if (this.typeCode != null && this.typeCode.SupportDefaultValue)
		{
			DefaultValue = this.typeCode.DefaultValue;
		}
	}

	public BGCalcValueInput(BGCalcUnit graph, string name, string id, Type type)
		: base(graph, name, id, BGCalcPortTypeEnum.ValueIn, type)
	{
		typeCode = BGCalcTypeCodeRegistry.Get(type);
		if (typeCode != null && typeCode.SupportDefaultValue)
		{
			DefaultValue = typeCode.DefaultValue;
		}
	}

	public override void Connect(BGCalcPortI port, bool connectBoth = true)
	{
		if (port == null)
		{
			throw new Exception("Can not connect: port is null");
		}
		if (!(port is BGCalcValueOutput))
		{
			throw new Exception("Can not connect: wrong port type, should be BGCalcValueOutput! type=" + port.GetType().FullName);
		}
		connectedPort?.Disconnect(this);
		BGCalcValueOutputI bGCalcValueOutputI = (BGCalcValueOutputI)port;
		connectedPort = bGCalcValueOutputI;
		if (connectBoth)
		{
			port.Connect(this, connectBoth: false);
			FireOnAnyChange();
		}
		OnChange?.Invoke();
	}

	public override void Disconnect(BGCalcPortI port, bool disconnectBoth = true)
	{
		if (connectedPort == port && port != null)
		{
			connectedPort = null;
			if (disconnectBoth)
			{
				port.Disconnect(this, disconnectBoth: false);
				FireOnAnyChange();
			}
		}
	}

	public override void DisconnectAll()
	{
		Disconnect(connectedPort);
	}

	public override bool CanConnectTo(BGCalcPortI toConnectPort)
	{
		if (toConnectPort.PortType != BGCalcPortTypeEnum.ValueOut)
		{
			return false;
		}
		if (TypeCode != null)
		{
			if (!object.Equals(TypeCode, toConnectPort.TypeCode) && !TypeCode.CanBeConvertedFrom(toConnectPort.TypeCode))
			{
				return false;
			}
		}
		else if (!base.Type.IsAssignableFrom(toConnectPort.Type))
		{
			return false;
		}
		if (HasRecursion((BGCalcValueOutputI)toConnectPort))
		{
			return false;
		}
		return true;
	}

	private bool HasRecursion(BGCalcValueOutputI port)
	{
		if (port.Unit == base.Unit)
		{
			return true;
		}
		Stack<BGCalcUnitI> stack = new Stack<BGCalcUnitI>();
		stack.Push(port.Unit);
		HashSet<BGCalcUnitI> hashSet = new HashSet<BGCalcUnitI> { port.Unit };
		while (stack.Count > 0)
		{
			BGCalcUnitI bGCalcUnitI = stack.Pop();
			List<BGCalcPortI> list = bGCalcUnitI.FindPorts((BGCalcPortI p) => p.PortType == BGCalcPortTypeEnum.ValueIn && p.IsConnected);
			foreach (BGCalcPortI item in list)
			{
				BGCalcValueOutputI bGCalcValueOutputI = ((BGCalcValueInputI)item).ConnectedPort;
				BGCalcUnitI bGCalcUnitI2 = bGCalcValueOutputI.Unit;
				if (!hashSet.Contains(bGCalcUnitI2))
				{
					if (bGCalcUnitI2 == base.Unit)
					{
						return true;
					}
					stack.Push(bGCalcUnitI2);
					hashSet.Add(bGCalcUnitI2);
				}
			}
		}
		return false;
	}

	public override bool IsEqual(BGCalcPortI other)
	{
		if (other == this)
		{
			return true;
		}
		if (!base.IsEqual(other))
		{
			return false;
		}
		if (!(other is BGCalcValueInput bGCalcValueInput))
		{
			return false;
		}
		if (typeCode != null && typeCode.SupportDefaultValue && !typeCode.AreEqual(defaultValue, bGCalcValueInput.defaultValue))
		{
			return false;
		}
		if (connectedPort != null && bGCalcValueInput.connectedPort == null)
		{
			return false;
		}
		if (connectedPort == null && bGCalcValueInput.connectedPort != null)
		{
			return false;
		}
		if (connectedPort != null)
		{
			if (!connectedPort.IsEqual(bGCalcValueInput.connectedPort))
			{
				return false;
			}
		}
		else if (bGCalcValueInput.connectedPort != null)
		{
			return false;
		}
		return true;
	}
}
