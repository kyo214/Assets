using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Encryption;

[Serializable]
[GeneratedCode("System.Xml", "4.8.3761.0")]
[DesignerCategory("code")]
[XmlType(Namespace = "http://schemas.microsoft.com/office/2006/encryption")]
[XmlRoot(Namespace = "http://schemas.microsoft.com/office/2006/encryption", IsNullable = true)]
public class CT_KeyEncryptor
{
	private object itemField;

	private CT_KeyEncryptorUri uriField;

	private bool uriFieldSpecified;

	[XmlElement("encryptedKey", typeof(CT_CertificateKeyEncryptor), Namespace = "http://schemas.microsoft.com/office/2006/keyEncryptor/certificate", Order = 0)]
	[XmlElement("encryptedKey", typeof(CT_PasswordKeyEncryptor), Namespace = "http://schemas.microsoft.com/office/2006/keyEncryptor/password", Order = 0)]
	public object Item
	{
		get
		{
			return itemField;
		}
		set
		{
			itemField = value;
		}
	}

	[XmlAttribute]
	public CT_KeyEncryptorUri uri
	{
		get
		{
			return uriField;
		}
		set
		{
			uriField = value;
		}
	}

	[XmlIgnore]
	public bool uriSpecified
	{
		get
		{
			return uriFieldSpecified;
		}
		set
		{
			uriFieldSpecified = value;
		}
	}

	public CT_PasswordKeyEncryptor AddNewEncryptedPasswordKey()
	{
		return (CT_PasswordKeyEncryptor)(itemField = new CT_PasswordKeyEncryptor());
	}

	public CT_CertificateKeyEncryptor AddNewEncryptedCertificateKey()
	{
		return (CT_CertificateKeyEncryptor)(itemField = new CT_CertificateKeyEncryptor());
	}

	public static CT_KeyEncryptor Parse(XmlNode node, XmlNamespaceManager nameSpaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_KeyEncryptor cT_KeyEncryptor = new CT_KeyEncryptor();
		if (node.Attributes["uri"] != null)
		{
			cT_KeyEncryptor.uriFieldSpecified = true;
			cT_KeyEncryptor.uriField = XmlHelper.GetEnumValueFromString<CT_KeyEncryptorUri>(node.Attributes["uri"].Value);
		}
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (cT_KeyEncryptor.uriField == CT_KeyEncryptorUri.httpschemasmicrosoftcomoffice2006keyEncryptorcertificate)
			{
				cT_KeyEncryptor.itemField = CT_CertificateKeyEncryptor.Parse(childNode, nameSpaceManager);
			}
			else
			{
				cT_KeyEncryptor.itemField = CT_PasswordKeyEncryptor.Parse(childNode, nameSpaceManager);
			}
		}
		return cT_KeyEncryptor;
	}
}
