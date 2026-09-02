using System.Collections;
using System.Xml.XPath;

namespace System.Xml.Xsl.XsltOld;

internal struct DocumentKeyList(XPathNavigator rootNav, Hashtable keyTable)
{
	private XPathNavigator rootNav = rootNav;

	private Hashtable keyTable = keyTable;

	public XPathNavigator RootNav => rootNav;

	public Hashtable KeyTable => keyTable;
}
