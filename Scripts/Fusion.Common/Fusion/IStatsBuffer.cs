namespace Fusion;

public interface IStatsBuffer
{
	int Count { get; }

	int Capacity { get; }

	FusionGraphVisualization DefaultVisualization { get; }

	FusionGraphVisualization VisualizationFlags { get; }

	ISampleData GetSampleAtIndex(int index);
}
