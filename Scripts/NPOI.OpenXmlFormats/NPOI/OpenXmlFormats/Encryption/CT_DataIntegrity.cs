using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Encryption;

[Serializable]
[GeneratedCode("System.Xml", "4.8.3761.0")]
[DesignerCategory("code")]
[XmlType(Namespace = "http://schemas.microsoft.com/office/2006/encryption")]
[XmlRoot(Namespace = "http://schemas.microsoft.com/office/2006/encryption", IsNullable = true)]
public class CT_DataIntegrity
{
	private byte[] encryptedHmacKeyField;

	private byte[] encryptedHmacValueField;

	[XmlAttribute(DataType = "base64Binary")]
	public byte[] encryptedHmacKey
	{
		get
		{
			return encryptedHmacKeyField;
		}
		set
		{
			encryptedHmacKeyField = value;
		}
	}

	[XmlAttribute(DataType = "base64Binary")]
	public byte[] encryptedHmacValue
	{
		get
		{
			return encryptedHmacValueField;
		}
		set
		{
			encryptedHmacValueField = value;
		}
	}

	internal static CT_DataIntegrity Parse(XmlNode node, XmlNamespaceManager nameSpaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_DataIntegrity cT_DataIntegrity = new CT_DataIntegrity();
		if (node.Attributes["encryptedHmacKey"] != null)
		{
			cT_DataIntegrity.encryptedHmacKey = Convert.FromBase64String(node.Attributes["encryptedHmacKey"].Value);
		}
		if (node.Attributes["encryptedHmacValue"] != null)
		{
			cT_DataIntegrity.encryptedHmacValue = Convert.FromBase64String(node.Attributes["encryptedHmacValue"].Value);
		}
		return cT_DataIntegrity;
	}
}
