using System;
using System.Text;
using NPOI.SS.UserModel;

namespace NPOI.SS.Formula.Constant;

public class ErrorConstant
{
	private static readonly ErrorConstant NULL = new ErrorConstant(FormulaError.NULL.Code);

	private static readonly ErrorConstant DIV_0 = new ErrorConstant(FormulaError.DIV0.Code);

	private static readonly ErrorConstant VALUE = new ErrorConstant(FormulaError.VALUE.Code);

	private static readonly ErrorConstant REF = new ErrorConstant(FormulaError.REF.Code);

	private static readonly ErrorConstant NAME = new ErrorConstant(FormulaError.NAME.Code);

	private static readonly ErrorConstant NUM = new ErrorConstant(FormulaError.NUM.Code);

	private static readonly ErrorConstant NA = new ErrorConstant(FormulaError.NA.Code);

	private int _errorCode;

	public int ErrorCode => _errorCode;

	public string Text
	{
		get
		{
			if (FormulaError.IsValidCode(_errorCode))
			{
				return FormulaError.ForInt(_errorCode).String;
			}
			return "unknown error code (" + _errorCode + ")";
		}
	}

	private ErrorConstant(int errorCode)
	{
		_errorCode = errorCode;
	}

	public static ErrorConstant ValueOf(int errorCode)
	{
		if (FormulaError.IsValidCode(errorCode))
		{
			switch ((FormulaErrorEnum)errorCode)
			{
			case FormulaErrorEnum.NULL:
				return NULL;
			case FormulaErrorEnum.DIV_0:
				return DIV_0;
			case FormulaErrorEnum.VALUE:
				return VALUE;
			case FormulaErrorEnum.REF:
				return REF;
			case FormulaErrorEnum.NAME:
				return NAME;
			case FormulaErrorEnum.NUM:
				return NUM;
			case FormulaErrorEnum.NA:
				return NA;
			}
		}
		Console.Error.WriteLine("Warning - Unexpected error code (" + errorCode + ")");
		return new ErrorConstant(errorCode);
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder(64);
		stringBuilder.Append(GetType().Name).Append(" [");
		stringBuilder.Append(Text);
		stringBuilder.Append("]");
		return stringBuilder.ToString();
	}
}
