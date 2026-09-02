using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace MoreMountains.Tools;

public abstract class MMSaveLoadManagerEncrypter
{
	protected string _saltText = "SaltTextGoesHere";

	public string Key { get; set; } = "yourDefaultKey";

	protected virtual void Encrypt(Stream inputStream, Stream outputStream, string sKey)
	{
		RijndaelManaged rijndaelManaged = new RijndaelManaged();
		Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(sKey, Encoding.ASCII.GetBytes(_saltText));
		rijndaelManaged.Key = rfc2898DeriveBytes.GetBytes(rijndaelManaged.KeySize / 8);
		rijndaelManaged.IV = rfc2898DeriveBytes.GetBytes(rijndaelManaged.BlockSize / 8);
		new CryptoStream(inputStream, rijndaelManaged.CreateEncryptor(), CryptoStreamMode.Read).CopyTo(outputStream);
	}

	protected virtual void Decrypt(Stream inputStream, Stream outputStream, string sKey)
	{
		RijndaelManaged rijndaelManaged = new RijndaelManaged();
		Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(sKey, Encoding.ASCII.GetBytes(_saltText));
		rijndaelManaged.Key = rfc2898DeriveBytes.GetBytes(rijndaelManaged.KeySize / 8);
		rijndaelManaged.IV = rfc2898DeriveBytes.GetBytes(rijndaelManaged.BlockSize / 8);
		new CryptoStream(inputStream, rijndaelManaged.CreateDecryptor(), CryptoStreamMode.Read).CopyTo(outputStream);
	}
}
