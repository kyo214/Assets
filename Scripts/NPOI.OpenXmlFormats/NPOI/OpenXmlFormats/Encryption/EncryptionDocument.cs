using System;
using System.IO;
using System.Xml;

namespace NPOI.OpenXmlFormats.Encryption;

public class EncryptionDocument
{
	public static string ENCRYPTION_DEFAULT = "http://schemas.microsoft.com/office/2006/encryption";

	public static string ENCRYPTION_PASSWORD = "http://schemas.microsoft.com/office/2006/keyEncryptor/password";

	public static string ENCRYPTION_CERTIFICATE = "http://schemas.microsoft.com/office/2006/keyEncryptor/certificate";

	private static XmlNamespaceManager nsm = null;

	private CT_Encryption ctEncryption;

	public static XmlNamespaceManager EncryptionNamespaceManager
	{
		get
		{
			if (nsm == null)
			{
				nsm = CreateEncryptionNSM();
			}
			return nsm;
		}
	}

	internal static XmlNamespaceManager CreateEncryptionNSM()
	{
		XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(new NameTable());
		xmlNamespaceManager.AddNamespace(string.Empty, ENCRYPTION_DEFAULT);
		xmlNamespaceManager.AddNamespace("p", ENCRYPTION_PASSWORD);
		xmlNamespaceManager.AddNamespace("c", ENCRYPTION_CERTIFICATE);
		xmlNamespaceManager.AddNamespace("xsd", "http://www.w3.org/2001/XMLSchema");
		return xmlNamespaceManager;
	}

	public EncryptionDocument()
	{
	}

	public EncryptionDocument(CT_Encryption encryption)
	{
		ctEncryption = encryption;
	}

	public static EncryptionDocument Parse(XmlDocument xmlDoc, XmlNamespaceManager NameSpaceManager)
	{
		return new EncryptionDocument(CT_Encryption.Parse(xmlDoc.DocumentElement, NameSpaceManager));
	}

	public CT_Encryption GetEncryption()
	{
		return ctEncryption;
	}

	public void SetEncryption(CT_Encryption encryption)
	{
		ctEncryption = encryption;
	}

	public CT_Encryption AddNewEncryption()
	{
		ctEncryption = new CT_Encryption();
		return ctEncryption;
	}

	public void Save(Stream stream)
	{
		StreamWriter streamWriter = new StreamWriter(stream);
		streamWriter.Write("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\" ?>");
		streamWriter.Flush();
		throw new NotImplementedException();
	}

	public static EncryptionDocument NewInstance()
	{
		throw new NotImplementedException();
	}

	public static EncryptionDocument Parse(string descriptor)
	{
		throw new NotImplementedException();
	}

	public static EncryptionDocument Parse(XmlDocument xmlDoc)
	{
		return Parse(xmlDoc, EncryptionNamespaceManager);
	}
}
