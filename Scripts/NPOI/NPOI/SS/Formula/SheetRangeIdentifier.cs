using System.Text;

namespace NPOI.SS.Formula;

public class SheetRangeIdentifier : SheetIdentifier
{
	public NameIdentifier _lastSheetIdentifier;

	public NameIdentifier FirstSheetIdentifier => base.SheetId;

	public NameIdentifier LastSheetIdentifier => _lastSheetIdentifier;

	public SheetRangeIdentifier(string bookName, NameIdentifier firstSheetIdentifier, NameIdentifier lastSheetIdentifier)
		: base(bookName, firstSheetIdentifier)
	{
		_lastSheetIdentifier = lastSheetIdentifier;
	}

	protected override void AsFormulaString(StringBuilder sb)
	{
		base.AsFormulaString(sb);
		sb.Append(':');
		if (_lastSheetIdentifier.IsQuoted)
		{
			sb.Append("'").Append(_lastSheetIdentifier.Name).Append("'");
		}
		else
		{
			sb.Append(_lastSheetIdentifier.Name);
		}
	}
}
