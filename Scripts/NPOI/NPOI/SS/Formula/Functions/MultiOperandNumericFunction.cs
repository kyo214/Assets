using System;
using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public abstract class MultiOperandNumericFunction : Function
{
	private class DoubleList
	{
		private double[] _array;

		private int _Count;

		public DoubleList()
		{
			_array = new double[8];
			_Count = 0;
		}

		public double[] ToArray()
		{
			if (_Count < 1)
			{
				return EMPTY_DOUBLE_ARRAY;
			}
			double[] array = new double[_Count];
			Array.Copy(_array, 0, array, 0, _Count);
			return array;
		}

		public void Add(double[] values)
		{
			int num = values.Length;
			EnsureCapacity(_Count + num);
			Array.Copy(values, 0, _array, _Count, num);
			_Count += num;
		}

		private void EnsureCapacity(int reqSize)
		{
			if (reqSize > _array.Length)
			{
				double[] array = new double[reqSize * 3 / 2];
				Array.Copy(_array, 0, array, 0, _Count);
				_array = array;
			}
		}

		public void Add(double value)
		{
			EnsureCapacity(_Count + 1);
			_array[_Count] = value;
			_Count++;
		}
	}

	private static double[] EMPTY_DOUBLE_ARRAY = new double[0];

	private bool _isReferenceBoolCounted;

	private bool _isBlankCounted;

	private const int DEFAULT_MAX_NUM_OPERANDS = 30;

	protected virtual int MaxNumOperands => 30;

	public virtual bool IsSubtotalCounted => true;

	protected MultiOperandNumericFunction(bool isReferenceBoolCounted, bool isBlankCounted)
	{
		_isReferenceBoolCounted = isReferenceBoolCounted;
		_isBlankCounted = isBlankCounted;
	}

	protected internal abstract double Evaluate(double[] values);

	public ValueEval Evaluate(ValueEval[] args, int srcCellRow, int srcCellCol)
	{
		double num;
		try
		{
			double[] numberArray = GetNumberArray(args);
			num = Evaluate(numberArray);
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

	private void CollectValues(ValueEval operand, DoubleList temp)
	{
		if (operand is ThreeDEval)
		{
			ThreeDEval threeDEval = (ThreeDEval)operand;
			for (int i = threeDEval.FirstSheetIndex; i <= threeDEval.LastSheetIndex; i++)
			{
				int width = threeDEval.Width;
				int height = threeDEval.Height;
				for (int j = 0; j < height; j++)
				{
					for (int k = 0; k < width; k++)
					{
						ValueEval value = threeDEval.GetValue(i, j, k);
						if (IsSubtotalCounted || !threeDEval.IsSubTotal(j, k))
						{
							CollectValue(value, isViaReference: true, temp);
						}
					}
				}
			}
		}
		else if (operand is TwoDEval)
		{
			TwoDEval twoDEval = (TwoDEval)operand;
			int width2 = twoDEval.Width;
			int height2 = twoDEval.Height;
			for (int l = 0; l < height2; l++)
			{
				for (int m = 0; m < width2; m++)
				{
					ValueEval value2 = twoDEval.GetValue(l, m);
					if (IsSubtotalCounted || !twoDEval.IsSubTotal(l, m))
					{
						CollectValue(value2, isViaReference: true, temp);
					}
				}
			}
		}
		else if (operand is RefEval)
		{
			RefEval refEval = (RefEval)operand;
			for (int n = refEval.FirstSheetIndex; n <= refEval.LastSheetIndex; n++)
			{
				CollectValue(refEval.GetInnerValueEval(n), isViaReference: true, temp);
			}
		}
		else
		{
			CollectValue(operand, isViaReference: false, temp);
		}
	}

	private void CollectValue(ValueEval ve, bool isViaReference, DoubleList temp)
	{
		if (ve == null)
		{
			throw new ArgumentException("ve must not be null");
		}
		if (ve is BoolEval)
		{
			if (!isViaReference || _isReferenceBoolCounted)
			{
				BoolEval boolEval = (BoolEval)ve;
				temp.Add(boolEval.NumberValue);
			}
			return;
		}
		if (ve is NumberEval)
		{
			NumberEval numberEval = (NumberEval)ve;
			temp.Add(numberEval.NumberValue);
			return;
		}
		if (ve is StringEval)
		{
			if (!isViaReference)
			{
				double num = OperandResolver.ParseDouble(((StringEval)ve).StringValue);
				if (double.IsNaN(num))
				{
					throw new EvaluationException(ErrorEval.VALUE_INVALID);
				}
				temp.Add(num);
			}
			return;
		}
		if (ve is ErrorEval)
		{
			throw new EvaluationException((ErrorEval)ve);
		}
		if (ve == BlankEval.instance)
		{
			if (_isBlankCounted)
			{
				temp.Add(0.0);
			}
			return;
		}
		throw new InvalidOperationException("Invalid ValueEval type passed for conversion: (" + ve.GetType()?.ToString() + ")");
	}

	protected double[] GetNumberArray(ValueEval[] operands)
	{
		if (operands.Length > MaxNumOperands)
		{
			throw EvaluationException.InvalidValue();
		}
		DoubleList doubleList = new DoubleList();
		int i = 0;
		for (int num = operands.Length; i < num; i++)
		{
			CollectValues(operands[i], doubleList);
		}
		return doubleList.ToArray();
	}

	protected static bool AreSubArraysConsistent(double[][] values)
	{
		if (values == null || values.Length < 1)
		{
			return true;
		}
		if (values[0] == null)
		{
			return false;
		}
		int num = values.Length;
		int num2 = values[0].Length;
		for (int i = 1; i < num; i++)
		{
			double[] array = values[i];
			if (array == null)
			{
				return false;
			}
			if (num2 != array.Length)
			{
				return false;
			}
		}
		return true;
	}
}
