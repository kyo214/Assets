using System;
using NPOI.DDF;
using NPOI.HSSF.UserModel;

namespace NPOI.HSSF.Model;

public class ConvertAnchor
{
	public static EscherRecord CreateAnchor(HSSFAnchor userAnchor)
	{
		if (userAnchor is HSSFClientAnchor)
		{
			HSSFClientAnchor hSSFClientAnchor = (HSSFClientAnchor)userAnchor;
			return new EscherClientAnchorRecord
			{
				RecordId = -4080,
				Options = 0,
				Flag = (short)hSSFClientAnchor.AnchorType,
				Col1 = (short)Math.Min(hSSFClientAnchor.Col1, hSSFClientAnchor.Col2),
				Dx1 = (short)hSSFClientAnchor.Dx1,
				Row1 = (short)Math.Min(hSSFClientAnchor.Row1, hSSFClientAnchor.Row2),
				Dy1 = (short)hSSFClientAnchor.Dy1,
				Col2 = (short)Math.Max(hSSFClientAnchor.Col1, hSSFClientAnchor.Col2),
				Dx2 = (short)hSSFClientAnchor.Dx2,
				Row2 = (short)Math.Max(hSSFClientAnchor.Row1, hSSFClientAnchor.Row2),
				Dy2 = (short)hSSFClientAnchor.Dy2
			};
		}
		HSSFChildAnchor hSSFChildAnchor = (HSSFChildAnchor)userAnchor;
		return new EscherChildAnchorRecord
		{
			RecordId = -4081,
			Options = 0,
			Dx1 = (short)Math.Min(hSSFChildAnchor.Dx1, hSSFChildAnchor.Dx2),
			Dy1 = (short)Math.Min(hSSFChildAnchor.Dy1, hSSFChildAnchor.Dy2),
			Dx2 = (short)Math.Max(hSSFChildAnchor.Dx2, hSSFChildAnchor.Dx1),
			Dy2 = (short)Math.Max(hSSFChildAnchor.Dy2, hSSFChildAnchor.Dy1)
		};
	}
}
