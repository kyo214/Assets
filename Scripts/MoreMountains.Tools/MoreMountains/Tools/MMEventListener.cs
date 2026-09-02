namespace MoreMountains.Tools;

public interface MMEventListener<T> : MMEventListenerBase
{
	void OnMMEvent(T eventType);
}
