namespace Doozy.Runtime.Reactor.Ticker;

public interface IUseTickService
{
	void RegisterToTickService();

	void UnregisterFromTickService();

	void Tick();
}
