using NPOI.HSSF.UserModel;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel.Helpers;

namespace NPOI.XSSF.UserModel.Extensions;

public abstract class XSSFHeaderFooter : IHeaderFooter
{
	private HeaderFooterHelper helper;

	private CT_HeaderFooter headerFooter;

	private bool stripFields;

	public abstract string Text { get; set; }

	public string Center
	{
		get
		{
			string centerSection = helper.GetCenterSection(Text);
			if (stripFields)
			{
				return StripFields(centerSection);
			}
			return centerSection;
		}
		set
		{
			Text = helper.SetCenterSection(Text, value);
		}
	}

	public string Left
	{
		get
		{
			string leftSection = helper.GetLeftSection(Text);
			if (stripFields)
			{
				return StripFields(leftSection);
			}
			return leftSection;
		}
		set
		{
			Text = helper.SetLeftSection(Text, value);
		}
	}

	public string Right
	{
		get
		{
			string rightSection = helper.GetRightSection(Text);
			if (stripFields)
			{
				return StripFields(rightSection);
			}
			return rightSection;
		}
		set
		{
			Text = helper.SetRightSection(Text, value);
		}
	}

	public XSSFHeaderFooter(CT_HeaderFooter headerFooter)
	{
		this.headerFooter = headerFooter;
		helper = new HeaderFooterHelper();
	}

	public CT_HeaderFooter GetHeaderFooter()
	{
		return headerFooter;
	}

	public string GetValue()
	{
		string text = Text;
		if (text == null)
		{
			return "";
		}
		return text;
	}

	public bool AreFieldsStripped()
	{
		return stripFields;
	}

	public void SetAreFieldsStripped(bool stripFields)
	{
		this.stripFields = stripFields;
	}

	public static string StripFields(string text)
	{
		return HeaderFooter.StripFields(text);
	}
}
