using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Encryption;

[Serializable]
[GeneratedCode("System.Xml", "4.8.3761.0")]
[XmlType(Namespace = "http://schemas.microsoft.com/office/2006/encryption")]
[XmlRoot(Namespace = "http://schemas.microsoft.com/office/2006/encryption", IsNullable = false)]
public enum ST_CipherChaining
{
	ChainingModeCBC = 0,
	ChainingModeCFB = 1
}
