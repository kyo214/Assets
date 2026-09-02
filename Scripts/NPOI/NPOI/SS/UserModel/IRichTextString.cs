namespace NPOI.SS.UserModel;

public interface IRichTextString
{
	string String { get; }

	int Length { get; }

	int NumFormattingRuns { get; }

	void ApplyFont(int startIndex, int endIndex, short fontIndex);

	void ApplyFont(int startIndex, int endIndex, IFont font);

	void ApplyFont(IFont font);

	void ClearFormatting();

	int GetIndexOfFormattingRun(int index);

	void ApplyFont(short fontIndex);
}
