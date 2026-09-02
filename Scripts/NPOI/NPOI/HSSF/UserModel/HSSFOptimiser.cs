using System.Collections;
using NPOI.HSSF.Record;
using NPOI.SS.UserModel;

namespace NPOI.HSSF.UserModel;

public class HSSFOptimiser
{
	public static void OptimiseFonts(HSSFWorkbook workbook)
	{
		short[] array = new short[workbook.Workbook.NumberOfFontRecords + 1];
		bool[] array2 = new bool[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = (short)i;
			array2[i] = false;
		}
		FontRecord[] array3 = new FontRecord[array.Length];
		for (int j = 0; j < array.Length; j++)
		{
			if (j != 4)
			{
				array3[j] = workbook.Workbook.GetFontRecordAt(j);
			}
		}
		for (int k = 5; k < array.Length; k++)
		{
			int num = -1;
			for (int l = 0; l < k; l++)
			{
				if (num != -1)
				{
					break;
				}
				if (l != 4 && workbook.Workbook.GetFontRecordAt(l).SameProperties(array3[k]))
				{
					num = l;
				}
			}
			if (num != -1)
			{
				array[k] = (short)num;
				array2[k] = true;
			}
		}
		for (int m = 5; m < array.Length; m++)
		{
			short num2 = array[m];
			short num3 = num2;
			for (int n = 0; n < num2; n++)
			{
				if (array2[n])
				{
					num3--;
				}
			}
			array[m] = num3;
		}
		for (int num4 = 5; num4 < array.Length; num4++)
		{
			if (array2[num4])
			{
				workbook.Workbook.RemoveFontRecord(array3[num4]);
			}
		}
		workbook.ResetFontCache();
		for (int num5 = 0; num5 < workbook.Workbook.NumExFormats; num5++)
		{
			ExtendedFormatRecord exFormatAt = workbook.Workbook.GetExFormatAt(num5);
			exFormatAt.FontIndex = array[exFormatAt.FontIndex];
		}
		ArrayList arrayList = new ArrayList();
		for (int num6 = 0; num6 < workbook.NumberOfSheets; num6++)
		{
			foreach (IRow item in workbook.GetSheetAt(num6))
			{
				foreach (ICell item2 in item)
				{
					if (item2.CellType != CellType.String)
					{
						continue;
					}
					UnicodeString rawUnicodeString = ((HSSFRichTextString)item2.RichStringCellValue).RawUnicodeString;
					if (arrayList.Contains(rawUnicodeString))
					{
						continue;
					}
					for (short num7 = 5; num7 < array.Length; num7++)
					{
						if (num7 != array[num7])
						{
							rawUnicodeString.SwapFontUse(num7, array[num7]);
						}
					}
					arrayList.Add(rawUnicodeString);
				}
			}
		}
	}

	public static void OptimiseCellStyles(HSSFWorkbook workbook)
	{
		short[] array = new short[workbook.Workbook.NumExFormats];
		bool[] array2 = new bool[array.Length];
		bool[] array3 = new bool[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array2[i] = false;
			array[i] = (short)i;
			array3[i] = false;
		}
		ExtendedFormatRecord[] array4 = new ExtendedFormatRecord[array.Length];
		for (int j = 0; j < array.Length; j++)
		{
			array4[j] = workbook.Workbook.GetExFormatAt(j);
		}
		for (int k = 21; k < array.Length; k++)
		{
			int num = -1;
			for (int l = 0; l < k; l++)
			{
				if (num != -1)
				{
					break;
				}
				if (workbook.Workbook.GetExFormatAt(l).Equals(array4[k]))
				{
					num = l;
				}
			}
			if (num != -1)
			{
				array[k] = (short)num;
				array3[k] = true;
			}
			if (num != -1)
			{
				array2[num] = true;
			}
		}
		for (int m = 0; m < workbook.NumberOfSheets; m++)
		{
			foreach (IRow item in (HSSFSheet)workbook.GetSheetAt(m))
			{
				foreach (HSSFCell item2 in item)
				{
					short xFIndex = item2.CellValueRecord.XFIndex;
					array2[xFIndex] = true;
				}
			}
		}
		for (int n = 21; n < array2.Length; n++)
		{
			if (!array2[n])
			{
				array3[n] = true;
				array[n] = 0;
			}
		}
		for (int num2 = 21; num2 < array.Length; num2++)
		{
			short num3 = array[num2];
			short num4 = num3;
			for (int num5 = 0; num5 < num3; num5++)
			{
				if (array3[num5])
				{
					num4--;
				}
			}
			array[num2] = num4;
		}
		int num6 = array.Length;
		int num7 = 0;
		for (int num8 = 21; num8 < num6; num8++)
		{
			if (array3[num8 + num7])
			{
				workbook.Workbook.RemoveExFormatRecord(num8);
				num8--;
				num6--;
				num7++;
			}
		}
		for (int num9 = 0; num9 < workbook.NumberOfSheets; num9++)
		{
			foreach (IRow item3 in (HSSFSheet)workbook.GetSheetAt(num9))
			{
				foreach (ICell item4 in item3)
				{
					short xFIndex2 = ((HSSFCell)item4).CellValueRecord.XFIndex;
					ICellStyle cellStyleAt = workbook.GetCellStyleAt(array[xFIndex2]);
					item4.CellStyle = cellStyleAt;
				}
			}
		}
	}
}
