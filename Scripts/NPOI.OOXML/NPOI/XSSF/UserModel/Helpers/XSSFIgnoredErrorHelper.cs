using System;
using System.Collections.Generic;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS.UserModel;

namespace NPOI.XSSF.UserModel.Helpers;

public class XSSFIgnoredErrorHelper
{
	public static bool IsSet(IgnoredErrorType errorType, CT_IgnoredError error)
	{
		return errorType switch
		{
			IgnoredErrorType.CalculatedColumn => error.calculatedColumn, 
			IgnoredErrorType.EmptyCellReference => error.emptyCellReference, 
			IgnoredErrorType.EvaluationError => error.evalError, 
			IgnoredErrorType.Formula => error.formula, 
			IgnoredErrorType.FormulaRange => error.formulaRange, 
			IgnoredErrorType.ListDataValidation => error.listDataValidation, 
			IgnoredErrorType.NumberStoredAsText => error.numberStoredAsText, 
			IgnoredErrorType.TwoDigitTextYear => error.twoDigitTextYear, 
			IgnoredErrorType.UnlockedFormula => error.unlockedFormula, 
			_ => throw new InvalidOperationException(), 
		};
	}

	public static void Set(IgnoredErrorType errorType, CT_IgnoredError error)
	{
		switch (errorType)
		{
		case IgnoredErrorType.CalculatedColumn:
			error.calculatedColumn = true;
			break;
		case IgnoredErrorType.EmptyCellReference:
			error.emptyCellReference = true;
			break;
		case IgnoredErrorType.EvaluationError:
			error.evalError = true;
			break;
		case IgnoredErrorType.Formula:
			error.formula = true;
			break;
		case IgnoredErrorType.FormulaRange:
			error.formulaRange = true;
			break;
		case IgnoredErrorType.ListDataValidation:
			error.listDataValidation = true;
			break;
		case IgnoredErrorType.NumberStoredAsText:
			error.numberStoredAsText = true;
			break;
		case IgnoredErrorType.TwoDigitTextYear:
			error.twoDigitTextYear = true;
			break;
		case IgnoredErrorType.UnlockedFormula:
			error.unlockedFormula = true;
			break;
		default:
			throw new InvalidOperationException();
		}
	}

	public static void AddIgnoredErrors(CT_IgnoredError err, string ref1, params IgnoredErrorType[] ignoredErrorTypes)
	{
		err.sqref.Clear();
		err.sqref.Add(ref1);
		for (int i = 0; i < ignoredErrorTypes.Length; i++)
		{
			Set(ignoredErrorTypes[i], err);
		}
	}

	public static ISet<IgnoredErrorType> GetErrorTypes(CT_IgnoredError err)
	{
		ISet<IgnoredErrorType> set = new HashSet<IgnoredErrorType>();
		IgnoredErrorType[] values = IgnoredErrorTypeValues.Values;
		foreach (IgnoredErrorType ignoredErrorType in values)
		{
			if (IsSet(ignoredErrorType, err))
			{
				set.Add(ignoredErrorType);
			}
		}
		return set;
	}
}
