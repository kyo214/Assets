namespace NPOI.SS.UserModel;

public interface IDataValidationConstraint
{
	int Operator { get; set; }

	string[] ExplicitListValues { get; set; }

	string Formula1 { get; set; }

	string Formula2 { get; set; }

	int GetValidationType();
}
