using System;
using System.Collections.Generic;
using System.Text;
using NPOI.Util;

namespace NPOI.DDF;

public class EscherDggRecord : EscherRecord
{
	public class FileIdCluster
	{
		private int field_1_drawingGroupId;

		private int field_2_numShapeIdsUsed;

		public int DrawingGroupId => field_1_drawingGroupId;

		public int NumShapeIdsUsed => field_2_numShapeIdsUsed;

		public FileIdCluster(int drawingGroupId, int numShapeIdsUsed)
		{
			field_1_drawingGroupId = drawingGroupId;
			field_2_numShapeIdsUsed = numShapeIdsUsed;
		}

		public void IncrementShapeId()
		{
			field_2_numShapeIdsUsed++;
		}
	}

	private class EscherDggRecordComparer : IComparer<FileIdCluster>
	{
		public int Compare(FileIdCluster f1, FileIdCluster f2)
		{
			if (f1.DrawingGroupId == f2.DrawingGroupId)
			{
				return 0;
			}
			if (f1.DrawingGroupId < f2.DrawingGroupId)
			{
				return -1;
			}
			return 1;
		}
	}

	public const short RECORD_ID = -4090;

	public const string RECORD_DESCRIPTION = "MsofbtDgg";

	private int field_1_shapeIdMax;

	private int field_3_numShapesSaved;

	private int field_4_drawingsSaved;

	private FileIdCluster[] field_5_fileIdClusters;

	private int maxDgId;

	public override int RecordSize => 24 + 8 * field_5_fileIdClusters.Length;

	public override short RecordId => -4090;

	public override string RecordName => "Dgg";

	public int ShapeIdMax
	{
		get
		{
			return field_1_shapeIdMax;
		}
		set
		{
			field_1_shapeIdMax = value;
		}
	}

	public int NumIdClusters => field_5_fileIdClusters.Length + 1;

	public int NumShapesSaved
	{
		get
		{
			return field_3_numShapesSaved;
		}
		set
		{
			field_3_numShapesSaved = value;
		}
	}

	public int DrawingsSaved
	{
		get
		{
			return field_4_drawingsSaved;
		}
		set
		{
			field_4_drawingsSaved = value;
		}
	}

	public int MaxDrawingGroupId
	{
		get
		{
			return maxDgId;
		}
		set
		{
			maxDgId = value;
		}
	}

	public FileIdCluster[] FileIdClusters
	{
		get
		{
			return field_5_fileIdClusters;
		}
		set
		{
			field_5_fileIdClusters = (FileIdCluster[])value.Clone();
		}
	}

	public override int FillFields(byte[] data, int offset, IEscherRecordFactory recordFactory)
	{
		int num = ReadHeader(data, offset);
		int num2 = offset + 8;
		int num3 = 0;
		field_1_shapeIdMax = LittleEndian.GetInt(data, num2 + num3);
		num3 += 4;
		num3 += 4;
		field_3_numShapesSaved = LittleEndian.GetInt(data, num2 + num3);
		num3 += 4;
		field_4_drawingsSaved = LittleEndian.GetInt(data, num2 + num3);
		num3 += 4;
		field_5_fileIdClusters = new FileIdCluster[(num - num3) / 8];
		for (int i = 0; i < field_5_fileIdClusters.Length; i++)
		{
			field_5_fileIdClusters[i] = new FileIdCluster(LittleEndian.GetInt(data, num2 + num3), LittleEndian.GetInt(data, num2 + num3 + 4));
			maxDgId = Math.Max(maxDgId, field_5_fileIdClusters[i].DrawingGroupId);
			num3 += 8;
		}
		num -= num3;
		if (num != 0)
		{
			throw new RecordFormatException("Expecting no remaining data but got " + num + " byte(s).");
		}
		return 8 + num3 + num;
	}

