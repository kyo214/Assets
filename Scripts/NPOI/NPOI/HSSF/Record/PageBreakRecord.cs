using System.Collections;
using System.Collections.Generic;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class PageBreakRecord : StandardRecord
{
	public class Break
	{
		public const int ENCODED_SIZE = 6;

		public int main;

		public int subFrom;

		public int subTo;

		public Break(RecordInputStream in1)
		{
			main = in1.ReadUShort() - 1;
			subFrom = in1.ReadUShort();
			subTo = in1.ReadUShort();
		}

		public Break(int main, int subFrom, int subTo)
		{
			this.main = main;
			this.subFrom = subFrom;
			this.subTo = subTo;
		}

		public void Serialize(ILittleEndianOutput out1)
		{
			out1.WriteShort(main + 1);
			out1.WriteShort(subFrom);
			out1.WriteShort(subTo);
		}
	}

	private const bool IS_EMPTY_RECORD_WRITTEN = false;

	private static readonly int[] EMPTY_INT_ARRAY = new int[0];

	public short sid;

	private IList<Break> _breaks;

	private Hashtable _breakMap;

	public override short Sid => sid;

	protected override int DataSize => 2 + _breaks.Count * 6;

	public override int RecordSize
	{
		get
		{
			if (_breaks.Count < 1)
			{
				return 0;
			}
			return 4 + DataSize;
		}
	}

	public int NumBreaks => _breaks.Count;

	public bool IsEmpty => _breaks.Count == 0;

	public PageBreakRecord()
	{
		_breaks = new List<Break>();
		_breakMap = new Hashtable();
	}

	public PageBreakRecord(RecordInputStream in1)
	{
		int num = in1.ReadShort();
		_breaks = new List<Break>(num + 2);
		_breakMap = new Hashtable();
		for (int i = 0; i < num; i++)
		{
			Break obj = new Break(in1);
			_breaks.Add(obj);
			_breakMap[obj.main] = obj;
		}
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		int count = _breaks.Count;
		out1.WriteShort(count);
		for (int i = 0; i < count; i++)
		{
			_breaks[i].Serialize(out1);
		}
	}

	public IEnumerator<Break> GetBreaksEnumerator()
	{
		return _breaks.GetEnumerator();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		string text;
		string value;
		string value2;
		if (Sid == 27)
		{
			text = "HORIZONTALPAGEBREAK";
			value = "row";
			value2 = "col";
		}
		else
		{
			text = "VERTICALPAGEBREAK";
			value = "column";
			value2 = "row";
		}
		stringBuilder.Append("[" + text + "]").Append("\n");
		stringBuilder.Append("     .Sid        =").Append(Sid).Append("\n");
		stringBuilder.Append("     .num_breaks =").Append(NumBreaks).Append("\n");
		IEnumerator breaksEnumerator = GetBreaksEnumerator();
		for (int i = 0; i < NumBreaks; i++)
		{
			Break obj = (Break)breaksEnumerator.Current;
			stringBuilder.Append("     .").Append(value).Append(" (zero-based) =")
				.Append(obj.main)
				.Append("\n");
			stringBuilder.Append("     .").Append(value2).Append("From    =")
				.Append(obj.subFrom)
				.Append("\n");
			stringBuilder.Append("     .").Append(value2).Append("To      =")
				.Append(obj.subTo)
				.Append("\n");
		}
		stringBuilder.Append("[" + text + "]").Append("\n");
		return stringBuilder.ToString();
	}

	public void AddBreak(int main, int subFrom, int subTo)
	{
		Break obj = (Break)_breakMap[main];
		if (obj != null)
		{
			obj.main = main;
			obj.subFrom = subFrom;
			obj.subTo = subTo;
		}
		else
		{
			obj = new Break(main, subFrom, subTo);
			_breaks.Add(obj);
		}
		_breakMap[main] = obj;
	}

	public void RemoveBreak(int main)
	{
		Break item = (Break)_breakMap[main];
		_breaks.Remove(item);
		_breakMap.Remove(main);
	}

	public Break GetBreak(int main)
	{
		return (Break)_breakMap[main];
	}

	public int[] GetBreaks()
	{
		int numBreaks = NumBreaks;
		if (numBreaks < 1)
		{
			return EMPTY_INT_ARRAY;
		}
		int[] array = new int[numBreaks];
		for (int i = 0; i < numBreaks; i++)
		{
			Break obj = _breaks[i];
			array[i] = obj.main;
		}
		return array;
	}
}
