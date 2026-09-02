using System.Collections;
using System.IO;
using System.Text;

namespace NPOI.Util;

public class HexRead
{
	public static byte[] ReadData(string filename)
	{
		FileStream fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read);
		try
		{
			return ReadData(fileStream, -1);
		}
		finally
		{
			fileStream.Close();
		}
	}

	public static byte[] ReadData(Stream stream, string section)
	{
		try
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			int num = stream.ReadByte();
			while (true)
			{
				switch (num)
				{
				case 91:
					flag = true;
					goto IL_006b;
				case 10:
				case 13:
					flag = false;
					stringBuilder = new StringBuilder();
					goto IL_006b;
				case 93:
					flag = false;
					if (stringBuilder.ToString().Equals(section))
					{
						return ReadData(stream, 91);
					}
					stringBuilder = new StringBuilder();
					goto IL_006b;
				default:
					if (flag)
					{
						stringBuilder.Append((char)num);
					}
					goto IL_006b;
				case -1:
					break;
				}
				break;
				IL_006b:
				num = stream.ReadByte();
			}
		}
		finally
		{
			stream.Close();
		}
		throw new IOException("Section '" + section + "' not found");
	}

	public static byte[] ReadData(string filename, string section)
	{
		using FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read);
		return ReadData(stream, section);
	}

	public static byte[] ReadData(Stream stream, int eofChar)
	{
		int num = 0;
		byte b = 0;
		ArrayList arrayList = new ArrayList();
		char c = 'W';
		char c2 = '7';
		while (true)
		{
			int num2 = stream.ReadByte();
			int num3 = -1;
			if (48 <= num2 && num2 <= 57)
			{
				num3 = num2 - 48;
			}
			else if (65 <= num2 && num2 <= 70)
			{
				num3 = num2 - c2;
			}
			else if (97 <= num2 && num2 <= 102)
			{
				num3 = num2 - c;
			}
			else if (35 == num2)
			{
				ReadToEOL(stream);
			}
			else if (-1 == num2 || eofChar == num2)
			{
				break;
			}
			if (num3 != -1)
			{
				b <<= 4;
				b += (byte)num3;
				num++;
				if (num == 2)
				{
					arrayList.Add(b);
					num = 0;
					b = 0;
				}
			}
		}
		return (byte[])arrayList.ToArray(typeof(byte));
	}

	public static byte[] ReadFromString(string data)
	{
		using MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(data));
		return ReadData(stream, -1);
	}

	private static void ReadToEOL(Stream stream)
	{
		int num = stream.ReadByte();
		while (num != -1 && num != 10 && num != 13)
		{
			num = stream.ReadByte();
		}
	}
}
