using System;
using System.Collections;

namespace NPOI.POIFS.Dev;

public interface POIFSViewable
{
	bool PreferArray { get; }

	string ShortDescription { get; }

	Array ViewableArray { get; }

	IEnumerator ViewableIterator { get; }
}
