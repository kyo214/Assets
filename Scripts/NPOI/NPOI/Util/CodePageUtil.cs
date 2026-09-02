using System;
using System.Text;

namespace NPOI.Util;

public class CodePageUtil
{
	public const int CP_037 = 37;

	public const int CP_SJIS = 932;

	public const int CP_GBK = 936;

	public const int CP_MS949 = 949;

	public const int CP_UTF16 = 1200;

	public const int CP_UTF16_BE = 1201;

	public const int CP_WINDOWS_1250 = 1250;

	public const int CP_WINDOWS_1251 = 1251;

	public const int CP_WINDOWS_1252 = 1252;

	public const int CP_WINDOWS_1252_BIFF23 = 32769;

	public const int CP_WINDOWS_1253 = 1253;

	public const int CP_WINDOWS_1254 = 1254;

	public const int CP_WINDOWS_1255 = 1255;

	public const int CP_WINDOWS_1256 = 1256;

	public const int CP_WINDOWS_1257 = 1257;

	public const int CP_WINDOWS_1258 = 1258;

	public const int CP_JOHAB = 1361;

	public const int CP_MAC_ROMAN = 10000;

	public const int CP_MAC_ROMAN_BIFF23 = 32768;

	public const int CP_MAC_JAPAN = 10001;

	public const int CP_MAC_CHINESE_TRADITIONAL = 10002;

	public const int CP_MAC_KOREAN = 10003;

	public const int CP_MAC_ARABIC = 10004;

	public const int CP_MAC_HEBREW = 10005;

	public const int CP_MAC_GREEK = 10006;

	public const int CP_MAC_CYRILLIC = 10007;

	public const int CP_MAC_CHINESE_SIMPLE = 10008;

	public const int CP_MAC_ROMANIA = 10010;

	public const int CP_MAC_UKRAINE = 10017;

	public const int CP_MAC_THAI = 10021;

	public const int CP_MAC_CENTRAL_EUROPE = 10029;

	public const int CP_MAC_ICELAND = 10079;

	public const int CP_MAC_TURKISH = 10081;

	public const int CP_MAC_CROATIAN = 10082;

	public const int CP_US_ACSII = 20127;

	public const int CP_KOI8_R = 20866;

	public const int CP_ISO_8859_1 = 28591;

	public const int CP_ISO_8859_2 = 28592;

	public const int CP_ISO_8859_3 = 28593;

	public const int CP_ISO_8859_4 = 28594;

	public const int CP_ISO_8859_5 = 28595;

	public const int CP_ISO_8859_6 = 28596;

	public const int CP_ISO_8859_7 = 28597;

	public const int CP_ISO_8859_8 = 28598;

	public const int CP_ISO_8859_9 = 28599;

	public const int CP_ISO_2022_JP1 = 50220;

	public const int CP_ISO_2022_JP2 = 50221;

	public const int CP_ISO_2022_JP3 = 50222;

	public const int CP_ISO_2022_KR = 50225;

	public const int CP_EUC_JP = 51932;

	public const int CP_EUC_KR = 51949;

	public const int CP_GB2312 = 52936;

	public const int CP_GB18030 = 54936;

	public const int CP_US_ASCII2 = 65000;

	public const int CP_UTF8 = 65001;

	public const int CP_UNICODE = 1200;

	static CodePageUtil()
	{
	}

	public static byte[] GetBytesInCodePage(string string1, int codepage)
	{
		string name = CodepageToEncoding(codepage);
		Encoding encoding;
		try
		{
			encoding = Encoding.GetEncoding(name);
		}
		catch (Exception)
		{
			encoding = Encoding.ASCII;
		}
		return encoding.GetBytes(string1);
	}

	public static string GetStringFromCodePage(byte[] string1, int codepage)
	{
		return GetStringFromCodePage(string1, 0, string1.Length, codepage);
	}

	public static string GetStringFromCodePage(byte[] string1, int offset, int length, int codepage)
	{
		return Encoding.GetEncoding(CodepageToEncoding(codepage)).GetString(string1, offset, length);
	}

	public static string CodepageToEncoding(int codepage)
	{
		if (codepage <= 0)
		{
			throw new ArgumentException("Codepage number may not be " + codepage);
		}
		switch (codepage)
		{
		case 1200:
			return "UTF-16";
		case 1201:
			return "UTF-16BE";
		case 65001:
			return "UTF-8";
		case 37:
			return "cp037";
		case 936:
			return "GBK";
		case 949:
			return "ms949";
		case 1250:
			return "windows-1250";
		case 1251:
			return "windows-1251";
		case 1252:
		case 32769:
			return "windows-1252";
		case 1253:
			return "windows-1253";
		case 1254:
			return "windows-1254";
		case 1255:
			return "windows-1255";
		case 1256:
			return "windows-1256";
		case 1257:
			return "windows-1257";
		case 1258:
			return "windows-1258";
		case 1361:
			return "johab";
		case 10000:
		case 32768:
			return "MacRoman";
		case 10001:
			return "SJIS";
		case 10002:
			return "Big5";
		case 10003:
			return "EUC-KR";
		case 10004:
			return "MacArabic";
		case 10005:
			return "MacHebrew";
		case 10006:
			return "MacGreek";
		case 10007:
			return "MacCyrillic";
		case 10008:
			return "EUC_CN";
		case 10010:
			return "MacRomania";
		case 10017:
			return "MacUkraine";
		case 10021:
			return "MacThai";
		case 10029:
			return "MacCentralEurope";
		case 10079:
			return "MacIceland";
		case 10081:
			return "MacTurkish";
		case 10082:
			return "MacCroatian";
		case 20127:
		case 65000:
			return "US-ASCII";
		case 20866:
			return "KOI8-R";
		case 28591:
			return "ISO-8859-1";
		case 28592:
			return "ISO-8859-2";
		case 28593:
			return "ISO-8859-3";
		case 28594:
			return "ISO-8859-4";
		case 28595:
			return "ISO-8859-5";
		case 28596:
			return "ISO-8859-6";
		case 28597:
			return "ISO-8859-7";
		case 28598:
			return "ISO-8859-8";
		case 28599:
			return "ISO-8859-9";
		case 50220:
		case 50221:
		case 50222:
			return "ISO-2022-JP";
		case 50225:
			return "ISO-2022-KR";
		case 51932:
			return "EUC-JP";
		case 51949:
			return "EUC-KR";
		case 52936:
			return "GB2312";
		case 54936:
			return "GB18030";
		case 932:
			return "SJIS";
		default:
			return "cp" + codepage;
		}
	}
}
