namespace NPOI.SS.UserModel;

public interface IName
{
	string SheetName { get; }

	string NameName { get; set; }

	string RefersToFormula { get; set; }

	bool IsFunctionName { get; }

	bool IsDeleted { get; }

	int SheetIndex { get; set; }

	string Comment { get; set; }

	void SetFunction(bool value);
}
