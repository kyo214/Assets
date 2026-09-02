using System;
using System.Text;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public class BGCalcSaver
{
	public const ushort LastVersion = 1;

	private readonly BGCalcGraph graph;

	private readonly BGBinaryWriter writer;

	public BGBinaryWriter Writer => writer;

	public BGCalcSaver(BGCalcGraph graph)
	{
		this.graph = graph;
		writer = new BGBinaryWriter();
	}

	public byte[] Save()
	{
		if (graph.UnitsCount > 255)
		{
			throw new Exception($"Can not serialize graph, cause the number of units={graph.UnitsCount} exceeds maximum {byte.MaxValue}");
		}
		writer.Clear();
		writer.AddUShort(1);
		BGCalcVarContainer.ToBytes(writer, graph.GetVars());
		BGCalcSaveContext context = new BGCalcSaveContext(graph);
		AddArrayByteLength(() =>
		{
			for (int i = 0; i < context.UnitWrappers.Count; i++)
			{
				UnitToBytes(context, context.UnitWrappers[i]);
			}
		}, (byte)context.UnitWrappers.Count);
		return writer.ToArray();
	}

	private void UnitToBytes(BGCalcSaveContext context, BGCalcSaveContext.CalcUnityWrapper unitWrapper)
	{
		if (unitWrapper.unit.PortsCount > 255)
		{
			throw new Exception($"Can not serialize graph, cause the number of ports={unitWrapper.unit.PortsCount} " + $"for unit {unitWrapper.unit.Title} exceeds maximum {byte.MaxValue}");
		}
		BGCalcUnitI unit = unitWrapper.unit;
		ushort typeCode = unit.TypeCode;
		writer.AddUShort(typeCode);
		if (typeCode == 0)
		{
			writer.AddString(unit.GetType().AssemblyQualifiedName);
		}
		Vector2 position = unit.Position;
		writer.AddFloat(position.x);
		writer.AddFloat(position.y);
		BGCalcVarLiteContainer.ToBytes(writer, unit.GetVars());
		ProcessPorts(unitWrapper, context);
	}

	private void ProcessPorts(BGCalcSaveContext.CalcUnityWrapper unitWrapper, BGCalcSaveContext context)
	{
		AddArrayByteLength(() =>
		{
			for (int i = 0; i < unitWrapper.ports.Count; i++)
			{
				ToBytes(unitWrapper.ports[i], context);
			}
		}, (byte)unitWrapper.ports.Count);
	}

	private void ToBytes(BGCalcPortI port, BGCalcSaveContext context)
	{
		AddStringByteLength(port.Id);
		writer.AddByte((byte)port.PortType);
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
			if (!port.IsConnected)
			{
				bGCalcValueInputI.TypeCode.ValueToBytes(writer, bGCalcValueInputI.DefaultValue);
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

	private void AddReference(BGCalcPortI connectedPort, BGCalcSaveContext context)
	{
		if (connectedPort != null)
		{
			BGCalcSaveContext.CalcUnityWrapper? unitWrapper = context.GetUnitWrapper(connectedPort.Unit);
			writer.AddByte((byte)unitWrapper.Value.index);
			writer.AddByte((byte)unitWrapper.Value.GetPortIndex(connectedPort));
		}
		else
		{
			writer.AddByte(0);
		}
	}

	private void AddType(BGCalcPortI port)
	{
		if (port.TypeCode != null)
		{
			writer.AddByte(port.TypeCode.TypeCode);
			if (port.TypeCode is BGCalcTypeCodeStateful bGCalcTypeCodeStateful)
			{
				bGCalcTypeCodeStateful.WriteState(writer);
			}
		}
		else
		{
			writer.AddByte(0);
			writer.AddString(port.Type.AssemblyQualifiedName);
		}
	}

	public void AddArrayByteLength(Action action, byte count = 0)
	{
		writer.AddByte(count);
		if (count > 0)
		{
			action();
		}
	}

	public void AddStringByteLength(string value)
	{
		writer.AddByte((byte)value.Length);
		writer.AddBytesRaw(Encoding.UTF8.GetBytes(value));
	}
}
