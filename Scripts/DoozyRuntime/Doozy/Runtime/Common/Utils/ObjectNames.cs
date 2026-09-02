using System.Text.RegularExpressions;
using Doozy.Runtime.Common.Extensions;

namespace Doozy.Runtime.Common.Utils;

public static class ObjectNames
{
	public static string NicifyVariableName(string name)
	{
		if (name[0] == 'k')
		{
			name = name.Right(name.Length - 1);
		}
		name = name.Replace("m_", "").Replace("_", " ");
		name = Regex.Replace(name, "[A-Z]", " $0");
		name = name.TrimStart().TrimEnd();
		return name;
	}
}
