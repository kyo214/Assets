using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class OldSheetRecord
{
	public const short sid = 133;

	private int field_1_position_of_BOF;

	private int field_2_visibility;

	private int field_3_type;

	private byte[] field_5_sheetname;

	private CodepageRecord codepage;

	public short Sid => 133;

	public int PositionOfBof => field_1_position_of_BOF;

	public string Sheetname => OldStringRecord.GetString(field_5_sheetname, codepage);

	public OldSheetRecord(RecordInputStream in1)
	{
		field_1_position_of_BOF = in1.ReadInt();
		field_2_visibility = in1.ReadUByte();
		field_3_type = in1.ReadUByte();
		int num = in1.ReadUByte();
		field_5_sheetname = new byte[num];
		in1.Read(field_5_sheetname, 0, num);
	}

	public void SetCodePage(CodepageRecord codepage)
	{
		this.codepage = codepage;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[BOUNDSHEET]\n");
		stringBuilder.Append("    .bof        = ").Append(HexDump.IntToHex(PositionOfBof)).Append("\n");
		stringBuilder.Append("    .visibility = ").Append(HexDump.ShortToHex(field_2_visibility)).Append("\n");
		stringBuilder.Append("    .type       = ").Append(HexDump.ByteToHex(field_3_type)).Append("\n");
		stringBuilder.Append("    .sheetname  = ").Append(Sheetname).Append("\n");
		stringBuilder.Append("[/BOUNDSHEET]\n");
		return stringBuilder.ToString();
	}
}
