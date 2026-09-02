using System;

namespace BansheeGz.BGDatabase;

[Serializable]
public class BGCalcVarLiteModel
{
	public byte Id;

	public byte TypeCode;

	public string Value;

	public string State;

	public BGCalcVarLiteModel(BGCalcVarLite variable)
	{
		Id = variable.Id;
		TypeCode = (byte)((variable.TypeCode != null) ? variable.TypeCode.TypeCode : 0);
		if (variable.TypeCode != null)
		{
			if (variable.TypeCode is BGCalcTypeCodeStateful bGCalcTypeCodeStateful)
			{
				State = bGCalcTypeCodeStateful.WriteState();
			}
			if (variable.TypeCode.SupportDefaultValue)
			{
				Value = variable.TypeCode.ValueToString(variable.Value);
			}
		}
	}

	public BGCalcVarLite ToVar(BGCalcVarsLiteOwnerI owner)
	{
		BGCalcVarLite bGCalcVarLite = BGCalcVarLite.Create(owner, Id, BGCalcTypeCodeRegistry.Get(TypeCode));
		if (bGCalcVarLite.TypeCode != null)
		{
			if (bGCalcVarLite.TypeCode is BGCalcTypeCodeStateful bGCalcTypeCodeStateful)
			{
				bGCalcTypeCodeStateful.ReadState(State);
			}
			if (bGCalcVarLite.TypeCode.SupportDefaultValue)
			{
				bGCalcVarLite.Value = bGCalcVarLite.TypeCode.ValueFromString(Value);
			}
		}
		return bGCalcVarLite;
	}
}
