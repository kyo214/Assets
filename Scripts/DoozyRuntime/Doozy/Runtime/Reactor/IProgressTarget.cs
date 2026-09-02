namespace Doozy.Runtime.Reactor;

public interface IProgressTarget
{
	void OnProgressUpdate();

	void SetProgressAt(float progress);
}
