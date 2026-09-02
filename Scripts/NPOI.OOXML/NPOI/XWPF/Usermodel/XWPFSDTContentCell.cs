using System;
using System.IO;
using System.Text;
using System.Xml;
using NPOI.OpenXmlFormats.Wordprocessing;

namespace NPOI.XWPF.UserModel;

public class XWPFSDTContentCell : ISDTContent
{
	private string text = "";

	public string Text => text;

	public XWPFSDTContentCell(CT_SdtContentCell sdtContentCell, XWPFTableRow xwpfTableRow, IBody part)
	{
		StringBuilder stringBuilder = new StringBuilder();
		int num = 0;
		int num2 = 0;
		int num3 = 1;
		using (StringReader input = new StringReader(sdtContentCell.ToString()))
		{
			XmlParserContext inputContext = new XmlParserContext(null, POIXMLDocumentPart.NamespaceManager, null, XmlSpace.Preserve);
			using XmlReader xmlReader = XmlReader.Create(input, null, inputContext);
			while (xmlReader.Read() && num3 > 0)
			{
				if (xmlReader.NodeType == XmlNodeType.Text)
				{
					stringBuilder.Append(xmlReader.ReadContentAsString());
				}
				else if (IsStartToken(xmlReader, "tr"))
				{
					num = 0;
					num2 = 0;
				}
				else if (IsStartToken(xmlReader, "tc"))
				{
					if (num++ > 0)
					{
						stringBuilder.Append("\t");
					}
					num2 = 0;
				}
				else if (IsStartToken(xmlReader, "p") || IsStartToken(xmlReader, "tbl") || IsStartToken(xmlReader, "sdt"))
				{
					if (num2 > 0)
					{
						stringBuilder.Append("\n");
					}
					num2++;
				}
			}
		}
		text = stringBuilder.ToString();
	}

	private bool IsStartToken(XmlReader cursor, string string1)
	{
		if (!cursor.IsStartElement())
		{
			return false;
		}
		if (cursor.LocalName == string1)
		{
			return true;
		}
		return false;
	}

	private bool IsStartToken(object cursor, string string1)
	{
		throw new NotImplementedException();
	}

	public override string ToString()
	{
		return Text;
	}
}
