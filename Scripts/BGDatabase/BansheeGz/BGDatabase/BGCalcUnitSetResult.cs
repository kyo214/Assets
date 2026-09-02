using System;

namespace BansheeGz.BGDatabase;

public class BGCalcUnitSetResult : BGCalcUnit2ControlsA
{
	public static readonly byte TypeCodeVarId = 1;

	private BGCalcValueInput valueInput;

	public const int Code = 110;

	public override ushort TypeCode => 110;

	public override string Title => "Set result";

	public BGCalcTypeCode ResultTypeCode => BGCalcTypeCodeRegistry.Get((byte)GetVar(TypeCodeVarId).Value);

	public override void Definition()
	{
		base.Definition();
		BGCalcTypeCode resultTypeCode = ResultTypeCode;
		if (resultTypeCode == null)
		{
			throw new Exception("Result type code var is not found!");
		}
		valueInput = ValueInput(resultTypeCode, "result", "r");
	}

	protected override void Run(BGCalcFlowI flow)
	{
		object value = flow.GetValue(valueInput);
		flow.Result = value;
	}

	public void Init(BGCalcTypeCode resultCode)
	{
		GetVars()?.Variables.Clear();
		BGCalcVarLite bGCalcVarLite = BGCalcVarLite.Create(this, TypeCodeVarId, BGCalcTypeCodeRegistry.Byte);
		bGCalcVarLite.Value = resultCode.TypeCode;
	}
}
