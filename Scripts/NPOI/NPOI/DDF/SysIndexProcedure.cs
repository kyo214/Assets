using NPOI.Util;

namespace NPOI.DDF;

public class SysIndexProcedure
{
	public static SysIndexProcedure DARKEN_COLOR = new SysIndexProcedure(1);

	public static SysIndexProcedure LIGHTEN_COLOR = new SysIndexProcedure(2);

	public static SysIndexProcedure ADD_GRAY_LEVEL = new SysIndexProcedure(3);

	public static SysIndexProcedure SUB_GRAY_LEVEL = new SysIndexProcedure(4);

	public static SysIndexProcedure REVERSE_GRAY_LEVEL = new SysIndexProcedure(5);

	public static SysIndexProcedure THRESHOLD = new SysIndexProcedure(6);

	public static SysIndexProcedure INVERT_AFTER = new SysIndexProcedure(32);

	public static SysIndexProcedure INVERT_HIGHBIT_AFTER = new SysIndexProcedure(64);

	internal BitField mask;

	internal static SysIndexProcedure[] Values()
	{
		return new SysIndexProcedure[8] { DARKEN_COLOR, LIGHTEN_COLOR, ADD_GRAY_LEVEL, SUB_GRAY_LEVEL, REVERSE_GRAY_LEVEL, THRESHOLD, INVERT_AFTER, INVERT_HIGHBIT_AFTER };
	}

	internal SysIndexProcedure(int mask)
	{
		this.mask = new BitField(mask);
	}
}
