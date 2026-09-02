namespace NPOI.XWPF.UserModel;

public class TextSegment
{
	private PositionInParagraph beginPos;

	private PositionInParagraph endPos;

	public PositionInParagraph BeginPos
	{
		get
		{
			return beginPos;
		}
		set
		{
			beginPos = value;
		}
	}

	public PositionInParagraph EndPos => endPos;

	public int BeginRun
	{
		get
		{
			return beginPos.Run;
		}
		set
		{
			beginPos.Run = value;
		}
	}

	public int BeginText
	{
		get
		{
			return beginPos.Text;
		}
		set
		{
			beginPos.Text = value;
		}
	}

	public int BeginChar
	{
		get
		{
			return beginPos.Char;
		}
		set
		{
			beginPos.Char = value;
		}
	}

	public int EndRun
	{
		get
		{
			return endPos.Run;
		}
		set
		{
			endPos.Run = value;
		}
	}

	public int EndText
	{
		get
		{
			return endPos.Text;
		}
		set
		{
			endPos.Text = value;
		}
	}

	public int EndChar
	{
		get
		{
			return endPos.Char;
		}
		set
		{
			endPos.Char = value;
		}
	}

	public TextSegment()
	{
		beginPos = new PositionInParagraph();
		endPos = new PositionInParagraph();
	}

	public TextSegment(int beginRun, int endRun, int beginText, int endText, int beginChar, int endChar)
	{
		PositionInParagraph positionInParagraph = new PositionInParagraph(beginRun, beginText, beginChar);
		PositionInParagraph positionInParagraph2 = new PositionInParagraph(endRun, endText, endChar);
		beginPos = positionInParagraph;
		endPos = positionInParagraph2;
	}

	public TextSegment(PositionInParagraph beginPos, PositionInParagraph endPos)
	{
		this.beginPos = beginPos;
		this.endPos = endPos;
	}
}
