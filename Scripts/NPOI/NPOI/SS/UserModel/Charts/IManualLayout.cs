namespace NPOI.SS.UserModel.Charts;

public interface IManualLayout
{
	void SetTarget(LayoutTarget target);

	LayoutTarget GetTarget();

	void SetXMode(LayoutMode mode);

	LayoutMode GetXMode();

	void SetYMode(LayoutMode mode);

	LayoutMode GetYMode();

	double GetX();

	void SetX(double x);

	double GetY();

	void SetY(double y);

	void SetWidthMode(LayoutMode mode);

	LayoutMode GetWidthMode();

	void SetHeightMode(LayoutMode mode);

	LayoutMode GetHeightMode();

	void SetWidthRatio(double ratio);

	double GetWidthRatio();

	void SetHeightRatio(double ratio);

	double GetHeightRatio();
}
