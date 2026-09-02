using System.Text;

namespace NPOI.SS.Formula;

public class NameIdentifier
{
	private string _name;

	private bool _isQuoted;

	public string Name => _name;

	public bool IsQuoted => _isQuoted;

	public NameIdentifier(string name, bool isQuoted)
	{
		_name = name;
		_isQuoted = isQuoted;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder(64);
		stringBuilder.Append(GetType().Name);
		stringBuilder.Append(" [");
		if (_isQuoted)
		{
			stringBuilder.Append("'").Append(_name).Append("'");
		}
		else
		{
			stringBuilder.Append(_name);
		}
		stringBuilder.Append("]");
		return stringBuilder.ToString();
	}
}
