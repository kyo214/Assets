using System;
using System.Text;
using NPOI.HSSF.Record.Cont;
using NPOI.Util;

namespace NPOI.HSSF.Record;

[Serializable]
public class StringRecord : ContinuableRecord
{
	public const short sid = 519;

	private bool _is16bitUnicode;

	private string _text;

	public override short Sid => 519;

	public string String
	{
		get
		{
			return _text;
		}
		set
		{
			_text = value;
			_is16bitUnicode = StringUtil.HasMultibyte(value);
		}
	}

	public StringRecord()
	{
	}

	public StringRecord(RecordInputStream in1)
	{
		int requestedLength = in1.ReadShort();
		_is16bitUnicode = in1.ReadByte() != 0;
		if (_is16bitUnicode)
		{
			_text = in1.ReadUnicodeLEString(requestedLength);
		}
		else
		{
			_text = in1.ReadCompressedUnicode(requestedLength);
		}
	}

	protected override void Serialize(ContinuableRecordOutput out1)
	{
		out1.WriteShort(_text.Length);
		out1.WriteStringData(_text);
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[STRING]\n");
		stringBuilder.Append("    .string            = ").Append(_text).Append("\n");
		stringBuilder.Append("[/STRING]\n");
		return stringBuilder.ToString();
	}

	public override object Clone()
	{
		return new StringRecord
		{
			_is16bitUnicode = _is16bitUnicode,
			_text = _text
		};
	}
}
