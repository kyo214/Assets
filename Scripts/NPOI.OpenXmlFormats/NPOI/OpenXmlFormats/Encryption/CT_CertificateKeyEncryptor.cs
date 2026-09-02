using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Encryption;

[Serializable]
[GeneratedCode("System.Xml", "4.8.3761.0")]
[DesignerCategory("code")]
[XmlType(Namespace = "http://schemas.microsoft.com/office/2006/keyEncryptor/certificate")]
[XmlRoot("encryptedKey", Namespace = "http://schemas.microsoft.com/office/2006/keyEncryptor/certificate", IsNullable = false)]
public class CT_CertificateKeyEncryptor
{
	private byte[] encryptedKeyValueField;

	private byte[] x509CertificateField;

	private byte[] certVerifierField;

	[XmlAttribute(DataType = "base64Binary")]
	public byte[] encryptedKeyValue
	{
		get
		{
			return encryptedKeyValueField;
		}
		set
		{
			encryptedKeyValueField = value;
		}
	}

	[XmlAttribute(DataType = "base64Binary")]
	public byte[] X509Certificate
	{
		get
		{
			return x509CertificateField;
		}
		set
		{
			x509CertificateField = value;
		}
	}

	[XmlAttribute(DataType = "base64Binary")]
	public byte[] certVerifier
	{
		get
		{
			return certVerifierField;
		}
		set
		{
			certVerifierField = value;
		}
	}

	public static CT_CertificateKeyEncryptor Parse(XmlNode node, XmlNamespaceManager nameSpaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_CertificateKeyEncryptor cT_CertificateKeyEncryptor = new CT_CertificateKeyEncryptor();
		if (node.Attributes["certVerifier"] != null)
		{
			cT_CertificateKeyEncryptor.certVerifier = Convert.FromBase64String(node.Attributes["certVerifier"].Value);
		}
		if (node.Attributes["encryptedKeyValue"] != null)
		{
			cT_CertificateKeyEncryptor.encryptedKeyValue = Convert.FromBase64String(node.Attributes["encryptedKeyValue"].Value);
		}
		if (node.Attributes["x509Certificate"] != null)
		{
			cT_CertificateKeyEncryptor.X509Certificate = Convert.FromBase64String(node.Attributes["x509Certificate"].Value);
		}
		return cT_CertificateKeyEncryptor;
	}
}
