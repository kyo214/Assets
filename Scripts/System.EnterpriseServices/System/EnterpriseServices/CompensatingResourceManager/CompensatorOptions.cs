namespace System.EnterpriseServices.CompensatingResourceManager;

[Serializable]
[Flags]
public enum CompensatorOptions
{
	PreparePhase = 1,
	CommitPhase = 2,
	AbortPhase = 4,
	AllPhases = PreparePhase | CommitPhase | AbortPhase,
	FailIfInDoubtsRemain = 0x10
}
