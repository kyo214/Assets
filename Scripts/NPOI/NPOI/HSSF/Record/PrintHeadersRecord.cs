using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class PrintHeadersRecord : StandardRecord
{
	public const short sid = 42;

	private short field_1_print_headers;

	public bool PrintHeaders
	{
		get
		{
			return field_1_print_headers == 1;
		}
		set
		{
			field_1_print_headers = (short)(value ? 1 : 0);
		}
	}

	protected override int DataSize => 2;

	public override short Sid => 42;

	public PrintHeadersRecord()
	{
	}

	public PrintHeadersRecord(RecordInputStream in1)
	{
		field_1_print_headers = in1.ReadShort();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[PRINTHEADERS]\n");
		stringBuilder.Append("    .printheaders   = ").Append(PrintHeaders).Append("\n");
		stringBuilder.Append("[/PRINTHEADERS]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(field_1_print_headers);
	}

	public override object Clone()
	{
		return new PrintHeadersRecord
		{
			field_1_print_headers = field_1_print_headers
		};
	}
}
