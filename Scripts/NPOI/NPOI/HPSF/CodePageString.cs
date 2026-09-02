using System;
using System.IO;
using System.Text;
using NPOI.Util;

namespace NPOI.HPSF;

public class CodePageString
{
	private byte[] _value;

	public int Size => 4 + _value.Length;

	public CodePageString(byte[] data, int startOffset)
	{
		int num = startOffset;
		int num2 = LittleEndian.GetInt(data, num);
		num += 4;
		_value = LittleEndian.GetByteArray(data, num, num2);
		if (num2 != 0 && _value[num2 - 1] != 0)
		{
			Console.WriteLine("CodePageString started at offset #" + num + " is not NULL-terminated");
		}
	}

	public CodePageString(string aString, int codepage)
	{
		SetJavaValue(aString, codepage);
	}

	public string GetJavaValue(int codepage)
	{
		string text = ((codepage != -1) ? Encoding.GetEncoding(codepage).GetString(_value) : Encoding.UTF8.GetString(_value));
		int num = text.IndexOf('\0');
		if (num == -1)
		{
			return text;
		}
		_ = text.Length - 1;
		return text.Substring(0, num);
	}

	public void SetJavaValue(string aString, int codepage)
	{
		string text = aString + "\0";
		if (codepage == -1)
		{
			_value = Encoding.UTF8.GetBytes(text);
		}
		else
		{
			_value = CodePageUtil.GetBytesInCodePage(text, codepage);
		}
	}

	public int Write(Stream out1)
	{
		LittleEndian.PutInt(_value.Length, out1);
		out1.Write(_value, 0, _value.Length);
		return 4 + _value.Length;
	}
}
