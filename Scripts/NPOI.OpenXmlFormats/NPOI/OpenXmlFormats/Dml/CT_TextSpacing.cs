using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml;

[Serializable]
[DebuggerStepThrough]
[DesignerCategory("code")]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main", IsNullable = true)]
public class CT_TextSpacing
{
	private CT_TextSpacingPercent spcPctField;

	private CT_TextSpacingPoint spcPtsField;

	public CT_TextSpacingPercent spcPct
	{
		get
		{
			return spcPctField;
		}
		set
		{
			spcPctField = value;
		}
	}

	public CT_TextSpacingPoint spcPts
	{
		get
		{
			return spcPtsField;
		}
		set
		{
			spcPtsField = value;
		}
	}

	public static CT_TextSpacing Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_TextSpacing cT_TextSpacing = new CT_TextSpacing();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "spcPct")
			{
				cT_TextSpacing.spcPct = CT_TextSpacingPercent.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "spcPts")
			{
				cT_TextSpacing.spcPts = CT_TextSpacingPoint.Parse(childNode, namespaceManager);
			}
		}
		return cT_TextSpacing;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<a:{nodeName}>");
		if (spcPct != null)
		{
			spcPct.Write(sw, "spcPct");
		}
		if (spcPts != null)
		{
			spcPts.Write(sw, "spcPts");
		}
		sw.Write($"</a:{nodeName}>");
	}

	public CT_TextSpacingPercent AddNewSpcPct()
	{
		spcPctField = new CT_TextSpacingPercent();
		return spcPctField;
	}

	public CT_TextSpacingPoint AddNewSpcPts()
	{
		spcPtsField = new CT_TextSpacingPoint();
		return spcPtsField;
	}

	public bool IsSetSpcPct()
	{
		return spcPctField != null;
	}

	public bool IsSetSpcPts()
	{
		return spcPtsField != null;
	}
}
