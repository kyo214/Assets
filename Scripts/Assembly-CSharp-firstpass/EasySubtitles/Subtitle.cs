namespace EasySubtitles;

public class Subtitle
{
	private static Subtitle _empty;

	public static Subtitle Empty => _empty ?? (_empty = new Subtitle(0, 0f, 0f, 0, 0, 0, 0, string.Empty));

	public int Index { get; }

	public float Start { get; }

	public float End { get; }

	public float Duration { get; }

	public int X1 { get; }

	public int X2 { get; }

	public int Y1 { get; }

	public int Y2 { get; }

	public string Text { get; }

	public Subtitle(int index, float start, float end, int x1, int x2, int y1, int y2, string text)
	{
		Index = index;
		Start = start;
		End = end;
		Duration = end - start;
		X1 = x1;
		X2 = x2;
		Y1 = y1;
		Y2 = y2;
		Text = text;
	}

	public override string ToString()
	{
		return $"Index: {Index}\nText: {Text}\nStart: {Start}\nEnd: {End}\nDuration: {Duration}\nX1: {X1}\nX2: {X2}\nY1: {Y1}\nY2: {Y2}";
	}
}
