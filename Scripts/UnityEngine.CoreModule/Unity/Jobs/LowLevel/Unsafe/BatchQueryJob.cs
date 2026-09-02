using Unity.Collections;

namespace Unity.Jobs.LowLevel.Unsafe;

public struct BatchQueryJob<CommandT, ResultT>(NativeArray<CommandT> commands, NativeArray<ResultT> results) where CommandT : struct where ResultT : struct
{
	[ReadOnly]
	internal NativeArray<CommandT> commands = commands;

	internal NativeArray<ResultT> results = results;
}
