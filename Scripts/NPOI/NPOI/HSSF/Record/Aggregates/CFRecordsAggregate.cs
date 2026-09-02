using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NPOI.HSSF.Model;
using NPOI.SS.Formula;
using NPOI.SS.Formula.PTG;
using NPOI.SS.Util;
using NPOI.Util;

namespace NPOI.HSSF.Record.Aggregates;

public class CFRecordsAggregate : RecordAggregate
{
	private const int MAX_97_2003_CONDTIONAL_FORMAT_RULES = 3;

	public const short sid = -2008;

	private CFHeaderBase header;

	private List<CFRuleBase> rules;

	public override short Sid => -2008;

	public CFHeaderBase Header => header;

	public int NumberOfRules => rules.Count;

	private CFRecordsAggregate(CFHeaderBase pHeader, CFRuleBase[] pRules)
	{
		if (pHeader == null)
		{
			throw new ArgumentException("header must not be null");
		}
		if (pRules == null)
		{
			throw new ArgumentException("rules must not be null");
		}
		if (pRules.Length > 3)
		{
			Console.WriteLine("Excel versions before 2007 require that No more than " + 3 + " rules may be specified, " + pRules.Length + " were found, this file will cause problems with old Excel versions");
		}
		if (pRules.Length != pHeader.NumberOfConditionalFormats)
		{
			throw new RecordFormatException("Mismatch number of rules");
		}
		header = pHeader;
		rules = new List<CFRuleBase>(pRules.Length);
		foreach (CFRuleBase cFRuleBase in pRules)
		{
			CheckRuleType(cFRuleBase);
			rules.Add(cFRuleBase);
		}
	}

	public CFRecordsAggregate(CellRangeAddress[] regions, CFRuleBase[] rules)
		: this(CreateHeader(regions, rules), rules)
	{
	}

	private static CFHeaderBase CreateHeader(CellRangeAddress[] regions, CFRuleBase[] rules)
	{
		CFHeaderBase cFHeaderBase = ((rules.Length != 0 && !(rules[0] is CFRuleRecord)) ? ((CFHeaderBase)new CFHeader12Record(regions, rules.Length)) : ((CFHeaderBase)new CFHeaderRecord(regions, rules.Length)));
		cFHeaderBase.NeedRecalculation = true;
		return cFHeaderBase;
	}

