using System.Diagnostics;
using System.Xml.Xsl.Qil;
using System.Xml.Xsl.XPath;

namespace System.Xml.Xsl.Xslt;

internal struct SingletonFocus(XPathQilFactory f) : IFocus
{
	private XPathQilFactory f = f;

	private SingletonFocusType focusType = SingletonFocusType.None;

	private QilIterator current = null;

	public void SetFocus(SingletonFocusType focusType)
	{
		this.focusType = focusType;
	}

	public void SetFocus(QilIterator current)
	{
		if (current != null)
		{
			focusType = SingletonFocusType.Iterator;
			this.current = current;
		}
		else
		{
			focusType = SingletonFocusType.None;
			this.current = null;
		}
	}

	[Conditional("DEBUG")]
	private void CheckFocus()
	{
	}

	public QilNode GetCurrent()
	{
		return focusType switch
		{
			SingletonFocusType.InitialDocumentNode => f.Root(f.XmlContext()), 
			SingletonFocusType.InitialContextNode => f.XmlContext(), 
			_ => current, 
		};
	}

	public QilNode GetPosition()
	{
		return f.Double(1.0);
	}

	public QilNode GetLast()
	{
		return f.Double(1.0);
	}
}
