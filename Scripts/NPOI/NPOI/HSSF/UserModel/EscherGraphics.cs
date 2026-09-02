using System;
using System.Drawing;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.Util;

namespace NPOI.HSSF.UserModel;

public class EscherGraphics : IDisposable
{
	private HSSFShapeGroup escherGroup;

	private HSSFWorkbook workbook;

	private float verticalPointsPerPixel = 1f;

	private float verticalPixelsPerPoint;

	private Color foreground;

	private Color background = Color.White;

	private Font font;

	private static POILogger Logger = POILogFactory.GetLogger(typeof(EscherGraphics));

	public Rectangle Clip => ClipBounds;

	public Rectangle ClipBounds => Rectangle.Empty;

	public Color Color => foreground;

	public Font Font => font;

	public Color Background
	{
		get
		{
			return background;
		}
		set
		{
			background = value;
		}
	}

	public EscherGraphics(HSSFShapeGroup escherGroup, HSSFWorkbook workbook, Color forecolor, float verticalPointsPerPixel)
	{
		this.escherGroup = escherGroup;
		this.workbook = workbook;
		this.verticalPointsPerPixel = verticalPointsPerPixel;
		verticalPixelsPerPoint = 1f / verticalPointsPerPixel;
		font = new Font("Arial", 10f);
		foreground = forecolor;
	}

