using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public class BGCalcControlOutput : BGCalcPort, BGCalcControlOutputI, BGCalcPortI
{
	private BGCalcControlInputI connectedPort;

	public override bool IsConnected => connectedPort != null;

	public BGCalcControlInputI ConnectedPort => connectedPort;

	public override BGCalcTypeCode TypeCode => BGCalcTypeCodeRegistry.Control;

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

	public BGCalcControlOutput(BGCalcUnit unit, string name, string id)
		: base(unit, name, id, BGCalcPortTypeEnum.ControlOut, typeof(BGCalcControl))
	{
	}

	public override void Connect(BGCalcPortI port, bool connectBoth = true)
	{
		if (port == null)
		{
			throw new Exception("Can not connect: port is null");
		}
		if (!(port is BGCalcControlInputI bGCalcControlInputI))
		{
			throw new Exception("Can not connect: wrong port type, should be BGCalcControlInputI! type=" + port.GetType().FullName);
		}
		connectedPort?.Disconnect(this);
		connectedPort = bGCalcControlInputI;
		if (connectBoth)
		{
			bGCalcControlInputI.Connect(this, connectBoth: false);
			FireOnAnyChange();
		}
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
		if (toConnectPort.PortType != BGCalcPortTypeEnum.ControlIn)
		{
			return false;
		}
		if (base.Unit == toConnectPort.Unit)
		{
			return false;
		}
		if (HasRecursion((BGCalcControlInputI)toConnectPort))
		{
			return false;
		}
		return true;
	}

	private bool HasRecursion(BGCalcControlInputI port)
	{
		Stack<BGCalcUnitI> stack = new Stack<BGCalcUnitI>();
		stack.Push(port.Unit);
		while (stack.Count > 0)
		{
			BGCalcUnitI bGCalcUnitI = stack.Pop();
			List<BGCalcPortI> list = bGCalcUnitI.FindPorts((BGCalcPortI p) => p.PortType == BGCalcPortTypeEnum.ControlOut && p.IsConnected);
			foreach (BGCalcPortI item in list)
			{
				BGCalcControlInputI bGCalcControlInputI = ((BGCalcControlOutputI)item).ConnectedPort;
				BGCalcUnitI bGCalcUnitI2 = bGCalcControlInputI.Unit;
				if (bGCalcUnitI2 == base.Unit)
				{
					return true;
				}
				if (stack.Contains(bGCalcUnitI2))
				{
					return true;
				}
				stack.Push(bGCalcUnitI2);
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
		if (!(other is BGCalcControlOutput bGCalcControlOutput))
		{
			return false;
		}
		if (connectedPort != null)
		{
			if (!connectedPort.IsEqual(bGCalcControlOutput.connectedPort))
			{
				return false;
			}
		}
		else if (bGCalcControlOutput.connectedPort != null)
		{
			return false;
		}
		return true;
	}
}
