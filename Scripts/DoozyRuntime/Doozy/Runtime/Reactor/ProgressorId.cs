using System;
using Doozy.Runtime.Common;

namespace Doozy.Runtime.Reactor;

[Serializable]
public class ProgressorId : CategoryNameId
{
	public ProgressorId()
	{
	}

	public ProgressorId(string category, string name, bool custom = false)
		: base(category, name, custom)
	{
	}
}
