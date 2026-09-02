using System;
using System.IO;
using System.Text;
using NPOI.Util;

namespace NPOI.DDF;

public class EscherComplexProperty : EscherProperty
{
	protected byte[] _complexData = new byte[0];

	public byte[] ComplexData => _complexData;

	public override int PropertySize => 6 + _complexData.Length;

	public EscherComplexProperty(short id, byte[] complexData)
		: base(id)
	{
		if (complexData == null)
		{
			throw new ArgumentNullException("complexData can't be null");
		}
		_complexData = (byte[])complexData.Clone();
	}

	public EscherComplexProperty(short propertyNumber, bool isBlipId, byte[] complexData)
		: base(propertyNumber, isComplex: true, isBlipId)
	{
		if (complexData == null)
		{
			throw new ArgumentNullException("complexData can't be null");
		}
		_complexData = (byte[])complexData.Clone();
	}

	public override int SerializeSimplePart(byte[] data, int pos)
	{
		LittleEndian.PutShort(data, pos, Id);
		LittleEndian.PutInt(data, pos + 2, _complexData.Length);
		return 6;
	}

	public override int SerializeComplexPart(byte[] data, int pos)
	{
		Array.Copy(_complexData, 0, data, pos, _complexData.Length);
		return _complexData.Length;
	}

	public override bool Equals(object o)
	{
		if (this == o)
		{
			return true;
		}
		if (o == null || !(o is EscherComplexProperty))
		{
			return false;
		}
		EscherComplexProperty escherComplexProperty = (EscherComplexProperty)o;
		if (!Arrays.Equals(_complexData, escherComplexProperty._complexData))
		{
			return false;
		}
		return true;
	}

	public override int GetHashCode()
	{
		return Id * 11;
	}

	public override string ToString()
	{
		string text;
		using (MemoryStream memoryStream = new MemoryStream())
		{
			try
			{
				HexDump.Dump(_complexData, 0L, memoryStream, 0);
				text = memoryStream.ToString();
			}
			catch (Exception ex)
			{
				text = ex.ToString();
			}
		}
		return "propNum: " + PropertyNumber + ", propName: " + EscherProperties.GetPropertyName(PropertyNumber) + ", complex: " + IsComplex + ", blipId: " + IsBlipId + ", data: " + Environment.NewLine + text;
	}

	public override string ToXml(string tab)
	{
		HexDump.ToHex(_complexData, 32);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(tab).Append("<").Append(GetType().Name)
			.Append(" id=\"0x")
			.Append(HexDump.ToHex(Id))
			.Append("\" name=\"")
			.Append(Name)
			.Append("\" blipId=\"")
			.Append(IsBlipId)
			.Append("\">\n");
		stringBuilder.Append(tab).Append("</").Append(GetType().Name)
			.Append(">\n");
		return stringBuilder.ToString();
	}
}
