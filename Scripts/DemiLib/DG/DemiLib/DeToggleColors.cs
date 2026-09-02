using System;
using UnityEngine;

namespace DG.DemiLib;

[Serializable]
public class DeToggleColors
{
	public class ColorCombination
	{
		public Color2 offCols;

		public Color2 onCols;

		public ColorCombination(Color2 offCombination, Color2 onCombination)
		{
			offCols = offCombination;
			onCols = onCombination;
		}
	}

	public class Color2
	{
		public Color bg;

		public Color content;
	}

	public DeSkinColor bgOn = new DeSkinColor(new Color(0.3158468f, 0.875f, 0.1351103f, 1f), new Color(0.2183823f, 0.7279412f, 0.09099264f, 1f));

	public DeSkinColor bgOff = new DeSkinColor(0.75f, 0.4f);

	public DeSkinColor bgCritical = new DeSkinColor(new Color(240f / 255f, 0.2388736f, 0.006920422f, 1f), new Color(1f, 0.2482758f, 0f, 1f));

	public DeSkinColor bgYellow = new DeSkinColor(new Color(0.93f, 0.77f, 0.04f));

	public DeSkinColor bgOrange = new DeSkinColor(new Color(0.98f, 0.44f, 0f));

	public DeSkinColor bgBlue = new DeSkinColor(new Color(0f, 0.4f, 0.91f));

	public DeSkinColor bgCyan = new DeSkinColor(new Color(0f, 0.79f, 1f));

	public DeSkinColor bgPurple = new DeSkinColor(new Color(0.67f, 0.17f, 0.87f));

	public DeSkinColor contentOn = new DeSkinColor(new Color(1f, 0.9686275f, 0.6980392f, 1f), new Color(0.8025267f, 1f, 0.4705882f, 1f));

	public DeSkinColor contentOff = new DeSkinColor(new Color(105f / 255f, 0.3360727f, 0.3360727f, 1f), new Color(0.6470588f, 0.5185986f, 0.5185986f, 1f));

	public DeSkinColor contentCritical = new DeSkinColor(new Color(1f, 0.84f, 0.62f), new Color(1f, 0.84f, 0.62f));

	public DeSkinColor contentYellow = new DeSkinColor(new Color(1f, 1f, 0.64f));

	public DeSkinColor contentOrange = new DeSkinColor(new Color(1f, 0.96f, 0.57f));

	public DeSkinColor contentBlue = new DeSkinColor(new Color(0.35f, 0.96f, 0.94f));

	public DeSkinColor contentCyan = new DeSkinColor(new Color(0.62f, 1f, 0.89f));

	public DeSkinColor contentPurple = new DeSkinColor(new Color(1f, 0.81f, 0.98f));

	public ColorCombination GetColors(ToggleColors offType, ToggleColors onType)
	{
		return new ColorCombination(GetColor2(offType), GetColor2(onType));
	}

	private Color2 GetColor2(ToggleColors type)
	{
		Color2 color = new Color2();
		switch (type)
		{
		case ToggleColors.DefaultOn:
			color.bg = bgOn;
			color.content = contentOn;
			break;
		case ToggleColors.DefaultOff:
			color.bg = bgOff;
			color.content = contentOff;
			break;
		case ToggleColors.Critical:
			color.bg = bgCritical;
			color.content = contentCritical;
			break;
		case ToggleColors.Yellow:
			color.bg = bgYellow;
			color.content = contentYellow;
			break;
		case ToggleColors.Orange:
			color.bg = bgOrange;
			color.content = contentOrange;
			break;
		case ToggleColors.Blue:
			color.bg = bgBlue;
			color.content = contentBlue;
			break;
		case ToggleColors.Cyan:
			color.bg = bgCyan;
			color.content = contentCyan;
			break;
		case ToggleColors.Purple:
			color.bg = bgPurple;
			color.content = contentPurple;
			break;
		}
		return color;
	}
}
