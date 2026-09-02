using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class ExternSheetSubRecord : Record
{
	public const short sid = 4095;

	private short field_1_index_to_supbook;

	private short field_2_index_to_first_supbook_sheet;

	private short field_3_index_to_last_supbook_sheet;

	public override int RecordSize => 6;

	public override short Sid => 4095;

	public ExternSheetSubRecord()
	{
	}

	public ExternSheetSubRecord(RecordInputStream in1)
	{
		field_1_index_to_supbook = in1.ReadShort();
		field_2_index_to_first_supbook_sheet = in1.ReadShort();
		field_3_index_to_last_supbook_sheet = in1.ReadShort();
	}

	public void SetIndexToSupBook(short index)
	{
		field_1_index_to_supbook = index;
	}

	public short GetIndexToSupBook()
	{
		return field_1_index_to_supbook;
	}

	public void SetIndexToFirstSupBook(short index)
	{
		field_2_index_to_first_supbook_sheet = index;
	}

	public short GetIndexToFirstSupBook()
	{
		return field_2_index_to_first_supbook_sheet;
	}

	public void SetIndexToLastSupBook(short index)
	{
		field_3_index_to_last_supbook_sheet = index;
	}

	public short GetIndexToLastSupBook()
	{
		return field_3_index_to_last_supbook_sheet;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("   supbookindex =").Append(GetIndexToSupBook()).Append('\n');
		stringBuilder.Append("   1stsbindex   =").Append(GetIndexToFirstSupBook()).Append('\n');
		stringBuilder.Append("   lastsbindex  =").Append(GetIndexToLastSupBook()).Append('\n');
		return stringBuilder.ToString();
	}

	public override int Serialize(int offset, byte[] data)
	{
		LittleEndian.PutShort(data, offset, GetIndexToSupBook());
		LittleEndian.PutShort(data, 2 + offset, GetIndexToFirstSupBook());
		LittleEndian.PutShort(data, 4 + offset, GetIndexToLastSupBook());
		return RecordSize;
	}
}
