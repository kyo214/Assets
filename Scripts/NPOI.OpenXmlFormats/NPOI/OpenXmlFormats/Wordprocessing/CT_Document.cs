using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
[XmlRoot("document", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main", IsNullable = false)]
public class CT_Document : CT_DocumentBase
{
	private CT_Body bodyField;

	[XmlElement(Order = 0)]
	public CT_Body body
	{
		get
		{
			return bodyField;
		}
		set
		{
			bodyField = value;
		}
	}

	public static CT_Document Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_Document cT_Document = new CT_Document();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "body")
			{
				cT_Document.body = CT_Body.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "background")
			{
				cT_Document.background = CT_Background.Parse(childNode, namespaceManager);
			}
		}
		return cT_Document;
	}

	internal void Write(StreamWriter sw)
	{
		sw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>");
		sw.Write("<w:document xmlns:wpc=\"http://schemas.microsoft.com/office/word/2010/wordprocessingCanvas\" ");
		sw.Write("xmlns:cx=\"http://schemas.microsoft.com/office/drawing/2014/chartex\" xmlns:cx1=\"http://schemas.microsoft.com/office/drawing/2015/9/8/chartex\" xmlns:cx2=\"http://schemas.microsoft.com/office/drawing/2015/10/21/chartex\" xmlns:cx3=\"http://schemas.microsoft.com/office/drawing/2016/5/9/chartex\" ");
		sw.Write("xmlns:cx4=\"http://schemas.microsoft.com/office/drawing/2016/5/10/chartex\" xmlns:cx5=\"http://schemas.microsoft.com/office/drawing/2016/5/11/chartex\" xmlns:cx6=\"http://schemas.microsoft.com/office/drawing/2016/5/12/chartex\" ");
		sw.Write("xmlns:cx7=\"http://schemas.microsoft.com/office/drawing/2016/5/13/chartex\" xmlns:cx8=\"http://schemas.microsoft.com/office/drawing/2016/5/14/chartex\" ");
		sw.Write("xmlns:mc=\"http://schemas.openxmlformats.org/markup-compatibility/2006\" ");
		sw.Write("xmlns:aink=\"http://schemas.microsoft.com/office/drawing/2016/ink\" xmlns:am3d=\"http://schemas.microsoft.com/office/drawing/2017/model3d\" ");
		sw.Write("xmlns:o=\"urn:schemas-microsoft-com:office:office\" ");
		sw.Write("xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\" xmlns:m=\"http://schemas.openxmlformats.org/officeDocument/2006/math\" ");
		sw.Write("xmlns:v=\"urn:schemas-microsoft-com:vml\" xmlns:wp14=\"http://schemas.microsoft.com/office/word/2010/wordprocessingDrawing\" xmlns:wp=\"http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing\" ");
		sw.Write("xmlns:w10=\"urn:schemas-microsoft-com:office:word\" xmlns:w=\"http://schemas.openxmlformats.org/wordprocessingml/2006/main\" ");
		sw.Write("xmlns:w14=\"http://schemas.microsoft.com/office/word/2010/wordml\" xmlns:w15=\"http://schemas.microsoft.com/office/word/2012/wordml\" xmlns:w16cex=\"http://schemas.microsoft.com/office/word/2018/wordml/cex\" ");
		sw.Write("xmlns:w16cid=\"http://schemas.microsoft.com/office/word/2016/wordml/cid\" xmlns:w16=\"http://schemas.microsoft.com/office/word/2018/wordml\" xmlns:w16sdtdh=\"http://schemas.microsoft.com/office/word/2020/wordml/sdtdatahash\" ");
		sw.Write("xmlns:w16se=\"http://schemas.microsoft.com/office/word/2015/wordml/symex\" xmlns:wpg=\"http://schemas.microsoft.com/office/word/2010/wordprocessingGroup\" xmlns:wpi=\"http://schemas.microsoft.com/office/word/2010/wordprocessingInk\" ");
		sw.Write("xmlns:wne=\"http://schemas.microsoft.com/office/word/2006/wordml\" xmlns:wps=\"http://schemas.microsoft.com/office/word/2010/wordprocessingShape\" ");
		sw.Write("mc:Ignorable=\"w14 w15 w16se w16cid w16 w16cex w16sdtdh wp14\">");
		if (body != null)
		{
			body.Write(sw, "body");
		}
		if (base.background != null)
		{
			base.background.Write(sw, "background");
		}
		sw.Write("</w:document>");
	}

	public CT_Document()
	{
		bodyField = new CT_Body();
	}

	public void AddNewBody()
	{
		bodyField = new CT_Body();
	}
}
