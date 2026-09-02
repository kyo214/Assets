namespace Fusion;

public interface IPredictedSpawnBehaviour
{
	void PredictedSpawnSpawned();

	void PredictedSpawnUpdate();

	void PredictedSpawnRender();

	void PredictedSpawnFailed();

	void PredictedSpawnSuccess();
}
