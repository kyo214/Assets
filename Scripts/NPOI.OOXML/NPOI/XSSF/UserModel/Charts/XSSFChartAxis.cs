using System;
using NPOI.OpenXmlFormats.Dml.Chart;
using NPOI.SS.UserModel.Charts;

namespace NPOI.XSSF.UserModel.Charts;

public abstract class XSSFChartAxis : IChartAxis
{
	protected XSSFChart chart;

	private static double Min_LOG_BASE = 2.0;

	private static double Max_LOG_BASE = 1000.0;

	public abstract long Id { get; }

	public AxisPosition Position
	{
		get
		{
			return toAxisPosition(GetCTAxPos());
		}
		set
		{
			GetCTAxPos().val = fromAxisPosition(value);
		}
	}

	public string NumberFormat
	{
		get
		{
			return GetCTNumFmt().formatCode;
		}
		set
		{
			GetCTNumFmt().formatCode = value;
			GetCTNumFmt().sourceLinked = true;
		}
	}

	public bool IsSetLogBase => GetCTScaling().IsSetLogBase();

	public double LogBase
	{
		get
		{
			return GetCTScaling().logBase?.val ?? 0.0;
		}
		set
		{
			if (value < Min_LOG_BASE || Max_LOG_BASE < value)
			{
				throw new ArgumentException("Axis log base must be between 2 and 1000 (inclusive), got: " + value);
			}
			CT_Scaling cTScaling = GetCTScaling();
			if (cTScaling.IsSetLogBase())
			{
				cTScaling.logBase.val = value;
			}
			else
			{
				cTScaling.AddNewLogBase().val = value;
			}
		}
	}

	public bool IsSetMinimum => GetCTScaling().IsSetMin();

	public double Minimum
	{
		get
		{
			CT_Scaling cTScaling = GetCTScaling();
			if (cTScaling.IsSetMin())
			{
				return cTScaling.min.val;
			}
			return 0.0;
		}
		set
		{
			CT_Scaling cTScaling = GetCTScaling();
			if (cTScaling.IsSetMin())
			{
				cTScaling.min.val = value;
			}
			else
			{
				cTScaling.AddNewMin().val = value;
			}
		}
	}

	public bool IsSetMaximum => GetCTScaling().IsSetMax();

	public double Maximum
	{
		get
		{
			CT_Scaling cTScaling = GetCTScaling();
			if (cTScaling.IsSetMax())
			{
				return cTScaling.max.val;
			}
			return 0.0;
		}
		set
		{
			CT_Scaling cTScaling = GetCTScaling();
			if (cTScaling.IsSetMax())
			{
				cTScaling.max.val = value;
			}
			else
			{
				cTScaling.AddNewMax().val = value;
			}
		}
	}

	public AxisOrientation Orientation
	{
		get
		{
			return toAxisOrientation(GetCTScaling().orientation);
		}
		set
		{
			CT_Scaling cTScaling = GetCTScaling();
			ST_Orientation val = fromAxisOrientation(value);
			if (cTScaling.IsSetOrientation())
			{
				cTScaling.orientation.val = val;
			}
			else
			{
				GetCTScaling().AddNewOrientation().val = val;
			}
		}
	}

	public AxisCrosses Crosses
	{
		get
		{
			return toAxisCrosses(GetCTCrosses());
		}
		set
		{
			GetCTCrosses().val = fromAxisCrosses(value);
		}
	}

	public bool IsVisible
	{
		get
		{
			return GetDelete().val == 0;
		}
		set
		{
			GetDelete().val = ((!value) ? 1 : 0);
		}
	}

	public AxisTickMark MajorTickMark
	{
		get
		{
			return toAxisTickMark(GetMajorCTTickMark());
		}
		set
		{
			GetMajorCTTickMark().val = fromAxisTickMark(value);
		}
	}

