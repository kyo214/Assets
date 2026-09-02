using System;

namespace BansheeGz.BGDatabase;

public class BGCalcUnitGetVar : BGCalcUnitVarA
{
	public const int Code = 116;

	public override ushort TypeCode => 116;

	public override void Definition()
	{
		ValueOutput(base.GraphVar.TypeCode, "value", "v", GetValue);
	}

	private object GetValue(BGCalcFlowI flow)
	{
		BGCalcVar var = flow.GetVars(createIfMissing: true).GetVar(base.VariableId);
		if (var == null)
		{
			throw new Exception("Can not get target graph variable! id=" + base.VariableId.ToString());
		}
		return var.Value;
	}
}
