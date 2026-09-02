using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using NPOI.OpenXml4Net.OPC;
using NPOI.OpenXmlFormats.Dml.Spreadsheet;
using NPOI.OpenXmlFormats.Vml;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.Util;
using NPOI.XSSF.Model;

namespace NPOI.XSSF.UserModel;

public class XSSFDrawing : POIXMLDocumentPart, IDrawing
{
	public static string NAMESPACE_A = XSSFRelation.NS_DRAWINGML;

	public static string NAMESPACE_C = XSSFRelation.NS_CHART;

	private CT_Drawing drawing = NewDrawing();

	private long numOfGraphicFrames;

	public XSSFDrawing()
	{
		drawing = NewDrawing();
	}

	internal XSSFDrawing(PackagePart part)
		: base(part)
	{
		XmlDocument xmldoc = POIXMLDocumentPart.ConvertStreamToXml(part.GetInputStream());
		drawing = CT_Drawing.Parse(xmldoc, POIXMLDocumentPart.NamespaceManager);
	}

	public XSSFDrawing(PackagePart part, PackageRelationship rel)
		: this(part)
	{
	}

	private static CT_Drawing NewDrawing()
	{
		return new CT_Drawing();
	}

	public CT_Drawing GetCTDrawing()
	{
		return drawing;
	}

	protected internal override void Commit()
	{
		Stream outputStream = GetPackagePart().GetOutputStream();
		drawing.Save(outputStream);
		outputStream.Close();
	}

	public IClientAnchor CreateAnchor(int dx1, int dy1, int dx2, int dy2, int col1, int row1, int col2, int row2)
	{
		return new XSSFClientAnchor(dx1, dy1, dx2, dy2, col1, row1, col2, row2);
	}

	public XSSFTextBox CreateTextbox(IClientAnchor anchor)
	{
		long num = newShapeId();
		NPOI.OpenXmlFormats.Dml.Spreadsheet.CT_Shape cT_Shape = CreateTwoCellAnchor(anchor).AddNewSp();
		cT_Shape.Set(XSSFSimpleShape.Prototype());
		cT_Shape.nvSpPr.cNvPr.id = (uint)num;
		return new XSSFTextBox(this, cT_Shape)
		{
			anchor = (XSSFClientAnchor)anchor
		};
	}

	public IPicture CreatePicture(XSSFClientAnchor anchor, int pictureIndex)
	{
		PackageRelationship pictureReference = AddPictureReference(pictureIndex);
		long num = newShapeId();
		CT_Picture cT_Picture = CreateTwoCellAnchor(anchor).AddNewPic();
		cT_Picture.Set(XSSFPicture.Prototype());
		cT_Picture.nvPicPr.cNvPr.id = (uint)num;
		cT_Picture.nvPicPr.cNvPr.name = "Picture " + num;
		XSSFPicture xSSFPicture = new XSSFPicture(this, cT_Picture);
		xSSFPicture.anchor = anchor;
		xSSFPicture.SetPictureReference(pictureReference);
		return xSSFPicture;
	}

	public IPicture CreatePicture(IClientAnchor anchor, int pictureIndex)
	{
		return CreatePicture((XSSFClientAnchor)anchor, pictureIndex);
	}

	public IChart CreateChart(IClientAnchor anchor)
	{
		int idx = GetPackagePart().Package.GetPartsByContentType(XSSFRelation.CHART.ContentType).Count + 1;
		RelationPart relationPart = CreateRelationship(XSSFRelation.CHART, XSSFFactory.GetInstance(), idx, noRelation: false);
		XSSFChart xSSFChart = relationPart.DocumentPart as XSSFChart;
		string id = relationPart.Relationship.Id;
		CreateGraphicFrame((XSSFClientAnchor)anchor).SetChart(xSSFChart, id);
		return xSSFChart;
	}

	internal PackageRelationship AddPictureReference(int pictureIndex)
	{
		XSSFPictureData part = new XSSFPictureData(((XSSFPictureData)((XSSFWorkbook)GetParent().GetParent()).GetAllPictures()[pictureIndex]).GetPackagePart());
		return AddRelation(null, XSSFRelation.IMAGES, part).Relationship;
	}