	private EscherGraphics(HSSFShapeGroup escherGroup, HSSFWorkbook workbook, Color foreground, Font font, float verticalPointsPerPixel)
	{
		this.escherGroup = escherGroup;
		this.workbook = workbook;
		this.foreground = foreground;
		this.font = font;
		this.verticalPointsPerPixel = verticalPointsPerPixel;
		verticalPixelsPerPoint = 1f / verticalPointsPerPixel;
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposing && font != null)
		{
			font.Dispose();
			font = null;
		}
	}

	public void ClearRect(int x, int y, int width, int height)
	{
		Color color = foreground;
		SetColor(background);
		FillRect(x, y, width, height);
		SetColor(color);
	}

	public void ClipRect(int x, int y, int width, int height)
	{
		if (Logger.Check(5))
		{
			Logger.Log(5, "clipRect not supported");
		}
	}

	public void CopyArea(int x, int y, int width, int height, int dx, int dy)
	{
		if (Logger.Check(5))
		{
			Logger.Log(5, "copyArea not supported");
		}
	}

	public EscherGraphics Create()
	{
		return new EscherGraphics(escherGroup, workbook, foreground, font, verticalPointsPerPixel);
	}

	public void DrawArc(int x, int y, int width, int height, int startAngle, int arcAngle)
	{
		if (Logger.Check(5))
		{
			Logger.Log(5, "DrawArc not supported");
		}
	}

	public bool DrawImage(Image img, int dx1, int dy1, int dx2, int dy2, int sx1, int sy1, int sx2, int sy2, Color bgcolor)
	{
		if (Logger.Check(5))
		{
			Logger.Log(5, "DrawImage not supported");
		}
		throw new NotImplementedException();
	}

	public bool DrawImage(Image img, int dx1, int dy1, int dx2, int dy2, int sx1, int sy1, int sx2, int sy2)
	{
		if (Logger.Check(5))
		{
			Logger.Log(5, "DrawImage not supported");
		}
		throw new NotImplementedException();
	}

	public bool DrawImage(Image image, int i, int j, int k, int l, Color color)
	{
		return DrawImage(image, i, j, i + k, j + l, 0, 0, image.Width, image.Height, color);
	}

	public bool DrawImage(Image image, int i, int j, int k, int l)
	{
		return DrawImage(image, i, j, i + k, j + l, 0, 0, image.Width, image.Height);
	}

	public bool DrawImage(Image image, int i, int j, Color color)
	{
		return DrawImage(image, i, j, image.Width, image.Height, color);
	}

	public bool DrawImage(Image image, int i, int j)
	{
		return DrawImage(image, i, j, image.Width, image.Height);
	}

	public void DrawLine(int x1, int y1, int x2, int y2)
	{
		DrawLine(x1, y1, x2, y2, 0);
	}

	public void DrawLine(int x1, int y1, int x2, int y2, int width)
	{
		HSSFSimpleShape hSSFSimpleShape = escherGroup.CreateShape(new HSSFChildAnchor(x1, y1, x2, y2));
		hSSFSimpleShape.ShapeType = 20;
		hSSFSimpleShape.LineWidth = width;
		hSSFSimpleShape.SetLineStyleColor(foreground.R, foreground.G, foreground.B);
	}

	public void DrawOval(int x, int y, int width, int height)
	{
		HSSFSimpleShape hSSFSimpleShape = escherGroup.CreateShape(new HSSFChildAnchor(x, y, x + width, y + height));
		hSSFSimpleShape.ShapeType = 3;
		hSSFSimpleShape.LineWidth = 0;
		hSSFSimpleShape.SetLineStyleColor(foreground.R, foreground.G, foreground.B);
		hSSFSimpleShape.IsNoFill = true;
	}

	public void DrawPolygon(int[] xPoints, int[] yPoints, int nPoints)
	{
		int num = FindBiggest(xPoints);
		int num2 = FindBiggest(yPoints);
		int num3 = FindSmallest(xPoints);
		int num4 = FindSmallest(yPoints);
		HSSFPolygon hSSFPolygon = escherGroup.CreatePolygon(new HSSFChildAnchor(num3, num4, num, num2));
		hSSFPolygon.SetPolygonDrawArea(num - num3, num2 - num4);
		hSSFPolygon.SetPoints(AddToAll(xPoints, -num3), AddToAll(yPoints, -num4));
		hSSFPolygon.SetLineStyleColor(foreground.R, foreground.G, foreground.B);
		hSSFPolygon.LineWidth = 0;
		hSSFPolygon.IsNoFill = true;
	}

	private int[] AddToAll(int[] values, int amount)
	{
		int[] array = new int[values.Length];
		for (int i = 0; i < values.Length; i++)
		{
			array[i] = values[i] + amount;
		}
		return array;
	}

	public void DrawPolyline(int[] xPoints, int[] yPoints, int nPoints)
	{
		if (Logger.Check(5))
		{
			Logger.Log(5, "DrawPolyline not supported");
		}
	}

	public void DrawRect(int x, int y, int width, int height)
	{
		if (Logger.Check(5))
		{
			Logger.Log(5, "DrawRect not supported");
		}
	}

	public void DrawRoundRect(int x, int y, int width, int height, int arcWidth, int arcHeight)
	{
		if (Logger.Check(5))
		{
			Logger.Log(5, "DrawRoundRect not supported");
		}
	}

	public void DrawString(string str, int x, int y)
	{
		if (string.IsNullOrEmpty(str))
		{
			return;
		}
		using Font font = new Font(this.font.Name.Equals("SansSerif") ? "Arial" : this.font.Name, (int)(this.font.Size / verticalPixelsPerPoint), this.font.Style);
		int num = StaticFontMetrics.GetFontDetails(font).GetStringWidth(str) * 8 + 12;
		int num2 = (int)(this.font.Size / verticalPixelsPerPoint + 6f) * 2;
		y -= Convert.ToInt32(this.font.Size / verticalPixelsPerPoint + 2f * verticalPixelsPerPoint);
		HSSFTextbox hSSFTextbox = escherGroup.CreateTextbox(new HSSFChildAnchor(x, y, x + num, y + num2));
		hSSFTextbox.IsNoFill = true;
		hSSFTextbox.LineStyle = LineStyle.None;
		HSSFRichTextString hSSFRichTextString = new HSSFRichTextString(str);
		HSSFFont hSSFFont = MatchFont(font);
		hSSFRichTextString.ApplyFont(hSSFFont);
		hSSFTextbox.String = hSSFRichTextString;
	}

	private HSSFFont MatchFont(Font font)
	{
		HSSFColor hSSFColor = workbook.GetCustomPalette().FindColor(foreground.R, foreground.G, foreground.B);
		if (hSSFColor == null)
		{
			hSSFColor = workbook.GetCustomPalette().FindSimilarColor(foreground.R, foreground.G, foreground.B);
		}
		bool bold = font.Bold;
		bool italic = font.Italic;
		HSSFFont hSSFFont = (HSSFFont)workbook.FindFont((short)(bold ? 700 : 400), hSSFColor.Indexed, (short)(font.Size * 20f), font.Name, italic, strikeout: false, FontSuperScript.None, FontUnderlineType.None);
		if (hSSFFont == null)
		{
			hSSFFont = (HSSFFont)workbook.CreateFont();
			hSSFFont.Boldweight = (short)(bold ? 700 : 0);
			hSSFFont.Color = hSSFColor.Indexed;
			hSSFFont.FontHeight = (short)(font.Size * 20f);
			hSSFFont.FontName = font.Name;
			hSSFFont.IsItalic = italic;
			hSSFFont.IsStrikeout = false;
			hSSFFont.TypeOffset = FontSuperScript.None;
			hSSFFont.Underline = FontUnderlineType.None;
		}
		return hSSFFont;
	}

	public void FillArc(int x, int y, int width, int height, int startAngle, int arcAngle)
	{
		if (Logger.Check(5))
		{
			Logger.Log(5, "FillArc not supported");
		}
	}

	public void FillOval(int x, int y, int width, int height)
	{
		HSSFSimpleShape hSSFSimpleShape = escherGroup.CreateShape(new HSSFChildAnchor(x, y, x + width, y + height));
		hSSFSimpleShape.ShapeType = 3;
		hSSFSimpleShape.LineStyle = LineStyle.None;
		hSSFSimpleShape.SetFillColor(foreground.R, foreground.G, foreground.B);
		hSSFSimpleShape.SetLineStyleColor(foreground.R, foreground.G, foreground.B);
		hSSFSimpleShape.IsNoFill = false;
	}

	public void FillPolygon(int[] xPoints, int[] yPoints, int nPoints)
	{
		int num = FindBiggest(xPoints);
		int num2 = FindBiggest(yPoints);
		int num3 = FindSmallest(xPoints);
		int num4 = FindSmallest(yPoints);
		HSSFPolygon hSSFPolygon = escherGroup.CreatePolygon(new HSSFChildAnchor(num3, num4, num, num2));
		hSSFPolygon.SetPolygonDrawArea(num - num3, num2 - num4);
		hSSFPolygon.SetPoints(AddToAll(xPoints, -num3), AddToAll(yPoints, -num4));
		hSSFPolygon.SetLineStyleColor(foreground.R, foreground.G, foreground.B);
		hSSFPolygon.SetFillColor(foreground.R, foreground.G, foreground.B);
	}

	private int FindBiggest(int[] values)
	{
		int num = int.MinValue;
		for (int i = 0; i < values.Length; i++)
		{
			if (values[i] > num)
			{
				num = values[i];
			}
		}
		return num;
	}

	private int FindSmallest(int[] values)
	{
		int num = int.MaxValue;
		for (int i = 0; i < values.Length; i++)
		{
			if (values[i] < num)
			{
				num = values[i];
			}
		}
		return num;
	}

	public void FillRect(int x, int y, int width, int height)
	{
		HSSFSimpleShape hSSFSimpleShape = escherGroup.CreateShape(new HSSFChildAnchor(x, y, x + width, y + height));
		hSSFSimpleShape.ShapeType = 1;
		hSSFSimpleShape.LineStyle = LineStyle.None;
		hSSFSimpleShape.SetFillColor(foreground.R, foreground.G, foreground.B);
		hSSFSimpleShape.SetLineStyleColor(foreground.R, foreground.G, foreground.B);
	}

	public void FillRoundRect(int x, int y, int width, int height, int arcWidth, int arcHeight)
	{
		if (Logger.Check(5))
		{
			Logger.Log(5, "FillRoundRect not supported");
		}
	}

	public void SetClip(int x, int y, int width, int height)
	{
		SetClip(new Rectangle(x, y, width, height));
	}

	public void SetClip(Rectangle shape)
	{
		throw new NotImplementedException();
	}

	public void SetColor(Color color)
	{
		foreground = color;
	}

	public void SetFont(Font f)
	{
		font = f;
	}

	public void SetPaintMode()
	{
		if (Logger.Check(5))
		{
			Logger.Log(5, "SetPaintMode not supported");
		}
		throw new NotImplementedException();
	}

	public void SetXORMode(Color color)
	{
		if (Logger.Check(5))
		{
			Logger.Log(5, "SetXORMode not supported");
		}
		throw new NotImplementedException();
	}

	public void Translate(int x, int y)
	{
		if (Logger.Check(5))
		{
			Logger.Log(5, "translate not supported");
		}
		throw new NotImplementedException();
	}

	private HSSFShapeGroup GetEscherGraphics()
	{
		return escherGroup;
	}
}
