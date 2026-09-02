using NPOI.SS.Formula.Function;
using NPOI.Util;

namespace NPOI.SS.Formula.PTG;

public class FuncVarPtg : AbstractFunctionPtg
{
	public const byte sid = 34;

	private const int SIZE = 4;

	public static readonly OperationPtg SUM = Create("SUM", 1);

	public override int Size => 4;

	private FuncVarPtg(int functionIndex, int returnClass, byte[] paramClasses, int numArgs)
		: base(functionIndex, returnClass, paramClasses, numArgs)
	{
	}

	public static FuncVarPtg Create(ILittleEndianInput in1)
	{
		return Create(in1.ReadByte(), in1.ReadShort());
	}

	public static FuncVarPtg Create(string pName, int numArgs)
	{
		return Create(numArgs, AbstractFunctionPtg.LookupIndex(pName));
	}

	private static FuncVarPtg Create(int numArgs, int functionIndex)
	{
		FunctionMetadata functionByIndex = FunctionMetadataRegistry.GetFunctionByIndex(functionIndex);
		if (functionByIndex == null)
		{
			return new FuncVarPtg(functionIndex, 32, new byte[1] { 32 }, numArgs);
		}
		return new FuncVarPtg(functionIndex, functionByIndex.ReturnClassCode, functionByIndex.ParameterClassCodes, numArgs);
	}

	public override void Write(ILittleEndianOutput out1)
	{
		out1.WriteByte(34 + base.PtgClass);
		out1.WriteByte(_numberOfArgs);
		out1.WriteShort(_functionIndex);
	}
}
