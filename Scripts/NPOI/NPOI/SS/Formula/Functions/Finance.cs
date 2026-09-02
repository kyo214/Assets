using System;

namespace NPOI.SS.Formula.Functions;

public class Finance
{
	public static double PMT(double r, int nper, double pv, double fv, int type)
	{
		return (0.0 - r) * (pv * Math.Pow(1.0 + r, nper) + fv) / ((1.0 + r * (double)type) * (Math.Pow(1.0 + r, nper) - 1.0));
	}

	public static double PMT(double r, int nper, double pv, double fv)
	{
		return PMT(r, nper, pv, fv, 0);
	}

	public static double PMT(double r, int nper, double pv)
	{
		return PMT(r, nper, pv, 0.0);
	}

	public static double IPMT(double r, int per, int nper, double pv, double fv, int type)
	{
		double num = FV(r, per - 1, PMT(r, nper, pv, fv, type), pv, type) * r;
		if (type == 1)
		{
			num /= 1.0 + r;
		}
		return num;
	}

	public static double IPMT(double r, int per, int nper, double pv, double fv)
	{
		return IPMT(r, per, nper, pv, fv, 0);
	}

	public static double IPMT(double r, int per, int nper, double pv)
	{
		return IPMT(r, per, nper, pv, 0.0);
	}

	public static double PPMT(double r, int per, int nper, double pv, double fv, int type)
	{
		return PMT(r, nper, pv, fv, type) - IPMT(r, per, nper, pv, fv, type);
	}

	public static double PPMT(double r, int per, int nper, double pv, double fv)
	{
		return PMT(r, nper, pv, fv) - IPMT(r, per, nper, pv, fv);
	}

	public static double PPMT(double r, int per, int nper, double pv)
	{
		return PMT(r, nper, pv) - IPMT(r, per, nper, pv);
	}

	public static double FV(double r, int nper, double pmt, double pv, int type)
	{
		return 0.0 - (pv * Math.Pow(1.0 + r, nper) + pmt * (1.0 + r * (double)type) * (Math.Pow(1.0 + r, nper) - 1.0) / r);
	}

	public static double FV(double r, int nper, double c, double pv)
	{
		return FV(r, nper, c, pv, 0);
	}
}
