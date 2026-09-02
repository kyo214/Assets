using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.OPC.Internal;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
public class CT_Fill
{
	private CT_PatternFill patternFillField;

	private CT_GradientFill gradientFillField;

	[XmlElement]
	public CT_PatternFill patternFill
	{
		get
		{
			return patternFillField;
		}
		set
		{
			patternFillField = value;
		}
	}

	[XmlElement]
	public CT_GradientFill gradientFill
	{
		get
		{
			return gradientFillField;
		}
		set
		{
			gradientFillField = value;
		}
	}

	public CT_PatternFill GetPatternFill()
	{
		return patternFillField;
	}

	public CT_PatternFill AddNewPatternFill()
	{
		patternFillField = new CT_PatternFill();
		return GetPatternFill();
	}

	public bool IsSetPatternFill()
	{
		return patternFillField != null;
	}

	public CT_Fill Copy()
	{
		return Parse(ToString());
	}

	public static CT_Fill Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_Fill cT_Fill = new CT_Fill();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "patternFill")
			{
				cT_Fill.patternFill = CT_PatternFill.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "gradientFill")
			{
				cT_Fill.gradientFill = CT_GradientFill.Parse(childNode, namespaceManager);
			}
		}
		return cT_Fill;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		sw.Write(">");
		if (patternFill != null)
		{
			patternFill.Write(sw, "patternFill");
		}
		if (gradientFill != null)
		{
			gradientFill.Write(sw, "gradientFill");
		}
		sw.Write($"</{nodeName}>");
	}

	public override string ToString()
	{
		using MemoryStream memoryStream = new MemoryStream();
		StreamWriter streamWriter = new StreamWriter(memoryStream);
		Write(streamWriter, "fill");
		streamWriter.Flush();
		memoryStream.Position = 0L;
		return new StreamReader(memoryStream).ReadToEnd();
	}

	public static CT_Fill Parse(string p)
	{
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(p);
		return Parse(xmlDocument.DocumentElement, CreateDefaultNSM());
	}

	public static XmlNamespaceManager CreateDefaultNSM()
	{
		XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(new NameTable());
		xmlNamespaceManager.AddNamespace(string.Empty, "http://schemas.openxmlformats.org/spreadsheetml/2006/main");
		xmlNamespaceManager.AddNamespace("d", "http://schemas.openxmlformats.org/spreadsheetml/2006/main");
		xmlNamespaceManager.AddNamespace("a", "http://schemas.openxmlformats.org/drawingml/2006/main");
		xmlNamespaceManager.AddNamespace("xdr", "http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing");
		xmlNamespaceManager.AddNamespace("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
		xmlNamespaceManager.AddNamespace("c", "http://schemas.openxmlformats.org/drawingml/2006/chart");
		xmlNamespaceManager.AddNamespace("vt", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes");
		xmlNamespaceManager.AddNamespace("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
		xmlNamespaceManager.AddNamespace("wp", "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing");
		xmlNamespaceManager.AddNamespace("m", "http://schemas.openxmlformats.org/officeDocument/2006/math");
		xmlNamespaceManager.AddNamespace("ve", "http://schemas.openxmlformats.org/markup-compatibility/2006");
		xmlNamespaceManager.AddNamespace("o", "urn:schemas-microsoft-com:office:office");
		xmlNamespaceManager.AddNamespace("v", "urn:schemas-microsoft-com:vml");
		xmlNamespaceManager.AddNamespace("wne", "http://schemas.microsoft.com/office/word/2006/wordml");
		xmlNamespaceManager.AddNamespace("xp", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/extended-properties");
		xmlNamespaceManager.AddNamespace("ctp", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/custom-properties");
		xmlNamespaceManager.AddNamespace("cp", PackagePropertiesPart.NAMESPACE_CP_URI);
		xmlNamespaceManager.AddNamespace("dc", PackagePropertiesPart.NAMESPACE_DC_URI);
		xmlNamespaceManager.AddNamespace("dcterms", PackagePropertiesPart.NAMESPACE_DCTERMS_URI);
		xmlNamespaceManager.AddNamespace("dcmitype", "http://purl.org/dc/dcmitype/");
		xmlNamespaceManager.AddNamespace("xsi", PackagePropertiesPart.NAMESPACE_XSI_URI);
		xmlNamespaceManager.AddNamespace("xsd", "http://www.w3.org/2001/XMLSchema");
		return xmlNamespaceManager;
	}
}
