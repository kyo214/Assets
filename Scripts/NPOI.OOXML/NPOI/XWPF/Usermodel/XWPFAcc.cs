using NPOI.OpenXmlFormats.Shared;
using NPOI.XWPF.UserModel;

namespace NPOI.XWPF.Usermodel;

public class XWPFAcc : IRunBody
{
	private CT_Acc acc;

	private IRunBody parent;

	private readonly XWPFOMathArg e;

	public string AccPr
	{
		get
		{
			return acc.accPr.chr.val;
		}
		set
		{
			acc.accPr.chr.val = value;
		}
	}

	public XWPFOMathArg Element => e;

	public XWPFDocument Document => parent.Document;

	public POIXMLDocumentPart Part => parent.Part;

	public XWPFAcc(CT_Acc acc, IRunBody p)
	{
		this.acc = acc;
		parent = p;
		if (acc.e == null)
		{
			acc.e = new CT_OMathArg();
		}
		e = new XWPFOMathArg(acc.e, this);
		if (acc.accPr == null)
		{
			acc.accPr = new CT_AccPr();
		}
		if (acc.accPr.chr == null)
		{
			acc.accPr.chr = new CT_Char();
		}
	}
}
