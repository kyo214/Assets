using System;
using System.Drawing;
using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using NPOI.Util;

namespace NPOI.DDF;

public class EscherPictBlip : EscherBlipRecord
{
	private static POILogger log = POILogFactory.GetLogger(typeof(EscherPictBlip));

	public const short RECORD_ID_EMF = -4070;

	public const short RECORD_ID_WMF = -4069;

	public const short RECORD_ID_PICT = -4068;

	private const int HEADER_SIZE = 8;

	private byte[] field_1_UID = new byte[16];

	private int field_2_cb;

	private int field_3_rcBounds_x1;

	private int field_3_rcBounds_y1;

	private int field_3_rcBounds_x2;

	private int field_3_rcBounds_y2;

	private int field_4_ptSize_w;

	private int field_4_ptSize_h;

	private int field_5_cbSave;

	private byte field_6_fCompression;

	private byte field_7_fFilter;

	private byte[] raw_pictureData;

	public override int RecordSize => 58 + raw_pictureData.Length;

	public byte[] UID
	{
		get
		{
			return field_1_UID;
		}
		set
		{
			if (value == null || value.Length != 16)
			{
				throw new ArgumentException("uid must be byte[16]");
			}
			Array.Copy(value, 0, field_1_UID, 0, field_1_UID.Length);
		}
	}

	public int UncompressedSize
	{
		get
		{
			return field_2_cb;
		}
		set
		{
			field_2_cb = value;
		}
	}

	public Rectangle Bounds
	{
		get
		{
			return new Rectangle(field_3_rcBounds_x1, field_3_rcBounds_y1, field_3_rcBounds_x2 - field_3_rcBounds_x1, field_3_rcBounds_y2 - field_3_rcBounds_y1);
		}
		set
		{
			field_3_rcBounds_x1 = value.X;
			field_3_rcBounds_y1 = value.Y;
			field_3_rcBounds_x2 = value.X + value.Width;
			field_3_rcBounds_y2 = value.Y + value.Height;
		}
	}

	public Size SizeEMU
	{
		get
		{
			return new Size(field_4_ptSize_w, field_4_ptSize_h);
		}
		set
		{
			field_4_ptSize_w = value.Width;
			field_4_ptSize_h = value.Height;
		}
	}

	public int CompressedSize
	{
		get
		{
			return field_5_cbSave;
		}
		set
		{
			field_5_cbSave = value;
		}
	}

	public bool IsCompressed
	{
		get
		{
			return field_6_fCompression == 0;
		}
		set
		{
			field_6_fCompression = (byte)((!value) ? 254 : 0);
		}
	}

	public override int FillFields(byte[] data, int offset, IEscherRecordFactory recordFactory)
	{
		int num = ReadHeader(data, offset);
		int num2 = offset + 8;
		Array.Copy(data, num2, field_1_UID, 0, 16);
		num2 += 16;
		field_2_cb = LittleEndian.GetInt(data, num2);
		num2 += 4;
		field_3_rcBounds_x1 = LittleEndian.GetInt(data, num2);
		num2 += 4;
		field_3_rcBounds_y1 = LittleEndian.GetInt(data, num2);
		num2 += 4;
		field_3_rcBounds_x2 = LittleEndian.GetInt(data, num2);
		num2 += 4;
		field_3_rcBounds_y2 = LittleEndian.GetInt(data, num2);
		num2 += 4;
		field_4_ptSize_w = LittleEndian.GetInt(data, num2);
		num2 += 4;
		field_4_ptSize_h = LittleEndian.GetInt(data, num2);
		num2 += 4;
		field_5_cbSave = LittleEndian.GetInt(data, num2);
		num2 += 4;
		field_6_fCompression = data[num2];
		num2++;
		field_7_fFilter = data[num2];
		num2++;
		raw_pictureData = new byte[field_5_cbSave];
		Array.Copy(data, num2, raw_pictureData, 0, field_5_cbSave);
		if (field_6_fCompression == 0)
		{
			field_pictureData = InflatePictureData(raw_pictureData);
		}
		else
		{
			field_pictureData = raw_pictureData;
		}
		return num + 8;
	}

