using System;
using System.Drawing;
using System.IO;
using System.Xml;
using NPOI.OpenXml4Net.OPC;
using NPOI.OpenXmlFormats.Dml;
using NPOI.OpenXmlFormats.Dml.Spreadsheet;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.Util;

namespace NPOI.XSSF.UserModel;

public class XSSFPicture : XSSFShape, IPicture
{
	private static POILogger logger = POILogFactory.GetLogger(typeof(XSSFPicture));

	private static CT_Picture prototype = null;

	private CT_Picture ctPicture;

	public new int CountOfAllChildren
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public new int FillColor
	{
		get
		{
			throw new NotImplementedException();
		}
		set
		{
			throw new NotImplementedException();
		}
	}

	public new LineStyle LineStyle
	{
		get
		{
			throw new NotImplementedException();
		}
		set
		{
			base.LineStyle = value;
		}
	}

	public new int LineStyleColor
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public new int LineWidth
	{
		get
		{
			throw new NotImplementedException();
		}
		set
		{
			base.LineWidth = value;
		}
	}

	public IPictureData PictureData
	{
		get
		{
			string embed = ctPicture.blipFill.blip.embed;
			return (XSSFPictureData)GetDrawing().GetRelationById(embed);
		}
	}

	public IClientAnchor ClientAnchor
	{
		get
		{
			XSSFAnchor xSSFAnchor = GetAnchor();
			if (!(xSSFAnchor is XSSFClientAnchor))
			{
				return null;
			}
			return (XSSFClientAnchor)xSSFAnchor;
		}
	}

	public ISheet Sheet => (XSSFSheet)GetDrawing().GetParent();

	public XSSFPicture(XSSFDrawing drawing, CT_Picture ctPicture)
	{
		base.drawing = drawing;
		this.ctPicture = ctPicture;
	}

	public XSSFPicture(XSSFDrawing drawing, XmlNode ctPicture)
	{
		base.drawing = drawing;
		this.ctPicture = CT_Picture.Parse(ctPicture, POIXMLDocumentPart.NamespaceManager);
	}

	internal static CT_Picture Prototype()
	{
		CT_Picture cT_Picture = new CT_Picture();
		CT_PictureNonVisual cT_PictureNonVisual = cT_Picture.AddNewNvPicPr();
		NPOI.OpenXmlFormats.Dml.Spreadsheet.CT_NonVisualDrawingProps cT_NonVisualDrawingProps = cT_PictureNonVisual.AddNewCNvPr();
		cT_NonVisualDrawingProps.id = 1u;
		cT_NonVisualDrawingProps.name = "Picture 1";
		cT_NonVisualDrawingProps.descr = "Picture";
		cT_PictureNonVisual.AddNewCNvPicPr().AddNewPicLocks().noChangeAspect = true;
		NPOI.OpenXmlFormats.Dml.Spreadsheet.CT_BlipFillProperties cT_BlipFillProperties = cT_Picture.AddNewBlipFill();
		cT_BlipFillProperties.AddNewBlip().embed = "";
		cT_BlipFillProperties.AddNewStretch().AddNewFillRect();
		NPOI.OpenXmlFormats.Dml.Spreadsheet.CT_ShapeProperties cT_ShapeProperties = cT_Picture.AddNewSpPr();
		CT_Transform2D cT_Transform2D = cT_ShapeProperties.AddNewXfrm();
		CT_PositiveSize2D cT_PositiveSize2D = cT_Transform2D.AddNewExt();
		cT_PositiveSize2D.cx = 0L;
		cT_PositiveSize2D.cy = 0L;
		CT_Point2D cT_Point2D = cT_Transform2D.AddNewOff();
		cT_Point2D.x = 0L;
		cT_Point2D.y = 0L;
		CT_PresetGeometry2D cT_PresetGeometry2D = cT_ShapeProperties.AddNewPrstGeom();
		cT_PresetGeometry2D.prst = ST_ShapeType.rect;
		cT_PresetGeometry2D.AddNewAvLst();
		prototype = cT_Picture;
		return prototype;
	}

	internal void SetPictureReference(PackageRelationship rel)
	{
		ctPicture.blipFill.blip.embed = rel.Id;
	}

	public CT_Picture GetCTPicture()
	{
		return ctPicture;
	}

	public void Resize()
	{
		Resize(double.MaxValue);
	}

	public void Resize(double scale)
	{
		Resize(scale, scale);
	}

	public void Resize(double scaleX, double scaleY)
	{
		XSSFClientAnchor obj = (XSSFClientAnchor)GetAnchor();
		IClientAnchor preferredSize = GetPreferredSize(scaleX, scaleY);
		int row = ((IClientAnchor)obj).Row1 + (preferredSize.Row2 - preferredSize.Row1);
		int col = ((IClientAnchor)obj).Col1 + (preferredSize.Col2 - preferredSize.Col1);
		((IClientAnchor)obj).Col2 = col;
		((IClientAnchor)obj).Dx2 = preferredSize.Dx2;
		((IClientAnchor)obj).Row2 = row;
		((IClientAnchor)obj).Dy2 = preferredSize.Dy2;
	}

	public IClientAnchor GetPreferredSize()
	{
		return GetPreferredSize(1.0);
	}

	public IClientAnchor GetPreferredSize(double scale)
	{
		return GetPreferredSize(scale, scale);
	}

	public IClientAnchor GetPreferredSize(double scaleX, double scaleY)
	{
		Size size = ImageUtils.SetPreferredSize(this, scaleX, scaleY);
		CT_PositiveSize2D ext = ctPicture.spPr.xfrm.ext;
		ext.cx = size.Width;
		ext.cy = size.Height;
		return ClientAnchor;
	}

	protected static Size GetImageDimension(PackagePart part, PictureType type)
	{
		try
		{
			return ImageUtils.GetImageDimension(part.GetInputStream());
		}
		catch (IOException exception)
		{
			logger.Log(5, exception);
			return default;
		}
	}

	public Size GetImageDimension()
	{
		XSSFPictureData xSSFPictureData = PictureData as XSSFPictureData;
		return GetImageDimension(xSSFPictureData.GetPackagePart(), xSSFPictureData.PictureType);
	}

	protected internal override NPOI.OpenXmlFormats.Dml.Spreadsheet.CT_ShapeProperties GetShapeProperties()
	{
		return ctPicture.spPr;
	}

	public new void SetLineStyleColor(int lineStyleColor)
	{
		throw new NotImplementedException();
	}
}
