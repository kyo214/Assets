using System.IO;
using System.Xml;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

public class CT_GradientStop
{
	private int positionField;

	private CT_Color colorField;

	public static CT_GradientStop Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_GradientStop cT_GradientStop = new CT_GradientStop();
		cT_GradientStop.positionField = XmlHelper.ReadInt(node.Attributes["position"]);
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "color")
			{
				cT_GradientStop.colorField = CT_Color.Parse(childNode, namespaceManager);
				break;
			}
		}
		return cT_GradientStop;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "position", positionField, writeIfBlank: true);
		sw.Write(">");
		if (colorField != null)
		{
			colorField.Write(sw, "color");
		}
		sw.Write($"</{nodeName}>");
	}
}
