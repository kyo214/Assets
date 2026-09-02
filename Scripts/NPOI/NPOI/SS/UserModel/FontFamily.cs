namespace NPOI.SS.UserModel;

public class FontFamily
{
	public static readonly FontFamily NOT_APPLICABLE = new FontFamily(0);

	public static readonly FontFamily ROMAN = new FontFamily(1);

	public static readonly FontFamily SWISS = new FontFamily(2);

	public static readonly FontFamily MODERN = new FontFamily(3);

	public static readonly FontFamily SCRIPT = new FontFamily(4);

	public static readonly FontFamily DECORATIVE = new FontFamily(5);

	private int family;

	public int Value => family;

	private FontFamily(int value)
	{
		family = value;
	}

	public static FontFamily ValueOf(int family)
	{
		return family switch
		{
			0 => NOT_APPLICABLE, 
			1 => ROMAN, 
			2 => SWISS, 
			3 => MODERN, 
			4 => SCRIPT, 
			5 => DECORATIVE, 
			_ => NOT_APPLICABLE, 
		};
	}
}
