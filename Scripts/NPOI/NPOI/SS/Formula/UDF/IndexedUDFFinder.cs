using System.Collections.Generic;
using NPOI.SS.Formula.Functions;

namespace NPOI.SS.Formula.UDF;

public class IndexedUDFFinder : AggregatingUDFFinder
{
	private Dictionary<int, string> _funcMap;

	public IndexedUDFFinder(params UDFFinder[] usedToolPacks)
		: base(usedToolPacks)
	{
		_funcMap = new Dictionary<int, string>();
	}

	public override FreeRefFunction FindFunction(string name)
	{
		FreeRefFunction freeRefFunction = base.FindFunction(name);
		if (freeRefFunction != null)
		{
			int functionIndex = GetFunctionIndex(name);
			_funcMap[functionIndex] = name;
		}
		return freeRefFunction;
	}

	public string GetFunctionName(int idx)
	{
		return _funcMap[idx];
	}

	public int GetFunctionIndex(string name)
	{
		return name.GetHashCode();
	}
}
