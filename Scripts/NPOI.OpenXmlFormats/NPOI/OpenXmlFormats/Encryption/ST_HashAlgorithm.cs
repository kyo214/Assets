using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Encryption;

[Serializable]
[GeneratedCode("System.Xml", "4.8.3761.0")]
[XmlType(Namespace = "http://schemas.microsoft.com/office/2006/encryption")]
[XmlRoot(Namespace = "http://schemas.microsoft.com/office/2006/encryption", IsNullable = false)]
public enum ST_HashAlgorithm
{
	SHA1 = 0,
	SHA256 = 1,
	SHA384 = 2,
	SHA512 = 3,
	MD5 = 4,
	MD4 = 5,
	MD2 = 6,
	[XmlEnum("RIPEMD-128")]
	RIPEMD128 = 7,
	[XmlEnum("RIPEMD-160")]
	RIPEMD160 = 8,
	WHIRLPOOL = 9
}
