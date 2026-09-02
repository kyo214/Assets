namespace MS.Internal.Xml.Cache;

internal struct XPathNodeRef(XPathNode[] page, int idx)
{
	private XPathNode[] _page = page;

	private int _idx = idx;

	public XPathNode[] Page => _page;

	public int Index => _idx;

	public override int GetHashCode()
	{
		return XPathNodeHelper.GetLocation(_page, _idx);
	}
}