	public AxisTickMark MinorTickMark
	{
		get
		{
			return toAxisTickMark(GetMinorCTTickMark());
		}
		set
		{
			GetMinorCTTickMark().val = fromAxisTickMark(value);
		}
	}

	protected XSSFChartAxis(XSSFChart chart)
	{
		this.chart = chart;
	}

	public abstract void CrossAxis(IChartAxis axis);

	protected abstract CT_AxPos GetCTAxPos();

	protected abstract CT_NumFmt GetCTNumFmt();

	protected abstract CT_Scaling GetCTScaling();

	protected abstract CT_Crosses GetCTCrosses();

	protected abstract CT_Boolean GetDelete();

	protected abstract CT_TickMark GetMajorCTTickMark();

	protected abstract CT_TickMark GetMinorCTTickMark();

	private static ST_Orientation fromAxisOrientation(AxisOrientation orientation)
	{
		return orientation switch
		{
			AxisOrientation.MinToMax => ST_Orientation.minMax, 
			AxisOrientation.MaxToMin => ST_Orientation.maxMin, 
			_ => throw new ArgumentException(), 
		};
	}

	private static AxisOrientation toAxisOrientation(CT_Orientation ctOrientation)
	{
		return ctOrientation.val switch
		{
			ST_Orientation.minMax => AxisOrientation.MinToMax, 
			ST_Orientation.maxMin => AxisOrientation.MaxToMin, 
			_ => throw new ArgumentException(), 
		};
	}

	private static ST_Crosses fromAxisCrosses(AxisCrosses crosses)
	{
		return crosses switch
		{
			AxisCrosses.AutoZero => ST_Crosses.autoZero, 
			AxisCrosses.Min => ST_Crosses.min, 
			AxisCrosses.Max => ST_Crosses.max, 
			_ => throw new ArgumentException(), 
		};
	}

	private static AxisCrosses toAxisCrosses(CT_Crosses ctCrosses)
	{
		return ctCrosses.val switch
		{
			ST_Crosses.autoZero => AxisCrosses.AutoZero, 
			ST_Crosses.max => AxisCrosses.Max, 
			ST_Crosses.min => AxisCrosses.Min, 
			_ => throw new ArgumentException(), 
		};
	}

	private static ST_AxPos fromAxisPosition(AxisPosition position)
	{
		return position switch
		{
			AxisPosition.Bottom => ST_AxPos.b, 
			AxisPosition.Left => ST_AxPos.l, 
			AxisPosition.Right => ST_AxPos.r, 
			AxisPosition.Top => ST_AxPos.t, 
			_ => throw new ArgumentException(), 
		};
	}

	private static AxisPosition toAxisPosition(CT_AxPos ctAxPos)
	{
		return ctAxPos.val switch
		{
			ST_AxPos.b => AxisPosition.Bottom, 
			ST_AxPos.l => AxisPosition.Left, 
			ST_AxPos.r => AxisPosition.Right, 
			ST_AxPos.t => AxisPosition.Top, 
			_ => AxisPosition.Bottom, 
		};
	}

	private static ST_TickMark fromAxisTickMark(AxisTickMark tickMark)
	{
		return tickMark switch
		{
			AxisTickMark.None => ST_TickMark.none, 
			AxisTickMark.In => ST_TickMark.@in, 
			AxisTickMark.Out => ST_TickMark.@out, 
			AxisTickMark.Cross => ST_TickMark.cross, 
			_ => throw new ArgumentException("Unknown AxisTickMark: " + tickMark), 
		};
	}

	private static AxisTickMark toAxisTickMark(CT_TickMark ctTickMark)
	{
		return ctTickMark.val switch
		{
			ST_TickMark.none => AxisTickMark.None, 
			ST_TickMark.@in => AxisTickMark.In, 
			ST_TickMark.@out => AxisTickMark.Out, 
			ST_TickMark.cross => AxisTickMark.Cross, 
			_ => AxisTickMark.Cross, 
		};
	}
}
