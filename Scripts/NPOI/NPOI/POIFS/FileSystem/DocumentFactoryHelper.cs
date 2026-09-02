using System;
using System.IO;
using NPOI.POIFS.Common;
using NPOI.POIFS.Crypt;
using NPOI.Util;

namespace NPOI.POIFS.FileSystem;

public class DocumentFactoryHelper
{
	private class FilterInputStream1 : FilterInputStream
	{
		private NPOIFSFileSystem fs;

		public FilterInputStream1(InputStream input, NPOIFSFileSystem fs)
			: base(input)
		{
			this.fs = fs;
		}

		public override void Close()
		{
			fs.Close();
			base.Close();
		}
	}

	public static InputStream GetDecryptedStream(NPOIFSFileSystem fs, string password)
	{
		Decryptor instance = Decryptor.GetInstance(new EncryptionInfo(fs));
		try
		{
			bool flag = false;
			if (password != null && instance.VerifyPassword(password))
			{
				flag = true;
			}
			if (!flag && instance.VerifyPassword(Decryptor.DEFAULT_PASSWORD))
			{
				flag = true;
			}
			if (flag)
			{
				return new FilterInputStream1(instance.GetDataStream(fs.Root), fs);
			}
			if (password != null)
			{
				throw new EncryptedDocumentException("Password incorrect");
			}
			throw new EncryptedDocumentException("The supplied spreadsheet is protected, but no password was supplied");
		}
		catch (Exception innerException)
		{
			throw new IOException("password does not match", innerException);
		}
	}

	public static bool HasOOXMLHeader(Stream inp)
	{
		byte[] array = new byte[4];
		int num = IOUtils.ReadFully(inp, array);
		if (inp is PushbackStream)
		{
			((PushbackStream)inp).Position -= 4L;
		}
		else
		{
			inp.Position = 0L;
		}
		if (num == 4 && array[0] == POIFSConstants.OOXML_FILE_HEADER[0] && array[1] == POIFSConstants.OOXML_FILE_HEADER[1] && array[2] == POIFSConstants.OOXML_FILE_HEADER[2])
		{
			return array[3] == POIFSConstants.OOXML_FILE_HEADER[3];
		}
		return false;
	}
}
