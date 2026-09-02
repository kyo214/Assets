namespace System.Xml.Xsl.Xslt;

internal struct Pattern(TemplateMatch match, int priority)
{
	public readonly TemplateMatch Match = match;

	public readonly int Priority = priority;
}
