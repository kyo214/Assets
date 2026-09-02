namespace NPOI.SS.UserModel;

public class FontScheme
{
	public static readonly FontScheme NONE = new FontScheme(1);

	public static readonly FontScheme MAJOR = new FontScheme(2);

	public static readonly FontScheme MINOR = new FontScheme(3);

	private int value;

	public int Value => value;

	private FontScheme(int val)
	{
		value = val;
	}

	public static FontScheme ValueOf(int value)
	{
		return value switch
		{
			1 => NONE, 
			2 => MAJOR, 
			3 => MINOR, 
			_ => NONE, 
		};
	}
}
