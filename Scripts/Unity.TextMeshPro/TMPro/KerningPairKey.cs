namespace TMPro;

public struct KerningPairKey(uint ascii_left, uint ascii_right)
{
	public uint ascii_Left = ascii_left;

	public uint ascii_Right = ascii_right;

	public uint key = (ascii_right << 16) + ascii_left;
}
