using System;
using NPOI.OpenXmlFormats.Wordprocessing;
using NPOI.XWPF.UserModel;

namespace NPOI.Util;

public static class EnumConverter
{
	public static ST_Jc ValueOf(ParagraphAlignment val)
	{
		return val switch
		{
			ParagraphAlignment.BOTH => ST_Jc.both, 
			ParagraphAlignment.CENTER => ST_Jc.center, 
			ParagraphAlignment.DISTRIBUTE => ST_Jc.distribute, 
			ParagraphAlignment.HIGH_KASHIDA => ST_Jc.highKashida, 
			ParagraphAlignment.LOW_KASHIDA => ST_Jc.lowKashida, 
			ParagraphAlignment.MEDIUM_KASHIDA => ST_Jc.mediumKashida, 
			ParagraphAlignment.NUM_TAB => ST_Jc.numTab, 
			ParagraphAlignment.RIGHT => ST_Jc.right, 
			ParagraphAlignment.THAI_DISTRIBUTE => ST_Jc.thaiDistribute, 
			_ => ST_Jc.left, 
		};
	}

	public static ParagraphAlignment ValueOf(ST_Jc val)
	{
		return val switch
		{
			ST_Jc.both => ParagraphAlignment.BOTH, 
			ST_Jc.center => ParagraphAlignment.CENTER, 
			ST_Jc.distribute => ParagraphAlignment.DISTRIBUTE, 
			_ => ParagraphAlignment.LEFT, 
		};
	}

	public static T ValueOf<T, F>(F val)
	{
		string name = Enum.GetName(val.GetType(), val);
		return (T)Enum.Parse(typeof(T), name, ignoreCase: true);
	}
}
