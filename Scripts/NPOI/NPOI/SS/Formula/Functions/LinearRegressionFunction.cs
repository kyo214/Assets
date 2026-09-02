using System;
using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public class LinearRegressionFunction : Fixed2ArgFunction
{
	private abstract class ValueArray : ValueVector
	{
		private int _size;

		public int Size => _size;

		protected ValueArray(int size)
		{
			_size = size;
		}

		public ValueEval GetItem(int index)
		{
			if (index < 0 || index > _size)
			{
				throw new ArgumentException("Specified index " + index + " is outside range (0.." + (_size - 1) + ")");
			}
			return GetItemInternal(index);
		}

		protected abstract ValueEval GetItemInternal(int index);
	}

	private class SingleCellValueArray : ValueArray
	{
		private ValueEval _value;

		public SingleCellValueArray(ValueEval value)
			: base(1)
		{
			_value = value;
		}

		protected override ValueEval GetItemInternal(int index)
		{
			return _value;
		}
	}

	private class RefValueArray : ValueArray
	{
		private RefEval _ref;

		private int _width;

		public RefValueArray(RefEval ref1)
			: base(ref1.NumberOfSheets)
		{
			_ref = ref1;
			_width = ref1.NumberOfSheets;
		}

		protected override ValueEval GetItemInternal(int index)
		{
			int sheetIndex = index % _width + _ref.FirstSheetIndex;
			return _ref.GetInnerValueEval(sheetIndex);
		}
	}

	private class AreaValueArray : ValueArray
	{
		private TwoDEval _ae;

		private int _width;

		public AreaValueArray(TwoDEval ae)
			: base(ae.Width * ae.Height)
		{
			_ae = ae;
			_width = ae.Width;
		}

		protected override ValueEval GetItemInternal(int index)
		{
			int rowIndex = index / _width;
			int columnIndex = index % _width;
			return _ae.GetValue(rowIndex, columnIndex);
		}
	}

	public enum FUNCTION
	{
		INTERCEPT = 0,
		SLOPE = 1
	}

	public FUNCTION function;

	public LinearRegressionFunction(FUNCTION function)
	{
		this.function = function;
	}

	public override ValueEval Evaluate(int srcRowIndex, int srcColumnIndex, ValueEval arg0, ValueEval arg1)
	{
		double num;
		try
		{
			ValueVector valueVector = CreateValueVector(arg0);
			ValueVector valueVector2 = CreateValueVector(arg1);
			int size = valueVector2.Size;
			if (size == 0 || valueVector.Size != size)
			{
				return ErrorEval.NA;
			}
			num = EvaluateInternal(valueVector2, valueVector, size);
		}
		catch (EvaluationException ex)
		{
			return ex.GetErrorEval();
		}
		if (double.IsNaN(num) || double.IsInfinity(num))
		{
			return ErrorEval.NUM_ERROR;
		}
		return new NumberEval(num);
	}

	private double EvaluateInternal(ValueVector x, ValueVector y, int size)
	{
		ErrorEval errorEval = null;
		ErrorEval errorEval2 = null;
		bool flag = false;
		double num = 0.0;
		double num2 = 0.0;
		for (int i = 0; i < size; i++)
		{
			ValueEval item = x.GetItem(i);
			ValueEval item2 = y.GetItem(i);
			if (item is ErrorEval && errorEval == null)
			{
				errorEval = (ErrorEval)item;
			}
			else if (item2 is ErrorEval && errorEval2 == null)
			{
				errorEval2 = (ErrorEval)item2;
			}
			else if (item is NumberEval && item2 is NumberEval)
			{
				flag = true;
				NumberEval numberEval = (NumberEval)item;
				NumberEval numberEval2 = (NumberEval)item2;
				num += numberEval.NumberValue;
				num2 += numberEval2.NumberValue;
			}
		}
		double num3 = num / (double)size;
		double num4 = num2 / (double)size;
		double num5 = 0.0;
		double num6 = 0.0;
		for (int j = 0; j < size; j++)
		{
			ValueEval item3 = x.GetItem(j);
			ValueEval item4 = y.GetItem(j);
			if (item3 is ErrorEval && errorEval == null)
			{
				errorEval = (ErrorEval)item3;
			}
			else if (item4 is ErrorEval && errorEval2 == null)
			{
				errorEval2 = (ErrorEval)item4;
			}
			else if (item3 is NumberEval && item4 is NumberEval)
			{
				NumberEval numberEval3 = (NumberEval)item3;
				NumberEval numberEval4 = (NumberEval)item4;
				num5 += (numberEval3.NumberValue - num3) * (numberEval3.NumberValue - num3);
				num6 += (numberEval3.NumberValue - num3) * (numberEval4.NumberValue - num4);
			}
		}
		double num7 = num6 / num5;
		double result = num4 - num7 * num3;
		if (errorEval != null)
		{
			throw new EvaluationException(errorEval);
		}
		if (errorEval2 != null)
		{
			throw new EvaluationException(errorEval2);
		}
		if (!flag)
		{
			throw new EvaluationException(ErrorEval.DIV_ZERO);
		}
		if (function == FUNCTION.INTERCEPT)
		{
			return result;
		}
		return num7;
	}

	private ValueVector CreateValueVector(ValueEval arg)
	{
		if (arg is ErrorEval)
		{
			throw new EvaluationException((ErrorEval)arg);
		}
		if (arg is TwoDEval)
		{
			return new AreaValueArray((TwoDEval)arg);
		}
		if (arg is RefEval)
		{
			return new RefValueArray((RefEval)arg);
		}
		return new SingleCellValueArray(arg);
	}
}
