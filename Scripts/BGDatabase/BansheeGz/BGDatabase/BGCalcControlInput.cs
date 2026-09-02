using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public class BGCalcControlInput : BGCalcPort, BGCalcControlInputI, BGCalcPortI
{
	private readonly List<BGCalcControlOutputI> connectedPorts = new List<BGCalcControlOutputI>();

	public override bool IsConnected => connectedPorts.Count > 0;

	public Func<BGCalcFlowI, BGCalcControlOutputI> Action { get; }

	public override BGCalcTypeCode TypeCode => BGCalcTypeCodeRegistry.Control;

	public override List<BGCalcPortI> ConnectedPorts
	{
		get
		{
			if (connectedPorts.Count != 0)
			{
				return new List<BGCalcPortI>(connectedPorts);
			}
			return null;
		}
	}

	public BGCalcControlInput(BGCalcUnit unit, string name, string id, Func<BGCalcFlowI, BGCalcControlOutputI> action)
		: base(unit, name, id, BGCalcPortTypeEnum.ControlIn, typeof(BGCalcControl))
	{
		Action = action;
	}

	public override void Connect(BGCalcPortI port, bool connectBoth = true)
	{
		if (port == null)
		{
			throw new Exception("Can not connect: port is null");
		}
		if (!(port is BGCalcControlOutputI bGCalcControlOutputI))
		{
			throw new Exception("Can not connect: wrong port type, should be BGCalcControlOutputI! type=" + port.GetType().FullName);
		}
		if (!connectedPorts.Contains(bGCalcControlOutputI))
		{
			connectedPorts.Add(bGCalcControlOutputI);
			if (connectBoth)
			{
				bGCalcControlOutputI.Connect(this, connectBoth: false);
				FireOnAnyChange();
			}
		}
	}

	public override void Disconnect(BGCalcPortI port, bool disconnectBoth = true)
	{
		if (port is BGCalcControlOutputI bGCalcControlOutputI)
		{
			connectedPorts.Remove(bGCalcControlOutputI);
			if (disconnectBoth)
			{
				bGCalcControlOutputI.Disconnect(this, disconnectBoth: false);
				FireOnAnyChange();
			}
		}
	}

	public override void DisconnectAll()
	{
		if (connectedPorts.Count == 0)
		{
			return;
		}
		base.Unit.Graph.Batch(() =>
		{
			for (int num = connectedPorts.Count - 1; num >= 0; num--)
			{
				Disconnect(connectedPorts[num]);
			}
		});
	}

	public override bool CanConnectTo(BGCalcPortI toConnectPort)
	{
		if (toConnectPort.PortType != BGCalcPortTypeEnum.ControlOut)
		{
			return false;
		}
		return ((BGCalcControlOutputI)toConnectPort).CanConnectTo(this);
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
		if (!(other is BGCalcControlInput { connectedPorts: var list }))
		{
			return false;
		}
		if (!BGCalcPort.ListEqual(connectedPorts, list))
		{
			return false;
		}
		return true;
	}
}
