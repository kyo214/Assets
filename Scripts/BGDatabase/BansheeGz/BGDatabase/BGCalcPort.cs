using System;
using System.Collections;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public abstract class BGCalcPort : BGCalcPortI
{
	private readonly BGCalcUnitI unit;

	public string Name { get; }

	public string Id { get; }

	public BGCalcPortTypeEnum PortType { get; }

	public BGCalcUnitI Unit => unit;

	public Type Type { get; }

	public abstract BGCalcTypeCode TypeCode { get; }

	public bool IsSingle
	{
		get
		{
			switch (PortType)
			{
			case BGCalcPortTypeEnum.ControlIn:
			case BGCalcPortTypeEnum.ValueOut:
				return false;
			case BGCalcPortTypeEnum.ControlOut:
			case BGCalcPortTypeEnum.ValueIn:
				return true;
			default:
				throw new ArgumentOutOfRangeException("portType");
			}
		}
	}

	public abstract bool IsConnected { get; }

	public bool IsInput
	{
		get
		{
			if (PortType != BGCalcPortTypeEnum.ControlIn)
			{
				return PortType == BGCalcPortTypeEnum.ValueIn;
			}
			return true;
		}
	}

	public abstract List<BGCalcPortI> ConnectedPorts { get; }

	protected BGCalcPort(BGCalcUnit unit, string name, string id, BGCalcPortTypeEnum portType, Type type)
	{
		if (id == null)
		{
			throw new Exception("id can not be null");
		}
		if (id.Length > 31)
		{
			throw new Exception("id length is 31 chars maximum. incorrect value=" + id);
		}
		this.unit = unit ?? throw new Exception("Unit can not be null");
		Name = name;
		Id = id;
		PortType = portType;
		Type = type;
	}

	public abstract void Connect(BGCalcPortI port, bool connectBoth = true);

	public abstract void Disconnect(BGCalcPortI port, bool disconnectBoth = true);

	public abstract void DisconnectAll();

	public abstract bool CanConnectTo(BGCalcPortI toConnectPort);

	public override string ToString()
	{
		return Unit.Title + "." + Name + ((TypeCode != null) ? (" [" + TypeCode.TypeTitle + "]") : (" (" + Type.Name + ")")) + " id=" + Id;
	}

	public static bool ShouldPortBeSerialized(BGCalcPortI port)
	{
		if (port.IsConnected)
		{
			return true;
		}
		if (port.PortType == BGCalcPortTypeEnum.ValueIn && ((BGCalcValueInputI)port).HasDefaultValue)
		{
			return true;
		}
		return false;
	}

	public void FireOnAnyChange()
	{
		Unit.Graph.FireOnAnyChange();
	}

	public virtual bool IsEqual(BGCalcPortI other)
	{
		if (other == null)
		{
			return false;
		}
		if (Name == other.Name && Id == other.Id && PortType == other.PortType && Type == other.Type && object.Equals(TypeCode, other.TypeCode))
		{
			return IsConnected == other.IsConnected;
		}
		return false;
	}

	protected static bool ListEqual<T>(List<T> list1, List<T> list2) where T : BGCalcPortI
	{
		if (list1.Count != list2.Count)
		{
			return false;
		}
		BitArray bitArray = new BitArray(list1.Count);
		for (int i = 0; i < list1.Count; i++)
		{
			T val = list1[i];
			BGCalcPortI bGCalcPortI = null;
			for (int j = 0; j < list2.Count; j++)
			{
				if (!bitArray[j])
				{
					T val2 = list2[j];
					if (!(val.Id != val2.Id))
					{
						bitArray[j] = true;
						bGCalcPortI = val2;
						break;
					}
				}
			}
			if (bGCalcPortI == null)
			{
				return false;
			}
		}
		return true;
	}
}
