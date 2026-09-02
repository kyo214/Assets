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
public class CT_GvmlShapeNonVisual
{
	private CT_NonVisualDrawingProps cNvPrField;

	private CT_NonVisualDrawingShapeProps cNvSpPrField;

	[XmlElement(Order = 0)]
	public CT_NonVisualDrawingProps cNvPr
	{
		get
		{
			return cNvPrField;
		}
		set
		{
			cNvPrField = value;
		}
	}

	[XmlElement(Order = 1)]
	public CT_NonVisualDrawingShapeProps cNvSpPr
	{
		get
		{
			return cNvSpPrField;
		}
		set
		{
			cNvSpPrField = value;
		}
	}
}
