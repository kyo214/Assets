using System;
using System.Collections.Generic;
using System.Text;

namespace NPOI.OpenXml4Net.OPC;

public class PackagePartCollection : SortedList<PackagePartName, PackagePart>
{
	private static long serialVersionUID = 2515031135957635515L;

	private List<string> registerPartNameStr = new List<string>();

	public PackagePart Put(PackagePartName partName, PackagePart part)
	{
		string[] array = partName.URI.OriginalString.Split(new char[1] { PackagingUriHelper.FORWARD_SLASH_CHAR });
		StringBuilder stringBuilder = new StringBuilder();
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (!text.Equals(""))
			{
				stringBuilder.Append(PackagingUriHelper.FORWARD_SLASH_CHAR);
			}
			stringBuilder.Append(text);
			if (registerPartNameStr.Contains(stringBuilder.ToString()))
			{
				throw new InvalidOperationException("You can't add a part with a part name derived from another part ! [M1.11]");
			}
		}
		registerPartNameStr.Add(partName.Name);
		return base[partName] = part;
	}

	public new void Remove(PackagePartName key)
	{
		registerPartNameStr.Remove(key.Name);
		base.Remove(key);
	}
}
