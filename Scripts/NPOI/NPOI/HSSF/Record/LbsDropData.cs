using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class LbsDropData
{
	public const int STYLE_COMBO_DROPDOWN = 0;

	public const int STYLE_COMBO_EDIT_DROPDOWN = 1;

	public const int STYLE_COMBO_SIMPLE_DROPDOWN = 2;

	internal int _wStyle;

	internal int _cLine;

	private int _dxMin;

	private string _str;

	private byte _unused;

	public int DataSize => 6 + StringUtil.GetEncodedSize(_str) + _unused;

	public LbsDropData()
	{
		_str = "";
		_unused = 0;
	}

	public LbsDropData(ILittleEndianInput in1)
	{
		_wStyle = in1.ReadUShort();
		_cLine = in1.ReadUShort();
		_dxMin = in1.ReadUShort();
		_str = StringUtil.ReadUnicodeString(in1);
		if (StringUtil.GetEncodedSize(_str) % 2 != 0)
		{
			_unused = (byte)in1.ReadByte();
		}
	}

	public void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(_wStyle);
		out1.WriteShort(_cLine);
		out1.WriteShort(_dxMin);
		StringUtil.WriteUnicodeString(out1, _str);
		out1.WriteByte(_unused);
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[LbsDropData]\n");
		stringBuilder.Append("  ._wStyle:  ").Append(_wStyle).Append('\n');
		stringBuilder.Append("  ._cLine:  ").Append(_cLine).Append('\n');
		stringBuilder.Append("  ._dxMin:  ").Append(_dxMin).Append('\n');
		stringBuilder.Append("  ._str:  ").Append(_str).Append('\n');
		stringBuilder.Append("  ._unused:  ").Append(_unused).Append('\n');
		stringBuilder.Append("[/LbsDropData]\n");
		return stringBuilder.ToString();
	}
}
