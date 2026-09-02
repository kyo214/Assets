using NPOI.OpenXmlFormats.Shared;
using NPOI.XWPF.UserModel;

namespace NPOI.XWPF.Usermodel;

public class XWPFOMathArg : MathContainer
{
	public XWPFOMathArg(IOMathContainer c, IRunBody p)
		: base(c, p)
	{
	}
}
