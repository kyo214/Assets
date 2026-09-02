namespace BansheeGz.BGDatabase;

public abstract class CsvFileCommon
{
	protected char[] SpecialChars = new char[4] { ',', '"', '\r', '\n' };

	private const int DelimiterIndex = 0;

	private const int QuoteIndex = 1;

	public char Delimiter
	{
		get
		{
			return SpecialChars[0];
		}
		set
		{
			SpecialChars[0] = value;
		}
	}

	public char Quote
	{
		get
		{
			return SpecialChars[1];
		}
		set
		{
			SpecialChars[1] = value;
		}
	}
}