	public override int Serialize(int offset, byte[] data, EscherSerializationListener listener)
	{
		listener.BeforeRecordSerialize(offset, RecordId, this);
		int num = offset;
		LittleEndian.PutShort(data, num, Options);
		num += 2;
		LittleEndian.PutShort(data, num, RecordId);
		num += 2;
		int value = RecordSize - 8;
		LittleEndian.PutInt(data, num, value);
		num += 4;
		LittleEndian.PutInt(data, num, field_1_shapeIdMax);
		num += 4;
		LittleEndian.PutInt(data, num, NumIdClusters);
		num += 4;
		LittleEndian.PutInt(data, num, field_3_numShapesSaved);
		num += 4;
		LittleEndian.PutInt(data, num, field_4_drawingsSaved);
		num += 4;
		for (int i = 0; i < field_5_fileIdClusters.Length; i++)
		{
			LittleEndian.PutInt(data, num, field_5_fileIdClusters[i].DrawingGroupId);
			num += 4;
			LittleEndian.PutInt(data, num, field_5_fileIdClusters[i].NumShapeIdsUsed);
			num += 4;
		}
		listener.AfterRecordSerialize(num, RecordId, RecordSize, this);
		return RecordSize;
	}

	public override string ToString()
	{
		string newLine = Environment.NewLine;
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < field_5_fileIdClusters.Length; i++)
		{
			stringBuilder.Append("  DrawingGroupId").Append(i + 1).Append(": ");
			stringBuilder.Append(field_5_fileIdClusters[i].DrawingGroupId);
			stringBuilder.Append(newLine);
			stringBuilder.Append("  NumShapeIdsUsed").Append(i + 1).Append(": ");
			stringBuilder.Append(field_5_fileIdClusters[i].NumShapeIdsUsed);
			stringBuilder.Append(newLine);
		}
		return GetType().Name + ":" + newLine + "  RecordId: 0x" + HexDump.ToHex((short)(-4090)) + newLine + "  Version: 0x" + HexDump.ToHex(Version) + newLine + "  Instance: 0x" + HexDump.ToHex(Instance) + newLine + "  ShapeIdMax: " + field_1_shapeIdMax + newLine + "  NumIdClusters: " + NumIdClusters + newLine + "  NumShapesSaved: " + field_3_numShapesSaved + newLine + "  DrawingsSaved: " + field_4_drawingsSaved + newLine + stringBuilder.ToString();
	}

	public override string ToXml(string tab)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(tab).Append(FormatXmlRecordHeader(GetType().Name, HexDump.ToHex(RecordId), HexDump.ToHex(Version), HexDump.ToHex(Instance))).Append(tab)
			.Append("\t")
			.Append("<ShapeIdMax>")
			.Append(field_1_shapeIdMax)
			.Append("</ShapeIdMax>\n")
			.Append(tab)
			.Append("\t")
			.Append("<NumIdClusters>")
			.Append(NumIdClusters)
			.Append("</NumIdClusters>\n")
			.Append(tab)
			.Append("\t")
			.Append("<NumShapesSaved>")
			.Append(field_3_numShapesSaved)
			.Append("</NumShapesSaved>\n")
			.Append(tab)
			.Append("\t")
			.Append("<DrawingsSaved>")
			.Append(field_4_drawingsSaved)
			.Append("</DrawingsSaved>\n");
		stringBuilder.Append(tab).Append("</").Append(GetType().Name)
			.Append(">\n");
		return stringBuilder.ToString();
	}

	public void AddCluster(int dgId, int numShapedUsed)
	{
		AddCluster(dgId, numShapedUsed, sort: true);
	}

	public void AddCluster(int dgId, int numShapedUsed, bool sort)
	{
		List<FileIdCluster> list = new List<FileIdCluster>(field_5_fileIdClusters);
		list.Add(new FileIdCluster(dgId, numShapedUsed));
		if (sort)
		{
			InsertionSort(list, new EscherDggRecordComparer());
		}
		maxDgId = Math.Min(maxDgId, dgId);
		field_5_fileIdClusters = list.ToArray();
	}

	public static void InsertionSort<T>(List<T> list, IComparer<T> comparison)
	{
		if (list == null)
		{
			throw new ArgumentNullException("list");
		}
		if (comparison == null)
		{
			throw new ArgumentNullException("comparison");
		}
		int count = list.Count;
		for (int i = 1; i < count; i++)
		{
			T val = list[i];
			int num = i - 1;
			while (num >= 0 && comparison.Compare(list[num], val) > 0)
			{
				list[num + 1] = list[num];
				num--;
			}
			list[num + 1] = val;
		}
	}
}
