#define DEBUG
namespace Fusion.Sockets;

internal struct NetBitBufferStack
{
	private int _capacity;

	public unsafe NetBitBuffer** Stack;

	public int Count;

	public unsafe bool TryPop(NetBitBuffer** result)
	{
		Assert.Check(Count >= 0);
		if (Count == 0)
		{
			return false;
		}
		*result = Stack[--Count];
		return true;
	}

	public unsafe static NetBitBufferStack Create(int capacity)
	{
		return new NetBitBufferStack
		{
			_capacity = capacity,
			Stack = Native.MallocAndClearPtrArray<NetBitBuffer>(capacity)
		};
	}

	public unsafe static void Free(NetBitBufferStack stack)
	{
		if (stack.Stack != null)
		{
			Native.Free(stack.Stack);
		}
	}

	public unsafe void PushFromHead(NetBitBuffer* head)
	{
		while (head != null)
		{
			Assert.Check(Count >= 0 && Count <= _capacity);
			if (Count == _capacity)
			{
				Stack = Native.DoublePtrArray(Stack, _capacity);
				_capacity *= 2;
			}
			Stack[Count++] = head;
			head = head->Next;
		}
	}
}
