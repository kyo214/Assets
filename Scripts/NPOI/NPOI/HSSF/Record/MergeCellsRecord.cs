using System;
using System.Collections;
using System.Text;
using NPOI.SS.Util;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class MergeCellsRecord : StandardRecord, ICloneable
{
	public const short sid = 229;

	private CellRangeAddress[] _regions;

	private int _startIndex;

	private int _numberOfRegions;

	public short NumAreas
	{
		get
		{
			return (short)_numberOfRegions;
		}
		set
		{
			throw new NotImplementedException();
		}
	}

	protected override int DataSize => CellRangeAddressList.GetEncodedSize(_numberOfRegions);

	public override short Sid => 229;

	public MergeCellsRecord(CellRangeAddress[] regions, int startIndex, int numberOfRegions)
	{
		_regions = regions;
		_startIndex = startIndex;
		_numberOfRegions = numberOfRegions;
	}

	public MergeCellsRecord(RecordInputStream in1)
	{
		int num = in1.ReadUShort();
		CellRangeAddress[] array = new CellRangeAddress[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = new CellRangeAddress(in1);
		}
		_numberOfRegions = num;
		_startIndex = 0;
		_regions = array;
	}

	public IEnumerator GetEnumerator()
	{
		return _regions.GetEnumerator();
	}

	public CellRangeAddress GetAreaAt(int index)
	{
		return _regions[_startIndex + index];
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		int numberOfRegions = _numberOfRegions;
		out1.WriteShort(numberOfRegions);
		for (int i = 0; i < _numberOfRegions; i++)
		{
			_regions[_startIndex + i].Serialize(out1);
		}
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[MERGEDCELLS]").Append("\n");
		stringBuilder.Append("     .numregions =").Append(NumAreas).Append("\n");
		for (int i = 0; i < _numberOfRegions; i++)
		{
			CellRangeAddress cellRangeAddress = _regions[_startIndex + i];
			stringBuilder.Append("     .rowfrom    =").Append(cellRangeAddress.FirstRow).Append("\n");
			stringBuilder.Append("     .rowto      =").Append(cellRangeAddress.LastRow).Append("\n");
			stringBuilder.Append("     .colfrom    =").Append(cellRangeAddress.FirstColumn).Append("\n");
			stringBuilder.Append("     .colto      =").Append(cellRangeAddress.LastColumn).Append("\n");
		}
		stringBuilder.Append("[MERGEDCELLS]").Append("\n");
		return stringBuilder.ToString();
	}

	public override object Clone()
	{
		int numberOfRegions = _numberOfRegions;
		CellRangeAddress[] array = new CellRangeAddress[numberOfRegions];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = _regions[_startIndex + i].Copy();
		}
		return new MergeCellsRecord(array, 0, numberOfRegions);
	}
}
