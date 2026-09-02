namespace NPOI.SS.UserModel;

public enum FormulaErrorEnum : long
{
	NO_ERROR = -1L,
	NULL = 0L,
	DIV_0 = 7L,
	VALUE = 15L,
	REF = 23L,
	NAME = 29L,
	NUM = 36L,
	NA = 42L,
	CIRCULAR_REF = 4294967236L,
	FUNCTION_NOT_IMPLEMENTED = 4294967266L
}
