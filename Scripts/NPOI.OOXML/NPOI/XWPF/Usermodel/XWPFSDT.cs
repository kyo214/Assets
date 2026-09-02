using NPOI.OpenXmlFormats.Wordprocessing;

namespace NPOI.XWPF.UserModel;

public class XWPFSDT : AbstractXWPFSDT, IBodyElement, IRunBody, ISDTContents, IRunElement
{
	private ISDTContent content;

	public override ISDTContent Content => content;

	public XWPFDocument Document => GetDocument();

	public POIXMLDocumentPart Part => GetPart();

	public IBody Body => GetBody();

	public BodyType PartType => GetPartType();

	public BodyElementType ElementType => GetElementType();

	public XWPFSDT(CT_SdtRun sdtRun, IBody part)
		: base(sdtRun.sdtPr, part)
	{
		content = new XWPFSDTContent(sdtRun.sdtContent, part, this);
	}

	public XWPFSDT(CT_SdtBlock block, IBody part)
		: base(block.sdtPr, part)
	{
		content = new XWPFSDTContent(block.sdtContent, part, this);
	}
}
