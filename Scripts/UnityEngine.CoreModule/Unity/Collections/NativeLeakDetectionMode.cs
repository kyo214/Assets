using UnityEngine.Scripting;

namespace Unity.Collections;

[UsedByNativeCode]
public enum NativeLeakDetectionMode
{
	Disabled = 1,
	Enabled = 2,
	EnabledWithStackTrace = 3
}
