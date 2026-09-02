using NPOI.HSSF.Record;
using NPOI.HSSF.Record.Aggregates;
using NPOI.SS.UserModel;

namespace NPOI.HSSF.UserModel;

public class HSSFHeader : HeaderFooter, IHeader, IHeaderFooter
{
	private PageSettingsBlock _psb;

	public override string RawText
	{
		get
		{
			HeaderRecord header = _psb.Header;
			if (header == null)
			{
				return string.Empty;
			}
			return header.Text;
		}
	}

	public HSSFHeader(PageSettingsBlock psb)
	{
		_psb = psb;
	}

	protected override void SetHeaderFooterText(string text)
	{
		HeaderRecord header = _psb.Header;
		if (header == null)
		{
			header = new HeaderRecord(text);
			_psb.Header = header;
		}
		else
		{
			header.Text = text;
		}
	}
}
