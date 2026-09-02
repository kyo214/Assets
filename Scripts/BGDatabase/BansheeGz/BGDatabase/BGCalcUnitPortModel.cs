using System;

namespace BansheeGz.BGDatabase;

[Serializable]
public class BGCalcUnitPortModel
{
	public string Id;

	public byte PortType;

	public string State;

	public byte TypeCode;

	public string Type;

	public byte UnitRef;

	public byte PortRef;

	public bool HasValue;

	public string Value;

	private readonly BGCalcPortI port;

	public BGCalcPortI Port => port;

	public BGCalcUnitPortModel(BGCalcPortI port, BGCalcSaveContext context)
	{
		this.port = port;
		Id = port.Id;
		PortType = (byte)port.PortType;
		switch (port.PortType)
		{
		case BGCalcPortTypeEnum.ControlOut:
			AddReference(((BGCalcControlOutputI)port).ConnectedPort, context);
			break;
		case BGCalcPortTypeEnum.ValueIn:
		{
			AddType(port);
			BGCalcValueInputI bGCalcValueInputI = (BGCalcValueInputI)port;
			AddReference(bGCalcValueInputI.ConnectedPort, context);
			if (bGCalcValueInputI.HasDefaultValue)
			{
				HasValue = true;
				Value = bGCalcValueInputI.TypeCode.ValueToString(bGCalcValueInputI.DefaultValue);
			}
			break;
		}
		case BGCalcPortTypeEnum.ValueOut:
			AddType(port);
			break;
		default:
			throw new ArgumentOutOfRangeException("PortType");
		case BGCalcPortTypeEnum.ControlIn:
			break;
		}
	}

	private void AddType(BGCalcPortI port)
	{
		if (port.TypeCode != null)
		{
			TypeCode = port.TypeCode.TypeCode;
			if (port.TypeCode is BGCalcTypeCodeStateful bGCalcTypeCodeStateful)
			{
				State = bGCalcTypeCodeStateful.WriteState();
			}
		}
		else
		{
			Type = port.Type.AssemblyQualifiedName;
		}
	}

	private void AddReference(BGCalcPortI connectedPort, BGCalcSaveContext context)
	{
		if (connectedPort != null)
		{
			BGCalcSaveContext.CalcUnityWrapper? unitWrapper = context.GetUnitWrapper(connectedPort.Unit);
			if (unitWrapper.HasValue)
			{
				int portIndex = unitWrapper.Value.GetPortIndex(connectedPort);
				UnitRef = (byte)unitWrapper.Value.index;
				PortRef = (byte)portIndex;
			}
		}
	}

	public void ToPort(BGCalcLoadContext.UnitWrapper unitWrapper)
	{
		BGCalcLoadContext.PortWrapper portWrapper = new BGCalcLoadContext.PortWrapper();
		unitWrapper.ports.Add(portWrapper);
		portWrapper.portId = Id;
		portWrapper.portType = (BGCalcPortTypeEnum)PortType;
		switch (portWrapper.portType)
		{
		case BGCalcPortTypeEnum.ControlIn:
			portWrapper.typeCode = 1;
			portWrapper.typeCodeObj = BGCalcTypeCodeRegistry.Control;
			break;
		case BGCalcPortTypeEnum.ControlOut:
			portWrapper.typeCode = 1;
			portWrapper.typeCodeObj = BGCalcTypeCodeRegistry.Control;
			portWrapper.UnitRef = UnitRef;
			portWrapper.PortRef = PortRef;
			break;
		case BGCalcPortTypeEnum.ValueIn:
			ReadType(portWrapper);
			if (UnitRef > 0)
			{
				portWrapper.UnitRef = UnitRef;
				portWrapper.PortRef = PortRef;
			}
			else if (HasValue)
			{
				portWrapper.HasValue = true;
				portWrapper.Value = portWrapper.typeCodeObj.ValueFromString(Value);
			}
			break;
		case BGCalcPortTypeEnum.ValueOut:
			ReadType(portWrapper);
			break;
		default:
			throw new ArgumentOutOfRangeException("portType");
		}
	}

	private void ReadType(BGCalcLoadContext.PortWrapper wrapper)
	{
		wrapper.typeCode = TypeCode;
		if (wrapper.typeCode != 0)
		{
			wrapper.typeCodeObj = BGCalcTypeCodeRegistry.Get(wrapper.typeCode);
			if (wrapper.typeCodeObj is BGCalcTypeCodeStateful bGCalcTypeCodeStateful)
			{
				bGCalcTypeCodeStateful.ReadState(State);
			}
		}
		else
		{
			wrapper.Type = Type;
		}
	}
}
