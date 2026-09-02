using System;
using System.Text;
using NPOI.SS.UserModel;
using NPOI.Util;

namespace NPOI.HSSF.Record.CF;

public class IconMultiStateFormatting : ICloneable
{
	private IconSet iconSet;

	private byte options;

	private Threshold[] thresholds;

	private static BitField iconOnly = BitFieldFactory.GetInstance(1);

	private static BitField reversed = BitFieldFactory.GetInstance(4);

	public IconSet IconSet
	{
		get
		{
			return iconSet;
		}
		set
		{
			iconSet = value;
		}
	}

	public Threshold[] Thresholds
	{
		get
		{
			return thresholds;
		}
		set
		{
			thresholds = ((value == null) ? null : ((Threshold[])value.Clone()));
		}
	}

	public bool IsIconOnly
	{
		get
		{
			return GetOptionFlag(iconOnly);
		}
		set
		{
			SetOptionFlag(value, iconOnly);
		}
	}

	public bool IsReversed
	{
		get
		{
			return GetOptionFlag(reversed);
		}
		set
		{
			SetOptionFlag(value, reversed);
		}
	}

	public int DataLength
	{
		get
		{
			int num = 6;
			Threshold[] array = thresholds;
			foreach (Threshold threshold in array)
			{
				num += threshold.DataLength;
			}
			return num;
		}
	}

	public IconMultiStateFormatting()
	{
		iconSet = IconSet.GYR_3_TRAFFIC_LIGHTS;
		options = 0;
		thresholds = new Threshold[iconSet.num];
	}

	public IconMultiStateFormatting(ILittleEndianInput in1)
	{
		in1.ReadShort();
		in1.ReadByte();
		int num = in1.ReadByte();
		int id = in1.ReadByte();
		iconSet = IconSet.ById(id);
		_ = iconSet.num;
		options = (byte)in1.ReadByte();
		thresholds = new Threshold[iconSet.num];
		for (int i = 0; i < thresholds.Length; i++)
		{
			thresholds[i] = new IconMultiStateThreshold(in1);
		}
	}

	private bool GetOptionFlag(BitField field)
	{
		if (field.GetValue(options) != 0)
		{
			return true;
		}
		return false;
	}

	private void SetOptionFlag(bool option, BitField field)
	{
		options = field.SetByteBoolean(options, option);
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("    [Icon Formatting]\n");
		stringBuilder.Append("          .icon_set = ").Append(iconSet).Append("\n");
		stringBuilder.Append("          .icon_only= ").Append(IsIconOnly).Append("\n");
		stringBuilder.Append("          .reversed = ").Append(IsReversed).Append("\n");
		Threshold[] array = thresholds;
		foreach (Threshold threshold in array)
		{
			stringBuilder.Append(threshold.ToString());
		}
		stringBuilder.Append("    [/Icon Formatting]\n");
		return stringBuilder.ToString();
	}

	public object Clone()
	{
		IconMultiStateFormatting iconMultiStateFormatting = new IconMultiStateFormatting();
		iconMultiStateFormatting.iconSet = iconSet;
		iconMultiStateFormatting.options = options;
		iconMultiStateFormatting.thresholds = new Threshold[thresholds.Length];
		Array.Copy(thresholds, 0, iconMultiStateFormatting.thresholds, 0, thresholds.Length);
		return iconMultiStateFormatting;
	}

	public void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(0);
		out1.WriteByte(0);
		out1.WriteByte(iconSet.num);
		out1.WriteByte(iconSet.id);
		out1.WriteByte(options);
		Threshold[] array = thresholds;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Serialize(out1);
		}
	}
}
