using System.Text;
using NPOI.Util;

namespace NPOI.POIFS.Crypt.Standard;

public class StandardEncryptionHeader : EncryptionHeader, EncryptionRecord
{
	internal StandardEncryptionHeader(ILittleEndianInput is1)
	{
		base.Flags = is1.ReadInt();
		base.SizeExtra = is1.ReadInt();
		base.CipherAlgorithm = CipherAlgorithm.FromEcmaId(is1.ReadInt());
		base.HashAlgorithm = HashAlgorithm.FromEcmaId(is1.ReadInt());
		int num = is1.ReadInt();
		if (num == 0)
		{
			num = 40;
		}
		base.KeySize = num;
		base.BlockSize = num;
		base.CipherProvider = CipherProvider.FromEcmaId(is1.ReadInt());
		is1.ReadLong();
		((ByteArrayInputStream)is1).Mark(5);
		int num2 = is1.ReadInt();
		((ByteArrayInputStream)is1).Reset();
		if (num2 == 16)
		{
			base.CspName = "";
		}
		else
		{
			StringBuilder stringBuilder = new StringBuilder();
			while (true)
			{
				char c = (char)is1.ReadShort();
				if (c == '\0')
				{
					break;
				}
				stringBuilder.Append(c);
			}
			base.CspName = stringBuilder.ToString();
		}
		base.ChainingMode = ChainingMode.ecb;
		base.KeySalt = null;
	}

	protected internal StandardEncryptionHeader(CipherAlgorithm cipherAlgorithm, HashAlgorithm hashAlgorithm, int keyBits, int blockSize, ChainingMode chainingMode)
	{
		base.CipherAlgorithm = cipherAlgorithm;
		base.HashAlgorithm = hashAlgorithm;
		base.KeySize = keyBits;
		base.BlockSize = blockSize;
		base.CipherProvider = cipherAlgorithm.provider;
		base.Flags = EncryptionInfo.flagCryptoAPI.SetBoolean(0, flag: true) | EncryptionInfo.flagAES.SetBoolean(0, cipherAlgorithm.provider == CipherProvider.aes);
	}

	public void Write(LittleEndianByteArrayOutputStream bos)
	{
		int writeIndex = bos.WriteIndex;
		ILittleEndianOutput littleEndianOutput = bos.CreateDelayedOutput(4);
		bos.WriteInt(base.Flags);
		bos.WriteInt(0);
		bos.WriteInt(base.CipherAlgorithm.ecmaId);
		bos.WriteInt(base.HashAlgorithm.ecmaId);
		bos.WriteInt(base.KeySize);
		bos.WriteInt(base.CipherProvider.ecmaId);
		bos.WriteInt(0);
		bos.WriteInt(0);
		string text = base.CspName;
		if (text == null)
		{
			text = base.CipherProvider.cipherProviderName;
		}
		bos.Write(StringUtil.GetToUnicodeLE(text));
		bos.WriteShort(0);
		int v = bos.WriteIndex - writeIndex - 4;
		littleEndianOutput.WriteInt(v);
	}
}
