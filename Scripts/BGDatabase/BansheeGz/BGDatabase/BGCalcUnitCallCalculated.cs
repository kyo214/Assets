using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("Special/Call calculated cell")]
public class BGCalcUnitCallCalculated : BGCalcUnit2ControlsA
{
	private BGCalcValueOutput r;

	public const byte CellVarId = 1;

	public const int Code = 115;

	public override ushort TypeCode => 115;

	public override string Title => "Call calculated";

	public event Action OnCellChange;

	public override string GetPublicVarLabel(byte varId)
	{
		if (1 != varId)
		{
			return null;
		}
		return "cell";
	}

	public override void Definition()
	{
		base.Definition();
		BGCalcVarLite orAddVar = GetOrAddVar(1, BGCalcTypeCodeRegistry.Cell);
		OnCellChanged();
		orAddVar.OnValueChange += OnCellChanged;
	}

	private void OnCellChanged()
	{
		for (int num = base.InControls.Count - 1; num >= 0; num--)
		{
			BGCalcControlInputI bGCalcControlInputI = base.InControls[num];
			if (!(bGCalcControlInputI.Id == "y"))
			{
				RemovePort(bGCalcControlInputI);
			}
		}
		for (int num2 = base.OutControls.Count - 1; num2 >= 0; num2--)
		{
			BGCalcControlOutputI bGCalcControlOutputI = base.OutControls[num2];
			if (!(bGCalcControlOutputI.Id == "z"))
			{
				RemovePort(bGCalcControlOutputI);
			}
		}
		for (int num3 = base.InValues.Count - 1; num3 >= 0; num3--)
		{
			BGCalcValueInputI port = base.InValues[num3];
			RemovePort(port);
		}
		for (int num4 = base.OutValues.Count - 1; num4 >= 0; num4--)
		{
			BGCalcValueOutputI port2 = base.OutValues[num4];
			RemovePort(port2);
		}
		BGCalcVarLite var = GetVar(1);
		if (var != null && var.Value != null)
		{
			BGCalcCell bGCalcCell = (BGCalcCell)var.Value;
			BGField field = bGCalcCell.Field;
			BGEntity entity = bGCalcCell.Entity;
			if (field != null && entity != null && field is BGFieldCalcI bGFieldCalcI && field is BGStorable<BGFieldCalcValue> storable)
			{
				BGCalcGraph bGCalcGraph = CellGraph(storable, bGFieldCalcI, entity, out var _);
				if (bGCalcGraph != null)
				{
					AddResultPort(bGFieldCalcI);
					BGCalcVarContainer vars = bGCalcGraph.GetVars();
					foreach (BGCalcVar variable in vars.Variables)
					{
						if (variable.IsPublic)
						{
							AddPorts(variable);
						}
					}
				}
			}
		}
		OnCellChange?.Invoke();
	}

	private void AddResultPort(BGFieldCalcI field)
	{
		r = ValueOutput(field.ResultCode, "result", "r", null);
	}

	private static BGCalcGraph CellGraph(BGStorable<BGFieldCalcValue> storable, BGFieldCalcI calcI, BGEntity entity, out BGCalcVarsProvider varsOverrides)
	{
		varsOverrides = null;
		BGFieldCalcValue storedValue = storable.GetStoredValue(entity.Index);
		BGCalcGraph graph;
		if (storedValue?.Graph != null)
		{
			graph = storedValue.Graph;
			varsOverrides = storedValue;
		}
		else
		{
			graph = calcI.Graph;
		}
		return graph;
	}

	private void AddPorts(BGCalcVar calcVar)
	{
		ValueInput(calcVar.TypeCode, calcVar.Name, GetInputId(calcVar.Id));
		ValueOutput(calcVar.TypeCode, calcVar.Name, GetOutputId(calcVar.Id), null);
	}

	private string GetInputId(BGId calcVarId)
	{
		BGId bGId = calcVarId;
		return "i_" + bGId.ToString();
	}

	private string GetOutputId(BGId calcVarId)
	{
		BGId bGId = calcVarId;
		return "o_" + bGId.ToString();
	}

	protected override void Run(BGCalcFlowI flow)
	{
		BGCalcVarLite var = GetVar(1);
		BGCalcCell bGCalcCell = (BGCalcCell)var.Value;
		if (bGCalcCell == null)
		{
			throw new Exception("Can not execute calculated cell, cause the cell value was not set!");
		}
		BGField field = bGCalcCell.Field;
		if (field == null)
		{
			throw new Exception("Can not execute calculated cell, cause the field is null!");
		}
		if (!(field is BGFieldCalcI calcI))
		{
			throw new Exception("Can not execute calculated cell, cause the field is not a calculated field!");
		}
		if (flow.Context.GraphType != BGCalcGraphTypeEnum.Action && field is BGFieldCalcAction)
		{
			throw new Exception("Action calculated field can only be called from another Action calculated field!");
		}
		if (!(field is BGStorable<BGFieldCalcValue> storable))
		{
			throw new Exception("Can not execute calculated cell, cause the field is not a BGStorable<BGFieldCalcValue> field!");
		}
		BGEntity entity = bGCalcCell.Entity;
		if (entity == null)
		{
			throw new Exception("Can not execute calculated cell, cause the entity is null!");
		}
		BGCalcGraph bGCalcGraph = CellGraph(storable, calcI, entity, out var varsOverrides);
		if (bGCalcGraph == null)
		{
			throw new Exception("Can not execute calculated cell, cause the graph is null!");
		}
		if (flow.Level >= 16)
		{
			throw new Exception("Can not execute calculated cell, cause the maximum nested level=16 is exceeded (seems like recursive call)!");
		}
		BGCalcFlowContext bGCalcFlowContext = BGCalcFlowContext.Get();
		try
		{
			bGCalcFlowContext.Graph = bGCalcGraph;
			bGCalcFlowContext.CurrentEntity = entity;
			bGCalcFlowContext.CurrentGameObject = flow.Context.CurrentGameObject;
			bGCalcFlowContext.VarsOverrides = varsOverrides;
			bGCalcFlowContext.GraphType = flow.Context.GraphType;
			BGCalcFlow bGCalcFlow = new BGCalcFlow(bGCalcFlowContext)
			{
				Level = flow.Level + 1
			};
			List<BGCalcVar> variables = bGCalcFlow.GetVars().Variables;
			foreach (BGCalcVar item in variables)
			{
				if (item.IsPublic)
				{
					string inputId = GetInputId(item.Id);
					BGCalcPortI bGCalcPortI = FindPort(inputId);
					if (bGCalcPortI != null)
					{
						item.Value = flow.GetValue((BGCalcValueInputI)bGCalcPortI);
					}
				}
			}
			BGCalcControlOutput startPort = bGCalcGraph.StartUnit.StartPort;
			bGCalcFlowContext.Graph = bGCalcGraph;
			if (startPort.IsConnected)
			{
				bGCalcFlow.Run();
			}
			flow.SetValue(r, bGCalcFlow.Result);
			variables = bGCalcFlow.GetVars().Variables;
			foreach (BGCalcVar item2 in variables)
			{
				if (item2.IsPublic)
				{
					string outputId = GetOutputId(item2.Id);
					BGCalcPortI bGCalcPortI2 = FindPort(outputId);
					if (bGCalcPortI2 != null)
					{
						flow.SetValue((BGCalcValueOutputI)bGCalcPortI2, item2.Value);
					}
				}
			}
		}
		finally
		{
			BGCalcFlowContext.Return(bGCalcFlowContext);
		}
	}
}
