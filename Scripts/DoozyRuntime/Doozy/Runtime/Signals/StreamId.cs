using System;
using Doozy.Runtime.Common;

namespace Doozy.Runtime.Signals;

[Serializable]
public class StreamId : CategoryNameId
{
	public StreamId()
	{
	}

	public StreamId(string category, string name, bool custom = false)
		: base(category, name, custom)
	{
	}

	public void SetStream(SignalStream stream)
	{
		if (stream != null)
		{
			Category = stream.category;
			Name = stream.name;
			Custom = true;
		}
	}
}
