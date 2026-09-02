using System;
using System.Collections;
using NPOI.HSSF.Record;
using NPOI.Util;

namespace NPOI.SS.Util;

public class CellRangeAddressList
{
	private ArrayList _list;

	public int Size => GetEncodedSize(_list.Count);

	public CellRangeAddress[] CellRangeAddresses => (CellRangeAddress[])_list.ToArray(typeof(CellRangeAddress));

	public CellRangeAddressList()
	{
		_list = new ArrayList();
	}

	public CellRangeAddressList(int firstRow, int lastRow, int firstCol, int lastCol)
		: this()
	{
		AddCellRangeAddress(firstRow, firstCol, lastRow, lastCol);
	}

	public CellRangeAddressList(RecordInputStream in1)
	{
		int num = in1.ReadUShort();
		_list = new ArrayList(num);
		for (int i = 0; i < num; i++)
		{
			_list.Add(new CellRangeAddress(in1));
		}
	}

	public int CountRanges()
	{
		return _list.Count;
	}

	public void AddCellRangeAddress(int firstRow, int firstCol, int lastRow, int lastCol)
	{
		CellRangeAddress cra = new CellRangeAddress(firstRow, lastRow, firstCol, lastCol);
		AddCellRangeAddress(cra);
	}

	public void AddCellRangeAddress(CellRangeAddress cra)
	{
		_list.Add(cra);
	}

	public CellRangeAddress Remove(int rangeIndex)
	{
		if (_list.Count == 0)
		{
			throw new Exception("List is empty");
		}
		if (rangeIndex < 0 || rangeIndex >= _list.Count)
		{
			throw new Exception("Range index (" + rangeIndex + ") is outside allowable range (0.." + (_list.Count - 1) + ")");
		}
		CellRangeAddress result = (CellRangeAddress)_list[rangeIndex];
		_list.Remove(rangeIndex);
		return result;
	}

	public CellRangeAddress GetCellRangeAddress(int index)
	{
		return (CellRangeAddress)_list[index];
	}

	public int Serialize(int offset, byte[] data)
	{
		int size = Size;
		Serialize(new LittleEndianByteArrayOutputStream(data, offset, size));
		return size;
	}

	public void Serialize(ILittleEndianOutput out1)
	{
		int count = _list.Count;
		out1.WriteShort(count);
		for (int i = 0; i < count; i++)
		{
			((CellRangeAddress)_list[i]).Serialize(out1);
		}
	}

	public static int GetEncodedSize(int numberOfRanges)
	{
		return 2 + CellRangeAddress.GetEncodedSize(numberOfRanges);
	}

	public CellRangeAddressList Copy()
	{
		CellRangeAddressList cellRangeAddressList = new CellRangeAddressList();
		int count = _list.Count;
		for (int i = 0; i < count; i++)
		{
			CellRangeAddress cellRangeAddress = (CellRangeAddress)_list[i];
			cellRangeAddressList.AddCellRangeAddress(cellRangeAddress.Copy());
		}
		return cellRangeAddressList;
	}
}
