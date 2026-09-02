using System.Collections.Generic;
using NPOI.SS.Formula.Functions;

namespace NPOI.SS.Formula.UDF;

public class AggregatingUDFFinder : UDFFinder
{
	private List<UDFFinder> _usedToolPacks = new List<UDFFinder>();

	public AggregatingUDFFinder(params UDFFinder[] usedToolPacks)
	{
		_usedToolPacks = new List<UDFFinder>(usedToolPacks.Length);
		_usedToolPacks.AddRange(usedToolPacks);
	}

	public override FreeRefFunction FindFunction(string name)
	{
		foreach (UDFFinder usedToolPack in _usedToolPacks)
		{
			FreeRefFunction freeRefFunction = usedToolPack.FindFunction(name);
			if (freeRefFunction != null)
			{
				return freeRefFunction;
			}
		}
		return null;
	}

	public void Add(UDFFinder toolPack)
	{
		_usedToolPacks.Add(toolPack);
	}
}
