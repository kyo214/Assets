using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Dml.Spreadsheet;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing")]
public class CT_AnchorClientData
{
	private bool _fLocksWithSheet;

	private bool _fPrintsWithSheet;

	[XmlAttribute]
	public bool fLocksWithSheet
	{
		get
		{
			return _fLocksWithSheet;
		}
		set
		{
			_fLocksWithSheet = value;
		}
	}

	[XmlAttribute]
	public bool fPrintsWithSheet
	{
		get
		{
			return _fPrintsWithSheet;
		}
		set
		{
			_fPrintsWithSheet = value;
		}
	}

	public static CT_AnchorClientData Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_AnchorClientData
		{
			fLocksWithSheet = XmlHelper.ReadBool(node.Attributes["fLocksWithSheet"]),
			fPrintsWithSheet = XmlHelper.ReadBool(node.Attributes["fPrintsWithSheet"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<xdr:{nodeName}");
		XmlHelper.WriteAttribute(sw, "fLocksWithSheet", fLocksWithSheet, writeIfBlank: false);
		XmlHelper.WriteAttribute(sw, "fPrintsWithSheet", fPrintsWithSheet, writeIfBlank: false);
		sw.Write("/>");
	}
}
