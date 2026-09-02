using NPOI.POIFS.Storage;

namespace NPOI.POIFS.Properties;

public class RootProperty : DirectoryProperty
{
	private const string NAME = "Root Entry";

	public override int Size
	{
		set
		{
			base.Size = SmallDocumentBlock.CalcSize(value);
		}
	}

	public RootProperty()
		: base("Root Entry")
	{
		base.NodeColor = 1;
		base.PropertyType = 5;
		base.StartBlock = -2;
	}

	public RootProperty(int index, byte[] array, int offset)
		: base(index, array, offset)
	{
	}
}
