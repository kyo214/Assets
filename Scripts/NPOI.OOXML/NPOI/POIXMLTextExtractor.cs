using NPOI.OpenXml4Net.OPC;

namespace NPOI;

public abstract class POIXMLTextExtractor : POITextExtractor
{
	private POIXMLDocument _document;

	public POIXMLDocument Document => _document;

	public OPCPackage Package => _document.Package;

	public override POITextExtractor MetadataTextExtractor => new POIXMLPropertiesTextExtractor(_document);

	public POIXMLTextExtractor(POIXMLDocument document)
	{
		_document = document;
	}

	public CoreProperties GetCoreProperties()
	{
		return _document.GetProperties().CoreProperties;
	}

	public ExtendedProperties GetExtendedProperties()
	{
		return _document.GetProperties().ExtendedProperties;
	}

	public CustomProperties GetCustomProperties()
	{
		return _document.GetProperties().CustomProperties;
	}

	public override void Close()
	{
		if (_document != null)
		{
			_document.Package?.Revert();
		}
		base.Close();
	}
}
