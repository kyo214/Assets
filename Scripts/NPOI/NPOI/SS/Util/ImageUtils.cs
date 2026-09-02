using System;
using System.Drawing;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.Util;

namespace NPOI.SS.Util;

public class ImageUtils
{
	private static POILogger logger = POILogFactory.GetLogger(typeof(ImageUtils));

	public static int PIXEL_DPI = 96;

	public static Size GetImageDimension(Stream is1)
	{
		using Image image = Image.FromStream(is1);
		int[] resolution = GetResolution(image);
		if (resolution[0] == 0)
		{
			resolution[0] = PIXEL_DPI;
		}
		if (resolution[1] == 0)
		{
			resolution[1] = PIXEL_DPI;
		}
		return new Size
		{
			Width = image.Width * PIXEL_DPI / resolution[0],
			Height = image.Height * PIXEL_DPI / resolution[1]
		};
	}

	public static Size GetImageDimension(Stream is1, PictureType type)
	{
		Size result = default;
		if ((uint)(type - 5) <= 2u)
		{
			using (Image image = Image.FromStream(is1))
			{
				int[] resolution = GetResolution(image);
				if (resolution[0] == 0)
				{
					resolution[0] = PIXEL_DPI;
				}
				if (resolution[1] == 0)
				{
					resolution[1] = PIXEL_DPI;
				}
				result.Width = image.Width * PIXEL_DPI / resolution[0];
				result.Height = image.Height * PIXEL_DPI / resolution[1];
				return result;
			}
		}
		logger.Log(5, "Only JPEG, PNG and DIB pictures can be automatically sized");
		return result;
	}

	public static int[] GetResolution(Image r)
	{
		return new int[2]
		{
			(int)r.HorizontalResolution,
			(int)r.VerticalResolution
		};
	}

	public static Size SetPreferredSize(IPicture picture, double scaleX, double scaleY)
	{
		IClientAnchor clientAnchor = picture.ClientAnchor;
		bool flag = clientAnchor is HSSFClientAnchor;
		IPictureData pictureData = picture.PictureData;
		ISheet sheet = picture.Sheet;
		Size imageDimension = GetImageDimension(new MemoryStream(pictureData.Data), pictureData.PictureType);
		Size dimensionFromAnchor = GetDimensionFromAnchor(picture);
		double num = ((scaleX == double.MaxValue) ? ((double)imageDimension.Width) : ((double)(dimensionFromAnchor.Width / Units.EMU_PER_PIXEL) * scaleX));
		double num2 = ((scaleY == double.MaxValue) ? ((double)imageDimension.Height) : ((double)(dimensionFromAnchor.Height / Units.EMU_PER_PIXEL) * scaleY));
		double num3 = 0.0;
		int num4 = clientAnchor.Col1;
		int num5 = 0;
		num3 = sheet.GetColumnWidthInPixels(num4++);
		for (num3 = ((!flag) ? (num3 - (double)clientAnchor.Dx1 / (double)Units.EMU_PER_PIXEL) : (num3 * (1.0 - (double)clientAnchor.Dx1 / 1024.0))); num3 < num; num3 += (double)sheet.GetColumnWidthInPixels(num4++))
		{
		}
		if (num3 > num)
		{
			double num6 = sheet.GetColumnWidthInPixels(--num4);
			double num7 = num3 - num;
			num5 = ((!flag) ? ((int)((num6 - num7) * (double)Units.EMU_PER_PIXEL)) : ((int)((num6 - num7) / num6 * 1024.0)));
			if (num5 < 0)
			{
				num5 = 0;
			}
		}
		clientAnchor.Col2 = num4;
		clientAnchor.Dx2 = num5;
		double num8 = 0.0;
		int num9 = clientAnchor.Row1;
		int num10 = 0;
		num8 = GetRowHeightInPixels(sheet, num9++);
		for (num8 = ((!flag) ? (num8 - (double)clientAnchor.Dy1 / (double)Units.EMU_PER_PIXEL) : (num8 * (1.0 - (double)clientAnchor.Dy1 / 256.0))); num8 < num2; num8 += GetRowHeightInPixels(sheet, num9++))
		{
		}
		if (num8 > num2)
		{
			double rowHeightInPixels = GetRowHeightInPixels(sheet, --num9);
			double num11 = num8 - num2;
			num10 = ((!flag) ? ((int)((rowHeightInPixels - num11) * (double)Units.EMU_PER_PIXEL)) : ((int)((rowHeightInPixels - num11) / rowHeightInPixels * 256.0)));
			if (num10 < 0)
			{
				num10 = 0;
			}
		}
		clientAnchor.Row2 = num9;
		clientAnchor.Dy2 = num10;
		return new Size((int)Math.Round(num * (double)Units.EMU_PER_PIXEL), (int)Math.Round(num2 * (double)Units.EMU_PER_PIXEL));
	}

	public static Size GetDimensionFromAnchor(IPicture picture)
	{
		IClientAnchor clientAnchor = picture.ClientAnchor;
		bool flag = clientAnchor is HSSFClientAnchor;
		ISheet sheet = picture.Sheet;
		double num = 0.0;
		int col = clientAnchor.Col1;
		num = sheet.GetColumnWidthInPixels(col++);
		num = ((!flag) ? (num - (double)clientAnchor.Dx1 / (double)Units.EMU_PER_PIXEL) : (num * (1.0 - (double)clientAnchor.Dx1 / 1024.0)));
		while (col < clientAnchor.Col2)
		{
			num += (double)sheet.GetColumnWidthInPixels(col++);
		}
		num = ((!flag) ? (num + (double)clientAnchor.Dx2 / (double)Units.EMU_PER_PIXEL) : (num + (double)(sheet.GetColumnWidthInPixels(col) * (float)clientAnchor.Dx2) / 1024.0));
		double num2 = 0.0;
		int row = clientAnchor.Row1;
		num2 = GetRowHeightInPixels(sheet, row++);
		num2 = ((!flag) ? (num2 - (double)clientAnchor.Dy1 / (double)Units.EMU_PER_PIXEL) : (num2 * (1.0 - (double)clientAnchor.Dy1 / 256.0)));
		while (row < clientAnchor.Row2)
		{
			num2 += GetRowHeightInPixels(sheet, row++);
		}
		num2 = ((!flag) ? (num2 + (double)clientAnchor.Dy2 / (double)Units.EMU_PER_PIXEL) : (num2 + GetRowHeightInPixels(sheet, row) * (double)clientAnchor.Dy2 / 256.0));
		num *= (double)Units.EMU_PER_PIXEL;
		num2 *= (double)Units.EMU_PER_PIXEL;
		return new Size((int)Math.Round(num), (int)Math.Round(num2));
	}

	private static double GetRowHeightInPixels(ISheet sheet, int rowNum)
	{
		return Units.ToEMU(sheet.GetRow(rowNum)?.HeightInPoints ?? sheet.DefaultRowHeightInPoints) / Units.EMU_PER_PIXEL;
	}
}
