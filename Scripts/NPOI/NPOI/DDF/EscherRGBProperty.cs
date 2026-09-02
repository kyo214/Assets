using System.Text;
using NPOI.Util;

namespace NPOI.DDF;

public class EscherRGBProperty : EscherSimpleProperty
{
	public int RgbColor => propertyValue;

	public byte Red => (byte)(propertyValue & 0xFF);

	public byte Green => (byte)((propertyValue >> 8) & 0xFF);

	public byte Blue => (byte)((propertyValue >> 16) & 0xFF);

	public EscherRGBProperty(short propertyNumber, int rgbColor)
		: base(propertyNumber, rgbColor)
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
			.Append("\" blipId=\"")
			.Append(IsBlipId)
			.Append("\" value=\"0x")
			.Append(HexDump.ToHex(propertyValue))
			.Append("\"/>\n");
		return stringBuilder.ToString();
	}
}
