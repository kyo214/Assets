using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public class BGCalcValueOutput : BGCalcPort, BGCalcValueOutputI, BGCalcPortI
{
	private readonly Func<BGCalcFlowI, object> getValue;

	private readonly BGCalcTypeCode typeCode;

	private readonly List<BGCalcValueInputI> connectedPorts = new List<BGCalcValueInputI>();

	public Func<BGCalcFlowI, object> GetValue => getValue;

	public override bool IsConnected => connectedPorts.Count > 0;

	public override BGCalcTypeCode TypeCode => typeCode;

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

	public BGCalcValueOutput(BGCalcUnit unit, string name, string id, Type type, Func<BGCalcFlowI, object> getValue)
		: base(unit, name, id, BGCalcPortTypeEnum.ValueOut, type)
	{
		typeCode = BGCalcTypeCodeRegistry.Get(type);
		this.getValue = getValue;
	}

	public BGCalcValueOutput(BGCalcUnit unit, string name, string id, BGCalcTypeCode typeCode, Func<BGCalcFlowI, object> getValue)
		: base(unit, name, id, BGCalcPortTypeEnum.ValueOut, typeCode.Type)
	{
		this.typeCode = typeCode;
		this.getValue = getValue;
	}

	public override void Connect(BGCalcPortI port, bool connectBoth = true)
	{
		if (port == null)
		{
			throw new Exception("Can not connect: port is null");
		}
		if (!(port is BGCalcValueInputI bGCalcValueInputI))
		{
			throw new Exception("Can not connect: wrong port type, should be BGCalcValueInputI! type=" + port.GetType().FullName);
		}
		if (!connectedPorts.Contains(bGCalcValueInputI))
		{
			connectedPorts.Add(bGCalcValueInputI);
			if (connectBoth)
			{
				bGCalcValueInputI.Connect(this, connectBoth: false);
				FireOnAnyChange();
			}
		}
	}

	public override void Disconnect(BGCalcPortI port, bool disconnectBoth = true)
	{
		if (port is BGCalcValueInputI bGCalcValueInputI)
		{
			connectedPorts.Remove(bGCalcValueInputI);
			if (disconnectBoth)
			{
				bGCalcValueInputI.Disconnect(this, disconnectBoth: false);
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
		if (toConnectPort.PortType != BGCalcPortTypeEnum.ValueIn)
		{
			return false;
		}
		return ((BGCalcValueInputI)toConnectPort).CanConnectTo(this);
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
		if (!(other is BGCalcValueOutput { connectedPorts: var list }))
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
