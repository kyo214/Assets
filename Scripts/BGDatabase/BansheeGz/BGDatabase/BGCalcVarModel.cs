using System;

namespace BansheeGz.BGDatabase;

[Serializable]
public class BGCalcVarModel
{
	public string Id;

	public string Name;

	public bool IsPublic;

	public byte TypeCode;

	public string State;

	public string Value;

	public BGCalcVarModel(BGCalcVar variable)
	{
		Id = variable.Id.ToString();
		Name = variable.Name;
		IsPublic = variable.IsPublic;
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

	public BGCalcVar ToVar(BGCalcVarsOwnerI owner)
	{
		BGCalcVar bGCalcVar = BGCalcVar.Create(owner, BGId.Parse(Id), Name, BGCalcTypeCodeRegistry.Get(TypeCode));
		bGCalcVar.IsPublic = IsPublic;
		if (bGCalcVar.TypeCode != null)
		{
			if (bGCalcVar.TypeCode is BGCalcTypeCodeStateful)
			{
				((BGCalcTypeCodeStateful)bGCalcVar).ReadState(State);
			}
			if (bGCalcVar.TypeCode.SupportDefaultValue)
			{
				bGCalcVar.Value = bGCalcVar.TypeCode.ValueFromString(Value);
			}
		}
		return bGCalcVar;
	}
}
