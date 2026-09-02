using NPOI.OpenXmlFormats.Shared;
using NPOI.XWPF.UserModel;

namespace NPOI.XWPF.Usermodel;

public class XWPFRad : IRunBody
{
	private CT_Rad rad;

	private IRunBody parent;

	private XWPFOMathArg deg;

	private XWPFOMathArg e;

	public XWPFDocument Document => parent.Document;

	public POIXMLDocumentPart Part => parent.Part;

	public XWPFOMathArg Degree => deg;

	public XWPFOMathArg Element => e;

	public XWPFRad(CT_Rad rad, IRunBody p)
	{
		this.rad = rad;
		parent = p;
		if (rad.radPr == null)
		{
			rad.radPr = new CT_RadPr();
		}
		if (rad.deg == null)
		{
			rad.deg = new CT_OMathArg();
		}
		deg = new XWPFOMathArg(rad.deg, this);
		if (rad.e == null)
		{
			rad.e = new CT_OMathArg();
		}
		e = new XWPFOMathArg(rad.e, this);
	}
}
