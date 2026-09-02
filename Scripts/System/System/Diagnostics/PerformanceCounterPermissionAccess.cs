namespace System.Diagnostics;

[Flags]
public enum PerformanceCounterPermissionAccess
{
	None = 0,
	[Obsolete]
	Browse = 1,
	Read = Browse,
	Write = 2,
	[Obsolete]
	Instrument = Browse | Write,
	Administer = 7
}
