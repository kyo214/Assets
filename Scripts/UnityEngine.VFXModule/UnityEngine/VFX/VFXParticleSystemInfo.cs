using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.VFX;

[NativeHeader("Modules/VFX/Public/Systems/VFXParticleSystem.h")]
[UsedByNativeCode]
public struct VFXParticleSystemInfo(uint aliveCount, uint capacity, bool sleeping, Bounds bounds)
{
	public uint aliveCount = aliveCount;

	public uint capacity = capacity;

	public bool sleeping = sleeping;

	public Bounds bounds = bounds;
}
