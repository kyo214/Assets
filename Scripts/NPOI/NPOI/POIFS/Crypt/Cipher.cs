using System;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace NPOI.POIFS.Crypt;

public class Cipher
{
	public static int DECRYPT_MODE = 2;

	public static int ENCRYPT_MODE = 1;

	public static int WRAP_MODE = 3;

	public static int UNWRAP_MODE = 4;

	public static int PUBLIC_KEY = 1;

	public static int PRIVATE_KEY = 2;

	public static int SECRET_KEY = 3;

	protected IBufferedCipher cipherImpl;

	public byte[] DoFinal(byte[] block)
	{
		return cipherImpl.DoFinal(block);
	}

	public int DoFinal(byte[] input, int inputOffset, int inputLen, byte[] output)
	{
		return cipherImpl.DoFinal(input, inputOffset, inputLen, output, 0);
	}

	public static Cipher GetInstance(string transformation)
	{
		return new Cipher
		{
			cipherImpl = CipherUtilities.GetCipher(transformation)
		};
	}

	public static Cipher GetInstance(string transformation, string provider)
	{
		return GetInstance(transformation);
	}

	public void Init(int cipherMode, IKey key, AlgorithmParameterSpec aps)
	{
		ICipherParameters parameters;
		if (aps is RC2ParameterSpec)
		{
			parameters = new RC2Parameters(key.GetEncoded(), (aps as RC2ParameterSpec).GetEffectiveKeyBits());
		}
		else
		{
			if (!(aps is IvParameterSpec))
			{
				throw new NotImplementedException();
			}
			parameters = new KeyParameter(key.GetEncoded());
			parameters = new ParametersWithIV(parameters, (aps as IvParameterSpec).GetIV());
		}
		cipherImpl.Init(cipherMode == ENCRYPT_MODE, parameters);
	}

	public void Init(int cipherMode, IKey key)
	{
		ICipherParameters parameters = new RC2Parameters(key.GetEncoded());
		cipherImpl.Init(cipherMode == ENCRYPT_MODE, parameters);
	}

	public void Init(int cipherMode, ICipherParameters cipherParameters)
	{
		cipherImpl.Init(cipherMode == ENCRYPT_MODE, cipherParameters);
	}

	public static int GetMaxAllowedKeyLength(string jceId)
	{
		return jceId switch
		{
			"RC2" => 128, 
			"RC4" => 128, 
			"DES" => 64, 
			"AES" => 128, 
			"DESede" => int.MaxValue, 
			"RSA" => int.MaxValue, 
			_ => throw new NotImplementedException(), 
		};
	}

	public void Update(byte[] input, int inputOffset, int inputLen, byte[] output)
	{
		if (input == null || inputOffset < 0 || inputLen > input.Length - inputOffset || inputLen < 0)
		{
			throw new ArgumentException("Bad arguments");
		}
		cipherImpl.ProcessBytes(input, inputOffset, inputLen, output, 0);
	}

	public void Update(byte[] input, int inputOffset, int inputLen, byte[] output, int outputOffset)
	{
		if (input == null || inputOffset < 0 || inputLen > input.Length - inputOffset || inputLen < 0 || outputOffset < 0)
		{
			throw new ArgumentException("Bad arguments");
		}
		cipherImpl.ProcessBytes(input, inputOffset, inputLen, output, outputOffset);
	}

	public byte[] DoFinal()
	{
		return cipherImpl.DoFinal();
	}

	public byte[] Update(byte[] ibuffer, int inOff, int length)
	{
		return cipherImpl.ProcessBytes(ibuffer, inOff, length);
	}
}
