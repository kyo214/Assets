using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace NPOI.OpenXml4Net.Util;

public class ZipInputStreamZipEntrySource : ZipEntrySource
{
	internal class EntryEnumerator : IEnumerator
	{
		private List<FakeZipEntry>.Enumerator iterator;

		public object Current => iterator.Current;

		internal EntryEnumerator(List<FakeZipEntry> zipEntries)
		{
			iterator = zipEntries.GetEnumerator();
		}

		public bool MoveNext()
		{
			return iterator.MoveNext();
		}

		public void Reset()
		{
			throw new NotImplementedException();
		}
	}

	public class FakeZipEntry : ZipEntry
	{
		private byte[] data;

		public FakeZipEntry(ZipEntry entry, ZipInputStream inp)
			: base(entry.Name)
		{
			MemoryStream memoryStream = new MemoryStream();
			long num = entry.Size;
			if (num != -1)
			{
				if (num >= int.MaxValue)
				{
					throw new IOException("ZIP entry size is too large");
				}
				memoryStream = new MemoryStream((int)num);
			}
			else
			{
				memoryStream = new MemoryStream();
			}
			byte[] array = new byte[4096];
			int num2 = 0;
			while ((num2 = inp.Read(array, 0, array.Length)) > 0)
			{
				memoryStream.Write(array, 0, num2);
			}
			data = memoryStream.ToArray();
		}

		public Stream GetInputStream()
		{
			return new MemoryStream(data);
		}
	}

	private List<FakeZipEntry> zipEntries;

	public IEnumerator Entries => new EntryEnumerator(zipEntries);

	public bool IsClosed => zipEntries == null;

	public ZipInputStreamZipEntrySource(ZipInputStream inp)
	{
		zipEntries = new List<FakeZipEntry>();
		bool flag = true;
		while (flag)
		{
			ZipEntry nextEntry = inp.GetNextEntry();
			if (nextEntry == null)
			{
				flag = false;
				continue;
			}
			FakeZipEntry item = new FakeZipEntry(nextEntry, inp);
			zipEntries.Add(item);
		}
		inp.Close();
	}

	public Stream GetInputStream(ZipEntry zipEntry)
	{
		return ((FakeZipEntry)zipEntry).GetInputStream();
	}

	public void Close()
	{
		zipEntries = null;
	}
}
