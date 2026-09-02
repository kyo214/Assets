using System;
using System.Collections;
using System.IO;
using System.Text;
using NPOI.POIFS.Dev;
using NPOI.Util;

namespace NPOI.POIFS.Properties;

public abstract class Property : Child, POIFSViewable
{
	private const byte _default_fill = 0;

	private const int _name_size_offset = 64;

	private const int _max_name_length = 31;

	protected const int _NO_INDEX = -1;

	private const int _node_color_offset = 67;

	private const int _previous_property_offset = 68;

	private const int _next_property_offset = 72;

	private const int _child_property_offset = 76;

	private const int _storage_clsid_offset = 80;

	private const int _user_flags_offset = 96;

	private const int _seconds_1_offset = 100;

	private const int _days_1_offset = 104;

	private const int _seconds_2_offset = 108;

	private const int _days_2_offset = 112;

	private const int _start_block_offset = 116;

	private const int _size_offset = 120;

	protected const byte _NODE_BLACK = 1;

	protected const byte _NODE_RED = 0;

	private const int _big_block_minimum_bytes = 4096;

	private string _name;

	private ShortField _name_size;

	private ByteField _property_type;

	private ByteField _node_color;

	private IntegerField _previous_property;

	private IntegerField _next_property;

	private IntegerField _child_property;

	private ClassID _storage_clsid;

	private IntegerField _user_flags;

	private IntegerField _seconds_1;

	private IntegerField _days_1;

	private IntegerField _seconds_2;

	private IntegerField _days_2;

	private IntegerField _start_block;

	private IntegerField _size;

	private byte[] _raw_data;

	private int _index;

	private Child _next_child;

	private Child _previous_child;

	public int StartBlock
	{
		get
		{
			return _start_block.Value;
		}
		set
		{
			_start_block.Set(value, _raw_data);
		}
	}

	public bool ShouldUseSmallBlocks => IsSmall(_size.Value);

	public string Name
	{
		get
		{
			return _name;
		}
		set
		{
			char[] array = value.ToCharArray();
			int num = Math.Min(array.Length, 31);
			_name = new string(array, 0, num);
			short num2 = 0;
			int i;
			for (i = 0; i < num; i++)
			{
				ShortField.Write(num2, (short)array[i], ref _raw_data);
				num2 += 2;
			}
			for (; i < 32; i++)
			{
				ShortField.Write(num2, 0, ref _raw_data);
				num2 += 2;
			}
			_name_size.Set((short)((num + 1) * 2), ref _raw_data);
		}
	}

	public virtual bool IsDirectory => false;

	public ClassID StorageClsid
	{
		get
		{
			return _storage_clsid;
		}
		set
		{
			_storage_clsid = value;
			if (value == null)
			{
				for (int i = 80; i < 96; i++)
				{
					_raw_data[i] = 0;
				}
			}
			else
			{
				value.Write(_raw_data, 80);
			}
		}
	}

	public byte PropertyType
	{
		set
		{
			_property_type.Set(value, _raw_data);
		}
	}

	public byte NodeColor
	{
		set
		{
			_node_color.Set(value, _raw_data);
		}
	}

	public int ChildProperty
	{
		set
		{
			_child_property.Set(value, _raw_data);
		}
	}

	public int ChildIndex => _child_property.Value;

	public virtual int Size
	{
		get
		{
			return _size.Value;
		}
		set
		{
			_size.Set(value, _raw_data);
		}
	}

	public int Index
	{
		get
		{
			return _index;
		}
		set
		{
			_index = value;
		}
	}

	public int NextChildIndex => _next_property.Value;

	public int PreviousChildIndex => _previous_property.Value;

	public Child PreviousChild
	{
		get
		{
			return _previous_child;
		}
		set
		{
			_previous_child = value;
			_previous_property.Set((value == null) ? (-1) : ((Property)value).Index, _raw_data);
		}
	}

