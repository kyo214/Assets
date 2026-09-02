using System;
using System.Globalization;
using System.Text;
using NPOI.SS.Formula.PTG;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class LbsDataSubRecord : SubRecord, ICloneable
{
	public const int sid = 19;

	private int _cbFContinued;

	private int _unknownPreFormulaInt;

	private Ptg _linkPtg;

	private byte? _unknownPostFormulaByte;

	private int _cLines;

	private int _iSel;

	private int _flags;

	private int _idEdit;

	private LbsDropData _dropData;

	private string[] _rgLines;

	private bool[] _bsels;

	public override bool IsTerminating => true;

	public Ptg Formula => _linkPtg;

	public int NumberOfItems => _cLines;

	public override short Sid => 19;

	public override int DataSize
	{
		get
		{
			int num = 2;
			if (_linkPtg != null)
			{
				num += 2;
				num += 4;
				num += _linkPtg.Size;
				if (_unknownPostFormulaByte.HasValue)
				{
					num++;
				}
			}
			num += 8;
			if (_dropData != null)
			{
				num += _dropData.DataSize;
			}
			if (_rgLines != null)
			{
				string[] rgLines = _rgLines;
				foreach (string value in rgLines)
				{
					num += StringUtil.GetEncodedSize(value);
				}
			}
			if (_bsels != null)
			{
				num += _bsels.Length;
			}
			return num;
		}
	}

	private LbsDataSubRecord()
	{
	}

	public LbsDataSubRecord(ILittleEndianInput in1, int cbFContinued, int cmoOt)
	{
		_cbFContinued = cbFContinued;
		int num = in1.ReadUShort();
		if (num > 0)
		{
			int num2 = in1.ReadUShort();
			_unknownPreFormulaInt = in1.ReadInt();
			Ptg[] array = Ptg.ReadTokens(num2, in1);
			if (array.Length != 1)
			{
				throw new RecordFormatException("Read " + array.Length + " tokens but expected exactly 1");
			}
			_linkPtg = array[0];
			switch (num - num2 - 6)
			{
			case 1:
				_unknownPostFormulaByte = (byte)in1.ReadByte();
				break;
			case 0:
				_unknownPostFormulaByte = null;
				break;
			default:
				throw new RecordFormatException("Unexpected leftover bytes");
			}
		}
		_cLines = in1.ReadUShort();
		_iSel = in1.ReadUShort();
		_flags = in1.ReadUShort();
		_idEdit = in1.ReadUShort();
		if (cmoOt == 20)
		{
			_dropData = new LbsDropData(in1);
		}
		if ((_flags & 2) != 0)
		{
			_rgLines = new string[_cLines];
			for (int i = 0; i < _cLines; i++)
			{
				_rgLines[i] = StringUtil.ReadUnicodeString(in1);
			}
		}
		if (((_flags >> 4) & 2) != 0)
		{
			_bsels = new bool[_cLines];
			for (int j = 0; j < _cLines; j++)
			{
				_bsels[j] = in1.ReadByte() == 1;
			}
		}
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(19);
		out1.WriteShort(_cbFContinued);
		if (_linkPtg == null)
		{
			out1.WriteShort(0);
		}
		else
		{
			int size = _linkPtg.Size;
			int num = size + 6;
			if (_unknownPostFormulaByte.HasValue)
			{
				num++;
			}
			out1.WriteShort(num);
			out1.WriteShort(size);
			out1.WriteInt(_unknownPreFormulaInt);
			_linkPtg.Write(out1);
			if (_unknownPostFormulaByte.HasValue)
			{
				out1.WriteByte(Convert.ToByte(_unknownPostFormulaByte, CultureInfo.InvariantCulture));
			}
		}
		out1.WriteShort(_cLines);
		out1.WriteShort(_iSel);
		out1.WriteShort(_flags);
		out1.WriteShort(_idEdit);
		if (_dropData != null)
		{
			_dropData.Serialize(out1);
		}
		if (_rgLines != null)
		{
			string[] rgLines = _rgLines;
			foreach (string value in rgLines)
			{
				StringUtil.WriteUnicodeString(out1, value);
			}
		}
		if (_bsels != null)
		{
			bool[] bsels = _bsels;
			foreach (bool flag in bsels)
			{
				out1.WriteByte(flag ? 1 : 0);
			}
		}
	}

	private static Ptg ReadRefPtg(byte[] formulaRawBytes)
	{
		ILittleEndianInput littleEndianInput = new LittleEndianByteArrayInputStream(formulaRawBytes);
		return (byte)littleEndianInput.ReadByte() switch
		{
			37 => new AreaPtg(littleEndianInput), 
			59 => new Area3DPtg(littleEndianInput), 
			36 => new RefPtg(littleEndianInput), 
			58 => new Ref3DPtg(littleEndianInput), 
			_ => null, 
		};
	}

	public override object Clone()
	{
		return this;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder(256);
		stringBuilder.Append("[ftLbsData]\n");
		stringBuilder.Append("    .unknownshort1 =").Append(HexDump.ShortToHex(_cbFContinued)).Append("\n");
		stringBuilder.Append("    .formula        = ").Append('\n');
		if (_linkPtg != null)
		{
			stringBuilder.Append(_linkPtg.ToString()).Append(_linkPtg.RVAType).Append('\n');
		}
		stringBuilder.Append("    .nEntryCount   =").Append(HexDump.ShortToHex(_cLines)).Append("\n");
		stringBuilder.Append("    .selEntryIx    =").Append(HexDump.ShortToHex(_iSel)).Append("\n");
		stringBuilder.Append("    .style         =").Append(HexDump.ShortToHex(_flags)).Append("\n");
		stringBuilder.Append("    .unknownshort10=").Append(HexDump.ShortToHex(_idEdit)).Append("\n");
		if (_dropData != null)
		{
			stringBuilder.Append('\n').Append(_dropData.ToString());
		}
		stringBuilder.Append("[/ftLbsData]\n");
		return stringBuilder.ToString();
	}

	public static LbsDataSubRecord CreateAutoFilterInstance()
	{
		LbsDataSubRecord lbsDataSubRecord = new LbsDataSubRecord();
		lbsDataSubRecord._cbFContinued = 8174;
		lbsDataSubRecord._iSel = 0;
		lbsDataSubRecord._flags = 769;
		lbsDataSubRecord._dropData = new LbsDropData();
		lbsDataSubRecord._dropData._wStyle = 2;
		lbsDataSubRecord._dropData._cLine = 8;
		return lbsDataSubRecord;
	}
}
