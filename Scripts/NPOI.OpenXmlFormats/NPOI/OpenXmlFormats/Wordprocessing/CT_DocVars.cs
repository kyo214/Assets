using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main", IsNullable = true)]
public class CT_DocVars
{
	private List<CT_DocVar> docVarField;

	[XmlElement("docVar", Order = 0)]
	public List<CT_DocVar> docVar
	{
		get
		{
			return docVarField;
		}
		set
		{
			docVarField = value;
		}
	}

	public CT_DocVars()
	{
		docVarField = new List<CT_DocVar>();
	}
}
