namespace NPOI.SS.Util;

public class IEEEDouble
{
	private const long EXPONENT_MASK = 9218868437227405312L;

	private const int EXPONENT_SHIFT = 52;

	public const long FRAC_MASK = 4503599627370495L;

	public const int EXPONENT_BIAS = 1023;

	public const long FRAC_ASSUMED_HIGH_BIT = 4503599627370496L;

	public const int BIASED_EXPONENT_SPECIAL_VALUE = 2047;

	public static int GetBiasedExponent(long rawBits)
	{
		return (int)((rawBits & 0x7FF0000000000000L) >> 52);
	}
}
