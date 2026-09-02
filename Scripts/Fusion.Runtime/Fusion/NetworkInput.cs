#define DEBUG
namespace Fusion;

public struct NetworkInput
{
	private unsafe int* _ptr;

	private int _wordCount;

	private unsafe int Type
	{
		get
		{
			return (_ptr == null) ? (-1) : (*_ptr);
		}
		set
		{
			if (_ptr != null)
			{
				*_ptr = value;
			}
		}
	}

	public unsafe int* Data => (_ptr == null) ? null : (_ptr + 1);

	private unsafe bool Valid => _ptr != null;

	public unsafe NetworkInput(int* ptr, int wordCount)
	{
		_ptr = ptr;
		_wordCount = wordCount;
	}

	public unsafe bool TryGet<T>(out T input) where T : unmanaged, INetworkInput
	{
		Assert.Check(Valid);
		if (_ptr == null || Type != NetworkInputUtils.GetTypeKey(typeof(T)))
		{
			input = default;
			return false;
		}
		input = *(T*)Data;
		return true;
	}

	public unsafe bool TrySet<T>(T input) where T : unmanaged, INetworkInput
	{
		Assert.Check(Valid);
		if (_ptr == null || Type != NetworkInputUtils.GetTypeKey(typeof(T)))
		{
			return false;
		}
		*(T*)Data = input;
		return true;
	}

	public unsafe T Get<T>() where T : unmanaged, INetworkInput
	{
		Assert.Check(Valid);
		Convert<T>();
		return *(T*)Data;
	}

	public unsafe void Set<T>(T value) where T : unmanaged, INetworkInput
	{
		Assert.Check(Valid);
		Convert<T>();
		*(T*)Data = value;
	}

	public unsafe void Convert<T>() where T : unmanaged, INetworkInput
	{
		Assert.Check(Valid);
		int typeKey = NetworkInputUtils.GetTypeKey(typeof(T));
		if (typeKey != Type)
		{
			Native.MemClear(_ptr, _wordCount * 4);
			Type = typeKey;
		}
	}

	public bool Is<T>() where T : unmanaged, INetworkInput
	{
		Assert.Check(Valid);
		return Type == NetworkInputUtils.GetTypeKey(typeof(T));
	}
}
