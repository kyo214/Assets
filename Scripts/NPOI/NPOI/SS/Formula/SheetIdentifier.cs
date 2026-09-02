using System.Text;

namespace NPOI.SS.Formula;

public class SheetIdentifier
{
	public string _bookName;

	public NameIdentifier _sheetIdentifier;

	public string BookName => _bookName;

	public NameIdentifier SheetId => _sheetIdentifier;

	public SheetIdentifier(string bookName, NameIdentifier sheetIdentifier)
	{
		_bookName = bookName;
		_sheetIdentifier = sheetIdentifier;
	}

	protected virtual void AsFormulaString(StringBuilder sb)
	{
		if (_bookName != null)
		{
			sb.Append(" [").Append(_sheetIdentifier.Name).Append("]");
		}
		if (_sheetIdentifier.IsQuoted)
		{
			sb.Append("'").Append(_sheetIdentifier.Name).Append("'");
		}
		else
		{
			sb.Append(_sheetIdentifier.Name);
		}
	}

	public string AsFormulaString()
	{
		StringBuilder stringBuilder = new StringBuilder(32);
		AsFormulaString(stringBuilder);
		return stringBuilder.ToString();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder(64);
		stringBuilder.Append(GetType().Name);
		stringBuilder.Append(" [");
		AsFormulaString(stringBuilder);
		stringBuilder.Append("]");
		return stringBuilder.ToString();
	}
}
