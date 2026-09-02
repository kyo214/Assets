using NPOI.OpenXmlFormats.Shared;
using NPOI.XWPF.UserModel;

namespace NPOI.XWPF.Usermodel;

public class XWPFSSup : IRunBody
{
	private CT_SSup ssup;

	private IRunBody parent;

	private readonly XWPFOMathArg e;

	private readonly XWPFOMathArg sup;

	public XWPFDocument Document => parent.Document;

	public POIXMLDocumentPart Part => parent.Part;

	public XWPFOMathArg Element => e;

	public XWPFOMathArg Superscript => sup;

	public XWPFSSup(CT_SSup ssup, IRunBody p)
	{
		this.ssup = ssup;
		parent = p;
		if (ssup.e == null)
		{
			ssup.e = new CT_OMathArg();
		}
		e = new XWPFOMathArg(ssup.e, this);
		if (ssup.sup == null)
		{
			ssup.sup = new CT_OMathArg();
		}
		sup = new XWPFOMathArg(ssup.sup, this);
	}
}
