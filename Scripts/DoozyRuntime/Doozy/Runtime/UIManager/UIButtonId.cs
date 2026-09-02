using System;
using Doozy.Runtime.Common;

namespace Doozy.Runtime.UIManager;

[Serializable]
public class UIButtonId : CategoryNameId
{
	public UIButtonId()
	{
	}

	public UIButtonId(string category, string name, bool custom = false)
		: base(category, name, custom)
	{
	}
}
