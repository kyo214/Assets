using System.Text;
using NPOI.SS.Formula.PTG;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class LinkedDataFormulaField
{
	private Ptg[] formulaTokens;

	public int Size => 2 + Ptg.GetEncodedSize(formulaTokens);

	public Ptg[] FormulaTokens
	{
		get
		{
			return (Ptg[])formulaTokens.Clone();
		}
		set
		{
			formulaTokens = (Ptg[])value.Clone();
		}
	}

	public int FillField(RecordInputStream in1)
	{
		short num = in1.ReadShort();
		formulaTokens = Ptg.ReadTokens(num, in1);
		return num + 2;
	}

	public void toString(StringBuilder buffer)
	{
		for (int i = 0; i < formulaTokens.Length; i++)
		{
			buffer.Append("Formula ").Append(i).Append("=")
				.Append(formulaTokens[i].ToString())
				.Append("\n");
		}
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		toString(stringBuilder);
		return stringBuilder.ToString();
	}

	public int SerializeField(int offset, byte[] data)
	{
		int size = Size;
		LittleEndian.PutShort(data, offset, (short)(size - 2));
		int num = offset + 2;
		num += Ptg.SerializePtgs(formulaTokens, data, num);
		return size;
	}

	public LinkedDataFormulaField Copy()
	{
		return new LinkedDataFormulaField
		{
			formulaTokens = FormulaTokens
		};
	}
}
