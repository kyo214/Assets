using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public class BGCalcVarContainer : BGCalcVarContainerBaseA<BGCalcVar>
{
	public event Action<List<BGCalcVar>> OnDelete;

	public BGCalcVarContainer(BGCalcVarsOwnerBaseI owner)
		: base(owner)
	{
	}

	public BGCalcVar GetVar(BGId varId)
	{
		for (int i = 0; i < vars.Count; i++)
		{
			BGCalcVar bGCalcVar = vars[i];
			if (bGCalcVar.Id == varId)
			{
				return bGCalcVar;
			}
		}
		return null;
	}

	public bool HasVar(BGId id)
	{
		return GetVar(id) != null;
	}

	public int RemoveVar(BGId id)
	{
		if (vars.Count == 0)
		{
			return 0;
		}
		int num = 0;
		List<BGCalcVar> list = new List<BGCalcVar>();
		for (int num2 = vars.Count - 1; num2 >= 0; num2--)
		{
			BGCalcVar bGCalcVar = vars[num2];
			if (!(bGCalcVar.Id != id))
			{
				list.Add(bGCalcVar);
				vars.RemoveAt(num2);
				num++;
			}
		}
		if (num > 0)
		{
			OnDelete?.Invoke(list);
		}
		FireOnAnyChange();
		return num;
	}

	public BGCalcVar GetVar(string varName)
	{
		for (int i = 0; i < vars.Count; i++)
		{
			BGCalcVar bGCalcVar = vars[i];
			if (bGCalcVar.Name == varName)
			{
				return bGCalcVar;
			}
		}
		return null;
	}

	public void CloneTo(BGCalcVarsOwnerI owner)
	{
		for (int i = 0; i < vars.Count; i++)
		{
			vars[i].CloneTo(owner, cloneId: true, cloneValue: true);
		}
	}

	public static void ToBytes(BGBinaryWriter writer, BGCalcVarContainer container)
	{
		int num = container?.Count ?? 0;
		writer.AddByte((byte)num);
		if (num == 0)
		{
			return;
		}
		foreach (BGCalcVar var in container.vars)
		{
			writer.AddId(var.Id);
			writer.AddString(var.Name);
			writer.AddBool(var.IsPublic);
			writer.AddByte(var.TypeCode.TypeCode);
			if (var.TypeCode is BGCalcTypeCodeStateful bGCalcTypeCodeStateful)
			{
				bGCalcTypeCodeStateful.WriteState(writer);
			}
			if (var.TypeCode.SupportDefaultValue)
			{
				var.TypeCode.ValueToBytes(writer, var.Value);
			}
		}
	}

	public static void FromBytes(BGBinaryReader reader, BGCalcVarsOwnerI owner)
	{
		byte b = reader.ReadByte();
		if (b <= 0)
		{
			return;
		}
		for (int i = 0; i < b; i++)
		{
			BGId id = reader.ReadId();
			string name = reader.ReadString();
			bool isPublic = reader.ReadBool();
			BGCalcTypeCode bGCalcTypeCode = BGCalcTypeCodeRegistry.Get(reader.ReadByte());
			if (bGCalcTypeCode is BGCalcTypeCodeStateful bGCalcTypeCodeStateful)
			{
				bGCalcTypeCodeStateful.ReadState(reader);
			}
			BGCalcVar bGCalcVar = BGCalcVar.Create(owner, id, name, bGCalcTypeCode);
			if (bGCalcVar.TypeCode.SupportDefaultValue)
			{
				bGCalcVar.Value = bGCalcTypeCode.ValueFromBytes(reader);
			}
			bGCalcVar.IsPublic = isPublic;
		}
	}
}
