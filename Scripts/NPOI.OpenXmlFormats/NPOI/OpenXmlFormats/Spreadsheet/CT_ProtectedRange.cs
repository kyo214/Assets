using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
public class CT_ProtectedRange
{
	private byte[] passwordField;

	private string sqrefField;

	private string nameField;

	private string securityDescriptorField;

	public byte[] password
	{
		get
		{
			return passwordField;
		}
		set
		{
			passwordField = value;
		}
	}

	public string sqref
	{
		get
		{
			return sqrefField;
		}
		set
		{
			sqrefField = value;
		}
	}

	public string name
	{
		get
		{
			return nameField;
		}
		set
		{
			nameField = value;
		}
	}

	public string securityDescriptor
	{
		get
		{
			return securityDescriptorField;
		}
		set
		{
			securityDescriptorField = value;
		}
	}

	public static CT_ProtectedRange Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_ProtectedRange
		{
			password = XmlHelper.ReadBytes(node.Attributes["password"]),
			name = XmlHelper.ReadString(node.Attributes["name"]),
			securityDescriptor = XmlHelper.ReadString(node.Attributes["securityDescriptor"]),
			sqref = XmlHelper.ReadString(node.Attributes["sqref"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "password", password);
		XmlHelper.WriteAttribute(sw, "name", name);
		XmlHelper.WriteAttribute(sw, "securityDescriptor", securityDescriptor);
		if (sqref != null)
		{
			XmlHelper.WriteAttribute(sw, "sqref", XmlHelper.EncodeXml(sqref));
		}
		sw.Write("/>");
	}
}
