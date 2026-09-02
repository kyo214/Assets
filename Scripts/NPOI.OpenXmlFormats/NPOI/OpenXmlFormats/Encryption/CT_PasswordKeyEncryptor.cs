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
[XmlType(Namespace = "http://schemas.microsoft.com/office/2006/keyEncryptor/password")]
[XmlRoot("encryptedKey", Namespace = "http://schemas.microsoft.com/office/2006/keyEncryptor/password", IsNullable = false)]
public class CT_PasswordKeyEncryptor
{
	private uint saltSizeField;

	private uint blockSizeField;

	private uint keyBitsField;

	private uint hashSizeField;

	private ST_CipherAlgorithm cipherAlgorithmField;

	private ST_CipherChaining cipherChainingField;

	private ST_HashAlgorithm hashAlgorithmField;

	private byte[] saltValueField;

	private uint spinCountField;

	private byte[] encryptedVerifierHashInputField;

	private byte[] encryptedVerifierHashValueField;

	private byte[] encryptedKeyValueField;

	[XmlAttribute]
	public uint saltSize
	{
		get
		{
			return saltSizeField;
		}
		set
		{
			saltSizeField = value;
		}
	}

	[XmlAttribute]
	public uint blockSize
	{
		get
		{
			return blockSizeField;
		}
		set
		{
			blockSizeField = value;
		}
	}

	[XmlAttribute]
	public uint keyBits
	{
		get
		{
			return keyBitsField;
		}
		set
		{
			keyBitsField = value;
		}
	}

	[XmlAttribute]
	public uint hashSize
	{
		get
		{
			return hashSizeField;
		}
		set
		{
			hashSizeField = value;
		}
	}

	[XmlAttribute]
	public ST_CipherAlgorithm cipherAlgorithm
	{
		get
		{
			return cipherAlgorithmField;
		}
		set
		{
			cipherAlgorithmField = value;
		}
	}

	[XmlAttribute]
	public ST_CipherChaining cipherChaining
	{
		get
		{
			return cipherChainingField;
		}
		set
		{
			cipherChainingField = value;
		}
	}

	[XmlAttribute]
	public ST_HashAlgorithm hashAlgorithm
	{
		get
		{
			return hashAlgorithmField;
		}
		set
		{
			hashAlgorithmField = value;
		}
	}

	[XmlAttribute(DataType = "base64Binary")]
	public byte[] saltValue
	{
		get
		{
			return saltValueField;
		}
		set
		{
			saltValueField = value;
		}
	}

	[XmlAttribute]
	public uint spinCount
	{
		get
		{
			return spinCountField;
		}
		set
		{
			spinCountField = value;
		}
	}

	[XmlAttribute(DataType = "base64Binary")]
	public byte[] encryptedVerifierHashInput
	{
		get
		{
			return encryptedVerifierHashInputField;
		}
		set
		{
			encryptedVerifierHashInputField = value;
		}
	}

	[XmlAttribute(DataType = "base64Binary")]
	public byte[] encryptedVerifierHashValue
	{
		get
		{
			return encryptedVerifierHashValueField;
		}
		set
		{
			encryptedVerifierHashValueField = value;
		}
	}

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

	public static CT_PasswordKeyEncryptor Parse(XmlNode node, XmlNamespaceManager nameSpaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_PasswordKeyEncryptor cT_PasswordKeyEncryptor = new CT_PasswordKeyEncryptor();
		cT_PasswordKeyEncryptor.spinCount = XmlHelper.ReadUInt(node.Attributes["spinCount"]);
		cT_PasswordKeyEncryptor.saltSize = XmlHelper.ReadUInt(node.Attributes["saltSize"]);
		cT_PasswordKeyEncryptor.blockSize = XmlHelper.ReadUInt(node.Attributes["blockSize"]);
		cT_PasswordKeyEncryptor.keyBits = XmlHelper.ReadUInt(node.Attributes["keyBits"]);
		cT_PasswordKeyEncryptor.hashSize = XmlHelper.ReadUInt(node.Attributes["hashSize"]);
		cT_PasswordKeyEncryptor.cipherAlgorithm = XmlHelper.ReadEnum<ST_CipherAlgorithm>(node.Attributes["cipherAlgorithm"]);
		cT_PasswordKeyEncryptor.cipherChaining = XmlHelper.ReadEnum<ST_CipherChaining>(node.Attributes["cipherChaining"]);
		cT_PasswordKeyEncryptor.hashAlgorithm = XmlHelper.ReadEnum<ST_HashAlgorithm>(node.Attributes["hashAlgorithm"]);
		if (node.Attributes["saltValue"] != null)
		{
			cT_PasswordKeyEncryptor.saltValue = Convert.FromBase64String(node.Attributes["saltValue"].Value);
		}
		if (node.Attributes["encryptedVerifierHashInput"] != null)
		{
			cT_PasswordKeyEncryptor.encryptedVerifierHashInput = Convert.FromBase64String(node.Attributes["encryptedVerifierHashInput"].Value);
		}
		if (node.Attributes["encryptedVerifierHashValue"] != null)
		{
			cT_PasswordKeyEncryptor.encryptedVerifierHashValue = Convert.FromBase64String(node.Attributes["encryptedVerifierHashValue"].Value);
		}
		if (node.Attributes["encryptedKeyValue"] != null)
		{
			cT_PasswordKeyEncryptor.encryptedKeyValue = Convert.FromBase64String(node.Attributes["encryptedKeyValue"].Value);
		}
		return cT_PasswordKeyEncryptor;
	}
}
