using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Encryption;

[Serializable]
[GeneratedCode("System.Xml", "4.8.3761.0")]
[XmlType(Namespace = "http://schemas.microsoft.com/office/2006/encryption")]
[XmlRoot(Namespace = "http://schemas.microsoft.com/office/2006/encryption", IsNullable = false)]
public enum ST_CipherAlgorithm
{
	AES = 0,
	RC2 = 1,
	RC4 = 2,
	DES = 3,
	DESX = 4,
	[XmlEnum("3DES")]
	Item3DES = 5,
	[XmlEnum("3DES_112")]
	Item3DES_112 = 6
}
