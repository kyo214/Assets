using System.Runtime.InteropServices;

namespace UnityEngineInternal.Input;

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 20)]
internal struct NativeInputEvent(NativeInputEventType type, int sizeInBytes, int deviceId, double time)
{
	public const int structSize = 20;

	[FieldOffset(0)]
	public NativeInputEventType type = type;

	[FieldOffset(4)]
	public ushort sizeInBytes = (ushort)sizeInBytes;

	[FieldOffset(6)]
	public ushort deviceId = (ushort)deviceId;

	[FieldOffset(8)]
	public double time = time;

	[FieldOffset(16)]
	public int eventId = 0;
}
