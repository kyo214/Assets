using System;
using System.Collections.Generic;
using System.Text;
using NPOI.HSSF.Record.Cont;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class UnicodeString : IComparable<UnicodeString>
{
	public class FormatRun : IComparable<FormatRun>
	{
		internal short _character;

		internal short _fontIndex;

		public short CharacterPos => _character;

		public short FontIndex => _fontIndex;

		public FormatRun(short character, short fontIndex)
		{
			_character = character;
			_fontIndex = fontIndex;
		}

		public FormatRun(ILittleEndianInput in1)
			: this(in1.ReadShort(), in1.ReadShort())
		{
		}

		public override bool Equals(object o)
		{
			if (!(o is FormatRun))
			{
				return false;
			}
			FormatRun formatRun = (FormatRun)o;
			if (_character == formatRun._character)
			{
				return _fontIndex == formatRun._fontIndex;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public int CompareTo(FormatRun r)
		{
			if (_character == r._character && _fontIndex == r._fontIndex)
			{
				return 0;
			}
			if (_character == r._character)
			{
				return _fontIndex - r._fontIndex;
			}
			return _character - r._character;
		}

		public override string ToString()
		{
			return "character=" + _character + ",fontIndex=" + _fontIndex;
		}

		public void Serialize(ILittleEndianOutput out1)
		{
			out1.WriteShort(_character);
			out1.WriteShort(_fontIndex);
		}
	}

	public class ExtRst : IComparable<ExtRst>
	{
		private short reserved;

		private short formattingFontIndex;

		private short formattingOptions;

		private int numberOfRuns;

		private string phoneticText;

		private PhRun[] phRuns;

		private byte[] extraData;

		internal int DataSize => 10 + 2 * phoneticText.Length + 6 * phRuns.Length + extraData.Length;

		public short FormattingFontIndex => formattingFontIndex;

		public short FormattingOptions => formattingOptions;

		public int NumberOfRuns => numberOfRuns;

		public string PhoneticText => phoneticText;

		public PhRun[] PhRuns => phRuns;

		private void populateEmpty()
		{
			reserved = 1;
			phoneticText = "";
			phRuns = new PhRun[0];
			extraData = new byte[0];
		}

		public override int GetHashCode()
		{
			int num = reserved;
			num = 31 * num + formattingFontIndex;
			num = 31 * num + formattingOptions;
			num = 31 * num + numberOfRuns;
			num = 31 * num + phoneticText.GetHashCode();
			if (phRuns != null)
			{
				PhRun[] array = phRuns;
				foreach (PhRun phRun in array)
				{
					num = 31 * num + phRun.phoneticTextFirstCharacterOffset;
					num = 31 * num + phRun.realTextFirstCharacterOffset;
					num = 31 * num + phRun.realTextLength;
				}
			}
			return num;
		}

		internal ExtRst()
		{
			populateEmpty();
		}

		internal ExtRst(ILittleEndianInput in1, int expectedLength)
		{
			reserved = in1.ReadShort();
			if (reserved == -1)
			{
				populateEmpty();
				return;
			}
			if (reserved != 1)
			{
				_logger.Log(5, "Warning - ExtRst has wrong magic marker, expecting 1 but found " + reserved + " - ignoring");
				for (int i = 0; i < expectedLength - 2; i++)
				{
					in1.ReadByte();
				}
				populateEmpty();
				return;
			}
			short num = in1.ReadShort();
			formattingFontIndex = in1.ReadShort();
			formattingOptions = in1.ReadShort();
			numberOfRuns = in1.ReadUShort();
			short num2 = in1.ReadShort();
			short num3 = in1.ReadShort();
			if (num2 == 0 && num3 > 0)
			{
				num3 = 0;
			}
			if (num2 != num3)
			{
				throw new InvalidOperationException("The two length fields of the Phonetic Text don't agree! " + num2 + " vs " + num3);
			}
			phoneticText = StringUtil.ReadUnicodeLE(in1, num2);
			int num4 = num - 4 - 6 - 2 * phoneticText.Length;
			int num5 = num4 / 6;
			phRuns = new PhRun[num5];
			for (int j = 0; j < phRuns.Length; j++)
			{
				phRuns[j] = new PhRun(in1);
			}
			int num6 = num4 - num5 * 6;
			if (num6 < 0)
			{
				num6 = 0;
			}
			extraData = new byte[num6];
			for (int k = 0; k < extraData.Length; k++)
			{
				extraData[k] = (byte)in1.ReadByte();
			}
		}

		internal void Serialize(ContinuableRecordOutput out1)
		{
			int dataSize = DataSize;
			out1.WriteContinueIfRequired(8);
			out1.WriteShort(reserved);
			out1.WriteShort(dataSize);
			out1.WriteShort(formattingFontIndex);
			out1.WriteShort(formattingOptions);
			out1.WriteContinueIfRequired(6);
			out1.WriteShort(numberOfRuns);
			out1.WriteShort(phoneticText.Length);
			out1.WriteShort(phoneticText.Length);
			out1.WriteContinueIfRequired(phoneticText.Length * 2);
			StringUtil.PutUnicodeLE(phoneticText, out1);
			for (int i = 0; i < phRuns.Length; i++)
			{
				phRuns[i].Serialize(out1);
			}
			out1.Write(extraData);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is ExtRst))
			{
				return false;
			}
			ExtRst o = (ExtRst)obj;
			return CompareTo(o) == 0;
		}

		public override string ToString()
		{
			return base.ToString();
		}

		public int CompareTo(ExtRst o)
		{
			int num = reserved - o.reserved;
			if (num != 0)
			{
				return num;
			}
			num = formattingFontIndex - o.formattingFontIndex;
			if (num != 0)
			{
				return num;
			}
			num = formattingOptions - o.formattingOptions;
			if (num != 0)
			{
				return num;
			}
			num = numberOfRuns - o.numberOfRuns;
			if (num != 0)
			{
				return num;
			}
			num = string.Compare(phoneticText, o.phoneticText, StringComparison.CurrentCulture);
			if (num != 0)
			{
				return num;
			}
			num = phRuns.Length - o.phRuns.Length;
			if (num != 0)
			{
				return num;
			}
			for (int i = 0; i < phRuns.Length; i++)
			{
				num = phRuns[i].phoneticTextFirstCharacterOffset - o.phRuns[i].phoneticTextFirstCharacterOffset;
				if (num != 0)
				{
					return num;
				}
				num = phRuns[i].realTextFirstCharacterOffset - o.phRuns[i].realTextFirstCharacterOffset;
				if (num != 0)
				{
					return num;
				}
				num = phRuns[i].realTextLength - o.phRuns[i].realTextLength;
				if (num != 0)
				{
					return num;
				}
			}
			return Arrays.HashCode(extraData) - Arrays.HashCode(o.extraData);
		}

		internal ExtRst Clone()
		{
			ExtRst extRst = new ExtRst();
			extRst.reserved = reserved;
			extRst.formattingFontIndex = formattingFontIndex;
			extRst.formattingOptions = formattingOptions;
			extRst.numberOfRuns = numberOfRuns;
			extRst.phoneticText = phoneticText;
			extRst.phRuns = new PhRun[phRuns.Length];
			for (int i = 0; i < extRst.phRuns.Length; i++)
			{
				extRst.phRuns[i] = new PhRun(phRuns[i].phoneticTextFirstCharacterOffset, phRuns[i].realTextFirstCharacterOffset, phRuns[i].realTextLength);
			}
			return extRst;
		}
	}

	public class PhRun
	{
		internal int phoneticTextFirstCharacterOffset;

		internal int realTextFirstCharacterOffset;

		internal int realTextLength;

		public PhRun(int phoneticTextFirstCharacterOffset, int realTextFirstCharacterOffset, int realTextLength)
		{
			this.phoneticTextFirstCharacterOffset = phoneticTextFirstCharacterOffset;
			this.realTextFirstCharacterOffset = realTextFirstCharacterOffset;
			this.realTextLength = realTextLength;
		}

		internal PhRun(ILittleEndianInput in1)
		{
			phoneticTextFirstCharacterOffset = in1.ReadUShort();
			realTextFirstCharacterOffset = in1.ReadUShort();
			realTextLength = in1.ReadUShort();
		}

		internal void Serialize(ContinuableRecordOutput out1)
		{
			out1.WriteContinueIfRequired(6);
			out1.WriteShort(phoneticTextFirstCharacterOffset);
			out1.WriteShort(realTextFirstCharacterOffset);
			out1.WriteShort(realTextLength);
		}
	}

	private static POILogger _logger = POILogFactory.GetLogger(typeof(UnicodeString));

	private short field_1_charCount;

	private byte field_2_optionflags;

	private string field_3_string;

	private List<FormatRun> field_4_format_runs;

	private ExtRst field_5_ext_rst;

	private static BitField highByte = BitFieldFactory.GetInstance(1);

	private static BitField extBit = BitFieldFactory.GetInstance(4);

	private static BitField richText = BitFieldFactory.GetInstance(8);

	public int CharCount
	{
		get
		{
			if (field_1_charCount < 0)
			{
				return field_1_charCount + 65536;
			}
			return field_1_charCount;
		}
		set
		{
			field_1_charCount = (short)value;
		}
	}

	public short CharCountShort => field_1_charCount;

	public byte OptionFlags
	{
		get
		{
			return field_2_optionflags;
		}
		set
		{
			field_2_optionflags = value;
		}
	}

	public string String
	{
		get
		{
			return field_3_string;
		}
		set
		{
			field_3_string = value;
			CharCount = (short)field_3_string.Length;
			bool flag = false;
			int length = value.Length;
			for (int i = 0; i < length; i++)
			{
				if (value[i] > 'ÿ')
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				field_2_optionflags = highByte.SetByte(field_2_optionflags);
			}
			else
			{
				field_2_optionflags = highByte.ClearByte(field_2_optionflags);
			}
		}
	}

	public int FormatRunCount
	{
		get
		{
			if (field_4_format_runs != null)
			{
				return field_4_format_runs.Count;
			}
			return 0;
		}
	}

	public ExtRst ExtendedRst
	{
		get
		{
			return field_5_ext_rst;
		}
		set
		{
			if (value != null)
			{
				field_2_optionflags = extBit.SetByte(field_2_optionflags);
			}
			else
			{
				field_2_optionflags = extBit.ClearByte(field_2_optionflags);
			}
			field_5_ext_rst = value;
		}
	}

	private bool IsRichText => richText.IsSet(OptionFlags);

	private bool IsExtendedText => extBit.IsSet(OptionFlags);

	private UnicodeString()
	{
	}

	public UnicodeString(string str)
	{
		String = str;
	}

	public override int GetHashCode()
	{
		int num = 0;
		if (field_3_string != null)
		{
			num = field_3_string.GetHashCode();
		}
		return field_1_charCount + num;
	}

	public override bool Equals(object o)
	{
		if (!(o is UnicodeString))
		{
			return false;
		}
		UnicodeString unicodeString = (UnicodeString)o;
		if (field_1_charCount != unicodeString.field_1_charCount || field_2_optionflags != unicodeString.field_2_optionflags || !field_3_string.Equals(unicodeString.field_3_string))
		{
			return false;
		}
		if (field_4_format_runs == null)
		{
			return unicodeString.field_4_format_runs == null;
		}
		if (unicodeString.field_4_format_runs == null)
		{
			return false;
		}
		int count = field_4_format_runs.Count;
		if (count != unicodeString.field_4_format_runs.Count)
		{
			return false;
		}
		for (int i = 0; i < count; i++)
		{
			FormatRun formatRun = field_4_format_runs[i];
			FormatRun obj = unicodeString.field_4_format_runs[i];
			if (!formatRun.Equals(obj))
			{
				return false;
			}
		}
		if (field_5_ext_rst == null)
		{
			return unicodeString.field_5_ext_rst == null;
		}
		if (unicodeString.field_5_ext_rst == null)
		{
			return false;
		}
		return field_5_ext_rst.Equals(unicodeString.field_5_ext_rst);
	}

	public UnicodeString(RecordInputStream in1)
	{
		field_1_charCount = in1.ReadShort();
		field_2_optionflags = (byte)in1.ReadByte();
		int num = 0;
		int num2 = 0;
		if (IsRichText)
		{
			num = in1.ReadShort();
		}
		if (IsExtendedText)
		{
			num2 = in1.ReadInt();
		}
		bool flag = (field_2_optionflags & 1) == 0;
		int charCount = CharCount;
		field_3_string = (flag ? in1.ReadCompressedUnicode(charCount) : in1.ReadUnicodeLEString(charCount));
		if (IsRichText && num > 0)
		{
			field_4_format_runs = new List<FormatRun>(num);
			for (int i = 0; i < num; i++)
			{
				field_4_format_runs.Add(new FormatRun(in1));
			}
		}
		if (IsExtendedText && num2 > 0)
		{
			field_5_ext_rst = new ExtRst(new ContinuableRecordInput(in1), num2);
			if (field_5_ext_rst.DataSize + 4 != num2)
			{
				_logger.Log(5, "ExtRst was supposed to be " + num2 + " bytes long, but seems to actually be " + (field_5_ext_rst.DataSize + 4));
			}
		}
	}

	public FormatRun GetFormatRun(int index)
	{
		if (field_4_format_runs == null)
		{
			return null;
		}
		if (index < 0 || index >= field_4_format_runs.Count)
		{
			return null;
		}
		return field_4_format_runs[index];
	}

	private int FindFormatRunAt(int characterPos)
	{
		int count = field_4_format_runs.Count;
		for (int i = 0; i < count; i++)
		{
			FormatRun formatRun = field_4_format_runs[i];
			if (formatRun._character == characterPos)
			{
				return i;
			}
			if (formatRun._character > characterPos)
			{
				return -1;
			}
		}
		return -1;
	}

	public void AddFormatRun(FormatRun r)
	{
		if (field_4_format_runs == null)
		{
			field_4_format_runs = new List<FormatRun>();
		}
		int num = FindFormatRunAt(r._character);
		if (num != -1)
		{
			field_4_format_runs.RemoveAt(num);
		}
		field_4_format_runs.Add(r);
		field_4_format_runs.Sort();
		field_2_optionflags = richText.SetByte(field_2_optionflags);
	}

	public List<FormatRun> FormatIterator()
	{
		if (field_4_format_runs != null)
		{
			return field_4_format_runs;
		}
		return null;
	}

	public void RemoveFormatRun(FormatRun r)
	{
		field_4_format_runs.Remove(r);
		if (field_4_format_runs.Count == 0)
		{
			field_4_format_runs = null;
			field_2_optionflags = richText.ClearByte(field_2_optionflags);
		}
	}

	public void ClearFormatting()
	{
		field_4_format_runs = null;
		field_2_optionflags = richText.ClearByte(field_2_optionflags);
	}

	public void SwapFontUse(short oldFontIndex, short newFontIndex)
	{
		foreach (FormatRun field_4_format_run in field_4_format_runs)
		{
			if (field_4_format_run._fontIndex == oldFontIndex)
			{
				field_4_format_run._fontIndex = newFontIndex;
			}
		}
	}

	public override string ToString()
	{
		return String;
	}

	public string GetDebugInfo()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[UNICODESTRING]\n");
		stringBuilder.Append("    .charcount       = ").Append(StringUtil.ToHexString(CharCount)).Append("\n");
		stringBuilder.Append("    .optionflags     = ").Append(StringUtil.ToHexString(OptionFlags)).Append("\n");
		stringBuilder.Append("    .string          = ").Append(String).Append("\n");
		if (field_4_format_runs != null)
		{
			for (int i = 0; i < field_4_format_runs.Count; i++)
			{
				FormatRun formatRun = field_4_format_runs[i];
				stringBuilder.Append("      .format_Run" + i + "          = ").Append(formatRun.ToString()).Append("\n");
			}
		}
		if (field_5_ext_rst != null)
		{
			stringBuilder.Append("    .field_5_ext_rst          = ").Append("\n");
			stringBuilder.Append(field_5_ext_rst.ToString()).Append("\n");
		}
		stringBuilder.Append("[/UNICODESTRING]\n");
		return stringBuilder.ToString();
	}

	public void Serialize(ContinuableRecordOutput out1)
	{
		int num = 0;
		int num2 = 0;
		if (IsRichText && field_4_format_runs != null)
		{
			num = field_4_format_runs.Count;
		}
		if (IsExtendedText && field_5_ext_rst != null)
		{
			num2 = 4 + field_5_ext_rst.DataSize;
		}
		out1.WriteString(field_3_string, num, num2);
		if (num > 0)
		{
			for (int i = 0; i < num; i++)
			{
				if (out1.AvailableSpace < 4)
				{
					out1.WriteContinue();
				}
				field_4_format_runs[i].Serialize(out1);
			}
		}
		if (num2 > 0)
		{
			field_5_ext_rst.Serialize(out1);
		}
	}

	public int CompareTo(UnicodeString str)
	{
		int num = string.Compare(String, str.String, StringComparison.CurrentCulture);
		if (num != 0)
		{
			return num;
		}
		if (field_4_format_runs == null)
		{
			if (str.field_4_format_runs != null)
			{
				return 1;
			}
			return 0;
		}
		if (str.field_4_format_runs == null)
		{
			return -1;
		}
		int count = field_4_format_runs.Count;
		if (count != str.field_4_format_runs.Count)
		{
			return count - str.field_4_format_runs.Count;
		}
		for (int i = 0; i < count; i++)
		{
			FormatRun formatRun = field_4_format_runs[i];
			FormatRun r = str.field_4_format_runs[i];
			num = formatRun.CompareTo(r);
			if (num != 0)
			{
				return num;
			}
		}
		if (field_5_ext_rst == null)
		{
			if (str.field_5_ext_rst != null)
			{
				return 1;
			}
			return 0;
		}
		if (str.field_5_ext_rst == null)
		{
			return -1;
		}
		return field_5_ext_rst.CompareTo(str.field_5_ext_rst);
	}

	public object Clone()
	{
		UnicodeString unicodeString = new UnicodeString();
		unicodeString.field_1_charCount = field_1_charCount;
		unicodeString.field_2_optionflags = field_2_optionflags;
		unicodeString.field_3_string = field_3_string;
		if (field_4_format_runs != null)
		{
			unicodeString.field_4_format_runs = new List<FormatRun>();
			foreach (FormatRun field_4_format_run in field_4_format_runs)
			{
				unicodeString.field_4_format_runs.Add(new FormatRun(field_4_format_run._character, field_4_format_run._fontIndex));
			}
		}
		if (field_5_ext_rst != null)
		{
			unicodeString.field_5_ext_rst = field_5_ext_rst.Clone();
		}
		return unicodeString;
	}
}
