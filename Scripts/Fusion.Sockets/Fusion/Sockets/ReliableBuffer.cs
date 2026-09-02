#define DEBUG
namespace Fusion.Sockets;

public struct ReliableBuffer
{
	public const int SEQ_BYTES = 4;

	private NetSequencer _sequencer;

	private ReliableList _receiveList;

	private ulong _receiveSequence;

	public int SequenceBits => _sequencer.Bits;

	public static ReliableBuffer Create()
	{
		return new ReliableBuffer
		{
			_sequencer = new NetSequencer(4)
		};
	}

	public ulong NextSendSequence()
	{
		return _sequencer.Next();
	}

	public unsafe void Dispose()
	{
		while (_receiveList.Count > 0)
		{
			Native.Free(_receiveList.RemoveHead());
		}
	}

	public unsafe bool LateReceive(out void* root, out int key, out byte* data, out int length)
	{
		for (ReliableHeader* ptr = _receiveList.Head; ptr != null; ptr = ptr->Next)
		{
			if (_sequencer.Distance(ptr->Sequence, _receiveSequence) == 1)
			{
				_receiveSequence = ptr->Sequence;
				_receiveList.Remove(ptr);
				root = ptr;
				key = ptr->Key;
				data = (byte*)ptr + sizeof(ReliableHeader);
				length = ptr->Length;
				return true;
			}
		}
		root = null;
		data = null;
		length = 0;
		key = 0;
		return false;
	}

	public unsafe void LateFree(void* root)
	{
		Native.Free(root);
	}

	public unsafe bool Receive(NetBitBuffer* buffer, out int key)
	{
		Assert.Always(sizeof(ReliableHeader) == 32, "ReliableHeader size mismatch", sizeof(ReliableHeader));
		ulong num = buffer->ReadUInt64(_sequencer.Bits);
		key = buffer->ReadInt32();
		if (_sequencer.Distance(num, _receiveSequence) == 1)
		{
			_receiveSequence = num;
			return true;
		}
		Assert.Check(buffer->IsOnEvenByte);
		int num2 = buffer->LengthBytes - buffer->OffsetBytes;
		byte* ptr = (byte*)Native.Malloc(num2 + sizeof(ReliableHeader));
		Native.MemCpy(ptr + sizeof(ReliableHeader), buffer->PadToByteBoundaryAndGetPtr(), num2);
		ReliableHeader* ptr2 = (ReliableHeader*)ptr;
		ptr2->Sequence = num;
		ptr2->Length = num2;
		ptr2->Key = key;
		_receiveList.AddLast(ptr2);
		return false;
	}
}
