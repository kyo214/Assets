using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NPOI.Util;

namespace NPOI.DDF;

public class EscherArrayProperty : EscherComplexProperty, IEnumerable<byte[]>, IEnumerable
{
	private class EscherArrayEnumerator : IEnumerator<byte[]>, IDisposable, IEnumerator
	{
		private EscherArrayProperty dataHolder;

		private int idx = -1;

		public byte[] Current
		{
			get
			{
				if (idx < 0 || idx > dataHolder.NumberOfElementsInArray)
				{
					throw new IndexOutOfRangeException();
				}
				return dataHolder.GetElement(idx);
			}
		}

		object IEnumerator.Current => Current;

		public EscherArrayEnumerator(EscherArrayProperty eap)
		{
			dataHolder = eap;
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}

		public bool MoveNext()
		{
			idx++;
			return idx < dataHolder.NumberOfElementsInArray;
		}

		public void Reset()
		{
			throw new NotImplementedException();
		}
	}

	private const int FIXED_SIZE = 6;

	private bool sizeIncludesHeaderSize = true;

	private bool emptyComplexPart;

	public int NumberOfElementsInArray
	{
		get
		{
			if (emptyComplexPart)
			{
				return 0;
			}
			return LittleEndian.GetUShort(_complexData, 0);
		}
		set
		{
			int num = value * GetActualSizeOfElements(SizeOfElements) + 6;
			if (num != _complexData.Length)
			{
				byte[] array = new byte[num];
				Array.Copy(_complexData, 0, array, 0, _complexData.Length);
				_complexData = array;
			}
			LittleEndian.PutShort(_complexData, 0, (short)value);
		}
	}

	public int NumberOfElementsInMemory
	{
		get
		{
			return LittleEndian.GetUShort(_complexData, 2);
		}
		set
		{
			int num = value * GetActualSizeOfElements(SizeOfElements) + 6;
			if (num != _complexData.Length)
			{
				byte[] array = new byte[num];
				Array.Copy(_complexData, 0, array, 0, num);
				_complexData = array;
			}
			LittleEndian.PutShort(_complexData, 2, (short)value);
		}
	}

	public short SizeOfElements
	{
		get
		{
			return LittleEndian.GetShort(_complexData, 4);
		}
		set
		{
			LittleEndian.PutShort(_complexData, 4, value);
			int num = NumberOfElementsInArray * GetActualSizeOfElements(SizeOfElements) + 6;
			if (num != _complexData.Length)
			{
				byte[] array = new byte[num];
				Array.Copy(_complexData, 0, array, 0, 6);
				_complexData = array;
			}
		}
	}

	public EscherArrayProperty(short id, byte[] complexData)
		: base(id, CheckComplexData(complexData))
	{
		emptyComplexPart = complexData.Length == 0;
	}

	public EscherArrayProperty(short propertyNumber, bool isBlipId, byte[] complexData)
		: base(propertyNumber, isBlipId, CheckComplexData(complexData))
	{
	}

	private static byte[] CheckComplexData(byte[] complexData)
	{
		if (complexData == null || complexData.Length == 0)
		{
			complexData = new byte[6];
		}
		return complexData;
	}

	public byte[] GetElement(int index)
	{
		int actualSizeOfElements = GetActualSizeOfElements(SizeOfElements);
		byte[] array = new byte[actualSizeOfElements];
		Array.Copy(_complexData, 6 + index * actualSizeOfElements, array, 0, array.Length);
		return array;
	}

	public void SetElement(int index, byte[] element)
	{
		int actualSizeOfElements = GetActualSizeOfElements(SizeOfElements);
		Array.Copy(element, 0, _complexData, 6 + index * actualSizeOfElements, actualSizeOfElements);
	}

	public override string ToString()
	{
		string newLine = Environment.NewLine;
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("    {EscherArrayProperty:" + newLine);
		stringBuilder.Append("     Num Elements: " + NumberOfElementsInArray + newLine);
		stringBuilder.Append("     Num Elements In Memory: " + NumberOfElementsInMemory + newLine);
		stringBuilder.Append("     Size of elements: " + SizeOfElements + newLine);
		for (int i = 0; i < NumberOfElementsInArray; i++)
		{
			stringBuilder.Append("     Element " + i + ": " + HexDump.ToHex(GetElement(i)) + newLine);
		}
		stringBuilder.Append("}" + newLine);
		return "propNum: " + PropertyNumber + ", propName: " + EscherProperties.GetPropertyName(PropertyNumber) + ", complex: " + IsComplex + ", blipId: " + IsBlipId + ", data: " + newLine + stringBuilder.ToString();
	}

	public override string ToXml(string tab)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(tab).Append("<").Append(GetType().Name)
			.Append(" id=\"0x")
			.Append(HexDump.ToHex(Id))
			.Append("\" name=\"")
			.Append(Name)
			.Append("\" blipId=\"")
			.Append(IsBlipId.ToString().ToLower())
			.Append("\">\n");
		for (int i = 0; i < NumberOfElementsInArray; i++)
		{
			stringBuilder.Append("\t").Append(tab).Append("<Element>")
				.Append(HexDump.ToHex(GetElement(i)))
				.Append("</Element>\n");
		}
		stringBuilder.Append(tab).Append("</").Append(GetType().Name)
			.Append(">\n");
		return stringBuilder.ToString();
	}

	public int SetArrayData(byte[] data, int offset)
	{
		if (emptyComplexPart)
		{
			_complexData = new byte[0];
		}
		else
		{
			short num = LittleEndian.GetShort(data, offset);
			int num2 = GetActualSizeOfElements(LittleEndian.GetShort(data, offset + 4)) * num;
			if (num2 == _complexData.Length)
			{
				_complexData = new byte[num2 + 6];
				sizeIncludesHeaderSize = false;
			}
			Array.Copy(data, offset, _complexData, 0, _complexData.Length);
		}
		return _complexData.Length;
	}

	public override int SerializeSimplePart(byte[] data, int pos)
	{
		LittleEndian.PutShort(data, pos, Id);
		int num = _complexData.Length;
		if (!sizeIncludesHeaderSize)
		{
			num -= 6;
		}
		LittleEndian.PutInt(data, pos + 2, num);
		return 6;
	}

	public static int GetActualSizeOfElements(short sizeOfElements)
	{
		if (sizeOfElements < 0)
		{
			return (short)(-sizeOfElements >> 2);
		}
		return sizeOfElements;
	}

	public IEnumerator<byte[]> GetEnumerator()
	{
		return new EscherArrayEnumerator(this);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