	public Child NextChild
	{
		get
		{
			return _next_child;
		}
		set
		{
			_next_child = value;
			_next_property.Set((value == null) ? (-1) : ((Property)value).Index, _raw_data);
		}
	}

	public Array ViewableArray
	{
		get
		{
			string[] array = new string[5];
			array.SetValue("Name          = \"" + Name + "\"", 0);
			array.SetValue("Property Type = " + _property_type.Value, 1);
			array.SetValue("Node Color    = " + _node_color.Value, 2);
			long num = _days_1.Value;
			num <<= 32;
			array.SetValue("Time 1        = " + (num + (long)((ulong)_seconds_1.Value & 0xFFFFuL)), 3);
			num = _days_2.Value;
			num <<= 32;
			array.SetValue("Time 2        = " + (num + (long)((ulong)_seconds_2.Value & 0xFFFFuL)), 4);
			return array;
		}
	}

	public IEnumerator ViewableIterator => ArrayList.ReadOnly(new ArrayList()).GetEnumerator();

	public bool PreferArray => true;

	public string ShortDescription
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("Property: \"").Append(Name).Append("\"");
			return stringBuilder.ToString();
		}
	}

	protected Property()
	{
		_raw_data = new byte[128];
		for (int i = 0; i < _raw_data.Length; i++)
		{
			_raw_data[i] = 0;
		}
		_name_size = new ShortField(64);
		_property_type = new ByteField(66);
		_node_color = new ByteField(67);
		_previous_property = new IntegerField(68, -1, _raw_data);
		_next_property = new IntegerField(72, -1, _raw_data);
		_child_property = new IntegerField(76, -1, _raw_data);
		_storage_clsid = new ClassID(_raw_data, 80);
		_user_flags = new IntegerField(96, 0, _raw_data);
		_seconds_1 = new IntegerField(100, 0, _raw_data);
		_days_1 = new IntegerField(104, 0, _raw_data);
		_seconds_2 = new IntegerField(108, 0, _raw_data);
		_days_2 = new IntegerField(112, 0, _raw_data);
		_start_block = new IntegerField(116);
		_size = new IntegerField(120, 0, _raw_data);
		_index = -1;
		Name = "";
		NextChild = null;
		PreviousChild = null;
	}

	protected Property(int index, byte[] array, int offset)
	{
		_raw_data = new byte[128];
		Array.Copy(array, offset, _raw_data, 0, 128);
		_name_size = new ShortField(64, _raw_data);
		_property_type = new ByteField(66, _raw_data);
		_node_color = new ByteField(67, _raw_data);
		_previous_property = new IntegerField(68, _raw_data);
		_next_property = new IntegerField(72, _raw_data);
		_child_property = new IntegerField(76, _raw_data);
		_storage_clsid = new ClassID(_raw_data, 80);
		_user_flags = new IntegerField(96, 0, _raw_data);
		_seconds_1 = new IntegerField(100, _raw_data);
		_days_1 = new IntegerField(104, _raw_data);
		_seconds_2 = new IntegerField(108, _raw_data);
		_days_2 = new IntegerField(112, _raw_data);
		_start_block = new IntegerField(116, _raw_data);
		_size = new IntegerField(120, _raw_data);
		_index = index;
		int num = _name_size.Value / 2 - 1;
		if (num < 1)
		{
			_name = "";
		}
		else
		{
			char[] array2 = new char[num];
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				array2[i] = (char)new ShortField(num2, _raw_data).Value;
				num2 += 2;
			}
			_name = new string(array2, 0, num);
		}
		_next_child = null;
		_previous_child = null;
	}

	public void WriteData(Stream stream)
	{
		stream.Write(_raw_data, 0, _raw_data.Length);
	}

	public static bool IsSmall(int length)
	{
		return length < 4096;
	}

	public abstract void PreWrite();

	public static bool IsValidIndex(int index)
	{
		return index != -1;
	}
}
