using NPOI.OpenXmlFormats.Shared;
using NPOI.XWPF.Usermodel;

namespace NPOI.XWPF.UserModel;

public class XWPFOMath : MathContainer
{
	protected CT_OMath oMath => (CT_OMath)container;

	public XWPFOMath(CT_OMath oMath, IRunBody p)
		: base(oMath, p)
	{
	}
}
