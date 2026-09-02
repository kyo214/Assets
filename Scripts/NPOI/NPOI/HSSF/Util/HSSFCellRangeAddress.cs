using System.Collections;
using NPOI.HSSF.Record;
using NPOI.Util;

namespace NPOI.HSSF.Util;

public class HSSFCellRangeAddress
{
	public class AddrStructure
	{
		private short _first_row;

		private short _first_col;

		private short _last_row;

		private short _last_col;

		public short FirstColumn
		{
			get
			{
				return _first_col;
			}
			set
			{
				_first_col = value;
			}
		}

		public short FirstRow
		{
			get
			{
				return _first_row;
			}
			set
			{
				_first_row = value;
			}
		}

		public short LastColumn
		{
			get
			{
				return _last_col;
			}
			set
			{
				_last_col = value;
			}
		}

		public short LastRow
		{
			get
			{
				return _last_row;
			}
			set
			{
				_last_row = value;
			}
		}

		public AddrStructure(short first_row, short last_row, short first_col, short last_col)
		{
			_first_row = first_row;
			_last_row = last_row;
			_first_col = first_col;
			_last_col = last_col;
		}
	}

	private static POILogger logger = POILogFactory.GetLogger(typeof(HSSFCellRangeAddress));

	private short field_Addr_number;

	private ArrayList field_regions_list;

	public short AddRStructureNumber => field_Addr_number;

	public int Size => 2 + field_Addr_number * 8;

	public HSSFCellRangeAddress()
	{
	}

	public HSSFCellRangeAddress(RecordInputStream in1)
	{
		FillFields(in1);
	}

	public void FillFields(RecordInputStream in1)
	{
		field_Addr_number = in1.ReadShort();
		field_regions_list = new ArrayList(field_Addr_number);
		for (int i = 0; i < field_Addr_number; i++)
		{
			short num = in1.ReadShort();
			short num2 = in1.ReadShort();
			short first_col = num;
			short last_col = num2;
			if (in1.Remaining >= 4)
			{
				first_col = in1.ReadShort();
				last_col = in1.ReadShort();
			}
			else
			{
				logger.Log(5, "Ran out of data reading cell references for DVRecord");
				i = field_Addr_number;
			}
			AddrStructure value = new AddrStructure(num, num2, first_col, last_col);
			field_regions_list.Add(value);
		}
	}

	public int AddAddRStructure(short first_row, short first_col, short last_row, short last_col)
	{
		if (field_regions_list == null)
		{
			field_Addr_number = 0;
			field_regions_list = new ArrayList(10);
		}
		AddrStructure value = new AddrStructure(first_row, last_row, first_col, last_col);
		field_regions_list.Add(value);
		field_Addr_number++;
		return field_Addr_number;
	}

	public void RemoveAddRStructureAt(int index)
	{
		field_regions_list.Remove(index);
		field_Addr_number--;
	}

	public AddrStructure GetAddRStructureAt(int index)
	{
		return (AddrStructure)field_regions_list[index];
	}

	public int Serialize(int offSet, byte[] data)
	{
		int num = 2;
		LittleEndian.PutShort(data, offSet, AddRStructureNumber);
		for (int i = 0; i < AddRStructureNumber; i++)
		{
			AddrStructure addRStructureAt = GetAddRStructureAt(i);
			LittleEndian.PutShort(data, offSet + num, addRStructureAt.FirstRow);
			num += 2;
			LittleEndian.PutShort(data, offSet + num, addRStructureAt.LastRow);
			num += 2;
			LittleEndian.PutShort(data, offSet + num, addRStructureAt.FirstColumn);
			num += 2;
			LittleEndian.PutShort(data, offSet + num, addRStructureAt.LastColumn);
			num += 2;
		}
		return Size;
	}
}
