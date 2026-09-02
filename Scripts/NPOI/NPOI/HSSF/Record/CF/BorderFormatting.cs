using System;
using System.Text;
using NPOI.SS.UserModel;
using NPOI.Util;

namespace NPOI.HSSF.Record.CF;

public class BorderFormatting : ICloneable
{
	private int field_13_border_styles1;

	private static BitField bordLeftLineStyle = BitFieldFactory.GetInstance(15);

	private static BitField bordRightLineStyle = BitFieldFactory.GetInstance(240);

	private static BitField bordTopLineStyle = BitFieldFactory.GetInstance(3840);

	private static BitField bordBottomLineStyle = BitFieldFactory.GetInstance(61440);

	private static BitField bordLeftLineColor = BitFieldFactory.GetInstance(8323072);

	private static BitField bordRightLineColor = BitFieldFactory.GetInstance(1065353216);

	private static BitField bordTlBrLineOnOff = BitFieldFactory.GetInstance(1073741824);

	private static BitField bordBlTrtLineOnOff = BitFieldFactory.GetInstance(int.MinValue);

	private int field_14_border_styles2;

	private static BitField bordTopLineColor = BitFieldFactory.GetInstance(127);

	private static BitField bordBottomLineColor = BitFieldFactory.GetInstance(16256);

	private static BitField bordDiagLineColor = BitFieldFactory.GetInstance(2080768);

	private static BitField bordDiagLineStyle = BitFieldFactory.GetInstance(31457280);

	public int DataLength => 8;

	public BorderStyle BorderLeft
	{
		get
		{
			return (BorderStyle)bordLeftLineStyle.GetValue(field_13_border_styles1);
		}
		set
		{
			field_13_border_styles1 = bordLeftLineStyle.SetValue(field_13_border_styles1, (int)value);
		}
	}

	public BorderStyle BorderRight
	{
		get
		{
			return (BorderStyle)bordRightLineStyle.GetValue(field_13_border_styles1);
		}
		set
		{
			field_13_border_styles1 = bordRightLineStyle.SetValue(field_13_border_styles1, (int)value);
		}
	}

	public BorderStyle BorderTop
	{
		get
		{
			return (BorderStyle)bordTopLineStyle.GetValue(field_13_border_styles1);
		}
		set
		{
			field_13_border_styles1 = bordTopLineStyle.SetValue(field_13_border_styles1, (int)value);
		}
	}

	public BorderStyle BorderBottom
	{
		get
		{
			return (BorderStyle)bordBottomLineStyle.GetValue(field_13_border_styles1);
		}
		set
		{
			field_13_border_styles1 = bordBottomLineStyle.SetValue(field_13_border_styles1, (int)value);
		}
	}

	public BorderStyle BorderDiagonal
	{
		get
		{
			return (BorderStyle)bordDiagLineStyle.GetValue(field_14_border_styles2);
		}
		set
		{
			field_14_border_styles2 = bordDiagLineStyle.SetValue(field_14_border_styles2, (int)value);
		}
	}

	public short LeftBorderColor
	{
		get
		{
			return (short)bordLeftLineColor.GetValue(field_13_border_styles1);
		}
		set
		{
			field_13_border_styles1 = bordLeftLineColor.SetValue(field_13_border_styles1, value);
		}
	}

	public short RightBorderColor
	{
		get
		{
			return (short)bordRightLineColor.GetValue(field_13_border_styles1);
		}
		set
		{
			field_13_border_styles1 = bordRightLineColor.SetValue(field_13_border_styles1, value);
		}
	}

	public short TopBorderColor
	{
		get
		{
			return (short)bordTopLineColor.GetValue(field_14_border_styles2);
		}
		set
		{
			field_14_border_styles2 = bordTopLineColor.SetValue(field_14_border_styles2, value);
		}
	}

	public short BottomBorderColor
	{
		get
		{
			return (short)bordBottomLineColor.GetValue(field_14_border_styles2);
		}
		set
		{
			field_14_border_styles2 = bordBottomLineColor.SetValue(field_14_border_styles2, value);
		}
	}

	public short DiagonalBorderColor
	{
		get
		{
			return (short)bordDiagLineColor.GetValue(field_14_border_styles2);
		}
		set
		{
			field_14_border_styles2 = bordDiagLineColor.SetValue(field_14_border_styles2, value);
		}
	}

	public bool IsForwardDiagonalOn
	{
		get
		{
			return bordBlTrtLineOnOff.IsSet(field_13_border_styles1);
		}
		set
		{
			field_13_border_styles1 = bordBlTrtLineOnOff.SetBoolean(field_13_border_styles1, value);
		}
	}

	public bool IsBackwardDiagonalOn
	{
		get
		{
			return bordTlBrLineOnOff.IsSet(field_13_border_styles1);
		}
		set
		{
			field_13_border_styles1 = bordTlBrLineOnOff.SetBoolean(field_13_border_styles1, value);
		}
	}

	public BorderFormatting()
	{
		field_13_border_styles1 = 0;
		field_14_border_styles2 = 0;
	}

	public BorderFormatting(RecordInputStream in1)
	{
		field_13_border_styles1 = in1.ReadInt();
		field_14_border_styles2 = in1.ReadInt();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("    [Border Formatting]\n");
		stringBuilder.Append("          .lftln     = ").Append(StringUtil.ToHexString((int)BorderLeft)).Append("\n");
		stringBuilder.Append("          .rgtln     = ").Append(StringUtil.ToHexString((int)BorderRight)).Append("\n");
		stringBuilder.Append("          .topln     = ").Append(StringUtil.ToHexString((int)BorderTop)).Append("\n");
		stringBuilder.Append("          .btmln     = ").Append(StringUtil.ToHexString((int)BorderBottom)).Append("\n");
		stringBuilder.Append("          .leftborder= ").Append(StringUtil.ToHexString(LeftBorderColor)).Append("\n");
		stringBuilder.Append("          .rghtborder= ").Append(StringUtil.ToHexString(RightBorderColor)).Append("\n");
		stringBuilder.Append("          .topborder= ").Append(StringUtil.ToHexString(TopBorderColor)).Append("\n");
		stringBuilder.Append("          .bottomborder= ").Append(StringUtil.ToHexString(BottomBorderColor)).Append("\n");
		stringBuilder.Append("          .fwdiag= ").Append(IsForwardDiagonalOn).Append("\n");
		stringBuilder.Append("          .bwdiag= ").Append(IsBackwardDiagonalOn).Append("\n");
		stringBuilder.Append("    [/Border Formatting]\n");
		return stringBuilder.ToString();
	}

	public object Clone()
	{
		return new BorderFormatting
		{
			field_13_border_styles1 = field_13_border_styles1,
			field_14_border_styles2 = field_14_border_styles2
		};
	}

	public int Serialize(int offset, byte[] data)
	{
		LittleEndian.PutInt(data, offset, field_13_border_styles1);
		offset += 4;
		LittleEndian.PutInt(data, offset, field_14_border_styles2);
		offset += 4;
		return 8;
	}

	public void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteInt(field_13_border_styles1);
		out1.WriteInt(field_14_border_styles2);
	}
}
