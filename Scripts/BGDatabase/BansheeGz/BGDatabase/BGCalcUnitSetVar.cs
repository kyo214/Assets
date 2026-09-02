using System;

namespace BansheeGz.BGDatabase;

public class BGCalcUnitSetVar : BGCalcUnitVarA
{
	private BGCalcValueInput value;

	private BGCalcControlOutput exit;

	public const int Code = 117;

	public override ushort TypeCode => 117;

	public override void Definition()
	{
		ControlInput("enter", "e", Run);
		exit = ControlOutput("exit", "x");
		value = ValueInput(base.VariableTypeCode.Type, "value", "v");
	}

	private BGCalcControlOutput Run(BGCalcFlowI flow)
	{
		BGCalcVar var = flow.GetVars(createIfMissing: true).GetVar(base.VariableId);
		if (var == null)
		{
			throw new Exception("Can not get target graph variable!");
		}
		var.Value = flow.GetValue(value);
		return exit;
	}
}
