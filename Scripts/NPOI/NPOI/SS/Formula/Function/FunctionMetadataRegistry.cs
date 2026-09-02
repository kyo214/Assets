using System.Collections;
using System.Collections.Generic;

namespace NPOI.SS.Formula.Function;

public class FunctionMetadataRegistry
{
	public const string FUNCTION_NAME_IF = "IF";

	public const int FUNCTION_INDEX_IF = 1;

	public const short FUNCTION_INDEX_SUM = 4;

	public const int FUNCTION_INDEX_CHOOSE = 100;

	public const short FUNCTION_INDEX_INDIRECT = 148;

	public const short FUNCTION_INDEX_EXTERNAL = 255;

	private static FunctionMetadataRegistry _instance;

	private FunctionMetadata[] _functionDataByIndex;

	private Dictionary<string, FunctionMetadata> _functionDataByName;

	private static FunctionMetadataRegistry GetInstance()
	{
		if (_instance == null)
		{
			_instance = FunctionMetadataReader.CreateRegistry();
		}
		return _instance;
	}

	public FunctionMetadataRegistry(FunctionMetadata[] functionDataByIndex, Dictionary<string, FunctionMetadata> functionDataByName)
	{
		_functionDataByIndex = functionDataByIndex;
		_functionDataByName = functionDataByName;
	}

	public ICollection GetAllFunctionNames()
	{
		return _functionDataByName.Keys;
	}

	public static FunctionMetadata GetFunctionByIndex(int index)
	{
		return GetInstance().GetFunctionByIndexInternal(index);
	}

	private FunctionMetadata GetFunctionByIndexInternal(int index)
	{
		return _functionDataByIndex[index];
	}

	public static short LookupIndexByName(string name)
	{
		FunctionMetadata functionByNameInternal = GetInstance().GetFunctionByNameInternal(name);
		if (functionByNameInternal == null)
		{
			return -1;
		}
		return (short)functionByNameInternal.Index;
	}

	private FunctionMetadata GetFunctionByNameInternal(string name)
	{
		if (_functionDataByName.ContainsKey(name))
		{
			return _functionDataByName[name];
		}
		return null;
	}

	public static FunctionMetadata GetFunctionByName(string name)
	{
		return GetInstance().GetFunctionByNameInternal(name);
	}
}
