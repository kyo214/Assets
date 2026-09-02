using System;
using NPOI.SS.UserModel;
using NPOI.Util;

namespace NPOI.SS.Formula.PTG;

public class ErrPtg : ScalarConstantPtg
{
	public static readonly ErrPtg NULL_INTERSECTION = new ErrPtg(FormulaError.NULL.Code);

	public static readonly ErrPtg DIV_ZERO = new ErrPtg(FormulaError.DIV0.Code);

	public static readonly ErrPtg VALUE_INVALID = new ErrPtg(FormulaError.VALUE.Code);

	public static readonly ErrPtg REF_INVALID = new ErrPtg(FormulaError.REF.Code);

	public static readonly ErrPtg NAME_INVALID = new ErrPtg(FormulaError.NAME.Code);

	public static readonly ErrPtg NUM_ERROR = new ErrPtg(FormulaError.NUM.Code);

	public static readonly ErrPtg N_A = new ErrPtg(FormulaError.NA.Code);

	public const byte sid = 28;

	private const int SIZE = 2;

	private int field_1_error_code;

	public override int Size => 2;

	public int ErrorCode => field_1_error_code;

	public static ErrPtg ValueOf(int code)
	{
		return (FormulaErrorEnum)code switch
		{
			FormulaErrorEnum.DIV_0 => DIV_ZERO, 
			FormulaErrorEnum.NA => N_A, 
			FormulaErrorEnum.NAME => NAME_INVALID, 
			FormulaErrorEnum.NULL => NULL_INTERSECTION, 
			FormulaErrorEnum.NUM => NUM_ERROR, 
			FormulaErrorEnum.REF => REF_INVALID, 
			FormulaErrorEnum.VALUE => VALUE_INVALID, 
			_ => throw new InvalidOperationException("Unexpected error code (" + code + ")"), 
		};
	}

	public ErrPtg(int errorCode)
	{
		if (!FormulaError.IsValidCode(errorCode))
		{
			throw new ArgumentException("Invalid error code (" + errorCode + ")");
		}
		field_1_error_code = errorCode;
	}

	public ErrPtg(ILittleEndianInput in1)
		: this(in1.ReadByte())
	{
	}

	public override void Write(ILittleEndianOutput out1)
	{
		out1.WriteByte(28 + base.PtgClass);
		out1.WriteByte((byte)field_1_error_code);
	}

	public override string ToFormulaString()
	{
		return FormulaError.ForInt(field_1_error_code).String;
	}
}
