using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class PrintGridlinesRecord : StandardRecord
{
	public const short sid = 43;

	private short field_1_print_gridlines;

	public bool PrintGridlines
	{
		get
		{
			return field_1_print_gridlines == 1;
		}
		set
		{
			if (value)
			{
				field_1_print_gridlines = 1;
			}
			else
			{
				field_1_print_gridlines = 0;
			}
		}
	}

	protected override int DataSize => 2;

	public override short Sid => 43;

	public PrintGridlinesRecord()
	{
	}

	public PrintGridlinesRecord(RecordInputStream in1)
	{
		field_1_print_gridlines = in1.ReadShort();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[PRINTGRIDLINES]\n");
		stringBuilder.Append("    .printgridlines = ").Append(PrintGridlines).Append("\n");
		stringBuilder.Append("[/PRINTGRIDLINES]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(field_1_print_gridlines);
	}

	public override object Clone()
	{
		return new PrintGridlinesRecord
		{
			field_1_print_gridlines = field_1_print_gridlines
		};
	}
}
