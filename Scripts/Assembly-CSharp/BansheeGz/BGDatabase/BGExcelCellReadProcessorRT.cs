using NPOI.SS.UserModel;

namespace BansheeGz.BGDatabase;

public interface BGExcelCellReadProcessorRT
{
	void OnRead(ICell cell, BGField field, BGEntity entity);
}