	public override int Serialize(int offset, byte[] data, EscherSerializationListener listener)
	{
		listener.BeforeRecordSerialize(offset, RecordId, this);
		int num = offset;
		LittleEndian.PutShort(data, num, Options);
		num += 2;
		LittleEndian.PutShort(data, num, RecordId);
		num += 2;
		LittleEndian.PutInt(data, 0, RecordSize - 8);
		num += 4;
		Array.Copy(field_1_UID, 0, data, num, 16);
		num += 16;
		LittleEndian.PutInt(data, num, field_2_cb);
		num += 4;
		LittleEndian.PutInt(data, num, field_3_rcBounds_x1);
		num += 4;
		LittleEndian.PutInt(data, num, field_3_rcBounds_y1);
		num += 4;
		LittleEndian.PutInt(data, num, field_3_rcBounds_x2);
		num += 4;
		LittleEndian.PutInt(data, num, field_3_rcBounds_y2);
		num += 4;
		LittleEndian.PutInt(data, num, field_4_ptSize_w);
		num += 4;
		LittleEndian.PutInt(data, num, field_4_ptSize_h);
		num += 4;
		LittleEndian.PutInt(data, num, field_5_cbSave);
		num += 4;
		data[num] = field_6_fCompression;
		num++;
		data[num] = field_7_fFilter;
		num++;
		Array.Copy(raw_pictureData, 0, data, num, raw_pictureData.Length);
		listener.AfterRecordSerialize(offset + RecordSize, RecordId, RecordSize, this);
		return 25 + raw_pictureData.Length;
	}

	private static byte[] InflatePictureData(byte[] data)
	{
		using MemoryStream memoryStream = new MemoryStream();
		try
		{
			using MemoryStream baseInputStream = new MemoryStream(data);
			Inflater inf = new Inflater(noHeader: false);
			using InflaterInputStream inflaterInputStream = new InflaterInputStream(baseInputStream, inf);
			byte[] array = new byte[4096];
			int count;
			while ((count = inflaterInputStream.Read(array, 0, array.Length)) > 0)
			{
				memoryStream.Write(array, 0, count);
			}
			return memoryStream.ToArray();
		}
		catch (IOException exception)
		{
			log.Log(3, "Possibly corrupt compression or non-compressed data", exception);
			return data;
		}
	}

	public override string ToString()
	{
		string newLine = Environment.NewLine;
		using MemoryStream memoryStream = new MemoryStream();
		string text;
		try
		{
			HexDump.Dump(field_pictureData, 0L, memoryStream, 0);
			text = memoryStream.ToString();
		}
		catch (Exception ex)
		{
			text = ex.ToString();
		}
		return GetType().Name + ":" + newLine + "  RecordId: 0x" + HexDump.ToHex(RecordId) + newLine + "  Version: 0x" + HexDump.ToHex(Version) + "\n  Instance: 0x" + HexDump.ToHex(Instance) + "\n  UID: 0x" + HexDump.ToHex(field_1_UID) + newLine + "  Uncompressed Size: " + HexDump.ToHex(field_2_cb) + newLine + "  Bounds: " + Bounds.ToString() + newLine + "  Size in EMU: " + SizeEMU.ToString() + newLine + "  Compressed Size: " + HexDump.ToHex(field_5_cbSave) + newLine + "  Compression: " + HexDump.ToHex(field_6_fCompression) + newLine + "  Filter: " + HexDump.ToHex(field_7_fFilter) + newLine + "  Extra Data:" + newLine + text;
	}

	public override string ToXml(string tab)
	{
		string value = "";
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(tab).Append(FormatXmlRecordHeader(GetType().Name, HexDump.ToHex(RecordId), HexDump.ToHex(Version), HexDump.ToHex(Instance))).Append(tab)
			.Append("\t")
			.Append("<UID>0x")
			.Append(HexDump.ToHex(field_1_UID))
			.Append("</UID>\n")
			.Append(tab)
			.Append("\t")
			.Append("<UncompressedSize>0x")
			.Append(HexDump.ToHex(field_2_cb))
			.Append("</UncompressedSize>\n")
			.Append(tab)
			.Append("\t")
			.Append("<Bounds>")
			.Append(Bounds)
			.Append("</Bounds>\n")
			.Append(tab)
			.Append("\t")
			.Append("<SizeInEMU>")
			.Append(SizeEMU)
			.Append("</SizeInEMU>\n")
			.Append(tab)
			.Append("\t")
			.Append("<CompressedSize>0x")
			.Append(HexDump.ToHex(field_5_cbSave))
			.Append("</CompressedSize>\n")
			.Append(tab)
			.Append("\t")
			.Append("<Compression>0x")
			.Append(HexDump.ToHex(field_6_fCompression))
			.Append("</Compression>\n")
			.Append(tab)
			.Append("\t")
			.Append("<Filter>0x")
			.Append(HexDump.ToHex(field_7_fFilter))
			.Append("</Filter>\n")
			.Append(tab)
			.Append("\t")
			.Append("<ExtraData>")
			.Append(value)
			.Append("</ExtraData>\n");
		stringBuilder.Append(tab).Append("</").Append(GetType().Name)
			.Append(">\n");
		return stringBuilder.ToString();
	}
}
