using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

[Serializable]
public class BGCalcGraphModel
{
	public int Version;

	public List<BGCalcVarModel> variables;

	public List<BGCalcUnitModel> units;

	public BGCalcGraphModel()
	{
	}

	public BGCalcGraphModel(BGCalcGraph graph)
	{
		BGCalcGraphModel bGCalcGraphModel = this;
		Version = 1;
		BGCalcVarContainer vars = graph.GetVars();
		if (vars != null && vars.Variables.Count > 0)
		{
			variables = new List<BGCalcVarModel>();
			foreach (BGCalcVar variable in graph.GetVars().Variables)
			{
				variables.Add(new BGCalcVarModel(variable));
			}
		}
		if (graph.UnitsCount > 0)
		{
			if (graph.UnitsCount > 255)
			{
				throw new Exception($"Can not serialize graph, cause the number of units={graph.UnitsCount} exceeds maximum {byte.MaxValue}");
			}
			BGCalcSaveContext context = new BGCalcSaveContext(graph);
			units = new List<BGCalcUnitModel>();
			graph.ForEachUnit((BGCalcUnitI unit) =>
			{
				bGCalcGraphModel.units.Add(new BGCalcUnitModel(unit, context));
			});
		}
	}

	public void ToGraph(BGCalcGraph graph)
	{
		List<BGCalcVarModel> list = variables;
		if (list != null && list.Count > 0)
		{
			foreach (BGCalcVarModel variable in variables)
			{
				variable.ToVar(graph);
			}
		}
		BGCalcLoadContext bGCalcLoadContext = new BGCalcLoadContext();
		List<BGCalcUnitModel> list2 = units;
		if (list2 != null && list2.Count > 0)
		{
			foreach (BGCalcUnitModel unit in units)
			{
				unit.ToUnit(graph, bGCalcLoadContext);
			}
		}
		bGCalcLoadContext.MapPorts();
	}
}
