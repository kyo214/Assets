using System;
using System.IO;

namespace NPOI.Util;

public class RLEDecompressingInputStream : InputStream
{
	private static int[] POWER2 = new int[16]
	{
		1, 2, 4, 8, 16, 32, 64, 128, 256, 512,
		1024, 2048, 4096, 8192, 16384, 32768
	};

	private Stream input;

	private byte[] buf;

	private int pos;

	private int len;

	public override bool CanRead => input.CanRead;

	public override bool CanSeek => input.CanSeek;

	public override bool CanWrite => input.CanWrite;

	public override long Length => input.Length;

	public override long Position
	{
		get
		{
			return input.Position;
		}
		set
		{
			input.Position = value;
		}
	}

	public RLEDecompressingInputStream(Stream input)
	{
		this.input = input;
		buf = new byte[4096];
		pos = 0;
		int num = input.ReadByte();
		if (num != 1)
		{
			throw new ArgumentException($"Header byte 0x01 expected, received 0x{num & 0xFF:X2}");
		}
		len = ReadChunk();
	}

	public override int Read()
	{
		if (len == -1)
		{
			return -1;
		}
		if (pos >= len && (len = ReadChunk()) == -1)
		{
			return -1;
		}
		return buf[pos++];
	}

	public override int Read(byte[] b)
	{
		return Read(b, 0, b.Length);
	}

	public override int Read(byte[] b, int off, int l)
	{
		if (len == -1)
		{
			return -1;
		}
		int num = off;
		int num2 = l;
		while (num2 > 0)
		{
			if (pos >= len && (len = ReadChunk()) == -1)
			{
				if (num <= off)
				{
					return -1;
				}
				return num - off;
			}
			int num3 = Math.Min(num2, len - pos);
			Array.Copy(buf, pos, b, num, num3);
			pos += num3;
			num2 -= num3;
			num += num3;
		}
		return l;
	}

	public override long Skip(long n)
	{
		long num = n;
		while (num > 0)
		{
			if (pos >= len && (len = ReadChunk()) == -1)
			{
				return -1L;
			}
			int num2 = (int)Math.Min(n, len - pos);
			pos += num2;
			num -= num2;
		}
		return n;
	}

	public override int Available()
	{
		if (len <= 0)
		{
			return 0;
		}
		return len - pos;
	}

	public override void Close()
	{
		input.Close();
	}

	private int ReadChunk()
	{
		pos = 0;
		int num = ReadShort(input);
		if (num == -1)
		{
			return -1;
		}
		int num2 = (num & 0xFFF) + 1;
		if ((num & 0x7000) != 12288)
		{
			throw new ArgumentException($"Chunksize header A should be 0x3000, received 0x{num & 0xE000:X4}");
		}
		if ((num & 0x8000) == 0)
		{
			if (input.Read(buf, 0, num2) < num2)
			{
				throw new InvalidOperationException($"Not enough bytes Read, expected {num2}");
			}
			return num2;
		}
		int num3 = 0;
		int num4 = 0;
		while (num3 < num2)
		{
			int num5 = input.ReadByte();
			num3++;
			if (num5 == -1)
			{
				break;
			}
			for (int i = 0; i < 8; i++)
			{
				if (num3 >= num2)
				{
					break;
				}
				if ((num5 & POWER2[i]) == 0)
				{
					int num6 = input.ReadByte();
					if (num6 == -1)
					{
						return -1;
					}
					buf[num4++] = (byte)num6;
					num3++;
					continue;
				}
				int num7 = ReadShort(input);
				if (num7 == -1)
				{
					return -1;
				}
				num3 += 2;
				int copyLenBits = GetCopyLenBits(num4 - 1);
				int num8 = (num7 >> copyLenBits) + 1;
				int num9 = (num7 & (POWER2[copyLenBits] - 1)) + 3;
				int num10 = num4 - num8;
				int num11 = num10 + num9;
				for (int j = num10; j < num11; j++)
				{
					buf[num4++] = buf[j];
				}
			}
		}
		return num4;
	}

	private static int GetCopyLenBits(int offset)
	{
		for (int num = 11; num >= 4; num--)
		{
			if ((offset & POWER2[num]) != 0)
			{
				return 15 - num;
			}
		}
		return 12;
	}

	public int ReadShort()
	{
		return ReadShort(this);
	}

	public int ReadInt()
	{
		return ReadInt(this);
	}

	private int ReadShort(Stream stream)
	{
		int num;
		if ((num = stream.ReadByte()) == -1)
		{
			return -1;
		}
		int num2;
		if ((num2 = stream.ReadByte()) == -1)
		{
			return -1;
		}
		return (num & 0xFF) | ((num2 & 0xFF) << 8);
	}

	private int ReadInt(InputStream stream)
	{
		int num;
		if ((num = stream.Read()) == -1)
		{
			return -1;
		}
		int num2;
		if ((num2 = stream.Read()) == -1)
		{
			return -1;
		}
		int num3;
		if ((num3 = stream.Read()) == -1)
		{
			return -1;
		}
		int num4;
		if ((num4 = stream.Read()) == -1)
		{
			return -1;
		}
		return (num & 0xFF) | ((num2 & 0xFF) << 8) | ((num3 & 0xFF) << 16) | ((num4 & 0xFF) << 24);
	}

	public static byte[] Decompress(byte[] compressed)
	{
		return Decompress(compressed, 0, compressed.Length);
	}

	public static byte[] Decompress(byte[] compressed, int offset, int length)
	{
		MemoryStream memoryStream = new MemoryStream();
		RLEDecompressingInputStream rLEDecompressingInputStream = new RLEDecompressingInputStream(new MemoryStream(compressed, offset, length));
		IOUtils.Copy(rLEDecompressingInputStream, memoryStream);
		rLEDecompressingInputStream.Close();
		memoryStream.Close();
		return memoryStream.ToArray();
	}

	public override void Flush()
	{
		input.Flush();
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		return input.Seek(offset, origin);
	}

	public override void SetLength(long value)
	{
		input.SetLength(value);
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		input.Write(buffer, offset, count);
	}
}
