using System;
using System.Collections.Generic;

namespace Fusion.Internal;

[Serializable]
public abstract class UnityDictionarySurrogate<KeyType, KeyReaderWriter, ValueType, ValueReaderWriter> : UnitySurrogateBase where KeyType : unmanaged where KeyReaderWriter : unmanaged, IElementReaderWriter<KeyType> where ValueType : unmanaged where ValueReaderWriter : unmanaged, IElementReaderWriter<ValueType>
{
	private static IElementReaderWriter<KeyType> _keyReaderWriter = new KeyReaderWriter();

	private static IElementReaderWriter<ValueType> _valReaderWriter = new ValueReaderWriter();

	public abstract SerializableDictionary<KeyType, ValueType> DataProperty { get; set; }

	public unsafe override void Read(int* data, int capacity)
	{
		bool flag = false;
		SerializableDictionary<KeyType, ValueType> dataProperty = DataProperty;
		NetworkDictionary<KeyType, ValueType> networkDictionary = new NetworkDictionary<KeyType, ValueType>(data, capacity, _keyReaderWriter, _valReaderWriter);
		if (networkDictionary.Count != dataProperty.Count)
		{
			flag = true;
		}
		else
		{
			foreach (KeyValuePair<KeyType, ValueType> item in networkDictionary)
			{
				if (!dataProperty.ContainsKey(item.Key))
				{
					flag = true;
					break;
				}
				dataProperty[item.Key] = item.Value;
			}
		}
		if (flag)
		{
			dataProperty.Clear();
			foreach (KeyValuePair<KeyType, ValueType> item2 in networkDictionary)
			{
				dataProperty.Add(item2.Key, item2.Value);
			}
		}
		dataProperty.Store();
	}

	public unsafe override void Write(int* data, int capacity)
	{
		NetworkDictionary<KeyType, ValueType> networkDictionary = new NetworkDictionary<KeyType, ValueType>(data, capacity, _keyReaderWriter, _valReaderWriter);
		networkDictionary.Clear();
		foreach (KeyValuePair<KeyType, ValueType> item in DataProperty)
		{
			networkDictionary.Add(item.Key, item.Value);
		}
	}

	public override void Init(int capacity)
	{
		DataProperty = new SerializableDictionary<KeyType, ValueType>();
	}
}
