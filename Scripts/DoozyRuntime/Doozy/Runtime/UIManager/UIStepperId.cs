using System;
using Doozy.Runtime.Common;

namespace Doozy.Runtime.UIManager;

[Serializable]
public class UIStepperId : CategoryNameId
{
	public UIStepperId()
	{
	}

	public UIStepperId(string category, string name, bool custom = false)
		: base(category, name, custom)
	{
	}
}
