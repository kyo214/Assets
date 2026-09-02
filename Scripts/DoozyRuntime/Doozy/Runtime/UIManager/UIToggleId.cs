using System;
using Doozy.Runtime.Common;

namespace Doozy.Runtime.UIManager;

[Serializable]
public class UIToggleId : CategoryNameId
{
	public UIToggleId()
	{
	}

	public UIToggleId(string category, string name, bool custom = false)
		: base(category, name, custom)
	{
	}
}
