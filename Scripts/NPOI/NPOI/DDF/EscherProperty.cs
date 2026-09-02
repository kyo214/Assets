using System.Text;

namespace NPOI.DDF;

public abstract class EscherProperty
{
	protected short id;

	public virtual short Id => id;

	public virtual short PropertyNumber => (short)(id & 0x3FFF);

	public virtual bool IsComplex => (id & -32768) != 0;

	public virtual bool IsBlipId => (id & 0x4000) != 0;

	public virtual string Name => EscherProperties.GetPropertyName(PropertyNumber);

	public virtual int PropertySize => 6;

	public EscherProperty(short id)
	{
		this.id = id;
	}

	public EscherProperty(short propertyNumber, bool isComplex, bool isBlipId)
	{
		id = (short)(propertyNumber + (isComplex ? (-32768) : 0) + (isBlipId ? 16384 : 0));
	}

	public virtual string ToXml(string tab)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(tab).Append("<").Append(GetType().Name)
			.Append(" id=\"")
			.Append(Id)
			.Append("\" name=\"")
			.Append(Name)
			.Append("\" blipId=\"")
			.Append(IsBlipId)
			.Append("\"/>\n");
		return stringBuilder.ToString();
	}

	public abstract int SerializeSimplePart(byte[] data, int pos);

	public abstract int SerializeComplexPart(byte[] data, int pos);
}