	public static CFRecordsAggregate CreateCFAggregate(RecordStream rs)
	{
		Record next = rs.GetNext();
		if (next.Sid != CFHeaderRecord.sid && next.Sid != CFHeader12Record.sid)
		{
			throw new InvalidOperationException("next record sid was " + next.Sid + " instead of " + CFHeaderRecord.sid + " or " + CFHeader12Record.sid + " as expected");
		}
		CFHeaderBase cFHeaderBase = (CFHeaderBase)next;
		CFRuleBase[] array = new CFRuleBase[cFHeaderBase.NumberOfConditionalFormats];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = (CFRuleBase)rs.GetNext();
		}
		return new CFRecordsAggregate(cFHeaderBase, array);
	}

	[Obsolete("Not found in poi(2015-07-14), maybe was removed")]
	public static CFRecordsAggregate CreateCFAggregate(IList recs, int pOffset)
	{
		Record record = (Record)recs[pOffset];
		if (record.Sid != CFHeaderRecord.sid)
		{
			throw new InvalidOperationException("next record sid was " + record.Sid + " instead of " + CFHeaderRecord.sid + " as expected");
		}
		CFHeaderRecord cFHeaderRecord = (CFHeaderRecord)record;
		int numberOfConditionalFormats = cFHeaderRecord.NumberOfConditionalFormats;
		CFRuleRecord[] array = new CFRuleRecord[numberOfConditionalFormats];
		int num = pOffset;
		int i;
		for (i = 0; i < array.Length; i++)
		{
			num++;
			if (num >= recs.Count)
			{
				break;
			}
			record = (Record)recs[num];
			if (!(record is CFRuleRecord))
			{
				break;
			}
			array[i] = (CFRuleRecord)record;
		}
		if (i < numberOfConditionalFormats)
		{
			cFHeaderRecord.NumberOfConditionalFormats = numberOfConditionalFormats;
			CFRuleRecord[] array2 = new CFRuleRecord[i];
			Array.Copy(array, 0, array2, 0, i);
			array = array2;
		}
		CFRuleBase[] pRules = array;
		return new CFRecordsAggregate(cFHeaderRecord, pRules);
	}

	public override void VisitContainedRecords(RecordVisitor rv)
	{
		rv.VisitRecord(header);
		foreach (CFRuleBase rule in rules)
		{
			rv.VisitRecord(rule);
		}
	}

	public CFRecordsAggregate CloneCFAggregate()
	{
		CFRuleBase[] array = new CFRuleBase[rules.Count];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = (CFRuleRecord)GetRule(i).Clone();
		}
		return new CFRecordsAggregate((CFHeaderBase)header.Clone(), array);
	}

	public override int Serialize(int offset, byte[] data)
	{
		int count = rules.Count;
		header.NumberOfConditionalFormats = count;
		int num = offset;
		num += header.Serialize(num, data);
		for (int i = 0; i < count; i++)
		{
			num += GetRule(i).Serialize(num, data);
		}
		return num - offset;
	}

	private void CheckRuleIndex(int idx)
	{
		if (idx < 0 || idx >= rules.Count)
		{
			throw new ArgumentException("Bad rule record index (" + idx + ") nRules=" + rules.Count);
		}
	}

	private void CheckRuleType(CFRuleBase r)
	{
		if ((header is CFHeaderRecord && r is CFRuleRecord) || (header is CFHeader12Record && r is CFRule12Record))
		{
			return;
		}
		throw new ArgumentException("Header and Rule must both be CF or both be CF12, can't mix");
	}

	public CFRuleBase GetRule(int idx)
	{
		CheckRuleIndex(idx);
		return rules[idx];
	}

	public void SetRule(int idx, CFRuleBase r)
	{
		CheckRuleIndex(idx);
		CheckRuleType(r);
		rules[idx] = r;
	}

	public bool UpdateFormulasAfterCellShift(FormulaShifter shifter, int currentExternSheetIx)
	{
		CellRangeAddress[] cellRanges = header.CellRanges;
		bool flag = false;
		List<CellRangeAddress> list = new List<CellRangeAddress>();
		CellRangeAddress[] array = cellRanges;
		foreach (CellRangeAddress cellRangeAddress in array)
		{
			CellRangeAddress cellRangeAddress2 = ShiftRange(shifter, cellRangeAddress, currentExternSheetIx);
			if (cellRangeAddress2 == null)
			{
				flag = true;
				continue;
			}
			list.Add(cellRangeAddress2);
			if (cellRangeAddress2 != cellRangeAddress)
			{
				flag = true;
			}
		}
		if (flag)
		{
			int count = list.Count;
			if (count == 0)
			{
				return false;
			}
			CellRangeAddress[] array2 = new CellRangeAddress[count];
			array2 = list.ToArray();
			header.CellRanges = array2;
		}
		foreach (CFRuleBase rule in rules)
		{
			Ptg[] parsedExpression = rule.ParsedExpression1;
			if (parsedExpression != null && shifter.AdjustFormula(parsedExpression, currentExternSheetIx))
			{
				rule.ParsedExpression1 = parsedExpression;
			}
			parsedExpression = rule.ParsedExpression2;
			if (parsedExpression != null && shifter.AdjustFormula(parsedExpression, currentExternSheetIx))
			{
				rule.ParsedExpression2 = parsedExpression;
			}
			if (rule is CFRule12Record)
			{
				CFRule12Record cFRule12Record = (CFRule12Record)rule;
				parsedExpression = cFRule12Record.ParsedExpressionScale;
				if (parsedExpression != null && shifter.AdjustFormula(parsedExpression, currentExternSheetIx))
				{
					cFRule12Record.ParsedExpressionScale = parsedExpression;
				}
			}
		}
		return true;
	}

	private static CellRangeAddress ShiftRange(FormulaShifter shifter, CellRangeAddress cra, int currentExternSheetIx)
	{
		AreaPtg areaPtg = new AreaPtg(cra.FirstRow, cra.LastRow, cra.FirstColumn, cra.LastColumn, firstRowRelative: false, lastRowRelative: false, firstColRelative: false, lastColRelative: false);
		Ptg[] array = new Ptg[1] { areaPtg };
		if (!shifter.AdjustFormula(array, currentExternSheetIx))
		{
			return cra;
		}
		Ptg ptg = array[0];
		if (ptg is AreaPtg)
		{
			AreaPtg areaPtg2 = (AreaPtg)ptg;
			return new CellRangeAddress(areaPtg2.FirstRow, areaPtg2.LastRow, areaPtg2.FirstColumn, areaPtg2.LastColumn);
		}
		if (ptg is AreaErrPtg)
		{
			return null;
		}
		throw new InvalidCastException("Unexpected shifted ptg class (" + ptg.GetType().Name + ")");
	}

	public void AddRule(CFRuleBase r)
	{
		if (rules.Count >= 3)
		{
			Console.WriteLine("Excel versions before 2007 cannot cope with any more than " + 3 + " - this file will cause problems with old Excel versions");
		}
		CheckRuleType(r);
		rules.Add(r);
		header.NumberOfConditionalFormats = rules.Count;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		string value = "CF";
		if (header is CFHeader12Record)
		{
			value = "CF12";
		}
		stringBuilder.Append("[").Append(value).Append("]\n");
		if (header != null)
		{
			stringBuilder.Append(header.ToString());
		}
		foreach (CFRuleBase rule in rules)
		{
			if (rule != null)
			{
				stringBuilder.Append(rule.ToString());
			}
		}
		stringBuilder.Append("[/CF]\n");
		return stringBuilder.ToString();
	}
}
