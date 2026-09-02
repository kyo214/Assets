using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[XmlRoot("hdr", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main", IsNullable = false)]
public class CT_Hdr : CT_HdrFtr
{
	public static CT_Hdr Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		return (CT_Hdr)CT_HdrFtr.Parse(new CT_Hdr(), node, namespaceManager);
	}

	internal void Write(StreamWriter sw)
	{
		Write(sw, "hdr");
	}
}
