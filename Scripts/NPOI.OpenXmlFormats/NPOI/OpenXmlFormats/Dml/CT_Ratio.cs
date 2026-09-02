using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main", IsNullable = true)]
public class CT_Ratio
{
	private long nField;

	private long dField;

	[XmlAttribute]
	public long n
	{
		get
		{
			return nField;
		}
		set
		{
			nField = value;
		}
	}

	[XmlAttribute]
	public long d
	{
		get
		{
			return dField;
		}
		set
		{
			dField = value;
		}
	}
}
