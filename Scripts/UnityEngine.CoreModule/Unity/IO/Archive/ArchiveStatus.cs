using UnityEngine.Scripting;

namespace Unity.IO.Archive;

[RequiredByNativeCode]
public enum ArchiveStatus
{
	InProgress = 0,
	Complete = 1,
	Failed = 2
}
