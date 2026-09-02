using NPOI.SS.Util;

namespace NPOI.SS.UserModel;

public interface IDataValidation
{
	IDataValidationConstraint ValidationConstraint { get; }

	int ErrorStyle { get; set; }

	bool EmptyCellAllowed { get; set; }

	bool SuppressDropDownArrow { get; set; }

	bool ShowPromptBox { get; set; }

	bool ShowErrorBox { get; set; }

	string PromptBoxTitle { get; }

	string PromptBoxText { get; }

	string ErrorBoxTitle { get; }

	string ErrorBoxText { get; }

	CellRangeAddressList Regions { get; }

	void CreatePromptBox(string title, string text);

	void CreateErrorBox(string title, string text);
}
