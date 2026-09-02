using System;
using System.Text;
using NPOI.SS.Formula.Function;

namespace NPOI.SS.Formula.PTG;

[Serializable]
public abstract class AbstractFunctionPtg : OperationPtg
{
	public const string FUNCTION_NAME_IF = "IF";

	private const short FUNCTION_INDEX_EXTERNAL = 255;

	protected byte returnClass;

	protected byte[] paramClass;

	protected byte _numberOfArgs;

	protected short _functionIndex;

	public override bool IsBaseToken => false;

	public short FunctionIndex => _functionIndex;

	public override int NumberOfOperands => _numberOfArgs;

	public string Name => LookupName(_functionIndex);

	public bool IsExternalFunction => _functionIndex == 255;

	public override byte DefaultOperandClass => returnClass;

	protected AbstractFunctionPtg(int functionIndex, int pReturnClass, byte[] paramTypes, int nParams)
	{
		_numberOfArgs = (byte)nParams;
		_functionIndex = (short)functionIndex;
		returnClass = (byte)pReturnClass;
		paramClass = paramTypes;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder(64);
		stringBuilder.Append(GetType().Name).Append(" [");
		stringBuilder.Append(LookupName(_functionIndex));
		stringBuilder.Append(" nArgs=").Append(_numberOfArgs);
		stringBuilder.Append("]");
		return stringBuilder.ToString();
	}

	public override string ToFormulaString()
	{
		return Name;
	}

	public override string ToFormulaString(string[] operands)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (IsExternalFunction)
		{
			stringBuilder.Append(operands[0]);
			AppendArgs(stringBuilder, 1, operands);
		}
		else
		{
			stringBuilder.Append(Name);
			AppendArgs(stringBuilder, 0, operands);
		}
		return stringBuilder.ToString();
	}

	private static void AppendArgs(StringBuilder buf, int firstArgIx, string[] operands)
	{
		buf.Append('(');
		for (int i = firstArgIx; i < operands.Length; i++)
		{
			if (i > firstArgIx)
			{
				buf.Append(',');
			}
			buf.Append(operands[i]);
		}
		buf.Append(")");
	}

	public static bool IsBuiltInFunctionName(string name)
	{
		return FunctionMetadataRegistry.LookupIndexByName(name.ToUpper()) >= 0;
	}

	protected string LookupName(short index)
	{
		if (index == 255)
		{
			return "#external#";
		}
		return (FunctionMetadataRegistry.GetFunctionByIndex(index) ?? throw new Exception("bad function index (" + index + ")")).Name;
	}

	protected static short LookupIndex(string name)
	{
		short num = FunctionMetadataRegistry.LookupIndexByName(name.ToUpper());
		if (num < 0)
		{
			return 255;
		}
		return num;
	}

	public byte GetParameterClass(int index)
	{
		if (index >= paramClass.Length)
		{
			return paramClass[paramClass.Length - 1];
		}
		return paramClass[index];
	}
}