	public XSSFSimpleShape CreateSimpleShape(XSSFClientAnchor anchor)
	{
		long num = newShapeId();
		NPOI.OpenXmlFormats.Dml.Spreadsheet.CT_Shape cT_Shape = CreateTwoCellAnchor(anchor).AddNewSp();
		cT_Shape.Set(XSSFSimpleShape.Prototype());
		cT_Shape.nvSpPr.cNvPr.id = (uint)num;
		return new XSSFSimpleShape(this, cT_Shape)
		{
			anchor = anchor
		};
	}

	public XSSFConnector CreateConnector(XSSFClientAnchor anchor)
	{
		CT_Connector cT_Connector = CreateTwoCellAnchor(anchor).AddNewCxnSp();
		cT_Connector.Set(XSSFConnector.Prototype());
		return new XSSFConnector(this, cT_Connector)
		{
			anchor = anchor
		};
	}

	public XSSFShapeGroup CreateGroup(XSSFClientAnchor anchor)
	{
		CT_GroupShape cT_GroupShape = CreateTwoCellAnchor(anchor).AddNewGrpSp();
		cT_GroupShape.Set(XSSFShapeGroup.Prototype());
		return new XSSFShapeGroup(this, cT_GroupShape)
		{
			anchor = anchor
		};
	}

	public IComment CreateCellComment(IClientAnchor anchor)
	{
		XSSFClientAnchor xSSFClientAnchor = (XSSFClientAnchor)anchor;
		XSSFSheet obj = (XSSFSheet)GetParent();
		CommentsTable commentsTable = obj.GetCommentsTable(create: true);
		NPOI.OpenXmlFormats.Vml.CT_Shape cT_Shape = obj.GetVMLDrawing(autoCreate: true).newCommentShape();
		if (xSSFClientAnchor.IsSet())
		{
			int num = xSSFClientAnchor.Dx1 / Units.EMU_PER_PIXEL;
			int num2 = xSSFClientAnchor.Dy1 / Units.EMU_PER_PIXEL;
			int num3 = xSSFClientAnchor.Dx2 / Units.EMU_PER_PIXEL;
			int num4 = xSSFClientAnchor.Dy2 / Units.EMU_PER_PIXEL;
			string value = xSSFClientAnchor.Col1 + ", " + num + ", " + xSSFClientAnchor.Row1 + ", " + num2 + ", " + xSSFClientAnchor.Col2 + ", " + num3 + ", " + xSSFClientAnchor.Row2 + ", " + num4;
			cT_Shape.GetClientDataArray(0).SetAnchorArray(0, value);
		}
		CellAddress cellAddress = new CellAddress(xSSFClientAnchor.Row1, xSSFClientAnchor.Col1);
		if (commentsTable.FindCellComment(cellAddress) != null)
		{
			throw new ArgumentException("Multiple cell comments in one cell are not allowed, cell: " + cellAddress);
		}
		return new XSSFComment(commentsTable, commentsTable.NewComment(cellAddress), cT_Shape);
	}

	private XSSFGraphicFrame CreateGraphicFrame(XSSFClientAnchor anchor)
	{
		CT_GraphicalObjectFrame cT_GraphicalObjectFrame = CreateTwoCellAnchor(anchor).AddNewGraphicFrame();
		cT_GraphicalObjectFrame.Set(XSSFGraphicFrame.Prototype());
		long id = numOfGraphicFrames++;
		return new XSSFGraphicFrame(this, cT_GraphicalObjectFrame)
		{
			Anchor = anchor,
			Id = id,
			Name = "Diagramm" + id
		};
	}

	public List<XSSFChart> GetCharts()
	{
		List<XSSFChart> list = new List<XSSFChart>();
		foreach (POIXMLDocumentPart relation in GetRelations())
		{
			if (relation is XSSFChart)
			{
				list.Add((XSSFChart)relation);
			}
		}
		return list;
	}

