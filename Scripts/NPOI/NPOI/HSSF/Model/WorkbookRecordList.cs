using System.Collections.Generic;
using NPOI.HSSF.Record;

namespace NPOI.HSSF.Model;

public class WorkbookRecordList
{
	private List<NPOI.HSSF.Record.Record> records = new List<NPOI.HSSF.Record.Record>();

	private int protpos;

	private int bspos;

	private int tabpos;

	private int fontpos;

	private int xfpos;

	private int backuppos;

	private int namepos;

	private int supbookpos;

	private int externsheetPos;

	private int palettepos = -1;

	public List<NPOI.HSSF.Record.Record> Records
	{
		get
		{
			return records;
		}
		set
		{
			records = value;
		}
	}

	public int Count => records.Count;

	public NPOI.HSSF.Record.Record this[int index] => records[index];

	public int Protpos
	{
		get
		{
			return protpos;
		}
		set
		{
			protpos = value;
		}
	}

	public int Bspos
	{
		get
		{
			return bspos;
		}
		set
		{
			bspos = value;
		}
	}

	public int Tabpos
	{
		get
		{
			return tabpos;
		}
		set
		{
			tabpos = value;
		}
	}

	public int Fontpos
	{
		get
		{
			return fontpos;
		}
		set
		{
			fontpos = value;
		}
	}

	public int Xfpos
	{
		get
		{
			return xfpos;
		}
		set
		{
			xfpos = value;
		}
	}

	public int Backuppos
	{
		get
		{
			return backuppos;
		}
		set
		{
			backuppos = value;
		}
	}

	public int Palettepos
	{
		get
		{
			return palettepos;
		}
		set
		{
			palettepos = value;
		}
	}

	public int Namepos
	{
		get
		{
			return namepos;
		}
		set
		{
			namepos = value;
		}
	}

	public int Supbookpos
	{
		get
		{
			return supbookpos;
		}
		set
		{
			supbookpos = value;
		}
	}

	public int ExternsheetPos
	{
		get
		{
			return externsheetPos;
		}
		set
		{
			externsheetPos = value;
		}
	}

	public void Add(int pos, NPOI.HSSF.Record.Record r)
	{
		records.Insert(pos, r);
		if (Protpos >= pos)
		{
			Protpos = protpos + 1;
		}
		if (Bspos >= pos)
		{
			Bspos = bspos + 1;
		}
		if (Tabpos >= pos)
		{
			Tabpos = tabpos + 1;
		}
		if (Fontpos >= pos)
		{
			Fontpos = fontpos + 1;
		}
		if (Xfpos >= pos)
		{
			Xfpos = xfpos + 1;
		}
		if (Backuppos >= pos)
		{
			Backuppos = backuppos + 1;
		}
		if (Namepos >= pos)
		{
			Namepos = namepos + 1;
		}
		if (Supbookpos >= pos)
		{
			Supbookpos = supbookpos + 1;
		}
		if (Palettepos != -1 && Palettepos >= pos)
		{
			Palettepos = palettepos + 1;
		}
		if (ExternsheetPos >= pos)
		{
			ExternsheetPos++;
		}
	}

	public IEnumerator<NPOI.HSSF.Record.Record> GetEnumerator()
	{
		return records.GetEnumerator();
	}

	public void Remove(NPOI.HSSF.Record.Record record)
	{
		int pos = records.IndexOf(record);
		Remove(pos);
	}

	public void Remove(int pos)
	{
		records.RemoveAt(pos);
		if (Protpos >= pos)
		{
			Protpos = protpos - 1;
		}
		if (Bspos >= pos)
		{
			Bspos = bspos - 1;
		}
		if (Tabpos >= pos)
		{
			Tabpos = tabpos - 1;
		}
		if (Fontpos >= pos)
		{
			Fontpos = fontpos - 1;
		}
		if (Xfpos >= pos)
		{
			Xfpos = xfpos - 1;
		}
		if (Backuppos >= pos)
		{
			Backuppos = backuppos - 1;
		}
		if (Namepos >= pos)
		{
			Namepos--;
		}
		if (Supbookpos >= pos)
		{
			Supbookpos--;
		}
		if (Palettepos != -1 && Palettepos >= pos)
		{
			Palettepos = palettepos - 1;
		}
		if (ExternsheetPos >= pos)
		{
			ExternsheetPos--;
		}
	}
}
