using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Encryption;

[Serializable]
[GeneratedCode("System.Xml", "4.8.3761.0")]
[XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/office/2006/encryption")]
public enum CT_KeyEncryptorUri
{
	[XmlEnum("http://schemas.microsoft.com/office/2006/keyEncryptor/password")]
	httpschemasmicrosoftcomoffice2006keyEncryptorpassword = 0,
	[XmlEnum("http://schemas.microsoft.com/office/2006/keyEncryptor/certificate")]
	httpschemasmicrosoftcomoffice2006keyEncryptorcertificate = 1
}
