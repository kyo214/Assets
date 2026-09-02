using System;
using NPOI.SS.Formula.PTG;

namespace NPOI.SS.Formula;

internal class OperandClassTransformer
{
	private FormulaType _formulaType;

	public OperandClassTransformer(FormulaType formulaType)
	{
		_formulaType = formulaType;
	}

	public void TransformFormula(ParseNode rootNode)
	{
		byte desiredOperandClass;
		switch (_formulaType)
		{
		case FormulaType.Cell:
			desiredOperandClass = 32;
			break;
		case FormulaType.Array:
			desiredOperandClass = 64;
			break;
		case FormulaType.NamedRange:
		case FormulaType.DataValidationList:
			desiredOperandClass = 0;
			break;
		default:
			throw new Exception("Incomplete code - formula type (" + _formulaType.ToString() + ") not supported yet");
		}
		TransformNode(rootNode, desiredOperandClass, callerForceArrayFlag: false);
	}

	private void TransformNode(ParseNode node, byte desiredOperandClass, bool callerForceArrayFlag)
	{
		Ptg ptg = node.GetToken();
		ParseNode[] children = node.GetChildren();
		if (IsSimpleValueFunction(ptg))
		{
			bool callerForceArrayFlag2 = desiredOperandClass == 64;
			for (int i = 0; i < children.Length; i++)
			{
				TransformNode(children[i], desiredOperandClass, callerForceArrayFlag2);
			}
			SetSimpleValueFuncClass((AbstractFunctionPtg)ptg, desiredOperandClass, callerForceArrayFlag);
			return;
		}
		if (IsSingleArgSum(ptg))
		{
			ptg = FuncVarPtg.SUM;
		}
		if (ptg is ValueOperatorPtg || ptg is ControlPtg || ptg is MemFuncPtg || ptg is MemAreaPtg || ptg is UnionPtg || ptg is IntersectionPtg)
		{
			byte desiredOperandClass2 = (byte)((desiredOperandClass == 0) ? 32 : desiredOperandClass);
			for (int j = 0; j < children.Length; j++)
			{
				TransformNode(children[j], desiredOperandClass2, callerForceArrayFlag);
			}
		}
		else if (ptg is AbstractFunctionPtg)
		{
			TransformFunctionNode((AbstractFunctionPtg)ptg, children, desiredOperandClass, callerForceArrayFlag);
		}
		else if (children.Length != 0)
		{
			if (!(ptg is OperationPtg))
			{
				throw new InvalidOperationException("Node should not have any children");
			}
		}
		else if (!ptg.IsBaseToken)
		{
			ptg.PtgClass = TransformClass(ptg.PtgClass, desiredOperandClass, callerForceArrayFlag);
		}
	}

	private static bool IsSingleArgSum(Ptg token)
	{
		if (token is AttrPtg)
		{
			return ((AttrPtg)token).IsSum;
		}
		return false;
	}

	private static bool IsSimpleValueFunction(Ptg token)
	{
		if (token is AbstractFunctionPtg)
		{
			AbstractFunctionPtg abstractFunctionPtg = (AbstractFunctionPtg)token;
			if (abstractFunctionPtg.DefaultOperandClass != 32)
			{
				return false;
			}
			for (int num = abstractFunctionPtg.NumberOfOperands - 1; num >= 0; num--)
			{
				if (abstractFunctionPtg.GetParameterClass(num) != 32)
				{
					return false;
				}
			}
			return true;
		}
		return false;
	}

	private byte TransformClass(byte currentOperandClass, byte desiredOperandClass, bool callerForceArrayFlag)
	{
		switch (desiredOperandClass)
		{
		case 32:
			if (!callerForceArrayFlag)
			{
				return 32;
			}
			return 64;
		case 64:
			return 64;
		case 0:
			if (!callerForceArrayFlag)
			{
				return currentOperandClass;
			}
			return 0;
		default:
			throw new InvalidOperationException("Unexpected operand class (" + desiredOperandClass + ")");
		}
	}

	private void TransformFunctionNode(AbstractFunctionPtg afp, ParseNode[] children, byte desiredOperandClass, bool callerForceArrayFlag)
	{
		byte defaultOperandClass = afp.DefaultOperandClass;
		bool callerForceArrayFlag2;
		if (callerForceArrayFlag)
		{
			switch (defaultOperandClass)
			{
			case 0:
				if (desiredOperandClass == 0)
				{
					afp.PtgClass = 0;
				}
				else
				{
					afp.PtgClass = 64;
				}
				callerForceArrayFlag2 = false;
				break;
			case 64:
				afp.PtgClass = 64;
				callerForceArrayFlag2 = false;
				break;
			case 32:
				afp.PtgClass = 64;
				callerForceArrayFlag2 = true;
				break;
			default:
				throw new InvalidOperationException("Unexpected operand class (" + defaultOperandClass + ")");
			}
		}
		else if (defaultOperandClass == desiredOperandClass)
		{
			callerForceArrayFlag2 = false;
			afp.PtgClass = defaultOperandClass;
		}
		else
		{
			switch (desiredOperandClass)
			{
			case 32:
				afp.PtgClass = 32;
				callerForceArrayFlag2 = false;
				break;
			case 64:
				switch (defaultOperandClass)
				{
				case 0:
					afp.PtgClass = 0;
					break;
				case 32:
					afp.PtgClass = 64;
					break;
				default:
					throw new InvalidOperationException("Unexpected operand class (" + defaultOperandClass + ")");
				}
				callerForceArrayFlag2 = defaultOperandClass == 32;
				break;
			case 0:
				switch (defaultOperandClass)
				{
				case 64:
					afp.PtgClass = 64;
					break;
				case 32:
					afp.PtgClass = 32;
					break;
				default:
					throw new InvalidOperationException("Unexpected operand class (" + defaultOperandClass + ")");
				}
				callerForceArrayFlag2 = false;
				break;
			default:
				throw new InvalidOperationException("Unexpected operand class (" + desiredOperandClass + ")");
			}
		}
		for (int i = 0; i < children.Length; i++)
		{
			ParseNode node = children[i];
			byte parameterClass = afp.GetParameterClass(i);
			TransformNode(node, parameterClass, callerForceArrayFlag2);
		}
	}

	private void SetSimpleValueFuncClass(AbstractFunctionPtg afp, byte desiredOperandClass, bool callerForceArrayFlag)
	{
		if (callerForceArrayFlag || desiredOperandClass == 64)
		{
			afp.PtgClass = 64;
		}
		else
		{
			afp.PtgClass = 32;
		}
	}
}
