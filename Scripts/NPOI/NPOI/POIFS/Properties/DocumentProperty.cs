using NPOI.POIFS.FileSystem;

namespace NPOI.POIFS.Properties;

public class DocumentProperty : Property
{
	private OPOIFSDocument _document;

	public OPOIFSDocument Document
	{
		get
		{
			return _document;
		}
		set
		{
			_document = value;
		}
	}

	public override bool IsDirectory => false;

	public DocumentProperty(string name, int size)
	{
		_document = null;
		base.Name = name;
		Size = size;
		base.NodeColor = 1;
		base.PropertyType = 2;
	}

	public DocumentProperty(int index, byte[] array, int offset)
		: base(index, array, offset)
	{
		_document = null;
	}

	public override void PreWrite()
	{
	}

	public void UpdateSize(int size)
	{
		Size = size;
	}
}
