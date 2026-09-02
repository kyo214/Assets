using System.Collections.Generic;

namespace NPOI.POIFS.Properties;

public interface Parent : Child
{
	IEnumerator<Property> Children { get; }

	new Child PreviousChild { get; set; }

	new Child NextChild { get; set; }

	void AddChild(Property property);
}
