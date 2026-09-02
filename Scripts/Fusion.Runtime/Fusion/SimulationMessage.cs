#define DEBUG
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace Fusion;

[StructLayout(LayoutKind.Explicit)]
public struct SimulationMessage : ILogBuilder
{
	[Flags]
	private enum BuiltInFlags
	{
		USER_MESSAGE = 1,
		REMOTE = 2,
		STATIC = 4,
		UNRELIABLE = 8,
		TARGET_PLAYER = 0x10,
		TARGET_SERVER = 0x20,
		NOT_TICK_ALIGNED = 0x80,
		DUMMY = 0x100
	}

	public const int SIZE = 28;

	public const int MAX_PAYLOAD_SIZE = 512;

	public const int FLAG_USER_MESSAGE = 1;

	public const int FLAG_REMOTE = 2;

	public const int FLAG_STATIC = 4;

	public const int FLAG_UNRELIABLE = 8;

	public const int FLAG_TARGET_PLAYER = 16;

	public const int FLAG_TARGET_SERVER = 32;

	public const int FLAG_INTERNAL = 64;

	public const int FLAG_NOT_TICK_ALIGNED = 128;

	public const int FLAG_DUMMY = 256;

	public const int FLAG_USER_FLAGS_START = 65536;

	public const int FLAGS_RESERVED = 65535;

	public const int FLAGS_RESERVED_BITS = 16;

	[FieldOffset(0)]
	public int Tick;

	[FieldOffset(4)]
	public PlayerRef Source;

	[FieldOffset(8)]
	public int Capacity;

	[FieldOffset(12)]
	public int Offset;

	[FieldOffset(16)]
	public int References;

	[FieldOffset(20)]
	public int Flags;

	[FieldOffset(24)]
	public PlayerRef Target;

	public bool IsUnreliable
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return (Flags & 8) == 8;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ReferenceCountAdd()
	{
		References++;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool ReferenceCountSub()
	{
		References--;
		Assert.Check(References >= 0);
		return References == 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetTarget(PlayerRef target)
	{
		Target = target;
		Flags |= (target.IsNone ? 32 : 16);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetStatic()
	{
		Flags |= 4;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetUnreliable()
	{
		Flags |= 8;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetNotTickAligned()
	{
		Flags |= 128;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetDummy()
	{
		Flags |= 256;
		Offset = 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool GetFlag(int flag)
	{
		return (Flags & flag) == flag;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool IsTargeted()
	{
		return (Flags & 0x30) != 0;
	}

	public unsafe static SimulationMessage* Clone(Simulation sim, SimulationMessage* message)
	{
		int num = Maths.BytesRequiredForBits(message->Capacity);
		SimulationMessage* ptr = Allocate(sim, num);
		Native.MemCpy(ptr, message, 28 + num);
		ptr->Tick = 0;
		ptr->References = 0;
		return ptr;
	}

	public unsafe static void WriteNetworkedObjectRef(SimulationMessage* message, NetworkId value)
	{
		*(NetworkId*)GetData(message) = value;
		message->Offset += sizeof(NetworkId) * 8;
	}

	public unsafe static NetworkId ReadNetworkedObjectRef(SimulationMessage* message)
	{
		NetworkId data = *(NetworkId*)GetData(message);
		message->Offset += sizeof(NetworkId) * 8;
		return data;
	}

	public unsafe static void WriteVector3(SimulationMessage* message, Vector3 value)
	{
		*(Vector3*)GetData(message) = value;
		message->Offset += sizeof(Vector3) * 8;
	}

	public unsafe static Vector3 ReadVector3(SimulationMessage* message)
	{
		Vector3 data = *(Vector3*)GetData(message);
		message->Offset += sizeof(Vector3) * 8;
		return data;
	}

	public unsafe static void WriteInt(SimulationMessage* message, int value)
	{
		*(int*)GetData(message) = value;
		message->Offset += 32;
	}

	public unsafe static int ReadInt(SimulationMessage* message)
	{
		int data = *(int*)GetData(message);
		message->Offset += 32;
		return data;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe static void Free(Simulation sim, SimulationMessage* message)
	{
		Assert.Always(message->References == 0, "Message is still referenced");
		sim.TempFree(message);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static byte* GetData(SimulationMessage* message)
	{
		Assert.Check(sizeof(SimulationMessage) == 28);
		return (byte*)message + 28;
	}

	public unsafe static SimulationMessage* Allocate(Simulation sim, int capacityInBytes)
	{
		Assert.Check(sizeof(SimulationMessage) == 28);
		SimulationMessage* ptr = (SimulationMessage*)sim.TempAlloc(28 + capacityInBytes);
		ptr->Capacity = capacityInBytes * 8;
		return ptr;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool CanAllocateUserPayload(int capacityInBytes)
	{
		return capacityInBytes <= 512;
	}

	public override string ToString()
	{
		return ToString(useBrackets: true);
	}

	public string ToString(bool useBrackets)
	{
		return string.Format("{0}{1}={2}, {3}={4}, {5}={6}, {7}={8}, {9}={10}, {11}={12}, Flags={13}, UserFlags={14}{15}", useBrackets ? "[SimulationMessage: " : "", "Tick", Tick, "Source", Source, "Capacity", Capacity, "Offset", Offset, "References", References, "Target", Target, (BuiltInFlags)(Flags & 0xFFFF), Flags & -65536, useBrackets ? "]" : "");
	}

	internal unsafe static string DumpContents(SimulationMessage message)
	{
		byte* data = GetData(&message);
		if (message.GetFlag(1))
		{
			return BinUtils.BytesToHex(data, Maths.BytesRequiredForBits(message.Capacity));
		}
		RpcHeader rpcHeader = RpcHeader.Read(data, out var size);
		return $"{rpcHeader} {BinUtils.BytesToHex(data + size, Maths.BytesRequiredForBits(message.Capacity - size))}";
	}

	void ILogBuilder.BuildLogMessage(StringBuilder builder, string message, in LogOptions options)
	{
		builder.Append(message);
		builder.Append(" ");
		builder.AppendLine(ToString());
		builder.Append(DumpContents(this));
	}
}
