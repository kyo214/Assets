#define DEBUG
using System.Text;
using Fusion.Sockets;

namespace Fusion;

internal struct SimulationMessageEnvelope : ILogBuilder
{
	public ulong Sequence;

	public unsafe SimulationMessage* Message;

	public unsafe SimulationMessageEnvelope* Prev;

	public unsafe SimulationMessageEnvelope* Next;

	private const int OffsetBlockSize = 10;

	private const int TickBlockSize = 16;

	private const int SequenceBlockSize = 16;

	private unsafe static int WriteInternal<T>(SimulationMessageEnvelope* envelope, T* buffer) where T : unmanaged, INetBitWriteStream
	{
		buffer->WriteInt32VarLength(envelope->Message->Offset, 10);
		buffer->WriteInt32VarLength(envelope->Message->Tick, 16);
		Assert.Check(!envelope->Message->Source.IsValid || !envelope->Message->Target.IsValid);
		PlayerRef.Write(buffer, envelope->Message->Source);
		PlayerRef.Write(buffer, envelope->Message->Target);
		int num = envelope->Message->Flags & -3;
		int num2 = num & 0xFFFF;
		int num3 = num >> 16;
		if (buffer->WriteBoolean(num2 != 0))
		{
			buffer->WriteInt32(num2, 16);
			if ((num2 & 1) == 1 && buffer->WriteBoolean(num3 != 0))
			{
				buffer->WriteInt32(num3, -12);
			}
		}
		else
		{
			Assert.Check(num3 == 0, "User flags should only be used if FLAG_USER_MESSAGE is set");
		}
		if ((envelope->Message->Flags & 8) != 8)
		{
			buffer->WriteUInt64VarLength(envelope->Sequence, 16);
		}
		int offsetBits = buffer->OffsetBits;
		if (envelope->Message->Offset > 0)
		{
			int length = Maths.BytesRequiredForBits(envelope->Message->Offset);
			buffer->WriteBytesAligned(SimulationMessage.GetData(envelope->Message), length);
		}
		return offsetBits;
	}

	public unsafe static void Write(SimulationMessageEnvelope* envelope, NetBitBuffer* buffer)
	{
		int offsetBits = buffer->OffsetBits;
		int num = WriteInternal(envelope, buffer);
	}

	public unsafe static int GetBitCount(SimulationMessageEnvelope* envelope, NetBitBuffer* buffer)
	{
		int offsetBits = buffer->OffsetBits;
		NetBitBufferNull netBitBufferNull = new NetBitBufferNull
		{
			OffsetBits = offsetBits
		};
		WriteInternal(envelope, &netBitBufferNull);
		return netBitBufferNull.OffsetBits - offsetBits;
	}

	public unsafe static SimulationMessageEnvelope* Read(Simulation sim, NetBitBuffer* buffer)
	{
		int num = buffer->ReadInt32VarLength(10);
		int tick = buffer->ReadInt32VarLength(16);
		PlayerRef source = PlayerRef.Read(buffer);
		PlayerRef target = PlayerRef.Read(buffer);
		int num2 = (buffer->ReadBoolean() ? buffer->ReadInt32(16) : 0);
		if ((num2 & 1) == 1)
		{
			int num3 = (buffer->ReadBoolean() ? buffer->ReadInt32(-12) : 0);
			num2 |= num3 << 16;
		}
		ulong sequence = (((num2 & 8) != 8) ? buffer->ReadUInt64VarLength(16) : 0);
		num2 |= 2;
		int num4 = Maths.BytesRequiredForBits(num);
		SimulationMessageEnvelope* ptr = Allocate(sim, SimulationMessage.Allocate(sim, num4), sequence);
		ptr->Message->Capacity = num;
		ptr->Message->Offset = 0;
		ptr->Message->Tick = tick;
		ptr->Message->Source = source;
		ptr->Message->Target = target;
		ptr->Message->Flags = num2;
		if (ptr->Message->Capacity > 0)
		{
			buffer->ReadBytesAligned(SimulationMessage.GetData(ptr->Message), num4);
		}
		return ptr;
	}

	public unsafe static SimulationMessageEnvelope* Allocate(Simulation sim, SimulationMessage* message, ulong sequence)
	{
		SimulationMessageEnvelope* ptr = sim.TempAlloc<SimulationMessageEnvelope>();
		ptr->Message = message;
		ptr->Sequence = sequence;
		message->ReferenceCountAdd();
		return ptr;
	}

	public unsafe static void Free(Simulation sim, SimulationMessageEnvelope* envelope)
	{
		if (envelope != null)
		{
			Assert.Check(envelope->Prev == null);
			Assert.Check(envelope->Next == null);
			if (envelope->Message != null && envelope->Message->ReferenceCountSub())
			{
				SimulationMessage.Free(sim, envelope->Message);
			}
			sim.TempFree(envelope);
		}
	}

	public unsafe override string ToString()
	{
		return string.Format("[SimulationMessageEnvelope: {0}={1}, {2}={3}]", "Sequence", Sequence, "Message", (Message != null) ? Message->ToString(useBrackets: false) : "null");
	}

	unsafe void ILogBuilder.BuildLogMessage(StringBuilder builder, string message, in LogOptions options)
	{
		builder.Append(message);
		builder.Append(" ");
		builder.AppendLine(ToString());
		if (Message != null)
		{
			builder.Append(SimulationMessage.DumpContents(*Message));
		}
	}
}
