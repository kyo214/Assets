namespace NPOI.POIFS.Crypt;

public interface IKey
{
	string GetAlgorithm();

	string GetFormat();

	byte[] GetEncoded();
}
