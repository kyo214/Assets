using System;
using System.Text;
using NPOI.HSSF.Record.CF;
using NPOI.HSSF.Record.Common;
using NPOI.HSSF.UserModel;
using NPOI.SS.Formula;
using NPOI.SS.Formula.PTG;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class CFRule12Record : CFRuleBase, IFutureRecord, ICloneable
{
	public static short sid = 2170;

	private FtrHeader futureHeader;

	private int ext_formatting_length;

	private byte[] ext_formatting_data;

	private Formula formula_scale;

	private byte ext_opts;

	private int priority;

	private int template_type;

	private byte template_param_length;

	private byte[] template_params;

	private DataBarFormatting data_bar;

	private IconMultiStateFormatting multistate;

	private ColorGradientFormatting color_gradient;

	private byte[] filter_data;

	public DataBarFormatting DataBarFormatting => data_bar;

	public IconMultiStateFormatting MultiStateFormatting => multistate;

	public ColorGradientFormatting ColorGradientFormatting => color_gradient;

	public Ptg[] ParsedExpressionScale
	{
		get
		{
			return formula_scale.Tokens;
		}
		set
		{
			formula_scale = Formula.Create(value);
		}
	}

	public override short Sid => sid;

	protected override int DataSize
	{
		get
		{
			int num = FtrHeader.GetDataSize() + 6;
			num = ((ext_formatting_length != 0) ? (num + (4 + base.FormattingBlockSize + ext_formatting_data.Length)) : (num + 6));
			num += CFRuleBase.GetFormulaSize(base.Formula1);
			num += CFRuleBase.GetFormulaSize(base.Formula2);
			num += 2 + CFRuleBase.GetFormulaSize(formula_scale);
			num += 6 + template_params.Length;
			switch (base.ConditionType)
			{
			case 3:
				num += color_gradient.DataLength;
				break;
			case 4:
				num += data_bar.DataLength;
				break;
			case 5:
				num += filter_data.Length;
				break;
			case 6:
				num += multistate.DataLength;
				break;
			}
			return num;
		}
	}

	private CFRule12Record(byte conditionType, byte comparisonOperation)
		: base(conditionType, comparisonOperation)
	{
		SetDefaults();
	}

	private CFRule12Record(byte conditionType, byte comparisonOperation, Ptg[] formula1, Ptg[] formula2, Ptg[] formulaScale)
		: base(conditionType, comparisonOperation, formula1, formula2)
	{
		SetDefaults();
		formula_scale = Formula.Create(formulaScale);
	}

	private void SetDefaults()
	{
		futureHeader = new FtrHeader();
		futureHeader.RecordType = sid;
		ext_formatting_length = 0;
		ext_formatting_data = new byte[4];
		formula_scale = Formula.Create(Ptg.EMPTY_PTG_ARRAY);
		ext_opts = 0;
		priority = 0;
		template_type = base.ConditionType;
		template_param_length = 16;
		template_params = new byte[template_param_length];
	}

	public static CFRule12Record Create(HSSFSheet sheet, string formulaText)
	{
		Ptg[] array = CFRuleBase.ParseFormula(formulaText, sheet);
		return new CFRule12Record(2, ComparisonOperator.NO_COMPARISON, array, null, null);
	}

	public static CFRule12Record Create(HSSFSheet sheet, byte comparisonOperation, string formulaText1, string formulaText2)
	{
		Ptg[] array = CFRuleBase.ParseFormula(formulaText1, sheet);
		Ptg[] array2 = CFRuleBase.ParseFormula(formulaText2, sheet);
		return new CFRule12Record(1, comparisonOperation, array, array2, null);
	}

	public static CFRule12Record Create(HSSFSheet sheet, byte comparisonOperation, string formulaText1, string formulaText2, string formulaTextScale)
	{
		Ptg[] array = CFRuleBase.ParseFormula(formulaText1, sheet);
		Ptg[] array2 = CFRuleBase.ParseFormula(formulaText2, sheet);
		Ptg[] formulaScale = CFRuleBase.ParseFormula(formulaTextScale, sheet);
		return new CFRule12Record(1, comparisonOperation, array, array2, formulaScale);
	}

	public static CFRule12Record Create(HSSFSheet sheet, NPOI.HSSF.Record.Common.ExtendedColor color)
	{
		CFRule12Record cFRule12Record = new CFRule12Record(4, ComparisonOperator.NO_COMPARISON);
		DataBarFormatting dataBarFormatting = cFRule12Record.CreateDataBarFormatting();
		dataBarFormatting.Color = color;
		dataBarFormatting.PercentMin = 0;
		dataBarFormatting.PercentMax = 100;
		DataBarThreshold dataBarThreshold = new DataBarThreshold();
		dataBarThreshold.SetType(RangeType.MIN.id);
		dataBarFormatting.ThresholdMin = dataBarThreshold;
		DataBarThreshold dataBarThreshold2 = new DataBarThreshold();
		dataBarThreshold2.SetType(RangeType.MAX.id);
		dataBarFormatting.ThresholdMax = dataBarThreshold2;
		return cFRule12Record;
	}

	public static CFRule12Record Create(HSSFSheet sheet, IconSet iconSet)
	{
		Threshold[] array = new Threshold[iconSet.num];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = new IconMultiStateThreshold();
		}
		CFRule12Record cFRule12Record = new CFRule12Record(6, ComparisonOperator.NO_COMPARISON);
		IconMultiStateFormatting iconMultiStateFormatting = cFRule12Record.CreateMultiStateFormatting();
		iconMultiStateFormatting.IconSet = iconSet;
		iconMultiStateFormatting.Thresholds = array;
		return cFRule12Record;
	}

	public static CFRule12Record CreateColorScale(HSSFSheet sheet)
	{
		int num = 3;
		NPOI.HSSF.Record.Common.ExtendedColor[] array = new NPOI.HSSF.Record.Common.ExtendedColor[num];
		ColorGradientThreshold[] array2 = new ColorGradientThreshold[num];
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i] = new ColorGradientThreshold();
			array[i] = new NPOI.HSSF.Record.Common.ExtendedColor();
		}
		CFRule12Record cFRule12Record = new CFRule12Record(3, ComparisonOperator.NO_COMPARISON);
		ColorGradientFormatting colorGradientFormatting = cFRule12Record.CreateColorGradientFormatting();
		colorGradientFormatting.NumControlPoints = num;
		colorGradientFormatting.Thresholds = array2;
		colorGradientFormatting.Colors = array;
		return cFRule12Record;
	}

	public CFRule12Record(RecordInputStream in1)
	{
		futureHeader = new FtrHeader(in1);
		base.ConditionType = (byte)in1.ReadByte();
		base.ComparisonOperation = (byte)in1.ReadByte();
		int encodedTokenLen = in1.ReadUShort();
		int encodedTokenLen2 = in1.ReadUShort();
		ext_formatting_length = in1.ReadInt();
		ext_formatting_data = new byte[0];
		if (ext_formatting_length == 0)
		{
			in1.ReadUShort();
		}
		else
		{
			int num = ReadFormatOptions(in1);
			if (num < ext_formatting_length)
			{
				ext_formatting_data = new byte[ext_formatting_length - num];
				in1.ReadFully(ext_formatting_data);
			}
		}
		base.Formula1 = Formula.Read(encodedTokenLen, in1);
		base.Formula2 = Formula.Read(encodedTokenLen2, in1);
		int encodedTokenLen3 = in1.ReadUShort();
		formula_scale = Formula.Read(encodedTokenLen3, in1);
		ext_opts = (byte)in1.ReadByte();
		priority = in1.ReadUShort();
		template_type = in1.ReadUShort();
		template_param_length = (byte)in1.ReadByte();
		if (template_param_length == 0 || template_param_length == 16)
		{
			template_params = new byte[template_param_length];
			in1.ReadFully(template_params);
		}
		else
		{
			in1.ReadRemainder();
		}
		switch (base.ConditionType)
		{
		case 3:
			color_gradient = new ColorGradientFormatting(in1);
			break;
		case 4:
			data_bar = new DataBarFormatting(in1);
			break;
		case 5:
			filter_data = in1.ReadRemainder();
			break;
		case 6:
			multistate = new IconMultiStateFormatting(in1);
			break;
		}
	}

	public bool ContainsDataBarBlock()
	{
		return data_bar != null;
	}

	public DataBarFormatting CreateDataBarFormatting()
	{
		if (data_bar != null)
		{
			return data_bar;
		}
		base.ConditionType = 4;
		data_bar = new DataBarFormatting();
		return data_bar;
	}

	public bool ContainsMultiStateBlock()
	{
		return multistate != null;
	}

	public IconMultiStateFormatting CreateMultiStateFormatting()
	{
		if (multistate != null)
		{
			return multistate;
		}
		base.ConditionType = 6;
		multistate = new IconMultiStateFormatting();
		return multistate;
	}

	public bool ContainsColorGradientBlock()
	{
		return color_gradient != null;
	}

	public ColorGradientFormatting CreateColorGradientFormatting()
	{
		if (color_gradient != null)
		{
			return color_gradient;
		}
		base.ConditionType = 3;
		color_gradient = new ColorGradientFormatting();
		return color_gradient;
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		futureHeader.Serialize(out1);
		int formulaSize = CFRuleBase.GetFormulaSize(base.Formula1);
		int formulaSize2 = CFRuleBase.GetFormulaSize(base.Formula2);
		out1.WriteByte(base.ConditionType);
		out1.WriteByte(base.ComparisonOperation);
		out1.WriteShort(formulaSize);
		out1.WriteShort(formulaSize2);
		if (ext_formatting_length == 0)
		{
			out1.WriteInt(0);
			out1.WriteShort(0);
		}
		else
		{
			out1.WriteInt(ext_formatting_length);
			SerializeFormattingBlock(out1);
			out1.Write(ext_formatting_data);
		}
		base.Formula1.SerializeTokens(out1);
		base.Formula2.SerializeTokens(out1);
		out1.WriteShort(CFRuleBase.GetFormulaSize(formula_scale));
		formula_scale.SerializeTokens(out1);
		out1.WriteByte(ext_opts);
		out1.WriteShort(priority);
		out1.WriteShort(template_type);
		out1.WriteByte(template_param_length);
		out1.Write(template_params);
		switch (base.ConditionType)
		{
		case 3:
			color_gradient.Serialize(out1);
			break;
		case 4:
			data_bar.Serialize(out1);
			break;
		case 5:
			out1.Write(filter_data);
			break;
		case 6:
			multistate.Serialize(out1);
			break;
		}
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[CFRULE12]\n");
		stringBuilder.Append("    .condition_type=").Append(base.ConditionType).Append("\n");
		stringBuilder.Append("    .dxfn12_length =0x").Append(HexDump.ToHex(ext_formatting_length)).Append("\n");
		stringBuilder.Append("    .option_flags  =0x").Append(HexDump.ToHex(base.Options)).Append("\n");
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
		stringBuilder.Append("    .dxfn12_ext=").Append(HexDump.ToHex(ext_formatting_data)).Append("\n");
		StringBuilder stringBuilder2 = stringBuilder.Append("    .Formula_1 =");
		object[] tokens = base.Formula1.Tokens;
		stringBuilder2.Append(Arrays.ToString(tokens)).Append("\n");
		StringBuilder stringBuilder3 = stringBuilder.Append("    .Formula_2 =");
		tokens = base.Formula2.Tokens;
		stringBuilder3.Append(Arrays.ToString(tokens)).Append("\n");
		StringBuilder stringBuilder4 = stringBuilder.Append("    .Formula_S =");
		tokens = formula_scale.Tokens;
		stringBuilder4.Append(Arrays.ToString(tokens)).Append("\n");
		stringBuilder.Append("    .ext_opts  =").Append(ext_opts).Append("\n");
		stringBuilder.Append("    .priority  =").Append(priority).Append("\n");
		stringBuilder.Append("    .template_type  =").Append(template_type).Append("\n");
		stringBuilder.Append("    .template_params=").Append(HexDump.ToHex(template_params)).Append("\n");
		stringBuilder.Append("    .filter_data    =").Append(HexDump.ToHex(filter_data)).Append("\n");
		if (color_gradient != null)
		{
			stringBuilder.Append(color_gradient);
		}
		if (multistate != null)
		{
			stringBuilder.Append(multistate);
		}
		if (data_bar != null)
		{
			stringBuilder.Append(data_bar);
		}
		stringBuilder.Append("[/CFRULE12]\n");
		return stringBuilder.ToString();
	}

	public override object Clone()
	{
		CFRule12Record cFRule12Record = new CFRule12Record(base.ConditionType, base.ComparisonOperation);
		cFRule12Record.futureHeader.AssociatedRange = futureHeader.AssociatedRange.Copy();
		CopyTo(cFRule12Record);
		cFRule12Record.ext_formatting_length = ext_formatting_length;
		cFRule12Record.ext_formatting_data = new byte[ext_formatting_length];
		Array.Copy(ext_formatting_data, 0, cFRule12Record.ext_formatting_data, 0, ext_formatting_length);
		cFRule12Record.formula_scale = formula_scale.Copy();
		cFRule12Record.ext_opts = ext_opts;
		cFRule12Record.priority = priority;
		cFRule12Record.template_type = template_type;
		cFRule12Record.template_param_length = template_param_length;
		cFRule12Record.template_params = new byte[template_param_length];
		Array.Copy(template_params, 0, cFRule12Record.template_params, 0, template_param_length);
		if (color_gradient != null)
		{
			cFRule12Record.color_gradient = (ColorGradientFormatting)color_gradient.Clone();
		}
		if (multistate != null)
		{
			cFRule12Record.multistate = (IconMultiStateFormatting)multistate.Clone();
		}
		if (data_bar != null)
		{
			cFRule12Record.data_bar = (DataBarFormatting)data_bar.Clone();
		}
		if (filter_data != null)
		{
			cFRule12Record.filter_data = new byte[filter_data.Length];
			Array.Copy(filter_data, 0, cFRule12Record.filter_data, 0, filter_data.Length);
		}
		return cFRule12Record;
	}

	public short GetFutureRecordType()
	{
		return futureHeader.RecordType;
	}

	public FtrHeader GetFutureHeader()
	{
		return futureHeader;
	}

	public CellRangeAddress GetAssociatedRange()
	{
		return futureHeader.AssociatedRange;
	}
}
