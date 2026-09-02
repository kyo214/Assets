using System.Collections.Generic;
using UnityEngine;

namespace Fusion;

internal interface ILagCompensationBroadphase
{
	void CopyFrom(ILagCompensationBroadphase other);

	void Traverse(IBoundsTraversalTest hitTest, List<HitboxRoot> candidateRoots, int layerMask);

	void Add(HitboxRoot root);

	bool Remove(HitboxRoot root);

	void Update(HitboxRoot changed, int tick);

	void Optimize();

	void RenderGizmos(Color color);
}
