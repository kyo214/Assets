using System.Text;
using NPOI.Util;

namespace NPOI.DDF;

public class EscherBoolProperty : EscherSimpleProperty
{
	public bool IsTrue => propertyValue != 0;

	public bool IsFalse => propertyValue == 0;

	public EscherBoolProperty(short propertyNumber, int value)
		: base(propertyNumber, value)
	{
	}

	public override string ToXml(string tab)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(tab).Append("<").Append(GetType().Name)
			.Append(" id=\"0x")
			.Append(HexDump.ToHex(Id))
			.Append("\" name=\"")
			.Append(Name)
			.Append("\" simpleValue=\"")
			.Append(base.PropertyValue)
			.Append("\" blipId=\"")
			.Append(IsBlipId)
			.Append("\" value=\"")
			.Append(IsTrue)
			.Append("\"")
			.Append("/>\n");
		return stringBuilder.ToString();
	}
}
