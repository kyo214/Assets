using System;
using System.Text;
using NPOI.HSSF.UserModel;
using NPOI.SS.Formula;
using NPOI.SS.Formula.PTG;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class CFRuleRecord : CFRuleBase, ICloneable
{
	public static short sid = 433;

	public override short Sid => sid;

	protected override int DataSize => 6 + base.FormattingBlockSize + CFRuleBase.GetFormulaSize(base.Formula1) + CFRuleBase.GetFormulaSize(base.Formula2);

	private CFRuleRecord(byte conditionType, byte comparisonOperation)
		: base(conditionType, comparisonOperation)
	{
		SetDefaults();
	}

	private CFRuleRecord(byte conditionType, byte comparisonOperation, Ptg[] formula1, Ptg[] formula2)
		: base(conditionType, comparisonOperation, formula1, formula2)
	{
		SetDefaults();
	}

	private void SetDefaults()
	{
		formatting_options = CFRuleBase.modificationBits.SetValue(formatting_options, -1);
		formatting_options = CFRuleBase.fmtBlockBits.SetValue(formatting_options, 0);
		formatting_options = CFRuleBase.undocumented.Clear(formatting_options);
		formatting_not_used = -32766;
		_fontFormatting = null;
		_borderFormatting = null;
		_patternFormatting = null;
	}

	public static CFRuleRecord Create(HSSFSheet sheet, string formulaText)
	{
		Ptg[] array = CFRuleBase.ParseFormula(formulaText, sheet);
		return new CFRuleRecord(2, ComparisonOperator.NO_COMPARISON, array, null);
	}

	public static CFRuleRecord Create(HSSFSheet sheet, byte comparisonOperation, string formulaText1, string formulaText2)
	{
		Ptg[] array = CFRuleBase.ParseFormula(formulaText1, sheet);
		Ptg[] array2 = CFRuleBase.ParseFormula(formulaText2, sheet);
		return new CFRuleRecord(1, comparisonOperation, array, array2);
	}

	public CFRuleRecord(RecordInputStream in1)
	{
		base.ConditionType = (byte)in1.ReadByte();
		base.ComparisonOperation = (byte)in1.ReadByte();
		int encodedTokenLen = in1.ReadUShort();
		int encodedTokenLen2 = in1.ReadUShort();
		ReadFormatOptions(in1);
		base.Formula1 = Formula.Read(encodedTokenLen, in1);
		base.Formula2 = Formula.Read(encodedTokenLen2, in1);
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		int formulaSize = CFRuleBase.GetFormulaSize(base.Formula1);
		int formulaSize2 = CFRuleBase.GetFormulaSize(base.Formula2);
		out1.WriteByte(base.ConditionType);
		out1.WriteByte(base.ComparisonOperation);
		out1.WriteShort(formulaSize);
		out1.WriteShort(formulaSize2);
		SerializeFormattingBlock(out1);
		base.Formula1.SerializeTokens(out1);
		base.Formula2.SerializeTokens(out1);
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[CFRULE]\n");
		stringBuilder.Append("    .condition_type   =").Append(base.ConditionType).Append("\n");
		stringBuilder.Append("    OPTION FLAGS=0x").Append(HexDump.ToHex(base.Options)).Append("\n");
		if (base.ContainsFontFormattingBlock)
		{
			stringBuilder.Append(_fontFormatting.ToString()).Append("\n");
		}
		if (base.ContainsBorderFormattingBlock)
		{
			stringBuilder.Append(_borderFormatting.ToString()).Append("\n");
		}
		if (base.ContainsPatternFormattingBlock)
		{
			stringBuilder.Append(_patternFormatting.ToString()).Append("\n");
		}
		StringBuilder stringBuilder2 = stringBuilder.Append("    Formula 1 =");
		object[] tokens = base.Formula1.Tokens;
		stringBuilder2.Append(Arrays.ToString(tokens)).Append("\n");
		StringBuilder stringBuilder3 = stringBuilder.Append("    Formula 2 =");
		tokens = base.Formula2.Tokens;
		stringBuilder3.Append(Arrays.ToString(tokens)).Append("\n");
		stringBuilder.Append("[/CFRULE]\n");
		return stringBuilder.ToString();
	}

	public override object Clone()
	{
		CFRuleRecord cFRuleRecord = new CFRuleRecord(base.ConditionType, base.ComparisonOperation);
		CopyTo(cFRuleRecord);
		return cFRuleRecord;
	}
}
