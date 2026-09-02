using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[Serializable]
public class BGCalcUnitModel
{
	public ushort TypeCode;

	public string TypeName;

	public float PosX;

	public float PosY;

	public List<BGCalcVarLiteModel> variables;

	public List<BGCalcUnitPortModel> ports;

	private readonly BGCalcUnitI unit;

	private readonly BGCalcSaveContext context;

	public BGCalcUnitI Unit => unit;

	public bool IsStartUnit => TypeCode == 1;

	public BGCalcUnitModel(BGCalcUnitI unit, BGCalcSaveContext context, bool processPorts = true)
	{
		this.unit = unit;
		this.context = context;
		TypeCode = unit.TypeCode;
		if (TypeCode == 0)
		{
			TypeName = unit.GetType().AssemblyQualifiedName;
		}
		PosX = unit.Position.x;
		PosY = unit.Position.y;
		BGCalcVarLiteContainer vars = unit.GetVars();
		if (vars != null && vars.Variables.Count > 0)
		{
			variables = new List<BGCalcVarLiteModel>();
			foreach (BGCalcVarLite variable in unit.GetVars().Variables)
			{
				variables.Add(new BGCalcVarLiteModel(variable));
			}
		}
		if (processPorts)
		{
			ProcessPorts();
		}
	}

	public void ProcessPorts(Func<BGCalcPortI, bool> filter = null)
	{
		List<BGCalcPortI> list = unit.FindPorts(null);
		if (list == null || list.Count <= 0)
		{
			return;
		}
		if (list.Count > 255)
		{
			throw new Exception($"Can not serialize graph, cause the number of ports={list.Count} in {unit.Title} exceeds maximum {byte.MaxValue}");
		}
		ports = new List<BGCalcUnitPortModel>();
		foreach (BGCalcPortI item in list)
		{
			if (BGCalcPort.ShouldPortBeSerialized(item) && (filter == null || filter(item)))
			{
				ports.Add(new BGCalcUnitPortModel(item, context));
			}
		}
	}

	public BGCalcUnitI ToUnit(BGCalcGraph graph, BGCalcLoadContext context)
	{
		BGCalcUnitI bGCalcUnitI;
		if (TypeCode != 0)
		{
			bGCalcUnitI = BGCalcUnitRegistry.Create(TypeCode);
		}
		else
		{
			Type type = BGUtil.GetType(TypeName);
			if (type == null)
			{
				throw new Exception("Can not find type " + TypeName);
			}
			bGCalcUnitI = (BGCalcUnitI)Activator.CreateInstance(type);
		}
		bGCalcUnitI.Position = new Vector2(PosX, PosY);
		BGCalcLoadContext.UnitWrapper unitWrapper = new BGCalcLoadContext.UnitWrapper(bGCalcUnitI);
		context.Add(unitWrapper);
		if (bGCalcUnitI.TypeCode == 123)
		{
			return bGCalcUnitI;
		}
		List<BGCalcVarLiteModel> list = variables;
		if (list != null && list.Count > 0)
		{
			foreach (BGCalcVarLiteModel variable in variables)
			{
				variable.ToVar(bGCalcUnitI);
			}
		}
		graph.Init(bGCalcUnitI);
		List<BGCalcUnitPortModel> list2 = ports;
		if (list2 != null && list2.Count > 0)
		{
			foreach (BGCalcUnitPortModel port in ports)
			{
				port.ToPort(unitWrapper);
			}
		}
		graph.AddUnitNoInit(bGCalcUnitI);
		return bGCalcUnitI;
	}
}
