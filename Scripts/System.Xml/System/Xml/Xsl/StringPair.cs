namespace System.Xml.Xsl;

internal struct StringPair(string left, string right)
{
	private string left = left;

	private string right = right;

	public string Left => left;

	public string Right => right;
}
