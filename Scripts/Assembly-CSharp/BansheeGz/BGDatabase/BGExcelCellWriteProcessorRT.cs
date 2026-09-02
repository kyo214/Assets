using NPOI.SS.UserModel;

namespace BansheeGz.BGDatabase;

public interface BGExcelCellWriteProcessorRT
{
	void OnWrite(ICell cell, BGField field, BGEntity entity);
}
