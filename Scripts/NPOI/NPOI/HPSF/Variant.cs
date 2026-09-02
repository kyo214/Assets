using System;
using System.Collections;

namespace NPOI.HPSF;

public class Variant
{
	public const int VT_EMPTY = 0;

	public const int VT_NULL = 1;

	public const int VT_I2 = 2;

	public const int VT_I4 = 3;

	public const int VT_R4 = 4;

	public const int VT_R8 = 5;

	public const int VT_CY = 6;

	public const int VT_DATE = 7;

	public const int VT_BSTR = 8;

	public const int VT_DISPATCH = 9;

	public const int VT_ERROR = 10;

	public const int VT_BOOL = 11;

	public const int VT_VARIANT = 12;

	public const int VT_UNKNOWN = 13;

	public const int VT_DECIMAL = 14;

	public const int VT_I1 = 16;

	public const int VT_UI1 = 17;

	public const int VT_UI2 = 18;

	public const int VT_UI4 = 19;

	public const int VT_I8 = 20;

	public const int VT_UI8 = 21;

	public const int VT_INT = 22;

	public const int VT_UINT = 23;

	public const int VT_VOID = 24;

	public const int VT_HRESULT = 25;

	public const int VT_PTR = 26;

	public const int VT_SAFEARRAY = 27;

	public const int VT_CARRAY = 28;

	public const int VT_USERDEFINED = 29;

	public const int VT_LPSTR = 30;

	public const int VT_LPWSTR = 31;

	public const int VT_FILETIME = 64;

	public const int VT_BLOB = 65;

	public const int VT_STREAM = 66;

	public const int VT_STORAGE = 67;

	public const int VT_STREAMED_OBJECT = 68;

	public const int VT_STORED_OBJECT = 69;

	public const int VT_BLOB_OBJECT = 70;

	public const int VT_CF = 71;

	public const int VT_CLSID = 72;

	public const int VT_VERSIONED_STREAM = 73;

	public const int VT_VECTOR = 4096;

	public const int VT_ARRAY = 8192;

	public const int VT_BYREF = 16384;

	public const int VT_RESERVED = 32768;

	public const int VT_ILLEGAL = 65535;

	public const int VT_ILLEGALMASKED = 4095;

	public const int VT_TYPEMASK = 4095;

	private static IDictionary numberToName;

	private static IDictionary numberToLength;

	public const int Length_UNKNOWN = -2;

	public const int Length_VARIABLE = -1;

	public const int Length_0 = 0;

	public const int Length_2 = 2;

	public const int Length_4 = 4;

	public const int Length_8 = 8;

	static Variant()
	{
		numberToName = new Hashtable
		{
			[0] = "VT_EMPTY",
			[1] = "VT_NULL",
			[2] = "VT_I2",
			[3] = "VT_I4",
			[4] = "VT_R4",
			[5] = "VT_R8",
			[6] = "VT_CY",
			[7] = "VT_DATE",
			[8] = "VT_BSTR",
			[9] = "VT_DISPATCH",
			[10] = "VT_ERROR",
			[11] = "VT_BOOL",
			[12] = "VT_VARIANT",
			[13] = "VT_UNKNOWN",
			[14] = "VT_DECIMAL",
			[16] = "VT_I1",
			[17] = "VT_UI1",
			[18] = "VT_UI2",
			[19] = "VT_UI4",
			[20] = "VT_I8",
			[21] = "VT_UI8",
			[22] = "VT_INT",
			[23] = "VT_UINT",
			[24] = "VT_VOID",
			[25] = "VT_HRESULT",
			[26] = "VT_PTR",
			[27] = "VT_SAFEARRAY",
			[28] = "VT_CARRAY",
			[29] = "VT_USERDEFINED",
			[30] = "VT_LPSTR",
			[31] = "VT_LPWSTR",
			[64] = "VT_FILETIME",
			[65] = "VT_BLOB",
			[66] = "VT_STREAM",
			[67] = "VT_STORAGE",
			[68] = "VT_STREAMED_OBJECT",
			[69] = "VT_STORED_OBJECT",
			[70] = "VT_BLOB_OBJECT",
			[71] = "VT_CF",
			[72] = "VT_CLSID"
		};
		numberToLength = new Hashtable
		{
			[0] = 0,
			[1] = -2,
			[2] = 2,
			[3] = 4,
			[4] = 4,
			[5] = 8,
			[6] = -2,
			[7] = -2,
			[8] = -2,
			[9] = -2,
			[10] = -2,
			[11] = -2,
			[12] = -2,
			[13] = -2,
			[14] = -2,
			[16] = -2,
			[17] = -2,
			[18] = -2,
			[19] = -2,
			[20] = -2,
			[21] = -2,
			[22] = -2,
			[23] = -2,
			[24] = -2,
			[25] = -2,
			[26] = -2,
			[27] = -2,
			[28] = -2,
			[29] = -2,
			[30] = -1,
			[31] = -2,
			[64] = 8,
			[65] = -2,
			[66] = -2,
			[67] = -2,
			[68] = -2,
			[69] = -2,
			[70] = -2,
			[71] = -2,
			[72] = -2
		};
	}

	public static string GetVariantName(long variantType)
	{
		string text = (string)numberToName[variantType];
		if (text == null)
		{
			return "unknown variant type";
		}
		return text;
	}

	public static int GetVariantLength(long variantType)
	{
		long num = (int)variantType;
		if (numberToLength.Contains(num))
		{
			return -2;
		}
		return Convert.ToInt32((long)numberToLength[num]);
	}
}