	private CT_TwoCellAnchor CreateTwoCellAnchor(IClientAnchor anchor)
	{
		CT_TwoCellAnchor cT_TwoCellAnchor = drawing.AddNewTwoCellAnchor();
		XSSFClientAnchor xSSFClientAnchor = (XSSFClientAnchor)anchor;
		cT_TwoCellAnchor.from = xSSFClientAnchor.From;
		cT_TwoCellAnchor.to = xSSFClientAnchor.To;
		cT_TwoCellAnchor.AddNewClientData();
		xSSFClientAnchor.To = cT_TwoCellAnchor.to;
		xSSFClientAnchor.From = cT_TwoCellAnchor.from;
		cT_TwoCellAnchor.editAs = anchor.AnchorType switch
		{
			AnchorType.DontMoveAndResize => NPOI.OpenXmlFormats.Dml.Spreadsheet.ST_EditAs.absolute, 
			AnchorType.MoveAndResize => NPOI.OpenXmlFormats.Dml.Spreadsheet.ST_EditAs.twoCell, 
			AnchorType.MoveDontResize => NPOI.OpenXmlFormats.Dml.Spreadsheet.ST_EditAs.oneCell, 
			_ => NPOI.OpenXmlFormats.Dml.Spreadsheet.ST_EditAs.oneCell, 
		};
		cT_TwoCellAnchor.editAsSpecified = true;
		return cT_TwoCellAnchor;
	}

	private long newShapeId()
	{
		return drawing.SizeOfTwoCellAnchorArray() + 1;
	}

	public bool ContainsChart()
	{
		throw new NotImplementedException();
	}

	public List<XSSFShape> GetShapes()
	{
		List<XSSFShape> list = new List<XSSFShape>();
		foreach (IEG_Anchor cellAnchor in drawing.CellAnchors)
		{
			XSSFShape xSSFShape = null;
			if (cellAnchor.picture != null)
			{
				xSSFShape = new XSSFPicture(this, cellAnchor.picture);
			}
			else if (cellAnchor.connector != null)
			{
				xSSFShape = new XSSFConnector(this, cellAnchor.connector);
			}
			else if (cellAnchor.groupShape != null)
			{
				xSSFShape = new XSSFShapeGroup(this, cellAnchor.groupShape);
			}
			else if (cellAnchor.graphicFrame != null)
			{
				xSSFShape = new XSSFGraphicFrame(this, cellAnchor.graphicFrame);
			}
			else if (cellAnchor.sp != null)
			{
				xSSFShape = new XSSFSimpleShape(this, cellAnchor.sp);
			}
			if (xSSFShape != null)
			{
				xSSFShape.anchor = GetAnchorFromIEGAnchor(cellAnchor);
				list.Add(xSSFShape);
			}
		}
		return list;
	}

	private XSSFAnchor GetAnchorFromIEGAnchor(IEG_Anchor ctAnchor)
	{
		CT_Marker cell = null;
		CT_Marker cell2 = null;
		if (ctAnchor is CT_TwoCellAnchor)
		{
			cell = ((CT_TwoCellAnchor)ctAnchor).from;
			cell2 = ((CT_TwoCellAnchor)ctAnchor).to;
		}
		else if (ctAnchor is CT_OneCellAnchor)
		{
			cell = ((CT_OneCellAnchor)ctAnchor).from;
		}
		return new XSSFClientAnchor(cell, cell2);
	}

	private XSSFAnchor GetAnchorFromParent(XmlNode obj)
	{
		XmlNode parentNode = obj.ParentNode;
		CT_Marker cell = CT_Marker.Parse(parentNode.SelectSingleNode("xdr:from", POIXMLDocumentPart.NamespaceManager) ?? throw new InvalidDataException("xdr:from node is missing"), POIXMLDocumentPart.NamespaceManager);
		XmlNode xmlNode = parentNode.SelectSingleNode("xdr:to", POIXMLDocumentPart.NamespaceManager);
		CT_Marker cell2 = null;
		if (xmlNode != null)
		{
			cell2 = CT_Marker.Parse(xmlNode, POIXMLDocumentPart.NamespaceManager);
		}
		return new XSSFClientAnchor(cell, cell2);
	}
}
