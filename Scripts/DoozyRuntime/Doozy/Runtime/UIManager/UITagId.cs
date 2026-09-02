using System;
using Doozy.Runtime.Common;

namespace Doozy.Runtime.UIManager;

[Serializable]
public class UITagId : CategoryNameId
{
	public UITagId()
	{
	}

	public UITagId(string categoryName, string name, bool custom = false)
		: base(categoryName, name, custom)
	{
	}
}
