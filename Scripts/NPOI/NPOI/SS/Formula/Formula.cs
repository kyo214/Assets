using NPOI.SS.Formula.PTG;
using NPOI.SS.Util;
using NPOI.Util;

namespace NPOI.SS.Formula;

public class Formula
{
	private static readonly Formula EMPTY = new Formula(new byte[0], 0);

	private byte[] _byteEncoding;

	private int _encodedTokenLen;

	public Ptg[] Tokens
	{
		get
		{
			ILittleEndianInput @in = new LittleEndianByteArrayInputStream(_byteEncoding);
			return Ptg.ReadTokens(_encodedTokenLen, @in);
		}
	}

	public int EncodedSize => 2 + _byteEncoding.Length;

	public int EncodedTokenSize => _encodedTokenLen;

	public CellReference ExpReference
	{
		get
		{
			byte[] byteEncoding = _byteEncoding;
			if (byteEncoding.Length != 5)
			{
				return null;
			}
			byte b = byteEncoding[0];
			if (b != 1 && b != 2)
			{
				return null;
			}
			int uShort = LittleEndian.GetUShort(byteEncoding, 1);
			int uShort2 = LittleEndian.GetUShort(byteEncoding, 3);
			return new CellReference(uShort, uShort2);
		}
	}

	private Formula(byte[] byteEncoding, int encodedTokenLen)
	{
		_byteEncoding = (byte[])byteEncoding.Clone();
		_encodedTokenLen = encodedTokenLen;
	}

	public static Formula Read(int encodedTokenLen, ILittleEndianInput in1)
	{
		return Read(encodedTokenLen, in1, encodedTokenLen);
	}

	public static Formula Read(int encodedTokenLen, ILittleEndianInput in1, int totalEncodedLen)
	{
		byte[] array = new byte[totalEncodedLen];
		in1.ReadFully(array);
		return new Formula(array, encodedTokenLen);
	}

	public void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(_encodedTokenLen);
		out1.Write(_byteEncoding);
	}

	public void SerializeTokens(ILittleEndianOutput out1)
	{
		out1.Write(_byteEncoding, 0, _encodedTokenLen);
	}

	public void SerializeArrayConstantData(ILittleEndianOutput out1)
	{
		int len = _byteEncoding.Length - _encodedTokenLen;
		out1.Write(_byteEncoding, _encodedTokenLen, len);
	}

	public static Formula Create(Ptg[] ptgs)
	{
		if (ptgs == null || ptgs.Length < 1)
		{
			return EMPTY;
		}
		byte[] array = new byte[Ptg.GetEncodedSize(ptgs)];
		Ptg.SerializePtgs(ptgs, array, 0);
		int encodedSizeWithoutArrayData = Ptg.GetEncodedSizeWithoutArrayData(ptgs);
		return new Formula(array, encodedSizeWithoutArrayData);
	}

	public static Ptg[] GetTokens(Formula formula)
	{
		return formula?.Tokens;
	}

	public Formula Copy()
	{
		return this;
	}

	public bool IsSame(Formula other)
	{
		return Arrays.Equals(_byteEncoding, other._byteEncoding);
	}
}
