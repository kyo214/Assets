using UnityEngine;

namespace Fusion.StatsInternal;

public interface IFusionStatsView
{
	bool isActiveAndEnabled { get; }

	Transform transform { get; }

	void Initialize();

	void CalculateLayout();

	void Refresh();
}
