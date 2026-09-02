using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class TabIdRecord : StandardRecord
{
	public const short sid = 317;

	private static short[] EMPTY_SHORT_ARRAY = new short[0];

	public short[] _tabids;

	protected override int DataSize => _tabids.Length * 2;

	public override short Sid => 317;

	public TabIdRecord()
	{
		_tabids = EMPTY_SHORT_ARRAY;
	}

	public TabIdRecord(RecordInputStream in1)
	{
		_tabids = new short[in1.Remaining / 2];
		for (int i = 0; i < _tabids.Length; i++)
		{
			_tabids[i] = in1.ReadShort();
		}
	}

	public void SetTabIdArray(short[] array)
	{
		_tabids = (short[])array.Clone();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[TABID]\n");
		stringBuilder.Append("    .elements        = ").Append(_tabids.Length).Append("\n");
		for (int i = 0; i < _tabids.Length; i++)
		{
			stringBuilder.Append("    .element_" + i + "       = ").Append(_tabids[i]).Append("\n");
		}
		stringBuilder.Append("[/TABID]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		short[] tabids = _tabids;
		for (int i = 0; i < tabids.Length; i++)
		{
			out1.WriteShort(tabids[i]);
		}
	}
}
