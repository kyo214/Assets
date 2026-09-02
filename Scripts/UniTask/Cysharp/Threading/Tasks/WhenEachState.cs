namespace Cysharp.Threading.Tasks;

internal enum WhenEachState : byte
{
	NotRunning = 0,
	Running = 1,
	Completed = 2
}
