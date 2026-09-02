using System;
using System.Text;
using NPOI.Util;

namespace NPOI.DDF;

public class EscherOptRecord : AbstractEscherOptRecord
{
	public const short RECORD_ID = -4085;

	public const string RECORD_DESCRIPTION = "msofbtOPT";

	public override short Instance
	{
		get
		{
			Instance = (short)properties.Count;
			return base.Instance;
		}
	}

	internal override short Options
	{
		get
		{
			_ = Instance;
			_ = Version;
			return base.Options;
		}
	}

	public override string RecordName => "Opt";

	public override short Version
	{
		get
		{
			Version = 3;
			return base.Version;
		}
		set
		{
			if (value != 3)
			{
				throw new ArgumentException("msofbtOPT can have only '0x3' version");
			}
			base.Version = value;
		}
	}

	public override string ToXml(string tab)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(tab).Append(FormatXmlRecordHeader(GetType().Name, HexDump.ToHex(RecordId), HexDump.ToHex(Version), HexDump.ToHex(Instance)));
		foreach (EscherProperty escherProperty in base.EscherProperties)
		{
			stringBuilder.Append(escherProperty.ToXml(tab + "\t"));
		}
		stringBuilder.Append(tab).Append("</").Append(GetType().Name)
			.Append(">\n");
		return stringBuilder.ToString();
	}
}
