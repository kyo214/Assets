using System;
using System.Collections;
using System.Collections.Generic;

namespace NPOI.SS.Formula.Function;

internal class FunctionDataBuilder
{
	private int _maxFunctionIndex;

	private Dictionary<string, FunctionMetadata> _functionDataByName;

	private Dictionary<int, FunctionMetadata> _functionDataByIndex;

	private HashSet<int> _mutatingFunctionIndexes;

	public FunctionDataBuilder(int sizeEstimate)
	{
		_maxFunctionIndex = -1;
		_functionDataByName = new Dictionary<string, FunctionMetadata>(sizeEstimate * 3 / 2);
		_functionDataByIndex = new Dictionary<int, FunctionMetadata>(sizeEstimate * 3 / 2);
		_mutatingFunctionIndexes = new HashSet<int>();
	}

	public void Add(int functionIndex, string functionName, int minParams, int maxParams, byte returnClassCode, byte[] parameterClassCodes, bool hasFootnote)
	{
		FunctionMetadata value = new FunctionMetadata(functionIndex, functionName, minParams, maxParams, returnClassCode, parameterClassCodes);
		int num = functionIndex;
		if (functionIndex > _maxFunctionIndex)
		{
			_maxFunctionIndex = functionIndex;
		}
		_functionDataByName.TryGetValue(functionName, out var value2);
		if (value2 != null)
		{
			if (!hasFootnote || !_mutatingFunctionIndexes.Contains(num))
			{
				throw new Exception("Multiple entries for function name '" + functionName + "'");
			}
			_functionDataByIndex.Remove(value2.Index);
		}
		_functionDataByIndex.TryGetValue(num, out value2);
		if (value2 != null)
		{
			if (!hasFootnote || !_mutatingFunctionIndexes.Contains(num))
			{
				throw new Exception("Multiple entries for function index (" + functionIndex + ")");
			}
			_functionDataByName.Remove(value2.Name);
		}
		if (hasFootnote)
		{
			_mutatingFunctionIndexes.Add(num);
		}
		_functionDataByIndex[num] = value;
		_functionDataByName[functionName] = value;
	}

	public FunctionMetadataRegistry Build()
	{
		_ = new FunctionMetadata[_functionDataByName.Count];
		IEnumerator enumerator = _functionDataByName.Values.GetEnumerator();
		FunctionMetadata[] array = new FunctionMetadata[_maxFunctionIndex + 1];
		while (enumerator.MoveNext())
		{
			FunctionMetadata functionMetadata = (FunctionMetadata)enumerator.Current;
			array[functionMetadata.Index] = functionMetadata;
		}
		return new FunctionMetadataRegistry(array, _functionDataByName);
	}
}
