namespace BansheeGz.BGDatabase;

public abstract class BGCalcUnitVarA : BGCalcUnit
{
	public static readonly byte VarId = 1;

	public static readonly byte VarType = 2;

	public BGCalcVar GraphVar => base.Graph.GetVars(createIfMissing: true).GetVar(VariableId);

	public BGId VariableId
	{
		get
		{
			BGCalcVarLite var = GetVar(VarId);
			return (BGId)var.Value;
		}
	}

	public override string Title
	{
		get
		{
			string text = ((this is BGCalcUnitGetVar) ? "Get" : "Set");
			BGCalcVar graphVar = GraphVar;
			if (graphVar == null)
			{
				return text + " variable[ERROR]";
			}
			return text + " variable [" + graphVar.Name + "]";
		}
	}

	public BGCalcTypeCode VariableTypeCode
	{
		get
		{
			BGCalcVarLite var = GetVar(VarType);
			return BGCalcTypeCodeRegistry.Get((byte)var.Value);
		}
	}

	public void Init(BGId varId, BGCalcTypeCode code)
	{
		GetVars()?.Variables.Clear();
		BGCalcVarLite bGCalcVarLite = BGCalcVarLite.Create(this, VarId, BGCalcTypeCodeRegistry.BGId);
		bGCalcVarLite.Value = varId;
		BGCalcVarLite bGCalcVarLite2 = BGCalcVarLite.Create(this, VarType, BGCalcTypeCodeRegistry.Byte);
		bGCalcVarLite2.Value = code.TypeCode;
	}
}
