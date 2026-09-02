using UnityEngine;
using UnityEngine.UI;

namespace MoreMountains.Tools;

[AddComponentMenu("More Mountains/Tools/GUI/MMRaycastTarget")]
public class MMRaycastTarget : Graphic
{
	public override void SetVerticesDirty()
	{
	}

	public override void SetMaterialDirty()
	{
	}

	protected override void OnPopulateMesh(VertexHelper vh)
	{
		vh.Clear();
	}
}
