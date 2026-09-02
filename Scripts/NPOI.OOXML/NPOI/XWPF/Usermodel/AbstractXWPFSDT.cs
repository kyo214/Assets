using NPOI.OpenXmlFormats.Wordprocessing;

namespace NPOI.XWPF.UserModel;

public abstract class AbstractXWPFSDT : ISDTContents
{
	private string title;

	private string tag;

	private IBody part;

	public abstract ISDTContent Content { get; }

	public AbstractXWPFSDT(CT_SdtPr pr, IBody part)
	{
		CT_String[] aliasArray = pr.GetAliasArray();
		if (aliasArray != null && aliasArray.Length != 0)
		{
			title = aliasArray[0].val;
		}
		else
		{
			title = "";
		}
		CT_String[] aliasArray2 = pr.GetAliasArray();
		if (aliasArray2 != null && aliasArray2.Length != 0)
		{
			tag = aliasArray2[0].val;
		}
		else
		{
			tag = "";
		}
		this.part = part;
	}

	public string GetTitle()
	{
		return title;
	}

	public string GetTag()
	{
		return tag;
	}

	public IBody GetBody()
	{
		return null;
	}

	public POIXMLDocumentPart GetPart()
	{
		return part.Part;
	}

	public BodyType GetPartType()
	{
		return BodyType.CONTENTCONTROL;
	}

	public BodyElementType GetElementType()
	{
		return BodyElementType.CONTENTCONTROL;
	}

	public XWPFDocument GetDocument()
	{
		return part.GetXWPFDocument();
	}
}
