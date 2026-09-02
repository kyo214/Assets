namespace BansheeGz.BGDatabase;

public class BGCalcUnitLiteral : BGCalcUnit
{
	public const int Code = 103;

	public static readonly byte ValueVarId = 1;

	public override ushort TypeCode => 103;

	public BGCalcVarLite ValueVar => GetVar(ValueVarId);

	public BGCalcTypeCode ConstantTypeCode => ValueVar.TypeCode;

	public override string Title
	{
		get
		{
			BGCalcTypeCode constantTypeCode = ConstantTypeCode;
			if (constantTypeCode == null)
			{
				return "Introduce [ERROR]";
			}
			return constantTypeCode.Name + " literal";
		}
	}

	public override void Definition()
	{
		ValueOutput(ConstantTypeCode, "result", "r", GetValue);
	}

	public override string GetPublicVarLabel(byte varId)
	{
		if (varId != ValueVarId)
		{
			return null;
		}
		return "value";
	}

	private object GetValue(BGCalcFlowI flow)
	{
		return ValueVar.Value;
	}

	public void Init(BGCalcTypeCode code)
	{
		GetVars()?.Variables.Clear();
		BGCalcVarLite bGCalcVarLite = BGCalcVarLite.Create(this, ValueVarId, code);
	}
}
