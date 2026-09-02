using System;
using NPOI.OpenXmlFormats.Dml.Chart;
using NPOI.SS.UserModel.Charts;

namespace NPOI.XSSF.UserModel.Charts;

public class XSSFManualLayout : IManualLayout
{
	private CT_ManualLayout layout;

	private static LayoutMode defaultLayoutMode;

	private static LayoutTarget defaultLayoutTarget;

	public XSSFManualLayout(CT_Layout ctLayout)
	{
		InitLayout(ctLayout);
	}

	public XSSFManualLayout(XSSFChart chart)
	{
		CT_PlotArea plotArea = chart.GetCTChart().plotArea;
		InitLayout(plotArea.IsSetLayout() ? plotArea.layout : plotArea.AddNewLayout());
	}

	public CT_ManualLayout GetCTManualLayout()
	{
		return layout;
	}

	public void SetWidthRatio(double ratio)
	{
		if (!layout.IsSetW())
		{
			layout.AddNewW();
		}
		layout.w.val = ratio;
	}

	public double GetWidthRatio()
	{
		if (!layout.IsSetW())
		{
			return 0.0;
		}
		return layout.w.val;
	}

	public void SetHeightRatio(double ratio)
	{
		if (!layout.IsSetH())
		{
			layout.AddNewH();
		}
		layout.h.val = ratio;
	}

	public double GetHeightRatio()
	{
		if (!layout.IsSetH())
		{
			return 0.0;
		}
		return layout.h.val;
	}

	public LayoutTarget GetTarget()
	{
		if (!layout.IsSetLayoutTarget())
		{
			return defaultLayoutTarget;
		}
		return toLayoutTarget(layout.layoutTarget);
	}

	public void SetTarget(LayoutTarget target)
	{
		if (!layout.IsSetLayoutTarget())
		{
			layout.AddNewLayoutTarget();
		}
		layout.layoutTarget.val = fromLayoutTarget(target);
	}

	public LayoutMode GetXMode()
	{
		if (!layout.IsSetXMode())
		{
			return defaultLayoutMode;
		}
		return toLayoutMode(layout.xMode);
	}

	public void SetXMode(LayoutMode mode)
	{
		if (!layout.IsSetXMode())
		{
			layout.AddNewXMode();
		}
		layout.xMode.val = fromLayoutMode(mode);
	}

	public LayoutMode GetYMode()
	{
		if (!layout.IsSetYMode())
		{
			return defaultLayoutMode;
		}
		return toLayoutMode(layout.yMode);
	}

	public void SetYMode(LayoutMode mode)
	{
		if (!layout.IsSetYMode())
		{
			layout.AddNewYMode();
		}
		layout.yMode.val = fromLayoutMode(mode);
	}

	public double GetX()
	{
		if (!layout.IsSetX())
		{
			return 0.0;
		}
		return layout.x.val;
	}

	public void SetX(double x)
	{
		if (!layout.IsSetX())
		{
			layout.AddNewX();
		}
		layout.x.val = x;
	}

	public double GetY()
	{
		if (!layout.IsSetY())
		{
			return 0.0;
		}
		return layout.y.val;
	}

	public void SetY(double y)
	{
		if (!layout.IsSetY())
		{
			layout.AddNewY();
		}
		layout.y.val = y;
	}

	public LayoutMode GetWidthMode()
	{
		if (!layout.IsSetWMode())
		{
			return defaultLayoutMode;
		}
		return toLayoutMode(layout.wMode);
	}

	public void SetWidthMode(LayoutMode mode)
	{
		if (!layout.IsSetWMode())
		{
			layout.AddNewWMode();
		}
		layout.wMode.val = fromLayoutMode(mode);
	}

	public LayoutMode GetHeightMode()
	{
		if (!layout.IsSetHMode())
		{
			return defaultLayoutMode;
		}
		return toLayoutMode(layout.hMode);
	}

	public void SetHeightMode(LayoutMode mode)
	{
		if (!layout.IsSetHMode())
		{
			layout.AddNewHMode();
		}
		layout.hMode.val = fromLayoutMode(mode);
	}

	private void InitLayout(CT_Layout ctLayout)
	{
		if (ctLayout.IsSetManualLayout())
		{
			layout = ctLayout.manualLayout;
		}
		else
		{
			layout = ctLayout.AddNewManualLayout();
		}
	}

	private ST_LayoutMode fromLayoutMode(LayoutMode mode)
	{
		return mode switch
		{
			LayoutMode.Edge => ST_LayoutMode.edge, 
			LayoutMode.Factor => ST_LayoutMode.factor, 
			_ => throw new ArgumentException(), 
		};
	}

	private LayoutMode toLayoutMode(CT_LayoutMode ctLayoutMode)
	{
		return ctLayoutMode.val switch
		{
			ST_LayoutMode.edge => LayoutMode.Edge, 
			ST_LayoutMode.factor => LayoutMode.Factor, 
			_ => throw new ArgumentException(), 
		};
	}

	private ST_LayoutTarget fromLayoutTarget(LayoutTarget target)
	{
		return target switch
		{
			LayoutTarget.Inner => ST_LayoutTarget.inner, 
			LayoutTarget.Outer => ST_LayoutTarget.outer, 
			_ => throw new ArgumentException(), 
		};
	}

	private LayoutTarget toLayoutTarget(CT_LayoutTarget ctLayoutTarget)
	{
		return ctLayoutTarget.val switch
		{
			ST_LayoutTarget.inner => LayoutTarget.Inner, 
			ST_LayoutTarget.outer => LayoutTarget.Outer, 
			_ => throw new ArgumentException(), 
		};
	}
}
