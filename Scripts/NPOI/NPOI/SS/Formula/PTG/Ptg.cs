using System;
using System.Collections;
using System.Collections.Generic;
using NPOI.Util;

namespace NPOI.SS.Formula.PTG;

[Serializable]
public abstract class Ptg : ICloneable
{
	public static Ptg[] EMPTY_PTG_ARRAY = new Ptg[0];

	public const byte CLASS_REF = 0;

	public const byte CLASS_VALUE = 32;

	public const byte CLASS_ARRAY = 64;

	private byte ptgClass;

	public abstract int Size { get; }

	public abstract bool IsBaseToken { get; }

	public byte PtgClass
	{
		get
		{
			return ptgClass;
		}
		set
		{
			if (IsBaseToken)
			{
				throw new Exception("SetClass should not be called on a base token");
			}
			ptgClass = value;
		}
	}

	public abstract byte DefaultOperandClass { get; }

	public char RVAType
	{
		get
		{
			if (IsBaseToken)
			{
				return '.';
			}
			return ptgClass switch
			{
				0 => 'R', 
				32 => 'V', 
				64 => 'A', 
				_ => throw new InvalidOperationException("Unknown operand class (" + ptgClass + ")"), 
			};
		}
	}

	public static Ptg[] ReadTokens(int size, ILittleEndianInput in1)
	{
		ArrayList arrayList = new ArrayList(4 + size / 2);
		int num = 0;
		bool flag = false;
		while (num < size)
		{
			Ptg ptg = CreatePtg(in1);
			if (ptg is ArrayPtg.Initial)
			{
				flag = true;
			}
			num += ptg.Size;
			arrayList.Add(ptg);
		}
		if (num != size)
		{
			throw new Exception("Ptg array size mismatch");
		}
		if (flag)
		{
			Ptg[] array = ToPtgArray(arrayList);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] is ArrayPtg.Initial)
				{
					array[i] = ((ArrayPtg.Initial)array[i]).FinishReading(in1);
				}
			}
			return array;
		}
		return ToPtgArray(arrayList);
	}

	public static Ptg CreatePtg(ILittleEndianInput in1)
	{
		byte b = (byte)in1.ReadByte();
		if (b < 32)
		{
			return CreateBasePtg(b, in1);
		}
		Ptg ptg = CreateClassifiedPtg(b, in1);
		if (b >= 96)
		{
			ptg.PtgClass = 64;
		}
		else if (b >= 64)
		{
			ptg.PtgClass = 32;
		}
		else
		{
			ptg.PtgClass = 0;
		}
		return ptg;
	}

	private static Ptg CreateClassifiedPtg(byte id, ILittleEndianInput in1)
	{
		switch ((id & 0x1F) | 0x20)
		{
		case 32:
			return new ArrayPtg.Initial(in1);
		case 33:
			return FuncPtg.Create(in1);
		case 34:
			return FuncVarPtg.Create(in1);
		case 35:
			return new NamePtg(in1);
		case 36:
			return new RefPtg(in1);
		case 37:
			return new AreaPtg(in1);
		case 38:
			return new MemAreaPtg(in1);
		case 39:
			return new MemErrPtg(in1);
		case 41:
			return new MemFuncPtg(in1);
		case 42:
			return new RefErrorPtg(in1);
		case 43:
			return new AreaErrPtg(in1);
		case 44:
			return new RefNPtg(in1);
		case 45:
			return new AreaNPtg(in1);
		case 57:
			return new NameXPtg(in1);
		case 58:
			return new Ref3DPtg(in1);
		case 59:
			return new Area3DPtg(in1);
		case 60:
			return new DeletedRef3DPtg(in1);
		case 61:
			return new DeletedArea3DPtg(in1);
		default:
		{
			string[] obj = new string[5]
			{
				" Unknown Ptg in Formula: 0x",
				StringUtil.ToHexString(id),
				" (",
				null,
				null
			};
			int num = id;
			obj[3] = num.ToString();
			obj[4] = ")";
			throw new NotSupportedException(string.Concat(obj));
		}
		}
	}

	private static Ptg CreateBasePtg(byte id, ILittleEndianInput in1)
	{
		return id switch
		{
			0 => new UnknownPtg(), 
			1 => new ExpPtg(in1), 
			2 => new TblPtg(in1), 
			3 => AddPtg.instance, 
			4 => SubtractPtg.instance, 
			5 => MultiplyPtg.instance, 
			6 => DividePtg.instance, 
			7 => PowerPtg.instance, 
			8 => ConcatPtg.instance, 
			9 => LessThanPtg.instance, 
			10 => LessEqualPtg.instance, 
			11 => EqualPtg.instance, 
			12 => GreaterEqualPtg.instance, 
			13 => GreaterThanPtg.instance, 
			14 => NotEqualPtg.instance, 
			15 => IntersectionPtg.instance, 
			16 => UnionPtg.instance, 
			17 => RangePtg.instance, 
			18 => UnaryPlusPtg.instance, 
			19 => UnaryMinusPtg.instance, 
			20 => PercentPtg.instance, 
			21 => ParenthesisPtg.instance, 
			22 => MissingArgPtg.instance, 
			23 => new StringPtg(in1), 
			25 => new AttrPtg(in1), 
			28 => new ErrPtg(in1), 
			29 => new BoolPtg(in1), 
			30 => new IntPtg(in1), 
			31 => new NumberPtg(in1), 
			_ => throw new Exception("Unexpected base token id (" + id + ")"), 
		};
	}

	private static Ptg[] ToPtgArray(ArrayList l)
	{
		if (l.Count == 0)
		{
			return EMPTY_PTG_ARRAY;
		}
		return (Ptg[])l.ToArray(typeof(Ptg));
	}

	public virtual object Clone()
	{
		return this.Copy();
	}

	public static int GetEncodedSize(Ptg[] ptgs)
	{
		int num = 0;
		foreach (Ptg ptg in ptgs)
		{
			num += ptg.Size;
		}
		return num;
	}

	public static int GetEncodedSizeWithoutArrayData(Ptg[] ptgs)
	{
		int num = 0;
		foreach (Ptg ptg in ptgs)
		{
			num = ((!(ptg is ArrayPtg)) ? (num + ptg.Size) : (num + 8));
		}
		return num;
	}

	public static int SerializePtgs(Ptg[] ptgs, byte[] array, int offset)
	{
		LittleEndianByteArrayOutputStream littleEndianByteArrayOutputStream = new LittleEndianByteArrayOutputStream(array, offset);
		List<Ptg> list = null;
		foreach (Ptg ptg in ptgs)
		{
			ptg.Write(littleEndianByteArrayOutputStream);
			if (ptg is ArrayPtg)
			{
				if (list == null)
				{
					list = new List<Ptg>(5);
				}
				list.Add(ptg);
			}
		}
		if (list != null)
		{
			foreach (ArrayPtg item in list)
			{
				item.WriteTokenValueBytes(littleEndianByteArrayOutputStream);
			}
		}
		return littleEndianByteArrayOutputStream.WriteIndex - offset;
	}

	public abstract void Write(ILittleEndianOutput out1);

	public abstract string ToFormulaString();

	public override string ToString()
	{
		return GetType().ToString();
	}

	object ICloneable.Clone()
	{
		throw new NotImplementedException();
	}

	public static bool DoesFormulaReferToDeletedCell(Ptg[] ptgs)
	{
		for (int i = 0; i < ptgs.Length; i++)
		{
			if (IsDeletedCellRef(ptgs[i]))
			{
				return true;
			}
		}
		return false;
	}

	private static bool IsDeletedCellRef(Ptg ptg)
	{
		if (ptg == ErrPtg.REF_INVALID)
		{
			return true;
		}
		if (ptg is DeletedArea3DPtg)
		{
			return true;
		}
		if (ptg is DeletedRef3DPtg)
		{
			return true;
		}
		if (ptg is AreaErrPtg)
		{
			return true;
		}
		if (ptg is RefErrorPtg)
		{
			return true;
		}
		return false;
	}
}
