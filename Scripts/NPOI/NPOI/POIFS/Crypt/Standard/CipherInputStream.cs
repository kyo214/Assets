using System;
using System.IO;
using NPOI.Util;

namespace NPOI.POIFS.Crypt.Standard;

public class CipherInputStream : ByteArrayInputStream
{
	private Cipher cipher;

	private ByteArrayInputStream input;

	private byte[] ibuffer = new byte[63];

	private bool done;

	private byte[] obuffer;

	private int ostart;

	private int ofinish;

	private bool closed;

	public CipherInputStream(ByteArrayInputStream paramInputStream, Cipher paramCipher)
	{
		input = paramInputStream;
		cipher = paramCipher;
	}

	protected CipherInputStream(ByteArrayInputStream paramInputStream)
	{
		input = paramInputStream;
		cipher = new NullCipher();
	}

	private int getMoreData()
	{
		if (done)
		{
			return -1;
		}
		int num = input.Read(ibuffer, 0, ibuffer.Length);
		if (num == -1)
		{
			done = true;
			try
			{
				obuffer = cipher.DoFinal();
			}
			catch (Exception ex)
			{
				obuffer = null;
				throw new IOException(ex.Message);
			}
			if (obuffer == null)
			{
				return -1;
			}
			ostart = 0;
			ofinish = obuffer.Length;
			return ofinish;
		}
		try
		{
			obuffer = cipher.Update(ibuffer, 0, num);
		}
		catch (Exception ex2)
		{
			obuffer = null;
			throw ex2;
		}
		ostart = 0;
		if (obuffer == null)
		{
			ofinish = 0;
		}
		else
		{
			ofinish = obuffer.Length;
		}
		return ofinish;
	}

	public override int Read()
	{
		if (ostart >= ofinish)
		{
			int num = 0;
			while (true)
			{
				switch (num)
				{
				case 0:
					goto IL_0012;
				case -1:
					return -1;
				}
				break;
				IL_0012:
				num = getMoreData();
			}
		}
		return obuffer[ostart++] & 0xFF;
	}

	public new int Read(byte[] b)
	{
		return Read(b, 0, b.Length);
	}

	public override int Read(byte[] b, int off, int len)
	{
		int num;
		if (ostart >= ofinish)
		{
			num = 0;
			while (true)
			{
				switch (num)
				{
				case 0:
					goto IL_0012;
				case -1:
					return -1;
				}
				break;
				IL_0012:
				num = getMoreData();
			}
		}
		if (len <= 0)
		{
			return 0;
		}
		num = ofinish - ostart;
		if (len < num)
		{
			num = len;
		}
		if (b != null)
		{
			Array.Copy(obuffer, ostart, b, off, num);
		}
		ostart += num;
		return num;
	}

	public new long Skip(long paramLong)
	{
		int num = ofinish - ostart;
		if (paramLong > num)
		{
			paramLong = num;
		}
		if (paramLong < 0)
		{
			return 0L;
		}
		ostart = (int)(ostart + paramLong);
		return paramLong;
	}

	public override int Available()
	{
		return ofinish - ostart;
	}

	public override void Close()
	{
		if (closed)
		{
			return;
		}
		closed = true;
		input.Close();
		if (!done)
		{
			try
			{
				cipher.DoFinal();
			}
			catch (Exception)
			{
			}
		}
		ostart = 0;
		ofinish = 0;
	}

	public new bool MarkSupported()
	{
		return false;
	}
}
