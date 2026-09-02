using System;
using System.Text;
using NPOI.SS.Formula.Constant;
using NPOI.SS.Util;
using NPOI.Util;

namespace NPOI.SS.Formula.PTG;

public class ArrayPtg : Ptg
{
	public class Initial : Ptg
	{
		private int _reserved0;

		private int _reserved1;

		private int _reserved2;

		public override byte DefaultOperandClass
		{
			get
			{
				throw Invalid();
			}
		}

		public override int Size => 8;

		public override bool IsBaseToken => false;

		public Initial(ILittleEndianInput in1)
		{
			_reserved0 = in1.ReadInt();
			_reserved1 = in1.ReadUShort();
			_reserved2 = in1.ReadUByte();
		}

		private static Exception Invalid()
		{
			throw new InvalidOperationException("This object is a partially initialised tArray, and cannot be used as a Ptg");
		}

		public override string ToFormulaString()
		{
			throw Invalid();
		}

		public override void Write(ILittleEndianOutput out1)
		{
			throw Invalid();
		}

		public ArrayPtg FinishReading(ILittleEndianInput in1)
		{
			int num = in1.ReadUByte();
			short num2 = in1.ReadShort();
			num++;
			num2++;
			int nValues = num2 * num;
			object[] arrayValues = ConstantValueParser.Parse(in1, nValues);
			return new ArrayPtg(_reserved0, _reserved1, _reserved2, num, num2, arrayValues)
			{
				PtgClass = base.PtgClass
			};
		}
	}

	public const byte sid = 32;

	private const int RESERVED_FIELD_LEN = 7;

	public const int PLAIN_TOKEN_SIZE = 8;

	private int _reserved0Int;

	private int _reserved1Short;

	private int _reserved2Byte;

	private int _nColumns;

	private int _nRows;

	private object[] _arrayValues;

	public override bool IsBaseToken => false;

	public int RowCount => _nRows;

	public int ColumnCount => _nColumns;

	public override int Size => 11 + ConstantValueParser.GetEncodedSize(_arrayValues);

	public override byte DefaultOperandClass => 64;

	private ArrayPtg(int reserved0, int reserved1, int reserved2, int nColumns, int nRows, object[] arrayValues)
	{
		_reserved0Int = reserved0;
		_reserved1Short = reserved1;
		_reserved2Byte = reserved2;
		_nColumns = nColumns;
		_nRows = nRows;
		_arrayValues = (object[])arrayValues.Clone();
	}

	public ArrayPtg(object[][] values2d)
	{
		int num = values2d[0].Length;
		int num2 = values2d.Length;
		_nColumns = (short)num;
		_nRows = (short)num2;
		object[] array = new object[_nColumns * _nRows];
		for (int i = 0; i < num2; i++)
		{
			object[] array2 = values2d[i];
			for (int j = 0; j < num; j++)
			{
				array[GetValueIndex(j, i)] = array2[j];
			}
		}
		_arrayValues = array;
		_reserved0Int = 0;
		_reserved1Short = 0;
		_reserved2Byte = 0;
	}

	public object[][] GetTokenArrayValues()
	{
		if (_arrayValues == null)
		{
			throw new InvalidOperationException("array values not read yet");
		}
		object[][] array = new object[_nRows][];
		for (int i = 0; i < _nRows; i++)
		{
			array[i] = new object[_nColumns];
			for (int j = 0; j < _nColumns; j++)
			{
				array[i][j] = _arrayValues[GetValueIndex(j, i)];
			}
		}
		return array;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder("[ArrayPtg]\n");
		stringBuilder.Append("columns = ").Append(ColumnCount).Append("\n");
		stringBuilder.Append("rows = ").Append(RowCount).Append("\n");
		for (int i = 0; i < ColumnCount; i++)
		{
			for (int j = 0; j < RowCount; j++)
			{
				object value = _arrayValues.GetValue(GetValueIndex(i, j));
				stringBuilder.Append("[").Append(i).Append("][")
					.Append(j)
					.Append("] = ")
					.Append(value)
					.Append("\n");
			}
		}
		return stringBuilder.ToString();
	}

	public int GetValueIndex(int colIx, int rowIx)
	{
		if (colIx < 0 || colIx >= _nColumns)
		{
			throw new ArgumentException("Specified colIx (" + colIx + ") is outside the allowed range (0.." + (_nColumns - 1) + ")");
		}
		if (rowIx < 0 || rowIx >= _nRows)
		{
			throw new ArgumentException("Specified rowIx (" + rowIx + ") is outside the allowed range (0.." + (_nRows - 1) + ")");
		}
		return rowIx * _nColumns + colIx;
	}

	public override void Write(ILittleEndianOutput out1)
	{
		out1.WriteByte(32 + base.PtgClass);
		out1.WriteInt(_reserved0Int);
		out1.WriteShort(_reserved1Short);
		out1.WriteByte(_reserved2Byte);
	}

	public int WriteTokenValueBytes(ILittleEndianOutput out1)
	{
		out1.WriteByte(_nColumns - 1);
		out1.WriteShort(_nRows - 1);
		ConstantValueParser.Encode(out1, _arrayValues);
		return 3 + ConstantValueParser.GetEncodedSize(_arrayValues);
	}

	public override string ToFormulaString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("{");
		for (int i = 0; i < _nRows; i++)
		{
			if (i > 0)
			{
				stringBuilder.Append(";");
			}
			for (int j = 0; j < _nColumns; j++)
			{
				if (j > 0)
				{
					stringBuilder.Append(",");
				}
				object value = _arrayValues.GetValue(GetValueIndex(j, i));
				stringBuilder.Append(GetConstantText(value));
			}
		}
		stringBuilder.Append("}");
		return stringBuilder.ToString();
	}

	private static string GetConstantText(object o)
	{
		if (o == null)
		{
			return "";
		}
		if (o is string)
		{
			return "\"" + (string)o + "\"";
		}
		if (o is double || o is double)
		{
			return NumberToTextConverter.ToText((double)o);
		}
		if (o is bool || o is bool)
		{
			return ((bool)o).ToString().ToUpper();
		}
		if (o is ErrorConstant)
		{
			return ((ErrorConstant)o).Text;
		}
		throw new ArgumentException("Unexpected constant class (" + o.GetType().Name + ")");
	}
}
