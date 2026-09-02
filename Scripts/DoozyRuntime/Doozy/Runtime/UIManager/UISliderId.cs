using System;
using Doozy.Runtime.Common;

namespace Doozy.Runtime.UIManager;

[Serializable]
public class UISliderId : CategoryNameId
{
	public UISliderId()
	{
	}

	public UISliderId(string category, string name, bool custom = false)
		: base(category, name, custom)
	{
	}
}
