using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml.Spreadsheet;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing")]
public class CT_GroupShape
{
	private CT_GroupShapeProperties grpSpPrField;

	private CT_GroupShapeNonVisual nvGrpSpPrField;

	private CT_Connector connectorField;

	private List<CT_Picture> pictures;

	private List<CT_Shape> shapes;

	public CT_GroupShapeNonVisual nvGrpSpPr
	{
		get
		{
			return nvGrpSpPrField;
		}
		set
		{
			nvGrpSpPrField = value;
		}
	}

	public CT_GroupShapeProperties grpSpPr
	{
		get
		{
			return grpSpPrField;
		}
		set
		{
			grpSpPrField = value;
		}
	}

	public CT_GroupShape()
	{
		pictures = new List<CT_Picture>();
		shapes = new List<CT_Shape>();
	}

	public void Set(CT_GroupShape groupShape)
	{
		grpSpPrField = groupShape.grpSpPr;
		nvGrpSpPrField = groupShape.nvGrpSpPr;
	}

	public CT_GroupShapeProperties AddNewGrpSpPr()
	{
		grpSpPrField = new CT_GroupShapeProperties();
		return grpSpPrField;
	}

	public CT_GroupShapeNonVisual AddNewNvGrpSpPr()
	{
		nvGrpSpPrField = new CT_GroupShapeNonVisual();
		return nvGrpSpPrField;
	}

	public CT_Connector AddNewCxnSp()
	{
		connectorField = new CT_Connector();
		return connectorField;
	}

	public CT_Shape AddNewSp()
	{
		CT_Shape cT_Shape = new CT_Shape();
		shapes.Add(cT_Shape);
		return cT_Shape;
	}

	public CT_Picture AddNewPic()
	{
		CT_Picture cT_Picture = new CT_Picture();
		pictures.Add(cT_Picture);
		return cT_Picture;
	}

	public static CT_GroupShape Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_GroupShape cT_GroupShape = new CT_GroupShape();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "nvGrpSpPr")
			{
				cT_GroupShape.nvGrpSpPr = CT_GroupShapeNonVisual.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "grpSpPr")
			{
				cT_GroupShape.grpSpPr = CT_GroupShapeProperties.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "pic")
			{
				CT_Picture item = CT_Picture.Parse(childNode, namespaceManager);
				cT_GroupShape.pictures.Add(item);
			}
			else if (childNode.LocalName == "sp")
			{
				CT_Shape item2 = CT_Shape.Parse(childNode, namespaceManager);
				cT_GroupShape.shapes.Add(item2);
			}
		}
		return cT_GroupShape;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<xdr:{nodeName}");
		sw.Write(">");
		if (nvGrpSpPr != null)
		{
			nvGrpSpPr.Write(sw, "xdr:nvGrpSpPr");
		}
		if (grpSpPr != null)
		{
			grpSpPr.Write(sw, "xdr:grpSpPr");
		}
		if (shapes.Count > 0)
		{
			foreach (CT_Shape shape in shapes)
			{
				shape.Write(sw, "sp");
			}
		}
		if (pictures.Count > 0)
		{
			foreach (CT_Picture picture in pictures)
			{
				picture.Write(sw, "pic");
			}
		}
		sw.Write($"</xdr:{nodeName}>");
	}
}
