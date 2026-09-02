using System;
using NPOI.OpenXml4Net.OPC;
using NPOI.OpenXmlFormats.Dml;
using NPOI.OpenXmlFormats.Dml.Spreadsheet;

namespace NPOI.XSSF.UserModel;

public class XSSFShapeGroup : XSSFShape
{
	private static CT_GroupShape prototype;

	private CT_GroupShape ctGroup;

	public XSSFShapeGroup(XSSFDrawing drawing, CT_GroupShape ctGroup)
	{
		base.drawing = drawing;
		this.ctGroup = ctGroup;
	}

	internal static CT_GroupShape Prototype()
	{
		CT_GroupShape cT_GroupShape = new CT_GroupShape();
		CT_GroupShapeNonVisual cT_GroupShapeNonVisual = cT_GroupShape.AddNewNvGrpSpPr();
		NPOI.OpenXmlFormats.Dml.Spreadsheet.CT_NonVisualDrawingProps cT_NonVisualDrawingProps = cT_GroupShapeNonVisual.AddNewCNvPr();
		cT_NonVisualDrawingProps.id = 0u;
		cT_NonVisualDrawingProps.name = "Group 0";
		cT_GroupShapeNonVisual.AddNewCNvGrpSpPr();
		CT_GroupTransform2D cT_GroupTransform2D = cT_GroupShape.AddNewGrpSpPr().AddNewXfrm();
		CT_PositiveSize2D cT_PositiveSize2D = cT_GroupTransform2D.AddNewExt();
		cT_PositiveSize2D.cx = 0L;
		cT_PositiveSize2D.cy = 0L;
		CT_Point2D cT_Point2D = cT_GroupTransform2D.AddNewOff();
		cT_Point2D.x = 0L;
		cT_Point2D.y = 0L;
		CT_PositiveSize2D cT_PositiveSize2D2 = cT_GroupTransform2D.AddNewChExt();
		cT_PositiveSize2D2.cx = 0L;
		cT_PositiveSize2D2.cy = 0L;
		CT_Point2D cT_Point2D2 = cT_GroupTransform2D.AddNewChOff();
		cT_Point2D2.x = 0L;
		cT_Point2D2.y = 0L;
		prototype = cT_GroupShape;
		return prototype;
	}

	public XSSFTextBox CreateTextbox(XSSFChildAnchor anchor)
	{
		CT_Shape cT_Shape = ctGroup.AddNewSp();
		cT_Shape.Set(XSSFSimpleShape.Prototype());
		XSSFTextBox xSSFTextBox = new XSSFTextBox(GetDrawing(), cT_Shape);
		xSSFTextBox.parent = this;
		xSSFTextBox.anchor = anchor;
		xSSFTextBox.GetCTShape().spPr.xfrm = anchor.GetCTTransform2D();
		return xSSFTextBox;
	}

	public XSSFSimpleShape CreateSimpleShape(XSSFChildAnchor anchor)
	{
		CT_Shape cT_Shape = ctGroup.AddNewSp();
		cT_Shape.Set(XSSFSimpleShape.Prototype());
		XSSFSimpleShape xSSFSimpleShape = new XSSFSimpleShape(GetDrawing(), cT_Shape);
		xSSFSimpleShape.parent = this;
		xSSFSimpleShape.anchor = anchor;
		xSSFSimpleShape.GetCTShape().spPr.xfrm = anchor.GetCTTransform2D();
		return xSSFSimpleShape;
	}

	public XSSFConnector CreateConnector(XSSFChildAnchor anchor)
	{
		CT_Connector cT_Connector = ctGroup.AddNewCxnSp();
		cT_Connector.Set(XSSFConnector.Prototype());
		XSSFConnector xSSFConnector = new XSSFConnector(GetDrawing(), cT_Connector);
		xSSFConnector.parent = this;
		xSSFConnector.anchor = anchor;
		xSSFConnector.GetCTConnector().spPr.xfrm = anchor.GetCTTransform2D();
		return xSSFConnector;
	}

	public XSSFPicture CreatePicture(XSSFClientAnchor anchor, int pictureIndex)
	{
		PackageRelationship pictureReference = GetDrawing().AddPictureReference(pictureIndex);
		CT_Picture cT_Picture = ctGroup.AddNewPic();
		cT_Picture.Set(XSSFPicture.Prototype());
		XSSFPicture xSSFPicture = new XSSFPicture(GetDrawing(), cT_Picture);
		xSSFPicture.parent = this;
		xSSFPicture.anchor = anchor;
		xSSFPicture.SetPictureReference(pictureReference);
		return xSSFPicture;
	}

	public CT_GroupShape GetCTGroupShape()
	{
		return ctGroup;
	}

	public void SetCoordinates(int x1, int y1, int x2, int y2)
	{
		CT_GroupTransform2D xfrm = ctGroup.grpSpPr.xfrm;
		CT_Point2D off = xfrm.off;
		off.x = x1;
		off.y = y1;
		CT_PositiveSize2D ext = xfrm.ext;
		ext.cx = x2;
		ext.cy = y2;
		CT_Point2D chOff = xfrm.chOff;
		chOff.x = x1;
		chOff.y = y1;
		CT_PositiveSize2D chExt = xfrm.chExt;
		chExt.cx = x2;
		chExt.cy = y2;
	}

	protected internal override NPOI.OpenXmlFormats.Dml.Spreadsheet.CT_ShapeProperties GetShapeProperties()
	{
		throw new InvalidOperationException("Not supported for shape group");
	}
}
