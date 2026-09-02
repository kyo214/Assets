using System;
using System.IO;
using NPOI.Util;

namespace NPOI.POIFS.Crypt.Standard;

internal class CipherOutputStream : ByteArrayOutputStream
{
	private byte[] ibuffer = new byte[1];

	private byte[] obuffer;

	private bool closed;

	private FileStream output;

	private Cipher cipher;

	public CipherOutputStream(FileStream rawStream, Cipher cipher)
	{
		output = rawStream;
		this.cipher = cipher;
	}

	protected CipherOutputStream(FileStream rawStream)
	{
		output = rawStream;
		cipher = new NullCipher();
	}

	public override void Write(int paramInt)
	{
		ibuffer[0] = (byte)paramInt;
		obuffer = cipher.Update(ibuffer, 0, 1);
		if (obuffer != null)
		{
			output.Write(obuffer, 0, obuffer.Length);
			obuffer = null;
		}
	}

	public override void Write(byte[] b)
	{
		Write(b, 0, b.Length);
	}

	public override void Write(byte[] b, int off, int len)
	{
		obuffer = cipher.Update(b, off, len);
		if (obuffer != null)
		{
			output.Write(obuffer, 0, obuffer.Length);
			output.Flush();
			obuffer = null;
		}
	}

	public override void Flush()
	{
		if (obuffer != null)
		{
			output.Write(obuffer, 0, obuffer.Length);
			obuffer = null;
		}
		output.Flush();
	}

	public override void Close()
	{
		if (!closed)
		{
			closed = true;
			try
			{
				obuffer = cipher.DoFinal();
			}
			catch (Exception)
			{
				obuffer = null;
			}
			try
			{
				Flush();
			}
			catch (IOException)
			{
			}
			Close();
		}
	}
}
