using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main", IsNullable = true)]
public class CT_DocPartBehaviors
{
	private List<CT_DocPartBehavior> itemsField;

	[XmlElement("behavior", Order = 0)]
	public List<CT_DocPartBehavior> Items
	{
		get
		{
			return itemsField;
		}
		set
		{
			itemsField = value;
		}
	}

	public CT_DocPartBehaviors()
	{
		itemsField = new List<CT_DocPartBehavior>();
	}
}
