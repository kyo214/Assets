namespace System.Net;

[Flags]
internal enum ThreadKinds
{
	Unknown = 0,
	User = 1,
	System = 2,
	Sync = 4,
	Async = 8,
	Timer = 0x10,
	CompletionPort = 0x20,
	Worker = 0x40,
	Finalization = 0x80,
	Other = 0x100,
	OwnerMask = User | System,
	SyncMask = Sync | Async,
	SourceMask = Timer | CompletionPort | Worker | Finalization | Other,
	SafeSources = 0x160,
	ThreadPool = 0x60
}
