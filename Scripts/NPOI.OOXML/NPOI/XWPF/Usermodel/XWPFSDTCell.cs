using NPOI.OpenXmlFormats.Wordprocessing;

namespace NPOI.XWPF.UserModel;

public class XWPFSDTCell : AbstractXWPFSDT, ICell
{
	private XWPFSDTContentCell cellContent;

	public override ISDTContent Content => cellContent;

	public XWPFSDTCell(CT_SdtCell sdtCell, XWPFTableRow xwpfTableRow, IBody part)
		: base(sdtCell.sdtPr, part)
	{
		cellContent = new XWPFSDTContentCell(sdtCell.sdtContent, xwpfTableRow, part);
	}
}
