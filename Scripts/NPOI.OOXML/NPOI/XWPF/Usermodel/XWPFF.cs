using NPOI.OpenXmlFormats.Shared;
using NPOI.XWPF.UserModel;

namespace NPOI.XWPF.Usermodel;

public class XWPFF : IRunBody
{
	private CT_F f;

	private IRunBody parent;

	private XWPFOMathArg num;

	private XWPFOMathArg den;

	public ST_FType FractionType
	{
		get
		{
			return f.fPr.type.val;
		}
		set
		{
			f.fPr.type.val = value;
		}
	}

	public XWPFDocument Document => parent.Document;

	public POIXMLDocumentPart Part => parent.Part;

	public XWPFOMathArg Numerator => num;

	public XWPFOMathArg Denominator => den;

	public XWPFF(CT_F f, IRunBody p)
	{
		this.f = f;
		parent = p;
		if (f.fPr == null)
		{
			f.fPr = new CT_FPr();
		}
		if (f.fPr.type == null)
		{
			f.fPr.type = new CT_FType();
		}
		if (f.num == null)
		{
			f.num = new CT_OMathArg();
		}
		num = new XWPFOMathArg(f.num, this);
		if (f.den == null)
		{
			f.den = new CT_OMathArg();
		}
		den = new XWPFOMathArg(f.den, this);
	}
}
