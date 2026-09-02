namespace NPOI.SS.UserModel;

public interface IConditionalFormattingThreshold
{
	RangeType RangeType { get; set; }

	string Formula { get; set; }

	double? Value { get; set; }
}
