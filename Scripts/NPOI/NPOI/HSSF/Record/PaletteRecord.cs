using System.Collections.Generic;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class PaletteRecord : StandardRecord
{
	public const short sid = 146;

	public const byte STANDARD_PALETTE_SIZE = 56;

	public const short FIRST_COLOR_INDEX = 8;

	private List<PColor> field_2_colors;

	public short NumColors => (short)field_2_colors.Count;

	protected override int DataSize => 2 + field_2_colors.Count * 4;

	public override short Sid => 146;

	public PaletteRecord()
	{
		PColor[] array = CreateDefaultPalette();
		field_2_colors = new List<PColor>(array.Length);
		for (int i = 0; i < array.Length; i++)
		{
			field_2_colors.Add(array[i]);
		}
	}

	public PaletteRecord(RecordInputStream in1)
	{
		short num = in1.ReadShort();
		field_2_colors = new List<PColor>(num);
		for (int i = 0; i < num; i++)
		{
			field_2_colors.Add(new PColor(in1));
		}
	}

	public void ClearColors()
	{
		field_2_colors.Clear();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[PALETTE]\n");
		stringBuilder.Append("  numcolors     = ").Append(field_2_colors.Count).Append('\n');
		for (int i = 0; i < field_2_colors.Count; i++)
		{
			PColor pColor = field_2_colors[i];
			stringBuilder.Append("* colornum      = ").Append(i).Append('\n');
			stringBuilder.Append(pColor.ToString());
			stringBuilder.Append("/*colornum      = ").Append(i).Append('\n');
		}
		stringBuilder.Append("[/PALETTE]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(field_2_colors.Count);
		for (int i = 0; i < field_2_colors.Count; i++)
		{
			field_2_colors[i].Serialize(out1);
		}
	}

	public byte[] GetColor(short byteIndex)
	{
		int num = byteIndex - 8;
		if (num < 0 || num >= field_2_colors.Count)
		{
			return null;
		}
		PColor pColor = field_2_colors[num];
		return new byte[3] { pColor._red, pColor._green, pColor._blue };
	}

	public void SetColor(short byteIndex, byte red, byte green, byte blue)
	{
		int num = byteIndex - 8;
		if (num >= 0 && num < 56)
		{
			while (field_2_colors.Count <= num)
			{
				field_2_colors.Add(new PColor(0, 0, 0));
			}
			PColor value = new PColor(red, green, blue);
			field_2_colors[num] = value;
		}
	}

	private static PColor[] CreateDefaultPalette()
	{
		return new PColor[56]
		{
			pc(0, 0, 0),
			pc(255, 255, 255),
			pc(255, 0, 0),
			pc(0, 255, 0),
			pc(0, 0, 255),
			pc(255, 255, 0),
			pc(255, 0, 255),
			pc(0, 255, 255),
			pc(128, 0, 0),
			pc(0, 128, 0),
			pc(0, 0, 128),
			pc(128, 128, 0),
			pc(128, 0, 128),
			pc(0, 128, 128),
			pc(192, 192, 192),
			pc(128, 128, 128),
			pc(153, 153, 255),
			pc(153, 51, 102),
			pc(255, 255, 204),
			pc(204, 255, 255),
			pc(102, 0, 102),
			pc(255, 128, 128),
			pc(0, 102, 204),
			pc(204, 204, 255),
			pc(0, 0, 128),
			pc(255, 0, 255),
			pc(255, 255, 0),
			pc(0, 255, 255),
			pc(128, 0, 128),
			pc(128, 0, 0),
			pc(0, 128, 128),
			pc(0, 0, 255),
			pc(0, 204, 255),
			pc(204, 255, 255),
			pc(204, 255, 204),
			pc(255, 255, 153),
			pc(153, 204, 255),
			pc(255, 153, 204),
			pc(204, 153, 255),
			pc(255, 204, 153),
			pc(51, 102, 255),
			pc(51, 204, 204),
			pc(153, 204, 0),
			pc(255, 204, 0),
			pc(255, 153, 0),
			pc(255, 102, 0),
			pc(102, 102, 153),
			pc(150, 150, 150),
			pc(0, 51, 102),
			pc(51, 153, 102),
			pc(0, 51, 0),
			pc(51, 51, 0),
			pc(153, 51, 0),
			pc(153, 51, 102),
			pc(51, 51, 153),
			pc(51, 51, 51)
		};
	}

	private static PColor pc(int r, int g, int b)
	{
		return new PColor(r, g, b);
	}
}
