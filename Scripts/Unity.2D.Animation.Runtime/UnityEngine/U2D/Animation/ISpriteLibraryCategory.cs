using System.Collections.Generic;

namespace UnityEngine.U2D.Animation;

public interface ISpriteLibraryCategory
{
	string name { get; }

	IEnumerable<ISpriteLibraryLabel> labels { get; }
}
