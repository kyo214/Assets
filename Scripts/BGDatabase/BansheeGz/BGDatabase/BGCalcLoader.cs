using System;
using System.Text;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public class BGCalcLoader
{
	private readonly BGCalcGraph graph;

	private readonly BGBinaryReader reader;

	private readonly ArraySegment<byte> array;

	public BGCalcGraph Graph => graph;

	public BGBinaryReader Reader => reader;

	public BGCalcLoader(BGCalcGraph graph, ArraySegment<byte> array)
	{
		this.graph = graph;
		this.array = array;
		reader = new BGBinaryReader(array);
	}

	public void Load()
	{
		graph.Clear();
		reader.Reset(array);
		graph.Batch(() =>
		{
			ushort num = reader.ReadUShort();
			if (num == 1)
			{
				BGCalcVarContainer.FromBytes(reader, graph);
				BGCalcLoadContext context = new BGCalcLoadContext();
				ReadUshortByte(() =>
				{
					UnitFromBytes(context);
				});
				context.MapPorts();
				return;
			}
			throw new Exception($"Unknown graph serialization format version {num}");
		}, fireEventInTheEnd: false);
	}

	private void UnitFromBytes(BGCalcLoadContext context)
	{
		ushort num = reader.ReadUShort();
		BGCalcUnitI bGCalcUnitI;
		if (num != 0)
		{
			bGCalcUnitI = BGCalcUnitRegistry.Create(num);
		}
		else
		{
			string text = reader.ReadString();
			Type type = BGUtil.GetType(text);
			if (type == null)
			{
				throw new Exception("Can not find type " + text);
			}
			bGCalcUnitI = (BGCalcUnitI)Activator.CreateInstance(type);
		}
		float x = reader.ReadFloat();
		float y = reader.ReadFloat();
		bGCalcUnitI.Position = new Vector2(x, y);
		BGCalcLoadContext.UnitWrapper unitWrapper = new BGCalcLoadContext.UnitWrapper(bGCalcUnitI);
		context.Add(unitWrapper);
		BGCalcVarLiteContainer.FromBytes(reader, unitWrapper.unit);
		graph.Init(bGCalcUnitI);
		ReadUshortByte(() =>
		{
			ReadPort(unitWrapper);
		});
		graph.AddUnitNoInit(bGCalcUnitI);
	}

	private void ReadPort(BGCalcLoadContext.UnitWrapper unit)
	{
		BGCalcLoadContext.PortWrapper portWrapper = new BGCalcLoadContext.PortWrapper();
		unit.ports.Add(portWrapper);
		portWrapper.portId = ReadString256();
		portWrapper.portType = (BGCalcPortTypeEnum)reader.ReadByte();
		switch (portWrapper.portType)
		{
		case BGCalcPortTypeEnum.ControlIn:
			portWrapper.typeCode = 1;
			portWrapper.typeCodeObj = BGCalcTypeCodeRegistry.Control;
			break;
		case BGCalcPortTypeEnum.ControlOut:
			portWrapper.typeCode = 1;
			portWrapper.typeCodeObj = BGCalcTypeCodeRegistry.Control;
			portWrapper.UnitRef = reader.ReadByte();
			portWrapper.PortRef = reader.ReadByte();
			break;
		case BGCalcPortTypeEnum.ValueIn:
			ReadType(portWrapper);
			portWrapper.UnitRef = reader.ReadByte();
			if (portWrapper.UnitRef != 0)
			{
				portWrapper.PortRef = reader.ReadByte();
			}
			else
			{
				portWrapper.Value = portWrapper.typeCodeObj.ValueFromBytes(reader);
			}
			break;
		case BGCalcPortTypeEnum.ValueOut:
			ReadType(portWrapper);
			break;
		default:
			throw new ArgumentOutOfRangeException("portType");
		}
	}

	private string ReadString256()
	{
		byte length = reader.ReadByte();
		ArraySegment<byte> arraySegment = reader.ReadByteArrayRaw(length);
		if (arraySegment.Count != 0)
		{
			return Encoding.UTF8.GetString(arraySegment.Array, arraySegment.Offset, arraySegment.Count);
		}
		return null;
	}

	private void ReadType(BGCalcLoadContext.PortWrapper wrapper)
	{
		wrapper.typeCode = reader.ReadByte();
		if (wrapper.typeCode != 0)
		{
			wrapper.typeCodeObj = BGCalcTypeCodeRegistry.Get(wrapper.typeCode);
			if (wrapper.typeCodeObj is BGCalcTypeCodeStateful bGCalcTypeCodeStateful)
			{
				bGCalcTypeCodeStateful.ReadState(reader);
			}
		}
		else
		{
			wrapper.Type = reader.ReadString();
		}
	}

	private void ReadUshortByte(Action action)
	{
		byte b = reader.ReadByte();
		for (int i = 0; i < b; i++)
		{
			action();
		}
	}
}
