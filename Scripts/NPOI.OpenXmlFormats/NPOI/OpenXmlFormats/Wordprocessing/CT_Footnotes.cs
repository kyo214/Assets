using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

public class CT_Footnotes
{
	private List<CT_FtnEdn> footnoteField;

	[XmlElement("footnote", Order = 0)]
	public List<CT_FtnEdn> footnote
	{
		get
		{
			return footnoteField;
		}
		set
		{
			footnoteField = value;
		}
	}

	public static CT_Footnotes Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_Footnotes cT_Footnotes = new CT_Footnotes();
		cT_Footnotes.footnote = new List<CT_FtnEdn>();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "footnote")
			{
				cT_Footnotes.footnote.Add(CT_FtnEdn.Parse(childNode, namespaceManager));
			}
		}
		return cT_Footnotes;
	}

	internal void Write(StreamWriter sw)
	{
		sw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>");
		sw.Write("<w:footnotes xmlns:wpc=\"http://schemas.microsoft.com/office/word/2010/wordprocessingCanvas\" xmlns:mc=\"http://schemas.openxmlformats.org/markup-compatibility/2006\" ");
		sw.Write("xmlns:o=\"urn:schemas-microsoft-com:office:office\" ");
		sw.Write("xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\" xmlns:m=\"http://schemas.openxmlformats.org/officeDocument/2006/math\" ");
		sw.Write("xmlns:v=\"urn:schemas-microsoft-com:vml\" xmlns:wp14=\"http://schemas.microsoft.com/office/word/2010/wordprocessingDrawing\" ");
		sw.Write("xmlns:wp=\"http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing\" xmlns:w=\"http://schemas.openxmlformats.org/wordprocessingml/2006/main\" ");
		sw.Write("xmlns:w14=\"http://schemas.microsoft.com/office/word/2010/wordml\" xmlns:w15=\"http://schemas.microsoft.com/office/word/2012/wordml\" xmlns:w10=\"urn:schemas-microsoft-com:office:word\" ");
		sw.Write("xmlns:wpg=\"http://schemas.microsoft.com/office/word/2010/wordprocessingGroup\" xmlns:wpi=\"http://schemas.microsoft.com/office/word/2010/wordprocessingInk\" ");
		sw.Write("xmlns:wne=\"http://schemas.microsoft.com/office/word/2006/wordml\" xmlns:wps=\"http://schemas.microsoft.com/office/word/2010/wordprocessingShape\" ");
		sw.Write("mc:Ignorable=\"w14 w15 wp14\">");
		if (footnote != null)
		{
			foreach (CT_FtnEdn item in footnote)
			{
				item.Write(sw, "footnote");
			}
		}
		sw.Write("</w:footnotes>");
	}

	public CT_FtnEdn AddNewFootnote()
	{
		CT_FtnEdn cT_FtnEdn = new CT_FtnEdn();
		if (footnoteField == null)
		{
			footnoteField = new List<CT_FtnEdn>();
		}
		footnoteField.Add(cT_FtnEdn);
		return cT_FtnEdn;
	}
}
