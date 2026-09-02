namespace BansheeGz.BGDatabase;

public interface BGCodedFieldDelegateLifeCycleI
{
	void OnLoad(BGCodedFieldDelegateLifeCycleContext context);

	void OnUnload(BGCodedFieldDelegateLifeCycleContext context);
}
