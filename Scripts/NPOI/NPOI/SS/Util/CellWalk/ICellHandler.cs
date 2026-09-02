using NPOI.SS.UserModel;

namespace NPOI.SS.Util.CellWalk;

public interface ICellHandler
{
	void OnCell(ICell cell, ICellWalkContext ctx);
}
