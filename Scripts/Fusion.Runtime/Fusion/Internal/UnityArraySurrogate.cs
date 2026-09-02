using System;

namespace Fusion.Internal;

[Serializable]
public abstract class UnityArraySurrogate<T, ReaderWriter> : UnitySurrogateBase where T : unmanaged where ReaderWriter : unmanaged, IElementReaderWriter<T>
{
	private static ReaderWriter _readerWriter;

	public abstract T[] DataProperty { get; set; }

	public unsafe override void Read(int* data, int capacity)
	{
		T[] array = DataProperty;
		Array.Resize(ref array, capacity);
		for (int i = 0; i < capacity; i++)
		{
			array[i] = _readerWriter.Read((byte*)data, i);
		}
		DataProperty = array;
	}

	public unsafe override void Write(int* data, int capacity)
	{
		T[] array = DataProperty;
		Array.Resize(ref array, capacity);
		for (int i = 0; i < capacity; i++)
		{
			_readerWriter.Write((byte*)data, i, array[i]);
		}
	}

	public override void Init(int capacity)
	{
		DataProperty = new T[capacity];
	}
}
