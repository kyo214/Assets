namespace BansheeGz.BGDatabase;

public class BGDnaDescriptor
{
	public readonly string DnaName;

	protected BGDnaDescriptor(string dnaName)
	{
		if (string.IsNullOrEmpty(dnaName))
		{
			throw new BGException("Name can not be null");
		}
		DnaName = dnaName;
	}

	public override string ToString()
	{
		return DnaName;
	}
}
