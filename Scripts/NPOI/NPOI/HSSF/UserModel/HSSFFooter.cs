using NPOI.HSSF.Record;
using NPOI.HSSF.Record.Aggregates;
using NPOI.SS.UserModel;

namespace NPOI.HSSF.UserModel;

public class HSSFFooter : HeaderFooter, IFooter, IHeaderFooter
{
	private PageSettingsBlock _psb;

	public override string RawText
	{
		get
		{
			FooterRecord footer = _psb.Footer;
			if (footer == null)
			{
				return string.Empty;
			}
			return footer.Text;
		}
	}

	public HSSFFooter(PageSettingsBlock psb)
	{
		_psb = psb;
	}

	protected override void SetHeaderFooterText(string text)
	{
		FooterRecord footer = _psb.Footer;
		if (footer == null)
		{
			footer = new FooterRecord(text);
			_psb.Footer = footer;
		}
		else
		{
			footer.Text = text;
		}
	}
}
