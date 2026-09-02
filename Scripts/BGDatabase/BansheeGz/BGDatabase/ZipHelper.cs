using System.IO;

namespace BansheeGz.BGDatabase;

internal static class ZipHelper
{
	internal const uint Mask32Bit = uint.MaxValue;

	internal const ushort Mask16Bit = ushort.MaxValue;

	private const int BackwardsSeekingBufferSize = 32;

	internal static void ReadBytes(Stream stream, byte[] buffer, int bytesToRead)
	{
		int num = bytesToRead;
		int num2 = 0;
		while (num > 0)
		{
			int num3 = stream.Read(buffer, num2, num);
			if (num3 == 0)
			{
				throw new IOException();
			}
			num2 += num3;
			num -= num3;
		}
	}

	internal static bool SeekBackwardsToSignature(Stream stream, uint signatureToFind)
	{
		int bufferPointer = 0;
		uint num = 0u;
		byte[] array = new byte[32];
		bool flag = false;
		bool flag2 = false;
		while (!flag2 && !flag)
		{
			flag = SeekBackwardsAndRead(stream, array, out bufferPointer);
			while (bufferPointer >= 0 && !flag2)
			{
				num = (num << 8) | array[bufferPointer];
				if (num == signatureToFind)
				{
					flag2 = true;
				}
				else
				{
					bufferPointer--;
				}
			}
		}
		if (!flag2)
		{
			return false;
		}
		stream.Seek(bufferPointer, SeekOrigin.Current);
		return true;
	}

	internal static void AdvanceToPosition(this Stream stream, long position)
	{
		long num = position - stream.Position;
		while (num != 0L)
		{
			int count = (int)((num > 64) ? 64 : num);
			int num2 = stream.Read(new byte[64], 0, count);
			if (num2 == 0)
			{
				throw new IOException();
			}
			num -= num2;
		}
	}

	private static bool SeekBackwardsAndRead(Stream stream, byte[] buffer, out int bufferPointer)
	{
		if (stream.Position >= buffer.Length)
		{
			stream.Seek(-buffer.Length, SeekOrigin.Current);
			ReadBytes(stream, buffer, buffer.Length);
			stream.Seek(-buffer.Length, SeekOrigin.Current);
			bufferPointer = buffer.Length - 1;
			return false;
		}
		int num = (int)stream.Position;
		stream.Seek(0L, SeekOrigin.Begin);
		ReadBytes(stream, buffer, num);
		stream.Seek(0L, SeekOrigin.Begin);
		bufferPointer = num - 1;
		return true;
	}
}
