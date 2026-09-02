using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main", IsNullable = true)]
public class CT_FFTextInput
{
	private CT_FFTextType typeField;

	private CT_String defaultField;

	private CT_DecimalNumber maxLengthField;

	private CT_String formatField;

	[XmlElement(Order = 0)]
	public CT_FFTextType type
	{
		get
		{
			return typeField;
		}
		set
		{
			typeField = value;
		}
	}

	[XmlElement(Order = 1)]
	public CT_String @default
	{
		get
		{
			return defaultField;
		}
		set
		{
			defaultField = value;
		}
	}

	[XmlElement(Order = 2)]
	public CT_DecimalNumber maxLength
	{
		get
		{
			return maxLengthField;
		}
		set
		{
			maxLengthField = value;
		}
	}

	[XmlElement(Order = 3)]
	public CT_String format
	{
		get
		{
			return formatField;
		}
		set
		{
			formatField = value;
		}
	}

	public CT_FFTextInput()
	{
		formatField = new CT_String();
		maxLengthField = new CT_DecimalNumber();
		defaultField = new CT_String();
		typeField = new CT_FFTextType();
	}

	public static CT_FFTextInput Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_FFTextInput cT_FFTextInput = new CT_FFTextInput();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "type")
			{
				cT_FFTextInput.typeField = CT_FFTextType.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "default")
			{
				cT_FFTextInput.defaultField = CT_String.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "format")
			{
				cT_FFTextInput.formatField = CT_String.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "maxLength")
			{
				cT_FFTextInput.maxLengthField = CT_DecimalNumber.Parse(childNode, namespaceManager);
			}
		}
		return cT_FFTextInput;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<w:{nodeName}>");
		if (typeField == null)
		{
			typeField.Write(sw, "type");
		}
		if (defaultField != null)
		{
			defaultField.Write(sw, "default");
		}
		if (formatField != null)
		{
			formatField.Write(sw, "format");
		}
		if (maxLengthField != null)
		{
			maxLengthField.Write(sw, "maxLength");
		}
		sw.Write($"</w:{nodeName}>");
	}
}
