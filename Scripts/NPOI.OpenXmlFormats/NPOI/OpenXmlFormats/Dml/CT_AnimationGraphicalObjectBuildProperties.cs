using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml;

[Serializable]
[DebuggerStepThrough]
[DesignerCategory("code")]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main", IsNullable = true)]
public class CT_AnimationGraphicalObjectBuildProperties
{
	private object itemField;

	[XmlElement("bldChart", typeof(CT_AnimationChartBuildProperties))]
	[XmlElement("bldDgm", typeof(CT_AnimationDgmBuildProperties))]
	public object Item
	{
		get
		{
			return itemField;
		}
		set
		{
			itemField = value;
		}
	}
}
