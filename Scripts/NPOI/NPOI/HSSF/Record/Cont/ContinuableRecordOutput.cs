using System;
using NPOI.Util;

namespace NPOI.HSSF.Record.Cont;

public class ContinuableRecordOutput : ILittleEndianOutput
{
	private class DelayableLittleEndianOutput1 : IDelayableLittleEndianOutput, ILittleEndianOutput
	{
		public ILittleEndianOutput CreateDelayedOutput(int size)
		{
			return this;
		}

		public void Write(byte[] b)
		{
		}

		public void Write(byte[] b, int offset, int len)
		{
		}

		public void WriteByte(int v)
		{
		}

		public void WriteDouble(double v)
		{
		}

		public void WriteInt(int v)
		{
		}

		public void WriteLong(long v)
		{
		}

		public void WriteShort(int v)
		{
		}
	}

	private ILittleEndianOutput _out;

	private UnknownLengthRecordOutput _ulrOutput;

	private int _totalPreviousRecordsSize;

	private static ILittleEndianOutput NOPOutput = new DelayableLittleEndianOutput1();

	public int TotalSize => _totalPreviousRecordsSize + _ulrOutput.TotalSize;

	public int AvailableSpace => _ulrOutput.AvailableSpace;

	internal ContinuableRecordOutput(ILittleEndianOutput out1, int sid)
	{
		_ulrOutput = new UnknownLengthRecordOutput(out1, sid);
		_out = out1;
		_totalPreviousRecordsSize = 0;
	}

	public static ContinuableRecordOutput CreateForCountingOnly()
	{
		return new ContinuableRecordOutput(NOPOutput, -777);
	}

	public void Terminate()
	{
		_ulrOutput.Terminate();
	}

	public void WriteContinue()
	{
		_ulrOutput.Terminate();
		_totalPreviousRecordsSize += _ulrOutput.TotalSize;
		_ulrOutput = new UnknownLengthRecordOutput(_out, 60);
	}

	public void WriteContinueIfRequired(int requiredContinuousSize)
	{
		if (_ulrOutput.AvailableSpace < requiredContinuousSize)
		{
			WriteContinue();
		}
	}

	public void WriteStringData(string text)
	{
		bool flag = StringUtil.HasMultibyte(text);
		int num = 2;
		int num2 = 0;
		if (flag)
		{
			num2 |= 1;
			num++;
		}
		WriteContinueIfRequired(num);
		WriteByte(num2);
		WriteCharacterData(text, flag);
	}

	public void WriteString(string text, int numberOfRichTextRuns, int extendedDataSize)
	{
		bool flag = StringUtil.HasMultibyte(text);
		int num = 4;
		int num2 = 0;
		if (flag)
		{
			num2 |= 1;
			num++;
		}
		if (numberOfRichTextRuns > 0)
		{
			num2 |= 8;
			num += 2;
		}
		if (extendedDataSize > 0)
		{
			num2 |= 4;
			num += 4;
		}
		WriteContinueIfRequired(num);
		WriteShort(text.Length);
		WriteByte(num2);
		if (numberOfRichTextRuns > 0)
		{
			WriteShort(numberOfRichTextRuns);
		}
		if (extendedDataSize > 0)
		{
			WriteInt(extendedDataSize);
		}
		WriteCharacterData(text, flag);
	}

	private void WriteCharacterData(string text, bool is16bitEncoded)
	{
		int length = text.Length;
		int num = 0;
		if (is16bitEncoded)
		{
			while (true)
			{
				for (int num2 = Math.Min(length - num, _ulrOutput.AvailableSpace / 2); num2 > 0; num2--)
				{
					_ulrOutput.WriteShort(text[num++]);
				}
				if (num < length)
				{
					WriteContinue();
					WriteByte(1);
					continue;
				}
				break;
			}
			return;
		}
		while (true)
		{
			for (int num3 = Math.Min(length - num, _ulrOutput.AvailableSpace / 1); num3 > 0; num3--)
			{
				_ulrOutput.WriteByte(text[num++]);
			}
			if (num < length)
			{
				WriteContinue();
				WriteByte(0);
				continue;
			}
			break;
		}
	}

	public void Write(byte[] b)
	{
		WriteContinueIfRequired(b.Length);
		_ulrOutput.Write(b);
	}

	public void Write(byte[] b, int offset, int len)
	{
		int num = 0;
		while (true)
		{
			for (int num2 = Math.Min(len - num, _ulrOutput.AvailableSpace / 1); num2 > 0; num2--)
			{
				_ulrOutput.WriteByte(b[offset + num++]);
			}
			if (num < len)
			{
				WriteContinue();
				continue;
			}
			break;
		}
	}

	public void WriteByte(int v)
	{
		WriteContinueIfRequired(1);
		_ulrOutput.WriteByte(v);
	}

	public void WriteDouble(double v)
	{
		WriteContinueIfRequired(8);
		_ulrOutput.WriteDouble(v);
	}

	public void WriteInt(int v)
	{
		WriteContinueIfRequired(4);
		_ulrOutput.WriteInt(v);
	}

	public void WriteLong(long v)
	{
		WriteContinueIfRequired(8);
		_ulrOutput.WriteLong(v);
	}

	public void WriteShort(int v)
	{
		WriteContinueIfRequired(2);
		_ulrOutput.WriteShort(v);
	}
}
