using System;
using System.Collections;
using System.Collections.Generic;
using NPOI.HSSF.Model;
using NPOI.HSSF.Record;
using NPOI.SS.UserModel;

namespace NPOI.HSSF.UserModel;

[Serializable]
public class HSSFDataFormat : IDataFormat
{
	public const int FIRST_USER_DEFINED_FORMAT_INDEX = 164;

	private static List<string> builtinFormats = new List<string>(BuiltinFormats.GetAll());

	private List<string> formats = new List<string>();

	private InternalWorkbook workbook;

	private bool movedBuiltins;

	public static int NumberOfBuiltinBuiltinFormats => builtinFormats.Count;

	public HSSFDataFormat(InternalWorkbook workbook)
	{
		this.workbook = workbook;
		IEnumerator enumerator = workbook.Formats.GetEnumerator();
		while (enumerator.MoveNext())
		{
			FormatRecord formatRecord = (FormatRecord)enumerator.Current;
			int num = formats.Count;
			while (formats.Count <= formatRecord.IndexCode)
			{
				formats.Add(null);
				num++;
			}
			formats[formatRecord.IndexCode] = formatRecord.FormatString;
		}
	}

	public static List<string> GetBuiltinFormats()
	{
		return builtinFormats;
	}

	public static short GetBuiltinFormat(string format)
	{
		if (format.ToUpper().Equals("TEXT"))
		{
			format = "@";
		}
		short result = -1;
		for (short num = 0; num <= 49; num++)
		{
			string text = builtinFormats[num];
			if (text != null && text.Equals(format))
			{
				result = num;
				break;
			}
		}
		return result;
	}

	public short GetFormat(string pFormat)
	{
		string text = ((!pFormat.ToUpper().Equals("TEXT")) ? pFormat : "@");
		IEnumerator enumerator;
		int num;
		if (!movedBuiltins)
		{
			enumerator = builtinFormats.GetEnumerator();
			num = 0;
			while (enumerator.MoveNext())
			{
				int num2 = formats.Count;
				while (formats.Count < num + 1)
				{
					formats.Add(null);
					num2++;
				}
				formats[num] = enumerator.Current as string;
				num++;
			}
			movedBuiltins = true;
		}
		enumerator = formats.GetEnumerator();
		num = 0;
		while (enumerator.MoveNext())
		{
			if (text.Equals(enumerator.Current))
			{
				return (short)num;
			}
			num++;
		}
		num = workbook.GetFormat(text, CreateIfNotFound: true);
		int num3 = formats.Count;
		while (formats.Count < num + 1)
		{
			formats.Add(null);
			num3++;
		}
		formats[num] = text;
		return (short)num;
	}

	public string GetFormat(short index)
	{
		if (movedBuiltins)
		{
			return formats[index];
		}
		if (index == -1)
		{
			return null;
		}
		string text = ((formats.Count > index) ? formats[index] : null);
		if (builtinFormats.Count > index && builtinFormats[index] != null)
		{
			if (text != null)
			{
				return text;
			}
			return builtinFormats[index];
		}
		return text;
	}

	public static string GetBuiltinFormat(short index)
	{
		return builtinFormats[index];
	}

	private void EnsureFormatsSize(int index)
	{
		if (formats.Count <= index)
		{
			formats.Capacity = index + 1;
		}
	}
}
