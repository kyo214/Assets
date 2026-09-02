using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public class BGCalcVarLiteContainer : BGCalcVarContainerBaseA<BGCalcVarLite>
{
	public BGCalcVarLiteContainer(BGCalcVarsOwnerBaseI owner)
		: base(owner)
	{
	}

	public BGCalcVarLite GetVar(byte varId)
	{
		for (int i = 0; i < vars.Count; i++)
		{
			BGCalcVarLite bGCalcVarLite = vars[i];
			if (bGCalcVarLite.Id == varId)
			{
				return bGCalcVarLite;
			}
		}
		return null;
	}

	public bool HasVar(byte varId)
	{
		return GetVar(varId) != null;
	}

	public int RemoveVar(byte varId)
	{
		if (vars.Count == 0)
		{
			return 0;
		}
		int num = 0;
		List<BGCalcVarLite> list = new List<BGCalcVarLite>();
		for (int num2 = vars.Count - 1; num2 >= 0; num2--)
		{
			BGCalcVarLite bGCalcVarLite = vars[num2];
			if (bGCalcVarLite.Id == varId)
			{
				list.Add(bGCalcVarLite);
				vars.RemoveAt(num2);
				num++;
			}
		}
		FireOnAnyChange();
		return num;
	}

	public void CloneTo(BGCalcVarsLiteOwnerI owner)
	{
		for (int i = 0; i < vars.Count; i++)
		{
			vars[i].CloneTo(owner, cloneValue: true);
		}
	}

	public static void ToBytes(BGBinaryWriter writer, BGCalcVarLiteContainer container)
	{
		int num = container?.Count ?? 0;
		writer.AddByte((byte)num);
		if (num == 0)
		{
			return;
		}
		foreach (BGCalcVarLite var in container.vars)
		{
			writer.AddByte(var.Id);
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

	public static void FromBytes(BGBinaryReader reader, BGCalcVarsLiteOwnerI owner)
	{
		byte b = reader.ReadByte();
		if (b <= 0)
		{
			return;
		}
		for (int i = 0; i < b; i++)
		{
			byte id = reader.ReadByte();
			BGCalcTypeCode bGCalcTypeCode = BGCalcTypeCodeRegistry.Get(reader.ReadByte());
			if (bGCalcTypeCode is BGCalcTypeCodeStateful bGCalcTypeCodeStateful)
			{
				bGCalcTypeCodeStateful.ReadState(reader);
			}
			BGCalcVarLite bGCalcVarLite = BGCalcVarLite.Create(owner, id, bGCalcTypeCode);
			if (bGCalcTypeCode.SupportDefaultValue)
			{
				bGCalcVarLite.Value = bGCalcTypeCode.ValueFromBytes(reader);
			}
		}
	}
}
