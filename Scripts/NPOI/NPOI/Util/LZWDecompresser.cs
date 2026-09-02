using System.IO;

namespace NPOI.Util;

public abstract class LZWDecompresser
{
	private bool maskMeansCompressed;

	private int codeLengthIncrease;

	private bool positionIsBigEndian;

	protected LZWDecompresser(bool maskMeansCompressed, int codeLengthIncrease, bool positionIsBigEndian)
	{
		this.maskMeansCompressed = maskMeansCompressed;
		this.codeLengthIncrease = codeLengthIncrease;
		this.positionIsBigEndian = positionIsBigEndian;
	}

	protected abstract int populateDictionary(byte[] dict);

	protected abstract int adjustDictionaryOffset(int offset);

	public byte[] decompress(Stream src)
	{
		using MemoryStream memoryStream = new MemoryStream();
		decompress(src, memoryStream);
		return memoryStream.ToArray();
	}

	public void decompress(Stream src, Stream res)
	{
		byte[] array = new byte[4096];
		int num = populateDictionary(array);
		byte[] array2 = new byte[16 + codeLengthIncrease];
		int num2;
		while ((num2 = src.ReadByte()) != -1)
		{
			for (int num3 = 1; num3 < 256; num3 <<= 1)
			{
				if (((num2 & num3) > 0) ^ maskMeansCompressed)
				{
					int b;
					if ((b = src.ReadByte()) != -1)
					{
						array[num & 0xFFF] = fromInt(b);
						num++;
						res.WriteByte(fromInt(b));
					}
				}
				else
				{
					int num4 = src.ReadByte();
					int num5 = src.ReadByte();
					if (num4 == -1 || num5 == -1)
					{
						break;
					}
					int num6 = (num5 & 0xF) + codeLengthIncrease;
					int offset = ((!positionIsBigEndian) ? (num4 + ((num5 & 0xF0) << 4)) : ((num4 << 4) + (num5 >> 4)));
					offset = adjustDictionaryOffset(offset);
					for (int i = 0; i < num6; i++)
					{
						array2[i] = array[(offset + i) & 0xFFF];
						array[(num + i) & 0xFFF] = array2[i];
					}
					res.Write(array2, 0, num6);
					num += num6;
				}
			}
		}
	}

	public static byte fromInt(int b)
	{
		if (b < 128)
		{
			return (byte)b;
		}
		return (byte)(b - 256);
	}

	public static int fromByte(byte b)
	{
		if (b >= 0)
		{
			return b;
		}
		return b + 256;
	}
}
