using NPOI.OpenXmlFormats.Shared;
using NPOI.XWPF.UserModel;

namespace NPOI.XWPF.Usermodel;

public class XWPFNary : IRunBody
{
	private CT_Nary nary;

	private IRunBody parent;

	private readonly XWPFOMathArg e;

	private readonly XWPFOMathArg sub;

	private readonly XWPFOMathArg sup;

	public XWPFDocument Document => parent.Document;

	public POIXMLDocumentPart Part => parent.Part;

	public XWPFOMathArg Element => e;

	public XWPFOMathArg Subscript => sub;

	public XWPFOMathArg Superscript => sup;

	public string NaryPr => nary.naryPr.chr.val;

	public XWPFNary(CT_Nary nary, IRunBody p)
	{
		this.nary = nary;
		parent = p;
		if (nary.e == null)
		{
			nary.e = new CT_OMathArg();
		}
		e = new XWPFOMathArg(nary.e, this);
		if (nary.sub == null)
		{
			nary.sub = new CT_OMathArg();
		}
		sub = new XWPFOMathArg(nary.sub, this);
		if (nary.sup == null)
		{
			nary.sup = new CT_OMathArg();
		}
		sup = new XWPFOMathArg(nary.sup, this);
		if (nary.naryPr == null)
		{
			nary.naryPr = new CT_NaryPr();
		}
		if (nary.naryPr.chr == null)
		{
			nary.naryPr.chr = new CT_Char();
		}
	}

	public XWPFNary SetSumm()
	{
		nary.naryPr.chr.val = "∑";
		return this;
	}

	public XWPFNary SetUnion()
	{
		nary.naryPr.chr.val = "⋃";
		return this;
	}

	public XWPFNary SetIntegral()
	{
		nary.naryPr.chr.val = "∫";
		return this;
	}

	public XWPFNary SetAnd()
	{
		nary.naryPr.chr.val = "⋀";
		return this;
	}
}
