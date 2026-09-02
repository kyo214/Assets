using System.Text;
using NPOI.SS.Formula;
using NPOI.SS.Formula.PTG;
using NPOI.SS.UserModel;
using NPOI.Util;

namespace NPOI.HSSF.Record.CF;

public abstract class Threshold
{
	private byte type;

	private Formula formula;

	private double? value;

	public byte Type
	{
		get
		{
			return type;
		}
		set
		{
			type = value;
			if (type == RangeType.MIN.id || type == RangeType.MAX.id || type == RangeType.FORMULA.id || type == RangeType.AUTOMIN.id || type == RangeType.AUTOMAX.id)
			{
				this.value = null;
			}
			else if (!this.value.HasValue)
			{
				this.value = 0.0;
			}
		}
	}

	protected Formula Formula => formula;

	public Ptg[] ParsedExpression
	{
		get
		{
			return formula.Tokens;
		}
		set
		{
			formula = Formula.Create(value);
			if (value.Length != 0)
			{
				this.value = null;
			}
		}
	}

	public double? Value
	{
		get
		{
			return value;
		}
		set
		{
			this.value = value;
		}
	}

	public virtual int DataLength
	{
		get
		{
			int num = 1 + formula.EncodedSize;
			if (value.HasValue)
			{
				num += 8;
			}
			return num;
		}
	}

	protected Threshold()
	{
		type = (byte)RangeType.NUMBER.id;
		formula = Formula.Create(null);
		value = 0.0;
	}

	protected Threshold(ILittleEndianInput in1)
	{
		type = (byte)in1.ReadByte();
		short num = in1.ReadShort();
		if (num > 0)
		{
			formula = Formula.Read(num, in1);
		}
		else
		{
			formula = Formula.Create(null);
		}
		if (num == 0 && type != RangeType.MIN.id && type != RangeType.MAX.id && type != RangeType.AUTOMIN.id && type != RangeType.AUTOMAX.id)
		{
			value = in1.ReadDouble();
		}
	}

	public void SetType(int type)
	{
		this.type = (byte)type;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("    [CF Threshold]\n");
		stringBuilder.Append("          .type    = ").Append(HexDump.ToHex(type)).Append("\n");
		StringBuilder stringBuilder2 = stringBuilder.Append("          .Formula = ");
		object[] tokens = formula.Tokens;
		stringBuilder2.Append(Arrays.ToString(tokens)).Append("\n");
		stringBuilder.Append("          .value   = ").Append(value).Append("\n");
		stringBuilder.Append("    [/CF Threshold]\n");
		return stringBuilder.ToString();
	}

	public void CopyTo(Threshold rec)
	{
		rec.type = type;
		rec.formula = formula;
		rec.value = value;
	}

	public virtual void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteByte(type);
		if (formula.Tokens.Length == 0)
		{
			out1.WriteShort(0);
		}
		else
		{
			formula.Serialize(out1);
		}
		if (value.HasValue)
		{
			out1.WriteDouble(value.Value);
		}
	}
}
