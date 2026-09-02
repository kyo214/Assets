namespace NPOI.DDF;

public class EscherPropertyMetaData
{
	public const byte TYPE_UNKNOWN = 0;

	public const byte TYPE_BOOL = 1;

	public const byte TYPE_RGB = 2;

	public const byte TYPE_SHAPEPATH = 3;

	public const byte TYPE_SIMPLE = 4;

	public const byte TYPE_ARRAY = 5;

	private string description;

	private byte type;

	public string Description => description;

	public byte Type => type;

	public EscherPropertyMetaData(string description)
	{
		this.description = description;
	}

	public EscherPropertyMetaData(string description, byte type)
	{
		this.description = description;
		this.type = type;
	}
}
