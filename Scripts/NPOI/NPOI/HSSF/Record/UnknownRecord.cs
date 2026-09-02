using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class UnknownRecord : StandardRecord
{
	public const int PLS_004D = 77;

	public const int SHEETPR_0081 = 129;

	public const int SORT_0090 = 144;

	public const int STANDARDWIDTH_0099 = 153;

	public const int BITMAP_00E9 = 233;

	public const int PHONETICPR_00EF = 239;

	public const int LABELRANGES_015F = 351;

	public const int QUICKTIP_0800 = 2048;

	public const int SHEETPROTECTION_0867 = 2151;

	public const int HEADER_FOOTER_089C = 2204;

	public const int CODENAME_1BA = 442;

	public const int PLV_MAC = 2248;

	private int _sid;

	private byte[] _rawData;

	protected override int DataSize => _rawData.Length;

	public override short Sid => (short)_sid;

	public UnknownRecord(int id, byte[] data)
	{
		_sid = id & 0xFFFF;
		_rawData = data;
	}

	public UnknownRecord(RecordInputStream in1)
	{
		_sid = in1.Sid;
		_rawData = in1.ReadRemainder();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.Write(_rawData);
	}

	public override string ToString()
	{
		string text = GetBiffName(_sid);
		if (text == null)
		{
			text = "UNKNOWNRECORD";
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[").Append(text).Append("] (0x");
		stringBuilder.Append(StringUtil.ToHexString(_sid).ToUpper() + ")\n");
		if (_rawData.Length != 0)
		{
			stringBuilder.Append("  rawData=").Append(HexDump.ToHex(_rawData)).Append("\n");
		}
		stringBuilder.Append("[/").Append(text).Append("]\n");
		return stringBuilder.ToString();
	}

	public static string GetBiffName(int sid)
	{
		switch (sid)
		{
		case 77:
			return "PLS";
		case 80:
			return "DCON";
		case 127:
			return "IMDATA";
		case 129:
			return "SHEETPR";
		case 144:
			return "SORT";
		case 148:
			return "LHRECORD";
		case 153:
			return "STANDARDWIDTH";
		case 157:
			return "AUTOFILTERINFO";
		case 174:
			return "SCENMAN";
		case 178:
			return "SXVI";
		case 180:
			return "SXIVD";
		case 181:
			return "SXLI";
		case 211:
			return "OBPROJ";
		case 220:
			return "PARAMQRY";
		case 222:
			return "OLESIZE";
		case 233:
			return "BITMAP";
		case 239:
			return "PHONETICPR";
		case 241:
			return "SXEX";
		case 351:
			return "LABELRANGES";
		case 442:
			return "CODENAME";
		case 425:
			return "USERBVIEW";
		case 429:
			return "QSI";
		case 448:
			return "EXCEL9FILE";
		case 2050:
			return "QSISXTAG";
		case 2051:
			return "DBQUERYEXT";
		case 2053:
			return "TXTQUERY";
		case 2064:
			return "SXVIEWEX9";
		case 2066:
			return "CONTINUEFRT";
		case 2048:
			return "QUICKTIP";
		case 2147:
			return "BOOKEXT";
		case 2148:
			return "SXADDL";
		case 2151:
			return "SHEETPROTECTION";
		case 2155:
			return "DATALABEXTCONTENTS";
		case 2156:
			return "CELLWATCH";
		case 2162:
			return "SHARED FEATURE v11";
		case 2164:
			return "DROPDOWNOBJIDS";
		case 2166:
			return "DCONN";
		case 2168:
			return "SHARED FEATURE v12";
		case 2171:
			return "CFEX";
		case 2172:
			return "XFCRC";
		case 2173:
			return "XFEXT";
		case 2175:
			return "CONTINUEFRT12";
		case 2187:
			return "PLV";
		case 2188:
			return "COMPAT12";
		case 2189:
			return "DXF";
		case 2194:
			return "STYLEEXT";
		case 2198:
			return "THEME";
		case 2199:
			return "GUIDTYPELIB";
		case 2202:
			return "MTRSETTINGS";
		case 2203:
			return "COMPRESSPICTURES";
		case 2204:
			return "HEADERFOOTER";
		case 2205:
			return "CRTLAYOUT12";
		case 2206:
			return "CRTMLFRT";
		case 2207:
			return "CRTMLFRTCONTINUE";
		case 2209:
			return "SHAPEPROPSSTREAM";
		case 2211:
			return "FORCEFULLCALCULATION";
		case 2212:
			return "SHAPEPROPSSTREAM";
		case 2213:
			return "TEXTPROPSSTREAM";
		case 2214:
			return "RICHTEXTSTREAM";
		case 2215:
			return "CRTLAYOUT12A";
		case 2248:
			return "PLV{Mac Excel}";
		case 4097:
			return "UNITS";
		case 4102:
			return "CHARTDATAFORMAT";
		case 4103:
			return "CHARTLINEFORMAT";
		default:
			if (IsObservedButUnknown(sid))
			{
				return "UNKNOWN-" + StringUtil.ToHexString(sid).ToUpper();
			}
			return "BIFF Record-" + StringUtil.ToHexString(sid).ToUpper();
		}
	}

	private static bool IsObservedButUnknown(int sid)
	{
		switch (sid)
		{
		case 51:
		case 52:
		case 445:
		case 450:
		case 4108:
		case 4120:
		case 4123:
		case 4128:
		case 4129:
		case 4130:
		case 4132:
		case 4134:
		case 4147:
		case 4148:
		case 4149:
		case 4161:
		case 4163:
		case 4164:
		case 4165:
		case 4170:
		case 4171:
		case 4174:
		case 4175:
		case 4176:
		case 4177:
		case 4188:
		case 4189:
		case 4191:
		case 4193:
		case 4194:
		case 4195:
		case 4196:
		case 4198:
		case 4199:
		case 4200:
			return true;
		default:
			return false;
		}
	}

	public override object Clone()
	{
		return this;
	}
}
