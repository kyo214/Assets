using NPOI.Util;

namespace NPOI.DDF;

public class EscherColorRef
{
	private int opid = -1;

	private int colorRef;

	private static BitField FLAG_SYS_INDEX = new BitField(268435456);

	private static BitField FLAG_SCHEME_INDEX = new BitField(134217728);

	private static BitField FLAG_SYSTEM_RGB = new BitField(67108864);

	private static BitField FLAG_PALETTE_RGB = new BitField(33554432);

	private static BitField FLAG_PALETTE_INDEX = new BitField(16777216);

	private static BitField FLAG_BLUE = new BitField(16711680);

	private static BitField FLAG_GREEN = new BitField(65280);

	private static BitField FLAG_RED = new BitField(255);

	public EscherColorRef(int colorRef)
	{
		this.colorRef = colorRef;
	}

	public EscherColorRef(byte[] source, int start, int len)
	{
		int num = start;
		if (len == 6)
		{
			opid = LittleEndian.GetUShort(source, num);
			num += 2;
		}
		colorRef = LittleEndian.GetInt(source, num);
	}

	public bool HasSysIndexFlag()
	{
		return FLAG_SYS_INDEX.IsSet(colorRef);
	}

	public void SetSysIndexFlag(bool flag)
	{
		colorRef = FLAG_SYS_INDEX.SetBoolean(colorRef, flag);
	}

	public bool HasSchemeIndexFlag()
	{
		return FLAG_SCHEME_INDEX.IsSet(colorRef);
	}

	public void SetSchemeIndexFlag(bool flag)
	{
		colorRef = FLAG_SCHEME_INDEX.SetBoolean(colorRef, flag);
	}

	public bool HasSystemRGBFlag()
	{
		return FLAG_SYSTEM_RGB.IsSet(colorRef);
	}

	public void SetSystemRGBFlag(bool flag)
	{
		colorRef = FLAG_SYSTEM_RGB.SetBoolean(colorRef, flag);
	}

	public bool HasPaletteRGBFlag()
	{
		return FLAG_PALETTE_RGB.IsSet(colorRef);
	}

	public void SetPaletteRGBFlag(bool flag)
	{
		colorRef = FLAG_PALETTE_RGB.SetBoolean(colorRef, flag);
	}

	public bool HasPaletteIndexFlag()
	{
		return FLAG_PALETTE_INDEX.IsSet(colorRef);
	}

	public void SetPaletteIndexFlag(bool flag)
	{
		colorRef = FLAG_PALETTE_INDEX.SetBoolean(colorRef, flag);
	}

	public int[] GetRGB()
	{
		return new int[3]
		{
			FLAG_RED.GetValue(colorRef),
			FLAG_GREEN.GetValue(colorRef),
			FLAG_BLUE.GetValue(colorRef)
		};
	}

	public SysIndexSource GetSysIndexSource()
	{
		if (!HasSysIndexFlag())
		{
			return null;
		}
		int value = FLAG_RED.GetValue(colorRef);
		SysIndexSource[] array = SysIndexSource.Values();
		foreach (SysIndexSource sysIndexSource in array)
		{
			if (sysIndexSource.value == value)
			{
				return sysIndexSource;
			}
		}
		return null;
	}

	public SysIndexProcedure GetSysIndexProcedure()
	{
		if (!HasSysIndexFlag())
		{
			return null;
		}
		int value = FLAG_RED.GetValue(colorRef);
		SysIndexProcedure[] array = SysIndexProcedure.Values();
		foreach (SysIndexProcedure sysIndexProcedure in array)
		{
			if (sysIndexProcedure != SysIndexProcedure.INVERT_AFTER && sysIndexProcedure != SysIndexProcedure.INVERT_HIGHBIT_AFTER && sysIndexProcedure.mask.IsSet(value))
			{
				return sysIndexProcedure;
			}
		}
		return null;
	}

	public int GetSysIndexInvert()
	{
		if (!HasSysIndexFlag())
		{
			return 0;
		}
		int value = FLAG_GREEN.GetValue(colorRef);
		if (SysIndexProcedure.INVERT_AFTER.mask.IsSet(value))
		{
			return 1;
		}
		if (SysIndexProcedure.INVERT_HIGHBIT_AFTER.mask.IsSet(value))
		{
			return 2;
		}
		return 0;
	}

	public int GetSchemeIndex()
	{
		if (!HasSchemeIndexFlag())
		{
			return -1;
		}
		return FLAG_RED.GetValue(colorRef);
	}

	public int GetPaletteIndex()
	{
		if (!HasPaletteIndexFlag())
		{
			return -1;
		}
		return (FLAG_GREEN.GetValue(colorRef) << 8) & FLAG_RED.GetValue(colorRef);
	}
}
