using System;

namespace NPOI.Util;

public class Units
{
	public static int EMU_PER_PIXEL = 9525;

	public static int EMU_PER_POINT = 12700;

	public static int EMU_PER_CENTIMETER = 360000;

	public static int MASTER_DPI = 576;

	public static int PIXEL_DPI = 96;

	public static int POINT_DPI = 72;

	public static int ToEMU(double value)
	{
		return (int)Math.Round((double)EMU_PER_POINT * value);
	}

	public static int PixelToEMU(int pixels)
	{
		return pixels * EMU_PER_PIXEL;
	}

	public static double ToPoints(long emu)
	{
		return (double)emu / (double)EMU_PER_POINT;
	}

	public static double FixedPointToDecimal(int fixedPoint)
	{
		int num = fixedPoint >> 16;
		int num2 = fixedPoint & 0xFFFF;
		return (double)num + (double)num2 / 65536.0;
	}

	public static int DoubleToFixedPoint(double floatPoint)
	{
		double num = floatPoint % 1.0;
		int num2 = (int)Math.Floor(floatPoint - num);
		int num3 = (int)Math.Round(num * 65536.0, MidpointRounding.ToEven);
		return (num2 << 16) | (num3 & 0xFFFF);
	}

	public static double MasterToPoints(int masterDPI)
	{
		return (double)masterDPI * (double)POINT_DPI / (double)MASTER_DPI;
	}

	public static int PointsToMaster(double points)
	{
		points *= (double)MASTER_DPI;
		points /= (double)POINT_DPI;
		return (int)Math.Round(points, MidpointRounding.ToEven);
	}

	public static int PointsToPixel(double points)
	{
		points *= (double)PIXEL_DPI;
		points /= (double)POINT_DPI;
		return (int)Math.Round(points, MidpointRounding.ToEven);
	}

	public static double PixelToPoints(int pixel)
	{
		return (double)pixel * (double)POINT_DPI / (double)PIXEL_DPI;
	}
}
