using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class WSBoolRecord : StandardRecord
{
	public const short sid = 129;

	private byte field_1_wsbool;

	private byte field_2_wsbool;

	private static BitField autobreaks = BitFieldFactory.GetInstance(1);

	private static BitField dialog = BitFieldFactory.GetInstance(16);

	private static BitField applystyles = BitFieldFactory.GetInstance(32);

	private static BitField rowsumsbelow = BitFieldFactory.GetInstance(64);

	private static BitField rowsumsright = BitFieldFactory.GetInstance(128);

	private static BitField fittopage = BitFieldFactory.GetInstance(1);

	private static BitField Displayguts = BitFieldFactory.GetInstance(6);

	private static BitField alternateexpression = BitFieldFactory.GetInstance(64);

	private static BitField alternateformula = BitFieldFactory.GetInstance(128);

	public byte WSBool1
	{
		get
		{
			return field_1_wsbool;
		}
		set
		{
			field_1_wsbool = value;
		}
	}

	public bool Autobreaks
	{
		get
		{
			return autobreaks.IsSet(field_1_wsbool);
		}
		set
		{
			field_1_wsbool = autobreaks.SetByteBoolean(field_1_wsbool, value);
		}
	}

	public bool Dialog
	{
		get
		{
			return dialog.IsSet(field_1_wsbool);
		}
		set
		{
			field_1_wsbool = dialog.SetByteBoolean(field_1_wsbool, value);
		}
	}

	public bool RowSumsBelow
	{
		get
		{
			return rowsumsbelow.IsSet(field_1_wsbool);
		}
		set
		{
			field_1_wsbool = rowsumsbelow.SetByteBoolean(field_1_wsbool, value);
		}
	}

	public bool RowSumsRight
	{
		get
		{
			return rowsumsright.IsSet(field_1_wsbool);
		}
		set
		{
			field_1_wsbool = rowsumsright.SetByteBoolean(field_1_wsbool, value);
		}
	}

	public byte WSBool2
	{
		get
		{
			return field_2_wsbool;
		}
		set
		{
			field_2_wsbool = value;
		}
	}

	public bool FitToPage
	{
		get
		{
			return fittopage.IsSet(field_2_wsbool);
		}
		set
		{
			field_2_wsbool = fittopage.SetByteBoolean(field_2_wsbool, value);
		}
	}

	public bool DisplayGuts
	{
		get
		{
			return Displayguts.IsSet(field_2_wsbool);
		}
		set
		{
			field_2_wsbool = Displayguts.SetByteBoolean(field_2_wsbool, value);
		}
	}

	public bool AlternateExpression
	{
		get
		{
			return alternateexpression.IsSet(field_2_wsbool);
		}
		set
		{
			field_2_wsbool = alternateexpression.SetByteBoolean(field_2_wsbool, value);
		}
	}

	public bool AlternateFormula
	{
		get
		{
			return alternateformula.IsSet(field_2_wsbool);
		}
		set
		{
			field_2_wsbool = alternateformula.SetByteBoolean(field_2_wsbool, value);
		}
	}

	protected override int DataSize => 2;

	public override short Sid => 129;

	public WSBoolRecord()
	{
	}

	public WSBoolRecord(RecordInputStream in1)
	{
		byte[] array = in1.ReadRemainder();
		field_1_wsbool = array[0];
		field_2_wsbool = array[1];
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[WSBOOL]\n");
		stringBuilder.Append("    .wsbool1        = ").Append(StringUtil.ToHexString(WSBool1)).Append("\n");
		stringBuilder.Append("        .autobreaks = ").Append(Autobreaks).Append("\n");
		stringBuilder.Append("        .dialog     = ").Append(Dialog).Append("\n");
		stringBuilder.Append("        .rowsumsbelw= ").Append(RowSumsBelow).Append("\n");
		stringBuilder.Append("        .rowsumsrigt= ").Append(RowSumsRight).Append("\n");
		stringBuilder.Append("    .wsbool2        = ").Append(StringUtil.ToHexString(WSBool2)).Append("\n");
		stringBuilder.Append("        .fittopage  = ").Append(FitToPage).Append("\n");
		stringBuilder.Append("        .Displayguts= ").Append(DisplayGuts).Append("\n");
		stringBuilder.Append("        .alternateex= ").Append(AlternateExpression).Append("\n");
		stringBuilder.Append("        .alternatefo= ").Append(AlternateFormula).Append("\n");
		stringBuilder.Append("[/WSBOOL]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteByte(WSBool1);
		out1.WriteByte(WSBool2);
	}

	public override object Clone()
	{
		return new WSBoolRecord
		{
			field_1_wsbool = field_1_wsbool,
			field_2_wsbool = field_2_wsbool
		};
	}
}
