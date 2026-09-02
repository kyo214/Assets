using NPOI.SS.Formula.Atp;
using NPOI.SS.Formula.Functions;

namespace NPOI.SS.Formula.UDF;

public abstract class UDFFinder
{
	public static UDFFinder GetDefault()
	{
		return new AggregatingUDFFinder(AnalysisToolPak.instance);
	}

	public abstract FreeRefFunction FindFunction(string name);
}
