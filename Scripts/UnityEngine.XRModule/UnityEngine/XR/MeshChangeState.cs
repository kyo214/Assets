using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.XR;

[UsedByNativeCode]
[NativeHeader("Modules/XR/Subsystems/Meshing/XRMeshBindings.h")]
public enum MeshChangeState
{
	Added = 0,
	Updated = 1,
	Removed = 2,
	Unchanged = 3
}
