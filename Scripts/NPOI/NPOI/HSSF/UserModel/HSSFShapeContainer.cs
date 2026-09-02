using System.Collections;
using System.Collections.Generic;

namespace NPOI.HSSF.UserModel;

public interface HSSFShapeContainer : IEnumerable<HSSFShape>, IEnumerable
{
	IList<HSSFShape> Children { get; }

	int X1 { get; }

	int Y1 { get; }

	int X2 { get; }

	int Y2 { get; }

	void AddShape(HSSFShape shape);

	void SetCoordinates(int x1, int y1, int x2, int y2);

	void Clear();

	bool RemoveShape(HSSFShape shape);
}
