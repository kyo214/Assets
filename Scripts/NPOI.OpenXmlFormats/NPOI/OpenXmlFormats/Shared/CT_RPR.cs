using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Shared;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/math")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/math", IsNullable = true)]
public class CT_RPR
{
	private CT_OnOff litField;

	private CT_OnOff norField;

	private CT_Script scrField;

	private CT_Style styField;

	private CT_ManualBreak brkField;

	private CT_OnOff alnField;

	[XmlElement(Order = 0)]
	public CT_OnOff lit
	{
		get
		{
			return litField;
		}
		set
		{
			litField = value;
		}
	}

	public CT_OnOff nor
	{
		get
		{
			return norField;
		}
		set
		{
			norField = value;
		}
	}

	public CT_Script scr
	{
		get
		{
			return scrField;
		}
		set
		{
			scrField = value;
		}
	}

	public CT_Style sty
	{
		get
		{
			return styField;
		}
		set
		{
			styField = value;
		}
	}

	[XmlElement(Order = 2)]
	public CT_ManualBreak brk
	{
		get
		{
			return brkField;
		}
		set
		{
			brkField = value;
		}
	}

	[XmlElement(Order = 3)]
	public CT_OnOff aln
	{
		get
		{
			return alnField;
		}
		set
		{
			alnField = value;
		}
	}

	internal static CT_RPR Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_RPR cT_RPR = new CT_RPR();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "lit")
			{
				cT_RPR.litField = CT_OnOff.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "nor")
			{
				cT_RPR.norField = CT_OnOff.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "aln")
			{
				cT_RPR.alnField = CT_OnOff.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "brk")
			{
				cT_RPR.brkField = CT_ManualBreak.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "scr")
			{
				cT_RPR.scrField = CT_Script.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "sty")
			{
				cT_RPR.styField = CT_Style.Parse(childNode, namespaceManager);
			}
		}
		return cT_RPR;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<m:{nodeName}>");
		if (litField != null)
		{
			litField.Write(sw, "lit");
		}
		if (norField != null)
		{
			norField.Write(sw, "nor");
		}
		if (scrField != null)
		{
			scrField.Write(sw, "scr");
		}
		if (styField != null)
		{
			styField.Write(sw, "sty");
		}
		if (brkField != null)
		{
			brkField.Write(sw, "brk");
		}
		if (alnField != null)
		{
			alnField.Write(sw, "aln");
		}
		sw.Write($"</m:{nodeName}>");
	}

	public bool IsSetI()
	{
		return litField != null;
	}

	public CT_OnOff AddNewI()
	{
		if (litField == null)
		{
			litField = new CT_OnOff();
		}
		return litField;
	}
}
