using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public class BGCalcSaveContext
{
	public struct CalcUnityWrapper
	{
		public readonly int index;

		public readonly List<BGCalcPortI> ports;

		public readonly BGCalcUnitI unit;

		public CalcUnityWrapper(int index, BGCalcUnitI unit)
		{
			this = default;
			this.index = index;
			this.unit = unit;
			ports = new List<BGCalcPortI>();
			AddAll(unit.InControls);
			AddAll(unit.InValues);
			AddAll(unit.OutControls);
			AddAll(unit.OutValues);
			if (ports.Count > 255)
			{
				throw new Exception($"Maximum number of ports [{byte.MaxValue}] is exceeded!");
			}
		}

		private void AddAll<T>(List<T> portsToAdd) where T : BGCalcPortI
		{
			for (int i = 0; i < portsToAdd.Count; i++)
			{
				T val = portsToAdd[i];
				if (BGCalcPort.ShouldPortBeSerialized(val))
				{
					ports.Add(val);
				}
			}
		}

		public int GetPortIndex(BGCalcPortI port)
		{
			for (int i = 0; i < ports.Count; i++)
			{
				BGCalcPortI bGCalcPortI = ports[i];
				if (port == bGCalcPortI)
				{
					return i;
				}
			}
			throw new Exception("Can not find connected port! " + port.Unit.Title + "." + port.Name);
		}
	}

	private readonly List<CalcUnityWrapper> unitWrappers = new List<CalcUnityWrapper>();

	public List<CalcUnityWrapper> UnitWrappers => unitWrappers;

	public BGCalcSaveContext(BGCalcGraph graph)
	{
		graph.ForEachUnit(AddUnit);
	}

	private void AddUnit(BGCalcUnitI unit)
	{
		CalcUnityWrapper item = new CalcUnityWrapper(unitWrappers.Count, unit);
		unitWrappers.Add(item);
	}

	public CalcUnityWrapper? GetUnitWrapper(BGCalcUnitI unit)
	{
		int count = unitWrappers.Count;
		for (int i = 0; i < count; i++)
		{
			CalcUnityWrapper value = unitWrappers[i];
			if (value.unit == unit)
			{
				return value;
			}
		}
		return null;
	}
}
