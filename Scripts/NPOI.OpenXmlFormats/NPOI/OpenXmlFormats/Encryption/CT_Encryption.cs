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
[XmlRoot("encryption", Namespace = "http://schemas.microsoft.com/office/2006/encryption", IsNullable = false)]
public class CT_Encryption
{
	private CT_KeyData keyDataField;

	private CT_DataIntegrity dataIntegrityField;

	private CT_KeyEncryptors keyEncryptorsField;

	[XmlElement(Order = 0)]
	public CT_KeyData keyData
	{
		get
		{
			return keyDataField;
		}
		set
		{
			keyDataField = value;
		}
	}

	[XmlElement(Order = 1)]
	public CT_DataIntegrity dataIntegrity
	{
		get
		{
			return dataIntegrityField;
		}
		set
		{
			dataIntegrityField = value;
		}
	}

	[XmlElement(Order = 2)]
	public CT_KeyEncryptors keyEncryptors
	{
		get
		{
			return keyEncryptorsField;
		}
		set
		{
			keyEncryptorsField = value;
		}
	}

	public CT_KeyData AddNewKeyData()
	{
		throw new NotImplementedException();
	}

	public CT_KeyEncryptors AddNewKeyEncryptors()
	{
		throw new NotImplementedException();
	}

	public CT_DataIntegrity AddNewDataIntegrity()
	{
		throw new NotImplementedException();
	}

	internal static CT_Encryption Parse(XmlNode node, XmlNamespaceManager nameSpaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_Encryption cT_Encryption = new CT_Encryption();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "keyData")
			{
				cT_Encryption.keyData = CT_KeyData.Parse(childNode, nameSpaceManager);
			}
			else if (childNode.LocalName == "dataIntegrity")
			{
				cT_Encryption.dataIntegrity = CT_DataIntegrity.Parse(childNode, nameSpaceManager);
			}
			else if (childNode.LocalName == "keyEncryptors")
			{
				cT_Encryption.keyEncryptorsField = CT_KeyEncryptors.Parse(childNode, nameSpaceManager);
			}
		}
		return cT_Encryption;
	}
}
