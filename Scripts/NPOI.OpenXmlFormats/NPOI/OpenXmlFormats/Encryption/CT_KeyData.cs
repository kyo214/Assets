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
public class CT_KeyData
{
	private uint saltSizeField;

	private uint blockSizeField;

	private uint keyBitsField;

	private uint hashSizeField;

	private ST_CipherAlgorithm cipherAlgorithmField;

	private ST_CipherChaining cipherChainingField;

	private ST_HashAlgorithm hashAlgorithmField;

	private byte[] saltValueField;

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

	internal static CT_KeyData Parse(XmlNode node, XmlNamespaceManager nameSpaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_KeyData cT_KeyData = new CT_KeyData();
		cT_KeyData.saltSize = XmlHelper.ReadUInt(node.Attributes["saltSize"]);
		cT_KeyData.blockSize = XmlHelper.ReadUInt(node.Attributes["blockSize"]);
		cT_KeyData.keyBits = XmlHelper.ReadUInt(node.Attributes["keyBits"]);
		cT_KeyData.hashSize = XmlHelper.ReadUInt(node.Attributes["hashSize"]);
		cT_KeyData.cipherAlgorithm = XmlHelper.ReadEnum<ST_CipherAlgorithm>(node.Attributes["cipherAlgorithm"]);
		cT_KeyData.cipherChaining = XmlHelper.ReadEnum<ST_CipherChaining>(node.Attributes["cipherChaining"]);
		cT_KeyData.hashAlgorithm = XmlHelper.ReadEnum<ST_HashAlgorithm>(node.Attributes["hashAlgorithm"]);
		if (node.Attributes["saltValue"] != null)
		{
			cT_KeyData.saltValue = Convert.FromBase64String(node.Attributes["saltValue"].Value);
		}
		return cT_KeyData;
	}
}
