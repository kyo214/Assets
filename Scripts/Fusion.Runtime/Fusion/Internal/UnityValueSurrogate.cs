using System;

namespace Fusion.Internal;

[Serializable]
public abstract class UnityValueSurrogate<T, ReaderWriter> : UnitySurrogateBase, IUnityValueSurrogate<T>, IUnitySurrogate where ReaderWriter : unmanaged, IElementReaderWriter<T>
{
	private static ReaderWriter _readerWriter;

	public abstract T DataProperty { get; set; }

	public unsafe override void Read(int* data, int capacity)
	{
		DataProperty = _readerWriter.Read((byte*)data, 0);
	}

	public unsafe override void Write(int* data, int capacity)
	{
		_readerWriter.Write((byte*)data, 0, DataProperty);
	}

	public override void Init(int capacity)
	{
	}
}
