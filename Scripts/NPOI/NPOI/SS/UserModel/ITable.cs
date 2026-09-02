namespace NPOI.SS.UserModel;

public interface ITable
{
	int StartColIndex { get; }

	int StartRowIndex { get; }

	int EndColIndex { get; }

	int EndRowIndex { get; }

	string Name { get; }

	string SheetName { get; }

	bool IsHasTotalsRow { get; }

	int FindColumnIndex(string columnHeader);
}
