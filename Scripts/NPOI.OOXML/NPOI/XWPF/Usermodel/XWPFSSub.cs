using NPOI.OpenXmlFormats.Shared;
using NPOI.XWPF.UserModel;

namespace NPOI.XWPF.Usermodel;

public class XWPFSSub : IRunBody
{
	private CT_SSub ssub;

	private IRunBody parent;

	private readonly XWPFOMathArg e;

	private readonly XWPFOMathArg sub;

	public XWPFDocument Document => parent.Document;

	public POIXMLDocumentPart Part => parent.Part;

	public XWPFOMathArg Element => e;

	public XWPFOMathArg Subscript => sub;

	public XWPFSSub(CT_SSub ssub, IRunBody p)
	{
		this.ssub = ssub;
		parent = p;
		if (ssub.e == null)
		{
			ssub.e = new CT_OMathArg();
		}
		e = new XWPFOMathArg(ssub.e, this);
		if (ssub.sub == null)
		{
			ssub.sub = new CT_OMathArg();
		}
		sub = new XWPFOMathArg(ssub.sub, this);
	}
}
