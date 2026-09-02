using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public class BGCalcLoadContext
{
	public class UnitWrapper
	{
		public readonly BGCalcUnitI unit;

		public readonly List<PortWrapper> ports = new List<PortWrapper>();

		public UnitWrapper(BGCalcUnitI unit)
		{
			this.unit = unit;
		}
	}

	public class PortWrapper
	{
		public string portId;

		public BGCalcPortTypeEnum portType;

		public int UnitRef;

		public int PortRef;

		public byte typeCode;

		public string Type;

		public BGCalcTypeCode typeCodeObj;

		public BGCalcPortI port;

		private object value;

		public bool HasValue;

		public object Value
		{
			get
			{
				return value;
			}
			set
			{
				this.value = value;
				HasValue = true;
			}
		}

		public string MapPort(BGCalcUnitI unit)
		{
			BGCalcPortI bGCalcPortI = unit.FindPort(portId);
			if (bGCalcPortI == null)
			{
				return "Can not find a port with id=" + portId + " at " + unit.Title + " unit- all connections to this port can not be resolved";
			}
			if (bGCalcPortI.PortType != portType)
			{
				return "Port with id=" + portId + " at " + unit.Title + " unit changed port type- all connections to this port can not be resolved";
			}
			if (!object.Equals(bGCalcPortI.TypeCode, typeCodeObj))
			{
				return "Port with id=" + portId + " at " + unit.Title + " unit changed type code- all connections to this port can not be resolved";
			}
			if (bGCalcPortI.TypeCode == null)
			{
				Type type = BGUtil.GetType(Type);
				if (bGCalcPortI.Type != type)
				{
					return "Port with id=" + portId + " at " + unit.Title + " unit changed type- all connections to this port can not be resolved";
				}
			}
			port = bGCalcPortI;
			return null;
		}

		public void Connect(List<UnitWrapper> units)
		{
			PortWrapper portWrapper = units[UnitRef].ports[PortRef];
			if (portWrapper.port != null)
			{
				port.Connect(portWrapper.port);
			}
		}
	}

	private readonly List<UnitWrapper> units = new List<UnitWrapper>();

	public void Add(UnitWrapper unitWrapper)
	{
		units.Add(unitWrapper);
	}

	public void MapPorts()
	{
		for (int i = 0; i < units.Count; i++)
		{
			UnitWrapper unitWrapper = units[i];
			List<PortWrapper> ports = unitWrapper.ports;
			for (int j = 0; j < ports.Count; j++)
			{
				PortWrapper portWrapper = ports[j];
				string text = portWrapper.MapPort(unitWrapper.unit);
				if (text != null)
				{
					Debug.Log("BGDatabase graph deserialization warning: " + text);
				}
			}
		}
		for (int k = 0; k < units.Count; k++)
		{
			UnitWrapper unitWrapper2 = units[k];
			List<PortWrapper> ports2 = unitWrapper2.ports;
			for (int l = 0; l < ports2.Count; l++)
			{
				PortWrapper portWrapper2 = ports2[l];
				if (portWrapper2.port == null)
				{
					continue;
				}
				switch (portWrapper2.port.PortType)
				{
				case BGCalcPortTypeEnum.ControlOut:
					if (portWrapper2.UnitRef > 0)
					{
						portWrapper2.Connect(units);
					}
					break;
				case BGCalcPortTypeEnum.ValueIn:
					if (portWrapper2.UnitRef > 0)
					{
						portWrapper2.Connect(units);
					}
					else if (portWrapper2.HasValue)
					{
						((BGCalcValueInputI)portWrapper2.port).DefaultValue = portWrapper2.Value;
					}
					break;
				default:
					throw new ArgumentOutOfRangeException();
				case BGCalcPortTypeEnum.ControlIn:
				case BGCalcPortTypeEnum.ValueOut:
					break;
				}
			}
		}
	}
}
