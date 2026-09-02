using System;
using NPOI.SS.UserModel;

namespace NPOI.HSSF.UserModel;

public class HSSFDateUtil : DateUtil
{
	public new static int AbsoluteDay(DateTime cal, bool use1904windowing)
	{
		return DateUtil.AbsoluteDay(cal, use1904windowing);
	}
}
