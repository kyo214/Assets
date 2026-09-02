using System.Text;
using NPOI.Util;

namespace NPOI.DDF;

public class EscherSimpleProperty : EscherProperty
{
	protected int propertyValue;

	public int PropertyValue
	{
		get
		{
			return propertyValue;
		}
		internal set
		{
			propertyValue = value;
		}
	}

	public EscherSimpleProperty(short id, int propertyValue)
		: base(id)
	{
		this.propertyValue = propertyValue;
	}

	public EscherSimpleProperty(short propertyNumber, bool isComplex, bool isBlipId, int propertyValue)
		: base(propertyNumber, isComplex, isBlipId)
	{
		this.propertyValue = propertyValue;
	}

	public override int SerializeSimplePart(byte[] data, int offset)
	{
		LittleEndian.PutShort(data, offset, Id);
		LittleEndian.PutInt(data, offset + 2, propertyValue);
		return 6;
	}

	public override int SerializeComplexPart(byte[] data, int pos)
	{
		return 0;
	}

	public override bool Equals(object o)
	{
		if (this == o)
		{
			return true;
		}
		if (!(o is EscherSimpleProperty))
		{
			return false;
		}
		EscherSimpleProperty escherSimpleProperty = (EscherSimpleProperty)o;
		if (propertyValue != escherSimpleProperty.propertyValue)
		{
			return false;
		}
		if (Id != escherSimpleProperty.Id)
		{
			return false;
		}
		return true;
	}

	public override int GetHashCode()
	{
		return propertyValue;
	}

	public override string ToString()
	{
		return "propNum: " + PropertyNumber + ", RAW: 0x" + HexDump.ToHex(Id) + ", propName: " + EscherProperties.GetPropertyName(PropertyNumber) + ", complex: " + IsComplex + ", blipId: " + IsBlipId + ", value: " + propertyValue + " (0x" + HexDump.ToHex(propertyValue) + ")";
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
			.Append("\" complex=\"")
			.Append(IsComplex)
			.Append("\" value=\"")
			.Append("0x")
			.Append(HexDump.ToHex(propertyValue))
			.Append("\"/>\n");
		return stringBuilder.ToString();
	}
}
