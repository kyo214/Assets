using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Encryption;

[Serializable]
[GeneratedCode("System.Xml", "4.8.3761.0")]
[DesignerCategory("code")]
[XmlType(Namespace = "http://schemas.microsoft.com/office/2006/encryption")]
[XmlRoot(Namespace = "http://schemas.microsoft.com/office/2006/encryption", IsNullable = true)]
public class CT_KeyEncryptors
{
	private List<CT_KeyEncryptor> keyEncryptorField;

	[XmlElement("keyEncryptor", Order = 0)]
	public List<CT_KeyEncryptor> keyEncryptor
	{
		get
		{
			return keyEncryptorField;
		}
		set
		{
			keyEncryptorField = value;
		}
	}

	public CT_KeyEncryptors()
	{
		keyEncryptorField = new List<CT_KeyEncryptor>();
	}

	public CT_KeyEncryptor AddNewKeyEncryptor()
	{
		CT_KeyEncryptor cT_KeyEncryptor = new CT_KeyEncryptor();
		keyEncryptorField.Add(cT_KeyEncryptor);
		return cT_KeyEncryptor;
	}

	internal static CT_KeyEncryptors Parse(XmlNode node, XmlNamespaceManager nameSpaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_KeyEncryptors cT_KeyEncryptors = new CT_KeyEncryptors();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			cT_KeyEncryptors.keyEncryptorField.Add(CT_KeyEncryptor.Parse(childNode, nameSpaceManager));
		}
		return cT_KeyEncryptors;
	}
}
