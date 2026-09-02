using System;
using NPOI.OpenXmlFormats.Dml;

namespace NPOI.XSSF.UserModel;

public class XSSFLineBreak : XSSFTextRun
{
	private CT_TextCharacterProperties _brProps;

	public XSSFLineBreak(CT_RegularTextRun r, XSSFTextParagraph p, CT_TextCharacterProperties brProps)
		: base(r, p)
	{
		_brProps = brProps;
	}

	protected new CT_TextCharacterProperties GetRPr()
	{
		return _brProps;
	}

	public void SetText(string text)
	{
		throw new InvalidOperationException("You cannot change text of a line break, it is always '\\n'");
	}
}
